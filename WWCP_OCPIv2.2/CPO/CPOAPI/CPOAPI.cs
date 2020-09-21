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

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Bcpg.OpenPgp;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.BouncyCastle;
using cloud.charging.open.protocols;
using org.GraphDefined.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// The HTTP API for charge point operators.
    /// </summary>
    public class CPOAPI : CommonAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI CPO HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI CPO HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("cpo/");

        #endregion

        #region Constructor(s)

        #region CPOAPI(RoamingNetwork, HTTPServerName = default, ...)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for Charge Point Operators
        /// using a newly created HTTP server.
        /// </summary>
        /// <param name="RoamingNetwork">The attached roaming network.</param>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="HTTPServerPort">An optional HTTP TCP port.</param>
        /// <param name="HTTPServerName">An optional HTTP server name.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional HTTP URL path prefix.</param>
        /// <param name="ServiceName">An optional HTTP service name.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        public CPOAPI(RoamingNetwork  RoamingNetwork,
                      String          HTTPServerName    = DefaultHTTPServerName,
                      HTTPHostname?   HTTPHostname      = null,
                      IPPort?         HTTPServerPort    = null,
                      String          ExternalDNSName   = null,
                      HTTPPath?       URLPathPrefix     = null,
                      String          ServiceName       = DefaultHTTPServerName,
                      DNSClient       DNSClient         = null)

            : base(RoamingNetwork,
                   HTTPHostname   ?? org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPHostname.Any,
                   HTTPServerPort ?? DefaultHTTPServerPort,
                   HTTPServerName ?? DefaultHTTPServerName,
                   ExternalDNSName,
                   URLPathPrefix  ?? DefaultURLPathPrefix,
                   ServiceName,
                   DNSClient)

        {

            RegisterURLTemplates();

        }

        #endregion

        #region CPOAPI(RoamingNetwork, HTTPServer, ...)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for Charge Point Operators
        /// using the given HTTP server.
        /// </summary>
        /// <param name="RoamingNetwork">The attached roaming network.</param>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="ServiceName">An optional name of the HTTP API service.</param>
        public CPOAPI(RoamingNetwork  RoamingNetwork,
                      HTTPServer      HTTPServer,
                      HTTPHostname?   HTTPHostname      = null,
                      String          ExternalDNSName   = null,
                      HTTPPath?       URLPathPrefix     = null,
                      String          ServiceName       = DefaultHTTPServerName)

            : base(RoamingNetwork,
                   HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   URLPathPrefix ?? DefaultURLPathPrefix,
                   ServiceName)

        {

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region GET    [/cpo] == /

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPathPrefix + "/cpo", "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPathPrefix + "/cpo/index.html",
            //                                 URLPathPrefix + "/cpo/"
            //                             },
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     Server          = DefaultHTTPServerName,
            //                                     Date            = DateTime.Now,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = "close"
            //                                 };

            //                             });

            #endregion

            #region GET    [/cpo]/versions

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.Now,
                                                 ContentType     = HTTPContentType.HTML_UTF8,
                                                 Content         = new JArray(new JObject(
                                                                                  new JProperty("version",  "2.2"),
                                                                                  new JProperty("url",      "http://" + Request.Host + URLPathPrefix + "/versions/2.2/")
                                                                   )).ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion

            #region GET    [/cpo]/versions/2.2/

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.Now,
                                                 ContentType     = HTTPContentType.HTML_UTF8,
                                                 Content         = JSONObject.Create(
                                                                       new JProperty("version",  "2.2"),
                                                                       new JProperty("endpoints", new JArray(
                                                                           new JObject(
                                                                               new JProperty("identifier", "credentials"),
                                                                               new JProperty("url",        "http://" + Request.Host + URLPathPrefix + "/versions/2.2/credentials/")
                                                                           ),
                                                                           new JObject(
                                                                               new JProperty("identifier", "locations"),
                                                                               new JProperty("url",        "http://" + Request.Host + URLPathPrefix + "/versions/2.2/locations/")
                                                                           )
                                                                   ))).ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion



            #region GET    [/cpo]/versions/2.2/locations

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2/locations",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             var from                         = Request.QueryString.GetDateTime("date_from");
                                             var to                           = Request.QueryString.GetDateTime("date_to");
                                             var offset                       = Request.QueryString.GetUInt64("offset");
                                             var limit                        = Request.QueryString.GetUInt64("limit");



                                             // X-Total-Count    The total number of objects available in the server system that match the given query (including the given query parameters.
                                             // X-Limit          The maximum number of objects that the server WILL return.

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.Now,
                                                 ContentType     = HTTPContentType.HTML_UTF8,
                                                 Content         = JSONObject.Create(
                                                                       new JProperty("version",  "2.2"),
                                                                       new JProperty("endpoints", new JArray(
                                                                           new JObject(
                                                                               new JProperty("identifier", "credentials"),
                                                                               new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                           ),
                                                                           new JObject(
                                                                               new JProperty("identifier", "locations"),
                                                                               new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
                                                                           )
                                                                   ))).ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion

            #region GET    [/cpo]/versions/2.2/locations/{locationId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2/locations/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.Now,
                                                 ContentType     = HTTPContentType.HTML_UTF8,
                                                 Content         = JSONObject.Create(
                                                                       new JProperty("version",  "2.2"),
                                                                       new JProperty("endpoints", new JArray(
                                                                           new JObject(
                                                                               new JProperty("identifier", "credentials"),
                                                                               new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                           ),
                                                                           new JObject(
                                                                               new JProperty("identifier", "locations"),
                                                                               new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
                                                                           )
                                                                   ))).ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion

            #region GET    [/cpo]/versions/2.2/locations/{locationId}/{evseId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2/locations/{locationId}/{evseId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {


                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.Now,
                                                 ContentType     = HTTPContentType.HTML_UTF8,
                                                 Content         = JSONObject.Create(
                                                                       new JProperty("version",  "2.2"),
                                                                       new JProperty("endpoints", new JArray(
                                                                           new JObject(
                                                                               new JProperty("identifier", "credentials"),
                                                                               new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                           ),
                                                                           new JObject(
                                                                               new JProperty("identifier", "locations"),
                                                                               new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
                                                                           )
                                                                   ))).ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion

            #region GET    [/cpo]/versions/2.2/locations/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2/locations/{locationId}/{evseId}/{connectorId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {


                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.Now,
                                                 ContentType     = HTTPContentType.HTML_UTF8,
                                                 Content         = JSONObject.Create(
                                                                       new JProperty("version",  "2.2"),
                                                                       new JProperty("endpoints", new JArray(
                                                                           new JObject(
                                                                               new JProperty("identifier", "credentials"),
                                                                               new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/credentials/")
                                                                           ),
                                                                           new JObject(
                                                                               new JProperty("identifier", "locations"),
                                                                               new JProperty("url",        "http://" + Request.Host + "/cpo/versions/2.2/locations/")
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
