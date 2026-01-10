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
using cloud.charging.open.protocols.OCPIv2_2_1.HUB.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1.HUB2EMSP.HTTP
{

    #region OnGetLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a get location request will be send.
    /// </summary>
    public delegate Task OnGetLocationRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      HUB2EMSPClient            Sender,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,

                                                      CountryCode               CountryCode,
                                                      Party_Id                  PartyId,
                                                      Location_Id               LocationId,

                                                      CancellationToken         CancellationToken,
                                                      EventTracking_Id          EventTrackingId,
                                                      TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get location request had been received.
    /// </summary>
    public delegate Task OnGetLocationResponseDelegate(DateTimeOffset           LogTimestamp,
                                                       HUB2EMSPClient           Sender,
                                                       Request_Id               RequestId,
                                                       Correlation_Id           CorrelationId,

                                                       CountryCode              CountryCode,
                                                       Party_Id                 PartyId,
                                                       Location_Id              LocationId,

                                                       CancellationToken        CancellationToken,
                                                       EventTracking_Id         EventTrackingId,
                                                       TimeSpan                 RequestTimeout,

                                                       OCPIResponse<Location>   Response,
                                                       TimeSpan                 Runtime);

    #endregion

    #region OnPutLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a put location request will be send.
    /// </summary>
    public delegate Task OnPutLocationRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      HUB2EMSPClient            Sender,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,

                                                      Location                  Location,

                                                      CancellationToken         CancellationToken,
                                                      EventTracking_Id          EventTrackingId,
                                                      TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put location request had been received.
    /// </summary>
    public delegate Task OnPutLocationResponseDelegate(DateTimeOffset           LogTimestamp,
                                                       HUB2EMSPClient           Sender,
                                                       Request_Id               RequestId,
                                                       Correlation_Id           CorrelationId,

                                                       Location                 Location,

                                                       CancellationToken        CancellationToken,
                                                       EventTracking_Id         EventTrackingId,
                                                       TimeSpan                 RequestTimeout,

                                                       OCPIResponse<Location>   Response,
                                                       TimeSpan                 Runtime);

    #endregion

    #region OnPatchLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch location request will be send.
    /// </summary>
    public delegate Task OnPatchLocationRequestDelegate(DateTimeOffset           LogTimestamp,
                                                        HUB2EMSPClient           Sender,
                                                        Request_Id               RequestId,
                                                        Correlation_Id           CorrelationId,

                                                        Location_Id              LocationId,
                                                        JObject                  LocationPatch,

                                                        CancellationToken        CancellationToken,
                                                        EventTracking_Id         EventTrackingId,
                                                        TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a patch location request had been received.
    /// </summary>
    public delegate Task OnPatchLocationResponseDelegate(DateTimeOffset           LogTimestamp,
                                                         HUB2EMSPClient           Sender,
                                                         Request_Id               RequestId,
                                                         Correlation_Id           CorrelationId,

                                                         Location_Id              LocationId,
                                                         JObject                  LocationPatch,

                                                         CancellationToken        CancellationToken,
                                                         EventTracking_Id         EventTrackingId,
                                                         TimeSpan                 RequestTimeout,

                                                         OCPIResponse<Location>   Response,
                                                         TimeSpan                 Runtime);

    #endregion


    #region OnGetEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a get EVSE request will be send.
    /// </summary>
    public delegate Task OnGetEVSERequestDelegate(DateTimeOffset           LogTimestamp,
                                                  HUB2EMSPClient           Sender,
                                                  Request_Id               RequestId,
                                                  Correlation_Id           CorrelationId,

                                                  Location_Id              LocationId,
                                                  EVSE_UId                 EVSEUId,

                                                  CancellationToken        CancellationToken,
                                                  EventTracking_Id         EventTrackingId,
                                                  TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get EVSE request had been received.
    /// </summary>
    public delegate Task OnGetEVSEResponseDelegate(DateTimeOffset           LogTimestamp,
                                                   HUB2EMSPClient           Sender,
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

    #region OnPutEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a put EVSE request will be send.
    /// </summary>
    public delegate Task OnPutEVSERequestDelegate(DateTimeOffset           LogTimestamp,
                                                  HUB2EMSPClient           Sender,
                                                  Request_Id               RequestId,
                                                  Correlation_Id           CorrelationId,

                                                  EVSE                     EVSE,
                                                  CountryCode              CountryCode,
                                                  Party_Id                 PartyId,
                                                  Location_Id              LocationId,

                                                  CancellationToken        CancellationToken,
                                                  EventTracking_Id         EventTrackingId,
                                                  TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put EVSE request had been received.
    /// </summary>
    public delegate Task OnPutEVSEResponseDelegate(DateTimeOffset           LogTimestamp,
                                                   HUB2EMSPClient           Sender,
                                                   Request_Id               RequestId,
                                                   Correlation_Id           CorrelationId,

                                                   EVSE                     EVSE,
                                                   CountryCode              CountryCode,
                                                   Party_Id                 PartyId,
                                                   Location_Id              LocationId,

                                                   CancellationToken        CancellationToken,
                                                   EventTracking_Id         EventTrackingId,
                                                   TimeSpan                 RequestTimeout,

                                                   OCPIResponse<EVSE>       Response,
                                                   TimeSpan                 Runtime);

    #endregion

    #region OnPatchEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a patch EVSE request will be send.
    /// </summary>
    public delegate Task OnPatchEVSERequestDelegate(DateTimeOffset           LogTimestamp,
                                                    HUB2EMSPClient           Sender,
                                                    Request_Id               RequestId,
                                                    Correlation_Id           CorrelationId,

                                                    CountryCode              CountryCode,
                                                    Party_Id                 PartyId,
                                                    Location_Id              LocationId,
                                                    EVSE_UId                 EVSEUId,
                                                    JObject                  EVSEPatch,

                                                    CancellationToken        CancellationToken,
                                                    EventTracking_Id         EventTrackingId,
                                                    TimeSpan                 RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a patch EVSE request had been received.
    /// </summary>
    public delegate Task OnPatchEVSEResponseDelegate(DateTimeOffset           LogTimestamp,
                                                     HUB2EMSPClient           Sender,
                                                     Request_Id               RequestId,
                                                     Correlation_Id           CorrelationId,

                                                     CountryCode              CountryCode,
                                                     Party_Id                 PartyId,
                                                     Location_Id              LocationId,
                                                     EVSE_UId                 EVSEUId,
                                                     JObject                  EVSEPatch,

                                                     CancellationToken        CancellationToken,
                                                     EventTracking_Id         EventTrackingId,
                                                     TimeSpan                 RequestTimeout,

                                                     OCPIResponse<EVSE>       Response,
                                                     TimeSpan                 Runtime);

    #endregion


    #region OnGetConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a get connector request will be send.
    /// </summary>
    public delegate Task OnGetConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       HUB2EMSPClient            Sender,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Location_Id               LocationId,
                                                       EVSE_UId                  EVSEUId,
                                                       Connector_Id              ConnectorId,

                                                       CancellationToken         CancellationToken,
                                                       EventTracking_Id          EventTrackingId,
                                                       TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get connector request had been received.
    /// </summary>
    public delegate Task OnGetConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        HUB2EMSPClient            Sender,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Location_Id               LocationId,
                                                        EVSE_UId                  EVSEUId,
                                                        Connector_Id              ConnectorId,

                                                        CancellationToken         CancellationToken,
                                                        EventTracking_Id          EventTrackingId,
                                                        TimeSpan                  RequestTimeout,

                                                        OCPIResponse<Connector>   Response,
                                                        TimeSpan                  Runtime);

    #endregion

    #region OnPutConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a put connector request will be send.
    /// </summary>
    public delegate Task OnPutConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       HUB2EMSPClient            Sender,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,

                                                       Connector                 Connector,

                                                       CancellationToken         CancellationToken,
                                                       EventTracking_Id          EventTrackingId,
                                                       TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put connector request had been received.
    /// </summary>
    public delegate Task OnPutConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        HUB2EMSPClient            Sender,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,

                                                        Connector                 Connector,

                                                        CancellationToken         CancellationToken,
                                                        EventTracking_Id          EventTrackingId,
                                                        TimeSpan                  RequestTimeout,

                                                        OCPIResponse<Connector>   Response,
                                                        TimeSpan                  Runtime);

    #endregion

    #region OnPatchConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch connector request will be send.
    /// </summary>
    public delegate Task OnPatchConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                         HUB2EMSPClient            Sender,
                                                         Request_Id                RequestId,
                                                         Correlation_Id            CorrelationId,

                                                         CountryCode               CountryCode,
                                                         Party_Id                  PartyId,
                                                         Location_Id               LocationId,
                                                         EVSE_UId                  EVSEUId,
                                                         Connector_Id              ConnectorId,
                                                         JObject                   ConnectorPatch,

                                                         CancellationToken         CancellationToken,
                                                         EventTracking_Id          EventTrackingId,
                                                         TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a patch connector request had been received.
    /// </summary>
    public delegate Task OnPatchConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                          HUB2EMSPClient            Sender,
                                                          Request_Id                RequestId,
                                                          Correlation_Id            CorrelationId,

                                                          CountryCode               CountryCode,
                                                          Party_Id                  PartyId,
                                                          Location_Id               LocationId,
                                                          EVSE_UId                  EVSEUId,
                                                          Connector_Id              ConnectorId,
                                                          JObject                   ConnectorPatch,

                                                          CancellationToken         CancellationToken,
                                                          EventTracking_Id          EventTrackingId,
                                                          TimeSpan                  RequestTimeout,

                                                          OCPIResponse<Connector>   Response,
                                                          TimeSpan                  Runtime);

    #endregion


    #region OnGetTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tariff request will be send.
    /// </summary>
    public delegate Task OnGetTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                    HUB2EMSPClient            Sender,
                                                    Request_Id                RequestId,
                                                    Correlation_Id            CorrelationId,

                                                    CountryCode               CountryCode,
                                                    Party_Id                  PartyId,
                                                    Tariff_Id                 TariffId,

                                                    CancellationToken         CancellationToken,
                                                    EventTracking_Id          EventTrackingId,
                                                    TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get tariff request had been received.
    /// </summary>
    public delegate Task OnGetTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                     HUB2EMSPClient            Sender,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,

                                                     CountryCode               CountryCode,
                                                     Party_Id                  PartyId,
                                                     Tariff_Id                 TariffId,

                                                     CancellationToken         CancellationToken,
                                                     EventTracking_Id          EventTrackingId,
                                                     TimeSpan                  RequestTimeout,

                                                     OCPIResponse<Tariff>      Response,
                                                     TimeSpan                  Runtime);

    #endregion

    #region OnPutTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a put tariff request will be send.
    /// </summary>
    public delegate Task OnPutTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                    HUB2EMSPClient            Sender,
                                                    Request_Id                RequestId,
                                                    Correlation_Id            CorrelationId,

                                                    Tariff                    Tariff,

                                                    CancellationToken         CancellationToken,
                                                    EventTracking_Id          EventTrackingId,
                                                    TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put tariff request had been received.
    /// </summary>
    public delegate Task OnPutTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                     HUB2EMSPClient            Sender,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,

                                                     Tariff                    Tariff,

                                                     CancellationToken         CancellationToken,
                                                     EventTracking_Id          EventTrackingId,
                                                     TimeSpan                  RequestTimeout,

                                                     OCPIResponse<Tariff>      Response,
                                                     TimeSpan                  Runtime);

    #endregion

    #region OnPatchTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch tariff request will be send.
    /// </summary>
    public delegate Task OnPatchTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      HUB2EMSPClient            Sender,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,

                                                      CountryCode               CountryCode,
                                                      Party_Id                  PartyId,
                                                      Tariff_Id                 TariffId,
                                                      JObject                   TariffPatch,

                                                      CancellationToken         CancellationToken,
                                                      EventTracking_Id          EventTrackingId,
                                                      TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a patch tariff request had been received.
    /// </summary>
    public delegate Task OnPatchTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                       HUB2EMSPClient            Sender,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Tariff_Id                 TariffId,
                                                       JObject                   TariffPatch,

                                                       CancellationToken         CancellationToken,
                                                       EventTracking_Id          EventTrackingId,
                                                       TimeSpan                  RequestTimeout,

                                                       OCPIResponse<Tariff>      Response,
                                                       TimeSpan                  Runtime);

    #endregion

    #region OnDeleteTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a delete tariff request will be send.
    /// </summary>
    public delegate Task OnDeleteTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       HUB2EMSPClient            Sender,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Tariff_Id                 TariffId,

                                                       CancellationToken         CancellationToken,
                                                       EventTracking_Id          EventTrackingId,
                                                       TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a delete tariff request had been received.
    /// </summary>
    public delegate Task OnDeleteTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        HUB2EMSPClient            Sender,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Tariff_Id                 TariffId,

                                                        CancellationToken         CancellationToken,
                                                        EventTracking_Id          EventTrackingId,
                                                        TimeSpan                  RequestTimeout,

                                                        OCPIResponse<Tariff>      Response,
                                                        TimeSpan                  Runtime);

    #endregion


    #region OnGetSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a get session request will be send.
    /// </summary>
    public delegate Task OnGetSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                     HUB2EMSPClient            Sender,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,

                                                     CountryCode               CountryCode,
                                                     Party_Id                  PartyId,
                                                     Session_Id                SessionId,

                                                     CancellationToken         CancellationToken,
                                                     EventTracking_Id          EventTrackingId,
                                                     TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get session request had been received.
    /// </summary>
    public delegate Task OnGetSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                      HUB2EMSPClient            Sender,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,

                                                      CountryCode               CountryCode,
                                                      Party_Id                  PartyId,
                                                      Session_Id                SessionId,

                                                      CancellationToken         CancellationToken,
                                                      EventTracking_Id          EventTrackingId,
                                                      TimeSpan                  RequestTimeout,

                                                      OCPIResponse<Session>     Response,
                                                      TimeSpan                  Runtime);

    #endregion

    #region OnPutSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a put session request will be send.
    /// </summary>
    public delegate Task OnPutSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                     HUB2EMSPClient            Sender,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,

                                                     Session                   Session,

                                                     CancellationToken         CancellationToken,
                                                     EventTracking_Id          EventTrackingId,
                                                     TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put session request had been received.
    /// </summary>
    public delegate Task OnPutSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                      HUB2EMSPClient            Sender,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,

                                                      Session                   Session,

                                                      CancellationToken         CancellationToken,
                                                      EventTracking_Id          EventTrackingId,
                                                      TimeSpan                  RequestTimeout,

                                                      OCPIResponse<Session>     Response,
                                                      TimeSpan                  Runtime);

    #endregion

    #region OnPatchSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch session request will be send.
    /// </summary>
    public delegate Task OnPatchSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       HUB2EMSPClient            Sender,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Session_Id                SessionId,
                                                       JObject                   SessionPatch,

                                                       CancellationToken         CancellationToken,
                                                       EventTracking_Id          EventTrackingId,
                                                       TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a patch session request had been received.
    /// </summary>
    public delegate Task OnPatchSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        HUB2EMSPClient            Sender,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Session_Id                SessionId,
                                                        JObject                   SessionPatch,

                                                        CancellationToken         CancellationToken,
                                                        EventTracking_Id          EventTrackingId,
                                                        TimeSpan                  RequestTimeout,

                                                        OCPIResponse<Session>     Response,
                                                        TimeSpan                  Runtime);

    #endregion

    #region OnDeleteSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a delete session request will be send.
    /// </summary>
    public delegate Task OnDeleteSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                        HUB2EMSPClient            Sender,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Session_Id                SessionId,

                                                        CancellationToken         CancellationToken,
                                                        EventTracking_Id          EventTrackingId,
                                                        TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a delete session request had been received.
    /// </summary>
    public delegate Task OnDeleteSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                         HUB2EMSPClient            Sender,
                                                         Request_Id                RequestId,
                                                         Correlation_Id            CorrelationId,

                                                         CountryCode               CountryCode,
                                                         Party_Id                  PartyId,
                                                         Session_Id                SessionId,

                                                         CancellationToken         CancellationToken,
                                                         EventTracking_Id          EventTrackingId,
                                                         TimeSpan                  RequestTimeout,

                                                         OCPIResponse<Session>     Response,
                                                         TimeSpan                  Runtime);

    #endregion


    #region OnGetCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a get charge detail record request will be send.
    /// </summary>
    public delegate Task OnGetCDRRequestDelegate(DateTimeOffset            LogTimestamp,
                                                 HUB2EMSPClient            Sender,
                                                 Request_Id                RequestId,
                                                 Correlation_Id            CorrelationId,

                                                 CountryCode               CountryCode,
                                                 Party_Id                  PartyId,
                                                 CDR_Id                    CDRId,

                                                 CancellationToken         CancellationToken,
                                                 EventTracking_Id          EventTrackingId,
                                                 TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get charge detail record request had been received.
    /// </summary>
    public delegate Task OnGetCDRResponseDelegate(DateTimeOffset            LogTimestamp,
                                                  HUB2EMSPClient            Sender,
                                                  Request_Id                RequestId,
                                                  Correlation_Id            CorrelationId,

                                                  CountryCode               CountryCode,
                                                  Party_Id                  PartyId,
                                                  CDR_Id                    CDRId,

                                                  CancellationToken         CancellationToken,
                                                  EventTracking_Id          EventTrackingId,
                                                  TimeSpan                  RequestTimeout,

                                                  OCPIResponse<CDR>         Response,
                                                  TimeSpan                  Runtime);

    #endregion

    #region OnPostCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a post charge detail record request will be send.
    /// </summary>
    public delegate Task OnPostCDRRequestDelegate(DateTimeOffset            LogTimestamp,
                                                  HUB2EMSPClient            Sender,
                                                  Request_Id                RequestId,
                                                  Correlation_Id            CorrelationId,

                                                  CDR                       CDR,

                                                  CancellationToken         CancellationToken,
                                                  EventTracking_Id          EventTrackingId,
                                                  TimeSpan                  RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a post charge detail record request had been received.
    /// </summary>
    public delegate Task OnPostCDRResponseDelegate(DateTimeOffset            LogTimestamp,
                                                   HUB2EMSPClient            Sender,
                                                   Request_Id                RequestId,
                                                   Correlation_Id            CorrelationId,

                                                   CDR                       CDR,

                                                   CancellationToken         CancellationToken,
                                                   EventTracking_Id          EventTrackingId,
                                                   TimeSpan                  RequestTimeout,

                                                   OCPIResponse<CDR>         Response,
                                                   TimeSpan                  Runtime);

    #endregion


    #region OnGetTokensRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tokens request will be send.
    /// </summary>
    public delegate Task OnGetTokensRequestDelegate(DateTimeOffset                     LogTimestamp,
                                                    HUB2EMSPClient                          Sender,
                                                    Request_Id                         RequestId,
                                                    Correlation_Id                     CorrelationId,

                                                    UInt64?                            Offset,
                                                    UInt64?                            Limit,

                                                    CancellationToken?                 CancellationToken,
                                                    EventTracking_Id                   EventTrackingId,
                                                    TimeSpan                           RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get tokens request had been received.
    /// </summary>
    public delegate Task OnGetTokensResponseDelegate(DateTimeOffset                     LogTimestamp,
                                                     HUB2EMSPClient                          Sender,
                                                     Request_Id                         RequestId,
                                                     Correlation_Id                     CorrelationId,

                                                     UInt64?                            Offset,
                                                     UInt64?                            Limit,

                                                     CancellationToken?                 CancellationToken,
                                                     EventTracking_Id                   EventTrackingId,
                                                     TimeSpan                           RequestTimeout,

                                                     OCPIResponse<IEnumerable<Token>>   Response,
                                                     TimeSpan                           Runtime);

    #endregion

    #region OnPostTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a post token request will be send.
    /// </summary>
    public delegate Task OnPostTokenRequestDelegate(DateTimeOffset                     LogTimestamp,
                                                    HUB2EMSPClient                          Sender,
                                                    Request_Id                         RequestId,
                                                    Correlation_Id                     CorrelationId,

                                                    Token_Id                           TokenId,
                                                    TokenType?                         TokenType,
                                                    LocationReference?                 LocationReference,

                                                    CancellationToken?                 CancellationToken,
                                                    EventTracking_Id                   EventTrackingId,
                                                    TimeSpan                           RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a post token request had been received.
    /// </summary>
    public delegate Task OnPostTokenResponseDelegate(DateTimeOffset                    LogTimestamp,
                                                     HUB2EMSPClient                         Sender,
                                                     Request_Id                        RequestId,
                                                     Correlation_Id                    CorrelationId,

                                                     Token_Id                          TokenId,
                                                     TokenType?                        TokenType,
                                                     LocationReference?                LocationReference,

                                                     CancellationToken?                CancellationToken,
                                                     EventTracking_Id                  EventTrackingId,
                                                     TimeSpan                          RequestTimeout,

                                                     OCPIResponse<AuthorizationInfo>   Response,
                                                     TimeSpan                          Runtime);

    #endregion


    #region OnSetChargingProfileRequest/-Response

    /// <summary>
    /// A delegate called whenever a set charging profile request will be send.
    /// </summary>
    public delegate Task OnSetChargingProfileRequestDelegate(DateTimeOffset                          LogTimestamp,
                                                             HUB2EMSPClient                               Sender,
                                                             Request_Id                              RequestId,
                                                             Correlation_Id                          CorrelationId,

                                                             Session_Id                              SessionId,
                                                             ChargingProfile                         ChargingProfile,

                                                             CancellationToken?                      CancellationToken,
                                                             EventTracking_Id                        EventTrackingId,
                                                             TimeSpan                                RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a set charging profile request had been received.
    /// </summary>
    public delegate Task OnSetChargingProfileResponseDelegate(DateTimeOffset                          LogTimestamp,
                                                              HUB2EMSPClient                               Sender,
                                                              Request_Id                              RequestId,
                                                              Correlation_Id                          CorrelationId,

                                                              Session_Id                              SessionId,
                                                              ChargingProfile                         ChargingProfile,

                                                              CancellationToken?                      CancellationToken,
                                                              EventTracking_Id                        EventTrackingId,
                                                              TimeSpan                                RequestTimeout,

                                                              OCPIResponse<ChargingProfileResponse>   Response,
                                                              TimeSpan                                Runtime);

    #endregion

}
