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
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Extention methods for the EMSP HTTP API.
    /// </summary>
    public static class EMSPAPIExtentions
    {

        #region ParseCountryCodeAndPartyId(this Request, EMSPAPI, out CountryCode, out PartyId,                                                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="HTTPResponseBuilder">A HTTP response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCountryCodeAndPartyId(this OCPIRequest          Request,
                                                         EMSPAPI                   EMSPAPI,
                                                         out CountryCode?          CountryCode,
                                                         out Party_Id?             PartyId,
                                                         out HTTPResponse.Builder  HTTPResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            HTTPResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 2)
            {

                HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion


        #region ParseLocationId             (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId,                                                                      out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationId(this OCPIRequest          Request,
                                              EMSPAPI                   EMSPAPI,
                                              out CountryCode?          CountryCode,
                                              out Party_Id?             PartyId,
                                              out Location_Id?          LocationId,
                                              out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            LocationId    = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion

        #region ParseLocation               (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out HTTPResponse, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <param name="FailOnMissingLocation">Whether to fail when the location for the given location was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocation(this OCPIRequest          Request,
                                            EMSPAPI                   EMSPAPI,
                                            out CountryCode?          CountryCode,
                                            out Party_Id?             PartyId,
                                            out Location_Id?          LocationId,
                                            out Location              Location,
                                            out HTTPResponse.Builder  HTTPResponse,
                                            Boolean                   FailOnMissingLocation = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            LocationId    = default;
            Location      = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location) &&
                 FailOnMissingLocation)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion


        #region ParseLocationEVSEId         (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,                                           out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEId(this OCPIRequest          Request,
                                                  EMSPAPI                   EMSPAPI,
                                                  out CountryCode?          CountryCode,
                                                  out Party_Id?             PartyId,
                                                  out Location_Id?          LocationId,
                                                  out EVSE_UId?             EVSEUId,
                                                  out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            LocationId    = default;
            EVSEUId       = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 4)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion

        #region ParseLocationEVSE           (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out HTTPResponse, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <param name="FailOnMissingEVSE">Whether to fail when the location for the given EVSE was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSE(this OCPIRequest          Request,
                                                EMSPAPI                   EMSPAPI,
                                                out CountryCode?          CountryCode,
                                                out Party_Id?             PartyId,
                                                out Location_Id?          LocationId,
                                                out Location              Location,
                                                out EVSE_UId?             EVSEUId,
                                                out EVSE                  EVSE,
                                                out HTTPResponse.Builder  HTTPResponse,
                                                Boolean                   FailOnMissingEVSE = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            LocationId    = default;
            Location      = default;
            EVSEUId       = default;
            EVSE          = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 4)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!LocationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[3]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value,
                                                  PartyId.    Value,
                                                  LocationId. Value, out Location))
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE) &&
                 FailOnMissingEVSE)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion


        #region ParseLocationEVSEConnectorId(this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,           out ConnectorId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnectorId(this OCPIRequest          Request,
                                                           EMSPAPI                   EMSPAPI,
                                                           out CountryCode?          CountryCode,
                                                           out Party_Id?             PartyId,
                                                           out Location_Id?          LocationId,
                                                           out EVSE_UId?             EVSEUId,
                                                           out Connector_Id?         ConnectorId,
                                                           out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            LocationId    = default;
            EVSEUId       = default;
            ConnectorId   = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 5)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid connector identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion

        #region ParseLocationEVSEConnector  (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
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
        public static Boolean ParseLocationEVSEConnector(this OCPIRequest          Request,
                                                         EMSPAPI                   EMSPAPI,
                                                         out CountryCode?          CountryCode,
                                                         out Party_Id?             PartyId,
                                                         out Location_Id?          LocationId,
                                                         out Location              Location,
                                                         out EVSE_UId?             EVSEUId,
                                                         out EVSE                  EVSE,
                                                         out Connector_Id?         ConnectorId,
                                                         out Connector             Connector,
                                                         out HTTPResponse.Builder  HTTPResponse,
                                                         Boolean                   FailOnMissingConnector = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            LocationId    = default;
            Location      = default;
            EVSEUId       = default;
            EVSE          = default;
            ConnectorId   = default;
            Connector     = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 5)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!LocationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[3]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[4]);

            if (!EVSEUId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid connector identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location))
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown location identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE))
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown EVSE identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            if (!EVSE.TryGetConnector(ConnectorId.Value, out Connector) &&
                FailOnMissingConnector)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown connector identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion



        #region ParseTariffId               (this Request, EMSPAPI, out CountryCode, out PartyId, out TariffId,                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTariffId(this OCPIRequest          Request,
                                            EMSPAPI                   EMSPAPI,
                                            out CountryCode?          CountryCode,
                                            out Party_Id?             PartyId,
                                            out Tariff_Id?            TariffId,
                                            out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            TariffId      = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            TariffId = Tariff_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!TariffId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid tariff identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion

        #region ParseTariff                 (this Request, EMSPAPI, out CountryCode, out PartyId, out TariffId,  out Tariff,   out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTariff(this OCPIRequest          Request,
                                          EMSPAPI                   EMSPAPI,
                                          out CountryCode?          CountryCode,
                                          out Party_Id?             PartyId,
                                          out Tariff_Id?            TariffId,
                                          out Tariff                Tariff,
                                          out HTTPResponse.Builder  HTTPResponse,
                                          Boolean                   FailOnMissingTariff = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            TariffId      = default;
            Tariff        = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            TariffId = Tariff_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!TariffId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid tariff identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetTariff(CountryCode.Value, PartyId.Value, TariffId.Value, out Tariff) &&
                 FailOnMissingTariff)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown tariff identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion


        #region ParseSessionId              (this Request, EMSPAPI, out CountryCode, out PartyId, out SessionId,               out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseSessionId(this OCPIRequest         Request,
                                            EMSPAPI                   EMSPAPI,
                                            out CountryCode?          CountryCode,
                                            out Party_Id?             PartyId,
                                            out Session_Id?           SessionId,
                                            out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            SessionId     = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            SessionId = Session_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!SessionId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid session identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion

        #region ParseSession                (this Request, EMSPAPI, out CountryCode, out PartyId, out SessionId, out Session,  out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseSession(this OCPIRequest         Request,
                                          EMSPAPI                   EMSPAPI,
                                          out CountryCode?          CountryCode,
                                          out Party_Id?             PartyId,
                                          out Session_Id?           SessionId,
                                          out Session               Session,
                                          out HTTPResponse.Builder  HTTPResponse,
                                          Boolean                   FailOnMissingSession = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            SessionId     = default;
            Session       = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            SessionId = Session_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!SessionId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid session identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetSession(CountryCode.Value, PartyId.Value, SessionId.Value, out Session) &&
                FailOnMissingSession)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown session identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion


        #region ParseCDRId                  (this Request, EMSPAPI, out CountryCode, out PartyId, out CDRId,                   out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCDRId(this OCPIRequest          Request,
                                         EMSPAPI                   EMSPAPI,
                                         out CountryCode?          CountryCode,
                                         out Party_Id?             PartyId,
                                         out CDR_Id?               CDRId,
                                         out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            CDRId         = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CDRId = CDR_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!CDRId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid CDR identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion

        #region ParseCDR                    (this Request, EMSPAPI, out CountryCode, out PartyId, out CDRId,     out CDR,      out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCDR(this OCPIRequest          Request,
                                       EMSPAPI                   EMSPAPI,
                                       out CountryCode?          CountryCode,
                                       out Party_Id?             PartyId,
                                       out CDR_Id?               CDRId,
                                       out CDR                   CDR,
                                       out HTTPResponse.Builder  HTTPResponse,
                                       Boolean                   FailOnMissingCDR = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode   = default;
            PartyId       = default;
            CDRId         = default;
            CDR           = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid country code!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid party identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            CDRId = CDR_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!CDRId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid charge detail record identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetCDR(CountryCode.Value, PartyId.Value, CDRId.Value, out CDR) &&
                FailOnMissingCDR)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown charge detail record identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion



        #region ParseTokenId                (this Request, EMSPAPI,                               out TokenId,                 out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTokenId(this OCPIRequest          Request,
                                           EMSPAPI                   EMSPAPI,
                                           out Token_Id?             TokenId,
                                           out HTTPResponse.Builder  HTTPResponse)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            TokenId       = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            TokenId = Token_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!TokenId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid token identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion

        #region ParseToken                  (this Request, EMSPAPI,                               out TokenId,   out Token,    out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="TokenStatus">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseToken(this OCPIRequest          Request,
                                         EMSPAPI                   EMSPAPI,
                                         out Token_Id?             TokenId,
                                         out TokenStatus           TokenStatus,
                                         out HTTPResponse.Builder  HTTPResponse,
                                         Boolean                   FailOnMissingToken = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            TokenId       = default;
            TokenStatus   = default;
            HTTPResponse  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            TokenId = Token_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!TokenId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid token identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetToken(Request.ToCountryCode ?? EMSPAPI.DefaultCountryCode,
                                               Request.ToPartyId     ?? EMSPAPI.DefaultPartyId,
                                               TokenId.Value,
                                               out TokenStatus) &&
                FailOnMissingToken)
            {

                HTTPResponse = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = EMSPAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown token identification!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                }.Set("X-Request-ID",      Request.RequestId).
                  Set("X-Correlation-ID",  Request.CorrelationId);

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The HTTP API for e-mobility service providers.
    /// CPOs will connect to this API.
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

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?     AllowDowngrades       { get; }

        #endregion

        #region Events

        #region (protected internal) DeleteLocationsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationsRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationsRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnDeleteLocationsRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) DeleteLocationsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete locations response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteLocationsResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete locations response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationsResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        HTTPResponse  Response)

            => OnDeleteLocationsResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion



        #region (protected internal) PutLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a put location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutLocationRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a put location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutLocationRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnPutLocationRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) PutLocationResponse   (Response)

        /// <summary>
        /// An event sent whenever a put location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutLocationResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a put location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutLocationResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    HTTPResponse  Response)

            => OnPutLocationResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) PatchLocationRequest  (Request)

        /// <summary>
        /// An event sent whenever a patch location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchLocationRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a patch location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchLocationRequest(DateTime     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnPatchLocationRequest?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) PatchLocationResponse (Response)

        /// <summary>
        /// An event sent whenever a patch location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchLocationResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a patch location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchLocationResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      HTTPResponse  Response)

            => OnPatchLocationResponse?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion


        #region (protected internal) DeleteLocationRequest (Request)

        /// <summary>
        /// An event sent whenever a delete location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnDeleteLocationRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) DeleteLocationResponse(Response)

        /// <summary>
        /// An event sent whenever a delete location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteLocationResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       HTTPResponse  Response)

            => OnDeleteLocationResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion



        #region (protected internal) PutEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a put EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutEVSERequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a put EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutEVSERequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnPutEVSERequest?.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) PutEVSEResponse   (Response)

        /// <summary>
        /// An event sent whenever a put EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutEVSEResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a put EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutEVSEResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                HTTPResponse  Response)

            => OnPutEVSEResponse?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) PatchEVSERequest  (Request)

        /// <summary>
        /// An event sent whenever a patch EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchEVSERequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a patch EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchEVSERequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPatchEVSERequest?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) PatchEVSEResponse (Response)

        /// <summary>
        /// An event sent whenever a patch EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchEVSEResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a patch EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchEVSEResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  HTTPResponse  Response)

            => OnPatchEVSEResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) DeleteEVSERequest (Request)

        /// <summary>
        /// An event sent whenever a delete EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteEVSERequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteEVSERequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnDeleteEVSERequest?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) DeleteEVSEResponse(Response)

        /// <summary>
        /// An event sent whenever a delete EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteEVSEResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteEVSEResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   HTTPResponse  Response)

            => OnDeleteEVSEResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion



        #region (protected internal) PutConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a put connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutConnectorRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a put connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutConnectorRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnPutConnectorRequest?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request);

        #endregion

        #region (protected internal) PutConnectorResponse   (Response)

        /// <summary>
        /// An event sent whenever a put connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutConnectorResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a put connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutConnectorResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     HTTPResponse  Response)

            => OnPutConnectorResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion


        #region (protected internal) PatchConnectorRequest  (Request)

        /// <summary>
        /// An event sent whenever a patch connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchConnectorRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a patch connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchConnectorRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPatchConnectorRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) PatchConnectorResponse (Response)

        /// <summary>
        /// An event sent whenever a patch connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchConnectorResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a patch connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchConnectorResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       HTTPResponse  Response)

            => OnPatchConnectorResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion


        #region (protected internal) DeleteConnectorRequest (Request)

        /// <summary>
        /// An event sent whenever a delete connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteConnectorRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteConnectorRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnDeleteConnectorRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) DeleteConnectorResponse(Response)

        /// <summary>
        /// An event sent whenever a delete connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteConnectorResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteConnectorResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        HTTPResponse  Response)

            => OnDeleteConnectorResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion




        #region (protected internal) DeleteTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffsRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffsRequest(DateTime     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnDeleteTariffsRequest?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) DeleteTariffsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete tariffs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTariffsResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete tariffs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffsResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      HTTPResponse  Response)

            => OnDeleteTariffsResponse?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion



        #region (protected internal) PutTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a put tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTariffRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a put tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTariffRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPutTariffRequest?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) PutTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a put tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTariffResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a put tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTariffResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  HTTPResponse  Response)

            => OnPutTariffResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) DeleteTariffRequest (Request)

        /// <summary>
        /// An event sent whenever a delete tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnDeleteTariffRequest?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request);

        #endregion

        #region (protected internal) DeleteTariffResponse(Response)

        /// <summary>
        /// An event sent whenever a delete tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTariffResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     HTTPResponse  Response)

            => OnDeleteTariffResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion




        #region (protected internal) DeleteSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionsRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnDeleteSessionsRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) DeleteSessionsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete sessions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteSessionsResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete sessions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       HTTPResponse  Response)

            => OnDeleteSessionsResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion



        #region (protected internal) PutSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a put session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutSessionRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a put session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutSessionRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnPutSessionRequest?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) PutSessionResponse   (Response)

        /// <summary>
        /// An event sent whenever a put session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutSessionResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a put session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutSessionResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   HTTPResponse  Response)

            => OnPutSessionResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) PatchSessionRequest  (Request)

        /// <summary>
        /// An event sent whenever a patch session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchSessionRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a patch session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchSessionRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnPatchSessionRequest?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request);

        #endregion

        #region (protected internal) PatchSessionResponse (Response)

        /// <summary>
        /// An event sent whenever a patch session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchSessionResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a patch session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchSessionResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     HTTPResponse  Response)

            => OnPatchSessionResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion


        #region (protected internal) DeleteSessionRequest (Request)

        /// <summary>
        /// An event sent whenever a delete session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionRequest(DateTime     Timestamp,
                                                     HTTPAPI      API,
                                                     OCPIRequest  Request)

            => OnDeleteSessionRequest?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) DeleteSessionResponse(Response)

        /// <summary>
        /// An event sent whenever a delete session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteSessionResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      HTTPResponse  Response)

            => OnDeleteSessionResponse?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion




        #region (protected internal) DeleteCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRsRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRsRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnDeleteCDRsRequest?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) DeleteCDRsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete CDRs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCDRsResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete CDRs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRsResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   HTTPResponse  Response)

            => OnDeleteCDRsResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) PostCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a post CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCDRRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a post CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCDRRequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnPostCDRRequest?.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) PostCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a post CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCDRResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a post CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCDRResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                HTTPResponse  Response)

            => OnPostCDRResponse?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) DeleteCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a delete CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnDeleteCDRRequest?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) DeleteCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a delete CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCDRResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  HTTPResponse  Response)

            => OnDeleteCDRResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion




        #region (protected internal) PostTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a post token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTokenRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a post token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTokenRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPostTokenRequest?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) PostTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a post token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostTokenResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a post token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTokenResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  HTTPResponse  Response)

            => OnPostTokenResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the HTTP API for e-mobility service providers
        /// using the given CommonAPI.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="DefaultCountryCode">The default country code to use.</param>
        /// <param name="DefaultPartyId">The default party identification to use.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="ServiceName">An optional name of the HTTP API service.</param>
        public EMSPAPI(CommonAPI      CommonAPI,
                       CountryCode    DefaultCountryCode,
                       Party_Id       DefaultPartyId,
                       Boolean?       AllowDowngrades   = null,

                       HTTPHostname?  HTTPHostname      = null,
                       String         ExternalDNSName   = null,
                       HTTPPath?      URLPathPrefix     = null,
                       String         ServiceName       = DefaultHTTPServerName)

            : base(CommonAPI?.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   URLPathPrefix ?? DefaultURLPathPrefix,
                   ServiceName)

        {

            this.CommonAPI           = CommonAPI ?? throw new ArgumentNullException(nameof(CommonAPI), "The given CommonAPI must not be null!");
            this.DefaultCountryCode  = DefaultCountryCode;
            this.DefaultPartyId      = DefaultPartyId;
            this.AllowDowngrades     = AllowDowngrades;

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

            //HTTPServer.AddOCPIMethod(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPathPrefix + "/emsp/index.html",
            //                                 URLPathPrefix + "/emsp/"
            //                             },
            //                             HTTPContentType.HTML_UTF8,
            //                             OCPIRequest: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(EMSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.EMSPAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(EMSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2.HTTPAPI.EMSPAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

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


            // Receiver Interface for eMSPs and NSPs

            #region ~/locations/{country_code}/{party_id}       [NonStandard]

            #region GET     ~/locations/{country_code}/{party_id}       [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "locations/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check Location(Id URI parameter)

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         var filters                 = Request.GetDateAndPaginationFilters();

                                         var allLocations            = CommonAPI.GetLocations(CountryCode, PartyId).
                                                                                 ToArray();

                                         var allLocationsCount       = allLocations.Length;


                                         var filteredLocations       = CommonAPI.GetLocations().
                                                                           Where(location => !filters.From.HasValue || location.LastUpdated >  filters.From.Value).
                                                                           Where(location => !filters.To.  HasValue || location.LastUpdated <= filters.To.  Value).
                                                                           ToArray();

                                         var filteredLocationsCount  = filteredLocations.Length;


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, DELETE",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<IEnumerable<Location>>.Create(
                                                                                  filteredLocations.SkipTakeFilter(filters.Offset,
                                                                                                                   filters.Limit),
                                                                                  locations => new JArray(locations.Select(location => location.ToJSON())),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               Set("X-Total-Count",     filteredLocationsCount).
                                               // X-Limit               The maximum number of objects that the server WILL return.
                                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                               AsImmutable);

                                     });

            #endregion

            #region DELETE  ~/locations/{country_code}/{party_id}       [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "locations/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteLocationsRequest,
                                     OCPIResponseLogger:  DeleteLocationsResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check CountryCode & PartyId

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         //ToDo: await...
                                         CommonAPI.RemoveAllLocations(CountryCode.Value,
                                                                      PartyId.    Value);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}

            #region GET     ~/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check location

                                         if (!Request.ParseLocation(this,
                                                                    out CountryCode?          CountryCode,
                                                                    out Party_Id?             PartyId,
                                                                    out Location_Id?          LocationId,
                                                                    out Location              Location,
                                                                    out HTTPResponse.Builder  HTTPResponse,
                                                                    FailOnMissingLocation: true))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<Location>.Create(
                                                                                  Location,
                                                                                  location => location.ToJSON(),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 LastModified               = Location.LastUpdated.ToIso8601(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               AsImmutable);

                                     });

            #endregion

            #region PUT     ~/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PUT,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PutLocationRequest,
                                     OCPIResponseLogger:  PutLocationResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing location

                                         if (!Request.ParseLocation(this,
                                                                    out CountryCode?          CountryCode,
                                                                    out Party_Id?             PartyId,
                                                                    out Location_Id?          LocationId,
                                                                    out Location              ExistingLocation,
                                                                    out HTTPResponse.Builder  HTTPResponse,
                                                                    FailOnMissingLocation: false))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse new or updated location JSON

                                         if (!Request.HTTPRequest.TryParseJObjectRequestBody(out JObject LocationJSON, out HTTPResponse))
                                             return HTTPResponse;

                                         if (!Location.TryParse(LocationJSON,
                                                                out Location  newOrUpdatedLocation,
                                                                out String    ErrorResponse,
                                                                CountryCode,
                                                                PartyId,
                                                                LocationId))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId);

                                         }

                                         #endregion

                                         #region Check whether the new location is "newer" than the existing location

                                         var bbb = Request.QueryString.GetBoolean("forceDowngrade") ??
                                                   // ToDo: Check AccessToken
                                                   AllowDowngrades;

                                         if (AllowDowngrades == false &&
                                             // ToDo: Check AccessToken
                                             newOrUpdatedLocation.LastUpdated < ExistingLocation.LastUpdated &&
                                             !Request.QueryString.GetBoolean("forceDowngrade", false))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.FailedDependency,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId).
                                                      AsImmutable;

                                         }

                                         #endregion


                                         var wasCreated = CommonAPI.LocationExists(newOrUpdatedLocation.CountryCode,
                                                                                   newOrUpdatedLocation.PartyId,
                                                                                   newOrUpdatedLocation.Id);


                                         //ToDo: Handle AddOrUpdate errors
                                         CommonAPI.AddOrUpdateLocation(newOrUpdatedLocation);



                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = wasCreated
                                                                                     ? HTTPStatusCode.Created
                                                                                     : HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Location>.Create(
                                                                                     newOrUpdatedLocation,
                                                                                     location => location.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    LastModified               = newOrUpdatedLocation.LastUpdated.ToIso8601(),
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #region PATCH   ~/locations/{country_code}/{party_id}/{locationId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PATCH,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PatchLocationRequest,
                                     OCPIResponseLogger:  PatchLocationResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check location

                                         if (!Request.ParseLocation(this,
                                                                    out CountryCode?          CountryCode,
                                                                    out Party_Id?             PartyId,
                                                                    out Location_Id?          LocationId,
                                                                    out Location              ExistingLocation,
                                                                    out HTTPResponse.Builder  HTTPResponse,
                                                                    FailOnMissingLocation: true))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse and apply Location JSON patch

                                         if (!Request.TryParseJObjectRequestBody(out JObject LocationPatch, out HTTPResponse))
                                             return HTTPResponse;

                                         #endregion


                                         //ToDo: await..., handle update errors!
                                         var patchedLocation = CommonAPI.PatchLocation(ExistingLocation,
                                                                                       LocationPatch);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Location>.Create(
                                                                                     patchedLocation,
                                                                                     location => location.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    LastModified               = patchedLocation.LastUpdated.ToIso8601(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #region DELETE  ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteLocationRequest,
                                     OCPIResponseLogger:  DeleteLocationResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing location

                                         if (!Request.ParseLocation(this,
                                                                    out CountryCode?          CountryCode,
                                                                    out Party_Id?             PartyId,
                                                                    out Location_Id?          LocationId,
                                                                    out Location              ExistingLocation,
                                                                    out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         //ToDo: await...
                                         CommonAPI.RemoveLocation(ExistingLocation);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Location>.Create(
                                                                                     ExistingLocation,
                                                                                     location => location.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    LastModified               = ExistingLocation.LastUpdated.ToIso8601(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            #region GET     ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check EVSE

                                         if (!Request.ParseLocationEVSE(this,
                                                                        out CountryCode?          CountryCode,
                                                                        out Party_Id?             PartyId,
                                                                        out Location_Id?          LocationId,
                                                                        out Location              Location,
                                                                        out EVSE_UId?             EVSEUId,
                                                                        out EVSE                  EVSE,
                                                                        out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<EVSE>.Create(
                                                                                  EVSE,
                                                                                  evse => evse.ToJSON(),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 LastModified               = EVSE.LastUpdated.ToIso8601(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               AsImmutable);

                                     });

            #endregion

            #region PUT     ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PUT,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PutEVSERequest,
                                     OCPIResponseLogger:  PutEVSEResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing EVSE

                                         if (!Request.ParseLocationEVSE(this,
                                                                        out CountryCode?          CountryCode,
                                                                        out Party_Id?             PartyId,
                                                                        out Location_Id?          LocationId,
                                                                        out Location              ExistingLocation,
                                                                        out EVSE_UId?             EVSEUId,
                                                                        out EVSE                  ExistingEVSE,
                                                                        out HTTPResponse.Builder  HTTPResponse,
                                                                        FailOnMissingEVSE: false))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse new or updated EVSE JSON

                                         if (!Request.TryParseJObjectRequestBody(out JObject EVSEJSON, out HTTPResponse))
                                             return HTTPResponse;

                                         if (!EVSE.TryParse(EVSEJSON,
                                                            out EVSE    newOrUpdatedEVSE,
                                                            out String  ErrorResponse,
                                                            EVSEUId))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId);

                                         }

                                         #endregion

                                         #region Check whether the new EVSE is "newer" than the existing EVSE

                                         if (newOrUpdatedEVSE.LastUpdated < ExistingEVSE.LastUpdated &&
                                             !Request.QueryString.GetBoolean("forceDowngrade", false))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.FailedDependency,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId).
                                                      AsImmutable;

                                         }

                                         #endregion


                                         //ToDo: Handle AddOrUpdate errors
                                         //CommonAPI.AddOrUpdate(newOrUpdatedLocation);

                                         var wasCreated = true;


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = wasCreated
                                                                                     ? HTTPStatusCode.Created
                                                                                     : HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<EVSE>.Create(
                                                                                     newOrUpdatedEVSE,
                                                                                     evse => evse.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #region PATCH   ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PATCH,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PatchEVSERequest,
                                     OCPIResponseLogger:  PatchEVSEResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check EVSE

                                         if (!Request.ParseLocationEVSE(this,
                                                                        out CountryCode?          CountryCode,
                                                                        out Party_Id?             PartyId,
                                                                        out Location_Id?          LocationId,
                                                                        out Location              OldLocation,
                                                                        out EVSE_UId?             EVSEUId,
                                                                        out EVSE                  ExistingEVSE,
                                                                        out HTTPResponse.Builder  HTTPResponse,
                                                                        FailOnMissingEVSE: true))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse and apply EVSE JSON patch

                                         if (!Request.TryParseJObjectRequestBody(out JObject EVSEPatch, out HTTPResponse))
                                             return HTTPResponse;

                                         #endregion


                                         //ToDo: Handle update errors
                                         var patchedEVSE = CommonAPI.PatchEVSE(ExistingEVSE,
                                                                               EVSEPatch);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<EVSE>.Create(
                                                                                     patchedEVSE,
                                                                                     evse => evse.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    LastModified               = patchedEVSE.LastUpdated.ToIso8601(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #region DELETE  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}     [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteEVSERequest,
                                     OCPIResponseLogger:  DeleteEVSEResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing Location/EVSE(UId URI parameter)

                                         if (!Request.ParseLocationEVSE(this,
                                                                        out CountryCode?          CountryCode,
                                                                        out Party_Id?             PartyId,
                                                                        out Location_Id?          LocationId,
                                                                        out Location              ExistingLocation,
                                                                        out EVSE_UId?             EVSEUId,
                                                                        out EVSE                  ExistingEVSE,
                                                                        out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Location>.Create(
                                                                                     ExistingLocation,
                                                                                     location => location.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    LastModified               = ExistingLocation.LastUpdated.ToIso8601(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            #region GET     ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check connector

                                         if (!Request.ParseLocationEVSEConnector(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out Location_Id?          LocationId,
                                                                                 out Location              Location,
                                                                                 out EVSE_UId?             EVSEId,
                                                                                 out EVSE                  EVSE,
                                                                                 out Connector_Id?         ConnectorId,
                                                                                 out Connector             Connector,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<Connector>.Create(
                                                                                  Connector,
                                                                                  connector => connector.ToJSON(),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 LastModified               = Connector.LastUpdated.ToIso8601(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               AsImmutable);

                                     });

            #endregion

            #region PUT     ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PUT,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PutConnectorRequest,
                                     OCPIResponseLogger:  PutConnectorResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check connector

                                         if (!Request.ParseLocationEVSEConnector(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out Location_Id?          LocationId,
                                                                                 out Location              Location,
                                                                                 out EVSE_UId?             EVSEUId,
                                                                                 out EVSE                  EVSE,
                                                                                 out Connector_Id?         ConnectorId,
                                                                                 out Connector             ExistingConnector,
                                                                                 out HTTPResponse.Builder  HTTPResponse,
                                                                                 FailOnMissingConnector: false))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse new or updated connector JSON

                                         if (!Request.TryParseJObjectRequestBody(out JObject ConnectorJSON, out HTTPResponse))
                                             return HTTPResponse;

                                         if (!Connector.TryParse(ConnectorJSON,
                                                                 out Connector  newOrUpdatedConnector,
                                                                 out String     ErrorResponse,
                                                                 ConnectorId))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId);

                                         }

                                         #endregion

                                         #region Check whether the new connector is "newer" than the existing connector

                                         if (newOrUpdatedConnector.LastUpdated < ExistingConnector.LastUpdated &&
                                             !Request.QueryString.GetBoolean("forceDowngrade", false))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.FailedDependency,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId).
                                                      AsImmutable;

                                         }

                                         #endregion


                                         //ToDo: Handle AddOrUpdate errors
                                         //CommonAPI.AddOrUpdateLocation(newOrUpdatedLocation);

                                         var wasCreated = true;


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = wasCreated
                                                                                     ? HTTPStatusCode.Created
                                                                                     : HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Connector>.Create(
                                                                                     newOrUpdatedConnector,
                                                                                     connector => connector.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #region PATCH   ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PATCH,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PatchConnectorRequest,
                                     OCPIResponseLogger:  PatchConnectorResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check connector

                                         if (!Request.ParseLocationEVSEConnector(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out Location_Id?          LocationId,
                                                                                 out Location              OldLocation,
                                                                                 out EVSE_UId?             EVSEUId,
                                                                                 out EVSE                  OldEVSE,
                                                                                 out Connector_Id?         ConnectorId,
                                                                                 out Connector             ExistingConnector,
                                                                                 out HTTPResponse.Builder  HTTPResponse,
                                                                                 FailOnMissingConnector: true))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse and apply Connector JSON patch

                                         if (!Request.TryParseJObjectRequestBody(out JObject ConnectorPatch, out HTTPResponse))
                                             return HTTPResponse;

                                         #endregion


                                         //ToDo: Handle update errors
                                         var patchedConnector = CommonAPI.PatchConnector(ExistingConnector,
                                                                                         ConnectorPatch);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Connector>.Create(
                                                                                     patchedConnector,
                                                                                     connector => connector.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    LastModified               = patchedConnector.LastUpdated.ToIso8601(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #region DELETE  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}       [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteConnectorRequest,
                                     OCPIResponseLogger:  DeleteConnectorResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing Location/EVSE/Connector(UId URI parameter)

                                         if (!Request.ParseLocationEVSEConnector(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out Location_Id?          LocationId,
                                                                                 out Location              ExistingLocation,
                                                                                 out EVSE_UId?             EVSEUId,
                                                                                 out EVSE                  ExistingEVSE,
                                                                                 out Connector_Id?         ConnectorId,
                                                                                 out Connector             ExistingConnector,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Connector>.Create(
                                                                                     ExistingConnector,
                                                                                     location => location.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #endregion



            #region ~/tariffs/{country_code}/{party_id}             [NonStandard]

            #region GET     ~/tariffs/{country_code}/{party_id}             [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "tariffs/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check Tariff(Id URI parameter)

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         var filters               = Request.GetDateAndPaginationFilters();

                                         var allTariffs            = CommonAPI.GetTariffs(CountryCode, PartyId).
                                                                               ToArray();

                                         var allTariffsCount       = allTariffs.Length;


                                         var filteredTariffs       = CommonAPI.GetTariffs().
                                                                           Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                                           Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                                           ToArray();

                                         var filteredTariffsCount  = filteredTariffs.Length;


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, PUT, PATCH",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<IEnumerable<Tariff>>.Create(
                                                                                  filteredTariffs.SkipTakeFilter(filters.Offset,
                                                                                                                 filters.Limit),
                                                                                  tariffs => new JArray(tariffs.Select(tariff => tariff.ToJSON())),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               Set("X-Total-Count",     filteredTariffsCount).
                                               // X-Limit               The maximum number of objects that the server WILL return.
                                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                               AsImmutable);

                                     });

            #endregion

            #region DELETE  ~/tariffs/{country_code}/{party_id}             [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "tariffs/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteTariffsRequest,
                                     OCPIResponseLogger:  DeleteTariffsResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check CountryCode & PartyId

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         CommonAPI.RemoveAllTariffs(CountryCode.Value,
                                                                    PartyId.    Value);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #endregion

            #region ~/tariffs/{country_code}/{party_id}/{tariffId}

            #region GET     ~/tariffs/{country_code}/{party_id}/{tariffId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check Tariff(Id URI parameter)

                                         if (!Request.ParseTariff(this,
                                                                  out CountryCode?          CountryCode,
                                                                  out Party_Id?             PartyId,
                                                                  out Tariff_Id?            TariffId,
                                                                  out Tariff                Tariff,
                                                                  out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, PUT, DELETE",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<Tariff>.Create(
                                                                                  Tariff,
                                                                                  tariff => tariff.ToJSON(),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               AsImmutable);

                                     });

            #endregion

            #region PUT     ~/tariffs/{country_code}/{party_id}/{tariffId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PUT,
                                     URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PutTariffRequest,
                                     OCPIResponseLogger:  PutTariffResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing tariff

                                         if (!Request.ParseTariff(this,
                                                                  out CountryCode?          CountryCode,
                                                                  out Party_Id?             PartyId,
                                                                  out Tariff_Id?            TariffId,
                                                                  out Tariff                ExistingTariff,
                                                                  out HTTPResponse.Builder  HTTPResponse,
                                                                  FailOnMissingTariff:      false))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse new or updated tariff

                                         if (!Request.TryParseJObjectRequestBody(out JObject TariffJSON, out HTTPResponse))
                                             return HTTPResponse;

                                         if (!Tariff.TryParse(TariffJSON,
                                                              out Tariff  newOrUpdatedTariff,
                                                              out String  ErrorResponse,
                                                              CountryCode,
                                                              PartyId,
                                                              TariffId))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId);

                                         }

                                         #endregion

                                         #region Check whether the new tariff is "newer" than the existing tariff

                                         if (newOrUpdatedTariff.LastUpdated < ExistingTariff.LastUpdated &&
                                             !Request.QueryString.GetBoolean("forceDowngrade", false))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.FailedDependency,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }.AsImmutable;

                                         }

                                         #endregion


                                         var wasCreated = CommonAPI.TariffExists(newOrUpdatedTariff.CountryCode,
                                                                                 newOrUpdatedTariff.PartyId,
                                                                                 newOrUpdatedTariff.Id);

                                         //ToDo: Handle AddOrUpdate errors
                                         CommonAPI.AddOrUpdateTariff(newOrUpdatedTariff);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = wasCreated
                                                                                     ? HTTPStatusCode.Created
                                                                                     : HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Tariff>.Create(
                                                                                     newOrUpdatedTariff,
                                                                                     tariff => tariff.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #region DELETE  ~/tariffs/{country_code}/{party_id}/{tariffId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteTariffRequest,
                                     OCPIResponseLogger:  DeleteTariffResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing tariff

                                         if (!Request.ParseTariff(this,
                                                                  out CountryCode?          CountryCode,
                                                                  out Party_Id?             PartyId,
                                                                  out Tariff_Id?            TariffId,
                                                                  out Tariff                ExistingTariff,
                                                                  out HTTPResponse.Builder  HTTPResponse,
                                                                  FailOnMissingTariff:      true))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         //ToDo: await...
                                         CommonAPI.RemoveTariff(ExistingTariff);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Tariff>.Create(
                                                                                     ExistingTariff,
                                                                                     tariff => tariff.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #endregion



            #region ~/sessions/{country_code}/{party_id}                [NonStandard]

            #region GET     ~/sessions/{country_code}/{party_id}                [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "sessions/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check Session(Id URI parameter)

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         var filters                = Request.GetDateAndPaginationFilters();

                                         var allSessions            = CommonAPI.GetSessions(CountryCode, PartyId).
                                                                                ToArray();

                                         var allSessionsCount       = allSessions.Length;


                                         var filteredSessions       = CommonAPI.GetSessions().
                                                                           Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                                           Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                                           ToArray();

                                         var filteredSessionsCount  = filteredSessions.Length;


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, DELETE",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<IEnumerable<Session>>.Create(
                                                                                  filteredSessions.SkipTakeFilter(filters.Offset,
                                                                                                                  filters.Limit),
                                                                                  sessions => new JArray(sessions.Select(session => session.ToJSON())),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               Set("X-Total-Count",     filteredSessionsCount).
                                               // X-Limit               The maximum number of objects that the server WILL return.
                                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                               AsImmutable);

                                     });

            #endregion

            #region DELETE  ~/sessions/{country_code}/{party_id}                [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "sessions/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteSessionsRequest,
                                     OCPIResponseLogger:  DeleteSessionsResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check CountryCode & PartyId

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         CommonAPI.RemoveAllSessions(CountryCode.Value,
                                                                     PartyId.    Value);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #endregion

            #region ~/sessions/{country_code}/{party_id}/{sessionId}

            #region GET     ~/sessions/{country_code}/{party_id}/{sessionId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check existing session

                                         if (!Request.ParseSession(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Session_Id?           SessionId,
                                                                   out Session               ExistingSession,
                                                                   out HTTPResponse.Builder  HTTPResponse,
                                                                   FailOnMissingSession: true))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<Session>.Create(
                                                                                  ExistingSession,
                                                                                  session => session.ToJSON(),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               AsImmutable);

                                     });

            #endregion

            #region PUT     ~/sessions/{country_code}/{party_id}/{sessionId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PUT,
                                     URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PutSessionRequest,
                                     OCPIResponseLogger:  PutSessionResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing session

                                         if (!Request.ParseSession(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Session_Id?           SessionId,
                                                                   out Session               ExistingSession,
                                                                   out HTTPResponse.Builder  HTTPResponse,
                                                                   FailOnMissingSession: false))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse new or updated session

                                         if (!Request.TryParseJObjectRequestBody(out JObject SessionJSON, out HTTPResponse))
                                             return HTTPResponse;

                                         if (!Session.TryParse(SessionJSON,
                                                               out Session  newOrUpdatedSession,
                                                               out String   ErrorResponse,
                                                               CountryCode,
                                                               PartyId,
                                                               SessionId))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId);

                                         }

                                         #endregion

                                         #region Check whether the new session is "newer" than the existing session

                                         if (newOrUpdatedSession.LastUpdated < ExistingSession.LastUpdated &&
                                             !Request.QueryString.GetBoolean("forceDowngrade", false))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.FailedDependency,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }.AsImmutable;

                                         }

                                         #endregion


                                         var wasCreated = CommonAPI.SessionExists(newOrUpdatedSession.CountryCode,
                                                                                  newOrUpdatedSession.PartyId,
                                                                                  newOrUpdatedSession.Id);

                                         //ToDo: Handle AddOrUpdate errors
                                         CommonAPI.AddOrUpdateSession(newOrUpdatedSession);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = wasCreated
                                                                                     ? HTTPStatusCode.Created
                                                                                     : HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Session>.Create(
                                                                                     newOrUpdatedSession,
                                                                                     session => session.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #region PATCH   ~/sessions/{country_code}/{party_id}/{sessionId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PATCH,
                                     URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PatchSessionRequest,
                                     OCPIResponseLogger:  PatchSessionResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check session

                                         if (!Request.ParseSession(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Session_Id?           SessionId,
                                                                   out Session               OldSession,
                                                                   out HTTPResponse.Builder  HTTPResponse,
                                                                   FailOnMissingSession: true))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse and apply Session JSON patch

                                         if (!Request.TryParseJObjectRequestBody(out JObject JSONPatch, out HTTPResponse))
                                             return HTTPResponse;

                                         var patchedSession = OldSession.Patch(JSONPatch);

                                         #endregion


                                         //ToDo: Handle update errors
                                         CommonAPI.UpdateSession(patchedSession);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Session>.Create(
                                                                                     patchedSession,
                                                                                     session => session.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    LastModified               = patchedSession.LastUpdated.ToIso8601(),
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #region DELETE  ~/sessions/{country_code}/{party_id}/{sessionId}    [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteSessionRequest,
                                     OCPIResponseLogger:  DeleteSessionResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing session

                                         if (!Request.ParseSession(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Session_Id?           SessionId,
                                                                   out Session               ExistingSession,
                                                                   out HTTPResponse.Builder  HTTPResponse,
                                                                   FailOnMissingSession: true))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         //ToDo: await...
                                         CommonAPI.RemoveSession(ExistingSession);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<Session>.Create(
                                                                                     ExistingSession,
                                                                                     session => session.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #endregion



            #region ~/cdrs/{country_code}/{party_id}            [NonStandard]

            #region GET     ~/cdrs/{country_code}/{party_id}            [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "cdrs/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check CDR(Id URI parameter)

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return Task.FromResult(HTTPResponse.AsImmutable);
                                         }

                                         #endregion


                                         var filters            = Request.GetDateAndPaginationFilters();

                                         // Link             Link to the 'next' page should be provided when this is NOT the last page.
                                         // X-Total-Count    The total number of objects available in the server system that match the given query (including the given query parameters.
                                         // X-Limit          The maximum number of objects that the server WILL return.

                                         var allCDRs            = CommonAPI.GetCDRs(CountryCode, PartyId).
                                                                            ToArray();

                                         var allCDRsCount       = allCDRs.Length;


                                         var filteredCDRs       = CommonAPI.GetCDRs().
                                                                           Where(cdr => !filters.From.HasValue || cdr.LastUpdated >  filters.From.Value).
                                                                           Where(cdr => !filters.To.  HasValue || cdr.LastUpdated <= filters.To.  Value).
                                                                           ToArray();

                                         var filteredCDRsCount  = filteredCDRs.Length;


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                 HTTPStatusCode             = HTTPStatusCode.OK,
                                                 Server                     = DefaultHTTPServerName,
                                                 Date                       = DateTime.UtcNow,
                                                 AccessControlAllowOrigin   = "*",
                                                 AccessControlAllowMethods  = "GET, POST, DELETE",
                                                 AccessControlAllowHeaders  = "Authorization",
                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                 Content                    = OCPIResponse<IEnumerable<CDR>>.Create(
                                                                                  filteredCDRs.SkipTakeFilter(filters.Offset,
                                                                                                              filters.Limit),
                                                                                  cdrs => new JArray(cdrs.Select(cdr => cdr.ToJSON())),
                                                                                  1000,
                                                                                  "Hello world!"
                                                                              ).ToUTF8Bytes(),
                                                 Connection                 = "close"
                                             }.Set("X-Request-ID",      Request.RequestId).
                                               Set("X-Correlation-ID",  Request.CorrelationId).
                                               Set("X-Total-Count",     filteredCDRsCount).
                                               // X-Limit               The maximum number of objects that the server WILL return.
                                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                               AsImmutable);

                                     });

            #endregion

            #region POST    ~/cdrs/{country_code}/{party_id}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.POST,
                                     URLPathPrefix + "cdrs/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PostCDRRequest,
                                     OCPIResponseLogger:  PostCDRResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check CountryCode & PartyId URI parameter

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         #region Parse newCDR JSON

                                         if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out HTTPResponse))
                                             return HTTPResponse;

                                         if (!CDR.TryParse(JSONCDR,
                                                           out CDR     newCDR,
                                                           out String  ErrorResponse,
                                                           CountryCode,
                                                           PartyId))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, POST, PATCH",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = OCPIResponse<CDR>.Create(
                                                                                         newCDR,
                                                                                         cdr => cdr.ToJSON(),
                                                                                         -1,
                                                                                         ErrorResponse
                                                                                     ).ToUTF8Bytes()
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId);

                                         }

                                         #endregion


                                         // ToDo: What kind of error might happen here?
                                         CommonAPI.AddCDR(newCDR);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.Created,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, POST, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<CDR>.Create(
                                                                                     newCDR,
                                                                                     cdr => cdr.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #region DELETE  ~/cdrs/{country_code}/{party_id}            [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "cdrs/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteCDRsRequest,
                                     OCPIResponseLogger:  DeleteCDRsResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check CountryCode & PartyId

                                         if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                 out CountryCode?          CountryCode,
                                                                                 out Party_Id?             PartyId,
                                                                                 out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         CommonAPI.RemoveAllCDRs(CountryCode.Value,
                                                                 PartyId.    Value);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #endregion

            #region ~/cdrs/{country_code}/{party_id}/{cdrId}

            #region GET     ~/cdrs/{country_code}/{party_id}/{cdrId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: async Request => {

                                         #region Check existing CDR

                                         if (!Request.ParseCDR(this,
                                                               out CountryCode?          CountryCode,
                                                               out Party_Id?             PartyId,
                                                               out CDR_Id?               CDRId,
                                                               out CDR                   ExistingCDR,
                                                               out HTTPResponse.Builder  HTTPResponse,
                                                               FailOnMissingCDR: true))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<CDR>.Create(
                                                                                     ExistingCDR,
                                                                                     cdr => cdr.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #region DELETE  ~/cdrs/{country_code}/{party_id}/{cdrId}    [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.DELETE,
                                     URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   DeleteCDRRequest,
                                     OCPIResponseLogger:  DeleteCDRResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check existing CDR

                                         if (!Request.ParseCDR(this,
                                                               out CountryCode?          CountryCode,
                                                               out Party_Id?             PartyId,
                                                               out CDR_Id?               CDRId,
                                                               out CDR                   ExistingCDR,
                                                               out HTTPResponse.Builder  HTTPResponse,
                                                               FailOnMissingCDR: true))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion


                                         //ToDo: await...
                                         CommonAPI.RemoveCDR(ExistingCDR);


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, PUT, PATCH, DELETE",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<CDR>.Create(
                                                                                     ExistingCDR,
                                                                                     CDR => CDR.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                };

                                     });

            #endregion

            #endregion


            #region ~/tokens

            #region GET     ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "tokens",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         var filters              = Request.GetDateAndPaginationFilters();

                                         var allTokens            = CommonAPI.GetTokens().
                                                                              Select(tokenStatus => tokenStatus.Token).
                                                                              ToArray();
                                         var allTokensCount       = allTokens.Length;

                                         var filteredTokens       = allTokens.Where(token => !filters.From.HasValue || token.LastUpdated >  filters.From.Value).
                                                                              Where(token => !filters.To.  HasValue || token.LastUpdated <= filters.To.  Value).
                                                                              ToArray();
                                         var filteredTokensCount  = filteredTokens.Length;


                                         return Task.FromResult(
                                             new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET, POST",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<IEnumerable<Token>>.Create(
                                                                                     filteredTokens.SkipTakeFilter(filters.Offset,
                                                                                                                   filters.Limit),
                                                                                     tokens => new JArray(tokens.Select(token => token.ToJSON())),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId).
                                                  Set("X-Total-Count",     filteredTokensCount).
                                                  // X-Limit               The maximum number of objects that the server WILL return.
                                                  // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                             AsImmutable);

                                     });

            #endregion

            #region POST    ~/tokens/{token_id}/authorize?type=RFID

            // A real-time authorization request
            // https://example.com/ocpi/2.2/emsp/tokens/012345678/authorize?type=RFID
            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.POST,
                                     URLPathPrefix + "tokens/{token_id}/authorize",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestLogger:   PostTokenRequest,
                                     OCPIResponseLogger:  PostTokenResponse,
                                     OCPIRequest:   async Request => {

                                         #region Check TokenId URI parameter

                                         if (!Request.ParseTokenId(this,
                                                                   out Token_Id?             TokenId,
                                                                   out HTTPResponse.Builder  HTTPResponse))
                                         {
                                             return HTTPResponse;
                                         }

                                         #endregion

                                         var requestedTokenType  = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;


                                         if (!CommonAPI.TryGetToken(Request.ToCountryCode ?? DefaultCountryCode,
                                                                    Request.ToPartyId     ?? DefaultPartyId,
                                                                    TokenId.Value,
                                                                    out TokenStatus tokenStatus) ||
                                             (tokenStatus.Token.Type != requestedTokenType))
                                         {

                                             return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                        Server                     = DefaultHTTPServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "POST",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                        Connection                 = "close"
                                                    }.Set("X-Request-ID",      Request.RequestId).
                                                      Set("X-Correlation-ID",  Request.CorrelationId);

                                         }


                                         #region Parse optional LocationReference JSON

                                         LocationReference? locationReference = null;

                                         if (Request.TryParseJObjectRequestBody(out JObject LocationReferenceJSON, out HTTPResponse, AllowEmptyHTTPBody: true))
                                         {

                                             if (!LocationReference.TryParse(LocationReferenceJSON,
                                                                             out LocationReference  _locationReference,
                                                                             out String             ErrorResponse))
                                             {

                                                 return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = DefaultHTTPServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "POST",
                                                            AccessControlAllowHeaders  = "Authorization",
                                                            //ContentType                = HTTPContentType.JSON_UTF8,
                                                            //Content                    = OCPIResponse<IEnumerable<Token>>.Create(
                                                            //                                 filteredTokens.SkipTakeFilter(offset, limit),
                                                            //                                 tokens => new JArray(tokens.Select(token => token.ToJSON())),
                                                            //                                 1000,
                                                            //                                 "Hello world!"
                                                            //                             ).ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.Set("X-Request-ID",      Request.RequestId).
                                                          Set("X-Correlation-ID",  Request.CorrelationId);;

                                             }
                                             else
                                             {

                                                 locationReference = _locationReference;

                                                 //ToDo: Somehow filter by location reference!

                                             }

                                         }

                                         #endregion


                                         return new HTTPResponse.Builder(Request.HTTPRequest) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = DefaultHTTPServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "POST",
                                                    AccessControlAllowHeaders  = "Authorization",
                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                    Content                    = OCPIResponse<AuthorizationInfo>.Create(
                                                                                     new AuthorizationInfo(tokenStatus.Status,
                                                                                                           tokenStatus.Token,
                                                                                                           locationReference,
                                                                                                           null,
                                                                                                           null),
                                                                                     authorizationInfo => authorizationInfo.ToJSON(),
                                                                                     1000,
                                                                                     "Hello world!"
                                                                                 ).ToUTF8Bytes(),
                                                    Connection                 = "close"
                                                }.Set("X-Request-ID",      Request.RequestId).
                                                  Set("X-Correlation-ID",  Request.CorrelationId);

                                     });

            #endregion

            #endregion

        }

        #endregion


    }

}
