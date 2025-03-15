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
    public delegate Task OnGetLocationsRequestDelegate(DateTime                              LogTimestamp,
                                                       EMSP2CPOClient                            Sender,
                                                       Request_Id                            RequestId,
                                                       Correlation_Id                        CorrelationId,

                                                       CancellationToken?                    CancellationToken,
                                                       EventTracking_Id                      EventTrackingId,
                                                       TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get locations request had been received.
    /// </summary>
    public delegate Task OnGetLocationsResponseDelegate(DateTime                              LogTimestamp,
                                                        EMSP2CPOClient                            Sender,
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
                                                      EMSP2CPOClient               Sender,
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
                                                       EMSP2CPOClient               Sender,
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
                                                  EMSP2CPOClient               Sender,
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
                                                   EMSP2CPOClient               Sender,
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
    public delegate Task OnGetConnectorResponseDelegate(DateTime                  LogTimestamp,
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
    public delegate Task OnGetTariffsRequestDelegate(DateTime                              LogTimestamp,
                                                     EMSP2CPOClient                            Sender,
                                                     Request_Id                            RequestId,
                                                     Correlation_Id                        CorrelationId,

                                                     CancellationToken?                    CancellationToken,
                                                     EventTracking_Id                      EventTrackingId,
                                                     TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get tariffs request had been received.
    /// </summary>
    public delegate Task OnGetTariffsResponseDelegate(DateTime                              LogTimestamp,
                                                      EMSP2CPOClient                            Sender,
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
    public delegate Task OnGetTariffRequestDelegate(DateTime                 LogTimestamp,
                                                    EMSP2CPOClient               Sender,
                                                    Request_Id               RequestId,
                                                    Correlation_Id           CorrelationId,

                                                    Tariff_Id                TariffId,

                                                    CancellationToken        CancellationToken,
                                                    EventTracking_Id         EventTrackingId,
                                                    TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get tariff by its identification request had been received.
    /// </summary>
    public delegate Task OnGetTariffResponseDelegate(DateTime                 LogTimestamp,
                                                     EMSP2CPOClient               Sender,
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
    public delegate Task OnGetSessionsRequestDelegate(DateTime                              LogTimestamp,
                                                      EMSP2CPOClient                            Sender,
                                                      Request_Id                            RequestId,
                                                      Correlation_Id                        CorrelationId,

                                                      CancellationToken?                    CancellationToken,
                                                      EventTracking_Id                      EventTrackingId,
                                                      TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get sessions request had been received.
    /// </summary>
    public delegate Task OnGetSessionsResponseDelegate(DateTime                              LogTimestamp,
                                                       EMSP2CPOClient                            Sender,
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
                                                     EMSP2CPOClient               Sender,
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
                                                      EMSP2CPOClient               Sender,
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
    public delegate Task OnGetCDRsRequestDelegate(DateTime                              LogTimestamp,
                                                  EMSP2CPOClient                            Sender,
                                                  Request_Id                            RequestId,
                                                  Correlation_Id                        CorrelationId,

                                                  CancellationToken?                    CancellationToken,
                                                  EventTracking_Id                      EventTrackingId,
                                                  TimeSpan                              RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get CDRs request had been received.
    /// </summary>
    public delegate Task OnGetCDRsResponseDelegate(DateTime                              LogTimestamp,
                                                   EMSP2CPOClient                            Sender,
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
    public delegate Task OnGetCDRRequestDelegate(DateTime                 LogTimestamp,
                                                 EMSP2CPOClient               Sender,
                                                 Request_Id               RequestId,
                                                 Correlation_Id           CorrelationId,

                                                 CDR_Id                   CDRId,

                                                 CancellationToken        CancellationToken,
                                                 EventTracking_Id         EventTrackingId,
                                                 TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get CDR by its identification request had been received.
    /// </summary>
    public delegate Task OnGetCDRResponseDelegate(DateTime                 LogTimestamp,
                                                  EMSP2CPOClient               Sender,
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
    public delegate Task OnGetTokenRequestDelegate(DateTime                 LogTimestamp,
                                                   EMSP2CPOClient               Sender,
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
    public delegate Task OnGetTokenResponseDelegate(DateTime                 LogTimestamp,
                                                    EMSP2CPOClient               Sender,
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
    public delegate Task OnPutTokenRequestDelegate(DateTime                 LogTimestamp,
                                                   EMSP2CPOClient               Sender,
                                                   Request_Id               RequestId,
                                                   Correlation_Id           CorrelationId,

                                                   Token                    Token,

                                                   CancellationToken        CancellationToken,
                                                   EventTracking_Id         EventTrackingId,
                                                   TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put token request had been received.
    /// </summary>
    public delegate Task OnPutTokenResponseDelegate(DateTime                 LogTimestamp,
                                                    EMSP2CPOClient               Sender,
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
    public delegate Task OnPatchTokenRequestDelegate(DateTime                  LogTimestamp,
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
    public delegate Task OnPatchTokenResponseDelegate(DateTime                  LogTimestamp,
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
    public delegate Task OnReserveNowRequestDelegate(DateTime                                            LogTimestamp,
                                                     EMSP2CPOClient                                          Sender,
                                                     Request_Id                                          RequestId,
                                                     Correlation_Id                                      CorrelationId,

                                                     Token                                               Token,
                                                     DateTime                                            ExpirationTimestamp,
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
    public delegate Task OnReserveNowResponseDelegate(DateTime                                            LogTimestamp,
                                                      EMSP2CPOClient                                          Sender,
                                                      Request_Id                                          RequestId,
                                                      Correlation_Id                                      CorrelationId,

                                                      Token                                               Token,
                                                      DateTime                                            ExpirationTimestamp,
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
    public delegate Task OnCancelReservationRequestDelegate(DateTime                                                   LogTimestamp,
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
    public delegate Task OnCancelReservationResponseDelegate(DateTime                                                   LogTimestamp,
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
    public delegate Task OnStartSessionRequestDelegate(DateTime                                              LogTimestamp,
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
    public delegate Task OnStartSessionResponseDelegate(DateTime                                              LogTimestamp,
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
    public delegate Task OnStopSessionRequestDelegate(DateTime                                             LogTimestamp,
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
    public delegate Task OnStopSessionResponseDelegate(DateTime                                             LogTimestamp,
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
    public delegate Task OnUnlockConnectorRequestDelegate(DateTime                                                 LogTimestamp,
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
    public delegate Task OnUnlockConnectorResponseDelegate(DateTime                                                 LogTimestamp,
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
    public delegate Task OnSetChargingProfileRequestDelegate(DateTime                                                    LogTimestamp,
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
    public delegate Task OnSetChargingProfileResponseDelegate(DateTime                                                    LogTimestamp,
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
