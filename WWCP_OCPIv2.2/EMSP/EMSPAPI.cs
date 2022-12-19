﻿/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using org.GraphDefined.Vanaheimr.Hermod.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Extension methods for the EMSP HTTP API.
    /// </summary>
    public static class EMSPAPIExtensions
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
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCountryCodeAndPartyId(this OCPIRequest          Request,
                                                         EMSPAPI                   EMSPAPI,
                                                         out CountryCode?          CountryCode,
                                                         out Party_Id?             PartyId,
                                                         out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

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


        #region ParseLocationId             (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId,                                                                      out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationId(this OCPIRequest          Request,
                                              EMSPAPI                   EMSPAPI,
                                              out CountryCode?          CountryCode,
                                              out Party_Id?             PartyId,
                                              out Location_Id?          LocationId,
                                              out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or location identification!",
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

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[2]);

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

        #region ParseLocation               (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <param name="FailOnMissingLocation">Whether to fail when the location for the given location identification was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocation(this OCPIRequest          Request,
                                            EMSPAPI                   EMSPAPI,
                                            out CountryCode?          CountryCode,
                                            out Party_Id?             PartyId,
                                            out Location_Id?          LocationId,
                                            out Location              Location,
                                            out OCPIResponse.Builder  OCPIResponseBuilder,
                                            Boolean                   FailOnMissingLocation = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            Location             = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or location identification!",
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

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[2]);

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


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location) &&
                 FailOnMissingLocation)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location identification!",
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


        #region ParseLocationEVSEId         (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,                                           out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEId(this OCPIRequest          Request,
                                                  EMSPAPI                   EMSPAPI,
                                                  out CountryCode?          CountryCode,
                                                  out Party_Id?             PartyId,
                                                  out Location_Id?          LocationId,
                                                  out EVSE_UId?             EVSEUId,
                                                  out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            EVSEUId              = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 4)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification, location identification and/or EVSE identification!",
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

        #region ParseLocationEVSE           (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <param name="FailOnMissingEVSE">Whether to fail when the location for the given EVSE identification was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSE(this OCPIRequest          Request,
                                                EMSPAPI                   EMSPAPI,
                                                out CountryCode?          CountryCode,
                                                out Party_Id?             PartyId,
                                                out Location_Id?          LocationId,
                                                out Location              Location,
                                                out EVSE_UId?             EVSEUId,
                                                out EVSE                  EVSE,
                                                out OCPIResponse.Builder  OCPIResponseBuilder,
                                                Boolean                   FailOnMissingEVSE = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 4)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification, location identification and/or EVSE identification!",
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

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[2]);

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

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[3]);

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


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value,
                                                  PartyId.    Value,
                                                  LocationId. Value, out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE) &&
                 FailOnMissingEVSE)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown EVSE identification!",
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


        #region ParseLocationEVSEConnectorId(this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId,               out EVSEUId,           out ConnectorId,                out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseLocationEVSEConnectorId(this OCPIRequest          Request,
                                                           EMSPAPI                   EMSPAPI,
                                                           out CountryCode?          CountryCode,
                                                           out Party_Id?             PartyId,
                                                           out Location_Id?          LocationId,
                                                           out EVSE_UId?             EVSEUId,
                                                           out Connector_Id?         ConnectorId,
                                                           out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            EVSEUId              = default;
            ConnectorId          = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 5)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification, location identification, EVSE identification and/or connector identification!",
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

        #region ParseLocationEVSEConnector  (this Request, EMSPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <param name="FailOnMissingConnector">Whether to fail when the connector for the given connector identification was not found.</param>
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
                                                         out OCPIResponse.Builder  OCPIResponseBuilder,
                                                         Boolean                   FailOnMissingConnector = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            ConnectorId          = default;
            Connector            = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 5)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification, location identification, EVSE identification and/or connector identification!",
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

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[2]);

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

            EVSEUId = EVSE_UId.TryParse(Request.ParsedURLParameters[3]);

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

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[4]);

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


            if (!EMSPAPI.CommonAPI.TryGetLocation(CountryCode.Value, PartyId.Value, LocationId.Value, out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!EVSE.TryGetConnector(ConnectorId.Value, out Connector) &&
                FailOnMissingConnector)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown connector identification!",
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



        #region ParseTariffId               (this Request, EMSPAPI, out CountryCode, out PartyId, out TariffId,                out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTariffId(this OCPIRequest          Request,
                                            EMSPAPI                   EMSPAPI,
                                            out CountryCode?          CountryCode,
                                            out Party_Id?             PartyId,
                                            out Tariff_Id?            TariffId,
                                            out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            TariffId             = default;
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

            TariffId = Tariff_Id.TryParse(Request.ParsedURLParameters[2]);

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

        #region ParseTariff                 (this Request, EMSPAPI, out CountryCode, out PartyId, out TariffId,  out Tariff,   out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <param name="FailOnMissingTariff">Whether to fail when the tariff for the given tariff identification was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTariff(this OCPIRequest          Request,
                                          EMSPAPI                   EMSPAPI,
                                          out CountryCode?          CountryCode,
                                          out Party_Id?             PartyId,
                                          out Tariff_Id?            TariffId,
                                          out Tariff                Tariff,
                                          out OCPIResponse.Builder  OCPIResponseBuilder,
                                          Boolean                   FailOnMissingTariff = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            TariffId             = default;
            Tariff               = default;
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

            TariffId = Tariff_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!TariffId.HasValue) {

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


            if (!EMSPAPI.CommonAPI.TryGetTariff(CountryCode.Value, PartyId.Value, TariffId.Value, out Tariff) &&
                 FailOnMissingTariff)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
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


        #region ParseSessionId              (this Request, EMSPAPI, out CountryCode, out PartyId, out SessionId,               out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseSessionId(this OCPIRequest          Request,
                                             EMSPAPI                   EMSPAPI,
                                             out CountryCode?          CountryCode,
                                             out Party_Id?             PartyId,
                                             out Session_Id?           SessionId,
                                             out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            SessionId            = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or session identification!",
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

            SessionId = Session_Id.TryParse(Request.ParsedURLParameters[2]);

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

        #region ParseSession                (this Request, EMSPAPI, out CountryCode, out PartyId, out SessionId, out Session,  out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <param name="FailOnMissingSession">Whether to fail when the session for the given session identification was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseSession(this OCPIRequest         Request,
                                          EMSPAPI                   EMSPAPI,
                                          out CountryCode?          CountryCode,
                                          out Party_Id?             PartyId,
                                          out Session_Id?           SessionId,
                                          out Session               Session,
                                          out OCPIResponse.Builder  OCPIResponseBuilder,
                                          Boolean                   FailOnMissingSession = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            SessionId            = default;
            Session              = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or session identification!",
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

            SessionId = Session_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!SessionId.HasValue) {

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


            if (!EMSPAPI.CommonAPI.TryGetSession(CountryCode.Value, PartyId.Value, SessionId.Value, out Session) &&
                FailOnMissingSession)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown session identification!",
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


        #region ParseCDRId                  (this Request, EMSPAPI, out CountryCode, out PartyId, out CDRId,                   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCDRId(this OCPIRequest          Request,
                                         EMSPAPI                   EMSPAPI,
                                         out CountryCode?          CountryCode,
                                         out Party_Id?             PartyId,
                                         out CDR_Id?               CDRId,
                                         out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            CDRId                = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or charge detail record identification!",
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

            CDRId = CDR_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!CDRId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid charge detail record identification!",
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

        #region ParseCDR                    (this Request, EMSPAPI, out CountryCode, out PartyId, out CDRId,     out CDR,      out OCPIResponseBuilder)

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
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <param name="FailOnMissingCDR">Whether to fail when the charge detail record for the given charge detail record identification was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCDR(this OCPIRequest          Request,
                                       EMSPAPI                   EMSPAPI,
                                       out CountryCode?          CountryCode,
                                       out Party_Id?             PartyId,
                                       out CDR_Id?               CDRId,
                                       out CDR                   CDR,
                                       out OCPIResponse.Builder  OCPIResponseBuilder,
                                       Boolean                   FailOnMissingCDR = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI is null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            CountryCode          = default;
            PartyId              = default;
            CDRId                = default;
            CDR                  = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or charge detail record identification!",
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

            CDRId = CDR_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!CDRId.HasValue) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid charge detail record identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!EMSPAPI.CommonAPI.TryGetCDR(CountryCode.Value, PartyId.Value, CDRId.Value, out CDR) &&
                FailOnMissingCDR)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown charge detail record identification!",
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



        #region ParseTokenId                (this Request, EMSPAPI,                               out TokenId,                 out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseTokenId(this OCPIRequest          Request,
                                           EMSPAPI                   EMSPAPI,
                                           out Token_Id?             TokenId,
                                           out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            TokenId              = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            TokenId = Token_Id.TryParse(Request.ParsedURLParameters[0]);

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

        #region ParseToken                  (this Request, EMSPAPI,                               out TokenId,   out Token,    out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The Users API.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="TokenStatus">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OICP response builder.</param>
        /// <param name="FailOnMissingToken">Whether to fail when the token for the given token identification was not found.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseToken(this OCPIRequest          Request,
                                         EMSPAPI                   EMSPAPI,
                                         out Token_Id?             TokenId,
                                         out TokenStatus           TokenStatus,
                                         out OCPIResponse.Builder  OCPIResponseBuilder,
                                         Boolean                   FailOnMissingToken = true)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given EMSP API must not be null!");

            #endregion

            TokenId              = default;
            TokenStatus          = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = "OPTIONS, GET, POST, PUT, DELETE",
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            TokenId = Token_Id.TryParse(Request.ParsedURLParameters[0]);

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


            if (!EMSPAPI.CommonAPI.TryGetToken(Request.ToCountryCode ?? EMSPAPI.DefaultCountryCode,
                                               Request.ToPartyId     ?? EMSPAPI.DefaultPartyId,
                                               TokenId.Value,
                                               out TokenStatus) &&
                FailOnMissingToken)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
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



        #region ParseCommandId              (this Request, EMSPAPI, out CommandId,                                                                     out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the command identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="EMSPAPI">The EMSP API.</param>
        /// <param name="CommandId">The parsed unique command identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseCommandId(this OCPIRequest          Request,
                                             EMSPAPI                   EMSPAPI,
                                             out Command_Id?           CommandId,
                                             out OCPIResponse.Builder  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request == null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (EMSPAPI  == null)
                throw new ArgumentNullException(nameof(EMSPAPI),  "The given CPO API must not be null!");

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

        protected Newtonsoft.Json.Formatting JSONFormat = Newtonsoft.Json.Formatting.Indented;

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

                /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan     RequestTimeout        { get; set; }

        #endregion

        #region Events

        #region Locations

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationsResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutLocationResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchLocationResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnDeleteLocationResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion

        #endregion

        #region EVSEs

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutEVSEResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchEVSEResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteEVSEResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnDeleteEVSEResponse?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion

        #endregion

        #region Connectors

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutConnectorResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchConnectorResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteConnectorResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnDeleteConnectorResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion

        #endregion

        #region Tariffs

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffsResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTariffResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPutTariffResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) PatchTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a patch tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTariffRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a patch tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTariffRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnPatchTariffRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) PatchTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a patch tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTariffResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a patch tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTariffResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnPatchTariffResponse?.WhenAll(Timestamp,
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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnDeleteTariffResponse?.WhenAll(Timestamp,
                                               API ?? this,
                                               Request,
                                               Response);

        #endregion

        #endregion

        #region Sessions

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutSessionResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchSessionResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionResponse(DateTime      Timestamp,
                                                      HTTPAPI       API,
                                                      OCPIRequest   Request,
                                                      OCPIResponse  Response)

            => OnDeleteSessionResponse?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion

        #endregion

        #region CDRs

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRsResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCDRResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnDeleteCDRResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion

        #endregion

        #region Tokens

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
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTokenResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPostTokenResponse?.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion

        #endregion




        public delegate Task<AuthorizationInfo> OnRFIDAuthTokenDelegate(CountryCode         From_CountryCode,
                                                                        Party_Id            From_PartyId,
                                                                        CountryCode         To_CountryCode,
                                                                        Party_Id            To_PartyId,
                                                                        Token_Id            TokenId,
                                                                        LocationReference?  LocationReference);

        public event OnRFIDAuthTokenDelegate OnRFIDAuthToken;



        // Command callbacks

        #region (protected internal) ReserveNowCallbackRequest        (Request)

        /// <summary>
        /// An event sent whenever a reserve now callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnReserveNowCallbackRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a reserve now callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowCallbackRequest(DateTime     Timestamp,
                                                          HTTPAPI      API,
                                                          OCPIRequest  Request)

            => OnReserveNowCallbackRequest?.WhenAll(Timestamp,
                                                    API ?? this,
                                                    Request);

        #endregion

        #region (protected internal) ReserveNowCallbackResponse       (Response)

        /// <summary>
        /// An event sent whenever a reserve now callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnReserveNowCallbackResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a reserve now callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task ReserveNowCallbackResponse(DateTime      Timestamp,
                                                           HTTPAPI       API,
                                                           OCPIRequest   Request,
                                                           OCPIResponse  Response)

            => OnReserveNowCallbackResponse?.WhenAll(Timestamp,
                                                     API ?? this,
                                                     Request,
                                                     Response);

        #endregion


        #region (protected internal) CancelReservationCallbackRequest (Request)

        /// <summary>
        /// An event sent whenever a cancel reservation callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnCancelReservationCallbackRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a cancel reservation callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationCallbackRequest(DateTime     Timestamp,
                                                                 HTTPAPI      API,
                                                                 OCPIRequest  Request)

            => OnCancelReservationCallbackRequest?.WhenAll(Timestamp,
                                                           API ?? this,
                                                           Request);

        #endregion

        #region (protected internal) CancelReservationCallbackResponse(Response)

        /// <summary>
        /// An event sent whenever a cancel reservation callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnCancelReservationCallbackResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a cancel reservation callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task CancelReservationCallbackResponse(DateTime      Timestamp,
                                                                  HTTPAPI       API,
                                                                  OCPIRequest   Request,
                                                                  OCPIResponse  Response)

            => OnCancelReservationCallbackResponse?.WhenAll(Timestamp,
                                                            API ?? this,
                                                            Request,
                                                            Response);

        #endregion


        #region (protected internal) StartSessionCallbackRequest      (Request)

        /// <summary>
        /// An event sent whenever a start session callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnStartSessionCallbackRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a start session callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionCallbackRequest(DateTime     Timestamp,
                                                            HTTPAPI      API,
                                                            OCPIRequest  Request)

            => OnStartSessionCallbackRequest?.WhenAll(Timestamp,
                                                      API ?? this,
                                                      Request);

        #endregion

        #region (protected internal) StartSessionCallbackResponse     (Response)

        /// <summary>
        /// An event sent whenever a start session callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStartSessionCallbackResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a start session callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StartSessionCallbackResponse(DateTime      Timestamp,
                                                             HTTPAPI       API,
                                                             OCPIRequest   Request,
                                                             OCPIResponse  Response)

            => OnStartSessionCallbackResponse?.WhenAll(Timestamp,
                                                       API ?? this,
                                                       Request,
                                                       Response);

        #endregion


        #region (protected internal) StopSessionCallbackRequest       (Request)

        /// <summary>
        /// An event sent whenever a stop session callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnStopSessionCallbackRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a stop session callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionCallbackRequest(DateTime     Timestamp,
                                                           HTTPAPI      API,
                                                           OCPIRequest  Request)

            => OnStopSessionCallbackRequest?.WhenAll(Timestamp,
                                                     API ?? this,
                                                     Request);

        #endregion

        #region (protected internal) StopSessionCallbackResponse      (Response)

        /// <summary>
        /// An event sent whenever a stop session callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStopSessionCallbackResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a stop session callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StopSessionCallbackResponse(DateTime      Timestamp,
                                                            HTTPAPI       API,
                                                            OCPIRequest   Request,
                                                            OCPIResponse  Response)

            => OnStopSessionCallbackResponse?.WhenAll(Timestamp,
                                                      API ?? this,
                                                      Request,
                                                      Response);

        #endregion


        #region (protected internal) UnlockConnectorCallbackRequest   (Request)

        /// <summary>
        /// An event sent whenever a unlock connector callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnUnlockConnectorCallbackRequest = new OCPIRequestLogEvent();

        /// <summary>
        /// An event sent whenever a unlock connector callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorCallbackRequest(DateTime     Timestamp,
                                                               HTTPAPI      API,
                                                               OCPIRequest  Request)

            => OnUnlockConnectorCallbackRequest?.WhenAll(Timestamp,
                                                         API ?? this,
                                                         Request);

        #endregion

        #region (protected internal) UnlockConnectorCallbackResponse  (Response)

        /// <summary>
        /// An event sent whenever a unlock connector callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnUnlockConnectorCallbackResponse = new OCPIResponseLogEvent();

        /// <summary>
        /// An event sent whenever a unlock connector callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task UnlockConnectorCallbackResponse(DateTime      Timestamp,
                                                                HTTPAPI       API,
                                                                OCPIRequest   Request,
                                                                OCPIResponse  Response)

            => OnUnlockConnectorCallbackResponse?.WhenAll(Timestamp,
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
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public EMSPAPI(CommonAPI                CommonAPI,
                       CountryCode              DefaultCountryCode,
                       Party_Id                 DefaultPartyId,
                       Boolean?                 AllowDowngrades      = null,

                       HTTPHostname?            HTTPHostname         = null,
                       String                   ExternalDNSName      = null,
                       HTTPPath?                URLPathPrefix        = null,
                       HTTPPath?                BasePath             = null,
                       String                   HTTPServiceName      = DefaultHTTPServerName,

                       Boolean?                 IsDevelopment        = false,
                       IEnumerable<String>?     DevelopmentServers   = null,
                       Boolean?                 DisableLogging       = false,
                       String?                  LoggingPath          = null,
                       String?                  LogfileName          = null,
                       LogfileCreatorDelegate?  LogfileCreator       = null)

            : base(CommonAPI?.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName,
                   BasePath,

                   URLPathPrefix ?? DefaultURLPathPrefix,
                   null, // HTMLTemplate,
                   null, // APIVersionHashes,

                   null, // DisableMaintenanceTasks,
                   null, // MaintenanceInitialDelay,
                   null, // MaintenanceEvery,

                   null, // DisableWardenTasks,
                   null, // WardenInitialDelay,
                   null, // WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator,
                   false) // Autostart

        {

            this.CommonAPI           = CommonAPI ?? throw new ArgumentNullException(nameof(CommonAPI), "The given CommonAPI must not be null!");
            this.DefaultCountryCode  = DefaultCountryCode;
            this.DefaultPartyId      = DefaultPartyId;
            this.AllowDowngrades     = AllowDowngrades;
            this.RequestTimeout      = TimeSpan.FromSeconds(60);

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

            //CommonAPI.AddOCPIMethod(HTTPHostname.Any,
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
            //                                     Date            = Timestamp.Now,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = "close"
            //                                 };

            //                             });

            #endregion


            // Receiver Interface for eMSPs and NSPs

            #region ~/locations/{country_code}/{party_id}                               [NonStandard]

            #region OPTIONS  ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "locations/{country_code}/{party_id}",
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

            #region GET      ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return Task.FromResult(OCPIResponse);
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
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(filteredLocations.SkipTakeFilter(filters.Offset,
                                                                                                                      filters.Limit).
                                                                                                       SafeSelect(location => location.ToJSON())),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", filteredLocationsCount)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteLocationsRequest,
                                    OCPIResponseLogger:  DeleteLocationsResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check CountryCode & PartyId

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion


                                        //ToDo: await...
                                        CommonAPI.RemoveAllLocations(CountryCode.Value,
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

            #region ~/locations/{country_code}/{party_id}/{locationId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET,
                                                                                        HTTPMethod.PUT,
                                                                                        HTTPMethod.PATCH,
                                                                                        HTTPMethod.DELETE
                                                                                    },
                                                       AcceptPatch                = new List<HTTPContentType> {
                                                                                        HTTPContentType.JSONMergePatch_UTF8
                                                                                    },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check location

                                        if (!Request.ParseLocation(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Location_Id?          LocationId,
                                                                   out Location              Location,
                                                                   out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                   FailOnMissingLocation: true))
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
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = Location.LastUpdated.ToIso8601(),
                                                       ETag                       = Location.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PutLocationRequest,
                                    OCPIResponseLogger:  PutLocationResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing location

                                        if (!Request.ParseLocation(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Location_Id?          LocationId,
                                                                   out Location              ExistingLocation,
                                                                   out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                   FailOnMissingLocation: false))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated location JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject LocationJSON, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        if (!Location.TryParse(LocationJSON,
                                                               out Location  newOrUpdatedLocation,
                                                               out String    ErrorResponse,
                                                               CountryCode,
                                                               PartyId,
                                                               LocationId))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given location JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateLocation(newOrUpdatedLocation,
                                                                                                    AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));

                                        //throw new Exception("lala!");
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
                                                           ETag                       = addOrUpdateResult.Data.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedLocation.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PatchLocationRequest,
                                    OCPIResponseLogger:  PatchLocationResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check location

                                        if (!Request.ParseLocation(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Location_Id?          LocationId,
                                                                   out Location              ExistingLocation,
                                                                   out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                   FailOnMissingLocation: true))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse location JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out JObject LocationPatch, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        #endregion


                                        // Validation-Checks for PATCHes
                                        // (E-Tag, Timestamp, ...)

                                        var patchedLocation = await CommonAPI.TryPatchLocation(ExistingLocation,
                                                                                               LocationPatch);

                                        //ToDo: Handle update errors!
                                        if (patchedLocation.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedLocation.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                               AccessControlAllowHeaders  = "Authorization",
                                                               LastModified               = patchedLocation.PatchedData.LastUpdated.ToIso8601(),
                                                               ETag                       = patchedLocation.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedLocation.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteLocationRequest,
                                    OCPIResponseLogger:  DeleteLocationResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing location

                                        if (!Request.ParseLocation(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Location_Id?          LocationId,
                                                                   out Location              ExistingLocation,
                                                                   out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion


                                        //ToDo: await...
                                        CommonAPI.RemoveLocation(ExistingLocation);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = ExistingLocation.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                    });

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET,
                                                                                        HTTPMethod.PUT,
                                                                                        HTTPMethod.PATCH,
                                                                                        HTTPMethod.DELETE
                                                                                    },
                                                       AcceptPatch                = new List<HTTPContentType> {
                                                                                        HTTPContentType.JSONMergePatch_UTF8
                                                                                    },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check EVSE

                                        if (!Request.ParseLocationEVSE(this,
                                                                       out CountryCode?          CountryCode,
                                                                       out Party_Id?             PartyId,
                                                                       out Location_Id?          LocationId,
                                                                       out Location              Location,
                                                                       out EVSE_UId?             EVSEUId,
                                                                       out EVSE                  EVSE,
                                                                       out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return Task.FromResult(OCPIResponse);
                                        }

                                        #endregion

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = EVSE.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = EVSE.LastUpdated.ToIso8601(),
                                                       ETag                       = EVSE.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PutEVSERequest,
                                    OCPIResponseLogger:  PutEVSEResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing EVSE

                                        if (!Request.ParseLocationEVSE(this,
                                                                       out CountryCode?          CountryCode,
                                                                       out Party_Id?             PartyId,
                                                                       out Location_Id?          LocationId,
                                                                       out Location              ExistingLocation,
                                                                       out EVSE_UId?             EVSEUId,
                                                                       out EVSE                  ExistingEVSE,
                                                                       out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                       FailOnMissingEVSE: false))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated EVSE JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject EVSEJSON, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        if (!EVSE.TryParse(EVSEJSON,
                                                           out EVSE    newOrUpdatedEVSE,
                                                           out String  ErrorResponse,
                                                           EVSEUId))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given EVSE JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateEVSE(ExistingLocation,
                                                                                                newOrUpdatedEVSE,
                                                                                                AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


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
                                                           LastModified               = newOrUpdatedEVSE.LastUpdated.ToIso8601(),
                                                           ETag                       = newOrUpdatedEVSE.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedEVSE.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PatchEVSERequest,
                                    OCPIResponseLogger:  PatchEVSEResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check EVSE

                                        if (!Request.ParseLocationEVSE(this,
                                                                       out CountryCode?          CountryCode,
                                                                       out Party_Id?             PartyId,
                                                                       out Location_Id?          LocationId,
                                                                       out Location              ExistingLocation,
                                                                       out EVSE_UId?             EVSEUId,
                                                                       out EVSE                  ExistingEVSE,
                                                                       out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                       FailOnMissingEVSE: true))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse EVSE JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out JObject EVSEPatch, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        #endregion


                                        var patchedEVSE = await CommonAPI.TryPatchEVSE(ExistingLocation,
                                                                                       ExistingEVSE,
                                                                                       EVSEPatch);

                                        //ToDo: Handle update errors!
                                        if (patchedEVSE.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedEVSE.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                               AccessControlAllowHeaders  = "Authorization",
                                                               LastModified               = patchedEVSE.PatchedData.LastUpdated.ToIso8601(),
                                                               ETag                       = patchedEVSE.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedEVSE.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}/{evseId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteEVSERequest,
                                    OCPIResponseLogger:  DeleteEVSEResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing Location/EVSE(UId URI parameter)

                                        if (!Request.ParseLocationEVSE(this,
                                                                       out CountryCode?          CountryCode,
                                                                       out Party_Id?             PartyId,
                                                                       out Location_Id?          LocationId,
                                                                       out Location              ExistingLocation,
                                                                       out EVSE_UId?             EVSEUId,
                                                                       out EVSE                  ExistingEVSE,
                                                                       out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion


                                        //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = ExistingEVSE.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                    });

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET,
                                                                                        HTTPMethod.PUT,
                                                                                        HTTPMethod.PATCH,
                                                                                        HTTPMethod.DELETE
                                                                                    },
                                                       AcceptPatch                = new List<HTTPContentType> {
                                                                                        HTTPContentType.JSONMergePatch_UTF8
                                                                                    },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return Task.FromResult(OCPIResponse);
                                        }

                                        #endregion

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = Connector.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = Connector.LastUpdated.ToIso8601(),
                                                       ETag                       = Connector.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PutConnectorRequest,
                                    OCPIResponseLogger:  PutConnectorResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check connector

                                        if (!Request.ParseLocationEVSEConnector(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out Location_Id?          LocationId,
                                                                                out Location              ExistingLocation,
                                                                                out EVSE_UId?             EVSEUId,
                                                                                out EVSE                  ExistingEVSE,
                                                                                out Connector_Id?         ConnectorId,
                                                                                out Connector             ExistingConnector,
                                                                                out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                                FailOnMissingConnector: false))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated connector JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject ConnectorJSON, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        if (!Connector.TryParse(ConnectorJSON,
                                                                out Connector  newOrUpdatedConnector,
                                                                out String     ErrorResponse,
                                                                ConnectorId))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given connector JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateConnector(ExistingLocation,
                                                                                                     ExistingEVSE,
                                                                                                     newOrUpdatedConnector,
                                                                                                     AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


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
                                                           LastModified               = newOrUpdatedConnector.LastUpdated.ToIso8601(),
                                                           ETag                       = newOrUpdatedConnector.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedConnector.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PatchConnectorRequest,
                                    OCPIResponseLogger:  PatchConnectorResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check connector

                                        if (!Request.ParseLocationEVSEConnector(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out Location_Id?          LocationId,
                                                                                out Location              ExistingLocation,
                                                                                out EVSE_UId?             EVSEUId,
                                                                                out EVSE                  ExistingEVSE,
                                                                                out Connector_Id?         ConnectorId,
                                                                                out Connector             ExistingConnector,
                                                                                out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                                FailOnMissingConnector: true))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse connector JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out JObject ConnectorPatch, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        #endregion


                                        var patchedConnector = await CommonAPI.TryPatchConnector(ExistingLocation,
                                                                                                 ExistingEVSE,
                                                                                                 ExistingConnector,
                                                                                                 ConnectorPatch);

                                        //ToDo: Handle update errors!
                                        if (patchedConnector.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedConnector.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                               AccessControlAllowHeaders  = "Authorization",
                                                               LastModified               = patchedConnector.PatchedData.LastUpdated.ToIso8601(),
                                                               ETag                       = patchedConnector.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedConnector.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/{connectorId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteConnectorRequest,
                                    OCPIResponseLogger:  DeleteConnectorResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion


                                        //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = ExistingConnector.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                    });

            #endregion

            #endregion


            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/status  [NonStandard]

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/status",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.POST
                                                                                    },
                                                       AcceptPatch                = new List<HTTPContentType> {
                                                                                        HTTPContentType.JSONMergePatch_UTF8
                                                                                    },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region POST     ~/locations/{country_code}/{party_id}/{locationId}/{evseId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseId}/status",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PutEVSERequest,
                                    OCPIResponseLogger:  PutEVSEResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing EVSE

                                        if (!Request.ParseLocationEVSE(this,
                                                                       out CountryCode?          CountryCode,
                                                                       out Party_Id?             PartyId,
                                                                       out Location_Id?          LocationId,
                                                                       out Location              ExistingLocation,
                                                                       out EVSE_UId?             EVSEUId,
                                                                       out EVSE                  ExistingEVSE,
                                                                       out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                       FailOnMissingEVSE: false))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse EVSE status JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject EVSEStatusJSON, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        //if (!EVSE.TryParse(EVSEJSON,
                                        //                   out EVSE    newOrUpdatedEVSE,
                                        //                   out String  ErrorResponse,
                                        //                   EVSEUId))
                                        //{

                                        //    return new OCPIResponse.Builder(Request) {
                                        //               StatusCode           = 2001,
                                        //               StatusMessage        = "Could not parse the given EVSE JSON: " + ErrorResponse,
                                        //               HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                        //                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                        //                   AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                        //                   AccessControlAllowHeaders  = "Authorization"
                                        //               }
                                        //           };

                                        //}

                                        #endregion

                                        //ToDo: Handle AddOrUpdate errors

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       //Data                 = newOrUpdatedEVSE.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                    });

            #endregion

            #endregion



            #region ~/tariffs/{country_code}/{party_id}                                 [NonStandard]

            #region OPTIONS  ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "tariffs/{country_code}/{party_id}",
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

            #region GET      ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tariffs/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return Task.FromResult(OCPIResponse);
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
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(filteredTariffs.SkipTakeFilter(filters.Offset,
                                                                                                                    filters.Limit).
                                                                                                     SafeSelect(tariff => tariff.ToJSON())),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", filteredTariffsCount)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region DELETE   ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tariffs/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteTariffsRequest,
                                    OCPIResponseLogger:  DeleteTariffsResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check CountryCode & PartyId

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion


                                        CommonAPI.RemoveAllTariffs(CountryCode.Value,
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

            #region ~/tariffs/{country_code}/{party_id}/{tariffId}

            #region OPTIONS  ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, DELETE",
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET,
                                                                                        HTTPMethod.PUT,
                                                                                        HTTPMethod.DELETE
                                                                                    },
                                                       AcceptPatch                = new List<HTTPContentType> {
                                                                                        HTTPContentType.JSONMergePatch_UTF8
                                                                                    },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region GET      ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check tariff

                                        if (!Request.ParseTariff(this,
                                                                 out CountryCode?          CountryCode,
                                                                 out Party_Id?             PartyId,
                                                                 out Tariff_Id?            TariffId,
                                                                 out Tariff                Tariff,
                                                                 out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return Task.FromResult(OCPIResponse);
                                        }

                                        #endregion

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = Tariff.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = Tariff.LastUpdated.ToIso8601(),
                                                       ETag                       = Tariff.SHA256Hash
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PutTariffRequest,
                                    OCPIResponseLogger:  PutTariffResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing tariff

                                        if (!Request.ParseTariff(this,
                                                                 out CountryCode?          CountryCode,
                                                                 out Party_Id?             PartyId,
                                                                 out Tariff_Id?            TariffId,
                                                                 out Tariff                ExistingTariff,
                                                                 out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                 FailOnMissingTariff:      false))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated tariff

                                        if (!Request.TryParseJObjectRequestBody(out JObject TariffJSON, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        if (!Tariff.TryParse(TariffJSON,
                                                             out Tariff  newOrUpdatedTariff,
                                                             out String  ErrorResponse,
                                                             CountryCode,
                                                             PartyId,
                                                             TariffId))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given tariff JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateTariff(newOrUpdatedTariff,
                                                                                                  AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


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
                                                   Data                 = newOrUpdatedTariff.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/tariffs/{country_code}/{party_id}/{tariffId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PatchTariffRequest,
                                    OCPIResponseLogger:  PatchTariffResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check tariff

                                        if (!Request.ParseTariff(this,
                                                                   out CountryCode?          CountryCode,
                                                                   out Party_Id?             PartyId,
                                                                   out Tariff_Id?            TariffId,
                                                                   out Tariff                ExistingTariff,
                                                                   out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                   FailOnMissingTariff: true))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse and apply Tariff JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out JObject TariffPatch, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        #endregion


                                        // Validation-Checks for PATCHes
                                        // (E-Tag, Timestamp, ...)

                                        var patchedTariff = await CommonAPI.TryPatchTariff(ExistingTariff,
                                                                                           TariffPatch);

                                        //ToDo: Handle update errors!
                                        if (patchedTariff.IsSuccess)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedTariff.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                               AccessControlAllowHeaders  = "Authorization",
                                                               LastModified               = patchedTariff.PatchedData.LastUpdated.ToIso8601(),
                                                               ETag                       = patchedTariff.PatchedData.SHA256Hash
                                                           }
                                                       };

                                        }

                                        else
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2000,
                                                           StatusMessage        = patchedTariff.ErrorResponse,
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                               AccessControlAllowHeaders  = "Authorization"
                                                           }
                                                       };

                                        }

                                    });

            #endregion

            #region DELETE   ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteTariffRequest,
                                    OCPIResponseLogger:  DeleteTariffResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing tariff

                                        if (!Request.ParseTariff(this,
                                                                 out CountryCode?          CountryCode,
                                                                 out Party_Id?             PartyId,
                                                                 out Tariff_Id?            TariffId,
                                                                 out Tariff                ExistingTariff,
                                                                 out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                 FailOnMissingTariff:      true))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion


                                        //ToDo: await...
                                        CommonAPI.RemoveTariff(ExistingTariff);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = ExistingTariff.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                    });

            #endregion

            #endregion



            #region ~/sessions/{country_code}/{party_id}                                [NonStandard]

            #region OPTIONS  ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "sessions/{country_code}/{party_id}",
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

            #region GET      ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check Session(Id URI parameter)

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return Task.FromResult(OCPIResponse);
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
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(filteredSessions.SkipTakeFilter(filters.Offset,
                                                                                                                     filters.Limit).
                                                                                                      SafeSelect(session => session.ToJSON())),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", filteredSessionsCount)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region DELETE   ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteSessionsRequest,
                                    OCPIResponseLogger:  DeleteSessionsResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check CountryCode & PartyId

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion


                                        CommonAPI.RemoveAllSessions(CountryCode.Value,
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

            #region ~/sessions/{country_code}/{party_id}/{sessionId}

            #region OPTIONS  ~/sessions/{country_code}/{party_id}/{sessionId}    [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                     HTTPMethod.OPTIONS,
                                     URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                     HTTPContentType.JSON_UTF8,
                                     OCPIRequestHandler: Request => {

                                         return Task.FromResult(
                                             new OCPIResponse.Builder(Request) {
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                        Allow                      = new List<HTTPMethod> {
                                                                                         HTTPMethod.OPTIONS,
                                                                                         HTTPMethod.GET,
                                                                                         HTTPMethod.PUT,
                                                                                         HTTPMethod.PATCH,
                                                                                         HTTPMethod.DELETE
                                                                                     },
                                                        AcceptPatch                = new List<HTTPContentType> {
                                                                                         HTTPContentType.JSONMergePatch_UTF8
                                                                                     },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                             });

                                     });

            #endregion

            #region GET      ~/sessions/{country_code}/{party_id}/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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
                                                                  out CountryCode?          CountryCode,
                                                                  out Party_Id?             PartyId,
                                                                  out Session_Id?           SessionId,
                                                                  out Session               Session,
                                                                  out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                  FailOnMissingSession: true))
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
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = Session.LastUpdated.ToIso8601(),
                                                       ETag                       = Session.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region PUT      ~/sessions/{country_code}/{party_id}/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PUT,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PutSessionRequest,
                                    OCPIResponseLogger:  PutSessionResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing session

                                        if (!Request.ParseSession(this,
                                                                  out CountryCode?          CountryCode,
                                                                  out Party_Id?             PartyId,
                                                                  out Session_Id?           SessionId,
                                                                  out Session               ExistingSession,
                                                                  out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                  FailOnMissingSession: false))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse new or updated session

                                        if (!Request.TryParseJObjectRequestBody(out JObject SessionJSON, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        if (!Session.TryParse(SessionJSON,
                                                              out Session  newOrUpdatedSession,
                                                              out String   ErrorResponse,
                                                              CountryCode,
                                                              PartyId,
                                                              SessionId))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given session JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateSession(newOrUpdatedSession,
                                                                                                   AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


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
                                                           ETag                       = addOrUpdateResult.Data.ETag
                                                       }
                                                   };

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = addOrUpdateResult.ErrorResponse,
                                                   Data                 = newOrUpdatedSession.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #region PATCH    ~/sessions/{country_code}/{party_id}/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.PATCH,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PatchSessionRequest,
                                    OCPIResponseLogger:  PatchSessionResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check session

                                        if (!Request.ParseSession(this,
                                                                  out CountryCode?          CountryCode,
                                                                  out Party_Id?             PartyId,
                                                                  out Session_Id?           SessionId,
                                                                  out Session               ExistingSession,
                                                                  out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                  FailOnMissingSession: true))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse and apply Session JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out JObject SessionPatch, out OCPIResponseBuilder))
                                            return OCPIResponseBuilder;

                                        #endregion


                                        var patchedSession = await CommonAPI.TryPatchSession(ExistingSession,
                                                                                             SessionPatch);

                                        //ToDo: Handle update errors!
                                        if (patchedSession.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedSession.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                               AccessControlAllowHeaders  = "Authorization",
                                                               LastModified               = patchedSession.PatchedData.LastUpdated.ToIso8601(),
                                                               ETag                       = patchedSession.PatchedData.ETag
                                                           }
                                                       };

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedSession.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/sessions/{country_code}/{party_id}/{sessionId}    [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "sessions/{country_code}/{party_id}/{sessionId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteSessionRequest,
                                    OCPIResponseLogger:  DeleteSessionResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing session

                                        if (!Request.ParseSession(this,
                                                                  out CountryCode?          CountryCode,
                                                                  out Party_Id?             PartyId,
                                                                  out Session_Id?           SessionId,
                                                                  out Session               ExistingSession,
                                                                  out OCPIResponse.Builder  OCPIResponseBuilder,
                                                                  FailOnMissingSession: true))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion


                                        //ToDo: await...
                                        CommonAPI.RemoveSession(ExistingSession);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = ExistingSession.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                    });

            #endregion

            #endregion



            #region ~/cdrs/{country_code}/{party_id}                                    [NonStandard]

            #region OPTIONS  ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "cdrs/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, POST, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region GET      ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "cdrs/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check CDR(Id URI parameter)

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return Task.FromResult(OCPIResponse);
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
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(filteredCDRs.SkipTakeFilter(filters.Offset,
                                                                                                                 filters.Limit).
                                                                                                  SafeSelect(cdr => cdr.ToJSON())),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, POST, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", filteredCDRsCount)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #region POST     ~/cdrs/{country_code}/{party_id}       <= Unclear if this URL is correct!

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "cdrs/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PostCDRRequest,
                                    OCPIResponseLogger:  PostCDRResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check CountryCode & PartyId URI parameter

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion

                                        #region Parse newCDR JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse))
                                            return OCPIResponse;

                                        if (!CDR.TryParse(JSONCDR,
                                                          out CDR     newCDR,
                                                          out String  ErrorResponse,
                                                          CountryCode,
                                                          PartyId))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given charge detail record JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, POST, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        // ToDo: What kind of error might happen here?
                                        CommonAPI.AddCDR(newCDR);


                                        // https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/mod_cdrs.asciidoc#mod_cdrs_post_method
                                        // The response should contain the URL to the just created CDR object in the eMSP’s system.
                                        //
                                        // Parameter    Location
                                        // Datatype     URL
                                        // Required     yes
                                        // Description  URL to the newly created CDR in the eMSP’s system, can be used by the CPO system to perform a GET on the same CDR.
                                        // Example      https://www.server.com/ocpi/emsp/2.2/cdrs/123456

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = newCDR.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Created,
                                                           Location                   = URLPathPrefix + "cdrs" + newCDR.CountryCode.ToString() + newCDR.PartyId.ToString() + newCDR.Id.ToString(),
                                                           AccessControlAllowMethods  = "OPTIONS, GET, PUT, PATCH, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization",
                                                           LastModified               = newCDR.LastUpdated.ToIso8601(),
                                                           ETag                       = newCDR.ETag
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE   ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "cdrs/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteCDRsRequest,
                                    OCPIResponseLogger:  DeleteCDRsResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check CountryCode & PartyId

                                        if (!Request.ParseCountryCodeAndPartyId(this,
                                                                                out CountryCode?          CountryCode,
                                                                                out Party_Id?             PartyId,
                                                                                out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion


                                        CommonAPI.RemoveAllCDRs(CountryCode.Value,
                                                                PartyId.    Value);


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, POST, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/cdrs/{country_code}/{party_id}/{cdrId}

            #region OPTIONS  ~/cdrs/{country_code}/{party_id}/{cdrId}    [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET,
                                                                                        HTTPMethod.DELETE
                                                                                    },
                                                       AcceptPatch                = new List<HTTPContentType> {
                                                                                        HTTPContentType.JSONMergePatch_UTF8
                                                                                    },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region GET      ~/cdrs/{country_code}/{party_id}/{cdrId}       // The concrete URL is not specified by OCPI! m(

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing CDR

                                        if (!Request.ParseCDR(this,
                                                              out CountryCode?          CountryCode,
                                                              out Party_Id?             PartyId,
                                                              out CDR_Id?               CDRId,
                                                              out CDR                   CDR,
                                                              out OCPIResponse.Builder  OCPIResponseBuilder,
                                                              FailOnMissingCDR: true))
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
                                                       AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = CDR.LastUpdated.ToIso8601(),
                                                       ETag                       = CDR.ETag
                                                   }
                                            });

                                    });

            #endregion

            #region DELETE   ~/cdrs/{country_code}/{party_id}/{cdrId}    [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteCDRRequest,
                                    OCPIResponseLogger:  DeleteCDRResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check existing CDR

                                        if (!Request.ParseCDR(this,
                                                              out CountryCode?          CountryCode,
                                                              out Party_Id?             PartyId,
                                                              out CDR_Id?               CDRId,
                                                              out CDR                   ExistingCDR,
                                                              out OCPIResponse.Builder  OCPIResponseBuilder,
                                                              FailOnMissingCDR: true))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion


                                        //ToDo: await...
                                        CommonAPI.RemoveCDR(ExistingCDR);


                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = ExistingCDR.ToJSON(),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, DELETE",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                    });

            #endregion

            #endregion



            #region ~/tokens

            #region OPTIONS  ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "tokens",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET,
                                                                                        HTTPMethod.POST
                                                                                    },
                                                       AcceptPatch                = new List<HTTPContentType> {
                                                                                        HTTPContentType.JSONMergePatch_UTF8
                                                                                    },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            });

                                    });

            #endregion

            #region GET      ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tokens",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestHandler: Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new JArray(filteredTokens.SkipTakeFilter(filters.Offset,
                                                                                                                   filters.Limit).
                                                                                                    SafeSelect(token => token.ToJSON())),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                       //LastModified               = ?
                                                   }.
                                                   Set("X-Total-Count", filteredTokensCount)
                                                   // X-Limit               The maximum number of objects that the server WILL return.
                                                   // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                                            });

                                    });

            #endregion

            #endregion

            #region ~/tokens/{token_id}/authorize

            #region POST     ~/tokens/{token_id}/authorize?type=RFID

            // A real-time authorization request
            // https://example.com/ocpi/2.2/emsp/tokens/012345678/authorize?type=RFID
            // curl -X POST http://127.0.0.1:3000/2.2/emsp/tokens/012345678/authorize?type=RFID
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "tokens/{token_id}/authorize",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   PostTokenRequest,
                                    OCPIResponseLogger:  PostTokenResponse,
                                    OCPIRequestHandler:   async Request => {

                                        #region Check access token

                                        if (Request.AccessInfo.IsNot(Roles.CPO) ||
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

                                        #region Check TokenId URI parameter

                                        if (!Request.ParseTokenId(this,
                                                                  out Token_Id?             TokenId,
                                                                  out OCPIResponse.Builder  OCPIResponse))
                                        {
                                            return OCPIResponse;
                                        }

                                        #endregion

                                        var requestedTokenType  = Request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;

                                        #region Parse optional LocationReference JSON

                                        LocationReference? locationReference = null;

                                        if (Request.TryParseJObjectRequestBody(out JObject  LocationReferenceJSON,
                                                                               out          OCPIResponse,
                                                                               AllowEmptyHTTPBody: true))
                                        {

                                            if (!LocationReference.TryParse(LocationReferenceJSON,
                                                                            out LocationReference  _locationReference,
                                                                            out String             ErrorResponse))
                                            {

                                                return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given location reference JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                            }

                                            locationReference = _locationReference;

                                        }

                                        #endregion


                                        AuthorizationInfo authorizationInfo = null;

                                        var RFIDAuthTokenLocal = OnRFIDAuthToken;
                                        if (RFIDAuthTokenLocal != null)
                                        {

                                            try
                                            {

                                                var result = RFIDAuthTokenLocal(Request.FromCountryCode ?? DefaultCountryCode,
                                                                                Request.FromPartyId     ?? DefaultPartyId,
                                                                                Request.ToCountryCode   ?? DefaultCountryCode,
                                                                                Request.ToPartyId       ?? DefaultPartyId,
                                                                                TokenId.Value,
                                                                                locationReference).Result;

                                                authorizationInfo = result;

                                            }
                                            catch (Exception e)
                                            {

                                            }

                                        }

                                        else
                                        {

                                            #region Check existing token

                                            if (!CommonAPI.TryGetToken(Request.ToCountryCode ?? DefaultCountryCode,
                                                                       Request.ToPartyId     ?? DefaultPartyId,
                                                                       TokenId.Value,
                                                                       out TokenStatus _tokenStatus) ||
                                                (_tokenStatus.Token.Type != requestedTokenType))
                                            {

                                                return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 2004,
                                                           StatusMessage        = "Unknown token!",
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                               AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                               AccessControlAllowHeaders  = "Authorization"
                                                           }
                                                       };

                                            }

                                            #endregion

                                            authorizationInfo = new AuthorizationInfo(
                                                                      _tokenStatus.Status,
                                                                      _tokenStatus.Token,
                                                                      _tokenStatus.LocationReference,
                                                                      AuthorizationReference.NewRandom()
                                                                      //new DisplayText(
                                                                      //    _tokenStatus.Token.UILanguage ?? Languages.en,
                                                                      //    responseText
                                                                      //)
                                                                );

                                            #region Parse optional LocationReference JSON

                                            if (locationReference.HasValue)
                                            {

                                                Location validLocation = null;

                                                if (Request.FromCountryCode.HasValue && Request.FromPartyId.HasValue)
                                                {

                                                    if (!CommonAPI.TryGetLocation(Request.FromCountryCode.Value,
                                                                                  Request.FromPartyId.    Value,
                                                                                  locationReference.Value.LocationId,
                                                                                  out validLocation))
                                                    {

                                                        return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2001,
                                                            StatusMessage        = "The given location is unknown!",
                                                            Data                 = new AuthorizationInfo(
                                                                                       AllowedTypes.NOT_ALLOWED,
                                                                                       _tokenStatus.Token,
                                                                                       locationReference.Value,
                                                                                       null,
                                                                                       new DisplayText(Languages.en, "The given location is unknown!")
                                                                                   ).ToJSON(),
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                                AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                                AccessControlAllowHeaders  = "Authorization"
                                                            }
                                                        };

                                                    }

                                                }

                                                else
                                                {

                                                    if (Request.AccessInfo.Value.Roles.Where(role => role.Role == Roles.CPO).Count() != 1)
                                                    {

                                                        return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2001,
                                                            StatusMessage        = "Could not determine the country code and party identification of the given location!",
                                                            Data                 = new AuthorizationInfo(
                                                                                       AllowedTypes.NOT_ALLOWED,
                                                                                       _tokenStatus.Token,
                                                                                       locationReference.Value
                                                                                   ).ToJSON(),
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                                AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                                AccessControlAllowHeaders  = "Authorization"
                                                            }
                                                        };

                                                    }

                                                    var allTheirCPORoles = Request.AccessInfo.Value.Roles.Where(role => role.Role == Roles.CPO).ToArray();

                                                    if (!CommonAPI.TryGetLocation(allTheirCPORoles[0].CountryCode,
                                                                                  allTheirCPORoles[0].PartyId,
                                                                                  locationReference.Value.LocationId,
                                                                                  out validLocation))
                                                    {

                                                        return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2001,
                                                            StatusMessage        = "The given location is unknown!",
                                                            Data                 = new AuthorizationInfo(
                                                                                       AllowedTypes.NOT_ALLOWED,
                                                                                       _tokenStatus.Token,
                                                                                       locationReference.Value,
                                                                                       null,
                                                                                       new DisplayText(Languages.en, "The given location is unknown!")
                                                                                   ).ToJSON(),
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                                AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                                AccessControlAllowHeaders  = "Authorization"
                                                            }
                                                        };

                                                    }

                                                }


                                                //ToDo: Add a event/delegate for addditional location filters!


                                                if (locationReference.Value.EVSEUIds.SafeAny())
                                                {

                                                    locationReference = new LocationReference(locationReference.Value.LocationId,
                                                                                              locationReference.Value.EVSEUIds.
                                                                                                                      Where(evseuid => validLocation.EVSEExists(evseuid)));

                                                    if (!locationReference.Value.EVSEUIds.SafeAny())
                                                    {

                                                        return new OCPIResponse.Builder(Request) {
                                                            StatusCode           = 2001,
                                                            StatusMessage        = locationReference.Value.EVSEUIds.Count() == 1
                                                                                       ? "The EVSE at the given location is unknown!"
                                                                                       : "The EVSEs at the given location are unknown!",
                                                            Data                 = new AuthorizationInfo(
                                                                                       AllowedTypes.NOT_ALLOWED,
                                                                                       _tokenStatus.Token,
                                                                                       locationReference.Value,
                                                                                       null,
                                                                                       new DisplayText(
                                                                                           Languages.en,
                                                                                           locationReference.Value.EVSEUIds.Count() == 1
                                                                                               ? "The EVSE at the given location is unknown!"
                                                                                               : "The EVSEs at the given location are unknown!"
                                                                                       )
                                                                                   ).ToJSON(),
                                                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                                AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                                AccessControlAllowHeaders  = "Authorization"
                                                            }
                                                        };

                                                    }



                                                    //ToDo: Add a event/delegate for addditional EVSE filters!

                                                }

                                            }

                                            #endregion

                                        }

                                        if (authorizationInfo == null)
                                            authorizationInfo = new AuthorizationInfo(
                                                                      AllowedTypes.BLOCKED,
                                                                      new Token(
                                                                          CountryCode.Parse("DE"),
                                                                          Party_Id.Parse("XXX"),
                                                                          TokenId.Value,
                                                                          requestedTokenType,
                                                                          Contract_Id.Parse("DE-XXX-" + TokenId.ToString()),
                                                                          "Error!",
                                                                          false,
                                                                          WhitelistTypes.NEVER
                                                                      )
                                                                  );


                                        // too little information like e.g. no LocationReferences provided:
                                        //   => status_code 2002


                                        #region Set a user-friendly response message for the ev driver

                                        var responseText = "Sorry, charging is not allowed!";

                                        if (!authorizationInfo.Info.HasValue)
                                        {

                                            switch (authorizationInfo.Allowed)
                                            {

                                                #region ALLOWED

                                                case AllowedTypes.ALLOWED:

                                                    responseText = "Charging allowed!";

                                                    if (authorizationInfo.Token.UILanguage.HasValue)
                                                    {
                                                        switch (authorizationInfo.Token.UILanguage.Value)
                                                        {
                                                            case Languages.de:
                                                                responseText = "Der Ladevorgang wird gestartet!";
                                                                break;
                                                        }
                                                    }

                                                    break;

                                                #endregion

                                                #region BLOCKED

                                                case AllowedTypes.BLOCKED:

                                                    responseText = "Sorry, your token is blocked!";

                                                    if (authorizationInfo.Token.UILanguage.HasValue)
                                                    {
                                                        switch (authorizationInfo.Token.UILanguage.Value)
                                                        {
                                                            case Languages.de:
                                                                responseText = "Autorisierung fehlgeschlagen!";
                                                                break;
                                                        }
                                                    }

                                                    break;

                                                #endregion

                                                #region EXPIRED

                                                case AllowedTypes.EXPIRED:

                                                    responseText = "Sorry, your token has expired!";

                                                    if (authorizationInfo.Token.UILanguage.HasValue)
                                                    {
                                                        switch (authorizationInfo.Token.UILanguage.Value)
                                                        {
                                                            case Languages.de:
                                                                responseText = "Autorisierungstoken ungültig!";
                                                                break;
                                                        }
                                                    }

                                                    break;

                                                #endregion

                                                #region NO_CREDIT

                                                case AllowedTypes.NO_CREDIT:

                                                    responseText = "Sorry, your have not enough credits for charging!";

                                                    if (authorizationInfo.Token.UILanguage.HasValue)
                                                    {
                                                        switch (authorizationInfo.Token.UILanguage.Value)
                                                        {
                                                            case Languages.de:
                                                                responseText = "Nicht genügend Ladeguthaben!";
                                                                break;
                                                        }
                                                    }

                                                    break;

                                                #endregion

                                                #region NOT_ALLOWED

                                                case AllowedTypes.NOT_ALLOWED:

                                                    responseText = "Sorry, charging is not allowed!";

                                                    if (authorizationInfo.Token.UILanguage.HasValue)
                                                    {
                                                        switch (authorizationInfo.Token.UILanguage.Value)
                                                        {
                                                            case Languages.de:
                                                                responseText = "Autorisierung abgelehnt!";
                                                                break;
                                                        }
                                                    }

                                                    break;

                                                #endregion

                                                #region default

                                                default:
                                                    responseText = "An error occured!";
                                                    break;

                                                    #endregion

                                            }

                                        }

                                        #endregion


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = new AuthorizationInfo(
                                                                              authorizationInfo.Allowed,
                                                                              authorizationInfo.Token,
                                                                              authorizationInfo.Location,
                                                                              authorizationInfo.AuthorizationReference ?? AuthorizationReference.NewRandom(),
                                                                              authorizationInfo.Info                   ?? new DisplayText(
                                                                                                                              authorizationInfo.Token.UILanguage ?? Languages.en,
                                                                                                                              responseText
                                                                                                                          )
                                                                          ).ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, GET, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion



            // Commands

            #region ~/commands/RESERVE_NOW/{commandId}

            #region OPTIONS  ~/commands/RESERVE_NOW/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/RESERVE_NOW/{commandId}",
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

            #region POST     ~/commands/RESERVE_NOW/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/RESERVE_NOW/{commandId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   ReserveNowCallbackRequest,
                                    OCPIResponseLogger:  ReserveNowCallbackResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(this,
                                                                    out Command_Id?           CommandId,
                                                                    out OCPIResponse.Builder  OCPIResponseBuilder))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!CommandResult.TryParse(JSONCDR,
                                                                    out CommandResult  commandResult,
                                                                    out String         ErrorResponse))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'command result' JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(CommandId.Value, out CommandValues commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            #region Sending upstream command response...

                                            if (commandValues.UpstreamCommand != null)
                                            {

                                                try
                                                {

                                                    var HTTPResponse = await HTTPClientFactory.Create(commandValues.UpstreamCommand.ResponseURL,
                                                                                                      //null,
                                                                                                      //default,
                                                                                                      //RemoteCertificateValidator,
                                                                                                      //ClientCertificateSelector,
                                                                                                      //ClientCert,
                                                                                                      //HTTPUserAgent,
                                                                                                      //RequestTimeout,
                                                                                                      //TransmissionRetryDelay,
                                                                                                      //MaxNumberOfRetries,
                                                                                                      //UseHTTPPipelining,
                                                                                                      //HTTPLogger,
                                                                                                      DNSClient: DNSClient).

                                                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                                                     commandValues.UpstreamCommand.ResponseURL.Path,
                                                                                                                     requestbuilder => {
                                                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                                                         requestbuilder.Content       = commandValues.Response.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                                                         requestbuilder.Set("X-Request-ID",      commandValues.UpstreamCommand.RequestId);
                                                                                                                         requestbuilder.Set("X-Correlation-ID",  commandValues.UpstreamCommand.CorrelationId);
                                                                                                                     }),

                                                                                      //RequestLogDelegate:   OnStartSessionHTTPRequest,
                                                                                      //ResponseLogDelegate:  OnStartSessionHTTPResponse,
                                                                                      //CancellationToken:    CancellationToken,
                                                                                      //EventTrackingId:      EventTrackingId,
                                                                                      RequestTimeout:       this.RequestTimeout).

                                                                              ConfigureAwait(false);


                                                    HTTPResponse.AppendToLogfile(nameof(EMSPAPI) + "_upstream_RESERVE_NOW.log");


                                                } catch (Exception e)
                                                {
                                                    DebugX.Log("[" + nameof(EMSPAPI), "] Sending upstream RESERVE_NOW command response failed!");
                                                }

                                            }

                                            #endregion


                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = "Unknown 'reserve now' command identification!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/CANCEL_RESERVATION/{commandId}

            #region OPTIONS  ~/commands/CANCEL_RESERVATION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/CANCEL_RESERVATION/{commandId}",
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

            #region POST     ~/commands/CANCEL_RESERVATION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/CANCEL_RESERVATION/{commandId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   CancelReservationCallbackRequest,
                                    OCPIResponseLogger:  CancelReservationCallbackResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(this,
                                                                    out Command_Id?           CommandId,
                                                                    out OCPIResponse.Builder  OCPIResponseBuilder))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!CommandResult.TryParse(JSONCDR,
                                                                    out CommandResult  commandResult,
                                                                    out String         ErrorResponse))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'command result' JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(CommandId.Value, out CommandValues commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            #region Sending upstream command response...

                                            if (commandValues.UpstreamCommand != null)
                                            {

                                                try
                                                {

                                                    var HTTPResponse = await HTTPClientFactory.Create(commandValues.UpstreamCommand.ResponseURL,
                                                                                                      //null,
                                                                                                      //default,
                                                                                                      //RemoteCertificateValidator,
                                                                                                      //ClientCertificateSelector,
                                                                                                      //ClientCert,
                                                                                                      //HTTPUserAgent,
                                                                                                      //RequestTimeout,
                                                                                                      //TransmissionRetryDelay,
                                                                                                      //MaxNumberOfRetries,
                                                                                                      //UseHTTPPipelining,
                                                                                                      //HTTPLogger,
                                                                                                      DNSClient: DNSClient).

                                                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                                                     commandValues.UpstreamCommand.ResponseURL.Path,
                                                                                                                     requestbuilder => {
                                                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                                                         requestbuilder.Content       = commandValues.Response.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                                                         requestbuilder.Set("X-Request-ID",      commandValues.UpstreamCommand.RequestId);
                                                                                                                         requestbuilder.Set("X-Correlation-ID",  commandValues.UpstreamCommand.CorrelationId);
                                                                                                                     }),

                                                                                      //RequestLogDelegate:   OnStartSessionHTTPRequest,
                                                                                      //ResponseLogDelegate:  OnStartSessionHTTPResponse,
                                                                                      //CancellationToken:    CancellationToken,
                                                                                      //EventTrackingId:      EventTrackingId,
                                                                                      RequestTimeout:       this.RequestTimeout).

                                                                              ConfigureAwait(false);


                                                    HTTPResponse.AppendToLogfile(nameof(EMSPAPI) + "_upstream_CANCEL_RESERVATION.log");


                                                } catch (Exception e)
                                                {
                                                    DebugX.Log("[" + nameof(EMSPAPI), "] Sending upstream CANCEL_RESERVATION command response failed!");
                                                }

                                            }

                                            #endregion


                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = "Unknown 'cancel reservation' command identification!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/START_SESSION/{commandId}

            #region OPTIONS  ~/commands/START_SESSION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/START_SESSION/{commandId}",
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

            #region POST     ~/commands/START_SESSION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/START_SESSION/{commandId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   StartSessionCallbackRequest,
                                    OCPIResponseLogger:  StartSessionCallbackResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(this,
                                                                    out Command_Id?           CommandId,
                                                                    out OCPIResponse.Builder  OCPIResponseBuilder))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!CommandResult.TryParse(JSONCDR,
                                                                    out CommandResult  commandResult,
                                                                    out String         ErrorResponse))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'command result' JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(CommandId.Value, out CommandValues commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            #region Sending upstream command response...

                                            if (commandValues.UpstreamCommand != null)
                                            {

                                                try
                                                {

                                                    var HTTPResponse = await HTTPClientFactory.Create(commandValues.UpstreamCommand.ResponseURL,
                                                                                                      //null,
                                                                                                      //default,
                                                                                                      //RemoteCertificateValidator,
                                                                                                      //ClientCertificateSelector,
                                                                                                      //ClientCert,
                                                                                                      //HTTPUserAgent,
                                                                                                      //RequestTimeout,
                                                                                                      //TransmissionRetryDelay,
                                                                                                      //MaxNumberOfRetries,
                                                                                                      //UseHTTPPipelining,
                                                                                                      //HTTPLogger,
                                                                                                      DNSClient: DNSClient).

                                                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                                                     commandValues.UpstreamCommand.ResponseURL.Path,
                                                                                                                     requestbuilder => {
                                                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                                                         requestbuilder.Content       = commandResult.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                                                         requestbuilder.Set("X-Request-ID",      commandValues.UpstreamCommand.RequestId);
                                                                                                                         requestbuilder.Set("X-Correlation-ID",  commandValues.UpstreamCommand.CorrelationId);
                                                                                                                     }),

                                                                                      //RequestLogDelegate:   OnStartSessionHTTPRequest,
                                                                                      //ResponseLogDelegate:  OnStartSessionHTTPResponse,
                                                                                      //CancellationToken:    CancellationToken,
                                                                                      //EventTrackingId:      EventTrackingId,
                                                                                      RequestTimeout:       this.RequestTimeout).

                                                                              ConfigureAwait(false);


                                                    HTTPResponse.AppendToLogfile(nameof(EMSPAPI) + "_upstream_START_SESSION.log");


                                                } catch (Exception e)
                                                {
                                                    DebugX.Log("[" + nameof(EMSPAPI), "] Sending upstream START_SESSION command response failed!");
                                                }

                                            }

                                            #endregion


                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = "Unknown 'start session' command identification!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/STOP_SESSION/{commandId}

            #region OPTIONS  ~/commands/STOP_SESSION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/STOP_SESSION/{commandId}",
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

            #region POST     ~/commands/STOP_SESSION/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/STOP_SESSION/{commandId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   StopSessionCallbackRequest,
                                    OCPIResponseLogger:  StopSessionCallbackResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(this,
                                                                    out Command_Id?           CommandId,
                                                                    out OCPIResponse.Builder  OCPIResponseBuilder))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!CommandResult.TryParse(JSONCDR,
                                                                    out CommandResult  commandResult,
                                                                    out String         ErrorResponse))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'command result' JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(CommandId.Value, out CommandValues commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            #region Sending upstream command response...

                                            if (commandValues.UpstreamCommand != null)
                                            {

                                                try
                                                {

                                                    var HTTPResponse = await HTTPClientFactory.Create(commandValues.UpstreamCommand.ResponseURL,
                                                                                                      //null,
                                                                                                      //default,
                                                                                                      //RemoteCertificateValidator,
                                                                                                      //ClientCertificateSelector,
                                                                                                      //ClientCert,
                                                                                                      //HTTPUserAgent,
                                                                                                      //RequestTimeout,
                                                                                                      //TransmissionRetryDelay,
                                                                                                      //MaxNumberOfRetries,
                                                                                                      //UseHTTPPipelining,
                                                                                                      //HTTPLogger,
                                                                                                      DNSClient: DNSClient).

                                                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                                                     commandValues.UpstreamCommand.ResponseURL.Path,
                                                                                                                     requestbuilder => {
                                                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                                                         requestbuilder.Content       = commandValues.Response.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                                                         requestbuilder.Set("X-Request-ID",      commandValues.UpstreamCommand.RequestId);
                                                                                                                         requestbuilder.Set("X-Correlation-ID",  commandValues.UpstreamCommand.CorrelationId);
                                                                                                                     }),

                                                                                      //RequestLogDelegate:   OnStartSessionHTTPRequest,
                                                                                      //ResponseLogDelegate:  OnStartSessionHTTPResponse,
                                                                                      //CancellationToken:    CancellationToken,
                                                                                      //EventTrackingId:      EventTrackingId,
                                                                                      RequestTimeout:       this.RequestTimeout).

                                                                              ConfigureAwait(false);


                                                    HTTPResponse.AppendToLogfile(nameof(EMSPAPI) + "_upstream_STOP_SESSION.log");


                                                } catch (Exception e)
                                                {
                                                    DebugX.Log("[" + nameof(EMSPAPI), "] Sending upstream STOP_SESSION command response failed!");
                                                }

                                            }

                                            #endregion


                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = "Unknown 'stop session' command identification!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = "OPTIONS, POST",
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/commands/UNLOCK_CONNECTOR/{commandId}

            #region OPTIONS  ~/commands/UNLOCK_CONNECTOR/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "commands/UNLOCK_CONNECTOR/{commandId}",
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

            #region POST     ~/commands/UNLOCK_CONNECTOR/{commandId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/UNLOCK_CONNECTOR/{commandId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   UnlockConnectorCallbackRequest,
                                    OCPIResponseLogger:  UnlockConnectorCallbackResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check command identification

                                        if (!Request.ParseCommandId(this,
                                                                    out Command_Id?           CommandId,
                                                                    out OCPIResponse.Builder  OCPIResponseBuilder))
                                        {
                                            return OCPIResponseBuilder;
                                        }

                                        #endregion

                                        #region Parse command result JSON

                                        if (!Request.TryParseJObjectRequestBody(out JObject JSONCDR, out OCPIResponse.Builder OCPIResponse))
                                            return OCPIResponse;

                                        if (!CommandResult.TryParse(JSONCDR,
                                                                    out CommandResult  commandResult,
                                                                    out String         ErrorResponse))
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'command result' JSON: " + ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        if (CommonAPI.CommandValueStore.TryGetValue(CommandId.Value, out CommandValues commandValues))
                                        {

                                            commandValues.Result = commandResult;

                                            #region Sending upstream command response...

                                            if (commandValues.UpstreamCommand != null)
                                            {

                                                try
                                                {

                                                    var HTTPResponse = await HTTPClientFactory.Create(commandValues.UpstreamCommand.ResponseURL,
                                                                                                      //null,
                                                                                                      //default,
                                                                                                      //RemoteCertificateValidator,
                                                                                                      //ClientCertificateSelector,
                                                                                                      //ClientCert,
                                                                                                      //HTTPUserAgent,
                                                                                                      //RequestTimeout,
                                                                                                      //TransmissionRetryDelay,
                                                                                                      //MaxNumberOfRetries,
                                                                                                      //UseHTTPPipelining,
                                                                                                      //HTTPLogger,
                                                                                                      DNSClient: DNSClient).

                                                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                                                     commandValues.UpstreamCommand.ResponseURL.Path,
                                                                                                                     requestbuilder => {
                                                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                                                         requestbuilder.Content       = commandValues.Response.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                                                         requestbuilder.Set("X-Request-ID",      commandValues.UpstreamCommand.RequestId);
                                                                                                                         requestbuilder.Set("X-Correlation-ID",  commandValues.UpstreamCommand.CorrelationId);
                                                                                                                     }),

                                                                                      //RequestLogDelegate:   OnStartSessionHTTPRequest,
                                                                                      //ResponseLogDelegate:  OnStartSessionHTTPResponse,
                                                                                      //CancellationToken:    CancellationToken,
                                                                                      //EventTrackingId:      EventTrackingId,
                                                                                      RequestTimeout:       this.RequestTimeout).

                                                                              ConfigureAwait(false);


                                                    HTTPResponse.AppendToLogfile(nameof(EMSPAPI) + "_upstream_UNLOCK_CONNECTOR.log");


                                                } catch (Exception e)
                                                {
                                                    DebugX.Log("[" + nameof(EMSPAPI), "] Sending upstream UNLOCK_CONNECTOR command response failed!");
                                                }

                                            }

                                            #endregion


                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                                           AccessControlAllowMethods  = "OPTIONS, POST",
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = "Unknown 'unlock connector' command identification!",
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
