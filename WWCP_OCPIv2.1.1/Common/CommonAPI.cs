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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// A delegate for filtering remote parties.
    /// </summary>
    public delegate Boolean IncludeRemoteParty(RemoteParty RemoteParty);

    public delegate IEnumerable<Tariff>     GetTariffs2_Delegate  (CountryCode    CPOCountryCode,
                                                                   Party_Id       CPOPartyId,
                                                                   Location_Id?   LocationId       = null,
                                                                   EVSE_Id?       EVSEId           = null,
                                                                   Connector_Id?  ConnectorId      = null,
                                                                   EMSP_Id?       EMSPId           = null);


    public delegate IEnumerable<Tariff_Id>  GetTariffIds2_Delegate(CountryCode    CPOCountryCode,
                                                                   Party_Id       CPOPartyId,
                                                                   Location_Id?   LocationId       = null,
                                                                   EVSE_Id?       EVSEId           = null,
                                                                   Connector_Id?  ConnectorId      = null,
                                                                   EMSP_Id?       EMSPId           = null);

    public delegate Tariff?                 GetTariff2_Delegate   (Tariff_Id      TariffId,
                                                                   DateTimeOffset?StartTimestamp   = null,
                                                                   TimeSpan?      EVSEUId          = null);


    /// <summary>
    /// Extension methods for the Common HTTP API.
    /// </summary>
    public static class CommonAPIExtensions
    {

        #region ParseParseCountryCodePartyId (this Request,            out CountryCode, out PartyId,                                                        out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseParseCountryCodePartyId(this OCPIRequest                                Request,
                                                           [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                           [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                           [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
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

            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>   ("party_id",     Party_Id.TryParse,         out var partyId))
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


            return true;

        }

        #endregion


        #region ParseMandatoryLocation       (this Request, CommonAPI, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocation(this OCPIRequest                                Request,
                                                     CommonAPI                                       CommonAPI,
                                                     CountryCode                                     CountryCode,
                                                     Party_Id                                        PartyId,
                                                     [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                     [NotNullWhen(true)]  out Location?              Location,
                                                     [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            LocationId           = default;
            Location             = default;
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

            LocationId  = locationId;


            if (!CommonAPI.TryGetLocation(locationId, out Location) ||
                 Location.CountryCode != CountryCode                ||
                 Location.PartyId     != PartyId)
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

            return true;

        }

        #endregion

        #region ParseOptionalLocation        (this Request, CommonAPI, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocation(this OCPIRequest                                Request,
                                                    CommonAPI                                       CommonAPI,
                                                    CountryCode                                     CountryCode,
                                                    Party_Id                                        PartyId,
                                                    [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                                         out Location?              Location,
                                                    [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            LocationId           = default;
            Location             = default;
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

            LocationId  = locationId;

            CommonAPI.TryGetLocation(locationId, out Location);

            return true;

        }

        #endregion


        #region ParseMandatoryLocationEVSE   (this Request, CommonAPI, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocationEVSE(this OCPIRequest                                Request,
                                                         CommonAPI                                       CommonAPI,
                                                         CountryCode                                     CountryCode,
                                                         Party_Id                                        PartyId,
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

            LocationId  = locationId;


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

            EVSEUId     = evseUId;


            if (!CommonAPI.TryGetLocation(locationId, out Location) ||
                 Location.CountryCode != CountryCode                ||
                 Location.PartyId     != PartyId)
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

        #region ParseOptionalLocationEVSE    (this Request, CommonAPI, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocationEVSE(this OCPIRequest                                Request,
                                                        CommonAPI                                       CommonAPI,
                                                        CountryCode                                     CountryCode,
                                                        Party_Id                                        PartyId,
                                                        [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                        [NotNullWhen(true)]  out Location?              Location,
                                                        [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                                             out EVSE?                  EVSE,
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

            LocationId  = locationId;


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

            EVSEUId     = evseUId;


            if (!CommonAPI.TryGetLocation(locationId, out Location) ||
                 Location.CountryCode != CountryCode                ||
                 Location.PartyId     != PartyId)
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

            Location.TryGetEVSE(evseUId, out EVSE);

            return true;

        }

        #endregion


        #region ParseMandatoryLocationEVSEConnector   (this Request, CommonAPI, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocationEVSEConnector(this OCPIRequest                                Request,
                                                                  CommonAPI                                       CommonAPI,
                                                                  CountryCode                                     CountryCode,
                                                                  Party_Id                                        PartyId,
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

            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id> ("locationId",  Location_Id. TryParse, out var locationId))
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


            if (!Request.HTTPRequest.TryParseURLParameter<EVSE_UId>    ("evseId",      EVSE_UId.    TryParse, out var evseUId))
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


            if (!Request.HTTPRequest.TryParseURLParameter<Connector_Id>("connectorId", Connector_Id.TryParse, out var connectorId))
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


            if (!CommonAPI.TryGetLocation(locationId, out Location) ||
                 Location.CountryCode != CountryCode                ||
                 Location.PartyId     != PartyId)
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

        #region ParseOptionalLocationEVSEConnector    (this Request, CommonAPI, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="ConnectorId">The parsed unique connector identification.</param>
        /// <param name="Connector">The resolved connector.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocationEVSEConnector(this OCPIRequest                                Request,
                                                                 CommonAPI                                       CommonAPI,
                                                                 CountryCode                                     CountryCode,
                                                                 Party_Id                                        PartyId,
                                                                 [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                                 [NotNullWhen(true)]  out Location?              Location,
                                                                 [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                                 [NotNullWhen(true)]  out EVSE?                  EVSE,
                                                                 [NotNullWhen(true)]  out Connector_Id?          ConnectorId,
                                                                                      out Connector?             Connector,
                                                                 [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            LocationId           = default;
            Location             = default;
            EVSEUId              = default;
            EVSE                 = default;
            ConnectorId          = default;
            Connector            = default;
            OCPIResponseBuilder  = default;

            if (!Request.HTTPRequest.TryParseURLParameter<Location_Id> ("locationId",  Location_Id. TryParse, out var locationId))
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


            if (!Request.HTTPRequest.TryParseURLParameter<EVSE_UId>    ("evseId",      EVSE_UId.    TryParse, out var evseUId))
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


            if (!Request.HTTPRequest.TryParseURLParameter<Connector_Id>("connectorId", Connector_Id.TryParse, out var connectorId))
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


            if (!CommonAPI.TryGetLocation(locationId, out Location) ||
                 Location.CountryCode != CountryCode                ||
                 Location.PartyId     != PartyId)
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

            EVSE.TryGetConnector(connectorId, out Connector);

            return true;

        }

        #endregion



        #region ParseMandatoryTariff         (this Request, CommonAPI, out TariffId,  out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved tariff.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryTariff(this OCPIRequest                                Request,
                                                   CommonAPI                                       CommonAPI,
                                                   CountryCode                                     CountryCode,
                                                   Party_Id                                        PartyId,
                                                   [NotNullWhen(true)]  out Tariff_Id?             TariffId,
                                                   [NotNullWhen(true)]  out Tariff?                Tariff,
                                                   [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            TariffId             = default;
            Tariff               = default;
            OCPIResponseBuilder  = default;

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


            if (!CommonAPI.TryGetTariff(tariffId, out Tariff) ||
                 Tariff.CountryCode != CountryCode            ||
                 Tariff.PartyId     != PartyId)
            {

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

        #region ParseOptionalTariff          (this Request, CommonAPI, out TariffId,  out Tariff,   out OCPIResponseBuilder)

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
        public static Boolean ParseOptionalTariff(this OCPIRequest                                Request,
                                                  CommonAPI                                       CommonAPI,
                                                  CountryCode                                     CountryCode,
                                                  Party_Id                                        PartyId,
                                                  [NotNullWhen(true)]  out Tariff_Id?             TariffId,
                                                                       out Tariff?                Tariff,
                                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            TariffId             = default;
            Tariff               = default;
            OCPIResponseBuilder  = default;

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

            CommonAPI.TryGetTariff(tariffId, out Tariff);

            return true;

        }

        #endregion


        #region ParseMandatorySession        (this Request, CommonAPI, out SessionId, out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatorySession(this OCPIRequest                                Request,
                                                    CommonAPI                                       CommonAPI,
                                                    CountryCode                                     CountryCode,
                                                    Party_Id                                        PartyId,
                                                    [NotNullWhen(true)]  out Session_Id?            SessionId,
                                                    [NotNullWhen(true)]  out Session?               Session,
                                                    [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            SessionId            = default;
            Session              = default;
            OCPIResponseBuilder  = default;

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


            if (!CommonAPI.TryGetSession(SessionId.Value, out Session) ||
                 Session.CountryCode != CountryCode                    ||
                 Session.PartyId     != PartyId)
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

        #region ParseOptionalSession         (this Request, CommonAPI, out SessionId, out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalSession(this OCPIRequest                                Request,
                                                   CommonAPI                                       CommonAPI,
                                                   CountryCode                                     CountryCode,
                                                   Party_Id                                        PartyId,
                                                   [NotNullWhen(true)]  out Session_Id?            SessionId,
                                                                        out Session?               Session,
                                                   [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            SessionId            = default;
            Session              = default;
            OCPIResponseBuilder  = default;

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

            CommonAPI.TryGetSession(sessionId, out Session);

            return true;

        }

        #endregion


        #region ParseMandatoryCDR            (this Request, CommonAPI, out CDRId,     out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryCDR(this OCPIRequest                                Request,
                                                CommonAPI                                       CommonAPI,
                                                CountryCode                                     CountryCode,
                                                Party_Id                                        PartyId,
                                                [NotNullWhen(true)]  out CDR_Id?                CDRId,
                                                [NotNullWhen(true)]  out CDR?                   CDR,
                                                [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CDRId                = default;
            CDR                  = default;
            OCPIResponseBuilder  = default;

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


            if (!CommonAPI.TryGetCDR(CDRId.Value, out CDR) ||
                 CDR.CountryCode != CountryCode            ||
                 CDR.PartyId     != PartyId)
            {

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

        #region ParseOptionalCDR             (this Request, CommonAPI, out CDRId,     out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalCDR(this OCPIRequest                                Request,
                                               CommonAPI                                       CommonAPI,
                                               CountryCode                                     CountryCode,
                                               Party_Id                                        PartyId,
                                               [NotNullWhen(true)]  out CDR_Id?                CDRId,
                                                                    out CDR?                   CDR,
                                               [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CDRId                = default;
            CDR                  = default;
            OCPIResponseBuilder  = default;

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

            CommonAPI.TryGetCDR(cdrId, out CDR);

            return true;

        }

        #endregion


        #region ParseTokenId                 (this Request,                                          out TokenId,                  out OCPIResponseBuilder)

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

        #region ParseMandatoryToken          (this Request, CommonAPI, out CountryCode, out PartyId, out TokenId, out TokenStatus, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique tariff identification.</param>
        /// <param name="TokenStatus">The resolved tariff with status.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryToken(this OCPIRequest                                Request,
                                                  CommonAPI                                       CommonAPI,
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


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>("party_id",        Party_Id.TryParse,         out var partyId))
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


            if (!Request.HTTPRequest.TryParseURLParameter<Token_Id>("token_Id",        Token_Id.TryParse,         out var tokenId))
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


            if (!CommonAPI.TryGetToken(TokenId.Value, out var tokenStatus))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                                          StatusCode           = 2004,
                                          StatusMessage        = "Unknown token identification!",
                                          HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                              HTTPStatusCode             = HTTPStatusCode.NotFound,
                                              //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                              AccessControlAllowHeaders  = [ "Authorization" ]
                                          }
                                      };

                TokenStatus = null;
                return false;

            }

            TokenStatus = tokenStatus;


            if (tokenStatus.Token.CountryCode != CountryCode ||
                tokenStatus.Token.PartyId     != PartyId)
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

                TokenStatus = null;
                return false;

            }

            return true;

        }

        #endregion

        #region ParseOptionalToken           (this Request, CommonAPI, out CountryCode, out PartyId, out TokenId, out TokenStatus, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TokenId">The parsed unique tariff identification.</param>
        /// <param name="TokenStatus">The resolved tariff with status.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalToken(this OCPIRequest                                Request,
                                                 CommonAPI                                       CommonAPI,
                                                 [NotNullWhen(true)]  out CountryCode?           CountryCode,
                                                 [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                 [NotNullWhen(true)]  out Token_Id?              TokenId,
                                                                      out TokenStatus?           TokenStatus,
                                                 [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            CountryCode          = default;
            PartyId              = default;
            TokenId              = default;
            TokenStatus          = default;
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


            if (!Request.HTTPRequest.TryParseURLParameter<Party_Id>("party_id",        Party_Id.TryParse,         out var partyId))
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


            if (!Request.HTTPRequest.TryParseURLParameter<Token_Id>("token_Id",        Token_Id.TryParse,         out var tokenId))
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


            if (CommonAPI.TryGetToken(TokenId.Value, out var tokenStatus))
                TokenStatus = tokenStatus;

            return true;

        }

        #endregion


        #region ParseCommandId               (this Request, CommonAPI, out CommandId, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the command identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="CommonAPI">The EMSP API.</param>
        /// <param name="CommandId">The parsed unique command identification.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
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
    /// The Common API.
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
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails          OurBusinessDetails         { get; }

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode              OurCountryCode             { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                 OurPartyId                 { get; }

        /// <summary>
        /// Our business role.
        /// </summary>
        [Mandatory]
        public Role                     OurRole                    { get; }



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
        public CommonAPILogger?         Logger                     { get; set; }



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

        public CustomJObjectSerializerDelegate<VersionInformation>?           CustomVersionInformationSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<VersionDetail>?                CustomVersionDetailSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<VersionEndpoint>?              CustomVersionEndpointSerializer               { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                       CustomTariffSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<DisplayText>?                  CustomDisplayTextSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?                CustomTariffElementSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?               CustomPriceComponentSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?           CustomTariffRestrictionsSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                    CustomEnergyMixSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                 CustomEnergySourceSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?          CustomEnvironmentalImpactSerializer           { get; set; }


        public CustomJObjectSerializerDelegate<Session>?                      CustomSessionSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<Location>?                     CustomLocationSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?        CustomAdditionalGeoLocationSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                         CustomEVSESerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?               CustomStatusScheduleSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                    CustomConnectorSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<Location>>?        CustomLocationEnergyMeterSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?            CustomEVSEEnergyMeterSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?   CustomTransparencySoftwareStatusSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?         CustomTransparencySoftwareSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<Credentials>?                  CustomCredentialsSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?              CustomBusinessDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                        CustomHoursSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                        CustomImageSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?               CustomChargingPeriodSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                 CustomCDRDimensionSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<CDRCostDetails>?               CustomCDRCostDetailsSerializer                { get; set; }


        public CustomJObjectSerializerDelegate<Token>?                        CustomTokenSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<TokenStatus>?                  CustomTokenStatusSerializer                   { get; set; }

        public CustomJObjectSerializerDelegate<LocationReference>?            CustomLocationReferenceSerializer             { get; set; }


        public CustomJObjectSerializerDelegate<CDR>?                          CustomCDRSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                   CustomSignedDataSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                  CustomSignedValueSerializer                   { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CommonAPI using the given HTTP server.
        /// </summary>
        /// <param name="OurBusinessDetails"></param>
        /// <param name="OurCountryCode"></param>
        /// <param name="OurPartyId"></param>
        /// <param name="OurRole"></param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// 
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// 
        /// <param name="URLPathPrefix">An optional URL path prefix, used when defining URL templates.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable the log file.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        public CommonAPI(BusinessDetails              OurBusinessDetails,
                         CountryCode                  OurCountryCode,
                         Party_Id                     OurPartyId,
                         Role                         OurRole,

                         CommonHTTPAPI                BaseAPI,

                         I18NString?                  Description               = null,
                         HTTPPath?                    AdditionalURLPathPrefix   = null,
                         Func<EVSE, Boolean>?         KeepRemovedEVSEs          = null,

                         HTTPPath?                    BasePath                  = null,
                         HTTPPath?                    URLPathPrefix             = null,

                         String?                      ExternalDNSName           = null,
                         String?                      HTTPServerName            = DefaultHTTPServerName,
                         String?                      HTTPServiceName           = DefaultHTTPServiceName,
                         String?                      APIVersionHash            = null,
                         JObject?                     APIVersionHashes          = null,

                         String?                      DatabaseFilePath          = null,
                         String?                      RemotePartyDBFileName     = null,
                         String?                      AssetsDBFileName          = null,

                         Boolean?                     IsDevelopment             = false,
                         IEnumerable<String>?         DevelopmentServers        = null,
                         Boolean?                     DisableLogging            = false,
                         String?                      LoggingContext            = null,
                         String?                      LoggingPath               = null,
                         String?                      LogfileName               = null,
                         OCPILogfileCreatorDelegate?  LogfileCreator            = null)

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

            this.BaseAPI                   = BaseAPI;

            this.OurBusinessDetails        = OurBusinessDetails;
            this.OurCountryCode            = OurCountryCode;
            this.OurPartyId                = OurPartyId;
            this.OurRole                   = OurRole;

            this.KeepRemovedEVSEs          = KeepRemovedEVSEs ?? (evse => true);

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
            ReadAssetsDatabaseFile();

            RegisterURLTemplates();

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

          //  var URLPathPrefix = HTTPPath.Root;

            #region OPTIONS     ~/versions/2.1.1

            // ---------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions/2.1.1
            // ---------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.OPTIONS,
                URLPathPrefix + $"versions/{Version.Id}",
                request =>

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

            #region GET         ~/versions/2.1.1

            // ----------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions/2.1.1
            // ----------------------------------------------------------------------------
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

                    var endpoints  = new List<VersionEndpoint>() {

                                        new (
                                            Module_Id.Credentials,
                                            URL.Parse(
                                                BaseAPI.OurVersionsURL.Protocol.AsString() +
                                                (request.Host + (prefix + "credentials")).Replace("//", "/")
                                            )
                                        )

                                    };

                    #endregion


                    #region The other side is a CPO...

                    if (request.RemoteParty?.Role == Role.CPO)
                    {

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Locations,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/locations")).Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Tariffs,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/tariffs")).  Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Sessions,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/sessions")). Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.CDRs,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/cdrs")).Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Commands,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/commands")). Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Tokens,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "emsp/tokens")).   Replace("//", "/"))
                            )
                        );

                    }

                    #endregion

                    #region We are a CPO, the other side is unauthenticated and we export locations and AdHoc tariffs as Open Data...

                    if (request.RemoteParty is null &&
                        OurRole == Role.CPO)
                    {

                        if (BaseAPI.LocationsAsOpenData)
                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.Locations,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/locations")).Replace("//", "/")
                                    )
                                )
                            );

                        if (BaseAPI.TariffsAsOpenData)
                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.Tariffs,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/tariffs")).Replace("//", "/")
                                    )
                                )
                            );

                    }

                    #endregion

                    #region The other side is an EMSP...

                    if (request.RemoteParty?.Role == Role.EMSP)
                    {

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Locations,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/locations")).Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Tariffs,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/tariffs")).  Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Sessions,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/sessions")). Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.CDRs,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/cdrs")).     Replace("//", "/"))
                            )
                        );


                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Commands,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/commands")). Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Tokens,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/tokens")).   Replace("//", "/"))
                            )
                        );

                    }

                    #endregion


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


            #region OPTIONS     ~/v2.1.1/credentials

            // ------------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/v2.1.1/credentials
            // ------------------------------------------------------------
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

            #region GET         ~/v2.1.1/credentials

            // Retrieves the credentials object to access the server's platform.
            // The response contains the credentials object to access the server's platform.
            // This credentials object also contains extra information about the server such as its business details.

            // -------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.1.1/credentials
            // -------------------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.GET,
                URLPathPrefix + $"{Version.String}/credentials",
                GetCredentialsRequest,
                GetCredentialsResponse,
                request => {

                    #region Check access token... not allowed!

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
                                                              OurBusinessDetails,
                                                              OurCountryCode,
                                                              OurPartyId
                                                          ).ToJSON(
                                                                CustomCredentialsSerializer,
                                                                CustomBusinessDetailsSerializer
                                                            ),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               }
                           );

                }
            );

            #endregion

            #region POST        ~/v2.1.1/credentials

            // REGISTER new OCPI party!

            // -------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.1.1/credentials
            // -------------------------------------------------------------------------------
            this.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + $"{Version.String}/credentials",
                PostCredentialsRequest,
                PostCredentialsResponse,
                async request => {

                    if (request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                    {

                        if (request.LocalAccessInfo?.VersionsURL.HasValue == true)
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

            #region PUT         ~/v2.1.1/credentials

            // UPDATE the registration of an existing OCPI party!

            // Provides the server with updated credentials to access the client's system.
            // This credentials object also contains extra information about the client such as its business details.

            // A PUT will switch to the version that contains this credentials endpoint if it's different from the current version.
            // The server must fetch the client's endpoints again, even if the version has not changed.

            // If successful, the server must generate a new token for the client and respond with the client's updated credentials to access the server's system.
            // The credentials object in the response also contains extra information about the server such as its business details.

            // This must return a HTTP status code 405: method not allowed if the client was not registered yet.

            // -------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.1.1/credentials
            // -------------------------------------------------------------------------------
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

            #region DELETE      ~/v2.1.1/credentials

            // UNREGISTER an existing OCPI party!

            // Informs the server that its credentials to access the client's system are now invalid and can no longer be used.
            // Both parties must end any automated communication.
            // This is the unregistration process.

            // This must return a HTTP status code 405: method not allowed if the client was not registered.

            // -------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/v2.1.1/credentials
            // -------------------------------------------------------------------------------
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
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
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
                                   HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
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

            //        var result = AccessTokens.Values.Where(accessToken => accessToken.Role.Any(role => role.CountryCode == credentialsRole.CountryCode &&
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


            var commonClient              = new CommonClient(

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

            var justMySupportedVersion    = otherVersions.Data?.Where(version => version.Id == Version.Id).ToArray() ?? Array.Empty<VersionInformation>();

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

            var otherVersion2_1_1Details  = await commonClient.GetVersionDetails(Version.Id);

            #region ...or send error!

            if (otherVersion2_1_1Details.StatusCode != 1000)
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


            #region Validate, that neither the country code nor the party identification was changed!

            if (oldRemoteParty.CountryCode != receivedCredentials.CountryCode)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"Updating the country code from '{oldRemoteParty.CountryCode}' to '{receivedCredentials.CountryCode}' is not allowed!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            if (oldRemoteParty.PartyId != receivedCredentials.PartyId)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"Updating the party identification from '{oldRemoteParty.PartyId}' to '{receivedCredentials.PartyId}' is not allowed!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            var CREDENTIALS_TOKEN_C       = AccessToken.NewRandom();

            // Remove the old access token
            await RemoveAccessToken(
                      CREDENTIALS_TOKEN_A.Value
                  );

            // Store credential of the other side!
            await AddOrUpdateRemoteParty(

                      receivedCredentials.CountryCode,
                      receivedCredentials.PartyId,
                      oldRemoteParty.     Role,
                      receivedCredentials.BusinessDetails,

                      CREDENTIALS_TOKEN_C,                                      // LocalAccessToken
                      receivedCredentials.URL,                                  // RemoteVersionsURL
                      receivedCredentials.Token,                                // RemoteAccessToken
                      oldRemoteParty.RemoteAccessInfos.First().AccessTokenIsBase64Encoded,
                      oldRemoteParty.RemoteAccessInfos.First().TOTPConfig,

                      oldRemoteParty.RemoteAccessInfos.First().PreferIPv4,
                      oldRemoteParty.RemoteAccessInfos.First().RemoteCertificateValidator,
                      oldRemoteParty.RemoteAccessInfos.First().LocalCertificateSelector,
                      oldRemoteParty.RemoteAccessInfos.First().ClientCertificates,
                      oldRemoteParty.RemoteAccessInfos.First().ClientCertificateContext,
                      oldRemoteParty.RemoteAccessInfos.First().ClientCertificateChain,
                      oldRemoteParty.RemoteAccessInfos.First().TLSProtocols,
                      oldRemoteParty.RemoteAccessInfos.First().ContentType,
                      oldRemoteParty.RemoteAccessInfos.First().Accept,
                      oldRemoteParty.RemoteAccessInfos.First().HTTPUserAgent,
                      oldRemoteParty.RemoteAccessInfos.First().RequestTimeout,
                      oldRemoteParty.RemoteAccessInfos.First().TransmissionRetryDelay,
                      oldRemoteParty.RemoteAccessInfos.First().MaxNumberOfRetries,
                      oldRemoteParty.RemoteAccessInfos.First().InternalBufferSize,
                      oldRemoteParty.RemoteAccessInfos.First().UseHTTPPipelining,

                      RemoteAccessStatus.ONLINE,                                // RemoteStatus
                      otherVersions.Data?.Select(version => version.Id) ?? [],  // RemoteVersionIds
                      Version.Id,                                               // SelectedVersionId
                      oldRemoteParty.RemoteAccessInfos.First().NotBefore,
                      oldRemoteParty.RemoteAccessInfos.First().NotAfter,
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
                                                      OurBusinessDetails,
                                                      OurCountryCode,
                                                      OurPartyId
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
            TokenStatus  _tokenStatus;
            CDR?          cdr;

            var errorResponses = new List<Tuple<Command, String>>();

            switch (command.CommandName)
            {

                #region addLocation

                case CommonHTTPAPI.addLocation:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Location.TryParse(command.JSONObject,
                                              out location,
                                              out errorResponse))
                        {
                            locations.TryAdd(location.Id, location);
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
                            Location.TryParse(command.JSONObject,
                                              out location,
                                              out errorResponse))
                        {
                            locations.TryAdd(location.Id, location);
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
                            Location.TryParse(command.JSONObject,
                                              out location,
                                              out errorResponse))
                        {

                            if (locations.ContainsKey(location.Id))
                                locations.Remove(location.Id, out _);

                            locations.TryAdd(location.Id, location);

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
                            Location.TryParse(command.JSONObject,
                                              out location,
                                              out errorResponse))
                        {
                            locations.Remove(location.Id, out _);
                            locations.TryAdd(location.Id, location);
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
                            Location.TryParse(command.JSONObject,
                                              out location,
                                              out errorResponse))
                        {
                            locations.Remove(location.Id, out _);
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
                    locations.Clear();
                    break;

                #endregion


                // Experimental!

                #region addOrUpdateEVSE

                case CommonHTTPAPI.addOrUpdateEVSE:
                    try
                    {
                        if (command.JSONObject is not null &&

                            command.JSONObject.TryGetValue("locationId", out var locationId) &&
                            locationId.Type == JTokenType.String &&
                            Location_Id.TryParse(locationId?.Value<String>() ?? "", out var location_Id) &&
                            locations.ContainsKey(location_Id) &&

                            command.JSONObject.TryGetValue("evse",       out var evseJToken) &&
                            evseJToken.Type == JTokenType.Object &&
                            evseJToken is JObject &&
                            EVSE.TryParse((evseJToken as JObject)!,
                                          out evse,
                                          out errorResponse))

                        {

                            if (locations.TryGetValue(location_Id, out location))
                            {

                                var updatedLocation = location.Update(loc => {

                                    var newEVSEs = loc.EVSEs.Where(evseX => evseX.UId != evse.UId).ToList();
                                    newEVSEs.Add(evse);

                                    loc.EVSEs.Clear();

                                    foreach (var newEVSE in newEVSEs)
                                        loc.EVSEs.Add(newEVSE);

                                }, out var warnings);

                                if (updatedLocation is not null)
                                {
                                    locations.Remove(location.Id, out _);
                                    locations.TryAdd(location.Id, updatedLocation);
                                }

                            }

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


                #region addTariff

                case CommonHTTPAPI.addTariff:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Tariff.TryParse(command.JSONObject,
                                            out tariff,
                                            out errorResponse))
                        {
                            tariffs.TryAdd(tariff.Id, tariff);
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
                            Tariff.TryParse(command.JSONObject,
                                            out tariff,
                                            out errorResponse))
                        {
                            tariffs.TryAdd(tariff.Id, tariff);
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
                            Tariff.TryParse(command.JSONObject,
                                            out tariff,
                                            out errorResponse))
                        {

                            if (tariffs.ContainsKey(tariff.Id))
                                tariffs.Remove(tariff.Id);

                            tariffs.TryAdd(tariff.Id, tariff);

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
                            Tariff.TryParse(command.JSONObject,
                                            out tariff,
                                            out errorResponse))
                        {
                            tariffs.Remove(tariff.Id);
                            tariffs.TryAdd(tariff.Id, tariff);
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
                            Tariff.TryParse(command.JSONObject,
                                            out tariff,
                                            out errorResponse))
                        {
                            tariffs.Remove(tariff.Id);
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
                    tariffs.Clear();
                    break;

                #endregion


                #region addSession

                case CommonHTTPAPI.addSession:
                    try
                    {
                        if (command.JSONObject is not null &&
                            Session.TryParse(command.JSONObject,
                                             out session,
                                             out errorResponse))
                        {
                            chargingSessions.TryAdd(session.Id, session);
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
                            Session.TryParse(command.JSONObject,
                                             out session,
                                             out errorResponse))
                        {
                            chargingSessions.TryAdd(session.Id, session);
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
                            Session.TryParse(command.JSONObject,
                                             out session,
                                             out errorResponse))
                        {

                            if (chargingSessions.ContainsKey(session.Id))
                                chargingSessions.Remove(session.Id, out _);

                            chargingSessions.TryAdd(session.Id, session);

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
                            Session.TryParse(command.JSONObject,
                                             out session,
                                             out errorResponse))
                        {
                            chargingSessions.Remove(session.Id, out _);
                            chargingSessions.TryAdd(session.Id, session);
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
                            Session.TryParse(command.JSONObject,
                                             out session,
                                             out errorResponse))
                        {
                            chargingSessions.Remove(session.Id, out _);
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
                    chargingSessions.Clear();
                    break;

                #endregion


                #region addToken

                case CommonHTTPAPI.addTokenStatus:
                    try
                    {
                        if (command.JSONObject is not null &&
                            TokenStatus.TryParse(command.JSONObject,
                                                 out _tokenStatus,
                                                 out errorResponse))
                        {
                            tokenStatus.TryAdd(_tokenStatus.Token.Id, _tokenStatus);
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
                            TokenStatus.TryParse(command.JSONObject,
                                                 out _tokenStatus,
                                                 out errorResponse))
                        {
                            tokenStatus.TryAdd(_tokenStatus.Token.Id, _tokenStatus);
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
                            TokenStatus.TryParse(command.JSONObject,
                                                 out _tokenStatus,
                                                 out errorResponse))
                        {

                            if (tokenStatus.ContainsKey(_tokenStatus.Token.Id))
                                tokenStatus.Remove(_tokenStatus.Token.Id, out _);

                            tokenStatus.TryAdd(_tokenStatus.Token.Id, _tokenStatus);

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
                            TokenStatus.TryParse(command.JSONObject,
                                                 out _tokenStatus,
                                                 out errorResponse))
                        {
                            tokenStatus.Remove(_tokenStatus.Token.Id, out _);
                            tokenStatus.TryAdd(_tokenStatus.Token.Id, _tokenStatus);
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
                            TokenStatus.TryParse(command.JSONObject,
                                                 out _tokenStatus,
                                                 out errorResponse))
                        {
                            tokenStatus.Remove(_tokenStatus.Token.Id, out _);
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
                    tokenStatus.Clear();
                    break;

                #endregion


                #region addChargeDetailRecord

                case CommonHTTPAPI.addChargeDetailRecord:
                    try
                    {
                        if (command.JSONObject is not null &&
                            CDR.TryParse(command.JSONObject,
                                         out cdr,
                                         out errorResponse))
                        {
                            chargeDetailRecords.TryAdd(cdr.Id, cdr);
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
                            CDR.TryParse(command.JSONObject,
                                         out cdr,
                                         out errorResponse))
                        {
                            chargeDetailRecords.TryAdd(cdr.Id, cdr);
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
                            CDR.TryParse(command.JSONObject,
                                         out cdr,
                                         out errorResponse))
                        {

                            if (chargeDetailRecords.ContainsKey(cdr.Id))
                                chargeDetailRecords.Remove(cdr.Id, out _);

                            chargeDetailRecords.TryAdd(cdr.Id, cdr);

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
                            CDR.TryParse(command.JSONObject,
                                            out cdr,
                                            out errorResponse))
                        {
                            chargeDetailRecords.Remove(cdr.Id, out _);
                            chargeDetailRecords.TryAdd(cdr.Id, cdr);
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
                            CDR.TryParse(command.JSONObject,
                                         out cdr,
                                         out errorResponse))
                        {
                            chargeDetailRecords.Remove(cdr.Id, out _);
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
                    chargeDetailRecords.Clear();
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
        public CommonClient GetCommonClient(URL                                                        RemoteVersionsURL,
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
        public CommonClient GetCommonClient(RemoteParty_Id                                             RemotePartyId,

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
            if (AccessToken.TryParse(Token, out var token))
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

                    await LogAsset(
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
                                                remoteParty.CountryCode,
                                                remoteParty.PartyId,
                                                remoteParty.Role,
                                                remoteParty.BusinessDetails,
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

            LocalAccessInfo = null;
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

        private readonly ConcurrentDictionary<RemoteParty_Id, RemoteParty> remoteParties = [];

        /// <summary>
        /// Return an enumeration of all remote parties.
        /// </summary>
        public IEnumerable<RemoteParty> RemoteParties
            => remoteParties.Values;

        #endregion


        #region AddRemoteParty            (...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(CountryCode                                                CountryCode,
                           Party_Id                                                   PartyId,
                           Role                                                       Role,
                           BusinessDetails                                            BusinessDetails,

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

                           PartyStatus?                                               Status                            = PartyStatus.ENABLED,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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

        #region AddRemoteParty            (...)

        /// <summary>
        /// Create a new Remote Party with local access only.
        /// The remote party will start the OCPI registration process afterwards.
        /// </summary>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="Role"></param>
        /// <param name="BusinessDetails"></param>
        /// 
        /// <param name="LocalAccessToken"></param>
        /// <param name="LocalAccessTokenBase64Encoding"></param>
        /// <param name="LocalTOTPConfig"></param>
        /// <param name="LocalAccessNotBefore"></param>
        /// <param name="LocalAccessNotAfter"></param>
        /// <param name="LocalAllowDowngrades"></param>
        /// <param name="LocalAccessStatus"></param>
        /// 
        /// <param name="Status"></param>
        /// 
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(CountryCode        CountryCode,
                           Party_Id           PartyId,
                           Role               Role,
                           BusinessDetails    BusinessDetails,

                           AccessToken        LocalAccessToken,
                           Boolean?           LocalAccessTokenBase64Encoding   = null,
                           TOTPConfig?        LocalTOTPConfig                  = null,
                           DateTimeOffset?    LocalAccessNotBefore             = null,
                           DateTimeOffset?    LocalAccessNotAfter              = null,
                           Boolean?           LocalAllowDowngrades             = false,
                           AccessStatus?      LocalAccessStatus                = AccessStatus.ALLOWED,

                           PartyStatus?       Status                           = PartyStatus.ENABLED,

                           DateTimeOffset?    Created                          = null,
                           DateTimeOffset?    LastUpdated                      = null,
                           EventTracking_Id?  EventTrackingId                  = null,
                           User_Id?           CurrentUserId                    = null)

        {

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

                                     LocalAccessToken,
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

        #region AddRemoteParty            (...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(CountryCode                                                CountryCode,
                           Party_Id                                                   PartyId,
                           Role                                                       Role,
                           BusinessDetails                                            BusinessDetails,

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

                           PartyStatus?                                               Status                            = PartyStatus.ENABLED,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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

        #region AddRemoteParty            (...)

        public async Task<AddResult<RemoteParty>>

            AddRemoteParty(CountryCode                    CountryCode,
                           Party_Id                       PartyId,
                           Role                           Role,
                           BusinessDetails                BusinessDetails,

                           IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                           IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                           PartyStatus?                   Status            = PartyStatus.ENABLED,

                           DateTimeOffset?                Created           = null,
                           DateTimeOffset?                LastUpdated       = null,
                           EventTracking_Id?              EventTrackingId   = null,
                           User_Id?                       CurrentUserId     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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


        #region AddRemotePartyIfNotExists (...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(CountryCode                                                CountryCode,
                                      Party_Id                                                   PartyId,
                                      Role                                                       Role,
                                      BusinessDetails                                            BusinessDetails,

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

                                      PartyStatus?                                               Status                            = PartyStatus.ENABLED,

                                      DateTimeOffset?                                            Created                           = null,
                                      DateTimeOffset?                                            LastUpdated                       = null,
                                      EventTracking_Id?                                          EventTrackingId                   = null,
                                      User_Id?                                                   CurrentUserId                     = null)
        {

            if (remoteParties.TryGetValue(RemoteParty_Id.Parse($"{CountryCode}-{PartyId}_{Role}"), out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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

        #region AddRemotePartyIfNotExists (...)

        /// <summary>
        /// Create a new Remote Party with local access only, if it does not already exist.
        /// The remote party will start the OCPI registration process afterwards.
        /// </summary>
        /// <param name="CountryCode"></param>
        /// <param name="PartyId"></param>
        /// <param name="Role"></param>
        /// <param name="BusinessDetails"></param>
        /// 
        /// <param name="LocalAccessToken"></param>
        /// <param name="LocalAccessTokenBase64Encoding"></param>
        /// <param name="LocalTOTPConfig"></param>
        /// <param name="LocalAccessNotBefore"></param>
        /// <param name="LocalAccessNotAfter"></param>
        /// <param name="LocalAllowDowngrades"></param>
        /// <param name="LocalAccessStatus"></param>
        /// 
        /// <param name="Status"></param>
        /// 
        /// <param name="Created"></param>
        /// <param name="LastUpdated"></param>
        /// <param name="EventTrackingId"></param>
        /// <param name="CurrentUserId"></param>
        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(CountryCode        CountryCode,
                                      Party_Id           PartyId,
                                      Role               Role,
                                      BusinessDetails    BusinessDetails,

                                      AccessToken        LocalAccessToken,
                                      Boolean?           LocalAccessTokenBase64Encoding   = null,
                                      TOTPConfig?        LocalTOTPConfig                  = null,
                                      DateTimeOffset?    LocalAccessNotBefore             = null,
                                      DateTimeOffset?    LocalAccessNotAfter              = null,
                                      Boolean?           LocalAllowDowngrades             = false,
                                      AccessStatus?      LocalAccessStatus                = AccessStatus.ALLOWED,

                                      PartyStatus?       Status                           = PartyStatus.ENABLED,

                                      DateTimeOffset?    Created                          = null,
                                      DateTimeOffset?    LastUpdated                      = null,
                                      EventTracking_Id?  EventTrackingId                  = null,
                                      User_Id?           CurrentUserId                    = null)

        {

            if (remoteParties.TryGetValue(RemoteParty_Id.Parse($"{CountryCode}-{PartyId}_{Role}"), out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

                                     LocalAccessToken,
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

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
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

        #region AddRemotePartyIfNotExists (...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(CountryCode                                                CountryCode,
                                      Party_Id                                                   PartyId,
                                      Role                                                       Role,
                                      BusinessDetails                                            BusinessDetails,

                                      URL                                                        RemoteVersionsURL,
                                      AccessToken                                                RemoteAccessToken,

                                      PartyStatus?                                               Status                            = PartyStatus.ENABLED,

                                      Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                      TOTPConfig?                                                RemoteTOTPConfig                  = null,
                                      DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                      DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                      RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                      IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                      Version_Id?                                                SelectedVersionId                 = null,
                                      Boolean?                                                   RemoteAllowDowngrades             = null,

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

                                      DateTimeOffset?                                            Created                           = null,
                                      DateTimeOffset?                                            LastUpdated                       = null,
                                      EventTracking_Id?                                          EventTrackingId                   = null,
                                      User_Id?                                                   CurrentUserId                     = null)
        {

            if (remoteParties.TryGetValue(RemoteParty_Id.Parse($"{CountryCode}-{PartyId}_{Role}"), out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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

        #region AddRemotePartyIfNotExists (...)

        public async Task<AddResult<RemoteParty>>

            AddRemotePartyIfNotExists(CountryCode                    CountryCode,
                                      Party_Id                       PartyId,
                                      Role                           Role,
                                      BusinessDetails                BusinessDetails,

                                      IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                                      IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                                      PartyStatus?                   Status            = PartyStatus.ENABLED,

                                      DateTimeOffset?                Created           = null,
                                      DateTimeOffset?                LastUpdated       = null,
                                      EventTracking_Id?              EventTrackingId   = null,
                                      User_Id?                       CurrentUserId     = null)
        {

            if (remoteParties.TryGetValue(RemoteParty_Id.Parse($"{CountryCode}-{PartyId}_{Role}"), out var existingRemoteParty))
                return AddResult<RemoteParty>.NoOperation(
                           EventTracking_Id.New,
                           existingRemoteParty,
                           "The remote party already exists!"
                       );

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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


        #region AddOrUpdateRemoteParty    (...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(CountryCode                                                CountryCode,
                                   Party_Id                                                   PartyId,
                                   Role                                                       Role,
                                   BusinessDetails                                            BusinessDetails,

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

                                   PartyStatus?                                               Status                            = PartyStatus.ENABLED,

                                   DateTimeOffset?                                            Created                           = null,
                                   DateTimeOffset?                                            LastUpdated                       = null,
                                   EventTracking_Id?                                          EventTrackingId                   = null,
                                   User_Id?                                                   CurrentUserId                     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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

        #region AddOrUpdateRemoteParty    (...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(CountryCode        CountryCode,
                                   Party_Id           PartyId,
                                   Role               Role,
                                   BusinessDetails    BusinessDetails,

                                   AccessToken        LocalAccessToken,
                                   Boolean?           LocalAccessTokenBase64Encoding   = null,
                                   TOTPConfig?        LocalTOTPConfig                  = null,
                                   DateTimeOffset?    LocalAccessNotBefore             = null,
                                   DateTimeOffset?    LocalAccessNotAfter              = null,
                                   Boolean?           LocalAllowDowngrades             = false,
                                   AccessStatus?      LocalAccessStatus                = AccessStatus.ALLOWED,

                                   PartyStatus?       Status                           = PartyStatus.ENABLED,

                                   DateTimeOffset?    Created                          = null,
                                   DateTimeOffset?    LastUpdated                      = null,
                                   EventTracking_Id?  EventTrackingId                  = null,
                                   User_Id?           CurrentUserId                    = null)

        {

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

                                     LocalAccessToken,
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

        #region AddOrUpdateRemoteParty    (...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(CountryCode                                                CountryCode,
                                   Party_Id                                                   PartyId,
                                   Role                                                       Role,
                                   BusinessDetails                                            BusinessDetails,

                                   URL                                                        RemoteVersionsURL,
                                   AccessToken                                                RemoteAccessToken,
                                   Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                   TOTPConfig?                                                RemoteTOTPConfig                  = null,

                                   DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                   DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                   RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                   IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                   Version_Id?                                                SelectedVersionId                 = null,
                                   Boolean?                                                   RemoteAllowDowngrades             = null,

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

                                   PartyStatus?                                               Status                            = PartyStatus.ENABLED,

                                   DateTimeOffset?                                            Created                           = null,
                                   DateTimeOffset?                                            LastUpdated                       = null,
                                   EventTracking_Id?                                          EventTrackingId                   = null,
                                   User_Id?                                                   CurrentUserId                     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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

        #region AddOrUpdateRemoteParty    (...)

        public async Task<AddOrUpdateResult<RemoteParty>>

            AddOrUpdateRemoteParty(CountryCode                    CountryCode,
                                   Party_Id                       PartyId,
                                   Role                           Role,
                                   BusinessDetails                BusinessDetails,

                                   IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                                   IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,
                                   PartyStatus?                   Status            = PartyStatus.ENABLED,

                                   DateTimeOffset?                Created           = null,
                                   DateTimeOffset?                LastUpdated       = null,
                                   EventTracking_Id?              EventTrackingId   = null,
                                   User_Id?                       CurrentUserId     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

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


        #region UpdateRemoteParty         (...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                              BusinessDetails                                            BusinessDetails,

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

                              PartyStatus?                                               Status                            = PartyStatus.ENABLED,

                              DateTimeOffset?                                            Created                           = null,
                              DateTimeOffset?                                            LastUpdated                       = null,
                              EventTracking_Id?                                          EventTrackingId                   = null,
                              User_Id?                                                   CurrentUserId                     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.CountryCode,
                                     ExistingRemoteParty.PartyId,
                                     ExistingRemoteParty.Role,
                                     BusinessDetails,

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

        #region UpdateRemoteParty         (...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty        ExistingRemoteParty,
                              BusinessDetails    BusinessDetails,

                              AccessToken        LocalAccessToken,
                              Boolean?           LocalAccessTokenBase64Encoding   = null,
                              TOTPConfig?        LocalTOTPConfig                  = null,
                              DateTimeOffset?    LocalAccessNotBefore             = null,
                              DateTimeOffset?    LocalAccessNotAfter              = null,
                              Boolean?           LocalAllowDowngrades             = false,
                              AccessStatus?      LocalAccessStatus                = AccessStatus.ALLOWED,

                              PartyStatus?       Status                           = PartyStatus.ENABLED,

                              DateTimeOffset?    Created                          = null,
                              DateTimeOffset?    LastUpdated                      = null,
                              EventTracking_Id?  EventTrackingId                  = null,
                              User_Id?           CurrentUserId                    = null)

        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.CountryCode,
                                     ExistingRemoteParty.PartyId,
                                     ExistingRemoteParty.Role,
                                     BusinessDetails,

                                     LocalAccessToken,
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

        #region UpdateRemoteParty         (...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                              BusinessDetails                                            BusinessDetails,

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

                              PartyStatus?                                               Status                            = PartyStatus.ENABLED,

                              DateTimeOffset?                                            Created                           = null,
                              DateTimeOffset?                                            LastUpdated                       = null,
                              EventTracking_Id?                                          EventTrackingId                   = null,
                              User_Id?                                                   CurrentUserId                     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.CountryCode,
                                     ExistingRemoteParty.PartyId,
                                     ExistingRemoteParty.Role,
                                     BusinessDetails,

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

        #region UpdateRemoteParty         (...)

        public async Task<UpdateResult<RemoteParty>>

            UpdateRemoteParty(RemoteParty                    ExistingRemoteParty,
                              BusinessDetails                BusinessDetails,

                              IEnumerable<LocalAccessInfo>   LocalAccessInfos,
                              IEnumerable<RemoteAccessInfo>  RemoteAccessInfos,

                              PartyStatus?                   Status            = PartyStatus.ENABLED,

                              DateTimeOffset?                Created           = null,
                              DateTimeOffset?                LastUpdated       = null,
                              EventTracking_Id?              EventTrackingId   = null,
                              User_Id?                       CurrentUserId     = null)
        {

            var newRemoteParty = new RemoteParty(

                                     ExistingRemoteParty.CountryCode,
                                     ExistingRemoteParty.PartyId,
                                     ExistingRemoteParty.Role,
                                     BusinessDetails,

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
                             Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                  remoteParty.PartyId     == PartyId);

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
                             Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                  remoteParty.PartyId     == PartyId     &&
                                                  remoteParty.Role        == Role);

        #endregion

        #region GetRemoteParties          (Roles)

        /// <summary>
        /// Get all remote parties having one of the given roles.
        /// </summary>
        /// <param name="Roles">An optional list of roles.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(params Role[] Roles)

            => remoteParties.Values.
                             Where(remoteParty => Roles.Contains(remoteParty.Role));

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

        #region GetRemoteParties          (AccessToken, TOTP, out RemoteParties)

        public Boolean TryGetRemoteParties(AccessToken                                           AccessToken,
                                           String?                                               TOTP,
                                           out IEnumerable<Tuple<RemoteParty, LocalAccessInfo>>  RemoteParties)
        {

            var remoteParties = new List<Tuple<RemoteParty, LocalAccessInfo>>();

            foreach (var remoteParty in this.remoteParties.Values)
            {
                foreach (var localAccessInfo in remoteParty.LocalAccessInfos)
                {

                    if (localAccessInfo.TOTPConfig is not null)
                    {

                        var accessToken  = AccessToken.ToString();

                        var (previous,
                             current,
                             next,
                             _,
                             _) = TOTPGenerator.GenerateTOTPs(
                                      localAccessInfo.TOTPConfig.SharedSecret,
                                      localAccessInfo.TOTPConfig.ValidityTime,
                                      localAccessInfo.TOTPConfig.Length,
                                      localAccessInfo.TOTPConfig.Alphabet
                                  );

                        if (TOTP == current || TOTP == previous || TOTP == next)
                            remoteParties.Add(new Tuple<RemoteParty, LocalAccessInfo>(remoteParty, localAccessInfo));

                    }

                    else
                    {
                        if (localAccessInfo.AccessToken == AccessToken)
                            remoteParties.Add(new Tuple<RemoteParty, LocalAccessInfo>(remoteParty, localAccessInfo));
                    }

                }
            }

            RemoteParties = remoteParties;

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

        #region RemoveRemoteParty         (CountryCode, PartyId, AccessToken)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     AccessToken        AccessToken,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                                                  remoteParty.PartyId     == PartyId     &&
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

            await LogRemoteParty(
                      CommonHTTPAPI.removeAllRemoteParties,
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

        }

        #endregion

        #endregion


        #region Locations

        private readonly ConcurrentDictionary<Location_Id, Location> locations = [];

        #region Events

        public delegate Task OnLocationAddedDelegate    (Location        Location);
        public delegate Task OnLocationChangedDelegate  (Location        Location);
        public delegate Task OnLocationRemovedDelegate  (Location        Location);
        public delegate Task OnEVSEAddedDelegate        (EVSE            EVSE);
        public delegate Task OnEVSEChangedDelegate      (EVSE            EVSE);
        public delegate Task OnEVSEStatusChangedDelegate(DateTimeOffset  Timestamp, EVSE EVSE, StatusType NewEVSEStatus, StatusType? OldEVSEStatus = null);
        public delegate Task OnEVSERemovedDelegate      (EVSE            EVSE);

        public event OnLocationAddedDelegate?      OnLocationAdded;
        public event OnLocationChangedDelegate?    OnLocationChanged;
        public event OnLocationRemovedDelegate?    OnLocationRemoved;
        public event OnEVSEAddedDelegate?          OnEVSEAdded;
        public event OnEVSEChangedDelegate?        OnEVSEChanged;
        public event OnEVSEStatusChangedDelegate?  OnEVSEStatusChanged;
        public event OnEVSERemovedDelegate?        OnEVSERemoved;

        #endregion


        #region Locations

        #region AddLocation            (Location, ...)

        public async Task<AddResult<Location>>

            AddLocation(Location           Location,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null,
                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (locations.TryAdd(Location.Id, Location))
            {

                DebugX.Log($"OCPI {Version.String} Location '{Location.Id}': '{Location}' added...");

                Location.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addLocation,
                          Location.ToJSON(
                              true,
                              true,
                              null,
                              CustomLocationSerializer,
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

                    var OnLocationAddedLocal = OnLocationAdded;
                    if (OnLocationAddedLocal is not null)
                    {
                        try
                        {
                            await OnLocationAddedLocal(Location);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var OnEVSEAddedLocal = OnEVSEAdded;
                    if (OnEVSEAddedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in Location.EVSEs)
                                await OnEVSEAddedLocal(evse);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnEVSEAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

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

        #endregion

        #region AddLocationIfNotExists (Location, ...)

        public async Task<AddResult<Location>>

            AddLocationIfNotExists(Location           Location,
                                   Boolean            SkipNotifications   = false,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   User_Id?           CurrentUserId       = null,
                                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (locations.TryAdd(Location.Id, Location))
            {

                DebugX.Log($"OCPI {Version.String} Location '{Location.Id}': '{Location}' added...");

                Location.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addLocationIfNotExists,
                          Location.ToJSON(
                              true,
                              true,
                              null,
                              CustomLocationSerializer,
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

                    var OnLocationAddedLocal = OnLocationAdded;
                    if (OnLocationAddedLocal is not null)
                    {
                        try
                        {
                            OnLocationAddedLocal(Location).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocationIfNotExists), " ", nameof(OnLocationAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var OnEVSEAddedLocal = OnEVSEAdded;
                    if (OnEVSEAddedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in Location.EVSEs)
                                await OnEVSEAddedLocal(evse);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocationIfNotExists), " ", nameof(OnEVSEAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

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

        #endregion

        #region AddOrUpdateLocation    (Location,                  AllowDowngrades = false, ...)

        public async Task<AddOrUpdateResult<Location>>

            AddOrUpdateLocation(Location           Location,
                                Boolean?           AllowDowngrades     = false,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Update an existing location

            if (locations.TryGetValue(Location.Id, out var existingLocation))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Location.LastUpdated <= existingLocation.LastUpdated)
                {
                    return AddOrUpdateResult<Location>.Failed(
                               EventTrackingId,
                               Location,
                               "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!"
                           );
                }

                //if (Location.LastUpdated.ToISO8601() == existingLocation.LastUpdated.ToISO8601())
                //    return AddOrUpdateResult<Location>.NoOperation(Location,
                //                                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");

                if (locations.TryUpdate(Location.Id,
                                        Location,
                                        existingLocation))
                {

                    Location.CommonAPI = this;

                    await LogAsset(
                              CommonHTTPAPI.addOrUpdateLocation,
                              Location.ToJSON(
                                  true,
                                  true,
                                  null,
                                  CustomLocationSerializer,
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

                        var OnLocationChangedLocal = OnLocationChanged;
                        if (OnLocationChangedLocal is not null)
                        {
                            try
                            {
                                OnLocationChangedLocal(Location).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                        var oldEVSEUIds = new HashSet<EVSE_UId>(existingLocation.EVSEs.Select(evse => evse.UId));
                        var newEVSEUIds = new HashSet<EVSE_UId>(Location.        EVSEs.Select(evse => evse.UId));

                        foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                        {

                            if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId) && existingLocation.GetEVSE(evseUId)! != Location.GetEVSE(evseUId)!)
                            {
                                var OnEVSEChangedLocal = OnEVSEChanged;
                                if (OnEVSEChangedLocal is not null)
                                {
                                    try
                                    {
                                        await OnEVSEChangedLocal(existingLocation.GetEVSE(evseUId)!);
                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                    Environment.NewLine, e.Message,
                                                    Environment.NewLine, e.StackTrace ?? "");
                                    }
                                }
                            }
                            else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                            {
                                var OnEVSEAddedLocal = OnEVSEAdded;
                                if (OnEVSEAddedLocal is not null)
                                {
                                    try
                                    {
                                        await OnEVSEAddedLocal(existingLocation.GetEVSE(evseUId)!);
                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                                                    Environment.NewLine, e.Message,
                                                    Environment.NewLine, e.StackTrace ?? "");
                                    }
                                }
                            }
                            else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                            {
                                var OnEVSERemovedLocal = OnEVSERemoved;
                                if (OnEVSERemovedLocal is not null)
                                {
                                    try
                                    {
                                        await OnEVSERemovedLocal(existingLocation.GetEVSE(evseUId)!);
                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSERemoved), ": ",
                                                    Environment.NewLine, e.Message,
                                                    Environment.NewLine, e.StackTrace ?? "");
                                    }
                                }
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

            if (locations.TryAdd(Location.Id, Location))
            {

                Location.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addOrUpdateLocation,
                          Location.ToJSON(
                              true,
                              true,
                              null,
                              CustomLocationSerializer,
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

                    var OnLocationAddedLocal = OnLocationAdded;
                    if (OnLocationAddedLocal is not null)
                    {
                        try
                        {
                            OnLocationAddedLocal(Location).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var OnEVSEAddedLocal = OnEVSEAdded;
                    if (OnEVSEAddedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in Location.EVSEs)
                                await OnEVSEAddedLocal(evse);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

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

        #endregion

        #region UpdateLocation         (Location,                  AllowDowngrades = false, ...)

        public async Task<UpdateResult<Location>>

            UpdateLocation(Location           Location,
                           Boolean?           AllowDowngrades     = false,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null,
                           CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (!locations.TryGetValue(Location.Id, out var existingLocation))
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


            if (locations.TryUpdate(Location.Id,
                                    Location,
                                    existingLocation))
            {

                Location.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.updateLocation,
                          Location.ToJSON(
                              true,
                              true,
                              null,
                              CustomLocationSerializer,
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

                    var onLocationChanged = OnLocationChanged;
                    if (onLocationChanged is not null)
                    {
                        try
                        {

                            await Task.WhenAll(
                                      onLocationChanged.GetInvocationList().
                                          Cast<OnLocationChangedDelegate>().
                                          Select(e => e(Location))
                                  );

                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnLocationChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

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
                                {
                                    var onEVSEChanged = OnEVSEChanged;
                                    if (onEVSEChanged is not null)
                                    {
                                        try
                                        {

                                            await Task.WhenAll(
                                                      onEVSEChanged.GetInvocationList().
                                                          Cast<OnEVSEChangedDelegate>().
                                                          Select(e => e(newEVSE))
                                                  );

                                        }
                                        catch (Exception e)
                                        {
                                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                        Environment.NewLine, e.Message,
                                                        Environment.NewLine, e.StackTrace ?? "");
                                        }
                                    }
                                }

                                if (oldEVSE.Status != newEVSE.Status)
                                {
                                    var onEVSEStatusChanged = OnEVSEStatusChanged;
                                    if (onEVSEStatusChanged is not null)
                                    {
                                        try
                                        {

                                            await Task.WhenAll(
                                                      onEVSEStatusChanged.GetInvocationList().
                                                          Cast<OnEVSEStatusChangedDelegate>().
                                                          Select(e => e(Timestamp.Now,
                                                                        newEVSE,
                                                                        newEVSE.Status,
                                                                        oldEVSE.Status))
                                                  );

                                        }
                                        catch (Exception e)
                                        {
                                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                        Environment.NewLine, e.Message,
                                                        Environment.NewLine, e.StackTrace ?? "");
                                        }
                                    }
                                }

                            }

                        }
                        else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                        {

                            var onEVSEAdded = OnEVSEAdded;
                            if (onEVSEAdded is not null)
                            {
                                try
                                {
                                    if (Location.TryGetEVSE(evseUId, out var evse) &&
                                        evse is not null)
                                    {

                                        await Task.WhenAll(
                                                  onEVSEAdded.GetInvocationList().
                                                      Cast<OnEVSEAddedDelegate>().
                                                      Select(e => e(evse))
                                              );

                                    }
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                            var onEVSEStatusChanged = OnEVSEStatusChanged;
                            if (onEVSEStatusChanged is not null)
                            {
                                try
                                {
                                    if (Location.TryGetEVSE(evseUId, out var evse) &&
                                        evse is not null)
                                    {

                                        await Task.WhenAll(
                                                  onEVSEStatusChanged.GetInvocationList().
                                                      Cast<OnEVSEStatusChangedDelegate>().
                                                      Select(e => e(Timestamp.Now,
                                                                    evse,
                                                                    evse.Status))
                                              );

                                    }
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                        }
                        else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                        {

                            var onEVSERemoved = OnEVSERemoved;
                            if (onEVSERemoved is not null)
                            {
                                try
                                {
                                    if (existingLocation.TryGetEVSE(evseUId, out var evse) &&
                                        evse is not null)
                                    {

                                        await Task.WhenAll(
                                                  onEVSERemoved.GetInvocationList().
                                                      Cast<OnEVSERemovedDelegate>().
                                                      Select(e => e(evse))
                                              );

                                    }
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSERemoved), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                            var onEVSEStatusChanged = OnEVSEStatusChanged;
                            if (onEVSEStatusChanged is not null)
                            {
                                try
                                {
                                    if (existingLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                                        Location.        TryGetEVSE(evseUId, out var newEVSE) &&
                                        oldEVSE is not null &&
                                        newEVSE is not null)
                                    {

                                        await Task.WhenAll(
                                                  onEVSEStatusChanged.GetInvocationList().
                                                      Cast<OnEVSEStatusChangedDelegate>().
                                                      Select(e => e(Timestamp.Now,
                                                                    oldEVSE,
                                                                    newEVSE.Status,
                                                                    oldEVSE.Status))
                                              );

                                    }
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
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

        #endregion

        #region TryPatchLocation       (LocationId, LocationPatch, AllowDowngrades = false, ...)

        public async Task<PatchResult<Location>> TryPatchLocation(Location_Id        LocationId,
                                                                  JObject            LocationPatch,
                                                                  Boolean?           AllowDowngrades     = false,
                                                                  Boolean            SkipNotifications   = false,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  User_Id?           CurrentUserId       = null,
                                                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (locations.TryGetValue(LocationId, out var existingLocation))
            {

                var patchResult = existingLocation.TryPatch(
                                      LocationPatch,
                                      AllowDowngrades ?? this.AllowDowngrades ?? false,
                                      EventTrackingId
                                  );

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    var updateLocationResult = await UpdateLocation(
                                                         patchResult.PatchedData,
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
                                   "Could not update the location: " + updateLocationResult.ErrorResponse
                               );

                }

                return patchResult;

            }

            return PatchResult<Location>.Failed(
                       EventTrackingId,
                       $"The given location '{LocationId}' is unknown!"
                   );

        }

        #endregion


        #region RemoveLocation    (Location,             SkipNotifications = false)

        /// <summary>
        /// Remove the given charging location.
        /// </summary>
        /// <param name="Location">A charging location.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<Location>> RemoveLocation(Location           Location,
                                                           Boolean            SkipNotifications   = false,
                                                           EventTracking_Id?  EventTrackingId     = null,
                                                           User_Id?           CurrentUserId       = null)

            => RemoveLocation(Location.Id,
                              SkipNotifications);

        #endregion

        #region RemoveLocation    (LocationId,           SkipNotifications = false)

        /// <summary>
        /// Remove the given charging location.
        /// </summary>
        /// <param name="LocationId">An unique charging location identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<Location>> RemoveLocation(Location_Id        LocationId,
                                                                 Boolean            SkipNotifications   = false,
                                                                 EventTracking_Id?  EventTrackingId     = null,
                                                                 User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (locations.Remove(LocationId, out var location))
            {

                await LogAsset(
                          CommonHTTPAPI.removeLocation,
                          location.ToJSON(true,
                                          true,
                                          null,
                                          CustomLocationSerializer,
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
                                          CustomEnvironmentalImpactSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnLocationRemovedLocal = OnLocationRemoved;
                    if (OnLocationRemovedLocal is not null)
                    {
                        try
                        {
                            await OnLocationRemovedLocal(location);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var OnEVSERemovedLocal = OnEVSERemoved;
                    if (OnEVSERemovedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in location.EVSEs)
                                await OnEVSERemovedLocal(evse);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnEVSERemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<Location>.Success(EventTrackingId, location);

            }

            return RemoveResult<Location>.Failed(EventTrackingId,
                                                 "RemoveLocation(LocationId, ...) failed!");

        }

        #endregion

        #region RemoveAllLocations(                      SkipNotifications = false)

        /// <summary>
        /// Remove all charging locations.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> RemoveAllLocations(Boolean            SkipNotifications   = false,
                                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                                  User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var existingLocations = locations.Values.ToArray();

            locations.Clear();

            await LogAsset(
                      CommonHTTPAPI.removeAllLocations,
                      EventTrackingId,
                      CurrentUserId
                  );

            if (!SkipNotifications)
            {

                var OnLocationRemovedLocal = OnLocationRemoved;
                if (OnLocationRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var location in existingLocations)
                            await OnLocationRemovedLocal(location);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllLocations), " ", nameof(OnLocationRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

                var OnEVSERemovedLocal = OnEVSERemoved;
                if (OnEVSERemovedLocal is not null)
                {
                    try
                    {
                        foreach (var evse in existingLocations.SelectMany(location => location.EVSEs))
                            await OnEVSERemovedLocal(evse);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllLocations), " ", nameof(OnEVSERemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<Location>>.Success(EventTrackingId, existingLocations);

        }

        #endregion

        #region RemoveAllLocations(IncludeLocations,     SkipNotifications = false)

        /// <summary>
        /// Remove all matching charging locations.
        /// </summary>
        /// <param name="IncludeLocations">A charging location filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> RemoveAllLocations(Func<Location, Boolean>  IncludeLocations,
                                                                                  Boolean                  SkipNotifications   = false,
                                                                                  EventTracking_Id?        EventTrackingId     = null,
                                                                                  User_Id?                 CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedLocations  = new List<Location>();
            var failedLocations   = new List<RemoveResult<Location>>();

            foreach (var location in locations.Values.Where(IncludeLocations).ToArray())
            {

                var result = await RemoveLocation(location.Id,
                                                  SkipNotifications);

                if (result.IsSuccess)
                    removedLocations.Add(location);
                else
                    failedLocations. Add(result);

            }

            return removedLocations.Any() && !failedLocations.Any()
                       ? RemoveResult<IEnumerable<Location>>.Success(EventTrackingId, removedLocations)

                       : !removedLocations.Any() && !failedLocations.Any()
                             ? RemoveResult<IEnumerable<Location>>.NoOperation(EventTrackingId, Array.Empty<Location>())
                             : RemoveResult<IEnumerable<Location>>.Failed     (EventTrackingId, failedLocations.Select(location => location.Data)!,
                                                                               failedLocations.Select(location => location.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllLocations(IncludeLocationIds,   SkipNotifications = false)

        /// <summary>
        /// Remove all matching charging locations.
        /// </summary>
        /// <param name="IncludeLocationIds">A charging location identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> RemoveAllLocations(Func<Location_Id, Boolean>  IncludeLocationIds,
                                                                                  Boolean                     SkipNotifications   = false,
                                                                                  EventTracking_Id?           EventTrackingId     = null,
                                                                                  User_Id?                    CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedLocations  = new List<Location>();
            var failedLocations   = new List<RemoveResult<Location>>();

            foreach (var location in locations.Where  (kvp => IncludeLocationIds(kvp.Key)).
                                               Select (kvp => kvp.Value).
                                               ToArray())
            {

                var result = await RemoveLocation(location.Id,
                                                  SkipNotifications);

                if (result.IsSuccess)
                    removedLocations.Add(location);
                else
                    failedLocations. Add(result);

            }

            return removedLocations.Any() && !failedLocations.Any()
                       ? RemoveResult<IEnumerable<Location>>.Success(EventTrackingId, removedLocations)

                       : !removedLocations.Any() && !failedLocations.Any()
                             ? RemoveResult<IEnumerable<Location>>.NoOperation(EventTrackingId, Array.Empty<Location>())
                             : RemoveResult<IEnumerable<Location>>.Failed     (EventTrackingId, failedLocations.Select(location => location.Data)!,
                                                                               failedLocations.Select(location => location.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllLocations(CountryCode, PartyId, SkipNotifications = false)

        /// <summary>
        /// Remove all charging locations owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> RemoveAllLocations(CountryCode        CountryCode,
                                                                                  Party_Id           PartyId,
                                                                                  Boolean            SkipNotifications   = false,
                                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                                  User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedLocations  = new List<Location>();
            var failedLocations   = new List<RemoveResult<Location>>();

            foreach (var location in locations.Values.Where  (location => CountryCode == location.CountryCode &&
                                                                          PartyId     == location.PartyId).
                                                      ToArray())
            {

                var result = await RemoveLocation(location.Id,
                                                  SkipNotifications);

                if (result.IsSuccess)
                    removedLocations.Add(location);
                else
                    failedLocations. Add(result);

            }

            return removedLocations.Count != 0 && failedLocations.Count == 0
                       ? RemoveResult<IEnumerable<Location>>.Success(EventTrackingId, removedLocations)

                       : removedLocations.Count == 0 && failedLocations.Count == 0
                             ? RemoveResult<IEnumerable<Location>>.NoOperation(EventTrackingId, Array.Empty<Location>())
                             : RemoveResult<IEnumerable<Location>>.Failed     (EventTrackingId, failedLocations.Select(location => location.Data)!,
                                                                               failedLocations.Select(location => location.ErrorResponse).AggregateWith(", "));

        }

        #endregion


        #region LocationExists         (LocationId)

        public Boolean LocationExists(Location_Id  LocationId)

            => locations.ContainsKey(LocationId);

        #endregion

        #region GetLocation            (LocationId)

        public Location? GetLocation(Location_Id  LocationId)
        {

            if (locations.TryGetValue(LocationId, out var location))
                return location;

            return null;

        }

        #endregion

        #region TryGetLocation         (LocationId, out Location)

        public Boolean TryGetLocation(Location_Id                        LocationId,
                                      [NotNullWhen(true)] out Location?  Location)
        {

            if (locations.TryGetValue(LocationId, out Location))
                return true;

            Location = null;
            return false;

        }

        #endregion

        #region GetLocations           (PartyId = null)

        public IEnumerable<Location> GetLocations(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {

                var countryCode  = PartyId.Value.CountryCode;
                var partyId      = PartyId.Value.Party;

                var results      = locations.Values.
                                       Where(location => location.CountryCode == countryCode &&
                                                         location.PartyId     == partyId).
                                       ToArray();

                return results;

            }

            return locations.Values;

        }

        #endregion

        #region GetLocations           (CountryCode, PartyId)

        public IEnumerable<Location> GetLocations(CountryCode  CountryCode,
                                                  Party_Id     PartyId)

            => locations.Values.Where(location => location.CountryCode == CountryCode &&
                                                  location.PartyId     == PartyId);

        #endregion

        #region GetLocations           (IncludeLocation)

        public IEnumerable<Location> GetLocations(Func<Location, Boolean> IncludeLocation)
        {

            var allLocations = new List<Location>();

            foreach (var location in locations.Values)
            {
                if (location is not null &&
                    IncludeLocation(location))
                {
                    allLocations.Add(location);
                }
            }

            return allLocations;

        }

        #endregion

        #endregion

        #region EVSEs

        #region AddEVSE                (Location, EVSE,                                                     SkipNotifications = false)

        public async Task<AddResult<EVSE>> AddEVSE(Location           Location,
                                                   EVSE               EVSE,
                                                   Boolean            SkipNotifications   = false,
                                                   EventTracking_Id?  EventTrackingId     = null,
                                                   User_Id?           CurrentUserId       = null,
                                                   CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (Location.EVSEExists(EVSE.UId))
                return AddResult<EVSE>.Failed(EventTrackingId, EVSE,
                                              $"The given EVSE '{EVSE.UId}' already exists!");

            DebugX.Log($"OCPI v2.1.1 EVSE '{EVSE.UId}'/'{EVSE.EVSEId}': '{EVSE}' added...");

            var newLocation = Location.Update(locationBuilder => {
                                                  locationBuilder.SetEVSE(EVSE);
                                                  locationBuilder.LastUpdated  = EVSE.LastUpdated;
                                              },
                                              out var warnings);

            if (newLocation is null)
                return AddResult<EVSE>.Failed(
                           EventTrackingId, EVSE,
                           warnings.First().Text.FirstText()
                       );


            var updateLocationResult = await UpdateLocation(
                                                 newLocation,
                                                 AllowDowngrades,
                                                 SkipNotifications,
                                                 EventTrackingId,
                                                 CurrentUserId,
                                                 CancellationToken
                                             );


            return updateLocationResult.IsSuccess

                       ? AddResult<EVSE>.Success(
                             EventTrackingId, EVSE
                         )

                       : AddResult<EVSE>.Failed (
                             EventTrackingId, EVSE,
                             updateLocationResult.ErrorResponse ?? "Unknown error!"
                         );

        }

        #endregion

        #region AddEVSEIfNotExists     (Location, EVSE,                                                     SkipNotifications = false)

        public async Task<AddResult<EVSE>> AddEVSEIfNotExists(Location           Location,
                                                              EVSE               EVSE,
                                                              Boolean            SkipNotifications   = false,
                                                              EventTracking_Id?  EventTrackingId     = null,
                                                              User_Id?           CurrentUserId       = null,
                                                              CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (Location.EVSEExists(EVSE.UId))
                return AddResult<EVSE>.Failed(EventTrackingId, EVSE,
                                              $"The given EVSE '{EVSE.UId}' already exists!");


            var newLocation = Location.Update(locationBuilder => {
                                                  locationBuilder.SetEVSE(EVSE);
                                                  locationBuilder.LastUpdated  = EVSE.LastUpdated;
                                              },
                                              out var warnings);

            if (newLocation is null)
                return AddResult<EVSE>.Failed(EventTrackingId, EVSE,
                                              warnings.First().Text.FirstText());


            var updateLocationResult = await UpdateLocation(
                                                 newLocation,
                                                 AllowDowngrades,
                                                 SkipNotifications,
                                                 EventTrackingId,
                                                 CurrentUserId,
                                                 CancellationToken
                                             );

            return updateLocationResult.IsSuccess
                       ? AddResult<EVSE>.Success    (EventTrackingId, EVSE)
                       : AddResult<EVSE>.NoOperation(EventTrackingId, EVSE,
                                                     updateLocationResult.ErrorResponse);

        }

        #endregion

        #region AddOrUpdateEVSE        (Location, EVSE,                            AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<EVSE>> AddOrUpdateEVSE(Location           Location,
                                                                   EVSE               EVSE,
                                                                   Boolean?           AllowDowngrades     = false,
                                                                   Boolean            SkipNotifications   = false,
                                                                   EventTracking_Id?  EventTrackingId     = null,
                                                                   User_Id?           CurrentUserId       = null,
                                                                   CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            if (Location.TryGetEVSE(EVSE.UId, out var existingEVSE) &&
                existingEVSE is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    EVSE.LastUpdated <= existingEVSE.LastUpdated)
                {
                    return AddOrUpdateResult<EVSE>.Failed     (EventTrackingId, EVSE,
                                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");
                }

                //if (EVSE.LastUpdated.ToISO8601() == existingEVSE.LastUpdated.ToISO8601())
                //    return AddOrUpdateResult<EVSE>.NoOperation(EVSE,
                //                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");

            }

            #endregion

            var newLocation = Location.Update(locationBuilder => {

                                                  if (EVSE.Status != StatusType.REMOVED || KeepRemovedEVSEs(EVSE))
                                                      locationBuilder.SetEVSE(EVSE);
                                                  else
                                                      locationBuilder.RemoveEVSE(EVSE);

                                                  locationBuilder.LastUpdated  = EVSE.LastUpdated;

                                              },
                                              out var warnings);

            if (newLocation is null)
                return AddOrUpdateResult<EVSE>.Failed(EventTrackingId, EVSE,
                                                      warnings.First().Text.FirstText());


            var updateLocationResult = await UpdateLocation(
                                                 newLocation,
                                                 AllowDowngrades ?? this.AllowDowngrades,
                                                 SkipNotifications,
                                                 EventTrackingId,
                                                 CurrentUserId,
                                                 CancellationToken
                                             );

            return updateLocationResult.IsSuccess
                       ? existingEVSE is null
                             ? AddOrUpdateResult<EVSE>.Created(EventTrackingId, EVSE)
                             : AddOrUpdateResult<EVSE>.Updated(EventTrackingId, EVSE)
                       : AddOrUpdateResult<EVSE>.Failed(
                             EventTrackingId, EVSE,
                             updateLocationResult.ErrorResponse ?? "Unknown error!"
                         );

        }

        #endregion

        #region UpdateEVSE             (Location, EVSE,                            AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<EVSE>> UpdateEVSE(Location           Location,
                                                         EVSE               EVSE,
                                                         Boolean?           AllowDowngrades     = false,
                                                         Boolean            SkipNotifications   = false,
                                                         EventTracking_Id?  EventTrackingId     = null,
                                                         User_Id?           CurrentUserId       = null,
                                                         CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            if (Location.TryGetEVSE(EVSE.UId, out var existingEVSE) &&
                existingEVSE is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    EVSE.LastUpdated <= existingEVSE.LastUpdated)
                {
                    return UpdateResult<EVSE>.Failed(EventTrackingId, EVSE,
                                                     "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");
                }

                //if (EVSE.LastUpdated.ToISO8601() == existingEVSE.LastUpdated.ToISO8601())
                //    return AddOrUpdateResult<EVSE>.NoOperation(EVSE,
                //                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");

            }
            else
                return UpdateResult<EVSE>.Failed(EventTrackingId, EVSE,
                                                 $"The given EVSE '{EVSE.UId}' does not exist!");

            #endregion

            var newLocation = Location.Update(locationBuilder => {

                                                  if (EVSE.Status != StatusType.REMOVED || KeepRemovedEVSEs(EVSE))
                                                      locationBuilder.SetEVSE(EVSE);
                                                  else
                                                      locationBuilder.RemoveEVSE(EVSE);

                                                  locationBuilder.LastUpdated  = EVSE.LastUpdated;

                                              },
                                              out var warnings);

            if (newLocation is null)
                return UpdateResult<EVSE>.Failed(EventTrackingId, EVSE,
                                                 warnings.First().Text.FirstText());


            var updateLocationResult = await UpdateLocation(
                                                 newLocation,
                                                 AllowDowngrades ?? this.AllowDowngrades,
                                                 SkipNotifications,
                                                 EventTrackingId,
                                                 CurrentUserId,
                                                 CancellationToken
                                             );

            return updateLocationResult.IsSuccess
                       ? UpdateResult<EVSE>.Success(EventTrackingId, EVSE)
                       : UpdateResult<EVSE>.Failed (EventTrackingId, EVSE,
                                                    updateLocationResult.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region TryPatchEVSE           (Location, EVSE, EVSEPatch,                 AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<EVSE>> TryPatchEVSE(Location           Location,
                                                          EVSE               EVSE,
                                                          JObject            EVSEPatch,
                                                          Boolean?           AllowDowngrades     = false,
                                                          Boolean            SkipNotifications   = false,
                                                          EventTracking_Id?  EventTrackingId     = null,
                                                          User_Id?           CurrentUserId       = null,
                                                          CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (!EVSEPatch.HasValues)
                return PatchResult<EVSE>.Failed(EventTrackingId, EVSE,
                                                "The given EVSE patch must not be null or empty!");

            var patchResult = EVSE.TryPatch(EVSEPatch,
                                            AllowDowngrades ?? this.AllowDowngrades ?? false);

            //var justAStatusChange  = EVSEPatch.Children().Count() == 2 && EVSEPatch.ContainsKey("status") && EVSEPatch.ContainsKey("last_updated");

            if (patchResult.IsSuccess &&
                patchResult.PatchedData is not null)
            {

                var updateEVSEResult = await UpdateEVSE(
                                                 Location,
                                                 patchResult.PatchedData,
                                                 AllowDowngrades,
                                                 SkipNotifications,
                                                 EventTrackingId,
                                                 CurrentUserId,
                                                 CancellationToken
                                             );

                if (updateEVSEResult.IsFailed)
                    return PatchResult<EVSE>.Failed(EventTrackingId, EVSE,
                                                    updateEVSEResult.ErrorResponse ?? "Unknown error!");

            }

            return patchResult;

        }

        #endregion


        #region AddOrUpdateEVSEs       (Location, EVSEs,                           AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<IEnumerable<EVSE>>> AddOrUpdateEVSEs(Location           Location,
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

            foreach (var location in locations.Values)
            {
                if (location.TryGetEVSE(EVSE_UId, out EVSE))
                    return true;
            }

            return false;

        }

        #endregion

        #region Connectors

        #region AddConnector            (Location, EVSE, Connector,                                          SkipNotifications = false)

        public async Task<AddResult<Connector>> AddConnector(Location           Location,
                                                             EVSE               EVSE,
                                                             Connector          Connector,
                                                             Boolean            SkipNotifications   = false,
                                                             EventTracking_Id?  EventTrackingId     = null,
                                                             User_Id?           CurrentUserId       = null,
                                                             CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (EVSE.ConnectorExists(Connector.Id))
                return AddResult<Connector>.Failed(EventTrackingId, Connector,
                                                   $"The given charging connector identification '{Connector.Id}' already exists!");


            var newEVSE = EVSE.Update(evseBuilder => {
                                          evseBuilder.SetConnector(Connector);
                                          evseBuilder.LastUpdated = Connector.LastUpdated;
                                      },
                                      out var warnings);

            if (newEVSE is null)
                return AddResult<Connector>.Failed(EventTrackingId, Connector,
                                                   warnings.First().Text.FirstText());


            var updateEVSEResult = await UpdateEVSE(
                                             Location,
                                             newEVSE,
                                             AllowDowngrades ?? this.AllowDowngrades,
                                             SkipNotifications,
                                             EventTrackingId,
                                             CurrentUserId,
                                             CancellationToken
                                         );

            return updateEVSEResult.IsSuccess
                       ? AddResult<Connector>.Success(EventTrackingId, Connector)
                       : AddResult<Connector>.Failed (EventTrackingId, Connector,
                                                      updateEVSEResult.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region AddConnectorIfNotExists (Location, EVSE, Connector,                                          SkipNotifications = false)

        public async Task<AddResult<Connector>> AddConnectorIfNotExists(Location           Location,
                                                                        EVSE               EVSE,
                                                                        Connector          Connector,
                                                                        Boolean            SkipNotifications   = false,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        User_Id?           CurrentUserId       = null,
                                                                        CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (EVSE.ConnectorExists(Connector.Id))
                return AddResult<Connector>.Failed(EventTrackingId, Connector,
                                                   $"The given charging connector identification '{Connector.Id}' already exists!");


            var newEVSE = EVSE.Update(evseBuilder => {
                                          evseBuilder.SetConnector(Connector);
                                          evseBuilder.LastUpdated = Connector.LastUpdated;
                                      },
                                      out var warnings);

            if (newEVSE is null)
                return AddResult<Connector>.Failed(EventTrackingId, Connector,
                                                   warnings.First().Text.FirstText());


            var updateEVSEResult = await UpdateEVSE(
                                             Location,
                                             newEVSE,
                                             AllowDowngrades ?? this.AllowDowngrades,
                                             SkipNotifications,
                                             EventTrackingId,
                                             CurrentUserId,
                                             CancellationToken
                                         );

            return updateEVSEResult.IsSuccess
                       ? AddResult<Connector>.Success    (EventTrackingId, Connector)
                       : AddResult<Connector>.NoOperation(EventTrackingId, Connector,
                                                          updateEVSEResult.ErrorResponse);

        }

        #endregion

        #region AddOrUpdateConnector    (Location, EVSE, Connector,                 AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<Connector>> AddOrUpdateConnector(Location           Location,
                                                                             EVSE               EVSE,
                                                                             Connector          Connector,
                                                                             Boolean?           AllowDowngrades     = false,
                                                                             Boolean            SkipNotifications   = false,
                                                                             EventTracking_Id?  EventTrackingId     = null,
                                                                             User_Id?           CurrentUserId       = null,
                                                                             CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            if (EVSE.TryGetConnector(Connector.Id, out var existingConnector) &&
                existingConnector is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Connector.LastUpdated <= existingConnector.LastUpdated)
                {
                    return AddOrUpdateResult<Connector>.Failed(EventTrackingId, Connector,
                                                               "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");
                }

                //if (newOrUpdatedConnector.LastUpdated.ToISO8601() == existingConnector.LastUpdated.ToISO8601())
                //    return AddOrUpdateResult<Connector>.NoOperation(newOrUpdatedConnector,
                //                                                    "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");

            }

            #endregion


            var newEVSE = EVSE.Update(evseBuilder => {
                                          evseBuilder.SetConnector(Connector);
                                          evseBuilder.LastUpdated  = Connector.LastUpdated;
                                      },
                                      out var warnings);

            if (newEVSE is null)
                return AddOrUpdateResult<Connector>.Failed(EventTrackingId, Connector,
                                                           warnings.First().Text.FirstText());


            var result = await AddOrUpdateEVSE(
                                   Location,
                                   newEVSE,
                                   AllowDowngrades ?? this.AllowDowngrades,
                                   SkipNotifications,
                                   EventTrackingId,
                                   CurrentUserId,
                                   CancellationToken
                               );

            if (result.IsSuccess)
            {

                if (!SkipNotifications)
                {
                    var OnLocationChangedLocal = OnLocationChanged;
                    if (OnLocationChangedLocal is not null)
                    {
                        try
                        {
                            if (Connector.ParentEVSE?.ParentLocation is not null)
                                await OnLocationChangedLocal(Connector.ParentEVSE.ParentLocation);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateConnector), " ", nameof(OnLocationChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }
                }

                return result.WasCreated ?? false
                           ? AddOrUpdateResult<Connector>.Created(EventTrackingId, Connector)
                           : AddOrUpdateResult<Connector>.Updated(EventTrackingId, Connector);

            }

            return AddOrUpdateResult<Connector>.Failed(EventTrackingId, Connector,
                                                       result.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region UpdateConnector         (Location, EVSE, Connector,                 AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<Connector>> UpdateConnector(Location           Location,
                                                                   EVSE               EVSE,
                                                                   Connector          Connector,
                                                                   Boolean?           AllowDowngrades     = false,
                                                                   Boolean            SkipNotifications   = false,
                                                                   EventTracking_Id?  EventTrackingId     = null,
                                                                   User_Id?           CurrentUserId       = null,
                                                                   CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            if (EVSE.TryGetConnector(Connector.Id, out var existingConnector) &&
                existingConnector is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Connector.LastUpdated <= existingConnector.LastUpdated)
                {
                    return UpdateResult<Connector>.Failed(EventTrackingId, Connector,
                                                          "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");
                }

                //if (newOrUpdatedConnector.LastUpdated.ToISO8601() == existingConnector.LastUpdated.ToISO8601())
                //    return AddOrUpdateResult<Connector>.NoOperation(newOrUpdatedConnector,
                //                                                    "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");

            }
            else
                return UpdateResult<Connector>.Failed(EventTrackingId, Connector,
                                                      $"The given charging connector '{Connector.Id}' does not exist!");

            #endregion


            var newEVSE = EVSE.Update(evseBuilder => {
                                          evseBuilder.SetConnector(Connector);
                                          evseBuilder.LastUpdated  = Connector.LastUpdated;
                                      },
                                      out var warnings);

            if (newEVSE is null)
                return UpdateResult<Connector>.Failed(EventTrackingId, Connector,
                                                      warnings.First().Text.FirstText());


            var updateEVSEResult = await UpdateEVSE(
                                             Location,
                                             newEVSE,
                                             AllowDowngrades ?? this.AllowDowngrades,
                                             SkipNotifications,
                                             EventTrackingId,
                                             CurrentUserId,
                                             CancellationToken
                                         );

            return updateEVSEResult.IsSuccess
                       ? UpdateResult<Connector>.Success(EventTrackingId, Connector)
                       : UpdateResult<Connector>.Failed (EventTrackingId, Connector,
                                                         updateEVSEResult.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region TryPatchConnector       (Location, EVSE, Connector, ConnectorPatch, AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<Connector>> TryPatchConnector(Location           Location,
                                                                    EVSE               EVSE,
                                                                    Connector          Connector,
                                                                    JObject            ConnectorPatch,
                                                                    Boolean?           AllowDowngrades     = false,
                                                                    Boolean            SkipNotifications   = false,
                                                                    EventTracking_Id?  EventTrackingId     = null,
                                                                    User_Id?           CurrentUserId       = null,
                                                                    CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (!ConnectorPatch.HasValues)
                return PatchResult<Connector>.Failed(EventTrackingId, Connector,
                                                     "The given connector patch must not be null or empty!");

            var patchResult = Connector.TryPatch(ConnectorPatch,
                                                 AllowDowngrades ?? this.AllowDowngrades ?? false);

            if (patchResult.IsSuccess &&
                patchResult.PatchedData is not null)
            {

                var updateConnectorResult = await UpdateConnector(
                                                      Location,
                                                      EVSE,
                                                      patchResult.PatchedData,
                                                      AllowDowngrades,
                                                      SkipNotifications,
                                                      EventTrackingId,
                                                      CurrentUserId,
                                                      CancellationToken
                                                  );

                if (updateConnectorResult.IsFailed)
                    return PatchResult<Connector>.Failed(EventTrackingId, Connector,
                                                         updateConnectorResult.ErrorResponse ?? "Unknown error!");

            }

            return patchResult;

        }

        #endregion



        #endregion

        #endregion

        #region Tariffs

        #region Data

        private readonly TimeRangeDictionary<Tariff_Id , Tariff> tariffs = [];


        public delegate Task OnTariffAddedDelegate  (Tariff               Tariff);
        public delegate Task OnTariffChangedDelegate(Tariff               Tariff);
        public delegate Task OnTariffRemovedDelegate(IEnumerable<Tariff>  Tariff);

        public event OnTariffAddedDelegate?    OnTariffAdded;
        public event OnTariffChangedDelegate?  OnTariffChanged;
        public event OnTariffRemovedDelegate?  OnTariffRemoved;

        #endregion


        public GetTariffs2_Delegate?    GetTariffsDelegate      { get; set; }

        public GetTariffIds2_Delegate?  GetTariffIdsDelegate    { get; set; }


        #region AddTariff            (Tariff,                                       SkipNotifications = false, ...)

        public async Task<AddResult<Tariff>> AddTariff(Tariff             Tariff,
                                                       Boolean            SkipNotifications   = false,
                                                       EventTracking_Id?  EventTrackingId     = null,
                                                       User_Id?           CurrentUserId       = null,
                                                       CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addTariff,
                          Tariff.ToJSON(true,
                                        true,
                                        CustomTariffSerializer,
                                        CustomDisplayTextSerializer,
                                        CustomTariffElementSerializer,
                                        CustomPriceComponentSerializer,
                                        CustomTariffRestrictionsSerializer,
                                        CustomEnergyMixSerializer,
                                        CustomEnergySourceSerializer,
                                        CustomEnvironmentalImpactSerializer),
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
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddTariff), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Tariff>.Success(EventTrackingId, Tariff);

            }

            return AddResult<Tariff>.Failed(EventTrackingId, Tariff,
                                            "TryAdd(Tariff.Id, Tariff) failed!");

        }

        #endregion

        #region AddTariffIfNotExists (Tariff,                                       SkipNotifications = false, ...)

        public async Task<AddResult<Tariff>> AddTariffIfNotExists(Tariff             Tariff,
                                                                  Boolean            SkipNotifications   = false,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  User_Id?           CurrentUserId       = null,
                                                                  CancellationToken  CancellationToken   = default)
        {


            EventTrackingId ??= EventTracking_Id.New;

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addTariffIfNotExists,
                          Tariff.ToJSON(true,
                                        true,
                                        CustomTariffSerializer,
                                        CustomDisplayTextSerializer,
                                        CustomTariffElementSerializer,
                                        CustomPriceComponentSerializer,
                                        CustomTariffRestrictionsSerializer,
                                        CustomEnergyMixSerializer,
                                        CustomEnergySourceSerializer,
                                        CustomEnvironmentalImpactSerializer),
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
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddTariffIfNotExists), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Tariff>.Success(EventTrackingId, Tariff);

            }

            return AddResult<Tariff>.NoOperation(EventTrackingId, Tariff);

        }

        #endregion

        #region AddOrUpdateTariff    (Tariff,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<AddOrUpdateResult<Tariff>> AddOrUpdateTariff(Tariff             Tariff,
                                                                       Boolean?           AllowDowngrades     = false,
                                                                       Boolean            SkipNotifications   = false,
                                                                       EventTracking_Id?  EventTrackingId     = null,
                                                                       User_Id?           CurrentUserId       = null,
                                                                       CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Update an existing tariff

            if (tariffs.TryGetValue(Tariff.Id,
                                    out var existingTariff,
                                    Tariff.NotBefore ?? DateTimeOffset.MinValue))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Tariff.LastUpdated <= existingTariff.LastUpdated)
                {
                    return AddOrUpdateResult<Tariff>.Failed(EventTrackingId, Tariff,
                                                            "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!");
                }

                tariffs.AddOrUpdate(Tariff.Id, Tariff);
                Tariff.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addOrUpdateTariff,
                          Tariff.ToJSON(true,
                                        true,
                                        CustomTariffSerializer,
                                        CustomDisplayTextSerializer,
                                        CustomTariffElementSerializer,
                                        CustomPriceComponentSerializer,
                                        CustomTariffRestrictionsSerializer,
                                        CustomEnergyMixSerializer,
                                        CustomEnergySourceSerializer,
                                        CustomEnvironmentalImpactSerializer),
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
                            OnTariffChangedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Tariff>.Updated(EventTrackingId, Tariff);

            }

            #endregion

            #region Add a new tariff

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addOrUpdateTariff,
                          Tariff.ToJSON(true,
                                        true,
                                        CustomTariffSerializer,
                                        CustomDisplayTextSerializer,
                                        CustomTariffElementSerializer,
                                        CustomPriceComponentSerializer,
                                        CustomTariffRestrictionsSerializer,
                                        CustomEnergyMixSerializer,
                                        CustomEnergySourceSerializer,
                                        CustomEnvironmentalImpactSerializer),
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
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Tariff>.Created(EventTrackingId, Tariff);

            }

            return AddOrUpdateResult<Tariff>.Failed(EventTrackingId, Tariff,
                                                    "AddOrUpdateTariff(Tariff.Id, Tariff) failed!");

            #endregion

        }

        #endregion

        #region UpdateTariff         (Tariff,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<UpdateResult<Tariff>> UpdateTariff(Tariff             Tariff,
                                                             Boolean?           AllowDowngrades     = false,
                                                             Boolean            SkipNotifications   = false,
                                                             EventTracking_Id?  EventTrackingId     = null,
                                                             User_Id?           CurrentUserId       = null,
                                                             CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            if (tariffs.TryGetValue(Tariff.Id, out var existingTariff, Timestamp.Now))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Tariff.LastUpdated <= existingTariff.LastUpdated)
                {

                    return UpdateResult<Tariff>.Failed(EventTrackingId, Tariff,
                                                       "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!");

                }

            }
            else
                return UpdateResult<Tariff>.Failed(EventTrackingId, Tariff,
                                                   $"Unknown tariff identification '{Tariff.Id}'!");

            #endregion

            if (tariffs.TryUpdate(Tariff.Id, Tariff, existingTariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.updateTariff,
                          Tariff.ToJSON(true,
                                        true,
                                        CustomTariffSerializer,
                                        CustomDisplayTextSerializer,
                                        CustomTariffElementSerializer,
                                        CustomPriceComponentSerializer,
                                        CustomTariffRestrictionsSerializer,
                                        CustomEnergyMixSerializer,
                                        CustomEnergySourceSerializer,
                                        CustomEnvironmentalImpactSerializer),
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
                            OnTariffChangedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<Tariff>.Success(EventTrackingId, Tariff);

            }

            return UpdateResult<Tariff>.Failed(EventTrackingId, Tariff,
                                               "UpdateTariff(Tariff.Id, Tariff, Tariff) failed!");

        }

        #endregion

        #region TryPatchTariff       (Tariff, TariffPatch, AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<PatchResult<Tariff>> TryPatchTariff(Tariff             Tariff,
                                                              JObject            TariffPatch,
                                                              Boolean?           AllowDowngrades     = false,
                                                              Boolean            SkipNotifications   = false,
                                                              EventTracking_Id?  EventTrackingId     = null,
                                                              User_Id?           CurrentUserId       = null,
                                                              CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (!TariffPatch.HasValues)
                return PatchResult<Tariff>.Failed(EventTrackingId, Tariff,
                                                  "The given charging tariff patch must not be null or empty!");

            if (tariffs.TryGetValue(Tariff.Id, out var existingTariff, Timestamp.Now))
            {

                var patchResult = existingTariff.TryPatch(TariffPatch,
                                                          AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    tariffs.TryUpdate(Tariff.Id, Tariff, patchResult.PatchedData);

                    await LogAsset(
                              CommonHTTPAPI.updateTariff,
                              Tariff.ToJSON(true,
                                            true,
                                            CustomTariffSerializer,
                                            CustomDisplayTextSerializer,
                                            CustomTariffElementSerializer,
                                            CustomPriceComponentSerializer,
                                            CustomTariffRestrictionsSerializer,
                                            CustomEnergyMixSerializer,
                                            CustomEnergySourceSerializer,
                                            CustomEnvironmentalImpactSerializer),
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
                                OnTariffChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchTariff), " ", nameof(OnTariffChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<Tariff>.Failed(EventTrackingId, Tariff,
                                                  "The given charging tariff does not exist!");

        }

        #endregion


        #region RemoveTariff         (Tariff,                                       SkipNotifications = false, ...)

        /// <summary>
        /// Remove the given charging tariff.
        /// </summary>
        /// <param name="Tariff">A charging tariff.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<IEnumerable<Tariff>>> RemoveTariff(Tariff             Tariff,
                                                                    Boolean            SkipNotifications   = false,
                                                                    EventTracking_Id?  EventTrackingId     = null,
                                                                    User_Id?           CurrentUserId       = null,
                                                                    CancellationToken  CancellationToken   = default)

            => RemoveTariff(Tariff.Id,
                            SkipNotifications,
                            EventTrackingId,
                            CurrentUserId,
                            CancellationToken);

        #endregion

        #region RemoveTariff         (TariffId,                                     SkipNotifications = false, ...)

        /// <summary>
        /// Remove the given charging tariff.
        /// </summary>
        /// <param name="TariffId">An unique charging tariff identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveTariff(Tariff_Id          TariffId,
                                                                          Boolean            SkipNotifications   = false,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          User_Id?           CurrentUserId       = null,
                                                                          CancellationToken  CancellationToken   = default)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (tariffs.TryRemove(TariffId, out var removedTariffs))
            {

                await LogAsset(
                          CommonHTTPAPI.removeTariff,
                          new JArray(removedTariffs.Select(removedTariff => removedTariff.ToJSON(true,
                                                                                                 true,
                                                                                                 CustomTariffSerializer,
                                                                                                 CustomDisplayTextSerializer,
                                                                                                 CustomTariffElementSerializer,
                                                                                                 CustomPriceComponentSerializer,
                                                                                                 CustomTariffRestrictionsSerializer,
                                                                                                 CustomEnergyMixSerializer,
                                                                                                 CustomEnergySourceSerializer,
                                                                                                 CustomEnvironmentalImpactSerializer))),
                          EventTrackingId,
                          CurrentUserId,
                          CancellationToken
                      );

                if (!SkipNotifications)
                {

                    var OnTariffRemovedLocal = OnTariffRemoved;
                    if (OnTariffRemovedLocal is not null)
                    {
                        try
                        {
                            await OnTariffRemovedLocal(removedTariffs);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveTariff), " ", nameof(OnTariffRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<IEnumerable<Tariff>>.Success(EventTrackingId, removedTariffs);

            }

            return RemoveResult<IEnumerable<Tariff>>.Failed(EventTrackingId,
                                                            "RemoveTariff(TariffId, ...) failed!");

        }

        #endregion

        #region RemoveAllTariffs     (                                              SkipNotifications = false, ...)

        /// <summary>
        /// Remove all charging tariffs.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Boolean            SkipNotifications   = false,
                                                                              EventTracking_Id?  EventTrackingId     = null,
                                                                              User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var existingTariffs = tariffs.Values().ToArray();

            tariffs.Clear();

            await LogAsset(
                      CommonHTTPAPI.removeAllTariffs,
                      EventTrackingId,
                      CurrentUserId
                  );

            if (!SkipNotifications)
            {

                var OnTariffRemovedLocal = OnTariffRemoved;
                if (OnTariffRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var existingTariff in existingTariffs)
                            await OnTariffRemovedLocal([ existingTariff ]);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllTariffs), " ", nameof(OnTariffRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<Tariff>>.Success(EventTrackingId, existingTariffs);

        }

        #endregion

        #region RemoveAllTariffs     (IncludeTariffs,                               SkipNotifications = false, ...)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeTariffs">A charging tariff filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Func<Tariff, Boolean>  IncludeTariffs,
                                                                              Boolean                SkipNotifications   = false,
                                                                              EventTracking_Id?      EventTrackingId     = null,
                                                                              User_Id?               CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs  = new List<Tariff>();
            var failedTariffs   = new List<RemoveResult<IEnumerable<Tariff>>>();

            foreach (var tariff in tariffs.Values().Where(IncludeTariffs).ToArray())
            {

                var result = await RemoveTariff(
                                       tariff.Id,
                                       SkipNotifications
                                   );

                if (result.IsSuccess)
                    removedTariffs.Add(tariff);
                else
                    failedTariffs. Add(result);

            }

            return removedTariffs.Count != 0 && failedTariffs.Count == 0
                       ? RemoveResult<IEnumerable<Tariff>>.Success(EventTrackingId, removedTariffs)

                       : removedTariffs.Count == 0 && failedTariffs.Count == 0
                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation(EventTrackingId, [])
                             : RemoveResult<IEnumerable<Tariff>>.Failed     (EventTrackingId, failedTariffs.SelectMany(tariff => tariff.Data ?? []),
                                                                             failedTariffs.Select    (tariff => tariff.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllTariffs     (IncludeTariffIds,                             SkipNotifications = false, ...)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeTariffIds">A charging tariff identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Func<Tariff_Id, Boolean>  IncludeTariffIds,
                                                                              Boolean                   SkipNotifications   = false,
                                                                              EventTracking_Id?         EventTrackingId     = null,
                                                                              User_Id?                  CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs  = new List<Tariff>();
            var failedTariffs   = new List<RemoveResult<IEnumerable<Tariff>>>();

            foreach (var tariffsToRemove in tariffs.Where  (kvp => IncludeTariffIds(kvp.Key)).
                                                    Select (kvp => kvp.Value).
                                                    ToArray())
            {

                var result = await RemoveTariff(tariffsToRemove.First().Id,
                                                SkipNotifications);

                if (result.IsSuccess)
                    removedTariffs.AddRange(tariffsToRemove);
                else
                    failedTariffs. Add(result);

            }

            return removedTariffs.Count != 0 && failedTariffs.Count == 0
                       ? RemoveResult<IEnumerable<Tariff>>.Success(EventTrackingId, removedTariffs)

                       : removedTariffs.Count == 0 && failedTariffs.Count == 0
                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation(EventTrackingId, [])
                             : RemoveResult<IEnumerable<Tariff>>.Failed     (EventTrackingId, failedTariffs.SelectMany(tariff => tariff.Data ?? []),
                                                                             failedTariffs.Select    (tariff => tariff.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllTariffs     (CountryCode, PartyId,                         SkipNotifications = false, ...)

        /// <summary>
        /// Remove all charging tariffs owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(CountryCode        CountryCode,
                                                                              Party_Id           PartyId,
                                                                              Boolean            SkipNotifications   = false,
                                                                              EventTracking_Id?  EventTrackingId     = null,
                                                                              User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            await LogAssetComment(
                      $"{CommonHTTPAPI.removeAllTariffs}: {CountryCode} {PartyId}",
                      EventTrackingId,
                      CurrentUserId
                  );

            var removedTariffs  = new List<Tariff>();
            var failedTariffs   = new List<RemoveResult<IEnumerable<Tariff>>>();

            foreach (var tariff in tariffs.Values().Where  (tariff => CountryCode == tariff.CountryCode &&
                                                                      PartyId     == tariff.PartyId).
                                                    ToArray())
            {

                var result = await RemoveTariff(tariff.Id,
                                                SkipNotifications);

                if (result.IsSuccess)
                    removedTariffs.Add(tariff);
                else
                    failedTariffs. Add(result);

            }

            return removedTariffs.Any() && !failedTariffs.Any()
                       ? RemoveResult<IEnumerable<Tariff>>.Success(EventTrackingId, removedTariffs)

                       : !removedTariffs.Any() && !failedTariffs.Any()
                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation(EventTrackingId, Array.Empty<Tariff>())
                             : RemoveResult<IEnumerable<Tariff>>.Failed     (EventTrackingId, failedTariffs.SelectMany(tariff => tariff.Data ?? []),
                                                                             failedTariffs.Select    (tariff => tariff.ErrorResponse).AggregateWith(", "));

        }

        #endregion


        #region TariffExists(TariffId, Timestamp = null, Tolerance = null)

        public Boolean TariffExists(Tariff_Id        TariffId,
                                    DateTimeOffset?  Timestamp   = null,
                                    TimeSpan?        Tolerance   = null)

            => tariffs.ContainsKey(TariffId,
                                   Timestamp,
                                   Tolerance);

        #endregion

        #region TryGetTariff(TariffId, out Tariff, Timestamp = null, Tolerance = null)

        public Boolean TryGetTariff(Tariff_Id                        TariffId,
                                    [NotNullWhen(true)] out Tariff?  Tariff,
                                    DateTimeOffset?                  Timestamp   = null,
                                    TimeSpan?                        Tolerance   = null)
        {

            if (tariffs.TryGetValue(TariffId,
                                    out Tariff,
                                    Timestamp,
                                    Tolerance))
            {
                return true;
            }

            Tariff = null;
            return false;

        }

        #endregion

        #region GetTariff   (TariffId, Timestamp = null, Tolerance = null)

        public Tariff? GetTariff(Tariff_Id        TariffId,
                                 DateTimeOffset?  Timestamp   = null,
                                 TimeSpan?        Tolerance   = null)
        {

            if (tariffs.TryGetValue(TariffId,
                                    out var tariff,
                                    Timestamp,
                                    Tolerance))
            {
                return tariff;
            }

            return null;

        }

        #endregion

        #region GetTariffs  (IncludeTariff = null, Timestamp = null, Tolerance = null)

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, Boolean>?  IncludeTariff   = null,
                                              DateTimeOffset?         Timestamp       = null,
                                              TimeSpan?               Tolerance       = null)

            => IncludeTariff is null
                   ? tariffs.Values(Timestamp, Tolerance)
                   : tariffs.Values(Timestamp, Tolerance).Where(IncludeTariff);

        #endregion

        #region GetTariffs  (CountryCode, PartyId, Timestamp = null, Tolerance = null)

        public IEnumerable<Tariff> GetTariffs(CountryCode      CountryCode,
                                              Party_Id         PartyId,
                                              DateTimeOffset?  Timestamp   = null,
                                              TimeSpan?        Tolerance   = null)

            => tariffs.Values(Timestamp, Tolerance).
                       Where (tariff => tariff.CountryCode == CountryCode &&
                                        tariff.PartyId     == PartyId);

        #endregion


        #region GetTariffIds(CountryCode?, PartyId?, LocationId?, EVSEUId?, ConnectorId?, EMSPId?)

        public IEnumerable<Tariff_Id> GetTariffIds(CountryCode    CountryCode,
                                                   Party_Id       PartyId,
                                                   Location_Id?   LocationId,
                                                   EVSE_Id?       EVSEUId,
                                                   Connector_Id?  ConnectorId,
                                                   EMSP_Id?       EMSPId)
        {

            return GetTariffIdsDelegate?.Invoke(CountryCode,
                                                PartyId,
                                                LocationId,
                                                EVSEUId,
                                                ConnectorId,
                                                EMSPId) ?? [];

        }

        #endregion

        #endregion

        #region Tokens

        #region Data

        private readonly ConcurrentDictionary<Token_Id, TokenStatus> tokenStatus = [];


        public delegate Task               OnTokenAddedDelegate  (Token     Token);
        public delegate Task               OnTokenChangedDelegate(Token     Token);
        public delegate Task               OnTokenRemovedDelegate(Token     Token);
        public delegate Task<TokenStatus>  OnVerifyTokenDelegate (Token_Id  TokenId);

        public event OnTokenAddedDelegate?    OnTokenAdded;
        public event OnTokenChangedDelegate?  OnTokenChanged;
        public event OnTokenRemovedDelegate?  OnTokenRemoved;
        public event OnVerifyTokenDelegate?   OnVerifyToken;

        #endregion


        #region AddToken           (Token, Status = AllowedTypes.ALLOWED,                          SkipNotifications = false)

        public async Task<AddResult<Token>> AddToken(Token              Token,
                                                     AllowedType?       Status              = null,
                                                     Boolean            SkipNotifications   = false,
                                                     EventTracking_Id?  EventTrackingId     = null,
                                                     User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var newTokenStatus = new TokenStatus(Token,
                                                 Status ??= AllowedType.ALLOWED);

            if (tokenStatus.TryAdd(Token.Id, newTokenStatus))
            {

                Token.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addTokenStatus,
                          newTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                true,
                                                CustomTokenSerializer,
                                                CustomLocationReferenceSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnTokenAddedLocal = OnTokenAdded;
                    if (OnTokenAddedLocal is not null)
                    {
                        try
                        {
                            OnTokenAddedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddToken), " ", nameof(OnTokenAddedLocal), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Token>.Success(EventTrackingId, Token);

            }

            return AddResult<Token>.Failed(EventTrackingId, Token,
                                           "TryAdd(Token.Id, newTokenStatus) failed!");

        }

        #endregion

        #region AddTokenIfNotExists(Token, Status = AllowedTypes.ALLOWED,                          SkipNotifications = false)

        public async Task<AddResult<Token>> AddTokenIfNotExists(Token              Token,
                                                                AllowedType?       Status              = null,
                                                                Boolean            SkipNotifications   = false,
                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var newTokenStatus = new TokenStatus(Token,
                                                 Status ??= AllowedType.ALLOWED);

            if (tokenStatus.TryAdd(Token.Id, newTokenStatus))
            {

                Token.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addTokenStatus,
                          newTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                true,
                                                CustomTokenSerializer,
                                                CustomLocationReferenceSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnTokenAddedLocal = OnTokenAdded;
                    if (OnTokenAddedLocal is not null)
                    {
                        try
                        {
                            OnTokenAddedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddToken), " ", nameof(OnTokenAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Token>.Success(EventTrackingId, Token);

            }

            return AddResult<Token>.NoOperation(EventTrackingId, Token);

        }

        #endregion

        #region AddOrUpdateToken   (Token, Status = AllowedTypes.ALLOWED, AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<Token>> AddOrUpdateToken(Token              Token,
                                                                     AllowedType?       Status              = null,
                                                                     Boolean?           AllowDowngrades     = false,
                                                                     Boolean            SkipNotifications   = false,
                                                                     EventTracking_Id?  EventTrackingId     = null,
                                                                     User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Update an existing token

            if (tokenStatus.TryGetValue(Token.Id, out var existingTokenStatus))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Token.LastUpdated <= existingTokenStatus.Token.LastUpdated)
                {
                    return AddOrUpdateResult<Token>.Failed(EventTrackingId, Token,
                                                           "The 'lastUpdated' timestamp of the new token must be newer then the timestamp of the existing token!");
                }

                var updatedTokenStatus = new TokenStatus(Token,
                                                         Status ?? existingTokenStatus.Status);

                tokenStatus[Token.Id] = updatedTokenStatus;
                Token.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addOrUpdateTokenStatus,
                          updatedTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                    true,
                                                    CustomTokenSerializer,
                                                    CustomLocationReferenceSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnTokenChangedLocal = OnTokenChanged;
                    if (OnTokenChangedLocal is not null)
                    {
                        try
                        {
                            OnTokenChangedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateToken), " ", nameof(OnTokenChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Token>.Updated(EventTrackingId, Token);

            }

            #endregion

            #region Add a new token

            var newTokenStatus = new TokenStatus(Token,
                                                 Status ??= AllowedType.ALLOWED);

            if (tokenStatus.TryAdd(Token.Id, newTokenStatus))
            {

                Token.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addOrUpdateTokenStatus,
                          newTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                true,
                                                CustomTokenSerializer,
                                                CustomLocationReferenceSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnTokenAddedLocal = OnTokenAdded;
                    if (OnTokenAddedLocal is not null)
                    {
                        try
                        {
                            OnTokenAddedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateToken), " ", nameof(OnTokenAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Token>.Created(EventTrackingId, Token);

            }

            return AddOrUpdateResult<Token>.Failed(EventTrackingId, Token,
                                                   "AddOrUpdateToken(Token.Id, Token) failed!");

            #endregion

        }

        #endregion

        #region UpdateToken        (Token, Status = AllowedTypes.ALLOWED, AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<Token>> UpdateToken(Token              Token,
                                                           AllowedType?       Status              = null,
                                                           Boolean?           AllowDowngrades     = false,
                                                           Boolean            SkipNotifications   = false,
                                                           EventTracking_Id?  EventTrackingId     = null,
                                                           User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            if (tokenStatus.TryGetValue(Token.Id, out var existingTokenStatus))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Token.LastUpdated <= existingTokenStatus.Token.LastUpdated)
                {

                    return UpdateResult<Token>.Failed(EventTrackingId, Token,
                                                      "The 'lastUpdated' timestamp of the new charging token must be newer then the timestamp of the existing token!");

                }

            }
            else
                return UpdateResult<Token>.Failed(EventTrackingId, Token,
                                                  $"Unknown token identification '{Token.Id}'!");

            #endregion


            var updatedTokenStatus = new TokenStatus(Token,
                                                     AllowedType.ALLOWED);

            if (tokenStatus.TryUpdate(Token.Id,
                                      updatedTokenStatus,
                                      existingTokenStatus))
            {

                Token.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.updateTokenStatus,
                          updatedTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                    true,
                                                    CustomTokenSerializer,
                                                    CustomLocationReferenceSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnTokenChangedLocal = OnTokenChanged;
                    if (OnTokenChangedLocal is not null)
                    {
                        try
                        {
                            OnTokenChangedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateToken), " ", nameof(OnTokenChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<Token>.Success(EventTrackingId, Token);

            }

            return UpdateResult<Token>.Failed(EventTrackingId, Token,
                                              "UpdateToken(Token.Id, Token, Token) failed!");

        }

        #endregion


        #region TryPatchToken      (Token, TokenPatch, AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<Token>> TryPatchToken(Token              Token,
                                                            JObject            TokenPatch,
                                                            Boolean?           AllowDowngrades     = false,
                                                            Boolean            SkipNotifications   = false,
                                                            EventTracking_Id?  EventTrackingId     = null,
                                                            User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (TokenPatch is null || !TokenPatch.HasValues)
                return PatchResult<Token>.Failed(EventTrackingId, Token,
                                                 "The given token patch must not be null or empty!");

            if (tokenStatus.TryGetValue(Token.Id, out var existingTokenStatus))
            {

                var patchResult = existingTokenStatus.Token.TryPatch(TokenPatch,
                                                                     AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    var patchedTokenStatus = new TokenStatus(patchResult.PatchedData,
                                                             existingTokenStatus.Status);

                    tokenStatus[Token.Id] = patchedTokenStatus;

                    await LogAsset(
                              CommonHTTPAPI.updateTokenStatus,
                              patchedTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                        true,
                                                        CustomTokenSerializer,
                                                        CustomLocationReferenceSerializer),
                              EventTrackingId,
                              CurrentUserId
                          );

                    if (!SkipNotifications)
                    {

                        var OnTokenChangedLocal = OnTokenChanged;
                        if (OnTokenChangedLocal is not null)
                        {
                            try
                            {
                                OnTokenChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchToken), " ", nameof(OnTokenChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<Token>.Failed(EventTrackingId, Token,
                                                  "The given token does not exist!");

        }

        #endregion


        #region TokenExists(TokenId)

        public Boolean TokenExists(Token_Id TokenId)

            => tokenStatus.ContainsKey(TokenId);

        #endregion

        #region TryGetToken(TokenId, out TokenWithStatus)

        public Boolean TryGetToken(Token_Id                             TokenId,
                                   [NotNullWhen(true)] out TokenStatus  TokenWithStatus)
        {

            if (tokenStatus.TryGetValue(TokenId, out TokenWithStatus))
                return true;

            TokenWithStatus = default;
            return false;

        }

        #endregion

        #region GetTokens  (IncludeToken)

        public IEnumerable<TokenStatus> GetTokens(Func<Token, Boolean> IncludeToken)

            => IncludeToken is null
                   ? tokenStatus.Values
                   : tokenStatus.Values.Where(tokenStatus => IncludeToken(tokenStatus.Token));

        #endregion

        #region GetTokens  (IncludeTokenStatus = null)

        public IEnumerable<TokenStatus> GetTokens(Func<TokenStatus, Boolean>? IncludeTokenStatus = null)

            => IncludeTokenStatus is null
                   ? tokenStatus.Values
                   : tokenStatus.Values.Where(IncludeTokenStatus);

        #endregion

        #region GetTokens  (CountryCode, PartyId)

        public IEnumerable<TokenStatus> GetTokens(CountryCode  CountryCode,
                                                  Party_Id     PartyId)

            => tokenStatus.Values.Where(tokenStatus => tokenStatus.Token.CountryCode == CountryCode &&
                                                       tokenStatus.Token.PartyId     == PartyId);

        #endregion


        #region RemoveToken    (Token,                SkipNotifications = false)

        /// <summary>
        /// Remove the given token.
        /// </summary>
        /// <param name="Token">A token.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<Token>> RemoveToken(Token              Token,
                                                     Boolean            SkipNotifications   = false,
                                                     EventTracking_Id?  EventTrackingId     = null,
                                                     User_Id?           CurrentUserId       = null)

            => RemoveToken(Token.Id,
                           SkipNotifications);

        #endregion

        #region RemoveToken    (TokenId,              SkipNotifications = false)

        /// <summary>
        /// Remove the given token.
        /// </summary>
        /// <param name="TokenId">A unique identification of a token.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<Token>> RemoveToken(Token_Id           TokenId,
                                                           Boolean            SkipNotifications   = false,
                                                           EventTracking_Id?  EventTrackingId     = null,
                                                           User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (tokenStatus.Remove(TokenId, out var existingTokenStatus))
            {

                await LogAsset(
                          CommonHTTPAPI.removeToken,
                          existingTokenStatus.Token.ToJSON(true,
                                                           CustomTokenSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnTokenRemovedLocal = OnTokenRemoved;
                    if (OnTokenRemovedLocal is not null)
                    {
                        try
                        {
                            await OnTokenRemovedLocal(existingTokenStatus.Token);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveToken), " ", nameof(OnTokenRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<Token>.Success(EventTrackingId, existingTokenStatus.Token);

            }

            return RemoveResult<Token>.Failed(EventTrackingId,
                                              "RemoveToken(TokenId, ...) failed!");

        }

        #endregion

        #region RemoveAllTokens(                      SkipNotifications = false)

        /// <summary>
        /// Remove all tokens.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>> RemoveAllTokens(Boolean            SkipNotifications   = false,
                                                                            EventTracking_Id?  EventTrackingId     = null,
                                                                            User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var existingTokenStatus = tokenStatus.Values.ToArray();

            tokenStatus.Clear();

            await LogAsset(
                      CommonHTTPAPI.removeAllTokenStatus,
                      EventTrackingId,
                      CurrentUserId
                  );

            if (!SkipNotifications)
            {

                var OnTokenRemovedLocal = OnTokenRemoved;
                if (OnTokenRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var tokenStatus in existingTokenStatus)
                            await OnTokenRemovedLocal(tokenStatus.Token);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllTokens), " ", nameof(OnTokenRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<Token>>.Success(EventTrackingId, existingTokenStatus.Select(tokenStatus => tokenStatus.Token));

        }

        #endregion

        #region RemoveAllTokens(IncludeTokens,        SkipNotifications = false)

        /// <summary>
        /// Remove all tokens.
        /// </summary>
        /// <param name="IncludeTokens">A token filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>> RemoveAllTokens(Func<Token, Boolean>  IncludeTokens,
                                                                            Boolean               SkipNotifications   = false,
                                                                            EventTracking_Id?     EventTrackingId     = null,
                                                                            User_Id?              CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTokens  = new List<Token>();
            var failedTokens   = new List<RemoveResult<Token>>();

            foreach (var token_status in tokenStatus.Values.Where(tokenstatus => IncludeTokens(tokenstatus.Token)).ToArray())
            {

                var result = await RemoveToken(token_status.Token.Id,
                                               SkipNotifications);

                if (result.IsSuccess)
                    removedTokens.Add(token_status.Token);
                else
                    failedTokens. Add(result);

            }

            return removedTokens.Any() && !failedTokens.Any()
                       ? RemoveResult<IEnumerable<Token>>.Success(EventTrackingId, removedTokens)

                       : !removedTokens.Any() && !failedTokens.Any()
                             ? RemoveResult<IEnumerable<Token>>.NoOperation(EventTrackingId, Array.Empty<Token>())
                             : RemoveResult<IEnumerable<Token>>.Failed     (EventTrackingId, failedTokens.Select(token => token.Data)!,
                                                                            failedTokens.Select(token => token.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllTokens(IncludeTokenIds,      SkipNotifications = false)

        /// <summary>
        /// Remove all matching tokens.
        /// </summary>
        /// <param name="IncludeTokenIds">A token identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>> RemoveAllTokens(Func<Token_Id, Boolean>  IncludeTokenIds,
                                                                            Boolean                  SkipNotifications   = false,
                                                                            EventTracking_Id?        EventTrackingId     = null,
                                                                            User_Id?                 CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTokens  = new List<Token>();
            var failedTokens   = new List<RemoveResult<Token>>();

            foreach (var token_status in tokenStatus.Where  (kvp => IncludeTokenIds(kvp.Key)).
                                                     Select (kvp => kvp.Value).
                                                     ToArray())
            {

                var result = await RemoveToken(token_status.Token.Id,
                                               SkipNotifications);

                if (result.IsSuccess)
                    removedTokens.Add(token_status.Token);
                else
                    failedTokens. Add(result);

            }

            return removedTokens.Any() && !failedTokens.Any()
                       ? RemoveResult<IEnumerable<Token>>.Success(EventTrackingId, removedTokens)

                       : !removedTokens.Any() && !failedTokens.Any()
                             ? RemoveResult<IEnumerable<Token>>.NoOperation(EventTrackingId, Array.Empty<Token>())
                             : RemoveResult<IEnumerable<Token>>.Failed     (EventTrackingId, failedTokens.Select(token => token.Data)!,
                                                                             failedTokens.Select(token => token.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllTokens(CountryCode, PartyId, SkipNotifications = false)

        /// <summary>
        /// Remove all tokens owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>> RemoveAllTokens(CountryCode        CountryCode,
                                                                            Party_Id           PartyId,
                                                                            Boolean            SkipNotifications   = false,
                                                                            EventTracking_Id?  EventTrackingId     = null,
                                                                            User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTokens  = new List<Token>();
            var failedTokens   = new List<RemoveResult<Token>>();

            foreach (var token_status in tokenStatus.Values.Where  (tokenstatus => CountryCode == tokenstatus.Token.CountryCode &&
                                                                                   PartyId     == tokenstatus.Token.PartyId).
                                                            ToArray())
            {

                var result = await RemoveToken(token_status.Token.Id,
                                               SkipNotifications);

                if (result.IsSuccess)
                    removedTokens.Add(token_status.Token);
                else
                    failedTokens. Add(result);

            }

            return removedTokens.Any() && !failedTokens.Any()
                       ? RemoveResult<IEnumerable<Token>>.Success(EventTrackingId, removedTokens)

                       : !removedTokens.Any() && !failedTokens.Any()
                             ? RemoveResult<IEnumerable<Token>>.NoOperation(EventTrackingId, Array.Empty<Token>())
                             : RemoveResult<IEnumerable<Token>>.Failed     (EventTrackingId, failedTokens.Select(token => token.Data)!,
                                                                             failedTokens.Select(token => token.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #endregion

        #region Sessions

        #region Data

        private readonly ConcurrentDictionary<Session_Id , Session> chargingSessions = [];


        public delegate Task OnSessionAddedDelegate          (Session Session);
        public delegate Task OnChargingSessionChangedDelegate(Session Session);
        public delegate Task OnSessionRemovedDelegate        (Session Session);

        public event OnSessionAddedDelegate?            OnSessionAdded;
        public event OnChargingSessionChangedDelegate?  OnSessionChanged;
        public event OnSessionRemovedDelegate?          OnSessionRemoved;

        #endregion


        #region AddSession           (Session,                          SkipNotifications = false)

        public async Task<AddResult<Session>> AddSession(Session            Session,
                                                         Boolean            SkipNotifications   = false,
                                                         EventTracking_Id?  EventTrackingId     = null,
                                                         User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (chargingSessions.TryAdd(Session.Id, Session))
            {

                Session.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addSession,
                          Session.ToJSON(true,
                                         true,
                                         null,
                                         CustomSessionSerializer,
                                         CustomLocationSerializer,
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
                                         CustomChargingPeriodSerializer,
                                         CustomCDRDimensionSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnChargingSessionAddedLocal = OnSessionAdded;
                    if (OnChargingSessionAddedLocal is not null)
                    {
                        try
                        {
                            OnChargingSessionAddedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddSession), " ", nameof(OnSessionAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Session>.Success(EventTrackingId, Session);

            }

            return AddResult<Session>.Failed(EventTrackingId, Session,
                                            "AddSession(Session.Id, Session) failed!");

        }

        #endregion

        #region AddSessionIfNotExists(Session,                          SkipNotifications = false)

        public async Task<AddResult<Session>> AddSessionIfNotExists(Session            Session,
                                                                    Boolean            SkipNotifications   = false,
                                                                    EventTracking_Id?  EventTrackingId     = null,
                                                                    User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (chargingSessions.TryAdd(Session.Id, Session))
            {

                Session.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addSessionIfNotExists,
                          Session.ToJSON(true,
                                         true,
                                         null,
                                         CustomSessionSerializer,
                                         CustomLocationSerializer,
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
                                         CustomChargingPeriodSerializer,
                                         CustomCDRDimensionSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnChargingSessionAddedLocal = OnSessionAdded;
                    if (OnChargingSessionAddedLocal is not null)
                    {
                        try
                        {
                            OnChargingSessionAddedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddSessionIfNotExists), " ", nameof(OnSessionAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Session>.Success(EventTrackingId, Session);

            }

            return AddResult<Session>.NoOperation(EventTrackingId, Session);

        }

        #endregion

        #region AddOrUpdateSession   (Session, AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<Session>> AddOrUpdateSession(Session            Session,
                                                                         Boolean?           AllowDowngrades     = false,
                                                                         Boolean            SkipNotifications   = false,
                                                                         EventTracking_Id?  EventTrackingId     = null,
                                                                         User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Update an existing session

            if (chargingSessions.TryGetValue(Session.Id, out var existingSession))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Session.LastUpdated <= existingSession.LastUpdated)
                {
                    return AddOrUpdateResult<Session>.Failed(EventTrackingId, Session,
                                                            "The 'lastUpdated' timestamp of the new charging session must be newer then the timestamp of the existing session!");
                }

                chargingSessions[Session.Id] = Session;
                Session.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addOrUpdateSession,
                          Session.ToJSON(true,
                                         true,
                                         null,
                                         CustomSessionSerializer,
                                         CustomLocationSerializer,
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
                                         CustomChargingPeriodSerializer,
                                         CustomCDRDimensionSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnChargingSessionChangedLocal = OnSessionChanged;
                    if (OnChargingSessionChangedLocal is not null)
                    {
                        try
                        {
                            OnChargingSessionChangedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Session>.Updated(EventTrackingId, Session);

            }

            #endregion

            #region Add a new session

            if (chargingSessions.TryAdd(Session.Id, Session))
            {

                Session.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addOrUpdateSession,
                          Session.ToJSON(true,
                                         true,
                                         null,
                                         CustomSessionSerializer,
                                         CustomLocationSerializer,
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
                                         CustomChargingPeriodSerializer,
                                         CustomCDRDimensionSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnSessionAddedLocal = OnSessionAdded;
                    if (OnSessionAddedLocal is not null)
                    {
                        try
                        {
                            OnSessionAddedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateSession), " ", nameof(OnSessionAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Session>.Created(EventTrackingId, Session);

            }

            return AddOrUpdateResult<Session>.Failed(EventTrackingId, Session,
                                                     "AddOrUpdateSession(Session.Id, Session) failed!");

            #endregion

        }

        #endregion

        #region UpdateSession        (Session, AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<Session>> UpdateSession(Session            Session,
                                                               Boolean?           AllowDowngrades     = false,
                                                               Boolean            SkipNotifications   = false,
                                                               EventTracking_Id?  EventTrackingId     = null,
                                                               User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            if (chargingSessions.TryGetValue(Session.Id, out var existingSession))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Session.LastUpdated <= existingSession.LastUpdated)
                {

                    return UpdateResult<Session>.Failed(EventTrackingId, Session,
                                                        "The 'lastUpdated' timestamp of the new charging session must be newer then the timestamp of the existing session!");

                }

            }
            else
                return UpdateResult<Session>.Failed(EventTrackingId, Session,
                                                    $"Unknown session identification '{Session.Id}'!");

            #endregion


            if (chargingSessions.TryUpdate(Session.Id, Session, existingSession))
            {

                Session.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.updateSession,
                          Session.ToJSON(true,
                                         true,
                                         null,
                                         CustomSessionSerializer,
                                         CustomLocationSerializer,
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
                                         CustomChargingPeriodSerializer,
                                         CustomCDRDimensionSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnChargingSessionChangedLocal = OnSessionChanged;
                    if (OnChargingSessionChangedLocal is not null)
                    {
                        try
                        {
                            OnChargingSessionChangedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<Session>.Success(EventTrackingId, Session);

            }

            return UpdateResult<Session>.Failed(EventTrackingId, Session,
                                                "UpdateSession(Session.Id, Session, Session) failed!");

        }

        #endregion


        #region TryPatchSession      (Session, SessionPatch, AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<Session>> TryPatchSession(Session            Session,
                                                                JObject            SessionPatch,
                                                                Boolean?           AllowDowngrades     = false,
                                                                Boolean            SkipNotifications   = false,
                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (SessionPatch is null || !SessionPatch.HasValues)
                return PatchResult<Session>.Failed(EventTrackingId, Session,
                                                  "The given charging session patch must not be null or empty!");

            if (chargingSessions.TryGetValue(Session.Id, out var existingSession))
            {

                var patchResult = existingSession.TryPatch(SessionPatch,
                                                           AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    chargingSessions[Session.Id] = patchResult.PatchedData;

                    await LogAsset(
                              CommonHTTPAPI.updateSession,
                              Session.ToJSON(true,
                                             true,
                                             null,
                                             CustomSessionSerializer,
                                             CustomLocationSerializer,
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
                                             CustomChargingPeriodSerializer,
                                             CustomCDRDimensionSerializer),
                               EventTrackingId,
                               CurrentUserId
                           );

                    if (!SkipNotifications)
                    {

                        var OnChargingSessionChangedLocal = OnSessionChanged;
                        if (OnChargingSessionChangedLocal is not null)
                        {
                            try
                            {
                                OnChargingSessionChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchSession), " ", nameof(OnSessionChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<Session>.Failed(EventTrackingId, Session,
                                                  "The given charging session does not exist!");

        }

        #endregion


        #region SessionExists(SessionId)

        public Boolean SessionExists(Session_Id SessionId)

            => chargingSessions.ContainsKey(SessionId);

        #endregion

        #region TryGetSession(CountryCode, PartyId, SessionId, out Session)

        public Boolean TryGetSession(Session_Id                        SessionId,
                                     [NotNullWhen(true)] out Session?  Session)
        {

            if (chargingSessions.TryGetValue(SessionId, out Session))
                return true;

            Session = null;
            return false;

        }

        #endregion

        #region GetSessions  (IncludeSession = null)

        public IEnumerable<Session> GetSessions(Func<Session, Boolean>? IncludeSession = null)

            => IncludeSession is null
                   ? chargingSessions.Values
                   : chargingSessions.Values.Where(IncludeSession);

        #endregion

        #region GetSessions  (CountryCode, PartyId)

        public IEnumerable<Session> GetSessions(CountryCode  CountryCode,
                                                Party_Id     PartyId)

            => chargingSessions.Values.Where(chargingSession => chargingSession.CountryCode == CountryCode &&
                                                                chargingSession.PartyId     == PartyId);

        #endregion


        #region RemoveSession    (Session,              SkipNotifications = false)

        /// <summary>
        /// Remove the given charging session.
        /// </summary>
        /// <param name="Session">A charging session.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<Session>> RemoveSession(Session            Session,
                                                         Boolean            SkipNotifications   = false,
                                                         EventTracking_Id?  EventTrackingId     = null,
                                                         User_Id?           CurrentUserId       = null)

            => RemoveSession(Session.Id,
                             SkipNotifications);

        #endregion

        #region RemoveSession    (SessionId,            SkipNotifications = false)

        /// <summary>
        /// Remove the given charging session.
        /// </summary>
        /// <param name="SessionId">An unique charging session identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<Session>> RemoveSession(Session_Id         SessionId,
                                                               Boolean            SkipNotifications   = false,
                                                               EventTracking_Id?  EventTrackingId     = null,
                                                               User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (chargingSessions.Remove(SessionId, out var session))
            {

                await LogAsset(
                          CommonHTTPAPI.removeTariff,
                          session.ToJSON(true,
                                         true,
                                         null,
                                         CustomSessionSerializer,
                                         CustomLocationSerializer,
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
                                         CustomChargingPeriodSerializer,
                                         CustomCDRDimensionSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnSessionRemovedLocal = OnSessionRemoved;
                    if (OnSessionRemovedLocal is not null)
                    {
                        try
                        {
                            await OnSessionRemovedLocal(session);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveSession), " ", nameof(OnSessionRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<Session>.Success(EventTrackingId, session);

            }

            return RemoveResult<Session>.Failed(EventTrackingId,
                                                "RemoveSession(SessionId, ...) failed!");

        }

        #endregion

        #region RemoveAllSessions(                      SkipNotifications = false)

        /// <summary>
        /// Remove all charging sessions.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>> RemoveAllSessions(Boolean            SkipNotifications   = false,
                                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                                User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var existingSessions = chargingSessions.Values.ToArray();

            chargingSessions.Clear();

            await LogAsset(
                      CommonHTTPAPI.removeAllSessions,
                      EventTrackingId,
                      CurrentUserId
                  );

            if (!SkipNotifications)
            {

                var OnSessionRemovedLocal = OnSessionRemoved;
                if (OnSessionRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var location in existingSessions)
                            await OnSessionRemovedLocal(location);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllSessions), " ", nameof(OnSessionRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<Session>>.Success(EventTrackingId, existingSessions);

        }

        #endregion

        #region RemoveAllSessions(IncludeSessions,      SkipNotifications = false)

        /// <summary>
        /// Remove all matching sessions.
        /// </summary>
        /// <param name="IncludeSessions">A charging session filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>> RemoveAllSessions(Func<Session, Boolean>  IncludeSessions,
                                                                                Boolean                 SkipNotifications   = false,
                                                                                EventTracking_Id?       EventTrackingId     = null,
                                                                                User_Id?                CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedSessions  = new List<Session>();
            var failedSessions   = new List<RemoveResult<Session>>();

            foreach (var session in chargingSessions.Values.Where(IncludeSessions).ToArray())
            {

                var result = await RemoveSession(session.Id,
                                                 SkipNotifications);

                if (result.IsSuccess)
                    removedSessions.Add(session);
                else
                    failedSessions. Add(result);

            }

            return removedSessions.Any() && !failedSessions.Any()
                       ? RemoveResult<IEnumerable<Session>>.Success(EventTrackingId, removedSessions)

                       : !removedSessions.Any() && !failedSessions.Any()
                             ? RemoveResult<IEnumerable<Session>>.NoOperation(EventTrackingId, Array.Empty<Session>())
                             : RemoveResult<IEnumerable<Session>>.Failed     (EventTrackingId, failedSessions.Select(session => session.Data)!,
                                                                              failedSessions.Select(session => session.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllSessions(IncludeSessionIds,    SkipNotifications = false)

        /// <summary>
        /// Remove all matching sessions.
        /// </summary>
        /// <param name="IncludeSessionIds">A charging session identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>> RemoveAllSessions(Func<Session_Id, Boolean>  IncludeSessionIds,
                                                                                Boolean                    SkipNotifications   = false,
                                                                                EventTracking_Id?          EventTrackingId     = null,
                                                                                User_Id?                   CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedSessions  = new List<Session>();
            var failedSessions   = new List<RemoveResult<Session>>();

            foreach (var session in chargingSessions.Where  (kvp => IncludeSessionIds(kvp.Key)).
                                                     Select (kvp => kvp.Value).
                                                     ToArray())
            {

                var result = await RemoveSession(session.Id,
                                                 SkipNotifications);

                if (result.IsSuccess)
                    removedSessions.Add(session);
                else
                    failedSessions. Add(result);

            }

            return removedSessions.Any() && !failedSessions.Any()
                       ? RemoveResult<IEnumerable<Session>>.Success(EventTrackingId, removedSessions)

                       : !removedSessions.Any() && !failedSessions.Any()
                             ? RemoveResult<IEnumerable<Session>>.NoOperation(EventTrackingId, Array.Empty<Session>())
                             : RemoveResult<IEnumerable<Session>>.Failed     (EventTrackingId, failedSessions.Select(session => session.Data)!,
                                                                              failedSessions.Select(session => session.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllSessions(CountryCode, PartyId, SkipNotifications = false)

        /// <summary>
        /// Remove all charging sessions owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>> RemoveAllSessions(CountryCode        CountryCode,
                                                                                Party_Id           PartyId,
                                                                                Boolean            SkipNotifications   = false,
                                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                                User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedSessions  = new List<Session>();
            var failedSessions   = new List<RemoveResult<Session>>();

            foreach (var session in chargingSessions.Values.Where  (session => CountryCode == session.CountryCode &&
                                                                               PartyId     == session.PartyId).
                                                            ToArray())
            {

                var result = await RemoveSession(session.Id,
                                                 SkipNotifications);

                if (result.IsSuccess)
                    removedSessions.Add(session);
                else
                    failedSessions. Add(result);

            }

            return removedSessions.Any() && !failedSessions.Any()
                       ? RemoveResult<IEnumerable<Session>>.Success(EventTrackingId, removedSessions)

                       : !removedSessions.Any() && !failedSessions.Any()
                             ? RemoveResult<IEnumerable<Session>>.NoOperation(EventTrackingId, Array.Empty<Session>())
                             : RemoveResult<IEnumerable<Session>>.Failed     (EventTrackingId, failedSessions.Select(session => session.Data)!,
                                                                              failedSessions.Select(session => session.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #endregion

        #region ChargeDetailRecords

        #region Data

        private readonly ConcurrentDictionary<CDR_Id, CDR> chargeDetailRecords = [];


        public delegate Task OnChargeDetailRecordAddedDelegate  (CDR CDR);
        public delegate Task OnChargeDetailRecordChangedDelegate(CDR CDR);
        public delegate Task OnChargeDetailRecordRemovedDelegate(CDR CDR);

        public event OnChargeDetailRecordAddedDelegate?    OnCDRAdded;
        public event OnChargeDetailRecordChangedDelegate?  OnCDRChanged;
        public event OnChargeDetailRecordRemovedDelegate?  OnCDRRemoved;

        #endregion


        #region AddCDR           (CDR,                          SkipNotifications = false)

        public async Task<AddResult<CDR>> AddCDR(CDR                CDR,
                                                 Boolean            SkipNotifications   = false,
                                                 EventTracking_Id?  EventTrackingId     = null,
                                                 User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (chargeDetailRecords.TryAdd(CDR.Id, CDR))
            {

                CDR.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addChargeDetailRecord,
                          CDR.ToJSON(true,
                                     true,
                                     true,
                                     null,
                                     CustomCDRSerializer,
                                     CustomLocationSerializer,
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
                                     CustomTariffSerializer,
                                     CustomTariffElementSerializer,
                                     CustomPriceComponentSerializer,
                                     CustomTariffRestrictionsSerializer,
                                     CustomChargingPeriodSerializer,
                                     CustomCDRDimensionSerializer,
                                     CustomCDRCostDetailsSerializer,
                                     CustomSignedDataSerializer,
                                     CustomSignedValueSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnChargeDetailRecordAddedLocal = OnCDRAdded;
                    if (OnChargeDetailRecordAddedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordAddedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddCDR), " ", nameof(OnCDRAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<CDR>.Success(EventTrackingId, CDR);

            }

            return AddResult<CDR>.Failed(EventTrackingId, CDR,
                                         "TryAdd(CDR.Id, CDR) failed!");

        }

        #endregion

        #region AddCDRIfNotExists(CDR,                          SkipNotifications = false)

        public async Task<AddResult<CDR>> AddCDRIfNotExists(CDR                CDR,
                                                            Boolean            SkipNotifications   = false,
                                                            EventTracking_Id?  EventTrackingId     = null,
                                                            User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (chargeDetailRecords.TryAdd(CDR.Id, CDR))
            {

                CDR.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addChargeDetailRecord,
                          CDR.ToJSON(true,
                                     true,
                                     true,
                                     null,
                                     CustomCDRSerializer,
                                     CustomLocationSerializer,
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
                                     CustomTariffSerializer,
                                     CustomTariffElementSerializer,
                                     CustomPriceComponentSerializer,
                                     CustomTariffRestrictionsSerializer,
                                     CustomChargingPeriodSerializer,
                                     CustomCDRDimensionSerializer,
                                     CustomCDRCostDetailsSerializer,
                                     CustomSignedDataSerializer,
                                     CustomSignedValueSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnChargeDetailRecordAddedLocal = OnCDRAdded;
                    if (OnChargeDetailRecordAddedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordAddedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddCDR), " ", nameof(OnCDRAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<CDR>.Success(EventTrackingId, CDR);

            }

            return AddResult<CDR>.NoOperation(EventTrackingId, CDR);

        }

        #endregion

        #region AddOrUpdateCDR   (CDR, AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<CDR>> AddOrUpdateCDR(CDR                CDR,
                                                                 Boolean?           AllowDowngrades     = false,
                                                                 Boolean            SkipNotifications   = false,
                                                                 EventTracking_Id?  EventTrackingId     = null,
                                                                 User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Update an existing charge detail record

            if (chargeDetailRecords.TryGetValue(CDR.Id, out var existingCDR))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    CDR.LastUpdated <= existingCDR.LastUpdated)
                {
                    return AddOrUpdateResult<CDR>.Failed(EventTrackingId, CDR,
                                                         "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");
                }

                chargeDetailRecords[CDR.Id] = CDR;
                CDR.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.addOrUpdateChargeDetailRecord,
                          CDR.ToJSON(true,
                                     true,
                                     true,
                                     null,
                                     CustomCDRSerializer,
                                     CustomLocationSerializer,
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
                                     CustomTariffSerializer,
                                     CustomTariffElementSerializer,
                                     CustomPriceComponentSerializer,
                                     CustomTariffRestrictionsSerializer,
                                     CustomChargingPeriodSerializer,
                                     CustomCDRDimensionSerializer,
                                     CustomCDRCostDetailsSerializer,
                                     CustomSignedDataSerializer,
                                     CustomSignedValueSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnCDRChangedLocal = OnCDRChanged;
                    if (OnCDRChangedLocal is not null)
                    {
                        try
                        {
                            OnCDRChangedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnCDRChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<CDR>.Updated(EventTrackingId, CDR);

            }

            #endregion

            #region Add a new charge detail record

            if (chargeDetailRecords.TryAdd(CDR.Id, CDR))
            {

                CDR.CommonAPI = this;

                if (!SkipNotifications)
                {

                    var OnCDRAddedLocal = OnCDRAdded;
                    if (OnCDRAddedLocal is not null)
                    {
                        try
                        {
                            OnCDRAddedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnCDRAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<CDR>.Created(EventTrackingId, CDR);

            }

            return AddOrUpdateResult<CDR>.Failed(
                       EventTrackingId,
                       CDR,
                       "AddOrUpdateCDR(CDR.Id, CDR) failed!"
                   );

            #endregion

        }

        #endregion

        #region UpdateCDR        (CDR, AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<CDR>> UpdateCDR(CDR                CDR,
                                                       Boolean?           AllowDowngrades     = false,
                                                       Boolean            SkipNotifications   = false,
                                                       EventTracking_Id?  EventTrackingId     = null,
                                                       User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            #region Validate AllowDowngrades

            if (chargeDetailRecords.TryGetValue(CDR.Id, out var existingCDR))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    CDR.LastUpdated <= existingCDR.LastUpdated)
                {

                    return UpdateResult<CDR>.Failed(EventTrackingId, CDR,
                                                    "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");

                }

            }
            else
                return UpdateResult<CDR>.Failed(EventTrackingId, CDR,
                                                $"Unknown charge detail record identification '{CDR.Id}'!");

            #endregion


            if (chargeDetailRecords.TryUpdate(CDR.Id, CDR, existingCDR))
            {

                CDR.CommonAPI = this;

                await LogAsset(
                          CommonHTTPAPI.updateChargeDetailRecord,
                          CDR.ToJSON(true,
                                     true,
                                     true,
                                     null,
                                     CustomCDRSerializer,
                                     CustomLocationSerializer,
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
                                     CustomTariffSerializer,
                                     CustomTariffElementSerializer,
                                     CustomPriceComponentSerializer,
                                     CustomTariffRestrictionsSerializer,
                                     CustomChargingPeriodSerializer,
                                     CustomCDRDimensionSerializer,
                                     CustomCDRCostDetailsSerializer,
                                     CustomSignedDataSerializer,
                                     CustomSignedValueSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnCDRChangedLocal = OnCDRChanged;
                    if (OnCDRChangedLocal is not null)
                    {
                        try
                        {
                            OnCDRChangedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnCDRChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<CDR>.Success(EventTrackingId, CDR);

            }

            return UpdateResult<CDR>.Failed(EventTrackingId, CDR,
                                            "UpdateCDR(CDR.Id, CDR, CDR) failed!");

        }

        #endregion


        #region TryPatchCDR      (CDR, CDRPatch, AllowDowngrades = false, SkipNotifications = false)   // Non-Standard

        public async Task<PatchResult<CDR>> TryPatchCDR(CDR                CDR,
                                                        JObject            CDRPatch,
                                                        Boolean?           AllowDowngrades     = false,
                                                        Boolean            SkipNotifications   = false,
                                                        EventTracking_Id?  EventTrackingId     = null,
                                                        User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (CDRPatch is null || !CDRPatch.HasValues)
                return PatchResult<CDR>.Failed(EventTrackingId, CDR,
                                               "The given charge detail record patch must not be null or empty!");

            if (chargeDetailRecords.TryGetValue(CDR.Id, out var existingCDR))
            {

                var patchResult = existingCDR.TryPatch(CDRPatch,
                                                       AllowDowngrades ?? this.AllowDowngrades ?? false,
                                                       EventTrackingId);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    chargeDetailRecords[CDR.Id] = patchResult.PatchedData;

                    await LogAsset(
                              CommonHTTPAPI.updateChargeDetailRecord,
                              CDR.ToJSON(true,
                                         true,
                                         true,
                                         null,
                                         CustomCDRSerializer,
                                         CustomLocationSerializer,
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
                                         CustomTariffSerializer,
                                         CustomTariffElementSerializer,
                                         CustomPriceComponentSerializer,
                                         CustomTariffRestrictionsSerializer,
                                         CustomChargingPeriodSerializer,
                                         CustomCDRDimensionSerializer,
                                         CustomCDRCostDetailsSerializer,
                                         CustomSignedDataSerializer,
                                         CustomSignedValueSerializer),
                              EventTrackingId,
                              CurrentUserId
                          );

                    if (!SkipNotifications)
                    {

                        var OnChargeDetailRecordChangedLocal = OnCDRChanged;
                        if (OnChargeDetailRecordChangedLocal is not null)
                        {
                            try
                            {
                                OnChargeDetailRecordChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchCDR), " ", nameof(OnCDRChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<CDR>.Failed(EventTrackingId, CDR,
                                                  "The given charge detail record does not exist!");

        }

        #endregion


        #region CDRExists(CDRId)

        public Boolean CDRExists(CDR_Id CDRId)

            => chargeDetailRecords.ContainsKey(CDRId);

        #endregion

        #region TryGetCDR(CDRId, out CDR)

        public Boolean TryGetCDR(CDR_Id                        CDRId,
                                 [NotNullWhen(true)] out CDR?  CDR)
        {

            if (chargeDetailRecords.TryGetValue(CDRId, out CDR))
                return true;

            CDR = null;
            return false;

        }

        #endregion

        #region GetCDRs  (IncludeCDRs = null)

        /// <summary>
        /// Return all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">An optional charge detail record filter.</param>
        public IEnumerable<CDR> GetCDRs(Func<CDR, Boolean>? IncludeCDRs = null)

            => IncludeCDRs is null
                   ? chargeDetailRecords.Values
                   : chargeDetailRecords.Values.Where(IncludeCDRs);

        #endregion

        #region GetCDRs  (CountryCode, PartyId)

        /// <summary>
        /// Return all charge detail records owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public IEnumerable<CDR> GetCDRs(CountryCode  CountryCode,
                                        Party_Id     PartyId)

            => chargeDetailRecords.Values.Where(cdr => cdr.CountryCode == CountryCode &&
                                                       cdr.PartyId     == PartyId);

        #endregion


        #region RemoveCDR    (CDR,                  SkipNotifications = false)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="CDR">A charge detail record.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<CDR>> RemoveCDR(CDR                CDR,
                                                 Boolean            SkipNotifications   = false,
                                                 EventTracking_Id?  EventTrackingId     = null,
                                                 User_Id?           CurrentUserId       = null)

            => RemoveCDR(CDR.Id,
                         SkipNotifications);

        #endregion

        #region RemoveCDR    (CDRId,                SkipNotifications = false)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="CDRId">A unique identification of a charge detail record.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<CDR>> RemoveCDR(CDR_Id             CDRId,
                                                       Boolean            SkipNotifications   = false,
                                                       EventTracking_Id?  EventTrackingId     = null,
                                                       User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (chargeDetailRecords.Remove(CDRId, out var cdr))
            {

                await LogAsset(
                          CommonHTTPAPI.removeChargeDetailRecord,
                          cdr.ToJSON(true,
                                     true,
                                     true,
                                     null,
                                     CustomCDRSerializer,
                                     CustomLocationSerializer,
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
                                     CustomTariffSerializer,
                                     CustomTariffElementSerializer,
                                     CustomPriceComponentSerializer,
                                     CustomTariffRestrictionsSerializer,
                                     CustomChargingPeriodSerializer,
                                     CustomCDRDimensionSerializer,
                                     CustomCDRCostDetailsSerializer,
                                     CustomSignedDataSerializer,
                                     CustomSignedValueSerializer),
                          EventTrackingId,
                          CurrentUserId
                      );

                if (!SkipNotifications)
                {

                    var OnCDRRemovedLocal = OnCDRRemoved;
                    if (OnCDRRemovedLocal is not null)
                    {
                        try
                        {
                            await OnCDRRemovedLocal(cdr);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveCDR), " ", nameof(OnCDRRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<CDR>.Success(EventTrackingId, cdr);

            }

            return RemoveResult<CDR>.Failed(EventTrackingId,
                                            "Remove(CDRId, ...) failed!");

        }

        #endregion

        #region RemoveAllCDRs(                      SkipNotifications = false)

        /// <summary>
        /// Remove all charge detail records.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>> RemoveAllCDRs(Boolean            SkipNotifications   = false,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var existingCDRs = chargeDetailRecords.Values.ToArray();

            chargeDetailRecords.Clear();

            await LogAsset(
                      CommonHTTPAPI.removeAllChargeDetailRecords,
                      EventTrackingId,
                      CurrentUserId
                  );

            if (!SkipNotifications)
            {

                var OnCDRRemovedLocal = OnCDRRemoved;
                if (OnCDRRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var location in existingCDRs)
                            await OnCDRRemovedLocal(location);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllCDRs), " ", nameof(OnCDRRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<CDR>>.Success(EventTrackingId, existingCDRs);

        }

        #endregion

        #region RemoveAllCDRs(IncludeCDRs,          SkipNotifications = false)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">A charge detail record filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>> RemoveAllCDRs(Func<CDR, Boolean>  IncludeCDRs,
                                                                        Boolean             SkipNotifications   = false,
                                                                        EventTracking_Id?   EventTrackingId     = null,
                                                                        User_Id?            CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedCDRs  = new List<CDR>();
            var failedCDRs   = new List<RemoveResult<CDR>>();

            foreach (var cdr in chargeDetailRecords.Values.Where(IncludeCDRs).ToArray())
            {

                var result = await RemoveCDR(cdr.Id,
                                             SkipNotifications);

                if (result.IsSuccess)
                    removedCDRs.Add(cdr);
                else
                    failedCDRs. Add(result);

            }

            return removedCDRs.Any() && !failedCDRs.Any()
                       ? RemoveResult<IEnumerable<CDR>>.Success(EventTrackingId, removedCDRs)

                       : !removedCDRs.Any() && !failedCDRs.Any()
                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(EventTrackingId, Array.Empty<CDR>())
                             : RemoveResult<IEnumerable<CDR>>.Failed     (EventTrackingId, failedCDRs.Select(cdr => cdr.Data)!,
                                                                          failedCDRs.Select(cdr => cdr.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllCDRs(IncludeCDRIds,        SkipNotifications = false)

        /// <summary>
        /// Remove all matching cdrs.
        /// </summary>
        /// <param name="IncludeCDRIds">A charging cdr identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>> RemoveAllCDRs(Func<CDR_Id, Boolean>  IncludeCDRIds,
                                                                        Boolean                SkipNotifications   = false,
                                                                        EventTracking_Id?      EventTrackingId     = null,
                                                                        User_Id?               CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedCDRs  = new List<CDR>();
            var failedCDRs   = new List<RemoveResult<CDR>>();

            foreach (var cdr in chargeDetailRecords.Where  (kvp => IncludeCDRIds(kvp.Key)).
                                                    Select (kvp => kvp.Value).
                                                    ToArray())
            {

                var result = await RemoveCDR(cdr.Id,
                                             SkipNotifications);

                if (result.IsSuccess)
                    removedCDRs.Add(cdr);
                else
                    failedCDRs. Add(result);

            }

            return removedCDRs.Any() && !failedCDRs.Any()
                       ? RemoveResult<IEnumerable<CDR>>.Success(EventTrackingId, removedCDRs)

                       : !removedCDRs.Any() && !failedCDRs.Any()
                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(EventTrackingId, Array.Empty<CDR>())
                             : RemoveResult<IEnumerable<CDR>>.Failed     (EventTrackingId, failedCDRs.Select(cdr => cdr.Data)!,
                                                                          failedCDRs.Select(cdr => cdr.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllCDRs(CountryCode, PartyId, SkipNotifications = false)

        /// <summary>
        /// Remove all charge detail records owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>> RemoveAllCDRs(CountryCode        CountryCode,
                                                                        Party_Id           PartyId,
                                                                        Boolean            SkipNotifications   = false,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedCDRs  = new List<CDR>();
            var failedCDRs   = new List<RemoveResult<CDR>>();

            foreach (var cdr in chargeDetailRecords.Values.Where  (cdr => CountryCode == cdr.CountryCode &&
                                                                          PartyId     == cdr.PartyId).
                                                           ToArray())
            {

                var result = await RemoveCDR(cdr.Id,
                                             SkipNotifications);

                if (result.IsSuccess)
                    removedCDRs.Add(cdr);
                else
                    failedCDRs. Add(result);

            }

            return removedCDRs.Any() && !failedCDRs.Any()
                       ? RemoveResult<IEnumerable<CDR>>.Success(EventTrackingId, removedCDRs)

                       : !removedCDRs.Any() && !failedCDRs.Any()
                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(EventTrackingId, Array.Empty<CDR>())
                             : RemoveResult<IEnumerable<CDR>>.Failed     (EventTrackingId, failedCDRs.Select(cdr => cdr.Data)!,
                                                                          failedCDRs.Select(cdr => cdr.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #endregion


        public Task LogException(Exception e)
        {
            return Task.CompletedTask;
        }

    }

}
