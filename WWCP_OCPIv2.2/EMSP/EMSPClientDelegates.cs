/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    #region OnGetLocationsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get locations request will be send.
    /// </summary>
    public delegate Task OnGetLocationsRequestDelegate (DateTime                                    LogTimestamp,
                                                        DateTime                                    RequestTimestamp,
                                                        CommonClient                                Sender,
                                                        String                                      SenderId,
                                                        EventTracking_Id                            EventTrackingId,

                                                        //Partner_Id                                  PartnerId,
                                                        //Operator_Id                                 OperatorId,
                                                        //ChargingPool_Id                             ChargingPoolId,
                                                        //DateTime                                    StatusEventDate,
                                                        //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                        //Transaction_Id?                             TransactionId,
                                                        //DateTime?                                   AvailabilityStatusUntil,
                                                        //String                                      AvailabilityStatusComment,

                                                        TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get locations request had been received.
    /// </summary>
    public delegate Task OnGetLocationsResponseDelegate(DateTime                                    LogTimestamp,
                                                        DateTime                                    RequestTimestamp,
                                                        CommonClient                                Sender,
                                                        String                                      SenderId,
                                                        EventTracking_Id                            EventTrackingId,

                                                        //Partner_Id                                  PartnerId,
                                                        //Operator_Id                                 OperatorId,
                                                        //ChargingPool_Id                             ChargingPoolId,
                                                        //DateTime                                    StatusEventDate,
                                                        //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                        //Transaction_Id?                             TransactionId,
                                                        //DateTime?                                   AvailabilityStatusUntil,
                                                        //String                                      AvailabilityStatusComment,

                                                        TimeSpan                                    RequestTimeout,
                                                        //SetChargingPoolAvailabilityStatusResponse   Result,
                                                        TimeSpan                                    Duration);

    #endregion

    #region OnGetLocationByIdRequest/-Response

    /// <summary>
    /// A delegate called whenever a get location by its identification request will be send.
    /// </summary>
    public delegate Task OnGetLocationByIdRequestDelegate (DateTime                                    LogTimestamp,
                                                           DateTime                                    RequestTimestamp,
                                                           CommonClient                                Sender,
                                                           String                                      SenderId,
                                                           EventTracking_Id                            EventTrackingId,

                                                           //Partner_Id                                  PartnerId,
                                                           Location_Id                                 LocationId,

                                                           TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get location by its identification request had been received.
    /// </summary>
    public delegate Task OnGetLocationByIdResponseDelegate(DateTime                                    LogTimestamp,
                                                           DateTime                                    RequestTimestamp,
                                                           CommonClient                                Sender,
                                                           String                                      SenderId,
                                                           EventTracking_Id                            EventTrackingId,

                                                           //Partner_Id                                  PartnerId,
                                                           Location_Id                                 LocationId,

                                                           TimeSpan                                    RequestTimeout,
                                                           //SetChargingPoolAvailabilityStatusResponse   Result,
                                                           TimeSpan                                    Duration);

    #endregion

    #region OnGetEVSEByUIdRequest/-Response

    /// <summary>
    /// A delegate called whenever a get EVSE by its identification request will be send.
    /// </summary>
    public delegate Task OnGetEVSEByUIdRequestDelegate (DateTime                                    LogTimestamp,
                                                        DateTime                                    RequestTimestamp,
                                                        CommonClient                                Sender,
                                                        String                                      SenderId,
                                                        EventTracking_Id                            EventTrackingId,

                                                        //Partner_Id                                  PartnerId,
                                                        EVSE_Id                                     EVSEId,

                                                        TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get EVSE by its identification request had been received.
    /// </summary>
    public delegate Task OnGetEVSEByUIdResponseDelegate(DateTime                                    LogTimestamp,
                                                        DateTime                                    RequestTimestamp,
                                                        CommonClient                                Sender,
                                                        String                                      SenderId,
                                                        EventTracking_Id                            EventTrackingId,

                                                        //Partner_Id                                  PartnerId,
                                                        EVSE_Id                                     EVSEId,

                                                        TimeSpan                                    RequestTimeout,
                                                        //SetChargingPoolAvailabilityStatusResponse   Result,
                                                        TimeSpan                                    Duration);

    #endregion

    #region OnGetConnectorByIdRequest/-Response

    /// <summary>
    /// A delegate called whenever a get connector by its identification request will be send.
    /// </summary>
    public delegate Task OnGetConnectorByIdRequestDelegate (DateTime                                    LogTimestamp,
                                                            DateTime                                    RequestTimestamp,
                                                            CommonClient                                Sender,
                                                            String                                      SenderId,
                                                            EventTracking_Id                            EventTrackingId,

                                                            //Partner_Id                                  PartnerId,
                                                            Connector_Id                                 ConnectorId,

                                                            TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get connector by its identification request had been received.
    /// </summary>
    public delegate Task OnGetConnectorByIdResponseDelegate(DateTime                                    LogTimestamp,
                                                            DateTime                                    RequestTimestamp,
                                                            CommonClient                                Sender,
                                                            String                                      SenderId,
                                                            EventTracking_Id                            EventTrackingId,

                                                            //Partner_Id                                  PartnerId,
                                                            Connector_Id                                 ConnectorId,

                                                            TimeSpan                                    RequestTimeout,
                                                            //SetChargingPoolAvailabilityStatusResponse   Result,
                                                            TimeSpan                                    Duration);

    #endregion


    #region OnGetTariffsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tariffs request will be send.
    /// </summary>
    public delegate Task OnGetTariffsRequestDelegate (DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get tariffs request had been received.
    /// </summary>
    public delegate Task OnGetTariffsResponseDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout,
                                                      //SetChargingPoolAvailabilityStatusResponse   Result,
                                                      TimeSpan                                    Duration);

    #endregion

    #region OnGetTariffByIdRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tariff by its identification request will be send.
    /// </summary>
    public delegate Task OnGetTariffByIdRequestDelegate (DateTime                                    LogTimestamp,
                                                         DateTime                                    RequestTimestamp,
                                                         CommonClient                                Sender,
                                                         String                                      SenderId,
                                                         EventTracking_Id                            EventTrackingId,

                                                         //Partner_Id                                  PartnerId,
                                                         Tariff_Id                                 TariffId,

                                                         TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get tariff by its identification request had been received.
    /// </summary>
    public delegate Task OnGetTariffByIdResponseDelegate(DateTime                                    LogTimestamp,
                                                         DateTime                                    RequestTimestamp,
                                                         CommonClient                                Sender,
                                                         String                                      SenderId,
                                                         EventTracking_Id                            EventTrackingId,

                                                         //Partner_Id                                  PartnerId,
                                                         Tariff_Id                                 TariffId,

                                                         TimeSpan                                    RequestTimeout,
                                                         //SetChargingPoolAvailabilityStatusResponse   Result,
                                                         TimeSpan                                    Duration);

    #endregion


    #region OnGetSessionsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get sessions request will be send.
    /// </summary>
    public delegate Task OnGetSessionsRequestDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get sessions request had been received.
    /// </summary>
    public delegate Task OnGetSessionsResponseDelegate(DateTime                                    LogTimestamp,
                                                       DateTime                                    RequestTimestamp,
                                                       CommonClient                                Sender,
                                                       String                                      SenderId,
                                                       EventTracking_Id                            EventTrackingId,

                                                       //Partner_Id                                  PartnerId,
                                                       //Operator_Id                                 OperatorId,
                                                       //ChargingPool_Id                             ChargingPoolId,
                                                       //DateTime                                    StatusEventDate,
                                                       //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                       //Transaction_Id?                             TransactionId,
                                                       //DateTime?                                   AvailabilityStatusUntil,
                                                       //String                                      AvailabilityStatusComment,

                                                       TimeSpan                                    RequestTimeout,
                                                       //SetChargingPoolAvailabilityStatusResponse   Result,
                                                       TimeSpan                                    Duration);

    #endregion

    #region OnGetSessionByIdRequest/-Response

    /// <summary>
    /// A delegate called whenever a get session by its identification request will be send.
    /// </summary>
    public delegate Task OnGetSessionByIdRequestDelegate (DateTime                                    LogTimestamp,
                                                          DateTime                                    RequestTimestamp,
                                                          CommonClient                                Sender,
                                                          String                                      SenderId,
                                                          EventTracking_Id                            EventTrackingId,

                                                          //Partner_Id                                  PartnerId,
                                                          Session_Id                                 SessionId,

                                                          TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get session by its identification request had been received.
    /// </summary>
    public delegate Task OnGetSessionByIdResponseDelegate(DateTime                                    LogTimestamp,
                                                          DateTime                                    RequestTimestamp,
                                                          CommonClient                                Sender,
                                                          String                                      SenderId,
                                                          EventTracking_Id                            EventTrackingId,

                                                          //Partner_Id                                  PartnerId,
                                                          Session_Id                                 SessionId,

                                                          TimeSpan                                    RequestTimeout,
                                                          //SetChargingPoolAvailabilityStatusResponse   Result,
                                                          TimeSpan                                    Duration);

    #endregion


    #region OnGetCDRsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get CDRs request will be send.
    /// </summary>
    public delegate Task OnGetCDRsRequestDelegate(DateTime                                    LogTimestamp,
                                                  DateTime                                    RequestTimestamp,
                                                  CommonClient                                Sender,
                                                  String                                      SenderId,
                                                  EventTracking_Id                            EventTrackingId,

                                                  //Partner_Id                                  PartnerId,
                                                  //Operator_Id                                 OperatorId,
                                                  //ChargingPool_Id                             ChargingPoolId,
                                                  //DateTime                                    StatusEventDate,
                                                  //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                  //Transaction_Id?                             TransactionId,
                                                  //DateTime?                                   AvailabilityStatusUntil,
                                                  //String                                      AvailabilityStatusComment,

                                                  TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get CDRs request had been received.
    /// </summary>
    public delegate Task OnGetCDRsResponseDelegate(DateTime                                    LogTimestamp,
                                                   DateTime                                    RequestTimestamp,
                                                   CommonClient                                Sender,
                                                   String                                      SenderId,
                                                   EventTracking_Id                            EventTrackingId,

                                                   //Partner_Id                                  PartnerId,
                                                   //Operator_Id                                 OperatorId,
                                                   //ChargingPool_Id                             ChargingPoolId,
                                                   //DateTime                                    StatusEventDate,
                                                   //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                   //Transaction_Id?                             TransactionId,
                                                   //DateTime?                                   AvailabilityStatusUntil,
                                                   //String                                      AvailabilityStatusComment,

                                                   TimeSpan                                    RequestTimeout,
                                                   //SetChargingPoolAvailabilityStatusResponse   Result,
                                                   TimeSpan                                    Duration);

    #endregion

    #region OnGetCDRByIdRequest/-Response

    /// <summary>
    /// A delegate called whenever a get CDR by its identification request will be send.
    /// </summary>
    public delegate Task OnGetCDRByIdRequestDelegate (DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      CDR_Id                                 CDRId,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get CDR by its identification request had been received.
    /// </summary>
    public delegate Task OnGetCDRByIdResponseDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      CDR_Id                                 CDRId,

                                                      TimeSpan                                    RequestTimeout,
                                                      //SetChargingPoolAvailabilityStatusResponse   Result,
                                                      TimeSpan                                    Duration);

    #endregion


    #region OnGetTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a get token request will be send.
    /// </summary>
    public delegate Task OnGetTokenRequestDelegate(DateTime                                    LogTimestamp,
                                                   DateTime                                    RequestTimestamp,
                                                   CommonClient                                Sender,
                                                   String                                      SenderId,
                                                   EventTracking_Id                            EventTrackingId,

                                                   //Partner_Id                                  PartnerId,
                                                   //Operator_Id                                 OperatorId,
                                                   //ChargingPool_Id                             ChargingPoolId,
                                                   //DateTime                                    StatusEventDate,
                                                   //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                   //Transaction_Id?                             TransactionId,
                                                   //DateTime?                                   AvailabilityStatusUntil,
                                                   //String                                      AvailabilityStatusComment,

                                                   TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a get token request had been received.
    /// </summary>
    public delegate Task OnGetTokenResponseDelegate(DateTime                                    LogTimestamp,
                                                    DateTime                                    RequestTimestamp,
                                                    CommonClient                                Sender,
                                                    String                                      SenderId,
                                                    EventTracking_Id                            EventTrackingId,

                                                    //Partner_Id                                  PartnerId,
                                                    //Operator_Id                                 OperatorId,
                                                    //ChargingPool_Id                             ChargingPoolId,
                                                    //DateTime                                    StatusEventDate,
                                                    //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                    //Transaction_Id?                             TransactionId,
                                                    //DateTime?                                   AvailabilityStatusUntil,
                                                    //String                                      AvailabilityStatusComment,

                                                    TimeSpan                                    RequestTimeout,
                                                    //SetChargingPoolAvailabilityStatusResponse   Result,
                                                    TimeSpan                                    Duration);

    #endregion

    #region OnPutTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a put token request will be send.
    /// </summary>
    public delegate Task OnPutTokenRequestDelegate(DateTime                                    LogTimestamp,
                                                   DateTime                                    RequestTimestamp,
                                                   CommonClient                                Sender,
                                                   String                                      SenderId,
                                                   EventTracking_Id                            EventTrackingId,

                                                   //Partner_Id                                  PartnerId,
                                                   //Operator_Id                                 OperatorId,
                                                   //ChargingPool_Id                             ChargingPoolId,
                                                   //DateTime                                    StatusEventDate,
                                                   //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                   //Transaction_Id?                             TransactionId,
                                                   //DateTime?                                   AvailabilityStatusUntil,
                                                   //String                                      AvailabilityStatusComment,

                                                   TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a put token request had been received.
    /// </summary>
    public delegate Task OnPutTokenResponseDelegate(DateTime                                    LogTimestamp,
                                                    DateTime                                    RequestTimestamp,
                                                    CommonClient                                Sender,
                                                    String                                      SenderId,
                                                    EventTracking_Id                            EventTrackingId,

                                                    //Partner_Id                                  PartnerId,
                                                    //Operator_Id                                 OperatorId,
                                                    //ChargingPool_Id                             ChargingPoolId,
                                                    //DateTime                                    StatusEventDate,
                                                    //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                    //Transaction_Id?                             TransactionId,
                                                    //DateTime?                                   AvailabilityStatusUntil,
                                                    //String                                      AvailabilityStatusComment,

                                                    TimeSpan                                    RequestTimeout,
                                                    //SetChargingPoolAvailabilityStatusResponse   Result,
                                                    TimeSpan                                    Duration);

    #endregion

    #region OnPatchTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch token request will be send.
    /// </summary>
    public delegate Task OnPatchTokenRequestDelegate(DateTime                                    LogTimestamp,
                                                     DateTime                                    RequestTimestamp,
                                                     CommonClient                                Sender,
                                                     String                                      SenderId,
                                                     EventTracking_Id                            EventTrackingId,

                                                     //Partner_Id                                  PartnerId,
                                                     //Operator_Id                                 OperatorId,
                                                     //ChargingPool_Id                             ChargingPoolId,
                                                     //DateTime                                    StatusEventDate,
                                                     //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                     //Transaction_Id?                             TransactionId,
                                                     //DateTime?                                   AvailabilityStatusUntil,
                                                     //String                                      AvailabilityStatusComment,

                                                     TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a patch token request had been received.
    /// </summary>
    public delegate Task OnPatchTokenResponseDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout,
                                                      //SetChargingPoolAvailabilityStatusResponse   Result,
                                                      TimeSpan                                    Duration);

    #endregion


    #region OnReserveNowRequest/-Response

    /// <summary>
    /// A delegate called whenever a reserve now command request will be send.
    /// </summary>
    public delegate Task OnReserveNowRequestDelegate (DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a reserve now command request had been received.
    /// </summary>
    public delegate Task OnReserveNowResponseDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout,
                                                      //SetChargingPoolAvailabilityStatusResponse   Result,
                                                      TimeSpan                                    Duration);

    #endregion

    #region OnCancelReservationRequest/-Response

    /// <summary>
    /// A delegate called whenever a cancel reservation command request will be send.
    /// </summary>
    public delegate Task OnCancelReservationRequestDelegate (DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a cancel reservation command request had been received.
    /// </summary>
    public delegate Task OnCancelReservationResponseDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout,
                                                      //SetChargingPoolAvailabilityStatusResponse   Result,
                                                      TimeSpan                                    Duration);

    #endregion

    #region OnStartSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a start session command request will be send.
    /// </summary>
    public delegate Task OnStartSessionRequestDelegate (DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a start session command request had been received.
    /// </summary>
    public delegate Task OnStartSessionResponseDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout,
                                                      //SetChargingPoolAvailabilityStatusResponse   Result,
                                                      TimeSpan                                    Duration);

    #endregion

    #region OnStopSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a stop session command request will be send.
    /// </summary>
    public delegate Task OnStopSessionRequestDelegate (DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to a stop session command request had been received.
    /// </summary>
    public delegate Task OnStopSessionResponseDelegate(DateTime                                    LogTimestamp,
                                                      DateTime                                    RequestTimestamp,
                                                      CommonClient                                Sender,
                                                      String                                      SenderId,
                                                      EventTracking_Id                            EventTrackingId,

                                                      //Partner_Id                                  PartnerId,
                                                      //Operator_Id                                 OperatorId,
                                                      //ChargingPool_Id                             ChargingPoolId,
                                                      //DateTime                                    StatusEventDate,
                                                      //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                      //Transaction_Id?                             TransactionId,
                                                      //DateTime?                                   AvailabilityStatusUntil,
                                                      //String                                      AvailabilityStatusComment,

                                                      TimeSpan                                    RequestTimeout,
                                                      //SetChargingPoolAvailabilityStatusResponse   Result,
                                                      TimeSpan                                    Duration);

    #endregion

    #region OnUnlockConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever an unlock connector command request will be send.
    /// </summary>
    public delegate Task OnUnlockConnectorRequestDelegate (DateTime                                    LogTimestamp,
                                                           DateTime                                    RequestTimestamp,
                                                           CommonClient                                Sender,
                                                           String                                      SenderId,
                                                           EventTracking_Id                            EventTrackingId,

                                                           //Partner_Id                                  PartnerId,
                                                           //Operator_Id                                 OperatorId,
                                                           //ChargingPool_Id                             ChargingPoolId,
                                                           //DateTime                                    StatusEventDate,
                                                           //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                           //Transaction_Id?                             TransactionId,
                                                           //DateTime?                                   AvailabilityStatusUntil,
                                                           //String                                      AvailabilityStatusComment,

                                                           TimeSpan                                    RequestTimeout);

    /// <summary>
    /// A delegate called whenever a response to an unlock connector command request had been received.
    /// </summary>
    public delegate Task OnUnlockConnectorResponseDelegate(DateTime                                    LogTimestamp,
                                                           DateTime                                    RequestTimestamp,
                                                           CommonClient                                Sender,
                                                           String                                      SenderId,
                                                           EventTracking_Id                            EventTrackingId,

                                                           //Partner_Id                                  PartnerId,
                                                           //Operator_Id                                 OperatorId,
                                                           //ChargingPool_Id                             ChargingPoolId,
                                                           //DateTime                                    StatusEventDate,
                                                           //ChargingPoolAvailabilityStatusTypes         AvailabilityStatus,
                                                           //Transaction_Id?                             TransactionId,
                                                           //DateTime?                                   AvailabilityStatusUntil,
                                                           //String                                      AvailabilityStatusComment,

                                                           TimeSpan                                    RequestTimeout,
                                                           //SetChargingPoolAvailabilityStatusResponse   Result,
                                                           TimeSpan                                    Duration);

    #endregion

}
