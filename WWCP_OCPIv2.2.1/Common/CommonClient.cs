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
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using System.Net.Security;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The OCPI common client.
    /// </summary>
    public partial class CommonClient : ACommonHTTPClient
    {

        #region (class) CommonAPICounters

        public class CommonAPICounters(APICounterValues? GetVersions         = null,
                                       APICounterValues? GetVersionDetails   = null,

                                       APICounterValues? GetCredentials      = null,
                                       APICounterValues? PostCredentials     = null,
                                       APICounterValues? PutCredentials      = null,
                                       APICounterValues? DeleteCredentials   = null,

                                       APICounterValues? Register            = null)
        {

            #region Properties

            public APICounterValues GetVersions          { get; } = GetVersions       ?? new APICounterValues();
            public APICounterValues GetVersionDetails    { get; } = GetVersionDetails ?? new APICounterValues();

            public APICounterValues GetCredentials       { get; } = GetCredentials    ?? new APICounterValues();
            public APICounterValues PostCredentials      { get; } = PostCredentials   ?? new APICounterValues();
            public APICounterValues PutCredentials       { get; } = PutCredentials    ?? new APICounterValues();
            public APICounterValues DeleteCredentials    { get; } = DeleteCredentials ?? new APICounterValues();

            public APICounterValues Register             { get; } = Register          ?? new APICounterValues();

            #endregion


            public virtual JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("GetVersions",        GetVersions.      ToJSON()),
                       new JProperty("GetVersionDetails",  GetVersionDetails.ToJSON()),

                       new JProperty("GetCredentials",     GetCredentials.   ToJSON()),
                       new JProperty("PostCredentials",    PostCredentials.  ToJSON()),
                       new JProperty("PutCredentials",     PutCredentials.   ToJSON()),
                       new JProperty("DeleteCredentials",  DeleteCredentials.ToJSON()),

                       new JProperty("Register",           Register.         ToJSON())
                   );

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP user agent.
        /// </summary>
        public new const    String                                           DefaultHTTPUserAgent    = $"GraphDefined OCPI {Version.String} Common Client";

        /// <summary>
        /// The default logging context.
        /// </summary>
        public const        String                                           DefaultLoggingContext   = nameof(CommonClient);

        protected readonly  ConcurrentDictionary<Version_Id, VersionDetail>  versionDetails          = [];

        #endregion

        #region Properties

        /// <summary>
        /// The CommonAPI.
        /// </summary>
        public CommonAPI?                CommonAPI                { get; }

        /// <summary>
        /// The remote party.
        /// </summary>
        public RemoteParty               RemoteParty              { get; }

        /// <summary>
        /// CPO client event counters.
        /// </summary>
        public CommonAPICounters         Counters                 { get; }

        /// <summary>
        /// The attached HTTP client logger.
        /// </summary>
        public new Logger?               HTTPLogger
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

        #region OnGetVersionsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all versions will be send.
        /// </summary>
        public event OnGetVersionsRequestDelegate?     OnGetVersionsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all versions will be send.
        /// </summary>
        public event ClientRequestLogHandler?          OnGetVersionsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all versions HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?         OnGetVersionsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all versions request had been received.
        /// </summary>
        public event OnGetVersionsResponseDelegate?    OnGetVersionsResponse;

        #endregion

        #region OnGetVersionDetailsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting version details will be send.
        /// </summary>
        public event OnGetVersionDetailsRequestDelegate?     OnGetVersionDetailsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting version details will be send.
        /// </summary>
        public event ClientRequestLogHandler?                OnGetVersionDetailsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting version details HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?               OnGetVersionDetailsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting version details request had been received.
        /// </summary>
        public event OnGetVersionDetailsResponseDelegate?    OnGetVersionDetailsResponse;

        #endregion


        #region OnGetCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting credentials will be send.
        /// </summary>
        public event OnGetCredentialsRequestDelegate?   OnGetCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting credentials will be send.
        /// </summary>
        public event ClientRequestLogHandler?           OnGetCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting credentials HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?          OnGetCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting credentials request had been received.
        /// </summary>
        public event OnGetCredentialsResponseDelegate?  OnGetCredentialsResponse;

        #endregion

        #region OnPostCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request posting credentials will be send.
        /// </summary>
        public event OnPostCredentialsRequestDelegate?   OnPostCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request posting credentials will be send.
        /// </summary>
        public event ClientRequestLogHandler?            OnPostCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a posting credentials HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?           OnPostCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a posting credentials request had been received.
        /// </summary>
        public event OnPostCredentialsResponseDelegate?  OnPostCredentialsResponse;

        #endregion

        #region OnPutCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting credentials will be send.
        /// </summary>
        public event OnPutCredentialsRequestDelegate?   OnPutCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting credentials will be send.
        /// </summary>
        public event ClientRequestLogHandler?           OnPutCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting credentials HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?          OnPutCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting credentials request had been received.
        /// </summary>
        public event OnPutCredentialsResponseDelegate?  OnPutCredentialsResponse;

        #endregion

        #region OnDeleteCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request deleting credentials will be send.
        /// </summary>
        public event OnDeleteCredentialsRequestDelegate?   OnDeleteCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request deleting credentials will be send.
        /// </summary>
        public event ClientRequestLogHandler?              OnDeleteCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a deleting credentials HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler?             OnDeleteCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a deleting credentials request had been received.
        /// </summary>
        public event OnDeleteCredentialsResponseDelegate?  OnDeleteCredentialsResponse;

        #endregion


        #region OnRegisterRequest/-Response

        /// <summary>
        /// An event fired whenever a registration request will be send.
        /// </summary>
        public event OnRegisterRequestDelegate?   OnRegisterRequest;

        /// <summary>
        /// An event fired whenever a HTTP registration request will be send.
        /// </summary>
        public event ClientRequestLogHandler?     OnRegisterHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a HTTP registration request had been received.
        /// </summary>
        public event ClientResponseLogHandler?    OnRegisterHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a registration request had been received.
        /// </summary>
        public event OnRegisterResponseDelegate?  OnRegisterResponse;

        #endregion

        #endregion

        #region Custom JSON parsers

        public CustomJObjectParserDelegate<VersionInformation>?  CustomVersionInformationParser    { get; set; }
        public CustomJObjectParserDelegate<VersionDetail>?       CustomVersionDetailParser         { get; set; }
        public CustomJObjectParserDelegate<Credentials>?         CustomCredentialsParser           { get; set; }

        #endregion

        #region Constructor(s)

        #region CommonClient(CommonAPI, RemoteParty, ...)

        /// <summary>
        /// Create a new OCPI Common client.
        /// </summary>
        /// <param name="CommonAPI">The CommonAPI.</param>
        /// <param name="RemoteParty">The remote party.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this CPO client.</param>
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public CommonClient(CommonAPI                    CommonAPI,
                            RemoteParty                  RemoteParty,
                            HTTPHostname?                VirtualHostname   = null,
                            I18NString?                  Description       = null,
                            HTTPClientLogger?            HTTPLogger        = null,

                            Boolean?                     DisableLogging    = false,
                            String?                      LoggingPath       = null,
                            String?                      LoggingContext    = null,
                            OCPILogfileCreatorDelegate?  LogfileCreator    = null,
                            IDNSClient?                  DNSClient         = null)

            : base(RemoteParty.RemoteAccessInfos.First().VersionsURL,
                   RemoteParty.RemoteAccessInfos.First().AccessToken,
                   RemoteParty.RemoteAccessInfos.First().AccessTokenIsBase64Encoded,
                   RemoteParty.RemoteAccessInfos.First().TOTPConfig,

                   VirtualHostname,
                   Description,
                   RemoteParty.MaxNumberOfPooledClients,
                   RemoteParty.PreferIPv4,
                   RemoteParty.RemoteCertificateValidator,
                   RemoteParty.LocalCertificateSelector,
                   RemoteParty.ClientCertificates,
                   RemoteParty.ClientCertificateContext,
                   RemoteParty.ClientCertificateChain,
                   RemoteParty.TLSProtocols,

                   RemoteParty.HTTPUserAgent ?? DefaultHTTPUserAgent,
                   RemoteParty.Accept,
                   RemoteParty.ContentType,
                   RemoteParty.ConnectionType,

                   RemoteParty.RequestTimeout,
                   RemoteParty.TransmissionRetryDelay,
                   RemoteParty.MaxNumberOfRetries,
                   RemoteParty.InternalBufferSize,
                   RemoteParty.UseHTTPPipelining,
                   DisableLogging,
                   HTTPLogger,
                   DNSClient)

        {

            this.CommonAPI    = CommonAPI;
            this.RemoteParty  = RemoteParty;

            if (!RemoteParty.RemoteAccessInfos.Any())
                throw new ArgumentException(
                          "The given remote party must have at least one remote access info defined!",
                          nameof(RemoteParty)
                      );

            this.Counters     = new CommonAPICounters();

            base.HTTPLogger   = this.DisableLogging == false
                                    ? new Logger(
                                          this,
                                          LoggingPath,
                                          LoggingContext,
                                          LogfileCreator
                                      )
                                    : null;

        }

        #endregion

        #region CommonClient(CommonAPI, RemotePartyId, RemoteVersionsURL, RemoteAccessToken, ...)

        /// <summary>
        /// Create a new OCPI Common client.
        /// We will start the OCPI registration process afterwards.
        /// </summary>
        /// <param name="CommonAPI">The CommonAPI.</param>
        /// <param name="RemotePartyId">The identification of the remote party.</param>
        /// 
        /// <param name="RemoteVersionsURL">The remote URL of the "OCPI Versions" endpoint to connect to.</param>
        /// <param name="RemoteAccessToken">The remote access token to use.</param>
        /// <param name="RemoteAccessTokenBase64Encoding">Whether the remote access token is Base64 encoded.</param>
        /// <param name="RemoteTOTPConfig">The optional Time-Based One-Time Password (TOTP) configuration as 2nd factor authentication.</param>
        /// 
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this OCPI Common client.</param>
        /// <param name="PreferIPv4"></param>
        /// <param name="RemoteCertificateValidator">The remote TLS certificate validator.</param>
        /// <param name="LocalCertificateSelector"></param>
        /// <param name="ClientCertificates">The TLS client certificates to use for HTTP authentication.</param>
        /// <param name="TLSProtocols"></param>
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
        public CommonClient(CommonAPI                                                  CommonAPI,
                            RemoteParty_Id                                             RemotePartyId,

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

            : this(CommonAPI,
                   new RemoteParty(

                       RemotePartyId,
                       [],

                       RemoteVersionsURL,
                       RemoteAccessToken,

                       PartyStatus.PreRegistration,

                       RemoteAccessTokenBase64Encoding,
                       RemoteTOTPConfig,
                       null,  // RemoteAccessNotBefore
                       null,  // RemoteAccessNotAfter
                       null,  // RemoteStatus
                       null,  // RemoteVersionIds
                       null,  // SelectedVersionId
                       null,  // RemoteAllowDowngrades

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

                       null,  // Created
                       null   // LastUpdated

                   ),
                   VirtualHostname,
                   Description,
                   HTTPLogger,

                   DisableLogging,
                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   DNSClient)

        { }

        #endregion

        #endregion


        #region ToJSON()

        public virtual JObject ToJSON()
            => ToJSON(nameof(CommonClient));

        protected JObject ToJSON(String ClientType)
        {

            return JSONObject.Create(

                       //new JProperty("countryCode",              CountryCode.ToString()),
                       //new JProperty("partyId",                  PartyId.    ToString()),
                       //new JProperty("role",                     Role.       ToString()),

                       new JProperty("type",                     ClientType),

                       Description.IsNotNullOrEmpty()
                           ? new JProperty("description",        Description)
                           : null,

                       new JProperty("remoteVersionsURL",        RemoteVersionsURL.    ToString()),
                       new JProperty("accessToken",              RemoteAccessToken.          ToString()),

                       VirtualHostname.HasValue
                           ? new JProperty("virtualHostname",    VirtualHostname.Value.ToString())
                           : null,

                       new JProperty("requestTimeout",           RequestTimeout.TotalSeconds),

                       new JProperty("maxNumberOfRetries",       MaxNumberOfRetries),

                       versions.SafeAny()
                           ? new JProperty("versions",           new JObject(versions.Select(version => new JProperty(version.Key.  ToString(),
                                                                                                                      version.Value.ToString()))))
                           : null,

                       SelectedOCPIVersionId.HasValue
                           ? new JProperty("selectedVersionId",  SelectedOCPIVersionId.ToString())
                           : null,

                       versionDetails.SafeAny()
                           ? new JProperty("versionDetails",     new JArray(versionDetails.Values.Select(versionDetail => versionDetail.ToJSON())))
                           : null

                   );

        }

        #endregion


        #region GetVersions       (...)

        /// <summary>
        /// Get versions.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<IEnumerable<VersionInformation>>>

            GetVersions(Request_Id?        RequestId           = null,
                        Correlation_Id?    CorrelationId       = null,

                        DateTime?          Timestamp           = null,
                        EventTracking_Id?  EventTrackingId     = null,
                        TimeSpan?          RequestTimeout      = null,
                        CancellationToken  CancellationToken   = default)

        {

            #region Init

            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();

            var timestamp        = Timestamp       ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? base.RequestTimeout;

            var startTime        = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            Counters.GetVersions.IncRequests_OK();

            #endregion

            #region Send OnGetVersionsRequest event

            await LogEvent(
                      OnGetVersionsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          requestTimeout
                      )
                  );

            #endregion


            Byte                                           transmissionRetry = 0;
            OCPIResponse<IEnumerable<VersionInformation>>  response;

            do
            {

                try
                {

                    #region Upstream HTTP request...

                    var httpResponse = await newHTTPClient.GET(
                                                 Path:                  RemoteURL.Path,
                                                 Accept:                ocpiAcceptTypes,
                                                 Authentication:        TokenAuth,
                                                 RequestBuilder:        requestBuilder => {
                                                                            requestBuilder.Set("X-Request-ID",     requestId);
                                                                            requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                        },
                                                 RequestLogDelegate:    OnGetVersionsHTTPRequest,
                                                 ResponseLogDelegate:   OnGetVersionsHTTPResponse,
                                                 EventTrackingId:       eventTrackingId,
                                                 NumberOfRetry:         transmissionRetry,
                                                 RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                 CancellationToken:     CancellationToken).

                                             ConfigureAwait(false);

                    #endregion

                    #region Documentation

                    // {
                    //   "data": [
                    //     {
                    //       "version": "2.2",
                    //       "url":     "https://example.com/ocpi/versions/2.2/"
                    //     }
                    //   ],
                    //   "status_code": 1000,
                    //   "timestamp":  "2020-10-05T21:15:30.134Z"
                    // }

                    #endregion

                    response = OCPIResponse<VersionInformation>.ParseJArray(
                                   httpResponse,
                                   requestId,
                                   correlationId,
                                   json => VersionInformation.Parse(
                                               json,
                                               CustomVersionInformationParser
                                           )
                               );

                    switch (response.StatusCode)
                    {

                        case 1000:

                            if (response.Data is not null)
                            {

                                versions.Clear();

                                foreach (var versionInformation in response.Data)
                                {
                                    versions.TryAdd(
                                        versionInformation.Id,
                                        versionInformation.URL
                                    );
                                }

                            }

                            break;

                    }

                }
                catch (Exception e)
                {
                    response = OCPIResponse<IEnumerable<VersionInformation>>.Exception(e);
                }

            }
            while (transmissionRetry++ < MaxNumberOfRetries &&
                   response.HTTPResponse?.HTTPStatusCode.IsReasonForRetransmission == true);


            #region Update counters

            if (response.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.OK)
                Counters.GetVersions.IncResponses_OK();
            else
                Counters.GetVersions.IncResponses_Error();

            #endregion

            #region Send OnGetVersionsResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            await LogEvent(
                      OnGetVersionsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          requestTimeout,
                          response.Data ?? [],
                          endtime - startTime
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region GetVersionDetails (VersionId, ...)

        /// <summary>
        /// Get versions.
        /// </summary>
        /// <param name="VersionId">The requested version.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<Version_Id, VersionDetail>>

            GetVersionDetails(Version_Id?        VersionId             = null,
                              Boolean            SetAsDefaultVersion   = true,
                              Request_Id?        RequestId             = null,
                              Correlation_Id?    CorrelationId         = null,

                              DateTime?          Timestamp             = null,
                              EventTracking_Id?  EventTrackingId       = null,
                              TimeSpan?          RequestTimeout        = null,
                              CancellationToken  CancellationToken     = default)

        {

            #region Init

            var versionId        = VersionId       ?? SelectedOCPIVersionId;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();

            var timestamp        = Timestamp       ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? base.RequestTimeout;

            var startTime        = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            Counters.GetVersionDetails.IncRequests_OK();

            #endregion

            #region Send OnGetVersionDetailsRequest event

            await LogEvent(
                      OnGetVersionDetailsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          VersionId,
                          SetAsDefaultVersion,
                          RequestId,
                          CorrelationId,

                          requestTimeout
                      )
                  );

            #endregion


            OCPIResponse<Version_Id, VersionDetail> response;

            if (!versionId.HasValue)
                response = OCPIResponse<Version_Id, VersionDetail>.Error("No version identification available!");

            else
            {

                if (!versions.ContainsKey(versionId.Value))
                    await GetVersions(EventTrackingId:    eventTrackingId,
                                      CancellationToken:  CancellationToken);

                if (!versions.TryGetValue(versionId.Value, out var versionURL))
                    response = OCPIResponse<Version_Id, VersionDetail>.Error("Unknown version identification!");

                else
                {

                    Byte transmissionRetry = 0;

                    do
                    {

                        try
                        {

                            #region Upstream HTTP request...

                            var httpResponse = await newHTTPClient.GET(
                                                         Path:                  versionURL.Path,
                                                         Accept:                ocpiAcceptTypes,
                                                         Authentication:        TokenAuth,
                                                         Connection:            ConnectionType.KeepAlive,
                                                         RequestBuilder:        requestBuilder => {
                                                                                    requestBuilder.Set("X-Request-ID",     requestId);
                                                                                    requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                                },
                                                         RequestLogDelegate:    OnGetVersionDetailsHTTPRequest,
                                                         ResponseLogDelegate:   OnGetVersionDetailsHTTPResponse,
                                                         EventTrackingId:       eventTrackingId,
                                                         NumberOfRetry:         transmissionRetry,
                                                         RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                         CancellationToken:     CancellationToken).

                                                     ConfigureAwait(false);

                            #endregion

                            #region Documentation

                            // {
                            //   "data": {
                            //     "version": "2.2",
                            //     "endpoints": [
                            //       {
                            //         "identifier": "sessions",
                            //         "role":       "SENDER",
                            //         "url":        "https://example.com/ocpi/cpo/2.2/sessions/"
                            //       },
                            //       {
                            //         "identifier": "tariffs",
                            //         "role":       "SENDER",
                            //         "url":        "https://example.com/ocpi/cpo/2.2/tariffs/"
                            //       },
                            //       {
                            //         "identifier": "hubclientinfo",
                            //         "role":       "RECEIVER",
                            //         "url":        "https://example.com/ocpi/cpo/2.2/hubclientinfo/"
                            //       },
                            //       {
                            //         "identifier": "locations",
                            //         "role":       "SENDER",
                            //         "url":        "https://example.com/ocpi/cpo/2.2/locations/"
                            //       },
                            //       {
                            //         "identifier": "tokens",
                            //         "role":       "RECEIVER",
                            //         "url":        "https://example.com/ocpi/cpo/2.2/tokens/"
                            //       },
                            //       {
                            //         "identifier": "commands",
                            //         "role":       "RECEIVER",
                            //         "url":        "https://example.com/ocpi/cpo/2.2/commands/"
                            //       },
                            //       {
                            //         "identifier": "credentials",
                            //         "role":       "RECEIVER",
                            //         "url":        "https://example.com/ocpi/2.2/credentials/"
                            //       },
                            //       {
                            //         "identifier": "credentials",
                            //         "role":       "SENDER",
                            //         "url":        "https://example.com/ocpi/2.2/credentials/"
                            //       }
                            //     ]
                            //   },
                            //   "status_code": 1000
                            // }

                            #endregion

                            response = OCPIResponse<Version_Id, VersionDetail>.ParseJObject(
                                           versionId.Value,
                                           httpResponse,
                                           requestId,
                                           correlationId,
                                           json => VersionDetail.Parse(
                                                       json,
                                                       CustomVersionDetailParser
                                                   )
                                       );

                            switch (response.StatusCode)
                            {

                                case 1000:

                                    if (response.Data is not null)
                                    {

                                        versionDetails.TryRemove(versionId.Value, out _);

                                        versionDetails.TryAdd(
                                            versionId.Value,
                                            response.Data
                                        );

                                        if (SetAsDefaultVersion)
                                            SelectedOCPIVersionId = VersionId;

                                    }

                                    break;

                            }

                        }

                        catch (Exception e)
                        {
                            response = OCPIResponse<Version_Id, VersionDetail>.Exception(e);
                        }

                    }
                    while (transmissionRetry++ < MaxNumberOfRetries &&
                           response.HTTPResponse?.HTTPStatusCode.IsReasonForRetransmission == true);

                }

            }


            #region Send OnGetVersionDetailsResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                if (response.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.OK)
                    Counters.GetVersionDetails.IncResponses_OK();
                else
                    Counters.GetVersionDetails.IncResponses_Error();


                if (OnGetVersionDetailsResponse is not null)
                    await Task.WhenAll(OnGetVersionDetailsResponse.GetInvocationList().
                                       Cast<OnGetVersionDetailsResponseDelegate>().
                                       Select(e => e(endtime,
                                                     timestamp,
                                                     this,
                                                     $"{nameof(CommonClient)} {RemoteParty?.Id}",
                                                     eventTrackingId,

                                                     VersionId,
                                                     SetAsDefaultVersion,
                                                     RequestId,
                                                     CorrelationId,

                                                     requestTimeout,
                                                     response.Data,
                                                     endtime - startTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(CommonClient) + "." + nameof(OnGetVersionDetailsResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetModuleRemoteURL  (ModuleId, InterfaceRole, VersionId = null, ...)

        public async Task<URL?> GetModuleRemoteURL(Module_Id          ModuleId,
                                                   InterfaceRoles     InterfaceRole,
                                                   Version_Id?        VersionId           = null,

                                                   EventTracking_Id?  EventTrackingId     = null,
                                                   CancellationToken  CancellationToken   = default)
        {

            var versionId        = VersionId       ?? SelectedOCPIVersionId;
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;

            if (!versionId.HasValue)
            {

                if (!versionDetails.IsEmpty)
                    SelectedOCPIVersionId = versionId = versionDetails.Keys.OrderByDescending(id => id).First();
                else
                {

                    await GetVersions(
                              EventTrackingId:    eventTrackingId,
                              CancellationToken:  CancellationToken
                          );

                    if (!versions.IsEmpty)
                    {

                        SelectedOCPIVersionId = versionId = versions.Keys.OrderByDescending(id => id).First();

                        await GetVersionDetails(
                                  versionId.Value,
                                  EventTrackingId:    eventTrackingId,
                                  CancellationToken:  CancellationToken
                              );

                    }

                }

            }

            if (versionId.     HasValue)
            {

                if (!versionDetails.ContainsKey(versionId.Value))
                    await GetVersionDetails(
                              versionId.Value,
                              EventTrackingId:    eventTrackingId,
                              CancellationToken:  CancellationToken
                          );

                if (versionDetails.TryGetValue(versionId.Value, out var currentVersionDetails))
                {
                    foreach (var endpoint in currentVersionDetails.Endpoints)
                    {
                        if (endpoint.Identifier == ModuleId &&
                            endpoint.Role       == InterfaceRole)
                        {
                            return endpoint.URL;
                        }
                    }
                }

            }

            return new URL?();

        }

        #endregion

        #region GetModuleHTTPClient (ModuleId, InterfaceRole, VersionId = null, ...)

        /// <summary>
        /// OCPI allows every OCPI module to be hosted on a different server.
        /// </summary>
        /// <param name="ModuleId">The OCPI module identification.</param>
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<HTTPTestClient?>

            GetModuleHTTPClient(Module_Id          ModuleId,
                                InterfaceRoles     InterfaceRole,
                                Version_Id?        VersionId           = null,

                                EventTracking_Id?  EventTrackingId     = null,
                                CancellationToken  CancellationToken   = default)

        {

            var remoteURL = await GetModuleRemoteURL(
                                      ModuleId,
                                      InterfaceRole,
                                      VersionId,
                                      EventTrackingId,
                                      CancellationToken
                                  );

            if (remoteURL.HasValue)
            {

                HTTPTestClient? httpClient = null;

                if (remoteURL.Value.Hostname.ToString() == newHTTPClient.DomainName?.ToString())
                {
                    httpClient = newHTTPClient;
                }

                else
                {

                    httpClient = new HTTPTestClient(

                                     remoteURL.Value,
                                     Description:                           newHTTPClient.Description,
                                     HTTPUserAgent:                         newHTTPClient.HTTPUserAgent,
                                     Accept:                                newHTTPClient.Accept,
                                     ContentType:                           newHTTPClient.ContentType,
                                     Connection:                            newHTTPClient.Connection,
                                     DefaultRequestBuilder:                 newHTTPClient.DefaultRequestBuilder,

                                     RemoteCertificateValidator:            (sender, certificate, chain, httpTestClient, policyErrors) => {
                                                                                var x = newHTTPClient.RemoteCertificateValidator?.Invoke(sender, certificate, chain, httpTestClient, policyErrors);
                                                                                return x ?? (false,[]);
                                                                            },
                                     LocalCertificateSelector:              newHTTPClient.LocalCertificateSelector,
                                     ClientCertificates:                    newHTTPClient.ClientCertificates,
                                     ClientCertificateContext:              newHTTPClient.ClientCertificateContext,
                                     ClientCertificateChain:                newHTTPClient.ClientCertificateChain,
                                     TLSProtocols:                          newHTTPClient.TLSProtocols,
                                     CipherSuitesPolicy:                    newHTTPClient.CipherSuitesPolicy,
                                     CertificateChainPolicy:                newHTTPClient.CertificateChainPolicy,
                                     CertificateRevocationCheckMode:        newHTTPClient.CertificateRevocationCheckMode,
                                     ApplicationProtocols:                  newHTTPClient.ApplicationProtocols,
                                     AllowRenegotiation:                    newHTTPClient.AllowRenegotiation,
                                     AllowTLSResume:                        newHTTPClient.AllowTLSResume,

                                     PreferIPv4:                            newHTTPClient.PreferIPv4,
                                     ConnectTimeout:                        newHTTPClient.ConnectTimeout,
                                     ReceiveTimeout:                        newHTTPClient.ReceiveTimeout,
                                     SendTimeout:                           newHTTPClient.SendTimeout,
                                     TransmissionRetryDelay:                newHTTPClient.TransmissionRetryDelay,
                                     MaxNumberOfRetries:                    newHTTPClient.MaxNumberOfRetries,
                                     BufferSize:                            newHTTPClient.BufferSize,

                                     ConsumeRequestChunkedTEImmediately:    newHTTPClient.ConsumeRequestChunkedTEImmediately,
                                     ConsumeResponseChunkedTEImmediately:   newHTTPClient.ConsumeResponseChunkedTEImmediately,

                                     DNSClient:                             newHTTPClient.DNSClient

                                 );

                }

                return httpClient;

            }

            return null;

        }

        #endregion


        #region GetCredentials    (...)

        /// <summary>
        /// Get our credentials from the remote API.
        /// </summary>
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<Credentials>>

            GetCredentials(Version_Id?        VersionId           = null,
                           Request_Id?        RequestId           = null,
                           Correlation_Id?    CorrelationId       = null,

                           DateTime?          Timestamp           = null,
                           EventTracking_Id?  EventTrackingId     = null,
                           TimeSpan?          RequestTimeout      = null,
                           CancellationToken  CancellationToken   = default)

        {

            #region Init

            var versionId        = VersionId       ?? SelectedOCPIVersionId;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();

            var timestamp        = Timestamp       ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? base.RequestTimeout;

            var startTime        = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            Counters.GetCredentials.IncRequests_OK();

            #endregion

            #region Send OnGetCredentialsHTTPRequest event

            await LogEvent(
                      OnGetCredentialsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,

                          requestTimeout
                      )
                  );

            #endregion


            Byte                       transmissionRetry = 0;
            OCPIResponse<Credentials>  response;

            do
            {

                try
                {

                    var remoteURL = await GetModuleRemoteURL(
                                              Module_Id.Credentials,
                                              InterfaceRoles.RECEIVER,
                                              versionId,
                                              eventTrackingId,
                                              CancellationToken
                                          );

                    // Might be updated!
                    versionId ??= SelectedOCPIVersionId;

                    if      (!versionId.HasValue)
                        response = OCPIResponse<Credentials>.Error("No versionId available!");

                    else if (!remoteURL.HasValue)
                        response = OCPIResponse<Credentials>.Error("No remote URL available!");

                    else
                    {

                        #region Upstream HTTP request...

                        var httpResponse = await newHTTPClient.GET(
                                                     Path:                  remoteURL.Value.Path,
                                                     Authentication:        TokenAuth,
                                                     RequestBuilder:        requestBuilder => {
                                                                                requestBuilder.Set("X-Request-ID",     requestId);
                                                                                requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                            },
                                                     RequestLogDelegate:    OnGetCredentialsHTTPRequest,
                                                     ResponseLogDelegate:   OnGetCredentialsHTTPResponse,
                                                     EventTrackingId:       eventTrackingId,
                                                     NumberOfRetry:         transmissionRetry,
                                                     RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                     CancellationToken:     CancellationToken).

                                                 ConfigureAwait(false);

                        #endregion

                        response = OCPIResponse<Credentials>.ParseJObject(
                                       httpResponse,
                                       requestId,
                                       correlationId,
                                       json => Credentials.Parse(
                                                   json,
                                                   CustomCredentialsParser
                                               )
                                   );

                    }

                }
                catch (Exception e)
                {
                    response = OCPIResponse<String, Credentials>.Exception(e);
                }

            }
            while (transmissionRetry++ < MaxNumberOfRetries &&
                   response.HTTPResponse?.HTTPStatusCode.IsReasonForRetransmission == true);


            #region Update counters

            if (response.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.OK)
                Counters.GetCredentials.IncResponses_OK();
            else
                Counters.GetCredentials.IncResponses_Error();

            #endregion

            #region Send OnGetCredentialsHTTPResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            await LogEvent(
                      OnGetCredentialsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,

                          requestTimeout,
                          response.Data,
                          endtime - startTime
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PostCredentials   (Credentials, ...)

        /// <summary>
        /// Post our credentials onto the remote API.
        /// </summary>
        /// <param name="Credentials">The credentials to store/post at/onto the remote API.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<Credentials>>

            PostCredentials(Credentials        Credentials,

                            Version_Id?        VersionId           = null,
                            Request_Id?        RequestId           = null,
                            Correlation_Id?    CorrelationId       = null,

                            DateTime?          Timestamp           = null,
                            EventTracking_Id?  EventTrackingId     = null,
                            TimeSpan?          RequestTimeout      = null,
                            CancellationToken  CancellationToken   = default)

        {

            #region Init

            var versionId        = VersionId       ?? SelectedOCPIVersionId;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();

            var timestamp        = Timestamp       ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? base.RequestTimeout;

            var startTime        = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            Counters.PostCredentials.IncRequests_OK();

            #endregion

            #region Send OnPostCredentialsHTTPRequest event

            await LogEvent(
                      OnPostCredentialsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,
                          Credentials,

                          requestTimeout
                      )
                  );

            #endregion


            Byte                       transmissionRetry = 0;
            OCPIResponse<Credentials>  response;

            do
            {

                try
                {

                    var remoteURL = await GetModuleRemoteURL(
                                              Module_Id.Credentials,
                                              InterfaceRoles.RECEIVER,
                                              VersionId,
                                              eventTrackingId,
                                              CancellationToken
                                          );

                    // Might have been updated!
                    versionId ??= SelectedOCPIVersionId;

                    if (!versionId.HasValue)
                        response = OCPIResponse<Credentials>.Error("No versionId available!");

                    else if (!remoteURL.HasValue)
                        response = OCPIResponse<Credentials>.Error("No remote URL available!");

                    else
                    {

                        #region Upstream HTTP request...

                        var httpResponse = await newHTTPClient.POST(
                                                     Path:                  remoteURL.Value.Path,
                                                     Content:               Credentials.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                     Authentication:        TokenAuth,
                                                     Connection:            ConnectionType.Close,
                                                     RequestBuilder:        requestBuilder => {
                                                                                requestBuilder.Set("X-Request-ID",     requestId);
                                                                                requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                            },
                                                     RequestLogDelegate:    OnPostCredentialsHTTPRequest,
                                                     ResponseLogDelegate:   OnPostCredentialsHTTPResponse,
                                                     EventTrackingId:       eventTrackingId,
                                                     NumberOfRetry:         transmissionRetry,
                                                     RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                     CancellationToken:     CancellationToken).

                                                 ConfigureAwait(false);

                        #endregion

                        response = OCPIResponse<Credentials>.ParseJObject(
                                       httpResponse,
                                       requestId,
                                       correlationId,
                                       json => Credentials.Parse(
                                                   json,
                                                   CustomCredentialsParser
                                               )
                                   );

                    }

                }
                catch (Exception e)
                {
                    response = OCPIResponse<String, Credentials>.Exception(e);
                }

            }
            while (transmissionRetry++ < MaxNumberOfRetries &&
                   response.HTTPResponse?.HTTPStatusCode.IsReasonForRetransmission == true);


            #region Update counters

            if (response.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.OK)
                Counters.PostCredentials.IncResponses_OK();
            else
                Counters.PostCredentials.IncResponses_Error();

            #endregion

            #region Send OnPostCredentialsHTTPResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            await LogEvent(
                      OnPostCredentialsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,
                          Credentials,

                          requestTimeout,
                          response.Data,
                          endtime - startTime
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region PutCredentials    (Credentials, ...)

        /// <summary>
        /// Put our credentials onto the remote API.
        /// </summary>
        /// <param name="Credentials">The credentials to store/put at/onto the remote API.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<Credentials>>

            PutCredentials(Credentials        Credentials,

                           Version_Id?        VersionId           = null,
                           Request_Id?        RequestId           = null,
                           Correlation_Id?    CorrelationId       = null,

                           DateTime?          Timestamp           = null,
                           EventTracking_Id?  EventTrackingId     = null,
                           TimeSpan?          RequestTimeout      = null,
                           CancellationToken  CancellationToken   = default)

        {

            #region Init

            var versionId        = VersionId       ?? SelectedOCPIVersionId;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();

            var timestamp        = Timestamp       ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? base.RequestTimeout;

            var startTime        = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            Counters.PutCredentials.IncRequests_OK();

            #endregion

            #region Send OnPutCredentialsHTTPRequest event

            await LogEvent(
                      OnPutCredentialsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,
                          Credentials,

                          requestTimeout
                      )
                  );

            #endregion


            Byte                       transmissionRetry = 0;
            OCPIResponse<Credentials>  response;

            do
            {

                try
                {

                    var remoteURL = await GetModuleRemoteURL(
                                              Module_Id.Credentials,
                                              InterfaceRoles.RECEIVER,
                                              VersionId,
                                              eventTrackingId,
                                              CancellationToken
                                          );

                    // Might have been updated!
                    versionId ??= SelectedOCPIVersionId;

                    if (!versionId.HasValue)
                        response = OCPIResponse<Credentials>.Error("No versionId available!");

                    else if (!remoteURL.HasValue)
                        response = OCPIResponse<Credentials>.Error("No remote URL available!");

                    else
                    {

                        #region Upstream HTTP request...

                        var httpResponse = await newHTTPClient.PUT(
                                                     Path:                  remoteURL.Value.Path,
                                                     Content:               Credentials.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                     Authentication:        TokenAuth,
                                                     RequestBuilder:        requestBuilder => {
                                                                                requestBuilder.Set("X-Request-ID",     requestId);
                                                                                requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                            },
                                                     RequestLogDelegate:    OnPutCredentialsHTTPRequest,
                                                     ResponseLogDelegate:   OnPutCredentialsHTTPResponse,
                                                     EventTrackingId:       eventTrackingId,
                                                     NumberOfRetry:         transmissionRetry,
                                                     RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                     CancellationToken:     CancellationToken).

                                                 ConfigureAwait(false);

                        #endregion

                        response = OCPIResponse<Credentials>.ParseJObject(
                                       httpResponse,
                                       requestId,
                                       correlationId,
                                       json => Credentials.Parse(
                                                   json,
                                                   CustomCredentialsParser
                                               )
                                   );

                        if (response.Data is not null)
                        {

                            TokenAuth = HTTPTokenAuthentication.Parse(response.Data.Token.ToString().ToBase64());

                            var oldRemoteParty = this.RemoteParty;

                            // Validate, that neither the credentials roles had not been changed!
                            if (oldRemoteParty is not null &&
                                oldRemoteParty.Roles.Count() == response.Data.Roles.Count())
                            {

                                var failed = false;

                                foreach (var receivedCredentialsRole in response.Data.Roles)
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
                                        failed = true;

                                }

                                if (!failed)
                                {

                                    // Only the access token and the business details are allowed to be changed!
                                    var result = await CommonAPI.AddOrUpdateRemoteParty(

                                                           Id:                  oldRemoteParty.Id,
                                                           CredentialsRoles:    oldRemoteParty.Roles,// response.Data.Roles,

                                                           Status:              PartyStatus.ENABLED,

                                                           LocalAccessToken:    Credentials.Token,
                                                           RemoteVersionsURL:   response.Data.URL,
                                                           RemoteAccessToken:   response.Data.Token,
                                                           RemoteStatus:        RemoteAccessStatus.ONLINE,
                                                           RemoteVersionIds:    [ Version.Id ],
                                                           SelectedVersionId:   Version.Id,

                                                           LocalAccessStatus:   AccessStatus.ALLOWED

                                                       );

                                    if (!result.IsSuccess)
                                        DebugX.Log("Illegal AddOrUpdateRemoteParty(...) after PutCredentials(...)!");

                                }

                            }

                        }

                    }

                }
                catch (Exception e)
                {
                    response = OCPIResponse<String, Credentials>.Exception(e);
                }

            }
            while (transmissionRetry++ < MaxNumberOfRetries &&
                   response.HTTPResponse?.HTTPStatusCode.IsReasonForRetransmission == true);


            #region Update counters

            if (response.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.OK)
                Counters.PutCredentials.IncResponses_OK();
            else
                Counters.PutCredentials.IncResponses_Error();

            #endregion

            #region Send OnPutCredentialsHTTPResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            await LogEvent(
                      OnPutCredentialsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,
                          Credentials,

                          requestTimeout,
                          response.Data,
                          endtime - startTime
                      )
                  );

            #endregion

            return response;

        }

        #endregion

        #region DeleteCredentials (Credentials, ...)

        /// <summary>
        /// Remove our credentials from the remote API.
        /// </summary>
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse>

            DeleteCredentials(Version_Id?        VersionId           = null,
                              Request_Id?        RequestId           = null,
                              Correlation_Id?    CorrelationId       = null,

                              DateTime?          Timestamp           = null,
                              EventTracking_Id?  EventTrackingId     = null,
                              TimeSpan?          RequestTimeout      = null,
                              CancellationToken  CancellationToken   = default)

        {

            #region Init

            var versionId        = VersionId       ?? SelectedOCPIVersionId;
            var requestId        = RequestId       ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId   ?? Correlation_Id.NewRandom();

            var timestamp        = Timestamp       ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout  ?? base.RequestTimeout;

            var startTime        = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            Counters.DeleteCredentials.IncRequests_OK();

            #endregion

            #region Send OnDeleteCredentialsHTTPRequest event

            await LogEvent(
                      OnDeleteCredentialsRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,

                          requestTimeout
                      )
                  );

            #endregion


            Byte          transmissionRetry = 0;
            OCPIResponse  response;

            do
            {

                try
                {

                    var remoteURL = await GetModuleRemoteURL(
                                              Module_Id.Credentials,
                                              InterfaceRoles.RECEIVER,
                                              VersionId,
                                              eventTrackingId,
                                              CancellationToken
                                          );

                    // Might have been updated!
                    versionId ??= SelectedOCPIVersionId;

                    if (!versionId.HasValue)
                        response = OCPIResponse<Credentials>.Error("No versionId available!");

                    else if (!remoteURL.HasValue)
                        response = OCPIResponse<Credentials>.Error("No remote URL available!");

                    else
                    {

                        #region Upstream HTTP request...

                        var httpResponse = await newHTTPClient.DELETE(
                                                     Path:                  remoteURL.Value.Path,
                                                     Authentication:        TokenAuth,
                                                     RequestBuilder:        requestBuilder => {
                                                                                requestBuilder.Set("X-Request-ID",     requestId);
                                                                                requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                            },
                                                     RequestLogDelegate:    OnDeleteCredentialsHTTPRequest,
                                                     ResponseLogDelegate:   OnDeleteCredentialsHTTPResponse,
                                                     EventTrackingId:       eventTrackingId,
                                                     NumberOfRetry:         transmissionRetry,
                                                     RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                     CancellationToken:     CancellationToken).

                                                 ConfigureAwait(false);

                        #endregion

                        response = OCPIResponse.Parse(
                                       httpResponse,
                                       requestId,
                                       correlationId
                                   );

                    }

                }
                catch (Exception e)
                {
                    response = OCPIResponse.Exception(e);
                }

            }
            while (transmissionRetry++ < MaxNumberOfRetries &&
                   response.HTTPResponse?.HTTPStatusCode.IsReasonForRetransmission == true);


            #region Update counters

            if (response.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.OK)
                Counters.DeleteCredentials.IncResponses_OK();
            else
                Counters.DeleteCredentials.IncResponses_Error();

            #endregion

            #region Send OnDeleteCredentialsHTTPResponse event

            var endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            await LogEvent(
                      OnDeleteCredentialsResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,

                          requestTimeout,
                          endtime - startTime
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region Register          (VersionId, ...)

        //  1. We create <CREDENTIALS_TOKEN_A> and associate it with <CountryCode> + <PartyId>.
        //  2. We send <CREDENTIALS_TOKEN_A> and <VERSIONS endpoint> to the other party... e.g. via e-mail.
        //
        //    waiting...
        //
        //  3. Other party fetches <VERSIONS endpoint> using <CREDENTIALS_TOKEN_A> and stores all versions endpoint information.
        //  4. Other party chooses a common version, fetches version details from the found <VERSION DETAIL endpoint> using <CREDENTIALS_TOKEN_A> and stores all version endpoint information.
        //  5. Other party generates <CREDENTIALS_TOKEN_B> and POSTs its chosen version (as part of the request URL), its <VERSIONS endpoint> and this <CREDENTIALS_TOKEN_B>.
        //
        // >>> Here we are in a synchronous HTTP POST request! >>>
        //    6. We verify that the version the other side has chosen is at least valid.
        //    7. We fetch <VERSIONS> from the given <VERSIONS endpoint> using <CREDENTIALS_TOKEN_B> and store all versions endpoint information.
        //    8. We verify that the version the other side has chosen can be used.
        //    9. We fetch <VERSION DETAIL> from the given <VERSION DETAIL endpoint> for the common version using <CREDENTIALS_TOKEN_B> and store all versions endpoint information.
        //   10. We generate <CREDENTIALS_TOKEN_C> and return this as response to the POST request in 5.
        //
        //   In case the Receiver platform that cannot find the endpoints it expects, then it is expected to respond to the request with the status code 3003.
        //   On any error: Reply with HTTP Status Code 3001
        //
        // 11. Other party will replace <CREDENTIALS_TOKEN_A> with <CREDENTIALS_TOKEN_C>.


        /// <summary>
        /// Post the given credentials.
        /// </summary>
        /// <param name="RequestTimestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<OCPIResponse<Credentials>>

            Register(Version_Id?        VersionId             = null,
                     Boolean            SetAsDefaultVersion   = true,
                     AccessToken?       CredentialTokenB      = null,

                     Request_Id?        RequestId             = null,
                     Correlation_Id?    CorrelationId         = null,

                     DateTime?          RequestTimestamp      = null,
                     EventTracking_Id?  EventTrackingId       = null,
                     TimeSpan?          RequestTimeout        = null,
                     CancellationToken  CancellationToken     = default)

        {

            #region Init

            var versionId        = VersionId        ?? SelectedOCPIVersionId;
            var requestId        = RequestId        ?? Request_Id.    NewRandom();
            var correlationId    = CorrelationId    ?? Correlation_Id.NewRandom();

            var timestamp        = RequestTimestamp ?? Timestamp.Now;
            var eventTrackingId  = EventTrackingId  ?? EventTracking_Id.New;
            var requestTimeout   = RequestTimeout   ?? base.RequestTimeout;

            var startTime        = Timestamp.Now;

            Counters.Register.IncRequests_OK();

            #endregion

            #region Send OnRegisterRequest event

            await LogEvent(
                      OnRegisterRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,

                          requestTimeout
                      )
                  );

            #endregion


            Byte                       transmissionRetry = 0;
            OCPIResponse<Credentials>  response;

            var credentialTokenB = CredentialTokenB ?? AccessToken.NewRandom();

            do
            {

                try
                {

                    var remoteURL  = await GetModuleRemoteURL(
                                               Module_Id.Credentials,
                                               InterfaceRoles.RECEIVER,
                                               null,//versionId,
                                               eventTrackingId,
                                               CancellationToken
                                           );

                    remoteURL      = await GetModuleRemoteURL(
                                               Module_Id.Credentials,
                                               InterfaceRoles.RECEIVER,
                                               versionId,
                                               eventTrackingId,
                                               CancellationToken
                                           );

                    // Might be updated!
                    versionId ??= SelectedOCPIVersionId;

                    if      (!versionId.HasValue)
                        response = OCPIResponse<String, Credentials>.Error("No version identification available!");

                    else if (!remoteURL.HasValue)
                        response = OCPIResponse<String, Credentials>.Error("No remote URL available!");

                    else
                    {

                        var credentials = new Credentials(
                                              credentialTokenB,
                                              CommonAPI.BaseAPI.OurVersionsURL,
                                              CommonAPI.Parties.Select(partyData => partyData.ToCredentialsRole())
                                          );

                        #region Upstream HTTP request...

                        var httpResponse = await newHTTPClient.POST(
                                                     Path:                  remoteURL.Value.Path,
                                                     Content:               credentials.ToJSON().ToUTF8Bytes(JSONFormatting),
                                                     ContentType:           HTTPContentType.Application.JSON_UTF8,
                                                     Accept:                ocpiAcceptTypes,
                                                     Authentication:        TokenAuth,
                                                     Connection:            ConnectionType.Close,
                                                     RequestBuilder:        requestBuilder => {
                                                                                requestBuilder.Set("X-Request-ID",     requestId);
                                                                                requestBuilder.Set("X-Correlation-ID", correlationId);
                                                                            },
                                                     RequestLogDelegate:    OnRegisterHTTPRequest,
                                                     ResponseLogDelegate:   OnRegisterHTTPResponse,
                                                     EventTrackingId:       eventTrackingId,
                                                     NumberOfRetry:         transmissionRetry,
                                                     RequestTimeout:        RequestTimeout ?? this.RequestTimeout,
                                                     CancellationToken:     CancellationToken).

                                                 ConfigureAwait(false);

                        #endregion


                        #region Documentation

                        // {
                        //     "data": {
                        //         "token":                     "e~!Kgf457pApk5b&vG93K-<MQ#T&Q)io!S)HfQxSGyb#*b6beZP#36kF55~zhuq]",
                        //         "url":                       "https://example.com/ocpi/versions",
                        //         "roles": [{
                        //             "role":                  "CPO",
                        //             "business_details": {
                        //                 "name":              "Example Corp."
                        //             },
                        //             "party_id":              "EXP",
                        //             "country_code":          "DE"
                        //         }]
                        //     },
                        //     "status_code": 1000,
                        //     "timestamp": "2020-11-07T15:33:49.481Z"
                        // }

                        #endregion

                        response = OCPIResponse<Credentials>.ParseJObject(
                                       httpResponse,
                                       requestId,
                                       correlationId,
                                       json => Credentials.Parse(json)
                                   );

                        if (response.Data is not null)
                        {

                            SelectedOCPIVersionId  = versionId;
                            RemoteAccessToken      = response.Data.Token;
                            TokenAuth              = HTTPTokenAuthentication.Parse(RemoteAccessToken.Value.ToString().ToBase64());

                            //var oldRemoteParty     = this.RemoteParty;

                            //if (oldRemoteParty is not null)
                            //{

                                // Validate, that the number of credentials roles had not been changed!
                                //if (oldRemoteParty.Id.CountryCode.ToString() == "XX" ||
                                //    oldRemoteParty.Roles.Count() == response.Data.Roles.Count())
                                //{

                                    #region Validate, that neither the credentials roles had not been changed!

                                    //var failed = false;

                                    //if (oldRemoteParty.Id.CountryCode.ToString() != "XX")
                                    //{
                                    //    foreach (var receivedCredentialsRole in response.Data.Roles)
                                    //    {

                                    //        CredentialsRole? existingCredentialsRole = null;

                                    //        foreach (var oldCredentialsRole in oldRemoteParty.Roles)
                                    //        {
                                    //            if (oldCredentialsRole.PartyId == receivedCredentialsRole.PartyId &&
                                    //                oldCredentialsRole.Role    == receivedCredentialsRole.Role)
                                    //            {
                                    //                existingCredentialsRole = receivedCredentialsRole;
                                    //                break;
                                    //            }
                                    //        }

                                    //        if (existingCredentialsRole is null)
                                    //            failed = true;

                                    //    }
                                    //}

                                    #endregion

                                    //if (!failed)
                                    //{

                                        // Only the access token and the business details are allowed to be changed!
                                        var addOrUpdateResult = await CommonAPI.AddOrUpdateRemoteParty(

                                                                          this.RemoteParty.Id,        // Id
                                                                          response.Data.Roles,        // CredentialsRoles

                                                                          credentialTokenB,           // LocalAccessToken
                                                                          response.Data.URL,          // RemoteVersionsURL
                                                                          response.Data.Token,        // RemoteAccessToken

                                                                          PartyStatus.ENABLED,        // PartyStatus

                                                                          null,                       // RemoteAccessTokenBase64Encoding
                                                                          null,                       // RemoteTOTPConfig
                                                                          null,                       // RemoteAccessNotBefore
                                                                          null,                       // RemoteAccessNotAfter
                                                                          RemoteAccessStatus.ONLINE,  // RemoteStatus
                                                                          [ versionId.Value ],        // RemoteVersionIds
                                                                          versionId.Value,            // SelectedVersionId
                                                                          null,                       // RemoteAllowDowngrades

                                                                          null,                       // LocalAccessTokenBase64Encoding
                                                                          null,                       // LocalTOTPConfig
                                                                          null,                       // LocalAccessNotBefore
                                                                          null,                       // LocalAccessNotAfter
                                                                          null,                       // LocalAllowDowngrades
                                                                          AccessStatus.ALLOWED,       // LocalAccessStatus

                                                                          null,                       // PreferIPv4
                                                                          null,                       // RemoteCertificateValidator
                                                                          null,                       // LocalCertificateSelector
                                                                          null,                       // ClientCertificates
                                                                          null,                       // ClientCertificateContext
                                                                          null,                       // ClientCertificateChain
                                                                          null,                       // TLSProtocols
                                                                          null,                       // ContentType
                                                                          null,                       // Accept
                                                                          null,                       // HTTPUserAgent
                                                                          null,                       // RequestTimeout
                                                                          null,                       // TransmissionRetryDelay
                                                                          null,                       // MaxNumberOfRetries
                                                                          null,                       // InternalBufferSize
                                                                          null,                       // UseHTTPPipelining

                                                                          eventTrackingId,            // EventTrackingId
                                                                          null,                       // CurrentUserId
                                                                          null,                       // Created
                                                                          Timestamp.Now               // LastUpdated

                                                                      );

                                        if (!addOrUpdateResult.IsSuccess)
                                            DebugX.Log("Illegal AddOrUpdateRemoteParty(...) after Register(...)!");

                                    //}

                                //}
                                //else
                                //    DebugX.Log("The number of credentials roles has been changed!");

                            //}

                            //else
                            //{



                            //}


                        }

                    }

                }
                catch (Exception e)
                {
                    response = OCPIResponse<String, Credentials>.Exception(e);
                }

            }
            while (transmissionRetry++ < MaxNumberOfRetries &&
                   response.HTTPResponse?.HTTPStatusCode.IsReasonForRetransmission == true);


            #region Update counters

            if (response.HTTPResponse?.HTTPStatusCode == HTTPStatusCode.OK)
                Counters.Register.IncResponses_OK();
            else
                Counters.Register.IncResponses_Error();

            #endregion

            #region Send OnRegisterResponse event

            var endtime = Timestamp.Now;

            await LogEvent(
                      OnRegisterResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endtime,
                          timestamp,
                          this,
                          $"{nameof(CommonClient)} {RemoteParty?.Id}",
                          eventTrackingId,

                          RequestId,
                          CorrelationId,

                          versionId,

                          requestTimeout,
                          response.Data,
                          endtime - startTime
                      )
                  );

            #endregion

            return response;

        }

        #endregion


        #region (private)   LogEvent     (Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OCPICommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(CommonClient),
                   Logger,
                   LogHandler,
                   EventName,
                   OCPICommand
               );

        #endregion

        #region (protected) LogEvent     (OCPIIO, Logger, LogHandler, ...)

        protected async Task LogEvent<TDelegate>(String                                             OCPIIO,
                                                 TDelegate?                                         Logger,
                                                 Func<TDelegate, Task>                              LogHandler,
                                                 [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                                 [CallerMemberName()]                       String  OCPICommand   = "")

            where TDelegate : Delegate

        {
            if (Logger is not null)
            {
                try
                {

                    await Task.WhenAll(
                              Logger.GetInvocationList().
                                     OfType<TDelegate>().
                                     Select(LogHandler)
                          );

                }
                catch (Exception e)
                {
                    await HandleErrors(OCPIIO, $"{OCPICommand}.{EventName}", e);
                }
            }
        }

        #endregion

        #region (virtual)   HandleErrors (Module, Caller, ErrorResponse)

        public virtual Task HandleErrors(String  Module,
                                         String  Caller,
                                         String  ErrorResponse)
        {

            DebugX.Log($"{Module}.{Caller}: {ErrorResponse}");

            return Task.CompletedTask;

        }

        #endregion

        #region (virtual)   HandleErrors (Module, Caller, ExceptionOccurred)

        public virtual Task HandleErrors(String     Module,
                                         String     Caller,
                                         Exception  ExceptionOccurred)
        {

            DebugX.LogException(ExceptionOccurred, $"{Module}.{Caller}");

            return Task.CompletedTask;

        }

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
