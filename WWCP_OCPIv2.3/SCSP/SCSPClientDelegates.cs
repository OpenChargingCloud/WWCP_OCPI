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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.SCSP.HTTP
{

    #region OnGetLocationsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get locations request will be send.
    /// </summary>
    public delegate Task OnGetLocationsRequestDelegate(DateTime                              LogTimestamp,
                                                       SCSPClient                            Sender,
                                                       Request_Id                            RequestId,
                                                       Correlation_Id                        CorrelationId,

                                                       CancellationToken?                    CancellationToken,
                                                       EventTracking_Id                      EventTrackingId,
                                                       TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get locations request had been received.
    /// </summary>
    public delegate Task OnGetLocationsResponseDelegate(DateTime                              LogTimestamp,
                                                        SCSPClient                            Sender,
                                                        Request_Id                            RequestId,
                                                        Correlation_Id                        CorrelationId,

                                                        CancellationToken?                    CancellationToken,
                                                        EventTracking_Id                      EventTrackingId,
                                                        TimeSpan                              RequestTimeout,

                                                        OCPIResponse<IEnumerable<Location>>   Response,
                                                        TimeSpan                              Runtime);

    #endregion

    #region OnGetLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a get location by its identification request will be send.
    /// </summary>
    public delegate Task OnGetLocationRequestDelegate(DateTime                 LogTimestamp,
                                                      SCSPClient               Sender,
                                                      Request_Id               RequestId,
                                                      Correlation_Id           CorrelationId,

                                                      Location_Id              LocationId,

                                                      CancellationToken        CancellationToken,
                                                      EventTracking_Id         EventTrackingId,
                                                      TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get location by its identification request had been received.
    /// </summary>
    public delegate Task OnGetLocationResponseDelegate(DateTime                 LogTimestamp,
                                                       SCSPClient               Sender,
                                                       Request_Id               RequestId,
                                                       Correlation_Id           CorrelationId,

                                                       Location_Id              LocationId,

                                                       CancellationToken        CancellationToken,
                                                       EventTracking_Id         EventTrackingId,
                                                       TimeSpan                 RequestTimeout,

                                                       OCPIResponse<Location>   Response,
                                                       TimeSpan                 Runtime);

    #endregion

    #region OnGetEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a get EVSE by its identification request will be send.
    /// </summary>
    public delegate Task OnGetEVSERequestDelegate(DateTime                 LogTimestamp,
                                                  SCSPClient               Sender,
                                                  Request_Id               RequestId,
                                                  Correlation_Id           CorrelationId,

                                                  Location_Id              LocationId,
                                                  EVSE_UId                 EVSEUId,

                                                  CancellationToken        CancellationToken,
                                                  EventTracking_Id         EventTrackingId,
                                                  TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get EVSE by its identification request had been received.
    /// </summary>
    public delegate Task OnGetEVSEResponseDelegate(DateTime                 LogTimestamp,
                                                   SCSPClient               Sender,
                                                   Request_Id               RequestId,
                                                   Correlation_Id           CorrelationId,

                                                   Location_Id              LocationId,
                                                   EVSE_UId                 EVSEUId,

                                                   CancellationToken        CancellationToken,
                                                   EventTracking_Id         EventTrackingId,
                                                   TimeSpan                 RequestTimeout,

                                                   OCPIResponse<EVSE>       Response,
                                                   TimeSpan                 Runtime);

    #endregion

    #region OnGetConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a get connector by its identification request will be send.
    /// </summary>
    public delegate Task OnGetConnectorRequestDelegate(DateTime                  LogTimestamp,
                                                       SCSPClient                Sender,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,

                                                       Location_Id               LocationId,
                                                       EVSE_UId                  EVSEUId,
                                                       Connector_Id              ConnectorId,

                                                       CancellationToken         CancellationToken,
                                                       EventTracking_Id          EventTrackingId,
                                                       TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get connector by its identification request had been received.
    /// </summary>
    public delegate Task OnGetConnectorResponseDelegate(DateTime                  LogTimestamp,
                                                        SCSPClient                Sender,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,

                                                        Location_Id               LocationId,
                                                        EVSE_UId                  EVSEUId,
                                                        Connector_Id              ConnectorId,

                                                        CancellationToken         CancellationToken,
                                                        EventTracking_Id          EventTrackingId,
                                                        TimeSpan                  RequestTimeout,

                                                        OCPIResponse<Connector>   Response,
                                                        TimeSpan                  Runtime);

    #endregion


    #region OnGetSessionsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get sessions request will be send.
    /// </summary>
    public delegate Task OnGetSessionsRequestDelegate(DateTime                              LogTimestamp,
                                                      SCSPClient                            Sender,
                                                      Request_Id                            RequestId,
                                                      Correlation_Id                        CorrelationId,

                                                      CancellationToken?                    CancellationToken,
                                                      EventTracking_Id                      EventTrackingId,
                                                      TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get sessions request had been received.
    /// </summary>
    public delegate Task OnGetSessionsResponseDelegate(DateTime                              LogTimestamp,
                                                       SCSPClient                            Sender,
                                                       Request_Id                            RequestId,
                                                       Correlation_Id                        CorrelationId,

                                                       CancellationToken?                    CancellationToken,
                                                       EventTracking_Id                      EventTrackingId,
                                                       TimeSpan                              RequestTimeout,

                                                       OCPIResponse<IEnumerable<Session>>    Response,
                                                       TimeSpan                              Runtime);

    #endregion

    #region OnGetSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a get session by its identification request will be send.
    /// </summary>
    public delegate Task OnGetSessionRequestDelegate(DateTime                 LogTimestamp,
                                                     SCSPClient               Sender,
                                                     Request_Id               RequestId,
                                                     Correlation_Id           CorrelationId,

                                                     Session_Id               SessionId,

                                                     CancellationToken        CancellationToken,
                                                     EventTracking_Id         EventTrackingId,
                                                     TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get session by its identification request had been received.
    /// </summary>
    public delegate Task OnGetSessionResponseDelegate(DateTime                 LogTimestamp,
                                                      SCSPClient               Sender,
                                                      Request_Id               RequestId,
                                                      Correlation_Id           CorrelationId,

                                                      Session_Id               SessionId,

                                                      CancellationToken        CancellationToken,
                                                      EventTracking_Id         EventTrackingId,
                                                      TimeSpan                 RequestTimeout,

                                                      OCPIResponse<Session>    Response,
                                                      TimeSpan                 Runtime);

    #endregion


    #region OnSetChargingProfileRequest/-Response

    /// <summary>
    /// A delegate called whenever an unlock connector command request will be send.
    /// </summary>
    public delegate Task OnSetChargingProfileRequestDelegate(DateTime                                                    LogTimestamp,
                                                             SCSPClient                                                  Sender,
                                                             Request_Id                                                  RequestId,
                                                             Correlation_Id                                              CorrelationId,

                                                             Location_Id                                                 LocationId,
                                                             EVSE_UId                                                    EVSEUId,
                                                             Connector_Id                                                ConnectorId,

                                                             CancellationToken                                           CancellationToken,
                                                             EventTracking_Id                                            EventTrackingId,
                                                             TimeSpan                                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to an unlock connector command request had been received.
    /// </summary>
    public delegate Task OnSetChargingProfileResponseDelegate(DateTime                                                    LogTimestamp,
                                                              SCSPClient                                                  Sender,
                                                              Request_Id                                                  RequestId,
                                                              Correlation_Id                                              CorrelationId,

                                                              Location_Id                                                 LocationId,
                                                              EVSE_UId                                                    EVSEUId,
                                                              Connector_Id                                                ConnectorId,

                                                              CancellationToken                                           CancellationToken,
                                                              EventTracking_Id                                            EventTrackingId,
                                                              TimeSpan                                                    RequestTimeout,

                                                              OCPIResponse<SetChargingProfileCommand, CommandResponse>    Response,
                                                              TimeSpan                                                    Runtime);

    #endregion

}
