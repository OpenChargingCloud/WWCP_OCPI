/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using social.OpenData.UsersAPI;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPIv2_2.HTTP;
using cloud.charging.open.protocols.OCPIv2_2.CPO.HTTP;
using cloud.charging.open.protocols.OCPIv2_2.EMSP.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.WebAPI
{

    /// <summary>
    /// OCPI WebAPI extention methods.
    /// </summary>
    public static class ExtensionMethods
    {

        #region ParseRoamingNetwork(this HTTPRequest, HTTPServer, out RoamingNetwork, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the roaming network
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="RoamingNetwork">The roaming network.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when roaming network was found; false else.</returns>
        public static Boolean ParseRoamingNetwork(this HTTPRequest                             HTTPRequest,
                                                  HTTPServer<RoamingNetworks, RoamingNetwork>  HTTPServer,
                                                  out RoamingNetwork                           RoamingNetwork,
                                                  out HTTPResponse                             HTTPResponse)
        {

            if (HTTPServer == null)
                Console.WriteLine("HTTPServer == null!");

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException("HTTPRequest",  "The given HTTP request must not be null!");

            if (HTTPServer == null)
                throw new ArgumentNullException("HTTPServer",   "The given HTTP server must not be null!");

            #endregion

            RoamingNetwork_Id RoamingNetworkId;
                              RoamingNetwork    = null;
                              HTTPResponse      = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                };

                return false;

            }

            if (!RoamingNetwork_Id.TryParse(HTTPRequest.ParsedURLParameters[0], out RoamingNetworkId))
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid RoamingNetworkId!"" }".ToUTF8Bytes()
                };

                return false;

            }

            RoamingNetwork  = HTTPServer.
                                  GetAllTenants(HTTPRequest.Host).
                                  FirstOrDefault(roamingnetwork => roamingnetwork.Id == RoamingNetworkId);

            if (RoamingNetwork == null) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown RoamingNetworkId!"" }".ToUTF8Bytes()
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseRemotePartyId(this HTTPRequest, OCPIWebAPI, out RemotePartyId,                  out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the defibrillator identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPIWebAPI">The OCPI WebAPI.</param>
        /// <param name="RemotePartyId">The parsed unique defibrillator identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when defibrillator identification was found; false else.</returns>
        public static Boolean ParseRemotePartyId(this HTTPRequest           HTTPRequest,
                                                 OCPIWebAPI                 OCPIWebAPI,
                                                 out RemoteParty_Id?        RemotePartyId,
                                                 out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPIWebAPI  is null)
                throw new ArgumentNullException(nameof(OCPIWebAPI),   "The given OCPI WebAPI must not be null!");

            #endregion

            RemotePartyId  = null;
            HTTPResponse   = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPIWebAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            RemotePartyId = RemoteParty_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!RemotePartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPIWebAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid remote party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseRemoteParty  (this HTTPRequest, OCPIWebAPI, out RemotePartyId, out RemoteParty, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the defibrillator identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPIWebAPI">The OCPI WebAPI.</param>
        /// <param name="RemotePartyId">The parsed unique defibrillator identification.</param>
        /// <param name="RemoteParty">The resolved defibrillator.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when defibrillator identification was found; false else.</returns>
        public static Boolean ParseRemoteParty(this HTTPRequest           HTTPRequest,
                                               OCPIWebAPI                 OCPIWebAPI,
                                               out RemoteParty_Id?        RemotePartyId,
                                               out RemoteParty?           RemoteParty,
                                               out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (OCPIWebAPI  is null)
                throw new ArgumentNullException(nameof(OCPIWebAPI),   "The given OCPI WebAPI must not be null!");

            #endregion

            RemotePartyId  = null;
            RemoteParty    = null;
            HTTPResponse   = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPIWebAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            RemotePartyId = RemoteParty_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!RemotePartyId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPIWebAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid remote party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!OCPIWebAPI.CommonAPI.TryGetRemoteParty(RemotePartyId.Value, out RemoteParty)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = OCPIWebAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown remote party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// A HTTP API providing advanced OCPI data structures.
    /// </summary>
    public class OCPIWebAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP URI prefix.
        /// </summary>
        public new static readonly HTTPPath                         DefaultURLPathPrefix        = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public const String DefaultHTTPRealm = "Open Charging Cloud OCPIPlus WebAPI";

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public new const       String                               HTTPRoot                    = "cloud.charging.open.protocols.OCPIv2_2.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPI+ XML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPIPlusJSONContentType     = new HTTPContentType("application", "vnd.OCPIPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPI+ HTML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPIPlusHTMLContentType     = new HTTPContentType("application", "vnd.OCPIPlus+html", "utf-8", null, null);


        public static readonly HTTPEventSource_Id                   DebugLogId                  = HTTPEventSource_Id.Parse("OCPIDebugLog");

        #endregion

        #region Special HTTP methods

        /// <summary>
        /// HTTP method for creating a charging reservation.
        /// </summary>
        public static readonly HTTPMethod HTTP_ReserveNow           = new HTTPMethod("ReserveNow");

        /// <summary>
        /// HTTP method for canceling a charging reservation.
        /// </summary>
        public static readonly HTTPMethod HTTP_CancelReservation    = new HTTPMethod("CancelReservation");

        /// <summary>
        /// HTTP method for starting a charging reservation.
        /// </summary>
        public static readonly HTTPMethod HTTP_StartSession         = new HTTPMethod("StartSession");

        /// <summary>
        /// HTTP method for stopping a charging reservation.
        /// </summary>
        public static readonly HTTPMethod HTTP_StopSession          = new HTTPMethod("StopSession");

        /// <summary>
        /// HTTP method for unlocking a charging connector.
        /// </summary>
        public static readonly HTTPMethod HTTP_UnlockConnector      = new HTTPMethod("UnlockConnector");

        #endregion

        #region Properties

        /// <summary>
        /// The HTTP URI prefix.
        /// </summary>
        public HTTPPath?                                    URLPathPrefix1     { get; }

        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String                                       HTTPRealm          { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>    HTTPLogins         { get; }


        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>                     DebugLog           { get; }


        public CommonAPI                                    CommonAPI          { get; }

        public CommonAPILogger                              CommonAPILogger    { get; set; }


        public CPOAPI                                       CPOAPI             { get; set; }

        public CPOAPILogger                                 CPOAPILogger       { get; set; }


        public EMSPAPI                                      EMSPAPI            { get; set; }

        public EMSPAPILogger                                EMSPAPILogger      { get; set; }


        private readonly List<CPOClient>  cpoClients;

        public IEnumerable<CPOClient>                       CPOClients
            => cpoClients;


        private readonly List<EMSPClient> emspClients;
        public IEnumerable<EMSPClient>                      EMSPClients
            => emspClients;

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer         { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<AccessInfoStatus>?  CustomAccessInfoStatusSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer    { get; set; }

        #endregion

        #region Events

        #region Generic HTTP server logging

        ///// <summary>
        ///// An event called whenever a HTTP request came in.
        ///// </summary>
        //public HTTPRequestLogEvent   RequestLog    = new HTTPRequestLogEvent();

        ///// <summary>
        ///// An event called whenever a HTTP request could successfully be processed.
        ///// </summary>
        //public HTTPResponseLogEvent  ResponseLog   = new HTTPResponseLogEvent();

        ///// <summary>
        ///// An event called whenever a HTTP request resulted in an error.
        ///// </summary>
        //public HTTPErrorLogEvent     ErrorLog      = new HTTPErrorLogEvent();

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Attach the OCPI+ WebAPI to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="URLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public OCPIWebAPI(HTTPServer                                  HTTPServer,

                          CommonAPI                                   CommonAPI,

                          HTTPPath?                                   URLPathPrefix1   = null,
                          HTTPPath?                                   URLPathPrefix    = null,
                          HTTPPath?                                   BasePath         = null,
                          String                                      HTTPRealm        = DefaultHTTPRealm,
                          IEnumerable<KeyValuePair<String, String>>?  HTTPLogins       = null,
                          String?                                     HTMLTemplate     = null)

            : base(HTTPServer,
                   null,
                   null, // ExternalDNSName,
                   null, // HTTPServiceName,
                   BasePath,

                   URLPathPrefix ?? DefaultURLPathPrefix,
                   null, // HTMLTemplate,
                   null, // APIVersionHashes,

                   null, // DisableMaintenanceTasks,
                   null, // MaintenanceInitialDelay,
                   null, // MaintenanceEvery,

                   null, // DisableWardenTasks,
                   null, // WardenInitialDelay,
                   null, // WardenCheckEvery,

                   null, // IsDevelopment,
                   null, // DevelopmentServers,
                   null, // DisableLogging,
                   null, // LoggingPath,
                   null, // LogfileName,
                   null, // LogfileCreator,
                   false)// Autostart

        {

            this.CommonAPI           = CommonAPI;

            this.URLPathPrefix1      = URLPathPrefix1;
            this.HTTPRealm           = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins          = HTTPLogins    ?? Array.Empty<KeyValuePair<String, String>>();

            this.cpoClients          = new List<CPOClient>();
            this.emspClients         = new List<EMSPClient>();

            // Link HTTP events...
            HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix        = "HTTPSSEs" + Path.DirectorySeparatorChar;

            //this.DebugLog            = HTTPServer.AddJSONEventSource(EventIdentification:      DebugLogId,
            //                                                         URLTemplate:              this.URLPathPrefix + "/DebugLog",
            //                                                         MaxNumberOfCachedEvents:  10000,
            //                                                         RetryIntervall:           TimeSpan.FromSeconds(5),
            //                                                         EnableLogging:            true,
            //                                                         LogfilePrefix:            LogfilePrefix);

            RegisterURITemplates();

            this.HTMLTemplate = HTMLTemplate ?? GetResourceString("template.html");

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                                       new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                       new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                                new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly));

        #endregion

        #endregion

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            HTTPServer.RegisterResourcesFolder(this,
                                               HTTPHostname.Any,
                                               URLPathPrefix,
                                               "cloud.charging.open.protocols.OCPIv2_2.WebAPI.HTTPRoot",
                                               DefaultFilename: "index.html");

            if (URLPathPrefix1.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             URLPathPrefix1.Value,
                                             HTTPContentType.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "OPTIONS, GET",
                                                         AccessControlAllowHeaders  = "Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = ("<html><body>" +
                                                                                          "This is an Open Charge Point Interface HTTP service!<br /><br />" +
                                                                                          "<ul>" +
                                                                                              "<li><a href=\"versions\">Versions</a></li>" +
                                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/remoteParties\">Remote Parties</a></li>" +
                                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/clients\">Clients</a></li>" +
                                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/cpoclients\">CPO Clients</a></li>" +
                                                                                              "<li><a href=\"" + URLPathPrefix.ToString() + "/emspclients\">EMSP Clients</a></li>" +
                                                                                       "</ul><body></html>").ToUTF8Bytes(),
                                                         Connection                 = "close"
                                                     }.AsImmutable);

                                             });

            if (URLPathPrefix1.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             URLPathPrefix1.Value + "versions",
                                             HTTPContentType.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "OPTIONS, GET",
                                                         AccessControlAllowHeaders  = "Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("versions.versions.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });


            if (URLPathPrefix1.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             URLPathPrefix1.Value + "versions/{id}",
                                             HTTPContentType.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "OPTIONS, GET",
                                                         AccessControlAllowHeaders  = "Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("versions.versionDetails.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });


            #endregion


            #region ~/remoteParties

            #region OPTIONS            ~/remoteParties

            // --------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteParties
            // --------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "remoteParties",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "OPTIONS, GET, ReserveNow, CancelReservation, StartSession, StopSession, UnlockConnector",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST,
                                                                                      HTTP_ReserveNow,
                                                                                      HTTP_CancelReservation,
                                                                                      HTTP_StartSession,
                                                                                      HTTP_StopSession,
                                                                                      HTTP_UnlockConnector
                                                                                  },
                                                     AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteParties

            #region JSON

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/remoteParties
            // ---------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion


                                             var withMetadata                 = Request.QueryString.GetBoolean("withMetadata", false);
                                             var matchFilter                  = Request.QueryString.CreateStringFilter<RemoteParty>("match",
                                                                                                                                    (remoteParty, pattern) => remoteParty.Id.         ToString().Contains(pattern) ||
                                                                                                                                                              remoteParty.BusinessDetails?.Name. Contains(pattern) == true);
                                             var skip                         = Request.QueryString.GetUInt64 ("skip");
                                             var take                         = Request.QueryString.GetUInt64 ("take");
                                             var matchStatusFilter            = Request.QueryString.CreateMultiEnumFilter<PartyStatus>("matchStatus");
                                             var includeCryptoHash            = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             var allRemoteParties             = CommonAPI.RemoteParties.
                                                                                    //Where(remoteParty => HTTPOrganizations.Contains(remoteParty.Owner) ||
                                                                                    //                           Admins.InEdges(HTTPUser).
                                                                                    //                                  Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)).
                                                                                    ToArray();
                                             var totalCount                   = allRemoteParties.ULongCount();

                                             var filteredRemoteParties        = allRemoteParties.
                                                                                    Where(matchFilter).
                                                                                    Where(remoteParty => matchStatusFilter(remoteParty.Status)).
                                                                                    ToArray();
                                             var filteredCount                = filteredRemoteParties.ULongCount();

                                             var JSONResults                  = filteredRemoteParties.
                                                                                    OrderBy(remoteParty => remoteParty.Id).
                                                                                    ToJSON (skip,
                                                                                            take,
                                                                                            false, //Embedded
                                                                                            null,
                                                                                            null,
                                                                                            null);  //GetRemotePartySerializator(Request, HTTPUser),


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.OK,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = Timestamp.Now,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET, OPTIONS",
                                                     AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                     //ETag                          = "1",
                                                     ContentType                   = HTTPContentType.JSON_UTF8,
                                                     Content                       = withMetadata
                                                                                         ? JSONObject.Create(
                                                                                               new JProperty("totalCount",     totalCount),
                                                                                               new JProperty("filteredCount",  filteredCount),
                                                                                               new JProperty("searchResults",  JSONResults)
                                                                                           ).ToUTF8Bytes()
                                                                                         : JSONResults.ToUTF8Bytes(),
                                                     X_ExpectedTotalNumberOfItems  = filteredCount,
                                                     Connection                    = "close",
                                                     Vary                          = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region HTML

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/remoteParties
            // ---------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET",
                                                         AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteParties.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                         });

            #endregion

            #endregion


            #region OPTIONS            ~/remoteParties/{remotePartyId}

            // -------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteParties/DE-GDF-CPO
            // -------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "remoteParties/{remotePartyId}",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, OPTIONS",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteParties/{remotePartyId}

            #region JSON

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion


                                             var includeCryptoHash  = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             return Task.FromResult(
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                             //if (HTTPOrganizations.Contains(Defibrillator.Owner) ||
                                             //    Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin))
                                             //{

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET",
                                                         AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteParty.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = "GET",
                                             //               AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                             //               Connection                 = "close",
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion


            #region OPTIONS            ~/remoteParties/{remotePartyId}/reserveNow

            // ------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/reserveNow
            // ------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "/{remotePartyId}/reserveNow",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, OPTIONS",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteParties/{remotePartyId}/reserveNow

            #region JSON

            // ---------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO/reserveNow
            // ---------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/reserveNow",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion


                                             var includeCryptoHash  = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             return Task.FromResult(
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/reserveNow
            // --------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/reserveNow",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                             //if (HTTPOrganizations.Contains(Defibrillator.Owner) ||
                                             //    Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin))
                                             //{

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET",
                                                         AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.reserveNow.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = "GET",
                                             //               AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                             //               Connection                 = "close",
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region ReserveNow         ~/remoteParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_ReserveNow,
                                         URLPathPrefix + "remoteParties/{remotePartyId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return HTTPResponse.AsImmutable;
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Response.AsImmutable;
                                             //}

                                             #endregion


                                             #region Parse JSON, Token, ExpirationTimestamp, ...

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse Token                     [mandatory]

                                             if (!JSON.ParseMandatoryJSON("token",
                                                                          "token",
                                                                          OCPIv2_2.Token.TryParse,
                                                                          out Token Token,
                                                                          out var ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse ExpirationTimestamp       [mandatory]

                                             if (!JSON.ParseMandatory("expirationTimestamp",
                                                                      "expiration timestamp",
                                                                      out DateTime ExpirationTimestamp,
                                                                      out ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse ReservationId             [mandatory]

                                             if (!JSON.ParseMandatory("reservationId",
                                                                      "reservation identification",
                                                                      Reservation_Id.TryParse,
                                                                      out Reservation_Id ReservationId,
                                                                      out ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse LocationId                [mandatory]

                                             if (!JSON.ParseMandatory("locationId",
                                                                      "location identification",
                                                                      Location_Id.TryParse,
                                                                      out Location_Id LocationId,
                                                                      out ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse EVSEUId                   [optional]

                                             if (JSON.ParseOptional("evseUId",
                                                                    "EVSE unique identification",
                                                                    EVSE_UId.TryParse,
                                                                    out EVSE_UId? EVSEUId,
                                                                    out           ErrorResponse))
                                             {

                                                 if (ErrorResponse != null)
                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = Timestamp.Now,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = I18NString.Create(Languages.en,
                                                                                                               ErrorResponse).
                                                                                                        ToJSON().
                                                                                                        ToUTF8Bytes(),
                                                                Connection                 = "close"
                                                            }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse AuthorizationReference    [optional]

                                             if (JSON.ParseOptional("authorizationReference",
                                                                    "authorization reference",
                                                                    OCPIv2_2.AuthorizationReference.TryParse,
                                                                    out AuthorizationReference? AuthorizationReference,
                                                                    out                         ErrorResponse))
                                             {

                                                 if (ErrorResponse != null)
                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = Timestamp.Now,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = I18NString.Create(Languages.en,
                                                                                                               ErrorResponse).
                                                                                                        ToJSON().
                                                                                                        ToUTF8Bytes(),
                                                                Connection                 = "close"
                                                            }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = GetEMSPClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a apropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                      }.AsImmutable;

                                             #endregion


                                             var reserveNowResult = await emspClient.ReserveNow(Token,
                                                                                                ExpirationTimestamp,
                                                                                                ReservationId,
                                                                                                LocationId,
                                                                                                EVSEUId,
                                                                                                AuthorizationReference);

                                             return 
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion


            #region OPTIONS            ~/remoteParties/{remotePartyId}/cancelReservation

            // -------------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/cancelReservation
            // -------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "/{remotePartyId}/cancelReservation",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, OPTIONS",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteParties/{remotePartyId}/cancelReservation

            #region JSON

            // --------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO/cancelReservation
            // --------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/cancelReservation",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion


                                             var includeCryptoHash  = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             return Task.FromResult(
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // -------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/cancelReservation
            // -------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/cancelReservation",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                             //if (HTTPOrganizations.Contains(Defibrillator.Owner) ||
                                             //    Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin))
                                             //{

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET",
                                                         AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.cancelReservation.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = "GET",
                                             //               AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                             //               Connection                 = "close",
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region CancelReservation  ~/remoteParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_CancelReservation,
                                         URLPathPrefix + "remoteParties/{remotePartyId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return HTTPResponse.AsImmutable;
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Response.AsImmutable;
                                             //}

                                             #endregion


                                             #region Parse JSON and ReservationId

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse ReservationId    [mandatory]

                                             if (!JSON.ParseMandatory("reservationId",
                                                                      "reservation identification",
                                                                      Reservation_Id.TryParse,
                                                                      out Reservation_Id ReservationId,
                                                                      out var ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = GetEMSPClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a apropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                      }.AsImmutable;

                                             #endregion


                                             var cancelReservationResult = await emspClient.CancelReservation(ReservationId);

                                             return 
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion


            #region OPTIONS            ~/remoteParties/{remotePartyId}/startSession

            // --------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/startSession
            // --------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "/{remotePartyId}/startSession",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, OPTIONS",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteParties/{remotePartyId}/startSession

            #region JSON

            // ---------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO/startSession
            // ---------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/startSession",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion


                                             var includeCryptoHash  = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             return Task.FromResult(
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/startSession
            // --------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/startSession",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                             //if (HTTPOrganizations.Contains(Defibrillator.Owner) ||
                                             //    Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin))
                                             //{

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET",
                                                         AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.startSession.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = "GET",
                                             //               AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                             //               Connection                 = "close",
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region StartSession       ~/remoteParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_StartSession,
                                         URLPathPrefix + "remoteParties/{remotePartyId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return HTTPResponse.AsImmutable;
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Response.AsImmutable;
                                             //}

                                             #endregion


                                             #region Parse JSON and SessionId

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse Token                     [mandatory]

                                             if (!JSON.ParseMandatoryJSON("token",
                                                                          "token",
                                                                          OCPIv2_2.Token.TryParse,
                                                                          out Token? Token,
                                                                          out var ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse!).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse LocationId                [mandatory]

                                             if (!JSON.ParseMandatory("locationId",
                                                                      "location identification",
                                                                      Location_Id.TryParse,
                                                                      out Location_Id LocationId,
                                                                      out ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse EVSEUId                   [optional]

                                             if (JSON.ParseOptional("evseUId",
                                                                    "EVSE unique identification",
                                                                    EVSE_UId.TryParse,
                                                                    out EVSE_UId? EVSEUId,
                                                                    out ErrorResponse))
                                             {

                                                 if (ErrorResponse is not null)
                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = Timestamp.Now,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = I18NString.Create(Languages.en,
                                                                                                               ErrorResponse).
                                                                                                        ToJSON().
                                                                                                        ToUTF8Bytes(),
                                                                Connection                 = "close"
                                                            }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse ConnectorId               [optional]

                                             if (JSON.ParseOptional("connectorId",
                                                                    "connector identification",
                                                                    Connector_Id.TryParse,
                                                                    out Connector_Id? ConnectorId,
                                                                    out ErrorResponse))
                                             {

                                                 if (ErrorResponse is not null)
                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = Timestamp.Now,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = I18NString.Create(Languages.en,
                                                                                                               ErrorResponse).
                                                                                                        ToJSON().
                                                                                                        ToUTF8Bytes(),
                                                                Connection                 = "close"
                                                            }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse AuthorizationReference    [optional]

                                             if (JSON.ParseOptional("authorizationReference",
                                                                    "authorization reference",
                                                                    OCPIv2_2.AuthorizationReference.TryParse,
                                                                    out AuthorizationReference? AuthorizationReference,
                                                                    out                         ErrorResponse))
                                             {

                                                 if (ErrorResponse is not null)
                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = Timestamp.Now,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = I18NString.Create(Languages.en,
                                                                                                               ErrorResponse).
                                                                                                        ToJSON().
                                                                                                        ToUTF8Bytes(),
                                                                Connection                 = "close"
                                                            }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = GetEMSPClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a apropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                      }.AsImmutable;

                                             #endregion


                                             var startSessionResult = await emspClient.StartSession(Token,
                                                                                                    LocationId,
                                                                                                    EVSEUId,
                                                                                                    ConnectorId,
                                                                                                    AuthorizationReference);

                                             return 
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion


            #region OPTIONS            ~/remoteParties/{remotePartyId}/stopSession

            // -------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/stopSession
            // -------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "/{remotePartyId}/stopSession",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, OPTIONS",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteParties/{remotePartyId}/stopSession

            #region JSON

            // --------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO/stopSession
            // --------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/stopSession",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion


                                             var includeCryptoHash  = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             return Task.FromResult(
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // -------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/stopSession
            // -------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/stopSession",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                             //if (HTTPOrganizations.Contains(Defibrillator.Owner) ||
                                             //    Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin))
                                             //{

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET",
                                                         AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.stopSession.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = "GET",
                                             //               AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                             //               Connection                 = "close",
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region StopSession        ~/remoteParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_StopSession,
                                         URLPathPrefix + "remoteParties/{remotePartyId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return HTTPResponse.AsImmutable;
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Response.AsImmutable;
                                             //}

                                             #endregion


                                             #region Parse JSON and SessionId

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse SessionId    [mandatory]

                                             if (!JSON.ParseMandatory("sessionId",
                                                                      "session identification",
                                                                      Session_Id.TryParse,
                                                                      out Session_Id  SessionId,
                                                                      out String      ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = GetEMSPClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a apropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                      }.AsImmutable;

                                             #endregion


                                             var stopSessionResult = await emspClient.StopSession(SessionId);

                                             return 
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               stopSessionResult.ToJSON(commandResponse => commandResponse.ToJSON()).
                                                                                                                 ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion


            #region OPTIONS            ~/remoteParties/{remotePartyId}/unlockConnector

            // -----------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/unlockConnector
            // -----------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "/{remotePartyId}/unlockConnector",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, OPTIONS",
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteParties/{remotePartyId}/unlockConnector

            #region JSON

            // ------------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO/unlockConnector
            // ------------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/unlockConnector",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion


                                             var includeCryptoHash  = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             return Task.FromResult(
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomAccessInfoStatusSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // -------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteParties/DE-GDF-CPO/unlockConnector
            // -------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteParties/{remotePartyId}/unlockConnector",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             //// Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                             //if (HTTPOrganizations.Contains(Defibrillator.Owner) ||
                                             //    Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin))
                                             //{

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET",
                                                         AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.unlockConnector.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = "GET",
                                             //               AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                             //               Connection                 = "close",
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region UnlockConnector    ~/remoteParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_UnlockConnector,
                                         URLPathPrefix + "remoteParties/{remotePartyId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?       RemotePartyId,
                                                                           out RemoteParty           RemoteParty,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return HTTPResponse.AsImmutable;
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out User                   HTTPUser,
                                             //                    out HashSet<Organization>  HTTPOrganizations,
                                             //                    out HTTPResponse.Builder   Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Response.AsImmutable;
                                             //}

                                             #endregion


                                             #region Parse JSON and LocationId, EVSEUId, ConnectorId

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse LocationId    [mandatory]

                                             if (!JSON.ParseMandatory("locationId",
                                                                      "location identification",
                                                                      Location_Id.TryParse,
                                                                      out Location_Id LocationId,
                                                                      out var ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse EVSEUId       [mandatory]

                                             if (!JSON.ParseMandatory("evseUId",
                                                                      "EVSE unique identification",
                                                                      EVSE_UId.TryParse,
                                                                      out EVSE_UId EVSEUId,
                                                                      out ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse ConnectorId   [mandatory]

                                             if (!JSON.ParseMandatory("connectorId",
                                                                      "connector identification",
                                                                      Connector_Id.TryParse,
                                                                      out Connector_Id ConnectorId,
                                                                      out ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = GetEMSPClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a apropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                      }.AsImmutable;

                                             #endregion


                                             var unlockConnectorResult = await emspClient.UnlockConnector(LocationId,
                                                                                                          EVSEUId,
                                                                                                          ConnectorId);

                                             return 
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               unlockConnectorResult.ToJSON(commandResponse => commandResponse.ToJSON()).
                                                                                                                     ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion


            #region GET      ~/clients

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "clients",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var clients = new List<CommonClient>();
                                             clients.AddRange(CPOClients);
                                             clients.AddRange(EMSPClients);

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = new JArray(clients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     AccessControlAllowHeaders  = "Authorization"
                                                     //LastModified               = Location.LastUpdated.ToIso8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET      ~/cpoclients

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "cpoclients",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request)
                                                 {
                                                     HTTPStatusCode = HTTPStatusCode.OK,
                                                     ContentType = HTTPContentType.JSON_UTF8,
                                                     Content = new JArray(CPOClients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods = "OPTIONS, GET",
                                                     AccessControlAllowHeaders = "Authorization"
                                                     //LastModified               = Location.LastUpdated.ToIso8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET      ~/emspclients

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "emspclients",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = new JArray(EMSPClients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods  = "OPTIONS, GET",
                                                     AccessControlAllowHeaders  = "Authorization"
                                                     //LastModified               = Location.LastUpdated.ToIso8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion


        }

        #endregion


        #region GetEMSPClient(CountryCode, PartyId, Role = Roles.CPO, AccessTokenBase64Encoding = true)

        public EMSPClient? GetEMSPClient(CountryCode  CountryCode,
                                         Party_Id     PartyId,
                                         Roles        Role                        = Roles.CPO,
                                         Boolean      AccessTokenBase64Encoding   = true)
        {

            var remoteParty = CommonAPI.RemoteParties.FirstOrDefault(remoteparty => remoteparty.CountryCode == CountryCode &&
                                                                                    remoteparty.PartyId     == PartyId     &&
                                                                                    remoteparty.Role        == Role);

            if (remoteParty?.RemoteAccessInfos?.Any() == true)
                return emspClients.AddAndReturnElement(
                    new EMSPClient(
                        remoteParty.RemoteAccessInfos.First().VersionsURL,
                        remoteParty.RemoteAccessInfos.First().AccessToken,
                        CommonAPI,
                        RemoteCertificateValidator: (sender, certificate, chain, sslPolicyErrors) => true,
                        AccessTokenBase64Encoding:   AccessTokenBase64Encoding
                    ));

            return null;

        }

        #endregion

        #region GetEMSPClient(RemoteParty,   AccessTokenBase64Encoding = true)

        public EMSPClient? GetEMSPClient(RemoteParty  RemoteParty,
                                         Boolean      AccessTokenBase64Encoding = true)
        {

            if (RemoteParty is null)
                return null;

            var emspClient = EMSPClients.FirstOrDefault(emspclient => emspclient.RemoteVersionsURL == RemoteParty.RemoteAccessInfos.First().VersionsURL &&
                                                                      emspclient.AccessToken       == RemoteParty.RemoteAccessInfos.First().AccessToken);

            if (emspClient is not null)
                return emspClient;

            if (RemoteParty.RemoteAccessInfos?.Any() == true)
                return emspClients.AddAndReturnElement(
                    new EMSPClient(RemoteParty.RemoteAccessInfos.First().VersionsURL,
                                   RemoteParty.RemoteAccessInfos.First().AccessToken,
                                   CommonAPI,
                                   RemoteCertificateValidator: (sender, certificate, chain, sslPolicyErrors) => true,
                                   AccessTokenBase64Encoding:   AccessTokenBase64Encoding));

            return null;

        }

        #endregion

        #region GetEMSPClient(RemotePartyId, AccessTokenBase64Encoding = true)

        public EMSPClient? GetEMSPClient(RemoteParty_Id  RemotePartyId,
                                         Boolean         AccessTokenBase64Encoding = true)
        {

            var remoteParty  = CommonAPI.RemoteParties.FirstOrDefault(remoteparty => remoteparty.CountryCode == RemotePartyId.CountryCode &&
                                                                                     remoteparty.PartyId     == RemotePartyId.PartyId     &&
                                                                                     remoteparty.Role        == RemotePartyId.Role);

            var emspClient   = remoteParty is not null
                                   ? EMSPClients.FirstOrDefault(emspclient => emspclient.RemoteVersionsURL == remoteParty.RemoteAccessInfos.First().VersionsURL &&
                                                                              emspclient.AccessToken       == remoteParty.RemoteAccessInfos.First().AccessToken)
                                   : null;

            if (emspClient is not null)
                return emspClient;

            if (remoteParty?.RemoteAccessInfos?.Any() == true)
                return emspClients.AddAndReturnElement(
                    new EMSPClient(remoteParty.RemoteAccessInfos.First().VersionsURL,
                                   remoteParty.RemoteAccessInfos.First().AccessToken,
                                   CommonAPI,
                                   RemoteCertificateValidator: (sender, certificate, chain, sslPolicyErrors) => true,
                                   AccessTokenBase64Encoding:   AccessTokenBase64Encoding));

            return null;

        }

        #endregion


        #region GetCPOClient (CountryCode, PartyId, Role = Roles.EMSP)

        public CPOClient? GetCPOClient(CountryCode  CountryCode,
                                       Party_Id     PartyId,
                                       Roles        Role                        = Roles.EMSP,
                                       Boolean      AccessTokenBase64Encoding   = true)
        {

            var remoteParty = CommonAPI.RemoteParties.FirstOrDefault(remoteparty => remoteparty.CountryCode == CountryCode &&
                                                                                    remoteparty.PartyId     == PartyId     &&
                                                                                    remoteparty.Role        == Role);

            if (remoteParty?.RemoteAccessInfos?.Any() == true)
                return cpoClients.AddAndReturnElement(
                    new CPOClient(
                        remoteParty.RemoteAccessInfos.First().VersionsURL,
                        remoteParty.RemoteAccessInfos.First().AccessToken,
                        CommonAPI,
                        RemoteCertificateValidator: (sender, certificate, chain, sslPolicyErrors) => true,
                        AccessTokenBase64Encoding:   AccessTokenBase64Encoding
                    ));

            return null;

        }

        #endregion



        //public void Add(WWCPCPOAdapter CPOAdapter)
        //{

        //    _CPOAdapters.Add(CPOAdapter);

        //}


        //public void Add(WWCPEMPAdapter EMPAdapter)
        //{

        //    _EMPAdapters.Add(EMPAdapter);

        //}

    }

}
