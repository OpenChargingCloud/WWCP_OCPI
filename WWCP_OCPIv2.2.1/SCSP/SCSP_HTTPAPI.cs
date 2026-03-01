/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.SCSP.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    public static class SCSP_HTTPAPI_Extensions
    {

        #region ParseMandatoryLocation              (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The EMSP HTTP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocation(this OCPIRequest                                Request,
                                                     SCSP_HTTPAPI                                    SCSP_HTTPAPI,
                                                     [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                     [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                     [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                     [NotNullWhen(true)]  out Location?              Location,
                                                     [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            Location             = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode              = HTTPStatusCode.BadRequest,
                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                        AccessControlAllowHeaders   = [ "Authorization" ],
                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode              = HTTPStatusCode.BadRequest,
                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                        AccessControlAllowHeaders   = [ "Authorization" ],
                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                    }
                };

                return false;

            }

            PartyId = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id>("locationId",   Location_Id.     TryParse, out var locationId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode              = HTTPStatusCode.BadRequest,
                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                        AccessControlAllowHeaders   = [ "Authorization" ],
                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                    }
                };

                return false;

            }

            LocationId = locationId;


            if (!SCSP_HTTPAPI.TryGetRemoteLocation(
                Party_Idv3.From(
                    CountryCode.Value,
                    PartyId.Value
                ),
                locationId,
                out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode              = HTTPStatusCode.NotFound,
                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                        AccessControlAllowHeaders   = [ "Authorization" ],
                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                    }
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseOptionalLocation               (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The EMSP HTTP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocation(this OCPIRequest                                 Request,
                                                    SCSP_HTTPAPI                                     SCSP_HTTPAPI,
                                                    [NotNullWhen(true)]   out CountryCode?           CountryCode,
                                                    [NotNullWhen(true)]   out Party_Id?              PartyId,
                                                    [NotNullWhen(true)]   out Location_Id?           LocationId,
                                                    [MaybeNullWhen(true)] out Location?              Location,
                                                    [NotNullWhen(false)]  out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            Location             = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode              = HTTPStatusCode.BadRequest,
                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                        AccessControlAllowHeaders   = [ "Authorization" ],
                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode              = HTTPStatusCode.BadRequest,
                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                        AccessControlAllowHeaders   = [ "Authorization" ],
                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                    }
                };

                return false;

            }

            PartyId = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id>("locationId",   Location_Id.     TryParse, out var locationId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode              = HTTPStatusCode.BadRequest,
                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                        AccessControlAllowHeaders   = [ "Authorization" ],
                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                    }
                };

                return false;

            }

            LocationId = locationId;


            SCSP_HTTPAPI.TryGetRemoteLocation(
                Party_Idv3.From(
                    CountryCode.Value,
                    PartyId.Value
                ),
                locationId,
                out Location
            );

            return true;

        }

        #endregion


        #region ParseMandatoryLocationEVSE          (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocationEVSE(this OCPIRequest                                Request,
                                                         SCSP_HTTPAPI                                    SCSP_HTTPAPI,
                                                         [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                         [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                         [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                         [NotNullWhen(true)]  out Location?              Location,
                                                         [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                         [NotNullWhen(true)]  out EVSE?                  EVSE,
                                                         [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>("party_id", Party_Id.TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId     = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id>("locationId", Location_Id.TryParse, out var locationId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            LocationId  = locationId;


            if (!Request.HTTPRequest.TryParseURLParameter<EVSE_UId>("evseUId", EVSE_UId.TryParse, out var evseUId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            EVSEUId     = evseUId;


            if (!SCSP_HTTPAPI.TryGetRemoteLocation(
                Party_Idv3.From(
                    countryCode,
                    partyId
                ),
                locationId,
                out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(evseUId, out EVSE))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseOptionalLocationEVSE           (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocationEVSE(this OCPIRequest                                 Request,
                                                        SCSP_HTTPAPI                                     SCSP_HTTPAPI,
                                                        [NotNullWhen(true)]   out CountryCode?           CountryCode,
                                                        [NotNullWhen(true)]   out Party_Id?              PartyId,
                                                        [NotNullWhen(true)]   out Location_Id?           LocationId,
                                                        [NotNullWhen(true)]   out Location?              Location,
                                                        [NotNullWhen(true)]   out EVSE_UId?              EVSEUId,
                                                        [MaybeNullWhen(true)] out EVSE?                  EVSE,
                                                        [NotNullWhen(false)]  out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = null;
            PartyId              = null;
            LocationId           = null;
            Location             = null;
            EVSEUId              = null;
            EVSE                 = null;
            OCPIResponseBuilder  = null;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId     = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id>("locationId",   Location_Id.     TryParse, out var locationId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            LocationId  = locationId;


            if (!Request.HTTPRequest.TryParseURLParameter<EVSE_UId>   ("evseUId",      EVSE_UId.        TryParse, out var evseUId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            EVSEUId     = evseUId;


            SCSP_HTTPAPI.TryGetRemoteLocation(
                Party_Idv3.From(
                    countryCode,
                    partyId
                ),
                locationId,
                out Location
            );

            Location?.TryGetEVSE(
                evseUId,
                out EVSE
            );

            return true;

        }

        #endregion


        #region ParseMandatoryLocationEVSEConnector (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocationEVSEConnector(this OCPIRequest                                Request,
                                                                  SCSP_HTTPAPI                                    SCSP_HTTPAPI,
                                                                  [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                                  [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                                  [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                                  [NotNullWhen(true)]  out Location?              Location,
                                                                  [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                                  [NotNullWhen(true)]  out EVSE?                  EVSE,
                                                                  [NotNullWhen(true)]  out Connector_Id?          ConnectorId,
                                                                  [NotNullWhen(true)]  out Connector?             Connector,
                                                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            ConnectorId          = default;
            Connector            = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode> ("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>    ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId     = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id> ("locationId",   Location_Id.     TryParse, out var locationId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            LocationId  = locationId;


            if (!Request.HTTPRequest.TryParseURLParameter<EVSE_UId>    ("evseId",       EVSE_UId.        TryParse, out var evseUId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            EVSEUId     = evseUId;


            if (!Request.HTTPRequest.TryParseURLParameter<Connector_Id>("connectorId",  Connector_Id.    TryParse, out var connectorId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            ConnectorId = connectorId;


            if (!SCSP_HTTPAPI.TryGetRemoteLocation(
                Party_Idv3.From(
                    countryCode,
                    partyId
                ),
                locationId,
                out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(evseUId, out EVSE))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            if (!EVSE.TryGetConnector(connectorId, out Connector))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseOptionalLocationEVSEConnector  (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocationEVSEConnector(this OCPIRequest                                 Request,
                                                                 SCSP_HTTPAPI                                     SCSP_HTTPAPI,
                                                                 [NotNullWhen(true)]   out CountryCode?           CountryCode,
                                                                 [NotNullWhen(true)]   out Party_Id?              PartyId,
                                                                 [NotNullWhen(true)]   out Location_Id?           LocationId,
                                                                 [NotNullWhen(true)]   out Location?              Location,
                                                                 [NotNullWhen(true)]   out EVSE_UId?              EVSEUId,
                                                                 [NotNullWhen(true)]   out EVSE?                  EVSE,
                                                                 [NotNullWhen(true)]   out Connector_Id?          ConnectorId,
                                                                 [MaybeNullWhen(true)] out Connector?             Connector,
                                                                 [NotNullWhen(false)]  out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            ConnectorId          = default;
            Connector            = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId     = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id>("locationId",   Location_Id.     TryParse, out var locationId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            LocationId  = locationId;


            if (!Request.HTTPRequest.TryParseURLParameter<EVSE_UId>   ("locationId",   EVSE_UId.        TryParse, out var evseUId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            EVSEUId     = evseUId;


            if (!Request.HTTPRequest.TryParseURLParameter<Connector_Id>   ("connectorId",   Connector_Id.        TryParse, out var connectorId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            ConnectorId = connectorId;


            if (!SCSP_HTTPAPI.TryGetRemoteLocation(
                Party_Idv3.From(
                    countryCode,
                    partyId
                ),
                locationId,
                out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            if (!Location.TryGetEVSE(evseUId, out EVSE) ||
                 EVSE is null)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            EVSE.TryGetConnector(connectorId, out Connector);

            return true;

        }

        #endregion


        #region ParseMandatoryTariff                (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out TariffId,   out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryTariff(this OCPIRequest                                Request,
                                                   SCSP_HTTPAPI                                    SCSP_HTTPAPI,
                                                   [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                   [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                   [NotNullWhen(true)]  out Tariff_Id?             TariffId,
                                                   [NotNullWhen(true)]  out Tariff?                Tariff,
                                                   [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            TariffId             = default;
            Tariff               = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Tariff_Id>  ("tariffId",     Tariff_Id.       TryParse, out var tariffId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid tariff identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            TariffId = tariffId;


            if (!SCSP_HTTPAPI.TryGetRemoteTariff(
                Party_Idv3.From(
                    CountryCode.Value,
                    PartyId.Value
                ),
                TariffId.Value,
                out Tariff
            )) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown tariff identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseOptionalTariff                 (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out TariffId,   out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalTariff(this OCPIRequest                                  Request,
                                                  SCSP_HTTPAPI                                      SCSP_HTTPAPI,
                                                  [NotNullWhen  (true)]  out CountryCode?           CountryCode,
                                                  [NotNullWhen  (true)]  out Party_Id?              PartyId,
                                                  [NotNullWhen  (true)]  out Tariff_Id?             TariffId,
                                                  [MaybeNullWhen(true)]  out Tariff?                Tariff,
                                                  [NotNullWhen  (false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            TariffId             = default;
            Tariff               = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Tariff_Id>  ("tariffId",     Tariff_Id.       TryParse, out var tariffId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            TariffId = tariffId;


            SCSP_HTTPAPI.TryGetRemoteTariff(
                Party_Idv3.From(
                    CountryCode.Value,
                    PartyId.Value
                ),
                tariffId,
                out Tariff
            );

            return true;

        }

        #endregion


        #region ParseMandatorySession               (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out SessionId,  out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatorySession(this OCPIRequest                                Request,
                                                    SCSP_HTTPAPI                                    SCSP_HTTPAPI,
                                                    [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                    [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                    [NotNullWhen(true)]  out Session_Id?            SessionId,
                                                    [NotNullWhen(true)]  out Session?               Session,
                                                    [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            SessionId            = default;
            Session              = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId     = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Session_Id> ("session_id",   Session_Id.      TryParse, out var sessionId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid session identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            SessionId   = sessionId;


            if (!SCSP_HTTPAPI.TryGetRemoteSession(Party_Idv3.From(countryCode, partyId), sessionId, out Session))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown session identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseOptionalSession                (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out SessionId,  out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalSession(this OCPIRequest                                  Request,
                                                   SCSP_HTTPAPI                                      SCSP_HTTPAPI,
                                                   [NotNullWhen  (true)]  out CountryCode?           CountryCode,
                                                   [NotNullWhen  (true)]  out Party_Id?              PartyId,
                                                   [NotNullWhen  (true)]  out Session_Id?            SessionId,
                                                   [MaybeNullWhen(true)]  out Session?               Session,
                                                   [NotNullWhen  (false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            SessionId            = default;
            Session              = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId     = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Session_Id> ("session_id",   Session_Id.      TryParse, out var sessionId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid session identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            SessionId     = sessionId;


            SCSP_HTTPAPI.TryGetRemoteSession(
                Party_Idv3.From(
                    countryCode,
                    partyId
                ),
                sessionId,
                out Session
            );

            return true;

        }

        #endregion


        #region ParseMandatoryCDR                   (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out CDRId,      out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryCDR(this OCPIRequest                                Request,
                                                SCSP_HTTPAPI                                    SCSP_HTTPAPI,
                                                [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                [NotNullWhen(true)]  out CDR_Id?                CDRId,
                                                [NotNullWhen(true)]  out CDR?                   CDR,
                                                [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            CDRId                = default;
            CDR                  = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId     = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<CDR_Id>     ("cdrId",        CDR_Id.          TryParse, out var cdrId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid charge detail record identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CDRId = cdrId;


            if (!SCSP_HTTPAPI.TryGetRemoteCDR(
                Party_Idv3.From(
                    countryCode,
                    partyId
                ),
                cdrId,
                out CDR
            )) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown charge detail record identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseOptionalCDR                    (this Request, SCSP_HTTPAPI, out CountryCode, out PartyId, out CDRId,      out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="SCSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalCDR(this OCPIRequest                                  Request,
                                               SCSP_HTTPAPI                                      SCSP_HTTPAPI,
                                               [NotNullWhen  (true)]  out CountryCode?           CountryCode,
                                               [NotNullWhen  (true)]  out Party_Id?              PartyId,
                                               [NotNullWhen  (true)]  out CDR_Id?                CDRId,
                                               [MaybeNullWhen(true)]  out CDR?                   CDR,
                                               [NotNullWhen  (false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            CDRId                = default;
            CDR                  = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", OCPI.CountryCode.TryParse, out var countryCode))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid country code!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CountryCode = countryCode;


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.        TryParse, out var partyId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId     = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<CDR_Id>   ("cdrId",      CDR_Id.        TryParse, out var cdrId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid charge detail record identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CDRId = cdrId;


            SCSP_HTTPAPI.TryGetRemoteCDR(
                Party_Idv3.From(
                    countryCode,
                    partyId
                ),
                cdrId,
                out CDR
            );

            return true;

        }

        #endregion


    }


    /// <summary>
    /// The HTTP API for e-mobility service providers.
    /// CPOs will connect to this API.
    /// </summary>
    public class SCSP_HTTPAPI : AHTTPExtAPIXExtension2<CommonAPI, HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI SCSP HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI SCSP HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("/emsp");

        /// <summary>
        /// The default EMSP API logfile name.
        /// </summary>
        public  const              String    DefaultLogfileName       = "OCPI_SCSPAPI.log";

        protected Newtonsoft.Json.Formatting JSONFormat = Newtonsoft.Json.Formatting.Indented;

        #endregion

        #region Properties

        /// <summary>
        /// The CommonAPI.
        /// </summary>
        public CommonAPI       CommonAPI             { get; }

        /// <summary>
        /// The default country code to use.
        /// </summary>
        public CountryCode     DefaultCountryCode    { get; }

        /// <summary>
        /// The default party identification to use.
        /// </summary>
        public Party_Id        DefaultPartyId        { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?        AllowDowngrades       { get; }

                /// <summary>
        /// The timeout for upstream requests.
        /// </summary>
        public TimeSpan        RequestTimeout        { get; set; }

        /// <summary>
        /// The SCSP API logger.
        /// </summary>
        public SCSPAPILogger?  SCSPAPILogger         { get; }

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<PublishToken>?                CustomPublishTokenSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<Location>>?       CustomLocationEnergyMeterSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer           { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                       CustomPriceSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer            { get; set; }


        public CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CDRToken>?                    CustomCDRTokenSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                  { get; set; }


        public CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CDRLocation>?                 CustomCDRLocationSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                   { get; set; }


        public CustomJObjectSerializerDelegate<Token>?                       CustomTokenSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyContract>?              CustomEnergyContractSerializer                { get; set; }

        #endregion

        #region Events

        #region Locations

        #region (protected internal) GetLocationsRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetLocationsRequest = new();

        /// <summary>
        /// An event sent whenever a GET locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationsRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnGetLocationsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetLocationsResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET locations response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetLocationsResponse = new();

        /// <summary>
        /// An event sent whenever a GET locations response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationsResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnGetLocationsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteLocationsRequest (Request)

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationsRequest = new();

        /// <summary>
        /// An event sent whenever a delete locations request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationsRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

            => OnDeleteLocationsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteLocationsResponse(Response)

        /// <summary>
        /// An event sent whenever a delete locations response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteLocationsResponse = new();

        /// <summary>
        /// An event sent whenever a delete locations response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationsResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

            => OnDeleteLocationsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion



        #region (protected internal) GetLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetLocationRequest = new();

        /// <summary>
        /// An event sent whenever a GET location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetLocationRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnGetLocationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetLocationResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetLocationResponse = new();

        /// <summary>
        /// An event sent whenever a GET location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetLocationResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnGetLocationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PutLocationRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutLocationRequest = new();

        /// <summary>
        /// An event sent whenever a PUT location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutLocationRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnPutLocationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutLocationResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutLocationResponse = new();

        /// <summary>
        /// An event sent whenever a PUT location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutLocationResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnPutLocationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchLocationRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchLocationRequest = new();

        /// <summary>
        /// An event sent whenever a PATCH location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchLocationRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     CancellationToken  CancellationToken)

            => OnPatchLocationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchLocationResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchLocationResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchLocationResponse(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      OCPIResponse       Response,
                                                      CancellationToken  CancellationToken)

            => OnPatchLocationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteLocationRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE location request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteLocationRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE location request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteLocationRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnDeleteLocationRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteLocationResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE location response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteLocationResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE location response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteLocationResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnDeleteLocationResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region EVSEs

        #region (protected internal) GetEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetEVSERequest = new();

        /// <summary>
        /// An event sent whenever a GET EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetEVSERequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnGetEVSERequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetEVSEResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetEVSEResponse = new();

        /// <summary>
        /// An event sent whenever a GET EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetEVSEResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                OCPIResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnGetEVSEResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PutEVSERequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutEVSERequest = new();

        /// <summary>
        /// An event sent whenever a PUT EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutEVSERequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnPutEVSERequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutEVSEResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutEVSEResponse = new();

        /// <summary>
        /// An event sent whenever a PUT EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutEVSEResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                OCPIResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnPutEVSEResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchEVSERequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchEVSERequest = new();

        /// <summary>
        /// An event sent whenever a PATCH EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchEVSERequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnPatchEVSERequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchEVSEResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchEVSEResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchEVSEResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnPatchEVSEResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteEVSERequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE EVSE request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteEVSERequest = new();

        /// <summary>
        /// An event sent whenever a DELETE EVSE request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteEVSERequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnDeleteEVSERequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteEVSEResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE EVSE response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteEVSEResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE EVSE response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteEVSEResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnDeleteEVSEResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion



        #region (protected internal) OnPostEVSEStatusRequest    (Request)

        /// <summary>
        /// An event sent whenever a POST EVSE status request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostEVSEStatusRequest = new();

        /// <summary>
        /// An event sent whenever a POST EVSE status request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostEVSEStatusRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnPostEVSEStatusRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PostEVSEStatusResponse   (Response)

        /// <summary>
        /// An event sent whenever a POST EVSE status response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostEVSEStatusResponse = new();

        /// <summary>
        /// An event sent whenever a POST EVSE status response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostEVSEStatusResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnPostEVSEStatusResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Connectors

        #region (protected internal) GetConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a GET connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetConnectorRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnGetConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetConnectorResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a GET connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetConnectorResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnGetConnectorResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PutConnectorRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a PUT connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutConnectorRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnPutConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutConnectorResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a PUT connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutConnectorResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnPutConnectorResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchConnectorRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a PATCH connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchConnectorRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnPatchConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchConnectorResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchConnectorResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnPatchConnectorResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteConnectorRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE connector request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteConnectorRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE connector request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteConnectorRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

            => OnDeleteConnectorRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteConnectorResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE connector response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteConnectorResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE connector response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteConnectorResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

            => OnDeleteConnectorResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Tariffs

        #region (protected internal) GetTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffsRequest = new();

        /// <summary>
        /// An event sent whenever a GET tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffsRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnGetTariffsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetTariffsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET tariffs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTariffsResponse = new();

        /// <summary>
        /// An event sent whenever a GET tariffs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffsResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnGetTariffsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteTariffsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE tariffs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffsRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE tariffs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffsRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     CancellationToken  CancellationToken)

            => OnDeleteTariffsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteTariffsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE tariffs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTariffsResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE tariffs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffsResponse(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      OCPIResponse       Response,
                                                      CancellationToken  CancellationToken)

            => OnDeleteTariffsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion



        #region (protected internal) GetTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTariffRequest = new();

        /// <summary>
        /// An event sent whenever a GET tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTariffRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnGetTariffRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTariffResponse = new();

        /// <summary>
        /// An event sent whenever a GET tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTariffResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnGetTariffResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PutTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTariffRequest = new();

        /// <summary>
        /// An event sent whenever a PUT tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTariffRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnPutTariffRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTariffResponse = new();

        /// <summary>
        /// An event sent whenever a PUT tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTariffResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnPutTariffResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchTariffRequest    (Request)

        /// <summary>
        /// An event sent whenever a PATCH tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTariffRequest = new();

        /// <summary>
        /// An event sent whenever a PATCH tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTariffRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnPatchTariffRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchTariffResponse   (Response)

        /// <summary>
        /// An event sent whenever a PATCH tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTariffResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTariffResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnPatchTariffResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteTariffRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE tariff request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteTariffRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE tariff request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteTariffRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnDeleteTariffRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteTariffResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE tariff response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteTariffResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE tariff response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteTariffResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnDeleteTariffResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Sessions

        #region (protected internal) GetSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionsRequest = new();

        /// <summary>
        /// An event sent whenever a GET sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionsRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnGetSessionsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetSessionsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET sessions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetSessionsResponse = new();

        /// <summary>
        /// An event sent whenever a GET sessions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionsResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnGetSessionsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteSessionsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE sessions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionsRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE sessions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionsRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnDeleteSessionsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteSessionsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE sessions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteSessionsResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE sessions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionsResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnDeleteSessionsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion



        #region (protected internal) GetSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetSessionRequest = new();

        /// <summary>
        /// An event sent whenever a GET session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetSessionRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnGetSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetSessionResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetSessionResponse = new();

        /// <summary>
        /// An event sent whenever a GET session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetSessionResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnGetSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PutSessionRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutSessionRequest = new();

        /// <summary>
        /// An event sent whenever a PUT session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutSessionRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnPutSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutSessionResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutSessionResponse = new();

        /// <summary>
        /// An event sent whenever a PUT session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutSessionResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnPutSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PatchSessionRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchSessionRequest = new();

        /// <summary>
        /// An event sent whenever a PATCH session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchSessionRequest(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    CancellationToken  CancellationToken)

            => OnPatchSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PatchSessionResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchSessionResponse = new();

        /// <summary>
        /// An event sent whenever a PATCH session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchSessionResponse(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     OCPIResponse       Response,
                                                     CancellationToken  CancellationToken)

            => OnPatchSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteSessionRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE session request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteSessionRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE session request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteSessionRequest(DateTimeOffset     Timestamp,
                                                     HTTPAPIX           API,
                                                     OCPIRequest        Request,
                                                     CancellationToken  CancellationToken)

            => OnDeleteSessionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteSessionResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE session response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteSessionResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE session response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteSessionResponse(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      OCPIResponse       Response,
                                                      CancellationToken  CancellationToken)

            => OnDeleteSessionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region CDRs

        #region (protected internal) GetCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRsRequest = new();

        /// <summary>
        /// An event sent whenever a GET CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRsRequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnGetCDRsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetCDRsResponse(Response)

        /// <summary>
        /// An event sent whenever a GET CDRs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCDRsResponse = new();

        /// <summary>
        /// An event sent whenever a GET CDRs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRsResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                OCPIResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnGetCDRsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteCDRsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE CDRs request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRsRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE CDRs request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRsRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnDeleteCDRsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteCDRsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE CDRs response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCDRsResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE CDRs response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRsResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnDeleteCDRsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion



        #region (protected internal) GetCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a GET CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCDRRequest = new();

        /// <summary>
        /// An event sent whenever a GET CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCDRRequest(DateTimeOffset     Timestamp,
                                              HTTPAPIX           API,
                                              OCPIRequest        Request,
                                              CancellationToken  CancellationToken)

            => OnGetCDRRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a GET CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCDRResponse = new();

        /// <summary>
        /// An event sent whenever a GET CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCDRResponse(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               OCPIResponse       Response,
                                               CancellationToken  CancellationToken)

            => OnGetCDRResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PostCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a POST CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCDRRequest = new();

        /// <summary>
        /// An event sent whenever a POST CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCDRRequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               OCPIRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnPostCDRRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PostCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a POST CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCDRResponse = new();

        /// <summary>
        /// An event sent whenever a POST CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCDRResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                OCPIRequest        Request,
                                                OCPIResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnPostCDRResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteCDRRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE CDR request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCDRRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE CDR request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCDRRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnDeleteCDRRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteCDRResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE CDR response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCDRResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE CDR response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCDRResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnDeleteCDRResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Tokens

        #region (protected internal) PostTokenRequest (Request)

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTokenRequest = new();

        /// <summary>
        /// An event sent whenever a POST token request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTokenRequest(DateTimeOffset     Timestamp,
                                                 HTTPAPIX           API,
                                                 OCPIRequest        Request,
                                                 CancellationToken  CancellationToken)

            => OnPostTokenRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PostTokenResponse(Response)

        /// <summary>
        /// An event sent whenever a POST token response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostTokenResponse = new();

        /// <summary>
        /// An event sent whenever a POST token response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTokenResponse(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  OCPIResponse       Response,
                                                  CancellationToken  CancellationToken)

            => OnPostTokenResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion


        // Command callbacks

        #region (protected internal) ReserveNowCallbackRequest        (Request)

        /// <summary>
        /// An event sent whenever a reserve now callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnReserveNowCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a reserve now callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task ReserveNowCallbackRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

            => OnReserveNowCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) ReserveNowCallbackResponse       (Response)

        /// <summary>
        /// An event sent whenever a reserve now callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnReserveNowCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a reserve now callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task ReserveNowCallbackResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

            => OnReserveNowCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) CancelReservationCallbackRequest (Request)

        /// <summary>
        /// An event sent whenever a cancel reservation callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnCancelReservationCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a cancel reservation callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task CancelReservationCallbackRequest(DateTimeOffset     Timestamp,
                                                                 HTTPAPIX           API,
                                                                 OCPIRequest        Request,
                                                                 CancellationToken  CancellationToken)

            => OnCancelReservationCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) CancelReservationCallbackResponse(Response)

        /// <summary>
        /// An event sent whenever a cancel reservation callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnCancelReservationCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a cancel reservation callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task CancelReservationCallbackResponse(DateTimeOffset     Timestamp,
                                                                  HTTPAPIX           API,
                                                                  OCPIRequest        Request,
                                                                  OCPIResponse       Response,
                                                                  CancellationToken  CancellationToken)

            => OnCancelReservationCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) StartSessionCallbackRequest      (Request)

        /// <summary>
        /// An event sent whenever a start session callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnStartSessionCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a start session callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StartSessionCallbackRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

            => OnStartSessionCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) StartSessionCallbackResponse     (Response)

        /// <summary>
        /// An event sent whenever a start session callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStartSessionCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a start session callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StartSessionCallbackResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

            => OnStartSessionCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) StopSessionCallbackRequest       (Request)

        /// <summary>
        /// An event sent whenever a stop session callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnStopSessionCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a stop session callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task StopSessionCallbackRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

            => OnStopSessionCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) StopSessionCallbackResponse      (Response)

        /// <summary>
        /// An event sent whenever a stop session callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnStopSessionCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a stop session callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task StopSessionCallbackResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

            => OnStopSessionCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) UnlockConnectorCallbackRequest   (Request)

        /// <summary>
        /// An event sent whenever a unlock connector callback was received.
        /// </summary>
        public OCPIRequestLogEvent OnUnlockConnectorCallbackRequest = new();

        /// <summary>
        /// An event sent whenever a unlock connector callback was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback request.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task UnlockConnectorCallbackRequest(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               CancellationToken  CancellationToken)

            => OnUnlockConnectorCallbackRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) UnlockConnectorCallbackResponse  (Response)

        /// <summary>
        /// An event sent whenever a unlock connector callback response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnUnlockConnectorCallbackResponse = new();

        /// <summary>
        /// An event sent whenever a unlock connector callback response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the callback response.</param>
        /// <param name="API">The EMSP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task UnlockConnectorCallbackResponse(DateTimeOffset     Timestamp,
                                                                HTTPAPIX           API,
                                                                OCPIRequest        Request,
                                                                OCPIResponse       Response,
                                                                CancellationToken  CancellationToken)

            => OnUnlockConnectorCallbackResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

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
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public SCSP_HTTPAPI(CommonAPI                    CommonAPI,
                       CountryCode                  DefaultCountryCode,
                       Party_Id                     DefaultPartyId,
                       I18NString?                  Description               = null,
                       Boolean?                     AllowDowngrades           = null,

                       HTTPPath?                    BasePath                  = null,
                       HTTPPath?                    URLPathPrefix             = null,

                       String?                      ExternalDNSName           = null,
                       String?                      HTTPServerName            = DefaultHTTPServerName,
                       String?                      HTTPServiceName           = DefaultHTTPServiceName,
                       String?                      APIVersionHash            = null,
                       JObject?                     APIVersionHashes          = null,

                       Boolean?                     IsDevelopment             = false,
                       IEnumerable<String>?         DevelopmentServers        = null,
                       Boolean?                     DisableLogging            = false,
                       String?                      LoggingContext            = null,
                       String?                      LoggingPath               = null,
                       String?                      LogfileName               = DefaultLogfileName,
                       OCPILogfileCreatorDelegate?  LogfileCreator            = null)

            : base(CommonAPI,
                   URLPathPrefix   ?? DefaultURLPathPrefix,
                   BasePath,

                   Description     ?? I18NString.Create($"OCPI{Version.String} SCSP API"),

                   ExternalDNSName,
                   HTTPServerName  ?? DefaultHTTPServerName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   APIVersionHash,
                   APIVersionHashes,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName     ?? DefaultLogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null)

        {

            this.CommonAPI           = CommonAPI ?? throw new ArgumentNullException(nameof(CommonAPI), "The given CommonAPI must not be null!");
            this.DefaultCountryCode  = DefaultCountryCode;
            this.DefaultPartyId      = DefaultPartyId;
            this.AllowDowngrades     = AllowDowngrades;
            this.RequestTimeout      = TimeSpan.FromSeconds(60);

            this.SCSPAPILogger       = this.DisableLogging == false
                                           ? new SCSPAPILogger(
                                                 this,
                                                 LoggingContext,
                                                 LoggingPath,
                                                 LogfileCreator
                                             )
                                           : null;

            RegisterURLTemplates();

        }

        #endregion


        #region RemoteCPOs

        #region Data

        private readonly ConcurrentDictionary<Party_Idv3, PartyData> remoteCPOs = [];


        public delegate Task OnRemoteCPOAddedDelegate  (PartyData PartyData);
        public delegate Task OnRemoteCPOChangedDelegate(PartyData PartyData);
        public delegate Task OnRemoteCPORemovedDelegate(PartyData PartyData);

        public event OnRemoteCPOAddedDelegate?    OnRemoteCPOAdded;
        public event OnRemoteCPOChangedDelegate?  OnRemoteCPOChanged;
        public event OnRemoteCPORemovedDelegate?  OnRemoteCPORemoved;

        #endregion


        #region AddRemoteCPO            (Id, Role, BusinessDetails, AllowDowngrades = null, ...)

        public async Task<AddResult<PartyData>>

            AddRemoteCPO(PartyData          RemoteCPOPartyData,
                         Boolean?           AllowDowngrades     = null,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryAdd(RemoteCPOPartyData.Id, RemoteCPOPartyData))
            {

                await CommonAPI.LogAsset(
                          "addRemoteCPO",
                          JSONObject.Create(
                              new JProperty("id",  RemoteCPOPartyData.Id.ToString())
                          ),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    //await LogEvent(
                    //          OnRemoteCPOAdded,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              RemoteCPO
                    //          )
                    //      );

                }

                return AddResult<PartyData>.Success(
                           EventTrackingId,
                           RemoteCPOPartyData
                       );

            }

            return AddResult<PartyData>.Failed(
                       EventTrackingId,
                       RemoteCPOPartyData,
                       "The given party identification already exists!"
                   );

        }

        #endregion

        #region AddRemoteCPO            (Id, Role, BusinessDetails, AllowDowngrades = null, ...)

        public async Task<AddResult<PartyData>>

            AddRemoteCPO(Party_Idv3         Id,
                         Role               Role,
                         BusinessDetails    BusinessDetails,
                         Boolean?           AllowDowngrades     = null,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var newRemoteCPO = new PartyData(
                               Id,
                               Role,
                               BusinessDetails,
                               AllowDowngrades
                           );

            if (remoteCPOs.TryAdd(Id, newRemoteCPO))
            {

                await CommonAPI.LogAsset(
                          "addRemoteCPO",
                          JSONObject.Create(
                              new JProperty("id",  Id.ToString())
                          ),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    //await LogEvent(
                    //          OnRemoteCPOAdded,
                    //          loggingDelegate => loggingDelegate.Invoke(
                    //              RemoteCPO
                    //          )
                    //      );

                }

                return AddResult<PartyData>.Success(
                           EventTrackingId,
                           newRemoteCPO
                       );

            }

            return AddResult<PartyData>.Failed(
                       EventTrackingId,
                       newRemoteCPO,
                       "The given party identification already exists!"
                   );

        }

        #endregion


        public Boolean HasRemoteCPO(Party_Idv3 RemoteCPOId)
            => remoteCPOs.ContainsKey(RemoteCPOId);

        public IEnumerable<PartyData> Parties
            => remoteCPOs.Values;

        #endregion

        #region RemoteLocations

        #region RemoteLocations

        #region Events

        // Note: Charging locations/EVSEs are expected to be always in memory!

        public delegate Task OnRemoteLocationAddedDelegate  (Location RemoteLocation);
        public delegate Task OnRemoteLocationChangedDelegate(Location RemoteLocation);
        public delegate Task OnRemoteLocationRemovedDelegate(Location RemoteLocation);

        public event OnRemoteLocationAddedDelegate?    OnRemoteLocationAdded;
        public event OnRemoteLocationChangedDelegate?  OnRemoteLocationChanged;
        public event OnRemoteLocationRemovedDelegate?  OnRemoteLocationRemoved;

        #endregion


        #region AddRemoteLocation            (RemoteLocation, ...)

        /// <summary>
        /// Add the given remote charging location.
        /// </summary>
        /// <param name="RemoteLocation">The remote charging location to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Location>>

            AddRemoteLocation(Location           RemoteLocation,
                              Boolean            SkipNotifications   = false,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null,
                              CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteLocation.CountryCode,
                                    RemoteLocation.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (party.Locations.TryAdd(RemoteLocation.Id, RemoteLocation))
                {

                    RemoteLocation.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addRemoteLocation,
                              RemoteLocation.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  null,//EMSPId,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomEVSESerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomImageSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteLocationAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteLocation
                                  )
                              );

                        foreach (var evse in RemoteLocation.EVSEs)
                            await LogEvent(
                                      OnRemoteEVSEAdded,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          evse
                                      )
                                  );

                    }

                    return AddResult<Location>.Success(
                               EventTrackingId,
                               RemoteLocation
                           );

                }

                return AddResult<Location>.Failed(
                           EventTrackingId,
                           RemoteLocation,
                           "The given remote location already exists!"
                       );

            }

            return AddResult<Location>.Failed(
                       EventTrackingId,
                       RemoteLocation,
                       $"The party identification '{partyId}' of the remote location is unknown!"
                   );

        }

        #endregion

        #region AddRemoteLocationIfNotExists (RemoteLocation, ...)

        /// <summary>
        /// Add the given remote charging location if it does not already exist.
        /// </summary>
        /// <param name="RemoteLocation">The remote charging location to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Location>>

            AddRemoteLocationIfNotExists(Location           RemoteLocation,
                                         Boolean            SkipNotifications   = false,
                                         EventTracking_Id?  EventTrackingId     = null,
                                         User_Id?           CurrentUserId       = null,
                                         CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteLocation.CountryCode,
                                    RemoteLocation.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (party.Locations.TryAdd(RemoteLocation.Id, RemoteLocation))
                {

                    RemoteLocation.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addRemoteLocation,
                              RemoteLocation.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  null,//EMSPId,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomEVSESerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomImageSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteLocationAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteLocation
                                  )
                              );

                        foreach (var evse in RemoteLocation.EVSEs)
                            await LogEvent(
                                      OnRemoteEVSEAdded,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          evse
                                      )
                                  );

                    }

                    return AddResult<Location>.Success(
                               EventTrackingId,
                               RemoteLocation
                           );

                }

                return AddResult<Location>.NoOperation(
                           EventTrackingId,
                           RemoteLocation,
                           "The given remote location already exists."
                       );

            }

            return AddResult<Location>.Failed(
                       EventTrackingId,
                       RemoteLocation,
                       $"The party identification '{partyId}' of the remote location is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateRemoteLocation    (RemoteLocation,                           AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given remote charging location.
        /// </summary>
        /// <param name="RemoteLocation">The remote charging location to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<Location>>

            AddOrUpdateRemoteLocation(Location           RemoteLocation,
                                      Boolean?           AllowDowngrades     = false,
                                      Boolean            SkipNotifications   = false,
                                      EventTracking_Id?  EventTrackingId     = null,
                                      User_Id?           CurrentUserId       = null,
                                      CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteLocation.CountryCode,
                                    RemoteLocation.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                #region Update an existing location

                if (party.Locations.TryGetValue(RemoteLocation.Id, out var existingRemoteLocation))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        RemoteLocation.LastUpdated <= existingRemoteLocation.LastUpdated)
                    {
                        return AddOrUpdateResult<Location>.Failed(
                                   EventTrackingId,
                                   RemoteLocation,
                                   "The 'lastUpdated' timestamp of the new remote charging location must be newer then the timestamp of the existing location!"
                               );
                    }

                    //if (RemoteLocation.LastUpdated.ToISO8601() == existingRemoteLocation.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<RemoteLocation>.NoOperation(RemoteLocation,
                    //                                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");

                    if (party.Locations.TryUpdate(RemoteLocation.Id,
                                                  RemoteLocation,
                                                  existingRemoteLocation))
                    {

                        RemoteLocation.CommonAPI = CommonAPI;

                        await CommonAPI.LogAsset(
                                  CommonHTTPAPI.addOrUpdateRemoteLocation,
                                  RemoteLocation.ToJSON(
                                      //true,
                                      //true,
                                      //true,
                                      null,//EMSPId,
                                      CustomLocationSerializer,
                                      CustomPublishTokenSerializer,
                                      CustomAdditionalGeoLocationSerializer,
                                      CustomEVSESerializer,
                                      CustomStatusScheduleSerializer,
                                      CustomConnectorSerializer,
                                      CustomLocationEnergyMeterSerializer,
                                      CustomEVSEEnergyMeterSerializer,
                                      CustomTransparencySoftwareStatusSerializer,
                                      CustomTransparencySoftwareSerializer,
                                      CustomDisplayTextSerializer,
                                      CustomBusinessDetailsSerializer,
                                      CustomHoursSerializer,
                                      CustomImageSerializer,
                                      CustomEnergyMixSerializer,
                                      CustomEnergySourceSerializer,
                                      CustomEnvironmentalImpactSerializer
                                  ),
                                  EventTrackingId,
                                  CurrentUserId,
                                  CancellationToken
                              );

                        if (!SkipNotifications)
                        {

                            await LogEvent(
                                      OnRemoteLocationChanged,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          RemoteLocation
                                      )
                                  );

                            var oldEVSEUIds = new HashSet<EVSE_UId>(existingRemoteLocation.EVSEs.Select(evse => evse.UId));
                            var newEVSEUIds = new HashSet<EVSE_UId>(RemoteLocation.        EVSEs.Select(evse => evse.UId));

                            foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                            {

                                if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId) && existingRemoteLocation.GetEVSE(evseUId)! != RemoteLocation.GetEVSE(evseUId)!)
                                {

                                    if (existingRemoteLocation.TryGetEVSE(evseUId, out var evse))
                                        await LogEvent(
                                                  OnRemoteEVSEChanged,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      evse
                                                  )
                                              );

                                }
                                else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                                {

                                    if (existingRemoteLocation.TryGetEVSE(evseUId, out var evse))
                                        await LogEvent(
                                                  OnRemoteEVSEAdded,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      evse
                                                  )
                                              );

                                }
                                else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                                {

                                    if (existingRemoteLocation.TryGetEVSE(evseUId, out var evse))
                                        await LogEvent(
                                                  OnRemoteEVSERemoved,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      evse
                                                  )
                                              );

                                }

                            }

                        }

                        return AddOrUpdateResult<Location>.Updated(
                                   EventTrackingId,
                                   RemoteLocation
                               );

                    }

                    return AddOrUpdateResult<Location>.Failed(
                               EventTrackingId,
                               RemoteLocation,
                               "Updating the given remote charging location failed!"
                           );

                }

                #endregion

                #region Add a new location

                if (party.Locations.TryAdd(RemoteLocation.Id, RemoteLocation))
                {

                    RemoteLocation.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addOrUpdateRemoteLocation,
                              RemoteLocation.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  null,//EMSPId,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomEVSESerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomImageSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteLocationAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteLocation
                                  )
                              );

                        foreach (var evse in RemoteLocation.EVSEs)
                            await LogEvent(
                                      OnRemoteEVSEAdded,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          evse
                                      )
                                  );

                    }

                    return AddOrUpdateResult<Location>.Created(
                               EventTrackingId,
                               RemoteLocation
                           );

                }

                #endregion

                return AddOrUpdateResult<Location>.Failed(
                           EventTrackingId,
                           RemoteLocation,
                           "Adding the given remote charging location failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<Location>.Failed(
                       EventTrackingId,
                       RemoteLocation,
                       $"The party identification '{partyId}' of the remote charging location is unknown!"
                   );

        }

        #endregion

        #region UpdateRemoteLocation         (RemoteLocation,                           AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given remote charging location.
        /// </summary>
        /// <param name="RemoteLocation">The remote charging location to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<Location>>

            UpdateRemoteLocation(Location           RemoteLocation,
                                 Boolean?           AllowDowngrades     = false,
                                 Boolean            SkipNotifications   = false,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 User_Id?           CurrentUserId       = null,
                                 CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteLocation.CountryCode,
                                    RemoteLocation.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (!party.Locations.TryGetValue(RemoteLocation.Id, out var existingRemoteLocation))
                    return UpdateResult<Location>.Failed(
                               EventTrackingId,
                               RemoteLocation,
                               $"The given remote charging location identification '{RemoteLocation.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    RemoteLocation.LastUpdated <= existingRemoteLocation.LastUpdated)
                {

                    return UpdateResult<Location>.Failed(
                               EventTrackingId,
                               RemoteLocation,
                               "The 'lastUpdated' timestamp of the new remote charging location must be newer then the timestamp of the existing location!"
                           );

                }

                #endregion


                if (party.Locations.TryUpdate(RemoteLocation.Id,
                                              RemoteLocation,
                                              existingRemoteLocation))
                {

                    RemoteLocation.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.updateRemoteLocation,
                              RemoteLocation.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  null,//EMSPId,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomEVSESerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomImageSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteLocationChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteLocation
                                  )
                              );

                        var oldEVSEUIds = new HashSet<EVSE_UId>(existingRemoteLocation.EVSEs.Select(evse => evse.UId));
                        var newEVSEUIds = new HashSet<EVSE_UId>(RemoteLocation.        EVSEs.Select(evse => evse.UId));

                        foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                        {

                            if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                            {

                                if (existingRemoteLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                                    RemoteLocation.        TryGetEVSE(evseUId, out var newEVSE) &&
                                    oldEVSE is not null &&
                                    newEVSE is not null)
                                {

                                    if (oldEVSE != newEVSE)
                                        await LogEvent(
                                                  OnRemoteEVSEChanged,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      newEVSE
                                                  )
                                              );

                                    if (oldEVSE.Status != newEVSE.Status)
                                        await LogEvent(
                                                  OnRemoteEVSEStatusChanged,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      Timestamp.Now,
                                                      newEVSE,
                                                      newEVSE.Status,
                                                      oldEVSE.Status
                                                  )
                                              );

                                }

                            }
                            else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                            {

                                if (RemoteLocation.TryGetEVSE(evseUId, out var evse))
                                {

                                    await LogEvent(
                                              OnRemoteEVSEAdded,
                                              loggingDelegate => loggingDelegate.Invoke(
                                                  evse
                                              )
                                          );

                                    await LogEvent(
                                              OnRemoteEVSEStatusChanged,
                                              loggingDelegate => loggingDelegate.Invoke(
                                                  Timestamp.Now,
                                                  evse,
                                                  evse.Status
                                              )
                                          );

                                }

                            }
                            else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                            {

                                if (existingRemoteLocation.TryGetEVSE(evseUId, out var evse))
                                    await LogEvent(
                                              OnRemoteEVSERemoved,
                                              loggingDelegate => loggingDelegate.Invoke(
                                                  evse
                                              )
                                          );

                                if (existingRemoteLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                                    RemoteLocation.        TryGetEVSE(evseUId, out var newEVSE))
                                {
                                    await LogEvent(
                                              OnRemoteEVSEStatusChanged,
                                              loggingDelegate => loggingDelegate.Invoke(
                                                  Timestamp.Now,
                                                  oldEVSE,
                                                  newEVSE.Status,
                                                  oldEVSE.Status
                                              )
                                          );
                                }

                            }

                        }

                    }

                    return UpdateResult<Location>.Success(
                               EventTrackingId,
                               RemoteLocation
                           );

                }

                return UpdateResult<Location>.Failed(
                           EventTrackingId,
                           RemoteLocation,
                           "locations.TryUpdate(RemoteLocation.Id, RemoteLocation, RemoteLocation) failed!"
                       );

            }

            return UpdateResult<Location>.Failed(
                       EventTrackingId,
                       RemoteLocation,
                       $"The party identification '{partyId}' of the remote charging location is unknown!"
                   );

        }

        #endregion

        #region TryPatchRemoteLocation       (PartyId, RemoteLocationId, RemoteLocationPatch, AllowDowngrades = false, ...)

        /// <summary>
        /// Try to patch the given charging location with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the location.</param>
        /// <param name="RemoteLocationId">The identification of the charging location to patch.</param>
        /// <param name="RemoteLocationPatch">The JSON patch document to apply to the charging tariff.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<Location>>

            TryPatchRemoteLocation(Party_Idv3         PartyId,
                                   Location_Id        RemoteLocationId,
                                   JObject            RemoteLocationPatch,
                                   Boolean?           AllowDowngrades     = false,
                                   Boolean            SkipNotifications   = false,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   User_Id?           CurrentUserId       = null,
                                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                if (party.Locations.TryGetValue(RemoteLocationId, out var existingRemoteLocation))
                {

                    var patchResult = existingRemoteLocation.TryPatch(
                                          RemoteLocationPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedRemoteLocation))
                    {

                        var updateRemoteLocationResult = await UpdateRemoteLocation(
                                                             patchedRemoteLocation,
                                                             AllowDowngrades,
                                                             SkipNotifications,
                                                             EventTrackingId,
                                                             CurrentUserId,
                                                             CancellationToken
                                                         );

                        if (updateRemoteLocationResult.IsFailed)
                            return PatchResult<Location>.Failed(
                                       EventTrackingId,
                                       existingRemoteLocation,
                                       "Could not patch the remote charging location: " + updateRemoteLocationResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<Location>.Failed(
                           EventTrackingId,
                           $"The given remote charging location '{RemoteLocationId}' is unknown!"
                       );

            }

            return PatchResult<Location>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charging location is unknown!"
                   );

        }

        #endregion


        #region RemoveRemoteLocation         (RemoteLocation, ...)

        /// <summary>
        /// Remove the given remote charging location.
        /// </summary>
        /// <param name="RemoteLocation">A remote charging location.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<Location>>

            RemoveRemoteLocation(Location           RemoteLocation,
                                 Boolean            SkipNotifications   = false,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 User_Id?           CurrentUserId       = null,
                                 CancellationToken  CancellationToken   = default)

                => RemoveRemoteLocation(
                       Party_Idv3.From(
                           RemoteLocation.CountryCode,
                           RemoteLocation.PartyId
                       ),
                       RemoteLocation.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveRemoteLocation         (PartyId, RemoteLocationId, ...)

        /// <summary>
        /// Remove the given remote charging location.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the remote charging location.</param>
        /// <param name="RemoteLocationId">An unique remote charging location identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<Location>>

            RemoveRemoteLocation(Party_Idv3         PartyId,
                                 Location_Id        RemoteLocationId,
                                 Boolean            SkipNotifications   = false,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 User_Id?           CurrentUserId       = null,
                                 CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                if (party.Locations.TryRemove(RemoteLocationId, out var location))
                {

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.removeRemoteLocation,
                              location.ToJSON(
                                  null, //EMSPId,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomEVSESerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomImageSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer,
                                  true//IncludeCreatedTimestamp
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteLocationRemoved,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      location
                                  )
                              );

                        foreach (var evse in location.EVSEs)
                            await LogEvent(
                                      OnRemoteEVSERemoved,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          evse
                                      )
                                  );

                    }

                    return RemoveResult<Location>.Success(
                               EventTrackingId,
                               location
                           );

                }

                return RemoveResult<Location>.Failed(
                           EventTrackingId,
                           "The remote charging location identification of the location is unknown!"
                       );

            }

            return RemoveResult<Location>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charging location is unknown!"
                   );

        }

        #endregion

        #region RemoveAllRemoteLocations     (...)

        /// <summary>
        /// Remove all remote charging locations.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveAllRemoteLocations(Boolean            SkipNotifications   = false,
                                     EventTracking_Id?  EventTrackingId     = null,
                                     User_Id?           CurrentUserId       = null,
                                     CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var locations = new List<Location>();

            foreach (var party in remoteCPOs.Values)
            {
                locations.AddRange(party.Locations.Values);
                party.Locations.Clear();
            }

            await CommonAPI.LogAsset(
                      CommonHTTPAPI.removeAllRemoteLocations,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var location in locations)
                    await LogEvent(
                              OnRemoteLocationRemoved,
                              loggingDelegate => loggingDelegate.Invoke(
                                  location
                              )
                          );

            }

            return RemoveResult<IEnumerable<Location>>.Success(
                       EventTrackingId,
                       locations
                   );

        }

        #endregion

        #region RemoveAllRemoteLocations     (IncludeRemoteLocations, ...)

        /// <summary>
        /// Remove all matching remote charging locations.
        /// </summary>
        /// <param name="IncludeRemoteLocations">A filter delegate to include remote charging locations for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveAllRemoteLocations(Func<Location, Boolean>  IncludeRemoteLocations,
                                     Boolean                  SkipNotifications   = false,
                                     EventTracking_Id?        EventTrackingId     = null,
                                     User_Id?                 CurrentUserId       = null,
                                     CancellationToken        CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingRemoteLocations  = new List<Location>();
            var removedRemoteLocations   = new List<Location>();
            var failedRemoteLocations    = new List<RemoveResult<Location>>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (IncludeRemoteLocations(location))
                        matchingRemoteLocations.Add(location);
                }
            }

            foreach (var location in matchingRemoteLocations)
            {

                var result = await RemoveRemoteLocation(
                                       location,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedRemoteLocations.Add(location);
                else
                    failedRemoteLocations. Add(result);

            }

            return removedRemoteLocations.Count != 0 && failedRemoteLocations.Count == 0

                       ? RemoveResult<IEnumerable<Location>>.Success(
                             EventTrackingId,
                             removedRemoteLocations
                         )

                       : removedRemoteLocations.Count == 0 && failedRemoteLocations.Count == 0

                             ? RemoveResult<IEnumerable<Location>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Location>>.Failed(
                                   EventTrackingId,
                                   failedRemoteLocations.
                                       Select(removeResult => removeResult.Data).
                                       Where (location     => location is not null).
                                       Cast<Location>(),
                                   failedRemoteLocations.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllRemoteLocations     (IncludeRemoteLocationIds, ...)

        /// <summary>
        /// Remove all matching remote charging locations.
        /// </summary>
        /// <param name="IncludeRemoteLocationIds">A filter delegate to include remote charging locations for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveAllRemoteLocations(Func<Party_Idv3, Location_Id, Boolean>  IncludeRemoteLocationIds,
                                     Boolean                                 SkipNotifications   = false,
                                     EventTracking_Id?                       EventTrackingId     = null,
                                     User_Id?                                CurrentUserId       = null,
                                     CancellationToken                       CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingRemoteLocations  = new List<Location>();
            var removedRemoteLocations   = new List<Location>();
            var failedRemoteLocations    = new List<RemoveResult<Location>>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (IncludeRemoteLocationIds(party.Id, location.Id))
                        matchingRemoteLocations.Add(location);
                }
            }

            foreach (var location in matchingRemoteLocations)
            {

                var result = await RemoveRemoteLocation(
                                       location,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedRemoteLocations.Add(location);
                else
                    failedRemoteLocations. Add(result);

            }

            return removedRemoteLocations.Count != 0 && failedRemoteLocations.Count == 0

                       ? RemoveResult<IEnumerable<Location>>.Success(
                             EventTrackingId,
                             removedRemoteLocations
                         )

                       : removedRemoteLocations.Count == 0 && failedRemoteLocations.Count == 0

                             ? RemoveResult<IEnumerable<Location>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Location>>.Failed(
                                   EventTrackingId,
                                   failedRemoteLocations.
                                       Select(removeResult => removeResult.Data).
                                       Where (location      => location is not null).
                                       Cast<Location>(),
                                   failedRemoteLocations.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllRemoteLocations     (PartyId, ...)

        /// <summary>
        /// Remove all remote charging locations owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveAllRemoteLocations(Party_Idv3         PartyId,
                                     Boolean            SkipNotifications   = false,
                                     EventTracking_Id?  EventTrackingId     = null,
                                     User_Id?           CurrentUserId       = null,
                                     CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                var matchingRemoteLocations  = party.Locations.Values;
                var removedRemoteLocations   = new List<Location>();
                var failedRemoteLocations    = new List<RemoveResult<Location>>();

                foreach (var location in matchingRemoteLocations)
                {

                    var result = await RemoveRemoteLocation(
                                           location,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedRemoteLocations.Add(location);
                    else
                        failedRemoteLocations. Add(result);

                }

                return removedRemoteLocations.Count != 0 && failedRemoteLocations.Count == 0

                           ? RemoveResult<IEnumerable<Location>>.Success(
                                 EventTrackingId,
                                 removedRemoteLocations
                             )

                           : removedRemoteLocations.Count == 0 && failedRemoteLocations.Count == 0

                                 ? RemoveResult<IEnumerable<Location>>.NoOperation(
                                       EventTrackingId,
                                       []
                                   )

                                 : RemoveResult<IEnumerable<Location>>.Failed(
                                       EventTrackingId,
                                       failedRemoteLocations.
                                           Select(removeResult => removeResult.Data).
                                           Where (location      => location is not null).
                                           Cast<Location>(),
                                       failedRemoteLocations.Select(removeResult => removeResult.ErrorResponse).
                                           AggregateWith(", ")
                                   );

            }

            return RemoveResult<IEnumerable<Location>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' is unknown!"
                   );

        }

        #endregion


        #region RemoteLocationExists         (PartyId, RemoteLocationId)

        public Boolean RemoteLocationExists(Party_Idv3   PartyId,
                                            Location_Id  RemoteLocationId)
        {

            if (remoteCPOs.TryGetValue(PartyId, out var party))
                return party.Locations.ContainsKey(RemoteLocationId);

            return false;

        }

        #endregion

        #region TryGetRemoteLocation         (PartyId, RemoteLocationId, out RemoteLocation)

        public Boolean TryGetRemoteLocation(Party_Idv3                         PartyId,
                                            Location_Id                        RemoteLocationId,
                                            [NotNullWhen(true)] out Location?  RemoteLocation)
        {

            if (remoteCPOs.     TryGetValue(PartyId,          out var party) &&
                party.Locations.TryGetValue(RemoteLocationId, out var location))
            {
                RemoteLocation = location;
                return true;
            }

            RemoteLocation = null;
            return false;

        }

        #endregion

        #region GetRemoteLocations           (IncludeRemoteLocation)

        public IEnumerable<Location> GetRemoteLocations(Func<Location, Boolean> IncludeRemoteLocation)
        {

            var locations = new List<Location>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (IncludeRemoteLocation(location))
                        locations.Add(location);
                }
            }

            return locations;

        }

        #endregion

        #region GetRemoteLocations           (PartyId = null)

        public IEnumerable<Location> GetRemoteLocations(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (remoteCPOs.TryGetValue(PartyId.Value, out var party))
                    return party.Locations.Values;
            }

            else
            {

                var locations = new List<Location>();

                foreach (var party in remoteCPOs.Values)
                    locations.AddRange(party.Locations.Values);

                return locations;

            }

            return [];

        }

        #endregion

        #endregion

        #region RemoteEVSEs

        #region Events

        public delegate Task OnRemoteEVSEAddedDelegate  (EVSE RemoteEVSE);
        public delegate Task OnRemoteEVSEChangedDelegate(EVSE RemoteEVSE);
        public delegate Task OnRemoteEVSERemovedDelegate(EVSE RemoteEVSE);

        public event OnRemoteEVSEAddedDelegate?    OnRemoteEVSEAdded;
        public event OnRemoteEVSEChangedDelegate?  OnRemoteEVSEChanged;
        public event OnRemoteEVSERemovedDelegate?  OnRemoteEVSERemoved;


        public delegate Task OnRemoteEVSEStatusChangedDelegate(DateTimeOffset  Timestamp, EVSE RemoteEVSE, StatusType NewRemoteEVSEStatus, StatusType? OldRemoteEVSEStatus = null);

        public event OnRemoteEVSEStatusChangedDelegate? OnRemoteEVSEStatusChanged;

        #endregion


        #region AddOrUpdateRemoteEVSE        (RemoteLocation, RemoteEVSE,            AllowDowngrades = false)

        public async Task<AddOrUpdateResult<EVSE>>

            AddOrUpdateRemoteEVSE(Location           RemoteLocation,
                                  EVSE               RemoteEVSE,
                                  Boolean?           AllowDowngrades     = false,

                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            RemoteLocation.TryGetEVSE(RemoteEVSE.UId, out var existingRemoteEVSE);

            if (existingRemoteEVSE is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    RemoteEVSE.LastUpdated < existingRemoteEVSE.LastUpdated)
                {

                    return AddOrUpdateResult<EVSE>.Failed(
                               EventTrackingId,
                               RemoteEVSE,
                               "The 'lastUpdated' timestamp of the new remote EVSE must be newer then the timestamp of the existing EVSE!"
                           );

                }

                if (RemoteEVSE.LastUpdated.ToISO8601() == existingRemoteEVSE.LastUpdated.ToISO8601())
                    return AddOrUpdateResult<EVSE>.NoOperation(
                               EventTrackingId,
                               RemoteEVSE,
                               "The 'lastUpdated' timestamp of the new remote EVSE must be newer then the timestamp of the existing EVSE!"
                           );

            }


            RemoteLocation.SetEVSE(RemoteEVSE);

            // Update location timestamp!
            var builder = RemoteLocation.ToBuilder();
            builder.LastUpdated = RemoteEVSE.LastUpdated;
            await AddOrUpdateRemoteLocation(
                      builder,
                      (AllowDowngrades ?? this.AllowDowngrades) == false,
                      true,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );


            if (RemoteEVSE.ParentLocation is not null)
                await LogEvent(
                          OnRemoteLocationChanged,
                          loggingDelegate => loggingDelegate.Invoke(
                              RemoteEVSE.ParentLocation
                          )
                      );


            if (existingRemoteEVSE is not null)
            {

                if (existingRemoteEVSE.Status != StatusType.REMOVED)
                {

                    await LogEvent(
                              OnRemoteEVSEChanged,
                              loggingDelegate => loggingDelegate.Invoke(
                                  RemoteEVSE
                              )
                          );

                    if (existingRemoteEVSE.Status != RemoteEVSE.Status)
                    {

                        await LogEvent(
                                  OnRemoteEVSEStatusChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Timestamp.Now,
                                      RemoteEVSE,
                                      existingRemoteEVSE.Status,
                                      RemoteEVSE.Status
                                  )
                              );

                    }

                }
                else
                {

                    if (!CommonAPI.KeepRemovedEVSEs(RemoteEVSE))
                        RemoteLocation.RemoveEVSE(RemoteEVSE);

                    await LogEvent(
                              OnRemoteEVSERemoved,
                              loggingDelegate => loggingDelegate.Invoke(
                                  RemoteEVSE
                              )
                          );

                }
            }
            else
            {

                await LogEvent(
                          OnRemoteEVSEAdded,
                          loggingDelegate => loggingDelegate.Invoke(
                              RemoteEVSE
                          )
                      );

            }

            return existingRemoteEVSE is null
                       ? AddOrUpdateResult<EVSE>.Created(EventTrackingId, RemoteEVSE)
                       : AddOrUpdateResult<EVSE>.Updated(EventTrackingId, RemoteEVSE);

        }

        #endregion

        #region TryPatchRemoteEVSE           (RemoteLocation, RemoteEVSE, RemoteEVSEPatch, AllowDowngrades = false)

        public async Task<PatchResult<EVSE>>

            TryPatchRemoteEVSE(Location           RemoteLocation,
                               EVSE               RemoteEVSE,
                               JObject            RemoteEVSEPatch,

                               Boolean?           AllowDowngrades     = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            var patchResult        = RemoteEVSE.TryPatch(
                                         RemoteEVSEPatch,
                                         AllowDowngrades ?? this.AllowDowngrades ?? false,
                                         EventTrackingId
                                     );

            var justAStatusChange  = RemoteEVSEPatch.Children().Count() == 2 && RemoteEVSEPatch.ContainsKey("status") && RemoteEVSEPatch.ContainsKey("last_updated");

            if (patchResult.IsSuccessAndDataNotNull(out var data))
            {

                if (data.Status != StatusType.REMOVED || CommonAPI.KeepRemovedEVSEs(RemoteEVSE))
                    RemoteLocation.SetEVSE   (data);
                else
                    RemoteLocation.RemoveEVSE(data);

                // Update location timestamp!
                var builder = RemoteLocation.ToBuilder();
                builder.LastUpdated = data.LastUpdated;
                await AddOrUpdateRemoteLocation(
                          builder,
                          (AllowDowngrades ?? this.AllowDowngrades) == false,
                          SkipNotifications: true,
                          EventTrackingId,
                          CurrentUserId,
                          CancellationToken
                      );


                if (RemoteEVSE.Status != StatusType.REMOVED)
                {

                    if (justAStatusChange)
                    {

                        await LogEvent(
                                  OnRemoteEVSEStatusChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      data.LastUpdated,
                                      RemoteEVSE,
                                      RemoteEVSE.Status,
                                      data.Status
                                  )
                              );

                    }
                    else
                    {

                        await LogEvent(
                                  OnRemoteEVSEChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      data
                                  )
                              );

                    }

                }
                else
                {

                    await LogEvent(
                              OnRemoteEVSERemoved,
                              loggingDelegate => loggingDelegate.Invoke(
                                  data
                              )
                          );

                }

            }

            return patchResult;

        }

        #endregion


        #region AddOrUpdateRemoteEVSEs       (RemoteLocation, RemoteEVSEs,           AllowDowngrades = false, ...)

        public async Task<AddOrUpdateResult<IEnumerable<EVSE>>>

            AddOrUpdateRemoteEVSEs(Location           RemoteLocation,
                                   IEnumerable<EVSE>  RemoteEVSEs,
                                   Boolean?           AllowDowngrades     = false,

                                   Boolean            SkipNotifications   = false,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   User_Id?           CurrentUserId       = null,
                                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            foreach (var evse in RemoteEVSEs)
            {
                if (RemoteLocation.TryGetEVSE(evse.UId, out var existingRemoteEVSE))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        evse.LastUpdated <= existingRemoteEVSE.LastUpdated)
                    {

                        return AddOrUpdateResult<IEnumerable<EVSE>>.Failed(
                                   EventTrackingId,
                                   RemoteEVSEs,
                                   "The 'lastUpdated' timestamp of the new remote EVSE must be newer then the timestamp of the existing EVSE!"
                               );

                    }

                    //if (RemoteEVSE.LastUpdated.ToISO8601() == existingRemoteEVSE.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<RemoteEVSE>.NoOperation(RemoteEVSE,
                    //                                               "The 'lastUpdated' timestamp of the new RemoteEVSE must be newer then the timestamp of the existing RemoteEVSE!");

                }
            }

            #endregion

            var newRemoteLocation = RemoteLocation.Update(locationBuilder => {

                                                  foreach (var evse in RemoteEVSEs)
                                                  {

                                                      if (evse.Status != StatusType.REMOVED || CommonAPI.KeepRemovedEVSEs(evse))
                                                          locationBuilder.SetEVSE(evse);
                                                      else
                                                          locationBuilder.RemoveEVSE(evse);

                                                      if (evse.LastUpdated > locationBuilder.LastUpdated)
                                                          locationBuilder.LastUpdated  = evse.LastUpdated;

                                                  }

                                              },
                                              out var warnings);

            if (newRemoteLocation is null)
                return AddOrUpdateResult<IEnumerable<EVSE>>.Failed(
                           EventTrackingId,
                           RemoteEVSEs,
                           warnings.First().Text.FirstText()
                       );


            var updateRemoteLocationResult = await UpdateRemoteLocation(
                                                       newRemoteLocation,
                                                       AllowDowngrades ?? this.AllowDowngrades,
                                                       SkipNotifications,
                                                       EventTrackingId,
                                                       CurrentUserId,
                                                       CancellationToken
                                                   );


            //ToDo: Check if all RemoteEVSEs have been added OR updated!
            return updateRemoteLocationResult.IsSuccess
                       ? //existingRemoteEVSE is null
                         //    ? AddOrUpdateResult<IEnumerable<RemoteEVSE>>.Created(EventTrackingId, RemoteEVSEs)
                              AddOrUpdateResult<IEnumerable<EVSE>>.Updated(EventTrackingId, RemoteEVSEs)
                       : AddOrUpdateResult<IEnumerable<EVSE>>.Failed(
                             EventTrackingId,
                             RemoteEVSEs,
                             updateRemoteLocationResult.ErrorResponse ?? "Unknown error!"
                         );

        }

        #endregion


        public Boolean TryGetRemoteEVSE(EVSE_UId                       RemoteEVSE_UId,
                                        [NotNullWhen(true)] out EVSE?  RemoteEVSE)
        {

            RemoteEVSE = null;

            foreach (var locationKVP in remoteCPOs.SelectMany(party => party.Value.Locations))
            {
                if (locationKVP.Value.TryGetEVSE(RemoteEVSE_UId, out RemoteEVSE))
                    return true;
            }

            return false;

        }

        #endregion

        #region RemoteConnectors

        #region AddOrUpdateRemoteConnector  (RemoteLocation, EVSE, RemoteConnector,     AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Connector>>

            AddOrUpdateRemoteConnector(Location           RemoteLocation,
                                       EVSE               EVSE,
                                       Connector          RemoteConnector,
                                       Boolean?           AllowDowngrades     = false,

                                       Boolean            SkipNotifications   = false,
                                       EventTracking_Id?  EventTrackingId     = null,
                                       User_Id?           CurrentUserId       = null,
                                       CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var RemoteConnectorExistedBefore = EVSE.TryGetConnector(RemoteConnector.Id, out var existingRemoteConnector);

            if (existingRemoteConnector is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    RemoteConnector.LastUpdated < existingRemoteConnector.LastUpdated)
                {

                    return AddOrUpdateResult<Connector>.Failed(
                               EventTrackingId,
                               RemoteConnector,
                               "The 'lastUpdated' timestamp of the new remote connector must be newer then the timestamp of the existing connector!"
                           );

                }

                if (RemoteConnector.LastUpdated.ToISO8601() == existingRemoteConnector.LastUpdated.ToISO8601())
                    return AddOrUpdateResult<Connector>.NoOperation(
                               EventTrackingId,
                               RemoteConnector,
                               "The 'lastUpdated' timestamp of the new remote connector must be newer then the timestamp of the existing connector!"
                           );

            }

            EVSE.UpdateConnector(RemoteConnector);

            // Update EVSE/location timestamps!
            var evseBuilder         = EVSE.ToBuilder();
            evseBuilder.LastUpdated = RemoteConnector.LastUpdated;
            await AddOrUpdateRemoteEVSE(
                      RemoteLocation,
                      evseBuilder,
                      (AllowDowngrades ?? this.AllowDowngrades) == false,

                      SkipNotifications,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );


            if (RemoteConnector.ParentEVSE?.ParentLocation is not null)
                await LogEvent(
                          OnRemoteLocationChanged,
                          loggingDelegate => loggingDelegate.Invoke(
                              RemoteConnector.ParentEVSE.ParentLocation
                          )
                      );


            return RemoteConnectorExistedBefore
                        ? AddOrUpdateResult<Connector>.Updated(EventTrackingId, RemoteConnector)
                        : AddOrUpdateResult<Connector>.Created(EventTrackingId, RemoteConnector);

        }

        #endregion

        #region TryPatchRemoteConnector     (RemoteLocation, EVSE, RemoteConnector, RemoteConnectorPatch, AllowDowngrades = false)

        public async Task<PatchResult<Connector>>

            TryPatchRemoteConnector(Location           RemoteLocation,
                                    EVSE               EVSE,
                                    Connector          RemoteConnector,
                                    JObject            RemoteConnectorPatch,
                                    Boolean?           AllowDowngrades     = false,

                                    Boolean            SkipNotifications   = false,
                                    EventTracking_Id?  EventTrackingId     = null,
                                    User_Id?           CurrentUserId       = null,
                                    CancellationToken  CancellationToken   = default)

        {

            var patchResult = RemoteConnector.TryPatch(
                                  RemoteConnectorPatch,
                                  AllowDowngrades ?? this.AllowDowngrades ?? false
                              );

            if (patchResult.IsSuccessAndDataNotNull(out var data))
            {

                EVSE.UpdateConnector(data);

                // Update EVSE/location timestamps!
                var evseBuilder = EVSE.ToBuilder();
                evseBuilder.LastUpdated = data.LastUpdated;

                await AddOrUpdateRemoteEVSE(
                          RemoteLocation,
                          evseBuilder,
                          (AllowDowngrades ?? this.AllowDowngrades) == false,

                          SkipNotifications,
                          EventTrackingId,
                          CurrentUserId,
                          CancellationToken
                      );

            }

            return patchResult;

        }

        #endregion

        #endregion

        #endregion

        #region RemoteTariffs

        #region Events

        public delegate Task OnRemoteTariffAddedDelegate  (Tariff               RemoteTariff);
        public delegate Task OnRemoteTariffChangedDelegate(Tariff               RemoteTariff);
        public delegate Task OnRemoteTariffRemovedDelegate(IEnumerable<Tariff>  RemoteTariff);

        public event OnRemoteTariffAddedDelegate?    OnRemoteTariffAdded;
        public event OnRemoteTariffChangedDelegate?  OnRemoteTariffChanged;
        public event OnRemoteTariffRemovedDelegate?  OnRemoteTariffRemoved;

        #endregion


        public delegate Task<Tariff> OnRemoteTariffSlowStorageLookupDelegate(Party_Idv3       PartyId,
                                                                       Tariff_Id        RemoteTariffId,
                                                                       DateTimeOffset?  Timestamp,
                                                                       TimeSpan?        Tolerance);

        public event OnRemoteTariffSlowStorageLookupDelegate? OnRemoteTariffSlowStorageLookup;


        #region AddRemoteTariff            (RemoteTariff, ...)

        /// <summary>
        /// Add the given remote charging tariff.
        /// </summary>
        /// <param name="RemoteTariff">The remote charging tariff to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Tariff>>

            AddRemoteTariff(Tariff             RemoteTariff,
                            Boolean            SkipNotifications   = false,
                            EventTracking_Id?  EventTrackingId     = null,
                            User_Id?           CurrentUserId       = null,
                            CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteTariff.CountryCode,
                                    RemoteTariff.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (party.Tariffs.TryAdd(RemoteTariff.Id, RemoteTariff))
                {

                    RemoteTariff.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addRemoteTariff,
                              RemoteTariff.ToJSON(
                                  true,
                                  true,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteTariffAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteTariff
                                  )
                              );

                    }

                    return AddResult<Tariff>.Success(
                               EventTrackingId,
                               RemoteTariff
                           );

                }

                return AddResult<Tariff>.Failed(
                           EventTrackingId,
                           RemoteTariff,
                           "TryAdd(RemoteTariff.Id, RemoteTariff) failed!"
                       );

            }

            return AddResult<Tariff>.Failed(
                       EventTrackingId,
                       RemoteTariff,
                       $"The party identification '{partyId}' of the remote tariff is unknown!"
                   );

        }

        #endregion

        #region AddRemoteTariffIfNotExists (RemoteTariff, ...)

        /// <summary>
        /// Add the given remote charging tariff if it does not already exist.
        /// </summary>
        /// <param name="RemoteTariff">The remote charging tariff to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Tariff>>

            AddRemoteTariffIfNotExists(Tariff             RemoteTariff,
                                       Boolean            SkipNotifications   = false,
                                       EventTracking_Id?  EventTrackingId     = null,
                                       User_Id?           CurrentUserId       = null,
                                       CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteTariff.CountryCode,
                                    RemoteTariff.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (party.Tariffs.TryAdd(RemoteTariff.Id, RemoteTariff))
                {

                    RemoteTariff.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addRemoteTariffIfNotExists,
                              RemoteTariff.ToJSON(
                                  true,
                                  true,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteTariffAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteTariff
                                  )
                              );

                    }

                    return AddResult<Tariff>.Success(
                               EventTrackingId,
                               RemoteTariff
                           );

                }

                return AddResult<Tariff>.NoOperation(
                           EventTrackingId,
                           RemoteTariff,
                           "The given remote tariff already exists."
                       );

            }

            return AddResult<Tariff>.Failed(
                       EventTrackingId,
                       RemoteTariff,
                       $"The party identification '{partyId}' of the remote tariff is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateRemoteTariff    (RemoteTariff,                         AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given remote charging tariff.
        /// </summary>
        /// <param name="RemoteTariff">The remote charging tariff to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<Tariff>>

            AddOrUpdateRemoteTariff(Tariff             RemoteTariff,
                                    Boolean?           AllowDowngrades     = false,
                                    Boolean            SkipNotifications   = false,
                                    EventTracking_Id?  EventTrackingId     = null,
                                    User_Id?           CurrentUserId       = null,
                                    CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteTariff.CountryCode,
                                    RemoteTariff.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                #region Update an existing tariff

                if (party.Tariffs.TryGetValue(RemoteTariff.Id,
                                              out var existingRemoteTariff,
                                              RemoteTariff.NotBefore ?? DateTimeOffset.MinValue))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        RemoteTariff.LastUpdated <= existingRemoteTariff.LastUpdated)
                    {

                        return AddOrUpdateResult<Tariff>.Failed(
                                   EventTrackingId,
                                   RemoteTariff,
                                   "The 'lastUpdated' timestamp of the new remote charging tariff must be newer then the timestamp of the existing tariff!"
                               );

                    }

                    if (party.Tariffs.TryUpdate(RemoteTariff.Id,
                                                RemoteTariff,
                                                existingRemoteTariff))
                    {

                        RemoteTariff.CommonAPI = CommonAPI;

                        await CommonAPI.LogAsset(
                                  CommonHTTPAPI.addOrUpdateRemoteTariff,
                                  RemoteTariff.ToJSON(
                                      true,
                                      true,
                                      CustomTariffSerializer,
                                      CustomDisplayTextSerializer,
                                      CustomPriceSerializer,
                                      CustomTariffElementSerializer,
                                      CustomPriceComponentSerializer,
                                      CustomTariffRestrictionsSerializer,
                                      CustomEnergyMixSerializer,
                                      CustomEnergySourceSerializer,
                                      CustomEnvironmentalImpactSerializer
                                  ),
                                  EventTrackingId,
                                  CurrentUserId,
                                  CancellationToken
                              );

                        if (!SkipNotifications)
                        {

                            await LogEvent(
                                      OnRemoteTariffChanged,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          RemoteTariff
                                      )
                                  );

                        }

                        return AddOrUpdateResult<Tariff>.Updated(
                                   EventTrackingId,
                                   RemoteTariff
                               );

                    }

                    return AddOrUpdateResult<Tariff>.Failed(
                               EventTrackingId,
                               RemoteTariff,
                               "Updating the given remote charging tariff failed!"
                           );

                }

                #endregion

                #region Add a new tariff

                if (party.Tariffs.TryAdd(RemoteTariff.Id, RemoteTariff))
                {

                    RemoteTariff.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addOrUpdateRemoteTariff,
                              RemoteTariff.ToJSON(
                                  true,
                                  true,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteTariffAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteTariff
                                  )
                              );

                    }

                    return AddOrUpdateResult<Tariff>.Created(
                               EventTrackingId,
                               RemoteTariff
                           );

                }

                #endregion

                return AddOrUpdateResult<Tariff>.Failed(
                           EventTrackingId,
                           RemoteTariff,
                           "Adding the given remote charging tariff failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<Tariff>.Failed(
                       EventTrackingId,
                       RemoteTariff,
                       $"The party identification '{partyId}' of the remote charging tariff is unknown!"
                   );

        }

        #endregion

        #region UpdateRemoteTariff         (RemoteTariff,                         AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given remote charging tariff.
        /// </summary>
        /// <param name="RemoteTariff">The remote charging tariff to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<Tariff>>

            UpdateRemoteTariff(Tariff             RemoteTariff,
                               Boolean?           AllowDowngrades     = false,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteTariff.CountryCode,
                                    RemoteTariff.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (!party.Tariffs.TryGetValue(RemoteTariff.Id, out var existingRemoteTariff))
                    return UpdateResult<Tariff>.Failed(
                               EventTrackingId,
                               RemoteTariff,
                               $"The given remote charging tariff identification '{RemoteTariff.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    RemoteTariff.LastUpdated <= existingRemoteTariff.LastUpdated)
                {

                    return UpdateResult<Tariff>.Failed(
                               EventTrackingId,
                               RemoteTariff,
                               "The 'lastUpdated' timestamp of the new remote charging tariff must be newer then the timestamp of the existing tariff!"
                           );

                }

                #endregion


                if (party.Tariffs.TryUpdate(RemoteTariff.Id,
                                            RemoteTariff,
                                            existingRemoteTariff))
                {

                    RemoteTariff.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.updateRemoteTariff,
                              RemoteTariff.ToJSON(
                                  true,
                                  true,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteTariffChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteTariff
                                  )
                              );

                    }

                    return UpdateResult<Tariff>.Success(
                               EventTrackingId,
                               RemoteTariff
                           );

                }

                return UpdateResult<Tariff>.Failed(
                           EventTrackingId,
                           RemoteTariff,
                           "RemoteTariffs.TryUpdate(RemoteTariff.Id, RemoteTariff, RemoteTariff) failed!"
                       );

            }

            return UpdateResult<Tariff>.Failed(
                       EventTrackingId,
                       RemoteTariff,
                       $"The party identification '{partyId}' of the remote charging tariff is unknown!"
                   );

        }

        #endregion

        #region TryPatchRemoteTariff       (PartyId, RemoteTariffId, RemoteTariffPatch, AllowDowngrades = false, ...)

        /// <summary>
        /// Try to patch the given remote charging tariff with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the charging tariff.</param>
        /// <param name="RemoteTariffId">The identification of the remote charging tariff to patch.</param>
        /// <param name="RemoteTariffPatch">The JSON patch document to apply to the charging tariff.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<Tariff>>

            TryPatchRemoteTariff(Party_Idv3         PartyId,
                                 Tariff_Id          RemoteTariffId,
                                 JObject            RemoteTariffPatch,
                                 Boolean?           AllowDowngrades     = false,
                                 Boolean            SkipNotifications   = false,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 User_Id?           CurrentUserId       = null,
                                 CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                if (party.Tariffs.TryGetValue(RemoteTariffId, out var existingRemoteTariff, Timestamp.Now))
                {

                    var patchResult = existingRemoteTariff.TryPatch(
                                          RemoteTariffPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedRemoteTariff))
                    {

                        var updateRemoteTariffResult = await UpdateRemoteTariff(
                                                           patchedRemoteTariff,
                                                           AllowDowngrades,
                                                           SkipNotifications,
                                                           EventTrackingId,
                                                           CurrentUserId,
                                                           CancellationToken
                                                       );

                        if (updateRemoteTariffResult.IsFailed)
                            return PatchResult<Tariff>.Failed(
                                       EventTrackingId,
                                       existingRemoteTariff,
                                       "Could not patch the remote charging tariff: " + updateRemoteTariffResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<Tariff>.Failed(
                           EventTrackingId,
                           $"The given remote charging tariff '{RemoteTariffId}' is unknown!"
                       );

            }

            return PatchResult<Tariff>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charging tariff is unknown!"
                   );

        }

        #endregion


        #region RemoveRemoteTariff         (RemoteTariff, ...)

        /// <summary>
        /// Remove the given remote charging tariff.
        /// </summary>
        /// <param name="RemoteTariff">A remote charging tariff.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveRemoteTariff(Tariff             RemoteTariff,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

                => RemoveRemoteTariff(
                       Party_Idv3.From(
                           RemoteTariff.CountryCode,
                           RemoteTariff.PartyId
                       ),
                       RemoteTariff.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveRemoteTariff         (PartyId, RemoteTariffId, ...)

        /// <summary>
        /// Remove the given remote charging tariff.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the remote charging tariff.</param>
        /// <param name="RemoteTariffId">An unique remote charging tariff identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveRemoteTariff(Party_Idv3         PartyId,
                               Tariff_Id          RemoteTariffId,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                if (party.Tariffs.TryRemove(RemoteTariffId, out var tariffVersions))
                {

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.removeRemoteTariff,
                              new JArray(
                                  tariffVersions.Select(
                                      tariff => tariff.ToJSON(
                                                    true,
                                                    true,
                                                    CustomTariffSerializer,
                                                    CustomDisplayTextSerializer,
                                                    CustomPriceSerializer,
                                                    CustomTariffElementSerializer,
                                                    CustomPriceComponentSerializer,
                                                    CustomTariffRestrictionsSerializer,
                                                    CustomEnergyMixSerializer,
                                                    CustomEnergySourceSerializer,
                                                    CustomEnvironmentalImpactSerializer
                                                )
                                      )
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteTariffRemoved,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      tariffVersions
                                  )
                              );

                    }

                    return RemoveResult<IEnumerable<Tariff>>.Success(
                               EventTrackingId,
                               tariffVersions
                           );

                }

                return RemoveResult<IEnumerable<Tariff>>.Failed(
                           EventTrackingId,
                           $"The remote charging tariff '{PartyId}/{RemoteTariffId}' is unknown!"
                       );

            }

            return RemoveResult<IEnumerable<Tariff>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charging tariff is unknown!"
                   );

        }

        #endregion

        #region RemoveAllRemoteTariffs     (...)

        /// <summary>
        /// Remove all remote charging tariffs.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveAllRemoteTariffs(Boolean            SkipNotifications   = false,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   User_Id?           CurrentUserId       = null,
                                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var tariffVersionList = new List<IEnumerable<Tariff>>();

            foreach (var party in remoteCPOs.Values)
            {
                tariffVersionList.Add(party.Tariffs.Values());
                party.Tariffs.Clear();
            }

            await CommonAPI.LogAsset(
                      CommonHTTPAPI.removeAllRemoteTariffs,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var tariffVersion in tariffVersionList)
                    await LogEvent(
                              OnRemoteTariffRemoved,
                              loggingDelegate => loggingDelegate.Invoke(
                                  tariffVersion
                              )
                          );

            }

            return RemoveResult<IEnumerable<Tariff>>.Success(
                       EventTrackingId,
                       tariffVersionList.SelectMany(tariff => tariff)
                   );

        }

        #endregion

        #region RemoveAllRemoteTariffs     (IncludeRemoteTariffs,   ...)

        /// <summary>
        /// Remove all matching remote charging tariffs.
        /// </summary>
        /// <param name="IncludeRemoteTariffs">A filter delegate to include remote charging tariffs for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveAllRemoteTariffs(Func<Tariff, Boolean>  IncludeRemoteTariffs,
                                   Boolean                SkipNotifications   = false,
                                   EventTracking_Id?      EventTrackingId     = null,
                                   User_Id?               CurrentUserId       = null,
                                   CancellationToken      CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingRemoteTariffs  = new List<Tariff>();
            var removedRemoteTariffs   = new List<Tariff>();
            var failedRemoteTariffs    = new List<RemoveResult<Tariff>>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var tariff in party.Tariffs.Values())
                {
                    if (IncludeRemoteTariffs(tariff))
                        matchingRemoteTariffs.Add(tariff);
                }
            }

            foreach (var tariff in matchingRemoteTariffs)
            {

                var result = await RemoveRemoteTariff(
                                       tariff,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedRemoteTariffs.Add(tariff);
                else
                    failedRemoteTariffs. Add(
                        RemoveResult<Tariff>.Failed(
                            EventTrackingId,
                            tariff,
                            result.ErrorResponse ?? "Unknown error while removing the remote charging tariff!"
                        )
                    );

            }

            return removedRemoteTariffs.Count != 0 && failedRemoteTariffs.Count == 0

                       ? RemoveResult<IEnumerable<Tariff>>.Success(
                             EventTrackingId,
                             removedRemoteTariffs
                         )

                       : removedRemoteTariffs.Count == 0 && failedRemoteTariffs.Count == 0

                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Tariff>>.Failed(
                                   EventTrackingId,
                                   failedRemoteTariffs.
                                       Select(removeResult => removeResult.Data).
                                       Where (tariff       => tariff is not null).
                                       Cast<Tariff>(),
                                   failedRemoteTariffs.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllRemoteTariffs     (IncludeRemoteTariffIds, ...)

        /// <summary>
        /// Remove all matching remote charging tariffs.
        /// </summary>
        /// <param name="IncludeRemoteTariffIds">A filter delegate to include remote charging tariffs for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveAllRemoteTariffs(Func<Party_Idv3, Tariff_Id, Boolean>  IncludeRemoteTariffIds,
                                   Boolean                               SkipNotifications   = false,
                                   EventTracking_Id?                     EventTrackingId     = null,
                                   User_Id?                              CurrentUserId       = null,
                                   CancellationToken                     CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingRemoteTariffs  = new List<Tariff>();
            var removedRemoteTariffs   = new List<Tariff>();
            var failedRemoteTariffs    = new List<RemoveResult<Tariff>>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var tariff in party.Tariffs.Values())
                {
                    if (IncludeRemoteTariffIds(party.Id, tariff.Id))
                        matchingRemoteTariffs.Add(tariff);
                }
            }

            foreach (var tariff in matchingRemoteTariffs)
            {

                var result = await RemoveRemoteTariff(
                                       tariff,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedRemoteTariffs.Add(tariff);
                else
                    failedRemoteTariffs. Add(
                        RemoveResult<Tariff>.Failed(
                            EventTrackingId,
                            tariff,
                            result.ErrorResponse ?? "Unknown error while removing the remote charging tariff!"
                        )
                    );

            }

            return removedRemoteTariffs.Count != 0 && failedRemoteTariffs.Count == 0

                       ? RemoveResult<IEnumerable<Tariff>>.Success(
                             EventTrackingId,
                             removedRemoteTariffs
                         )

                       : removedRemoteTariffs.Count == 0 && failedRemoteTariffs.Count == 0

                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Tariff>>.Failed(
                                   EventTrackingId,
                                   failedRemoteTariffs.
                                       Select(removeResult => removeResult.Data).
                                       Where (tariff       => tariff is not null).
                                       Cast<Tariff>(),
                                   failedRemoteTariffs.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllRemoteTariffs     (PartyId, ...)

        /// <summary>
        /// Remove all remote charging tariffs owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveAllRemoteTariffs(Party_Idv3         PartyId,
                                   Boolean            SkipNotifications   = false,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   User_Id?           CurrentUserId       = null,
                                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                var matchingRemoteTariffs  = party.Tariffs.Values();
                var removedRemoteTariffs   = new List<Tariff>();
                var failedRemoteTariffs    = new List<RemoveResult<Tariff>>();

                foreach (var tariff in matchingRemoteTariffs)
                {

                    var result = await RemoveRemoteTariff(
                                           tariff,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedRemoteTariffs.Add(tariff);
                    else
                        failedRemoteTariffs. Add(
                            RemoveResult<Tariff>.Failed(
                                EventTrackingId,
                                tariff,
                                result.ErrorResponse ?? "Unknown error while removing the remote charging tariff!"
                            )
                        );

                }

                return removedRemoteTariffs.Count != 0 && failedRemoteTariffs.Count == 0

                           ? RemoveResult<IEnumerable<Tariff>>.Success(
                                 EventTrackingId,
                                 removedRemoteTariffs
                             )

                           : removedRemoteTariffs.Count == 0 && failedRemoteTariffs.Count == 0

                                 ? RemoveResult<IEnumerable<Tariff>>.NoOperation(
                                       EventTrackingId,
                                       []
                                   )

                                 : RemoveResult<IEnumerable<Tariff>>.Failed(
                                       EventTrackingId,
                                       failedRemoteTariffs.
                                           Select(removeResult => removeResult.Data).
                                           Where (tariff      => tariff is not null).
                                           Cast<Tariff>(),
                                       failedRemoteTariffs.Select(removeResult => removeResult.ErrorResponse).
                                           AggregateWith(", ")
                                   );

            }

            return RemoveResult<IEnumerable<Tariff>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' is unknown!"
                   );

        }

        #endregion


        #region RemoteTariffExist          (PartyId, RemoteTariffId,             Timestamp = null, Tolerance = null)

        public Boolean RemoteTariffExists(Party_Idv3       PartyId,
                                          Tariff_Id        RemoteTariffId,
                                          DateTimeOffset?  Timestamp   = null,
                                          TimeSpan?        Tolerance   = null)
        {

            if (remoteCPOs.TryGetValue(PartyId, out var party))
                return party.Tariffs.ContainsKey(RemoteTariffId);

            var onRemoteTariffSlowStorageLookup = OnRemoteTariffSlowStorageLookup;
            if (onRemoteTariffSlowStorageLookup is not null)
            {
                try
                {

                    return onRemoteTariffSlowStorageLookup(
                               PartyId,
                               RemoteTariffId,
                               Timestamp,
                               Tolerance
                           ).Result is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoteTariffExists), " ", nameof(OnRemoteTariffSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            return false;

        }

        #endregion

        #region GetRemoteTariff            (PartyId, RemoteTariffId,             Timestamp = null, Tolerance = null)

        public Tariff? GetRemoteTariff(Party_Idv3       PartyId,
                                       Tariff_Id        RemoteTariffId,
                                       DateTimeOffset?  Timestamp   = null,
                                       TimeSpan?        Tolerance   = null)
        {

            if (TryGetRemoteTariff(PartyId,
                                   RemoteTariffId,
                                   out var tariff,
                                   Timestamp,
                                   Tolerance))
            {
                return tariff;
            }

            return null;

        }

        #endregion

        #region TryGetRemoteTariff         (PartyId, RemoteTariffId, out RemoteTariff, Timestamp = null, Tolerance = null)

        public Boolean TryGetRemoteTariff(Party_Idv3                       PartyId,
                                          Tariff_Id                        RemoteTariffId,
                                          [NotNullWhen(true)] out Tariff?  RemoteTariff,
                                          DateTimeOffset?                  Timestamp   = null,
                                          TimeSpan?                        Tolerance   = null)
        {

            if (remoteCPOs.   TryGetValue(PartyId,        out var party) &&
                party.Tariffs.TryGetValue(RemoteTariffId, out var tariff))
            {
                RemoteTariff = tariff;
                return true;
            }

            var onRemoteTariffLookup = OnRemoteTariffSlowStorageLookup;
            if (onRemoteTariffLookup is not null)
            {
                try
                {

                    RemoteTariff = onRemoteTariffLookup(
                                       PartyId,
                                       RemoteTariffId,
                                       Timestamp,
                                       Tolerance
                                   ).Result;

                    return RemoteTariff is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetRemoteTariff), " ", nameof(OnRemoteTariffSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            RemoteTariff = null;
            return false;

        }

        #endregion

        #region GetRemoteTariffs           (IncludeRemoteTariff)

        public IEnumerable<Tariff> GetRemoteTariffs(Func<Tariff, Boolean>  IncludeRemoteTariff)
        {

            var tariffs = new List<Tariff>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var tariff in party.Tariffs.Values())
                {
                    if (IncludeRemoteTariff(tariff))
                        tariffs.Add(tariff);
                }
            }

            return tariffs;

        }

        #endregion

        #region GetRemoteTariffs           (PartyId = null)

        public IEnumerable<Tariff> GetRemoteTariffs(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (remoteCPOs.TryGetValue(PartyId.Value, out var party))
                    return party.Tariffs.Values();
            }

            else
            {

                var tariffs = new List<Tariff>();

                foreach (var party in remoteCPOs.Values)
                    tariffs.AddRange(party.Tariffs.Values());

                return tariffs;

            }

            return [];

        }

        #endregion


        #region GetRemoteTariffIds         (PartyId?, RemoteLocationId?, EVSEId?, ConnectorId?, EMSPId?)

        //public IEnumerable<Tariff_Id> GetRemoteTariffIds(Party_Idv3     PartyId,
        //                                                 Location_Id?   RemoteLocationId,
        //                                                 EVSE_Id?       EVSEId,
        //                                                 Connector_Id?  ConnectorId,
        //                                                 EMSP_Id?       EMSPId)

        //    => GetRemoteTariffIdsDelegate?.Invoke(
        //           PartyId,
        //           RemoteLocationId,
        //           EVSEId,
        //           ConnectorId,
        //           EMSPId
        //       ) ?? [];

        #endregion

        #endregion

        #region RemoteSessions

        #region Events

        public delegate Task OnRemoteSessionAddedDelegate          (Session RemoteSession);
        public delegate Task OnChargingRemoteSessionChangedDelegate(Session RemoteSession);
        public delegate Task OnRemoteSessionRemovedDelegate        (Session RemoteSession);

        public event OnRemoteSessionAddedDelegate?            OnRemoteSessionAdded;
        public event OnChargingRemoteSessionChangedDelegate?  OnRemoteSessionChanged;
        public event OnRemoteSessionRemovedDelegate?          OnRemoteSessionRemoved;

        #endregion


        public delegate Task<Session> OnRemoteSessionSlowStorageLookupDelegate(Party_Idv3  PartyId,
                                                                               Session_Id  RemoteSessionId);

        public event OnRemoteSessionSlowStorageLookupDelegate? OnRemoteSessionSlowStorageLookup;


        #region AddRemoteSession            (RemoteSession, ...)

        /// <summary>
        /// Add the given remote charging session.
        /// </summary>
        /// <param name="RemoteSession">The remote charging session to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Session>>

            AddRemoteSession(Session            RemoteSession,
                             Boolean            SkipNotifications   = false,
                             EventTracking_Id?  EventTrackingId     = null,
                             User_Id?           CurrentUserId       = null,
                             CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteSession.CountryCode,
                                    RemoteSession.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (party.Sessions.TryAdd(RemoteSession.Id, RemoteSession))
                {

                    RemoteSession.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addRemoteSession,
                              RemoteSession.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomPriceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteSessionAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteSession
                                  )
                              );

                    }

                    return AddResult<Session>.Success(
                               EventTrackingId,
                               RemoteSession
                           );

                }

                return AddResult<Session>.Failed(
                           EventTrackingId,
                           RemoteSession,
                           "The given remote session already exists!"
                       );

            }

            return AddResult<Session>.Failed(
                       EventTrackingId,
                       RemoteSession,
                       $"The party identification '{partyId}' of the remote session is unknown!"
                   );

        }

        #endregion

        #region AddRemoteSessionIfNotExists (RemoteSession, ...)

        /// <summary>
        /// Add the given remote charging session if it does not already exist.
        /// </summary>
        /// <param name="RemoteSession">The remote charging session to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Session>>

            AddRemoteSessionIfNotExists(Session            RemoteSession,
                                        Boolean            SkipNotifications   = false,
                                        EventTracking_Id?  EventTrackingId     = null,
                                        User_Id?           CurrentUserId       = null,
                                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteSession.CountryCode,
                                    RemoteSession.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (party.Sessions.TryAdd(RemoteSession.Id, RemoteSession))
                {

                    RemoteSession.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addRemoteSession,
                              RemoteSession.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomPriceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteSessionAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteSession
                                  )
                              );

                    }

                    return AddResult<Session>.Success(
                               EventTrackingId,
                               RemoteSession
                           );

                }

                return AddResult<Session>.NoOperation(
                           EventTrackingId,
                           RemoteSession,
                           "The given remote session already exists."
                       );

            }

            return AddResult<Session>.Failed(
                       EventTrackingId,
                       RemoteSession,
                       $"The party identification '{partyId}' of the remote session is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateRemoteSession    (RemoteSession,                          AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given remote charging session.
        /// </summary>
        /// <param name="RemoteSession">The remote charging session to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<Session>>

            AddOrUpdateRemoteSession(Session            RemoteSession,
                                     Boolean?           AllowDowngrades     = false,
                                     Boolean            SkipNotifications   = false,
                                     EventTracking_Id?  EventTrackingId     = null,
                                     User_Id?           CurrentUserId       = null,
                                     CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteSession.CountryCode,
                                    RemoteSession.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                #region Update an existing session

                if (party.Sessions.TryGetValue(RemoteSession.Id, out var existingRemoteSession))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        RemoteSession.LastUpdated <= existingRemoteSession.LastUpdated)
                    {

                        return AddOrUpdateResult<Session>.Failed(
                                   EventTrackingId,
                                   RemoteSession,
                                   "The 'lastUpdated' timestamp of the new remote charging session must be newer then the timestamp of the existing session!"
                               );

                    }

                    //if (RemoteSession.LastUpdated.ToISO8601() == existingRemoteSession.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<Session>.NoOperation(RemoteSession,
                    //                                                   "The 'lastUpdated' timestamp of the new session must be newer then the timestamp of the existing session!");

                    var aa = existingRemoteSession.Equals(existingRemoteSession);

                    if (party.Sessions.TryUpdate(RemoteSession.Id,
                                                 RemoteSession,
                                                 existingRemoteSession))
                    {

                        RemoteSession.CommonAPI = CommonAPI;

                        await CommonAPI.LogAsset(
                                  CommonHTTPAPI.addOrUpdateRemoteSession,
                                  RemoteSession.ToJSON(
                                      //true,
                                      //true,
                                      //true,
                                      //true,
                                      CustomSessionSerializer,
                                      CustomCDRTokenSerializer,
                                      CustomChargingPeriodSerializer,
                                      CustomCDRDimensionSerializer,
                                      CustomPriceSerializer
                                  ),
                                  EventTrackingId,
                                  CurrentUserId,
                                  CancellationToken
                              );

                        if (!SkipNotifications)
                        {

                            await LogEvent(
                                      OnRemoteSessionChanged,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          RemoteSession
                                      )
                                  );

                        }

                        return AddOrUpdateResult<Session>.Updated(
                                   EventTrackingId,
                                   RemoteSession
                               );

                    }

                    return AddOrUpdateResult<Session>.Failed(
                               EventTrackingId,
                               RemoteSession,
                               "Updating the given remote charging session failed!"
                           );

                }

                #endregion

                #region Add a new session

                if (party.Sessions.TryAdd(RemoteSession.Id, RemoteSession))
                {

                    RemoteSession.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addOrUpdateRemoteSession,
                              RemoteSession.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomPriceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteSessionAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteSession
                                  )
                              );

                    }

                    return AddOrUpdateResult<Session>.Created(
                               EventTrackingId,
                               RemoteSession
                           );

                }

                #endregion

                return AddOrUpdateResult<Session>.Failed(
                           EventTrackingId,
                           RemoteSession,
                           "Adding the given remote charging session failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<Session>.Failed(
                       EventTrackingId,
                       RemoteSession,
                       $"The party identification '{partyId}' of the remote charging session is unknown!"
                   );

        }

        #endregion

        #region UpdateRemoteSession         (RemoteSession,                          AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given remote charging session.
        /// </summary>
        /// <param name="RemoteSession">The remote charging session to update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<Session>>

            UpdateRemoteSession(Session            RemoteSession,
                                Boolean?           AllowDowngrades     = false,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteSession.CountryCode,
                                    RemoteSession.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (!party.Sessions.TryGetValue(RemoteSession.Id, out var existingRemoteSession))
                    return UpdateResult<Session>.Failed(
                               EventTrackingId,
                               RemoteSession,
                               $"The given remote charging session identification '{RemoteSession.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    RemoteSession.LastUpdated <= existingRemoteSession.LastUpdated)
                {

                    return UpdateResult<Session>.Failed(
                               EventTrackingId, RemoteSession,
                               "The 'lastUpdated' timestamp of the new remote charging session must be newer then the timestamp of the existing session!"
                           );

                }

                #endregion


                if (party.Sessions.TryUpdate(RemoteSession.Id,
                                             RemoteSession,
                                             existingRemoteSession))
                {

                    RemoteSession.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.updateRemoteSession,
                              RemoteSession.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomPriceSerializer
                             ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteSessionChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteSession
                                  )
                              );

                    }

                    return UpdateResult<Session>.Success(
                               EventTrackingId,
                               RemoteSession
                           );

                }

                return UpdateResult<Session>.Failed(
                           EventTrackingId,
                           RemoteSession,
                           "RemoteSessions.TryUpdate(RemoteSession.Id, RemoteSession, RemoteSession) failed!"
                       );

            }

            return UpdateResult<Session>.Failed(
                       EventTrackingId,
                       RemoteSession,
                       $"The party identification '{partyId}' of the remote charging session is unknown!"
                   );

        }

        #endregion

        #region TryPatchRemoteSession       (PartyId, RemoteSessionId, RemoteSessionPatch, AllowDowngrades = false, ...)

        /// <summary>
        /// Try to patch the given remote charging session with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the remote charging session.</param>
        /// <param name="RemoteSessionId">The identification of the remote charging session to patch.</param>
        /// <param name="RemoteSessionPatch">The JSON patch document to apply to the session.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<Session>>

            TryPatchRemoteSession(Party_Idv3         PartyId,
                                  Session_Id         RemoteSessionId,
                                  JObject            RemoteSessionPatch,
                                  Boolean?           AllowDowngrades     = false,
                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                if (party.Sessions.TryGetValue(RemoteSessionId, out var existingRemoteSession))
                {

                    var patchResult = existingRemoteSession.TryPatch(
                                          RemoteSessionPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedRemoteSession))
                    {

                        var updateRemoteSessionResult = await UpdateRemoteSession(
                                                                  patchedRemoteSession,
                                                                  AllowDowngrades,
                                                                  SkipNotifications,
                                                                  EventTrackingId,
                                                                  CurrentUserId,
                                                                  CancellationToken
                                                              );

                        if (updateRemoteSessionResult.IsFailed)
                            return PatchResult<Session>.Failed(
                                       EventTrackingId,
                                       existingRemoteSession,
                                       "Could not patch the remote charging session: " + updateRemoteSessionResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<Session>.Failed(
                           EventTrackingId,
                           $"The given remote charging session '{RemoteSessionId}' is does not exist!"
                       );

            }

            return PatchResult<Session>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charging session is unknown!"
                   );

        }

        #endregion


        #region RemoveRemoteSession         (RemoteSession, ...)

        /// <summary>
        /// Remove the given remote charging session.
        /// </summary>
        /// <param name="RemoteSession">A remote charging session.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<Session>>

            RemoveRemoteSession(Session            RemoteSession,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

                => RemoveRemoteSession(
                       Party_Idv3.From(
                           RemoteSession.CountryCode,
                           RemoteSession.PartyId
                       ),
                       RemoteSession.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveRemoteSession         (PartyId, RemoteSessionId, ...)

        /// <summary>
        /// Remove the given remote charging session.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the remote charging session.</param>
        /// <param name="RemoteSessionId">An unique remote charging session identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<Session>>

            RemoveRemoteSession(Party_Idv3         PartyId,
                                Session_Id         RemoteSessionId,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                if (party.Sessions.TryRemove(RemoteSessionId, out var session))
                {

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.removeRemoteSession,
                              session.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomPriceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteSessionRemoved,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      session
                                  )
                              );

                    }

                    return RemoveResult<Session>.Success(
                               EventTrackingId,
                               session
                           );

                }

                return RemoveResult<Session>.Failed(
                           EventTrackingId,
                           $"The remote charging session '{PartyId}/{RemoteSessionId}' is unknown!"
                       );

            }

            return RemoveResult<Session>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charging session is unknown!"
                   );

        }

        #endregion

        #region RemoveAllRemoteSessions     (...)

        /// <summary>
        /// Remove all remote charging sessions.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllRemoteSessions(Boolean            SkipNotifications   = false,
                                    EventTracking_Id?  EventTrackingId     = null,
                                    User_Id?           CurrentUserId       = null,
                                    CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var sessions = new List<Session>();

            foreach (var party in remoteCPOs.Values)
            {
                sessions.AddRange(party.Sessions.Values);
                party.Sessions.Clear();
            }

            await CommonAPI.LogAsset(
                      CommonHTTPAPI.removeAllRemoteSessions,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var session in sessions)
                    await LogEvent(
                              OnRemoteSessionRemoved,
                              loggingDelegate => loggingDelegate.Invoke(
                                  session
                              )
                          );

            }

            return RemoveResult<IEnumerable<Session>>.Success(
                       EventTrackingId,
                       sessions
                   );

        }

        #endregion

        #region RemoveAllRemoteSessions     (IncludeRemoteSessions, ...)

        /// <summary>
        /// Remove all matching remote charging sessions.
        /// </summary>
        /// <param name="IncludeRemoteSessions">A filter delegate to include remote charging sessions for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllRemoteSessions(Func<Session, Boolean>  IncludeRemoteSessions,
                                    Boolean                 SkipNotifications   = false,
                                    EventTracking_Id?       EventTrackingId     = null,
                                    User_Id?                CurrentUserId       = null,
                                    CancellationToken       CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingRemoteSessions  = new List<Session>();
            var removedRemoteSessions   = new List<Session>();
            var failedRemoteSessions    = new List<RemoveResult<Session>>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var session in party.Sessions.Values)
                {
                    if (IncludeRemoteSessions(session))
                        matchingRemoteSessions.Add(session);
                }
            }

            foreach (var session in matchingRemoteSessions)
            {

                var result = await RemoveRemoteSession(
                                       session,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedRemoteSessions.Add(session);
                else
                    failedRemoteSessions. Add(result);

            }

            return removedRemoteSessions.Count != 0 && failedRemoteSessions.Count == 0

                       ? RemoveResult<IEnumerable<Session>>.Success(
                             EventTrackingId,
                             removedRemoteSessions
                         )

                       : removedRemoteSessions.Count == 0 && failedRemoteSessions.Count == 0

                             ? RemoveResult<IEnumerable<Session>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Session>>.Failed(
                                   EventTrackingId,
                                   failedRemoteSessions.
                                       Select(removeResult => removeResult.Data).
                                       Where (session      => session is not null).
                                       Cast<Session>(),
                                   failedRemoteSessions.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllRemoteSessions     (IncludeRemoteSessionIds, ...)

        /// <summary>
        /// Remove all matching remote charging sessions.
        /// </summary>
        /// <param name="IncludeRemoteSessionIds">A filter delegate to include remote charging sessions for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllRemoteSessions(Func<Party_Idv3, Session_Id, Boolean>  IncludeRemoteSessionIds,
                                    Boolean                                SkipNotifications   = false,
                                    EventTracking_Id?                      EventTrackingId     = null,
                                    User_Id?                               CurrentUserId       = null,
                                    CancellationToken                      CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingRemoteSessions  = new List<Session>();
            var removedRemoteSessions   = new List<Session>();
            var failedRemoteSessions    = new List<RemoveResult<Session>>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var session in party.Sessions.Values)
                {
                    if (IncludeRemoteSessionIds(party.Id, session.Id))
                        matchingRemoteSessions.Add(session);
                }
            }

            foreach (var session in matchingRemoteSessions)
            {

                var result = await RemoveRemoteSession(
                                       session,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedRemoteSessions.Add(session);
                else
                    failedRemoteSessions. Add(result);

            }

            return removedRemoteSessions.Count != 0 && failedRemoteSessions.Count == 0

                       ? RemoveResult<IEnumerable<Session>>.Success(
                             EventTrackingId,
                             removedRemoteSessions
                         )

                       : removedRemoteSessions.Count == 0 && failedRemoteSessions.Count == 0

                             ? RemoveResult<IEnumerable<Session>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Session>>.Failed(
                                   EventTrackingId,
                                   failedRemoteSessions.
                                       Select(removeResult => removeResult.Data).
                                       Where (session      => session is not null).
                                       Cast<Session>(),
                                   failedRemoteSessions.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllRemoteSessions     (PartyId, ...)

        /// <summary>
        /// Remove all remote charging sessions owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllRemoteSessions(Party_Idv3         PartyId,
                                    Boolean            SkipNotifications   = false,
                                    EventTracking_Id?  EventTrackingId     = null,
                                    User_Id?           CurrentUserId       = null,
                                    CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                var matchingRemoteSessions  = party.Sessions.Values;
                var removedRemoteSessions   = new List<Session>();
                var failedRemoteSessions    = new List<RemoveResult<Session>>();

                foreach (var session in matchingRemoteSessions)
                {

                    var result = await RemoveRemoteSession(
                                           session,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedRemoteSessions.Add(session);
                    else
                        failedRemoteSessions. Add(result);

                }

                return removedRemoteSessions.Count != 0 && failedRemoteSessions.Count == 0

                           ? RemoveResult<IEnumerable<Session>>.Success(
                                 EventTrackingId,
                                 removedRemoteSessions
                             )

                           : removedRemoteSessions.Count == 0 && failedRemoteSessions.Count == 0

                                 ? RemoveResult<IEnumerable<Session>>.NoOperation(
                                       EventTrackingId,
                                       []
                                   )

                                 : RemoveResult<IEnumerable<Session>>.Failed(
                                       EventTrackingId,
                                       failedRemoteSessions.
                                           Select(removeResult => removeResult.Data).
                                           Where (session      => session is not null).
                                           Cast<Session>(),
                                       failedRemoteSessions.Select(removeResult => removeResult.ErrorResponse).
                                           AggregateWith(", ")
                                   );

            }

            return RemoveResult<IEnumerable<Session>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charging session is unknown!"
                   );

        }

        #endregion


        #region RemoteSessionExists         (PartyId, RemoteSessionId)

        public Boolean RemoteSessionExists(Party_Idv3  PartyId,
                                           Session_Id  RemoteSessionId)
        {

            if (remoteCPOs.TryGetValue(PartyId, out var party) &&
                party.Sessions.ContainsKey(RemoteSessionId))
            {
                return true;
            }

            var onRemoteSessionSlowStorageLookup = OnRemoteSessionSlowStorageLookup;
            if (onRemoteSessionSlowStorageLookup is not null)
            {
                try
                {

                    return onRemoteSessionSlowStorageLookup(
                               PartyId,
                               RemoteSessionId
                           ).Result is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoteSessionExists), " ", nameof(OnRemoteSessionSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            return false;

        }

        #endregion

        #region GetRemoteSession            (PartyId, RemoteSessionId)

        public Session? GetRemoteSession(Party_Idv3  PartyId,
                                         Session_Id  RemoteSessionId)
        {

            if (TryGetRemoteSession(PartyId,
                                    RemoteSessionId,
                                    out var cdr))
            {
                return cdr;
            }

            return null;

        }

        #endregion

        #region TryGetRemoteSession         (PartyId, RemoteSessionId, out RemoteSession)

        public Boolean TryGetRemoteSession(Party_Idv3                        PartyId,
                                           Session_Id                        RemoteSessionId,
                                           [NotNullWhen(true)] out Session?  RemoteSession)
        {

            if (remoteCPOs.    TryGetValue(PartyId,         out var party) &&
                party.Sessions.TryGetValue(RemoteSessionId, out RemoteSession))
            {
                return true;
            }

            var onRemoteSessionSlowStorageLookup = OnRemoteSessionSlowStorageLookup;
            if (onRemoteSessionSlowStorageLookup is not null)
            {
                try
                {

                    RemoteSession = onRemoteSessionSlowStorageLookup(
                                  PartyId,
                                  RemoteSessionId
                              ).Result;

                    return RemoteSession is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetRemoteSession), " ", nameof(OnRemoteSessionSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            RemoteSession = null;
            return false;

        }

        #endregion

        #region GetRemoteSessions           (IncludeRemoteSession)

        public IEnumerable<Session> GetRemoteSessions(Func<Session, Boolean> IncludeRemoteSession)
        {

            var sessions = new List<Session>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var session in party.Sessions.Values)
                {
                    if (IncludeRemoteSession(session))
                        sessions.Add(session);
                }
            }

            return sessions;

        }

        #endregion

        #region GetRemoteSessions           (PartyId = null)

        public IEnumerable<Session> GetRemoteSessions(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (remoteCPOs.TryGetValue(PartyId.Value, out var party))
                    return party.Sessions.Values;
            }

            else
            {

                var sessions = new List<Session>();

                foreach (var party in remoteCPOs.Values)
                    sessions.AddRange(party.Sessions.Values);

                return sessions;

            }

            return [];

        }

        #endregion

        #endregion

        #region RemoteChargeDetailRecords

        #region Events

        public delegate Task OnChargeDetailRecordAddedDelegate  (CDR RemoteCDR);
        public delegate Task OnChargeDetailRecordChangedDelegate(CDR RemoteCDR);
        public delegate Task OnChargeDetailRecordRemovedDelegate(CDR RemoteCDR);

        public event OnChargeDetailRecordAddedDelegate?    OnRemoteChargeDetailRecordAdded;
        public event OnChargeDetailRecordChangedDelegate?  OnRemoteChargeDetailRecordChanged;
        public event OnChargeDetailRecordRemovedDelegate?  OnRemoteChargeDetailRecordRemoved;

        #endregion


        public delegate Task<CDR> OnChargeDetailRecordSlowStorageLookupDelegate(Party_Idv3  PartyId,
                                                                                CDR_Id      RemoteCDRId);

        public event OnChargeDetailRecordSlowStorageLookupDelegate? OnRemoteChargeDetailRecordSlowStorageLookup;


        #region AddRemoteCDR            (RemoteCDR, ...)

        /// <summary>
        /// Add the given remote charge detail record.
        /// </summary>
        /// <param name="RemoteCDR">The remote charge detail record to add or update.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<CDR>>

            AddRemoteCDR(CDR                RemoteCDR,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null,
                         CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteCDR.CDRToken.CountryCode,
                                    RemoteCDR.CDRToken.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (party.CDRs.TryAdd(RemoteCDR.Id, RemoteCDR))
                {

                    RemoteCDR.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addChargeDetailRecord,
                              RemoteCDR.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomSignedDataSerializer,
                                  CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteChargeDetailRecordAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteCDR
                                  )
                              );

                    }

                    return AddResult<CDR>.Success(
                               EventTrackingId,
                               RemoteCDR
                           );

                }

                return AddResult<CDR>.Failed(
                           EventTrackingId,
                           RemoteCDR,
                           "The given remote charge detail record already exists!"
                       );

            }

            return AddResult<CDR>.Failed(
                       EventTrackingId,
                       RemoteCDR,
                       $"The party identification '{partyId}' of the remote charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddRemoteCDRIfNotExists (RemoteCDR, ...)

        /// <summary>
        /// Add the given remote charge detail record if it does not already exist.
        /// </summary>
        /// <param name="RemoteCDR">The remote charge detail record to add or update.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<CDR>>

            AddRemoteCDRIfNotExists(CDR                RemoteCDR,
                                    Boolean            SkipNotifications   = false,
                                    EventTracking_Id?  EventTrackingId     = null,
                                    User_Id?           CurrentUserId       = null,
                                    CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteCDR.CDRToken.CountryCode,
                                    RemoteCDR.CDRToken.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (party.CDRs.TryAdd(RemoteCDR.Id, RemoteCDR))
                {

                    RemoteCDR.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addChargeDetailRecordIfNotExists,
                              RemoteCDR.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomSignedDataSerializer,
                                  CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteChargeDetailRecordAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteCDR
                                  )
                              );

                    }

                    return AddResult<CDR>.Success(
                               EventTrackingId,
                               RemoteCDR
                           );

                }

                return AddResult<CDR>.NoOperation(
                           EventTrackingId,
                           RemoteCDR,
                           "The given remote charge detail record already exists."
                       );

            }

            return AddResult<CDR>.Failed(
                       EventTrackingId,
                       RemoteCDR,
                       $"The party identification '{partyId}' of the remote charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateRemoteCDR    (RemoteCDR, AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given remote charge detail record.
        /// </summary>
        /// <param name="RemoteCDR">The remote charge detail record to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<CDR>>

            AddOrUpdateRemoteCDR(CDR                RemoteCDR,
                                 Boolean?           AllowDowngrades     = false,
                                 Boolean            SkipNotifications   = false,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 User_Id?           CurrentUserId       = null,
                                 CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteCDR.CDRToken.CountryCode,
                                    RemoteCDR.CDRToken.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                #region Update an existing charge detail record

                if (party.CDRs.TryGetValue(RemoteCDR.Id, out var existingRemoteCDR))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        RemoteCDR.LastUpdated <= existingRemoteCDR.LastUpdated)
                    {

                        return AddOrUpdateResult<CDR>.Failed(
                                   EventTrackingId,
                                   RemoteCDR,
                                   "The 'lastUpdated' timestamp of the new remote charge detail record must be newer then the timestamp of the existing charge detail record!"
                               );

                    }

                    //if (RemoteCDR.LastUpdated.ToISO8601() == existingRemoteCDR.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<CDR>.NoOperation(RemoteCDR,
                    //                                                   "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");

                    var aa = existingRemoteCDR.Equals(existingRemoteCDR);

                    if (party.CDRs.TryUpdate(RemoteCDR.Id,
                                             RemoteCDR,
                                             existingRemoteCDR))
                    {

                        RemoteCDR.CommonAPI = CommonAPI;

                        await CommonAPI.LogAsset(
                                  CommonHTTPAPI.addOrUpdateChargeDetailRecord,
                                  RemoteCDR.ToJSON(
                                      //true,
                                      //true,
                                      //true,
                                      //true,
                                      CustomCDRSerializer,
                                      CustomCDRTokenSerializer,
                                      CustomCDRLocationSerializer,
                                      CustomEVSEEnergyMeterSerializer,
                                      CustomTransparencySoftwareSerializer,
                                      CustomTariffSerializer,
                                      CustomDisplayTextSerializer,
                                      CustomPriceSerializer,
                                      CustomTariffElementSerializer,
                                      CustomPriceComponentSerializer,
                                      CustomTariffRestrictionsSerializer,
                                      CustomEnergyMixSerializer,
                                      CustomEnergySourceSerializer,
                                      CustomEnvironmentalImpactSerializer,
                                      CustomChargingPeriodSerializer,
                                      CustomCDRDimensionSerializer,
                                      CustomSignedDataSerializer,
                                      CustomSignedValueSerializer
                                  ),
                                  EventTrackingId,
                                  CurrentUserId,
                                  CancellationToken
                              );

                        if (!SkipNotifications)
                        {

                            await LogEvent(
                                      OnRemoteChargeDetailRecordChanged,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          RemoteCDR
                                      )
                                  );

                        }

                        return AddOrUpdateResult<CDR>.Updated(
                                   EventTrackingId,
                                   RemoteCDR
                               );

                    }

                    return AddOrUpdateResult<CDR>.Failed(
                               EventTrackingId,
                               RemoteCDR,
                               "Updating the given remote charge detail record failed!"
                           );

                }

                #endregion

                #region Add a new charge detail record

                if (party.CDRs.TryAdd(RemoteCDR.Id, RemoteCDR))
                {

                    RemoteCDR.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.addOrUpdateChargeDetailRecord,
                              RemoteCDR.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomSignedDataSerializer,
                                  CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteChargeDetailRecordAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteCDR
                                  )
                              );

                    }

                    return AddOrUpdateResult<CDR>.Created(
                               EventTrackingId,
                               RemoteCDR
                           );

                }

                #endregion

                return AddOrUpdateResult<CDR>.Failed(
                           EventTrackingId,
                           RemoteCDR,
                           "Adding the given remote charge detail record failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<CDR>.Failed(
                       EventTrackingId,
                       RemoteCDR,
                       $"The party identification '{partyId}' of the remote charge detail record is unknown!"
                   );

        }

        #endregion

        #region UpdateRemoteCDR         (RemoteCDR, AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given remote charge detail record.
        /// </summary>
        /// <param name="RemoteCDR">The remote charge detail record to update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<CDR>>

            UpdateRemoteCDR(CDR                RemoteCDR,
                            Boolean?           AllowDowngrades     = false,
                            Boolean            SkipNotifications   = false,
                            EventTracking_Id?  EventTrackingId     = null,
                            User_Id?           CurrentUserId       = null,
                            CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    RemoteCDR.CDRToken.CountryCode,
                                    RemoteCDR.CDRToken.PartyId
                                );

            if (remoteCPOs.TryGetValue(partyId, out var party))
            {

                if (!party.CDRs.TryGetValue(RemoteCDR.Id, out var existingRemoteCDR))
                    return UpdateResult<CDR>.Failed(
                               EventTrackingId,
                               RemoteCDR,
                               $"The given remote charge detail record identification '{RemoteCDR.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    RemoteCDR.LastUpdated <= existingRemoteCDR.LastUpdated)
                {

                    return UpdateResult<CDR>.Failed(
                               EventTrackingId, RemoteCDR,
                               "The 'lastUpdated' timestamp of the new remote charging charge detail record must be newer then the timestamp of the existing charge detail record!"
                           );

                }

                #endregion


                if (party.CDRs.TryUpdate(RemoteCDR.Id,
                                         RemoteCDR,
                                         existingRemoteCDR))
                {

                    RemoteCDR.CommonAPI = CommonAPI;

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.updateChargeDetailRecord,
                              RemoteCDR.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomSignedDataSerializer,
                                  CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteChargeDetailRecordChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      RemoteCDR
                                  )
                              );

                    }

                    return UpdateResult<CDR>.Success(
                               EventTrackingId,
                               RemoteCDR
                           );

                }

                return UpdateResult<CDR>.Failed(
                           EventTrackingId,
                           RemoteCDR,
                           "charge detail records.TryUpdate(RemoteCDR.Id, RemoteCDR, RemoteCDR) failed!"
                       );

            }

            return UpdateResult<CDR>.Failed(
                       EventTrackingId,
                       RemoteCDR,
                       $"The party identification '{partyId}' of the remote charge detail record is unknown!"
                   );

        }

        #endregion

        #region TryPatchRemoteCDR       (PartyId, RemoteCDRId, RemoteCDRPatch, AllowDowngrades = false, ...)   // Non-Standard

        /// <summary>
        /// Try to patch the given remote charge detail record with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the remote charge detail record.</param>
        /// <param name="RemoteCDRId">The identification of the remote charge detail record to patch.</param>
        /// <param name="RemoteCDRPatch">The JSON patch document to apply to the given charge detail record.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<CDR>>

            TryPatchRemoteCDR(Party_Idv3         PartyId,
                              CDR_Id             RemoteCDRId,
                              JObject            RemoteCDRPatch,
                              Boolean?           AllowDowngrades     = false,
                              Boolean            SkipNotifications   = false,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null,
                              CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (RemoteCDRPatch is null || !RemoteCDRPatch.HasValues)
                return PatchResult<CDR>.Failed(
                           EventTrackingId,
                           "The given remote charge detail record patch must not be null or empty!"
                       );

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                if (party.CDRs.TryGetValue(RemoteCDRId, out var existingRemoteCDR))
                {
 
                    var patchResult = existingRemoteCDR.TryPatch(
                                          RemoteCDRPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedRemoteCDR))
                    {

                        var updateRemoteCDRResult = await UpdateRemoteCDR(
                                                        patchedRemoteCDR,
                                                        AllowDowngrades,
                                                        SkipNotifications,
                                                        EventTrackingId,
                                                        CurrentUserId,
                                                        CancellationToken
                                                    );

                        if (updateRemoteCDRResult.IsFailed)
                            return PatchResult<CDR>.Failed(
                                       EventTrackingId,
                                       existingRemoteCDR,
                                       "Could not patch the remote charge detail record: " + updateRemoteCDRResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<CDR>.Failed(
                           EventTrackingId,
                           $"The given remote charge detail record '{PartyId}/{RemoteCDRId}' does not exist!"
                       );

            }

            return PatchResult<CDR>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charge detail record is unknown!"
                   );

        }

        #endregion


        #region RemoveRemoteCDR         (RemoteCDR, ...)

        /// <summary>
        /// Remove the given remote charge detail record.
        /// </summary>
        /// <param name="RemoteCDR">The remote charge detail record to remove.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<CDR>>

            RemoveRemoteCDR(CDR                RemoteCDR,
                            Boolean            SkipNotifications   = false,
                            EventTracking_Id?  EventTrackingId     = null,
                            User_Id?           CurrentUserId       = null,
                            CancellationToken  CancellationToken   = default)

                => RemoveRemoteCDR(
                       Party_Idv3.From(
                           RemoteCDR.CDRToken.CountryCode,
                           RemoteCDR.CDRToken.PartyId
                       ),
                       RemoteCDR.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveRemoteCDR         (PartyId, RemoteCDRId, ...)

        /// <summary>
        /// Remove the given remote charge detail record.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the remote charge detail record.</param>
        /// <param name="RemoteCDRId">A unique identification of a remote charge detail record.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<CDR>>

            RemoveRemoteCDR(Party_Idv3         PartyId,
                            CDR_Id             RemoteCDRId,
                            Boolean            SkipNotifications   = false,
                            EventTracking_Id?  EventTrackingId     = null,
                            User_Id?           CurrentUserId       = null,
                            CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                if (party.CDRs.TryRemove(RemoteCDRId, out var cdr))
                {

                    await CommonAPI.LogAsset(
                              CommonHTTPAPI.removeChargeDetailRecord,
                              cdr.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTariffRestrictionsSerializer,
                                  CustomEnergyMixSerializer,
                                  CustomEnergySourceSerializer,
                                  CustomEnvironmentalImpactSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomSignedDataSerializer,
                                  CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnRemoteChargeDetailRecordRemoved,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      cdr
                                  )
                              );

                    }

                    return RemoveResult<CDR>.Success(
                               EventTrackingId,
                               cdr
                           );

                }

                return RemoveResult<CDR>.Failed(
                           EventTrackingId,
                           $"The remote charge detail record '{PartyId}/{RemoteCDRId}' is unknown!"
                       );

            }

            return RemoveResult<CDR>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveAllRemoteCDRs     (...)

        /// <summary>
        /// Remove all remote charge detail records.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllRemoteCDRs(Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var cdrs = new List<CDR>();

            foreach (var party in remoteCPOs.Values)
            {
                cdrs.AddRange(party.CDRs.Values);
                party.CDRs.Clear();
            }

            await CommonAPI.LogAsset(
                      CommonHTTPAPI.removeAllChargeDetailRecords,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var cdr in cdrs)
                    await LogEvent(
                              OnRemoteChargeDetailRecordRemoved,
                              loggingDelegate => loggingDelegate.Invoke(
                                  cdr
                              )
                          );

            }

            return RemoveResult<IEnumerable<CDR>>.Success(
                       EventTrackingId,
                       cdrs
                   );

        }

        #endregion

        #region RemoveAllRemoteCDRs     (IncludeRemoteCDRs,   ...)

        /// <summary>
        /// Remove all matching remote charge detail records.
        /// </summary>
        /// <param name="IncludeRemoteCDRs">A filter delegate to include remote charge detail records for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllRemoteCDRs(Func<CDR, Boolean>  IncludeRemoteCDRs,
                                Boolean             SkipNotifications   = false,
                                EventTracking_Id?   EventTrackingId     = null,
                                User_Id?            CurrentUserId       = null,
                                CancellationToken   CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingRemoteCDRs  = new List<CDR>();
            var removedRemoteCDRs   = new List<CDR>();
            var failedRemoteCDRs    = new List<RemoveResult<CDR>>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var cdr in party.CDRs.Values)
                {
                    if (IncludeRemoteCDRs(cdr))
                        matchingRemoteCDRs.Add(cdr);
                }
            }

            foreach (var cdr in matchingRemoteCDRs)
            {

                var result = await RemoveRemoteCDR(
                                       cdr,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedRemoteCDRs.Add(cdr);
                else
                    failedRemoteCDRs. Add(result);

            }

            return removedRemoteCDRs.Count != 0 && failedRemoteCDRs.Count == 0

                       ? RemoveResult<IEnumerable<CDR>>.Success(
                             EventTrackingId,
                             removedRemoteCDRs
                         )

                       : removedRemoteCDRs.Count == 0 && failedRemoteCDRs.Count == 0

                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<CDR>>.Failed(
                                   EventTrackingId,
                                   failedRemoteCDRs.
                                       Select(removeResult => removeResult.Data).
                                       Where (cdr          => cdr is not null).
                                       Cast<CDR>(),
                                   failedRemoteCDRs.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllRemoteCDRs     (IncludeRemoteCDRIds, ...)

        /// <summary>
        /// Remove all matching remote charge detail records.
        /// </summary>
        /// <param name="IncludeRemoteCDRIds">A filter delegate to include remote charge detail records for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllRemoteCDRs(Func<CDR_Id, Boolean>  IncludeRemoteCDRIds,
                                Boolean                SkipNotifications   = false,
                                EventTracking_Id?      EventTrackingId     = null,
                                User_Id?               CurrentUserId       = null,
                                CancellationToken      CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingRemoteCDRs  = new List<CDR>();
            var removedRemoteCDRs   = new List<CDR>();
            var failedRemoteCDRs    = new List<RemoveResult<CDR>>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var cdr in party.CDRs.Values)
                {
                    if (IncludeRemoteCDRIds(cdr.Id))
                        matchingRemoteCDRs.Add(cdr);
                }
            }

            foreach (var cdr in matchingRemoteCDRs)
            {

                var result = await RemoveRemoteCDR(
                                       cdr,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedRemoteCDRs.Add(cdr);
                else
                    failedRemoteCDRs. Add(result);

            }

            return removedRemoteCDRs.Count != 0 && failedRemoteCDRs.Count == 0

                       ? RemoveResult<IEnumerable<CDR>>.Success(
                             EventTrackingId,
                             removedRemoteCDRs
                         )

                       : removedRemoteCDRs.Count == 0 && failedRemoteCDRs.Count == 0

                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<CDR>>.Failed(
                                   EventTrackingId,
                                   failedRemoteCDRs.
                                       Select(removeResult => removeResult.Data).
                                       Where (cdr          => cdr is not null).
                                       Cast<CDR>(),
                                   failedRemoteCDRs.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllRemoteCDRs     (PartyId, ...)

        /// <summary>
        /// Remove all remote charge detail records owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllRemoteCDRs(Party_Idv3         PartyId,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (remoteCPOs.TryGetValue(PartyId, out var party))
            {

                var matchingRemoteCDRs  = party.CDRs.Values;
                var removedRemoteCDRs   = new List<CDR>();
                var failedRemoteCDRs    = new List<RemoveResult<CDR>>();

                foreach (var cdr in matchingRemoteCDRs)
                {

                    var result = await RemoveRemoteCDR(
                                           cdr,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedRemoteCDRs.Add(cdr);
                    else
                        failedRemoteCDRs. Add(result);

                }

                return removedRemoteCDRs.Count != 0 && failedRemoteCDRs.Count == 0

                           ? RemoveResult<IEnumerable<CDR>>.Success(
                                 EventTrackingId,
                                 removedRemoteCDRs
                             )

                           : removedRemoteCDRs.Count == 0 && failedRemoteCDRs.Count == 0

                                 ? RemoveResult<IEnumerable<CDR>>.NoOperation(
                                       EventTrackingId,
                                       []
                                   )

                                 : RemoveResult<IEnumerable<CDR>>.Failed(
                                       EventTrackingId,
                                       failedRemoteCDRs.
                                           Select(removeResult => removeResult.Data).
                                           Where (cdr          => cdr is not null).
                                           Cast<CDR>(),
                                       failedRemoteCDRs.Select(removeResult => removeResult.ErrorResponse).
                                           AggregateWith(", ")
                                   );

            }

            return RemoveResult<IEnumerable<CDR>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the remote charge detail record is unknown!"
                   );

        }

        #endregion


        #region RemoteCDRExists         (PartyId, RemoteCDRId)

        public Boolean RemoteCDRExists(Party_Idv3  PartyId,
                                       CDR_Id      RemoteCDRId)
        {

            if (remoteCPOs.TryGetValue(PartyId, out var party) &&
                party.CDRs.ContainsKey(RemoteCDRId))
            {
                return true;
            }

            var onChargeDetailRecordSlowStorageLookup = OnRemoteChargeDetailRecordSlowStorageLookup;
            if (onChargeDetailRecordSlowStorageLookup is not null)
            {
                try
                {

                    return onChargeDetailRecordSlowStorageLookup(
                               PartyId,
                               RemoteCDRId
                           ).Result is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoteCDRExists), " ", nameof(OnRemoteChargeDetailRecordSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            return false;

        }

        #endregion

        #region GetRemoteCDR            (PartyId, RemoteCDRId)

        public CDR? GetRemoteCDR(Party_Idv3  PartyId,
                                 CDR_Id      RemoteCDRId)
        {

            if (TryGetRemoteCDR(PartyId,
                                RemoteCDRId,
                                out var cdr))
            {
                return cdr;
            }

            return null;

        }

        #endregion

        #region TryGetRemoteCDR         (PartyId, RemoteCDRId, out RemoteCDR)

        public Boolean TryGetRemoteCDR(Party_Idv3                    PartyId,
                                       CDR_Id                        RemoteCDRId,
                                       [NotNullWhen(true)] out CDR?  RemoteCDR)
        {

            if (remoteCPOs.TryGetValue(PartyId,     out var party) &&
                party.CDRs.TryGetValue(RemoteCDRId, out RemoteCDR))
            {
                return true;
            }

            var onChargeDetailRecordLookup = OnRemoteChargeDetailRecordSlowStorageLookup;
            if (onChargeDetailRecordLookup is not null)
            {
                try
                {

                    RemoteCDR = onChargeDetailRecordLookup(
                              PartyId,
                              RemoteCDRId
                          ).Result;

                    return RemoteCDR is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetRemoteCDR), " ", nameof(OnRemoteChargeDetailRecordSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            RemoteCDR = null;
            return false;

        }

        #endregion

        #region GetRemoteCDRs           (IncludeRemoteCDR)

        public IEnumerable<CDR> GetRemoteCDRs(Func<CDR, Boolean> IncludeRemoteCDR)
        {

            var sessions = new List<CDR>();

            foreach (var party in remoteCPOs.Values)
            {
                foreach (var cdr in party.CDRs.Values)
                {
                    if (IncludeRemoteCDR(cdr))
                        sessions.Add(cdr);
                }
            }

            return sessions;

        }

        #endregion

        #region GetRemoteCDRs           (PartyId = null)

        public IEnumerable<CDR> GetRemoteCDRs(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (remoteCPOs.TryGetValue(PartyId.Value, out var party))
                    return party.CDRs.Values;
            }

            else
            {

                var sessions = new List<CDR>();

                foreach (var party in remoteCPOs.Values)
                    sessions.AddRange(party.CDRs.Values);

                return sessions;

            }

            return [];

        }

        #endregion

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            var URLPathPrefix = HTTPPath.Root;

            #region GET    [/emsp] == /

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPathPrefix + "/emsp", "cloud.charging.open.protocols.OCPIv2_2_1API.SCSPAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //CommonAPI.AddOCPIMethod(
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPathPrefix + "/emsp/index.html",
            //                                 URLPathPrefix + "/emsp/"
            //                             },
            //                             HTTPContentType.Text.HTML_UTF8,
            //                             OCPIRequest: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(SCSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1API.SCSPAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(SCSPAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_2_1API.SCSPAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request.HTTPRequest) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     Server          = DefaultHTTPServerName,
            //                                     Date            = Timestamp.Now,
            //                                     ContentType     = HTTPContentType.Text.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = ConnectionType.Close
            //                                 };

            //                             });

            #endregion


            // Receiver Interface for eMSPs and NSPs

            #region ~/locations/{country_code}/{party_id}                               [NonStandard]

            #region OPTIONS  ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.DELETE ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{country_code}/{party_id}",
                GetLocationsRequest,
                GetLocationsResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    var filters            = request.GetDateAndPaginationFilters();

                    var allLocations       = CommonAPI.GetLocations(partyId.Value).ToArray();

                    var filteredLocations  = allLocations.Where(location => !filters.From.HasValue || location.LastUpdated >  filters.From.Value).
                                                          Where(location => !filters.To.  HasValue || location.LastUpdated <= filters.To.  Value).
                                                          ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredLocations.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(location => location.ToJSON(
                                                                                     request.EMSPId,
                                                                                     CustomLocationSerializer,
                                                                                     CustomPublishTokenSerializer,
                                                                                     CustomAdditionalGeoLocationSerializer,
                                                                                     CustomEVSESerializer,
                                                                                     CustomStatusScheduleSerializer,
                                                                                     CustomConnectorSerializer,
                                                                                     CustomLocationEnergyMeterSerializer,
                                                                                     CustomEVSEEnergyMeterSerializer,
                                                                                     CustomTransparencySoftwareStatusSerializer,
                                                                                     CustomTransparencySoftwareSerializer,
                                                                                     CustomDisplayTextSerializer,
                                                                                     CustomBusinessDetailsSerializer,
                                                                                     CustomHoursSerializer,
                                                                                     CustomImageSerializer,
                                                                                     CustomEnergyMixSerializer,
                                                                                     CustomEnergySourceSerializer,
                                                                                     CustomEnvironmentalImpactSerializer
                                                                                 ))
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allLocations.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "locations/{country_code}/{party_id}",
                DeleteLocationsRequest,
                DeleteLocationsResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //ToDo: await...
                    var result = await CommonAPI.RemoveAllLocations(partyId.Value);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                GetLocationRequest,
                GetLocationResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check location

                    if (!request.ParseMandatoryLocation(this,
                                                        out var countryCode,
                                                        out var partyId,
                                                        out var locationId,
                                                        out var location,
                                                        out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = location.ToJSON(
                                                          request.EMSPId,
                                                          CustomLocationSerializer,
                                                          CustomPublishTokenSerializer,
                                                          CustomAdditionalGeoLocationSerializer,
                                                          CustomEVSESerializer,
                                                          CustomStatusScheduleSerializer,
                                                          CustomConnectorSerializer,
                                                          CustomLocationEnergyMeterSerializer,
                                                          CustomEVSEEnergyMeterSerializer,
                                                          CustomTransparencySoftwareStatusSerializer,
                                                          CustomTransparencySoftwareSerializer,
                                                          CustomDisplayTextSerializer,
                                                          CustomBusinessDetailsSerializer,
                                                          CustomHoursSerializer,
                                                          CustomImageSerializer,
                                                          CustomEnergyMixSerializer,
                                                          CustomEnergySourceSerializer,
                                                          CustomEnvironmentalImpactSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = location.LastUpdated,
                                   ETag                       = location.ETag
                               }
                        });

                }
            );

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                PutLocationRequest,
                PutLocationResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing location

                    if (!request.ParseOptionalLocation(this,
                                                       out var countryCode,
                                                       out var partyId,
                                                       out var locationId,
                                                       out var existingLocation,
                                                       out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated location JSON

                    if (!request.TryParseJObjectRequestBody(out var locationJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Location.TryParse(locationJSON,
                                           out var newOrUpdatedLocation,
                                           out var errorResponse,
                                           countryCode,
                                           partyId,
                                           locationId))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given location JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    var addOrUpdateResult = await CommonAPI.AddOrUpdateLocation(
                                                      newOrUpdatedLocation,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var locationData))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = locationData.ToJSON(
                                                              request.EMSPId,
                                                              CustomLocationSerializer,
                                                              CustomPublishTokenSerializer,
                                                              CustomAdditionalGeoLocationSerializer,
                                                              CustomEVSESerializer,
                                                              CustomStatusScheduleSerializer,
                                                              CustomConnectorSerializer,
                                                              CustomLocationEnergyMeterSerializer,
                                                              CustomEVSEEnergyMeterSerializer,
                                                              CustomTransparencySoftwareStatusSerializer,
                                                              CustomTransparencySoftwareSerializer,
                                                              CustomDisplayTextSerializer,
                                                              CustomBusinessDetailsSerializer,
                                                              CustomHoursSerializer,
                                                              CustomImageSerializer,
                                                              CustomEnergyMixSerializer,
                                                              CustomEnergySourceSerializer,
                                                              CustomEnvironmentalImpactSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = locationData.LastUpdated,
                                       ETag                       = locationData.ETag
                                   }
                               };

                    }

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedLocation.ToJSON(
                                                          request.EMSPId,
                                                          CustomLocationSerializer,
                                                          CustomPublishTokenSerializer,
                                                          CustomAdditionalGeoLocationSerializer,
                                                          CustomEVSESerializer,
                                                          CustomStatusScheduleSerializer,
                                                          CustomConnectorSerializer,
                                                          CustomLocationEnergyMeterSerializer,
                                                          CustomEVSEEnergyMeterSerializer,
                                                          CustomTransparencySoftwareStatusSerializer,
                                                          CustomTransparencySoftwareSerializer,
                                                          CustomDisplayTextSerializer,
                                                          CustomBusinessDetailsSerializer,
                                                          CustomHoursSerializer,
                                                          CustomImageSerializer,
                                                          CustomEnergyMixSerializer,
                                                          CustomEnergySourceSerializer,
                                                          CustomEnvironmentalImpactSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                PatchLocationRequest,
                PatchLocationResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check location

                    if (!request.ParseMandatoryLocation(this,
                                                        out var countryCode,
                                                        out var partyId,
                                                        out var locationId,
                                                        out var existingLocation,
                                                        out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse location JSON patch

                    if (!request.TryParseJObjectRequestBody(out var locationPatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    // Validation-Checks for PATCHes
                    // (E-Tag, Timestamp, ...)

                    var patchedLocation = await CommonAPI.TryPatchLocation(
                                                    Party_Idv3.From(
                                                        countryCode.Value,
                                                        partyId.    Value
                                                    ),
                                                    locationId.Value,
                                                    locationPatch
                                                );


                    //ToDo: Handle update errors!
                    if (patchedLocation.IsSuccessAndDataNotNull(out var locationData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = locationData.ToJSON(
                                                                  request.EMSPId,
                                                                  CustomLocationSerializer,
                                                                  CustomPublishTokenSerializer,
                                                                  CustomAdditionalGeoLocationSerializer,
                                                                  CustomEVSESerializer,
                                                                  CustomStatusScheduleSerializer,
                                                                  CustomConnectorSerializer,
                                                                  CustomLocationEnergyMeterSerializer,
                                                                  CustomEVSEEnergyMeterSerializer,
                                                                  CustomTransparencySoftwareStatusSerializer,
                                                                  CustomTransparencySoftwareSerializer,
                                                                  CustomDisplayTextSerializer,
                                                                  CustomBusinessDetailsSerializer,
                                                                  CustomHoursSerializer,
                                                                  CustomImageSerializer,
                                                                  CustomEnergyMixSerializer,
                                                                  CustomEnergySourceSerializer,
                                                                  CustomEnvironmentalImpactSerializer
                                                              ),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = locationData.LastUpdated,
                                           ETag                       = locationData.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedLocation.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                }
            );

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}",
                DeleteLocationRequest,
                DeleteLocationResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing location

                    if (!request.ParseMandatoryLocation(this,
                                                        out var countryCode,
                                                        out var partyId,
                                                        out var locationId,
                                                        out var location,
                                                        out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //ToDo: await...
                    var result = await CommonAPI.RemoveLocation(location);


                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = location.ToJSON(
                                                              request.EMSPId,
                                                              CustomLocationSerializer,
                                                              CustomPublishTokenSerializer,
                                                              CustomAdditionalGeoLocationSerializer,
                                                              CustomEVSESerializer,
                                                              CustomStatusScheduleSerializer,
                                                              CustomConnectorSerializer,
                                                              CustomLocationEnergyMeterSerializer,
                                                              CustomEVSEEnergyMeterSerializer,
                                                              CustomTransparencySoftwareStatusSerializer,
                                                              CustomTransparencySoftwareSerializer,
                                                              CustomDisplayTextSerializer,
                                                              CustomBusinessDetailsSerializer,
                                                              CustomHoursSerializer,
                                                              CustomImageSerializer,
                                                              CustomEnergyMixSerializer,
                                                              CustomEnergySourceSerializer,
                                                              CustomEnvironmentalImpactSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = location.LastUpdated,
                                       ETag                       = location.ETag
                                   }
                               };

                }
            );

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                GetEVSERequest,
                GetEVSEResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check EVSE

                    if (!request.ParseMandatoryLocationEVSE(this,
                                                            out var countryCode,
                                                            out var partyId,
                                                            out var locationId,
                                                            out var location,
                                                            out var evseUId,
                                                            out var evse,
                                                            out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = evse.ToJSON(
                                                          request.EMSPId,
                                                          CustomEVSESerializer,
                                                          CustomStatusScheduleSerializer,
                                                          CustomConnectorSerializer,
                                                          CustomEVSEEnergyMeterSerializer,
                                                          CustomTransparencySoftwareStatusSerializer,
                                                          CustomTransparencySoftwareSerializer,
                                                          CustomDisplayTextSerializer,
                                                          CustomImageSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = evse.LastUpdated,
                                   ETag                       = evse.ETag
                               }
                        });

                }
            );

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                PutEVSERequest,
                PutEVSEResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing EVSE

                    if (!request.ParseOptionalLocationEVSE(this,
                                                           out var countryCode,
                                                           out var partyId,
                                                           out var locationId,
                                                           out var existingLocation,
                                                           out var evseUId,
                                                           out var existingEVSE,
                                                           out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated EVSE JSON

                    if (!request.TryParseJObjectRequestBody(out var evseJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!EVSE.TryParse(evseJSON,
                                       out var newOrUpdatedEVSE,
                                       out var errorResponse,
                                       evseUId))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given EVSE JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    var addOrUpdateResult = await CommonAPI.AddOrUpdateEVSE(
                                                      existingLocation,
                                                      newOrUpdatedEVSE,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var evseData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = evseData.ToJSON(
                                                              request.EMSPId,
                                                              CustomEVSESerializer,
                                                              CustomStatusScheduleSerializer,
                                                              CustomConnectorSerializer,
                                                              CustomEVSEEnergyMeterSerializer,
                                                              CustomTransparencySoftwareStatusSerializer,
                                                              CustomTransparencySoftwareSerializer,
                                                              CustomDisplayTextSerializer,
                                                              CustomImageSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = evseData.LastUpdated,
                                       ETag                       = evseData.ETag
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedEVSE.ToJSON(
                                                          request.EMSPId,
                                                          CustomEVSESerializer,
                                                          CustomStatusScheduleSerializer,
                                                          CustomConnectorSerializer,
                                                          CustomEVSEEnergyMeterSerializer,
                                                          CustomTransparencySoftwareStatusSerializer,
                                                          CustomTransparencySoftwareSerializer,
                                                          CustomDisplayTextSerializer,
                                                          CustomImageSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedEVSE.LastUpdated,
                                   ETag                       = newOrUpdatedEVSE.ETag
                               }
                           };

                });

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                PatchEVSERequest,
                PatchEVSEResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check EVSE

                    if (!request.ParseMandatoryLocationEVSE(this,
                                                            out var countryCode,
                                                            out var partyId,
                                                            out var locationId,
                                                            out var existingLocation,
                                                            out var evseUId,
                                                            out var existingEVSE,
                                                            out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse EVSE JSON patch

                    if (!request.TryParseJObjectRequestBody(out var evsePatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    var patchedEVSE = await CommonAPI.TryPatchEVSE(
                                                existingLocation,
                                                existingEVSE,
                                                evsePatch
                                            );

                    //ToDo: Handle update errors!
                    if (patchedEVSE.IsSuccessAndDataNotNull(out var evseData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = evseData.ToJSON(
                                                                  request.EMSPId,
                                                                  CustomEVSESerializer,
                                                                  CustomStatusScheduleSerializer,
                                                                  CustomConnectorSerializer,
                                                                  CustomEVSEEnergyMeterSerializer,
                                                                  CustomTransparencySoftwareStatusSerializer,
                                                                  CustomTransparencySoftwareSerializer,
                                                                  CustomDisplayTextSerializer,
                                                                  CustomImageSerializer
                                                              ),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = evseData.LastUpdated,
                                           ETag                       = evseData.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedEVSE.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                }
            );

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}",
                DeleteEVSERequest,
                DeleteEVSEResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing Location/EVSE(UId URI parameter)

                    if (!request.ParseMandatoryLocationEVSE(this,
                                                            out var countryCode,
                                                            out var partyId,
                                                            out var locationId,
                                                            out var existingLocation,
                                                            out var evseUId,
                                                            out var existingEVSE,
                                                            out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = existingEVSE.ToJSON(
                                                              request.EMSPId,
                                                              CustomEVSESerializer,
                                                              CustomStatusScheduleSerializer,
                                                              CustomConnectorSerializer,
                                                              CustomEVSEEnergyMeterSerializer,
                                                              CustomTransparencySoftwareStatusSerializer,
                                                              CustomTransparencySoftwareSerializer,
                                                              CustomDisplayTextSerializer,
                                                              CustomImageSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = existingEVSE.LastUpdated,
                                       ETag                       = existingEVSE.ETag
                                   }
                               };

                }
            );

            #endregion

            #endregion

            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                GetConnectorRequest,
                GetConnectorResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check connector

                    if (!request.ParseMandatoryLocationEVSEConnector(this,
                                                                     out var countryCode,
                                                                     out var partyId,
                                                                     out var locationId,
                                                                     out var location,
                                                                     out var evseId,
                                                                     out var evse,
                                                                     out var connectorId,
                                                                     out var connector,
                                                                     out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = connector.ToJSON(
                                                          true,
                                                          true,
                                                          request.EMSPId,
                                                          CustomConnectorSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = connector.LastUpdated,
                                   ETag                       = connector.ETag
                               }
                        });

                }
            );

            #endregion

            #region PUT      ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                PutConnectorRequest,
                PutConnectorResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check connector

                    if (!request.ParseOptionalLocationEVSEConnector(this,
                                                                    out var countryCode,
                                                                    out var partyId,
                                                                    out var locationId,
                                                                    out var existingLocation,
                                                                    out var evseUId,
                                                                    out var existingEVSE,
                                                                    out var connectorId,
                                                                    out var existingConnector,
                                                                    out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated connector JSON

                    if (!request.TryParseJObjectRequestBody(out var connectorJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Connector.TryParse(connectorJSON,
                                            out var newOrUpdatedConnector,
                                            out var errorResponse,
                                            connectorId))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given connector JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    var addOrUpdateResult = await CommonAPI.AddOrUpdateConnector(
                                                      existingLocation,
                                                      existingEVSE,
                                                      newOrUpdatedConnector,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var connectorData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = connectorData.ToJSON(
                                                              true,
                                                              true,
                                                              request.EMSPId,
                                                              CustomConnectorSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = newOrUpdatedConnector.LastUpdated,
                                       ETag                       = newOrUpdatedConnector.ETag
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedConnector.ToJSON(
                                                          true,
                                                          true,
                                                          request.EMSPId,
                                                          CustomConnectorSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedConnector.LastUpdated,
                                   ETag                       = newOrUpdatedConnector.ETag
                               }
                           };

                }
            );

            #endregion

            #region PATCH    ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                PatchConnectorRequest,
                PatchConnectorResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check connector

                    if (!request.ParseMandatoryLocationEVSEConnector(this,
                                                                     out var countryCode,
                                                                     out var partyId,
                                                                     out var locationId,
                                                                     out var existingLocation,
                                                                     out var evseUId,
                                                                     out var existingEVSE,
                                                                     out var connectorId,
                                                                     out var existingConnector,
                                                                     out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse connector JSON patch

                    if (!request.TryParseJObjectRequestBody(out var connectorPatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    var patchedConnector = await CommonAPI.TryPatchConnector(
                                                     existingLocation,
                                                     existingEVSE,
                                                     existingConnector,
                                                     connectorPatch
                                                 );

                    //ToDo: Handle update errors!
                    if (patchedConnector.IsSuccessAndDataNotNull(out var connectorData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = connectorData.ToJSON(
                                                                  true,
                                                                  true,
                                                                  request.EMSPId,
                                                                  CustomConnectorSerializer
                                                              ),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = connectorData.LastUpdated,
                                           ETag                       = connectorData.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedConnector.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                }
            );

            #endregion

            #region DELETE   ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/{connectorId}",
                DeleteConnectorRequest,
                DeleteConnectorResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing Location/EVSE/Connector(UId URI parameter)

                    if (!request.ParseMandatoryLocationEVSEConnector(this,
                                                                     out var countryCode,
                                                                     out var partyId,
                                                                     out var locationId,
                                                                     out var existingLocation,
                                                                     out var evseUId,
                                                                     out var existingEVSE,
                                                                     out var connectorId,
                                                                     out var existingConnector,
                                                                     out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //CommonAPI.Remove(ExistingLocation, ExistingEVSE);


                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = existingConnector.ToJSON(
                                                              true,
                                                              true,
                                                              request.EMSPId,
                                                              CustomConnectorSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = existingConnector.LastUpdated,
                                       ETag                       = existingConnector.ETag
                                   }
                               };

                }
            );

            #endregion

            #endregion


            #region ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/status  [NonStandard]

            #region OPTIONS  ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/status",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST ],
                                AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/locations/{country_code}/{party_id}/{locationId}/{evseUId}/status     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "locations/{country_code}/{party_id}/{locationId}/{evseUId}/status",
                PutEVSERequest,
                PutEVSEResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing EVSE

                    if (!request.ParseMandatoryLocationEVSE(this,
                                                            out var countryCode,
                                                            out var partyId,
                                                            out var locationId,
                                                            out var existingLocation,
                                                            out var evseUId,
                                                            out var existingEVSE,
                                                            out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse EVSE status JSON

                    if (!request.TryParseJObjectRequestBody(out var evseStatusJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    //ToDo: Handle AddOrUpdate errors


                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   //Data                 = newOrUpdatedEVSE.ToJSON(),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                }
            );

            #endregion

            #endregion



            #region ~/sessions/{country_code}/{party_id}                                [NonStandard]

            #region OPTIONS  ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{country_code}/{party_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.DELETE ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/sessions                                          [NonStandard]

            // Return all charging session for the given access token roles

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions",
                GetSessionsRequest,
                GetSessionsResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion


                    var filters           = request.GetDateAndPaginationFilters();

                    var allSessions       = CommonAPI.GetSessions(session => request.LocalAccessInfo.Roles.Any(role => role.PartyId.CountryCode == session.CountryCode &&
                                                                                                                       role.PartyId.PartyId       == session.PartyId)).
                                                      ToArray();

                    var filteredSessions  = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                        Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                        ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredSessions.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(session => session.ToJSON(
                                                                                    CustomSessionSerializer,
                                                                                    CustomCDRTokenSerializer,
                                                                                    CustomChargingPeriodSerializer,
                                                                                    CustomCDRDimensionSerializer,
                                                                                    CustomPriceSerializer
                                                                                ))
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allSessions.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region GET      ~/sessions/{country_code}/{party_id}                [NonStandard]

            // Return all charging session for the given country code and party identification

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions/{country_code}/{party_id}",
                GetSessionsRequest,
                GetSessionsResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    var filters           = request.GetDateAndPaginationFilters();

                    var allSessions       = CommonAPI.GetSessions(partyId.Value).ToArray();

                    var filteredSessions  = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                        Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                        ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredSessions.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(session => session.ToJSON(
                                                                                    CustomSessionSerializer,
                                                                                    CustomCDRTokenSerializer,
                                                                                    CustomChargingPeriodSerializer,
                                                                                    CustomCDRDimensionSerializer,
                                                                                    CustomPriceSerializer
                                                                                ))
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allSessions.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region DELETE   ~/sessions                                          [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "sessions",
                DeleteSessionsRequest,
                DeleteSessionsResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    foreach (var role in request.LocalAccessInfo.Roles)
                        await CommonAPI.RemoveAllSessions(role.PartyId);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #region DELETE   ~/sessions/{country_code}/{party_id}                [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "sessions/{country_code}/{party_id}",
                DeleteSessionsRequest,
                DeleteSessionsResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    var result = await CommonAPI.RemoveAllSessions(partyId.Value);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #endregion

            #region ~/sessions/{country_code}/{party_id}/{session_id}

            #region OPTIONS  ~/sessions/{country_code}/{party_id}/{session_id}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/sessions/{country_code}/{party_id}/{session_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                GetSessionRequest,
                GetSessionResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check session

                    if (!request.ParseMandatorySession(this,
                                                       out var countryCode,
                                                       out var partyId,
                                                       out var session_id,
                                                       out var session,
                                                       out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            StatusCode           = 1000,
                            StatusMessage        = "Hello world!",
                            Data                 = session.ToJSON(
                                                       CustomSessionSerializer,
                                                       CustomCDRTokenSerializer,
                                                       CustomChargingPeriodSerializer,
                                                       CustomCDRDimensionSerializer,
                                                       CustomPriceSerializer
                                                   ),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                LastModified               = session.LastUpdated,
                                ETag                       = session.ETag
                            }
                        });

                }
            );

            #endregion

            #region PUT      ~/sessions/{country_code}/{party_id}/{session_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                PutSessionRequest,
                PutSessionResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Parse session identification

                    if (!request.ParseSessionId(CommonAPI,
                                                out var countryCode,
                                                out var partyId,
                                                out var session_id,
                                                out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated session

                    if (!request.TryParseJObjectRequestBody(out var sessionJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Session.TryParse(sessionJSON,
                                          out var newOrUpdatedSession,
                                          out var errorResponse,
                                          countryCode,
                                          partyId,
                                          session_id))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given session JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    var addOrUpdateResult = await CommonAPI.AddOrUpdateSession(
                                                      newOrUpdatedSession,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var sessionData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = sessionData.ToJSON(
                                                              CustomSessionSerializer,
                                                              CustomCDRTokenSerializer,
                                                              CustomChargingPeriodSerializer,
                                                              CustomCDRDimensionSerializer,
                                                              CustomPriceSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = sessionData.LastUpdated,
                                       ETag                       = sessionData.ETag
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedSession.ToJSON(
                                                          CustomSessionSerializer,
                                                          CustomCDRTokenSerializer,
                                                          CustomChargingPeriodSerializer,
                                                          CustomCDRDimensionSerializer,
                                                          CustomPriceSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedSession.LastUpdated,
                                   ETag                       = newOrUpdatedSession.ETag
                               }
                           };

                }
            );

            #endregion

            #region PATCH    ~/sessions/{country_code}/{party_id}/{session_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                PatchSessionRequest,
                PatchSessionResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check session

                    if (!request.ParseMandatorySession(this,
                                                       out var countryCode,
                                                       out var partyId,
                                                       out var session_id,
                                                       out var existingSession,
                                                       out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse and apply Session JSON patch

                    if (!request.TryParseJObjectRequestBody(out var sessionPatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    var patchedSession = await CommonAPI.TryPatchSession(
                                                   Party_Idv3.From(
                                                       countryCode.Value,
                                                       partyId.    Value
                                                   ),
                                                   existingSession.Id,
                                                   sessionPatch
                                               );


                    //ToDo: Handle update errors!
                    if (patchedSession.IsSuccessAndDataNotNull(out var patchedSessionData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = patchedSessionData.ToJSON(
                                                                  CustomSessionSerializer,
                                                                  CustomCDRTokenSerializer,
                                                                  CustomChargingPeriodSerializer,
                                                                  CustomCDRDimensionSerializer,
                                                                  CustomPriceSerializer
                                                              ),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = patchedSessionData.LastUpdated,
                                           ETag                       = patchedSessionData.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedSession.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                }
            );

            #endregion

            #region DELETE   ~/sessions/{country_code}/{party_id}/{session_id}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "sessions/{country_code}/{party_id}/{session_id}",
                DeleteSessionRequest,
                DeleteSessionResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing session

                    if (!request.ParseMandatorySession(this,
                                                       out var countryCode,
                                                       out var partyId,
                                                       out var session_id,
                                                       out var existingSession,
                                                       out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //ToDo: await...
                    var result = await CommonAPI.RemoveSession(existingSession);


                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = existingSession.ToJSON(
                                                              CustomSessionSerializer,
                                                              CustomCDRTokenSerializer,
                                                              CustomChargingPeriodSerializer,
                                                              CustomCDRDimensionSerializer,
                                                              CustomPriceSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                       //LastModified               = Timestamp.Now.ToISO8601()
                                   }
                               };

                }
            );

            #endregion

            #endregion



            // For EMSPs and SCSPs
            #region POST  ~/chargingprofiles/{session_id}

            // https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/mod_charging_profiles.asciidoc


            #region POST  ~/chargingprofiles/{session_id}/activeChargingProfile

            // ActiveChargingProfileResult
            // Result of the GET ActiveChargingProfile request, from the Charge Point.

            #endregion

            #region PUT   ~/chargingprofiles/{session_id}/activeChargingProfile

            // ActiveChargingProfile update

            #endregion


            #region POST  ~/chargingprofiles/{session_id}/chargingProfile

            // ChargingProfileResult
            // Result of the PUT ChargingProfile request, from the Charge Point.

            #endregion

            #region POST  ~/chargingprofiles/{session_id}/clearProfile

            // ClearProfileResult
            // Result of the DELETE ChargingProfile request, from the Charge Point.

            #endregion

            #endregion


        }

        #endregion


        #region (private) LogEvent (Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OICPCommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(SCSP_HTTPAPI),
                   Logger,
                   LogHandler,
                   EventName,
                   OICPCommand
               );

        #endregion


    }

}
