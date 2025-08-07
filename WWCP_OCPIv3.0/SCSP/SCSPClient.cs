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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.SCSP.HTTP
{

    /// <summary>
    /// The OCPI SCSP client.
    /// </summary>
    public partial class SCSPClient : CommonClient
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


        #region Properties

        /// <summary>
        /// SCSP client event counters.
        /// </summary>
        public new APICounters  Counters      { get; }

        /// <summary>
        /// The SCSP client (HTTP client) logger.
        /// </summary>
        public new Logger       HTTPLogger
        {
            get
            {
                return base.HTTPLogger as Logger;
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
        public event OnGetSessionsRequestDelegate?   OnGetSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a session by it identification will be send.
        /// </summary>
        public event ClientRequestLogHandler?        OnGetSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a session by it identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?       OnGetSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a session by it identification request had been received.
        /// </summary>
        public event OnGetSessionsResponseDelegate?  OnGetSessionResponse;

        #endregion


        // Commands

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SCSP client.
        /// </summary>
        /// <param name="CommonAPI">The CommonAPI.</param>
        /// <param name="RemoteVersionsURL">The remote URL of the VERSIONS endpoint to connect to.</param>
        /// <param name="AccessToken">The access token.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this CPO client.</param>
        /// <param name="RemoteCertificateValidator">The remote TLS certificate validator.</param>
        /// <param name="ClientCert">The TLS client certificate to use of HTTP authentication.</param>
        /// <param name="Accept">The optional HTTP accept header.</param>
        /// <param name="Authentication">The optional HTTP authentication to use, e.g. HTTP Basic Auth.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public SCSPClient(CommonAPI                                                  CommonAPI,
                          URL                                                        RemoteVersionsURL,
                          AccessToken                                                AccessToken,
                          HTTPHostname?                                              VirtualHostname              = null,
                          I18NString?                                                Description                  = null,
                          Boolean?                                                   PreferIPv4                   = null,
                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                          X509Certificate?                                           ClientCert                   = null,
                          SslProtocols?                                              TLSProtocol                  = null,
                          AcceptTypes?                                               Accept                       = null,
                          IHTTPAuthentication?                                       Authentication               = null,
                          String?                                                    HTTPUserAgent                = DefaultHTTPUserAgent,
                          TimeSpan?                                                  RequestTimeout               = null,
                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                          UInt16?                                                    MaxNumberOfRetries           = null,
                          UInt32?                                                    InternalBufferSize           = null,
                          Boolean                                                    UseHTTPPipelining            = false,
                          HTTPClientLogger?                                          HTTPLogger                   = null,
                          Boolean                                                    AccessTokenBase64Encoding    = true,

                          Boolean?                                                   DisableLogging               = false,
                          String?                                                    LoggingPath                  = null,
                          String?                                                    LoggingContext               = null,
                          LogfileCreatorDelegate?                                    LogfileCreator               = null,
                          DNSClient?                                                 DNSClient                    = null)

            : base(CommonAPI,
                   RemoteVersionsURL,
                   AccessToken,
                   VirtualHostname,
                   Description,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   Accept,
                   Authentication,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,
                   HTTPLogger,
                   AccessTokenBase64Encoding,

                   DisableLogging,
                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   DNSClient)

        {

            this.Counters    = new APICounters();

            base.HTTPLogger  = this.DisableLogging == false
                                   ? new Logger(
                                         this,
                                         LoggingPath,
                                         LoggingContext,
                                         LogfileCreator
                                     )
                                   : null;

        }

        #endregion

        public override JObject ToJSON()
            => base.ToJSON(nameof(SCSPClient));


        #region GetLocations(...)

        /// <summary>
        /// Get all locations from the remote API.
        /// </summary>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Location>>>

            GetLocations(Version_Id?         VersionId           = null,
                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,

                         CancellationToken   CancellationToken   = default,
                         EventTracking_Id?   EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetLocations.IncRequests_OK();

            OCPIResponse<IEnumerable<Location>> response;

            #endregion

            #region Send OnGetLocationsRequest event

            try
            {

                if (OnGetLocationsRequest is not null)
                    await Task.WhenAll(OnGetLocationsRequest.GetInvocationList().
                                       Cast<OnGetLocationsRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetLocationsRequest));
            }

            #endregion


            try
            {

                var remoteURL = await GetRemoteURL(Module_Id.Locations,
                                                   InterfaceRoles.SENDER,
                                                   VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             PreferIPv4,
                                                             RemoteCertificateValidator,
                                                             LocalCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocols,
                                                             ContentType,
                                                             Accept,
                                                             Authentication,
                                                             HTTPUserAgent,
                                                             Connection,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             InternalBufferSize,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path,
                                                                                     RequestBuilder: requestBuilder => {
                                                                                         requestBuilder.Authorization  = TokenAuth;
                                                                                         requestBuilder.Connection     = ConnectionType.Close;
                                                                                         requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                         requestBuilder.Set("X-Request-ID",      requestId);
                                                                                         requestBuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnGetLocationsHTTPRequest,
                                                      ResponseLogDelegate:  OnGetLocationsHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      eventTrackingId,
                                                      RequestTimeout:       requestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Location>.ParseJArray(httpResponse,
                                                                  requestId,
                                                                  correlationId,
                                                                  json => Location.Parse(json));

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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetLocationsResponse is not null)
                    await Task.WhenAll(OnGetLocationsResponse.GetInvocationList().
                                       Cast<OnGetLocationsResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetLocationsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLocation (LocationId, ...)

        /// <summary>
        /// Get the location specified by the given location identification from the remote API.
        /// </summary>
        /// <param name="LocationId">The identification of the requested location.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            GetLocation(Location_Id         LocationId,

                        Version_Id?         VersionId           = null,
                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,

                        CancellationToken   CancellationToken   = default,
                        EventTracking_Id?   EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetLocation.IncRequests_OK();

            OCPIResponse<Location> response;

            #endregion

            #region Send OnGetLocationRequest event

            try
            {

                if (OnGetLocationRequest is not null)
                    await Task.WhenAll(OnGetLocationRequest.GetInvocationList().
                                       Cast<OnGetLocationRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetLocationRequest));
            }

            #endregion


            try
            {

                var remoteURL = await GetRemoteURL(Module_Id.Locations,
                                                   InterfaceRoles.SENDER,
                                                   VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             PreferIPv4,
                                                             RemoteCertificateValidator,
                                                             LocalCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocols,
                                                             ContentType,
                                                             Accept,
                                                             Authentication,
                                                             HTTPUserAgent,
                                                             Connection,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             InternalBufferSize,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                            Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                   remoteURL.Value.Path + LocationId.ToString(),
                                                                                   RequestBuilder: requestBuilder => {
                                                                                       requestBuilder.Authorization  = TokenAuth;
                                                                                       requestBuilder.Connection     = ConnectionType.Close;
                                                                                       requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                       requestBuilder.Set("X-Request-ID",      requestId);
                                                                                       requestBuilder.Set("X-Correlation-ID",  correlationId);
                                                                                   }),

                                                  RequestLogDelegate:   OnGetLocationHTTPRequest,
                                                  ResponseLogDelegate:  OnGetLocationHTTPResponse,
                                                  CancellationToken:    CancellationToken,
                                                  EventTrackingId:      eventTrackingId,
                                                  RequestTimeout:       requestTimeout).

                                            ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Location>.ParseJObject(httpResponse,
                                                                   requestId,
                                                                   correlationId,
                                                                   json => Location.Parse(json));

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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetLocationResponse is not null)
                    await Task.WhenAll(OnGetLocationResponse.GetInvocationList().
                                       Cast<OnGetLocationResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetEVSE     (LocationId, EVSEUId, ...)

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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<EVSE>>

            GetEVSE(Location_Id         LocationId,
                    EVSE_UId            EVSEUId,

                    Version_Id?         VersionId           = null,
                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,

                    CancellationToken   CancellationToken   = default,
                    EventTracking_Id?   EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetEVSE.IncRequests_OK();

            OCPIResponse<EVSE> response;

            #endregion

            #region Send OnGetEVSERequest event

            try
            {

                if (OnGetEVSERequest is not null)
                    await Task.WhenAll(OnGetEVSERequest.GetInvocationList().
                                       Cast<OnGetEVSERequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetEVSERequest));
            }

            #endregion


            try
            {

                var remoteURL = await GetRemoteURL(Module_Id.Locations,
                                                   InterfaceRoles.SENDER,
                                                   VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             PreferIPv4,
                                                             RemoteCertificateValidator,
                                                             LocalCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocols,
                                                             ContentType,
                                                             Accept,
                                                             Authentication,
                                                             HTTPUserAgent,
                                                             Connection,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             InternalBufferSize,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                            Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                   remoteURL.Value.Path + LocationId.ToString() + EVSEUId.ToString(),
                                                                                   RequestBuilder: requestBuilder => {
                                                                                       requestBuilder.Authorization  = TokenAuth;
                                                                                       requestBuilder.Connection     = ConnectionType.Close;
                                                                                       requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                       requestBuilder.Set("X-Request-ID",      requestId);
                                                                                       requestBuilder.Set("X-Correlation-ID",  correlationId);
                                                                                   }),

                                                  RequestLogDelegate:   OnGetEVSEHTTPRequest,
                                                  ResponseLogDelegate:  OnGetEVSEHTTPResponse,
                                                  CancellationToken:    CancellationToken,
                                                  EventTrackingId:      eventTrackingId,
                                                  RequestTimeout:       requestTimeout).

                                            ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<EVSE>.ParseJObject(httpResponse,
                                                               requestId,
                                                               correlationId,
                                                               json => EVSE.Parse(json));

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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetEVSEResponse is not null)
                    await Task.WhenAll(OnGetEVSEResponse.GetInvocationList().
                                       Cast<OnGetEVSEResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetEVSEResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetConnector(LocationId, EVSEUId, ConnectorId, ...)

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
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Connector>>

            GetConnector(Location_Id         LocationId,
                         EVSE_UId            EVSEUId,
                         Connector_Id        ConnectorId,

                         Version_Id?         VersionId           = null,
                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,

                         CancellationToken   CancellationToken   = default,
                         EventTracking_Id?   EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetConnector.IncRequests_OK();

            OCPIResponse<Connector> response;

            #endregion

            #region Send OnGetConnectorRequest event

            try
            {

                if (OnGetConnectorRequest is not null)
                    await Task.WhenAll(OnGetConnectorRequest.GetInvocationList().
                                       Cast<OnGetConnectorRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     LocationId,
                                                     EVSEUId,
                                                     ConnectorId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetConnectorRequest));
            }

            #endregion


            try
            {

                var remoteURL = await GetRemoteURL(Module_Id.Locations,
                                                   InterfaceRoles.SENDER,
                                                   VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             PreferIPv4,
                                                             RemoteCertificateValidator,
                                                             LocalCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocols,
                                                             ContentType,
                                                             Accept,
                                                             Authentication,
                                                             HTTPUserAgent,
                                                             Connection,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             InternalBufferSize,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                            Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                   remoteURL.Value.Path + LocationId.ToString() + EVSEUId.ToString() + ConnectorId.ToString(),
                                                                                   RequestBuilder: requestBuilder => {
                                                                                       requestBuilder.Authorization  = TokenAuth;
                                                                                       requestBuilder.Connection     = ConnectionType.Close;
                                                                                       requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                       requestBuilder.Set("X-Request-ID",      requestId);
                                                                                       requestBuilder.Set("X-Correlation-ID",  correlationId);
                                                                                   }),

                                                  RequestLogDelegate:   OnGetConnectorHTTPRequest,
                                                  ResponseLogDelegate:  OnGetConnectorHTTPResponse,
                                                  CancellationToken:    CancellationToken,
                                                  EventTrackingId:      eventTrackingId,
                                                  RequestTimeout:       requestTimeout).

                                            ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Connector>.ParseJObject(httpResponse,
                                                                    requestId,
                                                                    correlationId,
                                                                    json => Connector.Parse(json));

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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetConnectorResponse is not null)
                    await Task.WhenAll(OnGetConnectorResponse.GetInvocationList().
                                       Cast<OnGetConnectorResponseDelegate>().
                                       Select(e => e(endtime,
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
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetSessions   (...)

        /// <summary>
        /// Get all sessions from the remote API.
        /// </summary>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Session>>>

            GetSessions(Version_Id?         VersionId           = null,
                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,

                        CancellationToken   CancellationToken   = default,
                        EventTracking_Id?   EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetSessions.IncRequests_OK();

            OCPIResponse<IEnumerable<Session>> response;

            #endregion

            #region Send OnGetSessionRequest event

            try
            {

                if (OnGetSessionsRequest is not null)
                    await Task.WhenAll(OnGetSessionsRequest.GetInvocationList().
                                       Cast<OnGetSessionsRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetSessionsRequest));
            }

            #endregion


            try
            {

                var remoteURL = await GetRemoteURL(Module_Id.Sessions,
                                                   InterfaceRoles.SENDER,
                                                   VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             PreferIPv4,
                                                             RemoteCertificateValidator,
                                                             LocalCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocols,
                                                             ContentType,
                                                             Accept,
                                                             Authentication,
                                                             HTTPUserAgent,
                                                             Connection,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             InternalBufferSize,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                            Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                   remoteURL.Value.Path,
                                                                                   RequestBuilder: requestBuilder => {
                                                                                       requestBuilder.Authorization  = TokenAuth;
                                                                                       requestBuilder.Connection     = ConnectionType.Close;
                                                                                       requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                       requestBuilder.Set("X-Request-ID",      requestId);
                                                                                       requestBuilder.Set("X-Correlation-ID",  correlationId);
                                                                                   }),

                                                  RequestLogDelegate:   OnGetSessionsHTTPRequest,
                                                  ResponseLogDelegate:  OnGetSessionsHTTPResponse,
                                                  CancellationToken:    CancellationToken,
                                                  EventTrackingId:      eventTrackingId,
                                                  RequestTimeout:       requestTimeout).

                                            ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Session>.ParseJArray(httpResponse,
                                                                 requestId,
                                                                 correlationId,
                                                                 json => Session.Parse(json));

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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetSessionsResponse is not null)
                    await Task.WhenAll(OnGetSessionsResponse.GetInvocationList().
                                       Cast<OnGetSessionsResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetSessionsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetSession(SessionId, ...)

        /// <summary>
        /// Get the session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="SessionId">The identification of the requested session.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Session>>

            GetSession(Session_Id          SessionId,

                       Version_Id?         VersionId           = null,
                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,

                       CancellationToken   CancellationToken   = default,
                       EventTracking_Id?   EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            #region Init

            var startTime        = Timestamp.Now;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? this.RequestTimeout;

            Counters.GetSession.IncRequests_OK();

            OCPIResponse<Session> response;

            #endregion

            #region Send OnGetSessionRequest event

            try
            {

                if (OnGetSessionRequest is not null)
                    await Task.WhenAll(OnGetSessionRequest.GetInvocationList().
                                       Cast<OnGetSessionRequestDelegate>().
                                       Select(e => e(startTime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     SessionId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetSessionRequest));
            }

            #endregion


            try
            {

                var remoteURL = await GetRemoteURL(Module_Id.Sessions,
                                                   InterfaceRoles.SENDER,
                                                   VersionId);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var httpResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             PreferIPv4,
                                                             RemoteCertificateValidator,
                                                             LocalCertificateSelector,
                                                             ClientCert,
                                                             TLSProtocols,
                                                             ContentType,
                                                             Accept,
                                                             Authentication,
                                                             HTTPUserAgent,
                                                             Connection,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             InternalBufferSize,
                                                             UseHTTPPipelining,
                                                             DisableLogging,
                                                             HTTPLogger,
                                                             DNSClient).

                                            Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                   remoteURL.Value.Path + SessionId.ToString(),
                                                                                   RequestBuilder: requestBuilder => {
                                                                                       requestBuilder.Authorization  = TokenAuth;
                                                                                       requestBuilder.Connection     = ConnectionType.Close;
                                                                                       requestBuilder.Accept.Add(HTTPContentType.Application.JSON_UTF8);
                                                                                       requestBuilder.Set("X-Request-ID",      requestId);
                                                                                       requestBuilder.Set("X-Correlation-ID",  correlationId);
                                                                                   }),

                                                  RequestLogDelegate:   OnGetSessionHTTPRequest,
                                                  ResponseLogDelegate:  OnGetSessionHTTPResponse,
                                                  CancellationToken:    CancellationToken,
                                                  EventTrackingId:      eventTrackingId,
                                                  RequestTimeout:       requestTimeout).

                                            ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Session>.ParseJObject(httpResponse,
                                                                  requestId,
                                                                  correlationId,
                                                                  json => Session.Parse(json));

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

            var endtime = Timestamp.Now;

            try
            {

                if (OnGetSessionResponse is not null)
                    await Task.WhenAll(OnGetSessionResponse.GetInvocationList().
                                       Cast<OnGetSessionResponseDelegate>().
                                       Select(e => e(endtime,
                                                     this,
                                                     requestId,
                                                     correlationId,

                                                     SessionId,

                                                     CancellationToken,
                                                     eventTrackingId,
                                                     requestTimeout,

                                                     response,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(SCSPClient) + "." + nameof(OnGetSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        // PUT     ~/sessions/{session_id}/charging_preferences



        // Commands

        //ToDo: Add smart charging commands!


        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        { }

        #endregion

    }

}
