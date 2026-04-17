/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI WebAPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
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

using System.Reflection;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPI.WebAPI
{

    /// <summary>
    /// OCPI WebAPI extension methods.
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
        public static Boolean ParseRoamingNetwork(this HTTPRequest     HTTPRequest,
                                                  HTTPTestServerX      HTTPServer,
                                                  out RoamingNetwork?  RoamingNetwork,
                                                  out HTTPResponse?    HTTPResponse)
        {

            RoamingNetwork_Id RoamingNetworkId;
                              RoamingNetwork    = null;
                              HTTPResponse      = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                                   HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                   Server          = HTTPServer.HTTPServerName,
                                   Date            = Timestamp.Now,
                               };

                return false;

            }

            if (!RoamingNetwork_Id.TryParse(HTTPRequest.ParsedURLParameters[0], out RoamingNetworkId))
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                                   HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                   Server          = HTTPServer.HTTPServerName,
                                   Date            = Timestamp.Now,
                                   ContentType     = HTTPContentType.Application.JSON_UTF8,
                                   Content         = @"{ ""description"": ""Invalid RoamingNetworkId!"" }".ToUTF8Bytes()
                               };

                return false;

            }

            //RoamingNetwork  = HTTPServer.
            //                      GetAllTenants(HTTPRequest.Host).
            //                      FirstOrDefault(roamingnetwork => roamingnetwork.Id == RoamingNetworkId);

            if (RoamingNetwork is null) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                                   HTTPStatusCode  = HTTPStatusCode.NotFound,
                                   Server          = HTTPServer.HTTPServerName,
                                   Date            = Timestamp.Now,
                                   ContentType     = HTTPContentType.Application.JSON_UTF8,
                                   Content         = @"{ ""description"": ""Unknown RoamingNetworkId!"" }".ToUTF8Bytes()
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
    public class CommonWebAPI : AHTTPExtAPIXExtension2<CommonHTTPAPI, HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP URL prefix.
        /// </summary>
        public     static readonly  HTTPPath            DefaultURLPathPrefix      = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const            String              DefaultHTTPServerName     = "Open Charging Cloud OCPI WebAPI";

        /// <summary>
        /// The default HTTP service name.
        /// </summary>
        public new const            String              DefaultHTTPServiceName    = "Open Charging Cloud OCPI WebAPI";

        /// <summary>
        /// The HTTP root for embedded resources.
        /// </summary>
        public     const            String              HTTPRoot                  = "cloud.charging.open.protocols.OCPI.WebAPI.HTTPRoot.";

        ///// <summary>
        ///// The default HTTP realm, if HTTP Basic Authentication is used.
        ///// </summary>
        //public     const            String              DefaultHTTPRealm          = "Open Charging Cloud OCPI WebAPI";


        //ToDo: http://www.iana.org/form/media-types

        ///// <summary>
        ///// The HTTP content type for serving OCPI+ XML data.
        ///// </summary>
        //public static readonly HTTPContentType                      OCPIPlusJSONContentType     = new ("application", "vnd.OCPIPlus+json", "utf-8", null, null);

        ///// <summary>
        ///// The HTTP content type for serving OCPI+ HTML data.
        ///// </summary>
        //public static readonly HTTPContentType                      OCPIPlusHTMLContentType     = new ("application", "vnd.OCPIPlus+html", "utf-8", null, null);


        public static readonly      HTTPEventSource_Id  DefaultDebugLogId    = HTTPEventSource_Id.Parse($"OCPI_debugLog");

        /// <summary>
        /// The default WebAPI logfile name.
        /// </summary>
        public  const               String              DefaultLogfileName   = "OCPI_WebAPI.log";

        #endregion

        #region Properties

        public CommonHTTPAPI             CommonHTTPAPI
            => HTTPBaseAPI;

        /// <summary>
        /// The HTTP URI prefix.
        /// </summary>
        public HTTPPath?                 OverlayURLPathPrefix    { get; }

        /// <summary>
        /// The HTTP URI prefix.
        /// </summary>
        public HTTPPath?                 APIURLPathPrefix        { get; }


        /// <summary>
        /// Make use of HTTP Server Sent Events for debug information.
        /// </summary>
        public ServiceSettings           UseHTTPSSE              { get; }

        /// <summary>
        /// Debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>  DebugLog                { get; }

        #endregion

        #region Special HTTP methods

        /// <summary>
        /// HTTP method for creating a charging reservation.
        /// </summary>
        public static readonly HTTPMethod HTTP_ReserveNow         = HTTPMethod.TryParse("ReserveNow",        false)!;

        /// <summary>
        /// HTTP method for canceling a charging reservation.
        /// </summary>
        public static readonly HTTPMethod HTTP_CancelReservation  = HTTPMethod.TryParse("CancelReservation", false)!;

        /// <summary>
        /// HTTP method for starting a charging reservation.
        /// </summary>
        public static readonly HTTPMethod HTTP_StartSession       = HTTPMethod.TryParse("StartSession",      false)!;

        /// <summary>
        /// HTTP method for stopping a charging reservation.
        /// </summary>
        public static readonly HTTPMethod HTTP_StopSession        = HTTPMethod.TryParse("StopSession",       false)!;

        /// <summary>
        /// HTTP method for unlocking a charging connector.
        /// </summary>
        public static readonly HTTPMethod HTTP_UnlockConnector    = HTTPMethod.TryParse("UnlockConnector",   false)!;

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<Image>?             CustomImageSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<LocalAccessInfo>?   CustomLocalAccessInfoSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<RemoteAccessInfo>?  CustomRemoteAccessInfoSerializer    { get; set; }

        #endregion

        #region Events


        #endregion

        #region Constructor(s)

        static CommonWebAPI()
        {
            // Using static variables within normal constructors seems to
            // have a problem setting them up to their expected values!
        }

        /// <summary>
        /// Attach the OCPI WebAPI to the given OCPI Common HTTP API.
        /// </summary>
        /// <param name="CommonHTTPAPI">The OCPI Common HTTP API.</param>
        /// 
        /// <param name="OverlayURLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="APIURLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="WebAPIURLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="BasePath">The base path of the HTTP server.</param>
        public CommonWebAPI(CommonHTTPAPI            CommonHTTPAPI,

                            HTTPPath?                OverlayURLPathPrefix   = null,
                            HTTPPath?                APIURLPathPrefix       = null,
                            HTTPPath?                WebAPIURLPathPrefix    = null,
                            HTTPPath?                BasePath               = null,  // For URL prefixes in HTML!

                            I18NString?              Description            = null,

                            ServiceSettings?         UseHTTPSSE             = null,
                            HTTPEventSource_Id?      DebugLogId             = null,

                            String?                  ExternalDNSName        = null,
                            String?                  HTTPServerName         = DefaultHTTPServerName,
                            String?                  HTTPServiceName        = DefaultHTTPServiceName,
                            String?                  APIVersionHash         = null,
                            JObject?                 APIVersionHashes       = null,

                            Boolean?                 IsDevelopment          = null,
                            IEnumerable<String>?     DevelopmentServers     = null,
                            Boolean?                 DisableNotifications   = null,
                            Boolean?                 DisableLogging         = null,
                            String?                  LoggingPath            = null,
                            String?                  LogfileName            = null,
                            LogfileCreatorDelegate?  LogfileCreator         = null)

            : base(CommonHTTPAPI,
                   CommonHTTPAPI.URLPathPrefix + WebAPIURLPathPrefix,
                   CommonHTTPAPI.URLPathPrefix + BasePath,

                   Description     ?? I18NString.Create("OCPI Common Web API"),

                   ExternalDNSName,
                   HTTPServerName  ?? DefaultHTTPServerName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   APIVersionHash,
                   APIVersionHashes,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName     ?? DefaultLogfileName,
                   LogfileCreator)

        {

            this.OverlayURLPathPrefix  = CommonHTTPAPI.URLPathPrefix + OverlayURLPathPrefix;
            this.APIURLPathPrefix      = CommonHTTPAPI.URLPathPrefix + APIURLPathPrefix;

            this.UseHTTPSSE            = UseHTTPSSE ?? ServiceSettings.Disabled;

            this.DebugLog              = HTTPBaseAPI.HTTPBaseAPI.AddJSONEventSource(
                                             EventSourceId:            DebugLogId ?? DefaultDebugLogId,
                                             MaxNumberOfCachedEvents:  1000,
                                             RetryInterval :           TimeSpan.FromSeconds(5),
                                             EnableLogging:            true,
                                             LogfilePrefix:            this.LoggingPath + "HTTPSSEs" + Path.DirectorySeparatorChar
                                         );

            RegisterURLTemplates();

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        private readonly Tuple<String, Assembly>[] resourceAssemblies = [
            new Tuple<String, Assembly>(CommonWebAPI.HTTPRoot, typeof(CommonWebAPI).Assembly),
            new Tuple<String, Assembly>(HTTPAPI.     HTTPRoot, typeof(HTTPAPI).     Assembly)
        ];

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(
                   ResourceName,
                   resourceAssemblies
               );

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(
                   ResourceName,
                   resourceAssemblies
               );

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(
                   ResourceName,
                   resourceAssemblies
               );

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(
                   ResourceName,
                   resourceAssemblies
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(
                   ResourceName,
                   resourceAssemblies
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter)

        protected override String MixWithHTMLTemplate(String ResourceName, Func<String, String> HTMLConverter)

            => MixWithHTMLTemplate(
                   ResourceName,
                   HTMLConverter,
                   resourceAssemblies
               );

        #endregion

        #endregion


        /// <summary>
        /// The following will register HTTP overlays for text/html
        /// showing a html representation of the OCPI Common API!
        /// </summary>
        private void RegisterURLTemplates()
        {

            #region / (HTTPRoot)

            CommonHTTPAPI.HTTPBaseAPI.MapResourceAssemblyFolder(
                HTTPHostname.Any,
                URLPathPrefix,
                HTTPRoot,
                RequireAuthentication:  false,
                DefaultFilename:       "index.html"
            );

            #endregion


            if (OverlayURLPathPrefix.HasValue)
            {

                #region GET ~/

                CommonHTTPAPI.HTTPBaseAPI.AddHandler(

                    HTTPMethod.GET,
                    OverlayURLPathPrefix.Value,
                    HTTPContentType.Text.HTML_UTF8,
                    HTTPDelegate: request =>

                        Task.FromResult(
                            new HTTPResponse.Builder(request) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Server                     = HTTPServiceName,
                                Date                       = Timestamp.Now,
                                AccessControlAllowOrigin   = "*",
                                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                ContentType                = HTTPContentType.Text.HTML_UTF8,
                                Content                    = MixWithHTMLTemplate(
                                                                 "index.shtml",
                                                                 html => html.Replace("{{versionPath}}", "")
                                                             ).ToUTF8Bytes(),
                                Connection                 = ConnectionType.KeepAlive,
                                Vary                       = "Accept"
                            }.AsImmutable),

                    AllowReplacement: URLReplacement.Allow

                );


                //// Just for convenience...
                //if (OverlayURLPathPrefix.Value != HTTPPath.Root)
                //    CommonHTTPAPI.HTTPBaseAPI.AddHandler(
                //        HTTPMethod.GET,
                //        OverlayURLPathPrefix.Value + "/",
                //        HTTPContentType.Text.HTML_UTF8,
                //        HTTPDelegate: request =>

                //            Task.FromResult(
                //                new HTTPResponse.Builder(request) {
                //                    HTTPStatusCode             = HTTPStatusCode.OK,
                //                    Server                     = HTTPServiceName,
                //                    Date                       = Timestamp.Now,
                //                    AccessControlAllowOrigin   = "*",
                //                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                //                    AccessControlAllowHeaders  = [ "Authorization" ],
                //                    ContentType                = HTTPContentType.Text.HTML_UTF8,
                //                    Content                    = MixWithHTMLTemplate(
                //                                                     "index.shtml",
                //                                                     html => html.Replace("{{versionPath}}", "")
                //                                                 ).ToUTF8Bytes(),
                //                    Connection                 = ConnectionType.KeepAlive,
                //                    Vary                       = "Accept"
                //                }.AsImmutable),

                //        AllowReplacement: URLReplacement.Allow

                //    );

                #endregion

                #region GET ~/versions

                CommonHTTPAPI.HTTPBaseAPI.AddHandler(

                    HTTPMethod.GET,
                    OverlayURLPathPrefix.Value + "versions",
                    HTTPContentType.Text.HTML_UTF8,
                    HTTPDelegate: request =>

                        Task.FromResult(
                            new HTTPResponse.Builder(request) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Server                     = HTTPServiceName,
                                Date                       = Timestamp.Now,
                                AccessControlAllowOrigin   = "*",
                                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                ContentType                = HTTPContentType.Text.HTML_UTF8,
                                Content                    = MixWithHTMLTemplate(
                                                                 "versions.versions.shtml",
                                                                 html => html.Replace("{{versionPath}}", "")
                                                             ).ToUTF8Bytes(),
                                Connection                 = ConnectionType.KeepAlive,
                                Vary                       = "Accept"
                            }.AsImmutable)

                );

                #endregion


                #region GET ~/debugLog

                if (UseHTTPSSE != ServiceSettings.Disabled)
                {

                    HTTPBaseAPI.HTTPBaseAPI.MapJSONEventSource(
                        DebugLog,
                        OverlayURLPathPrefix.Value + "debugLog",
                        RequireAuthentication:  UseHTTPSSE == ServiceSettings.RequiresAuthentication
                    );

                    CommonHTTPAPI.HTTPBaseAPI.AddHandler(

                        HTTPMethod.GET,
                        OverlayURLPathPrefix.Value + "debug",
                        HTTPContentType.Text.HTML_UTF8,
                        HTTPDelegate: async request => {

                            #region Check authentication

                            if (request.User == null &&
                                UseHTTPSSE == ServiceSettings.RequiresAuthentication)
                            {

                                //ToDo: Maybe redirect to a login page instead of sending a 401?
                                return new HTTPResponse.Builder(request) {
                                           HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                           Server                     = HTTPServerName,
                                           Date                       = Timestamp.Now,
                                           AccessControlAllowOrigin   = "*",
                                           AccessControlAllowMethods  = [ "GET" ],
                                           AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                           Connection                 = ConnectionType.Close,
                                           Vary                       = "Accept"
                                       }.AsImmutable;

                            }

                            #endregion


                            return new HTTPResponse.Builder(request) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       Server                     = HTTPServerName,
                                       Date                       = Timestamp.Now,
                                       AccessControlAllowOrigin   = "*",
                                       AccessControlAllowMethods  = [ "GET" ],
                                       AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                       ContentType                = HTTPContentType.Text.HTML_UTF8,
                                       Content                    = MixWithHTMLTemplate("debugLog.debugLog.shtml").ToUTF8Bytes(),
                                       Connection                 = ConnectionType.KeepAlive,
                                       Vary                       = "Accept"
                                   }.AsImmutable;

                        }

                    );

                }

                #endregion


                #region GET ~/support

                CommonHTTPAPI.HTTPBaseAPI.AddHandler(
                    HTTPMethod.GET,
                    OverlayURLPathPrefix.Value + "/support",
                    HTTPContentType.Text.HTML_UTF8,
                    HTTPDelegate: request =>

                        Task.FromResult(
                            new HTTPResponse.Builder(request) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Server                     = HTTPServerName,
                                Date                       = Timestamp.Now,
                                AccessControlAllowOrigin   = "*",
                                AccessControlAllowMethods  = [ "GET" ],
                                AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                ContentType                = HTTPContentType.Text.HTML_UTF8,
                                Content                    = MixWithHTMLTemplate("support.support.shtml").ToUTF8Bytes(),
                                Connection                 = ConnectionType.KeepAlive,
                                Vary                       = "Accept"
                            }.AsImmutable
                        )

                );

                #endregion

                #region GET ~/favicon.png

                CommonHTTPAPI.HTTPBaseAPI.AddHandler(
                    HTTPMethod.GET,
                    OverlayURLPathPrefix.Value + "/favicon.png",
                    //HTTPContentType.Image.PNG,
                    HTTPDelegate: request =>

                        Task.FromResult(
                            new HTTPResponse.Builder(request) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Server                     = HTTPServerName,
                                Date                       = Timestamp.Now,
                                AccessControlAllowOrigin   = "*",
                                AccessControlAllowMethods  = [ "GET" ],
                                AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                ContentType                = HTTPContentType.Image.PNG,
                                Content                    = GetResourceBytes("images.favicon_big.png"),
                                Connection                 = ConnectionType.KeepAlive
                            }.AsImmutable
                        )

                );

                #endregion

            }

        }

        #endregion


    }

}
