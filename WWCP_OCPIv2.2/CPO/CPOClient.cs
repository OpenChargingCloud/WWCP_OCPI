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
using System.Linq;
using System.Threading;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{


    #region OnGetLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a get location request will be send.
    /// </summary>
    public delegate Task OnGetLocationRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a get location request had been received.
    /// </summary>
    public delegate Task OnGetLocationResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPutLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a put location request will be send.
    /// </summary>
    public delegate Task OnPutLocationRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a put location request had been received.
    /// </summary>
    public delegate Task OnPutLocationResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPatchLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch location request will be send.
    /// </summary>
    public delegate Task OnPatchLocationRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a patch location request had been received.
    /// </summary>
    public delegate Task OnPatchLocationResponseDelegate(DateTime                                    LogTimestamp,
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
    /// The CPO client.
    /// </summary>
    public partial class CPOClient : CommonClient
    {

        public class CPOCounters
        {

            public CounterValues GetTokens  { get; }
            public CounterValues PostTokens { get; }

            public CPOCounters(CounterValues? GetTokens  = null,
                               CounterValues? PostTokens = null)
            {

                this.GetTokens  = GetTokens  ?? new CounterValues();
                this.PostTokens = PostTokens ?? new CounterValues();

            }

            public JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("GetTokens",  GetTokens. ToJSON()),
                       new JProperty("PostTokens", PostTokens.ToJSON())
                   );

        }


        #region Data

        #endregion

        #region Properties

        #endregion

        #region Events


        #region OnGetLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a location will be send.
        /// </summary>
        public event OnGetLocationRequestDelegate   OnGetLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a location will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnGetLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a location request had been received.
        /// </summary>
        public event OnGetLocationResponseDelegate  OnGetLocationResponse;

        #endregion

        #region OnPutLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a location will be send.
        /// </summary>
        public event OnPutLocationRequestDelegate   OnPutLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a location will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnPutLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnPutLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a location request had been received.
        /// </summary>
        public event OnPutLocationResponseDelegate  OnPutLocationResponse;

        #endregion

        #region OnPatchLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a location will be send.
        /// </summary>
        public event OnPatchLocationRequestDelegate   OnPatchLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a location will be send.
        /// </summary>
        public event ClientRequestLogHandler          OnPatchLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler         OnPatchLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a location request had been received.
        /// </summary>
        public event OnPatchLocationResponseDelegate  OnPatchLocationResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP client.
        /// </summary>
        /// <param name="AccessToken">The access token.</param>
        /// <param name="URL">The remote URL to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="RemoteCertificateValidator">An optional remote SSL/TLS certificate validator.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        public CPOClient(AccessToken                          AccessToken,
                         URL                                  URL,
                         HTTPHostname?                        VirtualHostname              = null,
                         RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                         TimeSpan?                            RequestTimeout               = null,
                         Byte?                                MaxNumberOfRetries           = null,
                         DNSClient                            DNSClient                    = null)

            : base(AccessToken,
                   URL,
                   VirtualHostname,
                   RemoteCertificateValidator,
                   RequestTimeout,
                   MaxNumberOfRetries,
                   DNSClient)

        { }

        #endregion


        #region GetLocation(...)

        /// <summary>
        /// Get a location.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            GetLocation(CountryCode         CountryCode,
                        Party_Id            PartyId,
                        Location_Id         LocationId,

                        Version_Id?         VersionId           = null,
                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken?  CancellationToken   = null,
                        EventTracking_Id    EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Location> response;

            #region Send OnGetLocationRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetLocation.IncRequests();

                //if (OnGetLocationRequest != null)
                //    await Task.WhenAll(OnGetLocationRequest.GetInvocationList().
                //                       Cast<OnGetLocationRequestDelegate>().
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetLocationRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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


                URL LocationURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    LocationURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Locations &&
                                                                                      endpoint.Role       == InterfaceRoles.RECEIVER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse = await (LocationURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (LocationURL.Hostname,
                                                             RemotePort:  LocationURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(LocationURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  LocationURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationURL.Path + CountryCode.ToString() + PartyId.ToString() + LocationId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetLocationHTTPRequest,
                                              ResponseLogDelegate:  OnGetLocationHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Location>.ParseJObject(HTTPResponse,
                                                               requestId,
                                                               correlationId,
                                                               json => Location.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Location>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnGetLocationResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetLocationResponse != null)
                //    await Task.WhenAll(OnGetLocationResponse.GetInvocationList().
                //                       Cast<OnGetLocationResponseDelegate>().
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutLocation(...)

        /// <summary>
        /// Put a location.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            PutLocation(Location            Location,

                        Version_Id?         VersionId           = null,
                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken?  CancellationToken   = null,
                        EventTracking_Id    EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Location> response;

            #region Send OnPutLocationRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PutLocation.IncRequests();

                //if (OnPutLocationRequest != null)
                //    await Task.WhenAll(OnPutLocationRequest.GetInvocationList().
                //                       Cast<OnPutLocationRequestDelegate>().
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnPutLocationRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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


                URL LocationURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    LocationURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Locations &&
                                                                                      endpoint.Role       == InterfaceRoles.RECEIVER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse = await (LocationURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (LocationURL.Hostname,
                                                             RemotePort:  LocationURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(LocationURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  LocationURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                               LocationURL.Path + Location.CountryCode.ToString() + Location.PartyId.ToString() + Location.Id.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                   requestbuilder.Content       = Location.ToJSON().ToUTF8Bytes();
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnPutLocationHTTPRequest,
                                              ResponseLogDelegate:  OnPutLocationHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Location>.ParseJObject(HTTPResponse,
                                                               requestId,
                                                               correlationId,
                                                               json => Location.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Location>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnPutLocationResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPutLocationResponse != null)
                //    await Task.WhenAll(OnPutLocationResponse.GetInvocationList().
                //                       Cast<OnPutLocationResponseDelegate>().
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnPutLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchLocation(...)

        /// <summary>
        /// Patch a location.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            PatchLocation(CountryCode         CountryCode,
                          Party_Id            PartyId,
                          Location_Id         LocationId,
                          JObject             LocationPatch,

                          Version_Id?         VersionId           = null,
                          Request_Id?         RequestId           = null,
                          Correlation_Id?     CorrelationId       = null,

                          DateTime?           Timestamp           = null,
                          CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Location> response;

            #region Send OnPatchLocationRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PatchLocation.IncRequests();

                //if (OnPatchLocationRequest != null)
                //    await Task.WhenAll(OnPatchLocationRequest.GetInvocationList().
                //                       Cast<OnPatchLocationRequestDelegate>().
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnPatchLocationRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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


                URL LocationURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    LocationURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Locations &&
                                                                                      endpoint.Role       == InterfaceRoles.RECEIVER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse = await (LocationURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (LocationURL.Hostname,
                                                             RemotePort:  LocationURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(LocationURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  LocationURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnPatchLocationHTTPRequest,
                                              ResponseLogDelegate:  OnPatchLocationHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Location>.ParseJObject(HTTPResponse,
                                                               requestId,
                                                               correlationId,
                                                               json => Location.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Location>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnPatchLocationResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPatchLocationResponse != null)
                //    await Task.WhenAll(OnPatchLocationResponse.GetInvocationList().
                //                       Cast<OnPatchLocationResponseDelegate>().
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnPatchLocationResponse));
            }

            #endregion

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
