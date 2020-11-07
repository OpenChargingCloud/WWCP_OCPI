/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using org.GraphDefined.WWCP;

using cloud.charging.open.protocols.OCPIv2_2.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    #region OnGetVersionsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get versions request will be send.
    /// </summary>
    public delegate Task OnGetVersionsRequestDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get versions request had been received.
    /// </summary>
    public delegate Task OnGetVersionsResponseDelegate(DateTime                                    LogTimestamp,
                                                       DateTime                                    RequestTimestamp,
                                                       CommonClient                                Sender,
                                                       String                                      SenderId,
                                                       EventTracking_Id                            EventTrackingId,

                                                       //Partner_Id                                  PartnerId,
                                                       //Operator_Id                                 OperatorId,
                                                       //ChargingPool_Id                             ChargingPoolId,
                                                       //DateTime                                    StatusEventDate,
                                                       //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                       //Transaction_Id?                             TransactionId,
                                                       //DateTime?                                   AvailabilityStatusUntil,
                                                       //String                                      AvailabilityStatusComment,

                                                       TimeSpan                                    RequestTimeout,
                                                       //SetChargingPoolAvailabilityStatusResponse   Result,
                                                       TimeSpan                                    Duration);

    #endregion

    #region OnGetVersionDetailsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get version details request will be send.
    /// </summary>
    public delegate Task OnGetVersionDetailsRequestDelegate(DateTime                                    LogTimestamp,
                                                            DateTime                                    RequestTimestamp,
                                                            CommonClient                                Sender,
                                                            String                                      SenderId,
                                                            EventTracking_Id                            EventTrackingId,

                                                            //Partner_Id                                  PartnerId,
                                                            //Operator_Id                                 OperatorId,
                                                            //ChargingPool_Id                             ChargingPoolId,
                                                            //DateTime                                    StatusEventDate,
                                                            //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                            //Transaction_Id?                             TransactionId,
                                                            //DateTime?                                   AvailabilityStatusUntil,
                                                            //String                                      AvailabilityStatusComment,

                                                            TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get version details request had been received.
    /// </summary>
    public delegate Task OnGetVersionDetailsResponseDelegate(DateTime                                    LogTimestamp,
                                                             DateTime                                    RequestTimestamp,
                                                             CommonClient                                Sender,
                                                             String                                      SenderId,
                                                             EventTracking_Id                            EventTrackingId,

                                                             //Partner_Id                                  PartnerId,
                                                             //Operator_Id                                 OperatorId,
                                                             //ChargingPool_Id                             ChargingPoolId,
                                                             //DateTime                                    StatusEventDate,
                                                             //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                             //Transaction_Id?                             TransactionId,
                                                             //DateTime?                                   AvailabilityStatusUntil,
                                                             //String                                      AvailabilityStatusComment,

                                                             TimeSpan                                    RequestTimeout,
                                                             //SetChargingPoolAvailabilityStatusResponse   Result,
                                                             TimeSpan                                    Duration);

    #endregion


    /// <summary>
    /// The common client.
    /// </summary>
    public partial class CommonClient : IHTTPClient
    {

        public class CommonCounters
        {

            public CounterValues  GetVersions    { get; }

            public CommonCounters(CounterValues? GetVersions = null)
            {

                this.GetVersions = GetVersions ?? new CounterValues();

            }

            public JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("GetVersions", GetVersions.ToJSON())

                    
                   );

        }


        #region Data

        /// <summary>
        /// The default HTTP port.
        /// </summary>
        public static readonly IPPort    DefaultRemotePort       = IPPort.HTTPS;

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public static readonly TimeSpan  DefaultRequestTimeout   = TimeSpan.FromSeconds(180);


        protected HTTPTokenAuthentication TokenAuth;


        protected readonly Dictionary<Version_Id, URL>           Versions        = new Dictionary<Version_Id, URL>();

        protected readonly Dictionary<Version_Id, VersionDetail> VersionDetails  = new Dictionary<Version_Id, VersionDetail>();


        protected Newtonsoft.Json.Formatting JSONFormat = Newtonsoft.Json.Formatting.Indented;

        #endregion

        #region Properties

        /// <summary>
        /// The access token.
        /// (Might be updated during the REGISTRATION process!)
        /// </summary>
        public AccessToken                          AccessToken                   { get; private set; }


        public Version_Id?                          SelectedOCPIVersionId         { get; set; }

        /// <summary>
        /// The remote URL of the VERSIONS endpoint to connect to.
        /// </summary>
        public URL                                  VersionsURL                   { get; }

        /// <summary>
        /// The remote hostname.
        /// </summary>
        public HTTPHostname                         Hostname                      { get; }

        /// <summary>
        /// The remote HTTPS port.
        /// </summary>
        public IPPort                               RemotePort                    { get; }

        /// <summary>
        /// The remote virtual hostname.
        /// </summary>
        public HTTPHostname?                        VirtualHostname               { get; }

        /// <summary>
        /// The remote SSL/TLS certificate validator.
        /// </summary>
        public RemoteCertificateValidationCallback  RemoteCertificateValidator    { get; }

        /// <summary>
        /// The roaming network identification.
        /// </summary>
        public RoamingNetwork                       RoamingNetwork                { get; }

        /// <summary>
        /// The request timeout.
        /// </summary>
        public TimeSpan?                            RequestTimeout                { get; }

        /// <summary>
        /// The DNS client to use.
        /// </summary>
        public DNSClient                            DNSClient                     { get; }


        /// <summary>
        /// CPO client event counters.
        /// </summary>
        public CommonCounters                       Counters                      { get; }

        /// <summary>
        /// The attached eMIP CPO client (HTTP/SOAP client) logger.
        /// </summary>
        public CommonClientLogger                   Logger                        { get; }

        /// <summary>
        /// The maximum number of transmission retries.
        /// </summary>
        public Byte                                 MaxNumberOfRetries            { get; }

        #endregion

        #region Events

        #region OnGetVersionsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all versions will be send.
        /// </summary>
        public event OnGetVersionsRequestDelegate   OnGetVersionsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all versions will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnGetVersionsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all versions HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetVersionsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all versions request had been received.
        /// </summary>
        public event OnGetVersionsResponseDelegate  OnGetVersionsResponse;

        #endregion

        #region OnGetVersionDetailsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting version details will be send.
        /// </summary>
        public event OnGetVersionsRequestDelegate   OnGetVersionDetailsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting version details will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnGetVersionDetailsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting version details HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetVersionDetailsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting version details request had been received.
        /// </summary>
        public event OnGetVersionsResponseDelegate  OnGetVersionDetailsResponse;

        #endregion


        #region OnPostCredentialsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting version details will be send.
        /// </summary>
        public event OnGetVersionsRequestDelegate   OnPostCredentialsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting version details will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnPostCredentialsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting version details HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnPostCredentialsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting version details request had been received.
        /// </summary>
        public event OnGetVersionsResponseDelegate  OnPostCredentialsResponse;

        #endregion


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP client.
        /// </summary>
        /// <param name="AccessToken">The access token.</param>
        /// <param name="VersionsURL">The remote URL of the VERSIONS endpoint to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="RemoteCertificateValidator">An optional remote SSL/TLS certificate validator.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        public CommonClient(AccessToken                          AccessToken,
                            URL                                  VersionsURL,
                            HTTPHostname?                        VirtualHostname              = null,
                            RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                            TimeSpan?                            RequestTimeout               = null,
                            Byte?                                MaxNumberOfRetries           = null,
                            DNSClient                            DNSClient                    = null)
        {

            this.AccessToken                 = AccessToken;
            this.TokenAuth                   = new HTTPTokenAuthentication(AccessToken.ToString().EncodeBase64());
            this.VersionsURL                 = VersionsURL;
            this.Hostname                    = VersionsURL.Hostname;
            this.RemotePort                  = VersionsURL.Port   ?? DefaultRemotePort;
            this.VirtualHostname             = VirtualHostname;
            this.RemoteCertificateValidator  = RemoteCertificateValidator;
            this.RequestTimeout              = RequestTimeout     ?? DefaultRequestTimeout;
            this.MaxNumberOfRetries          = MaxNumberOfRetries ?? 3;
            this.DNSClient                   = DNSClient;

            this.Counters                    = new CommonCounters();

        }

        #endregion



        public async Task<URL?> GetRemoteURL(Version_Id?     VersionId,
                                             ModuleIDs       ModuleId,
                                             InterfaceRoles  InterfaceRole)
        {

            var versionId = VersionId ?? SelectedOCPIVersionId;

            if (!versionId.HasValue)
            {

                if (VersionDetails.Any())
                    versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                else
                {

                    await GetVersions();

                    if (Versions.Any())
                    {
                        versionId = Versions.Keys.OrderByDescending(id => id).First();
                        await GetVersionDetails(versionId.Value);
                    }

                }

            }

            if (versionId.     HasValue &&
                VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
            {
                foreach (var endpoint in versionDetails.Endpoints)
                {
                    if (endpoint.Identifier == ModuleId &&
                        endpoint.Role       == InterfaceRole)
                    {
                        return endpoint.URL;
                    }

                }
            }

            return new URL?();

        }



        #region Register(AccessToken,

        //  1. We create <CREDENTIALS_TOKEN_A> and associate it with <CountryCode> + <PartyId>.
        //  2. We send <CREDENTIALS_TOKEN_A> and <VERSIONS endpoint> to the other party... e.g. via e-mail.
        //
        //    waiting...
        //
        //  3. Other party fetches <VERSIONS endpoint> using <CREDENTIALS_TOKEN_A> and stores all versions endpoint information.
        //  4. Other party chooses a common version, fetches version details from the found <VERSION DETAIL endpoint> using <CREDENTIALS_TOKEN_A> and stores all version endpoint information.
        //  5. Other party generates <CREDENTIALS_TOKEN_B> and POSTs its choosen version (as part of the request URL), its <VERSIONS endpoint> and this <CREDENTIALS_TOKEN_B>.
        //
        // >>> Here we are in a synchronous HTTP POST request! >>>
        //    6. We verify that the version the other side has choosen is at least valid.
        //    7. We fetch <VERSIONS> from the given <VERSIONS endpoint> using <CREDENTIALS_TOKEN_B> and store all versions endpoint information.
        //    8. We verify that the version the other side has choosen can be used.
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
        /// <param name="CredentialTokenA"></param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<RemoteStartResult>

            Register(//AccessToken         CredentialTokenA,
                     //URL                 VersionsEndpoint,

                     DateTime?           Timestamp           = null,
                     CancellationToken?  CancellationToken   = null,
                     EventTracking_Id    EventTrackingId     = null,
                     TimeSpan?           RequestTimeout      = null)
        {

            #region Initial checks

            //if (CredentialTokenA is null)
            //    throw new ArgumentNullException(nameof(CredentialTokenA), "The given credentials must not be null!");

            #endregion


            var getVersion_HTTPResponse  = await GetVersions();

            if (getVersion_HTTPResponse.StatusCode == 1000)
            {

                var filteredVersions    = getVersion_HTTPResponse.Data.Where(version => version.Id.ToString() == "2.2").ToArray();

                if (filteredVersions.Length == 1)
                {

                    var versionV2_2                        = filteredVersions.First();

                    var getVersionDetails2_2_HTTPResponse  = await GetVersionDetails(Version_Id.Parse("2.2"));



                    var credentialTokenB  = AccessToken.Random();

                    //var 


                }
                else
                    return null; // Proper error message!

            }
            else
                return null; // Proper error message!

            return null;

        }

        #endregion

        #region PostCredentials(Credentials, ...)

        ///// <summary>
        ///// Post the given credentials.
        ///// </summary>
        ///// <param name="Credentials"></param>
        ///// 
        ///// <param name="Timestamp">The optional timestamp of the request.</param>
        ///// <param name="CancellationToken">An optional token to cancel this request.</param>
        ///// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        ///// <param name="RequestTimeout">An optional timeout for this request.</param>
        //public async Task<RemoteStartResult>

        //    PostCredentials(Credentials         Credentials,

        //                    DateTime?           Timestamp           = null,
        //                    CancellationToken?  CancellationToken   = null,
        //                    EventTracking_Id    EventTrackingId     = null,
        //                    TimeSpan?           RequestTimeout      = null)
        //{

        //    #region Initial checks

        //    if (Credentials is null)
        //        throw new ArgumentNullException(nameof(Credentials), "The given credentials must not be null!");

        //    #endregion

        //    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

        //    try
        //    {

        //        #region Upstream HTTP request...

        //        var response = await (RemoteCertificateValidator == null

        //                                   ? new HTTPClient (Hostname,
        //                                                     RemotePort:  RemotePort,
        //                                                     DNSClient:   DNSClient)

        //                                   : new HTTPSClient(Hostname,
        //                                                     RemoteCertificateValidator,
        //                                                     RemotePort:  RemotePort,
        //                                                     DNSClient:   DNSClient)).

        //                                Execute(client => client.CreateRequest(HTTPMethod.POST,
        //                                                                       HTTPPath.Parse("/backend/rest/ocpi/2.2/credentials"),

        //                                                                       requestbuilder => {
        //                                                                           requestbuilder.Host           = VirtualHostname ?? Hostname;
        //                                                                           requestbuilder.Authorization  = TokenAuth;
        //                                                                           requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
        //                                                                           requestbuilder.Content        = Credentials.ToJSON().
        //                                                                                                             ToUTF8Bytes();
        //                                                                           requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
        //                                                                       }),

        //                                      RequestLogDelegate:   OnPostCredentialsHTTPRequest,
        //                                      ResponseLogDelegate:  OnPostCredentialsHTTPResponse,
        //                                      CancellationToken:    CancellationToken,
        //                                      EventTrackingId:      EventTrackingId,
        //                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

        //                                ConfigureAwait(false);

        //        #endregion

              

        //    }

        //    catch (Exception e)
        //    {
        //        return RemoteStartResult.Error(e.Message);
        //    }

        //}

        #endregion




        #region GetVersions(...)

        /// <summary>
        /// Get versions.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Version>>>

            GetVersions(Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken?  CancellationToken   = null,
                        EventTracking_Id    EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            Byte                               TransmissionRetry = 0;
            OCPIResponse<IEnumerable<Version>> response;

            #region Send OnGetVersionsRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                Counters.GetVersions.IncRequests();

                //if (OnGetVersionsRequest != null)
                //    await Task.WhenAll(OnGetVersionsRequest.GetInvocationList().
                //                       Cast<OnGetVersionsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                e.Log(nameof(CommonClient) + "." + nameof(OnGetVersionsRequest));
            }

            #endregion


            do
            {

                try
                {

                    var requestId      = RequestId     ?? Request_Id.Random();
                    var correlationId  = CorrelationId ?? Correlation_Id.Random();

                    // ToDo: Add request logging!


                    #region Upstream HTTP request...

                    var HTTPResponse = await (VersionsURL.Protocol == HTTPProtocols.http

                                                 ? new HTTPClient (VersionsURL.Hostname,
                                                                   RemotePort:  RemotePort,
                                                                   DNSClient:   DNSClient)

                                                 : new HTTPSClient(VersionsURL.Hostname,
                                                                   RemoteCertificateValidator ?? ((a,b,c,d) => true),
                                                                   RemotePort:  RemotePort,
                                                                   DNSClient:   DNSClient)).

                                           Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                  VersionsURL.Path,
                                                                                  requestbuilder => {
                                                                                      //requestbuilder.Host           = VirtualHostname ?? Hostname;
                                                                                      requestbuilder.Authorization  = TokenAuth;
                                                                                      requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                      requestbuilder.Set("X-Request-ID",      requestId);
                                                                                      requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                  }),

                                                 RequestLogDelegate:   OnGetVersionsHTTPRequest,
                                                 ResponseLogDelegate:  OnGetVersionsHTTPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

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

                    response = OCPIResponse<Version>.ParseJArray(HTTPResponse,
                                                                 requestId,
                                                                 correlationId,
                                                                 json => Version.Parse(json));

                    switch (response.StatusCode)
                    {

                        case 1000:

                            lock (Versions)
                            {
                                Versions.Clear();
                                response.Data.ForEach(version => Versions.Add(version.Id, version.URL));
                            }

                            break;

                    }

                }

                catch (Exception e)
                {

                    response = new OCPIResponse<IEnumerable<Version>>(default,
                                                                      -1,
                                                                      e.Message,
                                                                      e.StackTrace);

                }


            }
            while ((response.HTTPResponse.HTTPStatusCode.IsServerError ||
                    response.HTTPResponse.HTTPStatusCode == HTTPStatusCode.RequestTimeout) &&
                   TransmissionRetry++ < MaxNumberOfRetries);


            #region Send OnGetVersionsResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetVersionsResponse != null)
                //    await Task.WhenAll(OnGetVersionsResponse.GetInvocationList().
                //                       Cast<OnGetVersionsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                e.Log(nameof(CommonClient) + "." + nameof(OnGetVersionsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetVersionDetails(VersionId, ...)

        /// <summary>
        /// Get versions.
        /// </summary>
        /// <param name="VersionId">The requested version.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Version_Id, VersionDetail>>

            GetVersionDetails(Version_Id          VersionId,
                              Request_Id?         RequestId           = null,
                              Correlation_Id?     CorrelationId       = null,

                              DateTime?           Timestamp           = null,
                              CancellationToken?  CancellationToken   = null,
                              EventTracking_Id    EventTrackingId     = null,
                              TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Version_Id, VersionDetail> response;



            if (!Versions.ContainsKey(VersionId))
                return new OCPIResponse<Version_Id, VersionDetail>(VersionId,
                                                                   default,
                                                                   -1,
                                                                   "Unkown version identification!");


            try
            {

                var StartTime      = DateTime.UtcNow;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

                // ToDo: Add request logging!


                #region Upstream HTTP request...

                var HTTPResponse = await (Versions[VersionId].Protocol == HTTPProtocols.http

                                             ? new HTTPClient (Versions[VersionId].Hostname,
                                                               RemotePort:  RemotePort,
                                                               DNSClient:   DNSClient)

                                             : new HTTPSClient(Versions[VersionId].Hostname,
                                                               RemoteCertificateValidator ?? ((a,b,c,d) => true),
                                                               RemotePort:  RemotePort,
                                                               DNSClient:   DNSClient)).

                                           Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                  Versions[VersionId].Path,
                                                                                  requestbuilder => {
                                                                                      //requestbuilder.Host           = HTTPHostname.Parse(Versions[VersionId].Hostname + (Versions[VersionId].Port.HasValue ? Versions[VersionId].Port.Value.ToString() : ""));
                                                                                      requestbuilder.Authorization  = TokenAuth;
                                                                                      requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                      requestbuilder.Set("X-Request-ID",      requestId);
                                                                                      requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                  }),

                                                 RequestLogDelegate:   OnGetVersionDetailsHTTPRequest,
                                                 ResponseLogDelegate:  OnGetVersionDetailsHTTPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

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

                response = OCPIResponse<Version_Id, VersionDetail>.ParseJObject(VersionId,
                                                                                requestId,
                                                                                correlationId,
                                                                                HTTPResponse,
                                                                                json => VersionDetail.Parse(json, null));

                switch (response.StatusCode)
                {

                    case 1000:

                        lock (VersionDetails)
                        {

                            if (VersionDetails.ContainsKey(VersionId))
                                VersionDetails.Remove(VersionId);

                            VersionDetails.Add(VersionId, response.Data);

                        }

                        break;

                }

            }

            catch (Exception e)
            {

                response = new OCPIResponse<Version_Id, VersionDetail>(VersionId,
                                                                       default,
                                                                       -1,
                                                                       e.Message,
                                                                       e.StackTrace);

            }


            // ToDo: Add response logging!

            return response;

        }

        #endregion



        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        { }

        #endregion

    }

}
