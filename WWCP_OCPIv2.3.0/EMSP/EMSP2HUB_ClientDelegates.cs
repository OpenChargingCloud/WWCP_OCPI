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

namespace cloud.charging.open.protocols.OCPIv2_3_0.EMSP.HUB.HTTP
{

    #region OnGetLocationsRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetLocations request will be send.
    /// </summary>
    public delegate Task OnGetLocationsRequestDelegate(DateTimeOffset                        LogTimestamp,
                                                       EMSP2HUB_HTTPClient                   Sender,
                                                       EventTracking_Id                      EventTrackingId,
                                                       RemoteParty_Id                        RemotePartyId,
                                                       Party_Idv3?                           From,
                                                       Party_Idv3?                           To,
                                                       Request_Id                            RequestId,
                                                       Correlation_Id                        CorrelationId,
                                                       TimeSpan                              RequestTimeout,

                                                       CancellationToken?                    CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetLocations request had been received.
    /// </summary>
    public delegate Task OnGetLocationsResponseDelegate(DateTimeOffset                       LogTimestamp,
                                                        EMSP2HUB_HTTPClient                  Sender,
                                                        EventTracking_Id                     EventTrackingId,
                                                        RemoteParty_Id                       RemotePartyId,
                                                        Party_Idv3?                          From,
                                                        Party_Idv3?                          To,
                                                        Request_Id                           RequestId,
                                                        Correlation_Id                       CorrelationId,
                                                        TimeSpan                             RequestTimeout,

                                                        OCPIResponse<IEnumerable<Location>>  Response,
                                                        TimeSpan                             Runtime,
                                                        CancellationToken                    CancellationToken);

    #endregion

    #region OnGetLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetLocation by its identification request will be send.
    /// </summary>
    public delegate Task OnGetLocationRequestDelegate(DateTimeOffset           LogTimestamp,
                                                      EMSP2HUB_HTTPClient      Sender,
                                                      EventTracking_Id         EventTrackingId,
                                                      RemoteParty_Id           RemotePartyId,
                                                      Party_Idv3?              From,
                                                      Party_Idv3?              To,
                                                      Request_Id               RequestId,
                                                      Correlation_Id           CorrelationId,
                                                      TimeSpan                 RequestTimeout,

                                                      Location_Id              LocationId,

                                                      CancellationToken        CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetLocation by its identification request had been received.
    /// </summary>
    public delegate Task OnGetLocationResponseDelegate(DateTimeOffset          LogTimestamp,
                                                       EMSP2HUB_HTTPClient     Sender,
                                                       EventTracking_Id        EventTrackingId,
                                                       RemoteParty_Id          RemotePartyId,
                                                       Party_Idv3?             From,
                                                       Party_Idv3?             To,
                                                       Request_Id              RequestId,
                                                       Correlation_Id          CorrelationId,
                                                       TimeSpan                RequestTimeout,

                                                       Location_Id             LocationId,

                                                       OCPIResponse<Location>  Response,
                                                       TimeSpan                Runtime,
                                                       CancellationToken       CancellationToken);

    #endregion

    #region OnGetEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a GetEVSE request will be send.
    /// </summary>
    public delegate Task OnGetEVSERequestDelegate(DateTimeOffset        LogTimestamp,
                                                  EMSP2HUB_HTTPClient   Sender,
                                                  EventTracking_Id      EventTrackingId,
                                                  RemoteParty_Id        RemotePartyId,
                                                  Party_Idv3?           From,
                                                  Party_Idv3?           To,
                                                  Request_Id            RequestId,
                                                  Correlation_Id        CorrelationId,
                                                  TimeSpan              RequestTimeout,

                                                  Location_Id           LocationId,
                                                  EVSE_UId              EVSEUId,

                                                  CancellationToken     CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetEVSE request had been received.
    /// </summary>
    public delegate Task OnGetEVSEResponseDelegate(DateTimeOffset       LogTimestamp,
                                                   EMSP2HUB_HTTPClient  Sender,
                                                   EventTracking_Id     EventTrackingId,
                                                   RemoteParty_Id       RemotePartyId,
                                                   Party_Idv3?          From,
                                                   Party_Idv3?          To,
                                                   Request_Id           RequestId,
                                                   Correlation_Id       CorrelationId,
                                                   TimeSpan             RequestTimeout,

                                                   Location_Id          LocationId,
                                                   EVSE_UId             EVSEUId,

                                                   OCPIResponse<EVSE>   Response,
                                                   TimeSpan             Runtime,
                                                   CancellationToken    CancellationToken);

    #endregion

    #region OnGetConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetConnector request will be send.
    /// </summary>
    public delegate Task OnGetConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       EMSP2HUB_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       RemoteParty_Id            RemotePartyId,
                                                       Party_Idv3?               From,
                                                       Party_Idv3?               To,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       Location_Id               LocationId,
                                                       EVSE_UId                  EVSEUId,
                                                       Connector_Id              ConnectorId,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetConnector request had been received.
    /// </summary>
    public delegate Task OnGetConnectorResponseDelegate(DateTimeOffset           LogTimestamp,
                                                        EMSP2HUB_HTTPClient      Sender,
                                                        EventTracking_Id         EventTrackingId,
                                                        RemoteParty_Id           RemotePartyId,
                                                        Party_Idv3?              From,
                                                        Party_Idv3?              To,
                                                        Request_Id               RequestId,
                                                        Correlation_Id           CorrelationId,
                                                        TimeSpan                 RequestTimeout,

                                                        Location_Id              LocationId,
                                                        EVSE_UId                 EVSEUId,
                                                        Connector_Id             ConnectorId,

                                                        OCPIResponse<Connector>  Response,
                                                        TimeSpan                 Runtime,
                                                        CancellationToken        CancellationToken);

    #endregion


    #region OnGetTariffsRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetTariffs request will be send.
    /// </summary>
    public delegate Task OnGetTariffsRequestDelegate(DateTimeOffset                      LogTimestamp,
                                                     EMSP2HUB_HTTPClient                 Sender,
                                                     EventTracking_Id                    EventTrackingId,
                                                     RemoteParty_Id                      RemotePartyId,
                                                     Party_Idv3?                         From,
                                                     Party_Idv3?                         To,
                                                     Request_Id                          RequestId,
                                                     Correlation_Id                      CorrelationId,
                                                     TimeSpan                            RequestTimeout,

                                                     DateTimeOffset?                     FromTimestamp,
                                                     DateTimeOffset?                     ToTimestamp,
                                                     UInt64?                             Offset,
                                                     UInt64?                             Limit,

                                                     CancellationToken?                  CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetTariffs request had been received.
    /// </summary>
    public delegate Task OnGetTariffsResponseDelegate(DateTimeOffset                     LogTimestamp,
                                                      EMSP2HUB_HTTPClient                Sender,
                                                      EventTracking_Id                   EventTrackingId,
                                                      RemoteParty_Id                     RemotePartyId,
                                                      Party_Idv3?                        From,
                                                      Party_Idv3?                        To,
                                                      Request_Id                         RequestId,
                                                      Correlation_Id                     CorrelationId,
                                                      TimeSpan                           RequestTimeout,

                                                      DateTimeOffset?                    FromTimestamp,
                                                      DateTimeOffset?                    ToTimestamp,
                                                      UInt64?                            Offset,
                                                      UInt64?                            Limit,

                                                      OCPIResponse<IEnumerable<Tariff>>  Response,
                                                      TimeSpan                           Runtime,
                                                      CancellationToken?                 CancellationToken);

    #endregion

    #region OnGetTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetTariff request will be send.
    /// </summary>
    public delegate Task OnGetTariffRequestDelegate(DateTimeOffset         LogTimestamp,
                                                    EMSP2HUB_HTTPClient    Sender,
                                                    EventTracking_Id       EventTrackingId,
                                                    RemoteParty_Id         RemotePartyId,
                                                    Party_Idv3?            From,
                                                    Party_Idv3?            To,
                                                    Request_Id             RequestId,
                                                    Correlation_Id         CorrelationId,
                                                    TimeSpan               RequestTimeout,

                                                    Tariff_Id              TariffId,
                                                    DateTimeOffset?        TariffTimestamp,
                                                    TimeSpan?              Tolerance,

                                                    CancellationToken      CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetTariff request had been received.
    /// </summary>
    public delegate Task OnGetTariffResponseDelegate(DateTimeOffset        LogTimestamp,
                                                     EMSP2HUB_HTTPClient   Sender,
                                                     EventTracking_Id      EventTrackingId,
                                                     RemoteParty_Id        RemotePartyId,
                                                     Party_Idv3?           From,
                                                     Party_Idv3?           To,
                                                     Request_Id            RequestId,
                                                     Correlation_Id        CorrelationId,
                                                     TimeSpan              RequestTimeout,

                                                     Tariff_Id             TariffId,
                                                     DateTimeOffset?       TariffTimestamp,
                                                     TimeSpan?             Tolerance,

                                                     OCPIResponse<Tariff>  Response,
                                                     TimeSpan              Runtime,
                                                     CancellationToken     CancellationToken);

    #endregion


    #region OnGetSessionsRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetSessions request will be send.
    /// </summary>
    public delegate Task OnGetSessionsRequestDelegate(DateTimeOffset                       LogTimestamp,
                                                      EMSP2HUB_HTTPClient                  Sender,
                                                      EventTracking_Id                     EventTrackingId,
                                                      RemoteParty_Id                       RemotePartyId,
                                                      Party_Idv3?                          From,
                                                      Party_Idv3?                          To,
                                                      Request_Id                           RequestId,
                                                      Correlation_Id                       CorrelationId,
                                                      TimeSpan                             RequestTimeout,

                                                      CancellationToken?                   CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetSessions request had been received.
    /// </summary>
    public delegate Task OnGetSessionsResponseDelegate(DateTimeOffset                      LogTimestamp,
                                                       EMSP2HUB_HTTPClient                 Sender,
                                                       EventTracking_Id                    EventTrackingId,
                                                       RemoteParty_Id                      RemotePartyId,
                                                       Party_Idv3?                         From,
                                                       Party_Idv3?                         To,
                                                       Request_Id                          RequestId,
                                                       Correlation_Id                      CorrelationId,
                                                       TimeSpan                            RequestTimeout,

                                                       OCPIResponse<IEnumerable<Session>>  Response,
                                                       TimeSpan                            Runtime,
                                                       CancellationToken?                  CancellationToken);

    #endregion

    #region OnGetSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetSession request will be send.
    /// </summary>
    public delegate Task OnGetSessionRequestDelegate(DateTimeOffset          LogTimestamp,
                                                     EMSP2HUB_HTTPClient     Sender,
                                                     EventTracking_Id        EventTrackingId,
                                                     RemoteParty_Id          RemotePartyId,
                                                     Party_Idv3?             From,
                                                     Party_Idv3?             To,
                                                     Request_Id              RequestId,
                                                     Correlation_Id          CorrelationId,
                                                     TimeSpan                RequestTimeout,

                                                     Session_Id              SessionId,

                                                     CancellationToken       CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetSession request had been received.
    /// </summary>
    public delegate Task OnGetSessionResponseDelegate(DateTimeOffset         LogTimestamp,
                                                      EMSP2HUB_HTTPClient    Sender,
                                                      EventTracking_Id       EventTrackingId,
                                                      RemoteParty_Id         RemotePartyId,
                                                      Party_Idv3?            From,
                                                      Party_Idv3?            To,
                                                      Request_Id             RequestId,
                                                      Correlation_Id         CorrelationId,
                                                      TimeSpan               RequestTimeout,

                                                      Session_Id             SessionId,

                                                      OCPIResponse<Session>  Response,
                                                      TimeSpan               Runtime,
                                                      CancellationToken      CancellationToken);

    #endregion


    #region OnGetCDRsRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetCDRs request will be send.
    /// </summary>
    public delegate Task OnGetCDRsRequestDelegate(DateTimeOffset                   LogTimestamp,
                                                  EMSP2HUB_HTTPClient              Sender,
                                                  EventTracking_Id                 EventTrackingId,
                                                  RemoteParty_Id                   RemotePartyId,
                                                  Party_Idv3?                      From,
                                                  Party_Idv3?                      To,
                                                  Request_Id                       RequestId,
                                                  Correlation_Id                   CorrelationId,
                                                  TimeSpan                         RequestTimeout,

                                                  CancellationToken?               CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetCDRs request had been received.
    /// </summary>
    public delegate Task OnGetCDRsResponseDelegate(DateTimeOffset                  LogTimestamp,
                                                   EMSP2HUB_HTTPClient             Sender,
                                                   EventTracking_Id                EventTrackingId,
                                                   RemoteParty_Id                  RemotePartyId,
                                                   Party_Idv3?                     From,
                                                   Party_Idv3?                     To,
                                                   Request_Id                      RequestId,
                                                   Correlation_Id                  CorrelationId,
                                                   TimeSpan                        RequestTimeout,

                                                   OCPIResponse<IEnumerable<CDR>>  Response,
                                                   TimeSpan                        Runtime,
                                                   CancellationToken?              CancellationToken);

    #endregion

    #region OnGetCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetCDR request will be send.
    /// </summary>
    public delegate Task OnGetCDRRequestDelegate(DateTimeOffset        LogTimestamp,
                                                 EMSP2HUB_HTTPClient   Sender,
                                                 EventTracking_Id      EventTrackingId,
                                                 RemoteParty_Id        RemotePartyId,
                                                 Party_Idv3?           From,
                                                 Party_Idv3?           To,
                                                 Request_Id            RequestId,
                                                 Correlation_Id        CorrelationId,
                                                 TimeSpan              RequestTimeout,

                                                 CDR_Id                CDRId,

                                                 CancellationToken     CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetCDR request had been received.
    /// </summary>
    public delegate Task OnGetCDRResponseDelegate(DateTimeOffset       LogTimestamp,
                                                  EMSP2HUB_HTTPClient  Sender,
                                                  EventTracking_Id     EventTrackingId,
                                                  RemoteParty_Id       RemotePartyId,
                                                  Party_Idv3?          From,
                                                  Party_Idv3?          To,
                                                  Request_Id           RequestId,
                                                  Correlation_Id       CorrelationId,
                                                  TimeSpan             RequestTimeout,

                                                  CDR_Id               CDRId,

                                                  OCPIResponse<CDR>    Response,
                                                  TimeSpan             Runtime,
                                                  CancellationToken    CancellationToken);

    #endregion


    #region OnGetTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetToken request will be send.
    /// </summary>
    public delegate Task OnGetTokenRequestDelegate(DateTimeOffset        LogTimestamp,
                                                   EMSP2HUB_HTTPClient   Sender,
                                                   EventTracking_Id      EventTrackingId,
                                                   RemoteParty_Id        RemotePartyId,
                                                   Party_Idv3?           From,
                                                   Party_Idv3?           To,
                                                   Request_Id            RequestId,
                                                   Correlation_Id        CorrelationId,
                                                   TimeSpan              RequestTimeout,

                                                   CountryCode           CountryCode,
                                                   Party_Id              PartyId,
                                                   Token_Id              TokenId,

                                                   CancellationToken     CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetToken request had been received.
    /// </summary>
    public delegate Task OnGetTokenResponseDelegate(DateTimeOffset       LogTimestamp,
                                                    EMSP2HUB_HTTPClient  Sender,
                                                    EventTracking_Id     EventTrackingId,
                                                    RemoteParty_Id       RemotePartyId,
                                                    Party_Idv3?          From,
                                                    Party_Idv3?          To,
                                                    Request_Id           RequestId,
                                                    Correlation_Id       CorrelationId,
                                                    TimeSpan             RequestTimeout,

                                                    CountryCode          CountryCode,
                                                    Party_Id             PartyId,
                                                    Token_Id             TokenId,

                                                    OCPIResponse<Token>  Response,
                                                    TimeSpan             Runtime,
                                                    CancellationToken    CancellationToken);

    #endregion

    #region OnPutTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a PutToken request will be send.
    /// </summary>
    public delegate Task OnPutTokenRequestDelegate(DateTimeOffset        LogTimestamp,
                                                   EMSP2HUB_HTTPClient   Sender,
                                                   EventTracking_Id      EventTrackingId,
                                                   RemoteParty_Id        RemotePartyId,
                                                   Party_Idv3?           From,
                                                   Party_Idv3?           To,
                                                   Request_Id            RequestId,
                                                   Correlation_Id        CorrelationId,
                                                   TimeSpan              RequestTimeout,

                                                   Token                 Token,

                                                   CancellationToken     CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PutToken request had been received.
    /// </summary>
    public delegate Task OnPutTokenResponseDelegate(DateTimeOffset       LogTimestamp,
                                                    EMSP2HUB_HTTPClient  Sender,
                                                    EventTracking_Id     EventTrackingId,
                                                    RemoteParty_Id       RemotePartyId,
                                                    Party_Idv3?          From,
                                                    Party_Idv3?          To,
                                                    Request_Id           RequestId,
                                                    Correlation_Id       CorrelationId,
                                                    TimeSpan             RequestTimeout,

                                                    Token                Token,

                                                    OCPIResponse<Token>  Response,
                                                    TimeSpan             Runtime,
                                                    CancellationToken    CancellationToken);

    #endregion

    #region OnPatchTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a PatchToken request will be send.
    /// </summary>
    public delegate Task OnPatchTokenRequestDelegate(DateTimeOffset        LogTimestamp,
                                                     EMSP2HUB_HTTPClient   Sender,
                                                     EventTracking_Id      EventTrackingId,
                                                     RemoteParty_Id        RemotePartyId,
                                                     Party_Idv3?           From,
                                                     Party_Idv3?           To,
                                                     Request_Id            RequestId,
                                                     Correlation_Id        CorrelationId,
                                                     TimeSpan              RequestTimeout,

                                                     CountryCode           CountryCode,
                                                     Party_Id              PartyId,
                                                     Token_Id              TokenId,
                                                     JObject               TokenPatch,

                                                     CancellationToken     CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PatchToken request had been received.
    /// </summary>
    public delegate Task OnPatchTokenResponseDelegate(DateTimeOffset       LogTimestamp,
                                                      EMSP2HUB_HTTPClient  Sender,
                                                      EventTracking_Id     EventTrackingId,
                                                      RemoteParty_Id       RemotePartyId,
                                                      Party_Idv3?          From,
                                                      Party_Idv3?          To,
                                                      Request_Id           RequestId,
                                                      Correlation_Id       CorrelationId,
                                                      TimeSpan             RequestTimeout,

                                                      CountryCode          CountryCode,
                                                      Party_Id             PartyId,
                                                      Token_Id             TokenId,
                                                      JObject              TokenPatch,

                                                      OCPIResponse<Token>  Response,
                                                      TimeSpan             Runtime,
                                                      CancellationToken    CancellationToken);

    #endregion


    #region OnGetBookingsRequest/-Response

    /// <summary>
    /// A delegate called whenever a GET ~/bookings request will be send.
    /// </summary>
    public delegate Task OnGetBookingsRequestDelegate(DateTimeOffset                       LogTimestamp,
                                                      EMSP2HUB_HTTPClient                  Sender,
                                                      EventTracking_Id                     EventTrackingId,
                                                      RemoteParty_Id                       RemotePartyId,
                                                      Party_Idv3?                          From,
                                                      Party_Idv3?                          To,
                                                      Request_Id                           RequestId,
                                                      Correlation_Id                       CorrelationId,
                                                      TimeSpan                             RequestTimeout,

                                                      CancellationToken                    CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GET ~/bookings request had been received.
    /// </summary>
    public delegate Task OnGetBookingsResponseDelegate(DateTimeOffset                      LogTimestamp,
                                                       EMSP2HUB_HTTPClient                 Sender,
                                                       EventTracking_Id                    EventTrackingId,
                                                       RemoteParty_Id                      RemotePartyId,
                                                       Party_Idv3?                         From,
                                                       Party_Idv3?                         To,
                                                       Request_Id                          RequestId,
                                                       Correlation_Id                      CorrelationId,
                                                       TimeSpan                            RequestTimeout,

                                                       OCPIResponse<IEnumerable<Booking>>  Response,
                                                       TimeSpan                            Runtime,
                                                       CancellationToken                   CancellationToken);

    #endregion

    #region OnPostBookingsRequest/-Response

    /// <summary>
    /// A delegate called whenever a POST ~/bookings request will be send.
    /// </summary>
    public delegate Task OnPostBookingsRequestDelegate(DateTimeOffset                       LogTimestamp,
                                                       EMSP2HUB_HTTPClient                  Sender,
                                                       EventTracking_Id                     EventTrackingId,
                                                       RemoteParty_Id                       RemotePartyId,
                                                       Party_Idv3?                          From,
                                                       Party_Idv3?                          To,
                                                       Request_Id                           RequestId,
                                                       Correlation_Id                       CorrelationId,
                                                       TimeSpan                             RequestTimeout,

                                                       BookingRequest                       BookingRequest,

                                                       CancellationToken                    CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a POST ~/bookings request had been received.
    /// </summary>
    public delegate Task OnPostBookingsResponseDelegate(DateTimeOffset                      LogTimestamp,
                                                        EMSP2HUB_HTTPClient                 Sender,
                                                        EventTracking_Id                    EventTrackingId,
                                                        RemoteParty_Id                      RemotePartyId,
                                                        Party_Idv3?                         From,
                                                        Party_Idv3?                         To,
                                                        Request_Id                          RequestId,
                                                        Correlation_Id                      CorrelationId,
                                                        TimeSpan                            RequestTimeout,

                                                        BookingRequest                      BookingRequest,

                                                        OCPIResponse<Booking>               Response,
                                                        TimeSpan                            Runtime,
                                                        CancellationToken                   CancellationToken);

    #endregion


    #region OnReserveNowRequest/-Response

    /// <summary>
    /// A delegate called whenever a ReserveNow command request will be send.
    /// </summary>
    public delegate Task OnReserveNowRequestDelegate(DateTimeOffset                                     LogTimestamp,
                                                     EMSP2HUB_HTTPClient                                Sender,
                                                     EventTracking_Id                                   EventTrackingId,
                                                     RemoteParty_Id                                     RemotePartyId,
                                                     Party_Idv3?                                        From,
                                                     Party_Idv3?                                        To,
                                                     Request_Id                                         RequestId,
                                                     Correlation_Id                                     CorrelationId,
                                                     TimeSpan                                           RequestTimeout,

                                                     Token                                              Token,
                                                     DateTimeOffset                                     ExpirationTimestamp,
                                                     Reservation_Id                                     ReservationId,
                                                     Location_Id                                        LocationId,
                                                     EVSE_UId?                                          EVSEUId,
                                                     AuthorizationReference?                            AuthorizationReference,

                                                     CancellationToken                                  CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a ReserveNow command request had been received.
    /// </summary>
    public delegate Task OnReserveNowResponseDelegate(DateTimeOffset                                    LogTimestamp,
                                                      EMSP2HUB_HTTPClient                               Sender,
                                                      EventTracking_Id                                  EventTrackingId,
                                                      RemoteParty_Id                                    RemotePartyId,
                                                      Party_Idv3?                                       From,
                                                      Party_Idv3?                                       To,
                                                      Request_Id                                        RequestId,
                                                      Correlation_Id                                    CorrelationId,
                                                      TimeSpan                                          RequestTimeout,

                                                      Token                                             Token,
                                                      DateTimeOffset                                    ExpirationTimestamp,
                                                      Reservation_Id                                    ReservationId,
                                                      Location_Id                                       LocationId,
                                                      EVSE_UId?                                         EVSEUId,
                                                      AuthorizationReference?                           AuthorizationReference,

                                                      OCPIResponse<ReserveNowCommand, CommandResponse>  Response,
                                                      TimeSpan                                          Runtime,
                                                      CancellationToken                                 CancellationToken);

    #endregion

    #region OnCancelReservationRequest/-Response

    /// <summary>
    /// A delegate called whenever a CancelReservation command request will be send.
    /// </summary>
    public delegate Task OnCancelReservationRequestDelegate(DateTimeOffset                                            LogTimestamp,
                                                            EMSP2HUB_HTTPClient                                       Sender,
                                                            EventTracking_Id                                          EventTrackingId,
                                                            RemoteParty_Id                                            RemotePartyId,
                                                            Party_Idv3?                                               From,
                                                            Party_Idv3?                                               To,
                                                            Request_Id                                                RequestId,
                                                            Correlation_Id                                            CorrelationId,
                                                            TimeSpan                                                  RequestTimeout,

                                                            Reservation_Id                                            ReservationId,

                                                            CancellationToken                                         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a CancelReservation command request had been received.
    /// </summary>
    public delegate Task OnCancelReservationResponseDelegate(DateTimeOffset                                           LogTimestamp,
                                                             EMSP2HUB_HTTPClient                                      Sender,
                                                             EventTracking_Id                                         EventTrackingId,
                                                             RemoteParty_Id                                           RemotePartyId,
                                                             Party_Idv3?                                              From,
                                                             Party_Idv3?                                              To,
                                                             Request_Id                                               RequestId,
                                                             Correlation_Id                                           CorrelationId,
                                                             TimeSpan                                                 RequestTimeout,

                                                             Reservation_Id                                           ReservationId,

                                                             OCPIResponse<CancelReservationCommand, CommandResponse>  Response,
                                                             TimeSpan                                                 Runtime,
                                                             CancellationToken                                        CancellationToken);

    #endregion

    #region OnStartSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a StartSession command request will be send.
    /// </summary>
    public delegate Task OnStartSessionRequestDelegate(DateTimeOffset                                       LogTimestamp,
                                                       EMSP2HUB_HTTPClient                                  Sender,
                                                       EventTracking_Id                                     EventTrackingId,
                                                       RemoteParty_Id                                       RemotePartyId,
                                                       Party_Idv3?                                          From,
                                                       Party_Idv3?                                          To,
                                                       Request_Id                                           RequestId,
                                                       Correlation_Id                                       CorrelationId,
                                                       TimeSpan                                             RequestTimeout,

                                                       Token                                                Token,
                                                       Location_Id                                          LocationId,
                                                       EVSE_UId?                                            EVSEUId,
                                                       Connector_Id?                                        ConnectorId,
                                                       AuthorizationReference?                              AuthorizationReference,

                                                       CancellationToken                                    CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a StartSession command request had been received.
    /// </summary>
    public delegate Task OnStartSessionResponseDelegate(DateTimeOffset                                      LogTimestamp,
                                                        EMSP2HUB_HTTPClient                                 Sender,
                                                        EventTracking_Id                                    EventTrackingId,
                                                        RemoteParty_Id                                      RemotePartyId,
                                                        Party_Idv3?                                         From,
                                                        Party_Idv3?                                         To,
                                                        Request_Id                                          RequestId,
                                                        Correlation_Id                                      CorrelationId,
                                                        TimeSpan                                            RequestTimeout,

                                                        Token                                               Token,
                                                        Location_Id                                         LocationId,
                                                        EVSE_UId?                                           EVSEUId,
                                                        Connector_Id?                                       ConnectorId,
                                                        AuthorizationReference?                             AuthorizationReference,

                                                        OCPIResponse<StartSessionCommand, CommandResponse>  Response,
                                                        TimeSpan                                            Runtime,
                                                        CancellationToken                                   CancellationToken);

    #endregion

    #region OnStopSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a StopSession command request will be send.
    /// </summary>
    public delegate Task OnStopSessionRequestDelegate(DateTimeOffset                                      LogTimestamp,
                                                      EMSP2HUB_HTTPClient                                 Sender,
                                                      EventTracking_Id                                    EventTrackingId,
                                                      RemoteParty_Id                                      RemotePartyId,
                                                      Party_Idv3?                                         From,
                                                      Party_Idv3?                                         To,
                                                      Request_Id                                          RequestId,
                                                      Correlation_Id                                      CorrelationId,
                                                      TimeSpan                                            RequestTimeout,

                                                      Session_Id                                          SessionId,

                                                      CancellationToken                                   CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a StopSession command request had been received.
    /// </summary>
    public delegate Task OnStopSessionResponseDelegate(DateTimeOffset                                     LogTimestamp,
                                                       EMSP2HUB_HTTPClient                                Sender,
                                                       EventTracking_Id                                   EventTrackingId,
                                                       RemoteParty_Id                                     RemotePartyId,
                                                       Party_Idv3?                                        From,
                                                       Party_Idv3?                                        To,
                                                       Request_Id                                         RequestId,
                                                       Correlation_Id                                     CorrelationId,
                                                       TimeSpan                                           RequestTimeout,

                                                       Session_Id                                         SessionId,

                                                       OCPIResponse<StopSessionCommand, CommandResponse>  Response,
                                                       TimeSpan                                           Runtime,
                                                       CancellationToken                                  CancellationToken);

    #endregion

    #region OnUnlockConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever an UnlockConnector command request will be send.
    /// </summary>
    public delegate Task OnUnlockConnectorRequestDelegate(DateTimeOffset                                          LogTimestamp,
                                                          EMSP2HUB_HTTPClient                                     Sender,
                                                          EventTracking_Id                                        EventTrackingId,
                                                          RemoteParty_Id                                          RemotePartyId,
                                                          Party_Idv3?                                             From,
                                                          Party_Idv3?                                             To,
                                                          Request_Id                                              RequestId,
                                                          Correlation_Id                                          CorrelationId,
                                                          TimeSpan                                                RequestTimeout,

                                                          Location_Id                                             LocationId,
                                                          EVSE_UId                                                EVSEUId,
                                                          Connector_Id                                            ConnectorId,

                                                          CancellationToken                                       CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to an UnlockConnector command request had been received.
    /// </summary>
    public delegate Task OnUnlockConnectorResponseDelegate(DateTimeOffset                                         LogTimestamp,
                                                           EMSP2HUB_HTTPClient                                    Sender,
                                                           EventTracking_Id                                       EventTrackingId,
                                                           RemoteParty_Id                                         RemotePartyId,
                                                           Party_Idv3?                                            From,
                                                           Party_Idv3?                                            To,
                                                           Request_Id                                             RequestId,
                                                           Correlation_Id                                         CorrelationId,
                                                           TimeSpan                                               RequestTimeout,

                                                           Location_Id                                            LocationId,
                                                           EVSE_UId                                               EVSEUId,
                                                           Connector_Id                                           ConnectorId,

                                                           OCPIResponse<UnlockConnectorCommand, CommandResponse>  Response,
                                                           TimeSpan                                               Runtime,
                                                           CancellationToken                                      CancellationToken);

    #endregion

    #region OnSetChargingProfileRequest/-Response

    /// <summary>
    /// A delegate called whenever a SetChargingProfile command request will be send.
    /// </summary>
    public delegate Task OnSetChargingProfileRequestDelegate(DateTimeOffset                                            LogTimestamp,
                                                             EMSP2HUB_HTTPClient                                       Sender,
                                                             EventTracking_Id                                          EventTrackingId,
                                                             RemoteParty_Id                                            RemotePartyId,
                                                             Party_Idv3?                                               From,
                                                             Party_Idv3?                                               To,
                                                             Request_Id                                                RequestId,
                                                             Correlation_Id                                            CorrelationId,
                                                             TimeSpan                                                  RequestTimeout,

                                                             ChargingProfile                                           ChargingProfile,

                                                             CancellationToken                                         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a SetChargingProfile command request had been received.
    /// </summary>
    public delegate Task OnSetChargingProfileResponseDelegate(DateTimeOffset                                            LogTimestamp,
                                                              EMSP2HUB_HTTPClient                                       Sender,
                                                              EventTracking_Id                                          EventTrackingId,
                                                              RemoteParty_Id                                            RemotePartyId,
                                                              Party_Idv3?                                               From,
                                                              Party_Idv3?                                               To,
                                                              Request_Id                                                RequestId,
                                                              Correlation_Id                                            CorrelationId,
                                                              TimeSpan                                                  RequestTimeout,

                                                              ChargingProfile                                           ChargingProfile,

                                                              OCPIResponse<SetChargingProfileCommand, CommandResponse>  Response,
                                                              TimeSpan                                                  Runtime,
                                                              CancellationToken                                         CancellationToken);

    #endregion


    // Open Charging Cloud Extensions

    #region OnNotifyWebPaymentsStartedRequest/-Response

    /// <summary>
    /// A delegate called whenever a NotifyWebPaymentsStarted command request will be send.
    /// </summary>
    public delegate Task OnNotifyWebPaymentsStartedRequestDelegate(DateTimeOffset                                                   LogTimestamp,
                                                                   EMSP2HUB_HTTPClient                                              Sender,
                                                                   EventTracking_Id                                                 EventTrackingId,
                                                                   RemoteParty_Id                                                   RemotePartyId,
                                                                   Party_Idv3?                                                      From,
                                                                   Party_Idv3?                                                      To,
                                                                   Request_Id                                                       RequestId,
                                                                   Correlation_Id                                                   CorrelationId,
                                                                   TimeSpan                                                         RequestTimeout,

                                                                   Location_Id                                                      LocationId,
                                                                   EVSE_UId                                                         EVSEUId,
                                                                   EVSE_Id?                                                         EVSEId,
                                                                   Connector_Id?                                                    ConnectorId,
                                                                   TimeSpan?                                                        Timeout,
                                                                   JObject?                                                         CustomData,

                                                                   CancellationToken                                                CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a NotifyWebPaymentsStarted command request had been received.
    /// </summary>
    public delegate Task OnNotifyWebPaymentsStartedResponseDelegate(DateTimeOffset                                                  LogTimestamp,
                                                                    EMSP2HUB_HTTPClient                                             Sender,
                                                                    EventTracking_Id                                                EventTrackingId,
                                                                    RemoteParty_Id                                                  RemotePartyId,
                                                                    Party_Idv3?                                                     From,
                                                                    Party_Idv3?                                                     To,
                                                                    Request_Id                                                      RequestId,
                                                                    Correlation_Id                                                  CorrelationId,
                                                                    TimeSpan                                                        RequestTimeout,

                                                                    Location_Id                                                     LocationId,
                                                                    EVSE_UId                                                        EVSEUId,
                                                                    EVSE_Id?                                                        EVSEId,
                                                                    Connector_Id?                                                   ConnectorId,
                                                                    TimeSpan?                                                       Timeout,
                                                                    JObject?                                                        CustomData,

                                                                    OCPIResponse<NotifyWebPaymentsStartedCommand, CommandResponse>  Response,
                                                                    TimeSpan                                                        Runtime,
                                                                    CancellationToken                                               CancellationToken);

    #endregion

    #region OnNotifyWebPaymentsFailedRequest/-Response

    /// <summary>
    /// A delegate called whenever a NotifyWebPaymentsFailed command request will be send.
    /// </summary>
    public delegate Task OnNotifyWebPaymentsFailedRequestDelegate(DateTimeOffset                                                  LogTimestamp,
                                                                  EMSP2HUB_HTTPClient                                             Sender,
                                                                  EventTracking_Id                                                EventTrackingId,
                                                                  RemoteParty_Id                                                  RemotePartyId,
                                                                  Party_Idv3?                                                     From,
                                                                  Party_Idv3?                                                     To,
                                                                  Request_Id                                                      RequestId,
                                                                  Correlation_Id                                                  CorrelationId,
                                                                  TimeSpan                                                        RequestTimeout,

                                                                  Location_Id                                                     LocationId,
                                                                  EVSE_UId                                                        EVSEUId,
                                                                  EVSE_Id?                                                        EVSEId,
                                                                  Connector_Id?                                                   ConnectorId,
                                                                  DisplayTexts?                                                   ErrorMessage,
                                                                  JObject?                                                        CustomData,

                                                                  CancellationToken                                               CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a NotifyWebPaymentsFailed command request had been received.
    /// </summary>
    public delegate Task OnNotifyWebPaymentsFailedResponseDelegate(DateTimeOffset                                                 LogTimestamp,
                                                                   EMSP2HUB_HTTPClient                                            Sender,
                                                                   EventTracking_Id                                               EventTrackingId,
                                                                   RemoteParty_Id                                                 RemotePartyId,
                                                                   Party_Idv3?                                                    From,
                                                                   Party_Idv3?                                                    To,
                                                                   Request_Id                                                     RequestId,
                                                                   Correlation_Id                                                 CorrelationId,
                                                                   TimeSpan                                                       RequestTimeout,

                                                                   Location_Id                                                    LocationId,
                                                                   EVSE_UId                                                       EVSEUId,
                                                                   EVSE_Id?                                                       EVSEId,
                                                                   Connector_Id?                                                  ConnectorId,
                                                                   DisplayTexts?                                                  ErrorMessage,
                                                                   JObject?                                                       CustomData,

                                                                   OCPIResponse<NotifyWebPaymentsFailedCommand, CommandResponse>  Response,
                                                                   TimeSpan                                                       Runtime,
                                                                   CancellationToken                                              CancellationToken);

    #endregion

}
