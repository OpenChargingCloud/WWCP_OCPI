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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Extention methods for the CPO HTTP API.
    /// </summary>
    public static class CPOAPIExtentions
    {

        #region ParseParseCountryCodePartyId(this Request, CPOAPI, out CountryCode, out PartyId,                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseParseCountryCodePartyId(this OCPIRequest  Request,
                                                           CPOAPI            CPOAPI,
                                                           out CountryCode?  CountryCode,
                                                           out Party_Id?     PartyId,
                                                           out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),       "The given CPO API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 2)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseLocationId             (this Request, CPOAPI, out LocationId,                                                                      out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationId(this OCPIRequest  Request,
                                              CPOAPI            CPOAPI,
                                              out Location_Id?  LocationId,
                                              out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId    = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
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

        #region ParseLocation               (this Request, CPOAPI, out LocationId, out Location,                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The Users API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocation(this OCPIRequest  Request,
                                            CPOAPI            CPOAPI,
                                            out Location_Id?  LocationId,
                                            out Location      Location,
                                            out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId    = null;
            Location      = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(CPOAPI.DefaultCountryCode,
                                                 CPOAPI.DefaultPartyId,
                                                 LocationId.Value,
                                                 out Location)) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
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


        #region ParseLocationEVSEId         (this Request, CPOAPI, out LocationId,               out EVSEUId,                                           out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEId(this OCPIRequest  Request,
                                                  CPOAPI            CPOAPI,
                                                  out Location_Id?  LocationId,
                                                  out EVSE_UId?     EVSEUId,
                                                  out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId    = null;
            EVSEUId       = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 2)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
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

        #region ParseLocationEVSE           (this Request, CPOAPI, out LocationId, out Location, out EVSEUId, out EVSE,                                 out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The Users API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSE(this OCPIRequest  Request,
                                                CPOAPI            CPOAPI,
                                                out Location_Id?  LocationId,
                                                out Location      Location,
                                                out EVSE_UId?     EVSEUId,
                                                out EVSE          EVSE,
                                                out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId    = null;
            Location      = null;
            EVSEUId       = null;
            EVSE          = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 2) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(CPOAPI.DefaultCountryCode,
                                                 CPOAPI.DefaultPartyId,
                                                 LocationId.Value,
                                                 out Location))
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE)) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
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


        #region ParseLocationEVSEConnectorId(this Request, CPOAPI, out LocationId,               out EVSEUId,           out ConnectorId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnectorId(this OCPIRequest   Request,
                                                           CPOAPI             CPOAPI,
                                                           out Location_Id?   LocationId,
                                                           out EVSE_UId?      EVSEUId,
                                                           out Connector_Id?  ConnectorId,
                                                           out HTTPResponse   HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId    = null;
            EVSEUId       = null;
            ConnectorId   = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
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

        #region ParseLocationEVSEConnector  (this Request, CPOAPI, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The Users API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnector(this OCPIRequest   Request,
                                                         CPOAPI             CPOAPI,
                                                         out Location_Id?   LocationId,
                                                         out Location       Location,
                                                         out EVSE_UId?      EVSEUId,
                                                         out EVSE           EVSE,
                                                         out Connector_Id?  ConnectorId,
                                                         out Connector      Connector,
                                                         out HTTPResponse   HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId    = null;
            Location      = null;
            EVSEUId       = null;
            EVSE          = null;
            ConnectorId   = null;
            Connector     = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 3) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid connector identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(CPOAPI.DefaultCountryCode,
                                                 CPOAPI.DefaultPartyId,
                                                 LocationId.Value,
                                                 out Location))
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE)) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!EVSE.TryGetConnector(ConnectorId.Value, out Connector)) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
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



        #region ParseTokenId               (this Request, CPOAPI, out CountryCode, out PartyId, out TokenId,               out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTokenId(this OCPIRequest  Request,
                                           CPOAPI            CPOAPI,
                                           out CountryCode?  CountryCode,
                                           out Party_Id?     PartyId,
                                           out Token_Id?     TokenId,
                                           out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            TokenId       = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            TokenId = Token_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!TokenId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid token identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseToken                 (this Request, CPOAPI, out CountryCode, out PartyId, out TokenId, out Token,    out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="Token">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseToken(this OCPIRequest  Request,
                                         CPOAPI            CPOAPI,
                                         out CountryCode?  CountryCode,
                                         out Party_Id?     PartyId,
                                         out Token_Id?     TokenId,
                                         out Token         Token,
                                         out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            TokenId       = null;
            Token         = null;
            HTTPResponse  = null;

            if (Request.ParsedURLParameters.Length < 3) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            TokenId = Token_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!TokenId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid token identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetToken(CountryCode.Value, PartyId.Value, TokenId.Value, out Token)) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CPOAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown token identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The HTTP API for charge point operators.
    /// </summary>
    public class CPOAPI : HTTPAPI
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

        #region Properties

        /// <summary>
        /// The CommonAPI.
        /// </summary>
        public CommonAPI    CommonAPI             { get; }

        /// <summary>
        /// The default country code to use.
        /// </summary>
        public CountryCode  DefaultCountryCode    { get; }

        /// <summary>
        /// The default party identification to use.
        /// </summary>
        public Party_Id     DefaultPartyId        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the HTTP API for charge point operators
        /// using the given CommonAPI.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="DefaultCountryCode">The default country code to use.</param>
        /// <param name="DefaultPartyId">The default party identification to use.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="ServiceName">An optional name of the HTTP API service.</param>
        public CPOAPI(CommonAPI      CommonAPI,
                      CountryCode    DefaultCountryCode,
                      Party_Id       DefaultPartyId,

                      HTTPHostname?  HTTPHostname         = null,
                      String         ExternalDNSName      = null,
                      HTTPPath?      URLPathPrefix        = null,
                      String         ServiceName          = DefaultHTTPServerName)

            : base(CommonAPI?.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   URLPathPrefix ?? DefaultURLPathPrefix,
                   ServiceName)

        {

            this.CommonAPI           = CommonAPI ?? throw new ArgumentNullException(nameof(CommonAPI), "The given CommonAPI must not be null!");
            this.DefaultCountryCode  = DefaultCountryCode;
            this.DefaultPartyId      = DefaultPartyId;

            RegisterURLTemplates();

        }

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region GET    [/cpo] == /

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPathPrefix + "/cpo", "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //HTTPServer.AddOCPIMethod(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPathPrefix + "/cpo/index.html",
            //                                 URLPathPrefix + "/cpo/"
            //                             },
            //                             HTTPContentType.HTML_UTF8,
            //                             OCPIRequest: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CPOAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request.HTTPRequest) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     Server          = DefaultHTTPServerName,
            //                                     Date            = DateTime.UtcNow,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = "close"
            //                                 };

            //                             });

            #endregion


            #region GET    ~/locations

            // https://example.com/ocpi/2.2/cpo/locations/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             var from                    = Request.QueryString.GetDateTime("date_from");
                                             var to                      = Request.QueryString.GetDateTime("date_to");
                                             var offset                  = Request.QueryString.GetUInt64  ("offset");
                                             var limit                   = Request.QueryString.GetUInt64  ("limit");


                                             // Link             Link to the 'next' page should be provided when this is NOT the last page.
                                             // X-Total-Count    The total number of objects available in the server system that match the given query (including the given query parameters.
                                             // X-Limit          The maximum number of objects that the server WILL return.

                                             var allLocations            = CommonAPI.   GetLocations(DefaultCountryCode,
                                                                                                     DefaultPartyId).
                                                                                        ToArray();
                                             var allLocationsCount       = allLocations.Length;

                                             var filteredLocations       = allLocations.Where(location => !from.HasValue || location.LastUpdated >  from.Value).
                                                                                        Where(location => !to.  HasValue || location.LastUpdated <= to.  Value).
                                                                                        ToArray();
                                             var filteredLocationsCount  = filteredLocations.Length;


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<IEnumerable<Location>>.Create(
                                                                                         filteredLocations.SkipTakeFilter(offset, limit),
                                                                                         locations => new JArray(locations.Select(location => location.ToJSON())),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #region GET    ~/locations/{locationId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check Location(Id URI parameter)

                                             if (!Request.ParseLocation(this,
                                                                        out Location_Id?  LocationId,
                                                                        out Location      Location,
                                                                        out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<Location>.Create(
                                                                                         Location,
                                                                                         location => location.ToJSON(),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #region GET    ~/locations/{locationId}/{evseId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations/{locationId}/{evseId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check EVSE(UId URI parameter)

                                             if (!Request.ParseLocationEVSE(this,
                                                                            out Location_Id?  LocationId,
                                                                            out Location      Location,
                                                                            out EVSE_UId?     EVSEId,
                                                                            out EVSE          EVSE,
                                                                            out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<EVSE>.Create(
                                                                                         EVSE,
                                                                                         evse => evse.ToJSON(),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #region GET    ~/locations/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check Connector(Id URI parameter)

                                             if (!Request.ParseLocationEVSEConnector(this,
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


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<Connector>.Create(
                                                                                         Connector,
                                                                                         connector => connector.ToJSON(),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion



            #region GET     ~/tariffs

            // https://example.com/ocpi/2.2/cpo/tariffs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100

            #endregion

            #region GET     ~/tariffs/{tariff_id}

            // https://example.com/ocpi/2.2/cpo/tariffs/12454

            #endregion



            #region GET     ~/sessions

            // https://example.com/ocpi/2.2/cpo/sessions/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100

            #endregion

            #region GET     ~/sessions/{session_id}

            // https://example.com/ocpi/2.2/cpo/sessions/12454

            #endregion

            #region PUT     ~/sessions/{session_id}/charging_preferences

            // https://example.com/ocpi/2.2/cpo/sessions/12454/charging_preferences

            #endregion



            #region GET     ~/cdrs

            // https://example.com/ocpi/2.2/cpo/cdrs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100

            #endregion

            #region GET     ~/cdrs/{cdr_id}

            // https://example.com/ocpi/2.2/cpo/cdrs/12454

            #endregion



            #region ~/tokens/{country_code}/{party_id}       [NonStandard]

            #region GET     ~/tokens/{country_code}/{party_id}       [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check CountryCode and PartyId

                                             if (!Request.ParseParseCountryCodePartyId(this,
                                                                                       out CountryCode?  CountryCode,
                                                                                       out Party_Id?     PartyId,
                                                                                       out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion


                                             var from                 = Request.QueryString.GetDateTime("date_from");
                                             var to                   = Request.QueryString.GetDateTime("date_to");
                                             var offset               = Request.QueryString.GetUInt64  ("offset");
                                             var limit                = Request.QueryString.GetUInt64  ("limit");


                                             // Link             Link to the 'next' page should be provided when this is NOT the last page.
                                             // X-Total-Count    The total number of objects available in the server system that match the given query (including the given query parameters.
                                             // X-Limit          The maximum number of objects that the server WILL return.

                                             var allTokens            = CommonAPI.GetTokens(CountryCode, PartyId);
                                             var allTokensCount       = allTokens.Count();

                                             var filteredTokens       = CommonAPI.GetTokens().
                                                                               Where(token => !from.HasValue || token.LastUpdated >  from.Value).
                                                                               Where(token => !to.  HasValue || token.LastUpdated <= to.  Value).
                                                                               SkipTakeFilter(offset, limit).
                                                                               ToArray();

                                             var filteredTokensCount  = filteredTokens.Count();


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, DELETE",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<IEnumerable<Token>>.Create(
                                                                                         filteredTokens,
                                                                                         tokens => new JArray(tokens.Select(token => token.ToJSON())),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #region DELETE  ~/tokens/{country_code}/{party_id}       [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.DELETE,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check CountryCode and PartyId

                                             if (!Request.ParseParseCountryCodePartyId(this,
                                                                                       out CountryCode?  CountryCode,
                                                                                       out Party_Id?     PartyId,
                                                                                       out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion


                                             CommonAPI.RemoveAllTokens(CountryCode.Value,
                                                                       PartyId.    Value);


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, DELETE",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #endregion

            #region ~/tokens/{country_code}/{party_id}/{tokenId}

            #region GET     ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check Token(Id URI parameter)

                                             if (!Request.ParseToken(this,
                                                                     out CountryCode?  CountryCode,
                                                                     out Party_Id?     PartyId,
                                                                     out Token_Id?     TokenId,
                                                                     out Token         Token,
                                                                     out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion


                                             //ToDo: What exactly to do with this information?
                                             var TokenType  = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<Token>.Create(
                                                                                         Token,
                                                                                         token => token.ToJSON(),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #region PUT     ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check TokenId URI parameter

                                             if (!Request.ParseTokenId(this,
                                                                        out CountryCode?  CountryCode,
                                                                        out Party_Id?     PartyId,
                                                                        out Token_Id?     TokenId,
                                                                        out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse newToken JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject newTokenJSON, out HTTPResponse))
                                                 return HTTPResponse;

                                             if (!Token.TryParse(newTokenJSON,
                                                                  out Token   newToken,
                                                                  out String  ErrorResponse,
                                                                  CountryCode,
                                                                  PartyId,
                                                                  TokenId))
                                             {

                                                 return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, PUT, PATCH",
                                                            AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = OCPIResponse<Token>.Create(
                                                                                             newToken,
                                                                                             token => token.ToJSON(),
                                                                                             -1,
                                                                                             ErrorResponse
                                                                                         ).ToUTF8Bytes()
                                                        }.AsImmutable;

                                             }

                                             #endregion


                                             //ToDo: What exactly to do with this information?
                                             var TokenType  = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;

                                             CommonAPI.AddOrUpdateToken(newToken);


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<Token>.Create(
                                                                                         newToken,
                                                                                         token => token.ToJSON(),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #region PATCH   ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.PATCH,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check Token(Id URI parameter)

                                             if (!Request.ParseToken(this,
                                                                      out CountryCode?  CountryCode,
                                                                      out Party_Id?     PartyId,
                                                                      out Token_Id?     TokenId,
                                                                      out Token         OldToken,
                                                                      out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion


                                             //ToDo: What exactly to do with this information?
                                             var TokenType = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;


                                             #region Parse and apply Token JSON patch

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONPatch, out HTTPResponse))
                                                 return HTTPResponse;

                                             var patchedToken = OldToken.Patch(JSONPatch);

                                             #endregion


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<Token>.Create(
                                                                                         patchedToken,
                                                                                         token => token.ToJSON(),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #region DELETE  ~/tokens/{country_code}/{party_id}/{tokenId}        [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.DELETE,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check Token(Id URI parameter)

                                             if (!Request.ParseToken(this,
                                                                     out CountryCode?  CountryCode,
                                                                     out Party_Id?     PartyId,
                                                                     out Token_Id?     TokenId,
                                                                     out Token         ToBeRemovedToken,
                                                                     out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion


                                             //ToDo: What exactly to do with this information?
                                             var TokenType     = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;

                                             var RemovedToken  = CommonAPI.RemoveToken(ToBeRemovedToken);


                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Content-Type, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<Token>.Create(
                                                                                         RemovedToken,
                                                                                         token => token.ToJSON(),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                         });

            #endregion

            #endregion


        }

        #endregion


    }

}
