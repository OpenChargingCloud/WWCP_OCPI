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
using Hermod = org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.CPO.HTTP
{

    #region OnGetLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a get location request will be send.
    /// </summary>
    public delegate Task OnGetLocationRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,
                                                      TimeSpan                  RequestTimeout,

                                                      Location_Id               LocationId,

                                                      CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a get location request had been received.
    /// </summary>
    public delegate Task OnGetLocationResponseDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       Location_Id               LocationId,

                                                       OCPIResponse<Location>    Response,
                                                       TimeSpan                  Runtime,
                                                       CancellationToken         CancellationToken);

    #endregion

    #region OnPutLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a put location request will be send.
    /// </summary>
    public delegate Task OnPutLocationRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,
                                                      TimeSpan                  RequestTimeout,

                                                      Location                  Location,

                                                      CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a put location request had been received.
    /// </summary>
    public delegate Task OnPutLocationResponseDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       Location                  Location,

                                                       OCPIResponse<Location>    Response,
                                                       TimeSpan                  Runtime,
                                                       CancellationToken         CancellationToken);

    #endregion

    #region OnPatchLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch location request will be send.
    /// </summary>
    public delegate Task OnPatchLocationRequestDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        Location_Id               LocationId,
                                                        JObject                   LocationPatch,

                                                        CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a patch location request had been received.
    /// </summary>
    public delegate Task OnPatchLocationResponseDelegate(DateTimeOffset            LogTimestamp,
                                                         CPO2EMSP_HTTPClient       Sender,
                                                         EventTracking_Id          EventTrackingId,
                                                         Request_Id                RequestId,
                                                         Correlation_Id            CorrelationId,
                                                         TimeSpan                  RequestTimeout,

                                                         Location_Id               LocationId,
                                                         JObject                   LocationPatch,

                                                         OCPIResponse<Location>    Response,
                                                         TimeSpan                  Runtime,
                                                         CancellationToken         CancellationToken);

    #endregion


    #region OnGetEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a get EVSE request will be send.
    /// </summary>
    public delegate Task OnGetEVSERequestDelegate(DateTimeOffset            LogTimestamp,
                                                  CPO2EMSP_HTTPClient       Sender,
                                                  EventTracking_Id          EventTrackingId,
                                                  Request_Id                RequestId,
                                                  Correlation_Id            CorrelationId,
                                                  TimeSpan                  RequestTimeout,

                                                  Location_Id               LocationId,
                                                  EVSE_UId                  EVSEUId,

                                                  CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a get EVSE request had been received.
    /// </summary>
    public delegate Task OnGetEVSEResponseDelegate(DateTimeOffset            LogTimestamp,
                                                   CPO2EMSP_HTTPClient       Sender,
                                                   EventTracking_Id          EventTrackingId,
                                                   Request_Id                RequestId,
                                                   Correlation_Id            CorrelationId,
                                                   TimeSpan                  RequestTimeout,

                                                   Location_Id               LocationId,
                                                   EVSE_UId                  EVSEUId,

                                                   OCPIResponse<EVSE>        Response,
                                                   TimeSpan                  Runtime,
                                                   CancellationToken         CancellationToken);

    #endregion

    #region OnPutEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a put EVSE request will be send.
    /// </summary>
    public delegate Task OnPutEVSERequestDelegate(DateTimeOffset            LogTimestamp,
                                                  CPO2EMSP_HTTPClient       Sender,
                                                  EventTracking_Id          EventTrackingId,
                                                  Request_Id                RequestId,
                                                  Correlation_Id            CorrelationId,
                                                  TimeSpan                  RequestTimeout,

                                                  EVSE                      EVSE,
                                                  Location_Id               LocationId,

                                                  CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a put EVSE request had been received.
    /// </summary>
    public delegate Task OnPutEVSEResponseDelegate(DateTimeOffset            LogTimestamp,
                                                   CPO2EMSP_HTTPClient       Sender,
                                                   EventTracking_Id          EventTrackingId,
                                                   Request_Id                RequestId,
                                                   Correlation_Id            CorrelationId,
                                                   TimeSpan                  RequestTimeout,

                                                   EVSE                      EVSE,
                                                   Location_Id               LocationId,

                                                   OCPIResponse<EVSE>        Response,
                                                   TimeSpan                  Runtime,
                                                   CancellationToken         CancellationToken);

    #endregion

    #region OnPatchEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a patch EVSE request will be send.
    /// </summary>
    public delegate Task OnPatchEVSERequestDelegate(DateTimeOffset            LogTimestamp,
                                                    CPO2EMSP_HTTPClient       Sender,
                                                    EventTracking_Id          EventTrackingId,
                                                    Request_Id                RequestId,
                                                    Correlation_Id            CorrelationId,
                                                    TimeSpan                  RequestTimeout,

                                                    Location_Id               LocationId,
                                                    EVSE_UId                  EVSEUId,
                                                    JObject                   EVSEPatch,

                                                    CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a patch EVSE request had been received.
    /// </summary>
    public delegate Task OnPatchEVSEResponseDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,
                                                     TimeSpan                  RequestTimeout,

                                                     Location_Id               LocationId,
                                                     EVSE_UId                  EVSEUId,
                                                     JObject                   EVSEPatch,

                                                     OCPIResponse<EVSE>        Response,
                                                     TimeSpan                  Runtime,
                                                     CancellationToken         CancellationToken);

    #endregion


    #region OnGetConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a get connector request will be send.
    /// </summary>
    public delegate Task OnGetConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       Location_Id               LocationId,
                                                       EVSE_UId                  EVSEUId,
                                                       Connector_Id              ConnectorId,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a get connector request had been received.
    /// </summary>
    public delegate Task OnGetConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        Location_Id               LocationId,
                                                        EVSE_UId                  EVSEUId,
                                                        Connector_Id              ConnectorId,

                                                        OCPIResponse<Connector>   Response,
                                                        TimeSpan                  Runtime,
                                                        CancellationToken         CancellationToken);

    #endregion

    #region OnPutConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a put connector request will be send.
    /// </summary>
    public delegate Task OnPutConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       Connector                 Connector,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a put connector request had been received.
    /// </summary>
    public delegate Task OnPutConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        Connector                 Connector,

                                                        OCPIResponse<Connector>   Response,
                                                        TimeSpan                  Runtime,
                                                        CancellationToken         CancellationToken);

    #endregion

    #region OnPatchConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch connector request will be send.
    /// </summary>
    public delegate Task OnPatchConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                         CPO2EMSP_HTTPClient       Sender,
                                                         EventTracking_Id          EventTrackingId,
                                                         Request_Id                RequestId,
                                                         Correlation_Id            CorrelationId,
                                                         TimeSpan                  RequestTimeout,

                                                         Location_Id               LocationId,
                                                         EVSE_UId                  EVSEUId,
                                                         Connector_Id              ConnectorId,
                                                         JObject                   ConnectorPatch,

                                                         CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a patch connector request had been received.
    /// </summary>
    public delegate Task OnPatchConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                          CPO2EMSP_HTTPClient       Sender,
                                                          EventTracking_Id          EventTrackingId,
                                                          Request_Id                RequestId,
                                                          Correlation_Id            CorrelationId,
                                                          TimeSpan                  RequestTimeout,

                                                          Location_Id               LocationId,
                                                          EVSE_UId                  EVSEUId,
                                                          Connector_Id              ConnectorId,
                                                          JObject                   ConnectorPatch,

                                                          OCPIResponse<Connector>   Response,
                                                          TimeSpan                  Runtime,
                                                          CancellationToken         CancellationToken);

    #endregion


    #region OnGetTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tariff request will be send.
    /// </summary>
    public delegate Task OnGetTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                    CPO2EMSP_HTTPClient       Sender,
                                                    EventTracking_Id          EventTrackingId,
                                                    Request_Id                RequestId,
                                                    Correlation_Id            CorrelationId,
                                                    TimeSpan                  RequestTimeout,

                                                    CountryCode               CountryCode,
                                                    Party_Id                  PartyId,
                                                    Tariff_Id                 TariffId,

                                                    CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a get tariff request had been received.
    /// </summary>
    public delegate Task OnGetTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,
                                                     TimeSpan                  RequestTimeout,

                                                     CountryCode               CountryCode,
                                                     Party_Id                  PartyId,
                                                     Tariff_Id                 TariffId,

                                                     OCPIResponse<Tariff>      Response,
                                                     TimeSpan                  Runtime,
                                                     CancellationToken         CancellationToken);

    #endregion

    #region OnPutTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a put tariff request will be send.
    /// </summary>
    public delegate Task OnPutTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                    CPO2EMSP_HTTPClient       Sender,
                                                    EventTracking_Id          EventTrackingId,
                                                    Request_Id                RequestId,
                                                    Correlation_Id            CorrelationId,
                                                    TimeSpan                  RequestTimeout,

                                                    Tariff                    Tariff,

                                                    CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a put tariff request had been received.
    /// </summary>
    public delegate Task OnPutTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,
                                                     TimeSpan                  RequestTimeout,

                                                     Tariff                    Tariff,

                                                     OCPIResponse<Tariff>      Response,
                                                     TimeSpan                  Runtime,
                                                     CancellationToken         CancellationToken);

    #endregion

    #region OnPatchTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch tariff request will be send.
    /// </summary>
    public delegate Task OnPatchTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,
                                                      TimeSpan                  RequestTimeout,

                                                      CountryCode               CountryCode,
                                                      Party_Id                  PartyId,
                                                      Tariff_Id                 TariffId,
                                                      JObject                   TariffPatch,

                                                      CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a patch tariff request had been received.
    /// </summary>
    public delegate Task OnPatchTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Tariff_Id                 TariffId,
                                                       JObject                   TariffPatch,

                                                       OCPIResponse<Tariff>      Response,
                                                       TimeSpan                  Runtime,
                                                       CancellationToken         CancellationToken);

    #endregion

    #region OnDeleteTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a delete tariff request will be send.
    /// </summary>
    public delegate Task OnDeleteTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Tariff_Id                 TariffId,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a delete tariff request had been received.
    /// </summary>
    public delegate Task OnDeleteTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Tariff_Id                 TariffId,

                                                        OCPIResponse<Tariff>      Response,
                                                        TimeSpan                  Runtime,
                                                        CancellationToken         CancellationToken);

    #endregion


    #region OnGetSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a get session request will be send.
    /// </summary>
    public delegate Task OnGetSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,
                                                     TimeSpan                  RequestTimeout,

                                                     CountryCode               CountryCode,
                                                     Party_Id                  PartyId,
                                                     Session_Id                SessionId,

                                                     CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a get session request had been received.
    /// </summary>
    public delegate Task OnGetSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,
                                                      TimeSpan                  RequestTimeout,

                                                      CountryCode               CountryCode,
                                                      Party_Id                  PartyId,
                                                      Session_Id                SessionId,

                                                      OCPIResponse<Session>     Response,
                                                      TimeSpan                  Runtime,
                                                      CancellationToken         CancellationToken);

    #endregion

    #region OnPutSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a put session request will be send.
    /// </summary>
    public delegate Task OnPutSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,
                                                     TimeSpan                  RequestTimeout,

                                                     Session                   Session,

                                                     CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a put session request had been received.
    /// </summary>
    public delegate Task OnPutSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,
                                                      TimeSpan                  RequestTimeout,

                                                      Session                   Session,

                                                      OCPIResponse<Session>     Response,
                                                      TimeSpan                  Runtime,
                                                      CancellationToken         CancellationToken);

    #endregion

    #region OnPatchSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch session request will be send.
    /// </summary>
    public delegate Task OnPatchSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Session_Id                SessionId,
                                                       JObject                   SessionPatch,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a patch session request had been received.
    /// </summary>
    public delegate Task OnPatchSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Session_Id                SessionId,
                                                        JObject                   SessionPatch,

                                                        OCPIResponse<Session>     Response,
                                                        TimeSpan                  Runtime,
                                                        CancellationToken         CancellationToken);

    #endregion

    #region OnDeleteSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a delete session request will be send.
    /// </summary>
    public delegate Task OnDeleteSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Session_Id                SessionId,

                                                        CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a delete session request had been received.
    /// </summary>
    public delegate Task OnDeleteSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                         CPO2EMSP_HTTPClient       Sender,
                                                         EventTracking_Id          EventTrackingId,
                                                         Request_Id                RequestId,
                                                         Correlation_Id            CorrelationId,
                                                         TimeSpan                  RequestTimeout,

                                                         CountryCode               CountryCode,
                                                         Party_Id                  PartyId,
                                                         Session_Id                SessionId,

                                                         OCPIResponse<Session>     Response,
                                                         TimeSpan                  Runtime,
                                                         CancellationToken         CancellationToken);

    #endregion


    #region OnGetCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a get charge detail record request will be send.
    /// </summary>
    public delegate Task OnGetCDRRequestDelegate(DateTimeOffset            LogTimestamp,
                                                 CPO2EMSP_HTTPClient       Sender,
                                                 EventTracking_Id          EventTrackingId,
                                                 Request_Id                RequestId,
                                                 Correlation_Id            CorrelationId,
                                                 TimeSpan                  RequestTimeout,

                                                 CDR_Id                    CDRId,

                                                 CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a get charge detail record request had been received.
    /// </summary>
    public delegate Task OnGetCDRResponseDelegate(DateTimeOffset            LogTimestamp,
                                                  CPO2EMSP_HTTPClient       Sender,
                                                  EventTracking_Id          EventTrackingId,
                                                  Request_Id                RequestId,
                                                  Correlation_Id            CorrelationId,
                                                  TimeSpan                  RequestTimeout,

                                                  CDR_Id                    CDRId,

                                                  OCPIResponse<CDR>         Response,
                                                  TimeSpan                  Runtime,
                                                  CancellationToken         CancellationToken);

    #endregion

    #region OnPostCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a post charge detail record request will be send.
    /// </summary>
    public delegate Task OnPostCDRRequestDelegate(DateTimeOffset                  LogTimestamp,
                                                  CPO2EMSP_HTTPClient             Sender,
                                                  EventTracking_Id                EventTrackingId,
                                                  Request_Id                      RequestId,
                                                  Correlation_Id                  CorrelationId,
                                                  TimeSpan                        RequestTimeout,

                                                  CDR                             CDR,

                                                  CancellationToken               CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a post charge detail record request had been received.
    /// </summary>
    public delegate Task OnPostCDRResponseDelegate(DateTimeOffset                 LogTimestamp,
                                                   CPO2EMSP_HTTPClient            Sender,
                                                   EventTracking_Id               EventTrackingId,
                                                   Request_Id                     RequestId,
                                                   Correlation_Id                 CorrelationId,
                                                   TimeSpan                       RequestTimeout,

                                                   CDR                            CDR,

                                                   OCPIResponse<Hermod.Location>  Response,
                                                   TimeSpan                       Runtime,
                                                   CancellationToken              CancellationToken);

    #endregion


    #region OnGetTokensRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tokens request will be send.
    /// </summary>
    public delegate Task OnGetTokensRequestDelegate(DateTimeOffset                     LogTimestamp,
                                                    CPO2EMSP_HTTPClient                Sender,
                                                    EventTracking_Id                   EventTrackingId,
                                                    Request_Id                         RequestId,
                                                    Correlation_Id                     CorrelationId,
                                                    TimeSpan                           RequestTimeout,

                                                    UInt64?                            Offset,
                                                    UInt64?                            Limit,

                                                    CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a get tokens request had been received.
    /// </summary>
    public delegate Task OnGetTokensResponseDelegate(DateTimeOffset                     LogTimestamp,
                                                     CPO2EMSP_HTTPClient                Sender,
                                                     EventTracking_Id                   EventTrackingId,
                                                     Request_Id                         RequestId,
                                                     Correlation_Id                     CorrelationId,
                                                     TimeSpan                           RequestTimeout,

                                                     UInt64?                            Offset,
                                                     UInt64?                            Limit,

                                                     OCPIResponse<IEnumerable<Token>>   Response,
                                                     TimeSpan                           Runtime,
                                                     CancellationToken                  CancellationToken);

    #endregion

    #region OnPostTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a post token request will be send.
    /// </summary>
    public delegate Task OnPostTokenRequestDelegate(DateTimeOffset                     LogTimestamp,
                                                    CPO2EMSP_HTTPClient                Sender,
                                                    EventTracking_Id                   EventTrackingId,
                                                    Request_Id                         RequestId,
                                                    Correlation_Id                     CorrelationId,
                                                    TimeSpan                           RequestTimeout,

                                                    Token_Id                           TokenId,
                                                    TokenType?                         TokenType,
                                                    LocationReference?                 LocationReference,

                                                    CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a post token request had been received.
    /// </summary>
    public delegate Task OnPostTokenResponseDelegate(DateTimeOffset                    LogTimestamp,
                                                     CPO2EMSP_HTTPClient               Sender,
                                                     EventTracking_Id                  EventTrackingId,
                                                     Request_Id                        RequestId,
                                                     Correlation_Id                    CorrelationId,
                                                     TimeSpan                          RequestTimeout,

                                                     Token_Id                          TokenId,
                                                     TokenType?                        TokenType,
                                                     LocationReference?                LocationReference,

                                                     OCPIResponse<AuthorizationInfo>   Response,
                                                     TimeSpan                          Runtime,
                                                     CancellationToken                 CancellationToken);

    #endregion


}
