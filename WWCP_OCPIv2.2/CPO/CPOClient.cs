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
    /// The CPO client.
    /// </summary>
    public class CPOClient : IHTTPClient
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


        private HTTPTokenAuthentication TokenAuth;


        private readonly Dictionary<Version_Id, URL>           Versions        = new Dictionary<Version_Id, URL>();

        private readonly Dictionary<Version_Id, VersionDetail> VersionDetails  = new Dictionary<Version_Id, VersionDetail>();


        #endregion

        #region Properties

        /// <summary>
        /// The access token.
        /// (Might be updated during the REGISTRATION process!)
        /// </summary>
        public AccessToken                          AccessToken                   { get; private set; }

        /// <summary>
        /// The common HTTP URL.
        /// </summary>
        public URL                                  URL                           { get; }

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
        public CPOClient(//RoamingNetwork                       RoamingNetwork,
                          AccessToken                          AccessToken,
                          URL                                  URL,
                          HTTPHostname?                        VirtualHostname              = null,
                          IPPort?                              RemotePort                   = null,
                          RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                          TimeSpan?                            RequestTimeout               = null,
                          DNSClient                            DNSClient                    = null)
        {

         //   this.RoamingNetwork              = RoamingNetwork             ?? throw new ArgumentNullException(nameof(RoamingNetwork), "The given roaming network must not be null!");
            this.AccessToken                 = AccessToken;
            this.TokenAuth                   = new HTTPTokenAuthentication(AccessToken.ToString().ToBase64());    //"Ms9_ZcT/s&Da}MY6W4v]u%{L4&Rzoh6GiZRo<hu+QTc+#7^SUyXG?ujGw3H_?F5U")
            this.URL                         = URL;
            this.Hostname                    = URL.Hostname;
            this.RemotePort                  = URL.Port                   ?? DefaultRemotePort;
            this.VirtualHostname             = VirtualHostname;
            this.RemoteCertificateValidator  = RemoteCertificateValidator;// ?? ((sender, certificate, chain, policyErrors) => true); // Otherwise forcing HTTP will not work!
            this.RequestTimeout              = RequestTimeout             ?? DefaultRequestTimeout;
            this.DNSClient                   = DNSClient;

        }

        #endregion


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

                var filteredVersions    = getVersion_HTTPResponse.Data.Where(version => version.VersionId.ToString() == "2.2").ToArray();

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

        /// <summary>
        /// Post the given credentials.
        /// </summary>
        /// <param name="Credentials"></param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<RemoteStartResult>

            PostCredentials(Credentials         Credentials,

                            DateTime?           Timestamp           = null,
                            CancellationToken?  CancellationToken   = null,
                            EventTracking_Id    EventTrackingId     = null,
                            TimeSpan?           RequestTimeout      = null)
        {

            #region Initial checks

            if (Credentials is null)
                throw new ArgumentNullException(nameof(Credentials), "The given credentials must not be null!");

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
                                                                                   requestbuilder.Content        = Credentials.ToJSON().
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

                var StartTime      = DateTime.UtcNow;
                var RequestId      = Request_Id.Random();
                var CorrelationId  = Correlation_Id.Random();

                // ToDo: Add request logging!


                #region Upstream HTTP request...

                var HTTPResponse = await (URL.Protocol == HTTPProtocols.http

                                             ? new HTTPClient (URL.Hostname,
                                                               RemotePort:  RemotePort,
                                                               DNSClient:   DNSClient)

                                             : new HTTPSClient(URL.Hostname,
                                                               RemoteCertificateValidator ?? ((a,b,c,d) => true),
                                                               RemotePort:  RemotePort,
                                                               DNSClient:   DNSClient)).

                                       Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                              URL.Path,
                                                                              requestbuilder => {
                                                                                  requestbuilder.Host           = VirtualHostname ?? Hostname;
                                                                                  requestbuilder.Authorization  = TokenAuth;
                                                                                  requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                  requestbuilder.Set("X-Request-ID",      RequestId);
                                                                                  requestbuilder.Set("X-Correlation-ID",  CorrelationId);
                                                                              }),

                                             RequestLogDelegate:   OnRemoteStartHTTPRequest,
                                             ResponseLogDelegate:  OnRemoteStartHTTPResponse,
                                             CancellationToken:    CancellationToken,
                                             EventTrackingId:      EventTrackingId,
                                             RequestTimeout:       RequestTimeout ?? this.RequestTimeout);

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
                                                             RequestId,
                                                             CorrelationId,
                                                             json => Version.Parse(json));

                if (response.StatusCode == 1000)
                {
                    lock (Versions)
                    {
                        Versions.Clear();
                        response.Data.ForEach(version => Versions.Add(version.VersionId, version.URL));
                    }
                }

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
        public async Task<OCPIResponse<Version_Id, VersionDetail>>

            GetVersionDetails(Version_Id          VersionId,

                              DateTime?           Timestamp           = null,
                              CancellationToken?  CancellationToken   = null,
                              EventTracking_Id    EventTrackingId     = null,
                              TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Version_Id, VersionDetail> response;

            try
            {

                var StartTime      = DateTime.UtcNow;
                var RequestId      = Request_Id.Random();
                var CorrelationId  = Correlation_Id.Random();

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
                                                                                      requestbuilder.Host           = Versions[VersionId].Hostname;
                                                                                      requestbuilder.Authorization  = TokenAuth;
                                                                                      requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                      requestbuilder.Set("X-Request-ID",      RequestId);
                                                                                      requestbuilder.Set("X-Correlation-ID",  CorrelationId);
                                                                                  }),

                                                 RequestLogDelegate:   OnRemoteStartHTTPRequest,
                                                 ResponseLogDelegate:  OnRemoteStartHTTPResponse,
                                                 CancellationToken:    CancellationToken,
                                                 EventTrackingId:      EventTrackingId,
                                                 RequestTimeout:       RequestTimeout ?? this.RequestTimeout);

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

                response = OCPIResponse<Version_Id, VersionDetail>.ParseJObject(VersionId,
                                                                                HTTPResponse,
                                                                                json => VersionDetail.Parse(json, null));

                if (response.StatusCode == 1000)
                {
                    lock (VersionDetails)
                    {

                        if (VersionDetails.ContainsKey(VersionId))
                            VersionDetails.Remove(VersionId);

                        VersionDetails.Add(VersionId, response.Data);

                    }
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
