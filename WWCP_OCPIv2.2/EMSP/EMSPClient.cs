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

    /// <summary>
    /// An EMSP client.
    /// </summary>
    public class EMSPClient : IHTTPClient,
                              IRemoteChargingStationOperator
    {

        #region Data

        /// <summary>
        /// The default HTTP port.
        /// </summary>
        public static readonly IPPort    DefaultRemotePort       = IPPort.HTTPS;

        /// <summary>
        /// The default request timeout.
        /// </summary>
        public static readonly TimeSpan  DefaultRequestTimeout   = TimeSpan.FromSeconds(180);


        private readonly HTTPTokenAuthentication TokenAuth;

        #endregion

        #region Properties

        /// <summary>
        /// The access token.
        /// </summary>
        public AccessToken                          AccessToken                   { get; }

        /// <summary>
        /// The common HTTP path prefix.
        /// </summary>
        public HTTPPath                             PathPrefix                    { get; }

        /// <summary>
        /// The remote hostname.
        /// </summary>
        public HTTPHostname                         Hostname                      { get; }

        /// <summary>
        /// The remote virtual hostname.
        /// </summary>
        public HTTPHostname?                        VirtualHostname               { get; }

        /// <summary>
        /// The remote HTTPS port.
        /// </summary>
        public IPPort                               RemotePort                    { get; }

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

        #endregion

        #region Events

        /// <summary>
        /// An event send whenever a GetCDRs request was sent.
        /// </summary>
        public event ClientRequestLogHandler   OnGetCDRsHTTPRequest;

        /// <summary>
        /// An event send whenever a response to a GetCDRs request was received.
        /// </summary>
        public event ClientResponseLogHandler  OnGetCDRsHTTPResponse;


        /// <summary>
        /// An event send whenever a remote reservation start request was sent.
        /// </summary>
        public event ClientRequestLogHandler   OnRemoteReservationStartHTTPRequest;

        /// <summary>
        /// An event send whenever a response to a remote reservation start request was received.
        /// </summary>
        public event ClientResponseLogHandler  OnRemoteReservationStartHTTPResponse;


        /// <summary>
        /// An event send whenever a remote reservation stop request was sent.
        /// </summary>
        public event ClientRequestLogHandler   OnRemoteReservationStopHTTPRequest;

        /// <summary>
        /// An event send whenever a response to a remote reservation stop request was received.
        /// </summary>
        public event ClientResponseLogHandler  OnRemoteReservationStopHTTPResponse;


        /// <summary>
        /// An event send whenever a remotestart request was sent.
        /// </summary>
        public event ClientRequestLogHandler   OnRemoteStartHTTPRequest;

        /// <summary>
        /// An event send whenever a response to a remotestart request was received.
        /// </summary>
        public event ClientResponseLogHandler  OnRemoteStartHTTPResponse;


        /// <summary>
        /// An event send whenever a remotestop request was sent.
        /// </summary>
        public event ClientRequestLogHandler   OnRemoteStopHTTPRequest;

        /// <summary>
        /// An event send whenever a response to a remotestop request was received.
        /// </summary>
        public event ClientResponseLogHandler  OnRemoteStopHTTPResponse;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP client.
        /// </summary>
        /// <param name="AccessToken">The access token.</param>
        /// <param name="Hostname">An optional remote hostname.</param>
        /// <param name="VirtualHostname">An optional remote virtual hostname.</param>
        /// <param name="RemotePort">An optional remote HTTPS port.</param>
        /// <param name="RemoteCertificateValidator">An optional remote SSL/TLS certificate validator.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        public EMSPClient(//RoamingNetwork                       RoamingNetwork,
                          AccessToken                          AccessToken,
                          HTTPHostname                         Hostname,
                          HTTPPath?                            PathPrefix                   = null,
                          HTTPHostname?                        VirtualHostname              = null,
                          IPPort?                              RemotePort                   = null,
                          RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                          TimeSpan?                            RequestTimeout               = null,
                          DNSClient                            DNSClient                    = null)
        {

         //   this.RoamingNetwork              = RoamingNetwork             ?? throw new ArgumentNullException(nameof(RoamingNetwork), "The given roaming network must not be null!");
            this.AccessToken                 = AccessToken;
            this.TokenAuth                   = new HTTPTokenAuthentication(AccessToken.ToString().ToBase64());    //"Ms9_ZcT/s&Da}MY6W4v]u%{L4&Rzoh6GiZRo<hu+QTc+#7^SUyXG?ujGw3H_?F5U")
            this.Hostname                    = Hostname;
            this.PathPrefix                  = PathPrefix                 ?? HTTPPath.Parse("/");
            this.VirtualHostname             = VirtualHostname            ?? this.Hostname;
            this.RemotePort                  = RemotePort                 ?? DefaultRemotePort;
            this.RemoteCertificateValidator  = RemoteCertificateValidator;// ?? ((sender, certificate, chain, policyErrors) => true); // Otherwise forcing HTTP will not work!
            this.RequestTimeout              = RequestTimeout             ?? DefaultRequestTimeout;
            this.DNSClient                   = DNSClient;

        }

        #endregion


        #region PostCredentials(AccessToken, ...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<RemoteStartResult>

            PostCredentials(Credentials         Cred,
            
                            DateTime?           Timestamp           = null,
                            CancellationToken?  CancellationToken   = null,
                            EventTracking_Id    EventTrackingId     = null,
                            TimeSpan?           RequestTimeout      = null)
        {

            #region Initial checks


            #endregion

            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            try
            {

                #region Upstream HTTP request...

                var requestTask = (RemoteCertificateValidator == null

                                           ? new HTTPClient (Hostname,
                                                             RemotePort:  RemotePort,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  RemotePort,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                               HTTPPath.Parse("/backend/rest/ocpi/2.2/credentials"),

                                                                               requestbuilder => {
                                                                                   requestbuilder.Host           = VirtualHostname ?? Hostname;
                                                                                   requestbuilder.Authorization  = TokenAuth;
                                                                                   requestbuilder.ContentType    = HTTPContentType.JSON_UTF8;
                                                                                   requestbuilder.Content        = Cred.ToJSON().
                                                                                                                     ToUTF8Bytes();
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                               }),

                                              RequestLogDelegate:   OnRemoteStartHTTPRequest,
                                              ResponseLogDelegate:  OnRemoteStartHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout);//.ContinueWith(t => { Thread.Sleep(6000); return t.Result; });

                //                        ConfigureAwait(false);

                #endregion

                var StartTime    = DateTime.UtcNow;
                var timeout      = TimeSpan.FromSeconds(7);
                var timeoutTask  = Task.Delay(timeout);
                var resultTask   = await Task.WhenAny(requestTask, timeoutTask).
                                              ConfigureAwait(false);

                RemoteStartResult result;

                if (resultTask == timeoutTask)
                    result = result = RemoteStartResult.Error();
                             //RemoteStartResult.AsyncOperation(new ChargingSession(SessionId.Value) {
                             //                                     EVSEId               = EVSEId,
                             //                                     ChargingProduct      = ChargingProduct,
                             //                                     ReservationId        = ReservationId,
                             //                                     ProviderIdStart      = ProviderId,
                             //                                     AuthenticationStart  = RemoteAuthentication
                             //                                 },
                             //                                 Runtime: DateTime.UtcNow - StartTime);

                else
                {

                    var httpresult = requestTask.Result;

                    #region HTTPStatusCode.OK

                    if (httpresult.HTTPStatusCode == HTTPStatusCode.OK)
                    {

                        // HTTP/1.1 200
                        // Server:                  nginx/1.10.1
                        // Date:                    Sat, 16 May 2020 05:23:13 GMT
                        // Content-Type:            application/json;charset=utf-8
                        // Content-Length:          22
                        // Connection:              keep-alive
                        // X-Content-Type-Options:  nosniff
                        // X-XSS-Protection:        1; mode=block
                        // Cache-Control:           no-cache, no-store, max-age=0, must-revalidate
                        // Pragma:                  no-cache
                        // Expires:                 0
                        // X-Frame-Options:         DENY
                        // 
                        // {"code":"Not_Allowed"}

                        try
                        {

                            var JSONResponse = JObject.Parse(httpresult.HTTPBody?.ToUTF8String());

                            //switch (JSONResponse["code"]?.Value<String>())
                            //{

                            //    // Hubject error codes
                            //    // -------------------
                            //    // 400 Session is invalid.
                            //    // 501 Communication to EVSE failed.
                            //    // 510 No EV connected to EVSE.
                            //    // 601 EVSE already reserved.
                            //    // 602 EVSE already in use/ wrong token.
                            //    // 603 Unknown EVSE ID.
                            //    // 604 EVSE ID is not Hubject compatible.
                            //    // 700 EVSE out of service.

                            //    case "EVSE_AlreadyInUse":
                            //        result = RemoteStartResult.AlreadyInUse       (httpresult.Runtime);
                            //        break;

                            //    case "SessionId_AlreadyInUse":
                            //        result = RemoteStartResult.InvalidSessionId   (httpresult.Runtime);
                            //        break;

                            //    case "Rejected":
                            //        result = RemoteStartResult.NoEVConnectedToEVSE(httpresult.Runtime);
                            //        break;

                            //    case "EVSE_Unknown":
                            //        result = RemoteStartResult.UnknownLocation    (httpresult.Runtime);
                            //        break;

                            //    case "EVSE_NotReachable":
                            //        result = RemoteStartResult.Offline            (httpresult.Runtime);
                            //        break;

                            //    case "Not_Allowed":
                            //        result = RemoteStartResult.InvalidCredentials (httpresult.Runtime);
                            //        break;

                            //    case "Start_Timeout":
                            //        result = RemoteStartResult.Timeout(Runtime:    httpresult.Runtime);
                            //        break;

                            //    case "Success":
                            //        result = RemoteStartResult.Success(new ChargingSession(SessionId.Value) {
                            //                                               EVSEId               = EVSEId,
                            //                                               ChargingProduct      = ChargingProduct,
                            //                                               ReservationId        = ReservationId,
                            //                                               ProviderIdStart      = ProviderId,
                            //                                               AuthenticationStart  = RemoteAuthentication
                            //                                           },
                            //                                           httpresult.Runtime);
                            //        break;

                            //    default:
                                    result = RemoteStartResult.Error();
                            //        break;

                            //}

                        }
                        catch (Exception e)
                        {
                            result = RemoteStartResult.Error("chargeIT mobility REMOTESTART response JSON could not be parsed: " +
                                                             e.Message    + Environment.NewLine +
                                                             e.StackTrace + Environment.NewLine +
                                                             httpresult.EntirePDU);
                        }

                    }

                    #endregion

                    else
                        result = RemoteStartResult.Error(httpresult.HTTPStatusCode.ToString(),
                                                         httpresult.EntirePDU);

                }

                return result;

            }

            catch (Exception e)
            {
                return RemoteStartResult.Error(e.Message);
            }

        }

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

            GetVersions(DateTime?                Timestamp              = null,
                        CancellationToken?       CancellationToken      = null,
                        EventTracking_Id         EventTrackingId        = null,
                        TimeSpan?                RequestTimeout         = null)
        {

            OCPIResponse<IEnumerable<Version>> response;

            try
            {

                var StartTime = DateTime.UtcNow;

                // ToDo: Add request logging!


                #region Upstream HTTP request...

                var HTTPResponse = await (RemoteCertificateValidator == null

                                          ? new HTTPClient (Hostname,
                                                            RemotePort:  RemotePort,
                                                            DNSClient:   DNSClient)

                                          : new HTTPSClient(Hostname,
                                                            RemoteCertificateValidator,
                                                            RemotePort:  RemotePort,
                                                            DNSClient:   DNSClient)).

                                       Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                              PathPrefix + "versions",

                                                                              requestbuilder => {
                                                                                  requestbuilder.Host           = VirtualHostname ?? Hostname;
                                                                                  requestbuilder.Authorization  = TokenAuth;
                                                                                  //requestbuilder.ContentType  = HTTPContentType.JSON_UTF8;
                                                                                  //requestbuilder.Content      = JSONObject.Create(

                                                                                  //                              ).ToString().
                                                                                  //                                ToUTF8Bytes();
                                                                                  requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                              }),

                                             RequestLogDelegate:   OnRemoteStartHTTPRequest,
                                             ResponseLogDelegate:  OnRemoteStartHTTPResponse,
                                             CancellationToken:    CancellationToken,
                                             EventTrackingId:      EventTrackingId,
                                             RequestTimeout:       RequestTimeout ?? this.RequestTimeout);//.ContinueWith(t => { Thread.Sleep(6000); return t.Result; });

                //                   ConfigureAwait(false);

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
                                                             json => Version.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<IEnumerable<Version>>(default,
                                                                  -1,
                                                                  e.Message,
                                                                  e.StackTrace);

            }


            // ToDo: Add response logging!

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
        public async Task<OCPIResponse<String, JObject>>

            GetVersionDetails(String              VersionId,

                              DateTime?           Timestamp           = null,
                              CancellationToken?  CancellationToken   = null,
                              EventTracking_Id    EventTrackingId     = null,
                              TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<String, JObject> response;

            try
            {

                var StartTime = DateTime.UtcNow;

                // ToDo: Add request logging!


                #region Upstream HTTP request...

                var HTTPResponse = await (RemoteCertificateValidator == null

                                              ? new HTTPClient (Hostname,
                                                                RemotePort:  RemotePort,
                                                                DNSClient:   DNSClient)

                                              : new HTTPSClient(Hostname,
                                                                RemoteCertificateValidator,
                                                                RemotePort:  RemotePort,
                                                                DNSClient:   DNSClient)).

                                           Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                  (PathPrefix + "versions") + VersionId,

                                                                                  requestbuilder => {
                                                                                      requestbuilder.Host           = VirtualHostname ?? Hostname;
                                                                                      requestbuilder.Authorization  = TokenAuth;
                                                                                      //requestbuilder.ContentType  = HTTPContentType.JSON_UTF8;
                                                                                      //requestbuilder.Content      = JSONObject.Create(

                                                                                      //                              ).ToString().
                                                                                      //                                ToUTF8Bytes();
                                                                                      requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                  }),

                                                 RequestLogDelegate:   OnRemoteStartHTTPRequest,
                                                 ResponseLogDelegate:  OnRemoteStartHTTPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout ?? this.RequestTimeout);//.ContinueWith(t => { Thread.Sleep(6000); return t.Result; });

                //                   ConfigureAwait(false);

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

                response = OCPIResponse<String, JObject>.ParseJObject(VersionId,
                                                                      HTTPResponse,
                                                                      json => json);

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, JObject>(VersionId,
                                                             default,
                                                             -1,
                                                             e.Message,
                                                             e.StackTrace);

            }


            // ToDo: Add response logging!

            return response;

        }

        #endregion



        #region GetLocations(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Location>>>

            GetLocations(DateTime?           Timestamp           = null,
                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Location>> response;

            try
            {

                var StartTime = DateTime.UtcNow;

                // ToDo: Add request logging!


                #region Upstream HTTP request...

                var HTTPResponse = await (RemoteCertificateValidator == null

                                           ? new HTTPClient (Hostname,
                                                             RemotePort:  RemotePort,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  RemotePort,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               PathPrefix + "cpo/2.2/locations",

                                                                               requestbuilder => {
                                                                                   requestbuilder.Host           = VirtualHostname ?? Hostname;
                                                                                   requestbuilder.Authorization  = new HTTPTokenAuthentication(AccessToken.ToString());
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                               }),

                                              RequestLogDelegate:   OnRemoteStartHTTPRequest,
                                              ResponseLogDelegate:  OnRemoteStartHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout);//.ContinueWith(t => { Thread.Sleep(6000); return t.Result; });

                //                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Location>.ParseJArray(HTTPResponse,
                                                              json => Location.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, IEnumerable<Location>>("",
                                                                           default,
                                                                           -1,
                                                                           e.Message,
                                                                           e.StackTrace);

            }


            // ToDo: Add response logging!

            return response;

        }

        #endregion









        // http://portal.chargeit-mobility.com/ps/rest/ext/gateway/CDRs?from=2018-07-01

        #region GetCDRs(...)

        public async Task<RemoteStartResult>

            GetCDRs(DateTime            From,
                    DNSClient           DNSClient           = null,

                    DateTime?           Timestamp           = null,
                    CancellationToken?  CancellationToken   = null,
                    EventTracking_Id    EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null)

        {

            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            try
            {

                #region Upstream HTTP request...

                var httpresult = await new HTTPSClient(Hostname,
                                                       RemoteCertificateValidator,
                                                       RemotePort:  RemotePort,
                                                       DNSClient:   DNSClient ?? this.DNSClient).

                                           Execute(client => client.GET(HTTPPath.Parse("/ps/rest/ext/gateway/CDRs" + From.ToString("yyyy-MM-dd")),

                                                                        requestbuilder => {
                                                                            requestbuilder.Host = VirtualHostname ?? Hostname;
                                                                            requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                        }),

                                                   RequestLogDelegate:   OnGetCDRsHTTPRequest,
                                                   ResponseLogDelegate:  OnGetCDRsHTTPResponse,
                                                   CancellationToken:    CancellationToken,
                                                   EventTrackingId:      EventTrackingId,
                                                   RequestTimeout:       RequestTimeout ?? TimeSpan.FromSeconds(60)).

                                           ConfigureAwait(false);

                #endregion

                var result  = RemoteStartResult.Error();

                #region HTTPStatusCode.OK

                if (httpresult.HTTPStatusCode == HTTPStatusCode.OK)
                {

                    // HTTP/1.1 200 OK
                    // Date: Fri, 28 Mar 2014 13:31:27 GMT
                    // Server: Apache/2.2.9 (Debian) mod_jk/1.2.26
                    // Content-Length: 34
                    // Content-Type: application/json
                    // 
                    // {
                    //   "code" : "EVSE_AlreadyInUse"
                    // }

                    JObject JSONResponse = null;

                    try
                    {

                        JSONResponse = JObject.Parse(httpresult.HTTPBody.ToUTF8String());

                    }
                    catch (Exception e)
                    {
                        DebugX.LogT("chargeIT mobility REMOTESTART response JSON could not be parsed! " + e.Message + " // " + httpresult.EntirePDU);
                        throw new Exception("chargeIT mobility REMOTESTART response JSON could not be parsed: " + e.Message);
                    }

                    switch (JSONResponse["code"].Value<String>())
                    {

                        case "EVSE_AlreadyInUse":
                            result = RemoteStartResult.AlreadyInUse    (httpresult.Runtime);
                            break;

                        case "SessionId_AlreadyInUse":
                            result = RemoteStartResult.InvalidSessionId(httpresult.Runtime);
                            break;

                        case "EVSE_Unknown":
                            result = RemoteStartResult.UnknownLocation (httpresult.Runtime);
                            break;

                        case "EVSE_NotReachable":
                            result = RemoteStartResult.Offline         (httpresult.Runtime);
                            break;

                        case "Start_Timeout":
                            result = RemoteStartResult.Timeout(Runtime: httpresult.Runtime);
                            break;

                        default:
                            result = RemoteStartResult.Error  (Runtime: httpresult.Runtime);
                            break;

                    }

                }

                #endregion

                return result;

            }

            catch (Exception e)
            {
                return RemoteStartResult.Error(e.Message);
            }

        }

        #endregion


        #region Reservations...

        #region Data

        private readonly Dictionary<ChargingReservation_Id, ChargingReservation> _Reservations;

        /// <summary>
        /// All current charging reservations.
        /// </summary>
        public IEnumerable<ChargingReservation> ChargingReservations
            => _Reservations.Select(_ => _.Value);

        #region TryGetReservationById(ReservationId, out Reservation)

        /// <summary>
        /// Return the charging reservation specified by the given identification.
        /// </summary>
        /// <param name="ReservationId">The charging reservation identification.</param>
        /// <param name="Reservation">The charging reservation.</param>
        public Boolean TryGetChargingReservationById(ChargingReservation_Id ReservationId, out ChargingReservation Reservation)
            => _Reservations.TryGetValue(ReservationId, out Reservation);

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a charging location is being reserved.
        /// </summary>
        public event OnReserveRequestDelegate             OnReserveRequest;

        /// <summary>
        /// An event fired whenever a charging location was reserved.
        /// </summary>
        public event OnReserveResponseDelegate            OnReserveResponse;


        /// <summary>
        /// An event fired whenever a new charging reservation was created.
        /// </summary>
        public event OnNewReservationDelegate             OnNewReservation;

        /// <summary>
        /// An event fired whenever a charging reservation was deleted.
        /// </summary>
        public event OnCancelReservationResponseDelegate  OnCancelReservationResponse;

        #endregion

        #region Reserve(ChargingLocation, ReservationLevel = EVSE, StartTime = null, Duration = null, ReservationId = null, ProviderId = null, ...)

        /// <summary>
        /// Reserve the possibility to charge at the given charging location.
        /// </summary>
        /// <param name="ChargingLocation">A charging location.</param>
        /// <param name="ReservationLevel">The level of the reservation to create (EVSE, charging station, ...).</param>
        /// <param name="StartTime">The starting time of the reservation.</param>
        /// <param name="Duration">The duration of the reservation.</param>
        /// <param name="ReservationId">An optional unique identification of the reservation. Mandatory for updates.</param>
        /// <param name="ProviderId">An optional unique identification of e-Mobility service provider.</param>
        /// <param name="RemoteAuthentication">An optional unique identification of e-Mobility account/customer requesting this reservation.</param>
        /// <param name="ChargingProduct">The charging product to be reserved.</param>
        /// <param name="AuthTokens">A list of authentication tokens, who can use this reservation.</param>
        /// <param name="eMAIds">A list of eMobility account identifications, who can use this reservation.</param>
        /// <param name="PINs">A list of PINs, who can be entered into a pinpad to use this reservation.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public Task<ReservationResult>

            Reserve(ChargingLocation                  ChargingLocation,
                    ChargingReservationLevel          ReservationLevel       = ChargingReservationLevel.EVSE,
                    DateTime?                         StartTime              = null,
                    TimeSpan?                         Duration               = null,
                    ChargingReservation_Id?           ReservationId          = null,
                    eMobilityProvider_Id?             ProviderId             = null,
                    RemoteAuthentication              RemoteAuthentication   = null,
                    ChargingProduct                   ChargingProduct        = null,
                    IEnumerable<Auth_Token>           AuthTokens             = null,
                    IEnumerable<eMobilityAccount_Id>  eMAIds                 = null,
                    IEnumerable<UInt32>               PINs                   = null,

                    DateTime?                         Timestamp              = null,
                    CancellationToken?                CancellationToken      = null,
                    EventTracking_Id                  EventTrackingId        = null,
                    TimeSpan?                         RequestTimeout         = null)

        {

            throw new NotImplementedException();

        }

        #endregion


        #region CancelReservation(ReservationId, Reason, ProviderId = null, ...)

        /// <summary>
        /// Try to remove the given charging reservation.
        /// </summary>
        /// <param name="ReservationId">The unique charging reservation identification.</param>
        /// <param name="Reason">A reason for this cancellation.</param>
        /// <param name="ProviderId">An optional unique identification of e-Mobility service provider.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<CancelReservationResult>

            CancelReservation(ChargingReservation_Id                 ReservationId,
                              ChargingReservationCancellationReason  Reason,
                             // eMobilityProvider_Id?                  ProviderId         = null,

                              DateTime?                              Timestamp          = null,
                              CancellationToken?                     CancellationToken  = null,
                              EventTracking_Id                       EventTrackingId    = null,
                              TimeSpan?                              RequestTimeout     = null)

        {

            throw new NotImplementedException();

        }

        #endregion

        #endregion

        #region RemoteStart/-Stop and Sessions...

        #region Data

        public IEnumerable<ChargingSession> ChargingSessions
            => RoamingNetwork.SessionsStore;

        #region TryGetChargingSessionById(SessionId, out ChargingSession)

        /// <summary>
        /// Return the charging session specified by the given identification.
        /// </summary>
        /// <param name="SessionId">The charging session identification.</param>
        /// <param name="ChargingSession">The charging session.</param>
        public Boolean TryGetChargingSessionById(ChargingSession_Id SessionId, out ChargingSession ChargingSession)
            => RoamingNetwork.SessionsStore.TryGet(SessionId, out ChargingSession);

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// An event fired whenever a remote start command was received.
        /// </summary>
        public event OnRemoteStartRequestDelegate     OnRemoteStartRequest;

        /// <summary>
        /// An event fired whenever a remote start command completed.
        /// </summary>
        public event OnRemoteStartResponseDelegate    OnRemoteStartResponse;


        /// <summary>
        /// An event fired whenever a remote stop command was received.
        /// </summary>
        public event OnRemoteStopRequestDelegate      OnRemoteStopRequest;

        /// <summary>
        /// An event fired whenever a remote stop command completed.
        /// </summary>
        public event OnRemoteStopResponseDelegate     OnRemoteStopResponse;


        /// <summary>
        /// An event fired whenever a new charging session was created.
        /// </summary>
        public event OnNewChargingSessionDelegate     OnNewChargingSession;

        /// <summary>
        /// An event fired whenever a new charge detail record was created.
        /// </summary>
        public event OnNewChargeDetailRecordDelegate  OnNewChargeDetailRecord;
        public event OnCancelReservationRequestDelegate OnCancelReservationRequest;
        public event OnReservationCanceledDelegate OnReservationCanceled;

        #endregion

        #region RemoteStart(ChargingLocation, ChargingProduct = null, ReservationId = null, SessionId = null, ProviderId = null, RemoteAuthentication = null, ...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
        /// <param name="ChargingLocation">The charging location.</param>
        /// <param name="ChargingProduct">The choosen charging product.</param>
        /// <param name="ReservationId">The unique identification for a charging reservation.</param>
        /// <param name="SessionId">The unique identification for this charging session.</param>
        /// <param name="ProviderId">The unique identification of the e-mobility service provider for the case it is different from the current message sender.</param>
        /// <param name="RemoteAuthentication">The unique identification of the e-mobility account.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<RemoteStartResult>

            RemoteStart(ChargingLocation         ChargingLocation,
                        ChargingProduct          ChargingProduct        = null,
                        ChargingReservation_Id?  ReservationId          = null,
                        ChargingSession_Id?      SessionId              = null,
                        eMobilityProvider_Id?    ProviderId             = null,
                        RemoteAuthentication     RemoteAuthentication   = null,

                        DateTime?                Timestamp              = null,
                        CancellationToken?       CancellationToken      = null,
                        EventTracking_Id         EventTrackingId        = null,
                        TimeSpan?                RequestTimeout         = null)
        {

            #region Initial checks

            if (!SessionId.HasValue)
                SessionId = ChargingSession_Id.New;

            if (!ProviderId.HasValue)
                throw new ArgumentNullException(nameof(ProviderId),            "The given e-mobility provider identification must not be null!");

            if (RemoteAuthentication == null)
                throw new ArgumentNullException(nameof(RemoteAuthentication),  "The given remote authentication must not be null!");

            #endregion

            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            try
            {

                var EVSEId = ChargingLocation.EVSEId.Value;

                #region Upstream HTTP request...

                var requestTask = (RemoteCertificateValidator == null

                                           ? new HTTPClient (Hostname,
                                                             RemotePort:  RemotePort,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  RemotePort,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                               HTTPPath.Parse("/ps/rest/hubject/RNs/" + RoamingNetwork.Id + "/EVSEs/" + EVSEId.ToString().Replace("+", "")),

                                                                             requestbuilder => {
                                                                                 requestbuilder.Host         = VirtualHostname ?? Hostname;
                                                                                 requestbuilder.ContentType  = HTTPContentType.JSON_UTF8;
                                                                                 requestbuilder.Content      = JSONObject.Create(

                                                                                                                         new JProperty("@context",             "https://open.charging.cloud/contexts/wwcp/remoteStartRequest"),

                                                                                                                   SessionId.HasValue
                                                                                                                       ? new JProperty("@id",                  SessionId.ToString())
                                                                                                                       : null,

                                                                                                                   ProviderId.HasValue
                                                                                                                       ? new JProperty("ProviderId",           ProviderId.ToString())
                                                                                                                       : null,


                                                                                                                   RemoteAuthentication.AuthToken.                  HasValue
                                                                                                                       ? new JProperty("authToken",            RemoteAuthentication.AuthToken.                  Value.      ToString())
                                                                                                                       : null,

                                                                                                                   //RemoteAuthentication.QRCodeIdentification.       HasValue
                                                                                                                   //    ? new JProperty("eMAId",                RemoteAuthentication.QRCodeIdentification.       Value.eMAId.ToString())
                                                                                                                   //    : null,

                                                                                                                   //RemoteAuthentication.PlugAndChargeIdentification.HasValue
                                                                                                                   //    ? new JProperty("eMAId",                RemoteAuthentication.PlugAndChargeIdentification.Value.      ToString())
                                                                                                                   //    : null,

                                                                                                                   //RemoteAuthentication.RemoteIdentification.       HasValue
                                                                                                                   //    ? new JProperty("eMAId",                RemoteAuthentication.RemoteIdentification.       Value.      ToString())
                                                                                                                   //    : null,

                                                                                                                   RemoteAuthentication.PIN.                        IsNotNullOrEmpty()
                                                                                                                       ? new JProperty("PIN",                  RemoteAuthentication.PIN)
                                                                                                                       : null,


                                                                                                                   new JProperty("eMAId",                      RemoteAuthentication.AuthToken.HasValue
                                                                                                                                                                   ? RemoteAuthentication.AuthToken.                  Value.      ToString()

                                                                                                                                                             : RemoteAuthentication.QRCodeIdentification.HasValue
                                                                                                                                                                   ? RemoteAuthentication.QRCodeIdentification.       Value.eMAId.ToString()

                                                                                                                                                             : RemoteAuthentication.PlugAndChargeIdentification.HasValue
                                                                                                                                                                   ? RemoteAuthentication.PlugAndChargeIdentification.Value.      ToString()

                                                                                                                                                             : RemoteAuthentication.RemoteIdentification.HasValue
                                                                                                                                                                   ? RemoteAuthentication.RemoteIdentification.       Value.      ToString()

                                                                                                                                                             : RemoteAuthentication.PIN.IsNotNullOrEmpty()
                                                                                                                                                                   ? RemoteAuthentication.PIN
                                                                                                                                                                   : "unknown"),


                                                                                                                   new JProperty("authId",                     RemoteAuthentication.AuthToken.HasValue
                                                                                                                                                                   ? RemoteAuthentication.AuthToken.                  Value.      ToString()

                                                                                                                                                             : RemoteAuthentication.QRCodeIdentification.HasValue
                                                                                                                                                                   ? RemoteAuthentication.QRCodeIdentification.       Value.eMAId.ToString()

                                                                                                                                                             : RemoteAuthentication.PlugAndChargeIdentification.HasValue
                                                                                                                                                                   ? RemoteAuthentication.PlugAndChargeIdentification.Value.      ToString()

                                                                                                                                                             : RemoteAuthentication.RemoteIdentification.HasValue
                                                                                                                                                                   ? RemoteAuthentication.RemoteIdentification.       Value.      ToString()

                                                                                                                                                             : RemoteAuthentication.PIN.IsNotNullOrEmpty()
                                                                                                                                                                   ? RemoteAuthentication.PIN
                                                                                                                                                                   : "unknown"),


                                                                                                                   new JProperty("authType",                   RemoteAuthentication.AuthToken.HasValue
                                                                                                                                                                   ? "RFID"

                                                                                                                                                             : RemoteAuthentication.QRCodeIdentification.HasValue
                                                                                                                                                                   ? "QRCode"

                                                                                                                                                             : RemoteAuthentication.PlugAndChargeIdentification.HasValue
                                                                                                                                                                   ? "PlugAndCharge"

                                                                                                                                                             : RemoteAuthentication.RemoteIdentification.HasValue
                                                                                                                                                                   ? "eMAId"

                                                                                                                                                             : RemoteAuthentication.PIN.IsNotNullOrEmpty()
                                                                                                                                                                   ? "PIN"
                                                                                                                                                                   : "unknown"),


                                                                                                                   ChargingProduct != null
                                                                                                                       ? new JProperty("ChargingProductId",    ChargingProduct.ToString())
                                                                                                                       : null,

                                                                                                                   ChargingProduct != null
                                                                                                                       ? new JProperty("chargingProduct",      ChargingProduct.ToJSON())
                                                                                                                       : null

                                                                                                               ).ToString().
                                                                                                                 ToUTF8Bytes();
                                                                                 requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                             }),

                                              RequestLogDelegate:   OnRemoteStartHTTPRequest,
                                              ResponseLogDelegate:  OnRemoteStartHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout);//.ContinueWith(t => { Thread.Sleep(6000); return t.Result; });

                //                        ConfigureAwait(false);

                #endregion

                var StartTime    = DateTime.UtcNow;
                var timeout      = TimeSpan.FromSeconds(7);
                var timeoutTask  = Task.Delay(timeout);
                var resultTask   = await Task.WhenAny(requestTask, timeoutTask).
                                              ConfigureAwait(false);

                RemoteStartResult result;

                if (resultTask == timeoutTask)
                    result = RemoteStartResult.AsyncOperation(new ChargingSession(SessionId.Value) {
                                                                  EVSEId               = EVSEId,
                                                                  ChargingProduct      = ChargingProduct,
                                                                  ReservationId        = ReservationId,
                                                                  ProviderIdStart      = ProviderId,
                                                                  AuthenticationStart  = RemoteAuthentication
                                                              },
                                                              Runtime: DateTime.UtcNow - StartTime);

                else
                {

                    var httpresult = requestTask.Result;

                    #region HTTPStatusCode.OK

                    if (httpresult.HTTPStatusCode == HTTPStatusCode.OK)
                    {

                        // HTTP/1.1 200
                        // Server:                  nginx/1.10.1
                        // Date:                    Sat, 16 May 2020 05:23:13 GMT
                        // Content-Type:            application/json;charset=utf-8
                        // Content-Length:          22
                        // Connection:              keep-alive
                        // X-Content-Type-Options:  nosniff
                        // X-XSS-Protection:        1; mode=block
                        // Cache-Control:           no-cache, no-store, max-age=0, must-revalidate
                        // Pragma:                  no-cache
                        // Expires:                 0
                        // X-Frame-Options:         DENY
                        // 
                        // {"code":"Not_Allowed"}

                        try
                        {

                            var JSONResponse = JObject.Parse(httpresult.HTTPBody?.ToUTF8String());

                            switch (JSONResponse["code"]?.Value<String>())
                            {

                                // Hubject error codes
                                // -------------------
                                // 400 Session is invalid.
                                // 501 Communication to EVSE failed.
                                // 510 No EV connected to EVSE.
                                // 601 EVSE already reserved.
                                // 602 EVSE already in use/ wrong token.
                                // 603 Unknown EVSE ID.
                                // 604 EVSE ID is not Hubject compatible.
                                // 700 EVSE out of service.

                                case "EVSE_AlreadyInUse":
                                    result = RemoteStartResult.AlreadyInUse       (httpresult.Runtime);
                                    break;

                                case "SessionId_AlreadyInUse":
                                    result = RemoteStartResult.InvalidSessionId   (httpresult.Runtime);
                                    break;

                                case "Rejected":
                                    result = RemoteStartResult.NoEVConnectedToEVSE(httpresult.Runtime);
                                    break;

                                case "EVSE_Unknown":
                                    result = RemoteStartResult.UnknownLocation    (httpresult.Runtime);
                                    break;

                                case "EVSE_NotReachable":
                                    result = RemoteStartResult.Offline            (httpresult.Runtime);
                                    break;

                                case "Not_Allowed":
                                    result = RemoteStartResult.InvalidCredentials (httpresult.Runtime);
                                    break;

                                case "Start_Timeout":
                                    result = RemoteStartResult.Timeout(Runtime:    httpresult.Runtime);
                                    break;

                                case "Success":
                                    result = RemoteStartResult.Success(new ChargingSession(SessionId.Value) {
                                                                           EVSEId               = EVSEId,
                                                                           ChargingProduct      = ChargingProduct,
                                                                           ReservationId        = ReservationId,
                                                                           ProviderIdStart      = ProviderId,
                                                                           AuthenticationStart  = RemoteAuthentication
                                                                       },
                                                                       httpresult.Runtime);
                                    break;

                                default:
                                    result = RemoteStartResult.Error();
                                    break;

                            }

                        }
                        catch (Exception e)
                        {
                            result = RemoteStartResult.Error("chargeIT mobility REMOTESTART response JSON could not be parsed: " +
                                                             e.Message    + Environment.NewLine +
                                                             e.StackTrace + Environment.NewLine +
                                                             httpresult.EntirePDU);
                        }

                    }

                    #endregion

                    else
                        result = RemoteStartResult.Error(httpresult.HTTPStatusCode.ToString(),
                                                         httpresult.EntirePDU);

                }

                return result;

            }

            catch (Exception e)
            {
                return RemoteStartResult.Error(e.Message);
            }

        }

        #endregion

        #region RemoteStop (SessionId, ReservationHandling = null, ProviderId = null, RemoteAuthentication = null, ...)

        /// <summary>
        /// Stop the given charging session.
        /// </summary>
        /// <param name="SessionId">The unique identification for this charging session.</param>
        /// <param name="ReservationHandling">Whether to remove the reservation after session end, or to keep it open for some more time.</param>
        /// <param name="ProviderId">The unique identification of the e-mobility service provider.</param>
        /// <param name="RemoteAuthentication">The unique identification of the e-mobility account.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<RemoteStopResult>

            RemoteStop(ChargingSession_Id     SessionId,
                       ReservationHandling?   ReservationHandling    = null,
                       eMobilityProvider_Id?  ProviderId             = null,
                       RemoteAuthentication   RemoteAuthentication   = null,

                       DateTime?              Timestamp              = null,
                       CancellationToken?     CancellationToken      = null,
                       EventTracking_Id       EventTrackingId        = null,
                       TimeSpan?              RequestTimeout         = null)

        {

            #region Initial checks

            if (!ProviderId.HasValue)
                throw new ArgumentNullException(nameof(ProviderId), "The given e-mobility provider identification must not be null!");

            #endregion

            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            try
            {

                if (RoamingNetwork.SessionsStore.TryGet(SessionId, out ChargingSession chargingSession) &&
                    chargingSession.EVSEId.HasValue)
                {

                    #region Upstream HTTPS request...

                    var requestTask = (RemoteCertificateValidator == null

                                           ? new HTTPClient (RemoteHost:                  Hostname,
                                                             VirtualHostname:             VirtualHostname,
                                                             RemotePort:                  RemotePort,
                                                             DNSClient:                   DNSClient)

                                           : new HTTPSClient(RemoteHost:                  Hostname,
                                                             VirtualHostname:             VirtualHostname,
                                                             RemotePort:                  RemotePort,
                                                             RemoteCertificateValidator:  RemoteCertificateValidator,
                                                             DNSClient:                   DNSClient)).

                                           Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                  HTTPPath.Parse("/ps/rest/hubject/RNs/" + RoamingNetwork.Id + "/EVSEs/" + chargingSession.EVSEId.ToString().Replace("+", "")),

                                                                               requestbuilder => {
                                                                                   requestbuilder.Host         = VirtualHostname ?? Hostname;
                                                                                   requestbuilder.ContentType  = HTTPContentType.JSON_UTF8;
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Content      = JSONObject.Create(

                                                                                                                     new JProperty("@context",          "https://open.charging.cloud/contexts/wwcp/remoteStopRequest"),
                                                                                                                     new JProperty("@id",               SessionId. ToString()),

                                                                                                                     ProviderId.HasValue
                                                                                                                         ? new JProperty("ProviderId",  ProviderId.ToString())
                                                                                                                         : null

                                                                                                                 ).ToString().
                                                                                                                   ToUTF8Bytes();
                                                                               }),

                                                             RequestLogDelegate:   OnRemoteStopHTTPRequest,
                                                             ResponseLogDelegate:  OnRemoteStopHTTPResponse,
                                                             CancellationToken:    CancellationToken,
                                                             EventTrackingId:      EventTrackingId,
                                                             RequestTimeout:       RequestTimeout ?? this.RequestTimeout);

                                         //  ConfigureAwait(false);

                    #endregion

                    var StartTime    = DateTime.UtcNow;
                    var timeout      = TimeSpan.FromSeconds(7);
                    var timeoutTask  = Task.Delay(timeout);
                    var resultTask   = await Task.WhenAny(requestTask, timeoutTask).
                                                  ConfigureAwait(false);

                    RemoteStopResult result;

                    if (resultTask == timeoutTask)
                        result = RemoteStopResult.AsyncOperation(SessionId,
                                                                 Runtime: DateTime.UtcNow - StartTime);

                    else
                    {

                        var httpresult = requestTask.Result;

                        #region HTTPStatusCode.OK

                        if (httpresult.HTTPStatusCode == HTTPStatusCode.OK)
                        {

                            // HTTP/1.1 200 OK
                            // Date: Fri, 28 Mar 2014 13:31:27 GMT
                            // Server: Apache/2.2.9 (Debian) mod_jk/1.2.26
                            // Content-Length: 34
                            // Content-Type: application/json
                            // 
                            // {
                            //   "code" : "EVSE_AlreadyInUse"
                            // }

                            try
                            {

                                var JSONResponse = JObject.Parse(httpresult.HTTPBody?.ToUTF8String());

                                switch (JSONResponse["code"]?.ToString())
                                {

                                    case "SessionId_Unknown":
                                        result = RemoteStopResult.InvalidSessionId(SessionId:       SessionId,
                                                                                   AdditionalInfo:  "Source: " + Hostname.ToString(),
                                                                                   Runtime:         httpresult.Runtime);
                                        break;

                                    case "EVSE_NotReachable":
                                        result = RemoteStopResult.Offline         (SessionId,
                                                                                   Runtime: httpresult.Runtime);
                                        break;

                                    case "Timeout":
                                        result = RemoteStopResult.Timeout         (SessionId,
                                                                                   Runtime: httpresult.Runtime);
                                        break;

                                    case "EVSE_unknown":
                                        result = RemoteStopResult.UnknownLocation (SessionId,
                                                                                   Runtime: httpresult.Runtime);
                                        break;

                                    case "EVSE_is_out_of_service":
                                        result = RemoteStopResult.OutOfService    (SessionId,
                                                                                   Runtime: httpresult.Runtime);
                                        break;

                                    case "Success":
                                        result = RemoteStopResult.Success         (SessionId,
                                                                                   Runtime: httpresult.Runtime);
                                        break;

                                    default:
                                        result = RemoteStopResult.Error           (SessionId,
                                                                                   Runtime: httpresult.Runtime);
                                        break;

                                }

                            }
                            catch (Exception e)
                            {
                                result = RemoteStopResult.Error(SessionId,
                                                                "chargeIT mobility REMOTESTOP response JSON could not be parsed: " +
                                                                e.Message    + Environment.NewLine +
                                                                e.StackTrace + Environment.NewLine +
                                                                httpresult.EntirePDU);
                            }

                        }

                        #endregion

                        else
                            result = RemoteStopResult.Error(SessionId,
                                                            httpresult.HTTPStatusCode.ToString(),
                                                            httpresult.EntirePDU);

                    }

                    return result;

                }

                return RemoteStopResult.InvalidSessionId(SessionId);

            }

            catch (Exception e)
            {
                return RemoteStopResult.Error(SessionId, e.Message);
            }

        }

        #endregion

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
