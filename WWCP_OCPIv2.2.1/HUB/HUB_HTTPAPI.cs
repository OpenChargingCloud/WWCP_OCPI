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
using cloud.charging.open.protocols.OCPIv2_2_1.HUB.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    public static class HUB_HTTPAPI_Extensions
    {

        #region ParseMandatoryLocation              (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The EMSP HTTP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocation(this OCPIRequest                                Request,
                                                     HUB_HTTPAPI                                    HUB_HTTPAPI,
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


            if (!HUB_HTTPAPI.TryGetRemoteLocation(
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

        #region ParseOptionalLocation               (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The EMSP HTTP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocation(this OCPIRequest                                 Request,
                                                    HUB_HTTPAPI                                     HUB_HTTPAPI,
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


            HUB_HTTPAPI.TryGetRemoteLocation(
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


        #region ParseMandatoryLocationEVSE          (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocationEVSE(this OCPIRequest                                Request,
                                                         HUB_HTTPAPI                                    HUB_HTTPAPI,
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


            if (!HUB_HTTPAPI.TryGetRemoteLocation(
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

        #region ParseOptionalLocationEVSE           (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocationEVSE(this OCPIRequest                                 Request,
                                                        HUB_HTTPAPI                                     HUB_HTTPAPI,
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


            HUB_HTTPAPI.TryGetRemoteLocation(
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


        #region ParseMandatoryLocationEVSEConnector (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
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
                                                                  HUB_HTTPAPI                                    HUB_HTTPAPI,
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


            if (!HUB_HTTPAPI.TryGetRemoteLocation(
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

        #region ParseOptionalLocationEVSEConnector  (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
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
                                                                 HUB_HTTPAPI                                     HUB_HTTPAPI,
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


            if (!HUB_HTTPAPI.TryGetRemoteLocation(
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


        #region ParseMandatoryTariff                (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out TariffId,   out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryTariff(this OCPIRequest                                Request,
                                                   HUB_HTTPAPI                                    HUB_HTTPAPI,
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


            if (!HUB_HTTPAPI.TryGetRemoteTariff(
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

        #region ParseOptionalTariff                 (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out TariffId,   out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalTariff(this OCPIRequest                                  Request,
                                                  HUB_HTTPAPI                                      HUB_HTTPAPI,
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


            HUB_HTTPAPI.TryGetRemoteTariff(
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


        #region ParseMandatorySession               (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out SessionId,  out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatorySession(this OCPIRequest                                Request,
                                                    HUB_HTTPAPI                                    HUB_HTTPAPI,
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


            if (!HUB_HTTPAPI.TryGetRemoteSession(Party_Idv3.From(countryCode, partyId), sessionId, out Session))
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

        #region ParseOptionalSession                (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out SessionId,  out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalSession(this OCPIRequest                                  Request,
                                                   HUB_HTTPAPI                                      HUB_HTTPAPI,
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


            HUB_HTTPAPI.TryGetRemoteSession(
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


        #region ParseMandatoryCDR                   (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out CDRId,      out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryCDR(this OCPIRequest                                Request,
                                                HUB_HTTPAPI                                    HUB_HTTPAPI,
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


            if (!HUB_HTTPAPI.TryGetRemoteCDR(
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

        #region ParseOptionalCDR                    (this Request, HUB_HTTPAPI, out CountryCode, out PartyId, out CDRId,      out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="HUB_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalCDR(this OCPIRequest                                  Request,
                                               HUB_HTTPAPI                                      HUB_HTTPAPI,
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


            HUB_HTTPAPI.TryGetRemoteCDR(
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
    /// The HTTP API for EV roaming hubs.
    /// CPOS and EMSPs will connect to this API.
    /// </summary>
    public class HUB_HTTPAPI : AHTTPExtAPIXExtension2<CommonAPI, HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName   = $"GraphDefined OCPI {Version.String} HUB HTTP API";

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public     static readonly HTTPPath  DefaultURLPathPrefix     = HTTPPath.Parse($"{Version.String}/hub/");

        /// <summary>
        /// The default HUB API logfile name.
        /// </summary>
        public     const           String    DefaultLogfileName       = $"OCPI{Version.String}_HUBAPI.log";

        #endregion

        #region Properties

        /// <summary>
        /// The OCPI CommonAPI.
        /// </summary>
        public CommonAPI            CommonAPI
            => HTTPBaseAPI;

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2.1 does not define any behaviour for this.
        /// </summary>
        public Boolean?             AllowDowngrades    { get; }

        /// <summary>
        /// The HUB HTTP API logger.
        /// </summary>
        public HUB_HTTPAPI_Logger?  HTTPLogger         { get; set; }

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
        public CustomJObjectSerializerDelegate<CommandResponse>?             CustomCommandResponseSerializer               { get; set; }


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

        public CustomJObjectSerializerDelegate<AuthorizationInfo>?           CustomAuthorizationInfoSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<LocationReference>?           CustomLocationReferenceSerializer             { get; set; }

        #endregion

        #region Events

        #region CPO

        public CPO_HTTPAPI.HTTP_Events  CPOEvents      { get; } = new CPO_HTTPAPI.HTTP_Events();


        // Commands

        #region OnReserveNowCommand

        public delegate Task<CommandResponse> OnReserveNowCommandDelegate(EMSP_Id            EMSPId,
                                                                          ReserveNowCommand  ReserveNowCommand);

        public event OnReserveNowCommandDelegate? OnReserveNowCommand;

        #endregion

        #region OnCancelReservationCommand

        public delegate Task<CommandResponse> OnCancelReservationCommandDelegate(EMSP_Id                   EMSPId,
                                                                                 CancelReservationCommand  CancelReservationCommand);

        public event OnCancelReservationCommandDelegate? OnCancelReservationCommand;

        #endregion

        #region OnStartSessionCommand

        public delegate Task<CommandResponse> OnStartSessionCommandDelegate(EMSP_Id              EMSPId,
                                                                            StartSessionCommand  StartSessionCommand);

        public event OnStartSessionCommandDelegate? OnStartSessionCommand;

        #endregion

        #region OnStopSessionCommand

        public delegate Task<CommandResponse> OnStopSessionCommandDelegate(EMSP_Id             EMSPId,
                                                                           StopSessionCommand  StopSessionCommand);

        public event OnStopSessionCommandDelegate? OnStopSessionCommand;

        #endregion

        #region OnUnlockConnectorCommand

        public delegate Task<CommandResponse> OnUnlockConnectorCommandDelegate(EMSP_Id                 EMSPId,
                                                                               UnlockConnectorCommand  UnlockConnectorCommand);

        public event OnUnlockConnectorCommandDelegate? OnUnlockConnectorCommand;

        #endregion

        #endregion

        #region EMSP

        public EMSP_HTTPAPI.HTTP_Events  EMSPEvents    { get; } = new EMSP_HTTPAPI.HTTP_Events();



        public delegate Task<AuthorizationInfo> OnRFIDAuthTokenDelegate(CountryCode         From_CountryCode,
                                                                        Party_Id            From_PartyId,
                                                                        CountryCode         To_CountryCode,
                                                                        Party_Id            To_PartyId,
                                                                        Token_Id            TokenId,
                                                                        LocationReference?  LocationReference);

        public event OnRFIDAuthTokenDelegate? OnRFIDAuthToken;



        public async Task<AuthorizationInfo?> RFIDAuthToken(CountryCode         From_CountryCode,
                                                            Party_Id            From_PartyId,
                                                            CountryCode         To_CountryCode,
                                                            Party_Id            To_PartyId,
                                                            Token_Id            TokenId,
                                                            LocationReference?  LocationReference)

            => OnRFIDAuthToken is not null

                    ? await OnRFIDAuthToken.Invoke(
                                From_CountryCode,
                                From_PartyId,
                                To_CountryCode,
                                To_PartyId,
                                TokenId,
                                LocationReference
                            )

                    : null;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the HTTP API for charge point operators
        /// using the given CommonAPI.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public HUB_HTTPAPI(CommonAPI                    CommonAPI,
                           I18NString?                  Description          = null,
                           Boolean?                     AllowDowngrades      = null,

                           HTTPPath?                    BasePath             = null,
                           HTTPPath?                    URLPathPrefix        = null,

                           String?                      ExternalDNSName      = null,
                           String?                      HTTPServerName       = DefaultHTTPServerName,
                           String?                      HTTPServiceName      = DefaultHTTPServiceName,
                           String?                      APIVersionHash       = null,
                           JObject?                     APIVersionHashes     = null,

                           Boolean?                     IsDevelopment        = false,
                           IEnumerable<String>?         DevelopmentServers   = null,
                           Boolean?                     DisableLogging       = false,
                           String?                      LoggingContext       = null,
                           String?                      LoggingPath          = null,
                           String?                      LogfileName          = null,
                           OCPILogfileCreatorDelegate?  LogfileCreator       = null)

            : base(CommonAPI,
                   CommonAPI.URLPathPrefix + (URLPathPrefix ?? DefaultURLPathPrefix),
                   BasePath,

                   Description     ?? I18NString.Create($"OCPI{Version.String} HUB HTTP API"),

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

            this.AllowDowngrades  = AllowDowngrades;

            this.HTTPLogger       = this.DisableLogging == false
                                        ? new HUB_HTTPAPI_Logger(
                                              this,
                                              LoggingContext,
                                              LoggingPath,
                                              LogfileCreator
                                          )
                                        : null;

            RegisterURLTemplates();

        }

        #endregion


        #region HUB-2-CPO  Clients

        private readonly ConcurrentDictionary<CPO_Id, HUB2CPOClient> hub2cpoClients = new();

        /// <summary>
        /// Return an enumeration of all HUB2CPO clients.
        /// </summary>
        public IEnumerable<HUB2CPOClient> HUB2CPOClients
            => hub2cpoClients.Values;


        #region GetCPOClient (CountryCode, PartyId,   Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a HUB create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote CPO.</param>
        /// <param name="PartyId">The party identification of the remote CPO.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached HUB clients.</param>
        public HUB2CPOClient? GetCPOClient(CountryCode  CountryCode,
                                           Party_Id     PartyId,
                                           I18NString?  Description          = null,
                                           Boolean      AllowCachedClients   = true)

            => GetCPOClient(
                   RemoteParty_Id.From(
                       CountryCode,
                       PartyId,
                       Role.CPO
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetCPOClient (             PartyIdv3, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a HUB create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="v">The party identification of the remote CPO.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached HUB clients.</param>
        public HUB2CPOClient? GetCPOClient(Party_Idv3   PartyIdv3,
                                           I18NString?  Description          = null,
                                           Boolean      AllowCachedClients   = true)

            => GetCPOClient(
                   RemoteParty_Id.From(
                       PartyIdv3,
                       Role.CPO
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetCPOClient (RemoteParty,            Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a HUB create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached HUB clients.</param>
        public HUB2CPOClient? GetCPOClient(RemoteParty  RemoteParty,
                                           I18NString?  Description          = null,
                                           Boolean      AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                hub2cpoClients.TryGetValue(cpoId, out var cachedHUBClient))
            {
                return cachedHUBClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var hub2CPOClient = new HUB2CPOClient(
                                         this,
                                         RemoteParty,
                                         null,
                                         Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                         null,
                                         CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                         CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                         CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                         CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                         CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                     );

                hub2cpoClients.TryAdd(cpoId, hub2CPOClient);

                return hub2CPOClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient (RemotePartyId,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a HUB create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached HUB clients.</param>
        public HUB2CPOClient? GetCPOClient(RemoteParty_Id  RemotePartyId,
                                           I18NString?     Description          = null,
                                           Boolean         AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                hub2cpoClients.TryGetValue(cpoId, out var cachedHUBClient))
            {
                return cachedHUBClient;
            }

            if (CommonAPI.TryGetRemoteParty(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var hub2CPOClient = new HUB2CPOClient(
                                         this,
                                         remoteParty,
                                         null,
                                         Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemotePartyId),
                                         null,
                                         CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                         CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                         CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                         CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                         CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                     );

                hub2cpoClients.TryAdd(cpoId, hub2CPOClient);

                return hub2CPOClient;

            }

            return null;

        }

        #endregion

        #endregion

        #region HUB-2-EMSP Clients

        private readonly ConcurrentDictionary<EMSP_Id, HUB2EMSPClient> hub2emspClients = new();

        /// <summary>
        /// Return an enumeration of all HUB2EMSP clients.
        /// </summary>
        public IEnumerable<HUB2EMSPClient> HUB2EMSPClients
            => hub2emspClients.Values;


        #region GetEMSPClient (CountryCode, PartyId,   Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a HUB create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached HUB clients.</param>
        public HUB2EMSPClient? GetEMSPClient(CountryCode  CountryCode,
                                             Party_Id     PartyId,
                                             I18NString?  Description          = null,
                                             Boolean      AllowCachedClients   = true)

            => GetEMSPClient(
                   RemoteParty_Id.From(
                       CountryCode,
                       PartyId,
                       Role.EMSP
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetEMSPClient (             PartyIdv3, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a HUB create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="v">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached HUB clients.</param>
        public HUB2EMSPClient? GetEMSPClient(Party_Idv3   PartyIdv3,
                                             I18NString?  Description          = null,
                                             Boolean      AllowCachedClients   = true)

            => GetEMSPClient(
                   RemoteParty_Id.From(
                       PartyIdv3,
                       Role.EMSP
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetEMSPClient (RemoteParty,            Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a HUB create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached HUB clients.</param>
        public HUB2EMSPClient? GetEMSPClient(RemoteParty  RemoteParty,
                                             I18NString?  Description          = null,
                                             Boolean      AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                hub2emspClients.TryGetValue(emspId, out var cachedHUBClient))
            {
                return cachedHUBClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var hub2EMSPClient = new HUB2EMSPClient(
                                         this,
                                         RemoteParty,
                                         null,
                                         Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                         null,
                                         CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                         CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                         CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                         CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                         CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                     );

                hub2emspClients.TryAdd(emspId, hub2EMSPClient);

                return hub2EMSPClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient (RemotePartyId,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a HUB create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached HUB clients.</param>
        public HUB2EMSPClient? GetEMSPClient(RemoteParty_Id  RemotePartyId,
                                             I18NString?     Description          = null,
                                             Boolean         AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                hub2emspClients.TryGetValue(emspId, out var cachedHUBClient))
            {
                return cachedHUBClient;
            }

            if (CommonAPI.TryGetRemoteParty(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var hub2EMSPClient = new HUB2EMSPClient(
                                         this,
                                         remoteParty,
                                         null,
                                         Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemotePartyId),
                                         null,
                                         CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                         CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                         CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                         CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                         CommonAPI.HTTPBaseAPI.HTTPServer.DNSClient
                                     );

                hub2emspClients.TryAdd(emspId, hub2EMSPClient);

                return hub2EMSPClient;

            }

            return null;

        }

        #endregion

        #endregion

        #region CloseAllClients()

        public Task CloseAllClients()
        {

            foreach (var client in hub2cpoClients.Values)
            {
                client.Close();
            }

            hub2cpoClients.Clear();


            foreach (var client in hub2emspClients.Values)
            {
                client.Close();
            }

            hub2emspClients.Clear();

            return Task.CompletedTask;

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

            //- AsCPO API endpoints ----------------------------------------------------------------------

            #region ~/locations

            #region OPTIONS  ~/locations

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations

            // https://example.com/ocpi/2.2/hub/locations/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations",
                CPOEvents.GetLocationsHTTPRequest,
                CPOEvents.GetLocationsHTTPResponse,
                request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.LocationsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status            != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo.IsNot(Role.EMSP) == true))
                    {


                    //if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                    //    Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    //{

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET", "DELETE" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                                }
                            });

                    }

                    #endregion


                    //var emspId               = Request.LocalAccessInfo is not null
                    //                               ? EMSP_Id.Parse(Request.LocalAccessInfo.CountryCode, Request.LocalAccessInfo.PartyId)
                    //                               : new EMSP_Id?();

                    var withExtensions       = request.QueryString.GetBoolean ("withExtensions", false);


                    var filters              = request.GetDateAndPaginationFilters();
                    var matchFilter          = request.QueryString.CreateStringFilter<Location>(
                                                   "match",
                                                   (location, pattern) => location.Id.     ToString().Contains(pattern)         ||
                                                                          location.Name?.             Contains(pattern) == true ||
                                                                          location.Address.           Contains(pattern)         ||
                                                                          location.City.              Contains(pattern)         ||
                                                                         (location.PostalCode ?? ""). Contains(pattern)         ||
                                                                          location.Country.ToString().Contains(pattern)         ||
                                                                          location.Directions.        Matches (pattern)         ||
                                                                          location.Operator?.   Name. Contains(pattern) == true ||
                                                                          location.SubOperator?.Name. Contains(pattern) == true ||
                                                                          location.Owner?.      Name. Contains(pattern) == true ||
                                                                          //location.Facilities.        Matches (pattern)         ||
                                                                          location.EVSEUIds.          Matches (pattern)         ||
                                                                          location.EVSEIds.           Matches (pattern)         
                                                                          //location.EVSEs.Any(evse => evse.Connectors.Any(connector => connector?.GetTariffId(emspId).ToString()?.Contains(pattern) == true))
                                               );

                                                                      //ToDo: Filter to NOT show all locations to everyone!
                    var allLocations         = CommonAPI.
                                                   GetLocations().//location => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == location.CountryCode &&
                                                                  //                                                       role.PartyId     == location.PartyId)).
                                                   ToArray();

                    var filteredLocations    = allLocations.
                                                   Where(matchFilter).
                                                   Where(location => !filters.From.HasValue || location.LastUpdated >  filters.From.Value).
                                                   Where(location => !filters.To.  HasValue || location.LastUpdated <= filters.To.  Value).
                                                   ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                                   Server                      = DefaultHTTPServerName,
                                                   Date                        = Timestamp.Now,
                                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                                               }.

                                               // The overall number of locations
                                               Set("X-Total-Count",     allLocations.     Length).

                                               // The number of locations matching search filters
                                               Set("X-Filtered-Count",  filteredLocations.Length).

                                               // The maximum number of locations that the server WILL return within a single request
                                               Set("X-Limit",           allLocations.     Length);


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
                        //   - Link: <https://www.server.com/ocpi/hub/2.0/cdrs/?offset=150&limit=50>; rel="next"
                        httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                 ? $"https://{ExternalDNSName}"
                                                                 : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/locations{queryParameters}>; rel=\"next\"");

                    }

                    #endregion


                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = httpResponseBuilder,
                                   Data                 = new JArray(
                                                              filteredLocations.
                                                                  OrderBy       (location => location.Created).
                                                                  SkipTakeFilter(filters.Offset,
                                                                                 filters.Limit).
                                                                  Select        (location => location.ToJSON(
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
                                                          )
                               }
                           );

                });

                //    return Task.FromResult(
                //        new OCPIResponse.Builder(Request) {
                //                StatusCode           = 1000,
                //                StatusMessage        = "Hello world!",
                //                Data                 = new JArray(filteredLocations.SkipTakeFilter(filters.Offset,
                //                                                                                   filters.Limit).
                //                                                                    SafeSelect(location => location.ToJSON(Request.EMSPId,
                                                                                                                           
                //                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                //                    HTTPStatusCode             = HTTPStatusCode.OK,
                //                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                //                    AccessControlAllowHeaders  = [ "Authorization" ]
                //                    //LastModified               = ?
                //                }.
                //                Set("X-Total-Count", allLocations.Length)
                //                // X-Limit               The maximum number of objects that the server WILL return.
                //                // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                //        });

                //});

            #endregion

            #endregion

            #region ~/locations/{locationId}

            #region OPTIONS  ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}",
                CPOEvents.GetLocationHTTPRequest,
                CPOEvents.GetLocationHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    if (!request.ParseMandatoryLocation(CommonAPI,
                                                        //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                        CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
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
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = location.LastUpdated,
                                   ETag                       = location.ETag
                               }
                        });

                });

            #endregion

            #endregion

            #region ~/locations/{locationId}/{evseId}

            #region OPTIONS  ~/locations/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}/{evseId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}/{evseId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}/{evseId}",
                CPOEvents.GetEVSEHTTPRequest,
                CPOEvents.GetEVSEHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    #region Check EVSE

                    if (!request.ParseMandatoryLocationEVSE(CommonAPI,
                                                            //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                            CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                            out var locationId,
                                                            out var location,
                                                            out var evseId,
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
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = evse.LastUpdated,
                                   ETag                       = evse.ETag
                               }
                        });

                });

            #endregion

            #endregion

            #region ~/locations/{locationId}/{evseId}/{connectorId}

            #region OPTIONS  ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/locations/{locationId}/{evseId}/{connectorId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "locations/{locationId}/{evseId}/{connectorId}",
                CPOEvents.GetConnectorHTTPRequest,
                CPOEvents.GetConnectorHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    #region Check connector

                    if (!request.ParseMandatoryLocationEVSEConnector(CommonAPI,
                                                                     //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                                     CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
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
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = connector.LastUpdated,
                                   ETag                       = connector.ETag
                               }
                        });

                });

            #endregion

            #endregion


            #region ~/tariffs

            #region OPTIONS  ~/tariffs

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/tariffs

            // https://example.com/ocpi/2.2/hub/tariffs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs",
                CPOEvents.GetTariffsHTTPRequest,
                CPOEvents.GetTariffsHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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


                    var filters          = request.GetDateAndPaginationFilters();

                    var allTariffs       = CommonAPI.//GetTariffs(tariff => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == tariff.CountryCode &&
                                                     //                                                                role.PartyId     == tariff.PartyId)).
                                                     GetTariffs(tariff => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == tariff.CountryCode &&
                                                                                                             partyData.Id.PartyId       == tariff.PartyId)).
                                                     ToArray();

                    var filteredTariffs  = allTariffs.Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                      Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                      ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                                   Server                     = DefaultHTTPServerName,
                                                   Date                       = Timestamp.Now,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //   - Link: <https://www.server.com/ocpi/hub/2.0/cdrs/?offset=150&limit=50>; rel="next"
                        httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                 ? $"https://{ExternalDNSName}"
                                                                 : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/tariffs{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = httpResponseBuilder,
                                   Data                 = new JArray(
                                                              filteredTariffs.
                                                              OrderBy       (tariff => tariff.Created).
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select        (tariff => tariff.ToJSON(
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
                                                                                       ))
                                                          )
                               }
                           );

                });

            #endregion

            #endregion

            #region ~/tariffs/{tariffId}        [NonStandard]

            #region OPTIONS  ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs/{tariffId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/tariffs/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs/{tariffId}",
                CPOEvents.GetTariffHTTPRequest,
                CPOEvents.GetTariffHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    #region Check tariff

                    if (!request.ParseMandatoryTariff(CommonAPI,
                                                      //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                      CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                      out var tariffId,
                                                      out var tariff,
                                                      out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = tariff.ToJSON(
                                                          false, //IncludeOwnerInformation
                                                          false, //IncludeExtensions
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
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = tariff.LastUpdated,
                                   ETag                       = tariff.ETag
                               }
                        });

                });

            #endregion

            #endregion


            #region ~/sessions

            #region OPTIONS  ~/sessions

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/sessions

            // https://example.com/ocpi/2.2/hub/sessions/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions",
                CPOEvents.GetSessionsHTTPRequest,
                CPOEvents.GetSessionsHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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


                    var filters              = request.GetDateAndPaginationFilters();

                    var allSessions          = CommonAPI.//GetSessions(session => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == session.CountryCode &&
                                                         //                                                                  role.PartyId     == session.PartyId)).
                                                         GetSessions(session => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == session.CountryCode &&
                                                                                                                   partyData.Id.PartyId       == session.PartyId)).
                                                         ToArray();

                    var filteredSessions     = allSessions.Where(session => !filters.From.HasValue || session.LastUpdated >  filters.From.Value).
                                                           Where(session => !filters.To.  HasValue || session.LastUpdated <= filters.To.  Value).
                                                           ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                                   Server                     = DefaultHTTPServerName,
                                                   Date                       = Timestamp.Now,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //   - Link: <https://www.server.com/ocpi/hub/2.0/cdrs/?offset=150&limit=50>; rel="next"
                        httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                 ? $"https://{ExternalDNSName}"
                                                                 : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/sessions{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = httpResponseBuilder,
                                   Data                 = new JArray(
                                                              filteredSessions.
                                                                  OrderBy       (session => session.Created).
                                                                  SkipTakeFilter(filters.Offset,
                                                                                 filters.Limit).
                                                                  Select        (session => session.ToJSON(
                                                                                                CustomSessionSerializer,
                                                                                                CustomCDRTokenSerializer,
                                                                                                CustomChargingPeriodSerializer,
                                                                                                CustomCDRDimensionSerializer,
                                                                                                CustomPriceSerializer
                                                                                            ))
                                                          )
                               }
                           );

                });

            #endregion

            #endregion

            #region ~/sessions/{sessionId}      [NonStandard]

            #region OPTIONS  ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "sessions/{sessionId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/sessions/{sessionId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "sessions/{sessionId}",
                CPOEvents.GetSessionHTTPRequest,
                CPOEvents.GetSessionHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    #region Check session

                    if (!request.ParseMandatorySession(CommonAPI,
                                                       //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                       CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                       out var sessionId,
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
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = session.LastUpdated,
                                   ETag                       = session.ETag
                               }
                        });

                });

            #endregion

            #endregion


            // For HUBs, but also for EMSPs, as SCSPs might talk to EMSPs!
            #region ~/chargingprofiles/{session_id}

            // https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/mod_charging_profiles.asciidoc

            #region GET      ~/chargingprofiles/{session_id}?duration={duration}&response_url=https://client.com/12345/

            // 1. GET will just return a ChargingProfileResponse (result=ACCEPTED).
            // 2. The resposeURL will be called with a ActiveProfileResult object (result=ACCEPTED, ActiveChargingProfile).

            // NOTE: This GET requests introduces state and thus is a VIOLATION of HTTP semantics!

            #endregion

            #region PUT      ~/chargingprofiles/{session_id}?response_url=https://client.com/12345/

            // 1. PUT (with a resposeURL): SetChargingProfile object
            // 2. The resposeURL will be called later, e.g. POST https://client.com/12345/ with a ChargingProfileResult object.

            #endregion

            #region DELETE   ~/chargingprofiles/{session_id}?response_url=https://client.com/12345/

            // 1. DELETE will just return a ChargingProfileResponse (result=ACCEPTED).
            // 2. The resposeURL will be called with a ClearProfileResult object (result=ACCEPTED).

            #endregion

            #endregion


            #region ~/sessions/{session_id}/charging_preferences <= Yet to do!

            //ToDo: Implement ~/sessions/{session_id}/charging_preferences!

            #region PUT      ~/sessions/{session_id}/charging_preferences

            // https://example.com/ocpi/2.2/hub/sessions/12454/charging_preferences

            #endregion

            #endregion


            #region ~/cdrs

            #region OPTIONS  ~/cdrs

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "CDRs",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/cdrs

            // https://example.com/ocpi/2.2/hub/cdrs/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs",
                CPOEvents.GetCDRsHTTPRequest,
                CPOEvents.GetCDRsHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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


                    var filters              = request.GetDateAndPaginationFilters();

                    var allCDRs              = CommonAPI.//GetCDRs(cdr => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == cdr.CountryCode &&
                                                         //                                                          role.PartyId     == cdr.PartyId)).
                                                         GetCDRs(cdr => CommonAPI.Parties.Any(partyData => partyData.Id.CountryCode == cdr.CountryCode &&
                                                                                                           partyData.Id.PartyId       == cdr.PartyId)).
                                                         ToArray();

                    var filteredCDRs         = allCDRs.Where(CDR => !filters.From.HasValue || CDR.LastUpdated >  filters.From.Value).
                                                       Where(CDR => !filters.To.  HasValue || CDR.LastUpdated <= filters.To.  Value).
                                                       ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                                   Server                     = DefaultHTTPServerName,
                                                   Date                       = Timestamp.Now,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //   - Link: <https://www.server.com/ocpi/hub/2.0/cdrs/?offset=150&limit=50>; rel="next"
                        httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                 ? $"https://{ExternalDNSName}"
                                                                 : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/cdrs{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = httpResponseBuilder,
                                   Data                 = new JArray(
                                                              filteredCDRs.
                                                              OrderBy       (cdr => cdr.Created).
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select        (cdr => cdr.ToJSON(
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
                                                                                    ))
                                                          )
                               }
                           );

                });

            #endregion

            #endregion

            #region ~/cdrs/{CDRId}

            #region OPTIONS  ~/cdrs/{CDRId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs/{CDRId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/cdrs/{CDRId}     // The concrete URL is not specified by OCPI! m(

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs/{CDRId}",
                CPOEvents.GetCDRHTTPRequest,
                CPOEvents.GetCDRHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    #region Check CDR

                    if (!request.ParseMandatoryCDR(CommonAPI,
                                                   //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                                   CommonAPI.Parties.Select(partyData => new Tuple<CountryCode, Party_Id>(partyData.Id.CountryCode, partyData.Id.PartyId)),
                                                   out var cdrId,
                                                   out var cdr,
                                                   out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = cdr.ToJSON(
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
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = cdr.LastUpdated,
                                   ETag                       = cdr.ETag
                               }
                        });

                });

            #endregion

            #endregion


            #region ~/tokens/{country_code}/{party_id}       [NonStandard]

            #region OPTIONS  ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens/{country_code}/{party_id}",
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

            #region GET      ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tokens/{country_code}/{party_id}",
                CPOEvents.GetTokensHTTPRequest,
                CPOEvents.GetTokensHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    var filters              = request.GetDateAndPaginationFilters();

                    var allTokens            = CommonAPI.GetTokenStatus(partyId.Value).ToArray();

                    var filteredTokens       = allTokens.Select(tokenStatus => tokenStatus.Token).
                                                         Where (token       => !filters.From.HasValue || token.LastUpdated >  filters.From.Value).
                                                         Where (token       => !filters.To.  HasValue || token.LastUpdated <= filters.To.  Value).
                                                         ToArray();


                    var httpResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                                   Server                     = DefaultHTTPServerName,
                                                   Date                       = Timestamp.Now,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //   - Link: <https://www.server.com/ocpi/hub/2.0/cdrs/?offset=150&limit=50>; rel="next"
                        httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                 ? $"https://{ExternalDNSName}"
                                                                 : $"http://127.0.0.1:{CommonAPI.BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort}")}{URLPathPrefix}/tokens/{partyId.Value.CountryCode}/{partyId.Value.PartyId}{queryParameters}>; rel=\"next\"");

                    }

                    #endregion

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = httpResponseBuilder,
                                   Data                 = new JArray(
                                                              filteredTokens.
                                                                  OrderBy       (token => token.Created).
                                                                  SkipTakeFilter(filters.Offset,
                                                                                 filters.Limit).
                                                                  Select        (token => token.ToJSON(CustomTokenSerializer,
                                                                                                       CustomEnergyContractSerializer))
                                                          )
                               }
                           );

                });

            #endregion

            #region DELETE   ~/tokens/{country_code}/{party_id}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tokens/{country_code}/{party_id}",
                CPOEvents.DeleteTokensHTTPRequest,
                CPOEvents.DeleteTokensHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    await CommonAPI.RemoveAllTokens(partyId.Value);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/tokens/{country_code}/{party_id}/{tokenId}

            #region OPTIONS  ~/tokens/{country_code}/{party_id}/{tokenId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.PUT, HTTPMethod.PATCH, HTTPMethod.DELETE ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE"],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ]
                               }
                        })

            );

            #endregion

            #region GET      ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                CPOEvents.GetTokenHTTPRequest,
                CPOEvents.GetTokenHTTPResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
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

                    #region Check token

                    if (!request.ParseMandatoryToken(CommonAPI,
                                                     request.LocalAccessInfo.Roles.Select(role => role.PartyId),
                                                     out var countryCode,
                                                     out var partyId,
                                                     out var tokenId,
                                                     out var tokenStatus,
                                                     out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    //ToDo: What exactly to do with this information?
                    var tokenType = request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = tokenStatus.Token.ToJSON(
                                                          CustomTokenSerializer,
                                                          CustomEnergyContractSerializer
                                                      ),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = tokenStatus.Token.LastUpdated,
                                   ETag                       = tokenStatus.Token.ETag
                               }
                        });

                });

            #endregion

            #region PUT      ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                CPOEvents.PutTokenHTTPRequest,
                CPOEvents.PutTokenHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
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

                    #region Check token

                    if (!request.ParseTokenId(out var countryCode,
                                              out var partyId,
                                              out var tokenId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated token JSON

                    if (!request.TryParseJObjectRequestBody(out var tokenJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Token.TryParse(tokenJSON,
                                        out var newOrUpdatedToken,
                                        out var errorResponse,
                                        countryCode,
                                        partyId,
                                        tokenId))
                    {

                        return new OCPIResponse.Builder(request) {
                               StatusCode           = 2001,
                               StatusMessage        = "Could not parse the given token JSON: " + errorResponse,
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                    }

                    #endregion


                    //ToDo: What exactly to do with this information?
                    var TokenType          = request.QueryString.TryParseEnum<TokenType>("type") ?? OCPI.TokenType.RFID;

                    var addOrUpdateResult  = await CommonAPI.AddOrUpdateToken(
                                                       newOrUpdatedToken,
                                                       AllowDowngrades: AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                   );


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var tokenData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = tokenData.Token.ToJSON(
                                                              CustomTokenSerializer,
                                                              CustomEnergyContractSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                              HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                                               ? HTTPStatusCode.Created
                                                                                               : HTTPStatusCode.OK,
                                                              AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                              AccessControlAllowHeaders  = [ "Authorization" ],
                                                              LastModified               = tokenData.Token.LastUpdated,
                                                              ETag                       = tokenData.Token.ETag
                                                          }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedToken.ToJSON(CustomTokenSerializer,
                                                                               CustomEnergyContractSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                          AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                                          AccessControlAllowHeaders  = [ "Authorization" ],
                                                          LastModified               = newOrUpdatedToken.LastUpdated,
                                                          ETag                       = newOrUpdatedToken.ETag
                                                      }
                           };

                });

            #endregion

            #region PATCH    ~/tokens/{country_code}/{party_id}/{tokenId}?type={type}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                CPOEvents.PatchTokenHTTPRequest,
                CPOEvents.PatchTokenHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo is null ||
                        request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
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

                    #region Check token

                    if (!request.ParseMandatoryToken(CommonAPI,
                                                     request.LocalAccessInfo.Roles.Select(role => role.PartyId),
                                                     out var countryCode,
                                                     out var partyId,
                                                     out var tokenId,
                                                     out var existingTokenStatus,
                                                     out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse token JSON patch

                    if (!request.TryParseJObjectRequestBody(out var tokenPatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    //ToDo: What exactly to do with this information?
                    var tokenType     = request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;


                    //ToDo: Validation-Checks for PATCHes (E-Tag, Timestamp, ...)
                    var patchedToken  = await CommonAPI.TryPatchToken(
                                                  Party_Idv3.From(
                                                      existingTokenStatus.Token.CountryCode,
                                                      existingTokenStatus.Token.PartyId
                                                  ),
                                                  existingTokenStatus.Token.Id,
                                                  tokenPatch,
                                                  AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                              );


                    if (patchedToken.IsSuccessAndDataNotNull(out var tokenData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = tokenData.ToJSON(
                                                                  CustomTokenSerializer,
                                                                  CustomEnergyContractSerializer
                                                              ),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = tokenData.LastUpdated,
                                           ETag                       = tokenData.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedToken.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                });

            #endregion

            #region DELETE   ~/tokens/{country_code}/{party_id}/{tokenId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tokens/{country_code}/{party_id}/{tokenId}",
                CPOEvents.DeleteTokenHTTPRequest,
                CPOEvents.DeleteTokenHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.EMSP) == true ||
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

                    #region Check token (status)

                    if (!request.ParseMandatoryToken(CommonAPI,
                                                     request.LocalAccessInfo.Roles.Select(role => role.PartyId),
                                                     out var countryCode,
                                                     out var partyId,
                                                     out var tokenId,
                                                     out var existingTokenStatus,
                                                     out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //ToDo: What exactly to do with this information?
                    var tokenType  = request.QueryString.TryParseEnum<TokenType>("type") ?? TokenType.RFID;

                    var result     = await CommonAPI.RemoveToken(existingTokenStatus.Token);

                    if (result.IsSuccessAndDataNotNull(out var tokenStatus))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = tokenStatus.Token.ToJSON(
                                                              CustomTokenSerializer,
                                                              CustomEnergyContractSerializer
                                                          ),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                       //LastModified               = Timestamp.Now.ToISO8601()
                                   }
                               };

                    else
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = existingTokenStatus.Token.ToJSON(
                                                              CustomTokenSerializer,
                                                              CustomEnergyContractSerializer
                                                          ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                       //LastModified               = Timestamp.Now.ToISO8601()
                                   }
                               };

                });

            #endregion

            #endregion


            // Commands

            #region ~/commands/RESERVE_NOW

            #region OPTIONS  ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/RESERVE_NOW",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/RESERVE_NOW

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/RESERVE_NOW",
                CPOEvents.ReserveNowHTTPRequest,
                CPOEvents.ReserveNowHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
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

                    #region Parse ReserveNow command JSON

                    if (!request.TryParseJObjectRequestBody(out var reserveNowJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!ReserveNowCommand.TryParse(reserveNowJSON,
                                                    out var reserveNowCommand,
                                                    out var errorResponse))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'RESERVE_NOW' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnReserveNowCommand is not null)
                        commandResponse = await OnReserveNowCommand.Invoke(
                                                    EMSP_Id.From(request.RemoteParty.Id),
                                                    reserveNowCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            reserveNowCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/commands/CANCEL_RESERVATION

            #region OPTIONS  ~/commands/CANCEL_RESERVATION

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/CANCEL_RESERVATION",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/CANCEL_RESERVATION

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/CANCEL_RESERVATION",
                CPOEvents.CancelReservationHTTPRequest,
                CPOEvents.CancelReservationHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
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

                    #region Parse CancelReservation command JSON

                    if (!request.TryParseJObjectRequestBody(out var cancelReservationJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!CancelReservationCommand.TryParse(cancelReservationJSON,
                                                           out var cancelReservationCommand,
                                                           out var errorResponse))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'CANCEL_RESERVATION' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnCancelReservationCommand is not null)
                        commandResponse = await OnCancelReservationCommand.Invoke(
                                                    EMSP_Id.From(request.RemoteParty.Id),
                                                    cancelReservationCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            cancelReservationCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/commands/START_SESSION

            #region OPTIONS  ~/commands/START_SESSION

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/START_SESSION",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/START_SESSION

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/START_SESSION",
                CPOEvents.StartSessionHTTPRequest,
                CPOEvents.StartSessionHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
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

                    #region Parse StartSession command JSON

                    if (!request.TryParseJObjectRequestBody(out var startSessionJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!StartSessionCommand.TryParse(startSessionJSON,
                                                      out var startSessionCommand,
                                                      out var errorResponse))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'START_SESSION' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnStartSessionCommand is not null)
                        commandResponse = await OnStartSessionCommand.Invoke(
                                                    EMSP_Id.From(request.RemoteParty.Id),
                                                    startSessionCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            startSessionCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/commands/STOP_SESSION

            #region OPTIONS  ~/commands/STOP_SESSION

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/STOP_SESSION",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/STOP_SESSION

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/STOP_SESSION",
                CPOEvents.StopSessionHTTPRequest,
                CPOEvents.StopSessionHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
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

                    #region Parse StopSession command JSON

                    if (!request.TryParseJObjectRequestBody(out var stopSessionJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!StopSessionCommand.TryParse(stopSessionJSON,
                                                     out var stopSessionCommand,
                                                     out var errorResponse))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'STOP_SESSION' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnStopSessionCommand is not null)
                        commandResponse = await OnStopSessionCommand.Invoke(
                                                    EMSP_Id.From(request.RemoteParty.Id),
                                                    stopSessionCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            stopSessionCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion

            #region ~/commands/UNLOCK_CONNECTOR

            #region OPTIONS  ~/commands/UNLOCK_CONNECTOR

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST"],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/commands/UNLOCK_CONNECTOR

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR",
                CPOEvents.UnlockConnectorHTTPRequest,
                CPOEvents.UnlockConnectorHTTPResponse,
                async request => {

                    #region Check access token

                    if (request.RemoteParty is null ||
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

                    #region Parse UnlockConnector command JSON

                    if (!request.TryParseJObjectRequestBody(out var unlockConnectorJSON, out var ocpiResponse))
                        return ocpiResponse;

                    if (!UnlockConnectorCommand.TryParse(unlockConnectorJSON,
                                                         out var unlockConnectorCommand,
                                                         out var errorResponse))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given 'UNLOCK_CONNECTOR' command JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    CommandResponse? commandResponse = null;

                    if (OnUnlockConnectorCommand is not null)
                        commandResponse = await OnUnlockConnectorCommand.Invoke(
                                                    EMSP_Id.From(request.RemoteParty.Id),
                                                    unlockConnectorCommand
                                                );

                    commandResponse ??= new CommandResponse(
                                            unlockConnectorCommand,
                                            CommandResponseTypes.NOT_SUPPORTED,
                                            Timeout: TimeSpan.FromSeconds(15),
                                            Message: [ new DisplayText(Languages.en, "Not supported!") ]
                                        );

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = commandResponse.ToJSON(
                                                          CustomCommandResponseSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #endregion





            //- AsEMSP endpoints ---------------------------------------------------------------------

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
                EMSPEvents.GetLocationsHTTPRequest,
                EMSPEvents.GetLocationsHTTPResponse,
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

                    var allLocations       = GetRemoteLocations(partyId.Value).ToArray();

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
                EMSPEvents.DeleteLocationsHTTPRequest,
                EMSPEvents.DeleteLocationsHTTPResponse,
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
                    var result = await RemoveAllRemoteLocations(partyId.Value);


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
                EMSPEvents.GetLocationHTTPRequest,
                EMSPEvents.GetLocationHTTPResponse,
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
                EMSPEvents.PutLocationHTTPRequest,
                EMSPEvents.PutLocationHTTPResponse,
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


                    var addOrUpdateResult = await AddOrUpdateRemoteLocation(
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
                EMSPEvents.PatchLocationHTTPRequest,
                EMSPEvents.PatchLocationHTTPResponse,
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

                    var patchedLocation = await TryPatchRemoteLocation(
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
                EMSPEvents.DeleteLocationHTTPRequest,
                EMSPEvents.DeleteLocationHTTPResponse,
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
                    var result = await RemoveRemoteLocation(location);


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
                EMSPEvents.GetEVSEHTTPRequest,
                EMSPEvents.GetEVSEHTTPResponse,
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
                EMSPEvents.PutEVSEHTTPRequest,
                EMSPEvents.PutEVSEHTTPResponse,
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


                    var addOrUpdateResult = await AddOrUpdateRemoteEVSE(
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
                EMSPEvents.PatchEVSEHTTPRequest,
                EMSPEvents.PatchEVSEHTTPResponse,
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


                    var patchedEVSE = await TryPatchRemoteEVSE(
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
                EMSPEvents.DeleteEVSEHTTPRequest,
                EMSPEvents.DeleteEVSEHTTPResponse,
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
                EMSPEvents.GetConnectorHTTPRequest,
                EMSPEvents.GetConnectorHTTPResponse,
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
                EMSPEvents.PutConnectorHTTPRequest,
                EMSPEvents.PutConnectorHTTPResponse,
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


                    var addOrUpdateResult = await AddOrUpdateRemoteConnector(
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
                EMSPEvents.PatchConnectorHTTPRequest,
                EMSPEvents.PatchConnectorHTTPResponse,
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


                    var patchedConnector = await TryPatchRemoteConnector(
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
                EMSPEvents.DeleteConnectorHTTPRequest,
                EMSPEvents.DeleteConnectorHTTPResponse,
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
                EMSPEvents.PutEVSEHTTPRequest,
                EMSPEvents.PutEVSEHTTPResponse,
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



            #region ~/tariffs/{country_code}/{party_id}                                 [NonStandard]

            #region OPTIONS  ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs/{country_code}/{party_id}",
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

            #region GET      ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs/{country_code}/{party_id}",
                EMSPEvents.GetTariffsHTTPRequest,
                EMSPEvents.GetTariffsHTTPResponse,
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


                    var filters          = request.GetDateAndPaginationFilters();

                    var allTariffs       = GetRemoteTariffs(partyId.Value).ToArray();

                    var filteredTariffs  = allTariffs.Where(tariff => !filters.From.HasValue || tariff.LastUpdated >  filters.From.Value).
                                                      Where(tariff => !filters.To.  HasValue || tariff.LastUpdated <= filters.To.  Value).
                                                      ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredTariffs.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(tariff => tariff.ToJSON(
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
                                                                               ))
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allTariffs.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region DELETE   ~/tariffs/{country_code}/{party_id}            [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tariffs/{country_code}/{party_id}",
                EMSPEvents.DeleteTariffsHTTPRequest,
                EMSPEvents.DeleteTariffsHTTPResponse,
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


                    var result = await RemoveAllRemoteTariffs(partyId.Value);


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

            #region ~/tariffs/{country_code}/{party_id}/{tariffId}

            #region OPTIONS  ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
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

            #region GET      ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                EMSPEvents.GetTariffHTTPRequest,
                EMSPEvents.GetTariffHTTPResponse,
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
                                    AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "PUT", "DELETE" },
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check tariff

                    if (!request.ParseMandatoryTariff(this,
                                                      out var countryCode,
                                                      out var partyId,
                                                      out var tariffId,
                                                      out var tariff,
                                                      out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = tariff.ToJSON(
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
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = ["OPTIONS", "GET", "PUT", "PATCH", "DELETE"],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = tariff.LastUpdated,
                                   ETag                       = tariff.ETag
                               }
                        });

                }
            );

            #endregion

            #region PUT      ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                EMSPEvents.PutTariffHTTPRequest,
                EMSPEvents.PutTariffHTTPResponse,
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

                    #region Check existing tariff

                    if (!request.ParseOptionalTariff(this,
                                                     out var countryCode,
                                                     out var partyId,
                                                     out var tariffId,
                                                     out var existingTariff,
                                                     out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated tariff

                    if (!request.TryParseJObjectRequestBody(out var tariffJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Tariff.TryParse(tariffJSON,
                                         out var newOrUpdatedTariff,
                                         out var errorResponse,
                                         countryCode,
                                         partyId,
                                         tariffId))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given tariff JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    var addOrUpdateResult = await AddOrUpdateRemoteTariff(
                                                      newOrUpdatedTariff,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    if (addOrUpdateResult.IsSuccessAndDataNotNull(out var data))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = data.ToJSON(
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
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = data.LastUpdated,
                                       ETag                       = data.ETag
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedTariff.ToJSON(
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
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = newOrUpdatedTariff.LastUpdated,
                                   ETag                       = newOrUpdatedTariff.ETag
                               }
                           };

                }
            );

            #endregion

            #region PATCH    ~/tariffs/{country_code}/{party_id}/{tariffId}      [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.PATCH,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                EMSPEvents.PatchTariffHTTPRequest,
                EMSPEvents.PatchTariffHTTPResponse,
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

                    #region Check tariff

                    if (!request.ParseMandatoryTariff(this,
                                                      out var countryCode,
                                                      out var partyId,
                                                      out var tariffId,
                                                      out var existingTariff,
                                                      out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse and apply Tariff JSON patch

                    if (!request.TryParseJObjectRequestBody(out var tariffPatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    // Validation-Checks for PATCHes
                    // (E-Tag, Timestamp, ...)

                    var patchedTariff = await TryPatchRemoteTariff(
                                                  Party_Idv3.From(
                                                      existingTariff.CountryCode,
                                                      existingTariff.PartyId
                                                  ),
                                                  existingTariff.Id,
                                                  tariffPatch
                                              );

                    //ToDo: Handle update errors!
                    if (patchedTariff.IsSuccessAndDataNotNull(out var patchedData))
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = patchedData.ToJSON(),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = patchedData.LastUpdated,
                                           ETag                       = patchedData.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedTariff.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                }
            );

            #endregion

            #region DELETE   ~/tariffs/{country_code}/{party_id}/{tariffId}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "tariffs/{country_code}/{party_id}/{tariffId}",
                EMSPEvents.DeleteTariffHTTPRequest,
                EMSPEvents.DeleteTariffHTTPResponse,
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

                    #region Check existing tariff

                    if (!request.ParseMandatoryTariff(this,
                                                      out var countryCode,
                                                      out var partyId,
                                                      out var tariffId,
                                                      out var existingTariff,
                                                      out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    var result = await RemoveRemoteTariff(existingTariff);


                    if (result.IsSuccessAndDataNotNull(out var tariffData))
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = existingTariff.ToJSON(
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
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = existingTariff.LastUpdated,
                                       ETag                       = existingTariff.ETag
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = "Failed!",
                               Data                 = existingTariff.ToJSON(
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
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = existingTariff.LastUpdated,
                                   ETag                       = existingTariff.ETag
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
                URLPathPrefix + "sessions_EMSP",
                EMSPEvents.GetSessionsHTTPRequest,
                EMSPEvents.GetSessionsHTTPResponse,
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
                EMSPEvents.GetSessionsHTTPRequest,
                EMSPEvents.GetSessionsHTTPResponse,
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
                EMSPEvents.DeleteSessionsHTTPRequest,
                EMSPEvents.DeleteSessionsHTTPResponse,
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
                EMSPEvents.DeleteSessionsHTTPRequest,
                EMSPEvents.DeleteSessionsHTTPResponse,
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
                EMSPEvents.GetSessionHTTPRequest,
                EMSPEvents.GetSessionHTTPResponse,
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
                EMSPEvents.PutSessionHTTPRequest,
                EMSPEvents.PutSessionHTTPResponse,
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
                EMSPEvents.PatchSessionHTTPRequest,
                EMSPEvents.PatchSessionHTTPResponse,
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
                EMSPEvents.DeleteSessionHTTPRequest,
                EMSPEvents.DeleteSessionHTTPResponse,
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



            #region ~/cdrs/{country_code}/{party_id}                                    [NonStandard]

            #region OPTIONS  ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs/{country_code}/{party_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.POST, HTTPMethod.DELETE ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET", "POST", "DELETE"],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/cdrs                                     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs_EMSP",
                EMSPEvents.GetCDRsHTTPRequest,
                EMSPEvents.GetCDRsHTTPResponse,
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

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    var filters       = request.GetDateAndPaginationFilters();

                    var allCDRs       = CommonAPI.GetCDRs(session => request.LocalAccessInfo.Roles.Any(role => role.PartyId.CountryCode == session.CountryCode &&
                                                                                                               role.PartyId.PartyId       == session.PartyId)).
                                                  ToArray();

                    var filteredCDRs  = allCDRs.Where(cdr => !filters.From.HasValue || cdr.LastUpdated >  filters.From.Value).
                                                Where(cdr => !filters.To.  HasValue || cdr.LastUpdated <= filters.To.  Value).
                                                ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredCDRs.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(cdr => cdr.ToJSON(
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
                                                                            ))
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allCDRs.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region GET      ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs/{country_code}/{party_id}",
                EMSPEvents.GetCDRsHTTPRequest,
                EMSPEvents.GetCDRsHTTPResponse,
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

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    var filters       = request.GetDateAndPaginationFilters();

                    var allCDRs       = CommonAPI.GetCDRs(partyId.Value).ToArray();

                    var filteredCDRs  = CommonAPI.GetCDRs().
                                                  Where(cdr => !filters.From.HasValue || cdr.LastUpdated >  filters.From.Value).
                                                  Where(cdr => !filters.To.  HasValue || cdr.LastUpdated <= filters.To.  Value).
                                                  ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredCDRs.
                                                              SkipTakeFilter(filters.Offset,
                                                                             filters.Limit).
                                                              Select(cdr => cdr.ToJSON(
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
                                                                            ))
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allCDRs.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #region POST     ~/cdrs               ///{country_code}/{party_id}       <= Unclear if this URL is correct!

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "cdrs",///{country_code}/{party_id}",
                EMSPEvents.PostCDRHTTPRequest,
                EMSPEvents.PostCDRHTTPResponse,
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

                    #region Check party identification

                    //if (!request.ParsePartyId(CommonAPI,
                    //                          out var partyId,
                    //                          out var ocpiResponseBuilder))
                    //{
                    //    return ocpiResponseBuilder;
                    //}

                    #endregion

                    #region Parse newCDR JSON

                    if (!request.TryParseJObjectRequestBody(out var jsonCDR, out var ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!CDR.TryParse(jsonCDR,
                                      out var newCDR,
                                      out var errorResponse
                                      //partyId.Value.CountryCode,
                                      //partyId.Value.Party
                                      ))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given charge detail record JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    // ToDo: What kind of error might happen here?
                    var result = await CommonAPI.AddCDR(newCDR);


                    // https://github.com/ocpi/ocpi/blob/release-2.2-bugfixes/mod_cdrs.asciidoc#mod_cdrs_post_method
                    // The response should contain the URL to the just created CDR object in the eMSP’s system.
                    //
                    // Parameter    Location
                    // Datatype     URL
                    // Required     yes
                    // Description  URL to the newly created CDR in the eMSP’s system, can be used by the CPO system to perform a GET on the same CDR.
                    // Example      https://www.server.com/ocpi/emsp/2.2/cdrs/123456

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = newCDR.ToJSON(
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
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Created,
                                       Location                   = org.GraphDefined.Vanaheimr.Hermod.HTTP.Location.From(URLPathPrefix + "cdrs" + newCDR.CountryCode.ToString() + newCDR.PartyId.ToString() + newCDR.Id.ToString()),
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = newCDR.LastUpdated,
                                       ETag                       = newCDR.ETag
                                   }
                               };

                }
            );

            #endregion

            #region DELETE   ~/cdrs                                     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "cdrs",
                EMSPEvents.DeleteCDRsHTTPRequest,
                EMSPEvents.DeleteCDRsHTTPResponse,
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


                    foreach (var role in request.LocalAccessInfo.Roles)
                        await CommonAPI.RemoveAllCDRs(role.PartyId);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #region DELETE   ~/cdrs/{country_code}/{party_id}           [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "cdrs/{country_code}/{party_id}",
                EMSPEvents.DeleteCDRsHTTPRequest,
                EMSPEvents.DeleteCDRsHTTPResponse,
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

                    #region Check party identification

                    if (!request.ParsePartyId(CommonAPI,
                                              out var partyId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    var result = await CommonAPI.RemoveAllCDRs(partyId.Value);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #endregion

            #region ~/cdrs/{country_code}/{party_id}/{cdrId}

            #region OPTIONS  ~/cdrs/{country_code}/{party_id}/{cdrId}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.DELETE ],
                                   AcceptPatch                = [ HTTPContentType.Application.JSONMergePatch_UTF8 ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/cdrs/{country_code}/{party_id}/{cdrId}       // The concrete URL is not specified by OCPI! m(

            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                EMSPEvents.GetCDRHTTPRequest,
                EMSPEvents.GetCDRHTTPResponse,
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

                    #region Check existing CDR

                    if (!request.ParseMandatoryCDR(this,
                                                   out var countryCode,
                                                   out var partyId,
                                                   out var cdrId,
                                                   out var cdr,
                                                   out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = cdr.ToJSON(
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
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = cdr.LastUpdated,
                                   ETag                       = cdr.ETag
                               }
                        });

                }
            );

            #endregion

            #region DELETE   ~/cdrs/{country_code}/{party_id}/{cdrId}    [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "cdrs/{country_code}/{party_id}/{cdrId}",
                EMSPEvents.DeleteCDRHTTPRequest,
                EMSPEvents.DeleteCDRHTTPResponse,
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

                    #region Check existing CDR

                    if (!request.ParseMandatoryCDR(this,
                                                   out var countryCode,
                                                   out var partyId,
                                                   out var cdrId,
                                                   out var existingCDR,
                                                   out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    //ToDo: await...
                    await CommonAPI.RemoveCDR(existingCDR);


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = existingCDR.ToJSON(
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
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = existingCDR.LastUpdated,
                                   ETag                       = existingCDR.ETag
                               }
                           };

                }
            );

            #endregion

            #endregion



            #region ~/tokens

            #region OPTIONS  ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/tokens

            // https://example.com/ocpi/2.2/cpo/tokens/?date_from=2019-01-28T12:00:00&date_to=2019-01-29T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + "tokens",
                EMSPEvents.GetTokensHTTPRequest,
                EMSPEvents.GetTokensHTTPResponse,
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


                    var filters              = request.GetDateAndPaginationFilters();

                    var allTokenStatus       = CommonAPI.GetTokenStatus().ToArray();

                    var filteredTokenStatus  = allTokenStatus.Where(tokenStatus => !filters.From.HasValue || tokenStatus.Token.LastUpdated >  filters.From.Value).
                                                              Where(tokenStatus => !filters.To.  HasValue || tokenStatus.Token.LastUpdated <= filters.To.  Value).
                                                              ToArray();


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new JArray(
                                                          filteredTokenStatus.SkipTakeFilter(filters.Offset,
                                                                                             filters.Limit).
                                                                              Select        (tokenStatus => tokenStatus.Token.ToJSON(
                                                                                                                CustomTokenSerializer,
                                                                                                                CustomEnergyContractSerializer
                                                                                                            ))
                                                      ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                   //LastModified               = ?
                               }.
                               Set("X-Total-Count", allTokenStatus.Length)
                               // X-Limit               The maximum number of objects that the server WILL return.
                               // Link                  Link to the 'next' page should be provided when this is NOT the last page.
                        });

                }
            );

            #endregion

            #endregion

            #region ~/tokens/{token_id}/authorize

            #region OPTIONS  ~/tokens/{token_id}/authorize

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "tokens/{token_id}/authorize",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.POST ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/tokens/{token_id}/authorize?type=RFID

            // A real-time authorization request
            // https://example.com/ocpi/2.2/emsp/tokens/012345678/authorize?type=RFID
            // curl -X POST http://127.0.0.1:3000/2.2/emsp/tokens/012345678/authorize?type=RFID
            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "tokens/{token_id}/authorize",
                EMSPEvents.PostTokenHTTPRequest,
                EMSPEvents.PostTokenHTTPResponse,
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

                    #region Check TokenId URI parameter

                    if (!request.ParseTokenId(out var tokenId,
                                              out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    var requestedTokenType  = request.QueryString.Map("type", TokenType.TryParse) ?? TokenType.RFID;

                    #region Parse optional LocationReference JSON

                    LocationReference? locationReference = null;

                    if (request.TryParseJObjectRequestBody(out var locationReferenceJSON,
                                                           out ocpiResponseBuilder,
                                                           AllowEmptyHTTPBody: true))
                    {

                        if (!LocationReference.TryParse(locationReferenceJSON,
                                                        out var _locationReference,
                                                        out var errorResponse))
                        {

                            return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given location reference JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                        }

                        locationReference = _locationReference;

                    }

                    #endregion


                    AuthorizationInfo? authorizationInfo    = null;

                    using var childCancellationTokenSource  = CancellationTokenSource.CreateLinkedTokenSource(request.HTTPRequest.CancellationToken);
                    var       timestamp                     = Timestamp.Now;
                    var       timeout                       = TimeSpan.FromMinutes(1);

                    var       postTokenTasks                = hub2emspClients.Values.
                                                                  Select(hub2EMSPClient => hub2EMSPClient.PostToken(
                                                                                               tokenId.Value,
                                                                                               requestedTokenType,
                                                                                               locationReference,
                                                                                               Request_Id.NewRandom(),
                                                                                               request.CorrelationId,
                                                                                               null, // VersionId
                                                                                               timestamp,
                                                                                               EventTracking_Id.New,
                                                                                               timeout,
                                                                                               childCancellationTokenSource.Token
                                                                                           )).
                                                                  ToArray();

                    await foreach (var postTokenTask in Task.WhenEach(postTokenTasks))
                    {

                        try
                        {

                            var postTokenResult = await postTokenTask;

                            if (postTokenResult.StatusCode    == 1000 &&
                                postTokenResult.Data?.Allowed == AllowedType.ALLOWED)
                            {
                                authorizationInfo = postTokenResult.Data;
                                break;
                            }

                        }
                        catch
                        {
                            // One hub failed: ignore (or log) and continue waiting for others
                            continue;
                        }

                    }

                    authorizationInfo ??= new AuthorizationInfo(
                                              AllowedType.NOT_ALLOWED
                                          );


                    #region Set a user-friendly response message for the ev driver

                    var responseText = "An error occurred!";

                    if (!authorizationInfo.Info.HasValue)
                    {

                        #region ALLOWED

                        if (authorizationInfo.Allowed == AllowedType.ALLOWED)
                        {

                            responseText = authorizationInfo.RemoteParty is not null
                                               ? $"Charging authorized by '{authorizationInfo.RemoteParty.Id}'!"
                                               :  "Charging authorized";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Der Ladevorgang wird gestartet!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region BLOCKED

                        else if (authorizationInfo.Allowed == AllowedType.BLOCKED)
                        {

                            responseText = "Sorry, your token is blocked!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Autorisierung fehlgeschlagen!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region EXPIRED

                        else if (authorizationInfo.Allowed == AllowedType.EXPIRED)
                        {

                            responseText = "Sorry, your token has expired!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Autorisierungstoken ungültig!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region NO_CREDIT

                        else if (authorizationInfo.Allowed == AllowedType.NO_CREDIT)
                        {

                            responseText = "Sorry, your have not enough credits for charging!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Nicht genügend Ladeguthaben!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region NOT_ALLOWED

                        else if (authorizationInfo.Allowed == AllowedType.NOT_ALLOWED)
                        {

                            responseText = "Sorry, charging is not allowed!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Autorisierung abgelehnt!";
                                        break;
                                }
                            }

                        }

                        #endregion

                        #region default

                        else
                        {

                            responseText = "An error occurred!";

                            if (authorizationInfo.Token?.UILanguage.HasValue == true)
                            {
                                switch (authorizationInfo.Token.UILanguage.Value)
                                {
                                    case Languages.de:
                                        responseText = "Ein Fehler ist aufgetreten!";
                                        break;
                                }
                            }

                        }

                        #endregion

                    }

                    #endregion


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = new AuthorizationInfo(
                                                          authorizationInfo.Allowed,
                                                          authorizationInfo.Token,
                                                          authorizationInfo.Location,
                                                          authorizationInfo.AuthorizationReference ?? AuthorizationReference.NewRandom(),
                                                          authorizationInfo.Info                   ?? new DisplayText(
                                                                                                          authorizationInfo.Token?.UILanguage ?? Languages.en,
                                                                                                          responseText
                                                                                                      ),
                                                          authorizationInfo.RemoteParty,
                                                          authorizationInfo.EMSPId,
                                                          authorizationInfo.Runtime
                                                      ).ToJSON(
                                                            CustomAuthorizationInfoSerializer,
                                                            CustomTokenSerializer,
                                                            CustomLocationReferenceSerializer,
                                                            CustomDisplayTextSerializer
                                                        ),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }
            );

            #endregion

            #endregion


            // Command result callbacks

            #region ~/commands/RESERVE_NOW/{command_id}

            #region OPTIONS  ~/commands/RESERVE_NOW/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/RESERVE_NOW/{command_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

                );

            #endregion

            #region POST     ~/commands/RESERVE_NOW/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/RESERVE_NOW/{command_id}",
                EMSPEvents.ReserveNowCallbackHTTPRequest,
                EMSPEvents.ReserveNowCallbackHTTPResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'RESERVE NOW' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    #endregion


                    if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                    {

                        commandValues.Result = commandResult;

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'RESERVE NOW' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #endregion

            #region ~/commands/CANCEL_RESERVATION/{command_id}

            #region OPTIONS  ~/commands/CANCEL_RESERVATION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/CANCEL_RESERVATION/{command_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/CANCEL_RESERVATION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/CANCEL_RESERVATION/{command_id}",
                EMSPEvents.CancelReservationCallbackHTTPRequest,
                EMSPEvents.CancelReservationCallbackHTTPResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'CANCEL RESERVATION' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    #endregion


                    if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                    {

                        commandValues.Result = commandResult;

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'CANCEL RESERVATION' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #endregion

            #region ~/commands/START_SESSION/{command_id}

            #region OPTIONS  ~/commands/START_SESSION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/START_SESSION/{command_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/START_SESSION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/START_SESSION/{command_id}",
                EMSPEvents.StartSessionCallbackHTTPRequest,
                EMSPEvents.StartSessionCallbackHTTPResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'START SESSION' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    #endregion


                    if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                    {

                        commandValues.Result = commandResult;

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'START SESSION' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #endregion

            #region ~/commands/STOP_SESSION/{command_id}

            #region OPTIONS  ~/commands/STOP_SESSION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/STOP_SESSION/{command_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/STOP_SESSION/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/STOP_SESSION/{command_id}",
                EMSPEvents.StopSessionCallbackHTTPRequest,
                EMSPEvents.StopSessionCallbackHTTPResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'STOP SESSION' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    #endregion


                    if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                    {

                        commandValues.Result = commandResult;

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'STOP SESSION' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #endregion

            #region ~/commands/UNLOCK_CONNECTOR/{command_id}

            #region OPTIONS  ~/commands/UNLOCK_CONNECTOR/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR/{command_id}",
                request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        })

            );

            #endregion

            #region POST     ~/commands/UNLOCK_CONNECTOR/{command_id}

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "commands/UNLOCK_CONNECTOR/{command_id}",
                EMSPEvents.UnlockConnectorCallbackHTTPRequest,
                EMSPEvents.UnlockConnectorCallbackHTTPResponse,
                request => {

                    #region Check command identification

                    if (!request.ParseCommandId(CommonAPI,
                                                out var commandId,
                                                out var ocpiResponseBuilder))
                    {
                        return Task.FromResult(ocpiResponseBuilder);
                    }

                    #endregion

                    #region Parse command result JSON

                    if (!request.TryParseJObjectRequestBody(out var json, out ocpiResponseBuilder))
                        return Task.FromResult(ocpiResponseBuilder);

                    if (!CommandResult.TryParse(json,
                                                out var commandResult,
                                                out var errorResponse))
                    {

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 2001,
                                       StatusMessage        = "Could not parse the given 'UNLOCK CONNECTOR' command result JSON: " + errorResponse,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    #endregion


                    if (CommonAPI.CommandValueStore.TryGetValue(commandId.Value, out var commandValues))
                    {

                        commandValues.Result = commandResult;

                        return Task.FromResult(
                                   new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Accepted,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   }
                               );

                    }

                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Unknown 'UNLOCK CONNECTOR' command identification!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

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
                   nameof(HUB_HTTPAPI),
                   Logger,
                   LogHandler,
                   EventName,
                   OICPCommand
               );

        #endregion


    }

}
