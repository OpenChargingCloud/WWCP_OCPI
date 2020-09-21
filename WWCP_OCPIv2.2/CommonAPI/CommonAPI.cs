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
using System.IO;
using System.Linq;
using System.Reflection;

using Org.BouncyCastle.Bcpg.OpenPgp;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// The common HTTP API.
    /// </summary>
    public abstract class CommonAPI : HTTPAPI
    {

        #region Data

        private static readonly Random         _Random                         = new Random();

        protected internal const String        __DefaultHTTPRoot               = "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot";

        private readonly Func<String, Stream>  _GetRessources;


        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public const           String          DefaultHTTPServerName           = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public static readonly IPPort          DefaultHTTPServerPort           = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP logfile.
        /// </summary>
        public const           String          DefaultLogfileName              = "OCPI_HTTPAPI.log";

        public static readonly HTTPPath         DefaultURLPathPrefix                = HTTPPath.Parse("/ext/OCPI");

        #endregion

        #region Properties

        public RoamingNetwork RoamingNetwork { get; }


        #region ServiceName

        private readonly String _ServiceName;

        /// <summary>
        /// The name of the Open Data API service.
        /// </summary>
        public String ServiceName
        {
            get
            {
                return _ServiceName;
            }
        }

        #endregion

        #endregion

        #region Events

        #region Generic HTTP server logging

        /// <summary>
        /// An event called whenever a HTTP request came in.
        /// </summary>
        public HTTPRequestLogEvent   RequestLog    = new HTTPRequestLogEvent();

        /// <summary>
        /// An event called whenever a HTTP request could successfully be processed.
        /// </summary>
        public HTTPResponseLogEvent  ResponseLog   = new HTTPResponseLogEvent();

        /// <summary>
        /// An event called whenever a HTTP request resulted in an error.
        /// </summary>
        public HTTPErrorLogEvent     ErrorLog      = new HTTPErrorLogEvent();

        #endregion

        #endregion

        #region Constructor(s)

        #region CommonAPI(HTTPServerName, ...)

        public CommonAPI(RoamingNetwork        RoamingNetwork,
                         String                HTTPServerName          = DefaultHTTPServerName,
                         IPPort?               HTTPServerPort          = null,
                         HTTPPath?             URLPathPrefix           = null,
                         Func<String, Stream>  GetRessources           = null,

                         String                ServiceName             = DefaultHTTPServerName,

                         DNSClient             DNSClient               = null,
                         String                LogfileName             = DefaultLogfileName)

            : this(RoamingNetwork,
                   new HTTPServer(DefaultServerName: DefaultHTTPServerName,
                                  DNSClient:         DNSClient),
                   URLPathPrefix,
                   GetRessources,

                   ServiceName,

                   LogfileName)

        {

            HTTPServer.AttachTCPPorts(HTTPServerPort ?? DefaultHTTPServerPort);

        }

        #endregion

        #region CommonAPI(HTTPServer, ...)

        /// <summary>
        /// Initialize the OCPI HTTP server using IPAddress.Any, http port 8080 and maybe start the server.
        /// </summary>
        public CommonAPI(RoamingNetwork        RoamingNetwork,
                         HTTPServer            HTTPServer,
                         HTTPPath?             URLPathPrefix           = null,
                         Func<String, Stream>  GetRessources           = null,

                         String                ServiceName             = DefaultHTTPServerName,

                         String                LogfileName             = DefaultLogfileName)

            : base(HTTPServer,
                   HTTPHostname.Any,
                   ServiceName,
                   null, //BaseURL
                   URLPathPrefix
                   //HTMLTemplate
                   )

        {

            #region Initial checks

            if (RoamingNetwork == null)
                throw new ArgumentNullException(nameof(RoamingNetwork), "The given roaming network must not be null!");

            #endregion

            #region Init data

            this._GetRessources           = GetRessources;

            this._ServiceName             = ServiceName;

            #endregion

            // Link HTTP events...
            HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            RegisterURITemplates();

        }

        #endregion

        #endregion


        #region (private) RegisterURITemplates()

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

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

            #endregion

        }

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

                //SendCompleted(this, DateTime.Now, Message);

            }

        }

        #endregion

    }

}
