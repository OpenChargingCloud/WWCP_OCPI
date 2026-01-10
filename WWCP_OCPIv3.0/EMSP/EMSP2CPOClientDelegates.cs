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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0.EMSP.HTTP
{

    #region OnGetLocationsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get locations request will be send.
    /// </summary>
    public delegate Task OnGetLocationsRequestDelegate(DateTimeOffset                        LogTimestamp,
                                                       EMSP2CPOClient                        Sender,
                                                       Request_Id                            RequestId,
                                                       Correlation_Id                        CorrelationId,

                                                       CancellationToken?                    CancellationToken,
                                                       EventTracking_Id                      EventTrackingId,
                                                       TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get locations request had been received.
    /// </summary>
    public delegate Task OnGetLocationsResponseDelegate(DateTimeOffset                        LogTimestamp,
                                                        EMSP2CPOClient                        Sender,
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
    public delegate Task OnGetLocationRequestDelegate(DateTimeOffset           LogTimestamp,
                                                      EMSP2CPOClient           Sender,
                                                      Request_Id               RequestId,
                                                      Correlation_Id           CorrelationId,

                                                      Location_Id              LocationId,

                                                      CancellationToken        CancellationToken,
                                                      EventTracking_Id         EventTrackingId,
                                                      TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get location by its identification request had been received.
    /// </summary>
    public delegate Task OnGetLocationResponseDelegate(DateTimeOffset           LogTimestamp,
                                                       EMSP2CPOClient           Sender,
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
    public delegate Task OnGetEVSERequestDelegate(DateTimeOffset           LogTimestamp,
                                                  EMSP2CPOClient           Sender,
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
    public delegate Task OnGetEVSEResponseDelegate(DateTimeOffset           LogTimestamp,
                                                   EMSP2CPOClient           Sender,
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
    public delegate Task OnGetConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       EMSP2CPOClient                Sender,
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
    public delegate Task OnGetConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        EMSP2CPOClient                Sender,
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


    #region OnGetTariffsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tariffs request will be send.
    /// </summary>
    public delegate Task OnGetTariffsRequestDelegate(DateTimeOffset                        LogTimestamp,
                                                     EMSP2CPOClient                        Sender,
                                                     Request_Id                            RequestId,
                                                     Correlation_Id                        CorrelationId,

                                                     CancellationToken?                    CancellationToken,
                                                     EventTracking_Id                      EventTrackingId,
                                                     TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get tariffs request had been received.
    /// </summary>
    public delegate Task OnGetTariffsResponseDelegate(DateTimeOffset                        LogTimestamp,
                                                      EMSP2CPOClient                        Sender,
                                                      Request_Id                            RequestId,
                                                      Correlation_Id                        CorrelationId,

                                                      CancellationToken?                    CancellationToken,
                                                      EventTracking_Id                      EventTrackingId,
                                                      TimeSpan                              RequestTimeout,

                                                      OCPIResponse<IEnumerable<Tariff>>     Response,
                                                      TimeSpan                              Runtime);

    #endregion

    #region OnGetTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tariff by its identification request will be send.
    /// </summary>
    public delegate Task OnGetTariffRequestDelegate(DateTimeOffset           LogTimestamp,
                                                    EMSP2CPOClient           Sender,
                                                    Request_Id               RequestId,
                                                    Correlation_Id           CorrelationId,

                                                    Tariff_Id                TariffId,

                                                    CancellationToken        CancellationToken,
                                                    EventTracking_Id         EventTrackingId,
                                                    TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get tariff by its identification request had been received.
    /// </summary>
    public delegate Task OnGetTariffResponseDelegate(DateTimeOffset           LogTimestamp,
                                                     EMSP2CPOClient           Sender,
                                                     Request_Id               RequestId,
                                                     Correlation_Id           CorrelationId,

                                                     Tariff_Id                TariffId,

                                                     CancellationToken        CancellationToken,
                                                     EventTracking_Id         EventTrackingId,
                                                     TimeSpan                 RequestTimeout,

                                                     OCPIResponse<Tariff>     Response,
                                                     TimeSpan                 Runtime);

    #endregion


    #region OnGetSessionsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get sessions request will be send.
    /// </summary>
    public delegate Task OnGetSessionsRequestDelegate(DateTimeOffset                        LogTimestamp,
                                                      EMSP2CPOClient                        Sender,
                                                      Request_Id                            RequestId,
                                                      Correlation_Id                        CorrelationId,

                                                      CancellationToken?                    CancellationToken,
                                                      EventTracking_Id                      EventTrackingId,
                                                      TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get sessions request had been received.
    /// </summary>
    public delegate Task OnGetSessionsResponseDelegate(DateTimeOffset                        LogTimestamp,
                                                       EMSP2CPOClient                        Sender,
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
    public delegate Task OnGetSessionRequestDelegate(DateTimeOffset           LogTimestamp,
                                                     EMSP2CPOClient           Sender,
                                                     Request_Id               RequestId,
                                                     Correlation_Id           CorrelationId,

                                                     Session_Id               SessionId,

                                                     CancellationToken        CancellationToken,
                                                     EventTracking_Id         EventTrackingId,
                                                     TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get session by its identification request had been received.
    /// </summary>
    public delegate Task OnGetSessionResponseDelegate(DateTimeOffset           LogTimestamp,
                                                      EMSP2CPOClient           Sender,
                                                      Request_Id               RequestId,
                                                      Correlation_Id           CorrelationId,

                                                      Session_Id               SessionId,

                                                      CancellationToken        CancellationToken,
                                                      EventTracking_Id         EventTrackingId,
                                                      TimeSpan                 RequestTimeout,

                                                      OCPIResponse<Session>    Response,
                                                      TimeSpan                 Runtime);

    #endregion


    #region OnGetCDRsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get CDRs request will be send.
    /// </summary>
    public delegate Task OnGetCDRsRequestDelegate(DateTimeOffset                        LogTimestamp,
                                                  EMSP2CPOClient                        Sender,
                                                  Request_Id                            RequestId,
                                                  Correlation_Id                        CorrelationId,

                                                  CancellationToken?                    CancellationToken,
                                                  EventTracking_Id                      EventTrackingId,
                                                  TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get CDRs request had been received.
    /// </summary>
    public delegate Task OnGetCDRsResponseDelegate(DateTimeOffset                        LogTimestamp,
                                                   EMSP2CPOClient                        Sender,
                                                   Request_Id                            RequestId,
                                                   Correlation_Id                        CorrelationId,

                                                   CancellationToken?                    CancellationToken,
                                                   EventTracking_Id                      EventTrackingId,
                                                   TimeSpan                              RequestTimeout,

                                                   OCPIResponse<IEnumerable<CDR>>        Response,
                                                   TimeSpan                              Runtime);

    #endregion

    #region OnGetCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a get CDR by its identification request will be send.
    /// </summary>
    public delegate Task OnGetCDRRequestDelegate(DateTimeOffset           LogTimestamp,
                                                 EMSP2CPOClient           Sender,
                                                 Request_Id               RequestId,
                                                 Correlation_Id           CorrelationId,

                                                 CDR_Id                   CDRId,

                                                 CancellationToken        CancellationToken,
                                                 EventTracking_Id         EventTrackingId,
                                                 TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get CDR by its identification request had been received.
    /// </summary>
    public delegate Task OnGetCDRResponseDelegate(DateTimeOffset           LogTimestamp,
                                                  EMSP2CPOClient           Sender,
                                                  Request_Id               RequestId,
                                                  Correlation_Id           CorrelationId,

                                                  CDR_Id                   CDRId,

                                                  CancellationToken        CancellationToken,
                                                  EventTracking_Id         EventTrackingId,
                                                  TimeSpan                 RequestTimeout,

                                                  OCPIResponse<CDR>        Response,
                                                  TimeSpan                 Runtime);

    #endregion


    #region OnGetTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a get token request will be send.
    /// </summary>
    public delegate Task OnGetTokenRequestDelegate(DateTimeOffset           LogTimestamp,
                                                   EMSP2CPOClient           Sender,
                                                   Request_Id               RequestId,
                                                   Correlation_Id           CorrelationId,

                                                   CountryCode              CountryCode,
                                                   Party_Idv3                 PartyId,
                                                   Token_Id                 TokenId,

                                                   CancellationToken        CancellationToken,
                                                   EventTracking_Id         EventTrackingId,
                                                   TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get token request had been received.
    /// </summary>
    public delegate Task OnGetTokenResponseDelegate(DateTimeOffset           LogTimestamp,
                                                    EMSP2CPOClient           Sender,
                                                    Request_Id               RequestId,
                                                    Correlation_Id           CorrelationId,

                                                    CountryCode              CountryCode,
                                                    Party_Idv3                 PartyId,
                                                    Token_Id                 TokenId,

                                                    CancellationToken        CancellationToken,
                                                    EventTracking_Id         EventTrackingId,
                                                    TimeSpan                 RequestTimeout,

                                                    OCPIResponse<Token>      Response,
                                                    TimeSpan                 Runtime);

    #endregion

    #region OnPutTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a put token request will be send.
    /// </summary>
    public delegate Task OnPutTokenRequestDelegate(DateTimeOffset           LogTimestamp,
                                                   EMSP2CPOClient           Sender,
                                                   Request_Id               RequestId,
                                                   Correlation_Id           CorrelationId,

                                                   Token                    Token,

                                                   CancellationToken        CancellationToken,
                                                   EventTracking_Id         EventTrackingId,
                                                   TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put token request had been received.
    /// </summary>
    public delegate Task OnPutTokenResponseDelegate(DateTimeOffset           LogTimestamp,
                                                    EMSP2CPOClient           Sender,
                                                    Request_Id               RequestId,
                                                    Correlation_Id           CorrelationId,

                                                    Token                    Token,

                                                    CancellationToken        CancellationToken,
                                                    EventTracking_Id         EventTrackingId,
                                                    TimeSpan                 RequestTimeout,

                                                    OCPIResponse<Token>      Response,
                                                    TimeSpan                 Runtime);

    #endregion

    #region OnPatchTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch token request will be send.
    /// </summary>
    public delegate Task OnPatchTokenRequestDelegate(DateTimeOffset            LogTimestamp,
                                                     EMSP2CPOClient                Sender,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,

                                                     CountryCode               CountryCode,
                                                     Party_Idv3                  PartyId,
                                                     Token_Id                  TokenId,
                                                     JObject                   TokenPatch,

                                                     CancellationToken         CancellationToken,
                                                     EventTracking_Id          EventTrackingId,
                                                     TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a patch token request had been received.
    /// </summary>
    public delegate Task OnPatchTokenResponseDelegate(DateTimeOffset            LogTimestamp,
                                                      EMSP2CPOClient                Sender,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,

                                                      CountryCode               CountryCode,
                                                      Party_Idv3                  PartyId,
                                                      Token_Id                  TokenId,
                                                      JObject                   TokenPatch,

                                                      CancellationToken         CancellationToken,
                                                      EventTracking_Id          EventTrackingId,
                                                      TimeSpan                  RequestTimeout,

                                                      OCPIResponse<Token>       Response,
                                                      TimeSpan                  Runtime);

    #endregion


    #region OnReserveNowRequest/-Response

    /// <summary>
    /// A delegate called whenever a reserve now command request will be send.
    /// </summary>
    public delegate Task OnReserveNowRequestDelegate(DateTimeOffset                                      LogTimestamp,
                                                     EMSP2CPOClient                                          Sender,
                                                     Request_Id                                          RequestId,
                                                     Correlation_Id                                      CorrelationId,

                                                     Token                                               Token,
                                                     DateTimeOffset                                      ExpirationTimestamp,
                                                     Reservation_Id                                      ReservationId,
                                                     Location_Id                                         LocationId,
                                                     EVSE_UId?                                           EVSEUId,
                                                     AuthorizationReference?                             AuthorizationReference,

                                                     CancellationToken                                   CancellationToken,
                                                     EventTracking_Id                                    EventTrackingId,
                                                     TimeSpan                                            RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a reserve now command request had been received.
    /// </summary>
    public delegate Task OnReserveNowResponseDelegate(DateTimeOffset                                      LogTimestamp,
                                                      EMSP2CPOClient                                          Sender,
                                                      Request_Id                                          RequestId,
                                                      Correlation_Id                                      CorrelationId,

                                                      Token                                               Token,
                                                      DateTimeOffset                                      ExpirationTimestamp,
                                                      Reservation_Id                                      ReservationId,
                                                      Location_Id                                         LocationId,
                                                      EVSE_UId?                                           EVSEUId,
                                                      AuthorizationReference?                             AuthorizationReference,

                                                      CancellationToken                                   CancellationToken,
                                                      EventTracking_Id                                    EventTrackingId,
                                                      TimeSpan                                            RequestTimeout,

                                                      OCPIResponse<ReserveNowCommand, CommandResponse>    Response,
                                                      TimeSpan                                            Runtime);

    #endregion

    #region OnCancelReservationRequest/-Response

    /// <summary>
    /// A delegate called whenever a cancel reservation command request will be send.
    /// </summary>
    public delegate Task OnCancelReservationRequestDelegate(DateTimeOffset                                             LogTimestamp,
                                                            EMSP2CPOClient                                                 Sender,
                                                            Request_Id                                                 RequestId,
                                                            Correlation_Id                                             CorrelationId,

                                                            Reservation_Id                                             ReservationId,

                                                            CancellationToken?                                         CancellationToken,
                                                            EventTracking_Id                                           EventTrackingId,
                                                            TimeSpan                                                   RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a cancel reservation command request had been received.
    /// </summary>
    public delegate Task OnCancelReservationResponseDelegate(DateTimeOffset                                             LogTimestamp,
                                                             EMSP2CPOClient                                                 Sender,
                                                             Request_Id                                                 RequestId,
                                                             Correlation_Id                                             CorrelationId,

                                                             Reservation_Id                                             ReservationId,

                                                             CancellationToken?                                         CancellationToken,
                                                             EventTracking_Id                                           EventTrackingId,
                                                             TimeSpan                                                   RequestTimeout,

                                                             OCPIResponse<CancelReservationCommand, CommandResponse>    Response,
                                                             TimeSpan                                                   Runtime);

    #endregion

    #region OnStartSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a start session command request will be send.
    /// </summary>
    public delegate Task OnStartSessionRequestDelegate(DateTimeOffset                                        LogTimestamp,
                                                       EMSP2CPOClient                                            Sender,
                                                       Request_Id                                            RequestId,
                                                       Correlation_Id                                        CorrelationId,

                                                       Token                                                 Token,
                                                       Location_Id                                           LocationId,
                                                       EVSE_UId?                                             EVSEUId,
                                                       Connector_Id?                                         ConnectorId,
                                                       AuthorizationReference?                               AuthorizationReference,

                                                       CancellationToken?                                    CancellationToken,
                                                       EventTracking_Id                                      EventTrackingId,
                                                       TimeSpan                                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a start session command request had been received.
    /// </summary>
    public delegate Task OnStartSessionResponseDelegate(DateTimeOffset                                        LogTimestamp,
                                                        EMSP2CPOClient                                            Sender,
                                                        Request_Id                                            RequestId,
                                                        Correlation_Id                                        CorrelationId,

                                                        Token                                                 Token,
                                                        Location_Id                                           LocationId,
                                                        EVSE_UId?                                             EVSEUId,
                                                        Connector_Id?                                         ConnectorId,
                                                        AuthorizationReference?                               AuthorizationReference,

                                                        CancellationToken?                                    CancellationToken,
                                                        EventTracking_Id                                      EventTrackingId,
                                                        TimeSpan                                              RequestTimeout,

                                                        OCPIResponse<StartSessionCommand, CommandResponse>    Response,
                                                        TimeSpan                                              Runtime);

    #endregion

    #region OnStopSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a stop session command request will be send.
    /// </summary>
    public delegate Task OnStopSessionRequestDelegate(DateTimeOffset                                       LogTimestamp,
                                                      EMSP2CPOClient                                           Sender,
                                                      Request_Id                                           RequestId,
                                                      Correlation_Id                                       CorrelationId,

                                                      Session_Id                                           SessionId,

                                                      CancellationToken?                                   CancellationToken,
                                                      EventTracking_Id                                     EventTrackingId,
                                                      TimeSpan                                             RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a stop session command request had been received.
    /// </summary>
    public delegate Task OnStopSessionResponseDelegate(DateTimeOffset                                       LogTimestamp,
                                                       EMSP2CPOClient                                           Sender,
                                                       Request_Id                                           RequestId,
                                                       Correlation_Id                                       CorrelationId,

                                                       Session_Id                                           SessionId,

                                                       CancellationToken?                                   CancellationToken,
                                                       EventTracking_Id                                     EventTrackingId,
                                                       TimeSpan                                             RequestTimeout,

                                                       OCPIResponse<StopSessionCommand, CommandResponse>    Response,
                                                       TimeSpan                                             Runtime);

    #endregion

    #region OnUnlockConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever an unlock connector command request will be send.
    /// </summary>
    public delegate Task OnUnlockConnectorRequestDelegate(DateTimeOffset                                           LogTimestamp,
                                                          EMSP2CPOClient                                               Sender,
                                                          Request_Id                                               RequestId,
                                                          Correlation_Id                                           CorrelationId,

                                                          Location_Id                                              LocationId,
                                                          EVSE_UId                                                 EVSEUId,
                                                          Connector_Id                                             ConnectorId,

                                                          CancellationToken?                                       CancellationToken,
                                                          EventTracking_Id                                         EventTrackingId,
                                                          TimeSpan                                                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to an unlock connector command request had been received.
    /// </summary>
    public delegate Task OnUnlockConnectorResponseDelegate(DateTimeOffset                                           LogTimestamp,
                                                           EMSP2CPOClient                                               Sender,
                                                           Request_Id                                               RequestId,
                                                           Correlation_Id                                           CorrelationId,

                                                           Location_Id                                              LocationId,
                                                           EVSE_UId                                                 EVSEUId,
                                                           Connector_Id                                             ConnectorId,

                                                           CancellationToken?                                       CancellationToken,
                                                           EventTracking_Id                                         EventTrackingId,
                                                           TimeSpan                                                 RequestTimeout,

                                                           OCPIResponse<UnlockConnectorCommand, CommandResponse>    Response,
                                                           TimeSpan                                                 Runtime);

    #endregion

    #region OnSetChargingProfileRequest/-Response

    /// <summary>
    /// A delegate called whenever an unlock connector command request will be send.
    /// </summary>
    public delegate Task OnSetChargingProfileRequestDelegate(DateTimeOffset                                              LogTimestamp,
                                                             EMSP2CPOClient                                                  Sender,
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
    public delegate Task OnSetChargingProfileResponseDelegate(DateTimeOffset                                              LogTimestamp,
                                                              EMSP2CPOClient                                                  Sender,
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
