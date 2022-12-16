﻿/*
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

using System;
using System.Linq;
using System.Threading;
using System.Net.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using System.Security.Authentication;

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


    /// <summary>
    /// The OCPI EMSP client.
    /// </summary>
    public partial class EMSPClient : CommonClient
    {

        public class EMSPCounters
        {

            public APICounterValues  GetLocations    { get; }

            public EMSPCounters(APICounterValues? GetLocations = null)
            {

                this.GetLocations = GetLocations ?? new APICounterValues();

            }

            public JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("GetLocations", GetLocations.ToJSON())
                   );

        }


        #region Properties

        /// <summary>
        /// EMSP client event counters.
        /// </summary>
        public new EMSPCounters  Counters      { get; }


        /// <summary>
        /// The EMSP client (HTTP client) logger.
        /// </summary>
        public new Logger        HTTPLogger    { get; }

        #endregion

        #region Events

        #region OnGetLocationsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all locations will be send.
        /// </summary>
        public event OnGetLocationsRequestDelegate   OnGetLocationsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all locations will be send.
        /// </summary>
        public event ClientRequestLogHandler         OnGetLocationsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all locations HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnGetLocationsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all locations request had been received.
        /// </summary>
        public event OnGetLocationsResponseDelegate  OnGetLocationsResponse;

        #endregion

        #region OnGetLocationByIdRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a location by its identification will be send.
        /// </summary>
        public event OnGetLocationByIdRequestDelegate   OnGetLocationByIdRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a location by its identification will be send.
        /// </summary>
        public event ClientRequestLogHandler            OnGetLocationByIdHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a location by its identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler           OnGetLocationByIdHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a location by its identification request had been received.
        /// </summary>
        public event OnGetLocationByIdResponseDelegate  OnGetLocationByIdResponse;

        #endregion

        #region OnGetEVSEByUIdRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a EVSE by its identification will be send.
        /// </summary>
        public event OnGetEVSEByUIdRequestDelegate   OnGetEVSEByUIdRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a EVSE by its identification will be send.
        /// </summary>
        public event ClientRequestLogHandler         OnGetEVSEByUIdHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a EVSE by its identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnGetEVSEByUIdHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a EVSE by its identification request had been received.
        /// </summary>
        public event OnGetEVSEByUIdResponseDelegate  OnGetEVSEByUIdResponse;

        #endregion

        #region OnGetConnectorByIdRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a connector by its identification will be send.
        /// </summary>
        public event OnGetConnectorByIdRequestDelegate   OnGetConnectorByIdRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a connector by its identification will be send.
        /// </summary>
        public event ClientRequestLogHandler             OnGetConnectorByIdHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a connector by its identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler            OnGetConnectorByIdHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a connector by its identification request had been received.
        /// </summary>
        public event OnGetConnectorByIdResponseDelegate  OnGetConnectorByIdResponse;

        #endregion


        #region OnGetTariffsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all tariffs will be send.
        /// </summary>
        public event OnGetTariffsRequestDelegate   OnGetTariffsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all tariffs will be send.
        /// </summary>
        public event ClientRequestLogHandler       OnGetTariffsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all tariffs HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler      OnGetTariffsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all tariffs request had been received.
        /// </summary>
        public event OnGetTariffsResponseDelegate  OnGetTariffsResponse;

        #endregion

        #region OnGetTariffByIdRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a tariff by it identification will be send.
        /// </summary>
        public event OnGetTariffsRequestDelegate   OnGetTariffByIdRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a tariff by it identification will be send.
        /// </summary>
        public event ClientRequestLogHandler       OnGetTariffByIdHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff by it identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler      OnGetTariffByIdHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff by it identification request had been received.
        /// </summary>
        public event OnGetTariffsResponseDelegate  OnGetTariffByIdResponse;

        #endregion


        #region OnGetSessionsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all sessions will be send.
        /// </summary>
        public event OnGetSessionsRequestDelegate   OnGetSessionsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all sessions will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnGetSessionsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all sessions HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetSessionsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all sessions request had been received.
        /// </summary>
        public event OnGetSessionsResponseDelegate  OnGetSessionsResponse;

        #endregion

        #region OnGetSessionByIdRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a session by it identification will be send.
        /// </summary>
        public event OnGetSessionsRequestDelegate   OnGetSessionByIdRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a session by it identification will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnGetSessionByIdHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a session by it identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetSessionByIdHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a session by it identification request had been received.
        /// </summary>
        public event OnGetSessionsResponseDelegate  OnGetSessionByIdResponse;

        #endregion


        #region OnGetCDRsRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting all CDRs will be send.
        /// </summary>
        public event OnGetCDRsRequestDelegate   OnGetCDRsRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting all CDRs will be send.
        /// </summary>
        public event ClientRequestLogHandler    OnGetCDRsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all CDRs HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler   OnGetCDRsHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting all CDRs request had been received.
        /// </summary>
        public event OnGetCDRsResponseDelegate  OnGetCDRsResponse;

        #endregion

        #region OnGetCDRByIdRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a CDR by it identification will be send.
        /// </summary>
        public event OnGetCDRsRequestDelegate   OnGetCDRByIdRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a CDR by it identification will be send.
        /// </summary>
        public event ClientRequestLogHandler    OnGetCDRByIdHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a CDR by it identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler   OnGetCDRByIdHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a CDR by it identification request had been received.
        /// </summary>
        public event OnGetCDRsResponseDelegate  OnGetCDRByIdResponse;

        #endregion


        #region OnGetTokenRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a token will be send.
        /// </summary>
        public event OnGetTokenRequestDelegate   OnGetTokenRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a token will be send.
        /// </summary>
        public event ClientRequestLogHandler     OnGetTokenHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a token HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler    OnGetTokenHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a token request had been received.
        /// </summary>
        public event OnGetTokenResponseDelegate  OnGetTokenResponse;

        #endregion

        #region OnPutTokenRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a token will be send.
        /// </summary>
        public event OnPutTokenRequestDelegate   OnPutTokenRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a token will be send.
        /// </summary>
        public event ClientRequestLogHandler     OnPutTokenHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a token HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler    OnPutTokenHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a token request had been received.
        /// </summary>
        public event OnPutTokenResponseDelegate  OnPutTokenResponse;

        #endregion

        #region OnPatchTokenRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a token will be send.
        /// </summary>
        public event OnPatchTokenRequestDelegate   OnPatchTokenRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a token will be send.
        /// </summary>
        public event ClientRequestLogHandler       OnPatchTokenHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a token HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler      OnPatchTokenHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a token request had been received.
        /// </summary>
        public event OnPatchTokenResponseDelegate  OnPatchTokenResponse;

        #endregion


        // Commands

        #region OnReserveNowRequest/-Response

        /// <summary>
        /// An event fired whenever a ReserveNow request will be send.
        /// </summary>
        public event OnReserveNowRequestDelegate   OnReserveNowRequest;

        /// <summary>
        /// An event fired whenever a ReserveNow HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler       OnReserveNowHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler      OnReserveNowHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a ReserveNow request had been received.
        /// </summary>
        public event OnReserveNowResponseDelegate  OnReserveNowResponse;

        #endregion

        #region OnCancelReservationRequest/-Response

        /// <summary>
        /// An event fired whenever a CancelReservation request will be send.
        /// </summary>
        public event OnCancelReservationRequestDelegate   OnCancelReservationRequest;

        /// <summary>
        /// An event fired whenever a CancelReservation HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler              OnCancelReservationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler             OnCancelReservationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a CancelReservation request had been received.
        /// </summary>
        public event OnCancelReservationResponseDelegate  OnCancelReservationResponse;

        #endregion

        #region OnStartSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a StartSession request will be send.
        /// </summary>
        public event OnStartSessionRequestDelegate   OnStartSessionRequest;

        /// <summary>
        /// An event fired whenever a StartSession HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler         OnStartSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a StartSession HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnStartSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a StartSession request had been received.
        /// </summary>
        public event OnStartSessionResponseDelegate  OnStartSessionResponse;

        #endregion

        #region OnStopSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a StopSession request will be send.
        /// </summary>
        public event OnStopSessionRequestDelegate   OnStopSessionRequest;

        /// <summary>
        /// An event fired whenever a StopSession HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnStopSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a StopSession HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnStopSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a StopSession request had been received.
        /// </summary>
        public event OnStopSessionResponseDelegate  OnStopSessionResponse;

        #endregion

        #region OnUnlockConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a UnlockConnector request will be send.
        /// </summary>
        public event OnUnlockConnectorRequestDelegate   OnUnlockConnectorRequest;

        /// <summary>
        /// An event fired whenever a UnlockConnector HTTP request will be send.
        /// </summary>
        public event ClientRequestLogHandler            OnUnlockConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a UnlockConnector HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler           OnUnlockConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a UnlockConnector request had been received.
        /// </summary>
        public event OnUnlockConnectorResponseDelegate  OnUnlockConnectorResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP client.
        /// </summary>
        /// <param name="RemoteVersionsURL">The remote URL of the VERSIONS endpoint to connect to.</param>
        /// <param name="AccessToken">The access token.</param>
        /// <param name="MyCommonAPI">My Common API.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="Description">An optional description of this CPO client.</param>
        /// <param name="RemoteCertificateValidator">The remote SSL/TLS certificate validator.</param>
        /// <param name="ClientCert">The SSL/TLS client certificate to use of HTTP authentication.</param>
        /// <param name="HTTPUserAgent">The HTTP user agent identification.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="TransmissionRetryDelay">The delay between transmission retries.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries for HTTP request.</param>
        /// <param name="DisableLogging">Disable all logging.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="LoggingContext">An optional context for logging.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        /// <param name="DNSClient">The DNS client to use.</param>
        public EMSPClient(URL                                   RemoteVersionsURL,
                          AccessToken                           AccessToken,
                          CommonAPI                             MyCommonAPI,
                          HTTPHostname?                         VirtualHostname              = null,
                          String?                               Description                  = null,
                          RemoteCertificateValidationCallback?  RemoteCertificateValidator   = null,
                          LocalCertificateSelectionCallback?    ClientCertificateSelector    = null,
                          X509Certificate?                      ClientCert                   = null,
                          SslProtocols?                         TLSProtocol                  = null,
                          Boolean?                              PreferIPv4                   = null,
                          String?                               HTTPUserAgent                = null,
                          TimeSpan?                             RequestTimeout               = null,
                          TransmissionRetryDelayDelegate?       TransmissionRetryDelay       = null,
                          UInt16?                               MaxNumberOfRetries           = null,
                          Boolean                               UseHTTPPipelining            = false,
                          HTTPClientLogger?                     HTTPLogger                   = null,
                          Boolean                               AccessTokenBase64Encoding    = true,

                          Boolean                               DisableLogging               = false,
                          String?                               LoggingPath                  = null,
                          String?                               LoggingContext               = null,
                          LogfileCreatorDelegate?               LogfileCreator               = null,
                          DNSClient?                            DNSClient                    = null)

            : base(RemoteVersionsURL,
                   AccessToken,
                   MyCommonAPI,
                   VirtualHostname,
                   Description,
                   RemoteCertificateValidator,
                   ClientCertificateSelector,
                   ClientCert,
                   TLSProtocol,
                   PreferIPv4,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   UseHTTPPipelining,
                   HTTPLogger,
                   AccessTokenBase64Encoding,

                   DisableLogging,
                   LoggingPath,
                   LoggingContext,
                   LogfileCreator,
                   DNSClient)

        {

            this.Counters    = new EMSPCounters();

            base.HTTPLogger  = DisableLogging == false
                                   ? new Logger(this,
                                                LoggingPath,
                                                LoggingContext,
                                                LogfileCreator)
                                   : null;

        }

        #endregion

        public override JObject ToJSON()
            => base.ToJSON(nameof(EMSPClient));


        #region GetLocations    (...)

        /// <summary>
        /// Get all locations from the remote API.
        /// </summary>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Location>>>

            GetLocations(Version_Id?         VersionId           = null,
                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,

                         DateTime?           Timestamp           = null,
                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Location>> response;

            #region Send OnGetLocationsRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetLocations.IncRequests();

                //if (OnGetLocationsRequest != null)
                //    await Task.WhenAll(OnGetLocationsRequest.GetInvocationList().
                //                       Cast<OnGetLocationsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetLocationsRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL LocationsURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    LocationsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.Locations &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var client = new HTTPSClient(LocationsURL,
                                             VirtualHostname,
                                             Description,
                                             RemoteCertificateValidator,
                                             ClientCertificateSelector,
                                             ClientCert,
                                             null,
                                             null,
                                             HTTPUserAgent,
                                             RequestTimeout,
                                             TransmissionRetryDelay,
                                             MaxNumberOfRetries,
                                             UseHTTPPipelining,
                                             HTTPLogger,
                                             DNSClient);

                 //client.OnChunkDataRead += async (Timestamp, BytesRead) => {
                 //    DebugX.Log("Additional " + BytesRead + " chunked bytes read!");
                 //};

                //client.OnDataRead += async (Timestamp, BytesRead, BytesExpected) => {
                //    DebugX.Log(BytesRead + " bytes of " + (BytesExpected ?? 0) + " read!");
                //};

                //client.OnChunkBlockFound += async (Timestamp, BlockLength) => {
                //    DebugX.Log("Chunked encoded block of length " + BlockLength + " bytes found!");
                //};

                var HTTPResponse = await client.Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationsURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetLocationsHTTPRequest,
                                              ResponseLogDelegate:  OnGetLocationsHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Location>.ParseJArray(HTTPResponse,
                                                              requestId,
                                                              correlationId,
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


            #region Send OnGetLocationsResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetLocationsResponse != null)
                //    await Task.WhenAll(OnGetLocationsResponse.GetInvocationList().
                //                       Cast<OnGetLocationsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetLocationsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLocationById (LocationId, ...)

        /// <summary>
        /// Get the location specified by the given location identification from the remote API.
        /// </summary>
        /// <param name="LocationId">The identification of the requested location.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            GetLocationById(Location_Id         LocationId,

                            Version_Id?         VersionId           = null,
                            Request_Id?         RequestId           = null,
                            Correlation_Id?     CorrelationId       = null,

                            DateTime?           Timestamp           = null,
                            CancellationToken?  CancellationToken   = null,
                            EventTracking_Id    EventTrackingId     = null,
                            TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Location> response;

            #region Send OnGetLocationByIdRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetLocations.IncRequests();

                //if (OnGetLocationsRequest != null)
                //    await Task.WhenAll(OnGetLocationsRequest.GetInvocationList().
                //                       Cast<OnGetLocationsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetLocationByIdRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL LocationsURL = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    LocationsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.Locations &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(LocationsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationsURL.Path + LocationId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetLocationByIdHTTPRequest,
                                              ResponseLogDelegate:  OnGetLocationByIdHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Location>.ParseJObject(HTTPResponse,
                                                               requestId,
                                                               correlationId,
                                                               json => Location.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Location>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnGetLocationByIdResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetLocationsResponse != null)
                //    await Task.WhenAll(OnGetLocationsResponse.GetInvocationList().
                //                       Cast<OnGetLocationsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetLocationByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetEVSEByUId    (LocationId, EVSEUId, ...)

        /// <summary>
        /// Get the EVSE specified by the given location and EVSE identification from the remote API.
        /// </summary>
        /// <param name="LocationId">The identification of the requested location.</param>
        /// <param name="EVSEUId">The unique identification of the EVSE location.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<EVSE>>

            GetEVSEByUId(Location_Id         LocationId,
                         EVSE_UId            EVSEUId,

                         Version_Id?         VersionId           = null,
                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,

                         DateTime?           Timestamp           = null,
                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<EVSE> response;

            #region Send OnGetEVSEByUIdRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetEVSEs.IncRequests();

                //if (OnGetEVSEsRequest != null)
                //    await Task.WhenAll(OnGetEVSEsRequest.GetInvocationList().
                //                       Cast<OnGetEVSEsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetEVSEByUIdRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL LocationsURL = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    LocationsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.Locations &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(LocationsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationsURL.Path + LocationId.ToString() + EVSEUId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetEVSEByUIdHTTPRequest,
                                              ResponseLogDelegate:  OnGetEVSEByUIdHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<EVSE>.ParseJObject(HTTPResponse,
                                                           requestId,
                                                           correlationId,
                                                           json => EVSE.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, EVSE>("",
                                                          default,
                                                          -1,
                                                          e.Message,
                                                          e.StackTrace);

            }


            #region Send OnGetEVSEByIdResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetEVSEsResponse != null)
                //    await Task.WhenAll(OnGetEVSEsResponse.GetInvocationList().
                //                       Cast<OnGetEVSEsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetEVSEByUIdResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetConnectorById(LocationId, EVSEUId, ConnectorId, ...)

        /// <summary>
        /// Get the connector specified by the given location, EVSE and connector identification from the remote API.
        /// </summary>
        /// <param name="LocationId">The identification of the requested location.</param>
        /// <param name="EVSEUId">The unique identification of the EVSE location.</param>
        /// <param name="ConnectorId">The identification of the requested connector.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Connector>>

            GetConnectorById(Location_Id         LocationId,
                             EVSE_UId            EVSEUId,
                             Connector_Id        ConnectorId,

                             Version_Id?         VersionId           = null,
                             Request_Id?         RequestId           = null,
                             Correlation_Id?     CorrelationId       = null,

                             DateTime?           Timestamp           = null,
                             CancellationToken?  CancellationToken   = null,
                             EventTracking_Id    EventTrackingId     = null,
                             TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Connector> response;

            #region Send OnGetConnectorByIdRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetConnectors.IncRequests();

                //if (OnGetConnectorsRequest != null)
                //    await Task.WhenAll(OnGetConnectorsRequest.GetInvocationList().
                //                       Cast<OnGetConnectorsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetConnectorByIdRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL LocationsURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    LocationsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.Locations &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(LocationsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationsURL.Path + LocationId.ToString() + EVSEUId.ToString() + ConnectorId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetConnectorByIdHTTPRequest,
                                              ResponseLogDelegate:  OnGetConnectorByIdHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Connector>.ParseJObject(HTTPResponse,
                                                                requestId,
                                                                correlationId,
                                                                json => Connector.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Connector>("",
                                                               default,
                                                               -1,
                                                               e.Message,
                                                               e.StackTrace);

            }


            #region Send OnGetConnectorByIdResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetConnectorsResponse != null)
                //    await Task.WhenAll(OnGetConnectorsResponse.GetInvocationList().
                //                       Cast<OnGetConnectorsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetConnectorByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetTariffs    (...)

        /// <summary>
        /// Get all tariffs from the remote API.
        /// </summary>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Tariff>>>

            GetTariffs(Version_Id?         VersionId           = null,
                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,

                       DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Tariff>> response;

            #region Send OnGetTariffsRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetTariffs.IncRequests();

                //if (OnGetTariffsRequest != null)
                //    await Task.WhenAll(OnGetTariffsRequest.GetInvocationList().
                //                       Cast<OnGetTariffsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetTariffsRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL TariffsURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    TariffsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.Tariffs &&
                                                                                     endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(TariffsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               TariffsURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetTariffsHTTPRequest,
                                              ResponseLogDelegate:  OnGetTariffsHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Tariff>.ParseJArray(HTTPResponse,
                                                            requestId,
                                                            correlationId,
                                                            json => Tariff.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, IEnumerable<Tariff>>("",
                                                                         default,
                                                                         -1,
                                                                         e.Message,
                                                                         e.StackTrace);

            }


            #region Send OnGetTariffsResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetTariffsResponse != null)
                //    await Task.WhenAll(OnGetTariffsResponse.GetInvocationList().
                //                       Cast<OnGetTariffsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetTariffsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetTariffById (TariffId, ...)

        /// <summary>
        /// Get the tariff specified by the given tariff identification from the remote API.
        /// </summary>
        /// <param name="TariffId">The identification of the requested tariff.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Tariff>>

            GetTariffById(Tariff_Id           TariffId,

                          Version_Id?         VersionId           = null,
                          Request_Id?         RequestId           = null,
                          Correlation_Id?     CorrelationId       = null,

                          DateTime?           Timestamp           = null,
                          CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Tariff> response;

            #region Send OnGetTariffByIdRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetTariffs.IncRequests();

                //if (OnGetTariffsRequest != null)
                //    await Task.WhenAll(OnGetTariffsRequest.GetInvocationList().
                //                       Cast<OnGetTariffsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetTariffByIdRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL TariffsURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    TariffsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.Tariffs &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(TariffsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               TariffsURL.Path + TariffId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetTariffByIdHTTPRequest,
                                              ResponseLogDelegate:  OnGetTariffByIdHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Tariff>.ParseJObject(HTTPResponse,
                                                             requestId,
                                                             correlationId,
                                                             json => Tariff.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Tariff>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnGetTariffByIdResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetTariffsResponse != null)
                //    await Task.WhenAll(OnGetTariffsResponse.GetInvocationList().
                //                       Cast<OnGetTariffsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetTariffByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetSessions   (...)

        /// <summary>
        /// Get all sessions from the remote API.
        /// </summary>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Session>>>

            GetSessions(Version_Id?         VersionId           = null,
                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken?  CancellationToken   = null,
                        EventTracking_Id    EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Session>> response;

            #region Send OnGetSessionsRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetSessions.IncRequests();

                //if (OnGetSessionsRequest != null)
                //    await Task.WhenAll(OnGetSessionsRequest.GetInvocationList().
                //                       Cast<OnGetSessionsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetSessionsRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL SessionsURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    SessionsURL  = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.Sessions &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(SessionsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               SessionsURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetSessionsHTTPRequest,
                                              ResponseLogDelegate:  OnGetSessionsHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Session>.ParseJArray(HTTPResponse,
                                                             requestId,
                                                             correlationId,
                                                             json => Session.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, IEnumerable<Session>>("",
                                                                          default,
                                                                          -1,
                                                                          e.Message,
                                                                          e.StackTrace);

            }


            #region Send OnGetSessionsResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetSessionsResponse != null)
                //    await Task.WhenAll(OnGetSessionsResponse.GetInvocationList().
                //                       Cast<OnGetSessionsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetSessionsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetSessionById(SessionId, ...)

        /// <summary>
        /// Get the session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="SessionId">The identification of the requested session.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Session>>

            GetSessionById(Session_Id          SessionId,

                           Version_Id?         VersionId           = null,
                           Request_Id?         RequestId           = null,
                           Correlation_Id?     CorrelationId       = null,

                           DateTime?           Timestamp           = null,
                           CancellationToken?  CancellationToken   = null,
                           EventTracking_Id    EventTrackingId     = null,
                           TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Session> response;

            #region Send OnGetSessionByIdRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetSessions.IncRequests();

                //if (OnGetSessionsRequest != null)
                //    await Task.WhenAll(OnGetSessionsRequest.GetInvocationList().
                //                       Cast<OnGetSessionsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetSessionByIdRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL SessionsURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    SessionsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.Sessions &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(SessionsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               SessionsURL.Path + SessionId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetSessionByIdHTTPRequest,
                                              ResponseLogDelegate:  OnGetSessionByIdHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<Session>.ParseJObject(HTTPResponse,
                                                              requestId,
                                                              correlationId,
                                                              json => Session.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Session>("",
                                                             default,
                                                             -1,
                                                             e.Message,
                                                             e.StackTrace);

            }


            #region Send OnGetSessionByIdResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetSessionsResponse != null)
                //    await Task.WhenAll(OnGetSessionsResponse.GetInvocationList().
                //                       Cast<OnGetSessionsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetSessionByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion

        // PUT     ~/sessions/{session_id}/charging_preferences


        #region GetCDRs   (...)

        /// <summary>
        /// Get all charge detail records from the remote API.
        /// </summary>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<CDR>>>

            GetCDRs(Version_Id?         VersionId           = null,
                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,

                    DateTime?           Timestamp           = null,
                    CancellationToken?  CancellationToken   = null,
                    EventTracking_Id    EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<CDR>> response;

            #region Send OnGetCDRsRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetCDRs.IncRequests();

                //if (OnGetCDRsRequest != null)
                //    await Task.WhenAll(OnGetCDRsRequest.GetInvocationList().
                //                       Cast<OnGetCDRsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetCDRsRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL CDRsURL = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    CDRsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.CDRs &&
                                                                                  endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(CDRsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               CDRsURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetCDRsHTTPRequest,
                                              ResponseLogDelegate:  OnGetCDRsHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<CDR>.ParseJArray(HTTPResponse,
                                                         requestId,
                                                         correlationId,
                                                         json => CDR.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, IEnumerable<CDR>>("",
                                                                      default,
                                                                      -1,
                                                                      e.Message,
                                                                      e.StackTrace);

            }


            #region Send OnGetCDRsResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetCDRsResponse != null)
                //    await Task.WhenAll(OnGetCDRsResponse.GetInvocationList().
                //                       Cast<OnGetCDRsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetCDRsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCDRById(CDRId, ...)         // The concrete URL is not specified by OCPI! m(

        /// <summary>
        /// Get the charge detail record specified by the given location identification from the remote API.
        /// </summary>
        /// <param name="CDRId">The identification of the requested charge detail record.</param>
        /// 
        /// <param name="VersionId">An optional OCPI version identification.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<CDR>>

            GetCDRById(CDR_Id              CDRId,

                       Version_Id?         VersionId           = null,
                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,

                       DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<CDR> response;

            #region Send OnGetCDRByIdRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetCDRs.IncRequests();

                //if (OnGetCDRsRequest != null)
                //    await Task.WhenAll(OnGetCDRsRequest.GetInvocationList().
                //                       Cast<OnGetCDRsRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetCDRByIdRequest));
            }

            #endregion

            try
            {

                #region Set versionId, requestId, correlationId

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                if (!versionId.HasValue)
                {

                    if (VersionDetails.Any())
                        versionId = VersionDetails.Keys.OrderByDescending(id => id).First();

                    else
                    {

                        await GetVersions();

                        if (Versions.Any())
                        {
                            versionId = Versions.Keys.OrderByDescending(id => id).First();
                            await GetVersionDetails(versionId.Value);
                        }

                    }

                }


                URL CDRsURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    CDRsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == Module_Id.CDRs &&
                                                                                  endpoint.Role       == InterfaceRoles.SENDER).URL;

                }

                #endregion

                #region Upstream HTTP request...

                var HTTPResponse     = await new HTTPSClient(CDRsURL,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               CDRsURL.Path + CDRId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
                                                                                   requestbuilder.Connection    = "close";
                                                                                   requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                   requestbuilder.Set("X-Request-ID",      requestId);
                                                                                   requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                               }),

                                              RequestLogDelegate:   OnGetCDRByIdHTTPRequest,
                                              ResponseLogDelegate:  OnGetCDRByIdHTTPResponse,
                                              CancellationToken:    CancellationToken,
                                              EventTrackingId:      EventTrackingId,
                                              RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                        ConfigureAwait(false);

                #endregion

                response = OCPIResponse<CDR>.ParseJObject(HTTPResponse,
                                                              requestId,
                                                              correlationId,
                                                              json => CDR.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, CDR>("",
                                                         default,
                                                         -1,
                                                         e.Message,
                                                         e.StackTrace);

            }


            #region Send OnGetCDRByIdResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetCDRsResponse != null)
                //    await Task.WhenAll(OnGetCDRsResponse.GetInvocationList().
                //                       Cast<OnGetCDRsResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetCDRByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetToken  (CountryCode, PartyId, TokenId, ...)

        /// <summary>
        /// Get the token specified by the given token identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Token>>

            GetToken(CountryCode         CountryCode,
                     Party_Id            PartyId,
                     Token_Id            TokenId,

                     Request_Id?         RequestId           = null,
                     Correlation_Id?     CorrelationId       = null,
                     Version_Id?         VersionId           = null,

                     DateTime?           Timestamp           = null,
                     CancellationToken?  CancellationToken   = null,
                     EventTracking_Id    EventTrackingId     = null,
                     TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Token> response;

            #region Send OnGetTokenRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetToken.IncRequests();

                //if (OnGetTokenRequest != null)
                //    await Task.WhenAll(OnGetTokenRequest.GetInvocationList().
                //                       Cast<OnGetTokenRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetTokenRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        Module_Id.Tokens,
                                                        InterfaceRoles.SENDER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            TokenId.    ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnGetTokenHTTPRequest,
                                                      ResponseLogDelegate:  OnGetTokenHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Token>.ParseJObject(HTTPResponse,
                                                                requestId,
                                                                correlationId,
                                                                json => Token.Parse(json));

                }

                else
                    response = new OCPIResponse<String, Token>("",
                                                               default,
                                                               -1,
                                                               "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Token>("",
                                                           default,
                                                           -1,
                                                           e.Message,
                                                           e.StackTrace);

            }


            #region Send OnGetTokenResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetTokenResponse != null)
                //    await Task.WhenAll(OnGetTokenResponse.GetInvocationList().
                //                       Cast<OnGetTokenResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnGetTokenResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutToken  (Token, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Token">The token to store/put at/onto the remote API.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Token>>

            PutToken(Token               Token,

                     Request_Id?         RequestId           = null,
                     Correlation_Id?     CorrelationId       = null,
                     Version_Id?         VersionId           = null,

                     DateTime?           Timestamp           = null,
                     CancellationToken?  CancellationToken   = null,
                     EventTracking_Id    EventTrackingId     = null,
                     TimeSpan?           RequestTimeout      = null)

        {

            if (Token is null)
                throw new ArgumentNullException(nameof(Token), "The given token must not be null!");

            OCPIResponse<Token> response;

            #region Send OnPutTokenRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PutToken.IncRequests();

                //if (OnPutTokenRequest != null)
                //    await Task.WhenAll(OnPutTokenRequest.GetInvocationList().
                //                       Cast<OnPutTokenRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnPutTokenRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        Module_Id.Tokens,
                                                        InterfaceRoles.SENDER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     remoteURL.Value.Path + Token.CountryCode.ToString() +
                                                                                                            Token.PartyId.    ToString() +
                                                                                                            Token.Id.         ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = Token.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnPutTokenHTTPRequest,
                                                      ResponseLogDelegate:  OnPutTokenHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Token>.ParseJObject(HTTPResponse,
                                                                requestId,
                                                                correlationId,
                                                                json => Token.Parse(json));

                }

                else
                    response = new OCPIResponse<String, Token>("",
                                                               default,
                                                               -1,
                                                               "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Token>("",
                                                           default,
                                                           -1,
                                                           e.Message,
                                                           e.StackTrace);

            }


            #region Send OnPutTokenResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPutTokenResponse != null)
                //    await Task.WhenAll(OnPutTokenResponse.GetInvocationList().
                //                       Cast<OnPutTokenResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnPutTokenResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchToken(CountryCode, PartyId, TokenId, ...)

        /// <summary>
        /// Start a charging token.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Token>>

            PatchToken(CountryCode         CountryCode,
                       Party_Id            PartyId,
                       Token_Id            TokenId,
                       JObject             TokenPatch,

                       Version_Id?         VersionId           = null,
                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,

                       DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            if (TokenPatch is null)
                throw new ArgumentNullException(nameof(TokenPatch), "The given token patch must not be null!");

            OCPIResponse<Token> response;

            #region Send OnPatchTokenRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PatchToken.IncRequests();

                //if (OnPatchTokenRequest != null)
                //    await Task.WhenAll(OnPatchTokenRequest.GetInvocationList().
                //                       Cast<OnPatchTokenRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnPatchTokenRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        Module_Id.Tokens,
                                                        InterfaceRoles.SENDER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.PATCH,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            TokenId.    ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = TokenPatch.ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnPatchTokenHTTPRequest,
                                                      ResponseLogDelegate:  OnPatchTokenHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Token>.ParseJObject(HTTPResponse,
                                                                requestId,
                                                                correlationId,
                                                                json => Token.Parse(json));

                }

                else
                    response = new OCPIResponse<String, Token>("",
                                                               default,
                                                               -1,
                                                               "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Token>("",
                                                           default,
                                                           -1,
                                                           e.Message,
                                                           e.StackTrace);

            }


            #region Send OnPatchTokenResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPatchTokenResponse != null)
                //    await Task.WhenAll(OnPatchTokenResponse.GetInvocationList().
                //                       Cast<OnPatchTokenResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnPatchTokenResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // Commands

        #region ReserveNow       (Token, ExpirationTimestamp, ReservationId, LocationId, EVSEUId, AuthorizationReference, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<ReserveNowCommand, CommandResponse>>

            ReserveNow(Token                    Token,
                       DateTime                 ExpirationTimestamp,
                       Reservation_Id           ReservationId,
                       Location_Id              LocationId,
                       EVSE_UId?                EVSEUId                  = null,
                       AuthorizationReference?  AuthorizationReference   = null,

                       Command_Id?              CommandId                = null,
                       Request_Id?              RequestId                = null,
                       Correlation_Id?          CorrelationId            = null,
                       Version_Id?              VersionId                = null,

                       DateTime?                Timestamp                = null,
                       CancellationToken?       CancellationToken        = null,
                       EventTracking_Id         EventTrackingId          = null,
                       TimeSpan?                RequestTimeout           = null)

        {

            OCPIResponse<ReserveNowCommand, CommandResponse> response;

            #region Send OnReserveNowRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.ReserveNow.IncRequests();

                //if (OnReserveNowRequest != null)
                //    await Task.WhenAll(OnReserveNowRequest.GetInvocationList().
                //                       Cast<OnReserveNowRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnReserveNowRequest));
            }

            #endregion


            try
            {

                var commandId      = CommandId     ?? Command_Id.    NewRandom();
                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                var remoteURL      = await GetRemoteURL(VersionId,
                                                        Module_Id.Commands,
                                                        InterfaceRoles.RECEIVER);

                var command        = new ReserveNowCommand(Token,
                                                           ExpirationTimestamp,
                                                           ReservationId,
                                                           LocationId,
                                                           MyCommonAPI.GetModuleURL(Module_Id.Commands,
                                                                                    SelectedOCPIVersionId.ToString() + "/emsp") + "RESERVE_NOW" + commandId.ToString(),
                                                           EVSEUId,
                                                           AuthorizationReference,
                                                           requestId,
                                                           correlationId);

                MyCommonAPI.CommandValueStore.AddOrUpdate(commandId,
                                                          (id)    => CommandValues.FromCommand(command),
                                                          (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand));


                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path + "RESERVE_NOW",
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = command.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnReserveNowHTTPRequest,
                                                      ResponseLogDelegate:  OnReserveNowHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<ReserveNowCommand, CommandResponse>.ParseJObject(command,
                                                                                             HTTPResponse,
                                                                                             requestId,
                                                                                             correlationId,
                                                                                             json => CommandResponse.Parse(command,
                                                                                                                           json));

                }

                else
                    response = new OCPIResponse<ReserveNowCommand, CommandResponse>(null,
                                                                                    default,
                                                                                    -1,
                                                                                    "No remote URL available!");


                if (MyCommonAPI.CommandValueStore.TryGetValue(commandId, out CommandValues commandValues))
                    commandValues.Response = response.Data;

            }

            catch (Exception e)
            {

                response = new OCPIResponse<ReserveNowCommand, CommandResponse>(null,
                                                                                default,
                                                                                -1,
                                                                                e.Message,
                                                                                e.StackTrace);

            }


            #region Send OnReserveNowResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnReserveNowResponse != null)
                //    await Task.WhenAll(OnReserveNowResponse.GetInvocationList().
                //                       Cast<OnReserveNowResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnReserveNowResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region CancelReservation(ReservationId, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<CancelReservationCommand, CommandResponse>>

            CancelReservation(Reservation_Id      ReservationId,

                              Command_Id?         CommandId           = null,
                              Request_Id?         RequestId           = null,
                              Correlation_Id?     CorrelationId       = null,
                              Version_Id?         VersionId           = null,

                              DateTime?           Timestamp           = null,
                              CancellationToken?  CancellationToken   = null,
                              EventTracking_Id    EventTrackingId     = null,
                              TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<CancelReservationCommand, CommandResponse> response;

            #region Send OnCancelReservationRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.CancelReservation.IncRequests();

                //if (OnCancelReservationRequest != null)
                //    await Task.WhenAll(OnCancelReservationRequest.GetInvocationList().
                //                       Cast<OnCancelReservationRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnCancelReservationRequest));
            }

            #endregion


            try
            {

                var commandId      = CommandId     ?? Command_Id.    NewRandom();
                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                var remoteURL      = await GetRemoteURL(VersionId,
                                                        Module_Id.Commands,
                                                        InterfaceRoles.RECEIVER);

                var command        = new CancelReservationCommand(ReservationId,
                                                                  MyCommonAPI.GetModuleURL(Module_Id.Commands,
                                                                                           SelectedOCPIVersionId.ToString() + "/emsp") + "CANCEL_RESERVATION" + commandId.ToString(),
                                                                  requestId,
                                                                  correlationId);

                MyCommonAPI.CommandValueStore.AddOrUpdate(commandId,
                                                          (id)    => CommandValues.FromCommand(command),
                                                          (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand));


                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path + "CANCEL_RESERVATION",
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = command.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnCancelReservationHTTPRequest,
                                                      ResponseLogDelegate:  OnCancelReservationHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<CancelReservationCommand, CommandResponse>.ParseJObject(command,
                                                                                                    HTTPResponse,
                                                                                                    requestId,
                                                                                                    correlationId,
                                                                                                    json => CommandResponse.Parse(command,
                                                                                                                                  json));

                }

                else
                    response = new OCPIResponse<CancelReservationCommand, CommandResponse>(null,
                                                                                           default,
                                                                                           -1,
                                                                                           "No remote URL available!");


                if (MyCommonAPI.CommandValueStore.TryGetValue(commandId, out CommandValues commandValues))
                    commandValues.Response = response.Data;

            }

            catch (Exception e)
            {

                response = new OCPIResponse<CancelReservationCommand, CommandResponse>(null,
                                                                                       default,
                                                                                       -1,
                                                                                       e.Message,
                                                                                       e.StackTrace);

            }


            #region Send OnCancelReservationResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnCancelReservationResponse != null)
                //    await Task.WhenAll(OnCancelReservationResponse.GetInvocationList().
                //                       Cast<OnCancelReservationResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnCancelReservationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StartSession     (Token, LocationId, EVSEUId, AuthorizationReference, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<StartSessionCommand, CommandResponse>>

            StartSession(Token                    Token,
                         Location_Id              LocationId,
                         EVSE_UId?                EVSEUId                  = null,
                         AuthorizationReference?  AuthorizationReference   = null,

                         Command_Id?              CommandId                = null,
                         Request_Id?              RequestId                = null,
                         Correlation_Id?          CorrelationId            = null,
                         Version_Id?              VersionId                = null,

                         DateTime?                Timestamp                = null,
                         CancellationToken?       CancellationToken        = null,
                         EventTracking_Id         EventTrackingId          = null,
                         TimeSpan?                RequestTimeout           = null)

        {

            OCPIResponse<StartSessionCommand, CommandResponse> response;

            #region Send OnStartSessionRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.StartSession.IncRequests();

                //if (OnStartSessionRequest != null)
                //    await Task.WhenAll(OnStartSessionRequest.GetInvocationList().
                //                       Cast<OnStartSessionRequestDelegate>().
                //                       Select(e => e(StartTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnStartSessionRequest));
            }

            #endregion


            try
            {

                var commandId      = CommandId     ?? Command_Id.    NewRandom();
                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                var remoteURL      = await GetRemoteURL(VersionId,
                                                        Module_Id.Commands,
                                                        InterfaceRoles.RECEIVER);

                var command        = new StartSessionCommand(Token,
                                                             LocationId,
                                                             MyCommonAPI.GetModuleURL(Module_Id.Commands,
                                                                                      SelectedOCPIVersionId.ToString() + "/emsp") + "START_SESSION" + commandId.ToString(),
                                                             EVSEUId,
                                                             AuthorizationReference,
                                                             requestId,
                                                             correlationId);

                MyCommonAPI.CommandValueStore.AddOrUpdate(commandId,
                                                          (id)    => CommandValues.FromCommand(command),
                                                          (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand));


                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path + "START_SESSION",
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = command.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnStartSessionHTTPRequest,
                                                      ResponseLogDelegate:  OnStartSessionHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<StartSessionCommand, CommandResponse>.ParseJObject(command,
                                                                                               HTTPResponse,
                                                                                               requestId,
                                                                                               correlationId,
                                                                                               json => CommandResponse.Parse(command,
                                                                                                                             json));

                }

                else
                    response = new OCPIResponse<StartSessionCommand, CommandResponse>(null,
                                                                                      default,
                                                                                      -1,
                                                                                      "No remote URL available!");


                if (MyCommonAPI.CommandValueStore.TryGetValue(commandId, out CommandValues commandValues))
                    commandValues.Response = response.Data;

            }

            catch (Exception e)
            {

                response = new OCPIResponse<StartSessionCommand, CommandResponse>(null,
                                                                                  default,
                                                                                  -1,
                                                                                  e.Message,
                                                                                  e.StackTrace);

            }


            #region Send OnStartSessionResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnStartSessionResponse != null)
                //    await Task.WhenAll(OnStartSessionResponse.GetInvocationList().
                //                       Cast<OnStartSessionResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StartTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnStartSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region StopSession      (SessionId, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<StopSessionCommand, CommandResponse>>

            StopSession(Session_Id          SessionId,

                        Command_Id?         CommandId           = null,
                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,
                        Version_Id?         VersionId           = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken?  CancellationToken   = null,
                        EventTracking_Id    EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<StopSessionCommand, CommandResponse> response;

            #region Send OnStopSessionRequest event

            var StopTime = DateTime.UtcNow;

            try
            {

                //Counters.StopSession.IncRequests();

                //if (OnStopSessionRequest != null)
                //    await Task.WhenAll(OnStopSessionRequest.GetInvocationList().
                //                       Cast<OnStopSessionRequestDelegate>().
                //                       Select(e => e(StopTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnStopSessionRequest));
            }

            #endregion


            try
            {

                var commandId      = CommandId     ?? Command_Id.    NewRandom();
                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                var remoteURL      = await GetRemoteURL(VersionId,
                                                        Module_Id.Commands,
                                                        InterfaceRoles.RECEIVER);

                var command        = new StopSessionCommand(SessionId,
                                                            MyCommonAPI.GetModuleURL(Module_Id.Commands,
                                                                                     SelectedOCPIVersionId.ToString() + "/emsp") + "STOP_SESSION" + commandId.ToString(),
                                                            requestId,
                                                            correlationId);

                MyCommonAPI.CommandValueStore.AddOrUpdate(commandId,
                                                          (id)    => CommandValues.FromCommand(command),
                                                          (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand));


                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path + "STOP_SESSION",
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = command.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnStopSessionHTTPRequest,
                                                      ResponseLogDelegate:  OnStopSessionHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<StopSessionCommand, CommandResponse>.ParseJObject(command,
                                                                                              HTTPResponse,
                                                                                              requestId,
                                                                                              correlationId,
                                                                                              json => CommandResponse.Parse(command,
                                                                                                                            json));

                }

                else
                    response = new OCPIResponse<StopSessionCommand, CommandResponse>(null,
                                                                                     default,
                                                                                     -1,
                                                                                     "No remote URL available!");


                if (MyCommonAPI.CommandValueStore.TryGetValue(commandId, out CommandValues commandValues))
                    commandValues.Response = response.Data;

            }

            catch (Exception e)
            {

                response = new OCPIResponse<StopSessionCommand, CommandResponse>(null,
                                                                                 default,
                                                                                 -1,
                                                                                 e.Message,
                                                                                 e.StackTrace);

            }


            #region Send OnStopSessionResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnStopSessionResponse != null)
                //    await Task.WhenAll(OnStopSessionResponse.GetInvocationList().
                //                       Cast<OnStopSessionResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StopTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnStopSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region UnlockConnector  (LocationId, EVSEUId, ConnectorId, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<UnlockConnectorCommand, CommandResponse>>

            UnlockConnector(Location_Id         LocationId,
                            EVSE_UId            EVSEUId,
                            Connector_Id        ConnectorId,

                            Command_Id?         CommandId           = null,
                            Request_Id?         RequestId           = null,
                            Correlation_Id?     CorrelationId       = null,
                            Version_Id?         VersionId           = null,

                            DateTime?           Timestamp           = null,
                            CancellationToken?  CancellationToken   = null,
                            EventTracking_Id    EventTrackingId     = null,
                            TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<UnlockConnectorCommand, CommandResponse> response;

            #region Send OnUnlockConnectorRequest event

            var StopTime = DateTime.UtcNow;

            try
            {

                //Counters.UnlockConnector.IncRequests();

                //if (OnUnlockConnectorRequest != null)
                //    await Task.WhenAll(OnUnlockConnectorRequest.GetInvocationList().
                //                       Cast<OnUnlockConnectorRequestDelegate>().
                //                       Select(e => e(StopTime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnUnlockConnectorRequest));
            }

            #endregion


            try
            {

                var commandId      = CommandId     ?? Command_Id.    NewRandom();
                var requestId      = RequestId     ?? Request_Id.    NewRandom();
                var correlationId  = CorrelationId ?? Correlation_Id.NewRandom();

                var remoteURL      = await GetRemoteURL(VersionId,
                                                        Module_Id.Commands,
                                                        InterfaceRoles.RECEIVER);

                var command        = new UnlockConnectorCommand(LocationId,
                                                                EVSEUId,
                                                                ConnectorId,
                                                                MyCommonAPI.GetModuleURL(Module_Id.Commands,
                                                                                         SelectedOCPIVersionId.ToString() + "/emsp") + "UNLOCK_CONNECTOR" + commandId.ToString(),
                                                                requestId,
                                                                correlationId);

                MyCommonAPI.CommandValueStore.AddOrUpdate(commandId,
                                                          (id)    => CommandValues.FromCommand(command),
                                                          (id, c) => CommandValues.FromUpstreamCommand(command, c.UpstreamCommand));


                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await new HTTPSClient(remoteURL.Value,
                                                             VirtualHostname,
                                                             Description,
                                                             RemoteCertificateValidator,
                                                             ClientCertificateSelector,
                                                             ClientCert,
                                                             null,
                                                             null,
                                                             HTTPUserAgent,
                                                             RequestTimeout,
                                                             TransmissionRetryDelay,
                                                             MaxNumberOfRetries,
                                                             UseHTTPPipelining,
                                                             HTTPLogger,
                                                             DNSClient).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path + "UNLOCK_CONNECTOR",
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = command.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Connection    = "close";
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnUnlockConnectorHTTPRequest,
                                                      ResponseLogDelegate:  OnUnlockConnectorHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<UnlockConnectorCommand, CommandResponse>.ParseJObject(command,
                                                                                                  HTTPResponse,
                                                                                                  requestId,
                                                                                                  correlationId,
                                                                                                  json => CommandResponse.Parse(command,
                                                                                                                                json));

                }

                else
                    response = new OCPIResponse<UnlockConnectorCommand, CommandResponse>(null,
                                                                                         default,
                                                                                         -1,
                                                                                         "No remote URL available!");


                if (MyCommonAPI.CommandValueStore.TryGetValue(commandId, out CommandValues commandValues))
                    commandValues.Response = response.Data;

            }

            catch (Exception e)
            {

                response = new OCPIResponse<UnlockConnectorCommand, CommandResponse>(null,
                                                                                     default,
                                                                                     -1,
                                                                                     e.Message,
                                                                                     e.StackTrace);

            }


            #region Send OnUnlockConnectorResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnUnlockConnectorResponse != null)
                //    await Task.WhenAll(OnUnlockConnectorResponse.GetInvocationList().
                //                       Cast<OnUnlockConnectorResponseDelegate>().
                //                       Select(e => e(Endtime,
                //                                     Request.Timestamp.Value,
                //                                     this,
                //                                     ClientId,
                //                                     Request.EventTrackingId,

                //                                     Request.PartnerId,
                //                                     Request.OperatorId,
                //                                     Request.ChargingPoolId,
                //                                     Request.StatusEventDate,
                //                                     Request.AvailabilityStatus,
                //                                     Request.TransactionId,
                //                                     Request.AvailabilityStatusUntil,
                //                                     Request.AvailabilityStatusComment,

                //                                     Request.RequestTimeout ?? RequestTimeout.Value,
                //                                     result.Content,
                //                                     Endtime - StopTime))).
                //                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(EMSPClient) + "." + nameof(OnUnlockConnectorResponse));
            }

            #endregion

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
