/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0.CPO.HTTP;
using cloud.charging.open.protocols.OCPIv3_0.EMSP.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.HTTP
{

    /// <summary>
    /// A delegate for filtering remote parties.
    /// </summary>
    public delegate Boolean IncludeRemoteParty(RemoteParty RemoteParty);

    public delegate IEnumerable<Tariff_Id>  GetTariffIds2_Delegate(CountryCode    CPOCountryCode,
                                                                   Party_Id       CPOPartyId,
                                                                   Location_Id?   Location      = null,
                                                                   EVSE_UId?      EVSEUId       = null,
                                                                   Connector_Id?  ConnectorId   = null,
                                                                   EMSP_Id?       EMSPId        = null);

    public class PartyData
    {

        public Party_Id  Id    { get; }

        public ConcurrentDictionary<Location_Id, Location> Locations  = [];
        public TimeRangeDictionary <Tariff_Id,   Tariff>   Tariffs    = [];
        public ConcurrentDictionary<Session_Id,  Session>  Sessions   = [];
        public ConcurrentDictionary<Token_Id,    Token>    Tokens     = [];
        public ConcurrentDictionary<CDR_Id,      CDR>      CDRs       = [];

    }


    /// <summary>
    /// Extension methods for the Common HTTP API.
    /// </summary>
    public static class CommonAPIExtensions
    {

        #region ParsePartyId                              (this Request, CommonAPI, out PartyId,                                                                                      out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The CommonAPI.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParsePartyId(this OCPIRequest                                Request,
                                           CommonAPI                                       CommonAPI,
                                           [NotNullWhen(true)]  out Party_Id?              PartyId,
                                           [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CommonAPI is null)
                throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

            #endregion

            PartyId              = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 1)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code and/or party identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[0]);

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

            return true;

        }

        #endregion

        #region ParseLocation                             (this Request, CommonAPI, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <param name="FailOnMissingLocation">Whether to fail when the location for the given location identification was not found.</param>
        public static Boolean ParseLocation(this OCPIRequest                                Request,
                                            CommonAPI                                       CommonAPI,
                                            [NotNullWhen(true)]  out Party_Id?              PartyId,
                                            [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                                 out Location?              Location,
                                            [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                            Boolean                                         FailOnMissingLocation = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CommonAPI is null)
                throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

            #endregion

            PartyId              = null;
            LocationId           = null;
            Location             = null;
            OCPIResponseBuilder  = null;

            if (Request.ParsedURLParameters.Length < 2)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or location identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[0]);

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

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!LocationId.HasValue)
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


            if (!CommonAPI.TryGetLocation(PartyId.Value, LocationId.Value, out Location) &&
                 FailOnMissingLocation)
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

        #region ParseLocationChargingStation              (this Request, CommonAPI, out PartyId, out LocationId, out Location, out ChargingStationId, out ChargingStation,                                 out OCPIResponseBuilder, FailOnMissingChargingStation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="ChargingStationId">The parsed unique ChargingStation identification.</param>
        /// <param name="ChargingStation">The resolved ChargingStation.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <param name="FailOnMissingChargingStation">Whether to fail when the location for the given ChargingStation identification was not found.</param>
        public static Boolean ParseLocationChargingStation(this OCPIRequest                                Request,
                                                           CommonAPI                                       CommonAPI,
                                                           [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                           [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                           [NotNullWhen(true)]  out Location?              Location,
                                                           [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                                                out ChargingStation?       ChargingStation,
                                                           [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                           Boolean                                         FailOnMissingChargingStation = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CommonAPI is null)
                throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

            #endregion

            PartyId              = null;
            LocationId           = null;
            Location             = null;
            ChargingStationId    = null;
            ChargingStation      = null;
            OCPIResponseBuilder  = null;

            if (Request.ParsedURLParameters.Length < 4)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification, location identification and/or ChargingStation identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[0]);

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

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!LocationId.HasValue) {

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

            ChargingStationId = ChargingStation_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!ChargingStationId.HasValue)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid ChargingStation identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }


            if (!CommonAPI.TryGetLocation(PartyId.    Value,
                                          LocationId. Value,
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

            if (!Location.TryGetChargingStation(ChargingStationId.Value, out ChargingStation) &&
                FailOnMissingChargingStation)
            {

                    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                        StatusCode           = 2001,
                        StatusMessage        = "Unknown ChargingStation identification!",
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

        #region ParseLocationChargingStationEVSE          (this Request, CommonAPI, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <param name="FailOnMissingEVSE">Whether to fail when the location for the given EVSE identification was not found.</param>
        public static Boolean ParseLocationChargingStationEVSE(this OCPIRequest                                Request,
                                                               CommonAPI                                       CommonAPI,
                                                               [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                               [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                               [NotNullWhen(true)]  out Location?              Location,
                                                               [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                               [NotNullWhen(true)]  out ChargingStation?       ChargingStation,
                                                               [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                                                    out EVSE?                  EVSE,
                                                               [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                               Boolean                                         FailOnMissingEVSE = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CommonAPI is null)
                throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

            #endregion

            PartyId              = null;
            LocationId           = null;
            Location             = null;
            ChargingStationId    = null;
            ChargingStation      = null;
            EVSEUId              = null;
            EVSE                 = null;
            OCPIResponseBuilder  = null;

            if (Request.ParsedURLParameters.Length < 4)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification, location identification and/or EVSE identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[0]);

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

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!LocationId.HasValue) {

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

            ChargingStationId = ChargingStation_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!ChargingStationId.HasValue) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid charging station identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }


            if (!CommonAPI.TryGetLocation(PartyId.    Value,
                                          LocationId. Value,
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

            if (!Location.TryGetChargingStation(ChargingStationId.Value, out ChargingStation))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown charging station identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            if (!ChargingStation.TryGetEVSE(EVSEUId.Value, out EVSE) &&
                 FailOnMissingEVSE)
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

        #region ParseLocationChargingStationEVSEConnector (this Request, CommonAPI, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

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
        /// <param name="FailOnMissingConnector">Whether to fail when the connector for the given connector identification was not found.</param>
        public static Boolean ParseLocationChargingStationEVSEConnector(this OCPIRequest                                Request,
                                                                        CommonAPI                                       CommonAPI,
                                                                        [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                                        [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                                        [NotNullWhen(true)]  out Location?              Location,
                                                                        [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                                        [NotNullWhen(true)]  out ChargingStation?       ChargingStation,
                                                                        [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                                        [NotNullWhen(true)]  out EVSE?                  EVSE,
                                                                        [NotNullWhen(true)]  out Connector_Id?          ConnectorId,
                                                                                             out Connector?             Connector,
                                                                        [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                                        Boolean                                         FailOnMissingConnector = true)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CommonAPI is null)
                throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

            #endregion

            PartyId              = null;
            LocationId           = null;
            Location             = null;
            ChargingStationId    = null;
            ChargingStation      = null;
            EVSEUId              = null;
            EVSE                 = null;
            ConnectorId          = null;
            Connector            = null;
            OCPIResponseBuilder  = null;

            if (Request.ParsedURLParameters.Length < 5)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification, location identification, EVSE identification and/or connector identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[0]);

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

            LocationId = Location_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!LocationId.HasValue)
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

            ChargingStationId = ChargingStation_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!ChargingStationId.HasValue) {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Invalid charging station identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
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
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            ConnectorId = Connector_Id.TryParse(Request.ParsedURLParameters[4]);

            if (!ConnectorId.HasValue)
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


            if (!CommonAPI.TryGetLocation(PartyId.    Value,
                                          LocationId. Value,
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

            if (!Location.TryGetChargingStation(ChargingStationId.Value, out ChargingStation))
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Unknown charging station identification!",
                    HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                        HTTPStatusCode             = HTTPStatusCode.NotFound,
                        //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                        AccessControlAllowHeaders  = [ "Authorization" ]
                    }
                };

                return false;

            }

            if (!ChargingStation.TryGetEVSE(EVSEUId.Value, out EVSE))
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

            if (!EVSE.TryGetConnector(ConnectorId.Value, out Connector) &&
                FailOnMissingConnector)
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


        #region ParseTariff                (this Request, CommonAPI, out PartyId, out TariffId,  out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        public static Boolean ParseTariff(this OCPIRequest                                Request,
                                          CommonAPI                                       CommonAPI,
                                          [NotNullWhen(true)]  out Party_Id?              PartyId,
                                          [NotNullWhen(true)]  out Tariff_Id?             TariffId,
                                          [NotNullWhen(true)]  out Tariff?                Tariff,
                                          [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),    "The given HTTP request must not be null!");

            if (CommonAPI is null)
                throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

            #endregion

            PartyId              = default;
            TariffId             = default;
            Tariff               = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 2)
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

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[0]);

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

            TariffId = Tariff_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!TariffId.HasValue) {

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


            if (!CommonAPI.TryGetTariff(PartyId.Value, TariffId.Value, out Tariff))
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

        #region ParseOptionalTariff        (this Request, CommonAPI, out PartyId, out TariffId,  out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="CommonAPI">The Users API.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        /// <param name="FailOnMissingTariff">Whether to fail when the tariff for the given tariff identification was not found.</param>
        public static Boolean ParseOptionalTariff(this OCPIRequest                                Request,
                                                  CommonAPI                                       CommonAPI,
                                                  [NotNullWhen(true)]  out Party_Id?              PartyId,
                                                  [NotNullWhen(true)]  out Tariff_Id?             TariffId,
                                                                       out Tariff?                Tariff,
                                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        {

            #region Initial checks

            if (Request is null)
                throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

            if (CommonAPI is null)
                throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

            #endregion

            PartyId              = default;
            TariffId             = default;
            Tariff               = default;
            OCPIResponseBuilder  = default;

            if (Request.ParsedURLParameters.Length < 2)
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

            PartyId = Party_Id.TryParse(Request.ParsedURLParameters[0]);

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

            TariffId = Tariff_Id.TryParse(Request.ParsedURLParameters[1]);

            if (!TariffId.HasValue) {

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


            CommonAPI.TryGetTariff(PartyId.Value, TariffId.Value, out Tariff);

            //if (!CommonAPI.TryGetTariff(PartyId.Value, TariffId.Value, out Tariff) &&
            //     FailOnMissingTariff)
            //{

            //    OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
            //        StatusCode           = 2001,
            //        StatusMessage        = "Unknown tariff identification!",
            //        HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //            HTTPStatusCode             = HTTPStatusCode.NotFound,
            //            //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
            //            AccessControlAllowHeaders  = [ "Authorization" ]
            //        }
            //    };

            //    return false;

            //}

            return true;

        }

        #endregion


        //#region ParseSession               (this Request, CommonAPI, out PartyId, out SessionId, out Session,  out OCPIResponseBuilder)

        ///// <summary>
        ///// Parse the given HTTP request and return the session identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="Request">A HTTP request.</param>
        ///// <param name="CommonAPI">The Users API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="SessionId">The parsed unique session identification.</param>
        ///// <param name="Session">The resolved session.</param>
        ///// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        ///// <param name="FailOnMissingSession">Whether to fail when the session for the given session identification was not found.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseSession(this OCPIRequest                               Request,
        //                                  CommonAPI                                       CommonAPI,
        //                                  [NotNullWhen(true)]  out Party_Id?              PartyId,
        //                                  [NotNullWhen(true)]  out Session_Id?            SessionId,
        //                                  [NotNullWhen(true)]  out Session?               Session,
        //                                  [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
        //                                  Boolean                                         FailOnMissingSession = true)
        //{

        //    #region Initial checks

        //    if (Request is null)
        //        throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

        //    if (CommonAPI is null)
        //        throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

        //    #endregion

        //    PartyId              = default;
        //    SessionId            = default;
        //    Session              = default;
        //    OCPIResponseBuilder  = default;

        //    if (Request.ParsedURLParameters.Length < 2)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Missing country code, party identification and/or session identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(Request.ParsedURLParameters[0]);

        //    if (!PartyId.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid party identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    SessionId = Session_Id.TryParse(Request.ParsedURLParameters[1]);

        //    if (!SessionId.HasValue) {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid session identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }


        //    if (!CommonAPI.TryGetSession(PartyId.Value, SessionId.Value, out Session) &&
        //        FailOnMissingSession)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Unknown session identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.NotFound,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

        //#region ParseCDR                   (this Request, CommonAPI, out CountryCode, out PartyId, out CDRId,     out CDR,      out OCPIResponseBuilder)

        ///// <summary>
        ///// Parse the given HTTP request and return the charge detail record identification
        ///// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        ///// </summary>
        ///// <param name="Request">A HTTP request.</param>
        ///// <param name="CommonAPI">The Users API.</param>
        ///// <param name="CountryCode">The parsed country code.</param>
        ///// <param name="PartyId">The parsed party identification.</param>
        ///// <param name="CDRId">The parsed unique charge detail record identification.</param>
        ///// <param name="CDR">The resolved charge detail record.</param>
        ///// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        ///// <param name="FailOnMissingCDR">Whether to fail when the charge detail record for the given charge detail record identification was not found.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseCDR(this OCPIRequest                                Request,
        //                               CommonAPI                                       CommonAPI,
        //                               [NotNullWhen(true)]  out CountryCode?           CountryCode,
        //                               [NotNullWhen(true)]  out Party_Id?              PartyId,
        //                               [NotNullWhen(true)]  out CDR_Id?                CDRId,
        //                               [NotNullWhen(true)]  out CDR?                   CDR,
        //                               [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
        //                               Boolean                                         FailOnMissingCDR = true)
        //{

        //    #region Initial checks

        //    if (Request is null)
        //        throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

        //    if (CommonAPI is null)
        //        throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

        //    #endregion

        //    CountryCode          = default;
        //    PartyId              = default;
        //    CDRId                = default;
        //    CDR                  = default;
        //    OCPIResponseBuilder  = default;

        //    if (Request.ParsedURLParameters.Length < 3)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Missing country code, party identification and/or charge detail record identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    CountryCode = OCPI.CountryCode.TryParse(Request.ParsedURLParameters[0]);

        //    if (!CountryCode.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid country code!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    PartyId = Party_Id.TryParse(Request.ParsedURLParameters[1]);

        //    if (!PartyId.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid party identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    CDRId = CDR_Id.TryParse(Request.ParsedURLParameters[2]);

        //    if (!CDRId.HasValue) {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid charge detail record identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }


        //    if (!CommonAPI.TryGetCDR(CountryCode.Value, PartyId.Value, CDRId.Value, out CDR) &&
        //        FailOnMissingCDR)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Unknown charge detail record identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.NotFound,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

        //#region ParseTokenId               (this Request, CommonAPI,                               out TokenId,                 out OCPIResponseBuilder)

        ///// <summary>
        ///// Parse the given HTTP request and return the token identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="Request">A HTTP request.</param>
        ///// <param name="CommonAPI">The CommonAPI.</param>
        ///// <param name="TokenId">The parsed unique token identification.</param>
        ///// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseTokenId(this OCPIRequest                                Request,
        //                                   CommonAPI                                       CommonAPI,
        //                                   [NotNullWhen(true)]  out Token_Id?              TokenId,
        //                                   [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        //{

        //    #region Initial checks

        //    if (Request is null)
        //        throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

        //    if (CommonAPI is null)
        //        throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

        //    #endregion

        //    TokenId              = default;
        //    OCPIResponseBuilder  = default;

        //    if (Request.ParsedURLParameters.Length < 1)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Missing token identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    TokenId = Token_Id.TryParse(Request.ParsedURLParameters[0]);

        //    if (!TokenId.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid token identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

        //#region ParseToken                 (this Request, CommonAPI,                               out TokenId,   out Token,    out OCPIResponseBuilder)

        ///// <summary>
        ///// Parse the given HTTP request and return the token identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="Request">A HTTP request.</param>
        ///// <param name="CommonAPI">The Users API.</param>
        ///// <param name="TokenId">The parsed unique token identification.</param>
        ///// <param name="TokenStatus">The resolved user.</param>
        ///// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        ///// <param name="FailOnMissingToken">Whether to fail when the token for the given token identification was not found.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseToken(this OCPIRequest                                Request,
        //                                 CommonAPI                                       CommonAPI,
        //                                 [NotNullWhen(true)]  out Token_Id?              TokenId,
        //                                 [NotNullWhen(true)]  out TokenStatus            TokenStatus,
        //                                 [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
        //                                 Boolean                                         FailOnMissingToken = true)
        //{

        //    #region Initial checks

        //    if (Request is null)
        //        throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

        //    if (CommonAPI is null)
        //        throw new ArgumentNullException(nameof(CommonAPI),  "The given CommonAPI must not be null!");

        //    #endregion

        //    TokenId              = default;
        //    TokenStatus          = default;
        //    OCPIResponseBuilder  = default;

        //    if (Request.ParsedURLParameters.Length < 1)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Missing token identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    TokenId = Token_Id.TryParse(Request.ParsedURLParameters[0]);

        //    if (!TokenId.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid token identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }


        //    if (!CommonAPI.TryGetToken(Request.ToCountryCode ?? CommonAPI.DefaultCountryCode,
        //                               Request.ToPartyId     ?? CommonAPI.DefaultPartyId,
        //                               TokenId.Value,
        //                               out TokenStatus) &&
        //        FailOnMissingToken)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Unknown token identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.NotFound,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion


        //#region ParseCommandId             (this Request, CommonAPI, out CommandId,                                                                     out HTTPResponse)

        ///// <summary>
        ///// Parse the given HTTP request and return the command identification
        ///// for the given HTTP hostname and HTTP query parameter
        ///// or an HTTP error response.
        ///// </summary>
        ///// <param name="Request">A HTTP request.</param>
        ///// <param name="CommonAPI">The CommonAPI.</param>
        ///// <param name="CommandId">The parsed unique command identification.</param>
        ///// <param name="OCPIResponseBuilder">An OCPI response builder.</param>
        ///// <returns>True, when user identification was found; false else.</returns>
        //public static Boolean ParseCommandId(this OCPIRequest                                Request,
        //                                     CommonAPI                                       CommonAPI,
        //                                     [NotNullWhen(true)]  out Command_Id?            CommandId,
        //                                     [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder)
        //{

        //    #region Initial checks

        //    if (Request is null)
        //        throw new ArgumentNullException(nameof(Request),  "The given HTTP request must not be null!");

        //    if (CommonAPI  is null)
        //        throw new ArgumentNullException(nameof(CommonAPI),  "The given CPO API must not be null!");

        //    #endregion

        //    CommandId            = default;
        //    OCPIResponseBuilder  = default;

        //    if (Request.ParsedURLParameters.Length < 1)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Missing command identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    CommandId = Command_Id.TryParse(Request.ParsedURLParameters[0]);

        //    if (!CommandId.HasValue)
        //    {

        //        OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
        //            StatusCode           = 2001,
        //            StatusMessage        = "Invalid command identification!",
        //            HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
        //                HTTPStatusCode             = HTTPStatusCode.BadRequest,
        //                //AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
        //                AccessControlAllowHeaders  = [ "Authorization" ]
        //            }
        //        };

        //        return false;

        //    }

        //    return true;

        //}

        //#endregion

    }


    /// <summary>
    /// The CommonAPI.
    /// </summary>
    /// <remarks>
    /// In OCPI 3.0 all data replication will be initiated by the data consumer, never the data producer.
    /// </remarks>
    public class CommonAPI : CommonAPIBase
    {

        #region Data

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

        /// <summary>
        /// The default log file name.
        /// </summary>
        public static readonly     String    DefaultLogfileName        = $"OCPI{Version.Id}-CommonAPI.log";

        /// <summary>
        /// The command values store.
        /// </summary>
        public readonly ConcurrentDictionary<Command_Id, CommandValues> CommandValueStore = [];

        #endregion

        #region Properties

        /// <summary>
        /// All our credential roles.
        /// </summary>
        public IEnumerable<CredentialsRole>  OurCredentialRoles         { get; }

        /// <summary>
        /// The default party identification to use.
        /// </summary>
        public Party_Id                      DefaultPartyId             { get; }

        /// <summary>
        /// Whether to keep or delete EVSEs marked as "REMOVED".
        /// </summary>
        public Func<EVSE, Boolean>           KeepRemovedEVSEs           { get; }

        /// <summary>
        /// Disable OCPI v2.1.1.
        /// </summary>
        public Boolean                       Disable_OCPIv2_1_1         { get; }

        /// <summary>
        /// The CommonAPI logger.
        /// </summary>
        public CommonAPILogger?              CommonAPILogger            { get; }

        #endregion

        #region Events

        #region (protected internal) GetVersionsRequest       (Request)

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetVersionsRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetVersionsRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) GetVersionsResponse      (Response)

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetVersionsResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetVersionsResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) GetVersionRequest        (Request)

        /// <summary>
        /// An event sent whenever a GET version request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionRequest = new ();

        /// <summary>
        /// An event sent whenever a GET version request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetVersionRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetVersionRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) GetVersionResponse       (Response)

        /// <summary>
        /// An event sent whenever a GET version response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionResponse = new ();

        /// <summary>
        /// An event sent whenever a GET version response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetVersionResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetVersionResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) GetCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCredentialsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnGetCredentialsRequest.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) GetCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCredentialsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnGetCredentialsResponse.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion


        #region (protected internal) PostCredentialsRequest   (Request)

        /// <summary>
        /// An event sent whenever a POST credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a POST credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCredentialsRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnPostCredentialsRequest.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) PostCredentialsResponse  (Response)

        /// <summary>
        /// An event sent whenever a POST credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a POST credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCredentialsResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnPostCredentialsResponse.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion


        #region (protected internal) PutCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutCredentialsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPutCredentialsRequest.WhenAll(Timestamp,
                                               API ?? this,
                                               Request) ?? Task.CompletedTask;

        #endregion

        #region (protected internal) PutCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutCredentialsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPutCredentialsResponse.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response) ?? Task.CompletedTask;

        #endregion


        #region (protected internal) DeleteCredentialsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCredentialsRequest(DateTime     Timestamp,
                                                         HTTPAPI      API,
                                                         OCPIRequest  Request)

            => OnDeleteCredentialsRequest.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request);

        #endregion

        #region (protected internal) DeleteCredentialsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The CommonAPI.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCredentialsResponse(DateTime      Timestamp,
                                                          HTTPAPI       API,
                                                          OCPIRequest   Request,
                                                          OCPIResponse  Response)

            => OnDeleteCredentialsResponse.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request,
                                                   Response);

        #endregion

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                     { get; }
        public CustomJObjectSerializerDelegate<PublishToken>?                CustomPublishTokenSerializer                 { get; }
        public CustomJObjectSerializerDelegate<Address>?                     CustomAddressSerializer                      { get; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer        { get; }
        public CustomJObjectSerializerDelegate<ChargingStation>?             CustomChargingStationSerializer              { get; }
        public CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                         { get; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer               { get; }
        public CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                    { get; }
        public CustomJObjectSerializerDelegate<EnergyMeter>?                 CustomEnergyMeterSerializer                  { get; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer   { get; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer         { get; }
        public CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                  { get; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer              { get; }
        public CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                        { get; }
        public CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                        { get; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                    { get; }
        public CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                 { get; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer          { get; }
        public CustomJObjectSerializerDelegate<LocationMaxPower>?            CustomLocationMaxPowerSerializer             { get; }


        public CustomJObjectSerializerDelegate<Tariff>?               CustomTariffSerializer                 { get; }
        public CustomJObjectSerializerDelegate<Price>?                CustomPriceSerializer                  { get; }
        public CustomJObjectSerializerDelegate<TariffElement>?        CustomTariffElementSerializer          { get; }
        public CustomJObjectSerializerDelegate<PriceComponent>?       CustomPriceComponentSerializer         { get; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?   CustomTariffRestrictionsSerializer     { get; }

        #endregion

        #region Constructor(s)

        #region CommonAPI(HTTPServerName, ...)

        /// <summary>
        /// Create a new common HTTP API.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="OurCredentialRoles">All our credential roles.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="HTTPServerPort">An optional HTTP TCP port.</param>
        /// <param name="HTTPServerName">An optional HTTP server name.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional HTTP URL path prefix.</param>
        /// <param name="HTTPServiceName">An optional HTTP service name.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        /// 
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CommonAPI(URL                                                        OurBaseURL,
                         URL                                                        OurVersionsURL,
                         IEnumerable<CredentialsRole>                               OurCredentialRoles,
                         Party_Id                                                   DefaultPartyId,

                         HTTPPath?                                                  AdditionalURLPathPrefix      = null,
                         Func<EVSE, Boolean>?                                       KeepRemovedEVSEs             = null,
                         Boolean                                                    LocationsAsOpenData          = true,
                         Boolean?                                                   AllowDowngrades              = null,
                         Boolean                                                    Disable_RootServices         = true,
                         Boolean                                                    Disable_OCPIv2_1_1           = true,

                         HTTPHostname?                                              HTTPHostname                 = null,
                         String?                                                    ExternalDNSName              = null,
                         IPPort?                                                    HTTPServerPort               = null,
                         HTTPPath?                                                  BasePath                     = null,
                         String?                                                    HTTPServerName               = DefaultHTTPServerName,

                         HTTPPath?                                                  URLPathPrefix                = null,
                         String?                                                    HTTPServiceName              = DefaultHTTPServiceName,
                         JObject?                                                   APIVersionHashes             = null,

                         ServerCertificateSelectorDelegate?                         ServerCertificateSelector    = null,
                         RemoteTLSClientCertificateValidationHandler<IHTTPServer>?  ClientCertificateValidator   = null,
                         LocalCertificateSelectionHandler?                          LocalCertificateSelector    = null,
                         SslProtocols?                                              AllowedTLSProtocols          = null,
                         Boolean?                                                   ClientCertificateRequired    = null,
                         Boolean?                                                   CheckCertificateRevocation   = null,

                         ServerThreadNameCreatorDelegate?                           ServerThreadNameCreator      = null,
                         ServerThreadPriorityDelegate?                              ServerThreadPrioritySetter   = null,
                         Boolean?                                                   ServerThreadIsBackground     = null,
                         ConnectionIdBuilder?                                       ConnectionIdBuilder          = null,
                         TimeSpan?                                                  ConnectionTimeout            = null,
                         UInt32?                                                    MaxClientConnections         = null,

                         Boolean?                                                   DisableMaintenanceTasks      = null,
                         TimeSpan?                                                  MaintenanceInitialDelay      = null,
                         TimeSpan?                                                  MaintenanceEvery             = null,

                         Boolean?                                                   DisableWardenTasks           = null,
                         TimeSpan?                                                  WardenInitialDelay           = null,
                         TimeSpan?                                                  WardenCheckEvery             = null,

                         Boolean?                                                   IsDevelopment                = null,
                         IEnumerable<String>?                                       DevelopmentServers           = null,
                         Boolean?                                                   DisableLogging               = null,
                         String?                                                    LoggingContext               = null,
                         String?                                                    LoggingPath                  = null,
                         String?                                                    LogfileName                  = null,
                         OCPILogfileCreatorDelegate?                                LogfileCreator               = null,
                         String?                                                    DatabaseFilePath             = null,
                         String?                                                    RemotePartyDBFileName        = null,
                         String?                                                    AssetsDBFileName             = null,
                         DNSClient?                                                 DNSClient                    = null,
                         Boolean                                                    AutoStart                    = false)


            : base(Version.Id,
                   OurBaseURL,
                   OurVersionsURL,

                   AdditionalURLPathPrefix,
                   LocationsAsOpenData,
                   AllowDowngrades,
                   Disable_RootServices,

                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServerPort,
                   BasePath,
                   HTTPServerName,

                   URLPathPrefix,
                   HTTPServiceName,
                   APIVersionHashes,

                   ServerCertificateSelector,
                   ClientCertificateValidator,
                   LocalCertificateSelector,
                   AllowedTLSProtocols,
                   ClientCertificateRequired,
                   CheckCertificateRevocation,

                   ServerThreadNameCreator,
                   ServerThreadPrioritySetter,
                   ServerThreadIsBackground,
                   ConnectionIdBuilder,
                   ConnectionTimeout,
                   MaxClientConnections,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingContext,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, remotePartyId, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   DatabaseFilePath,
                   RemotePartyDBFileName,
                   AssetsDBFileName,
                   DNSClient,
                   AutoStart)

        {

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            this.OurCredentialRoles    = OurCredentialRoles.Distinct();
            this.DefaultPartyId        = DefaultPartyId;

            this.KeepRemovedEVSEs      = KeepRemovedEVSEs ?? (evse => true);

            this.Disable_OCPIv2_1_1    = Disable_OCPIv2_1_1;

            this.CommonAPILogger       = this.DisableLogging == false
                                             ? null
                                             : new CommonAPILogger(
                                                   this,
                                                   LoggingContext,
                                                   LoggingPath,
                                                   LogfileCreator
                                               );

            if (!this.DisableLogging)
                ReadRemotePartyDatabaseFile();

            RegisterURLTemplates();

        }

        #endregion

        #region CommonAPI(HTTPServer, ...)

        /// <summary>
        /// Create a new common HTTP API.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="OurCredentialRoles">All our credential roles.</param>
        /// 
        /// <param name="HTTPServer">A HTTP server.</param>
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// 
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CommonAPI(URL                           OurBaseURL,
                         URL                           OurVersionsURL,
                         IEnumerable<CredentialsRole>  OurCredentialRoles,
                         Party_Id                      DefaultPartyId,

                         HTTPServer                    HTTPServer,
                         HTTPHostname?                 HTTPHostname              = null,
                         String?                       ExternalDNSName           = null,
                         HTTPPath?                     URLPathPrefix             = null,
                         HTTPPath?                     BasePath                  = null,
                         String?                       HTTPServiceName           = DefaultHTTPServerName,

                         HTTPPath?                     AdditionalURLPathPrefix   = null,
                         Func<EVSE, Boolean>?          KeepRemovedEVSEs          = null,
                         Boolean                       LocationsAsOpenData       = true,
                         Boolean?                      AllowDowngrades           = null,
                         Boolean                       Disable_RootServices      = false,
                         Boolean                       Disable_OCPIv2_1_1        = true,

                         JObject?                      APIVersionHashes          = null,

                         Boolean?                      DisableMaintenanceTasks   = false,
                         TimeSpan?                     MaintenanceInitialDelay   = null,
                         TimeSpan?                     MaintenanceEvery          = null,

                         Boolean?                      DisableWardenTasks        = false,
                         TimeSpan?                     WardenInitialDelay        = null,
                         TimeSpan?                     WardenCheckEvery          = null,

                         Boolean?                      IsDevelopment             = false,
                         IEnumerable<String>?          DevelopmentServers        = null,
                         Boolean?                      DisableLogging            = false,
                         String?                       LoggingContext            = null,
                         String?                       LoggingPath               = null,
                         String?                       LogfileName               = null,
                         OCPILogfileCreatorDelegate?   LogfileCreator            = null,
                         String?                       DatabaseFilePath          = null,
                         String?                       RemotePartyDBFileName     = null,
                         String?                       AssetsDBFileName          = null,
                         Boolean                       AutoStart                 = false)

            : base(Version.Id,
                   OurBaseURL,
                   OurVersionsURL,
                   HTTPServer,

                   AdditionalURLPathPrefix,
                   LocationsAsOpenData,
                   AllowDowngrades,
                   Disable_RootServices,

                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName,
                   BasePath,

                   URLPathPrefix,
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
                   LoggingContext,
                   LoggingPath,
                   LogfileName ?? DefaultLogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, remotePartyId, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   DatabaseFilePath,
                   RemotePartyDBFileName,
                   AssetsDBFileName,
                   AutoStart)

        {

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            this.OurCredentialRoles    = OurCredentialRoles?.Distinct() ?? Array.Empty<CredentialsRole>();
            this.DefaultPartyId        = DefaultPartyId;

            this.KeepRemovedEVSEs      = KeepRemovedEVSEs ?? (evse => true);

            this.Disable_OCPIv2_1_1    = Disable_OCPIv2_1_1;

            // Link HTTP events...
            HTTPServer.RequestLog     += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog    += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog       += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            this.CommonAPILogger       = this.DisableLogging == false
                                             ? new CommonAPILogger(
                                                   this,
                                                   LoggingContext,
                                                   LoggingPath,
                                                   LogfileCreator
                                               )
                                             : null;

            if (!this.DisableLogging)
                ReadRemotePartyDatabaseFile();

            RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region GetModuleURL(ModuleId, Prefix = "")

        /// <summary>
        /// Return the URL of an OCPI module.
        /// </summary>
        /// <param name="ModuleId">The identification of an OCPI module.</param>
        /// <param name="Prefix">An optional prefix.</param>
        public URL GetModuleURL(Module_Id  ModuleId,
                                String     Prefix   = "")

            => OurBaseURL + Prefix + ModuleId.ToString();

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region OPTIONS     ~/

            HTTPServer.AddMethodCallback(

                this,
                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix,
                HTTPDelegate: request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            Connection                 = ConnectionType.Close
                        }.AsImmutable)

            );

            #endregion

            #region GET         ~/

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPrefix + "/", "cloud.charging.open.protocols.OCPIv3_0.HTTPAPI.CommonAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //this.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPrefix + "/index.html",
            //                                 URLPrefix + "/"
            //                             },
            //                             HTTPContentType.Text.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv3_0.HTTPAPI.CommonAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv3_0.HTTPAPI.CommonAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     ContentType     = HTTPContentType.Text.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = ConnectionType.Close
            //                                 };

            //                             });

            #region Text

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix,
                                         HTTPContentType.Text.PLAIN,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = DefaultHTTPServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                     AccessControlAllowHeaders  = [ "Authorization" ],
                                                     ContentType                = HTTPContentType.Text.PLAIN,
                                                     Content                    = "This is an Open Charge Point Interface HTTP service!\r\nPlease check ~/versions!".ToUTF8Bytes(),
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion


            #region OPTIONS     ~/versions

            // ----------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions
            // ----------------------------------------------------
            this.AddOCPIMethod(

                Hostname,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "versions",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                Server                     = HTTPServiceName,
                                Date                       = Timestamp.Now,
                                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                Vary                       = "Accept"
                            }
                        })

            );

            #endregion

            #region GET         ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions
            // ----------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "versions",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetVersionsRequest,
                               OCPIResponseLogger:  GetVersionsResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   #endregion

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           StatusCode           = 1000,
                                           StatusMessage        = "Hello world!",
                                           Data                 = new JArray(
                                                                      new VersionInformation?[] {
                                                                          Disable_OCPIv2_1_1
                                                                              ? null
                                                                              : new VersionInformation(
                                                                                    Version_Id.Parse("2.1.1"),
                                                                                    URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                              (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + "/versions/2.1.1")).Replace("//", "/"))
                                                                                ),
                                                                          new VersionInformation(
                                                                              Version.Id,
                                                                              URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                        (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + $"/versions/{Version.Id}")).Replace("//", "/"))
                                                                          )
                                                                      }.Where (version => version is not null).
                                                                        Select(version => version?.ToJSON())
                                                                  ),
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                               Vary                       = "Accept"
                                           }
                                       });

                               });

            #endregion


            #region OPTIONS     ~/versions/{versionId}

            // --------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions/{id}
            // --------------------------------------------------------
            this.AddOCPIMethod(

                Hostname,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "versions/{versionId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                Vary                       = "Accept"
                            }
                        })

            );

            #endregion

            #region GET         ~/versions/{versionId}

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions/{id}
            // ---------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "versions/{versionId}",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetVersionRequest,
                               OCPIResponseLogger:  GetVersionResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   #endregion

                                   #region Get version identification URL parameter

                                   if (Request.ParsedURLParameters.Length < 1)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Version identification is missing!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   if (!Version_Id.TryParse(Request.ParsedURLParameters[0], out var versionId))
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Version identification is invalid!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   #endregion

                                   #region Only allow OCPI version v3.0

                                   if (versionId.ToString() != Version.Id.ToString())
                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "This OCPI version is not supported!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   #endregion


                                   var prefix = URLPathPrefix + AdditionalURLPathPrefix + $"v{versionId}";

                                   #region Common credential endpoints...

                                   var endpoints  = new List<VersionEndpoint>() {

                                                        new VersionEndpoint(Module_Id.Credentials,
                                                                            InterfaceRoles.SENDER,
                                                                            URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                      (Request.Host + (prefix + "credentials")).Replace("//", "/"))),

                                                        new VersionEndpoint(Module_Id.Credentials,
                                                                            InterfaceRoles.RECEIVER,
                                                                            URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                      (Request.Host + (prefix + "credentials")).Replace("//", "/")))

                                                    };

                                   #endregion


                                   #region The other side is a CPO...

                                   if (Request.RemoteParty?.Roles.Any(credentialsRole => credentialsRole.Role == Roles.CPO) == true)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") + 
                                                                                   (Request.Host + (prefix + "emsp/locations")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tariffs,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/tariffs")).  Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Sessions,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/sessions")). Replace("//", "/"))));

                                       // When the EMSP acts as smart charging receiver so that a SCSP can talk to him!
                                       endpoints.Add(new VersionEndpoint(Module_Id.ChargingProfiles,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/chargingprofiles")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.CDRs,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/cdrs")).     Replace("//", "/"))));


                                       endpoints.Add(new VersionEndpoint(Module_Id.Commands,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/commands")). Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tokens,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/tokens")).   Replace("//", "/"))));

                                       // hubclientinfo

                                   }

                                   #endregion

                                   #region We are a CPO, the other side is unauthenticated and we export locations as Open Data...

                                   if (OurCredentialRoles.Any(credentialRole => credentialRole.Role == Roles.CPO) &&
                                       LocationsAsOpenData &&
                                       Request.RemoteParty is null)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/locations")).Replace("//", "/"))));

                                   }

                                   #endregion

                                   #region The other side is an EMSP...

                                   if (Request.RemoteParty?.Roles.Any(credentialsRole => credentialsRole.Role == Roles.EMSP) == true)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/locations")).       Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tariffs,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/tariffs")).         Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Sessions,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/sessions")).        Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.ChargingProfiles,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/chargingprofiles")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.CDRs,
                                                                         InterfaceRoles.SENDER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/cdrs")).            Replace("//", "/"))));


                                       endpoints.Add(new VersionEndpoint(Module_Id.Commands,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/commands")).        Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tokens,
                                                                         InterfaceRoles.RECEIVER,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/tokens")).          Replace("//", "/"))));

                                       // hubclientinfo

                                   }

                                   #endregion


                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                               StatusCode           = 1000,
                                               StatusMessage        = "Hello world!",
                                               Data                 = new VersionDetail(
                                                                          versionId,
                                                                          endpoints
                                                                      ).ToJSON(),
                                               HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                                   Vary                       = "Accept"
                                               }
                                           });

                               });

            #endregion


            #region OPTIONS     ~/{versionId}/credentials

            // ----------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/2.2/credentials
            // ----------------------------------------------------------
            this.AddOCPIMethod(

                Hostname,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "{versionId}/credentials",
                OCPIRequestHandler: request => {

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
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       AccessControlAllowMethods  = accessControlAllowMethods,
                                       Allow                      = allow
                                   }
                               });

                }

            );

            #endregion

            #region GET         ~/{versionId}/credentials

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetCredentialsRequest,
                               OCPIResponseLogger:  GetCredentialsResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   #endregion

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           StatusCode           = 1000,
                                           StatusMessage        = "Hello world!",
                                           Data                 = new Credentials(
                                                                      Request.LocalAccessInfo?.AccessToken ?? AccessToken.Parse("<any>"),
                                                                      OurVersionsURL,
                                                                      OurCredentialRoles
                                                                  ).ToJSON(),
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                           }
                                       });

                               });

            #endregion

            #region POST        ~/{versionId}/credentials

            // REGISTER new OCPI party!

            // -----------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // -----------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.POST,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   PostCredentialsRequest,
                               OCPIResponseLogger:  PostCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                   {

                                       if (Request.LocalAccessInfo.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,                                              // CREDENTIALS_TOKEN_A
                                                      StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' is already registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                                      }
                                                  };

                                       return await POSTOrPUTCredentials(Request);

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          };

                               });

            #endregion

            #region PUT         ~/{versionId}/credentials

            // UPDATE the registration of an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.PUT,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   PutCredentialsRequest,
                               OCPIResponseLogger:  PutCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   #region The access token is known...

                                   if (Request.LocalAccessInfo is not null)
                                   {

                                       #region ...but access is blocked!

                                       if (Request.LocalAccessInfo?.Status == AccessStatus.BLOCKED)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + (Request.AccessToken?.ToString() ?? "") + "' is blocked!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                          AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                                      }
                                                  };

                                       #endregion

                                       #region ...and access is allowed, but maybe not yet full registered!

                                       if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                       {

                                           // The party is not yet fully registered!
                                           if (!Request.LocalAccessInfo?.VersionsURL.HasValue == true)
                                               return new OCPIResponse.Builder(Request) {
                                                          StatusCode           = 2000,
                                                          StatusMessage        = "The given access token '" + (Request.AccessToken?.ToString() ?? "") + "' is not yet registered!",
                                                          HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                              HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                              AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST" },
                                                              AccessControlAllowHeaders  = [ "Authorization" ]
                                                          }
                                                      };

                                           return await POSTOrPUTCredentials(Request);

                                       }

                                       #endregion

                                   }

                                   #endregion

                                   return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 2000,
                                                  StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                      AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                      AccessControlAllowHeaders  = [ "Authorization" ]
                                                  }
                                              };

                               });

            #endregion

            #region DELETE      ~/{versionId}/credentials

            // UNREGISTER an existing OCPI party!

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.2/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.DELETE,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   DeleteCredentialsRequest,
                               OCPIResponseLogger:  DeleteCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                   {

                                       #region Validations

                                       if (!Request.LocalAccessInfo.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' is not fully registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                                      }
                                                  };

                                       #endregion

                                       await RemoveAccessToken(Request.LocalAccessInfo.AccessToken);

                                       return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 1000,
                                                  StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' was deleted!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                                                      AccessControlAllowHeaders  = [ "Authorization" ]
                                                  }
                                              };

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
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


            var commonClient              = new CommonClient(this,
                                                             receivedCredentials.URL,
                                                             receivedCredentials.Token,  // CREDENTIALS_TOKEN_B
                                                             DNSClient: HTTPServer.DNSClient);

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
                    if (oldCredentialsRole.CountryCode == receivedCredentialsRole.CountryCode &&
                        oldCredentialsRole.PartyId     == receivedCredentialsRole.PartyId &&
                        oldCredentialsRole.Role        == receivedCredentialsRole.Role)
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
            await RemoveAccessToken     (CREDENTIALS_TOKEN_A.Value);

            // Store credential of the other side!
            await AddOrUpdateRemoteParty(oldRemoteParty.Id,
                                         receivedCredentials.Roles,

                                         CREDENTIALS_TOKEN_C,

                                         receivedCredentials.Token,
                                         receivedCredentials.URL,
                                         otherVersions.Data?.Select(version => version.Id) ?? Array.Empty<Version_Id>(),
                                         Version.Id,

                                         null,
                                         null,
                                         null,
                                         null,
                                         AccessStatus.      ALLOWED,
                                         RemoteAccessStatus.ONLINE,
                                         PartyStatus.       ENABLED);


            return new OCPIResponse.Builder(Request) {
                           StatusCode           = 1000,
                           StatusMessage        = "Hello world!",
                           Data                 = new Credentials(
                                                      CREDENTIALS_TOKEN_C,
                                                      OurVersionsURL,
                                                      OurCredentialRoles
                                                  ).ToJSON(),
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.OK,
                               AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "PUT", "DELETE" ],
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

        }

        #endregion


        //ToDo: Wrap the following into a plugable interface!

        #region AccessTokens

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

                    await LogRemoteParty(removeRemoteParty,
                                         remoteParty.ToJSON(true),
                                         EventTrackingId ?? EventTracking_Id.New,
                                         CurrentUserId);

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

                        await LogRemoteParty(updateRemoteParty,
                                             newRemoteParty.ToJSON(true),
                                             EventTrackingId ?? EventTracking_Id.New,
                                             CurrentUserId);

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

        private readonly ConcurrentDictionary<RemoteParty_Id, RemoteParty> remoteParties = new ();

        /// <summary>
        /// Return an enumeration of all remote parties.
        /// </summary>
        public IEnumerable<RemoteParty> RemoteParties
            => remoteParties.Values;

        #endregion


        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                                  IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                  AccessToken                                                AccessToken,

                                                  AccessToken                                                RemoteAccessToken,
                                                  URL                                                        RemoteVersionsURL,
                                                  IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                  Version_Id?                                                SelectedVersionId            = null,

                                                  DateTime?                                                  LocalAccessNotBefore         = null,
                                                  DateTime?                                                  LocalAccessNotAfter          = null,

                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = false,
                                                  AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                  RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                  PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                  DateTime?                                                  RemoteAccessNotBefore        = null,
                                                  DateTime?                                                  RemoteAccessNotAfter         = null,

                                                  Boolean?                                                   PreferIPv4                   = null,
                                                  RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                  LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                  X509Certificate?                                           ClientCert                   = null,
                                                  SslProtocols?                                              TLSProtocol                  = null,
                                                  String?                                                    HTTPUserAgent                = null,
                                                  TimeSpan?                                                  RequestTimeout               = null,
                                                  TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                  UInt16?                                                    MaxNumberOfRetries           = null,
                                                  UInt32?                                                    InternalBufferSize           = null,
                                                  Boolean?                                                   UseHTTPPipelining            = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(
                                     Id,
                                     CredentialsRoles,

                                     AccessToken,

                                     RemoteAccessToken,
                                     RemoteVersionsURL,
                                     RemoteVersionIds,
                                     SelectedVersionId,

                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,

                                     AccessTokenBase64Encoding,
                                     AllowDowngrades,
                                     AccessStatus,
                                     RemoteStatus,
                                     PartyStatus,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCert,
                                     TLSProtocol,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining
                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty)) {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                                  IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                  AccessToken                                                AccessToken,
                                                  DateTime?                                                  LocalAccessNotBefore         = null,
                                                  DateTime?                                                  LocalAccessNotAfter          = null,
                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = false,
                                                  AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                  PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

                                                  Boolean?                                                   PreferIPv4                   = null,
                                                  RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                  LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                  X509Certificate?                                           ClientCert                   = null,
                                                  SslProtocols?                                              TLSProtocol                  = null,
                                                  String?                                                    HTTPUserAgent                = null,
                                                  TimeSpan?                                                  RequestTimeout               = null,
                                                  TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                  UInt16?                                                    MaxNumberOfRetries           = null,
                                                  UInt32?                                                    InternalBufferSize           = null,
                                                  Boolean?                                                   UseHTTPPipelining            = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(
                                     Id,
                                     CredentialsRoles,

                                     AccessToken,
                                     AccessTokenBase64Encoding,
                                     AllowDowngrades,
                                     AccessStatus,

                                     PartyStatus,
                                     LocalAccessNotBefore,
                                     LocalAccessNotAfter,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCert,
                                     TLSProtocol,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining
                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                                  IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                  AccessToken                                                RemoteAccessToken,
                                                  URL                                                        RemoteVersionsURL,
                                                  IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                  Version_Id?                                                SelectedVersionId            = null,

                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = null,
                                                  RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                  PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                  DateTime?                                                  RemoteAccessNotBefore        = null,
                                                  DateTime?                                                  RemoteAccessNotAfter         = null,

                                                  Boolean?                                                   PreferIPv4                   = null,
                                                  RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                  LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                  X509Certificate?                                           ClientCert                   = null,
                                                  SslProtocols?                                              TLSProtocol                  = null,
                                                  String?                                                    HTTPUserAgent                = null,
                                                  TimeSpan?                                                  RequestTimeout               = null,
                                                  TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                  UInt16?                                                    MaxNumberOfRetries           = null,
                                                  UInt32?                                                    InternalBufferSize           = null,
                                                  Boolean?                                                   UseHTTPPipelining            = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(
                                     Id,
                                     CredentialsRoles,

                                     RemoteAccessToken,
                                     RemoteVersionsURL,
                                     RemoteVersionIds,
                                     SelectedVersionId,

                                     AccessTokenBase64Encoding,
                                     AllowDowngrades,
                                     RemoteStatus,
                                     PartyStatus,
                                     RemoteAccessNotBefore,
                                     RemoteAccessNotAfter,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCert,
                                     TLSProtocol,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining
                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                                  IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                  IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                  IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                  PartyStatus                                                Status                       = PartyStatus.ENABLED,

                                                  Boolean?                                                   PreferIPv4                   = null,
                                                  RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                  LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                  X509Certificate?                                           ClientCert                   = null,
                                                  SslProtocols?                                              TLSProtocol                  = null,
                                                  String?                                                    HTTPUserAgent                = null,
                                                  TimeSpan?                                                  RequestTimeout               = null,
                                                  TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                  UInt16?                                                    MaxNumberOfRetries           = null,
                                                  UInt32?                                                    InternalBufferSize           = null,
                                                  Boolean?                                                   UseHTTPPipelining            = null,

                                                  DateTime?                                                  LastUpdated                  = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(
                                     Id,
                                     CredentialsRoles,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCert,
                                     TLSProtocol,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,

                                     LastUpdated
                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                                             IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                             AccessToken                                                AccessToken,

                                                             AccessToken                                                RemoteAccessToken,
                                                             URL                                                        RemoteVersionsURL,
                                                             IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                             Version_Id?                                                SelectedVersionId            = null,

                                                             DateTime?                                                  LocalAccessNotBefore         = null,
                                                             DateTime?                                                  LocalAccessNotAfter          = null,

                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = false,
                                                             AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                             RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                             PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                             DateTime?                                                  RemoteAccessNotBefore        = null,
                                                             DateTime?                                                  RemoteAccessNotAfter         = null,

                                                             Boolean?                                                   PreferIPv4                   = null,
                                                             RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                             LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                             X509Certificate?                                           ClientCert                   = null,
                                                             SslProtocols?                                              TLSProtocol                  = null,
                                                             String?                                                    HTTPUserAgent                = null,
                                                             TimeSpan?                                                  RequestTimeout               = null,
                                                             TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                             UInt16?                                                    MaxNumberOfRetries           = null,
                                                             UInt32?                                                    InternalBufferSize           = null,
                                                             Boolean?                                                   UseHTTPPipelining            = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 AccessToken,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                                             IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                             AccessToken                                                AccessToken,
                                                             DateTime?                                                  LocalAccessNotBefore         = null,
                                                             DateTime?                                                  LocalAccessNotAfter          = null,
                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = false,
                                                             AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                             PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

                                                             Boolean?                                                   PreferIPv4                   = null,
                                                             RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                             LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                             X509Certificate?                                           ClientCert                   = null,
                                                             SslProtocols?                                              TLSProtocol                  = null,
                                                             String?                                                    HTTPUserAgent                = null,
                                                             TimeSpan?                                                  RequestTimeout               = null,
                                                             TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                             UInt16?                                                    MaxNumberOfRetries           = null,
                                                             UInt32?                                                    InternalBufferSize           = null,
                                                             Boolean?                                                   UseHTTPPipelining            = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                                             IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                             AccessToken                                                RemoteAccessToken,
                                                             URL                                                        RemoteVersionsURL,
                                                             IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                             Version_Id?                                                SelectedVersionId            = null,

                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = null,
                                                             RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                             PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                             DateTime?                                                  RemoteAccessNotBefore        = null,
                                                             DateTime?                                                  RemoteAccessNotAfter         = null,

                                                             Boolean?                                                   PreferIPv4                   = null,
                                                             RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                             LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                             X509Certificate?                                           ClientCert                   = null,
                                                             SslProtocols?                                              TLSProtocol                  = null,
                                                             String?                                                    HTTPUserAgent                = null,
                                                             TimeSpan?                                                  RequestTimeout               = null,
                                                             TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                             UInt16?                                                    MaxNumberOfRetries           = null,
                                                             UInt32?                                                    InternalBufferSize           = null,
                                                             Boolean?                                                   UseHTTPPipelining            = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(RemoteParty_Id                                             Id,
                                                             IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                             IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                             IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                             PartyStatus                                                Status                       = PartyStatus.ENABLED,

                                                             Boolean?                                                   PreferIPv4                   = null,
                                                             RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                             LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                             X509Certificate?                                           ClientCert                   = null,
                                                             SslProtocols?                                              TLSProtocol                  = null,
                                                             String?                                                    HTTPUserAgent                = null,
                                                             TimeSpan?                                                  RequestTimeout               = null,
                                                             TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                             UInt16?                                                    MaxNumberOfRetries           = null,
                                                             UInt32?                                                    InternalBufferSize           = null,
                                                             Boolean?                                                   UseHTTPPipelining            = null,

                                                             DateTime?                                                  LastUpdated                  = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

                                                 Status,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining,

                                                 LastUpdated);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                                          IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                          AccessToken                                                AccessToken,

                                                          AccessToken                                                RemoteAccessToken,
                                                          URL                                                        RemoteVersionsURL,
                                                          IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                          Version_Id?                                                SelectedVersionId            = null,

                                                          DateTime?                                                  LocalAccessNotBefore         = null,
                                                          DateTime?                                                  LocalAccessNotAfter          = null,

                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = false,
                                                          AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                          RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                          PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                          DateTime?                                                  RemoteAccessNotBefore        = null,
                                                          DateTime?                                                  RemoteAccessNotAfter         = null,

                                                          Boolean?                                                   PreferIPv4                   = null,
                                                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                          X509Certificate?                                           ClientCert                   = null,
                                                          SslProtocols?                                              TLSProtocol                  = null,
                                                          String?                                                    HTTPUserAgent                = null,
                                                          TimeSpan?                                                  RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                          UInt16?                                                    MaxNumberOfRetries           = null,
                                                          UInt32?                                                    InternalBufferSize           = null,
                                                          Boolean?                                                   UseHTTPPipelining            = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 AccessToken,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                                          IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                          AccessToken                                                AccessToken,
                                                          DateTime?                                                  LocalAccessNotBefore         = null,
                                                          DateTime?                                                  LocalAccessNotAfter          = null,
                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = false,
                                                          AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                          PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

                                                          Boolean?                                                   PreferIPv4                   = null,
                                                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                          X509Certificate?                                           ClientCert                   = null,
                                                          SslProtocols?                                              TLSProtocol                  = null,
                                                          String?                                                    HTTPUserAgent                = null,
                                                          TimeSpan?                                                  RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                          UInt16?                                                    MaxNumberOfRetries           = null,
                                                          UInt32?                                                    InternalBufferSize           = null,
                                                          Boolean?                                                   UseHTTPPipelining            = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                                          IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                          AccessToken                                                RemoteAccessToken,
                                                          URL                                                        RemoteVersionsURL,
                                                          IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                          Version_Id?                                                SelectedVersionId            = null,

                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = null,
                                                          RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                          PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                          DateTime?                                                  RemoteAccessNotBefore        = null,
                                                          DateTime?                                                  RemoteAccessNotAfter         = null,

                                                          Boolean?                                                   PreferIPv4                   = null,
                                                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                          X509Certificate?                                           ClientCert                   = null,
                                                          SslProtocols?                                              TLSProtocol                  = null,
                                                          String?                                                    HTTPUserAgent                = null,
                                                          TimeSpan?                                                  RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                          UInt16?                                                    MaxNumberOfRetries           = null,
                                                          UInt32?                                                    InternalBufferSize           = null,
                                                          Boolean?                                                   UseHTTPPipelining            = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(RemoteParty_Id                                             Id,
                                                          IEnumerable<CredentialsRole>                               CredentialsRoles,

                                                          IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                          IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                          PartyStatus                                                Status                       = PartyStatus.ENABLED,

                                                          Boolean?                                                   PreferIPv4                   = null,
                                                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                          X509Certificate?                                           ClientCert                   = null,
                                                          SslProtocols?                                              TLSProtocol                  = null,
                                                          String?                                                    HTTPUserAgent                = null,
                                                          TimeSpan?                                                  RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                          UInt16?                                                    MaxNumberOfRetries           = null,
                                                          UInt32?                                                    InternalBufferSize           = null,
                                                          Boolean?                                                   UseHTTPPipelining            = null,

                                                          DateTime?                                                  LastUpdated                  = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(Id,
                                                 CredentialsRoles,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

                                                 Status,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining,

                                                 LastUpdated);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion


        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,

                                                     AccessToken                                                AccessToken,

                                                     AccessToken                                                RemoteAccessToken,
                                                     URL                                                        RemoteVersionsURL,
                                                     IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                     Version_Id?                                                SelectedVersionId            = null,

                                                     DateTime?                                                  LocalAccessNotBefore         = null,
                                                     DateTime?                                                  LocalAccessNotAfter          = null,

                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = false,
                                                     AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                     RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                     PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                     DateTime?                                                  RemoteAccessNotBefore        = null,
                                                     DateTime?                                                  RemoteAccessNotAfter         = null,

                                                     Boolean?                                                   PreferIPv4                   = null,
                                                     RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                     LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                     X509Certificate?                                           ClientCert                   = null,
                                                     SslProtocols?                                              TLSProtocol                  = null,
                                                     String?                                                    HTTPUserAgent                = null,
                                                     TimeSpan?                                                  RequestTimeout               = null,
                                                     TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                     UInt16?                                                    MaxNumberOfRetries           = null,
                                                     UInt32?                                                    InternalBufferSize           = null,
                                                     Boolean?                                                   UseHTTPPipelining            = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.Id,
                                                 ExistingRemoteParty.Roles,

                                                 AccessToken,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,

                                                     AccessToken                                                AccessToken,
                                                     DateTime?                                                  LocalAccessNotBefore         = null,
                                                     DateTime?                                                  LocalAccessNotAfter          = null,
                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = false,
                                                     AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                     PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

                                                     Boolean?                                                   PreferIPv4                   = null,
                                                     RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                     LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                     X509Certificate?                                           ClientCert                   = null,
                                                     SslProtocols?                                              TLSProtocol                  = null,
                                                     String?                                                    HTTPUserAgent                = null,
                                                     TimeSpan?                                                  RequestTimeout               = null,
                                                     TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                     UInt16?                                                    MaxNumberOfRetries           = null,
                                                     UInt32?                                                    InternalBufferSize           = null,
                                                     Boolean?                                                   UseHTTPPipelining            = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.Id,
                                                 ExistingRemoteParty.Roles,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,

                                                     AccessToken                                                RemoteAccessToken,
                                                     URL                                                        RemoteVersionsURL,
                                                     IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                     Version_Id?                                                SelectedVersionId            = null,

                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = null,
                                                     RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                     PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                     DateTime?                                                  RemoteAccessNotBefore        = null,
                                                     DateTime?                                                  RemoteAccessNotAfter         = null,

                                                     Boolean?                                                   PreferIPv4                   = null,
                                                     RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                     LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                     X509Certificate?                                           ClientCert                   = null,
                                                     SslProtocols?                                              TLSProtocol                  = null,
                                                     String?                                                    HTTPUserAgent                = null,
                                                     TimeSpan?                                                  RequestTimeout               = null,
                                                     TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                     UInt16?                                                    MaxNumberOfRetries           = null,
                                                     UInt32?                                                    InternalBufferSize           = null,
                                                     Boolean?                                                   UseHTTPPipelining            = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.Id,
                                                 ExistingRemoteParty.Roles,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,

                                                     IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                     IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                     PartyStatus                                                Status                       = PartyStatus.ENABLED,

                                                     Boolean?                                                   PreferIPv4                   = null,
                                                     RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                     LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                     X509Certificate?                                           ClientCert                   = null,
                                                     SslProtocols?                                              TLSProtocol                  = null,
                                                     String?                                                    HTTPUserAgent                = null,
                                                     TimeSpan?                                                  RequestTimeout               = null,
                                                     TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                     UInt16?                                                    MaxNumberOfRetries           = null,
                                                     UInt32?                                                    InternalBufferSize           = null,
                                                     Boolean?                                                   UseHTTPPipelining            = null,

                                                     DateTime?                                                  LastUpdated                  = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.Id,
                                                 ExistingRemoteParty.Roles,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

                                                 Status,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining,

                                                 LastUpdated);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region ContainsRemoteParty(RemotePartyId)

        /// <summary>
        /// Whether this API contains a remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public Boolean ContainsRemoteParty(RemoteParty_Id RemotePartyId)

            => remoteParties.ContainsKey(RemotePartyId);

        #endregion

        #region GetRemoteParty     (RemotePartyId)

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

        #region TryGetRemoteParty  (RemotePartyId, out RemoteParty)

        /// <summary>
        /// Try to get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        /// <param name="RemoteParty">The remote party.</param>
        public Boolean TryGetRemoteParty(RemoteParty_Id    RemotePartyId,
                                         out RemoteParty?  RemoteParty)

            => remoteParties.TryGetValue(RemotePartyId,
                                         out RemoteParty);

        #endregion

        #region GetRemoteParties   (IncludeFilter = null)

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

        #region GetRemoteParties   (CountryCode, PartyId)

        /// <summary>
        /// Get all remote parties having the given country code and party identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Id     PartyId)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.CountryCode == CountryCode &&
                                                                                           credentialsRole.PartyId     == PartyId));

        #endregion

        #region GetRemoteParties   (CountryCode, PartyId, Role)

        /// <summary>
        /// Get all remote parties having the given country code, party identification and role.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A role.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Id     PartyId,
                                                         Roles        Role)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.CountryCode == CountryCode &&
                                                                                           credentialsRole.PartyId     == PartyId &&
                                                                                           credentialsRole.Role        == Role));

        #endregion

        #region GetRemoteParties   (Role)

        /// <summary>
        /// Get all remote parties having the given role.
        /// </summary>
        /// <param name="Role">The role of the remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(Roles Role)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.Role == Role));

        #endregion

        #region GetRemoteParties   (AccessToken)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken AccessToken)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken));

        #endregion

        #region GetRemoteParties   (AccessToken, AccessStatus)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken   AccessToken,
                                                         AccessStatus  AccessStatus)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken &&
                                                                                                      localAccessInfo.Status      == AccessStatus));

        #endregion

        #region GetRemoteParties   (AccessToken, out RemoteParties)

        public Boolean TryGetRemoteParties(AccessToken                   AccessToken,
                                           out IEnumerable<RemoteParty>  RemoteParties)
        {

            RemoteParties = remoteParties.Values.
                                          Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken));

            return RemoteParties.Any();

        }

        #endregion


        #region RemoveRemoteParty(RemoteParty)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty        RemoteParty,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.TryRemove(RemoteParty.Id, out var remoteParty))
            {

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty(RemotePartyId)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty_Id     RemotePartyId,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.Remove(RemotePartyId, out var remoteParty))
            {

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, Role)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     Roles              Role,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in GetRemoteParties(CountryCode,
                                                         PartyId,
                                                         Role))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

            }

            return true;

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, Role, AccessToken)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     Roles              Role,
                                                     AccessToken        AccessToken,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.
                                                      Where(remoteParty => remoteParty.Roles.            Any(credentialsRole  => credentialsRole.CountryCode == CountryCode &&
                                                                                                                                 credentialsRole.PartyId     == PartyId &&
                                                                                                                                 credentialsRole.Role        == Role) &&

                                                                           remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == AccessToken)))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

            }

            return true;

        }

        #endregion

        #region RemoveAllRemoteParties()

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

        #region CPOClients

        private readonly ConcurrentDictionary<EMSP_Id, CPOClient> cpoClients = new ();

        /// <summary>
        /// Return an enumeration of all CPO clients.
        /// </summary>
        public IEnumerable<CPOClient> CPOClients
            => cpoClients.Values;


        #region GetCPOClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(CountryCode    CountryCode,
                                       OCPI.Party_Id  PartyId,
                                       I18NString?    Description          = null,
                                       Boolean        AllowCachedClients   = true)
        {

            var emspId         = EMSP_Id.       Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (emspId);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (remoteParties.TryGetValue(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(remotePartyId),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                    ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                    ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(RemoteParty  RemoteParty,
                                       I18NString?  Description          = null,
                                       Boolean      AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    RemoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                    ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                    ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(RemoteParty_Id  RemotePartyId,
                                       I18NString?     Description          = null,
                                       Boolean         AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(RemotePartyId),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                    ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                    ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #endregion

        #region EMSPClients

        private readonly ConcurrentDictionary<CPO_Id, EMSPClient> emspClients = new ();

        /// <summary>
        /// Return an enumeration of all EMSP clients.
        /// </summary>
        public IEnumerable<EMSPClient> EMSPClients
            => emspClients.Values;


        #region GetEMSPClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(CountryCode    CountryCode,
                                         OCPI.Party_Id  PartyId,
                                         I18NString?    Description          = null,
                                         Boolean        AllowCachedClients   = true)
        {

            var cpoId          = CPO_Id.        Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (cpoId);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (remoteParties.TryGetValue(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     remoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(remotePartyId),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                     ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                     ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(RemoteParty  RemoteParty,
                                         I18NString?  Description          = null,
                                         Boolean      AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     RemoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                     ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                     ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(RemoteParty_Id  RemotePartyId,
                                         I18NString?     Description          = null,
                                         Boolean         AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     remoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(RemotePartyId),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                     ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                     ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #endregion


        private readonly ConcurrentDictionary<Party_Id, PartyData> parties = [];

        #region Locations

        #region Locations

        #region Events

        public delegate Task OnLocationAddedDelegate    (Location Location);
        public delegate Task OnLocationChangedDelegate  (Location Location);
        public delegate Task OnLocationRemovedDelegate  (Location Location);

        public event OnLocationAddedDelegate?      OnLocationAdded;
        public event OnLocationChangedDelegate?    OnLocationChanged;
        public event OnLocationRemovedDelegate?    OnLocationRemoved;

        #endregion


        #region AddLocation            (Location, SkipNotifications = false)

        public async Task<AddResult<Location>>

            AddLocation(Location           Location,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Location.PartyId, out var party))
            {

                if (party.Locations.TryAdd(Location.Id, Location))
                {

                    Location.CommonAPI = this;

                    await LogAsset(
                              addLocation,
                              Location.ToJSON(true,
                                              true,
                                              true,
                                              CustomLocationSerializer,
                                              CustomPublishTokenSerializer,
                                              CustomAddressSerializer,
                                              CustomAdditionalGeoLocationSerializer,
                                              CustomChargingStationSerializer,
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
                                              CustomLocationMaxPowerSerializer),
                              EventTrackingId,
                              CurrentUserId
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

                        //var OnEVSEAddedLocal = OnEVSEAdded;
                        //if (OnEVSEAddedLocal is not null)
                        //{
                        //    try
                        //    {
                        //        foreach (var evse in Location.EVSEs)
                        //            await OnEVSEAddedLocal(evse);
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnEVSEAdded), ": ",
                        //                    Environment.NewLine, e.Message,
                        //                    Environment.NewLine, e.StackTrace ?? "");
                        //    }
                        //}

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
                       "The party identification of the location is unknown!"
                   );

        }

        #endregion

        #region AddLocationIfNotExists (Location, SkipNotifications = false)

        public async Task<AddResult<Location>>

            AddLocationIfNotExists(Location           Location,
                                   Boolean            SkipNotifications   = false,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Location.PartyId, out var party))
            {

                if (party.Locations.TryAdd(Location.Id, Location))
                {

                    Location.CommonAPI = this;

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
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
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

                return AddResult<Location>.Success(
                           EventTrackingId,
                           Location,
                           "The given location already exists."
                       );

            }

            return AddResult<Location>.Failed(
                       EventTrackingId,
                       Location,
                       "The party identification of the location is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateLocation    (newOrUpdatedLocation, AllowDowngrades = false, ...)

        public async Task<AddOrUpdateResult<Location>>

            AddOrUpdateLocation(Location           Location,
                                Boolean?           AllowDowngrades     = false,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Location.PartyId, out var party))
            {

                if (party.Locations.TryGetValue(Location.Id, out var existingLocation))
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

                    if (Location.LastUpdated.ToIso8601() == existingLocation.LastUpdated.ToIso8601())
                        return AddOrUpdateResult<Location>.NoOperation(
                                   EventTrackingId,
                                   Location,
                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!"
                               );

                    party.Locations[Location.Id] = Location;
                    Location.CommonAPI = this;

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

                    //var OnEVSEChangedLocal = OnEVSEChanged;
                    //if (OnEVSEChangedLocal is not null)
                    //{
                    //    try
                    //    {
                    //        foreach (var evse in Location.EVSEs)
                    //            OnEVSEChangedLocal(evse).Wait();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                    //                    Environment.NewLine, e.Message,
                    //                    Environment.NewLine, e.StackTrace ?? "");
                    //    }
                    //}

                    return AddOrUpdateResult<Location>.Updated(
                               EventTrackingId,
                               Location
                           );

                }

                party.Locations.TryAdd(Location.Id, Location);
                Location.CommonAPI = this;

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

                return AddOrUpdateResult<Location>.Created(
                           EventTrackingId,
                           Location
                       );

            }

            return AddOrUpdateResult<Location>.Failed(
                       EventTrackingId,
                       Location,
                       "The given party of the location is unknown!"
                   );

        }

        #endregion

        #region UpdateLocation         (Location)

        public Location? UpdateLocation(Location           Location,
                                        Boolean            SkipNotifications   = false,
                                        EventTracking_Id?  EventTrackingId     = null,
                                        User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Location.PartyId, out var party))
            {

                if (party.Locations.ContainsKey(Location.Id))
                {

                    party.Locations[Location.Id] = Location;
                    Location.CommonAPI = this;

                    //var OnEVSEChangedLocal = OnEVSEChanged;
                    //if (OnEVSEChangedLocal is not null)
                    //{
                    //    try
                    //    {
                    //        foreach (var evse in Location.EVSEs)
                    //            OnEVSEChangedLocal(evse).Wait();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                    //                    Environment.NewLine, e.Message,
                    //                    Environment.NewLine, e.StackTrace ?? "");
                    //    }
                    //}

                    return Location;

                }

                return null;

            }

            return null;

        }

        #endregion

        #region TryPatchLocation       (Location, LocationPatch, AllowDowngrades = false)

        public async Task<PatchResult<Location>> TryPatchLocation(Location           Location,
                                                                  JObject            LocationPatch,
                                                                  Boolean?           AllowDowngrades     = false,
                                                                  Boolean            SkipNotifications   = false,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (LocationPatch is null || !LocationPatch.HasValues)
                return PatchResult<Location>.Failed(
                           EventTrackingId,
                           Location,
                           "The given location patch must not be null or empty!"
                       );

            if (parties.TryGetValue(Location.PartyId, out var party))
            {

                if (party.Locations.TryGetValue(Location.Id, out var existingLocation))
                {

                    var patchResult = existingLocation.TryPatch(LocationPatch,
                                                                AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess)
                    {

                        party.Locations[Location.Id] = patchResult.PatchedData;

                        var OnLocationChangedLocal = OnLocationChanged;
                        if (OnLocationChangedLocal is not null)
                        {
                            try
                            {
                                OnLocationChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchLocation), " ", nameof(OnLocationChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                        ////ToDo: MayBe nothing changed here... perhaps test for changes before sending events!
                        //var OnEVSEChangedLocal = OnEVSEChanged;
                        //if (OnEVSEChangedLocal is not null)
                        //{
                        //    try
                        //    {
                        //        foreach (var evse in patchResult.PatchedData.EVSEs)
                        //            OnEVSEChangedLocal(evse).Wait();
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchLocation), " ", nameof(OnEVSEChanged), ": ",
                        //                    Environment.NewLine, e.Message,
                        //                    Environment.NewLine, e.StackTrace ?? "");
                        //    }
                        //}

                    }

                    return patchResult;

                }

                else
                    return PatchResult<Location>.Failed(
                               EventTrackingId,
                               Location,
                               "The given location does not exist!"
                           );

            }

            return PatchResult<Location>.Failed(
                       EventTrackingId,
                       Location,
                       "The given party of the location is unknown!"
                   );

        }

        #endregion


        #region LocationExists         (PartyId, LocationId, VersionId = null)

        public Boolean LocationExists(Party_Id     PartyId,
                                      Location_Id  LocationId,
                                      UInt64?      VersionId   = null)
        {

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (VersionId.HasValue &&
                    party.Locations.TryGetValue(LocationId, out var location))
                {
                    return location.VersionId == VersionId;
                }

                return party.Locations.ContainsKey(LocationId);

            }

            return false;

        }

        #endregion

        #region TryGetLocation         (PartyId, LocationId, out Location, VersionId = null)

        public Boolean TryGetLocation(Party_Id                           PartyId,
                                      Location_Id                        LocationId,
                                      [NotNullWhen(true)] out Location?  Location,
                                      UInt64?                            VersionId   = null)
        {

            if (parties.        TryGetValue(PartyId,    out var party) &&
                party.Locations.TryGetValue(LocationId, out var location))
            {

                if (VersionId.HasValue)
                {
                    //ToDo: Check VersionId!
                    Location = location;
                    return true;
                }

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

            var allLocations = new List<Location>();

            foreach (var party in parties.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (location is not null &&
                        IncludeLocation(location))
                    {
                        allLocations.Add(location);
                    }
                }
            }

            return allLocations;

        }

        #endregion

        #region GetLocations           (PartyId = null)

        public IEnumerable<Location> GetLocations(Party_Id? PartyId = null)
        {

            if (PartyId.HasValue)
            {

                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Locations.Values;

                return [];

            }

            return parties.Values.SelectMany(party => party.Locations.Values);

        }

        #endregion


        #region RemoveLocation         (PartyId, Location, ...)

        public async Task<RemoveResult<Location>>

            RemoveLocation(Party_Id           PartyId,
                           Location           Location,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties .       TryGetValue(PartyId,     out var party) &&
                party.Locations.TryRemove  (Location.Id, out var location))
            {
                return RemoveResult<Location>.Success(
                           EventTrackingId,
                           Location
                       );
            }

            return RemoveResult<Location>.Failed(
                       EventTrackingId,
                       Location,
                       "The given location does not exist within this API!"
                   );

        }

        #endregion

        #region RemoveLocation         (PartyId, LocationId, ...)

        public async Task<RemoveResult<Location>>

            RemoveLocation(Party_Id           PartyId,
                           Location_Id        LocationId,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties .       TryGetValue(PartyId,    out var party) &&
                party.Locations.TryRemove  (LocationId, out var location))
            {
                return RemoveResult<Location>.Success(
                           EventTrackingId,
                           location
                       );
            }

            return RemoveResult<Location>.Failed(
                       EventTrackingId,
                       "The given location does not exist within this API!"
                   );

        }

        #endregion

        #region RemoveLocations        (IncludeLocations, ...)

        /// <summary>
        /// Removes matching locations.
        /// </summary>
        /// <param name="IncludeLocationIds">A delegate for selecting which locations to remove.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveLocations(Func<Location, Boolean>  IncludeLocationIds,
                            Boolean                  SkipNotifications   = false,
                            EventTracking_Id?        EventTrackingId     = null,
                            User_Id?                 CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var allLocations = new List<Location>();

            foreach (var party in parties.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (location is not null &&
                        IncludeLocationIds(location))
                    {
                        allLocations.Add(location);
                    }
                }
            }

            foreach (var location in allLocations)
            {
                if (parties.TryGetValue(location.PartyId, out var party))
                {
                    if (!party.Locations.TryRemove(location.Id, out _))
                    {
                        return RemoveResult<IEnumerable<Location>>.Failed(
                                   EventTrackingId,
                                   [ location ],
                                   $"The matched location '{location.PartyId}:{location.Id}' could not be removed!"
                               );
                    }
                }
            }

            return RemoveResult<IEnumerable<Location>>.Success(
                       EventTrackingId,
                       allLocations
                   );

        }

        #endregion

        #region RemoveLocations        (IncludeLocationIds, ...)

        /// <summary>
        /// Removes matching locations.
        /// </summary>
        /// <param name="IncludeLocationIds">A delegate for selecting which locations to remove.</param>
        public async Task<RemoveResult<IEnumerable<Location>>>

            RemoveLocations(Func<Party_Id, Location_Id, Boolean>  IncludeLocationIds,
                            Boolean                               SkipNotifications   = false,
                            EventTracking_Id?                     EventTrackingId     = null,
                            User_Id?                              CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var allLocations = new List<Location>();

            foreach (var party in parties.Values)
            {
                foreach (var location in party.Locations.Values)
                {
                    if (IncludeLocationIds(party.Id, location.Id))
                        allLocations.Add(location);
                }
            }

            foreach (var location in allLocations)
            {
                if (parties.TryGetValue(location.PartyId, out var party))
                {
                    if (!party.Locations.TryRemove(location.Id, out _))
                    {
                        return RemoveResult<IEnumerable<Location>>.Failed(
                                   EventTrackingId,
                                   [ location ],
                                   $"The matched location '{location.PartyId}:{location.Id}' could not be removed!"
                               );
                    }
                }
            }

            return RemoveResult<IEnumerable<Location>>.Success(
                       EventTrackingId,
                       allLocations
                   );

        }

        #endregion

        #region RemoveAllLocations     (PartyId = null, ...)

        /// <summary>
        /// Removes all locations.
        /// </summay>
        /// <param name="PartyId">The optional identification of a party.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> 

            RemoveAllLocations(Party_Id?          PartyId             = null,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedLocations = new List<Location>();

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                {
                    removedLocations.AddRange(party.Locations.Values);
                    party.Locations.Clear();
                }
            }

            else
            {
                foreach (var party in parties.Values)
                {
                    removedLocations.AddRange(party.Locations.Values);
                    party.Locations.Clear();
                }
            }

            return RemoveResult<IEnumerable<Location>>.Success(
                       EventTrackingId,
                       removedLocations
                   );

        }

        #endregion

        #endregion

        #region Charging Stations


        #region AddOrUpdateChargingStation       (Location, newOrUpdatedChargingStation, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<ChargingStation>>

            AddOrUpdateChargingStation(Location           Location,
                                       ChargingStation    newOrUpdatedChargingStation,
                                       Boolean?           AllowDowngrades   = false,
                                       EventTracking_Id?  EventTrackingId   = null,
                                       User_Id?           CurrentUserId     = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            Location.TryGetChargingStation(newOrUpdatedChargingStation.Id, out var existingChargingStation);

            if (existingChargingStation is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedChargingStation.LastUpdated < existingChargingStation.LastUpdated)
                {
                    return AddOrUpdateResult<ChargingStation>.Failed     (
                               EventTrackingId,
                               newOrUpdatedChargingStation,
                               "The 'lastUpdated' timestamp of the new ChargingStation must be newer then the timestamp of the existing ChargingStation!"
                           );
                }

                if (newOrUpdatedChargingStation.LastUpdated.ToIso8601() == existingChargingStation.LastUpdated.ToIso8601())
                    return AddOrUpdateResult<ChargingStation>.NoOperation(
                               EventTrackingId,
                               newOrUpdatedChargingStation,
                               "The 'lastUpdated' timestamp of the new ChargingStation must be newer then the timestamp of the existing ChargingStation!"
                           );

            }


            //Location.SetChargingStation(newOrUpdatedChargingStation);

            // Update charging station timestamp!
            var builder = Location.ToBuilder();
            builder.LastUpdated = newOrUpdatedChargingStation.LastUpdated;
            await AddOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);




            var OnLocationChangedLocal = OnLocationChanged;
            if (OnLocationChangedLocal is not null)
            {
                try
                {
                    OnLocationChangedLocal(newOrUpdatedChargingStation.ParentLocation).Wait();
                }
                catch (Exception e)
                {
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateChargingStation), " ", nameof(OnLocationChanged), ": ",
                                Environment.NewLine, e.Message,
                                e.StackTrace is not null
                                            ? Environment.NewLine + e.StackTrace
                                            : String.Empty);
                }
            }


            if (existingChargingStation is not null)
            {

                //if (existingChargingStation.Status != StatusType.REMOVED)
                //{

                    //var OnChargingStationChangedLocal = OnChargingStationChanged;
                    //if (OnChargingStationChangedLocal is not null)
                    //{
                    //    try
                    //    {
                    //        OnChargingStationChangedLocal(newOrUpdatedChargingStation).Wait();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateChargingStation), " ", nameof(OnChargingStationChanged), ": ",
                    //                    Environment.NewLine, e.Message,
                    //                    e.StackTrace is not null
                    //                        ? Environment.NewLine + e.StackTrace
                    //                        : String.Empty);
                    //    }
                    //}


                    //if (existingChargingStation.Status != newOrUpdatedChargingStation.Status)
                    //{
                    //    var OnChargingStationStatusChangedLocal = OnChargingStationStatusChanged;
                    //    if (OnChargingStationStatusChangedLocal is not null)
                    //    {
                    //        try
                    //        {

                    //            OnChargingStationStatusChangedLocal(Timestamp.Now,
                    //                                     newOrUpdatedChargingStation,
                    //                                     existingChargingStation.Status,
                    //                                     newOrUpdatedChargingStation.Status).Wait();

                    //        }
                    //        catch (Exception e)
                    //        {
                    //            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateChargingStation), " ", nameof(OnChargingStationStatusChanged), ": ",
                    //                        Environment.NewLine, e.Message,
                    //                        e.StackTrace is not null
                    //                            ? Environment.NewLine + e.StackTrace
                    //                            : String.Empty);
                    //        }
                    //    }
                    //}

                //}
                //else
                //{

                    //if (!KeepRemovedChargingStations(newOrUpdatedChargingStation))
                    //    Location.RemoveChargingStation(newOrUpdatedChargingStation);

                    //var OnChargingStationRemovedLocal = OnChargingStationRemoved;
                    //if (OnChargingStationRemovedLocal is not null)
                    //{
                    //    try
                    //    {
                    //        OnChargingStationRemovedLocal(newOrUpdatedChargingStation).Wait();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateChargingStation), " ", nameof(OnChargingStationRemoved), ": ",
                    //                    Environment.NewLine, e.Message,
                    //                    e.StackTrace is not null
                    //                        ? Environment.NewLine + e.StackTrace
                    //                        : String.Empty);
                    //    }
                    //}

                //}
            }
            else
            {
                //var OnChargingStationAddedLocal = OnChargingStationAdded;
                //if (OnChargingStationAddedLocal is not null)
                //{
                //    try
                //    {
                //        OnChargingStationAddedLocal(newOrUpdatedChargingStation).Wait();
                //    }
                //    catch (Exception e)
                //    {
                //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateChargingStation), " ", nameof(OnChargingStationAdded), ": ",
                //                    Environment.NewLine, e.Message,
                //                    e.StackTrace is not null
                //                            ? Environment.NewLine + e.StackTrace
                //                            : String.Empty);
                //    }
                //}
            }

            return existingChargingStation is null
                       ? AddOrUpdateResult<ChargingStation>.Created(EventTrackingId, newOrUpdatedChargingStation)
                       : AddOrUpdateResult<ChargingStation>.Updated(EventTrackingId, newOrUpdatedChargingStation);

        }

        #endregion


        #endregion

        #region EVSEs

        public delegate Task OnEVSEAddedDelegate(EVSE EVSE);

        public event OnEVSEAddedDelegate? OnEVSEAdded;


        public delegate Task OnEVSEChangedDelegate(EVSE EVSE);

        public event OnEVSEChangedDelegate? OnEVSEChanged;

        public delegate Task OnEVSEStatusChangedDelegate(DateTime Timestamp, EVSE EVSE, StatusType OldEVSEStatus, StatusType NewEVSEStatus);

        public event OnEVSEStatusChangedDelegate? OnEVSEStatusChanged;


        public delegate Task OnEVSERemovedDelegate(EVSE EVSE);

        public event OnEVSERemovedDelegate? OnEVSERemoved;


        #region AddOrUpdateEVSE       (ChargingStation, newOrUpdatedEVSE, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<EVSE>>

            AddOrUpdateEVSE(ChargingStation    ChargingStation,
                            EVSE               newOrUpdatedEVSE,
                            Boolean?           AllowDowngrades   = false,
                            EventTracking_Id?  EventTrackingId   = null,
                            User_Id?           CurrentUserId     = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            ChargingStation.TryGetEVSE(newOrUpdatedEVSE.UId, out var existingEVSE);

            if (existingEVSE is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    newOrUpdatedEVSE.LastUpdated < existingEVSE.LastUpdated)
                {
                    return AddOrUpdateResult<EVSE>.Failed     (
                               EventTrackingId,
                               newOrUpdatedEVSE,
                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!"
                           );
                }

                if (newOrUpdatedEVSE.LastUpdated.ToIso8601() == existingEVSE.LastUpdated.ToIso8601())
                    return AddOrUpdateResult<EVSE>.NoOperation(
                               EventTrackingId,
                               newOrUpdatedEVSE,
                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!"
                           );

            }


            //Location.SetEVSE(newOrUpdatedEVSE);

            // Update charging station timestamp!
            var builder = ChargingStation.ToBuilder();
            builder.LastUpdated = newOrUpdatedEVSE.LastUpdated;
            await AddOrUpdateChargingStation(ChargingStation.ParentLocation, builder, (AllowDowngrades ?? this.AllowDowngrades) == false);




            //var OnLocationChangedLocal = OnLocationChanged;
            //if (OnLocationChangedLocal is not null)
            //{
            //    try
            //    {
            //        OnLocationChangedLocal(newOrUpdatedEVSE.ParentLocation).Wait();
            //    }
            //    catch (Exception e)
            //    {
            //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnLocationChanged), ": ",
            //                    Environment.NewLine, e.Message,
            //                    e.StackTrace is not null
            //                                ? Environment.NewLine + e.StackTrace
            //                                : String.Empty);
            //    }
            //}


            if (existingEVSE is not null)
            {

                //if (existingEVSE.Status != StatusType.REMOVED)
                //{

                    //var OnEVSEChangedLocal = OnEVSEChanged;
                    //if (OnEVSEChangedLocal is not null)
                    //{
                    //    try
                    //    {
                    //        OnEVSEChangedLocal(newOrUpdatedEVSE).Wait();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEChanged), ": ",
                    //                    Environment.NewLine, e.Message,
                    //                    e.StackTrace is not null
                    //                        ? Environment.NewLine + e.StackTrace
                    //                        : String.Empty);
                    //    }
                    //}


                    //if (existingEVSE.Status != newOrUpdatedEVSE.Status)
                    //{
                    //    var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                    //    if (OnEVSEStatusChangedLocal is not null)
                    //    {
                    //        try
                    //        {

                    //            OnEVSEStatusChangedLocal(Timestamp.Now,
                    //                                     newOrUpdatedEVSE,
                    //                                     existingEVSE.Status,
                    //                                     newOrUpdatedEVSE.Status).Wait();

                    //        }
                    //        catch (Exception e)
                    //        {
                    //            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEStatusChanged), ": ",
                    //                        Environment.NewLine, e.Message,
                    //                        e.StackTrace is not null
                    //                            ? Environment.NewLine + e.StackTrace
                    //                            : String.Empty);
                    //        }
                    //    }
                    //}

                //}
                //else
                //{

                    //if (!KeepRemovedEVSEs(newOrUpdatedEVSE))
                    //    Location.RemoveEVSE(newOrUpdatedEVSE);

                    //var OnEVSERemovedLocal = OnEVSERemoved;
                    //if (OnEVSERemovedLocal is not null)
                    //{
                    //    try
                    //    {
                    //        OnEVSERemovedLocal(newOrUpdatedEVSE).Wait();
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSERemoved), ": ",
                    //                    Environment.NewLine, e.Message,
                    //                    e.StackTrace is not null
                    //                        ? Environment.NewLine + e.StackTrace
                    //                        : String.Empty);
                    //    }
                    //}

                //}
            }
            else
            {
                //var OnEVSEAddedLocal = OnEVSEAdded;
                //if (OnEVSEAddedLocal is not null)
                //{
                //    try
                //    {
                //        OnEVSEAddedLocal(newOrUpdatedEVSE).Wait();
                //    }
                //    catch (Exception e)
                //    {
                //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateEVSE), " ", nameof(OnEVSEAdded), ": ",
                //                    Environment.NewLine, e.Message,
                //                    e.StackTrace is not null
                //                            ? Environment.NewLine + e.StackTrace
                //                            : String.Empty);
                //    }
                //}
            }

            return existingEVSE is null
                       ? AddOrUpdateResult<EVSE>.Created(EventTrackingId, newOrUpdatedEVSE)
                       : AddOrUpdateResult<EVSE>.Updated(EventTrackingId, newOrUpdatedEVSE);

        }

        #endregion

        #region TryPatchChargingStation(Location, ChargingStation, EVSEPatch,  AllowDowngrades = false)

        public async Task<PatchResult<ChargingStation>> TryPatchChargingStation(Location           Location,
                                                                                ChargingStation    ChargingStation,
                                                                                JObject            EVSEPatch,
                                                                                Boolean?           AllowDowngrades   = false,
                                                                                EventTracking_Id?  EventTrackingId   = null,
                                                                                User_Id?           CurrentUserId     = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (!EVSEPatch.HasValues)
                return PatchResult<ChargingStation>.Failed(
                           EventTrackingId,
                           ChargingStation,
                           "The given EVSE patch must not be null or empty!"
                       );

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);

            //lock (locations)
            //{

                var patchResult        = ChargingStation.TryPatch(EVSEPatch,
                                                       AllowDowngrades ?? this.AllowDowngrades ?? false);

                var justAStatusChange  = EVSEPatch.Children().Count() == 2 && EVSEPatch.ContainsKey("status") && EVSEPatch.ContainsKey("last_updated");

                if (patchResult.IsSuccess)
                {

                    //if (patchResult.PatchedData.Status != StatusType.REMOVED || KeepRemovedEVSEs(EVSE))
                    //    Location.SetEVSE   (patchResult.PatchedData);
                    //else
                    //    Location.RemoveEVSE(patchResult.PatchedData);

                    // Update location timestamp!
                    var builder = Location.ToBuilder();
                    builder.LastUpdated = patchResult.PatchedData.LastUpdated;
                    await AddOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);


                    //if (ChargingStation.Status != StatusType.REMOVED)
                    //{

                    //    if (justAStatusChange)
                    //    {

                            //DebugX.Log("EVSE status change: " + ChargingStation.EVSEId + " => " + patchResult.PatchedData.Status);

                            //var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                            //if (OnEVSEStatusChangedLocal is not null)
                            //{
                            //    try
                            //    {

                            //        OnEVSEStatusChangedLocal(patchResult.PatchedData.LastUpdated,
                            //                                 ChargingStation,
                            //                                 ChargingStation.Status,
                            //                                 patchResult.PatchedData.Status).Wait();

                            //    }
                            //    catch (Exception e)
                            //    {
                            //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchChargingStation), " ", nameof(OnEVSEStatusChanged), ": ",
                            //                    Environment.NewLine, e.Message,
                            //                    e.StackTrace is not null
                            //                        ? Environment.NewLine + e.StackTrace
                            //                        : String.Empty);
                            //    }
                            //}

                        //}
                        //else
                        //{

                            //var OnEVSEChangedLocal = OnEVSEChanged;
                            //if (OnEVSEChangedLocal is not null)
                            //{
                            //    try
                            //    {
                            //        OnEVSEChangedLocal(patchResult.PatchedData).Wait();
                            //    }
                            //    catch (Exception e)
                            //    {
                            //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchChargingStation), " ", nameof(OnEVSEChanged), ": ",
                            //                    Environment.NewLine, e.Message,
                            //                    e.StackTrace is not null
                            //                        ? Environment.NewLine + e.StackTrace
                            //                        : String.Empty);
                            //    }
                            //}

                    //    }

                    //}
                    //else
                    //{

                        //var OnEVSERemovedLocal = OnEVSERemoved;
                        //if (OnEVSERemovedLocal is not null)
                        //{
                        //    try
                        //    {
                        //        OnEVSERemovedLocal(patchResult.PatchedData).Wait();
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchChargingStation), " ", nameof(OnEVSERemoved), ": ",
                        //                    Environment.NewLine, e.Message,
                        //                    e.StackTrace is not null
                        //                        ? Environment.NewLine + e.StackTrace
                        //                        : String.Empty);
                        //    }
                        //}

                    //}

                }

                return patchResult;

            //}

        }

        #endregion


        #region AddOrUpdateConnector  (Location, EVSE, newOrUpdatedConnector,     AllowDowngrades = false)

        public async Task<AddOrUpdateResult<Connector>> AddOrUpdateConnector(Location           Location,
                                                                             EVSE               EVSE,
                                                                             Connector          newOrUpdatedConnector,
                                                                             Boolean?           AllowDowngrades   = false,
                                                                             EventTracking_Id?  EventTrackingId   = null,
                                                                             User_Id?           CurrentUserId     = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (EVSE     is null)
                return AddOrUpdateResult<Connector>.Failed(
                           EventTrackingId,
                           newOrUpdatedConnector,
                           "The given EVSE must not be null!"
                       );

            if (newOrUpdatedConnector is null)
                return AddOrUpdateResult<Connector>.Failed(
                           EventTrackingId,
                           newOrUpdatedConnector,
                           "The given connector must not be null!"
                       );

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);


                var ConnectorExistedBefore = EVSE.TryGetConnector(newOrUpdatedConnector.Id, out var existingConnector);

                if (existingConnector is not null)
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        newOrUpdatedConnector.LastUpdated < existingConnector.LastUpdated)
                    {
                        return AddOrUpdateResult<Connector>.Failed     (
                                   EventTrackingId,
                                   newOrUpdatedConnector,
                                   "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!"
                               );
                    }

                    if (newOrUpdatedConnector.LastUpdated.ToIso8601() == existingConnector.LastUpdated.ToIso8601())
                        return AddOrUpdateResult<Connector>.NoOperation(
                                   EventTrackingId,
                                   newOrUpdatedConnector,
                                   "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!"
                               );

                }

                EVSE.UpdateConnector(newOrUpdatedConnector);

                //// Update EVSE/location timestamps!
                //var evseBuilder     = EVSE.ToBuilder();
                //evseBuilder.LastUpdated = newOrUpdatedConnector.LastUpdated;
                //__addOrUpdateEVSE    (Location, evseBuilder,     (AllowDowngrades ?? this.AllowDowngrades) == false);


                //var OnLocationChangedLocal = OnLocationChanged;
                //if (OnLocationChangedLocal is not null)
                //{
                //    try
                //    {
                //        OnLocationChangedLocal(newOrUpdatedConnector.ParentEVSE.ParentLocation).Wait();
                //    }
                //    catch (Exception e)
                //    {
                //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateConnector), " ", nameof(OnLocationChanged), ": ",
                //                    Environment.NewLine, e.Message,
                //                    Environment.NewLine, e.StackTrace ?? "");
                //    }
                //}


                return ConnectorExistedBefore
                           ? AddOrUpdateResult<Connector>.Updated(EventTrackingId, newOrUpdatedConnector)
                           : AddOrUpdateResult<Connector>.Created(EventTrackingId,newOrUpdatedConnector);


        }

        #endregion

        #region TryPatchConnector     (Location, EVSE, Connector, ConnectorPatch, AllowDowngrades = false)

        public async Task<PatchResult<Connector>> TryPatchConnector(Location           Location,
                                                                    EVSE               EVSE,
                                                                    Connector          Connector,
                                                                    JObject            ConnectorPatch,
                                                                    Boolean?           AllowDowngrades   = false,
                                                                    EventTracking_Id?  EventTrackingId   = null,
                                                                    User_Id?           CurrentUserId     = null)
        {

            if (EVSE is null)
                return PatchResult<Connector>.Failed(EventTrackingId, Connector,
                                                     "The given EVSE must not be null!");

            if (Connector is null)
                return PatchResult<Connector>.Failed(EventTrackingId, Connector,
                                                     "The given connector must not be null!");

            if (ConnectorPatch is null || !ConnectorPatch.HasValues)
                return PatchResult<Connector>.Failed(EventTrackingId, Connector,
                                                     "The given connector patch must not be null or empty!");

            // ToDo: Remove me and add a proper 'lock' mechanism!
            await Task.Delay(1);


                var patchResult = Connector.TryPatch(ConnectorPatch,
                                                     AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess)
                {

                    EVSE.UpdateConnector(patchResult.PatchedData);

                    //// Update EVSE/location timestamps!
                    //var evseBuilder     = EVSE.ToBuilder();
                    //evseBuilder.LastUpdated = patchResult.PatchedData.LastUpdated;
                    //__addOrUpdateEVSE    (Location, evseBuilder,     (AllowDowngrades ?? this.AllowDowngrades) == false);

                }

                return patchResult;


        }

        #endregion

        #endregion

        #endregion

        #region Tariffs

        #region Events

        public delegate Task OnTariffAddedDelegate(Tariff Tariff);

        public event OnTariffAddedDelegate? OnTariffAdded;


        public delegate Task OnTariffChangedDelegate(Tariff Tariff);

        public event OnTariffChangedDelegate? OnTariffChanged;

        #endregion


        #region AddTariff            (Tariff,                                       SkipNotifications = false, ...)

        public async Task<AddResult<Tariff>> AddTariff(Tariff             Tariff,
                                                       Boolean            SkipNotifications   = false,
                                                       EventTracking_Id?  EventTrackingId     = null,
                                                       User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Tariff.PartyId, out var party))
            {

                if (party.Tariffs.TryAdd(Tariff.Id, Tariff))
                {

                    Tariff.CommonAPI = this;

                    await LogAsset(addTariff,
                                   Tariff.ToJSON(true,
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
                                                 CustomEnvironmentalImpactSerializer),
                                   EventTrackingId,
                                   CurrentUserId);

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

                    return AddResult<Tariff>.Success(
                               EventTrackingId,
                               Tariff
                           );

                }

            }

            return AddResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       "TryAdd(Tariff.Id, Tariff) failed!"
                   );

        }

        #endregion

        #region AddTariffIfNotExists (Tariff,                                       SkipNotifications = false, ...)

        public async Task<AddResult<Tariff>> AddTariffIfNotExists(Tariff             Tariff,
                                                                  Boolean            SkipNotifications   = false,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Tariff.PartyId, out var party))
            {

                if (party.Tariffs.TryAdd(Tariff.Id, Tariff))
                {

                    Tariff.CommonAPI = this;

                    await LogAsset(addTariffIfNotExists,
                                   Tariff.ToJSON(true,
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
                                                 CustomEnvironmentalImpactSerializer),
                                   EventTrackingId,
                                   CurrentUserId);

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

                    return AddResult<Tariff>.Success(
                               EventTrackingId,
                               Tariff
                           );

                }

            }

            return AddResult<Tariff>.NoOperation(
                       EventTrackingId,
                       Tariff
                   );

        }

        #endregion

        #region AddOrUpdateTariff    (Tariff,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<AddOrUpdateResult<Tariff>> AddOrUpdateTariff(Tariff             Tariff,
                                                                       Boolean?           AllowDowngrades     = false,
                                                                       Boolean            SkipNotifications   = false,
                                                                       EventTracking_Id?  EventTrackingId     = null,
                                                                       User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Tariff.PartyId, out var party))
            {

                #region Update Tariff

                if (party.Tariffs.TryGetValue(Tariff.Id,
                                              out var existingTariff,
                                              Tariff.NotBefore ?? DateTime.MinValue))
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

                    party.Tariffs.AddOrUpdate(Tariff.Id, Tariff);
                    Tariff.CommonAPI = this;

                    await LogAsset(addOrUpdateTariff,
                                   Tariff.ToJSON(true,
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
                                                 CustomEnvironmentalImpactSerializer),
                                   EventTrackingId,
                                   CurrentUserId);

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

                    return AddOrUpdateResult<Tariff>.Updated(
                               EventTrackingId,
                               Tariff
                           );

                }

                #endregion

                #region Add a new tariff

                if (party.Tariffs.TryAdd(Tariff.Id, Tariff))
                {

                    Tariff.CommonAPI = this;

                    await LogAsset(addOrUpdateTariff,
                                   Tariff.ToJSON(true,
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
                                                 CustomEnvironmentalImpactSerializer),
                                   EventTrackingId,
                                   CurrentUserId);

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

                    return AddOrUpdateResult<Tariff>.Created(
                               EventTrackingId,
                               Tariff
                           );

                }

                #endregion

            }

            return AddOrUpdateResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       "AddOrUpdateTariff(Tariff.Id, Tariff) failed!"
                   );

        }

        #endregion

        #region UpdateTariff         (Tariff,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<UpdateResult<Tariff>> UpdateTariff(Tariff             Tariff,
                                                             Boolean?           AllowDowngrades     = false,
                                                             Boolean            SkipNotifications   = false,
                                                             EventTracking_Id?  EventTrackingId     = null,
                                                             User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Tariff.PartyId, out var party))
            {

                #region Validate AllowDowngrades

                if (party.Tariffs.TryGetValue(Tariff.Id, out var existingTariff, Timestamp.Now))
                {

                    if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                        Tariff.LastUpdated <= existingTariff.LastUpdated)
                    {

                        return UpdateResult<Tariff>.Failed(
                                   EventTrackingId,
                                   Tariff,
                                   "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!"
                               );

                    }

                }
                else
                    return UpdateResult<Tariff>.Failed(
                               EventTrackingId,
                               Tariff,
                               $"Unknown tariff identification '{Tariff.Id}'!"
                           );

                #endregion

                if (party.Tariffs.TryUpdate(Tariff.Id, Tariff, existingTariff))
                {

                    Tariff.CommonAPI = this;

                    await LogAsset(updateTariff,
                                   Tariff.ToJSON(true,
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
                                                 CustomEnvironmentalImpactSerializer),
                                   EventTrackingId,
                                   CurrentUserId);

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

                    return UpdateResult<Tariff>.Success(
                               EventTrackingId,
                               Tariff
                           );

                }

            }

            return UpdateResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       "UpdateTariff(Tariff.Id, Tariff, Tariff) failed!"
                   );

        }

        #endregion

        #region TryPatchTariff       (Tariff, TariffPatch, AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<PatchResult<Tariff>> TryPatchTariff(Tariff             Tariff,
                                                              JObject            TariffPatch,
                                                              Boolean?           AllowDowngrades     = false,
                                                              Boolean            SkipNotifications   = false,
                                                              EventTracking_Id?  EventTrackingId     = null,
                                                              User_Id?           CurrentUserId       = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            if (!TariffPatch.HasValues)
                return PatchResult<Tariff>.Failed(
                           EventTrackingId,
                           Tariff,
                           "The given charging tariff patch must not be null or empty!"
                       );

            if (parties.TryGetValue(Tariff.PartyId, out var party))
            {

                if (party.Tariffs.TryGetValue(Tariff.Id, out var existingTariff, Timestamp.Now))
                {

                    var patchResult = existingTariff.TryPatch(TariffPatch,
                                                              AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess &&
                        patchResult.PatchedData is not null)
                    {

                        party.Tariffs.TryUpdate(Tariff.Id, Tariff, patchResult.PatchedData);

                        await LogAsset(updateTariff,
                                       Tariff.ToJSON(true,
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
                                                     CustomEnvironmentalImpactSerializer),
                                       EventTrackingId ?? EventTracking_Id.New,
                                       CurrentUserId);

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
                    return PatchResult<Tariff>.Failed(
                               EventTrackingId,
                               Tariff,
                               "The given charging tariff does not exist!"
                           );

            }

            return PatchResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       "The given party does not exist!"
                   );

        }

        #endregion


        #region TariffExists         (PartyId, TariffId, VersionId = null)

        public Boolean TariffExists(Party_Id   PartyId,
                                    Tariff_Id  TariffId,
                                    UInt64?    VersionId   = null)
        {

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (VersionId.HasValue &&
                    party.Tariffs.TryGetValue(TariffId, out var tariff))
                {
                    return tariff.VersionId == VersionId;
                }

                return party.Tariffs.ContainsKey(TariffId);

            }

            return false;

        }

        #endregion

        #region TryGetTariff         (PartyId, TariffId, out Tariff, VersionId = null)

        public Boolean TryGetTariff(Party_Id                         PartyId,
                                    Tariff_Id                        TariffId,
                                    [NotNullWhen(true)] out Tariff?  Tariff,
                                    UInt64?                          VersionId   = null)
        {

            if (parties.      TryGetValue(PartyId,  out var party) &&
                party.Tariffs.TryGetValue(TariffId, out var location))
            {

                if (VersionId.HasValue)
                {
                    //ToDo: Check VersionId!
                    Tariff = location;
                    return true;
                }

                Tariff = location;
                return true;

            }

            Tariff = null;
            return false;

        }

        #endregion

        #region GetTariffs           (IncludeTariff)

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, Boolean> IncludeTariff)
        {

            var allTariffs = new List<Tariff>();

            foreach (var party in parties.Values)
            {
                foreach (var location in party.Tariffs.Values())
                {
                    if (location is not null &&
                        IncludeTariff(location))
                    {
                        allTariffs.Add(location);
                    }
                }
            }

            return allTariffs;

        }

        #endregion

        #region GetTariffs           (PartyId = null)

        public IEnumerable<Tariff> GetTariffs(Party_Id? PartyId = null)
        {

            if (PartyId.HasValue)
            {

                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Tariffs.Values();

                return [];

            }

            return parties.Values.SelectMany(party => party.Tariffs.Values());

        }

        #endregion


        #region GetTariffIds(CountryCode?, PartyId?, LocationId?, EVSEUId?, ConnectorId?, EMSPId?)

        //public GetTariffIds2_Delegate? GetTariffIdsDelegate { get; set; }

        //public IEnumerable<Tariff_Id> GetTariffIds(CountryCode    CountryCode,
        //                                           Party_Id       PartyId,
        //                                           Location_Id?   LocationId,
        //                                           EVSE_UId?      EVSEUId,
        //                                           Connector_Id?  ConnectorId,
        //                                           EMSP_Id?       EMSPId)

        //    => GetTariffIdsDelegate?.Invoke(CountryCode,
        //                                    PartyId,
        //                                    LocationId,
        //                                    EVSEUId,
        //                                    ConnectorId,
        //                                    EMSPId) ?? [];

        #endregion


        #region RemoveTariff         (PartyId, Tariff, ...)

        public async Task<RemoveResult<Tariff>>

            RemoveTariff(Party_Id           PartyId,
                         Tariff             Tariff,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties .     TryGetValue(PartyId,   out var party) &&
                party.Tariffs.TryRemove  (Tariff.Id, out var removedTariffs))
            {

                if (removedTariffs.Any())
                    await LogAsset(removeTariff,
                                   new JArray(
                                       removedTariffs.Select(tariff => tariff.ToJSON(
                                                                           true,
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
                                   CurrentUserId);

                return RemoveResult<Tariff>.Success(
                           EventTrackingId,
                           Tariff
                       );
            }

            return RemoveResult<Tariff>.Failed(
                       EventTrackingId,
                       Tariff,
                       "The given tariff does not exist within this API!"
                   );

        }

        #endregion

        #region RemoveTariff         (PartyId, TariffId, ...)

        public async Task<RemoveResult<Tariff>>

            RemoveTariff(Party_Id           PartyId,
                         Tariff_Id          TariffId,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties .     TryGetValue(PartyId,  out var party) &&
                party.Tariffs.TryRemove  (TariffId, out var removedTariffs))
            {

                if (removedTariffs.Any())
                    await LogAsset(removeTariff,
                                   new JArray(
                                       removedTariffs.Select(tariff => tariff.ToJSON(
                                                                           true,
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
                                   CurrentUserId);

                return RemoveResult<Tariff>.Success(
                           EventTrackingId,
                           removedTariffs.Last()
                       );

            }

            return RemoveResult<Tariff>.Failed(
                       EventTrackingId,
                       "The given tariff identification does not exist within this API!"
                   );

        }

        #endregion

        #region RemoveTariffs        (IncludeTariffs, ...)

        /// <summary>
        /// Removes matching tariffs.
        /// </summary>
        /// <param name="IncludeTariffs">A delegate for selecting which tariffs to remove.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveTariffs(Func<Tariff, Boolean>  IncludeTariffs,
                          Boolean                SkipNotifications   = false,
                          EventTracking_Id?      EventTrackingId     = null,
                          User_Id?               CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs = new List<Tariff>();

            foreach (var party in parties.Values)
            {
                foreach (var tariff in party.Tariffs.Values())
                {
                    if (tariff is not null &&
                        IncludeTariffs(tariff))
                    {
                        removedTariffs.Add(tariff);
                    }
                }
            }

            foreach (var tariff in removedTariffs)
            {
                if (parties.TryGetValue(tariff.PartyId, out var party))
                {
                    if (!party.Tariffs.TryRemove(tariff.Id, out _))
                    {
                        return RemoveResult<IEnumerable<Tariff>>.Failed(
                                   EventTrackingId,
                                   [ tariff ],
                                   $"The matched tariff '{tariff.PartyId}:{tariff.Id}' could not be removed!"
                               );
                    }
                }
            }


            if (removedTariffs.Count > 0)
                await LogAsset(removeTariff,
                                new JArray(
                                    removedTariffs.Select(tariff => tariff.ToJSON(
                                                                        true,
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
                                CurrentUserId);

            return RemoveResult<IEnumerable<Tariff>>.Success(
                       EventTrackingId,
                       removedTariffs
                   );

        }

        #endregion

        #region RemoveTariffs        (IncludeTariffIds, ...)

        /// <summary>
        /// Removes matching tariffs.
        /// </summary>
        /// <param name="IncludeTariffIds">A delegate for selecting which tariffs to remove.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>>

            RemoveTariffs(Func<Party_Id, Tariff_Id, Boolean>  IncludeTariffIds,
                          Boolean                             SkipNotifications   = false,
                          EventTracking_Id?                   EventTrackingId     = null,
                          User_Id?                            CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs = new List<Tariff>();

            foreach (var party in parties.Values)
            {
                foreach (var tariff in party.Tariffs.Values())
                {
                    if (IncludeTariffIds(party.Id, tariff.Id))
                        removedTariffs.Add(tariff);
                }
            }

            foreach (var tariff in removedTariffs)
            {
                if (parties.TryGetValue(tariff.PartyId, out var party))
                {
                    if (!party.Tariffs.TryRemove(tariff.Id, out _))
                    {
                        return RemoveResult<IEnumerable<Tariff>>.Failed(
                                   EventTrackingId,
                                   [ tariff ],
                                   $"The matched tariff '{tariff.PartyId}:{tariff.Id}' could not be removed!"
                               );
                    }
                }
            }


            if (removedTariffs.Count > 0)
                await LogAsset(removeTariff,
                                new JArray(
                                    removedTariffs.Select(tariff => tariff.ToJSON(
                                                                        true,
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
                                CurrentUserId);

            return RemoveResult<IEnumerable<Tariff>>.Success(
                       EventTrackingId,
                       removedTariffs
                   );

        }

        #endregion

        #region RemoveAllTariffs     (PartyId = null, ...)

        /// <summary>
        /// Removes all tariffs.
        /// </summay>
        /// <param name="PartyId">The optional identification of a party.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> 

            RemoveAllTariffs(Party_Id?          PartyId             = null,
                             Boolean            SkipNotifications   = false,
                             EventTracking_Id?  EventTrackingId     = null,
                             User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs = new List<Tariff>();

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                {
                    removedTariffs.AddRange(party.Tariffs.Values());
                    party.Tariffs.Clear();
                }
            }

            else
            {
                foreach (var party in parties.Values)
                {
                    removedTariffs.AddRange(party.Tariffs.Values());
                    party.Tariffs.Clear();
                }
            }

            if (removedTariffs.Count > 0)
                await LogAsset(removeTariff,
                                new JArray(
                                    removedTariffs.Select(tariff => tariff.ToJSON(
                                                                        true,
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
                                CurrentUserId);

            return RemoveResult<IEnumerable<Tariff>>.Success(
                       EventTrackingId,
                       removedTariffs
                   );

        }

        #endregion

        #endregion


    }

}
