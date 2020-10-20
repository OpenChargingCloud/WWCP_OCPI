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
    /// The EMSP client.
    /// </summary>
    public class EMSPClient : CommonClient
    {

        #region Data

        #endregion

        #region Properties

        #endregion

        #region Events

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
        public EMSPClient(AccessToken                          AccessToken,
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

                var StartTime      = DateTime.UtcNow;
                var RequestId      = Request_Id.Random();
                var CorrelationId  = Correlation_Id.Random();

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
                                                                               VersionsURL.Path + "cpo/2.2/locations",

                                                                               requestbuilder => {
                                                                                   requestbuilder.Host           = VirtualHostname ?? Hostname;
                                                                                   requestbuilder.Authorization  = new HTTPTokenAuthentication(AccessToken.ToString());
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                               }),

                                              //RequestLogDelegate:   OnRemoteStartHTTPRequest,
                                              //ResponseLogDelegate:  OnRemoteStartHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Location>.ParseJArray(HTTPResponse,
                                                              RequestId,
                                                              CorrelationId,
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






        #region Dispose()

        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        { }

        #endregion

    }

}
