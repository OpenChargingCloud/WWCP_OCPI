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

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Bcpg.OpenPgp;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.BouncyCastle;
using org.GraphDefined.WWCP;

#endregion

namespace org.GraphDefined.WWCP.OCPIv2_1.HTTP
{

    /// <summary>
    /// The OCPI HTTP API for e-Mobility Service Providers.
    /// </summary>
    public class EMSPAPI : GenericAPI
    {

        #region Data

        private static readonly Random  _Random                = new Random();

        public  const           String  DefaultHTTPServerName  = "GraphDefined OCPI EMSP HTTP API v0.1";
        public  static readonly IPPort  DefaultHTTPServerPort  = new IPPort(8080);

        public  const           String  LogfileName            = "OICP_EMSP_HTTPAPI.log";

        #endregion

        #region Constructor(s)

        #region EMSPAPI(HTTPServerName, ...)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for e-Mobility Service Providers
        /// using a newly created HTTP server.
        /// </summary>
        public EMSPAPI(RoamingNetwork    RoamingNetwork,
                       String            HTTPServerName    = DefaultHTTPServerName,
                       IPPort            HTTPServerPort    = null,
                       String            URIPrefix         = "",

                       String            ServiceName       = DefaultHTTPServerName,
                       EMailAddress      APIEMailAddress   = null,
                       PgpSecretKeyRing  APISecretKeyRing  = null,
                       String            APIPassphrase     = null,
                       EMailAddressList  APIAdminEMail     = null,
                       SMTPClient        APISMTPClient     = null,

                       DNSClient         DNSClient         = null,
                       String            LogfileName       = DefaultLogfileName)

            : base(RoamingNetwork,
                   HTTPServerName,
                   HTTPServerPort != null ? HTTPServerPort : DefaultHTTPServerPort,
                   URIPrefix,
                   ResourceName => typeof(EMSPAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.EMSPAPI.HTTPRoot." + ResourceName),

                   ServiceName,
                   APIEMailAddress,
                   null,//OpenPGP.ReadPublicKeyRing(typeof(EMSPAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.GenericAPI.HTTPRoot.robot@offenes-jena_pubring.gpg")),
                   APISecretKeyRing,
                   APIPassphrase,
                   APIAdminEMail,
                   APISMTPClient,

                   DNSClient,
                   LogfileName)

        {

            RegisterEMSPURITemplates();

        }

        #endregion

        #region EMSPAPI(HTTPServer, ...)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for e-Mobility Service Providers
        /// using the given HTTP server.
        /// </summary>
        public EMSPAPI(RoamingNetwork                               RoamingNetwork,
                       HTTPServer<RoamingNetworks, RoamingNetwork>  HTTPServer,
                       String                                       URIPrefix         = "",

                       String                                       ServiceName       = DefaultHTTPServerName,
                       EMailAddress                                 APIEMailAddress   = null,
                       PgpSecretKeyRing                             APISecretKeyRing  = null,
                       String                                       APIPassphrase     = null,
                       EMailAddressList                             APIAdminEMail     = null,
                       SMTPClient                                   APISMTPClient     = null,

                       DNSClient                                    DNSClient         = null)

            : base(RoamingNetwork,
                   HTTPServer,
                   URIPrefix,
                   ResourceName => typeof(EMSPAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.EMSPAPI.HTTPRoot." + ResourceName),

                   ServiceName,
                   APIEMailAddress,
                   OpenPGP.ReadPublicKeyRing(typeof(EMSPAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.EMSPAPI.HTTPRoot.About.robot@offenes-jena_pubring.gpg")),
                   APISecretKeyRing,
                   APIPassphrase,
                   APIAdminEMail,
                   APISMTPClient,

                   LogfileName)

        {

            RegisterEMSPURITemplates();

        }

        #endregion

        #endregion


        #region (private) RegisterEMSPURITemplates()

        private void RegisterEMSPURITemplates()
        {

            #region /emsp

            _HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
                                                URIPrefix + "/emsp", "org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.EMSPAPI.HTTPRoot",
                                                Assembly.GetCallingAssembly());

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new String[] { URIPrefix + "/emsp/index.html",
                                                         URIPrefix + "/emsp/" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

                                              var _MemoryStream = new MemoryStream();
                                              typeof(EMSPAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.EMSPAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
                                              typeof(EMSPAPI).Assembly.GetManifestResourceStream("org.GraphDefined.WWCP.OCPIv2_1.HTTPAPI.EMSPAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = _MemoryStream.ToArray(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

            #region /emsp/versions

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URIPrefix + "/emsp/versions",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = new JArray(new JObject(
                                                                                   new JProperty("version",  "2.0"),
                                                                                   new JProperty("url",      "http://" + Request.Host + "/emsp/versions/2.0/")
                                                                    )).ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

            #region /emsp/versions/2.0/

            _HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          URIPrefix + "/emsp/versions/2.0/",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  Server          = DefaultHTTPServerName,
                                                  Date            = DateTime.Now,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = new JObject(
                                                                        new JProperty("version",  "2.0"),
                                                                        new JProperty("endpoints", new JArray(
                                                                            new JObject(
                                                                                new JProperty("identifier", "credentials"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/emsp/versions/2.0/credentials/")
                                                                            ),
                                                                            new JObject(
                                                                                new JProperty("identifier", "locations"),
                                                                                new JProperty("url",        "http://" + Request.Host + "/emsp/versions/2.0/locations/")
                                                                            )
                                                                    ))).ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });

            #endregion

        }

        #endregion


    }

}
