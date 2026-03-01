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

using System.Diagnostics;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;
using Hermod = org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.EMSP.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    #region OnPutLocation    (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PUT Location request was received.
    /// </summary>
    public delegate Task OnPutLocationRequestDelegate2 (DateTimeOffset       Timestamp,
                                                        EMSP_HTTPAPI         Sender,
                                                        EventTracking_Id     EventTrackingId,
                                                        CountryCode?         From_CountryCode,
                                                        Party_Id?            From_PartyId,
                                                        CountryCode?         To_CountryCode,
                                                        Party_Id?            To_PartyId,

                                                        Location             Location,

                                                        CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PUT Location request had been sent.
    /// </summary>
    public delegate Task OnPutLocationResponseDelegate2(DateTimeOffset       Timestamp,
                                                        EMSP_HTTPAPI         Sender,
                                                        EventTracking_Id     EventTrackingId,
                                                        CountryCode?         From_CountryCode,
                                                        Party_Id?            From_PartyId,
                                                        CountryCode?         To_CountryCode,
                                                        Party_Id?            To_PartyId,

                                                        Location             Location,

                                                        TimeSpan             Runtime,
                                                        CancellationToken    CancellationToken);

    #endregion

    #region OnPatchLocation  (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PATCH Location request was received.
    /// </summary>
    public delegate Task OnPatchLocationRequestDelegate2 (DateTimeOffset       Timestamp,
                                                          EMSP_HTTPAPI         Sender,
                                                          EventTracking_Id     EventTrackingId,
                                                          CountryCode?         From_CountryCode,
                                                          Party_Id?            From_PartyId,
                                                          CountryCode?         To_CountryCode,
                                                          Party_Id?            To_PartyId,

                                                          Location_Id          LocationId,
                                                          JObject              LocationPatch,

                                                          CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PATCH Location request had been sent.
    /// </summary>
    public delegate Task OnPatchLocationResponseDelegate2(DateTimeOffset       Timestamp,
                                                          EMSP_HTTPAPI         Sender,
                                                          EventTracking_Id     EventTrackingId,
                                                          CountryCode?         From_CountryCode,
                                                          Party_Id?            From_PartyId,
                                                          CountryCode?         To_CountryCode,
                                                          Party_Id?            To_PartyId,

                                                          Location_Id          LocationId,
                                                          JObject              LocationPatch,

                                                          TimeSpan             Runtime,
                                                          CancellationToken    CancellationToken);

    #endregion

    #region OnPutEVSE        (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PUT EVSE request was received.
    /// </summary>
    public delegate Task OnPutEVSERequestDelegate2 (DateTimeOffset       Timestamp,
                                                    EMSP_HTTPAPI         Sender,
                                                    EventTracking_Id     EventTrackingId,
                                                    CountryCode?         From_CountryCode,
                                                    Party_Id?            From_PartyId,
                                                    CountryCode?         To_CountryCode,
                                                    Party_Id?            To_PartyId,

                                                    EVSE                 EVSE,

                                                    CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PUT EVSE request had been sent.
    /// </summary>
    public delegate Task OnPutEVSEResponseDelegate2(DateTimeOffset       Timestamp,
                                                    EMSP_HTTPAPI         Sender,
                                                    EventTracking_Id     EventTrackingId,
                                                    CountryCode?         From_CountryCode,
                                                    Party_Id?            From_PartyId,
                                                    CountryCode?         To_CountryCode,
                                                    Party_Id?            To_PartyId,

                                                    EVSE                 EVSE,

                                                    TimeSpan             Runtime,
                                                    CancellationToken    CancellationToken);

    #endregion

    #region OnPatchEVSE      (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PATCH EVSE request was received.
    /// </summary>
    public delegate Task OnPatchEVSERequestDelegate2 (DateTimeOffset       Timestamp,
                                                      EMSP_HTTPAPI         Sender,
                                                      EventTracking_Id     EventTrackingId,
                                                      CountryCode?         From_CountryCode,
                                                      Party_Id?            From_PartyId,
                                                      CountryCode?         To_CountryCode,
                                                      Party_Id?            To_PartyId,

                                                      EVSE_UId             EVSEUId,
                                                      JObject              EVSEPatch,

                                                      CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PATCH EVSE request had been sent.
    /// </summary>
    public delegate Task OnPatchEVSEResponseDelegate2(DateTimeOffset       Timestamp,
                                                      EMSP_HTTPAPI         Sender,
                                                      EventTracking_Id     EventTrackingId,
                                                      CountryCode?         From_CountryCode,
                                                      Party_Id?            From_PartyId,
                                                      CountryCode?         To_CountryCode,
                                                      Party_Id?            To_PartyId,

                                                      EVSE_UId             EVSEUId,
                                                      JObject              EVSEPatch,

                                                      TimeSpan             Runtime,
                                                      CancellationToken    CancellationToken);

    #endregion

    #region OnPutConnector   (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PUT Connector request was received.
    /// </summary>
    public delegate Task OnPutConnectorRequestDelegate2 (DateTimeOffset       Timestamp,
                                                         EMSP_HTTPAPI         Sender,
                                                         EventTracking_Id     EventTrackingId,
                                                         CountryCode?         From_CountryCode,
                                                         Party_Id?            From_PartyId,
                                                         CountryCode?         To_CountryCode,
                                                         Party_Id?            To_PartyId,

                                                         Connector            Connector,

                                                         CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PUT Connector request had been sent.
    /// </summary>
    public delegate Task OnPutConnectorResponseDelegate2(DateTimeOffset       Timestamp,
                                                         EMSP_HTTPAPI         Sender,
                                                         EventTracking_Id     EventTrackingId,
                                                         CountryCode?         From_CountryCode,
                                                         Party_Id?            From_PartyId,
                                                         CountryCode?         To_CountryCode,
                                                         Party_Id?            To_PartyId,

                                                         Connector            Connector,

                                                         TimeSpan             Runtime,
                                                         CancellationToken    CancellationToken);

    #endregion

    #region OnPatchConnector (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PATCH Connector request was received.
    /// </summary>
    public delegate Task OnPatchConnectorRequestDelegate2 (DateTimeOffset       Timestamp,
                                                           EMSP_HTTPAPI         Sender,
                                                           EventTracking_Id     EventTrackingId,
                                                           CountryCode?         From_CountryCode,
                                                           Party_Id?            From_PartyId,
                                                           CountryCode?         To_CountryCode,
                                                           Party_Id?            To_PartyId,

                                                           Connector_Id         ConnectorId,
                                                           JObject              ConnectorPatch,

                                                           CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PATCH Connector request had been sent.
    /// </summary>
    public delegate Task OnPatchConnectorResponseDelegate2(DateTimeOffset       Timestamp,
                                                           EMSP_HTTPAPI         Sender,
                                                           EventTracking_Id     EventTrackingId,
                                                           CountryCode?         From_CountryCode,
                                                           Party_Id?            From_PartyId,
                                                           CountryCode?         To_CountryCode,
                                                           Party_Id?            To_PartyId,

                                                           Connector_Id         ConnectorId,
                                                           JObject              ConnectorPatch,

                                                           TimeSpan             Runtime,
                                                           CancellationToken    CancellationToken);

    #endregion


    #region OnPutTariff      (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PUT Tariff request was received.
    /// </summary>
    public delegate Task OnPutTariffRequestDelegate2 (DateTimeOffset       Timestamp,
                                                      EMSP_HTTPAPI         Sender,
                                                      EventTracking_Id     EventTrackingId,
                                                      CountryCode?         From_CountryCode,
                                                      Party_Id?            From_PartyId,
                                                      CountryCode?         To_CountryCode,
                                                      Party_Id?            To_PartyId,

                                                      Tariff               Tariff,

                                                      CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PUT Tariff request had been sent.
    /// </summary>
    public delegate Task OnPutTariffResponseDelegate2(DateTimeOffset       Timestamp,
                                                      EMSP_HTTPAPI         Sender,
                                                      EventTracking_Id     EventTrackingId,
                                                      CountryCode?         From_CountryCode,
                                                      Party_Id?            From_PartyId,
                                                      CountryCode?         To_CountryCode,
                                                      Party_Id?            To_PartyId,

                                                      Tariff               Tariff,

                                                      TimeSpan             Runtime,
                                                      CancellationToken    CancellationToken);

    #endregion


    #region OnPutSession     (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PUT Session request was received.
    /// </summary>
    public delegate Task OnPutSessionRequestDelegate2 (DateTimeOffset       Timestamp,
                                                       EMSP_HTTPAPI         Sender,
                                                       EventTracking_Id     EventTrackingId,
                                                       CountryCode?         From_CountryCode,
                                                       Party_Id?            From_PartyId,
                                                       CountryCode?         To_CountryCode,
                                                       Party_Id?            To_PartyId,

                                                       Session              Session,

                                                       CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PUT Session request had been sent.
    /// </summary>
    public delegate Task OnPutSessionResponseDelegate2(DateTimeOffset       Timestamp,
                                                       EMSP_HTTPAPI         Sender,
                                                       EventTracking_Id     EventTrackingId,
                                                       CountryCode?         From_CountryCode,
                                                       Party_Id?            From_PartyId,
                                                       CountryCode?         To_CountryCode,
                                                       Party_Id?            To_PartyId,

                                                       Session              Session,

                                                       TimeSpan             Runtime,
                                                       CancellationToken    CancellationToken);

    #endregion

    #region OnPatchSession   (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a PATCH Session request was received.
    /// </summary>
    public delegate Task OnPatchSessionRequestDelegate2 (DateTimeOffset       Timestamp,
                                                         EMSP_HTTPAPI         Sender,
                                                         EventTracking_Id     EventTrackingId,
                                                         CountryCode?         From_CountryCode,
                                                         Party_Id?            From_PartyId,
                                                         CountryCode?         To_CountryCode,
                                                         Party_Id?            To_PartyId,

                                                         Session_Id           SessionId,
                                                         JObject              SessionPatch,

                                                         CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a PATCH Session request had been sent.
    /// </summary>
    public delegate Task OnPatchSessionResponseDelegate2(DateTimeOffset       Timestamp,
                                                         EMSP_HTTPAPI         Sender,
                                                         EventTracking_Id     EventTrackingId,
                                                         CountryCode?         From_CountryCode,
                                                         Party_Id?            From_PartyId,
                                                         CountryCode?         To_CountryCode,
                                                         Party_Id?            To_PartyId,

                                                         Session_Id           SessionId,
                                                         JObject              SessionPatch,

                                                         TimeSpan             Runtime,
                                                         CancellationToken    CancellationToken);

    #endregion


    #region OnPostCDR        (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a POST CDR request was received.
    /// </summary>
    public delegate Task OnPostCDRRequestDelegate2 (DateTimeOffset       Timestamp,
                                                    EMSP_HTTPAPI         Sender,
                                                    EventTracking_Id     EventTrackingId,
                                                    CountryCode?         From_CountryCode,
                                                    Party_Id?            From_PartyId,
                                                    CountryCode?         To_CountryCode,
                                                    Party_Id?            To_PartyId,

                                                    CDR                  CDR,

                                                    CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a POST CDR request had been sent.
    /// </summary>
    public delegate Task OnPostCDRResponseDelegate2(DateTimeOffset       Timestamp,
                                                    EMSP_HTTPAPI         Sender,
                                                    EventTracking_Id     EventTrackingId,
                                                    CountryCode?         From_CountryCode,
                                                    Party_Id?            From_PartyId,
                                                    CountryCode?         To_CountryCode,
                                                    Party_Id?            To_PartyId,

                                                    CDR                  CDR,

                                                    Hermod.Location      CDRLocation,
                                                    TimeSpan             Runtime,
                                                    CancellationToken    CancellationToken);

    #endregion


    #region OnPostToken      (Request|Response)Delegate

    /// <summary>
    /// A delegate called whenever a POST Token request was received.
    /// </summary>
    public delegate Task OnPostTokenRequestDelegate2 (DateTimeOffset       Timestamp,
                                                      EMSP_HTTPAPI         Sender,
                                                      EventTracking_Id     EventTrackingId,
                                                      CountryCode?         From_CountryCode,
                                                      Party_Id?            From_PartyId,
                                                      CountryCode?         To_CountryCode,
                                                      Party_Id?            To_PartyId,

                                                      Token_Id             TokenId,
                                                      TokenType?           RequestedTokenType,
                                                      LocationReference?   LocationReference,
                                                      CancellationToken    CancellationToken);

    /// <summary>
    /// A delegate whenever a response to a POST Token request had been sent.
    /// </summary>
    public delegate Task OnPostTokenResponseDelegate2(DateTimeOffset       Timestamp,
                                                      EMSP_HTTPAPI         Sender,
                                                      EventTracking_Id     EventTrackingId,
                                                      CountryCode?         From_CountryCode,
                                                      Party_Id?            From_PartyId,
                                                      CountryCode?         To_CountryCode,
                                                      Party_Id?            To_PartyId,

                                                      Token_Id             TokenId,
                                                      TokenType?           RequestedTokenType,
                                                      LocationReference?   LocationReference,

                                                      AuthorizationInfo    Result,
                                                      TimeSpan             Runtime,
                                                      CancellationToken    CancellationToken);

    #endregion


    public static class EMSP_HTTPAPI_Extensions
    {

        #region ParseMandatoryLocation              (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The EMSP HTTP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocation(this OCPIRequest                                Request,
                                                     EMSP_HTTPAPI                                    EMSP_HTTPAPI,
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


            if (!EMSP_HTTPAPI.TryGetRemoteLocation(
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

        #region ParseOptionalLocation               (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location,                                                        out OCPIResponseBuilder, FailOnMissingLocation = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The EMSP HTTP API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocation(this OCPIRequest                                 Request,
                                                    EMSP_HTTPAPI                                     EMSP_HTTPAPI,
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


            EMSP_HTTPAPI.TryGetRemoteLocation(
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


        #region ParseMandatoryLocationEVSE          (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved location.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryLocationEVSE(this OCPIRequest                                Request,
                                                         EMSP_HTTPAPI                                    EMSP_HTTPAPI,
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


            if (!EMSP_HTTPAPI.TryGetRemoteLocation(
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

        #region ParseOptionalLocationEVSE           (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE,                                 out OCPIResponseBuilder, FailOnMissingEVSE = true)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="LocationId">The parsed unique location identification.</param>
        /// <param name="Location">The resolved user.</param>
        /// <param name="EVSEUId">The parsed unique EVSE identification.</param>
        /// <param name="EVSE">The resolved EVSE.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalLocationEVSE(this OCPIRequest                                 Request,
                                                        EMSP_HTTPAPI                                     EMSP_HTTPAPI,
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


            EMSP_HTTPAPI.TryGetRemoteLocation(
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


        #region ParseMandatoryLocationEVSEConnector (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
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
                                                                  EMSP_HTTPAPI                                    EMSP_HTTPAPI,
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


            if (!EMSP_HTTPAPI.TryGetRemoteLocation(
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

        #region ParseOptionalLocationEVSEConnector  (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out LocationId, out Location, out EVSEUId, out EVSE, out ConnectorId, out Connector, out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the location identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
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
                                                                 EMSP_HTTPAPI                                     EMSP_HTTPAPI,
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


            if (!EMSP_HTTPAPI.TryGetRemoteLocation(
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


        #region ParseMandatoryTariff                (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out TariffId,   out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryTariff(this OCPIRequest                                Request,
                                                   EMSP_HTTPAPI                                    EMSP_HTTPAPI,
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


            if (!EMSP_HTTPAPI.TryGetRemoteTariff(
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

        #region ParseOptionalTariff                 (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out TariffId,   out Tariff,   out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the tariff identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="TariffId">The parsed unique tariff identification.</param>
        /// <param name="Tariff">The resolved user.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalTariff(this OCPIRequest                                  Request,
                                                  EMSP_HTTPAPI                                      EMSP_HTTPAPI,
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


            EMSP_HTTPAPI.TryGetRemoteTariff(
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


        #region ParseMandatorySession               (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out SessionId,  out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatorySession(this OCPIRequest                                Request,
                                                    EMSP_HTTPAPI                                    EMSP_HTTPAPI,
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


            if (!EMSP_HTTPAPI.TryGetRemoteSession(Party_Idv3.From(countryCode, partyId), sessionId, out Session))
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

        #region ParseOptionalSession                (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out SessionId,  out Session,  out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the session identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="SessionId">The parsed unique session identification.</param>
        /// <param name="Session">The resolved session.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalSession(this OCPIRequest                                  Request,
                                                   EMSP_HTTPAPI                                      EMSP_HTTPAPI,
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


            EMSP_HTTPAPI.TryGetRemoteSession(
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


        #region ParseMandatoryCDR                   (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out CDRId,      out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseMandatoryCDR(this OCPIRequest                                Request,
                                                EMSP_HTTPAPI                                    EMSP_HTTPAPI,
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


            if (!EMSP_HTTPAPI.TryGetRemoteCDR(
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

        #region ParseOptionalCDR                    (this Request, EMSP_HTTPAPI, out CountryCode, out PartyId, out CDRId,      out CDR,      out OCPIResponseBuilder)

        /// <summary>
        /// Parse the given HTTP request and return the charge detail record identification
        /// for the given HTTP hostname and HTTP query parameter or an HTTP error response.
        /// </summary>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="EMSP_HTTPAPI">The OCPI Common API.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        /// <param name="CDRId">The parsed unique charge detail record identification.</param>
        /// <param name="CDR">The resolved charge detail record.</param>
        /// <param name="OCPIResponseBuilder">The OCPI response builder in case of errors.</param>
        public static Boolean ParseOptionalCDR(this OCPIRequest                                  Request,
                                               EMSP_HTTPAPI                                      EMSP_HTTPAPI,
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


            EMSP_HTTPAPI.TryGetRemoteCDR(
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
    /// The EMSP HTTP API for e-mobility service providers.
    /// CPOs will connect to this API.
    /// </summary>
    public class EMSP_HTTPAPI : AHTTPExtAPIXExtension2<CommonAPI, HTTPExtAPIX>
    {

        #region (class) APICounters

        public class APICounters(APICounterValues?  PutLocation      = null,
                                 APICounterValues?  PatchLocation    = null,
                                 APICounterValues?  PutEVSE          = null,
                                 APICounterValues?  PatchEVSE        = null,
                                 APICounterValues?  PutConnector     = null,
                                 APICounterValues?  PatchConnector   = null,
                                 APICounterValues?  PutTariff        = null,
                                 APICounterValues?  PutSession       = null,
                                 APICounterValues?  PatchSession     = null,
                                 APICounterValues?  PostCDR          = null,
                                 APICounterValues?  PostToken        = null)
        {

            public APICounterValues PutLocation      { get; } = PutLocation    ?? new APICounterValues();
            public APICounterValues PatchLocation    { get; } = PatchLocation  ?? new APICounterValues();
            public APICounterValues PutEVSE          { get; } = PutEVSE        ?? new APICounterValues();
            public APICounterValues PatchEVSE        { get; } = PatchEVSE      ?? new APICounterValues();
            public APICounterValues PutConnector     { get; } = PutConnector   ?? new APICounterValues();
            public APICounterValues PatchConnector   { get; } = PatchConnector ?? new APICounterValues();
            public APICounterValues PutTariff        { get; } = PutTariff      ?? new APICounterValues();
            public APICounterValues PutSession       { get; } = PutSession     ?? new APICounterValues();
            public APICounterValues PatchSession     { get; } = PatchSession   ?? new APICounterValues();
            public APICounterValues PostCDR          { get; } = PostCDR        ?? new APICounterValues();
            public APICounterValues PostToken        { get; } = PostToken      ?? new APICounterValues();

            public JObject ToJSON()

                => JSONObject.Create(

                       new JProperty("PutLocation",     PutLocation.   ToJSON()),
                       new JProperty("PatchLocation",   PatchLocation. ToJSON()),
                       new JProperty("PutEVSE",         PutEVSE.       ToJSON()),
                       new JProperty("PatchEVSE",       PatchEVSE.     ToJSON()),
                       new JProperty("PutConnector",    PutConnector.  ToJSON()),
                       new JProperty("PatchConnector",  PatchConnector.ToJSON()),
                       new JProperty("PutTariff",       PutTariff.     ToJSON()),
                       new JProperty("PutSession",      PutSession.    ToJSON()),
                       new JProperty("PatchSession",    PatchSession.  ToJSON()),
                       new JProperty("PostCDR",         PostCDR.       ToJSON()),
                       new JProperty("PostToken",       PostToken.     ToJSON())

                   );

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName   = $"GraphDefined OCPI {Version.String} EMSP HTTP API";

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public     static readonly HTTPPath  DefaultURLPathPrefix     = HTTPPath.Parse($"{Version.String}/emsp/");

        /// <summary>
        /// The default EMSP API logfile name.
        /// </summary>
        public     const           String    DefaultLogfileName       = $"OCPI{Version.String}_EMSPAPI.log";

        #endregion

        #region Properties

        /// <summary>
        /// The OCPI CommonAPI.
        /// </summary>
        public CommonAPI             CommonAPI
            => HTTPBaseAPI;

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2.x does not define any behaviour for this.
        /// </summary>
        public Boolean?              AllowDowngrades    { get; }

        /// <summary>
        /// API Counters.
        /// </summary>
        public APICounters           Counters           { get; }

        /// <summary>
        /// The EMSP HTTP API logger.
        /// </summary>
        public EMSP_HTTPAPI_Logger?  HTTPLogger         { get; set; }

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

        public CustomJObjectSerializerDelegate<AuthorizationInfo>?           CustomAuthorizationInfoSerializer             { get; set; }
        public CustomJObjectSerializerDelegate<LocationReference>?           CustomLocationReferenceSerializer             { get; set; }

        #endregion

        #region Events

        public HTTP_Events  HTTPEvents    { get; } = new HTTP_Events();

        public class HTTP_Events
        {

            #region Location(s)

            #region (protected internal) GetLocationsHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a GET locations HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetLocationsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET locations HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetLocationsHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnGetLocationsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetLocationsHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a GET locations HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetLocationsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET locations HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetLocationsHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnGetLocationsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteLocationsHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a delete locations HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteLocationsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a delete locations HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteLocationsHTTPRequest(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               CancellationToken  CancellationToken)

                => OnDeleteLocationsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteLocationsHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a delete locations HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteLocationsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a delete locations HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteLocationsHTTPResponse(DateTimeOffset     Timestamp,
                                                                HTTPAPIX           API,
                                                                OCPIRequest        Request,
                                                                OCPIResponse       Response,
                                                                CancellationToken  CancellationToken)

                => OnDeleteLocationsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion



            #region (protected internal) GetLocationHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a GET location HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetLocationHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET location HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetLocationHTTPRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

                => OnGetLocationHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetLocationHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a GET location HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetLocationHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET location HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetLocationHTTPResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

                => OnGetLocationHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PutLocationHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a PUT location HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPutLocationHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PUT location HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PutLocationHTTPRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

                => OnPutLocationHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PutLocationHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a PUT location HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPutLocationHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PUT location HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PutLocationHTTPResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

                => OnPutLocationHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PatchLocationHTTPRequest    (Request)

            /// <summary>
            /// An event sent whenever a PATCH location HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPatchLocationHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PATCH location HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PatchLocationHTTPRequest(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             CancellationToken  CancellationToken)

                => OnPatchLocationHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PatchLocationHTTPResponse   (Response)

            /// <summary>
            /// An event sent whenever a PATCH location HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPatchLocationHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PATCH location HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PatchLocationHTTPResponse(DateTimeOffset     Timestamp,
                                                              HTTPAPIX           API,
                                                              OCPIRequest        Request,
                                                              OCPIResponse       Response,
                                                              CancellationToken  CancellationToken)

                => OnPatchLocationHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteLocationHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a DELETE location HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteLocationHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE location HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteLocationHTTPRequest(DateTimeOffset     Timestamp,
                                                              HTTPAPIX           API,
                                                              OCPIRequest        Request,
                                                              CancellationToken  CancellationToken)

                => OnDeleteLocationHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteLocationHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a DELETE location HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteLocationHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE location HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteLocationHTTPResponse(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               OCPIResponse       Response,
                                                               CancellationToken  CancellationToken)

                => OnDeleteLocationHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region EVSE/EVSE status

            #region (protected internal) GetEVSEHTTPRequest           (Request)

            /// <summary>
            /// An event sent whenever a GET EVSE HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetEVSEHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET EVSE HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetEVSEHTTPRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

                => OnGetEVSEHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetEVSEHTTPResponse          (Response)

            /// <summary>
            /// An event sent whenever a GET EVSE HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetEVSEHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET EVSE HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetEVSEHTTPResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

                => OnGetEVSEHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PutEVSEHTTPRequest           (Request)

            /// <summary>
            /// An event sent whenever a PUT EVSE HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPutEVSEHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PUT EVSE HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PutEVSEHTTPRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

                => OnPutEVSEHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PutEVSEHTTPResponse          (Response)

            /// <summary>
            /// An event sent whenever a PUT EVSE HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPutEVSEHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PUT EVSE HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PutEVSEHTTPResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

                => OnPutEVSEHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PatchEVSEHTTPRequest         (Request)

            /// <summary>
            /// An event sent whenever a PATCH EVSE HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPatchEVSEHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PATCH EVSE HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PatchEVSEHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnPatchEVSEHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PatchEVSEHTTPResponse        (Response)

            /// <summary>
            /// An event sent whenever a PATCH EVSE HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPatchEVSEHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PATCH EVSE HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PatchEVSEHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnPatchEVSEHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteEVSEHTTPRequest        (Request)

            /// <summary>
            /// An event sent whenever a DELETE EVSE HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteEVSEHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE EVSE HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteEVSEHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnDeleteEVSEHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteEVSEHTTPResponse       (Response)

            /// <summary>
            /// An event sent whenever a DELETE EVSE HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteEVSEHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE EVSE HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteEVSEHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnDeleteEVSEHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion



            #region (protected internal) OnPostEVSEStatusHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a POST EVSE status HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPostEVSEStatusHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a POST EVSE status HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PostEVSEStatusHTTPRequest(DateTimeOffset     Timestamp,
                                                              HTTPAPIX           API,
                                                              OCPIRequest        Request,
                                                              CancellationToken  CancellationToken)

                => OnPostEVSEStatusHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) OnPostEVSEStatusHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a POST EVSE status HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPostEVSEStatusHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a POST EVSE status HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PostEVSEStatusHTTPResponse(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               OCPIResponse       Response,
                                                               CancellationToken  CancellationToken)

                => OnPostEVSEStatusHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region Connector

            #region (protected internal) GetConnectorHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a GET connector HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetConnectorHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET connector HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetConnectorHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnGetConnectorHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetConnectorHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a GET connector HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetConnectorHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET connector HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetConnectorHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnGetConnectorHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PutConnectorHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a PUT connector HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPutConnectorHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PUT connector HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PutConnectorHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnPutConnectorHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PutConnectorHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a PUT connector HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPutConnectorHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PUT connector HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PutConnectorHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnPutConnectorHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PatchConnectorHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a PATCH connector HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPatchConnectorHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PATCH connector HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PatchConnectorHTTPRequest(DateTimeOffset     Timestamp,
                                                              HTTPAPIX           API,
                                                              OCPIRequest        Request,
                                                              CancellationToken  CancellationToken)

                => OnPatchConnectorHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PatchConnectorHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a PATCH connector HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPatchConnectorHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PATCH connector HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PatchConnectorHTTPResponse(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               OCPIResponse       Response,
                                                               CancellationToken  CancellationToken)

                => OnPatchConnectorHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteConnectorHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a DELETE connector HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteConnectorHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE connector HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteConnectorHTTPRequest(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               CancellationToken  CancellationToken)

                => OnDeleteConnectorHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteConnectorHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a DELETE connector HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteConnectorHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE connector HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteConnectorHTTPResponse(DateTimeOffset     Timestamp,
                                                                HTTPAPIX           API,
                                                                OCPIRequest        Request,
                                                                OCPIResponse       Response,
                                                                CancellationToken  CancellationToken)

                => OnDeleteConnectorHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region Tariff(s)

            #region (protected internal) GetTariffsHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a GET tariffs HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetTariffsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET tariffs HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetTariffsHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnGetTariffsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetTariffsHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a GET tariffs HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetTariffsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET tariffs HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetTariffsHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnGetTariffsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteTariffsHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a DELETE tariffs HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteTariffsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE tariffs HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteTariffsHTTPRequest(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             CancellationToken  CancellationToken)

                => OnDeleteTariffsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteTariffsHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a DELETE tariffs HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteTariffsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE tariffs HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteTariffsHTTPResponse(DateTimeOffset     Timestamp,
                                                              HTTPAPIX           API,
                                                              OCPIRequest        Request,
                                                              OCPIResponse       Response,
                                                              CancellationToken  CancellationToken)

                => OnDeleteTariffsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion



            #region (protected internal) GetTariffHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a GET tariff HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetTariffHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET tariff HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetTariffHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnGetTariffHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetTariffHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a GET tariff HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetTariffHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET tariff HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetTariffHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnGetTariffHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PutTariffHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a PUT tariff HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPutTariffHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PUT tariff HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PutTariffHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnPutTariffHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PutTariffHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a PUT tariff HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPutTariffHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PUT tariff HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PutTariffHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnPutTariffHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PatchTariffHTTPRequest    (Request)

            /// <summary>
            /// An event sent whenever a PATCH tariff HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPatchTariffHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PATCH tariff HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PatchTariffHTTPRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

                => OnPatchTariffHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PatchTariffHTTPResponse   (Response)

            /// <summary>
            /// An event sent whenever a PATCH tariff HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPatchTariffHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PATCH tariff HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PatchTariffHTTPResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

                => OnPatchTariffHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteTariffHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a DELETE tariff HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteTariffHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE tariff HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteTariffHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnDeleteTariffHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteTariffHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a DELETE tariff HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteTariffHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE tariff HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteTariffHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnDeleteTariffHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region Sessions

            #region (protected internal) GetSessionsHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a GET sessions HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetSessionsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET sessions HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetSessionsHTTPRequest(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           CancellationToken  CancellationToken)

                => OnGetSessionsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetSessionsHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a GET sessions HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetSessionsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET sessions HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetSessionsHTTPResponse(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            OCPIResponse       Response,
                                                            CancellationToken  CancellationToken)

                => OnGetSessionsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteSessionsHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a DELETE sessions HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteSessionsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE sessions HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteSessionsHTTPRequest(DateTimeOffset     Timestamp,
                                                              HTTPAPIX           API,
                                                              OCPIRequest        Request,
                                                              CancellationToken  CancellationToken)

                => OnDeleteSessionsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteSessionsHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a DELETE sessions HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteSessionsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE sessions HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteSessionsHTTPResponse(DateTimeOffset     Timestamp,
                                                               HTTPAPIX           API,
                                                               OCPIRequest        Request,
                                                               OCPIResponse       Response,
                                                               CancellationToken  CancellationToken)

                => OnDeleteSessionsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion



            #region (protected internal) GetSessionHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a GET session HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetSessionHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET session HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetSessionHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnGetSessionHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetSessionHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a GET session HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetSessionHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET session HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetSessionHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnGetSessionHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PutSessionHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a PUT session HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPutSessionHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PUT session HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PutSessionHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnPutSessionHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PutSessionHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a PUT session HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPutSessionHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PUT session HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PutSessionHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnPutSessionHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PatchSessionHTTPRequest    (Request)

            /// <summary>
            /// An event sent whenever a PATCH session HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPatchSessionHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a PATCH session HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PatchSessionHTTPRequest(DateTimeOffset     Timestamp,
                                                            HTTPAPIX           API,
                                                            OCPIRequest        Request,
                                                            CancellationToken  CancellationToken)

                => OnPatchSessionHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PatchSessionHTTPResponse   (Response)

            /// <summary>
            /// An event sent whenever a PATCH session HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPatchSessionHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a PATCH session HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PatchSessionHTTPResponse(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             OCPIResponse       Response,
                                                             CancellationToken  CancellationToken)

                => OnPatchSessionHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteSessionHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a DELETE session HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteSessionHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE session HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteSessionHTTPRequest(DateTimeOffset     Timestamp,
                                                             HTTPAPIX           API,
                                                             OCPIRequest        Request,
                                                             CancellationToken  CancellationToken)

                => OnDeleteSessionHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteSessionHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a DELETE session HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteSessionHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE session HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteSessionHTTPResponse(DateTimeOffset     Timestamp,
                                                              HTTPAPIX           API,
                                                              OCPIRequest        Request,
                                                              OCPIResponse       Response,
                                                              CancellationToken  CancellationToken)

                => OnDeleteSessionHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region CDRs

            #region (protected internal) GetCDRsHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a GET CDRs HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetCDRsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET CDRs HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetCDRsHTTPRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

                => OnGetCDRsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetCDRsHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a GET CDRs HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetCDRsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET CDRs HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetCDRsHTTPResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

                => OnGetCDRsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteCDRsHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a DELETE CDRs HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteCDRsHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE CDRs HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteCDRsHTTPRequest(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          CancellationToken  CancellationToken)

                => OnDeleteCDRsHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteCDRsHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a DELETE CDRs HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteCDRsHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE CDRs HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteCDRsHTTPResponse(DateTimeOffset     Timestamp,
                                                           HTTPAPIX           API,
                                                           OCPIRequest        Request,
                                                           OCPIResponse       Response,
                                                           CancellationToken  CancellationToken)

                => OnDeleteCDRsHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion



            #region (protected internal) GetCDRHTTPRequest      (Request)

            /// <summary>
            /// An event sent whenever a GET CDR HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetCDRHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET CDR HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetCDRHTTPRequest(DateTimeOffset     Timestamp,
                                                      HTTPAPIX           API,
                                                      OCPIRequest        Request,
                                                      CancellationToken  CancellationToken)

                => OnGetCDRHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetCDRHTTPResponse     (Response)

            /// <summary>
            /// An event sent whenever a GET CDR HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetCDRHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET CDR HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetCDRHTTPResponse(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       OCPIResponse       Response,
                                                       CancellationToken  CancellationToken)

                => OnGetCDRHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PostCDRHTTPRequest     (Request)

            /// <summary>
            /// An event sent whenever a POST CDR HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPostCDRHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a POST CDR HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PostCDRHTTPRequest(DateTimeOffset     Timestamp,
                                                       HTTPAPIX           API,
                                                       OCPIRequest        Request,
                                                       CancellationToken  CancellationToken)

                => OnPostCDRHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PostCDRHTTPResponse    (Response)

            /// <summary>
            /// An event sent whenever a POST CDR HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPostCDRHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a POST CDR HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PostCDRHTTPResponse(DateTimeOffset     Timestamp,
                                                        HTTPAPIX           API,
                                                        OCPIRequest        Request,
                                                        OCPIResponse       Response,
                                                        CancellationToken  CancellationToken)

                => OnPostCDRHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) DeleteCDRHTTPRequest   (Request)

            /// <summary>
            /// An event sent whenever a DELETE CDR HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnDeleteCDRHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a DELETE CDR HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task DeleteCDRHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnDeleteCDRHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) DeleteCDRHTTPResponse  (Response)

            /// <summary>
            /// An event sent whenever a DELETE CDR HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnDeleteCDRHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a DELETE CDR HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task DeleteCDRHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnDeleteCDRHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion

            #region Tokens

            #region (protected internal) GetTokensHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a GET Tokens HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnGetTokensHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a GET Tokens HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task GetTokensHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnGetTokensHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) GetTokensHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a GET Tokens HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnGetTokensHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a GET Tokens HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task GetTokensHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnGetTokensHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) PostTokenHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a POST token HTTP request was received.
            /// </summary>
            public OCPIRequestLogEvent OnPostTokenHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a POST token HTTP request was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task PostTokenHTTPRequest(DateTimeOffset     Timestamp,
                                                         HTTPAPIX           API,
                                                         OCPIRequest        Request,
                                                         CancellationToken  CancellationToken)

                => OnPostTokenHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) PostTokenHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a POST token HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnPostTokenHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a POST token HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the logging request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task PostTokenHTTPResponse(DateTimeOffset     Timestamp,
                                                          HTTPAPIX           API,
                                                          OCPIRequest        Request,
                                                          OCPIResponse       Response,
                                                          CancellationToken  CancellationToken)

                => OnPostTokenHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion

            #endregion



            // Command callbacks

            #region (protected internal) ReserveNowCallbackHTTPRequest         (Request)

            /// <summary>
            /// An event sent whenever a reserve now callback was received.
            /// </summary>
            public OCPIRequestLogEvent OnReserveNowCallbackHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a reserve now callback was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task ReserveNowCallbackHTTPRequest(DateTimeOffset     Timestamp,
                                                                  HTTPAPIX           API,
                                                                  OCPIRequest        Request,
                                                                  CancellationToken  CancellationToken)

                => OnReserveNowCallbackHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) ReserveNowCallbackHTTPResponse        (Response)

            /// <summary>
            /// An event sent whenever a reserve now callback HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnReserveNowCallbackHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a reserve now callback HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback response.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task ReserveNowCallbackHTTPResponse(DateTimeOffset     Timestamp,
                                                                   HTTPAPIX           API,
                                                                   OCPIRequest        Request,
                                                                   OCPIResponse       Response,
                                                                   CancellationToken  CancellationToken)

                => OnReserveNowCallbackHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) CancelReservationCallbackHTTPRequest  (Request)

            /// <summary>
            /// An event sent whenever a cancel reservation callback was received.
            /// </summary>
            public OCPIRequestLogEvent OnCancelReservationCallbackHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a cancel reservation callback was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task CancelReservationCallbackHTTPRequest(DateTimeOffset     Timestamp,
                                                                         HTTPAPIX           API,
                                                                         OCPIRequest        Request,
                                                                         CancellationToken  CancellationToken)

                => OnCancelReservationCallbackHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) CancelReservationCallbackHTTPResponse (Response)

            /// <summary>
            /// An event sent whenever a cancel reservation callback HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnCancelReservationCallbackHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a cancel reservation callback HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback response.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task CancelReservationCallbackHTTPResponse(DateTimeOffset     Timestamp,
                                                                          HTTPAPIX           API,
                                                                          OCPIRequest        Request,
                                                                          OCPIResponse       Response,
                                                                          CancellationToken  CancellationToken)

                => OnCancelReservationCallbackHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) StartSessionCallbackHTTPRequest       (Request)

            /// <summary>
            /// An event sent whenever a start session callback was received.
            /// </summary>
            public OCPIRequestLogEvent OnStartSessionCallbackHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a start session callback was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task StartSessionCallbackHTTPRequest(DateTimeOffset     Timestamp,
                                                                    HTTPAPIX           API,
                                                                    OCPIRequest        Request,
                                                                    CancellationToken  CancellationToken)

                => OnStartSessionCallbackHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) StartSessionCallbackHTTPResponse      (Response)

            /// <summary>
            /// An event sent whenever a start session callback HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnStartSessionCallbackHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a start session callback HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback response.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task StartSessionCallbackHTTPResponse(DateTimeOffset     Timestamp,
                                                                     HTTPAPIX           API,
                                                                     OCPIRequest        Request,
                                                                     OCPIResponse       Response,
                                                                     CancellationToken  CancellationToken)

                => OnStartSessionCallbackHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) StopSessionCallbackHTTPRequest        (Request)

            /// <summary>
            /// An event sent whenever a stop session callback was received.
            /// </summary>
            public OCPIRequestLogEvent OnStopSessionCallbackHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a stop session callback was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task StopSessionCallbackHTTPRequest(DateTimeOffset     Timestamp,
                                                                   HTTPAPIX           API,
                                                                   OCPIRequest        Request,
                                                                   CancellationToken  CancellationToken)

                => OnStopSessionCallbackHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) StopSessionCallbackHTTPResponse       (Response)

            /// <summary>
            /// An event sent whenever a stop session callback HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnStopSessionCallbackHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a stop session callback HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback response.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task StopSessionCallbackHTTPResponse(DateTimeOffset     Timestamp,
                                                                    HTTPAPIX           API,
                                                                    OCPIRequest        Request,
                                                                    OCPIResponse       Response,
                                                                    CancellationToken  CancellationToken)

                => OnStopSessionCallbackHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

            #endregion


            #region (protected internal) UnlockConnectorCallbackHTTPRequest    (Request)

            /// <summary>
            /// An event sent whenever a unlock connector callback was received.
            /// </summary>
            public OCPIRequestLogEvent OnUnlockConnectorCallbackHTTPRequest = new();

            /// <summary>
            /// An event sent whenever a unlock connector callback was received.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback request.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            protected internal Task UnlockConnectorCallbackHTTPRequest(DateTimeOffset     Timestamp,
                                                                       HTTPAPIX           API,
                                                                       OCPIRequest        Request,
                                                                       CancellationToken  CancellationToken)

                => OnUnlockConnectorCallbackHTTPRequest.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       CancellationToken
                   );

            #endregion

            #region (protected internal) UnlockConnectorCallbackHTTPResponse   (Response)

            /// <summary>
            /// An event sent whenever a unlock connector callback HTTP response was sent.
            /// </summary>
            public OCPIResponseLogEvent OnUnlockConnectorCallbackHTTPResponse = new();

            /// <summary>
            /// An event sent whenever a unlock connector callback HTTP response was sent.
            /// </summary>
            /// <param name="Timestamp">The timestamp of the callback response.</param>
            /// <param name="API">The EMSP API.</param>
            /// <param name="Request">The OCPI request.</param>
            /// <param name="Response">The OCPI response.</param>
            protected internal Task UnlockConnectorCallbackHTTPResponse(DateTimeOffset     Timestamp,
                                                                        HTTPAPIX           API,
                                                                        OCPIRequest        Request,
                                                                        OCPIResponse       Response,
                                                                        CancellationToken  CancellationToken)

                => OnUnlockConnectorCallbackHTTPResponse.WhenAll(
                       Timestamp,
                       API,
                       Request,
                       Response,
                       CancellationToken
                   );

                #endregion


        }


        public delegate Task<AuthorizationInfo> OnRFIDAuthTokenDelegate(Party_Idv3          From,
                                                                        Party_Idv3          To,
                                                                        Token_Id            TokenId,
                                                                        TokenType?          RequestedTokenType,
                                                                        LocationReference?  LocationReference);

        public event OnRFIDAuthTokenDelegate? OnRFIDAuthToken;



        #region Domain Events

        #region OnPutLocation    (Request/-Response)

        /// <summary>
        /// An event fired whenever a PUT Location request was received.
        /// </summary>
        public event OnPutLocationRequestDelegate2?   OnPutLocationRequest;

        /// <summary>
        /// An event fired whenever a response to a PUT Location request had been sent.
        /// </summary>
        public event OnPutLocationResponseDelegate2?  OnPutLocationResponse;

        #endregion

        #region OnPatchLocation  (Request/-Response)

        /// <summary>
        /// An event fired whenever a PATCH Location request was received.
        /// </summary>
        public event OnPatchLocationRequestDelegate2?   OnPatchLocationRequest;

        /// <summary>
        /// An event fired whenever a response to a PATCH Location request had been sent.
        /// </summary>
        public event OnPatchLocationResponseDelegate2?  OnPatchLocationResponse;

        #endregion

        #region OnPutEVSE        (Request/-Response)

        /// <summary>
        /// An event fired whenever a PUT EVSE request was received.
        /// </summary>
        public event OnPutEVSERequestDelegate2?   OnPutEVSERequest;

        /// <summary>
        /// An event fired whenever a response to a PUT EVSE request had been sent.
        /// </summary>
        public event OnPutEVSEResponseDelegate2?  OnPutEVSEResponse;

        #endregion

        #region OnPatchEVSE      (Request/-Response)

        /// <summary>
        /// An event fired whenever a PATCH EVSE request was received.
        /// </summary>
        public event OnPatchEVSERequestDelegate2?   OnPatchEVSERequest;

        /// <summary>
        /// An event fired whenever a response to a PATCH EVSE request had been sent.
        /// </summary>
        public event OnPatchEVSEResponseDelegate2?  OnPatchEVSEResponse;

        #endregion

        #region OnPutConnector   (Request/-Response)

        /// <summary>
        /// An event fired whenever a PUT Connector request was received.
        /// </summary>
        public event OnPutConnectorRequestDelegate2?   OnPutConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to a PUT Connector request had been sent.
        /// </summary>
        public event OnPutConnectorResponseDelegate2?  OnPutConnectorResponse;

        #endregion

        #region OnPatchConnector (Request/-Response)

        /// <summary>
        /// An event fired whenever a PATCH Connector request was received.
        /// </summary>
        public event OnPatchConnectorRequestDelegate2?   OnPatchConnectorRequest;

        /// <summary>
        /// An event fired whenever a response to a PATCH Connector request had been sent.
        /// </summary>
        public event OnPatchConnectorResponseDelegate2?  OnPatchConnectorResponse;

        #endregion


        #region OnPutTariff      (Request/-Response)

        /// <summary>
        /// An event fired whenever a PUT Tariff request was received.
        /// </summary>
        public event OnPutTariffRequestDelegate2?   OnPutTariffRequest;

        /// <summary>
        /// An event fired whenever a response to a PUT Tariff request had been sent.
        /// </summary>
        public event OnPutTariffResponseDelegate2?  OnPutTariffResponse;

        #endregion


        #region OnPutSession     (Request/-Response)

        /// <summary>
        /// An event fired whenever a PUT Session request was received.
        /// </summary>
        public event OnPutSessionRequestDelegate2?   OnPutSessionRequest;

        /// <summary>
        /// An event fired whenever a response to a PUT Session request had been sent.
        /// </summary>
        public event OnPutSessionResponseDelegate2?  OnPutSessionResponse;

        #endregion

        #region OnPatchSession   (Request/-Response)

        /// <summary>
        /// An event fired whenever a PATCH Session request was received.
        /// </summary>
        public event OnPatchSessionRequestDelegate2?   OnPatchSessionRequest;

        /// <summary>
        /// An event fired whenever a response to a PATCH Session request had been sent.
        /// </summary>
        public event OnPatchSessionResponseDelegate2?  OnPatchSessionResponse;

        #endregion


        #region OnPostCDR        (Request/-Response)

        /// <summary>
        /// An event fired whenever a POST CDR request was received.
        /// </summary>
        public event OnPostCDRRequestDelegate2?   OnPostCDRRequest;

        /// <summary>
        /// An event fired whenever a response to a POST CDR request had been sent.
        /// </summary>
        public event OnPostCDRResponseDelegate2?  OnPostCDRResponse;

        #endregion


        #region OnPostToken      (Request/-Response)

        /// <summary>
        /// An event fired whenever a POST Token request was received.
        /// </summary>
        public event OnPostTokenRequestDelegate2?   OnPostTokenRequest;

        /// <summary>
        /// An event fired whenever a response to a POST Token request had been sent.
        /// </summary>
        public event OnPostTokenResponseDelegate2?  OnPostTokenResponse;

        #endregion

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the EMSP HTTP API for e-mobility service providers
        /// using the given CommonAPI.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public EMSP_HTTPAPI(CommonAPI                    CommonAPI,
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

                   Description     ?? I18NString.Create($"OCPI{Version.String} EMSP HTTP API"),

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

            this.Counters         = new APICounters();

            this.HTTPLogger       = this.DisableLogging == false
                                        ? new EMSP_HTTPAPI_Logger(
                                              this,
                                              LoggingContext,
                                              LoggingPath,
                                              LogfileCreator
                                          )
                                        : null;

            RegisterURLTemplates();

        }

        #endregion


        #region EMSP-2-CPO Clients

        private readonly ConcurrentDictionary<CPO_Id, EMSP2CPOClient> emsp2cpoClients = new();

        /// <summary>
        /// Return an enumeration of all EMSP2CPO clients.
        /// </summary>
        public IEnumerable<EMSP2CPOClient> EMSP2CPOClients
            => emsp2cpoClients.Values;


        #region GetCPOClient (CountryCode, PartyId,   Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote CPO.</param>
        /// <param name="PartyId">The party identification of the remote CPO.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public EMSP2CPOClient? GetCPOClient(CountryCode  CountryCode,
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
        /// As a EMSP create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="PartyId">The party identification of the remote CPO.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public EMSP2CPOClient? GetCPOClient(Party_Idv3   PartyId,
                                            I18NString?  Description          = null,
                                            Boolean      AllowCachedClients   = true)

            => GetCPOClient(
                   RemoteParty_Id.From(
                       PartyId,
                       Role.CPO
                   ),
                   Description,
                   AllowCachedClients
               );

        #endregion

        #region GetCPOClient (RemoteParty,            Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSP2CPOClient? GetCPOClient(RemoteParty  RemoteParty,
                                            I18NString?  Description          = null,
                                            Boolean      AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                emsp2cpoClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emsp2CPOClient = new EMSP2CPOClient(
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

                emsp2cpoClients.TryAdd(cpoId, emsp2CPOClient);

                return emsp2CPOClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient (RemotePartyId,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote CPO.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSP2CPOClient? GetCPOClient(RemoteParty_Id  RemotePartyId,
                                            I18NString?     Description          = null,
                                            Boolean         AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                emsp2cpoClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (CommonAPI.TryGetRemoteParty(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emsp2CPOClient = new EMSP2CPOClient(
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

                emsp2cpoClients.TryAdd(cpoId, emsp2CPOClient);

                return emsp2CPOClient;

            }

            return null;

        }

        #endregion

        #endregion

        #region CloseAllClients()

        public Task CloseAllClients()
        {

            foreach (var client in emsp2cpoClients.Values)
            {
                client.Close();
            }

            emsp2cpoClients.Clear();

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
                       $"The party identification '{PartyId}' of the remote charging location is unknown!"
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
                       $"The party identification '{PartyId}' of the remote charging tariff is unknown!"
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
                                    RemoteCDR.CountryCode,
                                    RemoteCDR.PartyId
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

            // ---------------------

            //var localPartyId  = Party_Idv3.From(
            //                        RemoteCDR.CDRToken.CountryCode,
            //                        RemoteCDR.CDRToken.PartyId
            //                    );

            //await CommonAPI.AddCDR(RemoteCDR);

            // --------------------

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
                                    RemoteCDR.CountryCode,
                                    RemoteCDR.PartyId
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
                                    RemoteCDR.CountryCode,
                                    RemoteCDR.PartyId
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
                                    RemoteCDR.CountryCode,
                                    RemoteCDR.PartyId
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

            // Receiver Interface for eMSPs and NSPs

            #region ~/locations/{country_code}/{party_id}                                [NonStandard]

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
                HTTPEvents.GetLocationsHTTPRequest,
                HTTPEvents.GetLocationsHTTPResponse,
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
                HTTPEvents.DeleteLocationsHTTPRequest,
                HTTPEvents.DeleteLocationsHTTPResponse,
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
                HTTPEvents.GetLocationHTTPRequest,
                HTTPEvents.GetLocationHTTPResponse,
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
                HTTPEvents.PutLocationHTTPRequest,
                HTTPEvents.PutLocationHTTPResponse,
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


                    #region Send OnPutLocationRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PutLocation.IncRequests_OK();

                    await LogEvent(
                              OnPutLocationRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedLocation,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    var addOrUpdateResult = await AddOrUpdateRemoteLocation(
                                                      newOrUpdatedLocation,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    #region Send OnPutLocationResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPutLocationResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedLocation,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


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
                HTTPEvents.PatchLocationHTTPRequest,
                HTTPEvents.PatchLocationHTTPResponse,
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


                    #region Send OnPatchLocationRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PatchLocation.IncRequests_OK();

                    await LogEvent(
                              OnPatchLocationRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  locationId.Value,
                                  locationPatch,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

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


                    #region Send OnPatchLocationResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPatchLocationResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  locationId.Value,
                                  locationPatch,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


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
                HTTPEvents.DeleteLocationHTTPRequest,
                HTTPEvents.DeleteLocationHTTPResponse,
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
                HTTPEvents.GetEVSEHTTPRequest,
                HTTPEvents.GetEVSEHTTPResponse,
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
                HTTPEvents.PutEVSEHTTPRequest,
                HTTPEvents.PutEVSEHTTPResponse,
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


                    #region Send OnPutEVSERequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PutEVSE.IncRequests_OK();

                    await LogEvent(
                              OnPutEVSERequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedEVSE,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    var addOrUpdateResult = await AddOrUpdateRemoteEVSE(
                                                      existingLocation,
                                                      newOrUpdatedEVSE,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    #region Send OnPutEVSEResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPutEVSEResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedEVSE,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


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
                HTTPEvents.PatchEVSEHTTPRequest,
                HTTPEvents.PatchEVSEHTTPResponse,
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


                    #region Send OnPatchEVSERequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PatchEVSE.IncRequests_OK();

                    await LogEvent(
                              OnPatchEVSERequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  evseUId.Value,
                                  evsePatch,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    var patchedEVSE = await TryPatchRemoteEVSE(
                                                existingLocation,
                                                existingEVSE,
                                                evsePatch
                                            );


                    #region Send OnPatchEVSEResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPatchEVSEResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  evseUId.Value,
                                  evsePatch,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


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
                HTTPEvents.DeleteEVSEHTTPRequest,
                HTTPEvents.DeleteEVSEHTTPResponse,
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


                    //Remove(ExistingLocation, ExistingEVSE);


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
                HTTPEvents.GetConnectorHTTPRequest,
                HTTPEvents.GetConnectorHTTPResponse,
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
                HTTPEvents.PutConnectorHTTPRequest,
                HTTPEvents.PutConnectorHTTPResponse,
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


                    #region Send OnPutConnectorRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PutConnector.IncRequests_OK();

                    await LogEvent(
                              OnPutConnectorRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedConnector,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    var addOrUpdateResult = await AddOrUpdateRemoteConnector(
                                                      existingLocation,
                                                      existingEVSE,
                                                      newOrUpdatedConnector,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    #region Send OnPutConnectorResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPutConnectorResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedConnector,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion



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
                HTTPEvents.PatchConnectorHTTPRequest,
                HTTPEvents.PatchConnectorHTTPResponse,
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


                    #region Send OnPatchConnectorRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PatchConnector.IncRequests_OK();

                    await LogEvent(
                              OnPatchConnectorRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  connectorId.Value,
                                  connectorPatch,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    var patchedConnector = await TryPatchRemoteConnector(
                                                     existingLocation,
                                                     existingEVSE,
                                                     existingConnector,
                                                     connectorPatch
                                                 );


                    #region Send OnPatchConnectorResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPatchConnectorResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  connectorId.Value,
                                  connectorPatch,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


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
                HTTPEvents.DeleteConnectorHTTPRequest,
                HTTPEvents.DeleteConnectorHTTPResponse,
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


                    //Remove(ExistingLocation, ExistingEVSE);


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
                HTTPEvents.PutEVSEHTTPRequest,
                HTTPEvents.PutEVSEHTTPResponse,
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



            #region ~/tariffs/{country_code}/{party_id}                                  [NonStandard]

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
                HTTPEvents.GetTariffsHTTPRequest,
                HTTPEvents.GetTariffsHTTPResponse,
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
                HTTPEvents.DeleteTariffsHTTPRequest,
                HTTPEvents.DeleteTariffsHTTPResponse,
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
                HTTPEvents.GetTariffHTTPRequest,
                HTTPEvents.GetTariffHTTPResponse,
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
                HTTPEvents.PutTariffHTTPRequest,
                HTTPEvents.PutTariffHTTPResponse,
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


                    #region Send OnPutTariffRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PutTariff.IncRequests_OK();

                    await LogEvent(
                              OnPutTariffRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedTariff,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    var addOrUpdateResult = await AddOrUpdateRemoteTariff(
                                                      newOrUpdatedTariff,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    #region Send OnPutTariffResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPutTariffResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedTariff,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


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
                HTTPEvents.PatchTariffHTTPRequest,
                HTTPEvents.PatchTariffHTTPResponse,
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
                HTTPEvents.DeleteTariffHTTPRequest,
                HTTPEvents.DeleteTariffHTTPResponse,
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



            #region ~/sessions/{country_code}/{party_id}                                 [NonStandard]

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
                HTTPEvents.GetSessionsHTTPRequest,
                HTTPEvents.GetSessionsHTTPResponse,
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

                    var allSessions       = GetRemoteSessions(session => request.LocalAccessInfo.Roles.Any(role => role.PartyId.CountryCode == session.CountryCode &&
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
                HTTPEvents.GetSessionsHTTPRequest,
                HTTPEvents.GetSessionsHTTPResponse,
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

                    var allSessions       = GetRemoteSessions(partyId.Value).ToArray();

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
                HTTPEvents.DeleteSessionsHTTPRequest,
                HTTPEvents.DeleteSessionsHTTPResponse,
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
                        await RemoveAllRemoteSessions(role.PartyId);


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
                HTTPEvents.DeleteSessionsHTTPRequest,
                HTTPEvents.DeleteSessionsHTTPResponse,
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


                    var result = await RemoveAllRemoteSessions(partyId.Value);


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
                HTTPEvents.GetSessionHTTPRequest,
                HTTPEvents.GetSessionHTTPResponse,
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
                HTTPEvents.PutSessionHTTPRequest,
                HTTPEvents.PutSessionHTTPResponse,
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


                    #region Send OnPutSessionRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PutSession.IncRequests_OK();

                    await LogEvent(
                              OnPutSessionRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedSession,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    var addOrUpdateResult = await AddOrUpdateRemoteSession(
                                                      newOrUpdatedSession,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade")
                                                  );


                    #region Send OnPutSessionResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPutSessionResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newOrUpdatedSession,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


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
                HTTPEvents.PatchSessionHTTPRequest,
                HTTPEvents.PatchSessionHTTPResponse,
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


                    #region Send OnPatchSessionRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PatchSession.IncRequests_OK();

                    await LogEvent(
                              OnPatchSessionRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  existingSession.Id,
                                  sessionPatch,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    var patchedSession = await TryPatchRemoteSession(
                                                   Party_Idv3.From(
                                                       countryCode.Value,
                                                       partyId.    Value
                                                   ),
                                                   existingSession.Id,
                                                   sessionPatch
                                               );


                    #region Send OnPatchSessionResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPatchSessionResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  existingSession.Id,
                                  sessionPatch,

                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


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
                HTTPEvents.DeleteSessionHTTPRequest,
                HTTPEvents.DeleteSessionHTTPResponse,
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
                    var result = await RemoveRemoteSession(existingSession);


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



            #region ~/cdrs/{country_code}/{party_id}                                     [NonStandard]

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
                URLPathPrefix + "cdrs",
                HTTPEvents.GetCDRsHTTPRequest,
                HTTPEvents.GetCDRsHTTPResponse,
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

                    var allCDRs       = GetRemoteCDRs(session => request.LocalAccessInfo.Roles.Any(role => role.PartyId.CountryCode == session.CountryCode &&
                                                                                                           role.PartyId.PartyId     == session.PartyId)).
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
                HTTPEvents.GetCDRsHTTPRequest,
                HTTPEvents.GetCDRsHTTPResponse,
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

                    var allCDRs       = GetRemoteCDRs(partyId.Value).ToArray();

                    var filteredCDRs  = GetRemoteCDRs().
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

            #region POST     ~/cdrs                                 <= Unclear if this URL is correct!

            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "cdrs",
                HTTPEvents.PostCDRHTTPRequest,
                HTTPEvents.PostCDRHTTPResponse,
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
                                        AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
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


                    #region Send OnPostCDRRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PostCDR.IncRequests_OK();

                    await LogEvent(
                              OnPostCDRRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newCDR,

                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    //ToDo: How do we verify, that this CPO does not send CDRs for other CPOs?


                    // ToDo: What kind of error might happen here?
                    var addResult    = await AddRemoteCDR(newCDR);

                                       // ~/cdrs/{country_code}/{party_id}/{cdrId}
                    var cdrLocation  = Hermod.Location.From(
                                           URLPathPrefix + "cdrs" +
                                           newCDR.CountryCode.ToString() +
                                           newCDR.PartyId.    ToString() +
                                           newCDR.Id.         ToString()
                                       );


                    #region Send OnPostCDRResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPostCDRResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.CPOId?.CountryCode,
                                  request.CPOId?.PartyId,
                                  request.To?.CountryCode,
                                  request.To?.PartyId,

                                  newCDR,

                                  cdrLocation,
                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    // https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/mod_cdrs.asciidoc#mod_cdrs_post_method
                    // The response should contain the URL to the just created CDR object in the eMSP’s system.
                    //
                    // Parameter    Location
                    // Datatype     URL
                    // Required     yes
                    // Description  URL to the newly created CDR in the eMSP’s system, can be used by the CPO system to perform a GET on the same CDR.
                    // Example      https://www.server.com/ocpi/emsp/2.2.1/cdrs/123456

                    if (addResult.IsSuccess)
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
                                            Location                   = cdrLocation,
                                            AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                            AccessControlAllowHeaders  = [ "Authorization" ],
                                            LastModified               = newCDR.LastUpdated,
                                            ETag                       = newCDR.ETag
                                        }
                                    };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addResult.ErrorResponse,
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
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "POST", "DELETE" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                });

            #endregion

            #region DELETE   ~/cdrs                                     [NonStandard]

            CommonAPI.AddOCPIMethod(

                HTTPMethod.DELETE,
                URLPathPrefix + "cdrs",
                HTTPEvents.DeleteCDRsHTTPRequest,
                HTTPEvents.DeleteCDRsHTTPResponse,
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
                        await RemoveAllRemoteCDRs(role.PartyId);


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
                HTTPEvents.DeleteCDRsHTTPRequest,
                HTTPEvents.DeleteCDRsHTTPResponse,
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


                    var result = await RemoveAllRemoteCDRs(partyId.Value);


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
                HTTPEvents.GetCDRHTTPRequest,
                HTTPEvents.GetCDRHTTPResponse,
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
                HTTPEvents.DeleteCDRHTTPRequest,
                HTTPEvents.DeleteCDRHTTPResponse,
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
                    await RemoveRemoteCDR(existingCDR);


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
                HTTPEvents.GetTokensHTTPRequest,
                HTTPEvents.GetTokensHTTPResponse,
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
            // https://example.com/ocpi/2.2.1/emsp/tokens/012345678/authorize?type=RFID
            // curl -X POST http://127.0.0.1:3000/2.2.1/emsp/tokens/012345678/authorize?type=RFID
            CommonAPI.AddOCPIMethod(

                HTTPMethod.POST,
                URLPathPrefix + "tokens/{token_id}/authorize",
                HTTPEvents.PostTokenHTTPRequest,
                HTTPEvents.PostTokenHTTPResponse,
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

                    #region Check TokenId URI parameter

                    if (!request.ParseTokenId(out var tokenId,
                                              out var ocpiResponseBuilder))
                    {

                        Counters.PostToken.IncRequests_Error();

                        return ocpiResponseBuilder;

                    }

                    #endregion

                    var requestedTokenType = request.QueryString.Map("type", TokenType.TryParse);

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

                            Counters.PostToken.IncRequests_Error();

                            return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given location reference JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                        }

                        locationReference = _locationReference;

                    }

                    #endregion


                    #region Send OnTokenAuthorizeRequest event

                    var startTime  = Timestamp.Now;
                    var stopwatch  = Stopwatch.StartNew();

                    Counters.PostToken.IncRequests_OK();

                    await LogEvent(
                              OnPostTokenRequest,
                              loggingDelegate => loggingDelegate.Invoke(
                                  startTime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.From?.CountryCode ?? request.CPOId?.CountryCode,
                                  request.From?.PartyId     ?? request.CPOId?.PartyId,
                                  request.To?.  CountryCode,
                                  request.To?.  PartyId,
                                  tokenId.Value,
                                  requestedTokenType,
                                  locationReference,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    #region OnRFIDAuthToken...

                    AuthorizationInfo? authorizationInfo = null;

                    var onRFIDAuthTokenLocal = OnRFIDAuthToken;
                    if (onRFIDAuthTokenLocal is not null)
                    {

                        try
                        {

                            var result = await onRFIDAuthTokenLocal(
                                                   Party_Idv3.From(
                                                       request.From?.CountryCode ?? CommonAPI.DefaultPartyId.CountryCode,
                                                       request.From?.PartyId     ?? CommonAPI.DefaultPartyId.PartyId
                                                   ),
                                                   Party_Idv3.From(
                                                       request.To?.  CountryCode ?? CommonAPI.DefaultPartyId.CountryCode,
                                                       request.To?.  PartyId     ?? CommonAPI.DefaultPartyId.PartyId
                                                   ),
                                                   tokenId.Value,
                                                   requestedTokenType,
                                                   locationReference
                                               );

                            authorizationInfo = result;

                        }
                        catch (Exception e)
                        {

                            Counters.PostToken.IncResponses_Error();

                            authorizationInfo = new AuthorizationInfo(
                                                    AllowedType.NOT_ALLOWED,
                                                    new Token(
                                                        CommonAPI.DefaultPartyId.CountryCode,
                                                        CommonAPI.DefaultPartyId.PartyId,
                                                        tokenId.Value,
                                                        requestedTokenType ?? TokenType.RFID,
                                                        Contract_Id.Parse($"{CommonAPI.DefaultPartyId.ToString(Role.EMSP)}-{tokenId}"),
                                                        $"Could not call {nameof(EMSP_HTTPAPI)}.OnRFIDAuthToken(...): {e.Message}",
                                                        false,
                                                        WhitelistType.NEVER
                                                    )
                                                );

                        }

                    }

                    #endregion

                    #region ...or check local

                    else
                    {

                        #region Check existing token

                        if (!CommonAPI.TryGetTokenStatus(
                                request.To ?? CommonAPI.DefaultPartyId,
                                tokenId.Value,
                                out var _tokenStatus) ||
                            (_tokenStatus.Token.Type != requestedTokenType))
                        {

                            return new OCPIResponse.Builder(request) {
                                       StatusCode           = 2004,
                                       StatusMessage        = "Unknown token!",
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.NotFound,
                                           AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ]
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

                            Location? validLocation = null;

                            if (request.From.HasValue)
                            {

                                if (!CommonAPI.TryGetLocation(
                                    request.From.Value,
                                    locationReference.Value.LocationId,
                                    out validLocation))
                                {

                                    return new OCPIResponse.Builder(request) {
                                               StatusCode           = 2001,
                                               StatusMessage        = "The given location is unknown!",
                                               Data                 = new AuthorizationInfo(
                                                                          AllowedType.NOT_ALLOWED,
                                                                          _tokenStatus.Token,
                                                                          locationReference.Value,
                                                                          null,
                                                                          new DisplayText(Languages.en, "The given location is unknown!")
                                                                      ).ToJSON(
                                                                            CustomAuthorizationInfoSerializer,
                                                                            CustomTokenSerializer,
                                                                            CustomLocationReferenceSerializer,
                                                                            CustomDisplayTextSerializer
                                                                        ),
                                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                               }
                                           };

                                }

                            }

                            else
                            {

                                if (request.LocalAccessInfo.Roles.Where(role => role.Role == Role.CPO).Count() != 1)
                                {

                                    return new OCPIResponse.Builder(request) {
                                        StatusCode           = 2001,
                                        StatusMessage        = "Could not determine the country code and party identification of the given location!",
                                        Data                 = new AuthorizationInfo(
                                                                   AllowedType.NOT_ALLOWED,
                                                                   _tokenStatus.Token,
                                                                   locationReference.Value
                                                               ).ToJSON(
                                                                     CustomAuthorizationInfoSerializer,
                                                                     CustomTokenSerializer,
                                                                     CustomLocationReferenceSerializer,
                                                                     CustomDisplayTextSerializer
                                                                 ),
                                        HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                            AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                            AccessControlAllowHeaders  = [ "Authorization" ]
                                        }
                                    };

                                }

                                var allTheirCPORoles = request.LocalAccessInfo.Roles.Where(role => role.Role == Role.CPO).ToArray();

                                if (!CommonAPI.TryGetLocation(
                                    allTheirCPORoles[0].PartyId,
                                    locationReference.Value.LocationId,
                                    out validLocation))
                                {

                                    return new OCPIResponse.Builder(request) {
                                               StatusCode           = 2001,
                                               StatusMessage        = "The given location is unknown!",
                                               Data                 = new AuthorizationInfo(
                                                                          AllowedType.NOT_ALLOWED,
                                                                          _tokenStatus.Token,
                                                                          locationReference.Value,
                                                                          null,
                                                                          new DisplayText(Languages.en, "The given location is unknown!")
                                                                      ).ToJSON(
                                                                            CustomAuthorizationInfoSerializer,
                                                                            CustomTokenSerializer,
                                                                            CustomLocationReferenceSerializer,
                                                                            CustomDisplayTextSerializer
                                                                        ),
                                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                               }
                                           };

                                }

                            }


                            //ToDo: Add a event/delegate for additional location filters!


                            if (locationReference.Value.EVSEUIds.SafeAny())
                            {

                                locationReference = new LocationReference(locationReference.Value.LocationId,
                                                                          locationReference.Value.EVSEUIds.
                                                                                                  Where(evseuid => validLocation.EVSEExists(evseuid)));

                                if (!locationReference.Value.EVSEUIds.SafeAny())
                                {

                                    return new OCPIResponse.Builder(request) {
                                               StatusCode           = 2001,
                                               StatusMessage        = locationReference.Value.EVSEUIds.Count() == 1
                                                                          ? "The EVSE at the given location is unknown!"
                                                                          : "The EVSEs at the given location are unknown!",
                                               Data                 = new AuthorizationInfo(
                                                                          AllowedType.NOT_ALLOWED,
                                                                          _tokenStatus.Token,
                                                                          locationReference.Value,
                                                                          null,
                                                                          new DisplayText(
                                                                              Languages.en,
                                                                              locationReference.Value.EVSEUIds.Count() == 1
                                                                                  ? "The EVSE at the given location is unknown!"
                                                                                  : "The EVSEs at the given location are unknown!"
                                                                          )
                                                                      ).ToJSON(
                                                                            CustomAuthorizationInfoSerializer,
                                                                            CustomTokenSerializer,
                                                                            CustomLocationReferenceSerializer,
                                                                            CustomDisplayTextSerializer
                                                                        ),
                                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                               }
                                           };

                                }



                                //ToDo: Add a event/delegate for additional EVSE filters!

                            }

                        }

                        #endregion

                    }

                    authorizationInfo ??= new AuthorizationInfo(
                                              AllowedType.NOT_ALLOWED,
                                              new Token(
                                                  CommonAPI.DefaultPartyId.CountryCode,
                                                  CommonAPI.DefaultPartyId.PartyId,
                                                  tokenId.Value,
                                                  requestedTokenType ?? TokenType.RFID,
                                                  Contract_Id.Parse($"{CommonAPI.DefaultPartyId.ToString(Role.EMSP)}-{tokenId}"),
                                                  "Internal Error!",
                                                  false,
                                                  WhitelistType.NEVER
                                              )
                                          );

                    #endregion

                    // too little information like e.g. no LocationReferences provided:
                    //   => status_code 2002

                    #region Set a user-friendly response message for the ev driver

                    var responseText = "An error occurred!";

                    if (!authorizationInfo.Info.HasValue)
                    {

                        #region ALLOWED

                        if (authorizationInfo.Allowed == AllowedType.ALLOWED)
                        {

                            Counters.PostToken.IncResponses_OK();

                            responseText = "Charging allowed!";

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

                            Counters.PostToken.IncResponses_OK();

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

                            Counters.PostToken.IncResponses_OK();

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

                            Counters.PostToken.IncResponses_OK();

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

                            Counters.PostToken.IncResponses_OK();

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

                            Counters.PostToken.IncResponses_Error();

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

                    var authorizationInfo2 = new AuthorizationInfo(
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
                                             );

                    #endregion


                    #region Send OnTokenAuthorizeResponse event

                    var endtime = Timestamp.Now;
                    stopwatch.Stop();

                    await LogEvent(
                              OnPostTokenResponse,
                              loggingDelegate => loggingDelegate.Invoke(
                                  endtime,
                                  this,
                                  request.HTTPRequest.EventTrackingId,
                                  request.From?.CountryCode ?? request.CPOId?.CountryCode,
                                  request.From?.PartyId     ?? request.CPOId?.PartyId,
                                  request.To?.  CountryCode,
                                  request.To?.  PartyId,
                                  tokenId.Value,
                                  requestedTokenType,
                                  locationReference,
                                  authorizationInfo2,
                                  stopwatch.Elapsed,
                                  request.HTTPRequest.CancellationToken
                              )
                          );

                    #endregion


                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = authorizationInfo2.ToJSON(
                                                          CustomAuthorizationInfoSerializer,
                                                          CustomTokenSerializer,
                                                          CustomLocationReferenceSerializer,
                                                          CustomDisplayTextSerializer
                                                      ),
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
                HTTPEvents.ReserveNowCallbackHTTPRequest,
                HTTPEvents.ReserveNowCallbackHTTPResponse,
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
                HTTPEvents.CancelReservationCallbackHTTPRequest,
                HTTPEvents.CancelReservationCallbackHTTPResponse,
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
                HTTPEvents.StartSessionCallbackHTTPRequest,
                HTTPEvents.StartSessionCallbackHTTPResponse,
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
                HTTPEvents.StopSessionCallbackHTTPRequest,
                HTTPEvents.StopSessionCallbackHTTPResponse,
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
                HTTPEvents.UnlockConnectorCallbackHTTPRequest,
                HTTPEvents.UnlockConnectorCallbackHTTPResponse,
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


        public void LinkEventsToHTTPSSE(HTTPEventSource<JObject> HTTPSSE)
        {
            EventsToJSON(
                //async (txt, json, ct) => await HTTPSSE.SubmitEvent(txt, json, ct)
                HTTPSSE.SubmitEvent
            );
        }

        public void EventsToJSON(Func<String, JObject, CancellationToken, Task> Processor)
        {

            #region OnPutLocationRequest

            OnPutLocationRequest += async (timestamp,
                                           sender,
                                           eventTrackingId,
                                           from_CountryCode,
                                           from_PartyId,
                                           to_CountryCode,
                                           to_PartyId,

                                           location,
                                           cancellationToken) => {

                await Processor(
                    "OnPutLocationRequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("location",   location.ToJSON(
                                                              null, //EMSPId
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
                                                              true //IncludeCreatedTimestamp
                                                          ))

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutLocationResponse

            OnPutLocationResponse += async (timestamp,
                                            sender,
                                            eventTrackingId,
                                            from_CountryCode,
                                            from_PartyId,
                                            to_CountryCode,
                                            to_PartyId,

                                            location,
                                            runtime,
                                            cancellationToken) => {

                await Processor(
                    "OnPutLocationResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",       $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",         $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("location",   location.ToJSON(
                                                              null, //EMSPId
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
                                                              true //IncludeCreatedTimestamp
                                                         )),

                              new JProperty("runtime",   runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchLocationRequest

            OnPatchLocationRequest += async (timestamp,
                                             sender,
                                             eventTrackingId,
                                             from_CountryCode,
                                             from_PartyId,
                                             to_CountryCode,
                                             to_PartyId,

                                             locationId,
                                             locationPatch,
                                             cancellationToken) => {

                await Processor(
                    "OnPatchLocationRequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",            $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",              $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("locationId",      locationId.ToString()),
                              new JProperty("locationPatch",   locationPatch)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchLocationResponse

            OnPatchLocationResponse += async (timestamp,
                                              sender,
                                              eventTrackingId,
                                              from_CountryCode,
                                              from_PartyId,
                                              to_CountryCode,
                                              to_PartyId,

                                              locationId,
                                              locationPatch,
                                              runtime,
                                              cancellationToken) => {

                await Processor(
                    "OnPatchLocationResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",            $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",              $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("locationId",      locationId.ToString()),
                              new JProperty("locationPatch",   locationPatch),

                              new JProperty("runtime",         runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPutEVSERequest

            OnPutEVSERequest += async (timestamp,
                                       sender,
                                       eventTrackingId,
                                       from_CountryCode,
                                       from_PartyId,
                                       to_CountryCode,
                                       to_PartyId,

                                       evse,
                                       cancellationToken) => {

                await Processor(
                    "OnPutEVSERequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",   $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",     $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("evse",   evse.ToJSON(
                                                          null, //EMSPId
                                                          CustomEVSESerializer,
                                                          CustomStatusScheduleSerializer,
                                                          CustomConnectorSerializer,
                                                          CustomEVSEEnergyMeterSerializer,
                                                          CustomTransparencySoftwareStatusSerializer,
                                                          CustomTransparencySoftwareSerializer,
                                                          CustomDisplayTextSerializer,
                                                          CustomImageSerializer
                                                      ))

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutEVSEResponse

            OnPutEVSEResponse += async (timestamp,
                                        sender,
                                        eventTrackingId,
                                        from_CountryCode,
                                        from_PartyId,
                                        to_CountryCode,
                                        to_PartyId,

                                        evse,
                                        runtime,
                                        cancellationToken) => {

                await Processor(
                    "OnPutEVSEResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",      $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",        $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("evse",      evse.ToJSON(
                                                             null, //EMSPId
                                                             CustomEVSESerializer,
                                                             CustomStatusScheduleSerializer,
                                                             CustomConnectorSerializer,
                                                             CustomEVSEEnergyMeterSerializer,
                                                             CustomTransparencySoftwareStatusSerializer,
                                                             CustomTransparencySoftwareSerializer,
                                                             CustomDisplayTextSerializer,
                                                             CustomImageSerializer
                                                         )),

                              new JProperty("runtime",   runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchEVSERequest

            OnPatchEVSERequest += async (timestamp,
                                         sender,
                                         eventTrackingId,
                                         from_CountryCode,
                                         from_PartyId,
                                         to_CountryCode,
                                         to_PartyId,

                                         evseId,
                                         evsePatch,
                                         cancellationToken) => {

                await Processor(
                    "OnPatchEVSERequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",        $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",          $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("evseId",      evseId.ToString()),
                              new JProperty("evsePatch",   evsePatch)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchEVSEResponse

            OnPatchEVSEResponse += async (timestamp,
                                          sender,
                                          eventTrackingId,
                                          from_CountryCode,
                                          from_PartyId,
                                          to_CountryCode,
                                          to_PartyId,

                                          evseId,
                                          evsePatch,
                                          runtime,
                                          cancellationToken) => {

                await Processor(
                    "OnPatchEVSEResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",        $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",          $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("evseId",      evseId.ToString()),
                              new JProperty("evsePatch",   evsePatch),

                              new JProperty("runtime",     runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPutConnectorRequest

            OnPutConnectorRequest += async (timestamp,
                                            sender,
                                            eventTrackingId,
                                            from_CountryCode,
                                            from_PartyId,
                                            to_CountryCode,
                                            to_PartyId,

                                            connector,
                                            cancellationToken) => {

                await Processor(
                    "OnPutConnectorRequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",        $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",          $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("connector",   connector.ToJSON(
                                                               true, //IncludeCreatedTimestamp
                                                               true, //IncludeExtensions
                                                               null, //EMSPId
                                                               CustomConnectorSerializer
                                                           ))

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutConnectorResponse

            OnPutConnectorResponse += async (timestamp,
                                             sender,
                                             eventTrackingId,
                                             from_CountryCode,
                                             from_PartyId,
                                             to_CountryCode,
                                             to_PartyId,

                                             connector,
                                             runtime,
                                             cancellationToken) => {

                await Processor(
                    "OnPutConnectorResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",        $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",          $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("connector",   connector.ToJSON(
                                                               true, //IncludeCreatedTimestamp
                                                               true, //IncludeExtensions
                                                               null, //EMSPId
                                                               CustomConnectorSerializer
                                                           )),

                              new JProperty("runtime",     runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchConnectorRequest

            OnPatchConnectorRequest += async (timestamp,
                                              sender,
                                              eventTrackingId,
                                              from_CountryCode,
                                              from_PartyId,
                                              to_CountryCode,
                                              to_PartyId,

                                              connectorId,
                                              connectorPatch,
                                              cancellationToken) => {

                await Processor(
                    "OnPatchConnectorRequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",             $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",               $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("connectorId",      connectorId.ToString()),
                              new JProperty("connectorPatch",   connectorPatch)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchConnectorResponse

            OnPatchConnectorResponse += async (timestamp,
                                               sender,
                                               eventTrackingId,
                                               from_CountryCode,
                                               from_PartyId,
                                               to_CountryCode,
                                               to_PartyId,

                                               connectorId,
                                               connectorPatch,
                                               runtime,
                                               cancellationToken) => {

                await Processor(
                    "OnPatchConnectorResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",             $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",               $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("connectorId",      connectorId.ToString()),
                              new JProperty("connectorPatch",   connectorPatch),

                              new JProperty("runtime",          runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPutTariffRequest

            OnPutTariffRequest += async (timestamp,
                                         sender,
                                         eventTrackingId,
                                         from_CountryCode,
                                         from_PartyId,
                                         to_CountryCode,
                                         to_PartyId,

                                         tariff,
                                         cancellationToken) => {

                await Processor(
                    "OnPutTariffRequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",     $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",       $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("tariff",   tariff.ToJSON(
                                                            true, //IncludeOwnerInformation,
                                                            true, //IncludeExtensions,
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
                    cancellationToken
                );

            };

            #endregion

            #region OnPutTariffResponse

            OnPutTariffResponse += async (timestamp,
                                          sender,
                                          eventTrackingId,
                                          from_CountryCode,
                                          from_PartyId,
                                          to_CountryCode,
                                          to_PartyId,

                                          tariff,
                                          runtime,
                                          cancellationToken) => {

                await Processor(
                    "OnPutTariffResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",      $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",        $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("tariff",    tariff.ToJSON(
                                                             true, //IncludeOwnerInformation,
                                                             true, //IncludeExtensions,
                                                             CustomTariffSerializer,
                                                             CustomDisplayTextSerializer,
                                                             CustomPriceSerializer,
                                                             CustomTariffElementSerializer,
                                                             CustomPriceComponentSerializer,
                                                             CustomTariffRestrictionsSerializer,
                                                             CustomEnergyMixSerializer,
                                                             CustomEnergySourceSerializer,
                                                             CustomEnvironmentalImpactSerializer
                                                         )),

                              new JProperty("runtime",   runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPutSessionRequest

            OnPutSessionRequest += async (timestamp,
                                          sender,
                                          eventTrackingId,
                                          from_CountryCode,
                                          from_PartyId,
                                          to_CountryCode,
                                          to_PartyId,

                                          session,
                                          cancellationToken) => {

                await Processor(
                    "OnPutSessionRequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",      $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",        $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("session",   session.ToJSON(
                                                             CustomSessionSerializer,
                                                             CustomCDRTokenSerializer,
                                                             CustomChargingPeriodSerializer,
                                                             CustomCDRDimensionSerializer,
                                                             CustomPriceSerializer
                                                         ))

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPutSessionResponse

            OnPutSessionResponse += async (timestamp,
                                           sender,
                                           eventTrackingId,
                                           from_CountryCode,
                                           from_PartyId,
                                           to_CountryCode,
                                           to_PartyId,

                                           session,
                                           runtime,
                                           cancellationToken) => {

                await Processor(
                    "OnPutSessionResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",      $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",        $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("session",   session.ToJSON(
                                                             CustomSessionSerializer,
                                                             CustomCDRTokenSerializer,
                                                             CustomChargingPeriodSerializer,
                                                             CustomCDRDimensionSerializer,
                                                             CustomPriceSerializer
                                                         )),

                              new JProperty("runtime",   runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchSessionRequest

            OnPatchSessionRequest += async (timestamp,
                                            sender,
                                            eventTrackingId,
                                            from_CountryCode,
                                            from_PartyId,
                                            to_CountryCode,
                                            to_PartyId,

                                            sessionId,
                                            sessionPatch,
                                            cancellationToken) => {

                await Processor(
                    "OnPatchSessionRequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",           $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",             $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("sessionId",      sessionId.ToString()),
                              new JProperty("sessionPatch",   sessionPatch)

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPatchSessionResponse

            OnPatchSessionResponse += async (timestamp,
                                             sender,
                                             eventTrackingId,
                                             from_CountryCode,
                                             from_PartyId,
                                             to_CountryCode,
                                             to_PartyId,

                                             sessionId,
                                             sessionPatch,
                                             runtime,
                                             cancellationToken) => {

                await Processor(
                    "OnPatchSessionResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",           $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",             $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("sessionId",      sessionId.ToString()),
                              new JProperty("sessionPatch",   sessionPatch),

                              new JProperty("runtime",        runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPostCDRRequest

            OnPostCDRRequest += async (timestamp,
                                       sender,
                                       eventTrackingId,
                                       from_CountryCode,
                                       from_PartyId,
                                       to_CountryCode,
                                       to_PartyId,

                                       cdr,
                                       cancellationToken) => {

                await Processor(
                    "OnPostCDRRequest",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",   $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",     $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("cdr",    cdr.ToJSON(
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
                    cancellationToken
                );

            };

            #endregion

            #region OnPostCDRResponse

            OnPostCDRResponse += async (timestamp,
                                        sender,
                                        eventTrackingId,
                                        from_CountryCode,
                                        from_PartyId,
                                        to_CountryCode,
                                        to_PartyId,

                                        cdr,
                                        cdrLocation,
                                        runtime,
                                        cancellationToken) => {

                await Processor(
                    "OnPostCDRResponse",
                    JSONObject.Create(

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",          $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",            $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("cdr",           cdr.ToJSON(
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
                                                             )),

                              new JProperty("cdrLocation",   cdrLocation.ToString()),

                              new JProperty("runtime",       runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion


            #region OnPostTokenRequest

            OnPostTokenRequest += async (timestamp,
                                         sender,
                                         eventTrackingId,
                                         from_CountryCode,
                                         from_PartyId,
                                         to_CountryCode,
                                         to_PartyId,

                                         tokenId,
                                         requestedTokenType,
                                         locationReference,
                                         cancellationToken) => {

                await Processor(
                    "OnPostTokenRequest",
                    JSONObject.Create(

                              new JProperty("eventTrackingId",      eventTrackingId.ToString()),

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",                 $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",                   $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("tokenId",              tokenId.ToString()),

                        requestedTokenType.HasValue
                            ? new JProperty("requestedTokenType",   requestedTokenType.Value.ToString())
                            : null,

                        locationReference.HasValue
                            ? new JProperty("locationReference",    locationReference.Value.ToJSON(
                                                                        CustomLocationReferenceSerializer
                                                                    ))
                            : null

                    ),
                    cancellationToken
                );

            };

            #endregion

            #region OnPostTokenResponse

            OnPostTokenResponse += async (timestamp,
                                          sender,
                                          eventTrackingId,
                                          from_CountryCode,
                                          from_PartyId,
                                          to_CountryCode,
                                          to_PartyId,

                                          tokenId,
                                          requestedTokenType,
                                          locationReference,
                                          result,
                                          runtime,
                                          cancellationToken) => {

                await Processor(
                    "OnPostTokenResponse",
                    JSONObject.Create(

                              new JProperty("eventTrackingId",      eventTrackingId.ToString()),

                        from_CountryCode.HasValue && from_PartyId.HasValue
                            ? new JProperty("from",                 $"{from_CountryCode}*{from_PartyId}")
                            : null,

                        to_CountryCode.  HasValue && to_PartyId.  HasValue
                            ? new JProperty("to",                   $"{to_CountryCode}*{to_PartyId}")
                            : null,

                              new JProperty("tokenId",              tokenId.ToString()),

                        requestedTokenType.HasValue
                            ? new JProperty("requestedTokenType",   requestedTokenType.Value.ToString())
                            : null,

                        locationReference. HasValue
                            ? new JProperty("locationReference",    locationReference.Value.ToJSON(
                                                                        CustomLocationReferenceSerializer
                                                                    ))
                            : null,

                              new JProperty("result",               result.ToJSON(
                                                                        CustomAuthorizationInfoSerializer,
                                                                        CustomTokenSerializer,
                                                                        CustomLocationReferenceSerializer,
                                                                        CustomDisplayTextSerializer
                                                                    )),

                              new JProperty("runtime",              runtime.TotalSeconds)

                    ),
                    cancellationToken
                );

            };

            #endregion

        }



        public void LinkEventsToDebugText()
        {
            EventsToText(
                txt => {
                    DebugX.LogT(txt);
                    return Task.CompletedTask;
                }
            );
        }

        public void EventsToText(Func<String, Task> Processor)
        {

            #region OnPutLocationRequest

            OnPutLocationRequest += (timestamp,
                                     sender,
                                     eventTrackingId,
                                     from_CountryCode,
                                     from_PartyId,
                                     to_CountryCode,
                                     to_PartyId,

                                     location,
                                     cancellationToken) =>

                Processor(
                    $"PutLocation request for '{location.Id}'"
                );

            #endregion

            #region OnPutLocationResponse

            OnPutLocationResponse += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      from_CountryCode,
                                      from_PartyId,
                                      to_CountryCode,
                                      to_PartyId,

                                      location,
                                      runtime,
                                      cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutLocation response for for '{location.Id}'",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion

            #region OnPatchLocationRequest

            OnPatchLocationRequest += (timestamp,
                                       sender,
                                       eventTrackingId,
                                       from_CountryCode,
                                       from_PartyId,
                                       to_CountryCode,
                                       to_PartyId,

                                       locationId,
                                       locationPatch,

                                       cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PatchLocation request for '{locationId}'"
                    )
                );

            #endregion

            #region OnPatchLocationResponse

            OnPatchLocationResponse += (timestamp,
                                        sender,
                                        eventTrackingId,
                                        from_CountryCode,
                                        from_PartyId,
                                        to_CountryCode,
                                        to_PartyId,

                                        locationId,
                                        locationPatch,
                                        runtime,
                                        cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PatchLocation response for '{locationId}'",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion


            #region OnPutEVSERequest

            OnPutEVSERequest += (timestamp,
                                 sender,
                                 eventTrackingId,
                                 from_CountryCode,
                                 from_PartyId,
                                 to_CountryCode,
                                 to_PartyId,

                                 evse,
                                 cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutEVSE request for '{evse.UId}'"
                    )
                );

            #endregion

            #region OnPutEVSEResponse

            OnPutEVSEResponse += (timestamp,
                                  sender,
                                  eventTrackingId,
                                  from_CountryCode,
                                  from_PartyId,
                                  to_CountryCode,
                                  to_PartyId,

                                  evse,
                                  runtime,
                                  cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutEVSE response for '{evse.UId}'",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion

            #region OnPatchEVSERequest

            OnPatchEVSERequest += (timestamp,
                                   sender,
                                   eventTrackingId,
                                   from_CountryCode,
                                   from_PartyId,
                                   to_CountryCode,
                                   to_PartyId,

                                   evseUId,
                                   evsePatch,
                                   cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PatchEVSE request for '{evseUId}'"
                    )
                );

            #endregion

            #region OnPatchEVSEResponse

            OnPatchEVSEResponse += (timestamp,
                                    sender,
                                    eventTrackingId,
                                    from_CountryCode,
                                    from_PartyId,
                                    to_CountryCode,
                                    to_PartyId,

                                    evseUId,
                                    evsePatch,
                                    runtime,
                                    cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PatchEVSE response for '{evseUId}'",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion


            #region OnPutConnectorRequest

            OnPutConnectorRequest += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      from_CountryCode,
                                      from_PartyId,
                                      to_CountryCode,
                                      to_PartyId,

                                      connector,
                                      cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutConnector request for '{connector.Id}'"
                    )
                );

            #endregion

            #region OnPutConnectorResponse

            OnPutConnectorResponse += (timestamp,
                                       sender,
                                       eventTrackingId,
                                       from_CountryCode,
                                       from_PartyId,
                                       to_CountryCode,
                                       to_PartyId,

                                       connector,
                                       runtime,
                                       cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutConnector response for '{connector}",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion

            #region OnPatchConnectorRequest

            OnPatchConnectorRequest += (timestamp,
                                        sender,
                                        eventTrackingId,
                                        from_CountryCode,
                                        from_PartyId,
                                        to_CountryCode,
                                        to_PartyId,

                                        connectorId,
                                        connectorPatch,
                                        cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PatchConnector request for '{connectorId}'"
                    )
                );

            #endregion

            #region OnPatchConnectorResponse

            OnPatchConnectorResponse += (timestamp,
                                         sender,
                                         eventTrackingId,
                                         from_CountryCode,
                                         from_PartyId,
                                         to_CountryCode,
                                         to_PartyId,

                                         connectorId,
                                         connectorPatch,
                                         runtime,
                                         cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PatchConnector response for '{connectorId}'",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion


            #region OnPutTariffRequest

            OnPutTariffRequest += (timestamp,
                                   sender,
                                   eventTrackingId,
                                   from_CountryCode,
                                   from_PartyId,
                                   to_CountryCode,
                                   to_PartyId,

                                   tariff,
                                   cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutTariff request for '{tariff.Id}'"
                    )
                );

            #endregion

            #region OnPutTariffResponse

            OnPutTariffResponse += (timestamp,
                                    sender,
                                    eventTrackingId,
                                    from_CountryCode,
                                    from_PartyId,
                                    to_CountryCode,
                                    to_PartyId,

                                    tariff,
                                    runtime,
                                    cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutTariff response for '{tariff.Id}'",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion


            #region OnPutSessionRequest

            OnPutSessionRequest += (timestamp,
                                    sender,
                                    eventTrackingId,
                                    from_CountryCode,
                                    from_PartyId,
                                    to_CountryCode,
                                    to_PartyId,

                                    session,
                                    cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutSession request for '{session.Id}'"
                    )
                );

            #endregion

            #region OnPutSessionResponse

            OnPutSessionResponse += (timestamp,
                                     sender,
                                     eventTrackingId,
                                     from_CountryCode,
                                     from_PartyId,
                                     to_CountryCode,
                                     to_PartyId,

                                     session,
                                     runtime,
                                     cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PutSession response for '{session.Id}",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion

            #region OnPatchSessionRequest

            OnPatchSessionRequest += (timestamp,
                                      sender,
                                      eventTrackingId,
                                      from_CountryCode,
                                      from_PartyId,
                                      to_CountryCode,
                                      to_PartyId,

                                      sessionId,
                                      sessionPatch,
                                      cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PatchSession request for '{sessionId}'"
                    )
                );

            #endregion

            #region OnPatchSessionResponse

            OnPatchSessionResponse += (timestamp,
                                       sender,
                                       eventTrackingId,
                                       from_CountryCode,
                                       from_PartyId,
                                       to_CountryCode,
                                       to_PartyId,

                                       sessionId,
                                       sessionPatch,
                                       runtime,
                                       cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PatchSession response for '{sessionId}",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion


            #region OnPostCDRRequest

            OnPostCDRRequest += (timestamp,
                                 sender,
                                 eventTrackingId,
                                 from_CountryCode,
                                 from_PartyId,
                                 to_CountryCode,
                                 to_PartyId,

                                 cdr,
                                 cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PostCDR request '{cdr.Id}' for session '{cdr.SessionId}'"
                    )
                );

            #endregion

            #region OnPostCDRResponse

            OnPostCDRResponse += (timestamp,
                                  sender,
                                  eventTrackingId,
                                  from_CountryCode,
                                  from_PartyId,
                                  to_CountryCode,
                                  to_PartyId,

                                  cdr,
                                  cdrLocation,
                                  runtime,
                                  cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PostCDR response '{cdr.Id}' for session '{cdr.SessionId}' @ {cdrLocation}",
                        " => ",
                        $"({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion


            #region OnPostTokenRequest

            OnPostTokenRequest += (timestamp,
                                   sender,
                                   eventTrackingId,
                                   from_CountryCode,
                                   from_PartyId,
                                   to_CountryCode,
                                   to_PartyId,

                                   tokenId,
                                   requestedTokenType,
                                   locationReference,
                                   cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PostToken request '{tokenId}'",
                        requestedTokenType.HasValue
                            ? $" ({requestedTokenType})"
                            : "",
                        locationReference.HasValue
                            ? $" @{locationReference.Value.LocationId}{(locationReference.Value.EVSEUIds.Any() ? $"/{locationReference.Value.EVSEUIds.AggregateWith(", ")}" : "")}"
                            : ""
                    )
                );

            #endregion

            #region OnPostTokenResponse

            OnPostTokenResponse += (timestamp,
                                    sender,
                                    eventTrackingId,
                                    from_CountryCode,
                                    from_PartyId,
                                    to_CountryCode,
                                    to_PartyId,

                                    tokenId,
                                    requestedTokenType,
                                    locationReference,
                                    result,
                                    runtime,
                                    cancellationToken) =>

                Processor(
                    String.Concat(
                        $"PostToken response '{tokenId}'",
                        requestedTokenType.HasValue
                            ? $" ({requestedTokenType})"
                            : "",
                        locationReference.HasValue
                            ? $" @{locationReference.Value.LocationId}{(locationReference.Value.EVSEUIds.Any() ? $"/{locationReference.Value.EVSEUIds.AggregateWith(", ")}" : "")}"
                            : "",
                        " => ",
                        $"{result.Allowed} ({(UInt64) runtime.TotalMilliseconds} ms)"
                    )
                );

            #endregion

        }



        #region (private) LogEvent (Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OICPCommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(EMSP_HTTPAPI),
                   Logger,
                   LogHandler,
                   EventName,
                   OICPCommand
               );

        #endregion


    }

}
