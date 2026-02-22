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

using System.Net.Security;
using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// A delegate for filtering remote parties.
    /// </summary>
    public delegate Boolean IncludeRemoteParty(RemoteParty RemoteParty);


    public delegate IEnumerable<Tariff>     GetTariffs2_Delegate  (Party_Idv3       CPOPartyId,
                                                                   Location_Id?     LocationId       = null,
                                                                   EVSE_Id?         EVSEId           = null,
                                                                   Connector_Id?    ConnectorId      = null,
                                                                   EMSP_Id?         EMSPId           = null);


    public delegate IEnumerable<Tariff_Id>  GetTariffIds2_Delegate(Party_Idv3       CPOPartyId,
                                                                   Location_Id?     LocationId       = null,
                                                                   EVSE_Id?         EVSEId           = null,
                                                                   Connector_Id?    ConnectorId      = null,
                                                                   EMSP_Id?         EMSPId           = null);

    public delegate Tariff?                 GetTariff2_Delegate   (Party_Idv3       CPOPartyId,
                                                                   Tariff_Id        TariffId,
                                                                   DateTimeOffset?  StartTimestamp   = null,
                                                                   TimeSpan?        Tolerance        = null);

    /// <summary>
    /// Extension methods for the Common HTTP API.
    /// </summary>
    public static class CommonAPIExtensions
    {

        #region ParsePartyId                        (this Request, CommonAPI, out PartyId, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the party identification
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParsePartyId(this OCPIRequest                                Request,
                                           CommonAPI                                       CommonAPI,
                                           [NotNullWhen(true)]  out Party_Idv3?            PartyId,
                                           [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            PartyId              = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<CountryCode>("country_code", CountryCode.TryParse, out var countryCode))
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

            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.   TryParse, out var partyId))
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


            PartyId = Party_Idv3.From(
                          countryCode,
                          partyId
                      );

            if (!CommonAPI.HasParty(PartyId.Value))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown party identification!",
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


        #region ParseMandatoryLocation              (this Request, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryLocation(this OCPIRequest                                Request,
                                                     CommonAPI                                       CommonAPI,
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


            if (!CommonAPI.TryGetLocation(
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

        #region ParseOptionalLocation               (this Request, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseOptionalLocation(this OCPIRequest                                 Request,
                                                    CommonAPI                                        CommonAPI,
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


            CommonAPI.TryGetLocation(
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

        #region ParseMandatoryLocation              (this Request, CommonAPI, CountryCodesWithPartyIds,     out LocationId, out Location,                                                        out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocation(this OCPIRequest                                Request,
                                                     CommonAPI                                       CommonAPI,
                                                     IEnumerable<Tuple<CountryCode, Party_Id>>       CountryCodesWithPartyIds,
                                                     [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                     [NotNullWhen(true)]  out Location?              Location,
                                                     [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            LocationId           =  default;
            Location             =  default;
            OCPIResponseBuilder  =  default;

            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id> ("location_Id", Location_Id.TryParse, out var locationId))
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


            if (!CommonAPI.TryGetLocation(CommonAPI.DefaultPartyId,
                                          locationId,
                                          out Location))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2003,
                    StatusMessage        = "Unknown location!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode              = HTTPStatusCode.NotFound,
                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                        AccessControlAllowHeaders   = [ "Authorization" ],
                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                    }
                };

                return false;

            }


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetLocation(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    LocationId.Value,
                    out Location))
                {
                    return true;
                }
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                                      StatusCode           = 2001,
                                      StatusMessage        = "Unknown location!",
                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                          HTTPStatusCode              = HTTPStatusCode.NotFound,
                                          AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                          AccessControlAllowHeaders   = [ "Authorization" ],
                                          AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                                      }
                                  };

            return false;

        }

        #endregion


        #region ParseMandatoryLocationEVSE          (this Request, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryLocationEVSE(this OCPIRequest                                Request,
                                                         CommonAPI                                       CommonAPI,
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


            if (!CommonAPI.TryGetLocation(
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

        #region ParseOptionalLocationEVSE           (this Request, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseOptionalLocationEVSE(this OCPIRequest                                 Request,
                                                        CommonAPI                                        CommonAPI,
                                                        [NotNullWhen(true)]   out CountryCode?           CountryCode,
                                                        [NotNullWhen(true)]   out Party_Id?              PartyId,
                                                        [NotNullWhen(true)]   out Location_Id?           LocationId,
                                                        [MaybeNullWhen(true)] out Location?              Location,
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


            CommonAPI.TryGetLocation(
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

        #region ParseMandatoryLocationEVSE          (this Request, CommonAPI, CountryCodesWithPartyIds,     out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocationEVSE(this OCPIRequest                                Request,
                                                         CommonAPI                                       CommonAPI,
                                                         IEnumerable<Tuple<CountryCode, Party_Id>>       CountryCodesWithPartyIds,
                                                         [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                         [NotNullWhen(true)]  out Location?              Location,
                                                         [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                         [NotNullWhen(true)]  out EVSE?                  EVSE,
                                                         [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id> ("locationId", Location_Id.TryParse, out var locationId))
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

            LocationId = locationId;


            if (!Request.HTTPRequest.TryParseURLParameter<EVSE_UId>    ("evseId",     EVSE_UId.   TryParse, out var evseUId))
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

            EVSEUId = evseUId;


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetLocation(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    locationId,
                    out Location))
                {

                    if (!Location.TryGetEVSE(evseUId, out EVSE)) {

                        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                            StatusCode           = 2001,
                            StatusMessage        = "Unknown EVSE!",
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
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                                      StatusCode           = 2001,
                                      StatusMessage        = "Unknown location!",
                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                          HTTPStatusCode             = HTTPStatusCode.NotFound,
                                          //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                      }
                                  };

            return false;

        }

        #endregion


        #region ParseMandatoryLocationEVSEConnector (this Request, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryLocationEVSEConnector(this OCPIRequest                                Request,
                                                                  CommonAPI                                       CommonAPI,
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


            if (!CommonAPI.TryGetLocation(
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

        #region ParseOptionalLocationEVSEConnector  (this Request, CommonAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseOptionalLocationEVSEConnector(this OCPIRequest                                 Request,
                                                                 CommonAPI                                        CommonAPI,
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


            if (!CommonAPI.TryGetLocation(
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

        #region ParseMandatoryLocationEVSEConnector (this Request, CommonAPI, CountryCodesWithPartyIds,     out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocationEVSEConnector(this OCPIRequest                                Request,
                                                                  CommonAPI                                       CommonAPI,
                                                                  IEnumerable<Tuple<CountryCode, Party_Id>>       CountryCodesWithPartyIds,
                                                                  [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                                  [NotNullWhen(true)]  out Location?              Location,
                                                                  [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                                  [NotNullWhen(true)]  out EVSE?                  EVSE,
                                                                  [NotNullWhen(true)]  out Connector_Id?          ConnectorId,
                                                                  [NotNullWhen(true)]  out Connector?             Connector,
                                                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            ConnectorId          = default;
            Connector            = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id> ("locationId",   Location_Id. TryParse, out var locationId))
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


            if (!Request.HTTPRequest.TryParseURLParameter<EVSE_UId>    ("evseId",       EVSE_UId.    TryParse, out var evseUId))
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


            if (!Request.HTTPRequest.TryParseURLParameter<Connector_Id>("connectorId",  Connector_Id.TryParse, out var connectorId))
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


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetLocation(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    LocationId.Value,
                    out Location))
                {

                    if (!Location.TryGetEVSE(EVSEUId.Value, out EVSE) ||
                         EVSE is null) {

                        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                            StatusCode           = 2001,
                            StatusMessage        = "Unknown EVSE!",
                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.NotFound,
                                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
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
                                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        };

                        return false;

                    }

                    return true;

                }
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                                      StatusCode           = 2001,
                                      StatusMessage        = "Unknown location!",
                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                          HTTPStatusCode             = HTTPStatusCode.NotFound,
                                          //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                      }
                                  };

            return false;

        }

        #endregion


        #region ParseMandatoryTariff                (this Request, CommonAPI, out CountryCode, out PartyId, out TariffId,  out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryTariff(this OCPIRequest                                Request,
                                                   CommonAPI                                       CommonAPI,
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


            if (!CommonAPI.TryGetTariff(
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

        #region ParseOptionalTariff                 (this Request, CommonAPI, out CountryCode, out PartyId, out TariffId,  out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseOptionalTariff(this OCPIRequest                                  Request,
                                                  CommonAPI                                         CommonAPI,
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


            CommonAPI.TryGetTariff(
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

        #region ParseMandatoryTariff                (this Request, CommonAPI, CountryCodesWithPartyIds,     out TariffId,   out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryTariff(this OCPIRequest                                Request,
                                                   CommonAPI                                       CommonAPI,
                                                   IEnumerable<Tuple<CountryCode, Party_Id>>       CountryCodesWithPartyIds,
                                                   [NotNullWhen(true)]  out Tariff_Id?             TariffId,
                                                   [NotNullWhen(true)]  out Tariff?                Tariff,
                                                   [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            TariffId             =  default;
            Tariff               =  default;
            OCPIResponseBuilder  =  default;

            if (!Request.HTTPRequest.TryParseURLParameter<Tariff_Id>("tariffId", Tariff_Id.TryParse, out var tariffId))
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


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetTariff(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    tariffId,
                    out Tariff))
                {
                    return true;
                }
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                                      StatusCode           = 2003,
                                      StatusMessage        = "Unknown tariff!",
                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                          HTTPStatusCode             = HTTPStatusCode.NotFound,
                                          //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                      }
                                  };

            return false;

        }

        #endregion


        #region ParseSessionId                      (this Request, CommonAPI, out CountryCode, out PartyId, out SessionId,               out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseSessionId(this OCPIRequest                                  Request,
                                             CommonAPI                                         CommonAPI,
                                             [NotNullWhen  (true)]  out CountryCode?           CountryCode,
                                             [NotNullWhen  (true)]  out Party_Id?              PartyId,
                                             [NotNullWhen  (true)]  out Session_Id?            SessionId,
                                             [NotNullWhen  (false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            SessionId            = default;
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

            return true;

        }

        #endregion

        #region ParseMandatorySession               (this Request, CommonAPI, out CountryCode, out PartyId, out SessionId, out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatorySession(this OCPIRequest                                Request,
                                                    CommonAPI                                       CommonAPI,
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


            if (!CommonAPI.TryGetSession(Party_Idv3.From(countryCode, partyId), sessionId, out Session))
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

        #region ParseOptionalSession                (this Request, CommonAPI, out CountryCode, out PartyId, out SessionId, out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseOptionalSession(this OCPIRequest                                  Request,
                                                   CommonAPI                                         CommonAPI,
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


            CommonAPI.TryGetSession(
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

        #region ParseMandatorySession               (this Request, CommonAPI, CountryCodesWithPartyIds,     out SessionId,  out Session,                                                          out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatorySession(this OCPIRequest                                Request,
                                                    CommonAPI                                       CommonAPI,
                                                    IEnumerable<Tuple<CountryCode, Party_Id>>       CountryCodesWithPartyIds,
                                                    [NotNullWhen(true)]  out Session_Id?            SessionId,
                                                    [NotNullWhen(true)]  out Session?               Session,
                                                    [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            SessionId            =  default;
            Session              =  default;
            OCPIResponseBuilder  =  default;

            if (!Request.HTTPRequest.TryParseURLParameter<Session_Id> ("session_id", Session_Id.TryParse, out var sessionId))
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

            SessionId = sessionId;


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetSession(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    sessionId,
                    out Session))
                {
                    return true;
                }
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                                      StatusCode           = 2003,
                                      StatusMessage        = "Unknown session!",
                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                          HTTPStatusCode             = HTTPStatusCode.NotFound,
                                          //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                      }
                                  };

            return false;

        }

        #endregion


        #region ParseMandatoryCDR                   (this Request, CommonAPI, out CountryCode, out PartyId, out CDRId,     out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryCDR(this OCPIRequest                                Request,
                                                CommonAPI                                       CommonAPI,
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


            if (!Request.HTTPRequest.TryParseURLParameter<CDR_Id>     ("cdr_Id",       CDR_Id.          TryParse, out var cdrId))
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


            if (!CommonAPI.TryGetCDR(
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

        #region ParseOptionalCDR                    (this Request, CommonAPI, out CountryCode, out PartyId, out CDRId,     out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseOptionalCDR(this OCPIRequest                                  Request,
                                               CommonAPI                                         CommonAPI,
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


            if (!Request.HTTPRequest.TryParseURLParameter<CDR_Id>   ("cdr_Id",     CDR_Id.        TryParse, out var cdrId))
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


            CommonAPI.TryGetCDR(
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

        #region ParseMandatoryCDR                   (this Request, CommonAPI, CountryCodesWithPartyIds,     out CDRId,      out CDR,                                                              out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the CDR identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CDRId">The parsed unique CDR identification.</param>
        /// <param name="CDR">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryCDR(this OCPIRequest                                Request,
                                                CommonAPI                                       CommonAPI,
                                                IEnumerable<Tuple<CountryCode, Party_Id>>       CountryCodesWithPartyIds,
                                                [NotNullWhen(true)]  out CDR_Id?                CDRId,
                                                [NotNullWhen(true)]  out CDR?                   CDR,
                                                [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CDRId                =  default;
            CDR                  =  default;
            OCPIResponseBuilder  =  default;

            if (!Request.HTTPRequest.TryParseURLParameter<CDR_Id>("cdrId", CDR_Id.TryParse, out var cdrId))
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


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetCDR(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    CDRId.Value,
                    out CDR))
                {
                    return true;
                }
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                StatusCode           = 2003,
                StatusMessage        = "Unknown CDR!",
                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode             = HTTPStatusCode.NotFound,
                    //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                    AccessControlAllowHeaders  = [ "Authorization" ]
                }
            };

            return false;

        }

        #endregion


        #region ParseTokenId                        (this Request, CommonAPI,                               out TokenId,                 out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseTokenId(this OCPIRequest                                Request,
                                           [NotNullWhen(true)]  out Token_Id?              TokenId,
                                           [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            TokenId              = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<Token_Id>("token_id", Token_Id.TryParse, out var tokenId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            TokenId = tokenId;

            return true;

        }

        #endregion

        #region ParseTokenId                        (this Request, CommonAPI, out CountryCode, out PartyId, out TokenId,                                           out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique tariff identification.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseTokenId(this OCPIRequest                                Request,
                                           [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                           [NotNullWhen(true)]  out Party_Id?              PartyId,
                                           [NotNullWhen(true)]  out Token_Id?              TokenId,
                                           [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            TokenId              = default;
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

            PartyId = partyId;


            if (!Request.HTTPRequest.TryParseURLParameter<Token_Id>("token_Id", Token_Id.TryParse, out var tokenId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            TokenId = tokenId;

            return true;

        }

        #endregion

        #region ParseMandatoryToken                 (this Request, CommonAPI,                               out TokenId,   out Token,    out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="TokenStatus">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryToken(this OCPIRequest                                Request,
                                                  CommonAPI                                       CommonAPI,
                                                  [NotNullWhen(true)]  out Token_Id?              TokenId,
                                                  [NotNullWhen(true)]  out TokenStatus?           TokenStatus,
                                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            TokenId              = default;
            TokenStatus          = null;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }


            if (!CommonAPI.TryGetTokenStatus(
                Request.To ?? CommonAPI.DefaultPartyId,
                TokenId.Value,
                out TokenStatus))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown token identification!",
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

        #region ParseOptionalToken                  (this Request, CommonAPI,                               out TokenId,   out Token,    out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the token identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="TokenId">The parsed unique token identification.</param>
        /// <param name="TokenStatus">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseOptionalToken(this OCPIRequest                                  Request,
                                                 CommonAPI                                         CommonAPI,
                                                 [NotNullWhen  (true)]  out Token_Id?              TokenId,
                                                 [MaybeNullWhen(true)]  out TokenStatus?           TokenStatus,
                                                 [NotNullWhen  (false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            TokenId              = default;
            TokenStatus          = null;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }


            CommonAPI.TryGetTokenStatus(
                Request.To ?? CommonAPI.DefaultPartyId,
                TokenId.Value,
                out TokenStatus);

            return true;

        }

        #endregion

        #region ParseMandatoryToken                 (this Request, CommonAPI, out CountryCode, out PartyId, out TokenId, out TokenStatus,                          out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique tariff identification.</param>
        /// <param name="TokenStatus">The resolved tariff with status.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryToken(this OCPIRequest                                Request,
                                                  CommonAPI                                       CommonAPI,
                                                  IEnumerable<Tuple<CountryCode, Party_Id>>       CountryCodesWithPartyIds,
                                                  [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                  [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                  [NotNullWhen(true)]  out Token_Id?              TokenId,
                                                  [NotNullWhen(true)]  out TokenStatus?           TokenStatus,
                                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetTokenStatus(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    TokenId.Value,
                    out TokenStatus) &&
                    TokenStatus.Token is not null)
                {

                    if (TokenStatus.Token.CountryCode != CountryCode ||
                        TokenStatus.Token.PartyId     != PartyId)
                    {

                        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                            StatusCode           = 2004,
                            StatusMessage        = "Invalid token identification!",
                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.UnprocessableEntity,
                                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        };

                    }

                    return true;

                }
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                StatusCode           = 2004,
                StatusMessage        = "Unknown token identification!",
                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode             = HTTPStatusCode.NotFound,
                    //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                    AccessControlAllowHeaders  = [ "Authorization" ]
                }
            };

            return false;

        }

        #endregion

        #region ParseMandatoryToken                 (this Request, CommonAPI, out CountryCode, out PartyId, out TokenId, out TokenStatus,                          out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="PartyIds">The allowed party identifications.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique tariff identification.</param>
        /// <param name="TokenStatus">The resolved tariff with status.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryToken(this OCPIRequest                                Request,
                                                  CommonAPI                                       CommonAPI,
                                                  IEnumerable<Party_Idv3>                         PartyIds,
                                                  [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                  [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                  [NotNullWhen(true)]  out Token_Id?              TokenId,
                                                  [NotNullWhen(true)]  out TokenStatus?           TokenStatus,
                                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }


            foreach (var partyId in PartyIds)
            {
                if (CommonAPI.TryGetTokenStatus(
                    partyId,
                    TokenId.Value,
                    out TokenStatus) &&
                    TokenStatus.Token is not null)
                {

                    if (TokenStatus.Token.CountryCode != CountryCode ||
                        TokenStatus.Token.PartyId     != PartyId)
                    {

                        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                            StatusCode           = 2004,
                            StatusMessage        = "Invalid token identification!",
                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.UnprocessableEntity,
                                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        };

                    }

                    return true;

                }
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                StatusCode           = 2004,
                StatusMessage        = "Unknown token identification!",
                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode             = HTTPStatusCode.NotFound,
                    //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                    AccessControlAllowHeaders  = [ "Authorization" ]
                }
            };

            return false;

        }

        #endregion

        #region ParseOptionalToken                  (this Request, CommonAPI, out CountryCode, out PartyId, out TokenId, out TokenStatus,                          out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique tariff identification.</param>
        /// <param name="TokenStatus">The resolved tariff with status.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseOptionalToken(this OCPIRequest                                  Request,
                                                 CommonAPI                                         CommonAPI,
                                                 IEnumerable<Tuple<CountryCode, Party_Id>>         CountryCodesWithPartyIds,
                                                 [NotNullWhen  (true)]  out CountryCode?           CountryCode,
                                                 [NotNullWhen  (true)]  out Party_Id?              PartyId,
                                                 [NotNullWhen  (true)]  out Token_Id?              TokenId,
                                                 [MaybeNullWhen(true)]  out TokenStatus?           TokenStatus,
                                                 [NotNullWhen  (false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetTokenStatus(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    TokenId.Value,
                    out TokenStatus) &&
                    TokenStatus.Token is not null)
                {

                    if (TokenStatus.Token.CountryCode != CountryCode ||
                        TokenStatus.Token.PartyId     != PartyId)
                    {

                        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                            StatusCode           = 2004,
                            StatusMessage        = "Invalid token identification!",
                            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.UnprocessableEntity,
                                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                AccessControlAllowHeaders  = [ "Authorization" ]
                            }
                        };

                    }

                    return true;

                }
            }

            return true;

        }

        #endregion


        #region ParseTerminalId                     (this Request, CommonAPI, out TerminalId,                                            out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the terminal identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The EMSP API.</param>
        /// <param name="TerminalId">The parsed unique terminal identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParsePaymentTerminalId(this OCPIRequest                                Request,
                                                     CommonAPI                                       CommonAPI,
                                                     [NotNullWhen(true)]  out Terminal_Id?           TerminalId,
                                                     [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            TerminalId           = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<Terminal_Id>("terminalId", Terminal_Id.TryParse, out var terminalId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            TerminalId = terminalId;

            return true;

        }

        #endregion

        #region ParsePaymentTerminal                (this Request, CommonAPI, out TerminalId, out Terminal,                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the terminal identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="TerminalId">The parsed unique terminal identification.</param>
        /// <param name="Terminal">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParsePaymentTerminal(this OCPIRequest                                  Request,
                                                   CommonAPI                                         CommonAPI,
                                                   [NotNullWhen(true)]    out Terminal_Id?           TerminalId,
                                                   [MaybeNullWhen(true)]  out Terminal?              Terminal,
                                                   [NotNullWhen(false)]   out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                   Boolean                                           FailOnMissingTerminal = true)
        {

            TerminalId           =  default;
            Terminal             =  default;
            OCPIResponseBuilder  =  default;

            if (!Request.HTTPRequest.TryParseURLParameter<Terminal_Id>("terminalId", Terminal_Id.TryParse, out var terminalId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            TerminalId = terminalId;


            if (!CommonAPI.TryGetPaymentTerminal(
                CommonAPI.DefaultPartyId,
                terminalId,
                out Terminal))
            {

                if (FailOnMissingTerminal)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2003,
                        StatusMessage        = "Unknown terminal!",
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


            //foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            //{
            //    if (CommonAPI.TryGetTerminal(Party_Idv3.From(
            //                                     countryCodeWithPartyId.Item1,
            //                                     countryCodeWithPartyId.Item2
            //                                 ),
            //                                 TerminalId.Value,
            //                                 out Terminal))
            //    {
            //        return true;
            //    }
            //}


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                StatusCode           = 2001,
                StatusMessage        = "Unknown terminal!",
                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode             = HTTPStatusCode.NotFound,
                    //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                    AccessControlAllowHeaders  = [ "Authorization" ]
                }
            };

            return false;

        }

        #endregion

        #region ParseMandatoryPaymentTerminal       (this Request, CommonAPI, out TerminalId, out Terminal,                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the terminal identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="TerminalId">The parsed unique terminal identification.</param>
        /// <param name="Terminal">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseMandatoryPaymentTerminal(this OCPIRequest                                Request,
                                                            CommonAPI                                       CommonAPI,
                                                            [NotNullWhen(true)]  out Terminal_Id?           TerminalId,
                                                            [NotNullWhen(true)]  out Terminal?              Terminal,
                                                            [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                            Boolean                                         FailOnMissingTerminal = true)
        {

            TerminalId           =  default;
            Terminal             =  default;
            OCPIResponseBuilder  =  default;

            if (!Request.HTTPRequest.TryParseURLParameter<Terminal_Id>("terminalId", Terminal_Id.TryParse, out var terminalId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            TerminalId = terminalId;


            if (!CommonAPI.TryGetPaymentTerminal(
                CommonAPI.DefaultPartyId,
                TerminalId.Value,
                out Terminal))
            {

                if (FailOnMissingTerminal)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2003,
                        StatusMessage        = "Unknown terminal!",
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


            //foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            //{
            //    if (CommonAPI.TryGetTerminal(Party_Idv3.From(
            //                                     countryCodeWithPartyId.Item1,
            //                                     countryCodeWithPartyId.Item2
            //                                 ),
            //                                 TerminalId.Value,
            //                                 out Terminal))
            //    {
            //        return true;
            //    }
            //}


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                StatusCode           = 2001,
                StatusMessage        = "Unknown terminal!",
                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode             = HTTPStatusCode.NotFound,
                    //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                    AccessControlAllowHeaders  = [ "Authorization" ]
                }
            };

            return false;

        }

        #endregion

        #region ParsePaymentTerminal                (this Request, CommonAPI, out TerminalId, out Terminal,                                                        out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the terminal identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="TerminalId">The parsed unique terminal identification.</param>
        /// <param name="Terminal">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParsePaymentTerminal(this OCPIRequest                                  Request,
                                                   CommonAPI                                         CommonAPI,
                                                   IEnumerable<Tuple<CountryCode, Party_Id>>         CountryCodesWithPartyIds,
                                                   [NotNullWhen(true)]    out Terminal_Id?           TerminalId,
                                                   [MaybeNullWhen(true)]  out Terminal?              Terminal,
                                                   [NotNullWhen(false)]   out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                   Boolean                                           FailOnMissingTerminal = true)
        {

            TerminalId           =  default;
            Terminal             =  default;
            OCPIResponseBuilder  =  default;

            if (!Request.HTTPRequest.TryParseURLParameter<Terminal_Id>("terminalId", Terminal_Id.TryParse, out var terminalId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid token identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            TerminalId = terminalId;


            if (!CommonAPI.TryGetPaymentTerminal(
                CommonAPI.DefaultPartyId,
                TerminalId.Value,
                out Terminal))
            {

                if (FailOnMissingTerminal)
                {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2003,
                        StatusMessage        = "Unknown terminal!",
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


            foreach (var countryCodeWithPartyId in CountryCodesWithPartyIds)
            {
                if (CommonAPI.TryGetPaymentTerminal(
                    Party_Idv3.From(
                        countryCodeWithPartyId.Item1,
                        countryCodeWithPartyId.Item2
                    ),
                    TerminalId.Value,
                    out Terminal))
                {
                    return true;
                }
            }


            OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                StatusCode           = 2001,
                StatusMessage        = "Unknown terminal!",
                HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                    HTTPStatusCode             = HTTPStatusCode.NotFound,
                    //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                    AccessControlAllowHeaders  = [ "Authorization" ]
                }
            };

            return false;

        }

        #endregion


        #region ParseCommandId                      (this Request, CommonAPI,                               out CommandId,                                         out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the command identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The EMSP API.</param>
        /// <param name="CommandId">The parsed unique command identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseCommandId(this OCPIRequest                                Request,
                                             CommonAPI                                       CommonAPI,
                                             [NotNullWhen(true)]  out Command_Id?            CommandId,
                                             [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CommandId            = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<Command_Id>("command_id", Command_Id.TryParse, out var commandId))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid command identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            CommandId = commandId;

            return true;

        }

        #endregion

    }


    /// <summary>
    /// The CommonAPI.
    /// </summary>
    public class CommonAPI : AHTTPExtAPIXExtension<HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const        String  DefaultHTTPServerName           = $"GraphDefined OCPI {Version.String} Common HTTP API";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const        String  DefaultHTTPServiceName          = $"GraphDefined OCPI {Version.String} Common HTTP API";

        /// <summary>
        /// The default log file name.
        /// </summary>
        public static readonly  String  DefaultLogfileName              = $"CommonAPI_OCPI{Version.String}.log";

        /// <summary>
        /// The default database file name for all remote party configuration.
        /// </summary>
        public const            String  DefaultRemotePartyDBFileName    = $"RemoteParties_{Version.String}.db";

        /// <summary>
        /// The default database file name for all OCPI assets.
        /// </summary>
        public const            String  DefaultAssetsDBFileName         = $"Assets_{Version.String}.db";


        /// <summary>
        /// The command values store.
        /// </summary>
        public readonly ConcurrentDictionary<Command_Id, CommandValues>  CommandValueStore  = [];

        #endregion

        #region Properties

        /// <summary>
        /// The Common HTTP API.
        /// </summary>
        public CommonHTTPAPI            BaseAPI                     { get; }

        /// <summary>
        /// The (max supported) OCPI version.
        /// </summary>
        public Version_Id               OCPIVersion                 { get; } = Version.Id;


        /// <summary>
        /// The default party identification to use.
        /// </summary>
        public Party_Idv3               DefaultPartyId             { get; }



        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?                 AllowDowngrades            { get; }

        /// <summary>
        /// Whether to keep or delete EVSEs marked as "REMOVED".
        /// </summary>
        public Func<EVSE, Boolean>      KeepRemovedEVSEs           { get; }

        /// <summary>
        /// The Common API logger.
        /// </summary>
        public CommonAPILogger?         Logger                     { get; }



        public String                   DatabaseFilePath           { get; }

        /// <summary>
        /// The database file name for all remote party configuration.
        /// </summary>
        public String                   RemotePartyDBFileName      { get; protected set; }

        /// <summary>
        /// The database file name for all OCPI assets.
        /// </summary>
        public String                   AssetsDBFileName           { get; }

        #endregion

        #region Events

        #region (protected internal) GetVersionsRequest        (Request)

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionsRequest = new();

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetVersionsRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnGetVersionsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetVersionsResponse       (Response)

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionsResponse = new();

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetVersionsResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    OCPIRequest        Request,
                                                    OCPIResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnGetVersionsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) GetVersionRequest         (Request)

        /// <summary>
        /// An event sent whenever a GET version request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionRequest = new();

        /// <summary>
        /// An event sent whenever a GET version request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetVersionRequest(DateTimeOffset     Timestamp,
                                                  HTTPAPIX           API,
                                                  OCPIRequest        Request,
                                                  CancellationToken  CancellationToken)

            => OnGetVersionRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetVersionResponse        (Response)

        /// <summary>
        /// An event sent whenever a GET version response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionResponse = new();

        /// <summary>
        /// An event sent whenever a GET version response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetVersionResponse(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   OCPIRequest        Request,
                                                   OCPIResponse       Response,
                                                   CancellationToken  CancellationToken)

            => OnGetVersionResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) GetCredentialsRequest     (Request)

        /// <summary>
        /// An event sent whenever a GET credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCredentialsRequest = new();

        /// <summary>
        /// An event sent whenever a GET credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCredentialsRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnGetCredentialsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetCredentialsResponse    (Response)

        /// <summary>
        /// An event sent whenever a GET credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCredentialsResponse = new();

        /// <summary>
        /// An event sent whenever a GET credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCredentialsResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnGetCredentialsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PostCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a POST credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCredentialsRequest = new();

        /// <summary>
        /// An event sent whenever a POST credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCredentialsRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

            => OnPostCredentialsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PostCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a POST credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCredentialsResponse = new();

        /// <summary>
        /// An event sent whenever a POST credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCredentialsResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

            => OnPostCredentialsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) PutCredentialsRequest     (Request)

        /// <summary>
        /// An event sent whenever a PUT credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutCredentialsRequest = new();

        /// <summary>
        /// An event sent whenever a PUT credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutCredentialsRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

            => OnPutCredentialsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) PutCredentialsResponse    (Response)

        /// <summary>
        /// An event sent whenever a PUT credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutCredentialsResponse = new();

        /// <summary>
        /// An event sent whenever a PUT credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutCredentialsResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

            => OnPutCredentialsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) DeleteCredentialsRequest  (Request)

        /// <summary>
        /// An event sent whenever a DELETE credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCredentialsRequest = new();

        /// <summary>
        /// An event sent whenever a DELETE credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCredentialsRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

            => OnDeleteCredentialsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) DeleteCredentialsResponse (Response)

        /// <summary>
        /// An event sent whenever a DELETE credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCredentialsResponse = new();

        /// <summary>
        /// An event sent whenever a DELETE credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCredentialsResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

            => OnDeleteCredentialsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<VersionInformation>?            CustomVersionInformationSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<VersionDetail>?                 CustomVersionDetailSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<VersionEndpoint>?               CustomVersionEndpointSerializer               { get; set; }

        public CustomJObjectSerializerDelegate<Terminal>?                      CustomTerminalSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<Location>?                      CustomLocationSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<PublishToken>?                  CustomPublishTokenSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?         CustomAdditionalGeoLocationSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                          CustomEVSESerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?                CustomStatusScheduleSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                     CustomConnectorSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<Location>>?         CustomLocationEnergyMeterSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?             CustomEVSEEnergyMeterSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?    CustomTransparencySoftwareStatusSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?          CustomTransparencySoftwareSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<Parking>?                       CustomParkingSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<DisplayText>?                   CustomDisplayTextSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?               CustomBusinessDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                         CustomHoursSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVSEParking>?                   CustomEVSEParkingSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                         CustomImageSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                     CustomEnergyMixSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                  CustomEnergySourceSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?           CustomEnvironmentalImpactSerializer           { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                        CustomTariffSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                         CustomPriceSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<PriceLimit>?                    CustomPriceLimitSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?                 CustomTariffElementSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?                CustomPriceComponentSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?            CustomTariffRestrictionsSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<TaxAmount>?                     CustomTaxAmountSerializer                     { get; set; }


        public CustomJObjectSerializerDelegate<TokenStatus>?                   CustomTokenStatusSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<Token>?                         CustomTokenSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyContract>?                CustomEnergyContractSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<LocationReference>?             CustomLocationReferenceSerializer             { get; set; }

        public CustomJObjectSerializerDelegate<Session>?                       CustomSessionSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CDR>?                           CustomCDRSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CDRToken>?                      CustomCDRTokenSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<CDRLocation>?                   CustomCDRLocationSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?                CustomChargingPeriodSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                  CustomCDRDimensionSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                    CustomSignedDataSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                   CustomSignedValueSerializer                   { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CommonAPI.
        /// </summary>
        /// 
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// 
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        public CommonAPI(IEnumerable<PartyData>        OurPartyData,
                         Party_Idv3                    DefaultPartyId,

                         CommonHTTPAPI                 BaseAPI,

                         I18NString?                   Description               = null,
                         HTTPPath?                     AdditionalURLPathPrefix   = null,
                         Func<EVSE, Boolean>?          KeepRemovedEVSEs          = null,

                         HTTPPath?                     BasePath                  = null,
                         HTTPPath?                     URLPathPrefix             = null,

                         String?                       ExternalDNSName           = null,
                         String?                       HTTPServerName            = DefaultHTTPServerName,
                         String?                       HTTPServiceName           = DefaultHTTPServiceName,
                         String?                       APIVersionHash            = null,
                         JObject?                      APIVersionHashes          = null,

                         String?                       DatabaseFilePath          = null,
                         String?                       RemotePartyDBFileName     = null,
                         String?                       AssetsDBFileName          = null,

                         Boolean?                      IsDevelopment             = false,
                         IEnumerable<String>?          DevelopmentServers        = null,
                         Boolean?                      DisableLogging            = false,
                         String?                       LoggingContext            = null,
                         String?                       LoggingPath               = null,
                         String?                       LogfileName               = null,
                         OCPILogfileCreatorDelegate?   LogfileCreator            = null)

            : base(Description ?? I18NString.Create($"OCPI{Version.String} Common HTTP API"),
                   BaseAPI.HTTPBaseAPI,
                   BaseAPI.URLPathPrefix + URLPathPrefix,
                   BasePath,

                   ExternalDNSName,
                   HTTPServerName  ?? DefaultHTTPServerName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   APIVersionHash,
                   APIVersionHashes,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null)

        {

            if (!OurPartyData.Any())
                throw new ArgumentNullException(nameof(OurPartyData), "The given party data must not be null or empty!");

            this.BaseAPI                   = BaseAPI;
            this.DefaultPartyId            = DefaultPartyId;

            this.KeepRemovedEVSEs          = KeepRemovedEVSEs                   ?? (evse => true);

            this.DatabaseFilePath          = DatabaseFilePath                   ?? Path.Combine(
                                                                                       AppContext.BaseDirectory,
                                                                                       DefaultHTTPAPI_LoggingPath
                                                                                   );

            if (this.DatabaseFilePath[^1] != Path.DirectorySeparatorChar)
                this.DatabaseFilePath     += Path.DirectorySeparatorChar;

            this.RemotePartyDBFileName     = Path.Combine(this.DatabaseFilePath,
                                                          RemotePartyDBFileName ?? DefaultRemotePartyDBFileName);

            this.AssetsDBFileName          = Path.Combine(this.DatabaseFilePath,
                                                          AssetsDBFileName      ?? DefaultAssetsDBFileName);

            this.Logger                    = this.DisableLogging == false
                                                 ? new CommonAPILogger(
                                                       this,
                                                       LoggingContext,
                                                       LoggingPath,
                                                       LogfileCreator
                                                   )
                                                 : null;

            this.BaseAPI.AddVersionInformation(
                new VersionInformation(
                    Version.Id,
                    URL.Concat(
                        BaseAPI.OurVersionsURL.Protocol.AsString(),
                        BaseAPI.ExternalDNSName ?? ("localhost:" + BaseAPI.HTTPBaseAPI.HTTPServer.TCPPort),
                        BaseAPI.URLPathPrefix + URLPathPrefix + AdditionalURLPathPrefix + $"/versions/{Version.Id}"
                    )
                )
            ).GetAwaiter().GetResult();

            ReadRemotePartyDatabaseFile().GetAwaiter().GetResult();
            ReadAssetsDatabaseFile().     GetAwaiter().GetResult();

            RegisterURLTemplates();

            foreach (var partyData in OurPartyData)
                parties.TryAdd(partyData.Id, partyData);

        }

        #endregion


        #region GetModuleURL(ModuleId, Prefix = "")

        /// <summary>
        /// Return the URL of an OCPI module.
        /// </summary>
        /// <param name="ModuleId">The identification of an OCPI module.</param>
        /// <param name="Prefix">An optional prefix.</param>
        public URL GetModuleURL(Module_Id  ModuleId,
                                String     Prefix   = "")

            => BaseAPI.OurBaseURL + Prefix + ModuleId.ToString();

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region OPTIONS     ~/versions/2.3.0

            // -------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions/2.3.0
            // -------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + $"versions/{Version.Id}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Server                     = HTTPServiceName,
                                Date                       = Timestamp.Now,
                                AccessControlAllowOrigin   = "*",
                                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                Connection                 = ConnectionType.KeepAlive,
                                Vary                       = "Accept"
                            }
                        })

            );

            #endregion

            #region GET         ~/versions/2.3.0

            // --------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions/2.3.0
            // --------------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + $"versions/{Version.Id}",
                GetVersionRequest,
                GetVersionResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is not null &&
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    Server                     = HTTPServiceName,
                                    Date                       = Timestamp.Now,
                                    AccessControlAllowOrigin   = "*",
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                    AccessControlAllowHeaders  = [ "Authorization" ],
                                    Connection                 = ConnectionType.KeepAlive,
                                    Vary                       = "Accept"
                                }
                            });

                    }

                    #endregion


                    var prefix = URLPathPrefix + BaseAPI.AdditionalURLPathPrefix + Version.String;

                    #region Common credential endpoints...

                    var endpoints = new List<VersionEndpoint>() {

                                         //Note: OCPI seems to only require one role here.
                                         //      The specification is quite unclear about this.

                                         //new (
                                         //    Module_Id.Credentials,
                                         //    InterfaceRoles.SENDER,
                                         //    URL.Parse(
                                         //        BaseAPI.OurVersionsURL.Protocol.AsString() +
                                         //            (request.Host + (prefix + "credentials")).Replace("//", "/")
                                         //),

                                         new (
                                             Module_Id.Credentials,
                                             InterfaceRoles.RECEIVER,
                                             URL.Parse(
                                                 BaseAPI.OurVersionsURL.Protocol.AsString() +
                                                     (request.Host + (prefix + "credentials")).Replace("//", "/")
                                             )
                                         )

                                    };

                    #endregion


                    #region We are an EMSP...

                    if (request.CommonAPI.Parties.Any(partyData => partyData.Role == Role.EMSP))
                    {

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Locations,
                                InterfaceRoles.RECEIVER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/locations")).Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Tariffs,
                                InterfaceRoles.RECEIVER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/tariffs")).  Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Sessions,
                                InterfaceRoles.RECEIVER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/sessions")). Replace("//", "/"))
                            )
                        );

                        // When the EMSP acts as smart charging receiver so that a SCSP can talk to him!
                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.ChargingProfiles,
                                InterfaceRoles.SENDER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/chargingprofiles")).Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.CDRs,
                                InterfaceRoles.RECEIVER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/cdrs")).     Replace("//", "/"))
                            )
                        );


                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Commands,
                                InterfaceRoles.SENDER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/commands")). Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Tokens,
                                InterfaceRoles.SENDER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/tokens")).   Replace("//", "/"))
                            )
                        );

                        // hubclientinfo

                    }

                    #endregion

                    #region We are a  CPO...

                    if (request.CommonAPI.Parties.Any(partyData => partyData.Role == Role.CPO))
                    {

                        // Open Data for all!
                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Locations,
                                InterfaceRoles.SENDER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/locations")).       Replace("//", "/"))
                            )
                        );

                        // Open Data for all!
                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Tariffs,
                                InterfaceRoles.SENDER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/tariffs")).         Replace("//", "/"))
                            )
                        );

                        if (request.RemoteParty is not null)
                        {

                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.Sessions,
                                    InterfaceRoles.SENDER,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/sessions")).        Replace("//", "/"))
                                )
                            );

                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.ChargingProfiles,
                                    InterfaceRoles.RECEIVER,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/chargingprofiles")).Replace("//", "/"))
                                )
                            );

                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.CDRs,
                                    InterfaceRoles.SENDER,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/cdrs")).            Replace("//", "/"))
                                )
                            );


                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.Commands,
                                    InterfaceRoles.RECEIVER,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/commands")).        Replace("//", "/"))
                                )
                            );

                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.Tokens,
                                    InterfaceRoles.RECEIVER,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/tokens")).          Replace("//", "/"))
                                )
                            );

                        }

                    }

                    #endregion

                    // hubclientinfo


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            StatusCode           = 1000,
                            StatusMessage        = "Hello world!",
                            Data                 = new VersionDetail(
                                                       Version.Id,
                                                       endpoints
                                                   ).ToJSON(
                                                         CustomVersionDetailSerializer,
                                                         CustomVersionEndpointSerializer
                                                     ),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Server                     = HTTPServiceName,
                                Date                       = Timestamp.Now,
                                AccessControlAllowOrigin   = "*",
                                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                Connection                 = ConnectionType.KeepAlive,
                                Vary                       = "Accept"
                            }
                        }
                    );

                }

            );

            #endregion


            #region OPTIONS     ~/v2.3.0/credentials

            // ----------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/v2.3.0/credentials
            // ----------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + $"{Version.String}/credentials",
                request => {

                    #region Defaults

                    var accessControlAllowMethods  = new List<String> {
                                                         "OPTIONS",
                                                         "GET"
                                                     };

                    var allow                      = new List<HTTPMethod>() {
                                                         HTTPMethod.OPTIONS,
                                                         HTTPMethod.GET
                                                     };

                    #endregion

                    #region Check the access token whether the client is known, and its access is allowed!

                    if (request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                    {

                        accessControlAllowMethods.Add("POST");

                        allow.Add(HTTPMethod.POST);

                        // Only when the party is fully registered!
                        if (request.LocalAccessInfo?.VersionsURL.HasValue == true)
                        {

                            accessControlAllowMethods.Add("PUT");
                            accessControlAllowMethods.Add("DELETE");

                            allow.Add(HTTPMethod.PUT);
                            allow.Add(HTTPMethod.DELETE);

                        }

                    }

                    #endregion


                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       Server                     = HTTPServiceName,
                                       Date                       = Timestamp.Now,
                                       AccessControlAllowOrigin   = "*",
                                       AccessControlAllowMethods  = accessControlAllowMethods,
                                       Allow                      = allow,
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       Connection                 = ConnectionType.KeepAlive,
                                       Vary                       = "Accept"
                                   }
                               });

                }

            );

            #endregion

            #region GET         ~/v2.3.0/credentials

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.3.0/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + $"{Version.String}/credentials",
                GetCredentialsRequest,
                GetCredentialsResponse,
                request => {

                    #region Check access token

                    if (request.LocalAccessInfo is not null &&
                        request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    Server                     = HTTPServiceName,
                                    Date                       = Timestamp.Now,
                                    AccessControlAllowOrigin   = "*",
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                    AccessControlAllowHeaders  = [ "Authorization" ],
                                    Connection                 = ConnectionType.KeepAlive,
                                    Vary                       = "Accept"
                                }
                            }
                        );

                    }

                    #endregion


                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = new Credentials(
                                                              request.LocalAccessInfo?.AccessToken ?? AccessToken.Parse("<any>"),
                                                              BaseAPI.OurVersionsURL,
                                                              parties.Values.Select(partyData => partyData.ToCredentialsRole())
                                                          ).ToJSON(),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       Server                     = HTTPServiceName,
                                       Date                       = Timestamp.Now,
                                       AccessControlAllowOrigin   = "*",
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                       Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET, HTTPMethod.POST, HTTPMethod.PUT, HTTPMethod.DELETE ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       Connection                 = ConnectionType.KeepAlive,
                                       Vary                       = "Accept"
                                   }
                               }
                           );

                });

            #endregion

            #region POST        ~/v2.3.0/credentials

            // REGISTER new OCPI party!

            // -----------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.3.0/credentials
            // -----------------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + $"{Version.String}/credentials",
                PostCredentialsRequest,
                PostCredentialsResponse,
                async request => {

                    if (request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                    {

                        if (request.LocalAccessInfo.VersionsURL.HasValue)
                            return new OCPIResponse.Builder(request) {
                                       StatusCode           = 2000,                                              // CREDENTIALS_TOKEN_A
                                       StatusMessage        = $"The given access token '{request.LocalAccessInfo.AccessToken}' is already registered!",
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   };

                        return await POSTOrPUTCredentials(request);

                    }

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #region PUT         ~/v2.3.0/credentials

            // UPDATE the registration of an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.3.0/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.PUT,
                URLPathPrefix + $"{Version.String}/credentials",
                PutCredentialsRequest,
                PutCredentialsResponse,
                async request => {

                    #region The access token is known...

                    if (request.LocalAccessInfo is not null)
                    {

                        #region ...but access is blocked!

                        if (request.LocalAccessInfo?.Status == AccessStatus.BLOCKED)
                            return new OCPIResponse.Builder(request) {
                                       StatusCode           = 2000,
                                       StatusMessage        = "The given access token '" + (request.AccessToken?.ToString() ?? "") + "' is blocked!",
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   };

                        #endregion

                        #region ...and access is allowed, but maybe not yet full registered!

                        if (request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                        {

                            // The party is not yet fully registered!
                            if (!request.LocalAccessInfo?.VersionsURL.HasValue == true)
                                return new OCPIResponse.Builder(request) {
                                           StatusCode           = 2000,
                                           StatusMessage        = "The given access token '" + (request.AccessToken?.ToString() ?? "") + "' is not yet registered!",
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                           }
                                       };

                            return await POSTOrPUTCredentials(request);

                        }

                        #endregion

                    }

                    #endregion

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                });

            #endregion

            #region DELETE      ~/v2.3.0/credentials

            // UNREGISTER an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.3.0/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + $"{Version.String}/credentials",
                DeleteCredentialsRequest,
                DeleteCredentialsResponse,
                async request => {

                    if (request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                    {

                        #region Validations

                        if (!request.LocalAccessInfo.VersionsURL.HasValue)
                            return new OCPIResponse.Builder(request) {
                                       StatusCode           = 2000,
                                       StatusMessage        = $"The given access token '{request.LocalAccessInfo.AccessToken}' is not fully registered!",
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
                                       }
                                   };

                        #endregion

                        await RemoveAccessToken(request.LocalAccessInfo.AccessToken);

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = $"The given access token '{request.LocalAccessInfo.AccessToken}' was deleted!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

        }

        #endregion


        #region (private) POSTOrPUTCredentials(Request)

        private async Task<OCPIResponse.Builder> POSTOrPUTCredentials(OCPIRequest Request)
        {

            #region Validate CREDENTIALS_TOKEN_A

            var CREDENTIALS_TOKEN_A     = Request.AccessToken;

            if (!CREDENTIALS_TOKEN_A.HasValue)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "The received credential token must not be null!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            #region Validate old remote party

            var oldRemoteParty = GetRemoteParties(remoteParty => remoteParty.LocalAccessInfos.Any(accessInfoStatus => accessInfoStatus.AccessToken == CREDENTIALS_TOKEN_A.Value)).FirstOrDefault();

            if (oldRemoteParty is null)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"There is no remote party having the given access token '{CREDENTIALS_TOKEN_A}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            #region Parse JSON

            var errorResponse = String.Empty;

            if (!Request.TryParseJObjectRequestBody(out var JSON,
                                                    out var responseBuilder,
                                                    AllowEmptyHTTPBody: false))
            {
                return responseBuilder;
            }

            if (!Credentials.TryParse(JSON,
                                      out var receivedCredentials,
                                      out errorResponse) ||
                receivedCredentials is null)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not parse the received credentials JSON object: " + errorResponse,
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            }

            #endregion

            #region Additional security checks... (Non-Standard)

            //lock (AccessTokens)
            //{
            //    foreach (var credentialsRole in receivedCredentials.Roles)
            //    {

            //        var result = AccessTokens.Values.Where(accessToken => accessToken.Roles.Any(role => role.CountryCode == credentialsRole.CountryCode &&
            //                                                                                            role.PartyId     == credentialsRole.PartyId &&
            //                                                                                            role.Role        == credentialsRole.Role)).ToArray();

            //        if (result.Length == 0)
            //        {

            //            return new OCPIResponse.Builder(Request) {
            //                       StatusCode           = 2000,
            //                       StatusMessage        = "The given combination of country code, party identification and role is unknown!",
            //                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
            //                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
            //                           AccessControlAllowHeaders  = [ "Authorization" ]
            //                       }
            //                   };

            //        }

            //        if (result.Length > 0 &&
            //            result.First().VersionsURL.HasValue)
            //        {

            //            return new OCPIResponse.Builder(Request) {
            //                       StatusCode           = 2000,
            //                       StatusMessage        = "The given combination of country code, party identification and role is already registered!",
            //                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
            //                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
            //                           AccessControlAllowHeaders  = [ "Authorization" ]
            //                       }
            //                   };

            //        }

            //    }
            //}

            #endregion


            var commonClient              = new CommonHTTPClient(

                                                CommonAPI:                         this,
                                                RemotePartyId:                     oldRemoteParty.Id,

                                                RemoteVersionsURL:                 receivedCredentials.URL,
                                                RemoteAccessToken:                 receivedCredentials.Token,  // CREDENTIALS_TOKEN_B
                                                RemoteAccessTokenBase64Encoding:   oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.AccessTokenIsBase64Encoded,
                                                RemoteTOTPConfig:                  oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.TOTPConfig,

                                                //VirtualHostname:                   oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.
                                                //Description:                       oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?
                                                PreferIPv4:                        oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.PreferIPv4,
                                                RemoteCertificateValidator:        oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.RemoteCertificateValidator,
                                                LocalCertificateSelector:          oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.LocalCertificateSelector,
                                                ClientCertificates:                oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.ClientCertificates,
                                                ClientCertificateContext:          oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.ClientCertificateContext,
                                                ClientCertificateChain:            oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.ClientCertificateChain,
                                                TLSProtocols:                      oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.TLSProtocols,
                                                ContentType:                       oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.ContentType,
                                                Accept:                            oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.Accept,
                                                HTTPUserAgent:                     oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.HTTPUserAgent,
                                                RequestTimeout:                    oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.RequestTimeout,
                                                TransmissionRetryDelay:            oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.TransmissionRetryDelay,
                                                MaxNumberOfRetries:                oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.MaxNumberOfRetries,
                                                InternalBufferSize:                oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.InternalBufferSize,
                                                UseHTTPPipelining:                 oldRemoteParty.RemoteAccessInfos.FirstOrDefault()?.UseHTTPPipelining,
                                                //HTTPLogger:                        

                                                //DisableLogging:                    
                                                //LoggingPath:                       
                                                //LoggingContext:                    
                                                //LogfileCreator:                    

                                                DNSClient:                         BaseAPI.HTTPBaseAPI.HTTPServer.DNSClient

                                            );

            var otherVersions             = await commonClient.GetVersions();

            #region ...or send error!

            if (otherVersions.StatusCode != 1000)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not fetch VERSIONS information from '" + receivedCredentials.URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            var justMySupportedVersion    = otherVersions.Data?.Where(version => version.Id == Version.Id).ToArray() ?? [];

            #region ...or send error!

            if (justMySupportedVersion.Length == 0)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3003,
                           StatusMessage        = $"Could not find {Version.String} at '{receivedCredentials.URL}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            var otherVersion2_2_1Details  = await commonClient.GetVersionDetails(Version.Id);

            #region ...or send error!

            if (otherVersion2_2_1Details.StatusCode != 1000)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3001,
                           StatusMessage        = $"Could not fetch {Version.String} information from '{justMySupportedVersion.First().URL}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion


            #region Validate, that neither the credentials roles had not been changed!

            if (oldRemoteParty.Roles.Count() != receivedCredentials.Roles.Count())
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"Updating the number of credentials roles from '{oldRemoteParty.Roles.Count()}' to '{receivedCredentials.Roles.Count()}' is not allowed!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            foreach (var receivedCredentialsRole in receivedCredentials.Roles)
            {

                CredentialsRole? existingCredentialsRole = null;

                foreach (var oldCredentialsRole in oldRemoteParty.Roles)
                {
                    if (oldCredentialsRole.PartyId == receivedCredentialsRole.PartyId &&
                        oldCredentialsRole.Role    == receivedCredentialsRole.Role)
                    {
                        existingCredentialsRole = receivedCredentialsRole;
                        break;
                    }
                }

                if (existingCredentialsRole is null)
                    return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"Updating the credentials roles is not allowed!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            }

            #endregion

            var CREDENTIALS_TOKEN_C       = AccessToken.NewRandom();

            // Remove the old access token
            await RemoveAccessToken(
                      CREDENTIALS_TOKEN_A.Value
                  );

            // Store credential of the other side!
            await AddOrUpdateRemoteParty(

                      oldRemoteParty.Id,                                        // Id
                      receivedCredentials.Roles,                                // CredentialsRoles

                      CREDENTIALS_TOKEN_C,                                      // LocalAccessToken
                      receivedCredentials.URL,                                  // RemoteVersionsURL
                      receivedCredentials.Token,                                // RemoteAccessToken
                      null,                                                     // RemoteAccessTokenBase64Encoding
                      null,                                                     // RemoteTOTPConfig

                      null,                                                     // PreferIPv4
                      null,                                                     // RemoteCertificateValidator
                      null,                                                     // LocalCertificateSelector
                      null,                                                     // ClientCertificates
                      null,                                                     // ClientCertificateContext
                      null,                                                     // ClientCertificateChain
                      null,                                                     // TLSProtocols
                      null,                                                     // ContentType
                      null,                                                     // Accept
                      null,                                                     // HTTPUserAgent
                      null,                                                     // RequestTimeout
                      null,                                                     // TransmissionRetryDelay
                      null,                                                     // MaxNumberOfRetries
                      null,                                                     // InternalBufferSize
                      null,                                                     // UseHTTPPipelining

                      RemoteAccessStatus.ONLINE,                                // RemoteStatus
                      otherVersions.Data?.Select(version => version.Id) ?? [],  // RemoteVersionIds
                      Version.Id,                                               // SelectedVersionId
                      null,                                                     // RemoteAccessNotBefore
                      null,                                                     // RemoteAccessNotAfter
                      null,                                                     // RemoteAllowDowngrades

                      null,                                                     // LocalAccessTokenBase64Encoding
                      null,                                                     // LocalTOTPConfig
                      null,                                                     // LocalAccessNotBefore
                      null,                                                     // LocalAccessNotAfter
                      null,                                                     // LocalAllowDowngrades
                      AccessStatus.ALLOWED,                                     // LocalAccessStatus

                      PartyStatus.ENABLED,                                      // PartyStatus

                      oldRemoteParty.Created,
                      Timestamp.Now,                                            // LastUpdated
                      Request.HTTPRequest.EventTrackingId,
                      Request.HTTPRequest.User?.Id                              // CurrentUserId

                  );


            return new OCPIResponse.Builder(Request) {
                           StatusCode           = 1000,
                           StatusMessage        = "Hello world!",
                           Data                 = new Credentials(
                                                      CREDENTIALS_TOKEN_C,
                                                      BaseAPI.OurVersionsURL,
                                                      parties.Values.Select(partyData => partyData.ToCredentialsRole())
                                                  ).ToJSON(),
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.OK,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

        }

        #endregion



        #region Log/Read   Remote Parties

        #region LogRemoteParty        (Command,              ...)

        public ValueTask LogRemoteParty(String            Command,
                                        EventTracking_Id  EventTrackingId,
                                        User_Id?          CurrentUserId   = null)

            => BaseAPI.WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   null,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region LogRemoteParty        (Command, Text = null, ...)

        public ValueTask LogRemoteParty(String            Command,
                                        String?           Text,
                                        EventTracking_Id  EventTrackingId,
                                        User_Id?          CurrentUserId   = null)

            => BaseAPI.WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   Text is not null
                       ? JToken.Parse(Text)
                       : null,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region LogRemoteParty        (Command, JSON,        ...)

        public ValueTask LogRemoteParty(String            Command,
                                        JObject           JSON,
                                        EventTracking_Id  EventTrackingId,
                                        User_Id?          CurrentUserId   = null)

            => BaseAPI.WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   JSON,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region LogRemoteParty        (Command, Number,      ...)

        public ValueTask LogRemoteParty(String            Command,
                                        Int64             Number,
                                        EventTracking_Id  EventTrackingId,
                                        User_Id?          CurrentUserId   = null)

            => BaseAPI.WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   Number,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region LogRemotePartyComment (Text,                 ...)

        public ValueTask LogRemotePartyComment(String           Text,
                                               EventTracking_Id  EventTrackingId,
                                               User_Id?          CurrentUserId   = null)

            => BaseAPI.WriteCommentToDatabase(
                   RemotePartyDBFileName,
                   Text,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion


        #region ReadRemotePartyDatabaseFile (DatabaseFileName = null, CancellationToken = default)

        public async Task ReadRemotePartyDatabaseFile(String?            DatabaseFileName    = null,
                                                      CancellationToken  CancellationToken   = default)
        {

            try
            {

                await foreach (var command in BaseAPI.LoadCommandsFromDatabaseFile(
                                                  DatabaseFileName ?? RemotePartyDBFileName,
                                                  CancellationToken
                                              ))
                {
                    ProcessRemotePartyCommand(command);
                }

            }
            catch (FileNotFoundException)
            { }
            catch (DirectoryNotFoundException)
            { }

        }

        #endregion

        #region ProcessRemotePartyCommand   (Command)

        public void ProcessRemotePartyCommand(Command command)
        {

            String?      errorResponse   = null;
            RemoteParty? remoteParty;

            var errorResponses = new List<Tuple<Command, String>>();

            switch (command.CommandName)
            {

                #region addRemoteParty

                case CommonHTTPAPI.addRemoteParty:
                    try
                    {
                        if (command.JSONObject is not null &&
                            RemoteParty.TryParse(command.JSONObject,
                                                 out remoteParty,
                                                 out errorResponse))
                        {
                            remoteParties.TryAdd(remoteParty.Id, remoteParty);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addRemotePartyIfNotExists

                case CommonHTTPAPI.addRemotePartyIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            RemoteParty.TryParse(command.JSONObject,
                                                 out remoteParty,
                                                 out errorResponse))
                        {
                            remoteParties.TryAdd(remoteParty.Id, remoteParty);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateRemoteParty

                case CommonHTTPAPI.addOrUpdateRemoteParty:
                    try
                    {
                        if (command.JSONObject is not null &&
                            RemoteParty.TryParse(command.JSONObject,
                                                 out remoteParty,
                                                 out errorResponse))
                        {

                            if (remoteParties.ContainsKey(remoteParty.Id))
                                remoteParties.Remove(remoteParty.Id, out _);

                            remoteParties.TryAdd(remoteParty.Id, remoteParty);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateRemoteParty

                case CommonHTTPAPI.updateRemoteParty:
                    try
                    {
                        if (command.JSONObject is not null &&
                            RemoteParty.TryParse(command.JSONObject,
                                                 out remoteParty,
                                                 out errorResponse))
                        {
                            remoteParties.Remove(remoteParty.Id, out _);
                            remoteParties.TryAdd(remoteParty.Id, remoteParty);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateRemoteParty

                case CommonHTTPAPI.removeRemoteParty:
                    try
                    {
                        if (command.JSONObject is not null &&
                            RemoteParty.TryParse(command.JSONObject,
                                                 out remoteParty,
                                                 out errorResponse))
                        {
                            remoteParties.Remove(remoteParty.Id, out _);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllRemoteParties

                case CommonHTTPAPI.removeAllRemoteParties:
                    remoteParties.Clear();
                    break;

                #endregion

            }

        }

        #endregion

        #endregion

        #region Log/Read   Assets

        #region LogAsset               (Command,              ...)

        public ValueTask LogAsset(String             Command,
                                  EventTracking_Id   EventTrackingId,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   null,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogAsset               (Command, Text = null, ...)

        public ValueTask LogAsset(String             Command,
                                  String?            Text,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   Text is not null
                       ? JToken.Parse(Text)
                       : null,
                   EventTrackingId ?? EventTracking_Id.New,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogAsset               (Command, JSONObject,  ...)

        public ValueTask LogAsset(String             Command,
                                  JObject            JSONObject,
                                  EventTracking_Id   EventTrackingId,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   JSONObject,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogAsset               (Command, JSONArray,   ...)

        public ValueTask LogAsset(String             Command,
                                  JArray             JSONArray,
                                  EventTracking_Id   EventTrackingId,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   JSONArray,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogAsset               (Command, Number,      ...)

        public ValueTask LogAsset(String             Command,
                                  Int64              Number,
                                  EventTracking_Id   EventTrackingId,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   Number,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogAssetComment        (Text,                 ...)

        public ValueTask LogAssetComment(String             Text,
                                         EventTracking_Id   EventTrackingId,
                                         User_Id?           CurrentUserId       = null,
                                         CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteCommentToDatabase(
                   AssetsDBFileName,
                   Text,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion


        #region ReadAssetsDatabaseFile (DatabaseFileName = null, CancellationToken = default)

        public async Task ReadAssetsDatabaseFile(String?            DatabaseFileName    = null,
                                                 CancellationToken  CancellationToken   = default)
        {

            try
            {

                await foreach (var command in BaseAPI.LoadCommandsFromDatabaseFile(
                                                  DatabaseFileName ?? AssetsDBFileName,
                                                  CancellationToken
                                              ))
                {
                    ProcessAssetsCommand(command);
                }

            }
            catch (FileNotFoundException)
            { }
            catch (DirectoryNotFoundException)
            { }

        }

        #endregion

        #region ProcessAssetsCommand   (Command)

        public void ProcessAssetsCommand(Command command)
        {

            String?       errorResponse   = null;
            Location?     location;
            EVSE?         evse;
            Tariff?       tariff;
            Session?      session;
            TokenStatus?  tokenStatus;
            CDR?          cdr;

            var errorResponses = new List<Tuple<Command, String>>();

            switch (command.CommandName)
            {

                #region addLocation

                case CommonHTTPAPI.addLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            parties. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {
                            party.Locations.TryAdd(
                                location.Id,
                                location
                            );
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addLocationIfNotExists

                case CommonHTTPAPI.addLocationIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            parties. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {
                            party.Locations.TryAdd(
                                location.Id,
                                location
                            );
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateLocation

                case CommonHTTPAPI.addOrUpdateLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            parties. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {

                            if (party.Locations.ContainsKey(location.Id))
                                party.Locations.Remove(location.Id, out _);

                            party.Locations.TryAdd(location.Id, location);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateLocation

                case CommonHTTPAPI.updateLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            parties. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {
                            party.Locations.Remove(location.Id, out _);
                            party.Locations.TryAdd(location.Id, location);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateLocation

                case CommonHTTPAPI.removeLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(
                                         command.JSONObject,
                                         out location,
                                         out errorResponse
                                     ) &&
                            parties. TryGetValue(
                                         Party_Idv3.From(
                                             location.CountryCode,
                                             location.PartyId
                                         ),
                                         out var party
                                     ))
                        {
                            party.Locations.Remove(location.Id, out _);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllLocations

                case CommonHTTPAPI.removeAllLocations:
                    foreach (var party in parties.Values)
                        party.Locations.Clear();
                    break;

                #endregion


                // Experimental!

                #region addOrUpdateEVSE

                //case CommonHTTPAPI.addOrUpdateEVSE:
                //    try
                //    {
                //        if (command.JSONObject is not null &&

                //            command.JSONObject.TryGetValue("locationId", out var locationId) &&
                //            locationId.Type == JTokenType.String &&
                //            Location_Id.TryParse(locationId?.Value<String>() ?? "", out var location_Id) &&
                //            locations.ContainsKey(location_Id) &&

                //            command.JSONObject.TryGetValue("evse",       out var evseJToken) &&
                //            evseJToken.Type == JTokenType.Object &&
                //            evseJToken is JObject &&
                //            EVSE.TryParse((evseJToken as JObject)!,
                //                          out evse,
                //                          out errorResponse))

                //        {

                //            if (locations.TryGetValue(location_Id, out location))
                //            {

                //                var updatedLocation = location.Update(loc => {

                //                    var newEVSEs = loc.EVSEs.Where(evseX => evseX.UId != evse.UId).ToList();
                //                    newEVSEs.Add(evse);

                //                    loc.EVSEs.Clear();

                //                    foreach (var newEVSE in newEVSEs)
                //                        loc.EVSEs.Add(newEVSE);

                //                }, out var warnings);

                //                if (updatedLocation is not null)
                //                {
                //                    locations.Remove(location.Id, out _);
                //                    locations.TryAdd(location.Id, updatedLocation);
                //                }

                //            }

                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        errorResponse ??= e.Message;
                //    }
                //    if (errorResponse is not null)
                //        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                //    break;

                #endregion


                #region addTariff

                case CommonHTTPAPI.addTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Tariffs.TryAdd(tariff.Id, tariff);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addTariffIfNotExists

                case CommonHTTPAPI.addTariffIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Tariffs.TryAdd(tariff.Id, tariff);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateTariff

                case CommonHTTPAPI.addOrUpdateTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {

                            if (party.Tariffs.ContainsKey(tariff.Id))
                                party.Tariffs.Remove(tariff.Id);

                            party.Tariffs.TryAdd(tariff.Id, tariff);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateTariff

                case CommonHTTPAPI.updateTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Tariffs.Remove(tariff.Id);
                            party.Tariffs.TryAdd(tariff.Id, tariff);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateTariff

                case CommonHTTPAPI.removeTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff. TryParse(
                                        command.JSONObject,
                                        out tariff,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            tariff.CountryCode,
                                            tariff.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Tariffs.Remove(tariff.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllTariffs

                case CommonHTTPAPI.removeAllTariffs:
                    foreach (var party in parties.Values)
                        party.Tariffs.Clear();
                    break;

                #endregion


                #region addSession

                case CommonHTTPAPI.addSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Sessions.TryAdd(session.Id, session);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addSessionIfNotExists

                case CommonHTTPAPI.addSessionIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Sessions.TryAdd(session.Id, session);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateSession

                case CommonHTTPAPI.addOrUpdateSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {

                            if (party.Sessions.ContainsKey(session.Id))
                                party.Sessions.Remove(session.Id, out _);

                            party.Sessions.TryAdd(session.Id, session);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateSession

                case CommonHTTPAPI.updateSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Sessions.Remove(session.Id, out _);
                            party.Sessions.TryAdd(session.Id, session);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateSession

                case CommonHTTPAPI.removeSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(
                                        command.JSONObject,
                                        out session,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            session.CountryCode,
                                            session.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.Sessions.Remove(session.Id, out _);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllSessions

                case CommonHTTPAPI.removeAllSessions:
                    foreach (var party in parties.Values)
                        party.Sessions.Clear();
                    break;

                #endregion


                #region addToken

                case CommonHTTPAPI.addTokenStatus:
                    try
                    {
                        if (command.JSONObject is not null &&
                            TokenStatus.TryParse(
                                            command.JSONObject,
                                            out tokenStatus,
                                            out errorResponse
                                        ) &&
                            parties.    TryGetValue(
                                            Party_Idv3.From(
                                                tokenStatus.Token.CountryCode,
                                                tokenStatus.Token.PartyId
                                            ),
                                            out var party
                                        ))
                        {
                            party.Tokens.TryAdd(tokenStatus.Token.Id, tokenStatus);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addTokenIfNotExists

                case CommonHTTPAPI.addTokenStatusIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            TokenStatus.TryParse(
                                            command.JSONObject,
                                            out tokenStatus,
                                            out errorResponse
                                        ) &&
                            parties.    TryGetValue(
                                            Party_Idv3.From(
                                                tokenStatus.Token.CountryCode,
                                                tokenStatus.Token.PartyId
                                            ),
                                            out var party
                                        ))
                        {
                            party.Tokens.TryAdd(tokenStatus.Token.Id, tokenStatus);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateToken

                case CommonHTTPAPI.addOrUpdateTokenStatus:
                    try
                    {
                        if (command.JSONObject is not null &&
                            TokenStatus.TryParse(
                                            command.JSONObject,
                                            out tokenStatus,
                                            out errorResponse
                                        ) &&
                            parties.    TryGetValue(
                                            Party_Idv3.From(
                                                tokenStatus.Token.CountryCode,
                                                tokenStatus.Token.PartyId
                                            ),
                                            out var party
                                        ))
                        {

                            if (party.Tokens.ContainsKey(tokenStatus.Token.Id))
                                party.Tokens.Remove(tokenStatus.Token.Id, out _);

                            party.Tokens.TryAdd(tokenStatus.Token.Id, tokenStatus);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateToken

                case CommonHTTPAPI.updateTokenStatus:
                    try
                    {
                        if (command.JSONObject is not null &&
                            TokenStatus.TryParse(
                                            command.JSONObject,
                                            out tokenStatus,
                                            out errorResponse
                                        ) &&
                            parties.    TryGetValue(
                                            Party_Idv3.From(
                                                tokenStatus.Token.CountryCode,
                                                tokenStatus.Token.PartyId
                                            ),
                                            out var party
                                        ))
                        {
                            party.Tokens.Remove(tokenStatus.Token.Id, out _);
                            party.Tokens.TryAdd(tokenStatus.Token.Id, tokenStatus);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateToken

                case CommonHTTPAPI.removeToken:
                    try
                    {
                        if (command.JSONObject is not null &&
                            TokenStatus.TryParse(
                                            command.JSONObject,
                                            out tokenStatus,
                                            out errorResponse
                                        ) &&
                            parties.    TryGetValue(
                                            Party_Idv3.From(
                                                tokenStatus.Token.CountryCode,
                                                tokenStatus.Token.PartyId
                                            ),
                                            out var party
                                        ))
                        {
                            party.Tokens.Remove(tokenStatus.Token.Id, out _);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllTokens

                case CommonHTTPAPI.removeAllTokenStatus:
                    foreach (var party in parties.Values)
                        party.Tokens.Clear();
                    break;

                #endregion


                #region addChargeDetailRecord

                case CommonHTTPAPI.addChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.CDRs.TryAdd(cdr.Id, cdr);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addChargeDetailRecordIfNotExists

                case CommonHTTPAPI.addChargeDetailRecordIfNotExists:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.CDRs.TryAdd(cdr.Id, cdr);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region addOrUpdateChargeDetailRecord

                case CommonHTTPAPI.addOrUpdateChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {

                            if (party.CDRs.ContainsKey(cdr.Id))
                                party.CDRs.Remove(cdr.Id, out _);

                            party.CDRs.TryAdd(cdr.Id, cdr);

                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateChargeDetailRecord

                case CommonHTTPAPI.updateChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.CDRs.Remove(cdr.Id, out _);
                            party.CDRs.TryAdd(cdr.Id, cdr);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region updateCDR

                case CommonHTTPAPI.removeChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.    TryParse(
                                        command.JSONObject,
                                        out cdr,
                                        out errorResponse
                                    ) &&
                            parties.TryGetValue(
                                        Party_Idv3.From(
                                            cdr.CountryCode,
                                            cdr.PartyId
                                        ),
                                        out var party
                                    ))
                        {
                            party.CDRs.Remove(cdr.Id, out _);
                        }
                    }
                    catch (Exception e)
                    {
                        errorResponse ??= e.Message;
                    }
                    if (errorResponse is not null)
                        errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                    break;

                #endregion

                #region removeAllCDRs

                case CommonHTTPAPI.removeAllChargeDetailRecords:
                    foreach (var party in parties.Values)
                        party.CDRs.Clear();
                    break;

                #endregion


                default:
                    DebugX.Log($"Unknown OCPI {Version.String} command: '{command}'!");
                    break;

            }

        }

        #endregion

        #endregion


        #region GetCommonClient (               RemoteVersionsURL, ...)

        /// <summary>
        /// Return a common OCPI client for the given parameters.
        /// </summary>
        /// <param name="RemoteVersionsURL">The remote URL of the "OCPI Versions" endpoint to connect to.</param>
        /// <param name="RemoteAccessToken">The remote access token to use.</param>
        /// <param name="RemoteAccessTokenBase64Encoding">Whether the remote access token is Base64 encoded.</param>
        /// <param name="RemoteTOTPConfig">The optional Time-Based One-Time Password (TOTP) configuration as 2nd factor authentication.</param>
        /// 
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this OCPI Common client.</param>
        /// <param name="PreferIPv4">Whether IPv4 should be preferred over IPv6.</param>
        /// <param name="RemoteCertificateValidator">The remote TLS certificate validator.</param>
        /// <param name="LocalCertificateSelector">An optional local TLS certificate selector.</param>
        /// <param name="ClientCertificates">An optional TLS client certificate to use for HTTP authentication.</param>
        /// <param name="ClientCertificateContext">An optional TLS client certificate context.</param>
        /// <param name="ClientCertificateChain">An optional TLS client certificate chain.</param>
        /// <param name="TLSProtocols">Optional list of allowed TLS protocols.</param>
        /// <param name="ContentType">The optional HTTP content type to use.</param>
        /// <param name="Accept">The optional HTTP accept header to use.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// <param name="HTTPLogger"></param>
        /// 
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingPath"></param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CommonHTTPClient GetCommonClient(URL                                                        RemoteVersionsURL,
                                            AccessToken?                                               RemoteAccessToken                 = null,
                                            Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                            TOTPConfig?                                                RemoteTOTPConfig                  = null,

                                            HTTPHostname?                                              VirtualHostname                   = null,
                                            I18NString?                                                Description                       = null,
                                            Boolean?                                                   PreferIPv4                        = null,
                                            RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                                            LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                                            IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                                            SslStreamCertificateContext?                               ClientCertificateContext          = null,
                                            IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                                            SslProtocols?                                              TLSProtocols                      = null,
                                            HTTPContentType?                                           ContentType                       = null,
                                            AcceptTypes?                                               Accept                            = null,
                                            String?                                                    HTTPUserAgent                     = null,
                                            TimeSpan?                                                  RequestTimeout                    = null,
                                            TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                                            UInt16?                                                    MaxNumberOfRetries                = null,
                                            UInt32?                                                    InternalBufferSize                = null,
                                            Boolean?                                                   UseHTTPPipelining                 = null,
                                            HTTPClientLogger?                                          HTTPLogger                        = null,

                                            Boolean?                                                   DisableLogging                    = false,
                                            String?                                                    LoggingPath                       = null,
                                            String?                                                    LoggingContext                    = null,
                                            OCPILogfileCreatorDelegate?                                LogfileCreator                    = null,
                                            IDNSClient?                                                DNSClient                         = null)

            => GetCommonClient(

                   RemoteParty_Id.Unknown,

                   RemoteVersionsURL,
                   RemoteAccessToken,
                   RemoteAccessTokenBase64Encoding,
                   RemoteTOTPConfig,

                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCertificates,
                   ClientCertificateContext,
                   ClientCertificateChain,
                   TLSProtocols,
                   ContentType,
                   Accept,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,
                   HTTPLogger,

                   DisableLogging,
                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   DNSClient

               );

        #endregion

        #region GetCommonClient (RemotePartyId, RemoteVersionsURL, ...)

        /// <summary>
        /// Return a common OCPI client for the given parameters.
        /// </summary>
        /// <param name="RemotePartyId">The remote party identification.</param>
        /// 
        /// <param name="RemoteVersionsURL">The remote URL of the "OCPI Versions" endpoint to connect to.</param>
        /// <param name="RemoteAccessToken">The remote access token to use.</param>
        /// <param name="RemoteAccessTokenBase64Encoding">Whether the remote access token is Base64 encoded.</param>
        /// <param name="RemoteTOTPConfig">The optional Time-Based One-Time Password (TOTP) configuration as 2nd factor authentication.</param>
        /// 
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this OCPI Common client.</param>
        /// <param name="PreferIPv4">Whether IPv4 should be preferred over IPv6.</param>
        /// <param name="RemoteCertificateValidator">The remote TLS certificate validator.</param>
        /// <param name="LocalCertificateSelector">An optional local TLS certificate selector.</param>
        /// <param name="ClientCertificates">An optional TLS client certificate to use for HTTP authentication.</param>
        /// <param name="ClientCertificateContext">An optional TLS client certificate context.</param>
        /// <param name="ClientCertificateChain">An optional TLS client certificate chain.</param>
        /// <param name="TLSProtocols">Optional list of allowed TLS protocols.</param>
        /// <param name="ContentType">The optional HTTP content type to use.</param>
        /// <param name="Accept">The optional HTTP accept header to use.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// <param name="HTTPLogger"></param>
        /// 
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingPath"></param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CommonHTTPClient GetCommonClient(RemoteParty_Id                                             RemotePartyId,

                                            URL                                                        RemoteVersionsURL,
                                            AccessToken?                                               RemoteAccessToken                 = null,
                                            Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                            TOTPConfig?                                                RemoteTOTPConfig                  = null,

                                            HTTPHostname?                                              VirtualHostname                   = null,
                                            I18NString?                                                Description                       = null,
                                            Boolean?                                                   PreferIPv4                        = null,
                                            RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                                            LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                                            IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                                            SslStreamCertificateContext?                               ClientCertificateContext          = null,
                                            IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                                            SslProtocols?                                              TLSProtocols                      = null,
                                            HTTPContentType?                                           ContentType                       = null,
                                            AcceptTypes?                                               Accept                            = null,
                                            String?                                                    HTTPUserAgent                     = null,
                                            TimeSpan?                                                  RequestTimeout                    = null,
                                            TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                                            UInt16?                                                    MaxNumberOfRetries                = null,
                                            UInt32?                                                    InternalBufferSize                = null,
                                            Boolean?                                                   UseHTTPPipelining                 = null,
                                            HTTPClientLogger?                                          HTTPLogger                        = null,

                                            Boolean?                                                   DisableLogging                    = false,
                                            String?                                                    LoggingPath                       = null,
                                            String?                                                    LoggingContext                    = null,
                                            OCPILogfileCreatorDelegate?                                LogfileCreator                    = null,
                                            IDNSClient?                                                DNSClient                         = null)

            => new (

                   this,
                   RemotePartyId,

                   RemoteVersionsURL,
                   RemoteAccessToken,
                   RemoteAccessTokenBase64Encoding,
                   RemoteTOTPConfig,

                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCertificates,
                   ClientCertificateContext,
                   ClientCertificateChain,
                   TLSProtocols,
                   ContentType,
                   Accept,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,
                   HTTPLogger,

                   DisableLogging,
                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   DNSClient

               );

        #endregion


        //ToDo: Wrap the following into a pluggable interface!

        #region AccessTokens

        public async Task AddAccessToken(String        Token,
                                         AccessStatus  Status)
        {
            if (AccessToken.TryParseAsBASE64(Token, out var token))
            {
                await BaseAPI.AddAccessToken(
                    token,
                    Status
                );
            }
        }

        public async Task AddAccessToken(AccessToken   Token,
                                         AccessStatus  Status)
        {
            await BaseAPI.AddAccessToken(
                Token,
                Status
            );
        }


        // An access token might be used by more than one CountryCode + PartyId + Role combination!

        #region RemoveAccessToken(AccessToken)

        public async Task<CommonAPI> RemoveAccessToken(AccessToken        AccessToken,
                                                       EventTracking_Id?  EventTrackingId   = null,
                                                       User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.Where(party => party.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken)))
            {

                #region The remote party has only a single local access token, or...

                if (remoteParty.LocalAccessInfos.Count() <= 1)
                {

                    remoteParties.TryRemove(remoteParty.Id, out _);

                    await LogRemoteParty(
                              CommonHTTPAPI.removeRemoteParty,
                              remoteParty.ToJSON(true),
                              EventTrackingId ?? EventTracking_Id.New,
                              CurrentUserId
                          );

                }

                #endregion

                #region ...the remote party has multiple local access tokens!

                else
                {

                    remoteParties.TryRemove(remoteParty.Id, out _);

                    var newRemoteParty = new RemoteParty(
                                                remoteParty.Id,
                                                remoteParty.Roles,
                                                remoteParty.LocalAccessInfos.Where(localAccessInfo => localAccessInfo.AccessToken != AccessToken),
                                                remoteParty.RemoteAccessInfos,
                                                remoteParty.Status
                                            );

                    if (remoteParties.TryAdd(newRemoteParty.Id,
                                             newRemoteParty))
                    {

                        await LogRemoteParty(
                                  CommonHTTPAPI.updateRemoteParty,
                                  newRemoteParty.ToJSON(true),
                                  EventTrackingId ?? EventTracking_Id.New,
                                  CurrentUserId
                              );

                    }

                }

                #endregion

            }

            return this;

        }

        #endregion

        #region TryGetLocalAccessInfo(AccessToken, out LocalAccessInfo)

        public Boolean TryGetLocalAccessInfo(AccessToken AccessToken, out LocalAccessInfo? LocalAccessInfo)
        {

            var accessInfos = remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken)).
                                                   SelectMany(remoteParty => remoteParty.LocalAccessInfos).
                                                   ToArray();

            if (accessInfos.Length == 1)
            {
                LocalAccessInfo = accessInfos.First();
                return true;
            }

            LocalAccessInfo = default;
            return false;

        }

        #endregion

        #region GetLocalAccessInfos(AccessToken)

        public IEnumerable<LocalAccessInfo> GetLocalAccessInfos(AccessToken AccessToken)

            => remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken)).
                                    SelectMany(remoteParty => remoteParty.LocalAccessInfos);

        #endregion

        #region GetLocalAccessInfos(AccessToken, AccessStatus)

        public IEnumerable<LocalAccessInfo> GetLocalAccessInfos(AccessToken   AccessToken,
                                                                AccessStatus  AccessStatus)

            => remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken  == AccessToken &&
                                                                                                             accessInfo.Status       == AccessStatus)).
                                    SelectMany(remoteParty => remoteParty.LocalAccessInfos);

        #endregion

        #endregion

        #region RemoteParties

        #region Data

        private readonly ConcurrentDictionary<RemoteParty_Id, RemoteParty> remoteParties = new();

        /// <summary>
        /// Return an enumeration of all remote parties.
        /// </summary>
        public IEnumerable<RemoteParty> RemoteParties
            => remoteParties.Values;

        #endregion


        #region AddRemoteParty            (Id, CredentialsRoles, LocalAccessToken, ...)

        /// <summary>
        /// Create a new Remote Party with local access only.
        /// The remote party will start the OCPI registration process afterwards.
        /// Maybe some remote access parameters need already to be set for a successful registration.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CredentialsRoles"></param>
        /// 
        /// <param name="LocalAccessToken"></param>
        /// <param name="LocalAccessTokenBase64Encoding"></param>
        /// <param name="LocalTOTPConfig"></param>
        /// <param name="LocalAccessNotBefore"></param>
        /// <param name="LocalAccessNotAfter"></param>
        /// <param name="LocalAllowDowngrades"></param>
        /// <param name="LocalAccessStatus"></param>
        /// 
        /// <param name="RemoteAccessTokenBase64Encoding"></param>
        /// <param name="RemoteTOTPConfig"></param>
        /// <param name="RemoteAccessNotBefore"></param>
        /// <param name="RemoteAccessNotAfter"></param>
        /// <param name="PreferIPv4"></param>
        /// <param name="RemoteCertificateValidator"></param>
        /// <param name="LocalCertificateSelector"></param>
        /// <param name="ClientCertificates"></param>
        /// <param name="ClientCertificateContext"></param>
        /// <param name="ClientCertificateChain"></param>
        /// <param name="TLSProtocols"></param>
        /// <param name="ContentType"></param>
        /// <param name="Accept"></param>
        /// <param name="HTTPUserAgent"></param>
        /// <param name="RequestTimeout"></param>
        /// <param name="TransmissionRetryDelay"></param>
        /// <param name="MaxNumberOfRetries"></param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// <param name="RemoteStatus"></param>
        /// <param name="RemoteAllowDowngrades"></param>
        /// 
        /// <param name="Status"></param>
        /// 
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               CredentialsRoles,

                           AccessToken                                                LocalAccessToken,
                           Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                           TOTPConfig?                                                LocalTOTPConfig                   = null,
                           DateTimeOffset?                                            LocalAccessNotBefore              = null,
                           DateTimeOffset?                                            LocalAccessNotAfter               = null,
                           Boolean?                                                   LocalAllowDowngrades              = null,
                           AccessStatus?                                              LocalAccessStatus                 = null,

                           Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                           TOTPConfig?                                                RemoteTOTPConfig                  = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   PreferIPv4                        = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                           SslStreamCertificateContext?                               ClientCertificateContext          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                           SslProtocols?                                              TLSProtocols                      = null,
                           HTTPContentType?                                           ContentType                       = null,
                           AcceptTypes?                                               Accept                            = null,
                           String?                                                    HTTPUserAgent                     = null,
                           TimeSpan?                                                  RequestTimeout                    = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                           UInt16?                                                    MaxNumberOfRetries                = null,
                           UInt32?                                                    InternalBufferSize                = null,
                           Boolean?                                                   UseHTTPPipelining                 = null,
                           RemoteAccessStatus?                                        RemoteStatus                      = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           PartyStatus?                                               Status                            = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessToken,
                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,
                                     RemoteStatus,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemoteParty            (Id, CredentialsRoles,                   RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               CredentialsRoles,

                           URL                                                        RemoteVersionsURL,
                           AccessToken                                                RemoteAccessToken,
                           Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                           TOTPConfig?                                                RemoteTOTPConfig                  = null,

                           Boolean?                                                   PreferIPv4                        = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                           SslStreamCertificateContext?                               ClientCertificateContext          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                           SslProtocols?                                              TLSProtocols                      = null,
                           HTTPContentType?                                           ContentType                       = null,
                           AcceptTypes?                                               Accept                            = null,
                           String?                                                    HTTPUserAgent                     = null,
                           TimeSpan?                                                  RequestTimeout                    = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                           UInt16?                                                    MaxNumberOfRetries                = null,
                           UInt32?                                                    InternalBufferSize                = null,
                           Boolean?                                                   UseHTTPPipelining                 = null,

                           RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                           IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                           Version_Id?                                                SelectedVersionId                 = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           PartyStatus?                                               Status                            = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     RemoteVersionsURL,
                                     RemoteAccessToken,
                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,

                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemoteParty            (Id, CredentialsRoles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               CredentialsRoles,

                           AccessToken                                                LocalAccessToken,

                           URL                                                        RemoteVersionsURL,
                           AccessToken                                                RemoteAccessToken,
                           Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                           TOTPConfig?                                                RemoteTOTPConfig                  = null,

                           Boolean?                                                   PreferIPv4                        = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                           SslStreamCertificateContext?                               ClientCertificateContext          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                           SslProtocols?                                              TLSProtocols                      = null,
                           HTTPContentType?                                           ContentType                       = null,
                           AcceptTypes?                                               Accept                            = null,
                           String?                                                    HTTPUserAgent                     = null,
                           TimeSpan?                                                  RequestTimeout                    = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                           UInt16?                                                    MaxNumberOfRetries                = null,
                           UInt32?                                                    InternalBufferSize                = null,
                           Boolean?                                                   UseHTTPPipelining                 = null,
                           RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                           IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                           Version_Id?                                                SelectedVersionId                 = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                           TOTPConfig?                                                LocalTOTPConfig                   = null,
                           DateTimeOffset?                                            LocalAccessNotBefore              = null,
                           DateTimeOffset?                                            LocalAccessNotAfter               = null,
                           Boolean?                                                   LocalAllowDowngrades              = false,
                           AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                           PartyStatus?                                               Status                            = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessToken,

                                     RemoteVersionsURL,
                                     RemoteAccessToken,
                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,
                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty)) {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemoteParty            (Id, CredentialsRoles, LocalAccessInfos, RemoteAccessInfos, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                 Id,
                           IEnumerable<CredentialsRole>   CredentialsRoles,

                           IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                           IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                           PartyStatus?                   Status            = null,

                           DateTimeOffset?                Created           = null,
                           DateTimeOffset?                LastUpdated       = null,
                           EventTracking_Id?              EventTrackingId   = null,
                           User_Id?                       CurrentUserId     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion


        #region AddRemotePartyIfNotExists (Id, CredentialsRoles, LocalAccessToken, ...)

        /// <summary>
        /// Create a new Remote Party with local access only, if it does not already exist.
        /// The remote party will start the OCPI registration process afterwards.
        /// Maybe some remote access parameters need already to be set for a successful registration.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CredentialsRoles"></param>
        /// 
        /// <param name="LocalAccessToken"></param>
        /// <param name="LocalAccessTokenBase64Encoding"></param>
        /// <param name="LocalTOTPConfig"></param>
        /// <param name="LocalAccessNotBefore"></param>
        /// <param name="LocalAccessNotAfter"></param>
        /// <param name="LocalAllowDowngrades"></param>
        /// <param name="LocalAccessStatus"></param>
        /// 
        /// <param name="RemoteAccessTokenBase64Encoding"></param>
        /// <param name="RemoteTOTPConfig"></param>
        /// <param name="RemoteAccessNotBefore"></param>
        /// <param name="RemoteAccessNotAfter"></param>
        /// <param name="PreferIPv4"></param>
        /// <param name="RemoteCertificateValidator"></param>
        /// <param name="LocalCertificateSelector"></param>
        /// <param name="ClientCertificates"></param>
        /// <param name="ClientCertificateContext"></param>
        /// <param name="ClientCertificateChain"></param>
        /// <param name="TLSProtocols"></param>
        /// <param name="ContentType"></param>
        /// <param name="Accept"></param>
        /// <param name="HTTPUserAgent"></param>
        /// <param name="RequestTimeout"></param>
        /// <param name="TransmissionRetryDelay"></param>
        /// <param name="MaxNumberOfRetries"></param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// <param name="RemoteStatus"></param>
        /// <param name="RemoteAllowDowngrades"></param>
        /// 
        /// <param name="Status"></param>
        /// 
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                      IEnumerable<CredentialsRole>                               CredentialsRoles,

                                      AccessToken                                                LocalAccessToken,
                                      Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                                      TOTPConfig?                                                LocalTOTPConfig                   = null,
                                      DateTimeOffset?                                            LocalAccessNotBefore              = null,
                                      DateTimeOffset?                                            LocalAccessNotAfter               = null,
                                      Boolean?                                                   LocalAllowDowngrades              = null,
                                      AccessStatus?                                              LocalAccessStatus                 = null,

                                      Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                      TOTPConfig?                                                RemoteTOTPConfig                  = null,
                                      DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                      DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                      Boolean?                                                   PreferIPv4                        = null,
                                      RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                                      LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                                      IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                                      SslStreamCertificateContext?                               ClientCertificateContext          = null,
                                      IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                                      SslProtocols?                                              TLSProtocols                      = null,
                                      HTTPContentType?                                           ContentType                       = null,
                                      AcceptTypes?                                               Accept                            = null,
                                      String?                                                    HTTPUserAgent                     = null,
                                      TimeSpan?                                                  RequestTimeout                    = null,
                                      TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                                      UInt16?                                                    MaxNumberOfRetries                = null,
                                      UInt32?                                                    InternalBufferSize                = null,
                                      Boolean?                                                   UseHTTPPipelining                 = null,
                                      RemoteAccessStatus?                                        RemoteStatus                      = null,
                                      Boolean?                                                   RemoteAllowDowngrades             = null,

                                      PartyStatus?                                               Status                            = null,

                                      DateTimeOffset?                                            Created                           = null,
                                      DateTimeOffset?                                            LastUpdated                       = null,
                                      EventTracking_Id?                                          EventTrackingId                   = null,
                                      User_Id?                                                   CurrentUserId                     = null)

        {

            if (remoteParties.TryGetValue(Id, out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessToken,
                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,
                                     RemoteStatus,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemotePartyIfNotExists (Id, CredentialsRoles,                   RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                      IEnumerable<CredentialsRole>                               CredentialsRoles,

                                      URL                                                        RemoteVersionsURL,
                                      AccessToken                                                RemoteAccessToken,
                                      Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                      TOTPConfig?                                                RemoteTOTPConfig                  = null,

                                      Boolean?                                                   PreferIPv4                        = null,
                                      RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                                      LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                                      IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                                      SslStreamCertificateContext?                               ClientCertificateContext          = null,
                                      IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                                      SslProtocols?                                              TLSProtocols                      = null,
                                      HTTPContentType?                                           ContentType                       = null,
                                      AcceptTypes?                                               Accept                            = null,
                                      String?                                                    HTTPUserAgent                     = null,
                                      TimeSpan?                                                  RequestTimeout                    = null,
                                      TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                                      UInt16?                                                    MaxNumberOfRetries                = null,
                                      UInt32?                                                    InternalBufferSize                = null,
                                      Boolean?                                                   UseHTTPPipelining                 = null,

                                      RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                      IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                      Version_Id?                                                SelectedVersionId                 = null,
                                      DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                      DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                      Boolean?                                                   RemoteAllowDowngrades             = null,

                                      PartyStatus?                                               Status                            = null,

                                      DateTimeOffset?                                            Created                           = null,
                                      DateTimeOffset?                                            LastUpdated                       = null,
                                      EventTracking_Id?                                          EventTrackingId                   = null,
                                      User_Id?                                                   CurrentUserId                     = null)

        {

            if (remoteParties.TryGetValue(Id, out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     RemoteVersionsURL,
                                     RemoteAccessToken,
                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,

                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemotePartyIfNotExists (Id, CredentialsRoles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                      IEnumerable<CredentialsRole>                               CredentialsRoles,

                                      AccessToken                                                LocalAccessToken,

                                      URL                                                        RemoteVersionsURL,
                                      AccessToken                                                RemoteAccessToken,
                                      Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                      TOTPConfig?                                                RemoteTOTPConfig                  = null,

                                      Boolean?                                                   PreferIPv4                        = null,
                                      RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                                      LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                                      IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                                      SslStreamCertificateContext?                               ClientCertificateContext          = null,
                                      IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                                      SslProtocols?                                              TLSProtocols                      = null,
                                      HTTPContentType?                                           ContentType                       = null,
                                      AcceptTypes?                                               Accept                            = null,
                                      String?                                                    HTTPUserAgent                     = null,
                                      TimeSpan?                                                  RequestTimeout                    = null,
                                      TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                                      UInt16?                                                    MaxNumberOfRetries                = null,
                                      UInt32?                                                    InternalBufferSize                = null,
                                      Boolean?                                                   UseHTTPPipelining                 = null,
                                      RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                      IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                      Version_Id?                                                SelectedVersionId                 = null,
                                      DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                      DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                      Boolean?                                                   RemoteAllowDowngrades             = null,

                                      Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                                      TOTPConfig?                                                LocalTOTPConfig                   = null,
                                      DateTimeOffset?                                            LocalAccessNotBefore              = null,
                                      DateTimeOffset?                                            LocalAccessNotAfter               = null,
                                      Boolean?                                                   LocalAllowDowngrades              = false,
                                      AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                                      PartyStatus?                                               Status                            = null,

                                      DateTimeOffset?                                            Created                           = null,
                                      DateTimeOffset?                                            LastUpdated                       = null,
                                      EventTracking_Id?                                          EventTrackingId                   = null,
                                      User_Id?                                                   CurrentUserId                     = null)

        {

            if (remoteParties.TryGetValue(Id, out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessToken,

                                     RemoteVersionsURL,
                                     RemoteAccessToken,
                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,
                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion

        #region AddRemotePartyIfNotExists (Id, CredentialsRoles, LocalAccessInfos, RemoteAccessInfos, ...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(RemoteParty_Id                 Id,
                                      IEnumerable<CredentialsRole>   CredentialsRoles,

                                      IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                                      IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                                      PartyStatus?                   Status            = null,

                                      DateTimeOffset?                Created           = null,
                                      DateTimeOffset?                LastUpdated       = null,
                                      EventTracking_Id?              EventTrackingId   = null,
                                      User_Id?                       CurrentUserId     = null)

        {

            if (remoteParties.TryGetValue(Id, out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return AddResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return AddResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be added!"
                   );

        }

        #endregion


        #region AddOrUpdateRemoteParty    (Id, CredentialsRoles, LocalAccessToken, ...)

        /// <summary>
        /// Create or update a Remote Party with local access only.
        /// The remote party will start the OCPI registration process afterwards.
        /// Maybe some remote access parameters need already to be set for a successful registration.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CredentialsRoles"></param>
        /// 
        /// <param name="LocalAccessToken"></param>
        /// <param name="LocalAccessTokenBase64Encoding"></param>
        /// <param name="LocalTOTPConfig"></param>
        /// <param name="LocalAccessNotBefore"></param>
        /// <param name="LocalAccessNotAfter"></param>
        /// <param name="LocalAllowDowngrades"></param>
        /// <param name="LocalAccessStatus"></param>
        /// 
        /// <param name="RemoteAccessTokenBase64Encoding"></param>
        /// <param name="RemoteTOTPConfig"></param>
        /// <param name="RemoteAccessNotBefore"></param>
        /// <param name="RemoteAccessNotAfter"></param>
        /// <param name="PreferIPv4"></param>
        /// <param name="RemoteCertificateValidator"></param>
        /// <param name="LocalCertificateSelector"></param>
        /// <param name="ClientCertificates"></param>
        /// <param name="ClientCertificateContext"></param>
        /// <param name="ClientCertificateChain"></param>
        /// <param name="TLSProtocols"></param>
        /// <param name="ContentType"></param>
        /// <param name="Accept"></param>
        /// <param name="HTTPUserAgent"></param>
        /// <param name="RequestTimeout"></param>
        /// <param name="TransmissionRetryDelay"></param>
        /// <param name="MaxNumberOfRetries"></param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// <param name="RemoteStatus"></param>
        /// <param name="RemoteAllowDowngrades"></param>
        /// 
        /// <param name="Status"></param>
        /// 
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                   IEnumerable<CredentialsRole>                               CredentialsRoles,

                                   AccessToken                                                LocalAccessToken,
                                   Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                                   TOTPConfig?                                                LocalTOTPConfig                   = null,
                                   DateTimeOffset?                                            LocalAccessNotBefore              = null,
                                   DateTimeOffset?                                            LocalAccessNotAfter               = null,
                                   Boolean?                                                   LocalAllowDowngrades              = null,
                                   AccessStatus?                                              LocalAccessStatus                 = null,

                                   Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                   TOTPConfig?                                                RemoteTOTPConfig                  = null,
                                   DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                   DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                   Boolean?                                                   PreferIPv4                        = null,
                                   RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                                   LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                                   IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                                   SslStreamCertificateContext?                               ClientCertificateContext          = null,
                                   IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                                   SslProtocols?                                              TLSProtocols                      = null,
                                   HTTPContentType?                                           ContentType                       = null,
                                   AcceptTypes?                                               Accept                            = null,
                                   String?                                                    HTTPUserAgent                     = null,
                                   TimeSpan?                                                  RequestTimeout                    = null,
                                   TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                                   UInt16?                                                    MaxNumberOfRetries                = null,
                                   UInt32?                                                    InternalBufferSize                = null,
                                   Boolean?                                                   UseHTTPPipelining                 = null,
                                   RemoteAccessStatus?                                        RemoteStatus                      = null,
                                   Boolean?                                                   RemoteAllowDowngrades             = null,

                                   PartyStatus?                                               Status                            = null,

                                   DateTimeOffset?                                            Created                           = null,
                                   DateTimeOffset?                                            LastUpdated                       = null,
                                   EventTracking_Id?                                          EventTrackingId                   = null,
                                   User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessToken,
                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,
                                     RemoteStatus,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            var added = false;

            remoteParties.AddOrUpdate(
                newRemoteParty.Id,

                // Add
                id => {
                    added = true;
                    return newRemoteParty;
                },

                // Update
                (id, oldRemoteParty) => {
                    return newRemoteParty;
                }
            );

            await LogRemoteParty(
                      CommonHTTPAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

            return added
                       ? AddOrUpdateResult<RemoteParty>.Created(
                             EventTracking_Id.New,
                             newRemoteParty
                         )
                       : AddOrUpdateResult<RemoteParty>.Updated(
                             EventTracking_Id.New,
                             newRemoteParty
                         );

        }

        #endregion

        #region AddOrUpdateRemoteParty    (Id, CredentialsRoles,                   RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                   IEnumerable<CredentialsRole>                               CredentialsRoles,

                                   URL                                                        RemoteVersionsURL,
                                   AccessToken                                                RemoteAccessToken,
                                   Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                   TOTPConfig?                                                RemoteTOTPConfig                  = null,

                                   Boolean?                                                   PreferIPv4                        = null,
                                   RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                                   LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                                   IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                                   SslStreamCertificateContext?                               ClientCertificateContext          = null,
                                   IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                                   SslProtocols?                                              TLSProtocols                      = null,
                                   HTTPContentType?                                           ContentType                       = null,
                                   AcceptTypes?                                               Accept                            = null,
                                   String?                                                    HTTPUserAgent                     = null,
                                   TimeSpan?                                                  RequestTimeout                    = null,
                                   TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                                   UInt16?                                                    MaxNumberOfRetries                = null,
                                   UInt32?                                                    InternalBufferSize                = null,
                                   Boolean?                                                   UseHTTPPipelining                 = null,

                                   RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                   IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                   Version_Id?                                                SelectedVersionId                 = null,
                                   DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                   DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                   Boolean?                                                   RemoteAllowDowngrades             = null,

                                   PartyStatus?                                               Status                            = null,

                                   DateTimeOffset?                                            Created                           = null,
                                   DateTimeOffset?                                            LastUpdated                       = null,
                                   EventTracking_Id?                                          EventTrackingId                   = null,
                                   User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     RemoteVersionsURL,
                                     RemoteAccessToken,
                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,

                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            var added = false;

            remoteParties.AddOrUpdate(
                newRemoteParty.Id,

                // Add
                id => {
                    added = true;
                    return newRemoteParty;
                },

                // Update
                (id, oldRemoteParty) => {
                    return newRemoteParty;
                }
            );

            await LogRemoteParty(
                      CommonHTTPAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

            return added
                       ? AddOrUpdateResult<RemoteParty>.Created(
                             EventTracking_Id.New,
                             newRemoteParty
                         )
                       : AddOrUpdateResult<RemoteParty>.Updated(
                             EventTracking_Id.New,
                             newRemoteParty
                         );

        }

        #endregion

        #region AddOrUpdateRemoteParty    (Id, CredentialsRoles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                   IEnumerable<CredentialsRole>                               CredentialsRoles,

                                   AccessToken                                                LocalAccessToken,

                                   URL                                                        RemoteVersionsURL,
                                   AccessToken                                                RemoteAccessToken,
                                   Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                   TOTPConfig?                                                RemoteTOTPConfig                  = null,

                                   Boolean?                                                   PreferIPv4                        = null,
                                   RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                                   LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                                   IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                                   SslStreamCertificateContext?                               ClientCertificateContext          = null,
                                   IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                                   SslProtocols?                                              TLSProtocols                      = null,
                                   HTTPContentType?                                           ContentType                       = null,
                                   AcceptTypes?                                               Accept                            = null,
                                   String?                                                    HTTPUserAgent                     = null,
                                   TimeSpan?                                                  RequestTimeout                    = null,
                                   TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                                   UInt16?                                                    MaxNumberOfRetries                = null,
                                   UInt32?                                                    InternalBufferSize                = null,
                                   Boolean?                                                   UseHTTPPipelining                 = null,
                                   RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                   IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                   Version_Id?                                                SelectedVersionId                 = null,
                                   DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                   DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                   Boolean?                                                   RemoteAllowDowngrades             = null,

                                   Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                                   TOTPConfig?                                                LocalTOTPConfig                   = null,
                                   DateTimeOffset?                                            LocalAccessNotBefore              = null,
                                   DateTimeOffset?                                            LocalAccessNotAfter               = null,
                                   Boolean?                                                   LocalAllowDowngrades              = false,
                                   AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                                   PartyStatus?                                               Status                            = null,

                                   DateTimeOffset?                                            Created                           = null,
                                   DateTimeOffset?                                            LastUpdated                       = null,
                                   EventTracking_Id?                                          EventTrackingId                   = null,
                                   User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessToken,

                                     RemoteVersionsURL,
                                     RemoteAccessToken,
                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,
                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            var added = false;

            remoteParties.AddOrUpdate(
                newRemoteParty.Id,

                // Add
                id => {
                    added = true;
                    return newRemoteParty;
                },

                // Update
                (id, oldRemoteParty) => {
                    return newRemoteParty;
                }
            );

            await LogRemoteParty(
                      CommonHTTPAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

            return added
                       ? AddOrUpdateResult<RemoteParty>.Created(
                             EventTracking_Id.New,
                             newRemoteParty
                         )
                       : AddOrUpdateResult<RemoteParty>.Updated(
                             EventTracking_Id.New,
                             newRemoteParty
                         );

        }

        #endregion

        #region AddOrUpdateRemoteParty    (Id, CredentialsRoles, LocalAccessInfos, RemoteAccessInfos, ...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(RemoteParty_Id                 Id,
                                   IEnumerable<CredentialsRole>   CredentialsRoles,

                                   IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                                   IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                                   PartyStatus?                   Status            = null,

                                   DateTimeOffset?                Created           = null,
                                   DateTimeOffset?                LastUpdated       = null,
                                   EventTracking_Id?              EventTrackingId   = null,
                                   User_Id?                       CurrentUserId     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     Id,
                                     CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            var added = false;

            remoteParties.AddOrUpdate(
                newRemoteParty.Id,

                // Add
                id => {
                    added = true;
                    return newRemoteParty;
                },

                // Update
                (id, oldRemoteParty) => {
                    return newRemoteParty;
                }
            );

            await LogRemoteParty(
                      CommonHTTPAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

            return added
                       ? AddOrUpdateResult<RemoteParty>.Created(
                             EventTracking_Id.New,
                             newRemoteParty
                         )
                       : AddOrUpdateResult<RemoteParty>.Updated(
                             EventTracking_Id.New,
                             newRemoteParty
                         );

        }

        #endregion


        #region UpdateRemoteParty         (Id, CredentialsRoles, LocalAccessToken, ...)

        /// <summary>
        /// Update a Remote Party with local access only.
        /// The remote party will start the OCPI registration process afterwards.
        /// Maybe some remote access parameters need already to be set for a successful registration.
        /// </summary>
        /// <param name="ExistingRemoteParty"></param>
        /// <param name="CredentialsRoles"></param>
        /// 
        /// <param name="LocalAccessToken"></param>
        /// <param name="LocalAccessTokenBase64Encoding"></param>
        /// <param name="LocalTOTPConfig"></param>
        /// <param name="LocalAccessNotBefore"></param>
        /// <param name="LocalAccessNotAfter"></param>
        /// <param name="LocalAllowDowngrades"></param>
        /// <param name="LocalAccessStatus"></param>
        /// 
        /// <param name="RemoteAccessTokenBase64Encoding"></param>
        /// <param name="RemoteTOTPConfig"></param>
        /// <param name="RemoteAccessNotBefore"></param>
        /// <param name="RemoteAccessNotAfter"></param>
        /// <param name="PreferIPv4"></param>
        /// <param name="RemoteCertificateValidator"></param>
        /// <param name="LocalCertificateSelector"></param>
        /// <param name="ClientCertificates"></param>
        /// <param name="ClientCertificateContext"></param>
        /// <param name="ClientCertificateChain"></param>
        /// <param name="TLSProtocols"></param>
        /// <param name="ContentType"></param>
        /// <param name="Accept"></param>
        /// <param name="HTTPUserAgent"></param>
        /// <param name="RequestTimeout"></param>
        /// <param name="TransmissionRetryDelay"></param>
        /// <param name="MaxNumberOfRetries"></param>
        /// <param name="InternalBufferSize"></param>
        /// <param name="UseHTTPPipelining"></param>
        /// <param name="RemoteStatus"></param>
        /// <param name="RemoteAllowDowngrades"></param>
        /// 
        /// <param name="Status"></param>
        /// 
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                              IEnumerable<CredentialsRole>                               CredentialsRoles,

                              AccessToken                                                LocalAccessToken,
                              Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                              TOTPConfig?                                                LocalTOTPConfig                   = null,
                              DateTimeOffset?                                            LocalAccessNotBefore              = null,
                              DateTimeOffset?                                            LocalAccessNotAfter               = null,
                              Boolean?                                                   LocalAllowDowngrades              = null,
                              AccessStatus?                                              LocalAccessStatus                 = null,

                              Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                              TOTPConfig?                                                RemoteTOTPConfig                  = null,
                              DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                              DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                              Boolean?                                                   PreferIPv4                        = null,
                              RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                              LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                              IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                              SslStreamCertificateContext?                               ClientCertificateContext          = null,
                              IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                              SslProtocols?                                              TLSProtocols                      = null,
                              HTTPContentType?                                           ContentType                       = null,
                              AcceptTypes?                                               Accept                            = null,
                              String?                                                    HTTPUserAgent                     = null,
                              TimeSpan?                                                  RequestTimeout                    = null,
                              TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                              UInt16?                                                    MaxNumberOfRetries                = null,
                              UInt32?                                                    InternalBufferSize                = null,
                              Boolean?                                                   UseHTTPPipelining                 = null,
                              RemoteAccessStatus?                                        RemoteStatus                      = null,
                              Boolean?                                                   RemoteAllowDowngrades             = null,

                              PartyStatus?                                               Status                            = null,

                              DateTimeOffset?                                            Created                           = null,
                              DateTimeOffset?                                            LastUpdated                       = null,
                              EventTracking_Id?                                          EventTrackingId                   = null,
                              User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.Id,
                                     CredentialsRoles,

                                     LocalAccessToken,
                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,
                                     RemoteStatus,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return UpdateResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return UpdateResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be updated!"
                   );

        }

        #endregion

        #region UpdateRemoteParty         (Id, CredentialsRoles,                   RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                              IEnumerable<CredentialsRole>                               CredentialsRoles,

                              URL                                                        RemoteVersionsURL,
                              AccessToken                                                RemoteAccessToken,
                              Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                              TOTPConfig?                                                RemoteTOTPConfig                  = null,

                              Boolean?                                                   PreferIPv4                        = null,
                              RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                              LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                              IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                              SslStreamCertificateContext?                               ClientCertificateContext          = null,
                              IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                              SslProtocols?                                              TLSProtocols                      = null,
                              HTTPContentType?                                           ContentType                       = null,
                              AcceptTypes?                                               Accept                            = null,
                              String?                                                    HTTPUserAgent                     = null,
                              TimeSpan?                                                  RequestTimeout                    = null,
                              TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                              UInt16?                                                    MaxNumberOfRetries                = null,
                              UInt32?                                                    InternalBufferSize                = null,
                              Boolean?                                                   UseHTTPPipelining                 = null,

                              RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                              IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                              Version_Id?                                                SelectedVersionId                 = null,
                              DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                              DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                              Boolean?                                                   RemoteAllowDowngrades             = null,

                              PartyStatus?                                               Status                            = null,

                              DateTimeOffset?                                            Created                           = null,
                              DateTimeOffset?                                            LastUpdated                       = null,
                              EventTracking_Id?                                          EventTrackingId                   = null,
                              User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.Id,
                                     CredentialsRoles,

                                     RemoteVersionsURL,
                                     RemoteAccessToken,
                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,

                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return UpdateResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return UpdateResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be updated!"
                   );

        }

        #endregion

        #region UpdateRemoteParty         (Id, CredentialsRoles, LocalAccessToken, RemoteVersionsURL, RemoteAccessToken, ...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                              IEnumerable<CredentialsRole>                               CredentialsRoles,

                              AccessToken                                                LocalAccessToken,

                              URL                                                        RemoteVersionsURL,
                              AccessToken                                                RemoteAccessToken,
                              Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                              TOTPConfig?                                                RemoteTOTPConfig                  = null,

                              Boolean?                                                   PreferIPv4                        = null,
                              RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                              LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                              IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                              SslStreamCertificateContext?                               ClientCertificateContext          = null,
                              IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                              SslProtocols?                                              TLSProtocols                      = null,
                              HTTPContentType?                                           ContentType                       = null,
                              AcceptTypes?                                               Accept                            = null,
                              String?                                                    HTTPUserAgent                     = null,
                              TimeSpan?                                                  RequestTimeout                    = null,
                              TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                              UInt16?                                                    MaxNumberOfRetries                = null,
                              UInt32?                                                    InternalBufferSize                = null,
                              Boolean?                                                   UseHTTPPipelining                 = null,
                              RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                              IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                              Version_Id?                                                SelectedVersionId                 = null,
                              DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                              DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                              Boolean?                                                   RemoteAllowDowngrades             = null,

                              Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                              TOTPConfig?                                                LocalTOTPConfig                   = null,
                              DateTimeOffset?                                            LocalAccessNotBefore              = null,
                              DateTimeOffset?                                            LocalAccessNotAfter               = null,
                              Boolean?                                                   LocalAllowDowngrades              = false,
                              AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                              PartyStatus?                                               Status                            = null,

                              DateTimeOffset?                                            Created                           = null,
                              DateTimeOffset?                                            LastUpdated                       = null,
                              EventTracking_Id?                                          EventTrackingId                   = null,
                              User_Id?                                                   CurrentUserId                     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.Id,
                                     CredentialsRoles,

                                     LocalAccessToken,

                                     RemoteVersionsURL,
                                     RemoteAccessToken,
                                     RemoteAccessTokenBase64Encoding,
                                     RemoteTOTPConfig,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCertificates,
                                     ClientCertificateContext,
                                     ClientCertificateChain,
                                     TLSProtocols,
                                     ContentType,
                                     Accept,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,
                                     RemoteStatus,
                                     RemoteVersionIds,
                                     SelectedVersionId,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,
                                     RemoteAllowDowngrades,

                                     LocalAccessTokenBase64Encoding,
                                     LocalTOTPConfig,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,
                                     LocalAllowDowngrades,
                                     LocalAccessStatus,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return UpdateResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return UpdateResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be updated!"
                   );

        }

        #endregion

        #region UpdateRemoteParty         (Id, CredentialsRoles, LocalAccessInfos, RemoteAccessInfos, ...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                    ExistingRemoteParty,
                              IEnumerable<CredentialsRole>   CredentialsRoles,

                              IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                              IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                              PartyStatus?                   Status            = null,

                              DateTimeOffset?                Created           = null,
                              DateTimeOffset?                LastUpdated       = null,
                              EventTracking_Id?              EventTrackingId   = null,
                              User_Id?                       CurrentUserId     = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.Id,
                                     CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     Created,
                                     LastUpdated

                                 );

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return UpdateResult<RemoteParty>.Success(
                           EventTracking_Id.New,
                           newRemoteParty
                       );

            }

            return UpdateResult<RemoteParty>.Failed(
                       EventTracking_Id.New,
                       newRemoteParty,
                       "The remote party could not be updated!"
                   );

        }

        #endregion


        #region ContainsRemoteParty       (RemotePartyId)

        /// <summary>
        /// Whether this API contains a remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public Boolean ContainsRemoteParty(RemoteParty_Id RemotePartyId)

            => remoteParties.ContainsKey(RemotePartyId);

        #endregion

        #region GetRemoteParty            (RemotePartyId)

        /// <summary>
        /// Get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public RemoteParty? GetRemoteParty(RemoteParty_Id RemotePartyId)
        {

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty))
                return remoteParty;

            return null;

        }

        #endregion

        #region TryGetRemoteParty         (RemotePartyId, out RemoteParty)

        /// <summary>
        /// Try to get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        /// <param name="RemoteParty">The remote party.</param>
        public Boolean TryGetRemoteParty(RemoteParty_Id                        RemotePartyId,
                                         [NotNullWhen(true)] out RemoteParty?  RemoteParty)

            => remoteParties.TryGetValue(
                   RemotePartyId,
                   out RemoteParty
               );

        #endregion

        #region GetRemoteParties          (IncludeFilter = null)

        /// <summary>
        /// Get all remote parties machting the given optional filter.
        /// </summary>
        /// <param name="IncludeFilter">A delegate for filtering remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(IncludeRemoteParty? IncludeFilter = null)

            => IncludeFilter is null
                   ? remoteParties.Values
                   : remoteParties.Values.
                                   Where(remoteParty => IncludeFilter(remoteParty));

        #endregion

        #region GetRemoteParties          (CountryCode, PartyId)

        /// <summary>
        /// Get all remote parties having the given country code and party identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Id     PartyId)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.PartyId.CountryCode == CountryCode &&
                                                                                           credentialsRole.PartyId.PartyId       == PartyId));

        #endregion

        #region GetRemoteParties          (CountryCode, PartyId, Role)

        /// <summary>
        /// Get all remote parties having the given country code, party identification and role.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A role.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Id     PartyId,
                                                         Role         Role)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.PartyId.CountryCode == CountryCode &&
                                                                                           credentialsRole.PartyId.PartyId       == PartyId &&
                                                                                           credentialsRole.Role                == Role));

        #endregion

        #region GetRemoteParties          (Roles)

        /// <summary>
        /// Get all remote parties having one of the given roles.
        /// </summary>
        /// <param name="Roles">An optional list of roles.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(params Role[] Roles)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => Roles.Contains(credentialsRole.Role)));

        #endregion

        #region GetRemoteParties          (AccessToken)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken AccessToken)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken));

        #endregion

        #region GetRemoteParties          (AccessToken, AccessStatus)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken   AccessToken,
                                                         AccessStatus  AccessStatus)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken &&
                                                                                                      localAccessInfo.Status      == AccessStatus));

        #endregion

        #region TryGetRemoteParties       (AccessToken, TOTP, TLSExporterMaterial, out RemoteParties, out ErrorMessage)

        public Boolean TryGetRemoteParties(AccessToken                                           AccessToken,
                                           TOTPHTTPHeader?                                       TOTP,
                                           Byte[]?                                               TLSExporterMaterial,
                                           out IEnumerable<Tuple<RemoteParty, LocalAccessInfo>>  RemoteParties,
                                           [NotNullWhen(false)] out String?                      ErrorMessage)
        {

            var remoteParties = new List<Tuple<RemoteParty, LocalAccessInfo>>();

            foreach (var remoteParty in this.remoteParties.Values)
            {
                foreach (var localAccessInfo in remoteParty.LocalAccessInfos)
                {

                    if (localAccessInfo.AccessToken == AccessToken)
                    {

                        if (localAccessInfo.TOTPConfig is not null)
                        {

                            var (previous,
                                 current,
                                 next,
                                 _,
                                 _) = TOTPGenerator.GenerateTOTPs(
                                          localAccessInfo.TOTPConfig.SharedSecret,
                                          localAccessInfo.TOTPConfig.ValidityTime,
                                          localAccessInfo.TOTPConfig.Length,
                                          localAccessInfo.TOTPConfig.Alphabet,
                                          null,
                                          TLSExporterMaterial
                                      );

                            if (TOTP?.Value == current || TOTP?.Value == previous || TOTP?.Value == next)
                                remoteParties.Add(
                                    new Tuple<RemoteParty, LocalAccessInfo>(
                                        remoteParty,
                                        localAccessInfo
                                    )
                                );

                            else
                            {
                                RemoteParties  = [];
                                ErrorMessage   = "Invalid Time-based One-Time Password (TOTP)!";
                                return false;
                            }

                        }

                        else
                            remoteParties.Add(
                                new Tuple<RemoteParty, LocalAccessInfo>(
                                    remoteParty,
                                    localAccessInfo
                                )
                            );

                    }

                }
            }

            RemoteParties  = remoteParties;
            ErrorMessage   = remoteParties.Count == 0
                                 ? "Unknown access token!"
                                 : null;

            return remoteParties.Count > 0;

        }

        #endregion


        #region RemoveRemoteParty         (RemoteParty)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty        RemoteParty,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.TryRemove(RemoteParty.Id, out var remoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty         (RemotePartyId)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty_Id     RemotePartyId,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.Remove(RemotePartyId, out var remoteParty))
            {

                await LogRemoteParty(
                          CommonHTTPAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty         (CountryCode, PartyId, Role)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     Role               Role,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in GetRemoteParties(CountryCode,
                                                         PartyId,
                                                         Role))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(
                          CommonHTTPAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

            }

            return true;

        }

        #endregion

        #region RemoveRemoteParty         (CountryCode, PartyId, Role, AccessToken)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     Role               Role,
                                                     AccessToken        AccessToken,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.
                                                      Where(remoteParty => remoteParty.Roles.            Any(credentialsRole  => credentialsRole.PartyId.CountryCode == CountryCode &&
                                                                                                                                 credentialsRole.PartyId.PartyId       == PartyId &&
                                                                                                                                 credentialsRole.Role                == Role) &&

                                                                           remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == AccessToken)))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(
                          CommonHTTPAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

            }

            return true;

        }

        #endregion

        #region RemoveAllRemoteParties    ()

        public async Task RemoveAllRemoteParties(EventTracking_Id?  EventTrackingId   = null,
                                                 User_Id?           CurrentUserId     = null)
        {

            remoteParties.Clear();

            await LogRemoteParty("removeAllRemoteParties",
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

        }

        #endregion

        #endregion


        #region Parties (local)

        #region Data

        private readonly ConcurrentDictionary<Party_Idv3, PartyData> parties = [];


        public delegate Task OnPartyAddedDelegate  (PartyData Party);
        public delegate Task OnPartyChangedDelegate(PartyData Party);
        public delegate Task OnPartyRemovedDelegate(PartyData Party);

        public event OnPartyAddedDelegate?    OnPartyAdded;
        public event OnPartyChangedDelegate?  OnPartyChanged;
        public event OnPartyRemovedDelegate?  OnPartyRemoved;

        #endregion


        #region AddParty            (Id, Role, BusinessDetails, AllowDowngrades = null, ...)

        public async Task<AddResult<PartyData>>

            AddParty(PartyData          PartyData,
                     Boolean?           AllowDowngrades     = null,
                     Boolean            SkipNotifications   = false,
                     EventTracking_Id?  EventTrackingId     = null,
                     User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryAdd(PartyData.Id, PartyData))
            {

                await LogAsset(
                          "addParty",
                          JSONObject.Create(
                              new JProperty("id",  PartyData.Id.ToString())
                          ),
                          EventTrackingId,
                          CurrentUserId
                      );

                //if (!SkipNotifications)
                //{

                //    var OnPartyAddedLocal = OnPartyAdded;
                //    if (OnPartyAddedLocal is not null)
                //    {
                //        try
                //        {
                //            await OnPartyAddedLocal(Party);
                //        }
                //        catch (Exception e)
                //        {
                //            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddParty), " ", nameof(OnPartyAdded), ": ",
                //                        Environment.NewLine, e.Message,
                //                        Environment.NewLine, e.StackTrace ?? "");
                //        }
                //    }

                //}

                return AddResult<PartyData>.Success(
                           EventTrackingId,
                           PartyData
                       );

            }

            return AddResult<PartyData>.Failed(
                       EventTrackingId,
                       PartyData,
                       "The given party identification already exists!"
                   );

        }

        #endregion

        #region AddParty            (Id, Role, BusinessDetails, AllowDowngrades = null, ...)

        public async Task<AddResult<PartyData>>

            AddParty(Party_Idv3         Id,
                     Role               Role,
                     BusinessDetails    BusinessDetails,
                     Boolean?           AllowDowngrades     = null,
                     Boolean            SkipNotifications   = false,
                     EventTracking_Id?  EventTrackingId     = null,
                     User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var newParty = new PartyData(
                               Id,
                               Role,
                               BusinessDetails,
                               AllowDowngrades
                           );

            if (parties.TryAdd(Id, newParty))
            {

                await LogAsset(
                          "addParty",
                          JSONObject.Create(
                              new JProperty("id",  Id.ToString())
                          ),
                          EventTrackingId,
                          CurrentUserId
                      );

                //if (!SkipNotifications)
                //{

                //    var OnPartyAddedLocal = OnPartyAdded;
                //    if (OnPartyAddedLocal is not null)
                //    {
                //        try
                //        {
                //            await OnPartyAddedLocal(Party);
                //        }
                //        catch (Exception e)
                //        {
                //            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddParty), " ", nameof(OnPartyAdded), ": ",
                //                        Environment.NewLine, e.Message,
                //                        Environment.NewLine, e.StackTrace ?? "");
                //        }
                //    }

                //}

                return AddResult<PartyData>.Success(
                           EventTrackingId,
                           newParty
                       );

            }

            return AddResult<PartyData>.Failed(
                       EventTrackingId,
                       newParty,
                       "The given party identification already exists!"
                   );

        }

        #endregion


        public Boolean HasParty(Party_Idv3 PartyId)
            => parties.ContainsKey(PartyId);

        public IEnumerable<PartyData> Parties
            => parties.Values;

        #endregion


        #region Locations

        #region Locations

        #region Events

        // Note: Charging locations/EVSEs are expected to be always in memory!

        public delegate Task OnLocationAddedDelegate  (Location Location);
        public delegate Task OnLocationChangedDelegate(Location Location);
        public delegate Task OnLocationRemovedDelegate(Location Location);

        public event OnLocationAddedDelegate?    OnLocationAdded;
        public event OnLocationChangedDelegate?  OnLocationChanged;
        public event OnLocationRemovedDelegate?  OnLocationRemoved;

        #endregion


        #region AddLocation            (Location, ...)

        /// <summary>
        /// Add the given charging location.
        /// </summary>
        /// <param name="Location">The charging location to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Location>>

            AddLocation(Location           Location,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null,
                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Location.CountryCode,
                                    Location.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Locations.TryAdd(Location.Id, Location))
                {

                    Location.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addLocation,
                              Location.ToJSON(
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
                                  CustomParkingSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomEVSEParkingSerializer,
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
                                  OnLocationAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Location
                                  )
                              );

                        foreach (var evse in Location.EVSEs)
                            await LogEvent(
                                      OnEVSEAdded,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          evse
                                      )
                                  );

                    }

                    return AddResult<Location>.Success(
                               EventTrackingId,
                               Location
                           );

                }

                return AddResult<Location>.Failed(
                           EventTrackingId,
                           Location,
                           "The given location already exists!"
                       );

            }

            return AddResult<Location>.Failed(
                       EventTrackingId,
                       Location,
                       $"The party identification '{partyId}' of the location is unknown!"
                   );

        }

        #endregion

        #region AddLocationIfNotExists (Location, ...)

        /// <summary>
        /// Add the given charging location if it does not already exist.
        /// </summary>
        /// <param name="Location">The charging location to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Location>>

            AddLocationIfNotExists(Location           Location,
                                   Boolean            SkipNotifications   = false,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   User_Id?           CurrentUserId       = null,
                                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Location.CountryCode,
                                    Location.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Locations.TryAdd(Location.Id, Location))
                {

                    Location.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addLocation,
                              Location.ToJSON(
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
                                  CustomParkingSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomEVSEParkingSerializer,
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
                                  OnLocationAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Location
                                  )
                              );

                        foreach (var evse in Location.EVSEs)
                            await LogEvent(
                                      OnEVSEAdded,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          evse
                                      )
                                  );

                    }

                    return AddResult<Location>.Success(
                               EventTrackingId,
                               Location
                           );

                }

                return AddResult<Location>.NoOperation(
                           EventTrackingId,
                           Location,
                           "The given location already exists."
                       );

            }

            return AddResult<Location>.Failed(
                       EventTrackingId,
                       Location,
                       $"The party identification '{partyId}' of the location is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateLocation    (Location,                           AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given charging location.
        /// </summary>
        /// <param name="Location">The charging location to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<Location>>

            AddOrUpdateLocation(Location           Location,
                                Boolean?           AllowDowngrades     = false,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Location.CountryCode,
                                    Location.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                #region Update an existing location

                if (party.Locations.TryGetValue(Location.Id, out var existingLocation))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        Location.LastUpdated <= existingLocation.LastUpdated)
                    {
                        return AddOrUpdateResult<Location>.Failed(
                                   EventTrackingId, Location,
                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!"
                               );
                    }

                    //if (Location.LastUpdated.ToISO8601() == existingLocation.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<Location>.NoOperation(Location,
                    //                                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");

                    if (party.Locations.TryUpdate(Location.Id,
                                                  Location,
                                                  existingLocation))
                    {

                        Location.CommonAPI = this;

                        await LogAsset(
                                  CommonHTTPAPI.addOrUpdateLocation,
                                  Location.ToJSON(
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
                                      CustomParkingSerializer,
                                      CustomDisplayTextSerializer,
                                      CustomBusinessDetailsSerializer,
                                      CustomHoursSerializer,
                                      CustomEVSEParkingSerializer,
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
                                      OnLocationChanged,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          Location
                                      )
                                  );

                            var oldEVSEUIds = new HashSet<EVSE_UId>(existingLocation.EVSEs.Select(evse => evse.UId));
                            var newEVSEUIds = new HashSet<EVSE_UId>(Location.        EVSEs.Select(evse => evse.UId));

                            foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                            {

                                if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId) && existingLocation.GetEVSE(evseUId)! != Location.GetEVSE(evseUId)!)
                                {

                                    if (existingLocation.TryGetEVSE(evseUId, out var evse))
                                        await LogEvent(
                                                  OnEVSEChanged,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      evse
                                                  )
                                              );

                                }
                                else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                                {

                                    if (existingLocation.TryGetEVSE(evseUId, out var evse))
                                        await LogEvent(
                                                  OnEVSEAdded,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      evse
                                                  )
                                              );

                                }
                                else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                                {

                                    if (existingLocation.TryGetEVSE(evseUId, out var evse))
                                        await LogEvent(
                                                  OnEVSERemoved,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      evse
                                                  )
                                              );

                                }

                            }

                        }

                        return AddOrUpdateResult<Location>.Updated(
                                   EventTrackingId,
                                   Location
                               );

                    }

                    return AddOrUpdateResult<Location>.Failed(
                               EventTrackingId,
                               Location,
                               "Updating the given location failed!"
                           );

                }

                #endregion

                #region Add a new location

                if (party.Locations.TryAdd(Location.Id, Location))
                {

                    Location.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateLocation,
                              Location.ToJSON(
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
                                  CustomParkingSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomEVSEParkingSerializer,
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
                                  OnLocationAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Location
                                  )
                              );

                        foreach (var evse in Location.EVSEs)
                            await LogEvent(
                                      OnEVSEAdded,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          evse
                                      )
                                  );

                    }

                    return AddOrUpdateResult<Location>.Created(
                               EventTrackingId,
                               Location
                           );

                }

                #endregion

                return AddOrUpdateResult<Location>.Failed(
                           EventTrackingId,
                           Location,
                           "Adding the given location failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<Location>.Failed(
                       EventTrackingId,
                       Location,
                       $"The party identification '{partyId}' of the location is unknown!"
                   );

        }

        #endregion

        #region UpdateLocation         (Location,                           AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given charging location.
        /// </summary>
        /// <param name="Location">The charging location to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<Location>>

            UpdateLocation(Location           Location,
                           Boolean?           AllowDowngrades     = false,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null,
                           CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Location.CountryCode,
                                    Location.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (!party.Locations.TryGetValue(Location.Id, out var existingLocation))
                    return UpdateResult<Location>.Failed(
                               EventTrackingId,
                               Location,
                               $"The given location identification '{Location.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Location.LastUpdated <= existingLocation.LastUpdated)
                {

                    return UpdateResult<Location>.Failed(
                                EventTrackingId, Location,
                                "The 'lastUpdated' timestamp of the new charging location must be newer then the timestamp of the existing location!"
                            );

                }

                #endregion


                if (party.Locations.TryUpdate(Location.Id,
                                              Location,
                                              existingLocation))
                {

                    Location.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.updateLocation,
                              Location.ToJSON(
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
                                  CustomParkingSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomEVSEParkingSerializer,
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
                                  OnLocationChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Location
                                  )
                              );

                        var oldEVSEUIds = new HashSet<EVSE_UId>(existingLocation.EVSEs.Select(evse => evse.UId));
                        var newEVSEUIds = new HashSet<EVSE_UId>(Location.        EVSEs.Select(evse => evse.UId));

                        foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                        {

                            if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                            {

                                if (existingLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                                    Location.        TryGetEVSE(evseUId, out var newEVSE) &&
                                    oldEVSE is not null &&
                                    newEVSE is not null)
                                {

                                    if (oldEVSE != newEVSE)
                                        await LogEvent(
                                                  OnEVSEChanged,
                                                  loggingDelegate => loggingDelegate.Invoke(
                                                      newEVSE
                                                  )
                                              );

                                    if (oldEVSE.Status != newEVSE.Status)
                                        await LogEvent(
                                                  OnEVSEStatusChanged,
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

                                if (Location.TryGetEVSE(evseUId, out var evse))
                                {

                                    await LogEvent(
                                              OnEVSEAdded,
                                              loggingDelegate => loggingDelegate.Invoke(
                                                  evse
                                              )
                                          );

                                    await LogEvent(
                                              OnEVSEStatusChanged,
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

                                if (existingLocation.TryGetEVSE(evseUId, out var evse))
                                    await LogEvent(
                                              OnEVSERemoved,
                                              loggingDelegate => loggingDelegate.Invoke(
                                                  evse
                                              )
                                          );

                                if (existingLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                                    Location.        TryGetEVSE(evseUId, out var newEVSE))
                                {
                                    await LogEvent(
                                              OnEVSEStatusChanged,
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
                               Location
                           );

                }

                return UpdateResult<Location>.Failed(
                           EventTrackingId,
                           Location,
                           "locations.TryUpdate(Location.Id, Location, Location) failed!"
                       );

            }

            return UpdateResult<Location>.Failed(
                       EventTrackingId,
                       Location,
                       $"The party identification '{partyId}' of the location is unknown!"
                   );

        }

        #endregion

        #region TryPatchLocation       (PartyId, LocationId, LocationPatch, AllowDowngrades = false, ...)

        /// <summary>
        /// Try to patch the given charging location with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the location.</param>
        /// <param name="LocationId">The identification of the charging location to patch.</param>
        /// <param name="LocationPatch">The JSON patch document to apply to the charging tariff.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<Location>>

            TryPatchLocation(Party_Idv3         PartyId,
                             Location_Id        LocationId,
                             JObject            LocationPatch,
                             Boolean?           AllowDowngrades     = false,
                             Boolean            SkipNotifications   = false,
                             EventTracking_Id?  EventTrackingId     = null,
                             User_Id?           CurrentUserId       = null,
                             CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Locations.TryGetValue(LocationId, out var existingLocation))
                {

                    var patchResult = existingLocation.TryPatch(
                                          LocationPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedLocation))
                    {

                        var updateLocationResult = await UpdateLocation(
                                                             patchedLocation,
                                                             AllowDowngrades,
                                                             SkipNotifications,
                                                             EventTrackingId,
                                                             CurrentUserId,
                                                             CancellationToken
                                                         );

                        if (updateLocationResult.IsFailed)
                            return PatchResult<Location>.Failed(
                                       EventTrackingId,
                                       existingLocation,
                                       "Could not patch the location: " + updateLocationResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<Location>.Failed(
                           EventTrackingId,
                           $"The given location '{LocationId}' is unknown!"
                       );

            }

            return PatchResult<Location>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the location is unknown!"
                   );

        }

        #endregion


        #region RemoveLocation         (Location, ...)

        /// <summary>
        /// Remove the given charging location.
        /// </summary>
        /// <param name="Location">A charging location.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<Location>>

            RemoveLocation(Location           Location,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null,
                           CancellationToken  CancellationToken   = default)

                => RemoveLocation(
                       Party_Idv3.From(
                           Location.CountryCode,
                           Location.PartyId
                       ),
                       Location.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveLocation         (PartyId, LocationId, ...)

        /// <summary>
        /// Remove the given charging location.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the charging location.</param>
        /// <param name="LocationId">An unique charging location identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<Location>>

            RemoveLocation(Party_Idv3         PartyId,
                           Location_Id        LocationId,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null,
                           CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Locations.TryRemove(LocationId, out var location))
                {

                    await LogAsset(
                              CommonHTTPAPI.removeLocation,
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
                                  CustomParkingSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomBusinessDetailsSerializer,
                                  CustomHoursSerializer,
                                  CustomEVSEParkingSerializer,
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
                                  OnLocationRemoved,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      location
                                  )
                              );

                        foreach (var evse in location.EVSEs)
                            await LogEvent(
                                      OnEVSERemoved,
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
                           "The location identification of the location is unknown!"
                       );

            }

            return RemoveResult<Location>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the location is unknown!"
                   );

        }

        #endregion

        #region RemoveAllLocations     (...)

        /// <summary>
        /// Remove all charging locations.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveAllLocations(Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var locations = new List<Location>();

            foreach (var party in parties.Values)
            {
                locations.AddRange(party.Locations.Values);
                party.Locations.Clear();
            }

            await LogAsset(
                      CommonHTTPAPI.removeAllLocations,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var location in locations)
                    await LogEvent(
                              OnLocationRemoved,
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

        #region RemoveAllLocations     (IncludeLocations, ...)

        /// <summary>
        /// Remove all matching charging locations.
        /// </summary>
        /// <param name="IncludeLocations">A filter delegate to include charging locations for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveAllLocations(Func<Location, Boolean>  IncludeLocations,
                               Boolean                  SkipNotifications   = false,
                               EventTracking_Id?        EventTrackingId     = null,
                               User_Id?                 CurrentUserId       = null,
                               CancellationToken        CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingLocations  = new List<Location>();
            var removedLocations   = new List<Location>();
            var failedLocations    = new List<RemoveResult<Location>>();

            foreach (var party in parties.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (IncludeLocations(location))
                        matchingLocations.Add(location);
                }
            }

            foreach (var location in matchingLocations)
            {

                var result = await RemoveLocation(
                                       location,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedLocations.Add(location);
                else
                    failedLocations. Add(result);

            }

            return removedLocations.Count != 0 && failedLocations.Count == 0

                       ? RemoveResult<IEnumerable<Location>>.Success(
                             EventTrackingId,
                             removedLocations
                         )

                       : removedLocations.Count == 0 && failedLocations.Count == 0

                             ? RemoveResult<IEnumerable<Location>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Location>>.Failed(
                                   EventTrackingId,
                                   failedLocations.
                                       Select(removeResult => removeResult.Data).
                                       Where (location      => location is not null).
                                       Cast<Location>(),
                                   failedLocations.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllLocations     (IncludeLocationIds, ...)

        /// <summary>
        /// Remove all matching charging locations.
        /// </summary>
        /// <param name="IncludeLocationIds">A filter delegate to include charging locations for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveAllLocations(Func<Party_Idv3, Location_Id, Boolean>  IncludeLocationIds,
                               Boolean                                 SkipNotifications   = false,
                               EventTracking_Id?                       EventTrackingId     = null,
                               User_Id?                                CurrentUserId       = null,
                               CancellationToken                       CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingLocations  = new List<Location>();
            var removedLocations   = new List<Location>();
            var failedLocations    = new List<RemoveResult<Location>>();

            foreach (var party in parties.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (IncludeLocationIds(party.Id, location.Id))
                        matchingLocations.Add(location);
                }
            }

            foreach (var location in matchingLocations)
            {

                var result = await RemoveLocation(
                                       location,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedLocations.Add(location);
                else
                    failedLocations. Add(result);

            }

            return removedLocations.Count != 0 && failedLocations.Count == 0

                       ? RemoveResult<IEnumerable<Location>>.Success(
                             EventTrackingId,
                             removedLocations
                         )

                       : removedLocations.Count == 0 && failedLocations.Count == 0

                             ? RemoveResult<IEnumerable<Location>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Location>>.Failed(
                                   EventTrackingId,
                                   failedLocations.
                                       Select(removeResult => removeResult.Data).
                                       Where (location      => location is not null).
                                       Cast<Location>(),
                                   failedLocations.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllLocations     (PartyId, ...)

        /// <summary>
        /// Remove all charging locations owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveAllLocations(Party_Idv3         PartyId,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                var matchingLocations  = party.Locations.Values;
                var removedLocations   = new List<Location>();
                var failedLocations    = new List<RemoveResult<Location>>();

                foreach (var location in matchingLocations)
                {

                    var result = await RemoveLocation(
                                           location,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedLocations.Add(location);
                    else
                        failedLocations. Add(result);

                }

                return removedLocations.Count != 0 && failedLocations.Count == 0

                           ? RemoveResult<IEnumerable<Location>>.Success(
                                 EventTrackingId,
                                 removedLocations
                             )

                           : removedLocations.Count == 0 && failedLocations.Count == 0

                                 ? RemoveResult<IEnumerable<Location>>.NoOperation(
                                       EventTrackingId,
                                       []
                                   )

                                 : RemoveResult<IEnumerable<Location>>.Failed(
                                       EventTrackingId,
                                       failedLocations.
                                           Select(removeResult => removeResult.Data).
                                           Where (location      => location is not null).
                                           Cast<Location>(),
                                       failedLocations.Select(removeResult => removeResult.ErrorResponse).
                                           AggregateWith(", ")
                                   );

            }

            return RemoveResult<IEnumerable<Location>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' is unknown!"
                   );

        }

        #endregion


        #region LocationExists         (PartyId, LocationId)

        public Boolean LocationExists(Party_Idv3   PartyId,
                                      Location_Id  LocationId)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.Locations.ContainsKey(LocationId);

            return false;

        }

        #endregion

        #region TryGetLocation         (PartyId, LocationId, out Location)

        public Boolean TryGetLocation(Party_Idv3                         PartyId,
                                      Location_Id                        LocationId,
                                      [NotNullWhen(true)] out Location?  Location)
        {

            if (parties.        TryGetValue(PartyId,    out var party) &&
                party.Locations.TryGetValue(LocationId, out var location))
            {
                Location = location;
                return true;
            }

            Location = null;
            return false;

        }

        #endregion

        #region GetLocations           (IncludeLocation)

        public IEnumerable<Location> GetLocations(Func<Location, Boolean> IncludeLocation)
        {

            var locations = new List<Location>();

            foreach (var party in parties.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (IncludeLocation(location))
                        locations.Add(location);
                }
            }

            return locations;

        }

        #endregion

        #region GetLocations           (PartyId = null)

        public IEnumerable<Location> GetLocations(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Locations.Values;
            }

            else
            {

                var locations = new List<Location>();

                foreach (var party in parties.Values)
                    locations.AddRange(party.Locations.Values);

                return locations;

            }

            return [];

        }

        #endregion

        #endregion

        #region EVSEs

        #region Events

        public delegate Task OnEVSEAddedDelegate  (EVSE EVSE);
        public delegate Task OnEVSEChangedDelegate(EVSE EVSE);
        public delegate Task OnEVSERemovedDelegate(EVSE EVSE);

        public event OnEVSEAddedDelegate?    OnEVSEAdded;
        public event OnEVSEChangedDelegate?  OnEVSEChanged;
        public event OnEVSERemovedDelegate?  OnEVSERemoved;


        public delegate Task OnEVSEStatusChangedDelegate(DateTimeOffset  Timestamp, EVSE EVSE, StatusType NewEVSEStatus, StatusType? OldEVSEStatus = null);

        public event OnEVSEStatusChangedDelegate? OnEVSEStatusChanged;

        #endregion


        #region AddOrUpdateEVSE       (Location, newOrUpdatedEVSE, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<EVSE>>

            AddOrUpdateEVSE(Location           Location,
                            EVSE               newOrUpdatedEVSE,
                            Boolean?           AllowDowngrades   = false,
                            EventTracking_Id?  EventTrackingId   = null,
                            User_Id?           CurrentUserId     = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            Location.TryGetEVSE(newOrUpdatedEVSE.UId, out var existingEVSE);

            if (existingEVSE is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedEVSE.LastUpdated < existingEVSE.LastUpdated)
                {
                    return AddOrUpdateResult<EVSE>.Failed(
                               EventTrackingId,
                               newOrUpdatedEVSE,
                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!"
                           );
                }

                if (newOrUpdatedEVSE.LastUpdated.ToISO8601() == existingEVSE.LastUpdated.ToISO8601())
                    return AddOrUpdateResult<EVSE>.NoOperation(
                               EventTrackingId,
                               newOrUpdatedEVSE,
                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!"
                           );

            }


            Location.SetEVSE(newOrUpdatedEVSE);

            // Update location timestamp!
            var builder = Location.ToBuilder();
            builder.LastUpdated = newOrUpdatedEVSE.LastUpdated;
            await AddOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);

            var OnLocationChangedLocal = OnLocationChanged;
            if (OnLocationChangedLocal is not null)
            {
                try
                {
                    OnLocationChangedLocal(newOrUpdatedEVSE.ParentLocation).Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnLocationChanged), ": ",
                                Environment.NewLine, e.Message,
                                e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                }
            }


            if (existingEVSE is not null)
            {

                if (existingEVSE.Status != StatusType.REMOVED)
                {

                    var OnEVSEChangedLocal = OnEVSEChanged;
                    if (OnEVSEChangedLocal is not null)
                    {
                        try
                        {
                            OnEVSEChangedLocal(newOrUpdatedEVSE).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                        }
                    }


                    if (existingEVSE.Status != newOrUpdatedEVSE.Status)
                    {
                        var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        if (OnEVSEStatusChangedLocal is not null)
                        {
                            try
                            {

                                OnEVSEStatusChangedLocal(Timestamp.Now,
                                                         newOrUpdatedEVSE,
                                                         existingEVSE.Status,
                                                         newOrUpdatedEVSE.Status).Wait();

                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            e.StackTrace is not null
                                                ? Environment.NewLine + e.StackTrace
                                                : String.Empty);
                            }
                        }
                    }

                }
                else
                {

                    if (!KeepRemovedEVSEs(newOrUpdatedEVSE))
                        Location.RemoveEVSE(newOrUpdatedEVSE);

                    var OnEVSERemovedLocal = OnEVSERemoved;
                    if (OnEVSERemovedLocal is not null)
                    {
                        try
                        {
                            OnEVSERemovedLocal(newOrUpdatedEVSE).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSERemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                        }
                    }

                }
            }
            else
            {
                var OnEVSEAddedLocal = OnEVSEAdded;
                if (OnEVSEAddedLocal is not null)
                {
                    try
                    {
                        OnEVSEAddedLocal(newOrUpdatedEVSE).Wait();
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEAdded), ": ",
                                    Environment.NewLine, e.Message,
                                    e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                    }
                }
            }

            return existingEVSE is null
                       ? AddOrUpdateResult<EVSE>.Created(EventTrackingId, newOrUpdatedEVSE)
                       : AddOrUpdateResult<EVSE>.Updated(EventTrackingId, newOrUpdatedEVSE);

        }

        #endregion

        #region TryPatchEVSE          (Location, EVSE, EVSEPatch,  AllowDowngrades = false)

        public async Task<PatchResult<EVSE>>

            TryPatchEVSE(Location           Location,
                         EVSE               EVSE,
                         JObject            EVSEPatch,
                         Boolean?           AllowDowngrades   = false,
                         EventTracking_Id?  EventTrackingId   = null,
                         User_Id?           CurrentUserId     = null)

        {

            var patchResult        = EVSE.TryPatch(EVSEPatch,
                                                   AllowDowngrades ?? this.AllowDowngrades ?? false);

            var justAStatusChange  = EVSEPatch.Children().Count() == 2 && EVSEPatch.ContainsKey("status") && EVSEPatch.ContainsKey("last_updated");

            if (patchResult.IsSuccessAndDataNotNull(out var data))
            {

                if (data.Status != StatusType.REMOVED || KeepRemovedEVSEs(EVSE))
                    Location.SetEVSE   (data);
                else
                    Location.RemoveEVSE(data);

                // Update location timestamp!
                var builder = Location.ToBuilder();
                builder.LastUpdated = data.LastUpdated;
                await AddOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);


                if (EVSE.Status != StatusType.REMOVED)
                {

                    if (justAStatusChange)
                    {

                        var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        if (OnEVSEStatusChangedLocal is not null)
                        {
                            try
                            {

                                OnEVSEStatusChangedLocal(
                                    data.LastUpdated,
                                    EVSE,
                                    EVSE.Status,
                                    data.Status
                                ).Wait();

                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            e.StackTrace is not null
                                                ? Environment.NewLine + e.StackTrace
                                                : String.Empty);
                            }
                        }

                    }
                    else
                    {

                        var OnEVSEChangedLocal = OnEVSEChanged;
                        if (OnEVSEChangedLocal is not null)
                        {
                            try
                            {
                                OnEVSEChangedLocal(data).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchEVSE), " ", nameof(OnEVSEChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            e.StackTrace is not null
                                                ? Environment.NewLine + e.StackTrace
                                                : String.Empty);
                            }
                        }

                    }

                }
                else
                {

                    var OnEVSERemovedLocal = OnEVSERemoved;
                    if (OnEVSERemovedLocal is not null)
                    {
                        try
                        {
                            OnEVSERemovedLocal(data).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchEVSE), " ", nameof(OnEVSERemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                        }
                    }

                }

            }

            return patchResult;

        }

        #endregion


        #region AddOrUpdateEVSEs       (Location, EVSEs,   AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<IEnumerable<EVSE>>>

            AddOrUpdateEVSEs(Location           Location,
                             IEnumerable<EVSE>  EVSEs,
                             Boolean?           AllowDowngrades     = false,
                             Boolean            SkipNotifications   = false,
                             EventTracking_Id?  EventTrackingId     = null,
                             User_Id?           CurrentUserId       = null,
                             CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            foreach (var evse in EVSEs)
            {
                if (Location.TryGetEVSE(evse.UId, out var existingEVSE) &&
                    existingEVSE is not null)
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        evse.LastUpdated <= existingEVSE.LastUpdated)
                    {
                        return AddOrUpdateResult<IEnumerable<EVSE>>.Failed(
                                   EventTrackingId,
                                   EVSEs,
                                   "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!"
                               );
                    }

                    //if (EVSE.LastUpdated.ToISO8601() == existingEVSE.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<EVSE>.NoOperation(EVSE,
                    //                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");

                }
            }

            #endregion

            var newLocation = Location.Update(locationBuilder => {

                                                  foreach (var evse in EVSEs)
                                                  {

                                                      if (evse.Status != StatusType.REMOVED || KeepRemovedEVSEs(evse))
                                                          locationBuilder.SetEVSE(evse);
                                                      else
                                                          locationBuilder.RemoveEVSE(evse);

                                                      if (evse.LastUpdated > locationBuilder.LastUpdated)
                                                          locationBuilder.LastUpdated  = evse.LastUpdated;

                                                  }

                                              },
                                              out var warnings);

            if (newLocation is null)
                return AddOrUpdateResult<IEnumerable<EVSE>>.Failed(
                           EventTrackingId,
                           EVSEs,
                           warnings.First().Text.FirstText()
                       );


            var updateLocationResult = await UpdateLocation(
                                                 newLocation,
                                                 AllowDowngrades ?? this.AllowDowngrades,
                                                 SkipNotifications,
                                                 EventTrackingId,
                                                 CurrentUserId,
                                                 CancellationToken
                                             );


            //ToDo: Check if all EVSEs have been added OR updated!
            return updateLocationResult.IsSuccess
                       ? //existingEVSE is null
                         //    ? AddOrUpdateResult<IEnumerable<EVSE>>.Created(EventTrackingId, EVSEs)
                              AddOrUpdateResult<IEnumerable<EVSE>>.Updated(EventTrackingId, EVSEs)
                       : AddOrUpdateResult<IEnumerable<EVSE>>.Failed(
                             EventTrackingId,
                             EVSEs,
                             updateLocationResult.ErrorResponse ?? "Unknown error!"
                         );

        }

        #endregion


        public Boolean TryGetEVSE(EVSE_UId                       EVSE_UId,
                                  [NotNullWhen(true)] out EVSE?  EVSE)
        {

            EVSE = null;

            foreach (var locationKVP in parties.SelectMany(party => party.Value.Locations))
            {
                if (locationKVP.Value.TryGetEVSE(EVSE_UId, out EVSE))
                    return true;
            }

            return false;

        }

        #endregion

        #region Connectors

        #region AddOrUpdateConnector  (Location, EVSE, newOrUpdatedConnector,     AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Connector>>

            AddOrUpdateConnector(Location           Location,
                                 EVSE               EVSE,
                                 Connector          newOrUpdatedConnector,
                                 Boolean?           AllowDowngrades   = false,
                                 EventTracking_Id?  EventTrackingId   = null,
                                 User_Id?           CurrentUserId     = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var ConnectorExistedBefore = EVSE.TryGetConnector(newOrUpdatedConnector.Id, out var existingConnector);

            if (existingConnector is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedConnector.LastUpdated < existingConnector.LastUpdated)
                {
                    return AddOrUpdateResult<Connector>.Failed     (EventTrackingId, newOrUpdatedConnector,
                                                                    "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");
                }

                if (newOrUpdatedConnector.LastUpdated.ToISO8601() == existingConnector.LastUpdated.ToISO8601())
                    return AddOrUpdateResult<Connector>.NoOperation(EventTrackingId, newOrUpdatedConnector,
                                                                    "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");

            }

            EVSE.UpdateConnector(newOrUpdatedConnector);

            // Update EVSE/location timestamps!
            var evseBuilder     = EVSE.ToBuilder();
            evseBuilder.LastUpdated = newOrUpdatedConnector.LastUpdated;
            await AddOrUpdateEVSE(Location, evseBuilder,     (AllowDowngrades ?? this.AllowDowngrades) == false);


            var OnLocationChangedLocal = OnLocationChanged;
            if (OnLocationChangedLocal is not null)
            {
                try
                {
                    OnLocationChangedLocal(newOrUpdatedConnector.ParentEVSE.ParentLocation).Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateConnector), " ", nameof(OnLocationChanged), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }


            return ConnectorExistedBefore
                        ? AddOrUpdateResult<Connector>.Updated(EventTrackingId, newOrUpdatedConnector)
                        : AddOrUpdateResult<Connector>.Created(EventTrackingId, newOrUpdatedConnector);

        }

        #endregion

        #region TryPatchConnector     (Location, EVSE, Connector, ConnectorPatch, AllowDowngrades = false)

        public async Task<PatchResult<Connector>>

            TryPatchConnector(Location           Location,
                              EVSE               EVSE,
                              Connector          Connector,
                              JObject            ConnectorPatch,
                              Boolean?           AllowDowngrades   = false,
                              EventTracking_Id?  EventTrackingId   = null,
                              User_Id?           CurrentUserId     = null)

        {

            var patchResult = Connector.TryPatch(ConnectorPatch,
                                                 AllowDowngrades ?? this.AllowDowngrades ?? false);

            if (patchResult.IsSuccessAndDataNotNull(out var data))
            {

                EVSE.UpdateConnector(data);

                // Update EVSE/location timestamps!
                var evseBuilder = EVSE.ToBuilder();
                evseBuilder.LastUpdated = data.LastUpdated;
                await AddOrUpdateEVSE(Location, evseBuilder, (AllowDowngrades ?? this.AllowDowngrades) == false);

            }

            return patchResult;

        }

        #endregion

        #endregion

        #endregion

        #region Tariffs

        #region Events

        public delegate Task OnTariffAddedDelegate  (Tariff               Tariff);
        public delegate Task OnTariffChangedDelegate(Tariff               Tariff);
        public delegate Task OnTariffRemovedDelegate(IEnumerable<Tariff>  Tariff);

        public event OnTariffAddedDelegate?    OnTariffAdded;
        public event OnTariffChangedDelegate?  OnTariffChanged;
        public event OnTariffRemovedDelegate?  OnTariffRemoved;

        #endregion


        public GetTariffs2_Delegate?    GetTariffsDelegate      { get; set; }

        public GetTariffIds2_Delegate?  GetTariffIdsDelegate    { get; set; }


        public delegate Task<Tariff> OnTariffSlowStorageLookupDelegate(Party_Idv3       PartyId,
                                                                       Tariff_Id        TariffId,
                                                                       DateTimeOffset?  Timestamp,
                                                                       TimeSpan?        Tolerance);

        public event OnTariffSlowStorageLookupDelegate? OnTariffSlowStorageLookup;


        #region AddTariff            (Tariff, ...)

        /// <summary>
        /// Add the given charging tariff.
        /// </summary>
        /// <param name="Tariff">The charging tariff to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Tariff>>

            AddTariff(Tariff             Tariff,
                      Boolean            SkipNotifications   = false,
                      EventTracking_Id?  EventTrackingId     = null,
                      User_Id?           CurrentUserId       = null,
                      CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Tariff.CountryCode,
                                    Tariff.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Tariffs.TryAdd(Tariff.Id, Tariff))
                {

                    Tariff.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addTariff,
                              Tariff.ToJSON(
                                  true,
                                  true,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceLimitSerializer,
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
                                  OnTariffAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Tariff
                                  )
                              );

                    }

                    return AddResult<Tariff>.Success(
                               EventTrackingId,
                               Tariff
                           );

                }

                return AddResult<Tariff>.Failed(
                           EventTrackingId,
                           Tariff,
                           "TryAdd(Tariff.Id, Tariff) failed!"
                       );

            }

            return AddResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       $"The party identification '{partyId}' of the tariff is unknown!"
                   );

        }

        #endregion

        #region AddTariffIfNotExists (Tariff, ...)

        /// <summary>
        /// Add the given charging tariff if it does not already exist.
        /// </summary>
        /// <param name="Tariff">The charging tariff to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Tariff>>

            AddTariffIfNotExists(Tariff             Tariff,
                                 Boolean            SkipNotifications   = false,
                                 EventTracking_Id?  EventTrackingId     = null,
                                 User_Id?           CurrentUserId       = null,
                                 CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Tariff.CountryCode,
                                    Tariff.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Tariffs.TryAdd(Tariff.Id, Tariff))
                {

                    Tariff.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addTariffIfNotExists,
                              Tariff.ToJSON(
                                  true,
                                  true,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceLimitSerializer,
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
                                  OnTariffAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Tariff
                                  )
                              );

                    }

                    return AddResult<Tariff>.Success(
                               EventTrackingId,
                               Tariff
                           );

                }

                return AddResult<Tariff>.NoOperation(
                           EventTrackingId,
                           Tariff,
                           "The given tariff already exists."
                       );

            }

            return AddResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       $"The party identification '{partyId}' of the tariff is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateTariff    (Tariff,                         AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given charging tariff.
        /// </summary>
        /// <param name="Tariff">The charging tariff to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<Tariff>>

            AddOrUpdateTariff(Tariff             Tariff,
                              Boolean?           AllowDowngrades     = false,
                              Boolean            SkipNotifications   = false,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null,
                              CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Tariff.CountryCode,
                                    Tariff.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                #region Update an existing tariff

                if (party.Tariffs.TryGetValue(Tariff.Id,
                                              out var existingTariff,
                                              Tariff.NotBefore ?? DateTimeOffset.MinValue))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        Tariff.LastUpdated <= existingTariff.LastUpdated)
                    {
                        return AddOrUpdateResult<Tariff>.Failed(
                                   EventTrackingId,
                                   Tariff,
                                   "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!"
                               );
                    }

                    if (party.Tariffs.TryUpdate(Tariff.Id,
                                                Tariff,
                                                existingTariff))
                    {

                        Tariff.CommonAPI = this;

                        await LogAsset(
                                  CommonHTTPAPI.addOrUpdateTariff,
                                  Tariff.ToJSON(
                                      true,
                                      true,
                                      CustomTariffSerializer,
                                      CustomDisplayTextSerializer,
                                      CustomPriceLimitSerializer,
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

                            var OnTariffChangedLocal = OnTariffChanged;
                            if (OnTariffChangedLocal is not null)
                            {
                                try
                                {
                                    await OnTariffChangedLocal(Tariff);
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                        }

                        return AddOrUpdateResult<Tariff>.Updated(
                                   EventTrackingId,
                                   Tariff
                               );

                    }

                    return AddOrUpdateResult<Tariff>.Failed(
                               EventTrackingId,
                               Tariff,
                               "Updating the given tariff failed!"
                           );

                }

                #endregion

                #region Add a new tariff

                if (party.Tariffs.TryAdd(Tariff.Id, Tariff))
                {

                    Tariff.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateTariff,
                              Tariff.ToJSON(
                                  true,
                                  true,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceLimitSerializer,
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

                        var OnTariffAddedLocal = OnTariffAdded;
                        if (OnTariffAddedLocal is not null)
                        {
                            try
                            {
                                await OnTariffAddedLocal(Tariff);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return AddOrUpdateResult<Tariff>.Created(
                               EventTrackingId,
                               Tariff
                           );

                }

                #endregion

                return AddOrUpdateResult<Tariff>.Failed(
                           EventTrackingId,
                           Tariff,
                           "Adding the given tariff failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       $"The party identification '{partyId}' of the tariff is unknown!"
                   );

        }

        #endregion

        #region UpdateTariff         (Tariff,                         AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given charging tariff.
        /// </summary>
        /// <param name="Tariff">The charging tariff to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<Tariff>>

            UpdateTariff(Tariff             Tariff,
                         Boolean?           AllowDowngrades     = false,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null,
                         CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Tariff.CountryCode,
                                    Tariff.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (!party.Tariffs.TryGetValue(Tariff.Id, out var existingTariff))
                    return UpdateResult<Tariff>.Failed(
                               EventTrackingId,
                               Tariff,
                               $"The given tariff identification '{Tariff.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Tariff.LastUpdated <= existingTariff.LastUpdated)
                {

                    return UpdateResult<Tariff>.Failed(
                               EventTrackingId,
                               Tariff,
                               "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!"
                           );

                }

                #endregion


                if (party.Tariffs.TryUpdate(Tariff.Id,
                                            Tariff,
                                            existingTariff))
                {

                    Tariff.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.updateTariff,
                              Tariff.ToJSON(
                                  true,
                                  true,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceLimitSerializer,
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
                                  OnTariffChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Tariff
                                  )
                              );

                    }

                    return UpdateResult<Tariff>.Success(
                               EventTrackingId,
                               Tariff
                           );

                }

                return UpdateResult<Tariff>.Failed(
                           EventTrackingId,
                           Tariff,
                           "Tariffs.TryUpdate(Tariff.Id, Tariff, Tariff) failed!"
                       );

            }

            return UpdateResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       $"The party identification '{partyId}' of the tariff is unknown!"
                   );

        }

        #endregion

        #region TryPatchTariff       (PartyId, TariffId, TariffPatch, AllowDowngrades = false, ...)

        /// <summary>
        /// Try to patch the given charging tariff with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the token.</param>
        /// <param name="TariffId">The identification of the charging tariff to patch.</param>
        /// <param name="TariffPatch">The JSON patch document to apply to the charging tariff.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<Tariff>>

            TryPatchTariff(Party_Idv3         PartyId,
                           Tariff_Id          TariffId,
                           JObject            TariffPatch,
                           Boolean?           AllowDowngrades     = false,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null,
                           CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Tariffs.TryGetValue(TariffId, out var existingTariff, Timestamp.Now))
                {

                    var patchResult = existingTariff.TryPatch(
                                          TariffPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedTariff))
                    {

                        var updateTariffResult = await UpdateTariff(
                                                           patchedTariff,
                                                           AllowDowngrades,
                                                           SkipNotifications,
                                                           EventTrackingId,
                                                           CurrentUserId,
                                                           CancellationToken
                                                       );

                        if (updateTariffResult.IsFailed)
                            return PatchResult<Tariff>.Failed(
                                       EventTrackingId,
                                       existingTariff,
                                       "Could not patch the tariff: " + updateTariffResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<Tariff>.Failed(
                           EventTrackingId,
                           $"The given tariff '{TariffId}' is unknown!"
                       );

            }

            return PatchResult<Tariff>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the tariff is unknown!"
                   );

        }

        #endregion


        #region RemoveTariff         (Tariff, ...)

        /// <summary>
        /// Remove the given charging tariff.
        /// </summary>
        /// <param name="Tariff">A charging tariff.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveTariff(Tariff             Tariff,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null,
                         CancellationToken  CancellationToken   = default)

                => RemoveTariff(
                       Party_Idv3.From(
                           Tariff.CountryCode,
                           Tariff.PartyId
                       ),
                       Tariff.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveTariff         (PartyId, TariffId, ...)

        /// <summary>
        /// Remove the given charging tariff.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the charging tariff.</param>
        /// <param name="TariffId">An unique charging tariff identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveTariff(Party_Idv3         PartyId,
                         Tariff_Id          TariffId,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null,
                         CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Tariffs.TryRemove(TariffId, out var tariffVersions))
                {

                    await LogAsset(
                              CommonHTTPAPI.removeTariff,
                              new JArray(
                                  tariffVersions.Select(
                                      tariff => tariff.ToJSON(
                                                    true,
                                                    true,
                                                    CustomTariffSerializer,
                                                    CustomDisplayTextSerializer,
                                                    CustomPriceLimitSerializer,
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
                                  OnTariffRemoved,
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
                           $"The tariff '{PartyId}/{TariffId}' is unknown!"
                       );

            }

            return RemoveResult<IEnumerable<Tariff>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the token is unknown!"
                   );

        }

        #endregion

        #region RemoveAllTariffs     (...)

        /// <summary>
        /// Remove all charging tariffs.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveAllTariffs(Boolean            SkipNotifications   = false,
                             EventTracking_Id?  EventTrackingId     = null,
                             User_Id?           CurrentUserId       = null,
                             CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var tariffVersionList = new List<IEnumerable<Tariff>>();

            foreach (var party in parties.Values)
            {
                tariffVersionList.Add(party.Tariffs.Values());
                party.Tariffs.Clear();
            }

            await LogAsset(
                      CommonHTTPAPI.removeAllTariffs,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var tariffVersion in tariffVersionList)
                    await LogEvent(
                              OnTariffRemoved,
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

        #region RemoveAllTariffs     (IncludeTariffs,   ...)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeTariffs">A filter delegate to include charging tariffs for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveAllTariffs(Func<Tariff, Boolean>  IncludeTariffs,
                             Boolean                SkipNotifications   = false,
                             EventTracking_Id?      EventTrackingId     = null,
                             User_Id?               CurrentUserId       = null,
                             CancellationToken      CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingTariffs  = new List<Tariff>();
            var removedTariffs   = new List<Tariff>();
            var failedTariffs    = new List<RemoveResult<Tariff>>();

            foreach (var party in parties.Values)
            {
                foreach (var tariff in party.Tariffs.Values())
                {
                    if (IncludeTariffs(tariff))
                        matchingTariffs.Add(tariff);
                }
            }

            foreach (var tariff in matchingTariffs)
            {

                var result = await RemoveTariff(
                                       tariff,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedTariffs.Add(tariff);
                else
                    failedTariffs. Add(
                        RemoveResult<Tariff>.Failed(
                            EventTrackingId,
                            tariff,
                            result.ErrorResponse ?? "Unknown error while removing the charging tariff!"
                        )
                    );

            }

            return removedTariffs.Count != 0 && failedTariffs.Count == 0

                       ? RemoveResult<IEnumerable<Tariff>>.Success(
                             EventTrackingId,
                             removedTariffs
                         )

                       : removedTariffs.Count == 0 && failedTariffs.Count == 0

                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Tariff>>.Failed(
                                   EventTrackingId,
                                   failedTariffs.
                                       Select(removeResult => removeResult.Data).
                                       Where (tariff       => tariff is not null).
                                       Cast<Tariff>(),
                                   failedTariffs.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllTariffs     (IncludeTariffIds, ...)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeTariffIds">A filter delegate to include charging tariffs for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveAllTariffs(Func<Party_Idv3, Tariff_Id, Boolean>  IncludeTariffIds,
                             Boolean                               SkipNotifications   = false,
                             EventTracking_Id?                     EventTrackingId     = null,
                             User_Id?                              CurrentUserId       = null,
                             CancellationToken                     CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingTariffs  = new List<Tariff>();
            var removedTariffs   = new List<Tariff>();
            var failedTariffs    = new List<RemoveResult<Tariff>>();

            foreach (var party in parties.Values)
            {
                foreach (var tariff in party.Tariffs.Values())
                {
                    if (IncludeTariffIds(party.Id, tariff.Id))
                        matchingTariffs.Add(tariff);
                }
            }

            foreach (var tariff in matchingTariffs)
            {

                var result = await RemoveTariff(
                                       tariff,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedTariffs.Add(tariff);
                else
                    failedTariffs. Add(
                        RemoveResult<Tariff>.Failed(
                            EventTrackingId,
                            tariff,
                            result.ErrorResponse ?? "Unknown error while removing the charging tariff!"
                        )
                    );

            }

            return removedTariffs.Count != 0 && failedTariffs.Count == 0

                       ? RemoveResult<IEnumerable<Tariff>>.Success(
                             EventTrackingId,
                             removedTariffs
                         )

                       : removedTariffs.Count == 0 && failedTariffs.Count == 0

                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Tariff>>.Failed(
                                   EventTrackingId,
                                   failedTariffs.
                                       Select(removeResult => removeResult.Data).
                                       Where (tariff       => tariff is not null).
                                       Cast<Tariff>(),
                                   failedTariffs.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllTariffs     (PartyId, ...)

        /// <summary>
        /// Remove all charging tariffs owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveAllTariffs(Party_Idv3         PartyId,
                             Boolean            SkipNotifications   = false,
                             EventTracking_Id?  EventTrackingId     = null,
                             User_Id?           CurrentUserId       = null,
                             CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                var matchingTariffs  = party.Tariffs.Values();
                var removedTariffs   = new List<Tariff>();
                var failedTariffs    = new List<RemoveResult<Tariff>>();

                foreach (var tariff in matchingTariffs)
                {

                    var result = await RemoveTariff(
                                           tariff,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedTariffs.Add(tariff);
                    else
                        failedTariffs. Add(
                            RemoveResult<Tariff>.Failed(
                                EventTrackingId,
                                tariff,
                                result.ErrorResponse ?? "Unknown error while removing the charging tariff!"
                            )
                        );

                }

                return removedTariffs.Count != 0 && failedTariffs.Count == 0

                           ? RemoveResult<IEnumerable<Tariff>>.Success(
                                 EventTrackingId,
                                 removedTariffs
                             )

                           : removedTariffs.Count == 0 && failedTariffs.Count == 0

                                 ? RemoveResult<IEnumerable<Tariff>>.NoOperation(
                                       EventTrackingId,
                                       []
                                   )

                                 : RemoveResult<IEnumerable<Tariff>>.Failed(
                                       EventTrackingId,
                                       failedTariffs.
                                           Select(removeResult => removeResult.Data).
                                           Where (tariff      => tariff is not null).
                                           Cast<Tariff>(),
                                       failedTariffs.Select(removeResult => removeResult.ErrorResponse).
                                           AggregateWith(", ")
                                   );

            }

            return RemoveResult<IEnumerable<Tariff>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' is unknown!"
                   );

        }

        #endregion


        #region TariffExist          (PartyId, TariffId,             Timestamp = null, Tolerance = null)

        public Boolean TariffExists(Party_Idv3       PartyId,
                                    Tariff_Id        TariffId,
                                    DateTimeOffset?  Timestamp   = null,
                                    TimeSpan?        Tolerance   = null)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.Tariffs.ContainsKey(TariffId);

            var onTariffSlowStorageLookup = OnTariffSlowStorageLookup;
            if (onTariffSlowStorageLookup is not null)
            {
                try
                {

                    return onTariffSlowStorageLookup(
                               PartyId,
                               TariffId,
                               Timestamp,
                               Tolerance
                           ).Result is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TariffExists), " ", nameof(OnTariffSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            return false;

        }

        #endregion

        #region GetTariff            (PartyId, TariffId,             Timestamp = null, Tolerance = null)

        public Tariff? GetTariff(Party_Idv3       PartyId,
                                 Tariff_Id        TariffId,
                                 DateTimeOffset?  Timestamp   = null,
                                 TimeSpan?        Tolerance   = null)
        {

            if (TryGetTariff(PartyId,
                             TariffId,
                             out var tariff,
                             Timestamp,
                             Tolerance))
            {
                return tariff;
            }

            return null;

        }

        #endregion

        #region TryGetTariff         (PartyId, TariffId, out Tariff, Timestamp = null, Tolerance = null)

        public Boolean TryGetTariff(Party_Idv3                       PartyId,
                                    Tariff_Id                        TariffId,
                                    [NotNullWhen(true)] out Tariff?  Tariff,
                                    DateTimeOffset?                  Timestamp   = null,
                                    TimeSpan?                        Tolerance   = null)
        {

            if (parties.      TryGetValue(PartyId,  out var party) &&
                party.Tariffs.TryGetValue(TariffId, out var tariff))
            {
                Tariff = tariff;
                return true;
            }

            var onTariffLookup = OnTariffSlowStorageLookup;
            if (onTariffLookup is not null)
            {
                try
                {

                    Tariff = onTariffLookup(
                                 PartyId,
                                 TariffId,
                                 Timestamp,
                                 Tolerance
                             ).Result;

                    return Tariff is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetTariff), " ", nameof(OnTariffSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            Tariff = null;
            return false;

        }

        #endregion

        #region GetTariffs           (IncludeTariff)

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, Boolean>  IncludeTariff)
        {

            var tariffs = new List<Tariff>();

            foreach (var party in parties.Values)
            {
                foreach (var tariff in party.Tariffs.Values())
                {
                    if (IncludeTariff(tariff))
                        tariffs.Add(tariff);
                }
            }

            return tariffs;

        }

        #endregion

        #region GetTariffs           (PartyId = null)

        public IEnumerable<Tariff> GetTariffs(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Tariffs.Values();
            }

            else
            {

                var tariffs = new List<Tariff>();

                foreach (var party in parties.Values)
                    tariffs.AddRange(party.Tariffs.Values());

                return tariffs;

            }

            return [];

        }

        #endregion


        #region GetTariffIds         (PartyId?, LocationId?, EVSEId?, ConnectorId?, EMSPId?)

        public IEnumerable<Tariff_Id> GetTariffIds(Party_Idv3     PartyId,
                                                   Location_Id?   LocationId,
                                                   EVSE_Id?       EVSEId,
                                                   Connector_Id?  ConnectorId,
                                                   EMSP_Id?       EMSPId)

            => GetTariffIdsDelegate?.Invoke(
                   PartyId,
                   LocationId,
                   EVSEId,
                   ConnectorId,
                   EMSPId
               ) ?? [];

        #endregion

        #endregion

        #region Tokens

        #region Events

        public delegate Task               OnTokenStatusAddedDelegate  (TokenStatus  TokenStatus);
        public delegate Task               OnTokenStatusChangedDelegate(TokenStatus  TokenStatus);
        public delegate Task               OnTokenStatusRemovedDelegate(TokenStatus  TokenStatus);

        public event OnTokenStatusAddedDelegate?    OnTokenStatusAdded;
        public event OnTokenStatusChangedDelegate?  OnTokenStatusChanged;
        public event OnTokenStatusRemovedDelegate?  OnTokenStatusRemoved;

        #endregion


        public delegate Task<TokenStatus> OnTokenSlowStorageLookupDelegate(Party_Idv3  PartyId,
                                                                           Token_Id    TokenId);

        public event OnTokenSlowStorageLookupDelegate? OnTokenSlowStorageLookup;


        #region AddToken            (Token, Status = AllowedType.ALLOWED, ...)

        /// <summary>
        /// Add the given token.
        /// </summary>
        /// <param name="Token">The token to add.</param>
        /// <param name="Status">The optional status of the token.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<TokenStatus>>

            AddToken(Token              Token,
                     AllowedType?       Status              = null,
                     Boolean            SkipNotifications   = false,
                     EventTracking_Id?  EventTrackingId     = null,
                     User_Id?           CurrentUserId       = null,
                     CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;
            Status          ??= AllowedType.ALLOWED;

            var tokenStatus   = new TokenStatus(
                                    Token,
                                    Status.Value
                                );

            var partyId       = Party_Idv3.From(
                                    Token.CountryCode,
                                    Token.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Tokens.TryAdd(Token.Id, tokenStatus))
                {

                    Token.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addTokenStatus,
                              tokenStatus.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomTokenStatusSerializer,
                                  CustomTokenSerializer,
                                  CustomEnergyContractSerializer,
                                  CustomLocationReferenceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnTokenStatusAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      tokenStatus
                                  )
                              );

                    }

                    return AddResult<TokenStatus>.Success(
                               EventTrackingId,
                               tokenStatus
                           );

                }

                return AddResult<TokenStatus>.Failed(
                           EventTrackingId,
                           tokenStatus,
                           "The given token status already exists!"
                       );

            }

            return AddResult<TokenStatus>.Failed(
                       EventTrackingId,
                       tokenStatus,
                       $"The party identification '{partyId}' of the token status is unknown!"
                   );

        }

        #endregion

        #region AddTokenIfNotExists (Token, Status = AllowedType.ALLOWED, ...)

        /// <summary>
        /// Add the given token if it does not already exist.
        /// </summary>
        /// <param name="Token">The token to add.</param>
        /// <param name="Status">The optional status of the token.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<TokenStatus>>

            AddTokenIfNotExists(Token              Token,
                                AllowedType?       Status              = null,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;
            Status          ??= AllowedType.ALLOWED;

            var tokenStatus   = new TokenStatus(
                                    Token,
                                    Status.Value
                                );

            var partyId       = Party_Idv3.From(
                                    Token.CountryCode,
                                    Token.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Tokens.TryAdd(Token.Id, tokenStatus))
                {

                    Token.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addTokenStatusIfNotExists,
                              tokenStatus.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomTokenStatusSerializer,
                                  CustomTokenSerializer,
                                  CustomEnergyContractSerializer,
                                  CustomLocationReferenceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnTokenStatusAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      tokenStatus
                                  )
                              );

                    }

                    return AddResult<TokenStatus>.Success(
                               EventTrackingId,
                               tokenStatus
                           );

                }

                return AddResult<TokenStatus>.NoOperation(
                           EventTrackingId,
                           tokenStatus,
                           "The given token status already exists."
                       );

            }

            return AddResult<TokenStatus>.Failed(
                       EventTrackingId,
                       tokenStatus,
                       $"The party identification '{partyId}' of the token status is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateToken    (Token, Status = AllowedType.ALLOWED, AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given token.
        /// </summary>
        /// <param name="Token">The token to add or update.</param>
        /// <param name="Status">The optional status of the token.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<TokenStatus>>

            AddOrUpdateToken(Token              Token,
                             AllowedType?       Status              = null,
                             Boolean?           AllowDowngrades     = false,
                             Boolean            SkipNotifications   = false,
                             EventTracking_Id?  EventTrackingId     = null,
                             User_Id?           CurrentUserId       = null,
                             CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            TokenStatus? tokenStatus = null;

            var partyId       = Party_Idv3.From(
                                    Token.CountryCode,
                                    Token.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                #region Update an existing token status

                if (party.Tokens.TryGetValue(Token.Id, out var existingTokenStatus))
                {

                    Status    ??= existingTokenStatus.Status;
                    tokenStatus = new TokenStatus(Token, Status.Value);

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        Token.LastUpdated <= existingTokenStatus.Token.LastUpdated)
                    {

                        return AddOrUpdateResult<TokenStatus>.Failed(
                                   EventTrackingId,
                                   tokenStatus,
                                   "The 'lastUpdated' timestamp of the new token status must be newer then the timestamp of the existing token status!"
                               );

                    }

                    //if (Token.LastUpdated.ToISO8601() == existingToken.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<Token>.NoOperation(Token,
                    //                                                   "The 'lastUpdated' timestamp of the new token status must be newer then the timestamp of the existing token status!");

                    var aa = existingTokenStatus.Equals(existingTokenStatus);

                    if (party.Tokens.TryUpdate(Token.Id,
                                               tokenStatus,
                                               existingTokenStatus))
                    {

                        Token.CommonAPI = this;

                        await LogAsset(
                                  CommonHTTPAPI.addOrUpdateTokenStatus,
                                  tokenStatus.ToJSON(
                                      //true,
                                      //true,
                                      //true,
                                      //true,
                                      CustomTokenStatusSerializer,
                                      CustomTokenSerializer,
                                      CustomEnergyContractSerializer,
                                      CustomLocationReferenceSerializer
                                  ),
                                  EventTrackingId,
                                  CurrentUserId,
                                  CancellationToken
                              );

                        if (!SkipNotifications)
                        {

                            await LogEvent(
                                      OnTokenStatusChanged,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          tokenStatus
                                      )
                                  );

                        }

                        return AddOrUpdateResult<TokenStatus>.Updated(
                                   EventTrackingId,
                                   tokenStatus
                               );

                    }

                    return AddOrUpdateResult<TokenStatus>.Failed(
                               EventTrackingId,
                               tokenStatus,
                               "Updating the given token status failed!"
                           );

                }

                #endregion

                #region Add a new token status

                Status      ??= AllowedType.ALLOWED;
                tokenStatus ??= new TokenStatus(Token, Status.Value);

                if (party.Tokens.TryAdd(Token.Id, tokenStatus))
                {

                    Token.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateTokenStatus,
                              tokenStatus.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomTokenStatusSerializer,
                                  CustomTokenSerializer,
                                  CustomEnergyContractSerializer,
                                  CustomLocationReferenceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnTokenStatusAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      tokenStatus
                                  )
                              );

                    }

                    return AddOrUpdateResult<TokenStatus>.Created(
                               EventTrackingId,
                               tokenStatus
                           );

                }

                #endregion

                return AddOrUpdateResult<TokenStatus>.Failed(
                           EventTrackingId,
                           tokenStatus,
                           "Adding the given token status failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<TokenStatus>.Failed(
                       EventTrackingId,
                       tokenStatus,
                       $"The party identification '{partyId}' of the token status is unknown!"
                   );

        }

        #endregion

        #region UpdateToken         (Token, Status = AllowedType.ALLOWED, AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given token.
        /// </summary>
        /// <param name="Token">The token to add or update.</param>
        /// <param name="Status">The optional status of the token.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<TokenStatus>>

            UpdateToken(Token              Token,
                        AllowedType?       Status              = null,
                        Boolean?           AllowDowngrades     = false,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null,
                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Token.CountryCode,
                                    Token.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (!party.Tokens.TryGetValue(Token.Id, out var existingTokenStatus))
                    return UpdateResult<TokenStatus>.Failed(
                               EventTrackingId,
                               $"The given token identification '{Token.Id}' is unknown!"
                           );

                Status ??= existingTokenStatus.Status;
                var tokenStatus = new TokenStatus(
                                      Token,
                                      Status.Value
                                  );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Token.LastUpdated <= existingTokenStatus.Token.LastUpdated)
                {

                    return UpdateResult<TokenStatus>.Failed(
                               EventTrackingId,
                               tokenStatus,
                               "The 'lastUpdated' timestamp of the new charging token status must be newer then the timestamp of the existing token status!"
                           );

                }

                #endregion

                if (party.Tokens.TryUpdate(Token.Id,
                                           tokenStatus,
                                           existingTokenStatus))
                {

                    Token.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.updateTokenStatus,
                              tokenStatus.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomTokenStatusSerializer,
                                  CustomTokenSerializer,
                                  CustomEnergyContractSerializer,
                                  CustomLocationReferenceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnTokenStatusChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      tokenStatus
                                  )
                              );

                    }

                    return UpdateResult<TokenStatus>.Success(
                               EventTrackingId,
                               tokenStatus
                           );

                }

                return UpdateResult<TokenStatus>.Failed(
                           EventTrackingId,
                           tokenStatus,
                           "Token.TryUpdate(TokenStatus.Id, TokenStatus, TokenStatus) failed!"
                       );

            }

            return UpdateResult<TokenStatus>.Failed(
                       EventTrackingId,
                       $"The party identification '{partyId}' of the token is unknown!"
                   );

        }

        #endregion

        #region TryPatchToken       (PartyId, TokenId, TokenPatch,        AllowDowngrades = false, ...)

        /// <summary>
        /// Try to patch the given token with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the token.</param>
        /// <param name="TokenId">The identification of the token to patch.</param>
        /// <param name="TokenPatch">The JSON patch document to apply to the token.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<Token>>

            TryPatchToken(Party_Idv3         PartyId,
                          Token_Id           TokenId,
                          JObject            TokenPatch,
                          Boolean?           AllowDowngrades     = false,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Tokens.TryGetValue(TokenId, out var existingTokenStatus))
                {

                    var patchResult = existingTokenStatus.Token.TryPatch(
                                          TokenPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedToken))
                    {

                        var updateTokenResult = await UpdateToken(
                                                          patchedToken,
                                                          existingTokenStatus.Status,
                                                          AllowDowngrades,
                                                          SkipNotifications,
                                                          EventTrackingId,
                                                          CurrentUserId,
                                                          CancellationToken
                                                      );

                        if (updateTokenResult.IsFailed)
                            return PatchResult<Token>.Failed(
                                       EventTrackingId,
                                       existingTokenStatus.Token,
                                       "Could not patch the token: " + updateTokenResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<Token>.Failed(
                           EventTrackingId,
                           $"The given token '{TokenId}' is unknown!"
                       );

            }

            return PatchResult<Token>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the token is unknown!"
                   );

        }

        #endregion


        #region RemoveToken         (Token, ...)

        /// <summary>
        /// Remove the given token.
        /// </summary>
        /// <param name="Token">A token.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<TokenStatus>>

            RemoveToken(Token              Token,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null,
                        CancellationToken  CancellationToken   = default)

                => RemoveToken(
                       Party_Idv3.From(
                           Token.CountryCode,
                           Token.PartyId
                       ),
                       Token.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveToken         (PartyId, TokenId, ...)

        /// <summary>
        /// Remove the given token.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the token.</param>
        /// <param name="TokenId">An unique identification of the token.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<TokenStatus>>

            RemoveToken(Party_Idv3         PartyId,
                        Token_Id           TokenId,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null,
                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Tokens.TryRemove(TokenId, out var existingTokenStatus))
                {

                    await LogAsset(
                              CommonHTTPAPI.removeToken,
                              existingTokenStatus.Token.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  CustomTokenSerializer,
                                  CustomEnergyContractSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        await LogEvent(
                                  OnTokenStatusRemoved,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      existingTokenStatus
                                  )
                              );

                    }

                    return RemoveResult<TokenStatus>.Success(
                               EventTrackingId,
                               existingTokenStatus
                           );

                }

                return RemoveResult<TokenStatus>.Failed(
                           EventTrackingId,
                           $"The token '{PartyId}/{TokenId}' is unknown!"
                       );

            }

            return RemoveResult<TokenStatus>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the token is unknown!"
                   );

        }

        #endregion

        #region RemoveAllTokens     (...)

        /// <summary>
        /// Remove all tokens.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<TokenStatus>>>

            RemoveAllTokens(Boolean            SkipNotifications   = false,
                            EventTracking_Id?  EventTrackingId     = null,
                            User_Id?           CurrentUserId       = null,
                            CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var tokenStatusList = new List<TokenStatus>();

            foreach (var party in parties.Values)
            {
                tokenStatusList.AddRange(party.Tokens.Values);
                party.Tokens.Clear();
            }

            await LogAsset(
                      CommonHTTPAPI.removeAllTokens,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var tokenStatus in tokenStatusList)
                    await LogEvent(
                              OnTokenStatusRemoved,
                              loggingDelegate => loggingDelegate.Invoke(
                                  tokenStatus
                              )
                          );

            }

            return RemoveResult<IEnumerable<TokenStatus>>.Success(
                       EventTrackingId,
                       tokenStatusList
                   );

        }

        #endregion

        #region RemoveAllTokens     (IncludeTokens,   ...)

        /// <summary>
        /// Remove all matching tokens.
        /// </summary>
        /// <param name="IncludeTokens">A filter delegate to include certain tokens for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<TokenStatus>>>

            RemoveAllTokens(Func<Token, Boolean>  IncludeTokens,
                            Boolean               SkipNotifications   = false,
                            EventTracking_Id?     EventTrackingId     = null,
                            User_Id?              CurrentUserId       = null,
                            CancellationToken     CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTokens  = new List<TokenStatus>();
            var failedTokens   = new List<RemoveResult<TokenStatus>>();

            foreach (var party in parties.Values)
            {
                foreach (var token_status in party.Tokens.Values.Where(tokenstatus => IncludeTokens(tokenstatus.Token)).ToArray())
                {

                    var result = await RemoveToken(
                                           token_status.Token,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedTokens.Add(token_status);
                    else
                        failedTokens. Add(result);

                }
            }

            return removedTokens.Count != 0 && failedTokens.Count == 0

                       ? RemoveResult<IEnumerable<TokenStatus>>.Success(
                             EventTrackingId,
                             removedTokens
                         )

                       : removedTokens.Count == 0 && failedTokens.Count == 0

                             ? RemoveResult<IEnumerable<TokenStatus>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<TokenStatus>>.Failed(
                                   EventTrackingId,
                                   failedTokens.
                                       Select(removeResult => removeResult.Data).
                                       Where (tokenStatus  => tokenStatus is not null).
                                       Cast<TokenStatus>(),
                                   failedTokens.Select(token => token.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllTokens     (IncludeTokenIds, ...)

        /// <summary>
        /// Remove all matching tokens.
        /// </summary>
        /// <param name="IncludeTokenIds">A filter delegate to include certain token identifications for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<TokenStatus>>>

            RemoveAllTokens(Func<Party_Idv3, Token_Id, Boolean>  IncludeTokenIds,
                            Boolean                              SkipNotifications   = false,
                            EventTracking_Id?                    EventTrackingId     = null,
                            User_Id?                             CurrentUserId       = null,
                            CancellationToken                    CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTokens  = new List<TokenStatus>();
            var failedTokens   = new List<RemoveResult<TokenStatus>>();

            foreach (var party in parties.Values)
            {
                foreach (var token_status in party.Tokens.Values.Where(tokenstatus => IncludeTokenIds(
                                                                                          Party_Idv3.From(
                                                                                              tokenstatus.Token.CountryCode,
                                                                                              tokenstatus.Token.PartyId
                                                                                          ),
                                                                                          tokenstatus.Token.Id
                                                                                      )).ToArray())
                {

                    var result = await RemoveToken(
                                           token_status.Token,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedTokens.Add(token_status);
                    else
                        failedTokens. Add(result);

                }
            }

            return removedTokens.Count != 0 && failedTokens.Count == 0

                       ? RemoveResult<IEnumerable<TokenStatus>>.Success(
                             EventTrackingId,
                             removedTokens
                         )

                       : removedTokens.Count == 0 && failedTokens.Count == 0

                             ? RemoveResult<IEnumerable<TokenStatus>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<TokenStatus>>.Failed(
                                   EventTrackingId,
                                   failedTokens.
                                       Select(removeResult => removeResult.Data).
                                       Where (tokenStatus  => tokenStatus is not null).
                                       Cast<TokenStatus>(),
                                   failedTokens.Select(token => token.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllTokens     (PartyId, ...)

        /// <summary>
        /// Remove all tokens owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<TokenStatus>>>

            RemoveAllTokens(Party_Idv3         PartyId,
                            Boolean            SkipNotifications   = false,
                            EventTracking_Id?  EventTrackingId     = null,
                            User_Id?           CurrentUserId       = null,
                            CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTokens  = new List<TokenStatus>();
            var failedTokens   = new List<RemoveResult<TokenStatus>>();

            foreach (var party in parties.Values)
            {
                foreach (var token_status in party.Tokens.Values.Where(tokenstatus => tokenstatus.Token.CountryCode == PartyId.CountryCode &&
                                                                                      tokenstatus.Token.PartyId     == PartyId.PartyId).ToArray())
                {

                    var result = await RemoveToken(
                                           token_status.Token,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedTokens.Add(token_status);
                    else
                        failedTokens. Add(result);

                }
            }

            return removedTokens.Count != 0 && failedTokens.Count == 0

                       ? RemoveResult<IEnumerable<TokenStatus>>.Success(
                             EventTrackingId,
                             removedTokens
                         )

                       : removedTokens.Count == 0 && failedTokens.Count == 0

                             ? RemoveResult<IEnumerable<TokenStatus>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<TokenStatus>>.Failed(
                                   EventTrackingId,
                                   failedTokens.
                                       Select(removeResult => removeResult.Data).
                                       Where (tokenStatus  => tokenStatus is not null).
                                       Cast<TokenStatus>(),
                                   failedTokens.Select(token => token.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion


        #region TokenExists         (PartyId, TokenId)

        public Boolean TokenExists(Party_Idv3  PartyId,
                                   Token_Id    TokenId)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.Tokens.ContainsKey(TokenId);

            var onTokenSlowStorageLookup = OnTokenSlowStorageLookup;
            if (onTokenSlowStorageLookup is not null)
            {
                try
                {

                    return onTokenSlowStorageLookup(
                               PartyId,
                               TokenId
                           ).Result is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TokenExists), " ", nameof(OnTokenSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            return false;

        }

        #endregion

        #region GetTokenStatus      (PartyId, TokenId)

        public TokenStatus? GetTokenStatus(Party_Idv3  PartyId,
                                           Token_Id    TokenId)
        {

            if (TryGetTokenStatus(PartyId,
                                  TokenId,
                                  out var tokenStatus))
            {
                return tokenStatus;
            }

            return null;

        }

        #endregion

        #region TryGetTokenStatus   (PartyId, TokenId, out TokenStatus)

        public Boolean TryGetTokenStatus(Party_Idv3                            PartyId,
                                         Token_Id                              TokenId,
                                         [NotNullWhen(true)] out TokenStatus?  TokenStatus)
        {

            if (parties.     TryGetValue(PartyId, out var party) &&
                party.Tokens.TryGetValue(TokenId, out TokenStatus))
            {
                return true;
            }

            var onTokenSlowStorageLookup = OnTokenSlowStorageLookup;
            if (onTokenSlowStorageLookup is not null)
            {

                try
                {

                    var tokenStatus = onTokenSlowStorageLookup(
                                          PartyId,
                                          TokenId
                                      ).Result;

                    if (tokenStatus is not null)
                    {
                        TokenStatus = tokenStatus;
                        return true;
                    }

                } catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetTokenStatus), " ", nameof(OnTokenSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }

            }

            TokenStatus = null;
            return false;

        }

        #endregion

        #region GetTokenStatus      (IncludeTokenStatus)

        public IEnumerable<TokenStatus> GetTokenStatus(Func<TokenStatus, Boolean> IncludeTokenStatus)
        {

            var matchingTokenStatus = new List<TokenStatus>();

            foreach (var party in parties.Values)
            {
                foreach (var tokenStatus in party.Tokens.Values)
                {
                    if (IncludeTokenStatus(tokenStatus))
                        matchingTokenStatus.Add(tokenStatus);
                }
            }

            return matchingTokenStatus;

        }

        #endregion

        #region GetTokenStatus      (PartyId = null)

        public IEnumerable<TokenStatus> GetTokenStatus(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Tokens.Values;
            }

            else
            {

                var tokenStatus = new List<TokenStatus>();

                foreach (var party in parties.Values)
                    tokenStatus.AddRange(party.Tokens.Values);

                return tokenStatus;

            }

            return [];

        }

        #endregion

        #endregion

        #region Sessions

        #region Events

        public delegate Task OnSessionAddedDelegate          (Session Session);
        public delegate Task OnChargingSessionChangedDelegate(Session Session);
        public delegate Task OnSessionRemovedDelegate        (Session Session);

        public event OnSessionAddedDelegate?            OnSessionAdded;
        public event OnChargingSessionChangedDelegate?  OnSessionChanged;
        public event OnSessionRemovedDelegate?          OnSessionRemoved;

        #endregion


        public delegate Task<Session> OnSessionSlowStorageLookupDelegate(Party_Idv3  PartyId,
                                                                         Session_Id  SessionId);

        public event OnSessionSlowStorageLookupDelegate? OnSessionSlowStorageLookup;


        #region AddSession            (Session, ...)

        /// <summary>
        /// Add the given charging session.
        /// </summary>
        /// <param name="Session">The charging session to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Session>>

            AddSession(Session            Session,
                       Boolean            SkipNotifications   = false,
                       EventTracking_Id?  EventTrackingId     = null,
                       User_Id?           CurrentUserId       = null,
                       CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Session.CountryCode,
                                    Session.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Sessions.TryAdd(Session.Id, Session))
                {

                    Session.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addSession,
                              Session.ToJSON(
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
                                  OnSessionAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Session
                                  )
                              );

                    }

                    return AddResult<Session>.Success(
                               EventTrackingId,
                               Session
                           );

                }

                return AddResult<Session>.Failed(
                           EventTrackingId,
                           Session,
                           "The given session already exists!"
                       );

            }

            return AddResult<Session>.Failed(
                       EventTrackingId,
                       Session,
                       $"The party identification '{partyId}' of the session is unknown!"
                   );

        }

        #endregion

        #region AddSessionIfNotExists (Session, ...)

        /// <summary>
        /// Add the given charging session if it does not already exist.
        /// </summary>
        /// <param name="Session">The charging session to add.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<Session>>

            AddSessionIfNotExists(Session            Session,
                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Session.CountryCode,
                                    Session.PartyId
                                );

            if (parties.TryGetValue(Party_Idv3.From(Session.CountryCode, Session.PartyId), out var party))
            {

                if (party.Sessions.TryAdd(Session.Id, Session))
                {

                    Session.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addSession,
                              Session.ToJSON(
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
                                  OnSessionAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Session
                                  )
                              );

                    }

                    return AddResult<Session>.Success(
                               EventTrackingId,
                               Session
                           );

                }

                return AddResult<Session>.NoOperation(
                           EventTrackingId,
                           Session,
                           "The given session already exists."
                       );

            }

            return AddResult<Session>.Failed(
                       EventTrackingId,
                       Session,
                       $"The party identification '{partyId}' of the session is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateSession    (Session,                          AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given charging session.
        /// </summary>
        /// <param name="Session">The charging session to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<Session>>

            AddOrUpdateSession(Session            Session,
                               Boolean?           AllowDowngrades     = false,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Session.CountryCode,
                                    Session.PartyId
                                );

            if (parties.TryGetValue(Party_Idv3.From(Session.CountryCode, Session.PartyId), out var party))
            {

                #region Update an existing session

                if (party.Sessions.TryGetValue(Session.Id, out var existingSession))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        Session.LastUpdated <= existingSession.LastUpdated)
                    {

                        return AddOrUpdateResult<Session>.Failed(
                                   EventTrackingId,
                                   Session,
                                   "The 'lastUpdated' timestamp of the new session must be newer then the timestamp of the existing session!"
                               );

                    }

                    //if (Session.LastUpdated.ToISO8601() == existingSession.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<Session>.NoOperation(Session,
                    //                                                   "The 'lastUpdated' timestamp of the new session must be newer then the timestamp of the existing session!");

                    var aa = existingSession.Equals(existingSession);

                    if (party.Sessions.TryUpdate(Session.Id,
                                                 Session,
                                                 existingSession))
                    {

                        Session.CommonAPI = this;

                        await LogAsset(
                                  CommonHTTPAPI.addOrUpdateSession,
                                  Session.ToJSON(
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
                                      OnSessionChanged,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          Session
                                      )
                                  );

                        }

                        return AddOrUpdateResult<Session>.Updated(
                                   EventTrackingId,
                                   Session
                               );

                    }

                    return AddOrUpdateResult<Session>.Failed(
                               EventTrackingId,
                               Session,
                               "Updating the given session failed!"
                           );

                }

                #endregion

                #region Add a new session

                if (party.Sessions.TryAdd(Session.Id, Session))
                {

                    Session.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateSession,
                              Session.ToJSON(
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
                                  OnSessionAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Session
                                  )
                              );

                    }

                    return AddOrUpdateResult<Session>.Created(
                               EventTrackingId,
                               Session
                           );

                }

                #endregion

                return AddOrUpdateResult<Session>.Failed(
                           EventTrackingId,
                           Session,
                           "Adding the given session failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<Session>.Failed(
                       EventTrackingId,
                       Session,
                       $"The party identification '{partyId}' of the session is unknown!"
                   );

        }

        #endregion

        #region UpdateSession         (Session,                          AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given charging session.
        /// </summary>
        /// <param name="Session">The charging session to update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<Session>>

            UpdateSession(Session            Session,
                          Boolean?           AllowDowngrades     = false,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Session.CountryCode,
                                    Session.PartyId
                                );

            if (parties.TryGetValue(Party_Idv3.From(Session.CountryCode, Session.PartyId), out var party))
            {

                if (!party.Sessions.TryGetValue(Session.Id, out var existingSession))
                    return UpdateResult<Session>.Failed(
                               EventTrackingId,
                               Session,
                               $"The given session identification '{Session.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Session.LastUpdated <= existingSession.LastUpdated)
                {

                    return UpdateResult<Session>.Failed(
                               EventTrackingId, Session,
                               "The 'lastUpdated' timestamp of the new charging session must be newer then the timestamp of the existing session!"
                           );

                }

                #endregion


                if (party.Sessions.TryUpdate(Session.Id,
                                             Session,
                                             existingSession))
                {

                    Session.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.updateSession,
                              Session.ToJSON(
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
                                  OnSessionChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      Session
                                  )
                              );

                    }

                    return UpdateResult<Session>.Success(
                               EventTrackingId,
                               Session
                           );

                }

                return UpdateResult<Session>.Failed(
                           EventTrackingId,
                           Session,
                           "Sessions.TryUpdate(Session.Id, Session, Session) failed!"
                       );

            }

            return UpdateResult<Session>.Failed(
                       EventTrackingId,
                       Session,
                       $"The party identification '{partyId}' of the session is unknown!"
                   );

        }

        #endregion

        #region TryPatchSession       (PartyId, SessionId, SessionPatch, AllowDowngrades = false, ...)

        /// <summary>
        /// Try to patch the given charging session with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the charging session.</param>
        /// <param name="SessionId">The identification of the session to patch.</param>
        /// <param name="SessionPatch">The JSON patch document to apply to the session.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<Session>>

            TryPatchSession(Party_Idv3         PartyId,
                            Session_Id         SessionId,
                            JObject            SessionPatch,
                            Boolean?           AllowDowngrades     = false,
                            Boolean            SkipNotifications   = false,
                            EventTracking_Id?  EventTrackingId     = null,
                            User_Id?           CurrentUserId       = null,
                            CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Sessions.TryGetValue(SessionId, out var existingSession))
                {

                    var patchResult = existingSession.TryPatch(
                                          SessionPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedSession))
                    {

                        var updateSessionResult = await UpdateSession(
                                                            patchedSession,
                                                            AllowDowngrades,
                                                            SkipNotifications,
                                                            EventTrackingId,
                                                            CurrentUserId,
                                                            CancellationToken
                                                        );

                        if (updateSessionResult.IsFailed)
                            return PatchResult<Session>.Failed(
                                       EventTrackingId,
                                       existingSession,
                                       "Could not patch the session: " + updateSessionResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<Session>.Failed(
                           EventTrackingId,
                           $"The given session '{SessionId}' is does not exist!"
                       );

            }

            return PatchResult<Session>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the session is unknown!"
                   );

        }

        #endregion


        #region RemoveSession         (Session, ...)

        /// <summary>
        /// Remove the given charging session.
        /// </summary>
        /// <param name="Session">A charging session.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<Session>>

            RemoveSession(Session            Session,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

                => RemoveSession(
                       Party_Idv3.From(
                           Session.CountryCode,
                           Session.PartyId
                       ),
                       Session.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveSession         (PartyId, SessionId, ...)

        /// <summary>
        /// Remove the given charging session.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the charging session.</param>
        /// <param name="SessionId">An unique charging session identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<Session>>

            RemoveSession(Party_Idv3         PartyId,
                          Session_Id         SessionId,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Sessions.TryRemove(SessionId, out var session))
                {

                    await LogAsset(
                              CommonHTTPAPI.removeSession,
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
                                  OnSessionRemoved,
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
                           $"The session '{PartyId}/{SessionId}' is unknown!"
                       );

            }

            return RemoveResult<Session>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the session is unknown!"
                   );

        }

        #endregion

        #region RemoveAllSessions     (...)

        /// <summary>
        /// Remove all charging sessions.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllSessions(Boolean            SkipNotifications   = false,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null,
                              CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var sessions = new List<Session>();

            foreach (var party in parties.Values)
            {
                sessions.AddRange(party.Sessions.Values);
                party.Sessions.Clear();
            }

            await LogAsset(
                      CommonHTTPAPI.removeAllSessions,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var session in sessions)
                    await LogEvent(
                              OnSessionRemoved,
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

        #region RemoveAllSessions     (IncludeSessions, ...)

        /// <summary>
        /// Remove all matching charging sessions.
        /// </summary>
        /// <param name="IncludeSessions">A filter delegate to include charging sessions for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllSessions(Func<Session, Boolean>  IncludeSessions,
                              Boolean                 SkipNotifications   = false,
                              EventTracking_Id?       EventTrackingId     = null,
                              User_Id?                CurrentUserId       = null,
                              CancellationToken       CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingSessions  = new List<Session>();
            var removedSessions   = new List<Session>();
            var failedSessions    = new List<RemoveResult<Session>>();

            foreach (var party in parties.Values)
            {
                foreach (var session in party.Sessions.Values)
                {
                    if (IncludeSessions(session))
                        matchingSessions.Add(session);
                }
            }

            foreach (var session in matchingSessions)
            {

                var result = await RemoveSession(
                                       session,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedSessions.Add(session);
                else
                    failedSessions. Add(result);

            }

            return removedSessions.Count != 0 && failedSessions.Count == 0

                       ? RemoveResult<IEnumerable<Session>>.Success(
                             EventTrackingId,
                             removedSessions
                         )

                       : removedSessions.Count == 0 && failedSessions.Count == 0

                             ? RemoveResult<IEnumerable<Session>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Session>>.Failed(
                                   EventTrackingId,
                                   failedSessions.
                                       Select(removeResult => removeResult.Data).
                                       Where (session      => session is not null).
                                       Cast<Session>(),
                                   failedSessions.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllSessions     (IncludeSessionIds, ...)

        /// <summary>
        /// Remove all matching charging sessions.
        /// </summary>
        /// <param name="IncludeSessionIds">A filter delegate to include charging sessions for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllSessions(Func<Party_Idv3, Session_Id, Boolean>  IncludeSessionIds,
                              Boolean                                SkipNotifications   = false,
                              EventTracking_Id?                      EventTrackingId     = null,
                              User_Id?                               CurrentUserId       = null,
                              CancellationToken                      CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingSessions  = new List<Session>();
            var removedSessions   = new List<Session>();
            var failedSessions    = new List<RemoveResult<Session>>();

            foreach (var party in parties.Values)
            {
                foreach (var session in party.Sessions.Values)
                {
                    if (IncludeSessionIds(party.Id, session.Id))
                        matchingSessions.Add(session);
                }
            }

            foreach (var session in matchingSessions)
            {

                var result = await RemoveSession(
                                       session,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedSessions.Add(session);
                else
                    failedSessions. Add(result);

            }

            return removedSessions.Count != 0 && failedSessions.Count == 0

                       ? RemoveResult<IEnumerable<Session>>.Success(
                             EventTrackingId,
                             removedSessions
                         )

                       : removedSessions.Count == 0 && failedSessions.Count == 0

                             ? RemoveResult<IEnumerable<Session>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<Session>>.Failed(
                                   EventTrackingId,
                                   failedSessions.
                                       Select(removeResult => removeResult.Data).
                                       Where (session      => session is not null).
                                       Cast<Session>(),
                                   failedSessions.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllSessions     (PartyId, ...)

        /// <summary>
        /// Remove all charging sessions owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllSessions(Party_Idv3         PartyId,
                              Boolean            SkipNotifications   = false,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null,
                              CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                var matchingSessions  = party.Sessions.Values;
                var removedSessions   = new List<Session>();
                var failedSessions    = new List<RemoveResult<Session>>();

                foreach (var session in matchingSessions)
                {

                    var result = await RemoveSession(
                                           session,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedSessions.Add(session);
                    else
                        failedSessions. Add(result);

                }

                return removedSessions.Count != 0 && failedSessions.Count == 0

                           ? RemoveResult<IEnumerable<Session>>.Success(
                                 EventTrackingId,
                                 removedSessions
                             )

                           : removedSessions.Count == 0 && failedSessions.Count == 0

                                 ? RemoveResult<IEnumerable<Session>>.NoOperation(
                                       EventTrackingId,
                                       []
                                   )

                                 : RemoveResult<IEnumerable<Session>>.Failed(
                                       EventTrackingId,
                                       failedSessions.
                                           Select(removeResult => removeResult.Data).
                                           Where (session      => session is not null).
                                           Cast<Session>(),
                                       failedSessions.Select(removeResult => removeResult.ErrorResponse).
                                           AggregateWith(", ")
                                   );

            }

            return RemoveResult<IEnumerable<Session>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' is unknown!"
                   );

        }

        #endregion


        #region SessionExists         (PartyId, SessionId)

        public Boolean SessionExists(Party_Idv3  PartyId,
                                     Session_Id  SessionId)
{

            if (parties.TryGetValue(PartyId, out var party) &&
                party.Sessions.ContainsKey(SessionId))
            {
                return true;
            }

            var onSessionSlowStorageLookup = OnSessionSlowStorageLookup;
            if (onSessionSlowStorageLookup is not null)
            {
                try
                {

                    return onSessionSlowStorageLookup(
                               PartyId,
                               SessionId
                           ).Result is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(SessionExists), " ", nameof(OnSessionSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            return false;

        }

        #endregion

        #region GetSession            (PartyId, SessionId)

        public Session? GetSession(Party_Idv3  PartyId,
                                   Session_Id  SessionId)
        {

            if (TryGetSession(PartyId,
                              SessionId,
                              out var cdr))
            {
                return cdr;
            }

            return null;

        }

        #endregion

        #region TryGetSession         (PartyId, SessionId, out Session)

        public Boolean TryGetSession(Party_Idv3                        PartyId,
                                     Session_Id                        SessionId,
                                     [NotNullWhen(true)] out Session?  Session)
        {

            if (parties.       TryGetValue(PartyId,   out var party) &&
                party.Sessions.TryGetValue(SessionId, out Session))
            {
                return true;
            }

            var onSessionSlowStorageLookup = OnSessionSlowStorageLookup;
            if (onSessionSlowStorageLookup is not null)
            {
                try
                {

                    Session = onSessionSlowStorageLookup(
                                  PartyId,
                                  SessionId
                              ).Result;

                    return Session is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetSession), " ", nameof(OnSessionSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            Session = null;
            return false;

        }

        #endregion

        #region GetSessions           (IncludeSession)

        public IEnumerable<Session> GetSessions(Func<Session, Boolean> IncludeSession)
        {

            var sessions = new List<Session>();

            foreach (var party in parties.Values)
            {
                foreach (var session in party.Sessions.Values)
                {
                    if (IncludeSession(session))
                        sessions.Add(session);
                }
            }

            return sessions;

        }

        #endregion

        #region GetSessions           (PartyId = null)

        public IEnumerable<Session> GetSessions(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Sessions.Values;
            }

            else
            {

                var sessions = new List<Session>();

                foreach (var party in parties.Values)
                    sessions.AddRange(party.Sessions.Values);

                return sessions;

            }

            return [];

        }

        #endregion

        #endregion

        #region ChargeDetailRecords

        #region Events

        public delegate Task OnChargeDetailRecordAddedDelegate  (CDR CDR);
        public delegate Task OnChargeDetailRecordChangedDelegate(CDR CDR);
        public delegate Task OnChargeDetailRecordRemovedDelegate(CDR CDR);

        public event OnChargeDetailRecordAddedDelegate?    OnChargeDetailRecordAdded;
        public event OnChargeDetailRecordChangedDelegate?  OnChargeDetailRecordChanged;
        public event OnChargeDetailRecordRemovedDelegate?  OnChargeDetailRecordRemoved;

        #endregion


        public delegate Task<CDR> OnChargeDetailRecordSlowStorageLookupDelegate(Party_Idv3  PartyId,
                                                                                CDR_Id      CDRId);

        public event OnChargeDetailRecordSlowStorageLookupDelegate? OnChargeDetailRecordSlowStorageLookup;


        #region AddCDR            (CDR, ...)

        /// <summary>
        /// Add the given charge detail record.
        /// </summary>
        /// <param name="CDR">The charge detail record to add or update.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<CDR>>

            AddCDR(CDR                CDR,
                   Boolean            SkipNotifications   = false,
                   EventTracking_Id?  EventTrackingId     = null,
                   User_Id?           CurrentUserId       = null,
                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    CDR.CountryCode,
                                    CDR.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.CDRs.TryAdd(CDR.Id, CDR))
                {

                    CDR.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addChargeDetailRecord,
                              CDR.ToJSON(
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomPriceLimitSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTaxAmountSerializer,
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
                                  OnChargeDetailRecordAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      CDR
                                  )
                              );

                    }

                    return AddResult<CDR>.Success(
                               EventTrackingId,
                               CDR
                           );

                }

                return AddResult<CDR>.Failed(
                           EventTrackingId,
                           CDR,
                           "The given charge detail record already exists!"
                       );

            }

            return AddResult<CDR>.Failed(
                       EventTrackingId,
                       CDR,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddCDRIfNotExists (CDR, ...)

        /// <summary>
        /// Add the given charge detail record if it does not already exist.
        /// </summary>
        /// <param name="CDR">The charge detail record to add or update.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddResult<CDR>>

            AddCDRIfNotExists(CDR                CDR,
                              Boolean            SkipNotifications   = false,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null,
                              CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    CDR.CountryCode,
                                    CDR.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.CDRs.TryAdd(CDR.Id, CDR))
                {

                    CDR.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addChargeDetailRecordIfNotExists,
                              CDR.ToJSON(
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomPriceLimitSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTaxAmountSerializer,
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
                                  OnChargeDetailRecordAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      CDR
                                  )
                              );

                    }

                    return AddResult<CDR>.Success(
                               EventTrackingId,
                               CDR
                           );

                }

                return AddResult<CDR>.NoOperation(
                           EventTrackingId,
                           CDR,
                           "The given charge detail record already exists."
                       );

            }

            return AddResult<CDR>.Failed(
                       EventTrackingId,
                       CDR,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateCDR    (CDR, AllowDowngrades = false, ...)

        /// <summary>
        /// Add or update the given charge detail record.
        /// </summary>
        /// <param name="CDR">The charge detail record to add or update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<AddOrUpdateResult<CDR>>

            AddOrUpdateCDR(CDR                CDR,
                           Boolean?           AllowDowngrades     = false,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null,
                           CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    CDR.CountryCode,
                                    CDR.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                #region Update an existing charge detail record

                if (party.CDRs.TryGetValue(CDR.Id, out var existingCDR))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        CDR.LastUpdated <= existingCDR.LastUpdated)
                    {

                        return AddOrUpdateResult<CDR>.Failed(
                                   EventTrackingId,
                                   CDR,
                                   "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!"
                               );

                    }

                    //if (CDR.LastUpdated.ToISO8601() == existingCDR.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<CDR>.NoOperation(CDR,
                    //                                                   "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");

                    var aa = existingCDR.Equals(existingCDR);

                    if (party.CDRs.TryUpdate(CDR.Id,
                                             CDR,
                                             existingCDR))
                    {

                        CDR.CommonAPI = this;

                        await LogAsset(
                                  CommonHTTPAPI.addOrUpdateChargeDetailRecord,
                                  CDR.ToJSON(
                                      CustomCDRSerializer,
                                      CustomCDRTokenSerializer,
                                      CustomCDRLocationSerializer,
                                      CustomEVSEEnergyMeterSerializer,
                                      CustomTransparencySoftwareSerializer,
                                      CustomTariffSerializer,
                                      CustomDisplayTextSerializer,
                                      CustomPriceSerializer,
                                      CustomPriceLimitSerializer,
                                      CustomTariffElementSerializer,
                                      CustomPriceComponentSerializer,
                                      CustomTaxAmountSerializer,
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
                                      OnChargeDetailRecordChanged,
                                      loggingDelegate => loggingDelegate.Invoke(
                                          CDR
                                      )
                                  );

                        }

                        return AddOrUpdateResult<CDR>.Updated(
                                   EventTrackingId,
                                   CDR
                               );

                    }

                    return AddOrUpdateResult<CDR>.Failed(
                               EventTrackingId,
                               CDR,
                               "Updating the given charge detail record failed!"
                           );

                }

                #endregion

                #region Add a new charge detail record

                if (party.CDRs.TryAdd(CDR.Id, CDR))
                {

                    CDR.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateChargeDetailRecord,
                              CDR.ToJSON(
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomPriceLimitSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTaxAmountSerializer,
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
                                  OnChargeDetailRecordAdded,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      CDR
                                  )
                              );

                    }

                    return AddOrUpdateResult<CDR>.Created(
                               EventTrackingId,
                               CDR
                           );

                }

                #endregion

                return AddOrUpdateResult<CDR>.Failed(
                           EventTrackingId,
                           CDR,
                           "Adding the given charge detail record failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<CDR>.Failed(
                       EventTrackingId,
                       CDR,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region UpdateCDR         (CDR, AllowDowngrades = false, ...)

        /// <summary>
        /// Update the given charge detail record.
        /// </summary>
        /// <param name="CDR">The charge detail record to update.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<UpdateResult<CDR>>

            UpdateCDR(CDR                CDR,
                      Boolean?           AllowDowngrades     = false,
                      Boolean            SkipNotifications   = false,
                      EventTracking_Id?  EventTrackingId     = null,
                      User_Id?           CurrentUserId       = null,
                      CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    CDR.CountryCode,
                                    CDR.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (!party.CDRs.TryGetValue(CDR.Id, out var existingCDR))
                    return UpdateResult<CDR>.Failed(
                               EventTrackingId,
                               CDR,
                               $"The given charge detail record identification '{CDR.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    CDR.LastUpdated <= existingCDR.LastUpdated)
                {

                    return UpdateResult<CDR>.Failed(
                               EventTrackingId, CDR,
                               "The 'lastUpdated' timestamp of the new charging charge detail record must be newer then the timestamp of the existing charge detail record!"
                           );

                }

                #endregion


                if (party.CDRs.TryUpdate(CDR.Id,
                                         CDR,
                                         existingCDR))
                {

                    CDR.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.updateChargeDetailRecord,
                              CDR.ToJSON(
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareSerializer,
                                  CustomTariffSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomPriceSerializer,
                                  CustomPriceLimitSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTaxAmountSerializer,
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
                                  OnChargeDetailRecordChanged,
                                  loggingDelegate => loggingDelegate.Invoke(
                                      CDR
                                  )
                              );

                    }

                    return UpdateResult<CDR>.Success(
                               EventTrackingId,
                               CDR
                           );

                }

                return UpdateResult<CDR>.Failed(
                           EventTrackingId,
                           CDR,
                           "charge detail records.TryUpdate(CDR.Id, CDR, CDR) failed!"
                       );

            }

            return UpdateResult<CDR>.Failed(
                       EventTrackingId,
                       CDR,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region TryPatchCDR       (PartyId, CDRId, CDRPatch, AllowDowngrades = false, ...)   // Non-Standard

        /// <summary>
        /// Try to patch the given charge detail record with the given JSON patch document.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the charge detail record.</param>
        /// <param name="CDRId">The identification of the charge detail record to patch.</param>
        /// <param name="CDRPatch">The JSON patch document to apply to the given charge detail record.</param>
        /// <param name="AllowDowngrades">Whether to allow downgrades of the 'lastUpdated' timestamp or not.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<PatchResult<CDR>>

            TryPatchCDR(Party_Idv3         PartyId,
                        CDR_Id             CDRId,
                        JObject            CDRPatch,
                        Boolean?           AllowDowngrades     = false,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null,
                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (CDRPatch is null || !CDRPatch.HasValues)
                return PatchResult<CDR>.Failed(
                           EventTrackingId,
                           "The given charge detail record patch must not be null or empty!"
                       );

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.CDRs.TryGetValue(CDRId, out var existingCDR))
                {
 
                    var patchResult = existingCDR.TryPatch(
                                          CDRPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccessAndDataNotNull(out var patchedCDR))
                    {

                        var updateCDRResult = await UpdateCDR(
                                                        patchedCDR,
                                                        AllowDowngrades,
                                                        SkipNotifications,
                                                        EventTrackingId,
                                                        CurrentUserId,
                                                        CancellationToken
                                                    );

                        if (updateCDRResult.IsFailed)
                            return PatchResult<CDR>.Failed(
                                       EventTrackingId,
                                       existingCDR,
                                       "Could not patch the charge detail record: " + updateCDRResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<CDR>.Failed(
                           EventTrackingId,
                           $"The given charge detail record '{PartyId}/{CDRId}' does not exist!"
                       );

            }

            return PatchResult<CDR>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion


        #region RemoveCDR         (CDR, ...)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="CDR">The charge detail record to remove.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public Task<RemoveResult<CDR>>

            RemoveCDR(CDR                CDR,
                      Boolean            SkipNotifications   = false,
                      EventTracking_Id?  EventTrackingId     = null,
                      User_Id?           CurrentUserId       = null,
                      CancellationToken  CancellationToken   = default)

                => RemoveCDR(
                       Party_Idv3.From(
                           CDR.CountryCode,
                           CDR.PartyId
                       ),
                       CDR.Id,
                       SkipNotifications,
                       EventTrackingId,
                       CurrentUserId,
                       CancellationToken
                   );

        #endregion

        #region RemoveCDR         (PartyId, CDRId, ...)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="PartyId">The identification of the party owning the charge detail record.</param>
        /// <param name="CDRId">A unique identification of a charge detail record.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<CDR>>

            RemoveCDR(Party_Idv3         PartyId,
                      CDR_Id             CDRId,
                      Boolean            SkipNotifications   = false,
                      EventTracking_Id?  EventTrackingId     = null,
                      User_Id?           CurrentUserId       = null,
                      CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.CDRs.TryRemove(CDRId, out var cdr))
                {

                    await LogAsset(
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
                                  CustomPriceLimitSerializer,
                                  CustomTariffElementSerializer,
                                  CustomPriceComponentSerializer,
                                  CustomTaxAmountSerializer,
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
                                  OnChargeDetailRecordRemoved,
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
                           $"The charge detail record '{PartyId}/{CDRId}' is unknown!"
                       );

            }

            return RemoveResult<CDR>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveAllCDRs     (...)

        /// <summary>
        /// Remove all charge detail records.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllCDRs(Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var cdrs = new List<CDR>();

            foreach (var party in parties.Values)
            {
                cdrs.AddRange(party.CDRs.Values);
                party.CDRs.Clear();
            }

            await LogAsset(
                      CommonHTTPAPI.removeAllChargeDetailRecords,
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            if (!SkipNotifications)
            {

                foreach (var cdr in cdrs)
                    await LogEvent(
                              OnChargeDetailRecordRemoved,
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

        #region RemoveAllCDRs     (IncludeCDRs,   ...)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">A filter delegate to include charge detail records for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllCDRs(Func<CDR, Boolean>  IncludeCDRs,
                          Boolean             SkipNotifications   = false,
                          EventTracking_Id?   EventTrackingId     = null,
                          User_Id?            CurrentUserId       = null,
                          CancellationToken   CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingCDRs  = new List<CDR>();
            var removedCDRs   = new List<CDR>();
            var failedCDRs    = new List<RemoveResult<CDR>>();

            foreach (var party in parties.Values)
            {
                foreach (var cdr in party.CDRs.Values)
                {
                    if (IncludeCDRs(cdr))
                        matchingCDRs.Add(cdr);
                }
            }

            foreach (var cdr in matchingCDRs)
            {

                var result = await RemoveCDR(
                                       cdr,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedCDRs.Add(cdr);
                else
                    failedCDRs. Add(result);

            }

            return removedCDRs.Count != 0 && failedCDRs.Count == 0

                       ? RemoveResult<IEnumerable<CDR>>.Success(
                             EventTrackingId,
                             removedCDRs
                         )

                       : removedCDRs.Count == 0 && failedCDRs.Count == 0

                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<CDR>>.Failed(
                                   EventTrackingId,
                                   failedCDRs.
                                       Select(removeResult => removeResult.Data).
                                       Where (cdr          => cdr is not null).
                                       Cast<CDR>(),
                                   failedCDRs.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllCDRs     (IncludeCDRIds, ...)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRIds">A filter delegate to include charge detail records for removal.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllCDRs(Func<CDR_Id, Boolean>  IncludeCDRIds,
                          Boolean                SkipNotifications   = false,
                          EventTracking_Id?      EventTrackingId     = null,
                          User_Id?               CurrentUserId       = null,
                          CancellationToken      CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var matchingCDRs  = new List<CDR>();
            var removedCDRs   = new List<CDR>();
            var failedCDRs    = new List<RemoveResult<CDR>>();

            foreach (var party in parties.Values)
            {
                foreach (var cdr in party.CDRs.Values)
                {
                    if (IncludeCDRIds(cdr.Id))
                        matchingCDRs.Add(cdr);
                }
            }

            foreach (var cdr in matchingCDRs)
            {

                var result = await RemoveCDR(
                                       cdr,
                                       SkipNotifications,
                                       EventTrackingId,
                                       CurrentUserId,
                                       CancellationToken
                                   );

                if (result.IsSuccess)
                    removedCDRs.Add(cdr);
                else
                    failedCDRs. Add(result);

            }

            return removedCDRs.Count != 0 && failedCDRs.Count == 0

                       ? RemoveResult<IEnumerable<CDR>>.Success(
                             EventTrackingId,
                             removedCDRs
                         )

                       : removedCDRs.Count == 0 && failedCDRs.Count == 0

                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(
                                   EventTrackingId,
                                   []
                               )

                             : RemoveResult<IEnumerable<CDR>>.Failed(
                                   EventTrackingId,
                                   failedCDRs.
                                       Select(removeResult => removeResult.Data).
                                       Where (cdr          => cdr is not null).
                                       Cast<CDR>(),
                                   failedCDRs.Select(removeResult => removeResult.ErrorResponse).
                                       AggregateWith(", ")
                               );

        }

        #endregion

        #region RemoveAllCDRs     (PartyId, ...)

        /// <summary>
        /// Remove all charge detail records owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating log entries.</param>
        /// <param name="CurrentUserId">An optional user identification for correlating log entries.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllCDRs(Party_Idv3         PartyId,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                var matchingCDRs  = party.CDRs.Values;
                var removedCDRs   = new List<CDR>();
                var failedCDRs    = new List<RemoveResult<CDR>>();

                foreach (var cdr in matchingCDRs)
                {

                    var result = await RemoveCDR(
                                           cdr,
                                           SkipNotifications,
                                           EventTrackingId,
                                           CurrentUserId,
                                           CancellationToken
                                       );

                    if (result.IsSuccess)
                        removedCDRs.Add(cdr);
                    else
                        failedCDRs. Add(result);

                }

                return removedCDRs.Count != 0 && failedCDRs.Count == 0

                           ? RemoveResult<IEnumerable<CDR>>.Success(
                                 EventTrackingId,
                                 removedCDRs
                             )

                           : removedCDRs.Count == 0 && failedCDRs.Count == 0

                                 ? RemoveResult<IEnumerable<CDR>>.NoOperation(
                                       EventTrackingId,
                                       []
                                   )

                                 : RemoveResult<IEnumerable<CDR>>.Failed(
                                       EventTrackingId,
                                       failedCDRs.
                                           Select(removeResult => removeResult.Data).
                                           Where (cdr          => cdr is not null).
                                           Cast<CDR>(),
                                       failedCDRs.Select(removeResult => removeResult.ErrorResponse).
                                           AggregateWith(", ")
                                   );

            }

            return RemoveResult<IEnumerable<CDR>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' is unknown!"
                   );

        }

        #endregion


        #region CDRExists         (PartyId, CDRId)

        public Boolean CDRExists(Party_Idv3  PartyId,
                                 CDR_Id      CDRId)
        {

            if (parties.TryGetValue(PartyId, out var party) &&
                party.CDRs.ContainsKey(CDRId))
            {
                return true;
            }

            var onChargeDetailRecordSlowStorageLookup = OnChargeDetailRecordSlowStorageLookup;
            if (onChargeDetailRecordSlowStorageLookup is not null)
            {
                try
                {

                    return onChargeDetailRecordSlowStorageLookup(
                               PartyId,
                               CDRId
                           ).Result is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(CDRExists), " ", nameof(OnChargeDetailRecordSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            return false;

        }

        #endregion

        #region GetCDR            (PartyId, CDRId)

        public CDR? GetCDR(Party_Idv3  PartyId,
                           CDR_Id      CDRId)
        {

            if (TryGetCDR(PartyId,
                          CDRId,
                          out var cdr))
            {
                return cdr;
            }

            return null;

        }

        #endregion

        #region TryGetCDR         (PartyId, CDRId, out CDR)

        public Boolean TryGetCDR(Party_Idv3                    PartyId,
                                 CDR_Id                        CDRId,
                                 [NotNullWhen(true)] out CDR?  CDR)
        {

            if (parties.   TryGetValue(PartyId, out var party) &&
                party.CDRs.TryGetValue(CDRId,   out CDR))
            {
                return true;
            }

            var onChargeDetailRecordLookup = OnChargeDetailRecordSlowStorageLookup;
            if (onChargeDetailRecordLookup is not null)
            {
                try
                {

                    CDR = onChargeDetailRecordLookup(
                              PartyId,
                              CDRId
                          ).Result;

                    return CDR is not null;

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetCDR), " ", nameof(OnChargeDetailRecordSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }

            CDR = null;
            return false;

        }

        #endregion

        #region GetCDRs           (IncludeCDR)

        public IEnumerable<CDR> GetCDRs(Func<CDR, Boolean> IncludeCDR)
        {

            var sessions = new List<CDR>();

            foreach (var party in parties.Values)
            {
                foreach (var cdr in party.CDRs.Values)
                {
                    if (IncludeCDR(cdr))
                        sessions.Add(cdr);
                }
            }

            return sessions;

        }

        #endregion

        #region GetCDRs           (PartyId = null)

        public IEnumerable<CDR> GetCDRs(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.CDRs.Values;
            }

            else
            {

                var sessions = new List<CDR>();

                foreach (var party in parties.Values)
                    sessions.AddRange(party.CDRs.Values);

                return sessions;

            }

            return [];

        }

        #endregion

        #endregion


        #region Payment Terminals

        #region Events

        public delegate Task OnPaymentTerminalAddedDelegate  (Terminal PaymentTerminal);

        public event OnPaymentTerminalAddedDelegate?    OnPaymentTerminalAdded;


        public delegate Task OnPaymentTerminalChangedDelegate(Terminal PaymentTerminal);

        public event OnPaymentTerminalChangedDelegate?  OnPaymentTerminalChanged;

        #endregion


        #region AddOrUpdatePaymentTerminal    (Terminal,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<AddOrUpdateResult<Terminal>>

            AddOrUpdatePaymentTerminal(Terminal           Terminal,
                                       Boolean?           AllowDowngrades     = false,
                                       Boolean            SkipNotifications   = false,
                                       EventTracking_Id?  EventTrackingId     = null,
                                       User_Id?           CurrentUserId       = null,
                                       CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Terminal.CountryCode.Value,
                                    Terminal.PartyId.    Value
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                #region Update an existing terminal

                if (party.PaymentTerminals.TryGetValue(Terminal.Id, out var existingTerminal))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        Terminal.LastUpdated <= existingTerminal.LastUpdated)
                    {
                        return AddOrUpdateResult<Terminal>.Failed(
                                   EventTrackingId,
                                   Terminal,
                                   "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!"
                               );
                    }

                    if (party.PaymentTerminals.TryUpdate(Terminal.Id,
                                                  Terminal,
                                                  existingTerminal))
                    {

                        Terminal.CommonAPI = this;

                        await LogAsset(
                                  CommonHTTPAPI.addOrUpdateTerminal,
                                  Terminal.ToJSON(true,
                                                  true,
                                                  CustomTerminalSerializer,
                                                  CustomDisplayTextSerializer,
                                                  CustomImageSerializer),
                                  EventTrackingId,
                                  CurrentUserId,
                                  CancellationToken
                              );

                        if (!SkipNotifications)
                        {

                            var OnPaymentTerminalChangedLocal = OnPaymentTerminalChanged;
                            if (OnPaymentTerminalChangedLocal is not null)
                            {
                                try
                                {
                                    OnPaymentTerminalChangedLocal(Terminal).Wait(CancellationToken);
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdatePaymentTerminal), " ", nameof(OnPaymentTerminalChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                        }

                        return AddOrUpdateResult<Terminal>.Updated(
                                   EventTrackingId,
                                   Terminal
                               );

                    }

                    return AddOrUpdateResult<Terminal>.Failed(
                               EventTrackingId,
                               Terminal,
                               "Updating the given terminal failed!"
                           );

                }

                #endregion

                #region Add a new terminal

                if (party.PaymentTerminals.TryAdd(Terminal.Id, Terminal))
                {

                    Terminal.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateTerminal,
                              Terminal.ToJSON(true,
                                              true,
                                              CustomTerminalSerializer,
                                              CustomDisplayTextSerializer,
                                              CustomImageSerializer),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnPaymentTerminalAddedLocal = OnPaymentTerminalAdded;
                        if (OnPaymentTerminalAddedLocal is not null)
                        {
                            try
                            {
                                OnPaymentTerminalAddedLocal(Terminal).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdatePaymentTerminal), " ", nameof(OnPaymentTerminalAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return AddOrUpdateResult<Terminal>.Created(
                               EventTrackingId,
                               Terminal
                           );

                }

                #endregion

                return AddOrUpdateResult<Terminal>.Failed(
                            EventTrackingId,
                            Terminal,
                            "Adding the given session failed because of concurrency issues!"
                        );

            }

            return AddOrUpdateResult<Terminal>.Failed(
                        EventTrackingId,
                        Terminal,
                        $"The party identification '{partyId}' of the session is unknown!"
                    );

        }

        #endregion

        #region UpdatePaymentTerminal         (Terminal,                          AllowDowngrades = false, ...)

        public async Task<UpdateResult<Terminal>>

            UpdatePaymentTerminal(Terminal           Terminal,
                                  Boolean?           AllowDowngrades     = false,
                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Terminal.CountryCode.Value,
                                    Terminal.PartyId.    Value
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (!party.PaymentTerminals.TryGetValue(Terminal.Id, out var existingTerminal))
                    return UpdateResult<Terminal>.Failed(
                               EventTrackingId,
                               Terminal,
                               $"The given terminal identification '{Terminal.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Terminal.LastUpdated <= existingTerminal.LastUpdated)
                {

                    return UpdateResult<Terminal>.Failed(
                               EventTrackingId, Terminal,
                               "The 'lastUpdated' timestamp of the new terminal must be newer then the timestamp of the existing terminal!"
                           );

                }

                #endregion


                if (party.PaymentTerminals.TryUpdate(Terminal.Id,
                                             Terminal,
                                             existingTerminal))
                {

                    Terminal.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.updateTerminal,
                              Terminal.ToJSON(
                                  true,
                                  true,
                                  CustomTerminalSerializer,
                                  CustomDisplayTextSerializer,
                                  CustomImageSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnPaymentTerminalChangedLocal = OnPaymentTerminalChanged;
                        if (OnPaymentTerminalChangedLocal is not null)
                        {
                            try
                            {
                                OnPaymentTerminalChangedLocal(Terminal).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdatePaymentTerminal), " ", nameof(OnPaymentTerminalChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return UpdateResult<Terminal>.Success(
                               EventTrackingId,
                               Terminal
                           );

                }

                return UpdateResult<Terminal>.Failed(
                           EventTrackingId,
                           Terminal,
                           "Terminals.TryUpdate(Terminal.Id, Terminal, Terminal) failed!"
                       );

            }

            return UpdateResult<Terminal>.Failed(
                       EventTrackingId,
                       Terminal,
                       $"The party identification '{partyId}' of the terminal is unknown!"
                   );

        }

        #endregion

        #region TryPatchPaymentTerminal       (Terminal, TerminalPatch, AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<PatchResult<Terminal>>

            TryPatchPaymentTerminal(Party_Idv3         PartyId,
                                    Terminal_Id        TerminalId,
                                    JObject            TerminalPatch,
                                    Boolean?           AllowDowngrades     = false,
                                    Boolean            SkipNotifications   = false,
                                    EventTracking_Id?  EventTrackingId     = null,
                                    User_Id?           CurrentUserId       = null,
                                    CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.PaymentTerminals.TryGetValue(TerminalId, out var existingTerminal))
                {

                    var patchResult = existingTerminal.TryPatch(
                                          TerminalPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                if (patchResult.IsSuccessAndDataNotNull(out var data))
                {

                    var updateTerminalResult = await UpdatePaymentTerminal(
                                                         data,
                                                         AllowDowngrades,
                                                         SkipNotifications,
                                                         EventTrackingId,
                                                         CurrentUserId,
                                                         CancellationToken
                                                     );

                    if (updateTerminalResult.IsFailed)
                        return PatchResult<Terminal>.Failed(
                                    EventTrackingId,
                                    existingTerminal,
                                    "Could not update the terminal: " + updateTerminalResult.ErrorResponse
                                );

                }

                return patchResult;

                }

                return PatchResult<Terminal>.Failed(
                           EventTrackingId,
                           $"The given terminal '{TerminalId}' is unknown!"
                       );

            }

            return PatchResult<Terminal>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the terminal is unknown!"
                   );

        }

        #endregion


        #region PaymentTerminalExists         (PartyId, TerminalId)

        public Boolean PaymentTerminalExists(Party_Idv3   PartyId,
                                             Terminal_Id  TerminalId)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.PaymentTerminals.ContainsKey(TerminalId);

            return false;

        }

        #endregion

        #region TryGetPaymentTerminal         (PartyId, TerminalId, out Terminal)

        public Boolean TryGetPaymentTerminal(Party_Idv3                        PartyId,
                                             Terminal_Id                        TerminalId,
                                             [NotNullWhen(true)] out Terminal?  Terminal)
        {

            if (parties.        TryGetValue(PartyId,    out var party) &&
                party.PaymentTerminals.TryGetValue(TerminalId, out Terminal))
            {
                return true;
            }

            Terminal = null;
            return false;

        }

        #endregion

        #region GetPaymentTerminals           (IncludeTerminal)

        public IEnumerable<Terminal> GetPaymentTerminals(Func<Terminal, Boolean> IncludeTerminal)
        {

            var terminals = new List<Terminal>();

            foreach (var party in parties.Values)
            {
                foreach (var terminal in party.PaymentTerminals.Values)
                {
                    if (IncludeTerminal(terminal))
                        terminals.Add(terminal);
                }
            }

            return terminals;

        }

        #endregion

        #region GetPaymentTerminals           (PartyId = null)

        public IEnumerable<Terminal> GetPaymentTerminals(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.PaymentTerminals.Values;
            }

            else
            {

                var terminals = new List<Terminal>();

                foreach (var party in parties.Values)
                    terminals.AddRange(party.PaymentTerminals.Values);

                return terminals;

            }

            return [];

        }

        #endregion

        #endregion

        #region Bookings

        #region Events

        public delegate Task OnBookingAddedDelegate  (Booking Booking);

        public event OnBookingAddedDelegate?    OnBookingAdded;


        public delegate Task OnBookingChangedDelegate(Booking Booking);

        public event OnBookingChangedDelegate?  OnBookingChanged;

        #endregion


        public delegate Task<Booking> OnBookingSlowStorageLookupDelegate(Party_Idv3  PartyId,
                                                                         Booking_Id  BookingId);

        public event OnBookingSlowStorageLookupDelegate? OnBookingSlowStorageLookup;


        #region AddBooking            (Booking, ...)

        public async Task<AddResult<Booking>>

            AddBooking(Booking            Booking,
                       Boolean            SkipNotifications   = false,
                       EventTracking_Id?  EventTrackingId     = null,
                       User_Id?           CurrentUserId       = null,
                       CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Booking.CountryCode,
                                    Booking.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Bookings.TryAdd(Booking.Id, Booking))
                {

                    Booking.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addBooking,
                              Booking.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingSerializer,
                                  //CustomBookingTokenSerializer,
                                  //CustomBookingLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnBookingAddedLocal = OnBookingAdded;
                        if (OnBookingAddedLocal is not null)
                        {
                            try
                            {
                                await OnBookingAddedLocal(Booking);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddBooking), " ", nameof(OnBookingAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return AddResult<Booking>.Success(
                               EventTrackingId,
                               Booking
                           );

                }

                return AddResult<Booking>.Failed(
                           EventTrackingId,
                           Booking,
                           "The given charge detail record already exists!"
                       );

            }

            return AddResult<Booking>.Failed(
                       EventTrackingId,
                       Booking,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddBookingIfNotExists (Booking, ...)

        public async Task<AddResult<Booking>>

            AddBookingIfNotExists(Booking            Booking,
                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Booking.CountryCode,
                                    Booking.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Bookings.TryAdd(Booking.Id, Booking))
                {

                    Booking.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addBookingIfNotExists,
                              Booking.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingSerializer,
                                  //CustomBookingTokenSerializer,
                                  //CustomBookingLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnBookingAddedLocal = OnBookingAdded;
                        if (OnBookingAddedLocal is not null)
                        {
                            try
                            {
                                await OnBookingAddedLocal(Booking);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddBooking), " ", nameof(OnBookingAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return AddResult<Booking>.Success(
                               EventTrackingId,
                               Booking
                           );

                }

                return AddResult<Booking>.NoOperation(
                           EventTrackingId,
                           Booking,
                           "The given charge detail record already exists."
                       );

            }

            return AddResult<Booking>.Failed(
                       EventTrackingId,
                       Booking,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateBooking    (Booking, AllowDowngrades = false, ...)

        public async Task<AddOrUpdateResult<Booking>>

            AddOrUpdateBooking(Booking            Booking,
                               Boolean?           AllowDowngrades     = false,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Booking.CountryCode,
                                    Booking.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                #region Update an existing charge detail record

                if (party.Bookings.TryGetValue(Booking.Id, out var existingBooking))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        Booking.LastUpdated <= existingBooking.LastUpdated)
                    {
                        return AddOrUpdateResult<Booking>.Failed(
                                   EventTrackingId,
                                   Booking,
                                   "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!"
                               );
                    }

                    //if (Booking.LastUpdated.ToISO8601() == existingBooking.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<Booking>.NoOperation(Booking,
                    //                                                   "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");

                    var aa = existingBooking.Equals(existingBooking);

                    if (party.Bookings.TryUpdate(Booking.Id,
                                             Booking,
                                             existingBooking))
                    {

                        Booking.CommonAPI = this;

                        await LogAsset(
                                  CommonHTTPAPI.addOrUpdateBooking,
                                  Booking.ToJSON(
                                      //true,
                                      //true,
                                      //true,
                                      //true,
                                      //CustomBookingSerializer,
                                      //CustomBookingTokenSerializer,
                                      //CustomBookingLocationSerializer,
                                      //CustomEVSEEnergyMeterSerializer,
                                      //CustomTransparencySoftwareSerializer,
                                      //CustomTariffSerializer,
                                      //CustomDisplayTextSerializer,
                                      //CustomPriceSerializer,
                                      //CustomPriceLimitSerializer,
                                      //CustomTariffElementSerializer,
                                      //CustomPriceComponentSerializer,
                                      //CustomTaxAmountSerializer,
                                      //CustomTariffRestrictionsSerializer,
                                      //CustomEnergyMixSerializer,
                                      //CustomEnergySourceSerializer,
                                      //CustomEnvironmentalImpactSerializer,
                                      //CustomChargingPeriodSerializer,
                                      //CustomBookingDimensionSerializer,
                                      //CustomSignedDataSerializer,
                                      //CustomSignedValueSerializer
                                  ),
                                  EventTrackingId,
                                  CurrentUserId,
                                  CancellationToken
                              );

                        if (!SkipNotifications)
                        {

                            var OnBookingChangedLocal = OnBookingChanged;
                            if (OnBookingChangedLocal is not null)
                            {
                                try
                                {
                                    OnBookingChangedLocal(Booking).Wait(CancellationToken);
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateBooking), " ", nameof(OnBookingChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                        }

                        return AddOrUpdateResult<Booking>.Updated(
                                   EventTrackingId,
                                   Booking
                               );

                    }

                    return AddOrUpdateResult<Booking>.Failed(
                               EventTrackingId,
                               Booking,
                               "Updating the given charge detail record failed!"
                           );

                }

                #endregion

                #region Add a new charge detail record

                if (party.Bookings.TryAdd(Booking.Id, Booking))
                {

                    Booking.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateBooking,
                              Booking.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingSerializer,
                                  //CustomBookingTokenSerializer,
                                  //CustomBookingLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnBookingAddedLocal = OnBookingAdded;
                        if (OnBookingAddedLocal is not null)
                        {
                            try
                            {
                                OnBookingAddedLocal(Booking).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateBooking), " ", nameof(OnBookingAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return AddOrUpdateResult<Booking>.Created(
                               EventTrackingId,
                               Booking
                           );

                }

                #endregion

                return AddOrUpdateResult<Booking>.Failed(
                           EventTrackingId,
                           Booking,
                           "Adding the given charge detail record failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<Booking>.Failed(
                       EventTrackingId,
                       Booking,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region UpdateBooking         (Booking, AllowDowngrades = false, ...)

        public async Task<UpdateResult<Booking>>

            UpdateBooking(Booking            Booking,
                          Boolean?           AllowDowngrades     = false,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Booking.CountryCode,
                                    Booking.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (!party.Bookings.TryGetValue(Booking.Id, out var existingBooking))
                    return UpdateResult<Booking>.Failed(
                               EventTrackingId,
                               Booking,
                               $"The given charge detail record identification '{Booking.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Booking.LastUpdated <= existingBooking.LastUpdated)
                {

                    return UpdateResult<Booking>.Failed(
                               EventTrackingId, Booking,
                               "The 'lastUpdated' timestamp of the new charging charge detail record must be newer then the timestamp of the existing charge detail record!"
                           );

                }

                #endregion


                if (party.Bookings.TryUpdate(Booking.Id,
                                         Booking,
                                         existingBooking))
                {

                    Booking.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.updateBooking,
                              Booking.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingSerializer,
                                  //CustomBookingTokenSerializer,
                                  //CustomBookingLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnBookingChangedLocal = OnBookingChanged;
                        if (OnBookingChangedLocal is not null)
                        {
                            try
                            {
                                OnBookingChangedLocal(Booking).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateBooking), " ", nameof(OnBookingChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return UpdateResult<Booking>.Success(
                               EventTrackingId,
                               Booking
                           );

                }

                return UpdateResult<Booking>.Failed(
                           EventTrackingId,
                           Booking,
                           "charge detail records.TryUpdate(Booking.Id, Booking, Booking) failed!"
                       );

            }

            return UpdateResult<Booking>.Failed(
                       EventTrackingId,
                       Booking,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveBooking         (Booking, ...)

        public async Task<RemoveResult<Booking>>

            RemoveBooking(Booking            Booking,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    Booking.CountryCode,
                                    Booking.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.Bookings.TryRemove(Booking.Id, out var cdr))
                {

                    await LogAsset(
                              CommonHTTPAPI.removeBooking,
                              cdr.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingSerializer,
                                  //CustomBookingTokenSerializer,
                                  //CustomBookingLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    return RemoveResult<Booking>.Success(
                               EventTrackingId,
                               cdr
                           );

                }

                return RemoveResult<Booking>.Failed(
                           EventTrackingId,
                           Booking,
                           "The charge detail record identification of the charge detail record is unknown!"
                       );

            }

            return RemoveResult<Booking>.Failed(
                       EventTrackingId,
                       Booking,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveBooking         (PartyId, BookingId, ...)

        public async Task<RemoveResult<Booking>>

            RemoveBooking(Party_Idv3         PartyId,
                          Booking_Id         BookingId,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Bookings.TryRemove(BookingId, out var cdr))
                {

                    await LogAsset(
                              CommonHTTPAPI.removeBooking,
                              cdr.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingSerializer,
                                  //CustomBookingTokenSerializer,
                                  //CustomBookingLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    return RemoveResult<Booking>.Success(
                               EventTrackingId,
                               cdr
                           );

                }

                return RemoveResult<Booking>.Failed(
                           EventTrackingId,
                           "The charge detail record identification of the charge detail record is unknown!"
                       );

            }

            return RemoveResult<Booking>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveAllBookings     (IncludeBookings = null, ...)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeBookings">An optional charging charge detail record filter.</param>
        public async Task<RemoveResult<IEnumerable<Booking>>>

            RemoveAllBookings(Func<Booking, Boolean>?  IncludeBookings     = null,
                              EventTracking_Id?        EventTrackingId     = null,
                              User_Id?                 CurrentUserId       = null,
                              CancellationToken        CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedBookings = new List<Booking>();

            if (IncludeBookings is null)
            {
                foreach (var party in parties.Values)
                {
                    removedBookings.AddRange(party.Bookings.Values);
                    party.Bookings.Clear();
                }
            }

            else
            {

                foreach (var party in parties.Values)
                {
                    foreach (var cdr in party.Bookings.Values)
                    {
                        if (IncludeBookings(cdr))
                            removedBookings.Add(cdr);
                    }
                }

                foreach (var cdr in removedBookings)
                    parties[Party_Idv3.From(cdr.CountryCode, cdr.PartyId)].Bookings.TryRemove(cdr.Id, out _);

            }


            await LogAsset(
                      CommonHTTPAPI.removeAllBookings,
                      new JArray(
                          removedBookings.Select(
                              cdr => cdr.ToJSON(
                                         //true,
                                         //true,
                                         //true,
                                         //true,
                                         //CustomBookingSerializer,
                                         //CustomBookingTokenSerializer,
                                         //CustomBookingLocationSerializer,
                                         //CustomEVSEEnergyMeterSerializer,
                                         //CustomTransparencySoftwareSerializer,
                                         //CustomTariffSerializer,
                                         //CustomDisplayTextSerializer,
                                         //CustomPriceSerializer,
                                         //CustomPriceLimitSerializer,
                                         //CustomTariffElementSerializer,
                                         //CustomPriceComponentSerializer,
                                         //CustomTaxAmountSerializer,
                                         //CustomTariffRestrictionsSerializer,
                                         //CustomEnergyMixSerializer,
                                         //CustomEnergySourceSerializer,
                                         //CustomEnvironmentalImpactSerializer,
                                         //CustomChargingPeriodSerializer,
                                         //CustomBookingDimensionSerializer,
                                         //CustomSignedDataSerializer,
                                         //CustomSignedValueSerializer
                                     )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<Booking>>.Success(
                       EventTrackingId,
                       removedBookings
                   );

        }

        #endregion

        #region RemoveAllBookings     (IncludeBookingIds, ...)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeBookingIds">An optional charging charge detail record filter.</param>
        public async Task<RemoveResult<IEnumerable<Booking>>>

            RemoveAllBookings(Func<Booking_Id, Boolean>  IncludeBookingIds,
                              EventTracking_Id?          EventTrackingId     = null,
                              User_Id?                   CurrentUserId       = null,
                              CancellationToken          CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedBookings = new List<Booking>();

            foreach (var party in parties.Values)
            {
                foreach (var cdr in party.Bookings.Values)
                {
                    if (IncludeBookingIds(cdr.Id))
                        removedBookings.Add(cdr);
                }
            }

            foreach (var cdr in removedBookings)
                parties[Party_Idv3.From(cdr.CountryCode, cdr.PartyId)].Bookings.TryRemove(cdr.Id, out _);


            await LogAsset(
                      CommonHTTPAPI.removeAllBookings,
                      new JArray(
                          removedBookings.Select(
                              cdr => cdr.ToJSON(
                                         //true,
                                         //true,
                                         //true,
                                         //true,
                                         //CustomBookingSerializer,
                                         //CustomBookingTokenSerializer,
                                         //CustomBookingLocationSerializer,
                                         //CustomEVSEEnergyMeterSerializer,
                                         //CustomTransparencySoftwareSerializer,
                                         //CustomTariffSerializer,
                                         //CustomDisplayTextSerializer,
                                         //CustomPriceSerializer,
                                         //CustomPriceLimitSerializer,
                                         //CustomTariffElementSerializer,
                                         //CustomPriceComponentSerializer,
                                         //CustomTaxAmountSerializer,
                                         //CustomTariffRestrictionsSerializer,
                                         //CustomEnergyMixSerializer,
                                         //CustomEnergySourceSerializer,
                                         //CustomEnvironmentalImpactSerializer,
                                         //CustomChargingPeriodSerializer,
                                         //CustomBookingDimensionSerializer,
                                         //CustomSignedDataSerializer,
                                         //CustomSignedValueSerializer
                                     )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<Booking>>.Success(
                       EventTrackingId,
                       removedBookings
                   );

        }

        #endregion

        #region RemoveAllBookings     (PartyId, ...)

        /// <summary>
        /// Remove all charge detail records owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        public async Task<RemoveResult<IEnumerable<Booking>>>

            RemoveAllBookings(Party_Idv3         PartyId,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null,
                              CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                var removedBookings = party.Bookings.Values.ToArray();
                party.Bookings.Clear();

                await LogAsset(
                          CommonHTTPAPI.removeAllBookings,
                          new JArray(
                              removedBookings.Select(
                                  cdr => cdr.ToJSON(
                                             //true,
                                             //true,
                                             //true,
                                             //true,
                                             //CustomBookingSerializer,
                                             //CustomBookingTokenSerializer,
                                             //CustomBookingLocationSerializer,
                                             //CustomEVSEEnergyMeterSerializer,
                                             //CustomTransparencySoftwareSerializer,
                                             //CustomTariffSerializer,
                                             //CustomDisplayTextSerializer,
                                             //CustomPriceSerializer,
                                             //CustomPriceLimitSerializer,
                                             //CustomTariffElementSerializer,
                                             //CustomPriceComponentSerializer,
                                             //CustomTaxAmountSerializer,
                                             //CustomTariffRestrictionsSerializer,
                                             //CustomEnergyMixSerializer,
                                             //CustomEnergySourceSerializer,
                                             //CustomEnvironmentalImpactSerializer,
                                             //CustomChargingPeriodSerializer,
                                             //CustomBookingDimensionSerializer,
                                             //CustomSignedDataSerializer,
                                             //CustomSignedValueSerializer
                                         )
                                  )
                          ),
                          EventTrackingId,
                          CurrentUserId,
                          CancellationToken
                      );

                return RemoveResult<IEnumerable<Booking>>.Success(
                           EventTrackingId,
                           removedBookings
                       );

            }

            return RemoveResult<IEnumerable<Booking>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion


        #region BookingExists         (PartyId, BookingId)

        public Boolean BookingExists(Party_Idv3  PartyId,
                                 Booking_Id      BookingId)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.Bookings.ContainsKey(BookingId);

            return false;

        }

        #endregion

        #region TryGetBooking         (PartyId, BookingId, out Booking)

        public Boolean TryGetBooking(Party_Idv3                        PartyId,
                                     Booking_Id                        BookingId,
                                     [NotNullWhen(true)] out Booking?  Booking)
        {

            if (parties.       TryGetValue(PartyId,   out var party) &&
                party.Bookings.TryGetValue(BookingId, out Booking))
            {
                return true;
            }

            var OnBookingLookupLocal = OnBookingSlowStorageLookup;
            if (OnBookingLookupLocal is not null)
            {
                try
                {

                    var cdr = OnBookingLookupLocal(
                                    PartyId,
                                    BookingId
                                ).Result;

                    if (cdr is not null)
                    {
                        Booking = cdr;
                        return true;
                    }

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetBooking), " ", nameof(OnBookingSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }


            Booking = null;
            return false;

        }

        #endregion

        #region GetBookings           (IncludeBooking)

        public IEnumerable<Booking> GetBookings(Func<Booking, Boolean> IncludeBooking)
        {

            var sessions = new List<Booking>();

            foreach (var party in parties.Values)
            {
                foreach (var cdr in party.Bookings.Values)
                {
                    if (IncludeBooking(cdr))
                        sessions.Add(cdr);
                }
            }

            return sessions;

        }

        #endregion

        #region GetBookings           (PartyId = null)

        public IEnumerable<Booking> GetBookings(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Bookings.Values;
            }

            else
            {

                var sessions = new List<Booking>();

                foreach (var party in parties.Values)
                    sessions.AddRange(party.Bookings.Values);

                return sessions;

            }

            return [];

        }

        #endregion

        #endregion

        #region BookingLocations

        #region Events

        public delegate Task OnBookingLocationAddedDelegate  (BookingLocation BookingLocation);

        public event OnBookingLocationAddedDelegate?    OnBookingLocationAdded;


        public delegate Task OnBookingLocationChangedDelegate(BookingLocation BookingLocation);

        public event OnBookingLocationChangedDelegate?  OnBookingLocationChanged;

        #endregion


        public delegate Task<BookingLocation> OnBookingLocationSlowStorageLookupDelegate(Party_Idv3          PartyId,
                                                                                         BookingLocation_Id  BookingLocationId);

        public event OnBookingLocationSlowStorageLookupDelegate? OnBookingLocationSlowStorageLookup;


        #region AddBookingLocation            (BookingLocation, ...)

        public async Task<AddResult<BookingLocation>>

            AddBookingLocation(BookingLocation    BookingLocation,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    BookingLocation.CountryCode,
                                    BookingLocation.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.BookingLocations.TryAdd(BookingLocation.Id, BookingLocation))
                {

                    BookingLocation.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addBookingLocation,
                              BookingLocation.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingLocationSerializer,
                                  //CustomBookingLocationTokenSerializer,
                                  //CustomBookingLocationLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingLocationDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnBookingLocationAddedLocal = OnBookingLocationAdded;
                        if (OnBookingLocationAddedLocal is not null)
                        {
                            try
                            {
                                await OnBookingLocationAddedLocal(BookingLocation);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddBookingLocation), " ", nameof(OnBookingLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return AddResult<BookingLocation>.Success(
                               EventTrackingId,
                               BookingLocation
                           );

                }

                return AddResult<BookingLocation>.Failed(
                           EventTrackingId,
                           BookingLocation,
                           "The given charge detail record already exists!"
                       );

            }

            return AddResult<BookingLocation>.Failed(
                       EventTrackingId,
                       BookingLocation,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddBookingLocationIfNotExists (BookingLocation, ...)

        public async Task<AddResult<BookingLocation>>

            AddBookingLocationIfNotExists(BookingLocation    BookingLocation,
                                          Boolean            SkipNotifications   = false,
                                          EventTracking_Id?  EventTrackingId     = null,
                                          User_Id?           CurrentUserId       = null,
                                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    BookingLocation.CountryCode,
                                    BookingLocation.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.BookingLocations.TryAdd(BookingLocation.Id, BookingLocation))
                {

                    BookingLocation.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addBookingLocationIfNotExists,
                              BookingLocation.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingLocationSerializer,
                                  //CustomBookingLocationTokenSerializer,
                                  //CustomBookingLocationLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingLocationDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnBookingLocationAddedLocal = OnBookingLocationAdded;
                        if (OnBookingLocationAddedLocal is not null)
                        {
                            try
                            {
                                await OnBookingLocationAddedLocal(BookingLocation);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddBookingLocation), " ", nameof(OnBookingLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return AddResult<BookingLocation>.Success(
                               EventTrackingId,
                               BookingLocation
                           );

                }

                return AddResult<BookingLocation>.NoOperation(
                           EventTrackingId,
                           BookingLocation,
                           "The given charge detail record already exists."
                       );

            }

            return AddResult<BookingLocation>.Failed(
                       EventTrackingId,
                       BookingLocation,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateBookingLocation    (BookingLocation, AllowDowngrades = false, ...)

        public async Task<AddOrUpdateResult<BookingLocation>>

            AddOrUpdateBookingLocation(BookingLocation    BookingLocation,
                                       Boolean?           AllowDowngrades     = false,
                                       Boolean            SkipNotifications   = false,
                                       EventTracking_Id?  EventTrackingId     = null,
                                       User_Id?           CurrentUserId       = null,
                                       CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    BookingLocation.CountryCode,
                                    BookingLocation.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                #region Update an existing charge detail record

                if (party.BookingLocations.TryGetValue(BookingLocation.Id, out var existingBookingLocation))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        BookingLocation.LastUpdated <= existingBookingLocation.LastUpdated)
                    {
                        return AddOrUpdateResult<BookingLocation>.Failed(
                                   EventTrackingId,
                                   BookingLocation,
                                   "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!"
                               );
                    }

                    //if (BookingLocation.LastUpdated.ToISO8601() == existingBookingLocation.LastUpdated.ToISO8601())
                    //    return AddOrUpdateResult<BookingLocation>.NoOperation(BookingLocation,
                    //                                                   "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");

                    var aa = existingBookingLocation.Equals(existingBookingLocation);

                    if (party.BookingLocations.TryUpdate(BookingLocation.Id,
                                             BookingLocation,
                                             existingBookingLocation))
                    {

                        BookingLocation.CommonAPI = this;

                        await LogAsset(
                                  CommonHTTPAPI.addOrUpdateBookingLocation,
                                  BookingLocation.ToJSON(
                                      //true,
                                      //true,
                                      //true,
                                      //true,
                                      //CustomBookingLocationSerializer,
                                      //CustomBookingLocationTokenSerializer,
                                      //CustomBookingLocationLocationSerializer,
                                      //CustomEVSEEnergyMeterSerializer,
                                      //CustomTransparencySoftwareSerializer,
                                      //CustomTariffSerializer,
                                      //CustomDisplayTextSerializer,
                                      //CustomPriceSerializer,
                                      //CustomPriceLimitSerializer,
                                      //CustomTariffElementSerializer,
                                      //CustomPriceComponentSerializer,
                                      //CustomTaxAmountSerializer,
                                      //CustomTariffRestrictionsSerializer,
                                      //CustomEnergyMixSerializer,
                                      //CustomEnergySourceSerializer,
                                      //CustomEnvironmentalImpactSerializer,
                                      //CustomChargingPeriodSerializer,
                                      //CustomBookingLocationDimensionSerializer,
                                      //CustomSignedDataSerializer,
                                      //CustomSignedValueSerializer
                                  ),
                                  EventTrackingId,
                                  CurrentUserId,
                                  CancellationToken
                              );

                        if (!SkipNotifications)
                        {

                            var OnBookingLocationChangedLocal = OnBookingLocationChanged;
                            if (OnBookingLocationChangedLocal is not null)
                            {
                                try
                                {
                                    OnBookingLocationChangedLocal(BookingLocation).Wait(CancellationToken);
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateBookingLocation), " ", nameof(OnBookingLocationChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                        }

                        return AddOrUpdateResult<BookingLocation>.Updated(
                                   EventTrackingId,
                                   BookingLocation
                               );

                    }

                    return AddOrUpdateResult<BookingLocation>.Failed(
                               EventTrackingId,
                               BookingLocation,
                               "Updating the given charge detail record failed!"
                           );

                }

                #endregion

                #region Add a new charge detail record

                if (party.BookingLocations.TryAdd(BookingLocation.Id, BookingLocation))
                {

                    BookingLocation.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateBookingLocation,
                              BookingLocation.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingLocationSerializer,
                                  //CustomBookingLocationTokenSerializer,
                                  //CustomBookingLocationLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingLocationDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnBookingLocationAddedLocal = OnBookingLocationAdded;
                        if (OnBookingLocationAddedLocal is not null)
                        {
                            try
                            {
                                OnBookingLocationAddedLocal(BookingLocation).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateBookingLocation), " ", nameof(OnBookingLocationAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return AddOrUpdateResult<BookingLocation>.Created(
                               EventTrackingId,
                               BookingLocation
                           );

                }

                #endregion

                return AddOrUpdateResult<BookingLocation>.Failed(
                           EventTrackingId,
                           BookingLocation,
                           "Adding the given charge detail record failed because of concurrency issues!"
                       );

            }

            return AddOrUpdateResult<BookingLocation>.Failed(
                       EventTrackingId,
                       BookingLocation,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region UpdateBookingLocation         (BookingLocation, AllowDowngrades = false, ...)

        public async Task<UpdateResult<BookingLocation>>

            UpdateBookingLocation(BookingLocation    BookingLocation,
                                  Boolean?           AllowDowngrades     = false,
                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    BookingLocation.CountryCode,
                                    BookingLocation.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (!party.BookingLocations.TryGetValue(BookingLocation.Id, out var existingBookingLocation))
                    return UpdateResult<BookingLocation>.Failed(
                               EventTrackingId,
                               BookingLocation,
                               $"The given charge detail record identification '{BookingLocation.Id}' is unknown!"
                           );

                #region Validate AllowDowngrades

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    BookingLocation.LastUpdated <= existingBookingLocation.LastUpdated)
                {

                    return UpdateResult<BookingLocation>.Failed(
                               EventTrackingId, BookingLocation,
                               "The 'lastUpdated' timestamp of the new charging charge detail record must be newer then the timestamp of the existing charge detail record!"
                           );

                }

                #endregion


                if (party.BookingLocations.TryUpdate(BookingLocation.Id,
                                         BookingLocation,
                                         existingBookingLocation))
                {

                    BookingLocation.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.updateBookingLocation,
                              BookingLocation.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingLocationSerializer,
                                  //CustomBookingLocationTokenSerializer,
                                  //CustomBookingLocationLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingLocationDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    if (!SkipNotifications)
                    {

                        var OnBookingLocationChangedLocal = OnBookingLocationChanged;
                        if (OnBookingLocationChangedLocal is not null)
                        {
                            try
                            {
                                OnBookingLocationChangedLocal(BookingLocation).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateBookingLocation), " ", nameof(OnBookingLocationChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return UpdateResult<BookingLocation>.Success(
                               EventTrackingId,
                               BookingLocation
                           );

                }

                return UpdateResult<BookingLocation>.Failed(
                           EventTrackingId,
                           BookingLocation,
                           "charge detail records.TryUpdate(BookingLocation.Id, BookingLocation, BookingLocation) failed!"
                       );

            }

            return UpdateResult<BookingLocation>.Failed(
                       EventTrackingId,
                       BookingLocation,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveBookingLocation         (BookingLocation, ...)

        public async Task<RemoveResult<BookingLocation>>

            RemoveBookingLocation(BookingLocation    BookingLocation,
                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var partyId       = Party_Idv3.From(
                                    BookingLocation.CountryCode,
                                    BookingLocation.PartyId
                                );

            if (parties.TryGetValue(partyId, out var party))
            {

                if (party.BookingLocations.TryRemove(BookingLocation.Id, out var cdr))
                {

                    await LogAsset(
                              CommonHTTPAPI.removeBookingLocation,
                              cdr.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingLocationSerializer,
                                  //CustomBookingLocationTokenSerializer,
                                  //CustomBookingLocationLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingLocationDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    return RemoveResult<BookingLocation>.Success(
                               EventTrackingId,
                               cdr
                           );

                }

                return RemoveResult<BookingLocation>.Failed(
                           EventTrackingId,
                           BookingLocation,
                           "The charge detail record identification of the charge detail record is unknown!"
                       );

            }

            return RemoveResult<BookingLocation>.Failed(
                       EventTrackingId,
                       BookingLocation,
                       $"The party identification '{partyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveBookingLocation         (PartyId, BookingLocationId, ...)

        public async Task<RemoveResult<BookingLocation>>

            RemoveBookingLocation(Party_Idv3          PartyId,
                                  BookingLocation_Id  BookingLocationId,
                                  Boolean             SkipNotifications   = false,
                                  EventTracking_Id?   EventTrackingId     = null,
                                  User_Id?            CurrentUserId       = null,
                                  CancellationToken   CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.BookingLocations.TryRemove(BookingLocationId, out var cdr))
                {

                    await LogAsset(
                              CommonHTTPAPI.removeBookingLocation,
                              cdr.ToJSON(
                                  //true,
                                  //true,
                                  //true,
                                  //true,
                                  //CustomBookingLocationSerializer,
                                  //CustomBookingLocationTokenSerializer,
                                  //CustomBookingLocationLocationSerializer,
                                  //CustomEVSEEnergyMeterSerializer,
                                  //CustomTransparencySoftwareSerializer,
                                  //CustomTariffSerializer,
                                  //CustomDisplayTextSerializer,
                                  //CustomPriceSerializer,
                                  //CustomPriceLimitSerializer,
                                  //CustomTariffElementSerializer,
                                  //CustomPriceComponentSerializer,
                                  //CustomTaxAmountSerializer,
                                  //CustomTariffRestrictionsSerializer,
                                  //CustomEnergyMixSerializer,
                                  //CustomEnergySourceSerializer,
                                  //CustomEnvironmentalImpactSerializer,
                                  //CustomChargingPeriodSerializer,
                                  //CustomBookingLocationDimensionSerializer,
                                  //CustomSignedDataSerializer,
                                  //CustomSignedValueSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    return RemoveResult<BookingLocation>.Success(
                               EventTrackingId,
                               cdr
                           );

                }

                return RemoveResult<BookingLocation>.Failed(
                           EventTrackingId,
                           "The charge detail record identification of the charge detail record is unknown!"
                       );

            }

            return RemoveResult<BookingLocation>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveAllBookingLocations     (IncludeBookingLocations = null, ...)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeBookingLocations">An optional charging charge detail record filter.</param>
        public async Task<RemoveResult<IEnumerable<BookingLocation>>>

            RemoveAllBookingLocations(Func<BookingLocation, Boolean>?  IncludeBookingLocations   = null,
                                      EventTracking_Id?                EventTrackingId           = null,
                                      User_Id?                         CurrentUserId             = null,
                                      CancellationToken                CancellationToken         = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedBookingLocations = new List<BookingLocation>();

            if (IncludeBookingLocations is null)
            {
                foreach (var party in parties.Values)
                {
                    removedBookingLocations.AddRange(party.BookingLocations.Values);
                    party.BookingLocations.Clear();
                }
            }

            else
            {

                foreach (var party in parties.Values)
                {
                    foreach (var cdr in party.BookingLocations.Values)
                    {
                        if (IncludeBookingLocations(cdr))
                            removedBookingLocations.Add(cdr);
                    }
                }

                foreach (var cdr in removedBookingLocations)
                    parties[Party_Idv3.From(cdr.CountryCode, cdr.PartyId)].BookingLocations.TryRemove(cdr.Id, out _);

            }


            await LogAsset(
                      CommonHTTPAPI.removeAllBookingLocations,
                      new JArray(
                          removedBookingLocations.Select(
                              cdr => cdr.ToJSON(
                                         //true,
                                         //true,
                                         //true,
                                         //true,
                                         //CustomBookingLocationSerializer,
                                         //CustomBookingLocationTokenSerializer,
                                         //CustomBookingLocationLocationSerializer,
                                         //CustomEVSEEnergyMeterSerializer,
                                         //CustomTransparencySoftwareSerializer,
                                         //CustomTariffSerializer,
                                         //CustomDisplayTextSerializer,
                                         //CustomPriceSerializer,
                                         //CustomPriceLimitSerializer,
                                         //CustomTariffElementSerializer,
                                         //CustomPriceComponentSerializer,
                                         //CustomTaxAmountSerializer,
                                         //CustomTariffRestrictionsSerializer,
                                         //CustomEnergyMixSerializer,
                                         //CustomEnergySourceSerializer,
                                         //CustomEnvironmentalImpactSerializer,
                                         //CustomChargingPeriodSerializer,
                                         //CustomBookingLocationDimensionSerializer,
                                         //CustomSignedDataSerializer,
                                         //CustomSignedValueSerializer
                                     )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<BookingLocation>>.Success(
                       EventTrackingId,
                       removedBookingLocations
                   );

        }

        #endregion

        #region RemoveAllBookingLocations     (IncludeBookingLocationIds, ...)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeBookingLocationIds">An optional charging charge detail record filter.</param>
        public async Task<RemoveResult<IEnumerable<BookingLocation>>>

            RemoveAllBookingLocations(Func<BookingLocation_Id, Boolean>  IncludeBookingLocationIds,
                                      EventTracking_Id?                  EventTrackingId     = null,
                                      User_Id?                           CurrentUserId       = null,
                                      CancellationToken                  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedBookingLocations = new List<BookingLocation>();

            foreach (var party in parties.Values)
            {
                foreach (var cdr in party.BookingLocations.Values)
                {
                    if (IncludeBookingLocationIds(cdr.Id))
                        removedBookingLocations.Add(cdr);
                }
            }

            foreach (var cdr in removedBookingLocations)
                parties[Party_Idv3.From(cdr.CountryCode, cdr.PartyId)].BookingLocations.TryRemove(cdr.Id, out _);


            await LogAsset(
                      CommonHTTPAPI.removeAllBookingLocations,
                      new JArray(
                          removedBookingLocations.Select(
                              cdr => cdr.ToJSON(
                                         //true,
                                         //true,
                                         //true,
                                         //true,
                                         //CustomBookingLocationSerializer,
                                         //CustomBookingLocationTokenSerializer,
                                         //CustomBookingLocationLocationSerializer,
                                         //CustomEVSEEnergyMeterSerializer,
                                         //CustomTransparencySoftwareSerializer,
                                         //CustomTariffSerializer,
                                         //CustomDisplayTextSerializer,
                                         //CustomPriceSerializer,
                                         //CustomPriceLimitSerializer,
                                         //CustomTariffElementSerializer,
                                         //CustomPriceComponentSerializer,
                                         //CustomTaxAmountSerializer,
                                         //CustomTariffRestrictionsSerializer,
                                         //CustomEnergyMixSerializer,
                                         //CustomEnergySourceSerializer,
                                         //CustomEnvironmentalImpactSerializer,
                                         //CustomChargingPeriodSerializer,
                                         //CustomBookingLocationDimensionSerializer,
                                         //CustomSignedDataSerializer,
                                         //CustomSignedValueSerializer
                                     )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<BookingLocation>>.Success(
                       EventTrackingId,
                       removedBookingLocations
                   );

        }

        #endregion

        #region RemoveAllBookingLocations     (PartyId, ...)

        /// <summary>
        /// Remove all charge detail records owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        public async Task<RemoveResult<IEnumerable<BookingLocation>>>

            RemoveAllBookingLocations(Party_Idv3         PartyId,
                                      EventTracking_Id?  EventTrackingId     = null,
                                      User_Id?           CurrentUserId       = null,
                                      CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                var removedBookingLocations = party.BookingLocations.Values.ToArray();
                party.BookingLocations.Clear();

                await LogAsset(
                          CommonHTTPAPI.removeAllBookingLocations,
                          new JArray(
                              removedBookingLocations.Select(
                                  cdr => cdr.ToJSON(
                                             //true,
                                             //true,
                                             //true,
                                             //true,
                                             //CustomBookingLocationSerializer,
                                             //CustomBookingLocationTokenSerializer,
                                             //CustomBookingLocationLocationSerializer,
                                             //CustomEVSEEnergyMeterSerializer,
                                             //CustomTransparencySoftwareSerializer,
                                             //CustomTariffSerializer,
                                             //CustomDisplayTextSerializer,
                                             //CustomPriceSerializer,
                                             //CustomPriceLimitSerializer,
                                             //CustomTariffElementSerializer,
                                             //CustomPriceComponentSerializer,
                                             //CustomTaxAmountSerializer,
                                             //CustomTariffRestrictionsSerializer,
                                             //CustomEnergyMixSerializer,
                                             //CustomEnergySourceSerializer,
                                             //CustomEnvironmentalImpactSerializer,
                                             //CustomChargingPeriodSerializer,
                                             //CustomBookingLocationDimensionSerializer,
                                             //CustomSignedDataSerializer,
                                             //CustomSignedValueSerializer
                                         )
                                  )
                          ),
                          EventTrackingId,
                          CurrentUserId,
                          CancellationToken
                      );

                return RemoveResult<IEnumerable<BookingLocation>>.Success(
                           EventTrackingId,
                           removedBookingLocations
                       );

            }

            return RemoveResult<IEnumerable<BookingLocation>>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion


        #region BookingLocationExists         (PartyId, BookingLocationId)

        public Boolean BookingLocationExists(Party_Idv3          PartyId,
                                             BookingLocation_Id  BookingLocationId)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.BookingLocations.ContainsKey(BookingLocationId);

            return false;

        }

        #endregion

        #region TryGetBookingLocation         (PartyId, BookingLocationId, out BookingLocation)

        public Boolean TryGetBookingLocation(Party_Idv3                                PartyId,
                                             BookingLocation_Id                        BookingLocationId,
                                             [NotNullWhen(true)] out BookingLocation?  BookingLocation)
        {

            if (parties.       TryGetValue(PartyId,   out var party) &&
                party.BookingLocations.TryGetValue(BookingLocationId, out BookingLocation))
            {
                return true;
            }

            var OnBookingLocationLookupLocal = OnBookingLocationSlowStorageLookup;
            if (OnBookingLocationLookupLocal is not null)
            {
                try
                {

                    var cdr = OnBookingLocationLookupLocal(
                                    PartyId,
                                    BookingLocationId
                                ).Result;

                    if (cdr is not null)
                    {
                        BookingLocation = cdr;
                        return true;
                    }

                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetBookingLocation), " ", nameof(OnBookingLocationSlowStorageLookup), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }
            }


            BookingLocation = null;
            return false;

        }

        #endregion

        #region GetBookingLocations           (IncludeBookingLocation)

        public IEnumerable<BookingLocation> GetBookingLocations(Func<BookingLocation, Boolean> IncludeBookingLocation)
        {

            var sessions = new List<BookingLocation>();

            foreach (var party in parties.Values)
            {
                foreach (var cdr in party.BookingLocations.Values)
                {
                    if (IncludeBookingLocation(cdr))
                        sessions.Add(cdr);
                }
            }

            return sessions;

        }

        #endregion

        #region GetBookingLocations           (PartyId = null)

        public IEnumerable<BookingLocation> GetBookingLocations(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.BookingLocations.Values;
            }

            else
            {

                var sessions = new List<BookingLocation>();

                foreach (var party in parties.Values)
                    sessions.AddRange(party.BookingLocations.Values);

                return sessions;

            }

            return [];

        }

        #endregion

        #endregion



        #region (private) LogEvent (Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OICPCommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(CommonAPI),
                   Logger,
                   LogHandler,
                   EventName,
                   OICPCommand
               );

        #endregion


        public virtual Task LogException(Exception e)
        {
            DebugX.LogException(e, $"OCPI {Version.String}.{nameof(CommonAPI)}");
            return Task.CompletedTask;
        }

    }

}
