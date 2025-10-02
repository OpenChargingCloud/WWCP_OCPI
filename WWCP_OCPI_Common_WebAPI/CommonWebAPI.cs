/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPI;

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
    public class CommonWebAPI : //HTTPExtAPIX
                                AHTTPExtAPIXExtension2<CommonHTTPAPI, HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP URI prefix.
        /// </summary>
        public     static readonly  HTTPPath            DefaultURLPathPrefix      = HTTPPath.Parse("webapi");

        /// <summary>
        /// The default HTTP service name.
        /// </summary>
        public new const            String              DefaultHTTPServiceName    = $"Open Charging Cloud OCPI WebAPI";

        /// <summary>
        /// The default HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public     const            String              DefaultHTTPRealm          = $"Open Charging Cloud OCPI WebAPI";

        /// <summary>
        /// The HTTP root for embedded resources.
        /// </summary>
        public     const            String              HTTPRoot                  = "cloud.charging.open.protocols.OCPI.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        ///// <summary>
        ///// The HTTP content type for serving OCPI+ XML data.
        ///// </summary>
        //public static readonly HTTPContentType                      OCPIPlusJSONContentType     = new ("application", "vnd.OCPIPlus+json", "utf-8", null, null);

        ///// <summary>
        ///// The HTTP content type for serving OCPI+ HTML data.
        ///// </summary>
        //public static readonly HTTPContentType                      OCPIPlusHTMLContentType     = new ("application", "vnd.OCPIPlus+html", "utf-8", null, null);


        public static readonly HTTPEventSource_Id                   DebugLogId                  = HTTPEventSource_Id.Parse($"OCPI_debugLog");

        #endregion

        #region Properties

        /// <summary>
        /// The HTTP URI prefix.
        /// </summary>
        public HTTPPath?                                    OverlayURLPathPrefix    { get; }

        /// <summary>
        /// The HTTP URI prefix.
        /// </summary>
        public HTTPPath?                                    APIURLPathPrefix        { get; }


        /// <summary>
        /// The HTTP realm, if HTTP Basic Authentication is used.
        /// </summary>
        public String                                       HTTPRealm               { get; }

        /// <summary>
        /// An enumeration of logins for an optional HTTP Basic Authentication.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>>    HTTPLogins              { get; }


        /// <summary>
        /// Send debug information via HTTP Server Sent Events.
        /// </summary>
        public HTTPEventSource<JObject>                     DebugLog                { get; }


        public CommonHTTPAPI                                CommonHTTPAPI
            => HTTPBaseAPI;


        /// <summary>
        /// The default request timeout for new CPO/EMSP clients.
        /// </summary>
        //public TimeSpan?                                    RequestTimeout          { get; set; }

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

        static CommonWebAPI()
        {
            // Using static variables within normal constructors seems to
            // have a problem setting them up to their expected values!
        }

        /// <summary>
        /// Attach the OCPI WebAPI to the given HTTP server.
        /// </summary>
        /// <param name="CommonHTTPAPI">A OCPI Common API.</param>
        /// 
        /// <param name="OverlayURLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="APIURLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="WebAPIURLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="BasePath">The base path of the HTTP server.</param>
        /// 
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public CommonWebAPI(CommonHTTPAPI                               CommonHTTPAPI,

                            HTTPPath?                                   OverlayURLPathPrefix      = null,
                            HTTPPath?                                   APIURLPathPrefix          = null,
                            HTTPPath?                                   WebAPIURLPathPrefix       = null,
                            HTTPPath?                                   BasePath                  = null,  // For URL prefixes in HTML!

                            I18NString?                                 Description               = null,

                            String                                      HTTPRealm                 = DefaultHTTPRealm,
                            IEnumerable<KeyValuePair<String, String>>?  HTTPLogins                = null,

                            //HTTPPath?                                   URLPathPrefix             = null,

                            String?                                     ExternalDNSName           = null,
                            String?                                     HTTPServerName            = DefaultHTTPServerName,
                            String?                                     HTTPServiceName           = DefaultHTTPServiceName,
                            String?                                     APIVersionHash            = null,
                            JObject?                                    APIVersionHashes          = null,

                            Boolean?                                    IsDevelopment             = false,
                            IEnumerable<String>?                        DevelopmentServers        = null,
                            Boolean?                                    DisableNotifications      = false,
                            Boolean?                                    DisableLogging            = false,
                            String?                                     LoggingPath               = null,
                            String?                                     LogfileName               = null,
                            LogfileCreatorDelegate?                     LogfileCreator            = null)

            : base(CommonHTTPAPI,
                   WebAPIURLPathPrefix,
                   BasePath,

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
                   LogfileName,
                   LogfileCreator)

        {

            this.OverlayURLPathPrefix  = OverlayURLPathPrefix;
            this.APIURLPathPrefix      = APIURLPathPrefix;
            this.HTTPRealm             = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins            = HTTPLogins ?? [];

            var LogfilePrefix          = "HTTPSSEs" + Path.DirectorySeparatorChar;

          //  this.DebugLog              = this.AddJSONEventSource(EventIdentification:      DebugLogId,
          //                                                       URLTemplate:              HTTPPath.Root + DebugLogId.ToString(), //this.URLPathPrefix + DebugLogId.ToString(),
          //                                                       MaxNumberOfCachedEvents:  10000,
          //                                                       RetryInterval :           TimeSpan.FromSeconds(5),
          //                                                       EnableLogging:            true,
          //                                                       LogfilePrefix:            LogfilePrefix);

            RegisterURITemplates();

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(CommonWebAPI.HTTPRoot, typeof(CommonWebAPI).Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.     HTTPRoot, typeof(HTTPAPI).     Assembly)
               );

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(CommonWebAPI.HTTPRoot, typeof(CommonWebAPI).Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.     HTTPRoot, typeof(HTTPAPI).     Assembly)
               );

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(CommonWebAPI.HTTPRoot, typeof(CommonWebAPI).Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.     HTTPRoot, typeof(HTTPAPI).     Assembly)
               );

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(CommonWebAPI.HTTPRoot, typeof(CommonWebAPI).Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.     HTTPRoot, typeof(HTTPAPI).     Assembly)
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(CommonWebAPI.HTTPRoot, typeof(CommonWebAPI).Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.     HTTPRoot, typeof(HTTPAPI).     Assembly)
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter)

        protected override String MixWithHTMLTemplate(String ResourceName, Func<String, String> HTMLConverter)

            => MixWithHTMLTemplate(
                   ResourceName,
                   HTMLConverter,
                   new Tuple<String, System.Reflection.Assembly>(CommonWebAPI.HTTPRoot, typeof(CommonWebAPI).Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.     HTTPRoot, typeof(HTTPAPI).     Assembly)
               );

        #endregion

        #endregion

        /// <summary>
        /// The following will register HTTP overlays for text/html
        /// showing a html representation of the OCPI Common API!
        /// </summary>
        private void RegisterURITemplates()
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

                #region Text

                //CommonHTTPAPI.HTTPBaseAPI.AddHandler(
                //    HTTPMethod.GET,
                //    OverlayURLPathPrefix.Value,
                //    HTTPContentType.Text.HTML_UTF8,
                //    HTTPDelegate: request =>

                //        Task.FromResult(
                //            new HTTPResponse.Builder(request) {
                //                HTTPStatusCode             = HTTPStatusCode.OK,
                //                Server                     = HTTPServiceName,
                //                Date                       = Timestamp.Now,
                //                AccessControlAllowOrigin   = "*",
                //                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                //                AccessControlAllowHeaders  = [ "Authorization" ],
                //                ContentType                = HTTPContentType.Text.PLAIN,
                //                Content                    = ("This is an Open Charge Point Interface v2.x HTTP service!" + Environment.NewLine + "Please check ~/ versions!").ToUTF8Bytes(),
                //                Connection                 = ConnectionType.KeepAlive,
                //                Vary                       = "Accept"
                //            }.AsImmutable),

                //    AllowReplacement: URLReplacement.Allow

                //);


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
                //                    ContentType                = HTTPContentType.Text.PLAIN,
                //                    Content                    = ("This is an Open Charge Point Interface v2.x HTTP service!" + Environment.NewLine + "Please check ~/ versions!").ToUTF8Bytes(),
                //                    Connection                 = ConnectionType.KeepAlive,
                //                    Vary                       = "Accept"
                //                }.AsImmutable),

                //        AllowReplacement: URLReplacement.Allow

                //    );

                #endregion

                #region JSON

                //CommonHTTPAPI.HTTPBaseAPI.AddHandler(
                //    HTTPMethod.GET,
                //    OverlayURLPathPrefix.Value,
                //    HTTPContentType.Application.JSON_UTF8,
                //    HTTPDelegate: request =>

                //        Task.FromResult(
                //            new HTTPResponse.Builder(request) {
                //                HTTPStatusCode             = HTTPStatusCode.OK,
                //                Server                     = HTTPServiceName,
                //                Date                       = Timestamp.Now,
                //                AccessControlAllowOrigin   = "*",
                //                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                //                AccessControlAllowHeaders  = [ "Authorization" ],
                //                ContentType                = HTTPContentType.Application.JSON_UTF8,
                //                Content                    = JSONObject.Create(
                //                                                 new JProperty(
                //                                                     "message",
                //                                                     "This is an Open Charge Point Interface v2.x HTTP service! Please check ~/ versions!"
                //                                                 )
                //                                             ).ToUTF8Bytes(),
                //                Connection                 = ConnectionType.KeepAlive,
                //                Vary                       = "Accept"
                //            }.AsImmutable),

                //    AllowReplacement: URLReplacement.Allow

                //);


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
                //                    ContentType                = HTTPContentType.Application.JSON_UTF8,
                //                    Content                    = JSONObject.Create(
                //                                                     new JProperty(
                //                                                         "message",
                //                                                         "This is an Open Charge Point Interface v2.x HTTP service! Please check ~/ versions!"
                //                                                     )
                //                                                 ).ToUTF8Bytes(),
                //                    Connection                 = ConnectionType.KeepAlive,
                //                    Vary                       = "Accept"
                //                }.AsImmutable),

                //        AllowReplacement: URLReplacement.Allow

                //    );

                #endregion

                #region HTML

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
