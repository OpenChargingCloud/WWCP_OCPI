/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_1.HTTP
{

    /// <summary>
    /// The generic OCPI v2.0 HTTP API.
    /// </summary>
    public class GenericAPI
    {

        #region Data

        private static readonly Random         _Random                         = new Random();

        protected internal const String        __DefaultHTTPRoot               = "org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.GenericAPI.HTTPRoot";

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

        public static readonly HTTPURI         DefaultURIPrefix                = HTTPURI.Parse("/ext/OCPI");

        #endregion

        #region Properties

        #region RoamingNetwork

        private readonly RoamingNetwork _RoamingNetwork;

        public RoamingNetwork RoamingNetwork
        {
            get
            {
                return _RoamingNetwork;
            }
        }

        #endregion


        #region HTTPServer

        protected readonly HTTPServer<RoamingNetworks, RoamingNetwork> _HTTPServer;

        /// <summary>
        /// The HTTP server of the OCPI API.
        /// </summary>
        public HTTPServer<RoamingNetworks, RoamingNetwork> HTTPServer
        {
            get
            {
                return _HTTPServer;
            }
        }

        #endregion

        #region URIPrefix

        /// <summary>
        /// A common URI prefix for all URIs within this API.
        /// </summary>
        public HTTPURI URIPrefix { get; }

        #endregion


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

        #region APIEMailAddress

        private readonly EMailAddress _APIEMailAddress;

        /// <summary>
        /// A sender e-mail address for the Open Data API.
        /// </summary>
        public EMailAddress APIEMailAddress
        {
            get
            {
                return _APIEMailAddress;
            }
        }

        #endregion

        #region APIPublicKeyRing

        private readonly PgpPublicKeyRing _APIPublicKeyRing;

        /// <summary>
        /// The PGP/GPG public key ring of the Open Data API.
        /// </summary>
        public PgpPublicKeyRing APIPublicKeyRing
        {
            get
            {
                return _APIPublicKeyRing;
            }
        }

        #endregion

        #region APISecretKeyRing

        private readonly PgpSecretKeyRing _APISecretKeyRing;

        /// <summary>
        /// The PGP/GPG secret key ring of the Open Data API.
        /// </summary>
        public PgpSecretKeyRing APISecretKeyRing
        {
            get
            {
                return _APISecretKeyRing;
            }
        }

        #endregion

        #region APIPassphrase

        private readonly String _APIPassphrase;

        /// <summary>
        /// The passphrase of the PGP/GPG secret key of the Open Data API.
        /// </summary>
        public String APIPassphrase
        {
            get
            {
                return _APIPassphrase;
            }
        }

        #endregion

        #region APIAdminEMail

        private readonly EMailAddressList _APIAdminEMail;

        /// <summary>
        /// The E-Mail Addresses of the service admins.
        /// </summary>
        public EMailAddressList APIAdminEMail
        {
            get
            {
                return _APIAdminEMail;
            }
        }

        #endregion

        #region APISMTPClient

        private readonly SMTPClient _APISMTPClient;

        /// <summary>
        /// A SMTP client to be used by the Open Data API.
        /// </summary>
        public SMTPClient APISMTPClient
        {
            get
            {
                return _APISMTPClient;
            }
        }

        #endregion


        #region DNSClient

        protected readonly DNSClient _DNSClient = null;

        /// <summary>
        /// The DNS resolver to use.
        /// </summary>
        public DNSClient DNSClient
        {
            get
            {
                return _DNSClient;
            }
        }

        #endregion

        #endregion

        #region Events

        #region Generic HTTP/SOAP server logging

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

        #region GenericAPI(HTTPServerName, ...)

        internal GenericAPI(RoamingNetwork        RoamingNetwork,
                            String                HTTPServerName          = DefaultHTTPServerName,
                            IPPort?               HTTPServerPort          = null,
                            HTTPURI?              URIPrefix               = null,
                            Func<String, Stream>  GetRessources           = null,

                            String                ServiceName             = DefaultHTTPServerName,
                            EMailAddress          APIEMailAddress         = null,
                            PgpPublicKeyRing      APIPublicKeyRing        = null,
                            PgpSecretKeyRing      APISecretKeyRing        = null,
                            String                APIPassphrase           = null,
                            EMailAddressList      APIAdminEMail           = null,
                            SMTPClient            APISMTPClient           = null,

                            DNSClient             DNSClient               = null,
                            String                LogfileName             = DefaultLogfileName)

            : this(RoamingNetwork,
                   new HTTPServer<RoamingNetworks, RoamingNetwork>(DefaultServerName: DefaultHTTPServerName),
                   URIPrefix,
                   GetRessources,

                   ServiceName,
                   APIEMailAddress,
                   APIPublicKeyRing,
                   APISecretKeyRing,
                   APIPassphrase,
                   APIAdminEMail,
                   APISMTPClient,

                   LogfileName)

        {

            HTTPServer.AttachTCPPorts(HTTPServerPort ?? DefaultHTTPServerPort);

        }

        #endregion

        #region GenericAPI(HTTPServer, ...)

        /// <summary>
        /// Initialize the OCPI HTTP server using IPAddress.Any, http port 8080 and maybe start the server.
        /// </summary>
        internal GenericAPI(RoamingNetwork                               RoamingNetwork,
                            HTTPServer<RoamingNetworks, RoamingNetwork>  HTTPServer,
                            HTTPURI?                                     URIPrefix               = null,
                            Func<String, Stream>                         GetRessources           = null,

                            String                                       ServiceName             = DefaultHTTPServerName,
                            EMailAddress                                 APIEMailAddress         = null,
                            PgpPublicKeyRing                             APIPublicKeyRing        = null,
                            PgpSecretKeyRing                             APISecretKeyRing        = null,
                            String                                       APIPassphrase           = null,
                            EMailAddressList                             APIAdminEMail           = null,
                            SMTPClient                                   APISMTPClient           = null,

                            String                                       LogfileName             = DefaultLogfileName)

        {

            #region Initial checks

            if (RoamingNetwork == null)
                throw new ArgumentNullException("RoamingNetwork", "The given parameter must not be null!");

            if (HTTPServer == null)
                throw new ArgumentNullException("HTTPServer", "The given parameter must not be null!");

            #endregion

            #region Init data

            this._HTTPServer              = HTTPServer;
            this._GetRessources           = GetRessources;
            this.URIPrefix                = URIPrefix ?? DefaultURIPrefix;

            this._ServiceName             = ServiceName;
            this._APIEMailAddress         = APIEMailAddress;
            this._APIPublicKeyRing        = APIPublicKeyRing;
            this._APISecretKeyRing        = APISecretKeyRing;
            this._APIPassphrase           = APIPassphrase;
            this._APIAdminEMail           = APIAdminEMail;
            this._APISMTPClient           = APISMTPClient;

            this._DNSClient               = HTTPServer.DNSClient;

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

            _HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
                                                URIPrefix + "/", "org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.GenericAPI.HTTPRoot",
                                                Assembly.GetCallingAssembly());

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new HTTPURI[] {
                                              URIPrefix + "/index.html",
                                              URIPrefix + "/"
                                          },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

                                              var _MemoryStream = new MemoryStream();
                                              typeof(GenericAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.GenericAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
                                              typeof(GenericAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.GenericAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = _MemoryStream.ToArray(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

        }

        #endregion


        #region Start()

        public void Start()
        {

            lock (_HTTPServer)
            {

                if (!_HTTPServer.IsStarted)
                    _HTTPServer.Start();

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

            lock (_HTTPServer)
            {

                _HTTPServer.Shutdown(Message, Wait);

                //SendCompleted(this, DateTime.Now, Message);

            }

        }

        #endregion

    }

}
