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
using System.Threading.Tasks;

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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseParseCountryCodePartyId(this OCPIRequest          Request,
                                                           CPOAPI                    CPOAPI,
                                                           out CountryCode?          CountryCode,
                                                           out Party_Id?             PartyId,
                                                           out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 2)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code and/or party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationId(this OCPIRequest          Request,
                                              CPOAPI                    CPOAPI,
                                              out Location_Id?          LocationId,
                                              out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId           = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocation(this OCPIRequest          Request,
                                            CPOAPI                    CPOAPI,
                                            out Location_Id?          LocationId,
                                            out Location              Location,
                                            out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId           =  default;
            Location             =  default;
            OCPIResponseBuilder  =  default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(CPOAPI.DefaultCountryCode,
                                                 CPOAPI.DefaultPartyId,
                                                 LocationId.Value,
                                                 out Location)) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2003,
                    StatusMessage        = "Unknown location!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEId(this OCPIRequest          Request,
                                                  CPOAPI                    CPOAPI,
                                                  out Location_Id?          LocationId,
                                                  out EVSE_UId?             EVSEUId,
                                                  out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId           = default;
            EVSEUId              = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 2)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing location and/or EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSE(this OCPIRequest          Request,
                                                CPOAPI                    CPOAPI,
                                                out Location_Id?          LocationId,
                                                out Location              Location,
                                                out EVSE_UId?             EVSEUId,
                                                out EVSE                  EVSE,
                                                out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId    = default;
            Location      = default;
            EVSEUId       = default;
            EVSE          = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 2)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing location and/or EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(CPOAPI.DefaultCountryCode,
                                                 CPOAPI.DefaultPartyId,
                                                 LocationId.Value,
                                                 out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unkown location!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE)) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unkown EVSE!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnectorId(this OCPIRequest          Request,
                                                           CPOAPI                    CPOAPI,
                                                           out Location_Id?          LocationId,
                                                           out EVSE_UId?             EVSEUId,
                                                           out Connector_Id?         ConnectorId,
                                                           out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId           = default;
            EVSEUId              = default;
            ConnectorId          = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing location, EVSE and/or connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!EVSEUId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnector(this OCPIRequest          Request,
                                                         CPOAPI                    CPOAPI,
                                                         out Location_Id?          LocationId,
                                                         out Location              Location,
                                                         out EVSE_UId?             EVSEUId,
                                                         out EVSE                  EVSE,
                                                         out Connector_Id?         ConnectorId,
                                                         out Connector             Connector,
                                                         out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            ConnectorId          = default;
            Connector            = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing location, EVSE and/or connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!LocationId.HasValue) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[1]);

            if (!EVSEUId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!EVSEUId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(CPOAPI.DefaultCountryCode,
                                                 CPOAPI.DefaultPartyId,
                                                 LocationId.Value,
                                                 out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE)) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown EVSE!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!EVSE.TryGetConnector(ConnectorId.Value, out Connector)) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown connector!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTokenId(this OCPIRequest          Request,
                                           CPOAPI                    CPOAPI,
                                           out CountryCode?          CountryCode,
                                           out Party_Id?             PartyId,
                                           out Token_Id?             TokenId,
                                           out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            TokenId              = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            TokenId = Token_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!TokenId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
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
        /// <param name="TokenStatus">The resolved token with status.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <param name="FailOnMissingToken">Whether to fail when the token for the given token identification was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseToken(this OCPIRequest          Request,
                                         CPOAPI                    CPOAPI,
                                         out CountryCode?          CountryCode,
                                         out Party_Id?             PartyId,
                                         out Token_Id?             TokenId,
                                         out TokenStatus           TokenStatus,
                                         out OCPIResponse.Builder  OCPIResponseBuilder,
                                         Boolean                   FailOnMissingToken = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            TokenId              = default;
            TokenStatus          = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            CountryCode = OCPIv2_2.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!PartyId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            TokenId = Token_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!TokenId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetToken(CountryCode.Value, PartyId.Value, TokenId.Value, out TokenStatus) &&
                FailOnMissingToken)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2004,
                    StatusMessage        = "Unknown token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The HTTP API for charge point operators.
    /// EMSPs will connect to this API.
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

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?     AllowDowngrades       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the HTTP API for charge point operators
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
        public CPOAPI(CommonAPI      CommonAPI,
                      CountryCode    DefaultCountryCode,
                      Party_Id       DefaultPartyId,
                      Boolean?       AllowDowngrades      = null,

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
            this.AllowDowngrades     = AllowDowngrades;

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


            #region ~/locations

            #region OPTIONS  ~/locations

            // https://example.com/ocpi/2.2/cpo/locations/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "locations",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                             });

                                     });

            #endregion

            #region GET      ~/locations

            // https://example.com/ocpi/2.2/cpo/locations/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: Request => {

                                             var filters                 = Request.GetDateAndPaginationFilters();

                                             var allLocations            = CommonAPI.   GetLocations(DefaultCountryCode,
                                                                                                     DefaultPartyId).
                                                                                        ToArray();

                                             var allLocationsCount       = allLocations.Length;


                                             var filteredLocations       = allLocations.Where(location => !filters.From.HasValue || location.LastUpdated >  filters.From.Value).
                                                                                        Where(location => !filters.To.  HasValue || location.LastUpdated <= filters.To.  Value).
                                                                                        ToArray();

                                             var filteredLocationsCount  = filteredLocations.Length;


                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 1000,
                                                        StatusMessage        = "Hello world!",
                                                        Data                 = new JArray(filteredLocations.SkipTakeFilter(filters.Offset,
                                                                                                                           filters.Limit).
                                                                                                            SafeSelect(location => location.ToJSON())),
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                            //LastModified               = ?
                                                        }.
                                                        Set("X-Total-Count", filteredLocationsCount)
                                                        // X-Limit               The maximum number of objects that the server WILL return.
                                                        // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                                 });

                                         });

            #endregion

            #endregion

            #region ~/locations/{locationId}

            #region OPTIONS  ~/locations/{locationId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "locations/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: Request => {

                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                        }
                                                 });

                                         });

            #endregion

            #region GET      ~/locations/{locationId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: Request => {

                                             #region Check location

                                             if (!Request.ParseLocation(this,
                                                                        out Location_Id?          LocationId,
                                                                        out Location              Location,
                                                                        out OCPIResponse.Builder  OCPIResponseBuilder))
                                             {
                                                 return Task.FromResult(OCPIResponseBuilder);
                                             }

                                             #endregion

                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 1000,
                                                        StatusMessage        = "Hello world!",
                                                        Data                 = Location.ToJSON(),
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET",
                                                            AccessControlAllowHeaders  = "Authorization",
                                                            LastModified               = Location.LastUpdated.ToIso8601()
                                                        }
                                                 });

                                         });

            #endregion

            #endregion

            #region ~/locations/{locationId}/{evseId}

            #region OPTIONS  ~/locations/{locationId}/{evseId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "locations/{locationId}/{evseId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                             });

                                     });

            #endregion

            #region GET      ~/locations/{locationId}/{evseId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "locations/{locationId}/{evseId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check EVSE

                                         if (!Request.ParseLocationEVSE(this,
                                                                        out Location_Id?          LocationId,
                                                                        out Location              Location,
                                                                        out EVSE_UId?             EVSEId,
                                                                        out EVSE                  EVSE,
                                                                        out OCPIResponse.Builder  OCPIResponseBuilder))
                                         {
                                             return Task.FromResult(OCPIResponseBuilder);
                                         }

                                         #endregion

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 1000,
                                                    StatusMessage        = "Hello world!",
                                                    Data                 = EVSE.ToJSON(),
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                        LastModified               = EVSE.LastUpdated.ToIso8601()
                                                    }
                                             });

                                     });

            #endregion

            #endregion

            #region ~/locations/{locationId}/{evseId}/{connectorId}

            #region OPTIONS  ~/locations/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                             });

                                     });

            #endregion

            #region GET    ~/locations/{locationId}/{evseId}/{connectorId}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check connector

                                         if (!Request.ParseLocationEVSEConnector(this,
                                                                                 out Location_Id?          LocationId,
                                                                                 out Location              Location,
                                                                                 out EVSE_UId?             EVSEId,
                                                                                 out EVSE                  EVSE,
                                                                                 out Connector_Id?         ConnectorId,
                                                                                 out Connector             Connector,
                                                                                 out OCPIResponse.Builder  OCPIResponseBuilder))
                                         {
                                             return Task.FromResult(OCPIResponseBuilder);
                                         }

                                         #endregion

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 1000,
                                                    StatusMessage        = "Hello world!",
                                                    Data                 = Connector.ToJSON(),
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                        LastModified               = Connector.LastUpdated.ToIso8601()
                                                    }
                                             });

                                     });

            #endregion

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

            #region OPTIONS  ~/tokens/{country_code}/{party_id}      [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "tokens/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                        }
                                                 });

                                     });

            #endregion

            #region GET      ~/tokens/{country_code}/{party_id}      [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "tokens/{country_code}/{party_id}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: Request => {

                                         #region Check country code and party identification

                                         if (!Request.ParseParseCountryCodePartyId(this,
                                                                                   out CountryCode?          CountryCode,
                                                                                   out Party_Id?             PartyId,
                                                                                   out OCPIResponse.Builder  OCPIResponseBuilder))
                                         {
                                             return Task.FromResult(OCPIResponseBuilder);
                                         }

                                         #endregion


                                         var filters              = Request.GetDateAndPaginationFilters();

                                         var allTokens            = CommonAPI.GetTokens(CountryCode, PartyId);

                                         var allTokensCount       = allTokens.Count();


                                         var filteredTokens       = CommonAPI.GetTokens().
                                                                           Where(tokenStatus => !filters.From.HasValue || tokenStatus.Token.LastUpdated >  filters.From.Value).
                                                                           Where(tokenStatus => !filters.To.  HasValue || tokenStatus.Token.LastUpdated <= filters.To.  Value).
                                                                           ToArray();

                                         var filteredTokensCount  = filteredTokens.Count();



                                         return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 1000,
                                                        StatusMessage        = "Hello world!",
                                                        Data                 = new JArray(filteredTokens.SkipTakeFilter(filters.Offset,
                                                                                                                        filters.Limit).
                                                                                                         SafeSelect(token => token.ToJSON())),
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                            //LastModified               = ?
                                                        }.
                                                        Set("X-Total-Count", filteredTokensCount)
                                                        // X-Limit               The maximum number of objects that the server WILL return.
                                                        // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                                 });

                                     });

            #endregion

            #region DELETE   ~/tokens/{country_code}/{party_id}      [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.DELETE,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check country code and party identification

                                             if (!Request.ParseParseCountryCodePartyId(this,
                                                                                       out CountryCode?          CountryCode,
                                                                                       out Party_Id?             PartyId,
                                                                                       out OCPIResponse.Builder  OCPIResponseBuilder))
                                             {
                                                 return OCPIResponseBuilder;
                                             }

                                             #endregion


                                             CommonAPI.RemoveAllTokens(CountryCode.Value,
                                                                       PartyId.    Value);


                                             return new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 1000,
                                                        StatusMessage        = "Hello world!",
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                        }
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
                                     OCPIRequest: Request => {

                                         #region Check token

                                         if (!Request.ParseToken(this,
                                                                 out CountryCode?          CountryCode,
                                                                 out Party_Id?             PartyId,
                                                                 out Token_Id?             TokenId,
                                                                 out TokenStatus           TokenStatus,
                                                                 out OCPIResponse.Builder  OCPIResponseBuilder))
                                         {
                                             return Task.FromResult(OCPIResponseBuilder);
                                         }

                                         #endregion


                                         //ToDo: What exactly to do with this information?
                                         var TokenType  = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;


                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 1000,
                                                    StatusMessage        = "Hello world!",
                                                    Data                 = TokenStatus.Token.ToJSON(),
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                        LastModified               = TokenStatus.Token.LastUpdated.ToIso8601()
                                                    }
                                             });

                                     });

            #endregion

            #region PUT     ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.PUT,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check token

                                             if (!Request.ParseToken(this,
                                                                      out CountryCode?          CountryCode,
                                                                      out Party_Id?             PartyId,
                                                                      out Token_Id?             TokenId,
                                                                      out TokenStatus           ExistingTokenStatus,
                                                                      out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                      FailOnMissingToken: false))
                                             {
                                                 return OCPIResponseBuilder;
                                             }

                                             #endregion

                                             #region Parse new or updated token JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject TokenJSON, out OCPIResponseBuilder))
                                                 return OCPIResponseBuilder;

                                             if (!Token.TryParse(TokenJSON,
                                                                 out Token   newOrUpdatedToken,
                                                                 out String  ErrorResponse,
                                                                 CountryCode,
                                                                 PartyId,
                                                                 TokenId))
                                             {

                                                 return new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 2001,
                                                        StatusMessage        = "Could not parse the given token JSON: " + ErrorResponse,
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                        }
                                                    };

                                             }

                                             #endregion

                                             #region Check whether the new token is "newer" than the existing location

                                             var bbb = Request.QueryString.GetBoolean("forceDowngrade") ??
                                                       // ToDo: Check AccessToken
                                                       AllowDowngrades;

                                             if (AllowDowngrades == false &&
                                                 // ToDo: Check AccessToken
                                                 newOrUpdatedToken.LastUpdated < ExistingTokenStatus.Token.LastUpdated &&
                                                 !Request.QueryString.GetBoolean("forceDowngrade", false))
                                             {

                                                 return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2000,
                                                            StatusMessage        = "LastUpdated must ne newer then the existing one!",
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.FailedDependency,
                                                                AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                                AccessControlAllowHeaders  = "Authorization"
                                                            }
                                                        };

                                             }

                                             #endregion


                                             var wasCreated = CommonAPI.TokenExists(newOrUpdatedToken.CountryCode,
                                                                                    newOrUpdatedToken.PartyId,
                                                                                    newOrUpdatedToken.Id);

                                             //ToDo: What exactly to do with this information?
                                             var TokenType  = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;

                                             CommonAPI.AddOrUpdateToken(newOrUpdatedToken);


                                             return new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 1000,
                                                        StatusMessage        = "Hello world!",
                                                        Data                 = newOrUpdatedToken.ToJSON(),
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = wasCreated
                                                                                             ? HTTPStatusCode.Created
                                                                                             : HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                        }
                                                    };

                                         });

            #endregion

            #region PATCH   ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.PATCH,
                                     URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequest: async Request => {

                                         #region Check token

                                         if (!Request.ParseToken(this,
                                                                  out CountryCode?          CountryCode,
                                                                  out Party_Id?             PartyId,
                                                                  out Token_Id?             TokenId,
                                                                  out TokenStatus           ExistingTokenStatus,
                                                                  out OCPIResponse.Builder  OCPIResponseBuilder))
                                         {
                                             return OCPIResponseBuilder;
                                         }

                                         #endregion

                                         //ToDo: What exactly to do with this information?
                                         var TokenType = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;

                                         #region Parse and apply Token JSON patch

                                         if (!Request.TryParseJObjectRequestBody(out JObject TokenPatch, out OCPIResponseBuilder))
                                             return OCPIResponseBuilder;

                                         #endregion

                                         //ToDo: await..., handle update errors!
                                         var patchedToken = CommonAPI.PatchToken(ExistingTokenStatus.Token,
                                                                                 TokenPatch);


                                         return new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 1000,
                                                    StatusMessage        = "Hello world!",
                                                    Data                 = patchedToken.ToJSON(),
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                };

                                     });

            #endregion

            #region DELETE  ~/tokens/{country_code}/{party_id}/{tokenId}        [NonStandard]

            HTTPServer.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.DELETE,
                                         URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequest: async Request => {

                                             #region Check token

                                             if (!Request.ParseToken(this,
                                                                     out CountryCode?          CountryCode,
                                                                     out Party_Id?             PartyId,
                                                                     out Token_Id?             TokenId,
                                                                     out TokenStatus           ExistingTokenStatus,
                                                                     out OCPIResponse.Builder  OCPIResponseBuilder))
                                             {
                                                 return OCPIResponseBuilder;
                                             }

                                             #endregion


                                             //ToDo: What exactly to do with this information?
                                             var TokenType     = Request.QueryString.TryParseEnum<TokenTypes>("type") ?? TokenTypes.RFID;

                                             var RemovedToken  = CommonAPI.RemoveToken(ExistingTokenStatus.Token);


                                             return new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 1000,
                                                        StatusMessage        = "Hello world!",
                                                        Data                 = ExistingTokenStatus.Token.ToJSON(),
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                            //LastModified               = DateTime.UtcNow.ToIso8601()
                                                        }
                                                    };

                                         });

            #endregion

            #endregion


        }

        #endregion


    }

}
