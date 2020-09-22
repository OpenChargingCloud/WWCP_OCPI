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
    /// The OCPI HTTP API for e-Mobility Service Providers.
    /// </summary>
    public class EMSPAPI : CommonAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI EMSP HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI EMSP HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("/emsp");

        #endregion

        #region Constructor(s)

        #region EMSPAPI(HTTPServerName = default, ...)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for e-Mobility Service Providers
        /// using a newly created HTTP server.
        /// </summary>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="HTTPServerPort">An optional HTTP TCP port.</param>
        /// <param name="HTTPServerName">An optional HTTP server name.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional HTTP URL path prefix.</param>
        /// <param name="ServiceName">An optional HTTP service name.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        public EMSPAPI(String          HTTPServerName    = DefaultHTTPServerName,
                       HTTPHostname?   HTTPHostname      = null,
                       IPPort?         HTTPServerPort    = null,
                       String          ExternalDNSName   = null,
                       HTTPPath?       URLPathPrefix     = null,
                       String          ServiceName       = DefaultHTTPServerName,
                       DNSClient       DNSClient         = null)

            : base(HTTPHostname   ?? org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPHostname.Any,
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

        #region EMSPAPI(HTTPServer, ...)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for e-Mobility Service Providers
        /// using the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="ServiceName">An optional name of the HTTP API service.</param>
        public EMSPAPI(HTTPServer      HTTPServer,
                       HTTPHostname?   HTTPHostname      = null,
                       String          ExternalDNSName   = null,
                       HTTPPath?       URLPathPrefix     = null,
                       String          ServiceName       = DefaultHTTPServerName)

            : base(HTTPServer,
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

            #region GET    [/emsp] == /

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPathPrefix + "/emsp", "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.EMSPAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPathPrefix + "/emsp/index.html",
            //                                 URLPathPrefix + "/emsp/"
            //                             },
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(EMSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.EMSPAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(EMSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.EMSPAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     Server          = DefaultHTTPServerName,
            //                                     Date            = DateTime.UtcNow,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = "close"
            //                                 };

            //                             });

            #endregion

            #region GET    [/emsp]/versions

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "/emsp/versions",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ContentType     = HTTPContentType.HTML_UTF8,
                                                 Content         = new JArray(new JObject(
                                                                                  new JProperty("version",  "2.2"),
                                                                                  new JProperty("url",      "http://" + Request.Host + URLPathPrefix + "/versions/2.2/")
                                                                   )).ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion

            #region GET    [/emsp]/versions/2.2/

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2/",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ContentType     = HTTPContentType.HTML_UTF8,
                                                 Content         = new JObject(
                                                                       new JProperty("version",  "2.2"),
                                                                       new JProperty("endpoints", new JArray(
                                                                           new JObject(
                                                                               new JProperty("identifier", "credentials"),
                                                                               new JProperty("url",        "http://" + Request.Host + URLPathPrefix + "versions/2.2/credentials/")
                                                                           ),
                                                                           new JObject(
                                                                               new JProperty("identifier", "locations"),
                                                                               new JProperty("url",        "http://" + Request.Host + URLPathPrefix + "versions/2.2/locations/")
                                                                           )
                                                                   ))).ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion


            // Receiver Interface for eMSPs and NSPs

            #region [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}

            #region GET    [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check LocationId URI parameter

                                             if (!Request.ParseLocation(this,
                                                                        out CountryCode?  CountryCode,
                                                                        out Party_Id?     PartyId,
                                                                        out Location_Id?  LocationId,
                                                                        out Location      Location,
                                                                        out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             var JSON = Location.ToJSON();

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ContentType     = HTTPContentType.JSON_UTF8,
                                                 Content         = JSON.ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion

            #region PUT    [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check LocationId URI parameter

                                             if (!Request.ParseLocationId(this,
                                                                          out CountryCode?  CountryCode,
                                                                          out Party_Id?     PartyId,
                                                                          out Location_Id?  LocationId,
                                                                          out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return HTTPResponse;

                                             if (!OCPIv2_2.Location.TryParse(JSONObj,
                                                                             //_Organizations.TryGetValue,
                                                                             //_Communicators.TryGetValue,
                                                                             out Location  _Location,
                                                                             out String    ErrorResponse,
                                                                             LocationId))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ETag                       = "1",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = JSONObject.Create(
                                                                                             new JProperty("description", ErrorResponse)
                                                                                         ).ToUTF8Bytes()
                                                        }.AsImmutable;

                                             }

                                             #endregion



                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ContentType     = HTTPContentType.JSON_UTF8,
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

            #region PATCH  [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PATCH,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
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

            #endregion

            #region [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}

            #region GET    [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check LocationId URI parameter

                                             if (!Request.ParseLocationEVSE(this,
                                                                            out CountryCode?  CountryCode,
                                                                            out Party_Id?     PartyId,
                                                                            out Location_Id?  LocationId,
                                                                            out Location      Location,
                                                                            out EVSE_UId?     EVSEId,
                                                                            out EVSE          EVSE,
                                                                            out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             var JSON = EVSE.ToJSON();


                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ContentType     = HTTPContentType.JSON_UTF8,
                                                 Content         = JSON.ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion

            #region PUT    [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {


                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
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

            #region PATCH  [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PATCH,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {


                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
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

            #endregion

            #region [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            #region GET    [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check LocationId URI parameter

                                             if (!Request.ParseLocationEVSEConnector(this,
                                                                                     out CountryCode?   CountryCode,
                                                                                     out Party_Id?      PartyId,
                                                                                     out Location_Id?   LocationId,
                                                                                     out Location       Location,
                                                                                     out EVSE_UId?      EVSEId,
                                                                                     out EVSE           EVSE,
                                                                                     out Connector_Id?  ConnectorId,
                                                                                     out Connector      Connector,
                                                                                     out HTTPResponse   HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             var JSON = Connector.ToJSON();


                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ContentType     = HTTPContentType.JSON_UTF8,
                                                 Content         = JSON.ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion

            #region PUT    [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {


                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
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

            #region PATCH  [/emsp]/versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PATCH,
                                         URLPathPrefix + "versions/2.2/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {


                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
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

            #endregion

        }

        #endregion


    }

}
