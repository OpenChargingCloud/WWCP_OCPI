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
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_2_1.HUB2CPO.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.HUB.HTTP
{

    /// <summary>
    /// The HUB2CPO client is used by an HUB to talk to CPO.
    /// </summary>
    public partial class HUB2CPOClient : CommonHTTPClient
    {

        #region (class) APICounters

        public class APICounters : CommonAPICounters
        {

            #region Properties

            public APICounterValues  GetLocations          { get; }
            public APICounterValues  GetLocation           { get; }
            public APICounterValues  GetEVSE               { get; }
            public APICounterValues  GetConnector          { get; }


            public APICounterValues  GetTariffs            { get; }
            public APICounterValues  GetTariff             { get; }


            public APICounterValues  GetSessions           { get; }
            public APICounterValues  GetSession            { get; }


            public APICounterValues  GetCDRs               { get; }
            public APICounterValues  GetCDR                { get; }


            public APICounterValues  GetToken              { get; }
            public APICounterValues  PutToken              { get; }
            public APICounterValues  PatchToken            { get; }

            public APICounterValues  ReserveNow            { get; }
            public APICounterValues  CancelReservation     { get; }
            public APICounterValues  StartSession          { get; }
            public APICounterValues  StopSession           { get; }
            public APICounterValues  UnlockConnector       { get; }
            public APICounterValues  SetChargingProfile    { get; }

            #endregion

            #region Constructor(s)

            public APICounters(APICounterValues?  GetVersions          = null,
                               APICounterValues?  Register             = null,

                               APICounterValues?  GetLocations         = null,
                               APICounterValues?  GetLocation          = null,
                               APICounterValues?  GetEVSE              = null,
                               APICounterValues?  GetConnector         = null,

                               APICounterValues?  GetTariffs           = null,
                               APICounterValues?  GetTariff            = null,

                               APICounterValues?  GetSessions          = null,
                               APICounterValues?  GetSession           = null,

                               APICounterValues?  GetCDRs              = null,
                               APICounterValues?  GetCDR               = null,

                               APICounterValues?  GetToken             = null,
                               APICounterValues?  PutToken             = null,
                               APICounterValues?  PatchToken           = null,

                               APICounterValues?  ReserveNow           = null,
                               APICounterValues?  CancelReservation    = null,
                               APICounterValues?  StartSession         = null,
                               APICounterValues?  StopSession          = null,
                               APICounterValues?  UnlockConnector      = null,
                               APICounterValues?  SetChargingProfile   = null)

                : base(GetVersions,
                       Register)

            {

                this.GetLocations        = GetLocations       ?? new APICounterValues();
                this.GetLocation         = GetLocation        ?? new APICounterValues();
                this.GetEVSE             = GetEVSE            ?? new APICounterValues();
                this.GetConnector        = GetConnector       ?? new APICounterValues();

                this.GetTariffs          = GetTariffs         ?? new APICounterValues();
                this.GetTariff           = GetTariff          ?? new APICounterValues();

                this.GetSessions         = GetSessions        ?? new APICounterValues();
                this.GetSession          = GetSession         ?? new APICounterValues();

                this.GetCDRs             = GetCDRs            ?? new APICounterValues();
                this.GetCDR              = GetCDR             ?? new APICounterValues();

                this.GetToken            = GetToken           ?? new APICounterValues();
                this.PutToken            = PutToken           ?? new APICounterValues();
                this.PatchToken          = PatchToken         ?? new APICounterValues();

                this.ReserveNow          = ReserveNow         ?? new APICounterValues();
                this.CancelReservation   = CancelReservation  ?? new APICounterValues();
                this.StartSession        = StartSession       ?? new APICounterValues();
                this.StopSession         = StopSession        ?? new APICounterValues();
                this.UnlockConnector     = UnlockConnector    ?? new APICounterValues();
                this.SetChargingProfile  = SetChargingProfile ?? new APICounterValues();

            }

            #endregion


            #region ToJSON()

            public override JObject ToJSON()
            {

                var json = base.ToJSON();

                json.Add(new JProperty("getLocations",        GetLocations.      ToJSON()));
                json.Add(new JProperty("getLocation",         GetLocation.       ToJSON()));
                json.Add(new JProperty("getEVSE",             GetEVSE.           ToJSON()));
                json.Add(new JProperty("getConnecto",         GetConnector.      ToJSON()));

                json.Add(new JProperty("getTariffs",          GetTariffs.        ToJSON()));
                json.Add(new JProperty("getTariff",           GetTariff.         ToJSON()));

                json.Add(new JProperty("getSessions",         GetSessions.       ToJSON()));
                json.Add(new JProperty("getSession",          GetSession.        ToJSON()));

                json.Add(new JProperty("getCDRs",             GetCDRs.           ToJSON()));
                json.Add(new JProperty("getCDR",              GetCDR.            ToJSON()));

                json.Add(new JProperty("getToken",            GetToken.          ToJSON()));
                json.Add(new JProperty("putToken",            PutToken.          ToJSON()));
                json.Add(new JProperty("patchToken",          PatchToken.        ToJSON()));

                json.Add(new JProperty("reserveNow",          ReserveNow.        ToJSON()));
                json.Add(new JProperty("cancelReservation",   CancelReservation. ToJSON()));
                json.Add(new JProperty("startSession",        StartSession.      ToJSON()));
                json.Add(new JProperty("stopSession",         StopSession.       ToJSON()));
                json.Add(new JProperty("unlockConnector",     UnlockConnector.   ToJSON()));
                json.Add(new JProperty("setChargingProfile",  SetChargingProfile.ToJSON()));

                return json;

            }

            #endregion

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP user agent.
        /// </summary>
        public new const String  DefaultHTTPUserAgent    = $"GraphDefined OCPI {Version.String} {nameof(HUB2CPOClient)}";

        /// <summary>
        /// The default logging context.
        /// </summary>
        public new const String  DefaultLoggingContext   = nameof(HUB2CPOClient);

        #endregion

        #region Properties

        /// <summary>
        /// The identification of the remote Charge Point Operator.
        /// </summary>
        public CPO_Id                RemoteCPOId    { get; }

        /// <summary>
        /// Our HUB HTTP API.
        /// </summary>
        public HUB_HTTPAPI           HUB_HTTPAPI    { get; }

        /// <summary>
        /// HUB client event counters.
        /// </summary>
        public new APICounters       Counters       { get; }

        /// <summary>
        /// The HUB client (HTTP client) logger.
        /// </summary>
        public new HTTPClientLogger  HTTPLogger
        {
            get
            {
                return base.HTTPLogger as HTTPClientLogger;
            }
            set
            {
                base.HTTPLogger = value;
            }
        }

        #endregion

        #region Events

        #region OnGetLocationsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all locations will be send.
        /// </summary>
        public event OnGetLocationsRequestDelegate?   OnGetLocationsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all locations will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnGetLocationsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all locations HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnGetLocationsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all locations request had been received.
        /// </summary>
        public event OnGetLocationsResponseDelegate?  OnGetLocationsResponse;

        #endregion

        #region OnGetLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a location by its identification will be send.
        /// </summary>
        public event OnGetLocationRequestDelegate?   OnGetLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a location by its identification will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnGetLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a location by its identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnGetLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a location by its identification request had been received.
        /// </summary>
        public event OnGetLocationResponseDelegate?  OnGetLocationResponse;

        #endregion

        #region OnGetEVSERequest/-Response

        /// <summary>
        /// An event fired whenever a request getting an EVSE by its identification will be send.
        /// </summary>
        public event OnGetEVSERequestDelegate?   OnGetEVSERequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting an EVSE by its identification will be send.
        /// </summary>
        public event ClientRequestLogHandler?    OnGetEVSEHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting an EVSE by its identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?   OnGetEVSEHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting an EVSE by its identification request had been received.
        /// </summary>
        public event OnGetEVSEResponseDelegate?  OnGetEVSEResponse;

        #endregion

        #region OnGetConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a connector by its identification will be send.
        /// </summary>
        public event OnGetConnectorRequestDelegate?   OnGetConnectorRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a connector by its identification will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnGetConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a connector by its identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnGetConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a connector by its identification request had been received.
        /// </summary>
        public event OnGetConnectorResponseDelegate?  OnGetConnectorResponse;

        #endregion


        #region OnGetTariffsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all tariffs will be send.
        /// </summary>
        public event OnGetTariffsRequestDelegate?     OnGetTariffsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all tariffs will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnGetTariffsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all tariffs HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnGetTariffsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all tariffs request had been received.
        /// </summary>
        public event OnGetTariffsResponseDelegate?    OnGetTariffsResponse;

        #endregion

        #region OnGetTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a tariff by it identification will be send.
        /// </summary>
        public event OnGetTariffRequestDelegate?     OnGetTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a tariff by it identification will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnGetTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff by it identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnGetTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff by it identification request had been received.
        /// </summary>
        public event OnGetTariffResponseDelegate?    OnGetTariffResponse;

        #endregion


        #region OnGetSessionsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all sessions will be send.
        /// </summary>
        public event OnGetSessionsRequestDelegate?   OnGetSessionsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all sessions will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnGetSessionsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all sessions HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnGetSessionsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all sessions request had been received.
        /// </summary>
        public event OnGetSessionsResponseDelegate?  OnGetSessionsResponse;

        #endregion

        #region OnGetSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a session by it identification will be send.
        /// </summary>
        public event OnGetSessionRequestDelegate?     OnGetSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a session by it identification will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnGetSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a session by it identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnGetSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a session by it identification request had been received.
        /// </summary>
        public event OnGetSessionResponseDelegate?    OnGetSessionResponse;

        #endregion


        #region OnGetCDRsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all CDRs will be send.
        /// </summary>
        public event OnGetCDRsRequestDelegate?   OnGetCDRsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all CDRs will be send.
        /// </summary>
        public event ClientRequestLogHandler?    OnGetCDRsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all CDRs HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?   OnGetCDRsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all CDRs request had been received.
        /// </summary>
        public event OnGetCDRsResponseDelegate?  OnGetCDRsResponse;

        #endregion

        #region OnGetCDRRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a CDR by it identification will be send.
        /// </summary>
        public event OnGetCDRRequestDelegate?     OnGetCDRRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a CDR by it identification will be send.
        /// </summary>
        public event ClientRequestLogHandler?     OnGetCDRHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a CDR by it identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?    OnGetCDRHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a CDR by it identification request had been received.
        /// </summary>
        public event OnGetCDRResponseDelegate?    OnGetCDRResponse;

        #endregion


        #region OnGetTokenRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a token will be send.
        /// </summary>
        public event OnGetTokenRequestDelegate?   OnGetTokenRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a token will be send.
        /// </summary>
        public event ClientRequestLogHandler?     OnGetTokenHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a token HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?    OnGetTokenHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a token request had been received.
        /// </summary>
        public event OnGetTokenResponseDelegate?  OnGetTokenResponse;

        #endregion

        #region OnPutTokenRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a token will be send.
        /// </summary>
        public event OnPutTokenRequestDelegate?   OnPutTokenRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a token will be send.
        /// </summary>
        public event ClientRequestLogHandler?     OnPutTokenHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a token HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?    OnPutTokenHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a token request had been received.
        /// </summary>
        public event OnPutTokenResponseDelegate?  OnPutTokenResponse;

        #endregion

        #region OnPatchTokenRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a token will be send.
        /// </summary>
        public event OnPatchTokenRequestDelegate?   OnPatchTokenRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a token will be send.
        /// </summary>
        public event ClientRequestLogHandler?       OnPatchTokenHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a token HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?      OnPatchTokenHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a token request had been received.
        /// </summary>
        public event OnPatchTokenResponseDelegate?  OnPatchTokenResponse;

        #endregion


        // Commands

        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event fired whenever a ReserveNow request will be send.
        /// </summary>
        public event OnReserveNowRequestDelegate?   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a ReserveNow HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler?       OnReserveNowHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?      OnReserveNowHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request had been received.
        /// </summary>
        public event OnReserveNowResponseDelegate?  OnReserveNowResponse;

        #endregion

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event fired whenever a CancelReservation request will be send.
        /// </summary>
        public event OnCancelReservationRequestDelegate?   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a CancelReservation HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler?              OnCancelReservationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?             OnCancelReservationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request had been received.
        /// </summary>
        public event OnCancelReservationResponseDelegate?  OnCancelReservationResponse;

        #endregion

        #region OnStartSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a StartSession request will be send.
        /// </summary>
        public event OnStartSessionRequestDelegate?   OnStartSessionRequest;

        /// <summary>
        /// An event fired whenever a StartSession HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler?         OnStartSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a StartSession HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?        OnStartSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a StartSession request had been received.
        /// </summary>
        public event OnStartSessionResponseDelegate?  OnStartSessionResponse;

        #endregion

        #region OnStopSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a StopSession request will be send.
        /// </summary>
        public event OnStopSessionRequestDelegate?   OnStopSessionRequest;

        /// <summary>
        /// An event fired whenever a StopSession HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnStopSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a StopSession HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnStopSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a StopSession request had been received.
        /// </summary>
        public event OnStopSessionResponseDelegate?  OnStopSessionResponse;

        #endregion

        #region OnUnlockConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a UnlockConnector request will be send.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate?   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a UnlockConnector HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler?            OnUnlockConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a UnlockConnector HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?           OnUnlockConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a UnlockConnector request had been received.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate?  OnUnlockConnectorResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new HUB2CPO HTTP client.
        /// </summary>
        /// <param name="HUB_HTTPAPI">The HUB HTTP API.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this CPO client.</param>
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public HUB2CPOClient(HUB_HTTPAPI                                               HUB_HTTPAPI,
                             RemoteParty                                               RemoteParty,
                             HTTPHostname?                                             VirtualHostname   = null,
                             I18NString?                                               Description       = null,
                             org.GraphDefined.Vanaheimr.Hermod.HTTP.HTTPClientLogger?  HTTPLogger        = null,

                             Boolean?                                                  DisableLogging    = false,
                             String?                                                   LoggingPath       = null,
                             String?                                                   LoggingContext    = null,
                             OCPILogfileCreatorDelegate?                               LogfileCreator    = null,
                             IDNSClient?                                               DNSClient         = null)

            : base(HUB_HTTPAPI.CommonAPI,
                   RemoteParty,
                   VirtualHostname,
                   Description,
                   HTTPLogger,

                   DisableLogging,
                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   DNSClient)

        {

            var cpoId         = RemoteParty.Id.AsCPOId();

            if (!cpoId.HasValue)
                throw new ArgumentException("The given remote party identification is not a valid CPO identification!", nameof(RemoteParty));

            this.RemoteCPOId  = cpoId.Value;
            this.HUB_HTTPAPI  = HUB_HTTPAPI;
            this.Counters     = new APICounters();

            base.HTTPLogger   = this.DisableLogging == false
                                    ? new HTTPClientLogger(
                                          this,
                                          LoggingPath,
                                          LoggingContext,
                                          LogfileCreator
                                      )
                                    : null;

        }

        #endregion



        #region GetLocations      (...)

        /// <summary>
        /// Get all locations from the remote API.
        /// </summary>
        /// <param name="From">An optional 'from' timestamp (inclusive).</param>
        /// <param name="To">An optional 'to' timestamp (exclusive).</param>
        /// <param name="Offset">An optional 'offset' within the result set.</param>
        /// <param name="Limit">An optional 'limit' of the result set.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<IEnumerable<Location>>>

            GetLocations(DateTimeOffset?    From                = null,
                         DateTimeOffset?    To                  = null,
                         UInt64?            Offset              = null,
                         UInt64?            Limit               = null,

                         Version_Id?        VersionId           = null,
                         Request_Id?        RequestId           = null,
                         Correlation_Id?    CorrelationId       = null,

                         DateTimeOffset?    RequestTimestamp    = null,
                         EventTracking_Id?  EventTrackingId     = null,
                         TimeSpan?          RequestTimeout      = null,
                         CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetLocations.IncRequests_OK();

            OCPIResponse<IEnumerable<Location>> response;

            #endregion

            #region Send OnGetLocationsRequest event

            await LogEvent(
                      OnGetLocationsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var dateAndPaginationFilters  = new OCPIRequest.DateAndPaginationFilters(
                                                        From,
                                                        To,
                                                        Offset,
                                                        Limit
                                                    );

                    var httpResponse              = await NewHTTPClient.GET(
                                                              Path:                  httpClient.RemoteURL.Path + dateAndPaginationFilters.ToHTTPQueryString(),
                                                              Authentication:        TokenAuth,
                                                              RequestBuilder:        requestBuilder => {
                                                                                         requestBuilder.Set("X-Request-ID",     requestId);
                                                                                         requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                                     },
                                                              RequestLogDelegate:    OnGetLocationsHTTPRequest,
                                                              ResponseLogDelegate:   OnGetLocationsHTTPResponse,
                                                              EventTrackingId:       eventTrackingId,
                                                              //NumberOfRetry:         transmissionRetry,
                                                              RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                              CancellationToken:     CancellationToken).

                                                          ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Location>.ParseJArray(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Location.Parse(json)
                               );

                    Counters.GetLocations.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, IEnumerable<Location>>.Error("No remote URL available!");
                    Counters.GetLocations.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, IEnumerable<Location>>.Exception(e);
                Counters.GetLocations.IncResponses_Error();
            }


            #region Send OnGetLocationsResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetLocationsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region GetLocation       (LocationId, ...)

        /// <summary>
        /// Get the location specified by the given location identification from the remote API.
        /// </summary>
        /// <param name="LocationId">The identification of the requested location.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Location>>

            GetLocation(Location_Id        LocationId,

                        Version_Id?        VersionId           = null,
                        Request_Id?        RequestId           = null,
                        Correlation_Id?    CorrelationId       = null,

                        DateTimeOffset?    RequestTimestamp    = null,
                        EventTracking_Id?  EventTrackingId     = null,
                        TimeSpan?          RequestTimeout      = null,
                        CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetLocation.IncRequests_OK();

            OCPIResponse<Location> response;

            #endregion

            #region Send OnGetLocationRequest event

            await LogEvent(
                      OnGetLocationRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId.ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetLocationHTTPRequest,
                                                 ResponseLogDelegate:   OnGetLocationHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Location>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Location.Parse(json)
                               );

                    Counters.GetLocation.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Location>.Error("No remote URL available!");
                    Counters.GetLocation.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Location>.Exception(e);
                Counters.GetLocation.IncResponses_Error();
            }


            #region Send OnGetLocationResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetLocationResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region GetEVSE           (LocationId, EVSEUId, ...)

        /// <summary>
        /// Get the EVSE specified by the given location and EVSE identification from the remote API.
        /// </summary>
        /// <param name="LocationId">The identification of the requested location.</param>
        /// <param name="EVSEUId">The unique identification of the EVSE location.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<EVSE>>

            GetEVSE(Location_Id        LocationId,
                    EVSE_UId           EVSEUId,

                    Version_Id?        VersionId           = null,
                    Request_Id?        RequestId           = null,
                    Correlation_Id?    CorrelationId       = null,

                    DateTimeOffset?    RequestTimestamp    = null,
                    EventTracking_Id?  EventTrackingId     = null,
                    TimeSpan?          RequestTimeout      = null,
                    CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetEVSE.IncRequests_OK();

            OCPIResponse<EVSE> response;

            #endregion

            #region Send OnGetEVSERequest event

            await LogEvent(
                      OnGetEVSERequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          EVSEUId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId.ToString() +
                                                                                                    EVSEUId.   ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetEVSEHTTPRequest,
                                                 ResponseLogDelegate:   OnGetEVSEHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<EVSE>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => EVSE.Parse(json)
                               );

                    Counters.GetEVSE.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, EVSE>.Error("No remote URL available!");
                    Counters.GetEVSE.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, EVSE>.Exception(e);
                Counters.GetEVSE.IncResponses_Error();
            }


            #region Send OnGetEVSEResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetEVSEResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          EVSEUId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region GetConnector      (LocationId, EVSEUId, ConnectorId, ...)

        /// <summary>
        /// Get the connector specified by the given location, EVSE and connector identification from the remote API.
        /// </summary>
        /// <param name="LocationId">The identification of the requested location.</param>
        /// <param name="EVSEUId">The unique identification of the EVSE location.</param>
        /// <param name="ConnectorId">The identification of the requested connector.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Connector>>

            GetConnector(Location_Id         LocationId,
                         EVSE_UId            EVSEUId,
                         Connector_Id        ConnectorId,

                         Version_Id?         VersionId           = null,
                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,

                         DateTimeOffset?    RequestTimestamp    = null,
                         EventTracking_Id?  EventTrackingId     = null,
                         TimeSpan?          RequestTimeout      = null,
                         CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetConnector.IncRequests_OK();

            OCPIResponse<Connector> response;

            #endregion

            #region Send OnGetConnectorRequest event

            await LogEvent(
                      OnGetConnectorRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          EVSEUId,
                          ConnectorId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Locations,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + LocationId. ToString() +
                                                                                                    EVSEUId.    ToString() +
                                                                                                    ConnectorId.ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetConnectorHTTPRequest,
                                                 ResponseLogDelegate:   OnGetConnectorHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Connector>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Connector.Parse(json)
                               );

                    Counters.GetConnector.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Connector>.Error("No remote URL available!");
                    Counters.GetConnector.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Connector>.Exception(e);
                Counters.GetConnector.IncResponses_Error();
            }


            #region Send OnGetConnectorResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetConnectorResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          EVSEUId,
                          ConnectorId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region GetTariffs        (...)

        /// <summary>
        /// Get all tariffs from the remote API.
        /// </summary>
        /// <param name="From">An optional 'from' timestamp (inclusive).</param>
        /// <param name="To">An optional 'to' timestamp (exclusive).</param>
        /// <param name="Offset">An optional 'offset' within the result set.</param>
        /// <param name="Limit">An optional 'limit' of the result set.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<IEnumerable<Tariff>>>

            GetTariffs(DateTimeOffset?    From                = null,
                       DateTimeOffset?    To                  = null,
                       UInt64?            Offset              = null,
                       UInt64?            Limit               = null,

                       Version_Id?        VersionId           = null,
                       Request_Id?        RequestId           = null,
                       Correlation_Id?    CorrelationId       = null,

                       DateTimeOffset?    RequestTimestamp    = null,
                       EventTracking_Id?  EventTrackingId     = null,
                       TimeSpan?          RequestTimeout      = null,
                       CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetTariffs.IncRequests_OK();

            OCPIResponse<IEnumerable<Tariff>> response;

            #endregion

            #region Send OnGetTariffRequest event

            await LogEvent(
                      OnGetTariffsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          From,
                          To,
                          Offset,
                          Limit,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var dateAndPaginationFilters  = new OCPIRequest.DateAndPaginationFilters(
                                                        From,
                                                        To,
                                                        Offset,
                                                        Limit
                                                    );

                    var httpResponse              = await NewHTTPClient.GET(
                                                              Path:                  httpClient.RemoteURL.Path + dateAndPaginationFilters.ToHTTPQueryString(),
                                                              Authentication:        TokenAuth,
                                                              RequestBuilder:        requestBuilder => {
                                                                                         requestBuilder.Set("X-Request-ID",     requestId);
                                                                                         requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                                     },
                                                              RequestLogDelegate:    OnGetTariffsHTTPRequest,
                                                              ResponseLogDelegate:   OnGetTariffsHTTPResponse,
                                                              EventTrackingId:       eventTrackingId,
                                                              //NumberOfRetry:         transmissionRetry,
                                                              RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                              CancellationToken:     CancellationToken).

                                                          ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Tariff>.ParseJArray(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Tariff.Parse(json)
                               );

                    Counters.GetTariffs.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, IEnumerable<Tariff>>.Error("No remote URL available!");
                    Counters.GetTariffs.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, IEnumerable<Tariff>>.Exception(e);
                Counters.GetTariffs.IncResponses_Error();
            }


            #region Send OnGetTariffsResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetTariffsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          From,
                          To,
                          Offset,
                          Limit,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region GetTariff         (TariffId, ...)

        /// <summary>
        /// Get the tariff specified by the given tariff identification from the remote API.
        /// </summary>
        /// <param name="TariffId">The identification of the requested tariff.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Tariff>>

            GetTariff(Tariff_Id          TariffId,
                      DateTimeOffset?    TariffTimestamp     = null,
                      TimeSpan?          Tolerance           = null,

                      Version_Id?        VersionId           = null,
                      Request_Id?        RequestId           = null,
                      Correlation_Id?    CorrelationId       = null,

                      DateTimeOffset?    RequestTimestamp    = null,
                      EventTracking_Id?  EventTrackingId     = null,
                      TimeSpan?          RequestTimeout      = null,
                      CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetTariff.IncRequests_OK();

            OCPIResponse<Tariff> response;

            #endregion

            #region Send OnGetTariffRequest event

            await LogEvent(
                      OnGetTariffRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          TariffId,
                          TariffTimestamp,
                          Tolerance,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tariffs,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + TariffId.ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetTariffHTTPRequest,
                                                 ResponseLogDelegate:   OnGetTariffHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Tariff>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Tariff.Parse(json)
                               );

                    Counters.GetTariff.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Tariff>.Error("No remote URL available!");
                    Counters.GetTariff.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Tariff>.Exception(e);
                Counters.GetTariff.IncResponses_Error();
            }


            #region Send OnGetTariffResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetTariffResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          TariffId,
                          TariffTimestamp,
                          Tolerance,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region GetSessions       (...)

        /// <summary>
        /// Get all sessions from the remote API.
        /// </summary>
        /// <param name="From">An optional 'from' timestamp (inclusive).</param>
        /// <param name="To">An optional 'to' timestamp (exclusive).</param>
        /// <param name="Offset">An optional 'offset' within the result set.</param>
        /// <param name="Limit">An optional 'limit' of the result set.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<IEnumerable<Session>>>

            GetSessions(DateTimeOffset?    From                = null,
                        DateTimeOffset?    To                  = null,
                        UInt64?            Offset              = null,
                        UInt64?            Limit               = null,

                        Version_Id?        VersionId           = null,
                        Request_Id?        RequestId           = null,
                        Correlation_Id?    CorrelationId       = null,

                        DateTimeOffset?    RequestTimestamp    = null,
                        EventTracking_Id?  EventTrackingId     = null,
                        TimeSpan?          RequestTimeout      = null,
                        CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetSessions.IncRequests_OK();

            OCPIResponse<IEnumerable<Session>> response;

            #endregion

            #region Send OnGetSessionsRequest event

            await LogEvent(
                      OnGetSessionsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var dateAndPaginationFilters  = new OCPIRequest.DateAndPaginationFilters(
                                                        From,
                                                        To,
                                                        Offset,
                                                        Limit
                                                    );

                    var httpResponse              = await NewHTTPClient.GET(
                                                              Path:                  httpClient.RemoteURL.Path + dateAndPaginationFilters.ToHTTPQueryString(),
                                                              Authentication:        TokenAuth,
                                                              RequestBuilder:        requestBuilder => {
                                                                                         requestBuilder.Set("X-Request-ID",     requestId);
                                                                                         requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                                     },
                                                              RequestLogDelegate:    OnGetSessionsHTTPRequest,
                                                              ResponseLogDelegate:   OnGetSessionsHTTPResponse,
                                                              EventTrackingId:       eventTrackingId,
                                                              //NumberOfRetry:         transmissionRetry,
                                                              RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                              CancellationToken:     CancellationToken).

                                                          ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Session>.ParseJArray(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Session.Parse(json)
                               );

                    // Special error handling
                    if (response.HTTPResponse?.HTTPStatusCode.Code == 400 &&
                       !From.HasValue)
                    {
                        response = OCPIResponse<IEnumerable<Session>>.Error(
                                       response.StatusMessage,
                                      (response.AdditionalInformation is not null
                                           ? response.AdditionalInformation + Environment.NewLine
                                           : "") + "This might be caused by the mandatory OCPI requirement of a 'date_from' HTTP query parameter for GET Sessions requests, as defined in https://github.com/ocpi/ocpi/blob/release-2.2.1-bugfixes/mod_sessions.asciidoc#request-parameters",
                                       response.Timestamp,
                                       response.HTTPResponse,
                                       response.RequestId,
                                       response.CorrelationId
                                   );
                    }

                    Counters.GetSessions.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, IEnumerable<Session>>.Error("No remote URL available!");
                    Counters.GetSessions.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, IEnumerable<Session>>.Exception(e);
                Counters.GetSessions.IncResponses_Error();
            }


            #region Send OnGetSessionsResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetSessionsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region GetSession        (SessionId, ...)

        /// <summary>
        /// Get the session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="SessionId">The identification of the requested session.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional cancellation token to cancel this request.</param>
        public async Task<OCPIResponse<Session>>

            GetSession(Session_Id         SessionId,

                       Version_Id?        VersionId           = null,
                       Request_Id?        RequestId           = null,
                       Correlation_Id?    CorrelationId       = null,

                       DateTimeOffset?    RequestTimestamp    = null,
                       EventTracking_Id?  EventTrackingId     = null,
                       TimeSpan?          RequestTimeout      = null,
                       CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnGetSessionRequest event

            await LogEvent(
                      OnGetSessionRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          SessionId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Sessions,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + SessionId.ToString(),
                                                 Accept:                ocpiAcceptTypes,
                                                 Authentication:        TokenAuth,
                                                 Connection:            ConnectionType.Close,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetSessionHTTPRequest,
                                                 ResponseLogDelegate:   OnGetSessionHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Session>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Session.Parse(json)
                               );

                    Counters.GetSession.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Session>.Error("No remote URL available!");
                    Counters.GetSession.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Session>.Exception(e);
                Counters.GetSession.IncResponses_Error();
            }


            #region Send OnGetSessionResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetSessionResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          SessionId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        // PUT     ~/sessions/{session_id}/charging_preferences


        #region GetCDRs           (...)

        /// <summary>
        /// Get all charge detail records from the remote API.
        /// </summary>
        /// <param name="From">An optional 'from' timestamp (inclusive).</param>
        /// <param name="To">An optional 'to' timestamp (exclusive).</param>
        /// <param name="Offset">An optional 'offset' within the result set.</param>
        /// <param name="Limit">An optional 'limit' of the result set.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<IEnumerable<CDR>>>

            GetCDRs(DateTimeOffset?    From                = null,
                    DateTimeOffset?    To                  = null,
                    UInt64?            Offset              = null,
                    UInt64?            Limit               = null,

                    Version_Id?        VersionId           = null,
                    Request_Id?        RequestId           = null,
                    Correlation_Id?    CorrelationId       = null,

                    DateTimeOffset?    RequestTimestamp    = null,
                    EventTracking_Id?  EventTrackingId     = null,
                    TimeSpan?          RequestTimeout      = null,
                    CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetCDRs.IncRequests_OK();

            OCPIResponse<IEnumerable<CDR>> response;

            #endregion

            #region Send OnGetCDRRequest event

            await LogEvent(
                      OnGetCDRsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var remoteURL = await GetModuleRemoteURL(
                                          Module_Id.CDRs,
                                          InterfaceRoles.SENDER,
                                          VersionId,
                                          eventTrackingId,
                                          CancellationToken
                                      );

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var dateAndPaginationFilters  = new OCPIRequest.DateAndPaginationFilters(
                                                        From,
                                                        To,
                                                        Offset,
                                                        Limit
                                                    );

                    var httpResponse              = await NewHTTPClient.GET(
                                                              Path:                  remoteURL.Value.Path + dateAndPaginationFilters.ToHTTPQueryString(),
                                                              Authentication:        TokenAuth,
                                                              RequestBuilder:        requestBuilder => {
                                                                                         requestBuilder.Set("X-Request-ID",     requestId);
                                                                                         requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                                     },
                                                              RequestLogDelegate:    OnGetCDRsHTTPRequest,
                                                              ResponseLogDelegate:   OnGetCDRsHTTPResponse,
                                                              EventTrackingId:       eventTrackingId,
                                                              //NumberOfRetry:         transmissionRetry,
                                                              RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                              CancellationToken:     CancellationToken).

                                                          ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<CDR>.ParseJArray(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CDR.Parse(json)
                               );

                    Counters.GetCDRs.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, IEnumerable<CDR>>.Error("No remote URL available!");
                    Counters.GetCDRs.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, IEnumerable<CDR>>.Exception(e);
                Counters.GetCDRs.IncResponses_Error();
            }


            #region Send OnGetCDRsResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetCDRsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region GetCDR            (CDRId, ...)  // The concrete URL is not specified by OCPI! m(

        /// <summary>
        /// Get the charge detail record specified by the given location identification from the remote API.
        /// </summary>
        /// <param name="CDRId">The identification of the requested charge detail record.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<CDR>>

            GetCDR(CDR_Id             CDRId,

                   Version_Id?        VersionId           = null,
                   Request_Id?        RequestId           = null,
                   Correlation_Id?    CorrelationId       = null,

                   DateTimeOffset?    RequestTimestamp    = null,
                   EventTracking_Id?  EventTrackingId     = null,
                   TimeSpan?          RequestTimeout      = null,
                   CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetCDR.IncRequests_OK();

            OCPIResponse<CDR> response;

            #endregion

            #region Send OnGetCDRRequest event

            await LogEvent(
                      OnGetCDRRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CDRId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.CDRs,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CDRId.ToString(),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetCDRHTTPRequest,
                                                 ResponseLogDelegate:   OnGetCDRHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<CDR>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CDR.Parse(json)
                               );

                    Counters.GetCDR.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, CDR>.Error("No remote URL available!");
                    Counters.GetCDR.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, CDR>.Exception(e);
                Counters.GetCDR.IncResponses_Error();
            }


            #region Send OnGetCDRResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetCDRResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CDRId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region GetToken          (CountryCode, PartyId, TokenId, ...)

        /// <summary>
        /// Get the token specified by the given token identification from the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<Token>>

            GetToken(CountryCode        CountryCode,
                     Party_Id           PartyId,
                     Token_Id           TokenId,

                     Request_Id?        RequestId           = null,
                     Correlation_Id?    CorrelationId       = null,
                     Version_Id?        VersionId           = null,

                     DateTimeOffset?    RequestTimestamp    = null,
                     EventTracking_Id?  EventTrackingId     = null,
                     TimeSpan?          RequestTimeout      = null,
                     CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.GetToken.IncRequests_OK();

            OCPIResponse<Token> response;

            #endregion

            #region Send OnGetTokenRequest event

            await LogEvent(
                      OnGetTokenRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TokenId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Tokens,
                                           InterfaceRoles.SENDER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.GET(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    TokenId.    ToString(),
                                                 Accept:                ocpiAcceptTypes,
                                                 Authentication:        TokenAuth,
                                                 Connection:            ConnectionType.Close,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetTokenHTTPRequest,
                                                 ResponseLogDelegate:   OnGetTokenHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Token>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Token.Parse(json)
                               );

                    Counters.GetToken.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Token>.Error("No remote URL available!");
                    Counters.GetToken.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Token>.Exception(e);
                Counters.GetToken.IncResponses_Error();
            }


            #region Send OnGetTokenResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnGetTokenResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TokenId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PutToken          (Token, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Token">The token to store/put at/onto the remote API.</param>
        /// 
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<Token>>

            PutToken(Token              Token,

                     Request_Id?        RequestId           = null,
                     Correlation_Id?    CorrelationId       = null,
                     Version_Id?        VersionId           = null,

                     DateTimeOffset?    RequestTimestamp    = null,
                     EventTracking_Id?  EventTrackingId     = null,
                     TimeSpan?          RequestTimeout      = null,
                     CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PutToken.IncRequests_OK();

            OCPIResponse<Token> response;

            #endregion

            #region Send OnPutTokenRequest event

            await LogEvent(
                      OnPutTokenRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          Token,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var remoteURL = await GetModuleRemoteURL(
                                          Module_Id.Tokens,
                                          InterfaceRoles.SENDER,
                                          VersionId,
                                          eventTrackingId,
                                          CancellationToken
                                      );

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.PUT(
                                                 Path:                  remoteURL.Value.Path + Token.CountryCode.ToString() +
                                                                                               Token.PartyId.    ToString() +
                                                                                               Token.Id.         ToString(),
                                                 Content:               Token.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPutTokenHTTPRequest,
                                                 ResponseLogDelegate:   OnPutTokenHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Token>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Token.Parse(json)
                               );

                    Counters.PutToken.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Token>.Error("No remote URL available!");
                    Counters.PutToken.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Token>.Exception(e);
                Counters.PutToken.IncResponses_Error();
            }


            #region Send OnPutTokenResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPutTokenResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          Token,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PatchToken        (CountryCode, PartyId, TokenId, ...)

        /// <summary>
        /// Start a charging token.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<Token>>

            PatchToken(CountryCode         CountryCode,
                       Party_Id            PartyId,
                       Token_Id            TokenId,
                       JObject             TokenPatch,

                       Version_Id?         VersionId           = null,
                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,

                       DateTimeOffset?     RequestTimestamp    = null,
                       EventTracking_Id?   EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null,
                       CancellationToken   CancellationToken   = default)

        {

            #region Initial checks

            if (!TokenPatch.HasValues)
                return OCPIResponse<Token>.Error("The given token patch must not be null!");

            #endregion

            #region Init

            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.PatchToken.IncRequests_OK();

            OCPIResponse<Token> response;

            #endregion

            #region Send OnPatchTokenRequest event

            await LogEvent(
                      OnPatchTokenRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TokenId,
                          TokenPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient  = await GetModuleHTTPClient(
                                            Module_Id.Tokens,
                                            InterfaceRoles.RECEIVER,
                                            VersionId,
                                            eventTrackingId,
                                            CancellationToken
                                        );

                if (httpClient is not null)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.PATCH(
                                                 Path:                  httpClient.RemoteURL.Path + CountryCode.ToString() +
                                                                                                    PartyId.    ToString() +
                                                                                                    TokenId.    ToString(),
                                                 Content:               TokenPatch.ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnPatchTokenHTTPRequest,
                                                 ResponseLogDelegate:   OnPatchTokenHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Token>.ParseJObject(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => Token.Parse(json)
                               );

                    Counters.PatchToken.IncResponses_OK();

                }

                else
                {
                    response = OCPIResponse<String, Token>.Error("No remote URL available!");
                    Counters.PatchToken.IncRequests_Error();
                }

            }

            catch (Exception e)
            {
                response = OCPIResponse<String, Token>.Exception(e);
                Counters.PatchToken.IncResponses_Error();
            }


            #region Send OnPatchTokenResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnPatchTokenResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          CountryCode,
                          PartyId,
                          TokenId,
                          TokenPatch,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        // Commands

        #region ReserveNow        (Token, ExpirationTimestamp, ReservationId, LocationId, EVSEUId, AuthorizationReference, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<ReserveNowCommand, CommandResponse>>

            ReserveNow(Token                    Token,
                       DateTimeOffset           ExpirationTimestamp,
                       Reservation_Id           ReservationId,
                       Location_Id              LocationId,
                       EVSE_UId?                EVSEUId                  = null,
                       AuthorizationReference?  AuthorizationReference   = null,

                       Command_Id?              CommandId                = null,
                       Request_Id?              RequestId                = null,
                       Correlation_Id?          CorrelationId            = null,
                       Version_Id?              VersionId                = null,

                       DateTimeOffset?          RequestTimestamp         = null,
                       EventTracking_Id?        EventTrackingId          = null,
                       TimeSpan?                RequestTimeout           = null,
                       CancellationToken        CancellationToken        = default)

        {

            #region Init

            var commandId         = CommandId        ?? Command_Id.    NewRandom();
            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.ReserveNow.IncRequests_OK();

            OCPIResponse<ReserveNowCommand, CommandResponse> response;

            #endregion

            #region Send OnReserveNowRequest event

            await LogEvent(
                      OnReserveNowRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          Token,
                          ExpirationTimestamp,
                          ReservationId,
                          LocationId,
                          EVSEUId,
                          AuthorizationReference,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient  = await GetModuleHTTPClient(
                                            Module_Id.Commands,
                                            InterfaceRoles.RECEIVER,
                                            VersionId,
                                            eventTrackingId,
                                            CancellationToken
                                        );

                if (httpClient is not null)
                {

                    var command = new ReserveNowCommand(
                                      Token,
                                      ExpirationTimestamp,
                                      ReservationId,
                                      LocationId,
                                      CommonAPI.GetModuleURL(
                                          Module_Id.Commands,
                                          "v" + SelectedOCPIVersionId.ToString() + "/emsp"
                                      ) + "RESERVE_NOW" + commandId.ToString(),
                                      EVSEUId,
                                      AuthorizationReference,
                                      commandId,
                                      requestId,
                                      correlationId
                                  );

                    CommonAPI.CommandValueStore.AddOrUpdate(
                        commandId,
                        (id)    => CommandValues.FromCommand        (command),
                        (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand)
                    );


                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.POST(
                                                 Path:                  httpClient.RemoteURL.Path + "RESERVE_NOW",
                                                 Content:               command.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnReserveNowHTTPRequest,
                                                 ResponseLogDelegate:   OnReserveNowHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<ReserveNowCommand, CommandResponse>.ParseJObject(
                                   command,
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CommandResponse.Parse(
                                               command,
                                               json
                                           )
                               );

                    Counters.ReserveNow.IncResponses_OK();

                }
                else
                {
                    response = OCPIResponse<ReserveNowCommand, CommandResponse>.Error("No remote URL available!");
                    Counters.ReserveNow.IncRequests_Error();
                }

                if (CommonAPI.CommandValueStore.TryGetValue(commandId, out var commandValues))
                    commandValues.Response = response.Data;

            }
            catch (Exception e)
            {
                response = OCPIResponse<ReserveNowCommand, CommandResponse>.Exception(e);
                Counters.ReserveNow.IncResponses_Error();
            }


            #region Send OnReserveNowResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnReserveNowResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          Token,
                          ExpirationTimestamp,
                          ReservationId,
                          LocationId,
                          EVSEUId,
                          AuthorizationReference,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation (ReservationId, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<CancelReservationCommand, CommandResponse>>

            CancelReservation(Reservation_Id      ReservationId,

                              Command_Id?         CommandId           = null,
                              Request_Id?         RequestId           = null,
                              Correlation_Id?     CorrelationId       = null,
                              Version_Id?         VersionId           = null,

                              DateTimeOffset?    RequestTimestamp    = null,
                              EventTracking_Id?  EventTrackingId     = null,
                              TimeSpan?          RequestTimeout      = null,
                              CancellationToken  CancellationToken   = default)

        {

            #region Init

            var commandId         = CommandId        ?? Command_Id.    NewRandom();
            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.CancelReservation.IncRequests_OK();

            OCPIResponse<CancelReservationCommand, CommandResponse> response;

            #endregion

            #region Send OnCancelReservationRequest event

            await LogEvent(
                      OnCancelReservationRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          ReservationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion

            try
            {

                var httpClient  = await GetModuleHTTPClient(
                                            Module_Id.Commands,
                                            InterfaceRoles.RECEIVER,
                                            VersionId,
                                            eventTrackingId,
                                            CancellationToken
                                        );

                if (httpClient is not null)
                {

                    var command = new CancelReservationCommand(
                                      ReservationId,
                                      CommonAPI.GetModuleURL(
                                          Module_Id.Commands,
                                          "v" + SelectedOCPIVersionId.ToString() + "/emsp"
                                      ) + "CANCEL_RESERVATION" + commandId.ToString(),
                                      commandId,
                                      requestId,
                                      correlationId
                                  );

                    CommonAPI.CommandValueStore.AddOrUpdate(
                        commandId,
                        (id)    => CommandValues.FromCommand(command),
                        (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand)
                    );


                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.POST(
                                                 Path:                  httpClient.RemoteURL.Path + "CANCEL_RESERVATION",
                                                 Content:               command.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnCancelReservationHTTPRequest,
                                                 ResponseLogDelegate:   OnCancelReservationHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<CancelReservationCommand, CommandResponse>.ParseJObject(
                                   command,
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CommandResponse.Parse(
                                               command,
                                               json
                                           )
                               );

                    Counters.CancelReservation.IncResponses_OK();

                }
                else
                {
                    response = OCPIResponse<CancelReservationCommand, CommandResponse>.Error("No remote URL available!");
                    Counters.CancelReservation.IncRequests_Error();
                }

                if (CommonAPI.CommandValueStore.TryGetValue(commandId, out var commandValues))
                    commandValues.Response = response.Data;

            }
            catch (Exception e)
            {
                response = OCPIResponse<CancelReservationCommand, CommandResponse>.Exception(e);
                Counters.CancelReservation.IncResponses_Error();
            }


            #region Send OnCancelReservationResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnCancelReservationResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          ReservationId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region StartSession      (Token, LocationId, EVSEUId, AuthorizationReference, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<StartSessionCommand, CommandResponse>>

            StartSession(Token                    Token,
                         Location_Id              LocationId,
                         EVSE_UId?                EVSEUId                  = null,
                         Connector_Id?            ConnectorId              = null,
                         AuthorizationReference?  AuthorizationReference   = null,

                         Command_Id?              CommandId                = null,
                         Request_Id?              RequestId                = null,
                         Correlation_Id?          CorrelationId            = null,
                         Version_Id?              VersionId                = null,

                         DateTimeOffset?          RequestTimestamp         = null,
                         EventTracking_Id?        EventTrackingId          = null,
                         TimeSpan?                RequestTimeout           = null,
                         CancellationToken        CancellationToken        = default)

        {

            #region Init

            var commandId         = CommandId        ?? Command_Id.    NewRandom();
            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.StartSession.IncRequests_OK();

            OCPIResponse<StartSessionCommand, CommandResponse> response;

            #endregion

            #region Send OnStartSessionRequest event

            await LogEvent(
                      OnStartSessionRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          Token,
                          LocationId,
                          EVSEUId,
                          ConnectorId,
                          AuthorizationReference,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient = await GetModuleHTTPClient(
                                           Module_Id.Commands,
                                           InterfaceRoles.RECEIVER,
                                           VersionId,
                                           eventTrackingId,
                                           CancellationToken
                                       );

                if (httpClient is not null)
                {

                    var command    = new StartSessionCommand(
                                         Token,
                                         LocationId,
                                         CommonAPI.GetModuleURL(
                                             Module_Id.Commands,
                                             "v" + SelectedOCPIVersionId.ToString() + "/emsp"
                                         ) + "START_SESSION" + commandId.ToString(),
                                         EVSEUId,
                                         ConnectorId,
                                         AuthorizationReference,
                                         commandId,
                                         requestId,
                                         correlationId
                                     );

                    CommonAPI.CommandValueStore.AddOrUpdate(
                        commandId,
                        (id)    => CommandValues.FromCommand(command),
                        (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand)
                    );


                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.POST(
                                                 Path:                  httpClient.RemoteURL.Path + "START_SESSION",
                                                 Content:               command.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnStartSessionHTTPRequest,
                                                 ResponseLogDelegate:   OnStartSessionHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<StartSessionCommand, CommandResponse>.ParseJObject(
                                   command,
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CommandResponse.Parse(
                                               command,
                                               json
                                           )
                               );

                    Counters.StartSession.IncResponses_OK();

                }
                else
                {
                    response = OCPIResponse<StartSessionCommand, CommandResponse>.Error("No remote URL available!");
                    Counters.StartSession.IncRequests_Error();
                }

                if (CommonAPI.CommandValueStore.TryGetValue(commandId, out var commandValues))
                    commandValues.Response = response.Data;

            }
            catch (Exception e)
            {
                response = OCPIResponse<StartSessionCommand, CommandResponse>.Exception(e);
                Counters.StartSession.IncResponses_Error();
            }


            #region Send OnStartSessionResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnStartSessionResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          Token,
                          LocationId,
                          EVSEUId,
                          ConnectorId,
                          AuthorizationReference,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region StopSession       (SessionId, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<StopSessionCommand, CommandResponse>>

            StopSession(Session_Id          SessionId,

                        Command_Id?         CommandId           = null,
                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,
                        Version_Id?         VersionId           = null,

                        DateTimeOffset?    RequestTimestamp    = null,
                        EventTracking_Id?  EventTrackingId     = null,
                        TimeSpan?          RequestTimeout      = null,
                        CancellationToken  CancellationToken   = default)

        {

            #region Init

            var commandId         = CommandId        ?? Command_Id.    NewRandom();
            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.StopSession.IncRequests_OK();

            OCPIResponse<StopSessionCommand, CommandResponse> response;

            #endregion

            #region Send OnStopSessionRequest event

            await LogEvent(
                      OnStopSessionRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          SessionId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient  = await GetModuleHTTPClient(
                                            Module_Id.Commands,
                                            InterfaceRoles.RECEIVER,
                                            VersionId,
                                            eventTrackingId,
                                            CancellationToken
                                        );

                if (httpClient is not null)
                {

                    var command = new StopSessionCommand(
                                      SessionId,
                                      CommonAPI.GetModuleURL(
                                          Module_Id.Commands,
                                          SelectedOCPIVersionId.ToString() + "/emsp"
                                      ) + "STOP_SESSION" + commandId.ToString(),
                                      commandId,
                                      requestId,
                                      correlationId
                                  );

                    CommonAPI.CommandValueStore.AddOrUpdate(
                        commandId,
                        (id)    => CommandValues.FromCommand(command),
                        (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand)
                    );


                    #region Upstream HTTP request...

                    var httpResponse = await NewHTTPClient.POST(
                                                 Path:                  httpClient.RemoteURL.Path + "STOP_SESSION",
                                                 Content:               command.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnStopSessionHTTPRequest,
                                                 ResponseLogDelegate:   OnStopSessionHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<StopSessionCommand, CommandResponse>.ParseJObject(
                                   command,
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CommandResponse.Parse(
                                               command,
                                               json
                                           )
                               );

                    Counters.StopSession.IncResponses_OK();

                }
                else
                {
                    response = OCPIResponse<StopSessionCommand, CommandResponse>.Error("No remote URL available!");
                    Counters.StopSession.IncRequests_Error();
                }

                if (CommonAPI.CommandValueStore.TryGetValue(commandId, out var commandValues))
                    commandValues.Response = response.Data;

            }
            catch (Exception e)
            {
                response = OCPIResponse<StopSessionCommand, CommandResponse>.Exception(e);
                Counters.StopSession.IncResponses_Error();
            }


            #region Send OnStopSessionResponse event

            var endtime = Timestamp.Now;

            await LogEvent(
                      OnStopSessionResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          SessionId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector   (LocationId, EVSEUId, ConnectorId, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<UnlockConnectorCommand, CommandResponse>>

            UnlockConnector(Location_Id        LocationId,
                            EVSE_UId           EVSEUId,
                            Connector_Id       ConnectorId,

                            Command_Id?        CommandId           = null,
                            Request_Id?        RequestId           = null,
                            Correlation_Id?    CorrelationId       = null,
                            Version_Id?        VersionId           = null,

                            DateTimeOffset?    RequestTimestamp    = null,
                            EventTracking_Id?  EventTrackingId     = null,
                            TimeSpan?          RequestTimeout      = null,
                            CancellationToken  CancellationToken   = default)

        {

            #region Init

            var commandId         = CommandId        ?? Command_Id.    NewRandom();
            var requestId         = RequestId        ?? Request_Id.    NewRandom();
            var correlationId     = CorrelationId    ?? Correlation_Id.NewRandom();

            var requestTimestamp  = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId   = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout    = RequestTimeout   ?? this.RequestTimeout;

            var startTime         = Timestamp.Now;
            var stopwatch         = Stopwatch.StartNew();

            Counters.UnlockConnector.IncRequests_OK();

            OCPIResponse<UnlockConnectorCommand, CommandResponse> response;

            #endregion

            #region Send OnUnlockConnectorRequest event

            await LogEvent(
                      OnUnlockConnectorRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          EVSEUId,
                          ConnectorId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout
                      )
                  );

            #endregion


            try
            {

                var httpClient  = await GetModuleHTTPClient(
                                            Module_Id.Commands,
                                            InterfaceRoles.RECEIVER,
                                            VersionId,
                                            eventTrackingId,
                                            CancellationToken
                                        );

                if (httpClient is not null)
                {

                    var command = new UnlockConnectorCommand(
                                      LocationId,
                                      EVSEUId,
                                      ConnectorId,
                                      CommonAPI.GetModuleURL(
                                          Module_Id.Commands,
                                          "v" + SelectedOCPIVersionId.ToString() + "/emsp"
                                      ) + "UNLOCK_CONNECTOR" + commandId.ToString(),
                                      commandId,
                                      requestId,
                                      correlationId
                                  );

                    CommonAPI.CommandValueStore.AddOrUpdate(
                        commandId,
                        (id)    => CommandValues.FromCommand(command),
                        (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand)
                    );


                    #region Upstream HTTP request...

                    var httpResponse = await httpClient.POST(
                                                 Path:                  httpClient.RemoteURL.Path + "UNLOCK_CONNECTOR",
                                                 Content:               command.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnUnlockConnectorHTTPRequest,
                                                 ResponseLogDelegate:   OnUnlockConnectorHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 //NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<UnlockConnectorCommand, CommandResponse>.ParseJObject(
                                   command,
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => CommandResponse.Parse(
                                               command,
                                               json
                                           )
                               );

                    Counters.UnlockConnector.IncResponses_OK();

                }
                else
                {
                    response = OCPIResponse<UnlockConnectorCommand, CommandResponse>.Error("No remote URL available!");
                    Counters.UnlockConnector.IncRequests_Error();
                }

                if (CommonAPI.CommandValueStore.TryGetValue(commandId, out var commandValues))
                    commandValues.Response = response.Data;

            }
            catch (Exception e)
            {
                response = OCPIResponse<UnlockConnectorCommand, CommandResponse>.Exception(e);
                Counters.UnlockConnector.IncResponses_Error();
            }


            #region Send OnUnlockConnectorResponse event

            stopwatch.Stop();
            var endtime = Timestamp.Now;

            await LogEvent(
                      OnUnlockConnectorResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          this,
                          requestId,
                          correlationId,

                          LocationId,
                          EVSEUId,
                          ConnectorId,

                          CancellationToken,
                          eventTrackingId,
                          requestTimeout,

                          response,
                          stopwatch.Elapsed
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        //ToDo: Add smart charging commands!


        #region (private) LogEvent (Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPICommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(HUB2CPOClient),
                   Logger,
                   LogHandler,
                   EventName,
                   OCPICommand
               );

        #endregion


        #region ToJSON()

        public override JObject ToJSON()
            => base.ToJSON(nameof(HUB2CPOClient));

        #endregion

        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion


    }

}
