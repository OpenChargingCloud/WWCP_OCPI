/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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


        #region ParseTariffId               (this Request, CPOAPI, out TariffId,                                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTariffId(this OCPIRequest          Request,
                                            CPOAPI                    CPOAPI,
                                            out Tariff_Id?            TariffId,
                                            out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            TariffId           = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing tariff identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            TariffId = Tariff_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!TariffId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid tariff identification!",
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

        #region ParseTariff                 (this Request, CPOAPI, out TariffId, out Tariff,                                                            out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The Users API.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTariff(this OCPIRequest          Request,
                                          CPOAPI                    CPOAPI,
                                          out Tariff_Id?            TariffId,
                                          out Tariff                Tariff,
                                          out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            TariffId             =  default;
            Tariff               =  default;
            OCPIResponseBuilder  =  default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing tariff identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            TariffId = Tariff_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!TariffId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid tariff identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetTariff(CPOAPI.DefaultCountryCode,
                                               CPOAPI.DefaultPartyId,
                                               TariffId.Value,
                                               out Tariff)) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2003,
                    StatusMessage        = "Unknown tariff!",
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


        #region ParseSessionId              (this Request, CPOAPI, out SessionId,                                                                       out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseSessionId(this OCPIRequest          Request,
                                             CPOAPI                    CPOAPI,
                                             out Session_Id?           SessionId,
                                             out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            SessionId            = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing session identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            SessionId = Session_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!SessionId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid session identification!",
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

        #region ParseSession                (this Request, CPOAPI, out SessionId, out Session,                                                          out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The Users API.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseSession(this OCPIRequest          Request,
                                           CPOAPI                    CPOAPI,
                                           out Session_Id?           SessionId,
                                           out Session               Session,
                                           out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            SessionId            =  default;
            Session              =  default;
            OCPIResponseBuilder  =  default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing session identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            SessionId = Session_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!SessionId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid session identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetSession(CPOAPI.DefaultCountryCode,
                                                CPOAPI.DefaultPartyId,
                                                SessionId.Value,
                                                out Session)) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2003,
                    StatusMessage        = "Unknown session!",
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


        #region ParseCDRId                  (this Request, CPOAPI, out CDRId,                                                                           out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the CDR identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="CDRId">The parsed unique CDR identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCDRId(this OCPIRequest          Request,
                                         CPOAPI                    CPOAPI,
                                         out CDR_Id?               CDRId,
                                         out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            CDRId                = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing CDR identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            CDRId = CDR_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!CDRId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid CDR identification!",
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

        #region ParseCDR                    (this Request, CPOAPI, out CDRId, out CDR,                                                                  out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the CDR identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The Users API.</param>
        /// <param name="CDRId">The parsed unique CDR identification.</param>
        /// <param name="CDR">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCDR(this OCPIRequest          Request,
                                       CPOAPI                    CPOAPI,
                                       out CDR_Id?               CDRId,
                                       out CDR                   CDR,
                                       out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            CDRId                =  default;
            CDR                  =  default;
            OCPIResponseBuilder  =  default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing CDR identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            CDRId = CDR_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!CDRId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid CDR identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetCDR(CPOAPI.DefaultCountryCode,
                                            CPOAPI.DefaultPartyId,
                                            CDRId.Value,
                                            out CDR)) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2003,
                    StatusMessage        = "Unknown CDR!",
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


        #region ParseTokenId                (this Request, CPOAPI, out CountryCode, out PartyId, out TokenId,                                           out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique tariff identification.</param>
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
                    StatusMessage        = "Missing country code, party identification and/or tariff identification!",
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
                    StatusMessage        = "Invalid tariff identification!",
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

        #region ParseToken                  (this Request, CPOAPI, out CountryCode, out PartyId, out TokenId, out Token,                                out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique tariff identification.</param>
        /// <param name="TokenStatus">The resolved tariff with status.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <param name="FailOnMissingToken">Whether to fail when the tariff for the given tariff identification was not found.</param>
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
                    StatusMessage        = "Missing country code, party identification and/or tariff identification!",
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
                    StatusMessage        = "Invalid tariff identification!",
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
                    StatusMessage        = "Unknown tariff identification!",
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


        #region ParseCommandId              (this Request, CPOAPI, out CommandId,                                                                       out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the command identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CPOAPI">The CPO API.</param>
        /// <param name="CommandId">The parsed unique command identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCommandId(this OCPIRequest          Request,
                                             CPOAPI                    CPOAPI,
                                             out Command_Id?           CommandId,
                                             out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  == null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            CommandId            = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing command identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            CommandId = Command_Id.TryParse(Request.ParsedURLParameters[0]);

            if (!CommandId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid command identification!",
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

        #region Events

        #region (protected internal) PutTokenRequest    (Request)

        /// <summary>
        /// An event sent whenever a put token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTokenRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a put token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTokenRequest(DateTime     Timestamp,
                                                HTTPAPI      API,
                                                OCPIRequest  Request)

            => OnPutTokenRequest?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) PutTokenResponse   (Response)

        /// <summary>
        /// An event sent whenever a put token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTokenResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a put token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTokenResponse(DateTime      Timestamp,
                                                 HTTPAPI       API,
                                                 OCPIRequest   Request,
                                                 OCPIResponse  Response)

            => OnPutTokenResponse?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion


        #region (protected internal) PatchTokenRequest  (Request)

        /// <summary>
        /// An event sent whenever a patch token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTokenRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a patch token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTokenRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnPatchTokenRequest?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) PatchTokenResponse (Response)

        /// <summary>
        /// An event sent whenever a patch token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTokenResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a patch token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTokenResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnPatchTokenResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) DeleteTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a delete token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTokenRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a delete token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTokenRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnDeleteTokenRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) DeleteTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a delete token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTokenResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a delete token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTokenResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnDeleteTokenResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        // Commands

        #region (protected internal) ReserveNowRequest        (Request)

        /// <summary>
        /// An event sent whenever a reserve now command was received.
        /// </summary>
        public OCPIRequestLogEvent OnReserveNowRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a reserve now command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnReserveNowRequest?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) ReserveNowResponse       (Response)

        /// <summary>
        /// An event sent whenever a reserve now command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnReserveNowResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a reserve now command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task ReserveNowResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnReserveNowResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) CancelReservationRequest (Request)

        /// <summary>
        /// An event sent whenever a cancel reservation command was received.
        /// </summary>
        public OCPIRequestLogEvent OnCancelReservationRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a cancel reservation command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationRequest(DateTime     Timestamp,
                                                         HTTPAPI      API,
                                                         OCPIRequest  Request)

            => OnCancelReservationRequest?.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request);

        #endregion

        #region (protected internal) CancelReservationResponse(Response)

        /// <summary>
        /// An event sent whenever a cancel reservation command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnCancelReservationResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a cancel reservation command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task CancelReservationResponse(DateTime      Timestamp,
                                                          HTTPAPI       API,
                                                          OCPIRequest   Request,
                                                          OCPIResponse  Response)

            => OnCancelReservationResponse?.WhenAll(Timestamp,
                                                    API ?? this,
                                                    Request,
                                                    Response);

        #endregion


        #region (protected internal) StartSessionRequest      (Request)

        /// <summary>
        /// An event sent whenever a start session command was received.
        /// </summary>
        public OCPIRequestLogEvent OnStartSessionRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a start session command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnStartSessionRequest?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request);

        #endregion

        #region (protected internal) StartSessionResponse     (Response)

        /// <summary>
        /// An event sent whenever a start session command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStartSessionResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a start session command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StartSessionResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnStartSessionResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion


        #region (protected internal) StopSessionRequest      (Request)

        /// <summary>
        /// An event sent whenever a stop session command was received.
        /// </summary>
        public OCPIRequestLogEvent OnStopSessionRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a stop session command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnStopSessionRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) StopSessionResponse     (Response)

        /// <summary>
        /// An event sent whenever a stop session command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStopSessionResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a stop session command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StopSessionResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnStopSessionResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) UnlockConnectorRequest  (Request)

        /// <summary>
        /// An event sent whenever a unlock connector command was received.
        /// </summary>
        public OCPIRequestLogEvent OnUnlockConnectorRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a unlock connector command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnUnlockConnectorRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) UnlockConnectorResponse (Response)

        /// <summary>
        /// An event sent whenever a unlock connector command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnUnlockConnectorResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a unlock connector command response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task UnlockConnectorResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnUnlockConnectorResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion

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
                      Boolean?       AllowDowngrades   = null,

                      HTTPHostname?  HTTPHostname      = null,
                      String         ExternalDNSName   = null,
                      HTTPPath?      URLPathPrefix     = null,
                      HTTPPath?      BasePath          = null,
                      String         ServiceName       = DefaultHTTPServerName)

            : base(CommonAPI?.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   URLPathPrefix ?? DefaultURLPathPrefix,
                   BasePath,
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

            //CommonAPI.AddOCPIMethod(HTTPHostname.Any,
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
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "locations",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

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
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequestHandler: Request => {

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "locations/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequestHandler: Request => {

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "locations/{locationId}",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequestHandler: Request => {

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "locations/{locationId}/{evseId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "locations/{locationId}/{evseId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

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

            #region GET      ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

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


            #region ~/tariffs

            #region OPTIONS  ~/tariffs

            // https://example.com/ocpi/2.2/cpo/tariffs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "tariffs",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

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

            #region GET      ~/tariffs

            // https://example.com/ocpi/2.2/cpo/tariffs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "tariffs",
                                         HTTPContentType.JSON_UTF8,
                                         OCPIRequestHandler: Request => {

                                             var filters               = Request.GetDateAndPaginationFilters();

                                             var allTariffs            = CommonAPI.GetTariffs(DefaultCountryCode,
                                                                                              DefaultPartyId).
                                                                                   ToArray();

                                             var allTariffsCount       = allTariffs.Length;


                                             var filteredTariffs       = allTariffs.Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                                                    Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                                                    ToArray();

                                             var filteredTariffsCount  = filteredTariffs.Length;


                                             return Task.FromResult(
                                                 new OCPIResponse.Builder(Request) {
                                                        StatusCode           = 1000,
                                                        StatusMessage        = "Hello world!",
                                                        Data                 = new JArray(filteredTariffs.SkipTakeFilter(filters.Offset,
                                                                                                                         filters.Limit).
                                                                                                          SafeSelect(tariff => tariff.ToJSON())),
                                                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            AccessControlAllowMethods  = "OPTIONS, GET",
                                                            AccessControlAllowHeaders  = "Authorization"
                                                            //LastModified               = ?
                                                        }.
                                                        Set("X-Total-Count", filteredTariffsCount)
                                                        // X-Limit               The maximum number of objects that the server WILL return.
                                                        // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                                 });

                                         });

            #endregion

            #endregion

            #region ~/tariffs/{tariffId}        [NonStandard]

            #region OPTIONS  ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "tariffs/{tariffId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

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

            #region GET      ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.GET,
                                     URLPathPrefix + "tariffs/{tariffId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

                                         #region Check tariff

                                         if (!Request.ParseTariff(this,
                                                                  out Tariff_Id?            TariffId,
                                                                  out Tariff                Tariff,
                                                                  out OCPIResponse.Builder  OCPIResponseBuilder))
                                         {
                                             return Task.FromResult(OCPIResponseBuilder);
                                         }

                                         #endregion

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 1000,
                                                    StatusMessage        = "Hello world!",
                                                    Data                 = Tariff.ToJSON(),
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization",
                                                        LastModified               = Tariff.LastUpdated.ToIso8601()
                                                    }
                                             });

                                     });

            #endregion

            #endregion


            #region ~/sessions

            #region OPTIONS  ~/sessions

            // https://example.com/ocpi/2.2/cpo/sessions/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "sessions",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

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

            #region GET      ~/sessions

            // https://example.com/ocpi/2.2/cpo/sessions/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters                = Request.GetDateAndPaginationFilters();

                                        var allSessions            = CommonAPI.GetSessions(DefaultCountryCode,
                                                                                           DefaultPartyId).
                                                                               ToArray();

                                        var allSessionsCount       = allSessions.Length;


                                        var filteredSessions       = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                                                 Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                                                 ToArray();

                                        var filteredSessionsCount  = filteredSessions.Length;


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(filteredSessions.SkipTakeFilter(filters.Offset,
                                                                                                                     filters.Limit).
                                                                                                      SafeSelect(session => session.ToJSON())),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", filteredSessionsCount)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #endregion

            #region ~/sessions/{sessionId}      [NonStandard]

            #region OPTIONS  ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "sessions/{sessionId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

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

            #region GET      ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions/{sessionId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check session

                                        if (!Request.ParseSession(this,
                                                                  out Session_Id?           SessionId,
                                                                  out Session               Session,
                                                                  out OCPIResponse.Builder  OCPIResponseBuilder))
                                        {
                                            return Task.FromResult(OCPIResponseBuilder);
                                        }

                                        #endregion

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = Session.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET",
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = Session.LastUpdated.ToIso8601()
                                                   }
                                            });

                                    });

            #endregion

            #endregion

            #region ~/sessions/{session_id}/charging_preferences <= Yet to do!

            #region PUT     ~/sessions/{session_id}/charging_preferences

            // https://example.com/ocpi/2.2/cpo/sessions/12454/charging_preferences

            #endregion

            #endregion


            #region ~/cdrs

            #region OPTIONS  ~/cdrs

            // https://example.com/ocpi/2.2/cpo/CDRs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "CDRs",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

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

            #region GET      ~/cdrs

            // https://example.com/ocpi/2.2/cpo/cdrs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "cdrs",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters            = Request.GetDateAndPaginationFilters();

                                        var allCDRs            = CommonAPI.GetCDRs(DefaultCountryCode,
                                                                                   DefaultPartyId).
                                                                           ToArray();

                                        var allCDRsCount       = allCDRs.Length;


                                        var filteredCDRs       = allCDRs.Where(CDR => !filters.From.HasValue || CDR.LastUpdated >  filters.From.Value).
                                                                         Where(CDR => !filters.To.  HasValue || CDR.LastUpdated <= filters.To.  Value).
                                                                         ToArray();

                                        var filteredCDRsCount  = filteredCDRs.Length;


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(filteredCDRs.SkipTakeFilter(filters.Offset,
                                                                                                                 filters.Limit).
                                                                                                  SafeSelect(CDR => CDR.ToJSON())),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", filteredCDRsCount)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #endregion

            #region ~/cdrs/{CDRId}

            #region OPTIONS  ~/cdrs/{CDRId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "cdrs/{CDRId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

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

            #region GET      ~/cdrs/{CDRId}     // The concrete URL is not specified by OCPI! m(

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "cdrs/{CDRId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check CDR

                                        if (!Request.ParseCDR(this,
                                                              out CDR_Id?               CDRId,
                                                              out CDR                   CDR,
                                                              out OCPIResponse.Builder  OCPIResponseBuilder))
                                        {
                                            return Task.FromResult(OCPIResponseBuilder);
                                        }

                                        #endregion

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = CDR.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET",
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = CDR.LastUpdated.ToIso8601()
                                                   }
                                            });

                                    });

            #endregion

            #endregion


            #region ~/tokens/{country_code}/{party_id}       [NonStandard]

            #region OPTIONS  ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = "OPTIONS, GET",
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

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

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PutTokenRequest,
                                    OCPIResponseLogger:  PutTokenResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

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

                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateToken(newOrUpdatedToken,
                                                                                                 AllowDowngrades: AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (addOrUpdateResult.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = addOrUpdateResult.Data.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization",
                                                           LastModified               = addOrUpdateResult.Data.LastUpdated.ToIso8601(),
                                                           ETag                       = addOrUpdateResult.Data.SHA256Hash
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedToken.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH   ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PatchTokenRequest,
                                    OCPIResponseLogger:  PatchTokenResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

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


                                        // Validation-Checks for PATCHes
                                        // (E-Tag, Timestamp, ...)

                                        var patchedToken = await CommonAPI.TryPatchToken(ExistingTokenStatus.Token,
                                                                                         TokenPatch,
                                                                                         AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (patchedToken.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedToken.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                               AccessControlAllowHeaders  = "Authorization",
                                                               LastModified               = patchedToken.PatchedData.LastUpdated.ToIso8601(),
                                                               ETag                       = patchedToken.PatchedData.SHA256Hash
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedToken.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE  ~/tokens/{country_code}/{party_id}/{tokenId}        [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteTokenRequest,
                                    OCPIResponseLogger:  DeleteTokenResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

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


            // Commands

            #region ~/commands/RESERVE_NOW

            #region OPTIONS  ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/RESERVE_NOW",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region POST     ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/RESERVE_NOW",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:  ReserveNowRequest,
                                    OCPIResponseLogger: ReserveNowResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse ReserveNow command JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!ReserveNowCommand.TryParse(JSONCDR,
                                                                        out ReserveNowCommand  reserveNowCommand,
                                                                        out String             ErrorResponse
                                                                        //CountryCode,
                                                                        //PartyId
                                                                        ))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'RESERVE_NOW' command JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion



                                        // ToDo: ReserveNow!
                                        var commandResponse  = new CommandResponse(
                                                                   CommandResponseTypes.NOT_SUPPORTED,
                                                                   Timeout: TimeSpan.FromSeconds(15),
                                                                   Message: new DisplayText[] {
                                                                                new DisplayText(Languages.en, "Not supported!")
                                                                            }
                                                               );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/CANCEL_RESERVATION

            #region OPTIONS  ~/commands/CANCEL_RESERVATION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/CANCEL_RESERVATION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region POST     ~/commands/CANCEL_RESERVATION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/CANCEL_RESERVATION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:  CancelReservationRequest,
                                    OCPIResponseLogger: CancelReservationResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse CancelReservation command JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!CancelReservationCommand.TryParse(JSONCDR,
                                                                               out CancelReservationCommand  cancelReservationCommand,
                                                                               out String                    ErrorResponse
                                                                               //CountryCode,
                                                                               //PartyId
                                                                               ))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'CANCEL_RESERVATION' command JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        // ToDo: ReserveNow!
                                        var commandResponse  = new CommandResponse(
                                                                   CommandResponseTypes.NOT_SUPPORTED,
                                                                   Timeout: TimeSpan.FromSeconds(15),
                                                                   Message: new DisplayText[] {
                                                                                new DisplayText(Languages.en, "Not supported!")
                                                                            }
                                                               );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/START_SESSION

            #region OPTIONS  ~/commands/START_SESSION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/START_SESSION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region POST     ~/commands/START_SESSION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/START_SESSION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:  StartSessionRequest,
                                    OCPIResponseLogger: StartSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse StartSession command JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!StartSessionCommand.TryParse(JSONCDR,
                                                                          out StartSessionCommand  startSessionCommand,
                                                                          out String               ErrorResponse
                                                                          //CountryCode,
                                                                          //PartyId
                                                                          ))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'START_SESSION' command JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        // ToDo: ReserveNow!
                                        var commandResponse  = new CommandResponse(
                                                                   CommandResponseTypes.NOT_SUPPORTED,
                                                                   Timeout: TimeSpan.FromSeconds(15),
                                                                   Message: new DisplayText[] {
                                                                                new DisplayText(Languages.en, "Not supported!")
                                                                            }
                                                               );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/STOP_SESSION

            #region OPTIONS  ~/commands/STOP_SESSION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/STOP_SESSION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region POST     ~/commands/STOP_SESSION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/STOP_SESSION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:  StopSessionRequest,
                                    OCPIResponseLogger: StopSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse StopSession command JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!StopSessionCommand.TryParse(JSONCDR,
                                                                          out StopSessionCommand  stopSessionCommand,
                                                                          out String              ErrorResponse
                                                                          //CountryCode,
                                                                          //PartyId
                                                                          ))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'STOP_SESSION' command JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        // ToDo: ReserveNow!
                                        var commandResponse  = new CommandResponse(
                                                                   CommandResponseTypes.NOT_SUPPORTED,
                                                                   Timeout: TimeSpan.FromSeconds(15),
                                                                   Message: new DisplayText[] {
                                                                                new DisplayText(Languages.en, "Not supported!")
                                                                            }
                                                               );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/UNLOCK_CONNECTOR

            #region OPTIONS  ~/commands/UNLOCK_CONNECTOR

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/UNLOCK_CONNECTOR",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region POST     ~/commands/UNLOCK_CONNECTOR

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/UNLOCK_CONNECTOR",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:  UnlockConnectorRequest,
                                    OCPIResponseLogger: UnlockConnectorResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.EMSP) ||
                                            Request.AccessInfo.Value.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = "OPTIONS, GET",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse UnlockConnector command JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!UnlockConnectorCommand.TryParse(JSONCDR,
                                                                             out UnlockConnectorCommand  unlockConnectorCommand,
                                                                             out String                  ErrorResponse
                                                                             //CountryCode,
                                                                             //PartyId
                                                                             ))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'UNLOCK_CONNECTOR' command JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        // ToDo: ReserveNow!
                                        var commandResponse  = new CommandResponse(
                                                                   CommandResponseTypes.NOT_SUPPORTED,
                                                                   Timeout: TimeSpan.FromSeconds(15),
                                                                   Message: new DisplayText[] {
                                                                                new DisplayText(Languages.en, "Not supported!")
                                                                            }
                                                               );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion


        }

        #endregion

    }

}
