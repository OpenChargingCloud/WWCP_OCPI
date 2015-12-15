/*
 * Copyright (c) 2015 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
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

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using Org.BouncyCastle.Bcpg.OpenPgp;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.WWCP;

#endregion

namespace org.GraphDefined.WsWCP.OCPI_2_0.HTTP
{

    /// <summary>
    /// The generic OCPI HTTP API.
    /// </summary>
    public class GenericAPI
    {

        #region Data

        private static readonly Random          _Random                         = new Random();

        private const           String          DefaultHTTPServerName           = "GraphDefined OCPI HTTP API v0.1";
        protected internal const String         __DefaultHTTPRoot               = "org.GraphDefined.WWCP.OCPI_2_0.HTTPAPI.GenericAPI.HTTPRoot";
        private static readonly IPPort          DefaultHTTPServerPort           = new IPPort(8080);

        private readonly Func<String, Stream> _GetRessources;

        public  const           String          DefaultLogfileName              = "OCPI_HTTPAPI.log";

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

        protected readonly HTTPServer _HTTPServer;

        /// <summary>
        /// The HTTP server of the Open Data API.
        /// </summary>
        public HTTPServer HTTPServer
        {
            get
            {
                return _HTTPServer;
            }
        }

        #endregion

        #region URIPrefix

        private readonly String _URIPrefix;

        /// <summary>
        /// A common URI prefix for all URIs within this API.
        /// </summary>
        public String URIPrefix
        {
            get
            {
                return _URIPrefix;
            }
        }

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

        #region Constructor(s)

        #region GenericAPI(HTTPServerName, ...)

        internal GenericAPI(RoamingNetwork        RoamingNetwork,
                            String                HTTPServerName          = DefaultHTTPServerName,
                            IPPort                HTTPServerPort          = null,
                            String                URIPrefix               = "",
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
                   new HTTPServer(DefaultServerName: DefaultHTTPServerName).AttachTCPPorts(HTTPServerPort != null ? HTTPServerPort : DefaultHTTPServerPort),
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

        }

        #endregion

        #region GenericAPI(HTTPServer, ...)

        /// <summary>
        /// Initialize the OCPI HTTP server using IPAddress.Any, http port 8080 and maybe start the server.
        /// </summary>
        internal GenericAPI(RoamingNetwork        RoamingNetwork,
                            HTTPServer            HTTPServer,
                            String                URIPrefix               = "/ext/OCPI",
                            Func<String, Stream>  GetRessources           = null,

                            String                ServiceName             = DefaultHTTPServerName,
                            EMailAddress          APIEMailAddress         = null,
                            PgpPublicKeyRing      APIPublicKeyRing        = null,
                            PgpSecretKeyRing      APISecretKeyRing        = null,
                            String                APIPassphrase           = null,
                            EMailAddressList      APIAdminEMail           = null,
                            SMTPClient            APISMTPClient           = null,

                            String                LogfileName             = DefaultLogfileName)

        {

            #region Initial checks

            if (RoamingNetwork == null)
                throw new ArgumentNullException("RoamingNetwork", "The given parameter must not be null!");

            if (HTTPServer == null)
                throw new ArgumentNullException("HTTPServer", "The given parameter must not be null!");

            if (URIPrefix.IsNullOrEmpty())
                throw new ArgumentNullException("URIPrefix", "The given parameter must not be null or empty!");

            if (!URIPrefix.StartsWith("/"))
                URIPrefix = "/" + URIPrefix;

            #endregion

            #region Init data

            this._HTTPServer              = HTTPServer;
            this._GetRessources           = GetRessources;
            this._URIPrefix               = URIPrefix;

            this._ServiceName             = ServiceName;
            this._APIEMailAddress         = APIEMailAddress;
            this._APIPublicKeyRing        = APIPublicKeyRing;
            this._APISecretKeyRing        = APISecretKeyRing;
            this._APIPassphrase           = APIPassphrase;
            this._APIAdminEMail           = APIAdminEMail;
            this._APISMTPClient           = APISMTPClient;

            this._DNSClient               = HTTPServer.DNSClient;

            #endregion

            RegisterURITemplates();

            #region Configure HTTP Server events

            // Server events...
            _HTTPServer.OnStarted           += (Sender,    Timestamp, Message)                                    => Console.WriteLine("[" + Timestamp + "] '" + (Sender as HTTPServer).DefaultServerName + "' started on port(s) " + (Sender as HTTPServer).Select(tcpserver => tcpserver.Port).AggregateWith(", ") + (Message.IsNotNullOrEmpty() ? "; msg: '" + Message + "'..." : ""));
            //_HTTPServer.OnNewConnection     += (TCPServer, Timestamp, RemoteSocket, ConnectionId, TCPConnection)  => Console.WriteLine("[" + Timestamp + "] New TCP/HTTP connection from " + TCPConnection.RemoteSocket.ToString());
            _HTTPServer.OnExceptionOccured  += (Sender,    Timestamp, Exception)                                  => Console.WriteLine("[" + Timestamp + "] HTTP exception occured: '" + Exception.Message + "'");
            //_HTTPServer.OnConnectionClosed  += (Sender,    Timestamp, RemoteSocket, ConnectionId, ClosedBy)       => Console.WriteLine("[" + Timestamp + "] TCP/HTTP connection from " + RemoteSocket.ToString() + " closed by " + ClosedBy.ToString().ToLower() + "!");
            _HTTPServer.OnCompleted         += (Sender,    Timestamp, Message)                                    => Console.WriteLine("[" + Timestamp + "] '" + (Sender as HTTPServer).DefaultServerName + "' shutdown" + (Message.IsNotNullOrEmpty() ? "; msg: '" + Message + "'..." : "..."));

            // HTTP events...
            //_HTTPServer.RequestLog          += (Sender, Timestamp, Request)            => Console.WriteLine("[" + Timestamp + "] " + Request.HTTPMethod + " " + Request.URI);
            //_HTTPServer.AccessLog           += (Sender, Timestamp, Request, Response)  => Console.WriteLine("[" + Timestamp + "] " + Request.HTTPMethod + " " + Request.URI + " => " + Response.HTTPStatusCode.SimpleString);


            //_HTTPServer.AccessLog += (_HTTPServer, ServerTimestamp, Request, Response) => {
            //
            //    Console.WriteLine("[" + ServerTimestamp.ToString() + "] " +
            //                      (Request.X_Forwarded_For != null
            //                         ? Request.X_Forwarded_For + "(" +  Request.RemoteSocket + ") - "
            //                         : Request.RemoteSocket + " - ") +
            //                      Request.HTTPMethod   + " " +
            //                      Request.URI          + " " +
            //                      Response.HTTPStatusCode + " " +
            //                      Response.ContentLength + " bytes");
            //
            //};

            _HTTPServer.ErrorLog += (_HTTPServer, ServerTimestamp, Request, Response, Error, LastException) => {

                var _error            = (Error         == null) ? "" : Error;
                var _exceptionMessage = (LastException == null) ? "" : Environment.NewLine + LastException.Message;

                Console.Write("[" + ServerTimestamp.ToString() + "] " +
                              (Request.X_Forwarded_For != null
                                  ? Request.X_Forwarded_For + "(" + Request.RemoteSocket + ") - "
                                  : Request.RemoteSocket + " - ") +
                              Request.HTTPMethod + " " +
                              Request.URI        + " => ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[" + Response.HTTPStatusCode + "] ");
                Console.ResetColor();
                Console.WriteLine((_error.           IsNotNullOrEmpty() ? _error  + "/"     : "" ) +
                                  (_exceptionMessage.IsNotNullOrEmpty() ? _exceptionMessage : ""));

            };

            #endregion

        }

        #endregion

        #endregion


        #region (private) RegisterURITemplates()

        private void RegisterURITemplates()
        {

            #region / (HTTPRoot)

            _HTTPServer.RegisterResourcesFolder(URIPrefix + "/", "org.GraphDefined.WWCP.OCPI_2_0.HTTPAPI.GenericAPI.HTTPRoot", Assembly.GetCallingAssembly());

            _HTTPServer.AddMethodCallback(HTTPMethod.GET,
                                          new String[] { URIPrefix + "/index.html",
                                                         URIPrefix + "/" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: HTTPRequest => {

                                              var _MemoryStream = new MemoryStream();
                                              typeof(GenericAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPI_2_0.HTTPAPI.GenericAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
                                              typeof(GenericAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPI_2_0.HTTPAPI.GenericAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

                                              return new HTTPResponseBuilder() {
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
