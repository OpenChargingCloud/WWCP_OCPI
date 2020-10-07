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
    /// Extention methods for the Common API.
    /// </summary>
    public static class EMSPAPIExtentions
    {

        #region ParseLocationId             (this HTTPRequest, EMSPAPI, out CountryCode, out PartyId, out LocationId,                                                                      out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="EMSPAPI">The Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationId(this HTTPRequest  HTTPRequest,
                                              EMSPAPI         EMSPAPI,
                                              out CountryCode?  CountryCode,
                                              out Party_Id?     PartyId,
                                              out Location_Id?  LocationId,
                                              out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (EMSPAPI    == null)
                throw new ArgumentNullException(nameof(EMSPAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            LocationId    = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[2]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseLocation               (this HTTPRequest, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocation(this HTTPRequest  HTTPRequest,
                                            EMSPAPI         EMSPAPI,
                                            out CountryCode?  CountryCode,
                                            out Party_Id?     PartyId,
                                            out Location_Id?  LocationId,
                                            out Location      Location,
                                            out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (EMSPAPI    == null)
                throw new ArgumentNullException(nameof(EMSPAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            LocationId    = null;
            Location      = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 3) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[2]);

            if (!LocationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseLocationEVSEId         (this HTTPRequest, EMSPAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,                                           out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="EMSPAPI">The Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEId(this HTTPRequest  HTTPRequest,
                                                  EMSPAPI         EMSPAPI,
                                                  out CountryCode?  CountryCode,
                                                  out Party_Id?     PartyId,
                                                  out Location_Id?  LocationId,
                                                  out EVSE_UId?     EVSEUId,
                                                  out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (EMSPAPI    == null)
                throw new ArgumentNullException(nameof(EMSPAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            LocationId    = null;
            EVSEUId       = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 4)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseLocationEVSE           (this HTTPRequest, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSE(this HTTPRequest  HTTPRequest,
                                                EMSPAPI         EMSPAPI,
                                                out CountryCode?  CountryCode,
                                                out Party_Id?     PartyId,
                                                out Location_Id?  LocationId,
                                                out Location      Location,
                                                out EVSE_UId?     EVSEUId,
                                                out EVSE          EVSE,
                                                out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (EMSPAPI    == null)
                throw new ArgumentNullException(nameof(EMSPAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            LocationId    = null;
            Location      = null;
            EVSEUId       = null;
            EVSE          = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 4) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!LocationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value,
                                                  PartyId.    Value,
                                                  LocationId. Value, out Location)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseLocationEVSEConnectorId(this HTTPRequest, EMSPAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,           out ConnectorId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="EMSPAPI">The Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnectorId(this HTTPRequest   HTTPRequest,
                                                           EMSPAPI          EMSPAPI,
                                                           out CountryCode?   CountryCode,
                                                           out Party_Id?      PartyId,
                                                           out Location_Id?   LocationId,
                                                           out EVSE_UId?      EVSEUId,
                                                           out Connector_Id?  ConnectorId,
                                                           out HTTPResponse   HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (EMSPAPI    == null)
                throw new ArgumentNullException(nameof(EMSPAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            LocationId    = null;
            EVSEUId       = null;
            ConnectorId   = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 5)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            ConnectorId = Connector_Id.TryParse(HTTPRequest.ParsedURLParameters[2]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid connector identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseLocationEVSEConnector  (this HTTPRequest, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnector(this HTTPRequest   HTTPRequest,
                                                         EMSPAPI          EMSPAPI,
                                                         out CountryCode?   CountryCode,
                                                         out Party_Id?      PartyId,
                                                         out Location_Id?   LocationId,
                                                         out Location       Location,
                                                         out EVSE_UId?      EVSEUId,
                                                         out EVSE           EVSE,
                                                         out Connector_Id?  ConnectorId,
                                                         out Connector      Connector,
                                                         out HTTPResponse   HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (EMSPAPI    == null)
                throw new ArgumentNullException(nameof(EMSPAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            LocationId    = null;
            Location      = null;
            EVSEUId       = null;
            EVSE          = null;
            ConnectorId   = null;
            Connector     = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 5) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!LocationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(HTTPRequest.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            ConnectorId = Connector_Id.TryParse(HTTPRequest.ParsedURLParameters[2]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid connector identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!EVSE.TryGetConnector(ConnectorId.Value, out Connector)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown connector identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The OCPI HTTP API for e-Mobility Service Providers.
    /// </summary>
    public class EMSPAPI : HTTPAPI
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

        #region Properties

        /// <summary>
        /// The OCPI common API.
        /// </summary>
        public CommonAPI CommonAPI { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the OCPI HTTP API for e-Mobility Service Providers
        /// using the given HTTP server.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="ServiceName">An optional name of the HTTP API service.</param>
        public EMSPAPI(CommonAPI       CommonAPI,
                       HTTPHostname?   HTTPHostname      = null,
                       String          ExternalDNSName   = null,
                       HTTPPath?       URLPathPrefix     = null,
                       String          ServiceName       = DefaultHTTPServerName)

            : base(CommonAPI.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   URLPathPrefix ?? DefaultURLPathPrefix,
                   ServiceName)

        {

            this.CommonAPI = CommonAPI ?? throw new ArgumentNullException(nameof(CommonAPI), "The given OCPI common API must not be null!");

            RegisterURLTemplates();

        }

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


            // Receiver Interface for eMSPs and NSPs

            #region ~/locations/{country_code}/{party_id}/{locationId}

            #region GET    ~/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
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

                                             var JSON = Location.ToJSON().
                                                                 CreateResponse(1000,
                                                                                "",
                                                                                DateTime.UtcNow);

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

            #region PUT    ~/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
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

                                             if (!Location.TryParse(JSONObj,
                                                                    out Location  _Location,
                                                                    out String    ErrorResponse,
                                                                    CountryCode,
                                                                    PartyId,
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


                                             var JSON = _Location.ToJSON().
                                                                  CreateResponse(1000,
                                                                                 "",
                                                                                 DateTime.UtcNow);


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

            #region PATCH  ~/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PATCH,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
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

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            #region GET    ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
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

                                             var JSON = EVSE.ToJSON().
                                                             CreateResponse(1000,
                                                                            "",
                                                                            DateTime.UtcNow);


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

            #region PUT    ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check LocationId URI parameter

                                             if (!Request.ParseLocationEVSEId(this,
                                                                              out CountryCode?  CountryCode,
                                                                              out Party_Id?     PartyId,
                                                                              out Location_Id?  LocationId,
                                                                              out EVSE_UId?     EVSEUId,
                                                                              out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return HTTPResponse;

                                             if (!EVSE.TryParse(JSONObj,
                                                                out EVSE   _EVSE,
                                                                out String ErrorResponse,
                                                                EVSEUId))
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


                                             var JSON = _EVSE.ToJSON().
                                                              CreateResponse(1000,
                                                                             "",
                                                                             DateTime.UtcNow);


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

            #region PATCH  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PATCH,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
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

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            #region GET    ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
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

                                             var JSON = Connector.ToJSON().
                                                                  CreateResponse(1000,
                                                                                 "",
                                                                                 DateTime.UtcNow);


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

            #region PUT    ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Check LocationId URI parameter

                                             if (!Request.ParseLocationEVSEConnectorId(this,
                                                                                       out CountryCode?   CountryCode,
                                                                                       out Party_Id?      PartyId,
                                                                                       out Location_Id?   LocationId,
                                                                                       out EVSE_UId?      EVSEUId,
                                                                                       out Connector_Id?  ConnectorId,
                                                                                       out HTTPResponse   HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return HTTPResponse;

                                             if (!Connector.TryParse(JSONObj,
                                                                     out Connector _Connector,
                                                                     out String ErrorResponse,
                                                                     ConnectorId))
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

                                             var JSON = _Connector.ToJSON().
                                                                   CreateResponse(1000,
                                                                                  "",
                                                                                  DateTime.UtcNow);


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

            #region PATCH  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.PATCH,
                                         URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
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



            // GET   CDR {CDRId}
            // POST  CDR <= new one!

            // POST /authorize[?type={type}]     HTTP body: LocationReferences
            // https://www.server.com/ocpi/emsp/2.2/tokens/012345678/authorize
            // https://ocpi.server.com/2.2/tokens/012345678/authorize?type=RFID



        }

        #endregion


    }

}
