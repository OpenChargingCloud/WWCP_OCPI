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

namespace cloud.charging.open.protocols.OCPIv2_2_1.CPO.HTTP
{

    #region OnGetLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetLocation request will be send.
    /// </summary>
    public delegate Task OnGetLocationRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      RemoteParty_Id            RemotePartyId,
                                                      Party_Idv3?               From,
                                                      Party_Idv3?               To,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,
                                                      TimeSpan                  RequestTimeout,

                                                      CountryCode               CountryCode,
                                                      Party_Id                  PartyId,
                                                      Location_Id               LocationId,

                                                      CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetLocation request had been received.
    /// </summary>
    public delegate Task OnGetLocationResponseDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       RemoteParty_Id            RemotePartyId,
                                                       Party_Idv3?               From,
                                                       Party_Idv3?               To,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Location_Id               LocationId,

                                                       OCPIResponse<Location>    Response,
                                                       TimeSpan                  Runtime,
                                                       CancellationToken         CancellationToken);

    #endregion

    #region OnPutLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a PutLocation request will be send.
    /// </summary>
    public delegate Task OnPutLocationRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      RemoteParty_Id            RemotePartyId,
                                                      Party_Idv3?               From,
                                                      Party_Idv3?               To,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,
                                                      TimeSpan                  RequestTimeout,

                                                      Location                  Location,

                                                      CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PutLocation request had been received.
    /// </summary>
    public delegate Task OnPutLocationResponseDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       RemoteParty_Id            RemotePartyId,
                                                       Party_Idv3?               From,
                                                       Party_Idv3?               To,
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
    /// A delegate called whenever a PatchLocation request will be send.
    /// </summary>
    public delegate Task OnPatchLocationRequestDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        RemoteParty_Id            RemotePartyId,
                                                        Party_Idv3?               From,
                                                        Party_Idv3?               To,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        Location_Id               LocationId,
                                                        JObject                   LocationPatch,

                                                        CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PatchLocation request had been received.
    /// </summary>
    public delegate Task OnPatchLocationResponseDelegate(DateTimeOffset            LogTimestamp,
                                                         CPO2EMSP_HTTPClient       Sender,
                                                         EventTracking_Id          EventTrackingId,
                                                         RemoteParty_Id            RemotePartyId,
                                                         Party_Idv3?               From,
                                                         Party_Idv3?               To,
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
    /// A delegate called whenever a GetEVSE request will be send.
    /// </summary>
    public delegate Task OnGetEVSERequestDelegate(DateTimeOffset            LogTimestamp,
                                                  CPO2EMSP_HTTPClient       Sender,
                                                  EventTracking_Id          EventTrackingId,
                                                  RemoteParty_Id            RemotePartyId,
                                                  Party_Idv3?               From,
                                                  Party_Idv3?               To,
                                                  Request_Id                RequestId,
                                                  Correlation_Id            CorrelationId,
                                                  TimeSpan                  RequestTimeout,

                                                  Location_Id               LocationId,
                                                  EVSE_UId                  EVSEUId,

                                                  CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetEVSE request had been received.
    /// </summary>
    public delegate Task OnGetEVSEResponseDelegate(DateTimeOffset            LogTimestamp,
                                                   CPO2EMSP_HTTPClient       Sender,
                                                   EventTracking_Id          EventTrackingId,
                                                   RemoteParty_Id            RemotePartyId,
                                                   Party_Idv3?               From,
                                                   Party_Idv3?               To,
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
    /// A delegate called whenever a PutEVSE request will be send.
    /// </summary>
    public delegate Task OnPutEVSERequestDelegate(DateTimeOffset            LogTimestamp,
                                                  CPO2EMSP_HTTPClient       Sender,
                                                  EventTracking_Id          EventTrackingId,
                                                  RemoteParty_Id            RemotePartyId,
                                                  Party_Idv3?               From,
                                                  Party_Idv3?               To,
                                                  Request_Id                RequestId,
                                                  Correlation_Id            CorrelationId,
                                                  TimeSpan                  RequestTimeout,

                                                  EVSE                      EVSE,
                                                  CountryCode               CountryCode,
                                                  Party_Id                  PartyId,
                                                  Location_Id               LocationId,

                                                  CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PutEVSE request had been received.
    /// </summary>
    public delegate Task OnPutEVSEResponseDelegate(DateTimeOffset            LogTimestamp,
                                                   CPO2EMSP_HTTPClient       Sender,
                                                   EventTracking_Id          EventTrackingId,
                                                   RemoteParty_Id            RemotePartyId,
                                                   Party_Idv3?               From,
                                                   Party_Idv3?               To,
                                                   Request_Id                RequestId,
                                                   Correlation_Id            CorrelationId,
                                                   TimeSpan                  RequestTimeout,

                                                   EVSE                      EVSE,
                                                   CountryCode               CountryCode,
                                                   Party_Id                  PartyId,
                                                   Location_Id               LocationId,

                                                   OCPIResponse<EVSE>        Response,
                                                   TimeSpan                  Runtime,
                                                   CancellationToken         CancellationToken);

    #endregion

    #region OnPatchEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a PatchEVSE request will be send.
    /// </summary>
    public delegate Task OnPatchEVSERequestDelegate(DateTimeOffset            LogTimestamp,
                                                    CPO2EMSP_HTTPClient       Sender,
                                                    EventTracking_Id          EventTrackingId,
                                                    RemoteParty_Id            RemotePartyId,
                                                    Party_Idv3?               From,
                                                    Party_Idv3?               To,
                                                    Request_Id                RequestId,
                                                    Correlation_Id            CorrelationId,
                                                    TimeSpan                  RequestTimeout,

                                                    CountryCode               CountryCode,
                                                    Party_Id                  PartyId,
                                                    Location_Id               LocationId,
                                                    EVSE_UId                  EVSEUId,
                                                    JObject                   EVSEPatch,

                                                    CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PatchEVSE request had been received.
    /// </summary>
    public delegate Task OnPatchEVSEResponseDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     RemoteParty_Id            RemotePartyId,
                                                     Party_Idv3?               From,
                                                     Party_Idv3?               To,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,
                                                     TimeSpan                  RequestTimeout,

                                                     CountryCode               CountryCode,
                                                     Party_Id                  PartyId,
                                                     Location_Id               LocationId,
                                                     EVSE_UId                  EVSEUId,
                                                     JObject                   EVSEPatch,

                                                     OCPIResponse<EVSE>        Response,
                                                     TimeSpan                  Runtime,
                                                     CancellationToken         CancellationToken);

    #endregion


    #region OnPostEVSEStatusRequest/-Response

    /// <summary>
    /// A delegate called whenever a PostEVSEStatus request will be send.
    /// </summary>
    public delegate Task OnPostEVSEStatusRequestDelegate(DateTimeOffset            LogTimestamp,
                                                         CPO2EMSP_HTTPClient       Sender,
                                                         EventTracking_Id          EventTrackingId,
                                                         RemoteParty_Id            RemotePartyId,
                                                         Party_Idv3?               From,
                                                         Party_Idv3?               To,
                                                         Request_Id                RequestId,
                                                         Correlation_Id            CorrelationId,
                                                         TimeSpan                  RequestTimeout,

                                                         CountryCode               CountryCode,
                                                         Party_Id                  PartyId,
                                                         Location_Id               LocationId,
                                                         EVSE_UId                  EVSEUId,
                                                         StatusType                EVSEStatus,

                                                         CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PostEVSEStatus request had been received.
    /// </summary>
    public delegate Task OnPostEVSEStatusResponseDelegate(DateTimeOffset            LogTimestamp,
                                                          CPO2EMSP_HTTPClient       Sender,
                                                          EventTracking_Id          EventTrackingId,
                                                          RemoteParty_Id            RemotePartyId,
                                                          Party_Idv3?               From,
                                                          Party_Idv3?               To,
                                                          Request_Id                RequestId,
                                                          Correlation_Id            CorrelationId,
                                                          TimeSpan                  RequestTimeout,

                                                          CountryCode               CountryCode,
                                                          Party_Id                  PartyId,
                                                          Location_Id               LocationId,
                                                          EVSE_UId                  EVSEUId,
                                                          StatusType                EVSEStatus,

                                                          OCPIResponse<EVSE>        Response,
                                                          TimeSpan                  Runtime,
                                                          CancellationToken         CancellationToken);

    #endregion


    #region OnGetConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a GetConnector request will be send.
    /// </summary>
    public delegate Task OnGetConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       RemoteParty_Id            RemotePartyId,
                                                       Party_Idv3?               From,
                                                       Party_Idv3?               To,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Location_Id               LocationId,
                                                       EVSE_UId                  EVSEUId,
                                                       Connector_Id              ConnectorId,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetConnector request had been received.
    /// </summary>
    public delegate Task OnGetConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        RemoteParty_Id            RemotePartyId,
                                                        Party_Idv3?               From,
                                                        Party_Idv3?               To,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Location_Id               LocationId,
                                                        EVSE_UId                  EVSEUId,
                                                        Connector_Id              ConnectorId,

                                                        OCPIResponse<Connector>   Response,
                                                        TimeSpan                  Runtime,
                                                        CancellationToken         CancellationToken);

    #endregion

    #region OnPutConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a PutConnector request will be send.
    /// </summary>
    public delegate Task OnPutConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       RemoteParty_Id            RemotePartyId,
                                                       Party_Idv3?               From,
                                                       Party_Idv3?               To,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       Connector                 Connector,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PutConnector request had been received.
    /// </summary>
    public delegate Task OnPutConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        RemoteParty_Id            RemotePartyId,
                                                        Party_Idv3?               From,
                                                        Party_Idv3?               To,
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
    /// A delegate called whenever a PatchConnector request will be send.
    /// </summary>
    public delegate Task OnPatchConnectorRequestDelegate(DateTimeOffset            LogTimestamp,
                                                         CPO2EMSP_HTTPClient       Sender,
                                                         EventTracking_Id          EventTrackingId,
                                                         RemoteParty_Id            RemotePartyId,
                                                         Party_Idv3?               From,
                                                         Party_Idv3?               To,
                                                         Request_Id                RequestId,
                                                         Correlation_Id            CorrelationId,
                                                         TimeSpan                  RequestTimeout,

                                                         CountryCode               CountryCode,
                                                         Party_Id                  PartyId,
                                                         Location_Id               LocationId,
                                                         EVSE_UId                  EVSEUId,
                                                         Connector_Id              ConnectorId,
                                                         JObject                   ConnectorPatch,

                                                         CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PatchConnector request had been received.
    /// </summary>
    public delegate Task OnPatchConnectorResponseDelegate(DateTimeOffset            LogTimestamp,
                                                          CPO2EMSP_HTTPClient       Sender,
                                                          EventTracking_Id          EventTrackingId,
                                                          RemoteParty_Id            RemotePartyId,
                                                          Party_Idv3?               From,
                                                          Party_Idv3?               To,
                                                          Request_Id                RequestId,
                                                          Correlation_Id            CorrelationId,
                                                          TimeSpan                  RequestTimeout,

                                                          CountryCode               CountryCode,
                                                          Party_Id                  PartyId,
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
    /// A delegate called whenever a GetTariff request will be send.
    /// </summary>
    public delegate Task OnGetTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                    CPO2EMSP_HTTPClient       Sender,
                                                    EventTracking_Id          EventTrackingId,
                                                    RemoteParty_Id            RemotePartyId,
                                                    Party_Idv3?               From,
                                                    Party_Idv3?               To,
                                                    Request_Id                RequestId,
                                                    Correlation_Id            CorrelationId,
                                                    TimeSpan                  RequestTimeout,

                                                    CountryCode               CountryCode,
                                                    Party_Id                  PartyId,
                                                    Tariff_Id                 TariffId,

                                                    CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetTariff request had been received.
    /// </summary>
    public delegate Task OnGetTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     RemoteParty_Id            RemotePartyId,
                                                     Party_Idv3?               From,
                                                     Party_Idv3?               To,
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
    /// A delegate called whenever a PutTariff request will be send.
    /// </summary>
    public delegate Task OnPutTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                    CPO2EMSP_HTTPClient       Sender,
                                                    EventTracking_Id          EventTrackingId,
                                                    RemoteParty_Id            RemotePartyId,
                                                    Party_Idv3?               From,
                                                    Party_Idv3?               To,
                                                    Request_Id                RequestId,
                                                    Correlation_Id            CorrelationId,
                                                    TimeSpan                  RequestTimeout,

                                                    Tariff                    Tariff,

                                                    CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PutTariff request had been received.
    /// </summary>
    public delegate Task OnPutTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     RemoteParty_Id            RemotePartyId,
                                                     Party_Idv3?               From,
                                                     Party_Idv3?               To,
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
    /// A delegate called whenever a PatchTariff request will be send.
    /// </summary>
    public delegate Task OnPatchTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      RemoteParty_Id            RemotePartyId,
                                                      Party_Idv3?               From,
                                                      Party_Idv3?               To,
                                                      Request_Id                RequestId,
                                                      Correlation_Id            CorrelationId,
                                                      TimeSpan                  RequestTimeout,

                                                      CountryCode               CountryCode,
                                                      Party_Id                  PartyId,
                                                      Tariff_Id                 TariffId,
                                                      JObject                   TariffPatch,

                                                      CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PatchTariff request had been received.
    /// </summary>
    public delegate Task OnPatchTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       RemoteParty_Id            RemotePartyId,
                                                       Party_Idv3?               From,
                                                       Party_Idv3?               To,
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
    /// A delegate called whenever a DeleteTariff request will be send.
    /// </summary>
    public delegate Task OnDeleteTariffRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       RemoteParty_Id            RemotePartyId,
                                                       Party_Idv3?               From,
                                                       Party_Idv3?               To,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Tariff_Id                 TariffId,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a DeleteTariff request had been received.
    /// </summary>
    public delegate Task OnDeleteTariffResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        RemoteParty_Id            RemotePartyId,
                                                        Party_Idv3?               From,
                                                        Party_Idv3?               To,
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
    /// A delegate called whenever a GetSession request will be send.
    /// </summary>
    public delegate Task OnGetSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     RemoteParty_Id            RemotePartyId,
                                                     Party_Idv3?               From,
                                                     Party_Idv3?               To,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,
                                                     TimeSpan                  RequestTimeout,

                                                     CountryCode               CountryCode,
                                                     Party_Id                  PartyId,
                                                     Session_Id                SessionId,

                                                     CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetSession request had been received.
    /// </summary>
    public delegate Task OnGetSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      RemoteParty_Id            RemotePartyId,
                                                      Party_Idv3?               From,
                                                      Party_Idv3?               To,
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
    /// A delegate called whenever a PutSession request will be send.
    /// </summary>
    public delegate Task OnPutSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                     CPO2EMSP_HTTPClient       Sender,
                                                     EventTracking_Id          EventTrackingId,
                                                     RemoteParty_Id            RemotePartyId,
                                                     Party_Idv3?               From,
                                                     Party_Idv3?               To,
                                                     Request_Id                RequestId,
                                                     Correlation_Id            CorrelationId,
                                                     TimeSpan                  RequestTimeout,

                                                     Session                   Session,

                                                     CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PutSession request had been received.
    /// </summary>
    public delegate Task OnPutSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                      CPO2EMSP_HTTPClient       Sender,
                                                      EventTracking_Id          EventTrackingId,
                                                      RemoteParty_Id            RemotePartyId,
                                                      Party_Idv3?               From,
                                                      Party_Idv3?               To,
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
    /// A delegate called whenever a PatchSession request will be send.
    /// </summary>
    public delegate Task OnPatchSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                       CPO2EMSP_HTTPClient       Sender,
                                                       EventTracking_Id          EventTrackingId,
                                                       RemoteParty_Id            RemotePartyId,
                                                       Party_Idv3?               From,
                                                       Party_Idv3?               To,
                                                       Request_Id                RequestId,
                                                       Correlation_Id            CorrelationId,
                                                       TimeSpan                  RequestTimeout,

                                                       CountryCode               CountryCode,
                                                       Party_Id                  PartyId,
                                                       Session_Id                SessionId,
                                                       JObject                   SessionPatch,

                                                       CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PatchSession request had been received.
    /// </summary>
    public delegate Task OnPatchSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        RemoteParty_Id            RemotePartyId,
                                                        Party_Idv3?               From,
                                                        Party_Idv3?               To,
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
    /// A delegate called whenever a DeleteSession request will be send.
    /// </summary>
    public delegate Task OnDeleteSessionRequestDelegate(DateTimeOffset            LogTimestamp,
                                                        CPO2EMSP_HTTPClient       Sender,
                                                        EventTracking_Id          EventTrackingId,
                                                        RemoteParty_Id            RemotePartyId,
                                                        Party_Idv3?               From,
                                                        Party_Idv3?               To,
                                                        Request_Id                RequestId,
                                                        Correlation_Id            CorrelationId,
                                                        TimeSpan                  RequestTimeout,

                                                        CountryCode               CountryCode,
                                                        Party_Id                  PartyId,
                                                        Session_Id                SessionId,

                                                        CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a DeleteSession request had been received.
    /// </summary>
    public delegate Task OnDeleteSessionResponseDelegate(DateTimeOffset            LogTimestamp,
                                                         CPO2EMSP_HTTPClient       Sender,
                                                         EventTracking_Id          EventTrackingId,
                                                         RemoteParty_Id            RemotePartyId,
                                                         Party_Idv3?               From,
                                                         Party_Idv3?               To,
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
    /// A delegate called whenever a GetCDR request will be send.
    /// </summary>
    public delegate Task OnGetCDRRequestDelegate(DateTimeOffset            LogTimestamp,
                                                 CPO2EMSP_HTTPClient       Sender,
                                                 EventTracking_Id          EventTrackingId,
                                                 RemoteParty_Id            RemotePartyId,
                                                 Party_Idv3?               From,
                                                 Party_Idv3?               To,
                                                 Request_Id                RequestId,
                                                 Correlation_Id            CorrelationId,
                                                 TimeSpan                  RequestTimeout,

                                                 CountryCode               CountryCode,
                                                 Party_Id                  PartyId,
                                                 CDR_Id                    CDRId,

                                                 CancellationToken         CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetCDR record request had been received.
    /// </summary>
    public delegate Task OnGetCDRResponseDelegate(DateTimeOffset            LogTimestamp,
                                                  CPO2EMSP_HTTPClient       Sender,
                                                  EventTracking_Id          EventTrackingId,
                                                  RemoteParty_Id            RemotePartyId,
                                                  Party_Idv3?               From,
                                                  Party_Idv3?               To,
                                                  Request_Id                RequestId,
                                                  Correlation_Id            CorrelationId,
                                                  TimeSpan                  RequestTimeout,

                                                  CountryCode               CountryCode,
                                                  Party_Id                  PartyId,
                                                  CDR_Id                    CDRId,

                                                  OCPIResponse<CDR>         Response,
                                                  TimeSpan                  Runtime,
                                                  CancellationToken         CancellationToken);

    #endregion

    #region OnPostCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a PostCDR request will be send.
    /// </summary>
    public delegate Task OnPostCDRRequestDelegate(DateTimeOffset                  LogTimestamp,
                                                  CPO2EMSP_HTTPClient             Sender,
                                                  EventTracking_Id                EventTrackingId,
                                                  RemoteParty_Id                  RemotePartyId,
                                                  Party_Idv3?                     From,
                                                  Party_Idv3?                     To,
                                                  Request_Id                      RequestId,
                                                  Correlation_Id                  CorrelationId,
                                                  TimeSpan                        RequestTimeout,

                                                  CDR                             CDR,

                                                  CancellationToken               CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PostCDR request had been received.
    /// </summary>
    public delegate Task OnPostCDRResponseDelegate(DateTimeOffset                 LogTimestamp,
                                                   CPO2EMSP_HTTPClient            Sender,
                                                   EventTracking_Id               EventTrackingId,
                                                   RemoteParty_Id                 RemotePartyId,
                                                   Party_Idv3?                    From,
                                                   Party_Idv3?                    To,
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
    /// A delegate called whenever a GetTokens request will be send.
    /// </summary>
    public delegate Task OnGetTokensRequestDelegate(DateTimeOffset                     LogTimestamp,
                                                    CPO2EMSP_HTTPClient                Sender,
                                                    EventTracking_Id                   EventTrackingId,
                                                    RemoteParty_Id                     RemotePartyId,
                                                    Party_Idv3?                        From,
                                                    Party_Idv3?                        To,
                                                    Request_Id                         RequestId,
                                                    Correlation_Id                     CorrelationId,
                                                    TimeSpan                           RequestTimeout,

                                                    UInt64?                            Offset,
                                                    UInt64?                            Limit,

                                                    CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a GetTokens request had been received.
    /// </summary>
    public delegate Task OnGetTokensResponseDelegate(DateTimeOffset                     LogTimestamp,
                                                     CPO2EMSP_HTTPClient                Sender,
                                                     EventTracking_Id                   EventTrackingId,
                                                     RemoteParty_Id                     RemotePartyId,
                                                     Party_Idv3?                        From,
                                                     Party_Idv3?                        To,
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
    /// A delegate called whenever a PostToken request will be send.
    /// </summary>
    public delegate Task OnPostTokenRequestDelegate(DateTimeOffset                     LogTimestamp,
                                                    CPO2EMSP_HTTPClient                Sender,
                                                    EventTracking_Id                   EventTrackingId,
                                                    RemoteParty_Id                     RemotePartyId,
                                                    Party_Idv3?                        From,
                                                    Party_Idv3?                        To,
                                                    Request_Id                         RequestId,
                                                    Correlation_Id                     CorrelationId,
                                                    TimeSpan                           RequestTimeout,

                                                    Token_Id                           TokenId,
                                                    TokenType?                         TokenType,
                                                    LocationReference?                 LocationReference,

                                                    CancellationToken                  CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a PostToken request had been received.
    /// </summary>
    public delegate Task OnPostTokenResponseDelegate(DateTimeOffset                    LogTimestamp,
                                                     CPO2EMSP_HTTPClient               Sender,
                                                     EventTracking_Id                  EventTrackingId,
                                                     RemoteParty_Id                    RemotePartyId,
                                                     Party_Idv3?                       From,
                                                     Party_Idv3?                       To,
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


    #region OnSetChargingProfileRequest/-Response

    /// <summary>
    /// A delegate called whenever a SetChargingProfile request will be send.
    /// </summary>
    public delegate Task OnSetChargingProfileRequestDelegate(DateTimeOffset                          LogTimestamp,
                                                             CPO2EMSP_HTTPClient                     Sender,
                                                             EventTracking_Id                        EventTrackingId,
                                                             RemoteParty_Id                          RemotePartyId,
                                                             Party_Idv3?                             From,
                                                             Party_Idv3?                             To,
                                                             Request_Id                              RequestId,
                                                             Correlation_Id                          CorrelationId,
                                                             TimeSpan                                RequestTimeout,

                                                             Session_Id                              SessionId,
                                                             ChargingProfile                         ChargingProfile,

                                                             CancellationToken                       CancellationToken);

    /// <summary>
    /// A delegate called whenever a response to a SetChargingProfile request had been received.
    /// </summary>
    public delegate Task OnSetChargingProfileResponseDelegate(DateTimeOffset                          LogTimestamp,
                                                              CPO2EMSP_HTTPClient                     Sender,
                                                              EventTracking_Id                        EventTrackingId,
                                                              RemoteParty_Id                          RemotePartyId,
                                                              Party_Idv3?                             From,
                                                              Party_Idv3?                             To,
                                                              Request_Id                              RequestId,
                                                              Correlation_Id                          CorrelationId,
                                                              TimeSpan                                RequestTimeout,

                                                              Session_Id                              SessionId,
                                                              ChargingProfile                         ChargingProfile,

                                                              OCPIResponse<ChargingProfileResponse>   Response,
                                                              TimeSpan                                Runtime,
                                                              CancellationToken                       CancellationToken);

    #endregion


}
