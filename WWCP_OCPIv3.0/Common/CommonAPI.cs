/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
                                                                   Party_Idv3     CPOPartyId,
                                                                   Location_Id?   Location      = null,
                                                                   EVSE_UId?      EVSEUId       = null,
                                                                   Connector_Id?  ConnectorId   = null,
                                                                   EMSP_Id?       EMSPId        = null);

    public class PartyData(Party_Idv3       Id,
                           Role             Role,
                           BusinessDetails  BusinessDetails,
                           Boolean?         AllowDowngrades   = null)
    {

        public Party_Idv3                                      Id                 { get; } = Id;
        public Role                                            Role               { get; } = Role;
        public BusinessDetails                                 BusinessDetails    { get; } = BusinessDetails;
        public Boolean                                         AllowDowngrades    { get; } = AllowDowngrades ?? false;

        public ConcurrentDictionary<Location_Id, Location>     Locations          { get; } = [];
        public ConcurrentDictionary<Tariff_Id,   Tariff>       Tariffs            { get; } = [];
        public ConcurrentDictionary<Session_Id,  Session>      Sessions           { get; } = [];
        public ConcurrentDictionary<Token_Id,    TokenStatus>  Tokens             { get; } = [];
        public ConcurrentDictionary<CDR_Id,      CDR>          CDRs               { get; } = [];

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
                                           [NotNullWhen(true)]  out Party_Idv3?              PartyId,
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

            PartyId = Party_Idv3.TryParse(Request.ParsedURLParameters[0]);

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
                                            [NotNullWhen(true)]  out Party_Idv3?            PartyId,
                                            [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                                 out Location?              Location,
                                            [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                            Boolean                                         FailOnMissingLocation = true)
        {

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

            PartyId = Party_Idv3.TryParse(Request.ParsedURLParameters[0]);

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
                                                           [NotNullWhen(true)]  out Party_Idv3?              PartyId,
                                                           [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                           [NotNullWhen(true)]  out Location?              Location,
                                                           [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                                                out ChargingStation?       ChargingStation,
                                                           [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                           Boolean                                         FailOnMissingChargingStation = true)
        {

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

            PartyId = Party_Idv3.TryParse(Request.ParsedURLParameters[0]);

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
                                                               [NotNullWhen(true)]  out Party_Idv3?              PartyId,
                                                               [NotNullWhen(true)]  out Location_Id?           LocationId,
                                                               [NotNullWhen(true)]  out Location?              Location,
                                                               [NotNullWhen(true)]  out ChargingStation_Id?    ChargingStationId,
                                                               [NotNullWhen(true)]  out ChargingStation?       ChargingStation,
                                                               [NotNullWhen(true)]  out EVSE_UId?              EVSEUId,
                                                                                    out EVSE?                  EVSE,
                                                               [NotNullWhen(false)] out OCPIResponse.Builder?  OCPIResponseBuilder,
                                                               Boolean                                         FailOnMissingEVSE = true)
        {

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

            PartyId = Party_Idv3.TryParse(Request.ParsedURLParameters[0]);

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
                                                                        [NotNullWhen(true)]  out Party_Idv3?              PartyId,
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

            PartyId = Party_Idv3.TryParse(Request.ParsedURLParameters[0]);

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
                                          [NotNullWhen(true)]  out Party_Idv3?              PartyId,
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

            PartyId = Party_Idv3.TryParse(Request.ParsedURLParameters[0]);

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
                                                  [NotNullWhen(true)]  out Party_Idv3?              PartyId,
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

            PartyId = Party_Idv3.TryParse(Request.ParsedURLParameters[0]);

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


        #region ParseMandatorySession      (this Request, CommonAPI, out CountryCode, out PartyId, out SessionId, out Session,  out OCPIResponseBuilder)

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

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or session identification!",
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

            SessionId = Session_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!SessionId.HasValue) {

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


            if (!CommonAPI.TryGetSession(Party_Idv3.From(CountryCode.Value, PartyId.Value), SessionId.Value, out Session))
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

        #region ParseOptionalSession       (this Request, CommonAPI, out CountryCode, out PartyId, out SessionId, out Session,  out OCPIResponseBuilder)

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

            if (Request.ParsedURLParameters.Length < 3)
            {

                OCPIResponseBuilder = new OCPIResponse.Builder(Request) {
                    StatusCode           = 2001,
                    StatusMessage        = "Missing country code, party identification and/or session identification!",
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

            SessionId = Session_Id.TryParse(Request.ParsedURLParameters[2]);

            if (!SessionId.HasValue) {

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


            CommonAPI.TryGetSession(Party_Idv3.From(CountryCode.Value, PartyId.Value), SessionId.Value, out Session);

            return true;

        }

        #endregion



    }


    /// <summary>
    /// The CommonAPI.
    /// </summary>
    /// 
    /// <remarks>
    /// In OCPI 3.0 all data replication will be initiated by the data consumer, never the data producer.
    /// </remarks>
    public class CommonAPI : HTTPAPI // : CommonAPIBase
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
        /// The default database file name for all remote party configuration.
        /// </summary>
        public const               String      DefaultRemotePartyDBFileName    = "RemoteParties.db";

        /// <summary>
        /// The default database file name for all OCPI assets.
        /// </summary>
        public const               String      DefaultAssetsDBFileName         = "Assets.db";

        /// <summary>
        /// The command values store.
        /// </summary>
        public readonly ConcurrentDictionary<Command_Id, CommandValues> CommandValueStore = [];

        #endregion

        #region Properties

        public CommonBaseAPI            BaseAPI                     { get; }

        /// <summary>
        /// The (max supported) OCPI version.
        /// </summary>
        public Version_Id               OCPIVersion                 { get; } = Version.Id;

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?                 AllowDowngrades            { get; }



        /// <summary>
        /// All our credential roles.
        /// </summary>
        //public IEnumerable<CredentialsRole>  OurCredentialRoles         { get; }

        /// <summary>
        /// The default party identification to use.
        /// </summary>
        public Party_Idv3                    DefaultPartyId             { get; }

        /// <summary>
        /// Whether to keep or delete EVSEs marked as "REMOVED".
        /// </summary>
        public Func<EVSE, Boolean>           KeepRemovedEVSEs           { get; }

        /// <summary>
        /// The CommonAPI logger.
        /// </summary>
        public CommonAPILogger?              CommonAPILogger            { get; }



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

        public CustomJObjectSerializerDelegate<VersionInformation>?            CustomVersionInformationSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<VersionDetail>?                 CustomVersionDetailSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<VersionEndpoint>?               CustomVersionEndpointSerializer               { get; set; }


        public CustomJObjectSerializerDelegate<Location>?                      CustomLocationSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<PublishToken>?                  CustomPublishTokenSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<Address>?                       CustomAddressSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?         CustomAdditionalGeoLocationSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<ChargingStation>?               CustomChargingStationSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                          CustomEVSESerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<Parking>?                       CustomParkingSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<ParkingRestriction>?            CustomParkingRestrictionSerializer            { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?                CustomStatusScheduleSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                     CustomConnectorSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<Location>>?         CustomLocationEnergyMeterSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<ChargingStation>>?  CustomChargingStationEnergyMeterSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?             CustomEVSEEnergyMeterSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?    CustomTransparencySoftwareStatusSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?          CustomTransparencySoftwareSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<DisplayText>?                   CustomDisplayTextSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?               CustomBusinessDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                         CustomHoursSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                         CustomImageSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                     CustomEnergyMixSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                  CustomEnergySourceSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?           CustomEnvironmentalImpactSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<LocationMaxPower>?              CustomLocationMaxPowerSerializer              { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                        CustomTariffSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                         CustomPriceSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?                 CustomTariffElementSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?                CustomPriceComponentSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?            CustomTariffRestrictionsSerializer            { get; set; }


        public CustomJObjectSerializerDelegate<Session>?                       CustomSessionSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CDRToken>?                      CustomCDRTokenSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<SessionConnector>?              CustomSessionConnectorSerializer              { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?                CustomChargingPeriodSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                  CustomCDRDimensionSerializer                  { get; set; }


        public CustomJObjectSerializerDelegate<TokenStatus>?                   CustomTokenStatusSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<Token>?                         CustomTokenSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyContract>?                CustomEnergyContractSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<LocationReference>?             CustomLocationReferenceSerializer             { get; set; }


        public CustomJObjectSerializerDelegate<CDR>?                           CustomCDRSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CDRLocation>?                   CustomCDRLocationSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                    CustomSignedDataSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                   CustomSignedValueSerializer                   { get; set; }

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
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional HTTP URL path prefix.</param>
        /// <param name="HTTPServiceName">An optional HTTP service name.</param>
        /// <param name="DNSClient">An optional DNS client.</param>
        /// 
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CommonAPI(//URL                                                        OurBaseURL,
                         //URL                                                        OurVersionsURL,
                         IEnumerable<CredentialsRole>                               OurCredentialRoles,
                         Party_Idv3                                                 DefaultPartyId,

                         CommonBaseAPI                                              BaseAPI,

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
                         LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
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

            : base(HTTPHostname,
                   ExternalDNSName,
                   HTTPServerPort ?? DefaultHTTPServerPort,
                   BasePath,
                   HTTPServerName ?? DefaultHTTPServerName,

                   URLPathPrefix ?? DefaultURLPathPrefix,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   null, //HTMLTemplate,
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
                   LoggingPath,
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   DNSClient,
                   AutoStart)

            //: base(//Version.Id,
            //       URL.Parse(""),//OurBaseURL,
            //       URL.Parse(""),//OurVersionsURL,

            //       AdditionalURLPathPrefix,
            //       LocationsAsOpenData,
            //       AllowDowngrades,
            //       Disable_RootServices,

            //       HTTPHostname,
            //       ExternalDNSName,
            //       HTTPServerPort,
            //       BasePath,
            //       HTTPServerName,

            //       URLPathPrefix,
            //       HTTPServiceName,
            //       APIVersionHashes,

            //       ServerCertificateSelector,
            //       ClientCertificateValidator,
            //       LocalCertificateSelector,
            //       AllowedTLSProtocols,
            //       ClientCertificateRequired,
            //       CheckCertificateRevocation,

            //       ServerThreadNameCreator,
            //       ServerThreadPrioritySetter,
            //       ServerThreadIsBackground,
            //       ConnectionIdBuilder,
            //       ConnectionTimeout,
            //       MaxClientConnections,

            //       DisableMaintenanceTasks,
            //       MaintenanceInitialDelay,
            //       MaintenanceEvery,

            //       DisableWardenTasks,
            //       WardenInitialDelay,
            //       WardenCheckEvery,

            //       IsDevelopment,
            //       DevelopmentServers,
            //       DisableLogging,
            //       LoggingContext,
            //       LoggingPath,
            //       LogfileName,
            //       LogfileCreator is not null
            //           ? (loggingPath, remotePartyId, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
            //           : null,
            //       DatabaseFilePath,
            //       RemotePartyDBFileName,
            //       AssetsDBFileName,
            //       DNSClient,
            //       AutoStart)

        {

            this.BaseAPI             = BaseAPI;

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            //this.OurCredentialRoles  = OurCredentialRoles.Distinct();
            this.DefaultPartyId      = DefaultPartyId;

            this.KeepRemovedEVSEs    = KeepRemovedEVSEs ?? (evse => true);

            this.CommonAPILogger     = this.DisableLogging == false
                                           ? null
                                           : new CommonAPILogger(
                                                 this,
                                                 LoggingContext,
                                                 LoggingPath,
                                                 LogfileCreator
                                             );

            BaseAPI.AddVersionInformation(
                new VersionInformation(
                    Version.Id,
                    URL.Concat(
                        BaseAPI.OurVersionsURL.Protocol.AsString(),
                        ExternalDNSName ?? ("localhost:" + base.HTTPServer.IPPorts.First()),
                        URLPathPrefix + AdditionalURLPathPrefix + $"/versions/{Version.Id}"
                    )
                )
            ).GetAwaiter().GetResult();


            if (!this.DisableLogging)
            {
                ReadRemotePartyDatabaseFile();
                ReadAssetsDatabaseFile();
            }

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
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// 
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        public CommonAPI(URL                           OurBaseURL,
                         URL                           OurVersionsURL,
                         IEnumerable<CredentialsRole>  OurCredentialRoles,
                         Party_Idv3                    DefaultPartyId,

                         CommonBaseAPI                 BaseAPI,
                         HTTPServer?                   HTTPServer                = null,

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

            : base(HTTPServer ?? BaseAPI.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   BasePath,

                   URLPathPrefix,//   ?? DefaultURLPathPrefix,
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
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   AutoStart)

            //: base(//Version.Id,
            //       OurBaseURL,
            //       OurVersionsURL,
            //       HTTPServer,

            //       AdditionalURLPathPrefix,
            //       LocationsAsOpenData,
            //       AllowDowngrades,
            //       Disable_RootServices,

            //       HTTPHostname,
            //       ExternalDNSName,
            //       HTTPServiceName,
            //       BasePath,

            //       URLPathPrefix,
            //       APIVersionHashes,

            //       DisableMaintenanceTasks,
            //       MaintenanceInitialDelay,
            //       MaintenanceEvery,

            //       DisableWardenTasks,
            //       WardenInitialDelay,
            //       WardenCheckEvery,

            //       IsDevelopment,
            //       DevelopmentServers,
            //       DisableLogging,
            //       LoggingContext,
            //       LoggingPath,
            //       LogfileName ?? DefaultLogfileName,
            //       LogfileCreator is not null
            //           ? (loggingPath, remotePartyId, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
            //           : null,
            //       DatabaseFilePath,
            //       RemotePartyDBFileName,
            //       AssetsDBFileName,
            //       AutoStart)

        {

            this.BaseAPI               = BaseAPI;

            if (!OurCredentialRoles.SafeAny())
                throw new ArgumentNullException(nameof(OurCredentialRoles), "The given credential roles must not be null or empty!");

            //this.OurCredentialRoles    = OurCredentialRoles?.Distinct() ?? Array.Empty<CredentialsRole>();
            this.DefaultPartyId        = DefaultPartyId;

            this.KeepRemovedEVSEs      = KeepRemovedEVSEs ?? (evse => true);

            //this.Disable_OCPIv2_1_1    = Disable_OCPIv2_1_1;

            // Link HTTP events...
            base.HTTPServer.RequestLog     += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            base.HTTPServer.ResponseLog    += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            base.HTTPServer.ErrorLog       += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            this.CommonAPILogger       = this.DisableLogging == false
                                             ? new CommonAPILogger(
                                                   this,
                                                   LoggingContext,
                                                   LoggingPath,
                                                   LogfileCreator
                                               )
                                             : null;

            AddVersionInformation(
                new VersionInformation(
                    Version.Id,
                    URL.Concat(
                        OurVersionsURL.Protocol.AsString(),
                        ExternalDNSName ?? ("localhost:" + base.HTTPServer.IPPorts.First()),
                        URLPathPrefix + AdditionalURLPathPrefix + $"/versions/{Version.Id}"
                    )
                )
            ).GetAwaiter().GetResult();


            if (!this.DisableLogging)
            {
                ReadRemotePartyDatabaseFile();
                ReadAssetsDatabaseFile();
            }

            if (!Disable_RootServices)
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

            => BaseAPI.OurBaseURL + Prefix + ModuleId.ToString();

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

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
            this.AddOCPIMethod(

                Hostname,
                HTTPMethod.GET,
                URLPathPrefix + "versions/{versionId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   GetVersionRequest,
                OCPIResponseLogger:  GetVersionResponse,
                OCPIRequestHandler:  request => {

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
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Get version identification URL parameter

                    if (request.ParsedURLParameters.Length < 1)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Version identification is missing!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    if (!Version_Id.TryParse(request.ParsedURLParameters[0], out var versionId))
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Version identification is invalid!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
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
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "This OCPI version is not supported!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.NotFound,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    #endregion


                    var prefix = URLPathPrefix + BaseAPI.AdditionalURLPathPrefix + $"v{versionId}";

                    #region Common credential endpoints...

                    var endpoints = new List<VersionEndpoint>() {

                                        new (Module_Id.Credentials,
                                             InterfaceRoles.SENDER,
                                             URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                                         (request.Host + (prefix + "credentials")).Replace("//", "/"))),

                                        new (Module_Id.Credentials,
                                             InterfaceRoles.RECEIVER,
                                             URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                                         (request.Host + (prefix + "credentials")).Replace("//", "/")))

                                    };

                    #endregion


                    #region The other side is a CPO...

                    if (request.RemoteParty?.Roles.Any(credentialsRole => credentialsRole.Role == Role.CPO) == true)
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
                                    (request.Host + (prefix + "cpo/chargingprofiles")).Replace("//", "/"))
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

                    #region We are a CPO, the other side is unauthenticated and we export locations and AdHoc tariffs as Open Data...

                    if (request.RemoteParty is null &&
                        parties.Values.Any(party => party.Role == Role.CPO))
                    {

                        if (BaseAPI.LocationsAsOpenData)
                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.Locations,
                                    InterfaceRoles.SENDER,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/locations")))
                                )
                            );

                        if (BaseAPI.TariffsAsOpenData)
                            endpoints.Add(
                                new VersionEndpoint(
                                    Module_Id.Tariffs,
                                    InterfaceRoles.SENDER,
                                    URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                        (request.Host + (prefix + "cpo/tariffs")))
                                )
                            );

                    }

                    #endregion

                    #region The other side is an EMSP...

                    if (request.RemoteParty?.Roles.Any(credentialsRole => credentialsRole.Role == Role.EMSP) == true)
                    {

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Locations,
                                InterfaceRoles.SENDER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/locations")).       Replace("//", "/"))
                            )
                        );

                        endpoints.Add(
                            new VersionEndpoint(
                                Module_Id.Tariffs,
                                InterfaceRoles.SENDER,
                                URL.Parse(BaseAPI.OurVersionsURL.Protocol.AsString() +
                                    (request.Host + (prefix + "cpo/tariffs")).         Replace("//", "/"))
                            )
                        );

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
                                InterfaceRoles.SENDER,
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

                        // hubclientinfo

                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                            StatusCode           = 1000,
                            StatusMessage        = "Hello world!",
                            Data                 = new VersionDetail(
                                                       versionId,
                                                       endpoints
                                                   ).ToJSON(CustomVersionDetailSerializer,
                                                            CustomVersionEndpointSerializer),
                            HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                HTTPStatusCode             = HTTPStatusCode.OK,
                                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                AccessControlAllowHeaders  = [ "Authorization" ],
                                Vary                       = "Accept"
                            }
                        }
                    );

                }

            );

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
                                                                      BaseAPI.OurVersionsURL,
                                                                      parties.Values.Select(CredentialsRole.From)
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
                                                              AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST" ],
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
                                                      BaseAPI.OurVersionsURL,
                                                      parties.Values.Select(CredentialsRole.From)
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

        #region LogRemoteParty              (Command,              ...)

        public ValueTask LogRemoteParty(String             Command,
                                        EventTracking_Id   EventTrackingId,
                                        User_Id?           CurrentUserId       = null,
                                        CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   null,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogRemoteParty              (Command, Text = null, ...)

        public ValueTask LogRemoteParty(String             Command,
                                        String?            Text,
                                        EventTracking_Id   EventTrackingId,
                                        User_Id?           CurrentUserId       = null,
                                        CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   Text is not null
                       ? JToken.Parse(Text)
                       : null,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogRemoteParty              (Command, JSON,        ...)

        public ValueTask LogRemoteParty(String             Command,
                                        JObject            JSON,
                                        EventTracking_Id   EventTrackingId,
                                        User_Id?           CurrentUserId       = null,
                                        CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   JSON,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogRemoteParty              (Command, Number,      ...)

        public ValueTask Log(String             Command,
                             Int64              Number,
                             EventTracking_Id   EventTrackingId,
                             User_Id?           CurrentUserId       = null,
                             CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   Number,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region LogRemotePartyComment       (Text,                 ...)

        public ValueTask LogRemotePartyComment(String             Text,
                                               EventTracking_Id   EventTrackingId,
                                               User_Id?           CurrentUserId       = null,
                                               CancellationToken  CancellationToken   = default)

            => BaseAPI.WriteCommentToDatabase(
                   RemotePartyDBFileName,
                   Text,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion


        #region ReadRemotePartyDatabaseFile (DatabaseFileName = null)

        public IEnumerable<Command> ReadRemotePartyDatabaseFile(String? DatabaseFileName = null)

            => BaseAPI.LoadCommandsFromDatabaseFile(DatabaseFileName ?? RemotePartyDBFileName);

        #endregion

        #endregion

        #region Log/Read   Assets

        #region LogAsset              (Command,              ...)

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

        #region LogAsset              (Command, Text = null, ...)

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

        #region LogAsset              (Command, JSONObject,  ...)

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

        #region LogAsset              (Command, JSONArray,   ...)

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

        #region LogAsset              (Command, Number,      ...)

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

        #region LogAssetComment       (Text,                 ...)

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


        #region ReadAssetsDatabaseFile            (DatabaseFileName = null)

        public IEnumerable<Command> ReadAssetsDatabaseFile(String? DatabaseFileName = null)

            => BaseAPI.LoadCommandsFromDatabaseFile(DatabaseFileName ?? AssetsDBFileName);

        #endregion

        #endregion




        //ToDo: Wrap the following into a pluggable interface!

        #region AccessTokens

        public async Task AddAccessToken(String        Token,
                                         AccessStatus  Status)
        {
            if (AccessToken.TryParseBASE64(Token, out var token))
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
                              CommonBaseAPI.removeRemoteParty,
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
                                  CommonBaseAPI.updateRemoteParty,
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

                await LogRemoteParty(
                          CommonBaseAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.addRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.addRemotePartyIfNotExists,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

            await LogRemoteParty(
                      CommonBaseAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

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

            await LogRemoteParty(
                      CommonBaseAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

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

            await LogRemoteParty(
                      CommonBaseAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

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

            await LogRemoteParty(
                      CommonBaseAPI.addOrUpdateRemoteParty,
                      newRemoteParty.ToJSON(true),
                      EventTrackingId ?? EventTracking_Id.New,
                      CurrentUserId
                  );

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

                await LogRemoteParty(
                          CommonBaseAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.updateRemoteParty,
                          newRemoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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
                                                         Party_Idv3     PartyId)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.PartyId == PartyId));

        #endregion

        #region GetRemoteParties   (CountryCode, PartyId, Role)

        /// <summary>
        /// Get all remote parties having the given country code, party identification and role.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A role.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Idv3   PartyId,
                                                         Role         Role)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Roles.Any(credentialsRole => credentialsRole.PartyId == PartyId &&
                                                                                           credentialsRole.Role    == Role));

        #endregion

        #region GetRemoteParties   (Role)

        /// <summary>
        /// Get all remote parties having the given role.
        /// </summary>
        /// <param name="Role">The role of the remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(Role Role)

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

                await LogRemoteParty(
                          CommonBaseAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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

                await LogRemoteParty(
                          CommonBaseAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, Role)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Idv3           PartyId,
                                                     Role              Role,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in GetRemoteParties(CountryCode,
                                                         PartyId,
                                                         Role))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(
                          CommonBaseAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

            }

            return true;

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, Role, AccessToken)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Idv3           PartyId,
                                                     Role              Role,
                                                     AccessToken        AccessToken,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.
                                                      Where(remoteParty => remoteParty.Roles.            Any(credentialsRole  => credentialsRole. PartyId     == PartyId &&
                                                                                                                                 credentialsRole. Role        == Role)   &&

                                                                           remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == AccessToken)))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(
                          CommonBaseAPI.removeRemoteParty,
                          remoteParty.ToJSON(true),
                          EventTrackingId ?? EventTracking_Id.New,
                          CurrentUserId
                      );

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


        #region OCPI Version Informations

        private readonly ConcurrentDictionary<Version_Id, VersionInformation>  versionInformations   = [];

        #region AddVersionInformation            (VersionInformation, ...)

        public async Task<AddResult<VersionInformation>>

            AddVersionInformation(VersionInformation  VersionInformation,
                                  Boolean             SkipNotifications   = false,
                                  EventTracking_Id?   EventTrackingId     = null,
                                  User_Id?            CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (versionInformations.TryAdd(VersionInformation.Id, VersionInformation))
            {

                //await LogAsset(
                //          addLocation,
                //          Location.ToJSON(true,
                //                          true,
                //                          true,
                //                          CustomLocationSerializer,
                //                          CustomPublishTokenSerializer,
                //                          CustomAddressSerializer,
                //                          CustomAdditionalGeoLocationSerializer,
                //                          CustomChargingStationSerializer,
                //                          CustomEVSESerializer,
                //                          CustomStatusScheduleSerializer,
                //                          CustomConnectorSerializer,
                //                          CustomEnergyMeterSerializer,
                //                          CustomTransparencySoftwareStatusSerializer,
                //                          CustomTransparencySoftwareSerializer,
                //                          CustomDisplayTextSerializer,
                //                          CustomBusinessDetailsSerializer,
                //                          CustomHoursSerializer,
                //                          CustomImageSerializer,
                //                          CustomEnergyMixSerializer,
                //                          CustomEnergySourceSerializer,
                //                          CustomEnvironmentalImpactSerializer,
                //                          CustomLocationMaxPowerSerializer),
                //          EventTrackingId,
                //          CurrentUserId
                //      );

                //if (!SkipNotifications)
                //{

                //    var OnLocationAddedLocal = OnLocationAdded;
                //    if (OnLocationAddedLocal is not null)
                //    {
                //        try
                //        {
                //            await OnLocationAddedLocal(Location);
                //        }
                //        catch (Exception e)
                //        {
                //            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
                //                        Environment.NewLine, e.Message,
                //                        Environment.NewLine, e.StackTrace ?? "");
                //        }
                //    }

                //}

                return AddResult<VersionInformation>.Success(
                           EventTrackingId,
                           VersionInformation
                       );

            }

            return AddResult<VersionInformation>.Failed(
                       EventTrackingId,
                       VersionInformation,
                       "The given version information already exists!"
                   );

        }

        #endregion

        #endregion


        #region Parties (local)

        private readonly ConcurrentDictionary<Party_Idv3, PartyData> parties = [];

        public IEnumerable<PartyData> Parties
            => parties.Values;


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
                              new JProperty("id", Id.ToString())
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

        #endregion


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


        #region AddLocation            (Location, ...)

        public async Task<AddResult<Location>>

            AddLocation(Location           Location,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null,
                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Location.PartyId, out var party))
            {

                if (party.Locations.TryAdd(Location.Id, Location))
                {

                    DebugX.Log($"OCPI {Version.String} Location '{Location.Id}': '{Location}' added...");

                    Location.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addLocation,
                              Location.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAddressSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomChargingStationSerializer,
                                  CustomEVSESerializer,
                                  CustomParkingSerializer,
                                  CustomParkingRestrictionSerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomChargingStationEnergyMeterSerializer,
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
                                  CustomLocationMaxPowerSerializer
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

        #region AddLocationIfNotExists (Location, ...)

        public async Task<AddResult<Location>>

            AddLocationIfNotExists(Location           Location,
                                   Boolean            SkipNotifications   = false,
                                   EventTracking_Id?  EventTrackingId     = null,
                                   User_Id?           CurrentUserId       = null,
                                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Location.PartyId, out var party))
            {

                if (party.Locations.TryAdd(Location.Id, Location))
                {

                    DebugX.Log($"OCPI {Version.String} Location '{Location.Id}': '{Location}' added...");

                    Location.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addLocation,
                              Location.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAddressSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomChargingStationSerializer,
                                  CustomEVSESerializer,
                                  CustomParkingSerializer,
                                  CustomParkingRestrictionSerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomChargingStationEnergyMeterSerializer,
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
                                  CustomLocationMaxPowerSerializer
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

                return AddResult<Location>.NoOperation(
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

        #region AddOrUpdateLocation    (Location,                           AllowDowngrades = false, ...)

        public async Task<AddOrUpdateResult<Location>>

            AddOrUpdateLocation(Location           Location,
                                Boolean?           AllowDowngrades     = false,
                                Boolean            SkipNotifications   = false,
                                EventTracking_Id?  EventTrackingId     = null,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Location.PartyId, out var party))
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

                    var aa = existingLocation.Equals(existingLocation);

                    if (party.Locations.TryUpdate(Location.Id,
                                                  Location,
                                                  existingLocation))
                    {

                        Location.CommonAPI = this;

                        await LogAsset(
                                  CommonBaseAPI.addOrUpdateLocation,
                                  Location.ToJSON(
                                      true,
                                      true,
                                      true,
                                      true,
                                      CustomLocationSerializer,
                                      CustomPublishTokenSerializer,
                                      CustomAddressSerializer,
                                      CustomAdditionalGeoLocationSerializer,
                                      CustomChargingStationSerializer,
                                      CustomEVSESerializer,
                                      CustomParkingSerializer,
                                      CustomParkingRestrictionSerializer,
                                      CustomStatusScheduleSerializer,
                                      CustomConnectorSerializer,
                                      CustomLocationEnergyMeterSerializer,
                                      CustomChargingStationEnergyMeterSerializer,
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
                                      CustomLocationMaxPowerSerializer
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

                            //var oldEVSEUIds = new HashSet<EVSE_UId>(existingLocation.EVSEs.Select(evse => evse.UId));
                            //var newEVSEUIds = new HashSet<EVSE_UId>(Location.        EVSEs.Select(evse => evse.UId));

                            //foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                            //{

                            //    if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId) && existingLocation.GetEVSE(evseUId)! != Location.GetEVSE(evseUId)!)
                            //    {
                            //        var OnEVSEChangedLocal = OnEVSEChanged;
                            //        if (OnEVSEChangedLocal is not null)
                            //        {
                            //            try
                            //            {
                            //                await OnEVSEChangedLocal(existingLocation.GetEVSE(evseUId)!);
                            //            }
                            //            catch (Exception e)
                            //            {
                            //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                            //                            Environment.NewLine, e.Message,
                            //                            Environment.NewLine, e.StackTrace ?? "");
                            //            }
                            //        }
                            //    }
                            //    else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                            //    {
                            //        var OnEVSEAddedLocal = OnEVSEAdded;
                            //        if (OnEVSEAddedLocal is not null)
                            //        {
                            //            try
                            //            {
                            //                await OnEVSEAddedLocal(existingLocation.GetEVSE(evseUId)!);
                            //            }
                            //            catch (Exception e)
                            //            {
                            //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                            //                            Environment.NewLine, e.Message,
                            //                            Environment.NewLine, e.StackTrace ?? "");
                            //            }
                            //        }
                            //    }
                            //    else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                            //    {
                            //        var OnEVSERemovedLocal = OnEVSERemoved;
                            //        if (OnEVSERemovedLocal is not null)
                            //        {
                            //            try
                            //            {
                            //                await OnEVSERemovedLocal(existingLocation.GetEVSE(evseUId)!);
                            //            }
                            //            catch (Exception e)
                            //            {
                            //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSERemoved), ": ",
                            //                            Environment.NewLine, e.Message,
                            //                            Environment.NewLine, e.StackTrace ?? "");
                            //            }
                            //        }
                            //    }

                            //}

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
                              CommonBaseAPI.addOrUpdateLocation,
                              Location.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAddressSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomChargingStationSerializer,
                                  CustomEVSESerializer,
                                  CustomParkingSerializer,
                                  CustomParkingRestrictionSerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomChargingStationEnergyMeterSerializer,
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
                                  CustomLocationMaxPowerSerializer
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
                        //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                        //                    Environment.NewLine, e.Message,
                        //                    Environment.NewLine, e.StackTrace ?? "");
                        //    }
                        //}

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
                       "The party identification of the location is unknown!"
                   );

        }

        #endregion

        #region UpdateLocation         (Location,                           AllowDowngrades = false, ...)

        public async Task<UpdateResult<Location>>

            UpdateLocation(Location           Location,
                           Boolean?           AllowDowngrades     = false,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null,
                           CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Location.PartyId, out var party))
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
                              CommonBaseAPI.updateLocation,
                              Location.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomLocationSerializer,
                                  CustomPublishTokenSerializer,
                                  CustomAddressSerializer,
                                  CustomAdditionalGeoLocationSerializer,
                                  CustomChargingStationSerializer,
                                  CustomEVSESerializer,
                                  CustomParkingSerializer,
                                  CustomParkingRestrictionSerializer,
                                  CustomStatusScheduleSerializer,
                                  CustomConnectorSerializer,
                                  CustomLocationEnergyMeterSerializer,
                                  CustomChargingStationEnergyMeterSerializer,
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
                                  CustomLocationMaxPowerSerializer
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
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnLocationChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                        //var oldEVSEUIds = new HashSet<EVSE_UId>(existingLocation.EVSEs.Select(evse => evse.UId));
                        //var newEVSEUIds = new HashSet<EVSE_UId>(Location.        EVSEs.Select(evse => evse.UId));

                        //foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                        //{

                        //    if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                        //    {

                        //        if (existingLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                        //            Location.        TryGetEVSE(evseUId, out var newEVSE) &&
                        //            oldEVSE is not null &&
                        //            newEVSE is not null)
                        //        {

                        //            if (oldEVSE != newEVSE)
                        //            {
                        //                var OnEVSEChangedLocal = OnEVSEChanged;
                        //                if (OnEVSEChangedLocal is not null)
                        //                {
                        //                    try
                        //                    {
                        //                        await OnEVSEChangedLocal(newEVSE);
                        //                    }
                        //                    catch (Exception e)
                        //                    {
                        //                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                        //                                    Environment.NewLine, e.Message,
                        //                                    Environment.NewLine, e.StackTrace ?? "");
                        //                    }
                        //                }
                        //            }

                        //            if (oldEVSE.Status != newEVSE.Status)
                        //            {
                        //                var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        //                if (OnEVSEStatusChangedLocal is not null)
                        //                {
                        //                    try
                        //                    {
                        //                        await OnEVSEStatusChangedLocal(Timestamp.Now,
                        //                                                       newEVSE,
                        //                                                       newEVSE.Status,
                        //                                                       oldEVSE.Status);
                        //                    }
                        //                    catch (Exception e)
                        //                    {
                        //                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                        //                                    Environment.NewLine, e.Message,
                        //                                    Environment.NewLine, e.StackTrace ?? "");
                        //                    }
                        //                }
                        //            }

                        //        }

                        //    }
                        //    else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                        //    {

                        //        var OnEVSEAddedLocal = OnEVSEAdded;
                        //        if (OnEVSEAddedLocal is not null)
                        //        {
                        //            try
                        //            {
                        //                if (Location.TryGetEVSE(evseUId, out var evse) &&
                        //                    evse is not null)
                        //                {
                        //                    await OnEVSEAddedLocal(evse);
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                        //                            Environment.NewLine, e.Message,
                        //                            Environment.NewLine, e.StackTrace ?? "");
                        //            }
                        //        }

                        //        var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        //        if (OnEVSEStatusChangedLocal is not null)
                        //        {
                        //            try
                        //            {
                        //                if (Location.TryGetEVSE(evseUId, out var evse) &&
                        //                    evse is not null)
                        //                {
                        //                    await OnEVSEStatusChangedLocal(Timestamp.Now,
                        //                                                   evse,
                        //                                                   evse.Status);
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                        //                            Environment.NewLine, e.Message,
                        //                            Environment.NewLine, e.StackTrace ?? "");
                        //            }
                        //        }

                        //    }
                        //    else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                        //    {

                        //        var OnEVSERemovedLocal = OnEVSERemoved;
                        //        if (OnEVSERemovedLocal is not null)
                        //        {
                        //            try
                        //            {
                        //                if (existingLocation.TryGetEVSE(evseUId, out var evse) &&
                        //                    evse is not null)
                        //                {
                        //                    await OnEVSERemovedLocal(evse);
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSERemoved), ": ",
                        //                            Environment.NewLine, e.Message,
                        //                            Environment.NewLine, e.StackTrace ?? "");
                        //            }
                        //        }

                        //        var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        //        if (OnEVSEStatusChangedLocal is not null)
                        //        {
                        //            try
                        //            {
                        //                if (existingLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                        //                    Location.        TryGetEVSE(evseUId, out var newEVSE) &&
                        //                    oldEVSE is not null &&
                        //                    newEVSE is not null)
                        //                {
                        //                    await OnEVSEStatusChangedLocal(Timestamp.Now,
                        //                                                   oldEVSE,
                        //                                                   newEVSE.Status,
                        //                                                   oldEVSE.Status);
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                        //                            Environment.NewLine, e.Message,
                        //                            Environment.NewLine, e.StackTrace ?? "");
                        //            }
                        //        }

                        //    }

                        //}

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
                       "The party identification of the location is unknown!"
                   );

        }

        #endregion

        #region TryPatchLocation       (PartyId, LocationId, LocationPatch, AllowDowngrades = false, ...)

        public async Task<PatchResult<Location>> TryPatchLocation(Party_Idv3         PartyId,
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

            return PatchResult<Location>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the location is unknown!"
                   );

        }

        #endregion


        #region RemoveLocation         (PartyId, Location, ...)

        public async Task<RemoveResult<Location>>

            RemoveLocation(Party_Idv3           PartyId,
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

            RemoveLocation(Party_Idv3           PartyId,
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

            RemoveLocations(Func<Party_Idv3, Location_Id, Boolean>  IncludeLocationIds,
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

            RemoveAllLocations(Party_Idv3?          PartyId             = null,
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


        #region LocationExists         (PartyId, LocationId, VersionId = null)

        public Boolean LocationExists(Party_Idv3   PartyId,
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

        #region GetLocation            (PartyId, LocationId, VersionId = null)

        public Location? GetLocation(Party_Idv3   PartyId,
                                     Location_Id  LocationId,
                                     UInt64?      VersionId   = null)
        {

            if (parties.        TryGetValue(PartyId,    out var party) &&
                party.Locations.TryGetValue(LocationId, out var location))
            {

                if (VersionId.HasValue)
                {
                    //ToDo: Check VersionId!
                    return location;
                }

                return location;

            }

            return null;

        }

        #endregion

        #region TryGetLocation         (PartyId, LocationId, out Location, VersionId = null)

        public Boolean TryGetLocation(Party_Idv3                         PartyId,
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

        #region GetLocations           (PartyId = null)

        public IEnumerable<Location> GetLocations(Party_Idv3? PartyId = null)
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

        #endregion

        #region Charging Stations

        public delegate Task OnChargingStationAddedDelegate(ChargingStation ChargingStation);

        public event OnChargingStationAddedDelegate? OnChargingStationAdded;


        public delegate Task OnChargingStationChangedDelegate(ChargingStation ChargingStation);

        public event OnChargingStationChangedDelegate? OnChargingStationChanged;



        public delegate Task OnChargingStationRemovedDelegate(ChargingStation ChargingStation);

        public event OnChargingStationRemovedDelegate? OnChargingStationRemoved;


        #region AddOrUpdateChargingStation       (Location, newOrUpdatedChargingStation, AllowDowngrades = false)

        public async Task<AddOrUpdateResult<ChargingStation>>

            AddOrUpdateChargingStation(Party_Idv3         PartyId,
                                       Location_Id        LocationId,
                                       ChargingStation    newOrUpdatedChargingStation,
                                       Boolean?           AllowDowngrades   = false,
                                       EventTracking_Id?  EventTrackingId   = null,
                                       User_Id?           CurrentUserId     = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Locations.TryGetValue(LocationId, out var existingLocation))
                {

                    if (existingLocation.TryGetChargingStation(newOrUpdatedChargingStation.Id, out var existingChargingStation))
                    {

                        if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                            newOrUpdatedChargingStation.LastUpdated < existingChargingStation.LastUpdated)
                        {
                            return AddOrUpdateResult<ChargingStation>.Failed(
                                       EventTrackingId,
                                       newOrUpdatedChargingStation,
                                       "The 'lastUpdated' timestamp of the new ChargingStation must be newer then the timestamp of the existing ChargingStation!"
                                   );
                        }

                        if (newOrUpdatedChargingStation.LastUpdated.ToISO8601() == existingChargingStation.LastUpdated.ToISO8601())
                            return AddOrUpdateResult<ChargingStation>.NoOperation(
                                       EventTrackingId,
                                       newOrUpdatedChargingStation,
                                       "The 'lastUpdated' timestamp of the new ChargingStation must be newer then the timestamp of the existing ChargingStation!"
                                   );

                    }


                    var builder = existingLocation.ToBuilder();

                    if (!builder.ChargingPool.TryAdd(newOrUpdatedChargingStation.Id, newOrUpdatedChargingStation))
                        builder.ChargingPool[newOrUpdatedChargingStation.Id] = newOrUpdatedChargingStation;

                    builder.VersionId++;
                    builder.LastUpdated = newOrUpdatedChargingStation.LastUpdated;

                    var result = await AddOrUpdateLocation(builder, (AllowDowngrades ?? this.AllowDowngrades) == false);

                    if (result.IsSuccessAndDataNotNull(out var updatedLocation))
                    {

                        var OnLocationChangedLocal = OnLocationChanged;
                        if (OnLocationChangedLocal is not null)
                        {
                            try
                            {
                                OnLocationChangedLocal(newOrUpdatedChargingStation.Parent).Wait();
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

                }

                return AddOrUpdateResult<ChargingStation>.Failed(
                           EventTrackingId,
                           $"The given location '{LocationId}' is unknown!"
                       );

            }

            return AddOrUpdateResult<ChargingStation>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the location is unknown!"
                   );

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

            var PartyId            = ChargingStation.ParentLocation.PartyId;
            var LocationId         = ChargingStation.ParentLocation.Id;
            var ChargingStationId  = ChargingStation.Id;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.Locations.TryGetValue(LocationId, out var existingLocation))
                {

                    if (existingLocation.TryGetChargingStation(ChargingStationId, out var existingChargingStation))
                    {









                    }

                }

            }


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

                if (newOrUpdatedEVSE.LastUpdated.ToISO8601() == existingEVSE.LastUpdated.ToISO8601())
                    return AddOrUpdateResult<EVSE>.NoOperation(
                               EventTrackingId,
                               newOrUpdatedEVSE,
                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!"
                           );

            }


            var builder = ChargingStation.ToBuilder();

            if (!builder.EVSEs.TryAdd(newOrUpdatedEVSE.UId, newOrUpdatedEVSE))
                builder.EVSEs[newOrUpdatedEVSE.UId] = newOrUpdatedEVSE;

            //builder.VersionId++;
            builder.LastUpdated = newOrUpdatedEVSE.LastUpdated;

            await AddOrUpdateChargingStation(
                      ChargingStation.ParentLocation.PartyId,
                      ChargingStation.ParentLocation.Id,
                      builder,
                      (AllowDowngrades ?? this.AllowDowngrades) == false
                  );




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

        #endregion

        #region Connectors

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

                    if (newOrUpdatedConnector.LastUpdated.ToISO8601() == existingConnector.LastUpdated.ToISO8601())
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

            EventTrackingId ??= EventTracking_Id.New;

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

        // EVSE Status?????

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

                    await LogAsset(
                              CommonBaseAPI.addTariff,
                              Tariff.ToJSON(true,
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
                                            CustomEnvironmentalImpactSerializer),
                              EventTrackingId,
                              CurrentUserId
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

                    await LogAsset(
                              CommonBaseAPI.addTariffIfNotExists,
                              Tariff.ToJSON(true,
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
                                            CustomEnvironmentalImpactSerializer),
                              EventTrackingId,
                              CurrentUserId
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

                if (party.Tariffs.TryGetValue(Tariff.Id, out var existingTariff))
                                              //Tariff.NotBefore ?? DateTime.MinValue))
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

                    party.Tariffs.TryUpdate(Tariff.Id, Tariff, existingTariff);
                    Tariff.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addOrUpdateTariff,
                              Tariff.ToJSON(true,
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
                                            CustomEnvironmentalImpactSerializer),
                              EventTrackingId,
                              CurrentUserId
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

                    await LogAsset(
                              CommonBaseAPI.addOrUpdateTariff,
                              Tariff.ToJSON(true,
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
                                            CustomEnvironmentalImpactSerializer),
                              EventTrackingId,
                              CurrentUserId
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

                if (party.Tariffs.TryGetValue(Tariff.Id, out var existingTariff))//, Timestamp.Now))
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

                    await LogAsset(
                              CommonBaseAPI.updateTariff,
                              Tariff.ToJSON(true,
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
                                            CustomEnvironmentalImpactSerializer),
                              EventTrackingId,
                              CurrentUserId
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

                if (party.Tariffs.TryGetValue(Tariff.Id, out var existingTariff))//, Timestamp.Now))
                {

                    var patchResult = existingTariff.TryPatch(TariffPatch,
                                                              AllowDowngrades ?? this.AllowDowngrades ?? false);

                    if (patchResult.IsSuccess &&
                        patchResult.PatchedData is not null)
                    {

                        party.Tariffs.TryUpdate(Tariff.Id, Tariff, patchResult.PatchedData);

                        await LogAsset(
                                  CommonBaseAPI.updateTariff,
                                  Tariff.ToJSON(true,
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
                                                CustomEnvironmentalImpactSerializer),
                                  EventTrackingId ?? EventTracking_Id.New,
                                  CurrentUserId
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

        #region RemoveTariff         (PartyId, Tariff, ...)

        public async Task<RemoveResult<Tariff>>

            RemoveTariff(Party_Idv3         PartyId,
                         Tariff             Tariff,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties .     TryGetValue(PartyId,   out var party) &&
                party.Tariffs.TryRemove  (Tariff.Id, out var removedTariff))
            {

                //if (removedTariffs.Any())
                    await LogAsset(
                              CommonBaseAPI.removeTariff,
                              removedTariff.ToJSON(
                                                true,
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
                                            ),
                              //new JArray(
                              //    removedTariffs.Select(tariff => tariff.ToJSON(
                              //                                        true,
                              //                                        true,
                              //                                        true,
                              //                                        CustomTariffSerializer,
                              //                                        CustomDisplayTextSerializer,
                              //                                        CustomPriceSerializer,
                              //                                        CustomTariffElementSerializer,
                              //                                        CustomPriceComponentSerializer,
                              //                                        CustomTariffRestrictionsSerializer,
                              //                                        CustomEnergyMixSerializer,
                              //                                        CustomEnergySourceSerializer,
                              //                                        CustomEnvironmentalImpactSerializer
                              //                                    )
                              //                        )
                              //),
                              EventTrackingId,
                              CurrentUserId
                          );

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

            RemoveTariff(Party_Idv3         PartyId,
                         Tariff_Id          TariffId,
                         Boolean            SkipNotifications   = false,
                         EventTracking_Id?  EventTrackingId     = null,
                         User_Id?           CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties .     TryGetValue(PartyId,  out var party) &&
                party.Tariffs.TryRemove  (TariffId, out var removedTariff))
            {

                //if (removedTariffs.Any())
                    await LogAsset(
                              CommonBaseAPI.removeTariff,
                              new JArray(
                                  //removedTariffs.Select(tariff => tariff.ToJSON(
                                  //                                    true,
                                  //                                    true,
                                  //                                    true,
                                  //                                    CustomTariffSerializer,
                                  //                                    CustomDisplayTextSerializer,
                                  //                                    CustomPriceSerializer,
                                  //                                    CustomTariffElementSerializer,
                                  //                                    CustomPriceComponentSerializer,
                                  //                                    CustomTariffRestrictionsSerializer,
                                  //                                    CustomEnergyMixSerializer,
                                  //                                    CustomEnergySourceSerializer,
                                  //                                    CustomEnvironmentalImpactSerializer
                                  //                                )
                                  //                    )
                                  removedTariff.ToJSON(
                                      true,
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
                              ),
                              EventTrackingId,
                              CurrentUserId
                          );

                return RemoveResult<Tariff>.Success(
                           EventTrackingId,
                           removedTariff
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
                foreach (var tariff in party.Tariffs.Values)
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
                await LogAsset(
                          CommonBaseAPI.removeTariff,
                          new JArray(
                              removedTariffs.Select(tariff => tariff.ToJSON(
                                                                  true,
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
                          CurrentUserId
                      );

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

            RemoveTariffs(Func<Party_Idv3, Tariff_Id, Boolean>  IncludeTariffIds,
                          Boolean                               SkipNotifications   = false,
                          EventTracking_Id?                     EventTrackingId     = null,
                          User_Id?                              CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTariffs = new List<Tariff>();

            foreach (var party in parties.Values)
            {
                foreach (var tariff in party.Tariffs.Values)
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
                await LogAsset(
                          CommonBaseAPI.removeTariff,
                          new JArray(
                              removedTariffs.Select(tariff => tariff.ToJSON(
                                                                  true,
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
                          CurrentUserId
                      );

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

            RemoveAllTariffs(Party_Idv3?        PartyId             = null,
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
                    removedTariffs.AddRange(party.Tariffs.Values);
                    party.Tariffs.Clear();
                }
            }

            else
            {
                foreach (var party in parties.Values)
                {
                    removedTariffs.AddRange(party.Tariffs.Values);
                    party.Tariffs.Clear();
                }
            }

            if (removedTariffs.Count > 0)
                await LogAsset(
                          CommonBaseAPI.removeTariff,
                          new JArray(
                              removedTariffs.Select(tariff => tariff.ToJSON(
                                                                  true,
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
                          CurrentUserId
                      );

            return RemoveResult<IEnumerable<Tariff>>.Success(
                       EventTrackingId,
                       removedTariffs
                   );

        }

        #endregion


        #region TariffExists         (PartyId, TariffId, VersionId = null)

        public Boolean TariffExists(Party_Idv3   PartyId,
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

        public Boolean TryGetTariff(Party_Idv3                         PartyId,
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
                foreach (var location in party.Tariffs.Values)
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

        public IEnumerable<Tariff> GetTariffs(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {

                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Tariffs.Values;

                return [];

            }

            return parties.Values.SelectMany(party => party.Tariffs.Values);

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

        #endregion

        // Tariff Associations

        #region Tokens

        #region Events

        public delegate Task OnTokenStatusAddedDelegate  (TokenStatus TokenStatus);

        public event OnTokenStatusAddedDelegate?    OnTokenStatusAdded;


        public delegate Task OnTokenStatusChangedDelegate(TokenStatus TokenStatus);

        public event OnTokenStatusChangedDelegate?  OnTokenStatusChanged;

        #endregion


        private readonly Dictionary<CountryCode, Dictionary<Party_Id, Dictionary<Token_Id, TokenStatus>>> tokenStatus = [];


        public delegate Task<TokenStatus> OnVerifyTokenDelegate(Party_Idv3  PartyId,
                                                                Token_Id    TokenId);

        public event OnVerifyTokenDelegate? OnVerifyToken;


        #region AddToken            (Token, Status = AllowedType.ALLOWED, ...)

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
            var tokenStatus   = new TokenStatus(Token, Status.Value);

            if (parties.TryGetValue(Token.PartyId, out var party))
            {

                if (party.Tokens.TryAdd(Token.Id, tokenStatus))
                {

                    DebugX.Log($"OCPI {Version.String} Token '{Token.Id}' with status {Status}: '{Token}' added...");

                    Token.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addTokenStatus,
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

                        var OnTokenStatusAddedLocal = OnTokenStatusAdded;
                        if (OnTokenStatusAddedLocal is not null)
                        {
                            try
                            {
                                await OnTokenStatusAddedLocal(tokenStatus);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddToken), " ", nameof(OnTokenStatusAddedLocal), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the token status is unknown!"
                   );

        }

        #endregion

        #region AddTokenIfNotExists (Token, Status = AllowedType.ALLOWED, ...)

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
            var tokenStatus   = new TokenStatus(Token, Status.Value);

            if (parties.TryGetValue(Token.PartyId, out var party))
            {

                if (party.Tokens.TryAdd(Token.Id, tokenStatus))
                {

                    DebugX.Log($"OCPI {Version.String} Token '{Token.Id}' with status {Status}: '{Token}' added...");

                    Token.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addTokenStatusIfNotExists,
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

                        var OnTokenStatusAddedLocal = OnTokenStatusAdded;
                        if (OnTokenStatusAddedLocal is not null)
                        {
                            try
                            {
                                await OnTokenStatusAddedLocal(tokenStatus);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddToken), " ", nameof(OnTokenStatusAddedLocal), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the token status is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateToken    (Token, Status = AllowedType.ALLOWED, AllowDowngrades = false, ...)

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

            if (parties.TryGetValue(Token.PartyId, out var party))
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
                                  CommonBaseAPI.addOrUpdateTokenStatus,
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

                            var OnTokenStatusChangedLocal = OnTokenStatusChanged;
                            if (OnTokenStatusChangedLocal is not null)
                            {
                                try
                                {
                                    OnTokenStatusChangedLocal(tokenStatus).Wait(CancellationToken);
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateToken), " ", nameof(OnTokenStatusChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

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
                              CommonBaseAPI.addOrUpdateTokenStatus,
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

                        var OnTokenStatusAddedLocal = OnTokenStatusAdded;
                        if (OnTokenStatusAddedLocal is not null)
                        {
                            try
                            {
                                OnTokenStatusAddedLocal(tokenStatus).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateToken), " ", nameof(OnTokenStatusAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the token status is unknown!"
                   );

        }

        #endregion

        #region UpdateToken         (Token, Status = AllowedType.ALLOWED, AllowDowngrades = false, ...)

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

            if (parties.TryGetValue(Token.PartyId, out var party))
            {

                if (!party.Tokens.TryGetValue(Token.Id, out var existingTokenStatus))
                    return UpdateResult<TokenStatus>.Failed(
                               EventTrackingId,
                               $"The given token identification '{Token.Id}' is unknown!"
                           );

                Status ??= existingTokenStatus.Status;
                var tokenStatus = new TokenStatus(Token, Status.Value);

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
                              CommonBaseAPI.updateTokenStatus,
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

                        var OnTokenStatusChangedLocal = OnTokenStatusChanged;
                        if (OnTokenStatusChangedLocal is not null)
                        {
                            try
                            {
                                OnTokenStatusChangedLocal(tokenStatus).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateToken), " ", nameof(OnTokenStatusChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the token is unknown!"
                   );

        }

        #endregion

        #region TryPatchToken       (PartyId, TokenId, TokenPatch,        AllowDowngrades = false, ...)

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

                    if (patchResult.IsSuccess &&
                        patchResult.PatchedData is not null)
                    {

                        var updateTokenResult = await UpdateToken(
                                                          patchResult.PatchedData,
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
                                       "Could not update the token: " + updateTokenResult.ErrorResponse
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

        public async Task<RemoveResult<Token>>

            RemoveToken(Token              Token,
                        Boolean            SkipNotifications   = false,
                        EventTracking_Id?  EventTrackingId     = null,
                        User_Id?           CurrentUserId       = null,
                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Token.PartyId, out var party))
            {

                if (party.Tokens.TryRemove(Token.Id, out var tokenStatus))
                {

                    await LogAsset(
                              CommonBaseAPI.removeToken,
                              tokenStatus.Token.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomTokenSerializer,
                                  CustomEnergyContractSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    return RemoveResult<Token>.Success(
                               EventTrackingId,
                               tokenStatus.Token
                           );

                }

                return RemoveResult<Token>.Failed(
                           EventTrackingId,
                           Token,
                           "The token identification of the token is unknown!"
                       );

            }

            return RemoveResult<Token>.Failed(
                       EventTrackingId,
                       Token,
                       "The party identification of the token is unknown!"
                   );

        }

        #endregion

        #region RemoveToken         (PartyId, TokenId, ...)

        public async Task<RemoveResult<Token>>

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

                if (party.Tokens.TryRemove(TokenId, out var tokenStatus))
                {

                    await LogAsset(
                              CommonBaseAPI.removeToken,
                              tokenStatus.Token.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomTokenSerializer,
                                  CustomEnergyContractSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    return RemoveResult<Token>.Success(
                               EventTrackingId,
                               tokenStatus.Token
                           );

                }

                return RemoveResult<Token>.Failed(
                           EventTrackingId,
                           "The token identification of the token is unknown!"
                       );

            }

            return RemoveResult<Token>.Failed(
                       EventTrackingId,
                       "The party identification of the token is unknown!"
                   );

        }

        #endregion

        #region RemoveAllTokens     (IncludeTokens = null, ...)

        /// <summary>
        /// Remove all matching tokens.
        /// </summary>
        /// <param name="IncludeTokens">An optional charging token filter.</param>
        public async Task<RemoveResult<IEnumerable<Token>>>

            RemoveAllTokens(Func<Token, Boolean>?  IncludeTokens       = null,
                            EventTracking_Id?      EventTrackingId     = null,
                            User_Id?               CurrentUserId       = null,
                            CancellationToken      CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTokens = new List<Token>();

            if (IncludeTokens is null)
            {
                foreach (var party in parties.Values)
                {
                    removedTokens.AddRange(party.Tokens.Values.Select(tokenStatus => tokenStatus.Token));
                    party.Tokens.Clear();
                }
            }

            else
            {

                foreach (var party in parties.Values)
                {
                    foreach (var token in party.Tokens.Values.Select(tokenStatus => tokenStatus.Token))
                    {
                        if (IncludeTokens(token))
                            removedTokens.Add(token);
                    }
                }

                foreach (var token in removedTokens)
                    parties[token.PartyId].Tokens.TryRemove(token.Id, out _);

            }

            await LogAsset(
                      CommonBaseAPI.removeAllSessions,
                      new JArray(
                          removedTokens.Select(
                              token => token.ToJSON(
                                           true,
                                           true,
                                           true,
                                           true,
                                           CustomTokenSerializer,
                                           CustomEnergyContractSerializer
                                       )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<Token>>.Success(
                       EventTrackingId,
                       removedTokens
                   );

        }

        #endregion

        #region RemoveAllTokens     (IncludeTokenIds, ...)

        /// <summary>
        /// Remove all matching tokens.
        /// </summary>
        /// <param name="IncludeTokenIds">The token identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>>

            RemoveAllTokens(Func<Token_Id, Boolean>  IncludeTokenIds,
                            Boolean                  SkipNotifications   = false,
                            EventTracking_Id?        EventTrackingId     = null,
                            User_Id?                 CurrentUserId       = null,
                            CancellationToken        CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedTokens = new List<Token>();

            foreach (var party in parties.Values)
            {
                foreach (var token in party.Tokens.Values.Select(tokenStatus => tokenStatus.Token))
                {
                    if (IncludeTokenIds(token.Id))
                        removedTokens.Add(token);
                }
            }

            foreach (var token in removedTokens)
                parties[token.PartyId].Tokens.TryRemove(token.Id, out _);


            await LogAsset(
                      CommonBaseAPI.removeAllTokens,
                      new JArray(
                          removedTokens.Select(
                              token => token.ToJSON(
                                           true,
                                           true,
                                           true,
                                           true,
                                           CustomTokenSerializer,
                                           CustomEnergyContractSerializer
                                       )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<Token>>.Success(
                       EventTrackingId,
                       removedTokens
                   );

        }

        #endregion

        #region RemoveAllTokens     (PartyId, ...)

        /// <summary>
        /// Remove all tokens owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        public async Task<RemoveResult<IEnumerable<Token>>>

            RemoveAllTokens(Party_Idv3         PartyId,
                            EventTracking_Id?  EventTrackingId     = null,
                            User_Id?           CurrentUserId       = null,
                            CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                var removedTokens = party.Tokens.Values.Select(tokenStatus => tokenStatus.Token).ToArray();
                party.Tokens.Clear();

                await LogAsset(
                      CommonBaseAPI.removeAllTokens,
                      new JArray(
                          removedTokens.Select(
                              token => token.ToJSON(
                                           true,
                                           true,
                                           true,
                                           true,
                                           CustomTokenSerializer,
                                           CustomEnergyContractSerializer
                                       )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

                return RemoveResult<IEnumerable<Token>>.Success(
                           EventTrackingId,
                           removedTokens
                       );

            }

            return RemoveResult<IEnumerable<Token>>.Failed(
                       EventTrackingId,
                       "The party identification of the token is unknown!"
                   );

        }

        #endregion


        #region TokenExists         (PartyId, TokenId)

        public Boolean TokenExists(Party_Idv3  PartyId,
                                   Token_Id    TokenId)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.Tokens.ContainsKey(TokenId);

            return false;

        }

        #endregion

        #region TryGetToken         (PartyId, TokenId, out Token)

        public Boolean TryGetToken(Party_Idv3                      PartyId,
                                   Token_Id                        TokenId,
                                   [NotNullWhen(true)] out Token?  Token)
        {

            if (parties.     TryGetValue(PartyId, out var party) &&
                party.Tokens.TryGetValue(TokenId, out var tokenStatus))
            {
                Token = tokenStatus.Token;
                return true;
            }

            Token = null;
            return false;

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

            var VerifyTokenLocal = OnVerifyToken;
            if (VerifyTokenLocal is not null)
            {

                try
                {

                    var tokenStatus = VerifyTokenLocal(
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
                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryGetToken), " ", nameof(VerifyTokenLocal), ": ",
                                Environment.NewLine, e.Message,
                                Environment.NewLine, e.StackTrace ?? "");
                }

            }

            TokenStatus = null;
            return false;

        }

        #endregion

        #region GetTokens           (IncludeToken)

        public IEnumerable<Token> GetTokens(Func<Token, Boolean> IncludeToken)
        {

            var tokens = new List<Token>();

            foreach (var party in parties.Values)
            {
                foreach (var tokenStatus in party.Tokens.Values)
                {
                    if (IncludeToken(tokenStatus.Token))
                        tokens.Add(tokenStatus.Token);
                }
            }

            return tokens;

        }

        #endregion

        #region GetTokenStatus      (IncludeTokenStatus)

        public IEnumerable<TokenStatus> GetTokenStatus(Func<TokenStatus, Boolean> IncludeTokenStatus)
        {

            var tokens = new List<TokenStatus>();

            foreach (var party in parties.Values)
            {
                foreach (var tokenStatus in party.Tokens.Values)
                {
                    if (IncludeTokenStatus(tokenStatus))
                        tokens.Add(tokenStatus);
                }
            }

            return tokens;

        }

        #endregion

        #region GetTokens           (PartyId = null)

        public IEnumerable<Token> GetTokens(Party_Idv3? PartyId = null)
        {

            if (PartyId.HasValue)
            {
                if (parties.TryGetValue(PartyId.Value, out var party))
                    return party.Tokens.Values.Select(tokenStatus => tokenStatus.Token);
            }

            else
            {

                var tokens = new List<Token>();

                foreach (var party in parties.Values)
                    tokens.AddRange(party.Tokens.Values.Select(tokenStatus => tokenStatus.Token));

                return tokens;

            }

            return [];

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

        public delegate Task OnSessionAddedDelegate  (Session Session);

        public event OnSessionAddedDelegate?    OnSessionAdded;


        public delegate Task OnSessionChangedDelegate(Session Session);

        public event OnSessionChangedDelegate?  OnSessionChanged;

        #endregion


        public delegate Task<Session> OnSessionSlowStorageLookupDelegate(Party_Idv3   PartyId,
                                                                         Session_Id   SessionId);

        public event OnSessionSlowStorageLookupDelegate? OnSessionSlowStorageLookup;


        #region AddSession            (Session, ...)

        public async Task<AddResult<Session>>

            AddSession(Session            Session,
                       Boolean            SkipNotifications   = false,
                       EventTracking_Id?  EventTrackingId     = null,
                       User_Id?           CurrentUserId       = null,
                       CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Session.PartyId, out var party))
            {

                if (party.Sessions.TryAdd(Session.Id, Session))
                {

                    DebugX.Log($"OCPI {Version.String} Session '{Session.Id}': '{Session}' added...");

                    Session.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addSession,
                              Session.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomSessionConnectorSerializer,
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

                        var OnSessionAddedLocal = OnSessionAdded;
                        if (OnSessionAddedLocal is not null)
                        {
                            try
                            {
                                await OnSessionAddedLocal(Session);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddSession), " ", nameof(OnSessionAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the session is unknown!"
                   );

        }

        #endregion

        #region AddSessionIfNotExists (Session, ...)

        public async Task<AddResult<Session>>

            AddSessionIfNotExists(Session            Session,
                                  Boolean            SkipNotifications   = false,
                                  EventTracking_Id?  EventTrackingId     = null,
                                  User_Id?           CurrentUserId       = null,
                                  CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Session.PartyId, out var party))
            {

                if (party.Sessions.TryAdd(Session.Id, Session))
                {

                    DebugX.Log($"OCPI {Version.String} Session '{Session.Id}': '{Session}' added...");

                    Session.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addSession,
                              Session.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomSessionConnectorSerializer,
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

                        var OnSessionAddedLocal = OnSessionAdded;
                        if (OnSessionAddedLocal is not null)
                        {
                            try
                            {
                                await OnSessionAddedLocal(Session);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddSession), " ", nameof(OnSessionAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the session is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateSession    (Session,                          AllowDowngrades = false, ...)

        public async Task<AddOrUpdateResult<Session>>

            AddOrUpdateSession(Session            Session,
                               Boolean?           AllowDowngrades     = false,
                               Boolean            SkipNotifications   = false,
                               EventTracking_Id?  EventTrackingId     = null,
                               User_Id?           CurrentUserId       = null,
                               CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Session.PartyId, out var party))
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
                                  CommonBaseAPI.addOrUpdateSession,
                                  Session.ToJSON(
                                      true,
                                      true,
                                      true,
                                      true,
                                      CustomSessionSerializer,
                                      CustomCDRTokenSerializer,
                                      CustomSessionConnectorSerializer,
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

                            var OnSessionChangedLocal = OnSessionChanged;
                            if (OnSessionChangedLocal is not null)
                            {
                                try
                                {
                                    OnSessionChangedLocal(Session).Wait(CancellationToken);
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateSession), " ", nameof(OnSessionChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

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
                              CommonBaseAPI.addOrUpdateSession,
                              Session.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomSessionConnectorSerializer,
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

                        var OnSessionAddedLocal = OnSessionAdded;
                        if (OnSessionAddedLocal is not null)
                        {
                            try
                            {
                                OnSessionAddedLocal(Session).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateSession), " ", nameof(OnSessionAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the session is unknown!"
                   );

        }

        #endregion

        #region UpdateSession         (Session,                          AllowDowngrades = false, ...)

        public async Task<UpdateResult<Session>>

            UpdateSession(Session            Session,
                          Boolean?           AllowDowngrades     = false,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Session.PartyId, out var party))
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
                              CommonBaseAPI.updateSession,
                              Session.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomSessionConnectorSerializer,
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

                        var OnSessionChangedLocal = OnSessionChanged;
                        if (OnSessionChangedLocal is not null)
                        {
                            try
                            {
                                OnSessionChangedLocal(Session).Wait(CancellationToken);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateSession), " ", nameof(OnSessionChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                    return UpdateResult<Session>.Success(
                               EventTrackingId,
                               Session
                           );

                }

                return UpdateResult<Session>.Failed(
                           EventTrackingId,
                           Session,
                           "sessions.TryUpdate(Session.Id, Session, Session) failed!"
                       );

            }

            return UpdateResult<Session>.Failed(
                       EventTrackingId,
                       Session,
                       "The party identification of the session is unknown!"
                   );

        }

        #endregion

        #region TryPatchSession       (PartyId, SessionId, SessionPatch, AllowDowngrades = false, ...)

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

                    if (patchResult.IsSuccess &&
                        patchResult.PatchedData is not null)
                    {

                        var updateSessionResult = await UpdateSession(
                                                            patchResult.PatchedData,
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
                                       "Could not update the session: " + updateSessionResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<Session>.Failed(
                           EventTrackingId,
                           $"The given session '{SessionId}' is unknown!"
                       );

            }

            return PatchResult<Session>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the session is unknown!"
                   );

        }

        #endregion

        #region RemoveSession         (Session, ...)

        public async Task<RemoveResult<Session>>

            RemoveSession(Session            Session,
                          Boolean            SkipNotifications   = false,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(Session.PartyId, out var party))
            {

                if (party.Sessions.TryRemove(Session.Id, out var session))
                {

                    await LogAsset(
                              CommonBaseAPI.removeSession,
                              session.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomSessionConnectorSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomPriceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    return RemoveResult<Session>.Success(
                               EventTrackingId,
                               session
                           );

                }

                return RemoveResult<Session>.Failed(
                           EventTrackingId,
                           Session,
                           "The session identification of the session is unknown!"
                       );

            }

            return RemoveResult<Session>.Failed(
                       EventTrackingId,
                       Session,
                       "The party identification of the session is unknown!"
                   );

        }

        #endregion

        #region RemoveSession         (PartyId, SessionId, ...)

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
                              CommonBaseAPI.removeSession,
                              session.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomSessionSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomSessionConnectorSerializer,
                                  CustomChargingPeriodSerializer,
                                  CustomCDRDimensionSerializer,
                                  CustomPriceSerializer
                              ),
                              EventTrackingId,
                              CurrentUserId,
                              CancellationToken
                          );

                    return RemoveResult<Session>.Success(
                               EventTrackingId,
                               session
                           );

                }

                return RemoveResult<Session>.Failed(
                           EventTrackingId,
                           "The session identification of the session is unknown!"
                       );

            }

            return RemoveResult<Session>.Failed(
                       EventTrackingId,
                       "The party identification of the session is unknown!"
                   );

        }

        #endregion

        #region RemoveAllSessions     (IncludeSessions = null, ...)

        /// <summary>
        /// Remove all matching sessions.
        /// </summary>
        /// <param name="IncludeSessions">An optional charging session filter.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllSessions(Func<Session, Boolean>?  IncludeSessions     = null,
                              Boolean                  SkipNotifications   = false,
                              EventTracking_Id?        EventTrackingId     = null,
                              User_Id?                 CurrentUserId       = null,
                              CancellationToken        CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedSessions = new List<Session>();

            if (IncludeSessions is null)
            {
                foreach (var party in parties.Values)
                {
                    removedSessions.AddRange(party.Sessions.Values);
                    party.Sessions.Clear();
                }
            }

            else
            {

                foreach (var party in parties.Values)
                {
                    foreach (var session in party.Sessions.Values)
                    {
                        if (IncludeSessions(session))
                            removedSessions.Add(session);
                    }
                }

                foreach (var session in removedSessions)
                    parties[session.PartyId].Sessions.TryRemove(session.Id, out _);

            }

            await LogAsset(
                      CommonBaseAPI.removeAllSessions,
                      new JArray(
                          removedSessions.Select(
                              session => session.ToJSON(
                                             true,
                                             true,
                                             true,
                                             true,
                                             CustomSessionSerializer,
                                             CustomCDRTokenSerializer,
                                             CustomSessionConnectorSerializer,
                                             CustomChargingPeriodSerializer,
                                             CustomCDRDimensionSerializer,
                                             CustomPriceSerializer
                                         )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<Session>>.Success(
                       EventTrackingId,
                       removedSessions
                   );

        }

        #endregion

        #region RemoveAllSessions     (IncludeSessionIds, ...)

        /// <summary>
        /// Remove all matching sessions.
        /// </summary>
        /// <param name="IncludeSessionIds">The session identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>>

            RemoveAllSessions(Func<Session_Id, Boolean>  IncludeSessionIds,
                              Boolean                    SkipNotifications   = false,
                              EventTracking_Id?          EventTrackingId     = null,
                              User_Id?                   CurrentUserId       = null,
                              CancellationToken          CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedSessions = new List<Session>();

            foreach (var party in parties.Values)
            {
                foreach (var session in party.Sessions.Values)
                {
                    if (IncludeSessionIds(session.Id))
                        removedSessions.Add(session);
                }
            }

            foreach (var session in removedSessions)
                parties[session.PartyId].Sessions.TryRemove(session.Id, out _);


            await LogAsset(
                      CommonBaseAPI.removeAllSessions,
                      new JArray(
                          removedSessions.Select(
                              session => session.ToJSON(
                                             true,
                                             true,
                                             true,
                                             true,
                                             CustomSessionSerializer,
                                             CustomCDRTokenSerializer,
                                             CustomSessionConnectorSerializer,
                                             CustomChargingPeriodSerializer,
                                             CustomCDRDimensionSerializer,
                                             CustomPriceSerializer
                                         )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<Session>>.Success(
                       EventTrackingId,
                       removedSessions
                   );

        }

        #endregion

        #region RemoveAllSessions     (PartyId, ...)

        /// <summary>
        /// Remove all sessions owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
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

                var removedSessions = party.Sessions.Values.ToArray();
                party.Sessions.Clear();

                await LogAsset(
                      CommonBaseAPI.removeAllSessions,
                      new JArray(
                          removedSessions.Select(
                              session => session.ToJSON(
                                             true,
                                             true,
                                             true,
                                             true,
                                             CustomSessionSerializer,
                                             CustomCDRTokenSerializer,
                                             CustomSessionConnectorSerializer,
                                             CustomChargingPeriodSerializer,
                                             CustomCDRDimensionSerializer,
                                             CustomPriceSerializer
                                         )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

                return RemoveResult<IEnumerable<Session>>.Success(
                           EventTrackingId,
                           removedSessions
                       );

            }

            return RemoveResult<IEnumerable<Session>>.Failed(
                       EventTrackingId,
                       "The party identification of the session is unknown!"
                   );

        }

        #endregion


        #region SessionExists         (PartyId, SessionId)

        public Boolean SessionExists(Party_Idv3  PartyId,
                                     Session_Id  SessionId)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.Sessions.ContainsKey(SessionId);

            return false;

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

            var OnSessionSlowStorageLookupLocal = OnSessionSlowStorageLookup;
            if (OnSessionSlowStorageLookupLocal is not null)
            {
                try
                {

                    var session = OnSessionSlowStorageLookupLocal(
                                      PartyId,
                                      SessionId
                                  ).Result;

                    if (session is not null)
                    {
                        Session = session;
                        return true;
                    }

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

        public delegate Task OnCDRAddedDelegate(CDR CDR);

        public event OnCDRAddedDelegate? OnCDRAdded;

        public delegate Task OnCDRChangedDelegate(CDR CDR);

        public event OnCDRChangedDelegate? OnCDRChanged;

        #endregion


        public delegate Task<CDR> OnChargeDetailRecordSlowStorageLookupDelegate(Party_Idv3  PartyId,
                                                                                CDR_Id      CDRId);

        public event OnChargeDetailRecordSlowStorageLookupDelegate? OnChargeDetailRecordSlowStorageLookup;


        #region AddCDR            (CDR, ...)

        public async Task<AddResult<CDR>>

            AddCDR(CDR                CDR,
                   Boolean            SkipNotifications   = false,
                   EventTracking_Id?  EventTrackingId     = null,
                   User_Id?           CurrentUserId       = null,
                   CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(CDR.PartyId, out var party))
            {

                if (party.CDRs.TryAdd(CDR.Id, CDR))
                {

                    DebugX.Log($"OCPI {Version.String} CDR '{CDR.Id}': '{CDR}' added...");

                    CDR.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addChargeDetailRecord,
                              CDR.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
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

                        var OnCDRAddedLocal = OnCDRAdded;
                        if (OnCDRAddedLocal is not null)
                        {
                            try
                            {
                                await OnCDRAddedLocal(CDR);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddCDR), " ", nameof(OnCDRAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddCDRIfNotExists (CDR, ...)

        public async Task<AddResult<CDR>>

            AddCDRIfNotExists(CDR                CDR,
                              Boolean            SkipNotifications   = false,
                              EventTracking_Id?  EventTrackingId     = null,
                              User_Id?           CurrentUserId       = null,
                              CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(CDR.PartyId, out var party))
            {

                if (party.CDRs.TryAdd(CDR.Id, CDR))
                {

                    DebugX.Log($"OCPI {Version.String} CDR '{CDR.Id}': '{CDR}' added...");

                    CDR.CommonAPI = this;

                    await LogAsset(
                              CommonBaseAPI.addChargeDetailRecord,
                              CDR.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
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

                        var OnCDRAddedLocal = OnCDRAdded;
                        if (OnCDRAddedLocal is not null)
                        {
                            try
                            {
                                await OnCDRAddedLocal(CDR);
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddCDR), " ", nameof(OnCDRAdded), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

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
                       "The party identification of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region AddOrUpdateCDR    (CDR,                      AllowDowngrades = false, ...)

        public async Task<AddOrUpdateResult<CDR>>

            AddOrUpdateCDR(CDR                CDR,
                           Boolean?           AllowDowngrades     = false,
                           Boolean            SkipNotifications   = false,
                           EventTracking_Id?  EventTrackingId     = null,
                           User_Id?           CurrentUserId       = null,
                           CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(CDR.PartyId, out var party))
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
                                  CommonBaseAPI.addOrUpdateChargeDetailRecord,
                                  CDR.ToJSON(
                                      true,
                                      true,
                                      true,
                                      true,
                                      CustomCDRSerializer,
                                      CustomCDRTokenSerializer,
                                      CustomCDRLocationSerializer,
                                      CustomEVSEEnergyMeterSerializer,
                                      CustomTransparencySoftwareStatusSerializer,
                                      CustomTransparencySoftwareSerializer,
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

                            //var oldEVSEUIds = new HashSet<EVSE_UId>(existingCDR.EVSEs.Select(evse => evse.UId));
                            //var newEVSEUIds = new HashSet<EVSE_UId>(CDR.        EVSEs.Select(evse => evse.UId));

                            //foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                            //{

                            //    if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId) && existingCDR.GetEVSE(evseUId)! != CDR.GetEVSE(evseUId)!)
                            //    {
                            //        var OnEVSEChangedLocal = OnEVSEChanged;
                            //        if (OnEVSEChangedLocal is not null)
                            //        {
                            //            try
                            //            {
                            //                await OnEVSEChangedLocal(existingCDR.GetEVSE(evseUId)!);
                            //            }
                            //            catch (Exception e)
                            //            {
                            //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnEVSEChanged), ": ",
                            //                            Environment.NewLine, e.Message,
                            //                            Environment.NewLine, e.StackTrace ?? "");
                            //            }
                            //        }
                            //    }
                            //    else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                            //    {
                            //        var OnEVSEAddedLocal = OnEVSEAdded;
                            //        if (OnEVSEAddedLocal is not null)
                            //        {
                            //            try
                            //            {
                            //                await OnEVSEAddedLocal(existingCDR.GetEVSE(evseUId)!);
                            //            }
                            //            catch (Exception e)
                            //            {
                            //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnEVSEAdded), ": ",
                            //                            Environment.NewLine, e.Message,
                            //                            Environment.NewLine, e.StackTrace ?? "");
                            //            }
                            //        }
                            //    }
                            //    else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                            //    {
                            //        var OnEVSERemovedLocal = OnEVSERemoved;
                            //        if (OnEVSERemovedLocal is not null)
                            //        {
                            //            try
                            //            {
                            //                await OnEVSERemovedLocal(existingCDR.GetEVSE(evseUId)!);
                            //            }
                            //            catch (Exception e)
                            //            {
                            //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnEVSERemoved), ": ",
                            //                            Environment.NewLine, e.Message,
                            //                            Environment.NewLine, e.StackTrace ?? "");
                            //            }
                            //        }
                            //    }

                            //}

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
                              CommonBaseAPI.addOrUpdateChargeDetailRecord,
                              CDR.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
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

                        //var OnEVSEAddedLocal = OnEVSEAdded;
                        //if (OnEVSEAddedLocal is not null)
                        //{
                        //    try
                        //    {
                        //        foreach (var evse in CDR.EVSEs)
                        //            await OnEVSEAddedLocal(evse);
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnEVSEAdded), ": ",
                        //                    Environment.NewLine, e.Message,
                        //                    Environment.NewLine, e.StackTrace ?? "");
                        //    }
                        //}

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
                       "The party identification of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region UpdateCDR         (CDR,                      AllowDowngrades = false, ...)

        public async Task<UpdateResult<CDR>>

            UpdateCDR(CDR                CDR,
                      Boolean?           AllowDowngrades     = false,
                      Boolean            SkipNotifications   = false,
                      EventTracking_Id?  EventTrackingId     = null,
                      User_Id?           CurrentUserId       = null,
                      CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(CDR.PartyId, out var party))
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
                              CommonBaseAPI.updateChargeDetailRecord,
                              CDR.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
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

                        //var oldEVSEUIds = new HashSet<EVSE_UId>(existingCDR.EVSEs.Select(evse => evse.UId));
                        //var newEVSEUIds = new HashSet<EVSE_UId>(CDR.        EVSEs.Select(evse => evse.UId));

                        //foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                        //{

                        //    if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                        //    {

                        //        if (existingCDR.TryGetEVSE(evseUId, out var oldEVSE) &&
                        //            CDR.        TryGetEVSE(evseUId, out var newEVSE) &&
                        //            oldEVSE is not null &&
                        //            newEVSE is not null)
                        //        {

                        //            if (oldEVSE != newEVSE)
                        //            {
                        //                var OnEVSEChangedLocal = OnEVSEChanged;
                        //                if (OnEVSEChangedLocal is not null)
                        //                {
                        //                    try
                        //                    {
                        //                        await OnEVSEChangedLocal(newEVSE);
                        //                    }
                        //                    catch (Exception e)
                        //                    {
                        //                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnEVSEChanged), ": ",
                        //                                    Environment.NewLine, e.Message,
                        //                                    Environment.NewLine, e.StackTrace ?? "");
                        //                    }
                        //                }
                        //            }

                        //            if (oldEVSE.Status != newEVSE.Status)
                        //            {
                        //                var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        //                if (OnEVSEStatusChangedLocal is not null)
                        //                {
                        //                    try
                        //                    {
                        //                        await OnEVSEStatusChangedLocal(Timestamp.Now,
                        //                                                       newEVSE,
                        //                                                       newEVSE.Status,
                        //                                                       oldEVSE.Status);
                        //                    }
                        //                    catch (Exception e)
                        //                    {
                        //                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnEVSEChanged), ": ",
                        //                                    Environment.NewLine, e.Message,
                        //                                    Environment.NewLine, e.StackTrace ?? "");
                        //                    }
                        //                }
                        //            }

                        //        }

                        //    }
                        //    else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                        //    {

                        //        var OnEVSEAddedLocal = OnEVSEAdded;
                        //        if (OnEVSEAddedLocal is not null)
                        //        {
                        //            try
                        //            {
                        //                if (CDR.TryGetEVSE(evseUId, out var evse) &&
                        //                    evse is not null)
                        //                {
                        //                    await OnEVSEAddedLocal(evse);
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnEVSEAdded), ": ",
                        //                            Environment.NewLine, e.Message,
                        //                            Environment.NewLine, e.StackTrace ?? "");
                        //            }
                        //        }

                        //        var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        //        if (OnEVSEStatusChangedLocal is not null)
                        //        {
                        //            try
                        //            {
                        //                if (CDR.TryGetEVSE(evseUId, out var evse) &&
                        //                    evse is not null)
                        //                {
                        //                    await OnEVSEStatusChangedLocal(Timestamp.Now,
                        //                                                   evse,
                        //                                                   evse.Status);
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnEVSEChanged), ": ",
                        //                            Environment.NewLine, e.Message,
                        //                            Environment.NewLine, e.StackTrace ?? "");
                        //            }
                        //        }

                        //    }
                        //    else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                        //    {

                        //        var OnEVSERemovedLocal = OnEVSERemoved;
                        //        if (OnEVSERemovedLocal is not null)
                        //        {
                        //            try
                        //            {
                        //                if (existingCDR.TryGetEVSE(evseUId, out var evse) &&
                        //                    evse is not null)
                        //                {
                        //                    await OnEVSERemovedLocal(evse);
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnEVSERemoved), ": ",
                        //                            Environment.NewLine, e.Message,
                        //                            Environment.NewLine, e.StackTrace ?? "");
                        //            }
                        //        }

                        //        var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                        //        if (OnEVSEStatusChangedLocal is not null)
                        //        {
                        //            try
                        //            {
                        //                if (existingCDR.TryGetEVSE(evseUId, out var oldEVSE) &&
                        //                    CDR.        TryGetEVSE(evseUId, out var newEVSE) &&
                        //                    oldEVSE is not null &&
                        //                    newEVSE is not null)
                        //                {
                        //                    await OnEVSEStatusChangedLocal(Timestamp.Now,
                        //                                                   oldEVSE,
                        //                                                   newEVSE.Status,
                        //                                                   oldEVSE.Status);
                        //                }
                        //            }
                        //            catch (Exception e)
                        //            {
                        //                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnEVSEChanged), ": ",
                        //                            Environment.NewLine, e.Message,
                        //                            Environment.NewLine, e.StackTrace ?? "");
                        //            }
                        //        }

                        //    }

                        //}

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
                       "The party identification of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region TryPatchCDR       (PartyId, CDRId, CDRPatch, AllowDowngrades = false, ...)

        public async Task<PatchResult<CDR>> TryPatchCDR(Party_Idv3         PartyId,
                                                        CDR_Id             CDRId,
                                                        JObject            CDRPatch,
                                                        Boolean?           AllowDowngrades     = false,
                                                        Boolean            SkipNotifications   = false,
                                                        EventTracking_Id?  EventTrackingId     = null,
                                                        User_Id?           CurrentUserId       = null,
                                                        CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                if (party.CDRs.TryGetValue(CDRId, out var existingCDR))
                {

                    var patchResult = existingCDR.TryPatch(
                                          CDRPatch,
                                          AllowDowngrades ?? this.AllowDowngrades ?? false,
                                          EventTrackingId
                                      );

                    if (patchResult.IsSuccess &&
                        patchResult.PatchedData is not null)
                    {

                        var updateCDRResult = await UpdateCDR(
                                                        patchResult.PatchedData,
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
                                       "Could not update the charge detail record: " + updateCDRResult.ErrorResponse
                                   );

                    }

                    return patchResult;

                }

                return PatchResult<CDR>.Failed(
                           EventTrackingId,
                           $"The given charge detail record '{CDRId}' is unknown!"
                       );

            }

            return PatchResult<CDR>.Failed(
                       EventTrackingId,
                       $"The party identification '{PartyId}' of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveCDR         (CDR, ...)

        public async Task<RemoveResult<CDR>>

            RemoveCDR(CDR                CDR,
                      Boolean            SkipNotifications   = false,
                      EventTracking_Id?  EventTrackingId     = null,
                      User_Id?           CurrentUserId       = null,
                      CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(CDR.PartyId, out var party))
            {

                if (party.CDRs.TryRemove(CDR.Id, out var cdr))
                {

                    await LogAsset(
                              CommonBaseAPI.removeChargeDetailRecord,
                              cdr.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
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

                    return RemoveResult<CDR>.Success(
                               EventTrackingId,
                               cdr
                           );

                }

                return RemoveResult<CDR>.Failed(
                           EventTrackingId,
                           CDR,
                           "The charge detail record identification of the charge detail record is unknown!"
                       );

            }

            return RemoveResult<CDR>.Failed(
                       EventTrackingId,
                       CDR,
                       "The party identification of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveCDR         (PartyId, CDRId, ...)

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
                              CommonBaseAPI.removeChargeDetailRecord,
                              cdr.ToJSON(
                                  true,
                                  true,
                                  true,
                                  true,
                                  CustomCDRSerializer,
                                  CustomCDRTokenSerializer,
                                  CustomCDRLocationSerializer,
                                  CustomEVSEEnergyMeterSerializer,
                                  CustomTransparencySoftwareStatusSerializer,
                                  CustomTransparencySoftwareSerializer,
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

                    return RemoveResult<CDR>.Success(
                               EventTrackingId,
                               cdr
                           );

                }

                return RemoveResult<CDR>.Failed(
                           EventTrackingId,
                           "The charge detail record identification of the charge detail record is unknown!"
                       );

            }

            return RemoveResult<CDR>.Failed(
                       EventTrackingId,
                       "The party identification of the charge detail record is unknown!"
                   );

        }

        #endregion

        #region RemoveAllCDRs     (IncludeCDRs = null, ...)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">An optional charging charge detail record filter.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllCDRs(Func<CDR, Boolean>?  IncludeCDRs         = null,
                          EventTracking_Id?    EventTrackingId     = null,
                          User_Id?             CurrentUserId       = null,
                          CancellationToken    CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedCDRs = new List<CDR>();

            if (IncludeCDRs is null)
            {
                foreach (var party in parties.Values)
                {
                    removedCDRs.AddRange(party.CDRs.Values);
                    party.CDRs.Clear();
                }
            }

            else
            {

                foreach (var party in parties.Values)
                {
                    foreach (var cdr in party.CDRs.Values)
                    {
                        if (IncludeCDRs(cdr))
                            removedCDRs.Add(cdr);
                    }
                }

                foreach (var cdr in removedCDRs)
                    parties[cdr.PartyId].CDRs.TryRemove(cdr.Id, out _);

            }


            await LogAsset(
                      CommonBaseAPI.removeAllChargeDetailRecords,
                      new JArray(
                          removedCDRs.Select(
                              cdr => cdr.ToJSON(
                                         true,
                                         true,
                                         true,
                                         true,
                                         CustomCDRSerializer,
                                         CustomCDRTokenSerializer,
                                         CustomCDRLocationSerializer,
                                         CustomEVSEEnergyMeterSerializer,
                                         CustomTransparencySoftwareStatusSerializer,
                                         CustomTransparencySoftwareSerializer,
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
                                     )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<CDR>>.Success(
                       EventTrackingId,
                       removedCDRs
                   );

        }

        #endregion

        #region RemoveAllCDRs     (IncludeCDRIds, ...)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRIds">An optional charging charge detail record filter.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllCDRs(Func<CDR_Id, Boolean>  IncludeCDRIds,
                          EventTracking_Id?      EventTrackingId     = null,
                          User_Id?               CurrentUserId       = null,
                          CancellationToken      CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            var removedCDRs = new List<CDR>();

            foreach (var party in parties.Values)
            {
                foreach (var cdr in party.CDRs.Values)
                {
                    if (IncludeCDRIds(cdr.Id))
                        removedCDRs.Add(cdr);
                }
            }

            foreach (var cdr in removedCDRs)
                parties[cdr.PartyId].CDRs.TryRemove(cdr.Id, out _);


            await LogAsset(
                      CommonBaseAPI.removeAllChargeDetailRecords,
                      new JArray(
                          removedCDRs.Select(
                              cdr => cdr.ToJSON(
                                         true,
                                         true,
                                         true,
                                         true,
                                         CustomCDRSerializer,
                                         CustomCDRTokenSerializer,
                                         CustomCDRLocationSerializer,
                                         CustomEVSEEnergyMeterSerializer,
                                         CustomTransparencySoftwareStatusSerializer,
                                         CustomTransparencySoftwareSerializer,
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
                                     )
                              )
                      ),
                      EventTrackingId,
                      CurrentUserId,
                      CancellationToken
                  );

            return RemoveResult<IEnumerable<CDR>>.Success(
                       EventTrackingId,
                       removedCDRs
                   );

        }

        #endregion

        #region RemoveAllCDRs     (PartyId, ...)

        /// <summary>
        /// Remove all charge detail records owned by the given party.
        /// </summary>
        /// <param name="PartyId">The identification of the party.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>>

            RemoveAllCDRs(Party_Idv3         PartyId,
                          EventTracking_Id?  EventTrackingId     = null,
                          User_Id?           CurrentUserId       = null,
                          CancellationToken  CancellationToken   = default)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (parties.TryGetValue(PartyId, out var party))
            {

                var removedCDRs = party.CDRs.Values.ToArray();
                party.CDRs.Clear();

                await LogAsset(
                          CommonBaseAPI.removeAllChargeDetailRecords,
                          new JArray(
                              removedCDRs.Select(
                                  cdr => cdr.ToJSON(
                                             true,
                                             true,
                                             true,
                                             true,
                                             CustomCDRSerializer,
                                             CustomCDRTokenSerializer,
                                             CustomCDRLocationSerializer,
                                             CustomEVSEEnergyMeterSerializer,
                                             CustomTransparencySoftwareStatusSerializer,
                                             CustomTransparencySoftwareSerializer,
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
                                         )
                                  )
                          ),
                          EventTrackingId,
                          CurrentUserId,
                          CancellationToken
                      );

                return RemoveResult<IEnumerable<CDR>>.Success(
                           EventTrackingId,
                           removedCDRs
                       );

            }

            return RemoveResult<IEnumerable<CDR>>.Failed(
                       EventTrackingId,
                       "The party identification of the charge detail record is unknown!"
                   );

        }

        #endregion


        #region CDRExists         (PartyId, CDRId)

        public Boolean CDRExists(Party_Idv3  PartyId,
                                 CDR_Id      CDRId)
        {

            if (parties.TryGetValue(PartyId, out var party))
                return party.CDRs.ContainsKey(CDRId);

            return false;

        }

        #endregion

        #region TryGetCDR         (PartyId, CDRId, out CDR)

        public Boolean TryGetCDR(Party_Idv3                    PartyId,
                                 CDR_Id                        CDRId,
                                 [NotNullWhen(true)] out CDR?  CDR)
        {

            if (parties.       TryGetValue(PartyId,   out var party) &&
                party.CDRs.TryGetValue(CDRId, out CDR))
            {
                return true;
            }

            var OnChargeDetailRecordLookupLocal = OnChargeDetailRecordSlowStorageLookup;
            if (OnChargeDetailRecordLookupLocal is not null)
            {
                try
                {

                    var cdr = OnChargeDetailRecordLookupLocal(
                                    PartyId,
                                    CDRId
                                ).Result;

                    if (cdr is not null)
                    {
                        CDR = cdr;
                        return true;
                    }

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


    }

}
