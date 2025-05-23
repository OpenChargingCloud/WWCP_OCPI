﻿/*
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
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0.HTTP;
using System;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.WebAPI
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
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
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
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown RoamingNetworkId!"" }".ToUTF8Bytes()
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseRemotePartyId(this HTTPRequest, OCPIWebAPI, out RemotePartyId,                  out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the remote party identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPIWebAPI">The OCPI WebAPI.</param>
        /// <param name="RemotePartyId">The parsed unique remote party identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when remote party identification was found; false else.</returns>
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
                    Connection      = ConnectionType.Close
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
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid remote party identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseRemoteParty  (this HTTPRequest, OCPIWebAPI, out RemotePartyId, out RemoteParty, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the remote party identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="OCPIWebAPI">The OCPI WebAPI.</param>
        /// <param name="RemotePartyId">The parsed unique remote party identification.</param>
        /// <param name="RemoteParty">The resolved remote party.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when remote party identification was found; false else.</returns>
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
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            RemotePartyId = RemoteParty_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!RemotePartyId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = OCPIWebAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid remote party identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
                };

                return false;

            }

            if (!OCPIWebAPI.CommonAPI.TryGetRemoteParty(RemotePartyId.Value, out RemoteParty)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = OCPIWebAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.Application.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown remote party identification!"" }".ToUTF8Bytes(),
                    Connection      = ConnectionType.Close
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
        /// The HTTP root for embedded resources.
        /// </summary>
        public new const       String                               HTTPRoot                    = "cloud.charging.open.protocols.OCPIv2_3_0.WebAPI.HTTPRoot.";


        //ToDo: http://www.iana.org/form/media-types

        /// <summary>
        /// The HTTP content type for serving OCPI+ XML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPIPlusJSONContentType     = new ("application", "vnd.OCPIPlus+json", "utf-8", null, null);

        /// <summary>
        /// The HTTP content type for serving OCPI+ HTML data.
        /// </summary>
        public static readonly HTTPContentType                      OCPIPlusHTMLContentType     = new ("application", "vnd.OCPIPlus+html", "utf-8", null, null);


        public static readonly HTTPEventSource_Id                   DebugLogId                  = HTTPEventSource_Id.Parse($"OCPI{Version.String}_debugLog");

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


        public CommonAPI                                    CommonAPI               { get; }

        public CommonAPILogger                              CommonAPILogger         { get; set; }


        public CPOAPI?                                      CPOAPI                  { get; set; }

        public CPOAPILogger?                                CPOAPILogger            { get; set; }


        public EMSPAPI?                                     EMSPAPI                 { get; set; }

        public EMSPAPILogger?                               EMSPAPILogger           { get; set; }

        /// <summary>
        /// The default request timeout for new CPO/EMSP clients.
        /// </summary>
        public TimeSpan?                                    RequestTimeout          { get; set; }

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<RemoteParty>?       CustomRemotePartySerializer         { get; set; }
        public CustomJObjectSerializerDelegate<CredentialsRole>?   CustomCredentialsRoleSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?   CustomBusinessDetailsSerializer     { get; set; }
        public CustomJObjectSerializerDelegate<OCPI.Image>?        CustomImageSerializer               { get; set; }
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

        /// <summary>
        /// Attach the OCPI+ WebAPI to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="WebAPIURLPathPrefix">An optional prefix for the HTTP URIs.</param>
        /// <param name="HTTPRealm">The HTTP realm, if HTTP Basic Authentication is used.</param>
        /// <param name="HTTPLogins">An enumeration of logins for an optional HTTP Basic Authentication.</param>
        public OCPIWebAPI(HTTPServer                                  HTTPServer,

                          CommonAPI                                   CommonAPI,

                          HTTPPath?                                   OverlayURLPathPrefix    = null,
                          HTTPPath?                                   APIURLPathPrefix        = null,
                          HTTPPath?                                   WebAPIURLPathPrefix     = null,
                          HTTPPath?                                   BasePath                = null,
                          String                                      HTTPRealm               = DefaultHTTPRealm,
                          IEnumerable<KeyValuePair<String, String>>?  HTTPLogins              = null,
                          String?                                     HTMLTemplate            = null,

                          Organization_Id?                            AdminOrganizationId     = null,
                          EMailAddress?                               APIRobotEMailAddress    = null,
                          String?                                     APIRobotGPGPassphrase   = null,
                          ISMTPClient?                                SMTPClient              = null,

                          Boolean                                     SkipURLTemplates        = true,

                          TimeSpan?                                   RequestTimeout          = null)

            : base(HTTPServer,
                   null,
                   null, // ExternalDNSName,
                   null, // HTTPServiceName,
                   BasePath,
                   WebAPIURLPathPrefix ?? DefaultURLPathPrefix,
                   AutoStart: false)

        {

            this.CommonAPI             = CommonAPI;
            this.APIURLPathPrefix      = APIURLPathPrefix;
            this.OverlayURLPathPrefix  = OverlayURLPathPrefix;
            this.HTTPRealm             = HTTPRealm.IsNotNullOrEmpty() ? HTTPRealm : DefaultHTTPRealm;
            this.HTTPLogins            = HTTPLogins ?? [];

            //this.cpoClients            = new List<CPOClient>();
            //this.emspClients           = new List<EMSPClient>();

            // Link HTTP events...
            HTTPServer.RequestLog     += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog    += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog       += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            var LogfilePrefix          = "HTTPSSEs" + Path.DirectorySeparatorChar;

            this.DebugLog              = this.AddJSONEventSource(EventIdentification:      DebugLogId,
                                                                 URLTemplate:              this.URLPathPrefix + "debugLog",
                                                                 MaxNumberOfCachedEvents:  10000,
                                                                 RetryInterval:            TimeSpan.FromSeconds(5),
                                                                 EnableLogging:            true,
                                                                 LogfilePrefix:            LogfilePrefix);

            RegisterURITemplates();

            //this.HTMLTemplate          = HTMLTemplate ?? GetResourceString("template.html");
            this.RequestTimeout        = RequestTimeout;

        }

        #endregion


        #region (private) RegisterURLTemplates()

        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                   //new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
               );

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                   //new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
               );

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                   //new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
               );

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                   //new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(
                   ResourceName,
                   new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                   //new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
               );

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName, HTMLConverter)

        protected override String MixWithHTMLTemplate(String ResourceName, Func<String, String> HTMLConverter)

            => MixWithHTMLTemplate(
                   ResourceName,
                   HTMLConverter,
                   new Tuple<String, System.Reflection.Assembly>(OCPIWebAPI.HTTPRoot, typeof(OCPIWebAPI).Assembly),
                   //new Tuple<String, System.Reflection.Assembly>(UsersAPI.  HTTPRoot, typeof(UsersAPI).  Assembly),
                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI.   HTTPRoot, typeof(HTTPAPI).   Assembly)
               );

        #endregion

        #endregion

        /// <summary>
        /// The following will register HTTP overlays for text/html
        /// showing a html representation of the OCPI common API!
        /// </summary>
        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            this.MapResourceAssemblyFolder(HTTPHostname.Any,
                                           URLPathPrefix,
                                           HTTPRoot,
                                           DefaultFilename: "index.html");


            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value,
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                         AccessControlAllowHeaders  = [ "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("index.shtml",
                                                                                                          html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + "/",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                         AccessControlAllowHeaders  = [ "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("index.shtml",
                                                                                                          html => html.Replace("{{versionPath}}", "")).ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });

            #endregion

            #region ~/libs

            //this.MapResourceAssemblyFolder(HTTPHostname.Any,
            //                               URLPathPrefix + "libs",
            //                               UsersAPI.HTTPRoot,
            //                               typeof(UsersAPI).Assembly);

            #endregion


            #region ~/versions

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + "versions",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                         AccessControlAllowHeaders  = [ "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("versions.versions.shtml",
                                                                                                          html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });

            #endregion

            #region ~/versions/{versionId}

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + "versions/{versionId}",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                         AccessControlAllowHeaders  = [ "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("versions.versionDetails.shtml",
                                                                                                          html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });

            #endregion


            #region ~/v2.1.1/cpo/locations

            // ~/cpo/locations
            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + Version.String + "/cpo/locations",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: request => {

                                                 // Appending "?download" to the URL within a web browser will open a download dialog.
                                                 // Note: This happens here, as the ACCEPT types of the HTTP request do often not include "application/json"!
                                                 var download = request.QueryString.GetBoolean("download", false);

                                                 return Task.FromResult(
                                                     download

                                                         ? new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode              = HTTPStatusCode.OK,
                                                               Server                      = HTTPServer.DefaultServerName,
                                                               Date                        = Timestamp.Now,
                                                               AccessControlAllowOrigin    = "*",
                                                               AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders   = [ "Content-Type", "Accept", "Authorization" ],
                                                               AccessControlExposeHeaders  = [ "Link", "X-Total-Count", "X-Filtered-Count"],
                                                               ContentDisposition          = download
                                                                                                 ? @"attachment; filename = ""locations.json"""
                                                                                                 : null,
                                                               ContentType                 = HTTPContentType.Application.JSON_UTF8,
                                                               Content                     = new JArray(CommonAPI.GetLocations().Select(location => location.ToJSON())).ToUTF8Bytes(),
                                                               Vary                        = "Accept",
                                                               Connection                  = ConnectionType.Close
                                                           }.AsImmutable

                                                         : new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode              = HTTPStatusCode.OK,
                                                               Server                      = HTTPServer.DefaultServerName,
                                                               Date                        = Timestamp.Now,
                                                               AccessControlAllowOrigin    = "*",
                                                               AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders   = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentType                 = HTTPContentType.Text.HTML_UTF8,
                                                               Content                     = MixWithHTMLTemplate("locations.locations.shtml",
                                                                                                           html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                               Connection                  = ConnectionType.Close,
                                                               Vary                        = "Accept"
                                                           }.AsImmutable

                                                 );

                                             });


            // ~/cpo/locationStatistics
            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + Version.String + "/cpo/locationStatistics",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                         AccessControlAllowHeaders  = [ "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("locations.locationStatistics.shtml",
                                                                                                          html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });

            #endregion

            #region ~/v2.1.1/cpo/sessions

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + Version.String + "/cpo/sessions",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: request => {

                                                 // Appending "?download" to the URL within a web browser will open a download dialog.
                                                 // Note: This happens here, as the ACCEPT types of the HTTP request do often not include "application/json"!
                                                 var download = request.QueryString.GetBoolean("download", false);

                                                 return Task.FromResult(
                                                     download

                                                         ? new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               Server                     = HTTPServer.DefaultServerName,
                                                               Date                       = Timestamp.Now,
                                                               AccessControlAllowOrigin   = "*",
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentDisposition         = download
                                                                                                ? @"attachment; filename = ""sessions.json"""
                                                                                                : null,
                                                               ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                               //Content                    = new JArray(CommonAPI.GetSessions().Select(session => session.ToJSON())).ToUTF8Bytes(),
                                                               Vary                       = "Accept",
                                                               Connection                 = ConnectionType.Close
                                                           }.AsImmutable

                                                         : new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               Server                     = HTTPServer.DefaultServerName,
                                                               Date                       = Timestamp.Now,
                                                               AccessControlAllowOrigin   = "*",
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                               Content                    = MixWithHTMLTemplate("sessions.sessions.shtml",
                                                                                                                html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                               Connection                 = ConnectionType.Close,
                                                               Vary                       = "Accept"
                                                           }.AsImmutable

                                                 );

                                             });

            #endregion

            #region ~/v2.1.1/cpo/tariffs

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + Version.String + "/cpo/tariffs",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: request => {

                                                 // Appending "?download" to the URL within a web browser will open a download dialog.
                                                 // Note: This happens here, as the ACCEPT types of the HTTP request do often not include "application/json"!
                                                 var download = request.QueryString.GetBoolean("download", false);

                                                 return Task.FromResult(
                                                     download

                                                         ? new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               Server                     = HTTPServer.DefaultServerName,
                                                               Date                       = Timestamp.Now,
                                                               AccessControlAllowOrigin   = "*",
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentDisposition         = download
                                                                                                ? @"attachment; filename = ""tariffs.json"""
                                                                                                : null,
                                                               ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                               Content                    = new JArray(CommonAPI.GetTariffs().Select(tariff => tariff.ToJSON())).ToUTF8Bytes(),
                                                               Vary                       = "Accept",
                                                               Connection                 = ConnectionType.Close
                                                           }.AsImmutable

                                                         : new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               //Server                     = DefaultHTTPServerName,
                                                               Date                       = Timestamp.Now,
                                                               AccessControlAllowOrigin   = "*",
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                               Content                    = MixWithHTMLTemplate("tariffs.tariffs.shtml",
                                                                                                                html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                               Connection                 = ConnectionType.Close,
                                                               Vary                       = "Accept"
                                                           }.AsImmutable

                                                 );

                                             });

            #endregion

            #region ~/v2.1.1/cpo/cdrs

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + Version.String + "/cpo/cdrs",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: request => {

                                                 // Appending "?download" to the URL within a web browser will open a download dialog.
                                                 // Note: This happens here, as the ACCEPT types of the HTTP request do often not include "application/json"!
                                                 var download = request.QueryString.GetBoolean("download", false);

                                                 return Task.FromResult(
                                                     download

                                                         ? new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               Server                     = HTTPServer.DefaultServerName,
                                                               Date                       = Timestamp.Now,
                                                               AccessControlAllowOrigin   = "*",
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentDisposition         = download
                                                                                                ? @"attachment; filename = ""cdrs.json"""
                                                                                                : null,
                                                               ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                               //Content                    = new JArray(CommonAPI.GetCDRs().Select(cdr => cdr.ToJSON())).ToUTF8Bytes(),
                                                               Vary                       = "Accept",
                                                               Connection                 = ConnectionType.Close
                                                           }.AsImmutable

                                                         : new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               Server                     = HTTPServer.DefaultServerName,
                                                               Date                       = Timestamp.Now,
                                                               AccessControlAllowOrigin   = "*",
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                               Content                    = MixWithHTMLTemplate("cdrs.cdrs.shtml",
                                                                                                                html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                               Connection                 = ConnectionType.Close,
                                                               Vary                       = "Accept"
                                                           }.AsImmutable

                                                 );

                                             });

            #endregion

            #region ~/v2.1.1/cpo/commands

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + Version.String + "/cpo/commands",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: Request => {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         //Server                     = DefaultHTTPServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                         AccessControlAllowHeaders  = [ "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("commands.commands.shtml",
                                                                                                          html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             });

            #endregion

            #region ~/v2.1.1/cpo/tokens

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + Version.String + "/cpo/tokens",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: request => {

                                                 // Appending "?download" to the URL within a web browser will open a download dialog.
                                                 // Note: This happens here, as the ACCEPT types of the HTTP request do often not include "application/json"!
                                                 var download = request.QueryString.GetBoolean("download", false);

                                                 return Task.FromResult(
                                                     download

                                                         ? new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               Server                     = HTTPServer.DefaultServerName,
                                                               Date                       = Timestamp.Now,
                                                               AccessControlAllowOrigin   = "*",
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentDisposition         = download
                                                                                                ? @"attachment; filename = ""tokens.json"""
                                                                                                : null,
                                                               ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                               //Content                    = new JArray(CommonAPI.GetTokens().Select(token => token.ToJSON())).ToUTF8Bytes(),
                                                               Vary                       = "Accept",
                                                               Connection                 = ConnectionType.Close
                                                           }.AsImmutable

                                                         : new HTTPResponse.Builder(request) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               Server                     = HTTPServer.DefaultServerName,
                                                               Date                       = Timestamp.Now,
                                                               AccessControlAllowOrigin   = "*",
                                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                               ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                               Content                    = MixWithHTMLTemplate("tokens.tokens.shtml",
                                                                                                                html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                               Connection                 = ConnectionType.Close,
                                                               Vary                       = "Accept"
                                                           }.AsImmutable

                                                 );

                                             });

            #endregion


            #region ~/remoteXXXParties

            #region OPTIONS            ~/remoteXXXParties

            // --------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteXXXParties
            // --------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "remoteXXXParties",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "OPTIONS", "GET", "ReserveNow", "CancelReservation", "StartSession", "StopSession", "UnlockConnector" ],
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST,
                                                                                      HTTP_ReserveNow,
                                                                                      HTTP_CancelReservation,
                                                                                      HTTP_StartSession,
                                                                                      HTTP_StopSession,
                                                                                      HTTP_UnlockConnector
                                                                                  },
                                                     AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteParties

            #region JSON

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/remoteParties
            // ---------------------------------------------------------------------------
            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         OverlayURLPathPrefix.Value + "remoteParties",
                                         HTTPContentType.Application.JSON_UTF8,
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


                                             var matchFilter            = Request.QueryString.CreateStringFilter<RemoteParty>("match",
                                                                                                                              (remoteParty, pattern) => remoteParty.Id.         ToString().Contains(pattern) ||
                                                                                                                                                        remoteParty.Roles.Any(role => role.BusinessDetails?.Name. Contains(pattern) == true));
                                             var skip                   = Request.QueryString.GetUInt64 ("skip");
                                             var take                   = Request.QueryString.GetUInt64 ("take");
                                             var matchStatusFilter      = Request.QueryString.CreateMultiEnumFilter<PartyStatus>("matchStatus");

                                             var allRemoteParties       = CommonAPI.RemoteParties.
                                                                              //Where(remoteParty => HTTPOrganizations.Contains(remoteParty.Owner) ||
                                                                              //                           Admins.InEdges(HTTPUser).
                                                                              //                                  Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)).
                                                                              ToArray();
                                             var totalCount             = allRemoteParties.ULongCount();

                                             var filteredRemoteParties  = allRemoteParties.
                                                                              Where(matchFilter).
                                                                              Where(remoteParty => matchStatusFilter(remoteParty.Status)).
                                                                              ToArray();
                                             var filteredCount          = filteredRemoteParties.ULongCount();

                                             var jsonResults            = filteredRemoteParties.
                                                                              OrderBy(remoteParty => remoteParty.Id).
                                                                              ToJSON (skip,
                                                                                      take,
                                                                                      false, //Embedded
                                                                                      null,
                                                                                      null,
                                                                                      null);


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "GET", "OPTIONS" ],
                                                     AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                     ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                     Content                    = OCPIResponse<JArray>.Create(
                                                                                      jsonResults,
                                                                                      xxx => xxx,
                                                                                      1000,
                                                                                      ""
                                                                                  ).ToUTF8Bytes(),
                                                     Connection                 = ConnectionType.Close,
                                                     Vary                       = "Accept"
                                                 }.
                                                 Set("X-Total-Count",     allRemoteParties.     Length).
                                                 Set("X-Filtered-Count",  filteredRemoteParties.Length).
                                                 Set("X-Limit",           filteredRemoteParties.Length)
                                                 .AsImmutable);

                                         });

            #endregion

            #region HTML

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/remoteParties
            // ---------------------------------------------------------------------------
            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             Hostname,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + "remoteParties",
                                             HTTPContentType.Text.HTML_UTF8,
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
                                                                AccessControlAllowMethods  = [ "GET" ],
                                                                AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                                Content                    = MixWithHTMLTemplate("remoteParties.remoteParties.shtml",
                                                                                                                 html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                                Connection                 = ConnectionType.Close,
                                                                Vary                       = "Accept"
                                                            }.AsImmutable);
                                                        });

            #endregion

            #endregion


            #region OPTIONS            ~/remoteXXXParties/{remotePartyId}

            // -------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO
            // -------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "remoteXXXParties/{remotePartyId}",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "GET", "OPTIONS" ],
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteXXXParties/{remotePartyId}

            #region JSON

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/api/remoteXXXParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}",
                                         HTTPContentType.Application.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out var httpUser,
                                             //                    out var httpOrganizations,
                                             //                    out var httpResponseBuilder1,
                                             //                    Access_Levels.ReadWrite,
                                             //                    Recursive: true) ||
                                             //    httpUser is null)
                                             //{
                                             //    return Task.FromResult(httpResponseBuilder1!.AsImmutable);
                                             //}

                                             #endregion

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out var remotePartyId,
                                                                           out var remoteParty,
                                                                           out var httpResponseBuilder))
                                             {
                                                 return Task.FromResult(httpResponseBuilder!.AsImmutable);
                                             }

                                             #endregion


                                             return Task.FromResult(
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = remoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               remoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------
            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             Hostname,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + "remoteXXXParties/{remotePartyId}",
                                             HTTPContentType.Text.HTML_UTF8,
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
                                                                               out var remotePartyId,
                                                                               out var remoteParty,
                                                                               out var httpResponseBuilder))
                                                 {
                                                     return Task.FromResult(httpResponseBuilder.AsImmutable);
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
                                                         AccessControlAllowMethods  = [ "GET" ],
                                                         AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteXXXParties.remoteParty.shtml",
                                                                                                          html => html.Replace("{{versionPath}}", "v2.2/")).ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                                 //}

                                                 //return Task.FromResult(
                                                 //           new HTTPResponse.Builder(Request) {
                                                 //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                 //               Server                     = HTTPServer.DefaultServerName,
                                                 //               Date                       = Timestamp.Now,
                                                 //               AccessControlAllowOrigin   = "*",
                                                 //               AccessControlAllowMethods  = new[] { "GET" },
                                                 //               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                 //               Connection                 = ConnectionType.Close,
                                                 //               Vary                       = "Accept"
                                                 //           }.AsImmutable);

                                             }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion


            #region OPTIONS            ~/remoteXXXParties/{remotePartyId}/reserveNow

            // ------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/reserveNow
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
                                                     AccessControlAllowMethods  = [ "GET", "OPTIONS" ],
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteXXXParties/{remotePartyId}/reserveNow

            #region JSON

            // ---------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO/reserveNow
            // ---------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}/reserveNow",
                                         HTTPContentType.Application.JSON_UTF8,
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
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/reserveNow
            // --------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteXXXParties/{remotePartyId}/reserveNow",
                                         HTTPContentType.Text.HTML_UTF8,
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
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.reserveNow.shtml").ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = new[] { "GET" },
                                             //               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                             //               Connection                 = ConnectionType.Close,
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region ReserveNow         ~/remoteXXXParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_ReserveNow,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}",
                                         HTTPContentType.Application.JSON_UTF8,
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

                                             if (!Request.TryParseJSONObjectRequestBody(out var JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse Token                     [mandatory]

                                             if (!JSON.ParseMandatoryJSON("token",
                                                                          "token",
                                                                          OCPIv3_0.Token.TryParse,
                                                                          out Token? Token,
                                                                          out String? ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse ExpirationTimestamp       [mandatory]

                                             if (!JSON.ParseMandatory("expiryTimestamp",
                                                                      "expiry timestamp",
                                                                      out DateTime ExpirationTimestamp,
                                                                      out          ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse ReservationId             [mandatory]

                                             if (!JSON.ParseMandatory("reservationId",
                                                                      "reservation identification",
                                                                      Reservation_Id.TryParse,
                                                                      out Reservation_Id ReservationId,
                                                                      out                ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse LocationId                [mandatory]

                                             if (!JSON.ParseMandatory("locationId",
                                                                      "location identification",
                                                                      Location_Id.TryParse,
                                                                      out Location_Id LocationId,
                                                                      out             ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse EVSEUId                   [optional]

                                             if (JSON.ParseOptional("EVSEUId",
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
                                                                AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                               ErrorResponse).
                                                                                                        ToJSON().
                                                                                                        ToUTF8Bytes(),
                                                                Connection                 = ConnectionType.Close
                                                            }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = EMSPAPI.GetCPOClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a appropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                      }.AsImmutable;

                                             #endregion


                                             var reserveNowResult = await emspClient.ReserveNow(Token,
                                                                                                ExpirationTimestamp,
                                                                                                ReservationId,
                                                                                                LocationId,
                                                                                                EVSEUId);

                                             return 
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion


            #region OPTIONS            ~/remoteXXXParties/{remotePartyId}/cancelReservation

            // -------------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/cancelReservation
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
                                                     AccessControlAllowMethods  = [ "GET", "OPTIONS" ],
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteXXXParties/{remotePartyId}/cancelReservation

            #region JSON

            // --------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO/cancelReservation
            // --------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}/cancelReservation",
                                         HTTPContentType.Application.JSON_UTF8,
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
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // -------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/cancelReservation
            // -------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteXXXParties/{remotePartyId}/cancelReservation",
                                         HTTPContentType.Text.HTML_UTF8,
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
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.cancelReservation.shtml").ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = new[] { "GET" },
                                             //               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                             //               Connection                 = ConnectionType.Close,
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region CancelReservation  ~/remoteXXXParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_CancelReservation,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}",
                                         HTTPContentType.Application.JSON_UTF8,
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

                                             if (!Request.TryParseJSONObjectRequestBody(out var JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse ReservationId    [mandatory]

                                             if (!JSON.ParseMandatory("reservationId",
                                                                      "reservation identification",
                                                                      Reservation_Id.TryParse,
                                                                      out Reservation_Id ReservationId,
                                                                      out String         ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = EMSPAPI.GetCPOClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a appropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
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
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion


            #region OPTIONS            ~/remoteXXXParties/{remotePartyId}/startSession

            // --------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/startSession
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
                                                     AccessControlAllowMethods  = [ "GET", "OPTIONS" ],
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteXXXParties/{remotePartyId}/startSession

            #region JSON

            // ---------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO/startSession
            // ---------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}/startSession",
                                         HTTPContentType.Application.JSON_UTF8,
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
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/startSession
            // --------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteXXXParties/{remotePartyId}/startSession",
                                         HTTPContentType.Text.HTML_UTF8,
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
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.startSession.shtml").ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = new[] { "GET" },
                                             //               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                             //               Connection                 = ConnectionType.Close,
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region StartSession       ~/remoteXXXParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_StartSession,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}",
                                         HTTPContentType.Application.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out var RemotePartyId,
                                                                           out var RemoteParty,
                                                                           out var HTTPResponse))
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

                                             if (!Request.TryParseJSONObjectRequestBody(out var JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse Token                     [mandatory]

                                             if (!JSON.ParseMandatoryJSON("token",
                                                                          "token",
                                                                          OCPIv3_0.Token.TryParse,
                                                                          out Token?  Token,
                                                                          out String? ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse LocationId                [mandatory]

                                             if (!JSON.ParseMandatory("locationId",
                                                                      "location identification",
                                                                      Location_Id.TryParse,
                                                                      out Location_Id LocationId,
                                                                      out             ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse EVSEUId                   [mandatory]

                                             if (!JSON.ParseMandatory("EVSEUId",
                                                                      "EVSE unique identification",
                                                                      EVSE_UId.TryParse,
                                                                      out EVSE_UId EVSEUId,
                                                                      out          ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = EMSPAPI.GetCPOClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a appropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                      }.AsImmutable;

                                             #endregion


                                             var startSessionResult = await emspClient.StartSession(Token,
                                                                                                    LocationId,
                                                                                                    EVSEUId);

                                             return 
                                                        //HTTPOrganizations.Contains(RemoteParty.Owner) ||
                                                        //Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdgeTypes.IsAdmin)
                                                        1 == 1

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion


            #region OPTIONS            ~/remoteXXXParties/{remotePartyId}/stopSession

            // -------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/stopSession
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
                                                     AccessControlAllowMethods  = [ "GET", "OPTIONS" ],
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteXXXParties/{remotePartyId}/stopSession

            #region JSON

            // --------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO/stopSession
            // --------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}/stopSession",
                                         HTTPContentType.Application.JSON_UTF8,
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
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // -------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/stopSession
            // -------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteXXXParties/{remotePartyId}/stopSession",
                                         HTTPContentType.Text.HTML_UTF8,
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
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.stopSession.shtml").ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = new[] { "GET" },
                                             //               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                             //               Connection                 = ConnectionType.Close,
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region StopSession        ~/remoteXXXParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_StopSession,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}",
                                         HTTPContentType.Application.JSON_UTF8,
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

                                             if (!Request.TryParseJSONObjectRequestBody(out var JSON, out HTTPResponse))
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
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = EMSPAPI.GetCPOClient(RemoteParty);

                                             if (emspClient == null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a appropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
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
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               stopSessionResult.ToJSON(commandResponse => commandResponse.ToJSON()).
                                                                                                                 ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable;

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion


            #region OPTIONS            ~/remoteXXXParties/{remotePartyId}/unlockConnector

            // -----------------------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/unlockConnector
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
                                                     AccessControlAllowMethods  = [ "GET", "OPTIONS" ],
                                                     Allow                      = new List<HTTPMethod> {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.POST
                                                                                  },
                                                     AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET                ~/remoteXXXParties/{remotePartyId}/unlockConnector

            #region JSON

            // ------------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO/unlockConnector
            // ------------------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}/unlockConnector",
                                         HTTPContentType.Application.JSON_UTF8,
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
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               RemoteParty.ToJSON(false,
                                                                                                                  CustomRemotePartySerializer,
                                                                                                                  CustomCredentialsRoleSerializer,
                                                                                                                  CustomBusinessDetailsSerializer,
                                                                                                                  CustomImageSerializer,
                                                                                                                  CustomLocalAccessInfoSerializer,
                                                                                                                  CustomRemoteAccessInfoSerializer).
                                                                                                           ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
                                                            }.AsImmutable);

                                         },
                                         AllowReplacement: URLReplacement.Allow);

            #endregion

            #region HTML

            // -------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/remoteXXXParties/DE-GDF-CPO/unlockConnector
            // -------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "remoteXXXParties/{remotePartyId}/unlockConnector",
                                         HTTPContentType.Text.HTML_UTF8,
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
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                         ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("remoteParty.remoteCPO.unlockConnector.shtml").ToUTF8Bytes(),
                                                         Connection                 = ConnectionType.Close,
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                             //}

                                             //return Task.FromResult(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                             //               Server                     = HTTPServer.DefaultServerName,
                                             //               Date                       = Timestamp.Now,
                                             //               AccessControlAllowOrigin   = "*",
                                             //               AccessControlAllowMethods  = new[] { "GET" },
                                             //               AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                             //               Connection                 = ConnectionType.Close,
                                             //               Vary                       = "Accept"
                                             //           }.AsImmutable);

                                         }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region UnlockConnector    ~/remoteXXXParties/{remotePartyId}

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/remoteXXXParties/DE-GDF-CPO
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(this,
                                         Hostname,
                                         HTTP_UnlockConnector,
                                         APIURLPathPrefix + "remoteXXXParties/{remotePartyId}",
                                         HTTPContentType.Application.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check RemotePartyId URI parameter

                                             if (!Request.ParseRemoteParty(this,
                                                                           out RemoteParty_Id?        RemotePartyId,
                                                                           out RemoteParty?           RemoteParty,
                                                                           out HTTPResponse.Builder?  HTTPResponse))
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

                                             if (!Request.TryParseJSONObjectRequestBody(out var JSON, out HTTPResponse))
                                                 return HTTPResponse.AsImmutable;

                                             #region Parse LocationId    [mandatory]

                                             if (!JSON.ParseMandatory("locationId",
                                                                      "location identification",
                                                                      Location_Id.TryParse,
                                                                      out Location_Id LocationId,
                                                                      out String      ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse EVSEUId       [mandatory]

                                             if (!JSON.ParseMandatory("EVSEUId",
                                                                      "EVSE unique identification",
                                                                      EVSE_UId.TryParse,
                                                                      out EVSE_UId EVSEUId,
                                                                      out          ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Parse ConnectorId   [mandatory]

                                             if (!JSON.ParseMandatory("connectorId",
                                                                      "connector identification",
                                                                      Connector_Id.TryParse,
                                                                      out Connector_Id ConnectorId,
                                                                      out              ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(org.GraphDefined.Vanaheimr.Illias.Languages.en,
                                                                                                           ErrorResponse).
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Get EMSP client

                                             var emspClient = EMSPAPI.GetCPOClient(RemoteParty);

                                             if (emspClient is null)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadGateway,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = [ "GET", "SET" ],
                                                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                            Content                    = I18NString.Create(Languages.en,
                                                                                                           "Could not find a appropriate EMSP client for this request!").
                                                                                                    ToJSON().
                                                                                                    ToUTF8Bytes(),
                                                            Connection                 = ConnectionType.Close
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
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" ],
                                                                  ETag                       = RemoteParty.ETag,
                                                                  ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                                  Content                    = //GetRemotePartySerializator(Request, HTTPUser)
                                                                                               //        (RemoteParty,
                                                                                               //         false, //Embedded
                                                                                               //         includeCryptoHash).
                                                                                               unlockConnectorResult.ToJSON(commandResponse => commandResponse.ToJSON()).
                                                                                                                     ToUTF8Bytes(),
                                                                  Connection                 = ConnectionType.Close,
                                                                  Vary                       = "Accept"
                                                            }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = Timestamp.Now,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = [ "GET", "SET" ],
                                                                  AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                  Connection                 = ConnectionType.Close
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
                                         HTTPContentType.Application.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var clients = new List<CommonClient>();
                                             clients.AddRange(CPOAPI. CPOClients);
                                             clients.AddRange(EMSPAPI.EMSP2CPOClients);

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                     Content                    = new JArray(clients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                     AccessControlAllowHeaders  = [ "Authorization" ]
                                                     //LastModified               = Location.LastUpdated.ToISO8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET      ~/cpoclients

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "cpoclients",
                                         HTTPContentType.Application.JSON_UTF8,
                                         HTTPDelegate: Request => {


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request)
                                                 {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                     Content                    = new JArray(CPOAPI.CPOClients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                     AccessControlAllowHeaders  = [ "Authorization" ]
                                                     //LastModified               = Location.LastUpdated.ToISO8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET      ~/emspclients

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "emspclients",
                                         HTTPContentType.Application.JSON_UTF8,
                                         HTTPDelegate: Request => {


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     ContentType                = HTTPContentType.Application.JSON_UTF8,
                                                     Content                    = new JArray(EMSPAPI.EMSP2CPOClients.OrderBy(client => client.Description).Select(client => client.ToJSON())).ToUTF8Bytes(),
                                                     AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                     AccessControlAllowHeaders  = [ "Authorization" ]
                                                     //LastModified               = Location.LastUpdated.ToISO8601(),
                                                     //ETag                       = Location.SHA256Hash
                                                 }.AsImmutable);

                                         });

            #endregion


            #region GET      ~/support

            if (OverlayURLPathPrefix.HasValue)
                HTTPServer.AddMethodCallback(this,
                                             HTTPHostname.Any,
                                             HTTPMethod.GET,
                                             OverlayURLPathPrefix.Value + "/support",
                                             HTTPContentType.Text.HTML_UTF8,
                                             HTTPDelegate: request => {

                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(request) {
                                                                HTTPStatusCode             = HTTPStatusCode.OK,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = Timestamp.Now,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = [ "GET" ],
                                                                AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                                                                ContentType                = HTTPContentType.Text.HTML_UTF8,
                                                                Content                    = MixWithHTMLTemplate("support.support.shtml").ToUTF8Bytes(),
                                                                Connection                 = ConnectionType.Close,
                                                                Vary                       = "Accept"
                                                            }.AsImmutable
                                                        );

                                             });

            #endregion

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
