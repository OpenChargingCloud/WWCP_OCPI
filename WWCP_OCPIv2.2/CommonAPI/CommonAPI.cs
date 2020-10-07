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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Extention methods for the Common API.
    /// </summary>
    public static class CommonAPIExtentions
    {


        #region ParseParseCountryCodePartyId(this HTTPRequest, CommonAPI, out CountryCode, out PartyId,                                                         out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="CommonAPI">The Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseParseCountryCodePartyId(this HTTPRequest  HTTPRequest,
                                                           CommonAPI         CommonAPI,
                                                           out CountryCode?  CountryCode,
                                                           out Party_Id?     PartyId,
                                                           out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (CommonAPI    == null)
                throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 2)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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





        #region ParseLocationId             (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId,                                                                      out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="CommonAPI">The Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationId(this HTTPRequest  HTTPRequest,
                                              CommonAPI         CommonAPI,
                                              out CountryCode?  CountryCode,
                                              out Party_Id?     PartyId,
                                              out Location_Id?  LocationId,
                                              out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (CommonAPI    == null)
                throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            LocationId    = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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

        #region ParseLocation               (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocation(this HTTPRequest  HTTPRequest,
                                            CommonAPI         CommonAPI,
                                            out CountryCode?  CountryCode,
                                            out Party_Id?     PartyId,
                                            out Location_Id?  LocationId,
                                            out Location      Location,
                                            out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (CommonAPI    == null)
                throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

            #endregion

            CountryCode   = null;
            PartyId       = null;
            LocationId    = null;
            Location      = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 3) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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


        #region ParseLocationEVSEId         (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,                                           out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="CommonAPI">The Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEId(this HTTPRequest  HTTPRequest,
                                                  CommonAPI         CommonAPI,
                                                  out CountryCode?  CountryCode,
                                                  out Party_Id?     PartyId,
                                                  out Location_Id?  LocationId,
                                                  out EVSE_UId?     EVSEUId,
                                                  out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (CommonAPI    == null)
                throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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

        #region ParseLocationEVSE           (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSE(this HTTPRequest  HTTPRequest,
                                                CommonAPI         CommonAPI,
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

            if (CommonAPI    == null)
                throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!CommonAPI.TryGetLocation(CountryCode.Value,
                                          PartyId.    Value,
                                          LocationId. Value, out Location)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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


        #region ParseLocationEVSEConnectorId(this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,           out ConnectorId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="CommonAPI">The Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnectorId(this HTTPRequest   HTTPRequest,
                                                           CommonAPI          CommonAPI,
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

            if (CommonAPI    == null)
                throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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

        #region ParseLocationEVSEConnector  (this HTTPRequest, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
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
                                                         CommonAPI          CommonAPI,
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

            if (CommonAPI    == null)
                throw new ArgumentNullException(nameof(CommonAPI),    "The given Common API must not be null!");

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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid connector identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }


            if (!CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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
                    Server          = CommonAPI.HTTPServer.DefaultServerName,
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



        private static JObject createResponse(this JToken   Data,
                                              Int32         ErrorCode,
                                              String        StatusMessage,
                                              DateTime?     Timestamp = null)

            => JSONObject.Create(

                   new JProperty("data",                  Data),
                   new JProperty("status_code",           ErrorCode),

                   StatusMessage.IsNotNullOrEmpty()
                       ? new JProperty("status_message",  StatusMessage)
                       :  null,

                   new JProperty("timestamp",            (Timestamp ?? DateTime.UtcNow).ToIso8601())

               );

        public static JObject CreateResponse(this JObject  Data,
                                             Int32         ErrorCode,
                                             String        StatusMessage = null,
                                             DateTime?     Timestamp     = null)

            => createResponse(Data,
                              ErrorCode,
                              StatusMessage,
                              Timestamp);

        public static JObject CreateResponse(this JArray  Data,
                                             Int32        ErrorCode,
                                             String       StatusMessage = null,
                                             DateTime?    Timestamp     = null)

            => createResponse(Data,
                              ErrorCode,
                              StatusMessage,
                              Timestamp);

    }


    /// <summary>
    /// The common HTTP API.
    /// </summary>
    public class CommonAPI : HTTPAPI
    {

        #region Data

        private static readonly    Random    _Random                   = new Random();

        protected internal const   String    __DefaultHTTPRoot         = "cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.CommonAPI.HTTPRoot";

        //private readonly Func<String, Stream>  _GetRessources;


        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("io/OCPI/");


        private readonly Dictionary<AccessToken, AccessInfo> _AccessTokens;

        #endregion

        #region Properties

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        #region CommonAPI(HTTPServerName, ...)

        /// <summary>
        /// Create a new common HTTP API.
        /// </summary>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="HTTPServerPort">An optional HTTP TCP port.</param>
        /// <param name="HTTPServerName">An optional HTTP server name.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional HTTP URL path prefix.</param>
        /// <param name="ServiceName">An optional HTTP service name.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        public CommonAPI(HTTPHostname?   HTTPHostname      = null,
                         IPPort?         HTTPServerPort    = null,
                         String          HTTPServerName    = DefaultHTTPServerName,
                         String          ExternalDNSName   = null,
                         HTTPPath?       URLPathPrefix     = null,
                         String          ServiceName       = DefaultHTTPServiceName,
                         DNSClient       DNSClient         = null)

            : base(HTTPHostname,
                   HTTPServerPort ?? DefaultHTTPServerPort,
                   HTTPServerName ?? DefaultHTTPServerName,
                   ExternalDNSName,
                   URLPathPrefix  ?? DefaultURLPathPrefix,
                   ServiceName    ?? DefaultHTTPServiceName,
                   DNSClient)

        {

            this._AccessTokens  = new Dictionary<AccessToken, AccessInfo>();
            this._Locations     = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();

            RegisterURLTemplates();

        }

        #endregion

        #region CommonAPI(HTTPServer, ...)

        /// <summary>
        /// Create a new common HTTP API.
        /// </summary>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="ServiceName">An optional name of the HTTP API service.</param>
        public CommonAPI(HTTPServer      HTTPServer,
                         HTTPHostname?   HTTPHostname      = null,
                         String          ExternalDNSName   = null,
                         HTTPPath?       URLPathPrefix     = null,
                         String          ServiceName       = DefaultHTTPServerName)

            : base(HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   URLPathPrefix,
                   ServiceName)

        {

            this._AccessTokens  = new Dictionary<AccessToken, AccessInfo>();
            this._Locations     = new Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id, Location>>>();

            // Link HTTP events...
            HTTPServer.RequestLog   += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog  += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog     += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region GET    /

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

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix,
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = DefaultHTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ContentType     = HTTPContentType.TEXT_UTF8,
                                                 Content         = ("This is an Open Charge Point Interface HTTP service!\nPlease check /versions!").ToUTF8Bytes(),
                                                 Connection      = "close"
                                             };

                                         });

            #endregion


            #region GET         ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/versions
            // ----------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check access token

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                  AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken accessToken) &&
                                                 _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo  accessInfo)  &&
                                                 accessInfo.Status != AccessStatus.ALLOWED)
                                             {

                                                 // Invalid or blocked access token!
                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET",
                                                         AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                         Connection                    = "close",
                                                         Vary                          = "Accept"
                                                     }.AsImmutable);

                                             }

                                             #endregion


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.OK,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                     ContentType                   = HTTPContentType.JSON_UTF8,
                                                     Content                       = OCPIResponse<IEnumerable<Version>>.Create(
                                                                                         new Version[] {
                                                                                             new Version(
                                                                                                 Version_Id.Parse("2.2"),
                                                                                                 "https://api.chargeIT-mobility.com/io/ocpi/versions/2.2"
                                                                                             )
                                                                                         },
                                                                                         versions => new JArray(versions.Select(version => version.ToJSON())),
                                                                                         1000,
                                                                                         "Hello world!"
                                                                                     ).ToUTF8Bytes(),
                                                     Connection                    = "close",
                                                     Vary                          = "Accept"
                                                 }.AsImmutable);


                                         });

            #endregion

            #region GET         ~/versions/{id}

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/versions/{id}
            // ---------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "versions/{id}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get version identification URL parameter

                                             if (Request.ParsedURLParameters.Length < 1)
                                             {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                         Server          = HTTPServer.DefaultServerName,
                                                         Date            = DateTime.UtcNow,
                                                         Connection      = "close"
                                                     }.AsImmutable);

                                             }

                                             if (!Version_Id.TryParse(Request.ParsedURLParameters[0], out Version_Id versionId))
                                             {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                         Server          = HTTPServer.DefaultServerName,
                                                         Date            = DateTime.UtcNow,
                                                         Connection      = "close"
                                                     }.AsImmutable);

                                             }

                                             #endregion

                                             #region Check access token

                                             AccessInfo accessInfo = default;

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                  AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken accessToken) &&
                                                 _AccessTokens.TryGetValue(accessToken,                                  out             accessInfo)  &&
                                                 accessInfo.Status != AccessStatus.ALLOWED)
                                             {

                                                 // Invalid or blocked access token!
                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET",
                                                         AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                         Connection                    = "close",
                                                         Vary                          = "Accept"
                                                     }.AsImmutable);

                                             }

                                             #endregion

                                             #region Only allow versionId == "2.2"

                                             if (versionId.ToString() != "2.2")
                                             {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                         Server          = HTTPServer.DefaultServerName,
                                                         Date            = DateTime.UtcNow,
                                                         Connection      = "close"
                                                     }.AsImmutable);

                                             }

                                             #endregion


                                             var endpoints  = new List<VersionEndpoint>() {

                                                                  new VersionEndpoint(ModuleIDs.Credentials,
                                                                                      InterfaceRoles.SENDER,
                                                                                      "https://" + Request.Host + URLPathPrefix + "2.2/credentials/"),

                                                                  new VersionEndpoint(ModuleIDs.Credentials,
                                                                                      InterfaceRoles.RECEIVER,
                                                                                      "https://" + Request.Host + URLPathPrefix + "2.2/credentials/")

                                                              };


                                             if (accessInfo.Role == Roles.CPO)
                                             {

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/emsp/locations/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Tariffs,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/emsp/tariffs/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/emsp/sessions/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/emsp/cdrs/"));


                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Commands,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/emsp/commands/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Tokens,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/emsp/tokens/"));

                                                 // hubclientinfo

                                             }


                                             if (accessInfo.Role == Roles.EMSP || accessInfo.Role == Roles.OpenData)
                                             {

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/locations/"));

                                             }

                                             if (accessInfo.Role == Roles.EMSP)
                                             {

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/cdrs/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/sessions/"));


                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Locations,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/locations/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Tariffs,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/tariffs/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Sessions,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/sessions/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.CDRs,
                                                                                   InterfaceRoles.SENDER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/cdrs/"));


                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Commands,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/commands/"));

                                                 endpoints.Add(new VersionEndpoint(ModuleIDs.Tokens,
                                                                                   InterfaceRoles.RECEIVER,
                                                                                   "https://" + Request.Host + URLPathPrefix + "2.2/cpo/tokens/"));

                                                 // hubclientinfo

                                             }


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET",
                                                     AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = OCPIResponse<VersionDetail>.Create(
                                                                                      new VersionDetail(
                                                                                          Version_Id.Parse("2.2"),
                                                                                          endpoints),
                                                                                      version => version.ToJSON(),
                                                                                      1000,
                                                                                      "Hello world!"
                                                                                  ).ToUTF8Bytes(),
                                                     Connection                 = "close",
                                                     Vary                       = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion



            #region GET         ~/2.2/credentials

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "2.2/credentials",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                 AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken accessToken) &&
                                                _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo  accessInfo)  &&
                                                 accessInfo.Status == AccessStatus.ALLOWED)
                                             {

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.OK,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                         ContentType                   = HTTPContentType.JSON_UTF8,
                                                         Content                       = OCPIResponse<OCPIv2_2.Credentials>.Create(
                                                                                             accessInfo.Credentials,
                                                                                             credentials => credentials.ToJSON(),
                                                                                             1000,
                                                                                             "Hello world!"
                                                                                         ).ToUTF8Bytes(),
                                                         Connection                    = "close",
                                                         Vary                          = "Accept"
                                                     }.AsImmutable);

                                             }

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                     Connection                    = "close",
                                                     Vary                          = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region POST        ~/2.2/credentials

            // REGISTER new OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.POST,
                                         URLPathPrefix + "cpo/2.2/credentials",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                 AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken  accessToken) &&
                                                _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo   accessInfo)  &&
                                                 accessInfo.Status == AccessStatus.ALLOWED)
                                             {

                                                 if (accessInfo.Credentials != null)
                                                     return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.MethodNotAllowed,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse HTTPResp, AllowEmptyHTTPBody: false) ||
                                                     !OCPIv2_2.Credentials.TryParse(JSONObj, out OCPIv2_2.Credentials credentials, out String ErrorResponse))
                                                    return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.BadRequest,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 // Here we should check the other side!!!


                                                 SetAccessToken(accessToken,
                                                                accessInfo.Role,
                                                                credentials,
                                                                AccessStatus.ALLOWED);


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.OK,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                         ContentType                   = HTTPContentType.JSON_UTF8,
                                                         Content                       = OCPIResponse<OCPIv2_2.Credentials>.Create(
                                                                                             credentials,
                                                                                             credential => credential.ToJSON(),
                                                                                             1000,
                                                                                             "Hello world!"
                                                                                         ).ToUTF8Bytes(),
                                                         Connection                    = "close"
                                                     }.AsImmutable);

                                             }

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                     Connection                    = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region PUT         ~/2.2/credentials

            // UPDATE the registration of an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "2.2/credentials",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                 AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken  accessToken) &&
                                                _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo   accessInfo)  &&
                                                 accessInfo.Status == AccessStatus.ALLOWED)
                                             {

                                                 if (accessInfo.Credentials == null)
                                                     return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.MethodNotAllowed,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse HTTPResp, AllowEmptyHTTPBody: false) ||
                                                     !OCPIv2_2.Credentials.TryParse(JSONObj, out OCPIv2_2.Credentials credentials, out String ErrorResponse))
                                                    return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.BadRequest,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 // Here we should check the other side!!!


                                                 SetAccessToken(accessToken,
                                                                accessInfo.Role,
                                                                credentials,
                                                                AccessStatus.ALLOWED);


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.OK,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                         ContentType                   = HTTPContentType.JSON_UTF8,
                                                         Content                       = OCPIResponse<OCPIv2_2.Credentials>.Create(
                                                                                             accessInfo.Credentials,
                                                                                             credential => credential.ToJSON(),
                                                                                             1000,
                                                                                             "Hello world!"
                                                                                         ).ToUTF8Bytes(),
                                                         Connection                    = "close"
                                                     }.AsImmutable);

                                             }

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                     Connection                    = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region DELETE      ~/2.2/credentials

            // UNREGISTER an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/2.2/credentials
            // ---------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.DELETE,
                                         URLPathPrefix + "2.2/credentials",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             if (Request.Authorization is HTTPTokenAuthentication TokenAuth &&
                                                 AccessToken. TryParse   (StringExtensions.FromBase64(TokenAuth.Token), out AccessToken  accessToken) &&
                                                _AccessTokens.TryGetValue(accessToken,                                  out AccessInfo   accessInfo)  &&
                                                 accessInfo.Status == AccessStatus.ALLOWED)
                                             {

                                                 if (accessInfo.Credentials == null)
                                                     return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode                = HTTPStatusCode.MethodNotAllowed,
                                                             Server                        = HTTPServer.DefaultServerName,
                                                             Date                          = DateTime.UtcNow,
                                                             AccessControlAllowOrigin      = "*",
                                                             AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                             AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                             Connection                    = "close"
                                                         }.AsImmutable);


                                                 RemoveAccessToken(accessToken);


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode                = HTTPStatusCode.OK,
                                                         Server                        = HTTPServer.DefaultServerName,
                                                         Date                          = DateTime.UtcNow,
                                                         AccessControlAllowOrigin      = "*",
                                                         AccessControlAllowMethods     = "GET, POST, PUT, DELETE",
                                                         AccessControlAllowHeaders     = "Authorization",
                                                         Connection                    = "close"
                                                     }.AsImmutable);

                                             }

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                = HTTPStatusCode.Forbidden,
                                                     Server                        = HTTPServer.DefaultServerName,
                                                     Date                          = DateTime.UtcNow,
                                                     AccessControlAllowOrigin      = "*",
                                                     AccessControlAllowMethods     = "GET",
                                                     AccessControlAllowHeaders     = "Authorization",
                                                     Connection                    = "close"
                                                 }.AsImmutable);

                                         });

            #endregion


        }

        #endregion


        #region AccessTokens

        public CommonAPI SetAccessToken(AccessToken  AccessToken,
                                           Roles        Role,
                                           AccessStatus AccessStatus = AccessStatus.ALLOWED)
        {
            lock (_AccessTokens)
            {

                _AccessTokens.Remove(AccessToken);

                _AccessTokens.Add(AccessToken,
                                  new AccessInfo(Role,
                                                 AccessStatus,
                                                 null));

                return this;

            }
        }

        public CommonAPI SetAccessToken(AccessToken           AccessToken,
                                           Roles                 Role,
                                           OCPIv2_2.Credentials  Credentials,
                                           AccessStatus          AccessStatus = AccessStatus.ALLOWED)
        {
            lock (_AccessTokens)
            {

                _AccessTokens.Remove(AccessToken);

                _AccessTokens.Add(AccessToken,
                                  new AccessInfo(Role,
                                                 AccessStatus,
                                                 Credentials));

                return this;

            }
        }

        public CommonAPI RemoveAccessToken(AccessToken AccessToken)
        {
            lock (_AccessTokens)
            {
                _AccessTokens.Remove(AccessToken);
                return this;
            }
        }

        #endregion


        #region Locations

        private Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Location_Id , Location>>> _Locations;


        #region AddLocation(Location)

        public Location AddLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (!_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    _Locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (!locations.ContainsKey(Location.Id))
                {
                    locations.Add(Location.Id, Location);
                    return Location;
                }

                throw new ArgumentException("The given location already exists!");

            }

        }

        #endregion

        #region AddLocationIfNotExists(Location)

        public Location AddLocationIfNotExists(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (!_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    _Locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (!locations.ContainsKey(Location.Id))
                    locations.Add(Location.Id, Location);

                return Location;

            }

        }

        #endregion

        #region AddLocationOrUpdate(Location)

        public Location AddLocationOrUpdate(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (!_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    parties = new Dictionary<Party_Id, Dictionary<Location_Id, Location>>();
                    _Locations.Add(Location.CountryCode, parties);
                }

                if (!parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                {
                    locations = new Dictionary<Location_Id, Location>();
                    parties.Add(Location.PartyId, locations);
                }

                if (locations.ContainsKey(Location.Id))
                {
                    locations.Remove(Location.Id);
                }

                locations.Add(Location.Id, Location);
                return Location;

            }

        }

        #endregion


        #region TryGetLocation(CountryCode, PartyId, LocationId,, out Location)

        public Boolean TryGetLocation(CountryCode   CountryCode,
                                      Party_Id      PartyId,
                                      Location_Id   LocationId,
                                      out Location  Location)
        {

            lock (_Locations)
            {

                if (_Locations.TryGetValue(CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {
                    if (parties.TryGetValue(PartyId, out Dictionary<Location_Id, Location> locations))
                    {
                        if (locations.TryGetValue(LocationId, out Location))
                            return true;
                    }
                }

                Location = null;
                return false;

            }

        }

        #endregion

        #region GetLocations(CountryCode = null, PartyId = null)

        public IEnumerable<Location> GetLocations(CountryCode? CountryCode  = null,
                                                  Party_Id?    PartyId      = null)
        {

            lock (_Locations)
            {

                if (CountryCode.HasValue && PartyId.HasValue)
                {
                    if (_Locations.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                    {
                        if (parties.TryGetValue(PartyId.Value, out Dictionary<Location_Id, Location> locations))
                        {
                            return locations.Values.ToArray();
                        }
                    }
                }

                else if (!CountryCode.HasValue && PartyId.HasValue)
                {

                    var allLocations = new List<Location>();

                    foreach (var party in _Locations.Values)
                    {
                        if (party.TryGetValue(PartyId.Value, out Dictionary<Location_Id, Location> locations))
                        {
                            allLocations.AddRange(locations.Values);
                        }
                    }

                    return allLocations;

                }

                else if (CountryCode.HasValue && !PartyId.HasValue)
                {
                    if (_Locations.TryGetValue(CountryCode.Value, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                    {

                        var allLocations = new List<Location>();

                        foreach (var locations in parties.Values)
                        {
                            allLocations.AddRange(locations.Values);
                        }

                        return allLocations;

                    }
                }

                else
                {

                    var allLocations = new List<Location>();

                    foreach (var party in _Locations.Values)
                    {
                        foreach (var locations in party.Values)
                        {
                            allLocations.AddRange(locations.Values);
                        }
                    }

                    return allLocations;

                }

                return new Location[0];

            }

        }

        #endregion


        #region RemoveLocation(Location)

        public Location RemoveLocation(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location), "The given location must not be null!");

            lock (_Locations)
            {

                if (_Locations.TryGetValue(Location.CountryCode, out Dictionary<Party_Id, Dictionary<Location_Id, Location>> parties))
                {

                    if (parties.TryGetValue(Location.PartyId, out Dictionary<Location_Id, Location> locations))
                    {

                        if (locations.ContainsKey(Location.Id))
                        {
                            locations.Remove(Location.Id);
                        }

                        if (!locations.Any())
                            parties.Remove(Location.PartyId);

                    }

                    if (!parties.Any())
                        _Locations.Remove(Location.CountryCode);

                }

                return Location;

            }

        }

        #endregion

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

                //SendCompleted(this, DateTime.UtcNow, Message);

            }

        }

        #endregion

    }

}
