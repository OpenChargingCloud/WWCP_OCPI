/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// Extention methods for the CPO HTTP API.
    /// </summary>
    public static class CPOAPIExtentions
    {

        #region ParseParseCountryCodePartyId (this Request, CPOAPI, out CountryCode, out PartyId,                                                        out HTTPResponse)

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
        public static Boolean ParseParseCountryCodePartyId(this OCPIRequest           Request,
                                                           CPOAPI                     CPOAPI,
                                                           out CountryCode?           CountryCode,
                                                           out Party_Id?              PartyId,
                                                           out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            CountryCode = OCPI.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseLocation                (this Request, CPOAPI, out LocationId, out Location,                                                        out HTTPResponse)

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
        public static Boolean ParseLocation(this OCPIRequest           Request,
                                            CPOAPI                     CPOAPI,
                                            CountryCode                CountryCode,
                                            Party_Id                   PartyId,
                                            out Location_Id?           LocationId,
                                            out Location?              Location,
                                            out OCPIResponse.Builder?  OCPIResponseBuilder,
                                            Boolean                    FailOnMissingLocation = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(LocationId.Value, out Location) ||
                 Location is null                                                ||
                 Location.CountryCode != CountryCode                             ||
                 Location.PartyId     != PartyId)
            {

                if (FailOnMissingLocation)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2003,
                        StatusMessage        = "Unknown location!",
                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                            //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                            AccessControlAllowHeaders  = "Authorization"
                        }
                    };

                    return false;

                }

            }

            return true;

        }

        #endregion

        #region ParseLocationEVSE            (this Request, CPOAPI, out LocationId, out Location, out EVSEUId, out EVSE,                                 out HTTPResponse)

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
        public static Boolean ParseLocationEVSE(this OCPIRequest           Request,
                                                CPOAPI                     CPOAPI,
                                                CountryCode                CountryCode,
                                                Party_Id                   PartyId,
                                                out Location_Id?           LocationId,
                                                out Location?              Location,
                                                out EVSE_UId?              EVSEUId,
                                                out EVSE?                  EVSE,
                                                out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                Boolean                    FailOnMissingEVSE = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
                throw new ArgumentNullException(nameof(CPOAPI),   "The given CPO API must not be null!");

            #endregion

            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 2)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing location and/or EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(LocationId.Value, out Location) ||
                 Location is null                                                ||
                 Location.CountryCode != CountryCode                             ||
                 Location.PartyId     != PartyId)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unkown location!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE))
            {

                if (FailOnMissingEVSE)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2001,
                        StatusMessage        = "Unkown EVSE!",
                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                            //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                            AccessControlAllowHeaders  = "Authorization"
                        }
                    };

                    return false;

                }

            }

            return true;

        }

        #endregion

        #region ParseLocationEVSEConnector   (this Request, CPOAPI, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out HTTPResponse)

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
        public static Boolean ParseLocationEVSEConnector(this OCPIRequest           Request,
                                                         CPOAPI                     CPOAPI,
                                                         CountryCode                CountryCode,
                                                         Party_Id                   PartyId,
                                                         out Location_Id?           LocationId,
                                                         out Location?              Location,
                                                         out EVSE_UId?              EVSEUId,
                                                         out EVSE?                  EVSE,
                                                         out Connector_Id?          ConnectorId,
                                                         out Connector?             Connector,
                                                         out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                         Boolean                    FailOnMissingConnector = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!ConnectorId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetLocation(LocationId.Value, out Location) ||
                 Location is null                                                ||
                 Location.CountryCode != CountryCode                             ||
                 Location.PartyId     != PartyId)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE) ||
                 EVSE is null) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown EVSE!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            if (!EVSE.TryGetConnector(ConnectorId.Value, out Connector))
            {

                if (FailOnMissingConnector)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2001,
                        StatusMessage        = "Unknown connector!",
                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                            //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                            AccessControlAllowHeaders  = "Authorization"
                        }
                    };

                    return false;

                }

            }

            return true;

        }

        #endregion


        #region ParseTariff                  (this Request, CPOAPI, out TariffId, out Tariff,                                                            out HTTPResponse)

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
        public static Boolean ParseTariff(this OCPIRequest           Request,
                                          CPOAPI                     CPOAPI,
                                          CountryCode                CountryCode,
                                          Party_Id                   PartyId,
                                          out Tariff_Id?             TariffId,
                                          out Tariff?                Tariff,
                                          out OCPIResponse.Builder?  OCPIResponseBuilder,
                                          Boolean                    FailOnMissingTariff = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetTariff(TariffId.Value,
                                               out Tariff) ||
                 Tariff is null                            ||
                 Tariff.CountryCode != CountryCode         ||
                 Tariff.PartyId     != PartyId)
            {

                if (FailOnMissingTariff)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2003,
                        StatusMessage        = "Unknown tariff!",
                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                            //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                            AccessControlAllowHeaders  = "Authorization"
                        }
                    };

                    return false;

                }

            }

            return true;

        }

        #endregion

        #region ParseSession                 (this Request, CPOAPI, out SessionId, out Session,                                                          out HTTPResponse)

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
        public static Boolean ParseSession(this OCPIRequest           Request,
                                           CPOAPI                     CPOAPI,
                                           CountryCode                CountryCode,
                                           Party_Id                   PartyId,
                                           out Session_Id?            SessionId,
                                           out Session?               Session,
                                           out OCPIResponse.Builder?  OCPIResponseBuilder,
                                           Boolean                    FailOnMissingSession = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetSession(SessionId.Value,
                                               out Session) ||
                 Session is null                            ||
                 Session.CountryCode != CountryCode         ||
                 Session.PartyId     != PartyId)
            {

                if (FailOnMissingSession)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2003,
                        StatusMessage        = "Unknown session!",
                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                            //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                            AccessControlAllowHeaders  = "Authorization"
                        }
                    };

                    return false;

                }

            }

            return true;

        }

        #endregion

        #region ParseCDR                     (this Request, CPOAPI, out CDRId, out CDR,                                                                  out HTTPResponse)

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
        public static Boolean ParseCDR(this OCPIRequest           Request,
                                       CPOAPI                     CPOAPI,
                                       CountryCode                CountryCode,
                                       Party_Id                   PartyId,
                                       out CDR_Id?                CDRId,
                                       out CDR?                   CDR,
                                       out OCPIResponse.Builder?  OCPIResponseBuilder,
                                       Boolean                    FailOnMissingCDR = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetCDR(CDRId.Value,
                                            out CDR) ||
                 CDR is null                         ||
                 CDR.CountryCode != CountryCode      ||
                 CDR.PartyId     != PartyId)
            {

                if (FailOnMissingCDR)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2003,
                        StatusMessage        = "Unknown CDR!",
                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                            //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                            AccessControlAllowHeaders  = "Authorization"
                        }
                    };

                    return false;

                }

            }

            return true;

        }

        #endregion

        #region ParseToken                   (this Request, CPOAPI, out CountryCode, out PartyId, out TokenId, out Token,                                out HTTPResponse)

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
        public static Boolean ParseToken(this OCPIRequest           Request,
                                         CPOAPI                     CPOAPI,
                                         out CountryCode?           CountryCode,
                                         out Party_Id?              PartyId,
                                         out Token_Id?              TokenId,
                                         out TokenStatus?           TokenStatus,
                                         out OCPIResponse.Builder?  OCPIResponseBuilder,
                                         Boolean                    FailOnMissingToken = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }

            CountryCode = OCPI.CountryCode.TryParse(Request.ParsedURLParameters[0]);

            if (!CountryCode.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

                return false;

            }


            if (!CPOAPI.CommonAPI.TryGetToken(TokenId.Value, out var tokenStatus))
            {

                if (FailOnMissingToken)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2004,
                        StatusMessage        = "Unknown token identification!",
                        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                            //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                            AccessControlAllowHeaders  = "Authorization"
                        }
                    };

                    TokenStatus = null;
                    return false;

                }

                TokenStatus = tokenStatus;

            }

            if (tokenStatus.Token.CountryCode != CountryCode ||
                tokenStatus.Token.PartyId     != PartyId)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2004,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.UnprocessableEntity,
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                        AccessControlAllowHeaders  = "Authorization"
                    }
                };

            }

            return true;

        }

        #endregion


        #region ParseCommandId               (this Request, CPOAPI, out CommandId,                                                                       out HTTPResponse)

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
        public static Boolean ParseCommandId(this OCPIRequest           Request,
                                             CPOAPI                     CPOAPI,
                                             out Command_Id?            CommandId,
                                             out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CPOAPI  is null)
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
                        //AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
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
        public new const           String    DefaultHTTPServiceName   = "GraphDefined OCPI CPO HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort    = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix     = HTTPPath.Parse("cpo/");

        /// <summary>
        /// The default CPO API logfile name.
        /// </summary>
        public  const              String    DefaultLogfileName       = "OCPI_CPOAPI.log";

        #endregion

        #region Properties

        /// <summary>
        /// The CommonAPI.
        /// </summary>
        public CommonAPI      CommonAPI             { get; }

        /// <summary>
        /// The default country code to use.
        /// </summary>
        public CountryCode    DefaultCountryCode    { get; }

        /// <summary>
        /// The default party identification to use.
        /// </summary>
        public Party_Id       DefaultPartyId        { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?       AllowDowngrades       { get; }

        /// <summary>
        /// The CPO API logger.
        /// </summary>
        public CPOAPILogger?  CPOAPILogger          { get; }

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer           { get; set; }

        public Boolean                                                       IncludeSessionOwnerInformation               { get; set; }
        public CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                  { get; set; }


        public CustomJObjectSerializerDelegate<Token>?                       CustomTokenSerializer                        { get; set; }


        public CustomJObjectSerializerDelegate<AuthorizationInfo>?           CustomAuthorizationInfoSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<LocationReference>?           CustomLocationReferenceSerializer            { get; set; }

        #endregion

        #region Events

        #region Location(s)

        #region (protected internal) GetLocationsRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetLocationsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationsRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnGetLocationsRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) GetLocationsResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET locations response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetLocationsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET locations response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationsResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnGetLocationsResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) GetLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetLocationRequest = new ();

        /// <summary>
        /// An event sent whenever a GET location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetLocationRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) GetLocationResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetLocationResponse = new ();

        /// <summary>
        /// An event sent whenever a GET location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetLocationResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion

        #endregion

        #region EVSE

        #region (protected internal) GetEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetEVSERequest = new ();

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetEVSERequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnGetEVSERequest.WhenAll(Timestamp,
                                        API ?? this,
                                        Request);

        #endregion

        #region (protected internal) GetEVSEResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetEVSEResponse = new ();

        /// <summary>
        /// An event sent whenever a GET EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetEVSEResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnGetEVSEResponse.WhenAll(Timestamp,
                                         API ?? this,
                                         Request,
                                         Response);

        #endregion

        #endregion

        #region Connector

        #region (protected internal) GetConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetConnectorRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnGetConnectorRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) GetConnectorResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetConnectorResponse = new ();

        /// <summary>
        /// An event sent whenever a GET connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetConnectorResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnGetConnectorResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion

        #endregion

        #region Tariff(s)

        #region (protected internal) GetTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffsRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetTariffsRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) GetTariffsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET tariffs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTariffsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET tariffs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffsResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetTariffsResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) GetTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffRequest = new ();

        /// <summary>
        /// An event sent whenever a GET tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnGetTariffRequest.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) GetTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTariffResponse = new ();

        /// <summary>
        /// An event sent whenever a GET tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnGetTariffResponse.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion

        #endregion

        #region Session(s)

        #region (protected internal) GetSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionsRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetSessionsRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) GetSessionsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET sessions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetSessionsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET sessions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionsResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetSessionsResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) GetSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a GET session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetSessionRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) GetSessionResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetSessionResponse = new ();

        /// <summary>
        /// An event sent whenever a GET session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetSessionResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion

        #endregion

        #region CDR(s)

        #region (protected internal) GetCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRsRequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               OCPIRequest  Request)

            => OnGetCDRsRequest.WhenAll(Timestamp,
                                        API ?? this,
                                        Request);

        #endregion

        #region (protected internal) GetCDRsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET CDRs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCDRsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET CDRs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRsResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                OCPIRequest   Request,
                                                OCPIResponse  Response)

            => OnGetCDRsResponse.WhenAll(Timestamp,
                                         API ?? this,
                                         Request,
                                         Response);

        #endregion


        #region (protected internal) GetCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRRequest = new ();

        /// <summary>
        /// An event sent whenever a GET CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRRequest(DateTime     Timestamp,
                                              HTTPAPI      API,
                                              OCPIRequest  Request)

            => OnGetCDRRequest.WhenAll(Timestamp,
                                       API ?? this,
                                       Request);

        #endregion

        #region (protected internal) GetCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a GET CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCDRResponse = new ();

        /// <summary>
        /// An event sent whenever a GET CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRResponse(DateTime      Timestamp,
                                               HTTPAPI       API,
                                               OCPIRequest   Request,
                                               OCPIResponse  Response)

            => OnGetCDRResponse.WhenAll(Timestamp,
                                        API ?? this,
                                        Request,
                                        Response);

        #endregion

        #endregion

        #region Token(s)

        #region (protected internal) GetTokensRequest (Request)

        /// <summary>
        /// An event sent whenever a GET Tokens request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTokensRequest = new ();

        /// <summary>
        /// An event sent whenever a GET Tokens request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTokensRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnGetTokensRequest.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) GetTokensResponse(Response)

        /// <summary>
        /// An event sent whenever a GET Tokens response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTokensResponse = new ();

        /// <summary>
        /// An event sent whenever a GET Tokens response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTokensResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnGetTokensResponse.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion


        #region (protected internal) DeleteTokensRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE Tokens request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTokensRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE Tokens request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTokensRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnDeleteTokensRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) DeleteTokensResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE Tokens response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTokensResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE Tokens response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTokensResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnDeleteTokensResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        // Token

        #region (protected internal) GetTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a GET Token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTokenRequest = new ();

        /// <summary>
        /// An event sent whenever a GET Token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTokenRequest(DateTime     Timestamp,
                                                HTTPAPI      API,
                                                OCPIRequest  Request)

            => OnGetTokenRequest.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) GetTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a GET Token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTokenResponse = new ();

        /// <summary>
        /// An event sent whenever a GET Token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTokenResponse(DateTime      Timestamp,
                                                 HTTPAPI       API,
                                                 OCPIRequest   Request,
                                                 OCPIResponse  Response)

            => OnGetTokenResponse.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) PostTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTokenRequest = new ();

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTokenRequest(DateTime     Timestamp,
                                                 HTTPAPI      API,
                                                 OCPIRequest  Request)

            => OnPostTokenRequest.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) PostTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a POST token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostTokenResponse = new ();

        /// <summary>
        /// An event sent whenever a POST token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTokenResponse(DateTime      Timestamp,
                                                  HTTPAPI       API,
                                                  OCPIRequest   Request,
                                                  OCPIResponse  Response)

            => OnPostTokenResponse.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion


        #region (protected internal) PutTokenRequest    (Request)

        /// <summary>
        /// An event sent whenever a put token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTokenRequest = new ();

        /// <summary>
        /// An event sent whenever a put token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTokenRequest(DateTime     Timestamp,
                                                HTTPAPI      API,
                                                OCPIRequest  Request)

            => OnPutTokenRequest.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) PutTokenResponse   (Response)

        /// <summary>
        /// An event sent whenever a put token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTokenResponse = new ();

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

            => OnPutTokenResponse.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) PatchTokenRequest  (Request)

        /// <summary>
        /// An event sent whenever a patch token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTokenRequest = new ();

        /// <summary>
        /// An event sent whenever a patch token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTokenRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnPatchTokenRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) PatchTokenResponse (Response)

        /// <summary>
        /// An event sent whenever a patch token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTokenResponse = new ();

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

            => OnPatchTokenResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) DeleteTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a delete token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTokenRequest = new ();

        /// <summary>
        /// An event sent whenever a delete token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTokenRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnDeleteTokenRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) DeleteTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a delete token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTokenResponse = new ();

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

            => OnDeleteTokenResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion

        #endregion


        // Commands

        #region (protected internal) ReserveNowRequest        (Request)

        /// <summary>
        /// An event sent whenever a reserve now command was received.
        /// </summary>
        public OCPIRequestLogEvent OnReserveNowRequest = new ();

        /// <summary>
        /// An event sent whenever a reserve now command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnReserveNowRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region OnReserveNowCommand

        public delegate Task<CommandResponse> OnReserveNowCommandDelegate(EMSP_Id            EMSPId,
                                                                          ReserveNowCommand  ReserveNowCommand);

        public event OnReserveNowCommandDelegate? OnReserveNowCommand;

        #endregion

        #region (protected internal) ReserveNowResponse       (Response)

        /// <summary>
        /// An event sent whenever a reserve now command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnReserveNowResponse = new ();

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

            => OnReserveNowResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) CancelReservationRequest (Request)

        /// <summary>
        /// An event sent whenever a cancel reservation command was received.
        /// </summary>
        public OCPIRequestLogEvent OnCancelReservationRequest = new ();

        /// <summary>
        /// An event sent whenever a cancel reservation command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationRequest(DateTime     Timestamp,
                                                         HTTPAPI      API,
                                                         OCPIRequest  Request)

            => OnCancelReservationRequest.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request);

        #endregion

        #region OnCancelReservationCommand

        public delegate Task<CommandResponse> OnCancelReservationCommandDelegate(EMSP_Id                   EMSPId,
                                                                                 CancelReservationCommand  CancelReservationCommand);

        public event OnCancelReservationCommandDelegate? OnCancelReservationCommand;

        #endregion

        #region (protected internal) CancelReservationResponse(Response)

        /// <summary>
        /// An event sent whenever a cancel reservation command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnCancelReservationResponse = new ();

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

            => OnCancelReservationResponse.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request,
                                                   Response);

        #endregion


        #region (protected internal) StartSessionRequest      (Request)

        /// <summary>
        /// An event sent whenever a start session command was received.
        /// </summary>
        public OCPIRequestLogEvent OnStartSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a start session command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnStartSessionRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region OnStartSessionCommand

        public delegate Task<CommandResponse> OnStartSessionCommandDelegate(EMSP_Id              EMSPId,
                                                                            StartSessionCommand  StartSessionCommand);

        public event OnStartSessionCommandDelegate? OnStartSessionCommand;

        #endregion

        #region (protected internal) StartSessionResponse     (Response)

        /// <summary>
        /// An event sent whenever a start session command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStartSessionResponse = new ();

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

            => OnStartSessionResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) StopSessionRequest       (Request)

        /// <summary>
        /// An event sent whenever a stop session command was received.
        /// </summary>
        public OCPIRequestLogEvent OnStopSessionRequest = new ();

        /// <summary>
        /// An event sent whenever a stop session command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnStopSessionRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region OnStopSessionCommand

        public delegate Task<CommandResponse> OnStopSessionCommandDelegate(EMSP_Id             EMSPId,
                                                                           StopSessionCommand  StopSessionCommand);

        public event OnStopSessionCommandDelegate? OnStopSessionCommand;

        #endregion

        #region (protected internal) StopSessionResponse      (Response)

        /// <summary>
        /// An event sent whenever a stop session command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStopSessionResponse = new ();

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

            => OnStopSessionResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) UnlockConnectorRequest   (Request)

        /// <summary>
        /// An event sent whenever a unlock connector command was received.
        /// </summary>
        public OCPIRequestLogEvent OnUnlockConnectorRequest = new ();

        /// <summary>
        /// An event sent whenever a unlock connector command was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the command request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnUnlockConnectorRequest.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region OnUnlockConnectorCommand

        public delegate Task<CommandResponse> OnUnlockConnectorCommandDelegate(EMSP_Id                 EMSPId,
                                                                               UnlockConnectorCommand  UnlockConnectorCommand);

        public event OnUnlockConnectorCommandDelegate? OnUnlockConnectorCommand;

        #endregion

        #region (protected internal) UnlockConnectorResponse  (Response)

        /// <summary>
        /// An event sent whenever a unlock connector command response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnUnlockConnectorResponse = new ();

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

            => OnUnlockConnectorResponse.WhenAll(Timestamp,
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
        /// <param name="CommonAPI">The OCPI CommonAPI.</param>
        /// <param name="DefaultCountryCode">The default country code to use.</param>
        /// <param name="DefaultPartyId">The default party identification to use.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// 
        /// <param name="URLPathPrefix">An optional URL path prefix, used when defining URL templates.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="DisableMaintenanceTasks">Disable all maintenance tasks.</param>
        /// <param name="MaintenanceInitialDelay">The initial delay of the maintenance tasks.</param>
        /// <param name="MaintenanceEvery">The maintenance intervall.</param>
        /// 
        /// <param name="DisableWardenTasks">Disable all warden tasks.</param>
        /// <param name="WardenInitialDelay">The initial delay of the warden tasks.</param>
        /// <param name="WardenCheckEvery">The warden intervall.</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable the log file.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        /// <param name="Autostart">Whether to start the API automatically.</param>
        public CPOAPI(CommonAPI                CommonAPI,
                      CountryCode              DefaultCountryCode,
                      Party_Id                 DefaultPartyId,
                      Boolean?                 AllowDowngrades           = null,

                      HTTPHostname?            HTTPHostname              = null,
                      String?                  ExternalDNSName           = "",
                      String?                  HTTPServiceName           = DefaultHTTPServiceName,
                      HTTPPath?                BasePath                  = null,

                      HTTPPath?                URLPathPrefix             = null,
                      JObject?                 APIVersionHashes          = null,

                      Boolean?                 DisableMaintenanceTasks   = false,
                      TimeSpan?                MaintenanceInitialDelay   = null,
                      TimeSpan?                MaintenanceEvery          = null,

                      Boolean?                 DisableWardenTasks        = false,
                      TimeSpan?                WardenInitialDelay        = null,
                      TimeSpan?                WardenCheckEvery          = null,

                      Boolean?                 IsDevelopment             = false,
                      IEnumerable<String>?     DevelopmentServers        = null,
                      Boolean?                 DisableLogging            = false,
                      String?                  LoggingContext            = null,
                      String?                  LoggingPath               = null,
                      String?                  LogfileName               = DefaultLogfileName,
                      LogfileCreatorDelegate?  LogfileCreator            = null,
                      Boolean                  Autostart                 = false)

            : base(CommonAPI.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   BasePath,

                   URLPathPrefix   ?? DefaultURLPathPrefix,
                   null, //HTMLTemplate,
                   APIVersionHashes,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName     ?? DefaultLogfileName,
                   LogfileCreator,
                   Autostart)

        {

            this.CommonAPI           = CommonAPI;
            this.DefaultCountryCode  = DefaultCountryCode;
            this.DefaultPartyId      = DefaultPartyId;
            this.AllowDowngrades     = AllowDowngrades;

            this.CPOAPILogger        = this.DisableLogging == false
                                           ? new CPOAPILogger(
                                                 this,
                                                 LoggingContext,
                                                 LoggingPath,
                                                 LogfileCreator
                                             )
                                           : null;

            RegisterURLTemplates();

        }

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region GET    [/cpo] == /

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPathPrefix + "/cpo", "cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CPOAPI.HTTPRoot",
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
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CPOAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CPOAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CPOAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

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


            #region ~/locations

            #region OPTIONS  ~/locations

            // https://example.com/ocpi/2.2/cpo/locations/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "locations",
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetLocationsRequest,
                                    OCPIResponseLogger:  GetLocationsResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if ((Request.LocalAccessInfo is not null || CommonAPI.LocationsAsOpenData == false) &&
                                            (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                             Request.LocalAccessInfo?.Role   != Roles.EMSP))
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters              = Request.GetDateAndPaginationFilters();

                                                                                            //ToDo: Filter to NOT show all locations to everyone!
                                        var allLocations         = CommonAPI.GetLocations().//location => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == location.CountryCode &&
                                                                                            //                                                       role.PartyId     == location.PartyId)).
                                                                             ToArray();

                                        var filteredLocations    = allLocations.Where(location => !filters.From.HasValue || location.LastUpdated >  filters.From.Value).
                                                                                Where(location => !filters.To.  HasValue || location.LastUpdated <= filters.To.  Value).
                                                                                ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                                       AccessControlAllowHeaders  = "Authorization"
                                                                   }.

                                                                   // The overall number of locations
                                                                   Set("X-Total-Count",  allLocations.Length).

                                                                   // The maximum number of locations that the server WILL return within a single request
                                                                   Set("X-Limit",        allLocations.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allLocations.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/locations{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredLocations.
                                                                                      OrderBy       (location => location.Created).
                                                                                      SkipTakeFilter(filters.Offset,
                                                                                                     filters.Limit).
                                                                                      SafeSelect    (location => location.ToJSON(false,
                                                                                                                                 Request.EMSPId,
                                                                                                                                 CustomLocationSerializer,
                                                                                                                                 CustomAdditionalGeoLocationSerializer,
                                                                                                                                 CustomEVSESerializer,
                                                                                                                                 CustomStatusScheduleSerializer,
                                                                                                                                 CustomConnectorSerializer,
                                                                                                                                 CustomEnergyMeterSerializer,
                                                                                                                                 CustomTransparencySoftwareStatusSerializer,
                                                                                                                                 CustomTransparencySoftwareSerializer,
                                                                                                                                 CustomDisplayTextSerializer,
                                                                                                                                 CustomBusinessDetailsSerializer,
                                                                                                                                 CustomHoursSerializer,
                                                                                                                                 CustomImageSerializer,
                                                                                                                                 CustomEnergyMixSerializer,
                                                                                                                                 CustomEnergySourceSerializer,
                                                                                                                                 CustomEnvironmentalImpactSerializer))
                                                                              )
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/locations/{locationId}

            #region OPTIONS  ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "locations/{locationId}",
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetLocationRequest,
                                    OCPIResponseLogger:  GetLocationResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check location

                                        if (!Request.ParseLocation(this,
                                                                   CommonAPI.OurCountryCode,
                                                                   CommonAPI.OurPartyId,
                                                                   out var locationId,
                                                                   out var location,
                                                                   out var ocpiResponseBuilder,
                                                                   FailOnMissingLocation: true) ||
                                             location is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = location.ToJSON(false,
                                                                                          Request.EMSPId,
                                                                                          CustomLocationSerializer,
                                                                                          CustomAdditionalGeoLocationSerializer,
                                                                                          CustomEVSESerializer,
                                                                                          CustomStatusScheduleSerializer,
                                                                                          CustomConnectorSerializer,
                                                                                          CustomEnergyMeterSerializer,
                                                                                          CustomTransparencySoftwareStatusSerializer,
                                                                                          CustomTransparencySoftwareSerializer,
                                                                                          CustomDisplayTextSerializer,
                                                                                          CustomBusinessDetailsSerializer,
                                                                                          CustomHoursSerializer,
                                                                                          CustomImageSerializer,
                                                                                          CustomEnergyMixSerializer,
                                                                                          CustomEnergySourceSerializer,
                                                                                          CustomEnvironmentalImpactSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       Server                     = ServiceName,
                                                       Date                       = Timestamp.Now,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = location.LastUpdated.ToIso8601(),
                                                       ETag                       = location.ETag
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
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetEVSERequest,
                                    OCPIResponseLogger:  GetEVSEResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check EVSE

                                        if (!Request.ParseLocationEVSE(this,
                                                                       CommonAPI.OurCountryCode,  //Request.AccessInfo.Value.CountryCode,
                                                                       CommonAPI.OurPartyId,      //Request.AccessInfo.Value.PartyId,
                                                                       out var locationId,
                                                                       out var location,
                                                                       out var evseUId,
                                                                       out var evse,
                                                                       out var ocpiResponseBuilder) ||
                                             evse is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = evse.ToJSON(Request.EMSPId,
                                                                                      CustomEVSESerializer,
                                                                                      CustomStatusScheduleSerializer,
                                                                                      CustomConnectorSerializer,
                                                                                      CustomEnergyMeterSerializer,
                                                                                      CustomTransparencySoftwareStatusSerializer,
                                                                                      CustomTransparencySoftwareSerializer,
                                                                                      CustomDisplayTextSerializer,
                                                                                      CustomImageSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = evse.LastUpdated.ToIso8601(),
                                                       ETag                       = evse.ETag
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
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetConnectorRequest,
                                    OCPIResponseLogger:  GetConnectorResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check connector

                                        if (!Request.ParseLocationEVSEConnector(this,
                                                                                CommonAPI.OurCountryCode,  //Request.AccessInfo.Value.CountryCode,
                                                                                CommonAPI.OurPartyId,      //Request.AccessInfo.Value.PartyId,
                                                                                out var locationId,
                                                                                out var location,
                                                                                out var evseId,
                                                                                out var evse,
                                                                                out var connectorId,
                                                                                out var connector,
                                                                                out var ocpiResponseBuilder) ||
                                             connector is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = connector.ToJSON(Request.EMSPId,
                                                                                           CustomConnectorSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = connector.LastUpdated.ToIso8601()
                                                   }
                                            });

                                    });

            #endregion

            #endregion


            //ToDo: OpenData accesss to CPO public charging tariffs!

            #region ~/tariffs

            #region OPTIONS  ~/tariffs

            // https://example.com/ocpi/2.2/cpo/tariffs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "tariffs",
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetTariffsRequest,
                                    OCPIResponseLogger:  GetTariffsResponse,
                                    OCPIRequestHandler:  Request => {

                                        //ToDo: OpenData accesss to CPO public charging tariffs!

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters          = Request.GetDateAndPaginationFilters();

                                        //ToDo: Maybe not all EMSP should see all charging tariffs!
                                        var allTariffs       = CommonAPI.GetTariffs(//CommonAPI.OurCountryCode,  //Request.AccessInfo.Value.CountryCode,
                                                                                    //CommonAPI.OurPartyId       //Request.AccessInfo.Value.PartyId
                                                                                   ).ToArray();

                                        var filteredTariffs  = allTariffs.Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                                          Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                                          ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                                       AccessControlAllowHeaders  = "Authorization"
                                                                   }.

                                                                   // The overall number of tariffs
                                                                   Set("X-Total-Count",  allTariffs.Length).

                                                                   // The maximum number of tariffs that the server WILL return within a single request
                                                                   Set("X-Limit",        allTariffs.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allTariffs.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/tariffs{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredTariffs.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(tariff => tariff.ToJSON(false,
                                                                                                                 CustomTariffSerializer,
                                                                                                                 CustomDisplayTextSerializer,
                                                                                                                 CustomTariffElementSerializer,
                                                                                                                 CustomPriceComponentSerializer,
                                                                                                                 CustomTariffRestrictionsSerializer,
                                                                                                                 CustomEnergyMixSerializer,
                                                                                                                 CustomEnergySourceSerializer,
                                                                                                                 CustomEnvironmentalImpactSerializer))
                                                                              )
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/tariffs/{tariffId}        [NonStandard]

            #region OPTIONS  ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "tariffs/{tariffId}",
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetTariffRequest,
                                    OCPIResponseLogger:  GetTariffResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check tariff

                                        if (!Request.ParseTariff(this,
                                                                 CommonAPI.OurCountryCode,  //Request.AccessInfo.Value.CountryCode,
                                                                 CommonAPI.OurPartyId,      //Request.AccessInfo.Value.PartyId,
                                                                 out var tariffId,
                                                                 out var tariff,
                                                                 out var ocpiResponseBuilder,
                                                                 FailOnMissingTariff: true) ||
                                             tariff is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = tariff.ToJSON(false,
                                                                                        CustomTariffSerializer,
                                                                                        CustomDisplayTextSerializer,
                                                                                        CustomTariffElementSerializer,
                                                                                        CustomPriceComponentSerializer,
                                                                                        CustomTariffRestrictionsSerializer,
                                                                                        CustomEnergyMixSerializer,
                                                                                        CustomEnergySourceSerializer,
                                                                                        CustomEnvironmentalImpactSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = tariff.LastUpdated.ToIso8601(),
                                                       ETag                       = tariff.ETag
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
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetSessionsRequest,
                                    OCPIResponseLogger:  GetSessionsResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters              = Request.GetDateAndPaginationFilters();

                                        var allSessions          = CommonAPI.GetSessions(CommonAPI.OurCountryCode,  //Request.AccessInfo.Value.CountryCode,
                                                                                         CommonAPI.OurPartyId       //Request.AccessInfo.Value.PartyId
                                                                                        ).ToArray();

                                        var filteredSessions     = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                                               Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                                               ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                                       AccessControlAllowHeaders  = "Authorization"
                                                                   }.

                                                                   // The overall number of sessions
                                                                   Set("X-Total-Count",  allSessions.Length).

                                                                   // The maximum number of sessions that the server WILL return within a single request
                                                                   Set("X-Limit",        allSessions.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allSessions.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/sessions{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredSessions.
                                                                                      SkipTakeFilter(filters.Offset,
                                                                                                     filters.Limit).
                                                                                      Select(session => session.ToJSON(false,
                                                                                                                       Request.EMSPId,
                                                                                                                       CustomSessionSerializer,
                                                                                                                       CustomLocationSerializer,
                                                                                                                       CustomAdditionalGeoLocationSerializer,
                                                                                                                       CustomEVSESerializer,
                                                                                                                       CustomStatusScheduleSerializer,
                                                                                                                       CustomConnectorSerializer,
                                                                                                                       CustomEnergyMeterSerializer,
                                                                                                                       CustomTransparencySoftwareStatusSerializer,
                                                                                                                       CustomTransparencySoftwareSerializer,
                                                                                                                       CustomDisplayTextSerializer,
                                                                                                                       CustomBusinessDetailsSerializer,
                                                                                                                       CustomHoursSerializer,
                                                                                                                       CustomImageSerializer,
                                                                                                                       CustomEnergyMixSerializer,
                                                                                                                       CustomEnergySourceSerializer,
                                                                                                                       CustomEnvironmentalImpactSerializer,
                                                                                                                       CustomChargingPeriodSerializer,
                                                                                                                       CustomCDRDimensionSerializer))
                                                                              )
                                                   }
                                               );

                                    });

            #endregion

            #endregion

            #region ~/sessions/{sessionId}      [NonStandard]

            #region OPTIONS  ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "sessions/{sessionId}",
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetSessionRequest,
                                    OCPIResponseLogger:  GetSessionResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check session

                                        if (!Request.ParseSession(this,
                                                                  CommonAPI.OurCountryCode,  //Request.AccessInfo.Value.CountryCode,
                                                                  CommonAPI.OurPartyId,      //Request.AccessInfo.Value.PartyId,
                                                                  out var sessionId,
                                                                  out var session,
                                                                  out var ocpiResponseBuilder,
                                                                  FailOnMissingSession: true) ||
                                             session is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = session.ToJSON(false,
                                                                                         Request.EMSPId,
                                                                                         CustomSessionSerializer,
                                                                                         CustomLocationSerializer,
                                                                                         CustomAdditionalGeoLocationSerializer,
                                                                                         CustomEVSESerializer,
                                                                                         CustomStatusScheduleSerializer,
                                                                                         CustomConnectorSerializer,
                                                                                         CustomEnergyMeterSerializer,
                                                                                         CustomTransparencySoftwareStatusSerializer,
                                                                                         CustomTransparencySoftwareSerializer,
                                                                                         CustomDisplayTextSerializer,
                                                                                         CustomBusinessDetailsSerializer,
                                                                                         CustomHoursSerializer,
                                                                                         CustomImageSerializer,
                                                                                         CustomEnergyMixSerializer,
                                                                                         CustomEnergySourceSerializer,
                                                                                         CustomEnvironmentalImpactSerializer,
                                                                                         CustomChargingPeriodSerializer,
                                                                                         CustomCDRDimensionSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = session.LastUpdated.ToIso8601(),
                                                       ETag                       = session.ETag
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
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetCDRsRequest,
                                    OCPIResponseLogger:  GetCDRsResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion


                                        var filters              = Request.GetDateAndPaginationFilters();

                                        var allCDRs              = CommonAPI.GetCDRs(CommonAPI.OurCountryCode,  //Request.AccessInfo.Value.CountryCode,
                                                                                     CommonAPI.OurPartyId       //Request.AccessInfo.Value.PartyId
                                                                                    ).ToArray();

                                        var filteredCDRs         = allCDRs.Where(CDR => !filters.From.HasValue || CDR.LastUpdated >  filters.From.Value).
                                                                           Where(CDR => !filters.To.  HasValue || CDR.LastUpdated <= filters.To.  Value).
                                                                           ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                                       AccessControlAllowHeaders  = "Authorization"
                                                                   }.

                                                                   // The overall number of CDRs
                                                                   Set("X-Total-Count",  allCDRs.Length).

                                                                   // The maximum number of CDRs that the server WILL return within a single request
                                                                   Set("X-Limit",        allCDRs.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allCDRs.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/cdrs{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredCDRs.
                                                                                  SkipTakeFilter(filters.Offset,
                                                                                                 filters.Limit).
                                                                                  Select(CDR => CDR.ToJSON(false,
                                                                                                           Request.EMSPId,
                                                                                                           CustomCDRSerializer,
                                                                                                           CustomLocationSerializer,
                                                                                                           CustomAdditionalGeoLocationSerializer,
                                                                                                           CustomEVSESerializer,
                                                                                                           CustomStatusScheduleSerializer,
                                                                                                           CustomConnectorSerializer,
                                                                                                           CustomEnergyMeterSerializer,
                                                                                                           CustomTransparencySoftwareStatusSerializer,
                                                                                                           CustomTransparencySoftwareSerializer,
                                                                                                           CustomDisplayTextSerializer,
                                                                                                           CustomBusinessDetailsSerializer,
                                                                                                           CustomHoursSerializer,
                                                                                                           CustomImageSerializer,
                                                                                                           CustomEnergyMixSerializer,
                                                                                                           CustomEnergySourceSerializer,
                                                                                                           CustomEnvironmentalImpactSerializer,
                                                                                                           CustomTariffSerializer,
                                                                                                           CustomTariffElementSerializer,
                                                                                                           CustomPriceComponentSerializer,
                                                                                                           CustomTariffRestrictionsSerializer,
                                                                                                           CustomChargingPeriodSerializer,
                                                                                                           CustomCDRDimensionSerializer,
                                                                                                           CustomSignedDataSerializer,
                                                                                                           CustomSignedValueSerializer))
                                                                              )
                                                   }
                                               );

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
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       Allow                      = new List<HTTPMethod> {
                                                                                        HTTPMethod.OPTIONS,
                                                                                        HTTPMethod.GET
                                                                                    },
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
                                    OCPIRequestLogger:   GetCDRRequest,
                                    OCPIResponseLogger:  GetCDRResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check Charge Detail Record

                                        if (!Request.ParseCDR(this,
                                                              CommonAPI.OurCountryCode,  //Request.AccessInfo.Value.CountryCode,
                                                              CommonAPI.OurPartyId,      //Request.AccessInfo.Value.PartyId,
                                                              out var cdrId,
                                                              out var cdr,
                                                              out var ocpiResponseBuilder,
                                                              FailOnMissingCDR: true) ||
                                             cdr is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = cdr.ToJSON(false,
                                                                                     Request.EMSPId,
                                                                                     CustomCDRSerializer,
                                                                                     CustomLocationSerializer,
                                                                                     CustomAdditionalGeoLocationSerializer,
                                                                                     CustomEVSESerializer,
                                                                                     CustomStatusScheduleSerializer,
                                                                                     CustomConnectorSerializer,
                                                                                     CustomEnergyMeterSerializer,
                                                                                     CustomTransparencySoftwareStatusSerializer,
                                                                                     CustomTransparencySoftwareSerializer,
                                                                                     CustomDisplayTextSerializer,
                                                                                     CustomBusinessDetailsSerializer,
                                                                                     CustomHoursSerializer,
                                                                                     CustomImageSerializer,
                                                                                     CustomEnergyMixSerializer,
                                                                                     CustomEnergySourceSerializer,
                                                                                     CustomEnvironmentalImpactSerializer,
                                                                                     CustomTariffSerializer,
                                                                                     CustomTariffElementSerializer,
                                                                                     CustomPriceComponentSerializer,
                                                                                     CustomTariffRestrictionsSerializer,
                                                                                     CustomChargingPeriodSerializer,
                                                                                     CustomCDRDimensionSerializer,
                                                                                     CustomSignedDataSerializer,
                                                                                     CustomSignedValueSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = cdr.LastUpdated.ToIso8601(),
                                                       ETag                       = cdr.ETag
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
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                           Allow                      = new List<HTTPMethod> {
                                                                                            HTTPMethod.OPTIONS,
                                                                                            HTTPMethod.GET,
                                                                                            HTTPMethod.DELETE
                                                                                        },
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
                                    OCPIRequestLogger:   GetTokensRequest,
                                    OCPIResponseLogger:  GetTokensResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check country code and party identification

                                        if (!Request.ParseParseCountryCodePartyId(this,
                                                                                  out var countryCode,
                                                                                  out var partyId,
                                                                                  out var ocpiResponseBuilder) ||
                                            !countryCode.HasValue ||
                                            !partyId.    HasValue)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        var filters              = Request.GetDateAndPaginationFilters();

                                        var allTokens            = CommonAPI.GetTokens(CommonAPI.OurCountryCode,  //countryCode.Value,
                                                                                       CommonAPI.OurPartyId       //partyId.    Value
                                                                                      ).
                                                                             ToArray();

                                        var filteredTokens       = allTokens.Select(tokenStatus => tokenStatus.Token).
                                                                             Where (token       => !filters.From.HasValue || token.LastUpdated >  filters.From.Value).
                                                                             Where (token       => !filters.To.  HasValue || token.LastUpdated <= filters.To.  Value).
                                                                             ToArray();


                                        var httpResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                                       Server                     = DefaultHTTPServerName,
                                                                       Date                       = Timestamp.Now,
                                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                                       AccessControlAllowHeaders  = "Authorization"
                                                                   }.

                                                                   // The overall number of tokens
                                                                   Set("X-Total-Count",  allTokens.Length).

                                                                   // The maximum number of tokens that the server WILL return within a single request
                                                                   Set("X-Limit",        allTokens.Length);


                                        #region When the limit query parameter was set & this is not the last pagination page...

                                        if (filters.Limit.HasValue &&
                                            allTokens.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                                        {

                                            // The new query parameters for the "next" page of pagination within the HTTP Link header
                                            var queryParameters    = new List<String?>() {
                                                                         filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                                         filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                                         filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                                         filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                                     }.Where(queryParameter => queryParameter is not null).
                                                                       AggregateWith("&");

                                            if (queryParameters.Length > 0)
                                                queryParameters = "?" + queryParameters;

                                            // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                                            //   - Link: <https://www.server.com/ocpi/cpo/2.0/cdrs/?offset=150&limit=50>; rel="next"
                                            httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}{URLPathPrefix}/tokens/{countryCode}/{partyId}{queryParameters}>; rel=\"next\"");

                                        }

                                        #endregion

                                        return Task.FromResult(
                                                   new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       HTTPResponseBuilder  = httpResponseBuilder,
                                                       Data                 = new JArray(
                                                                                  filteredTokens.
                                                                                      SkipTakeFilter(filters.Offset,
                                                                                                     filters.Limit).
                                                                                      Select(token => token.ToJSON(false,
                                                                                                                   CustomTokenSerializer))
                                                                              )
                                                   }
                                               );

                                    });

            #endregion

            #region DELETE   ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteTokensRequest,
                                    OCPIResponseLogger:  DeleteTokensResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check country code and party identification

                                        if (!Request.ParseParseCountryCodePartyId(this,
                                                                                  out var countryCode,
                                                                                  out var partyId,
                                                                                  out var ocpiResponseBuilder) ||
                                            !countryCode.HasValue ||
                                            !partyId.    HasValue)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        CommonAPI.RemoveAllTokens(countryCode.Value,
                                                                  partyId.    Value);


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "DELETE" },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                    });

            #endregion

            #endregion

            #region ~/tokens/{country_code}/{party_id}/{tokenId}

            #region OPTIONS  ~/tokens/{country_code}/{party_id}/{tokenId}      [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.OPTIONS,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    OCPIRequestHandler: Request => {

                                        return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
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

            #region GET     ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.GET,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   GetTokenRequest,
                                    OCPIResponseLogger:  GetTokenResponse,
                                    OCPIRequestHandler:  Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return Task.FromResult(
                                                new OCPIResponse.Builder(Request) {
                                                    StatusCode           = 2000,
                                                    StatusMessage        = "Invalid or blocked access token!",
                                                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                        HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                        AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                        AccessControlAllowHeaders  = "Authorization"
                                                    }
                                                });

                                        }

                                        #endregion

                                        #region Check country code, party identification, token identification and existing token status

                                        if (!Request.ParseToken(this,
                                                                out var countryCode,
                                                                out var partyId,
                                                                out var tokenId,
                                                                out var tokenStatus,
                                                                out var ocpiResponseBuilder) ||
                                             tokenStatus?.Token is null)
                                        {
                                            return Task.FromResult(ocpiResponseBuilder!);
                                        }

                                        #endregion


                                        //ToDo: What exactly to do with this information?
                                        var tokenType  = Request.QueryString.Map("type", TokenType.TryParse) ?? TokenType.RFID;


                                        return Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = tokenStatus.Value.Token.ToJSON(false,
                                                                                                         CustomTokenSerializer),
                                                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = tokenStatus.Value.Token.LastUpdated.ToIso8601(),
                                                       ETag                       = tokenStatus.Value.Token.ETag
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
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo is null ||
                                            Request.LocalAccessInfo.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo.Role   != Roles.EMSP)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check country code, party identification and token identification

                                        if (!Request.ParseToken(this,
                                                                out var countryCode,
                                                                out var partyId,
                                                                out var tokenId,
                                                                out var existingTokenStatus,
                                                                out var ocpiResponseBuilder,
                                                                FailOnMissingToken: false))
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Parse new or updated token JSON

                                        if (!Request.TryParseJObjectRequestBody(out var tokenJSON, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        if (!Token.TryParse(tokenJSON,
                                                            out var newOrUpdatedToken,
                                                            out var errorResponse,
                                                            countryCode,
                                                            partyId,
                                                            tokenId) ||
                                             newOrUpdatedToken is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2001,
                                                   StatusMessage        = "Could not parse the given token JSON: " + errorResponse,
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                               };

                                        }

                                        #endregion


                                        //ToDo: What exactly to do with this information?
                                        var tokenType  = Request.QueryString.Map("type", TokenType.TryParse) ?? TokenType.RFID;

                                        var result     = await CommonAPI.AddOrUpdateToken(newOrUpdatedToken,
                                                                                          AllowDowngrades: AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (result.IsSuccess &&
                                            result.Data is not null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = result.Data.ToJSON(false,
                                                                                                 CustomTokenSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = result.WasCreated == true
                                                                                            ? HTTPStatusCode.Created
                                                                                            : HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                           AccessControlAllowHeaders  = "Authorization",
                                                           LastModified               = result.Data.LastUpdated.ToIso8601(),
                                                           ETag                       = result.Data.ETag
                                                       }
                                                   };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 2000,
                                                   StatusMessage        = result.ErrorResponse,
                                                   Data                 = newOrUpdatedToken.ToJSON(false,
                                                                                                   CustomTokenSerializer),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                       AccessControlAllowHeaders  = "Authorization",
                                                       LastModified               = result.Data?.LastUpdated.ToIso8601(),
                                                       ETag                       = result.Data?.ETag
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
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role != Roles.EMSP)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check country code, party identification, token identification and existing token status

                                        if (!Request.ParseToken(this,
                                                                 out var countryCode,
                                                                 out var partyId,
                                                                 out var tokenId,
                                                                 out var existingTokenStatus,
                                                                 out var ocpiResponseBuilder) ||
                                             existingTokenStatus?.Token is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion

                                        #region Try to parse the JSON patch

                                        if (!Request.TryParseJObjectRequestBody(out var tokenPatch, out ocpiResponseBuilder))
                                            return ocpiResponseBuilder;

                                        #endregion


                                        //ToDo: What exactly to do with this information?
                                        var tokenType = Request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;


                                        //ToDo: Validation-Checks for PATCHes (E-Tag, Timestamp, ...)
                                        var patchedToken = await CommonAPI.TryPatchToken(existingTokenStatus.Value.Token,
                                                                                         tokenPatch,
                                                                                         AllowDowngrades ?? Request.QueryString.GetBoolean("forceDowngrade"));


                                        if (patchedToken.IsSuccess &&
                                            patchedToken.PatchedData is not null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                           StatusCode           = 1000,
                                                           StatusMessage        = "Hello world!",
                                                           Data                 = patchedToken.PatchedData.ToJSON(),
                                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                               AccessControlAllowHeaders  = "Authorization",
                                                               LastModified               = patchedToken.PatchedData.LastUpdated.ToIso8601(),
                                                               ETag                       = patchedToken.PatchedData.ETag
                                                           }
                                                       };

                                        }

                                        return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = patchedToken.ErrorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                    });

            #endregion

            #region DELETE  ~/tokens/{country_code}/{party_id}/{tokenId}       [NonStandard]

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.DELETE,
                                    URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   DeleteTokenRequest,
                                    OCPIResponseLogger:  DeleteTokenResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED ||
                                            Request.LocalAccessInfo?.Role   != Roles.EMSP)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Check country code, party identification, token identification and existing token status

                                        if (!Request.ParseToken(this,
                                                                out var countryCode,
                                                                out var partyId,
                                                                out var tokenId,
                                                                out var existingTokenStatus,
                                                                out var ocpiResponseBuilder) ||
                                             existingTokenStatus?.Token is null)
                                        {
                                            return ocpiResponseBuilder!;
                                        }

                                        #endregion


                                        //ToDo: What exactly to do with this information?
                                        var tokenType  = Request.QueryString.Map("type", TokenType.TryParse) ?? TokenType.RFID;

                                        var result     = await CommonAPI.RemoveToken(existingTokenStatus.Value.Token);

                                        if (result.IsSuccess)
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 1000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingTokenStatus.Value.Token.ToJSON(false,
                                                                                                                     CustomTokenSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                           //LastModified               = Timestamp.Now.ToIso8601()
                                                       }
                                                   };

                                        else
                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Hello world!",
                                                       Data                 = existingTokenStatus.Value.Token.ToJSON(false,
                                                                                                                     CustomTokenSerializer),
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "PATCH", "DELETE" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                           //LastModified               = Timestamp.Now.ToIso8601()
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
                                    OCPIRequestHandler: Request =>

                                        Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            })

                                    );

            #endregion

            #region POST     ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/RESERVE_NOW",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   ReserveNowRequest,
                                    OCPIResponseLogger:  ReserveNowResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse ReserveNow command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var reserveNowJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!ReserveNowCommand.TryParse(reserveNowJSON,
                                                                        out var reserveNowCommand,
                                                                        out var errorResponse) ||
                                             reserveNowCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'RESERVE_NOW' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnReserveNowCommand is not null)
                                            commandResponse = await OnReserveNowCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                               reserveNowCommand);

                                        commandResponse ??= new CommandResponse(
                                                                reserveNowCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
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
                                    OCPIRequestHandler: Request =>

                                        Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            })

                                    );

            #endregion

            #region POST     ~/commands/CANCEL_RESERVATION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/CANCEL_RESERVATION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   CancelReservationRequest,
                                    OCPIResponseLogger:  CancelReservationResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse CancelReservation command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var cancelReservationJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!CancelReservationCommand.TryParse(cancelReservationJSON,
                                                                               out var cancelReservationCommand,
                                                                               out var errorResponse) ||
                                             cancelReservationCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'CANCEL_RESERVATION' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnCancelReservationCommand is not null)
                                            commandResponse = await OnCancelReservationCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                                      cancelReservationCommand);

                                        commandResponse ??= new CommandResponse(
                                                                cancelReservationCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
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
                                    OCPIRequestHandler: Request =>

                                        Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            })

                                    );

            #endregion

            #region POST     ~/commands/START_SESSION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/START_SESSION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   StartSessionRequest,
                                    OCPIResponseLogger:  StartSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse StartSession command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var startSessionJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!StartSessionCommand.TryParse(startSessionJSON,
                                                                          out var startSessionCommand,
                                                                          out var errorResponse) ||
                                             startSessionCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'START_SESSION' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnStartSessionCommand is not null)
                                            commandResponse = await OnStartSessionCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                                 startSessionCommand);

                                        commandResponse ??= new CommandResponse(
                                                                startSessionCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
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
                                    OCPIRequestHandler: Request =>

                                        Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            })

                                    );

            #endregion

            #region POST     ~/commands/STOP_SESSION

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/STOP_SESSION",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   StopSessionRequest,
                                    OCPIResponseLogger:  StopSessionResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse StopSession command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var stopSessionJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!StopSessionCommand.TryParse(stopSessionJSON,
                                                                          out var stopSessionCommand,
                                                                          out var errorResponse) ||
                                             stopSessionCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'STOP_SESSION' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnStopSessionCommand is not null)
                                            commandResponse = await OnStopSessionCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                                stopSessionCommand);

                                        commandResponse ??= new CommandResponse(
                                                                stopSessionCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
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
                                    OCPIRequestHandler: Request =>

                                        Task.FromResult(
                                            new OCPIResponse.Builder(Request) {
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                       AccessControlAllowHeaders  = "Authorization"
                                                   }
                                            })

                                    );

            #endregion

            #region POST     ~/commands/UNLOCK_CONNECTOR

            CommonAPI.AddOCPIMethod(HTTPHostname.Any,
                                    HTTPMethod.POST,
                                    URLPathPrefix + "commands/UNLOCK_CONNECTOR",
                                    HTTPContentType.JSON_UTF8,
                                    OCPIRequestLogger:   UnlockConnectorRequest,
                                    OCPIResponseLogger:  UnlockConnectorResponse,
                                    OCPIRequestHandler:  async Request => {

                                        #region Check access token

                                        if (Request.RemoteParty is null ||
                                            Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2000,
                                                       StatusMessage        = "Invalid or blocked access token!",
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion

                                        #region Parse UnlockConnector command JSON

                                        if (!Request.TryParseJObjectRequestBody(out var unlockConnectorJSON, out var ocpiResponse))
                                            return ocpiResponse;

                                        if (!UnlockConnectorCommand.TryParse(unlockConnectorJSON,
                                                                             out var unlockConnectorCommand,
                                                                             out var errorResponse) ||
                                             unlockConnectorCommand is null)
                                        {

                                            return new OCPIResponse.Builder(Request) {
                                                       StatusCode           = 2001,
                                                       StatusMessage        = "Could not parse the given 'UNLOCK_CONNECTOR' command JSON: " + errorResponse,
                                                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                           AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
                                                           AccessControlAllowHeaders  = "Authorization"
                                                       }
                                                   };

                                        }

                                        #endregion


                                        CommandResponse? commandResponse = null;

                                        if (OnUnlockConnectorCommand is not null)
                                            commandResponse = await OnUnlockConnectorCommand.Invoke(EMSP_Id.From(Request.RemoteParty.Id),
                                                                                                    unlockConnectorCommand);

                                        commandResponse ??= new CommandResponse(
                                                                unlockConnectorCommand,
                                                                CommandResponseTypes.NOT_SUPPORTED,
                                                                Timeout: TimeSpan.FromSeconds(15),
                                                                Message: new[] {
                                                                             new DisplayText(Languages.en, "Not supported!")
                                                                         }
                                                            );


                                        return new OCPIResponse.Builder(Request) {
                                                   StatusCode           = 1000,
                                                   StatusMessage        = "Hello world!",
                                                   Data                 = commandResponse.ToJSON(),
                                                   HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                                       AccessControlAllowMethods  = new[] { "OPTIONS", "POST" },
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
