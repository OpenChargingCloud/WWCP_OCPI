/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    #region OnGetLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a get location request will be send.
    /// </summary>
    public delegate Task OnGetLocationRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a get location request had been received.
    /// </summary>
    public delegate Task OnGetLocationResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPutLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a put location request will be send.
    /// </summary>
    public delegate Task OnPutLocationRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a put location request had been received.
    /// </summary>
    public delegate Task OnPutLocationResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPatchLocationRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch location request will be send.
    /// </summary>
    public delegate Task OnPatchLocationRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a patch location request had been received.
    /// </summary>
    public delegate Task OnPatchLocationResponseDelegate(DateTime                                    LogTimestamp,
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


    #region OnGetEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a get EVSE request will be send.
    /// </summary>
    public delegate Task OnGetEVSERequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a get EVSE request had been received.
    /// </summary>
    public delegate Task OnGetEVSEResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPutEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a put EVSE request will be send.
    /// </summary>
    public delegate Task OnPutEVSERequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a put EVSE request had been received.
    /// </summary>
    public delegate Task OnPutEVSEResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPatchEVSERequest/-Response

    /// <summary>
    /// A delegate called whenever a patch EVSE request will be send.
    /// </summary>
    public delegate Task OnPatchEVSERequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a patch EVSE request had been received.
    /// </summary>
    public delegate Task OnPatchEVSEResponseDelegate(DateTime                                    LogTimestamp,
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


    #region OnGetConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a get connector request will be send.
    /// </summary>
    public delegate Task OnGetConnectorRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a get connector request had been received.
    /// </summary>
    public delegate Task OnGetConnectorResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPutConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a put connector request will be send.
    /// </summary>
    public delegate Task OnPutConnectorRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a put connector request had been received.
    /// </summary>
    public delegate Task OnPutConnectorResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPatchConnectorRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch connector request will be send.
    /// </summary>
    public delegate Task OnPatchConnectorRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a patch connector request had been received.
    /// </summary>
    public delegate Task OnPatchConnectorResponseDelegate(DateTime                                    LogTimestamp,
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


    #region OnGetTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tariff request will be send.
    /// </summary>
    public delegate Task OnGetTariffRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a get tariff request had been received.
    /// </summary>
    public delegate Task OnGetTariffResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPutTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a put tariff request will be send.
    /// </summary>
    public delegate Task OnPutTariffRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a put tariff request had been received.
    /// </summary>
    public delegate Task OnPutTariffResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPatchTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch tariff request will be send.
    /// </summary>
    public delegate Task OnPatchTariffRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a patch tariff request had been received.
    /// </summary>
    public delegate Task OnPatchTariffResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnDeleteTariffRequest/-Response

    /// <summary>
    /// A delegate called whenever a delete tariff request will be send.
    /// </summary>
    public delegate Task OnDeleteTariffRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a delete tariff request had been received.
    /// </summary>
    public delegate Task OnDeleteTariffResponseDelegate(DateTime                                    LogTimestamp,
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


    #region OnGetSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a get session request will be send.
    /// </summary>
    public delegate Task OnGetSessionRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a get session request had been received.
    /// </summary>
    public delegate Task OnGetSessionResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPutSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a put session request will be send.
    /// </summary>
    public delegate Task OnPutSessionRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a put session request had been received.
    /// </summary>
    public delegate Task OnPutSessionResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPatchSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a patch session request will be send.
    /// </summary>
    public delegate Task OnPatchSessionRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a patch session request had been received.
    /// </summary>
    public delegate Task OnPatchSessionResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnDeleteSessionRequest/-Response

    /// <summary>
    /// A delegate called whenever a delete session request will be send.
    /// </summary>
    public delegate Task OnDeleteSessionRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a delete session request had been received.
    /// </summary>
    public delegate Task OnDeleteSessionResponseDelegate(DateTime                                    LogTimestamp,
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


    #region OnGetCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a get charge detail record request will be send.
    /// </summary>
    public delegate Task OnGetCDRRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a get charge detail record request had been received.
    /// </summary>
    public delegate Task OnGetCDRResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPostCDRRequest/-Response

    /// <summary>
    /// A delegate called whenever a post charge detail record request will be send.
    /// </summary>
    public delegate Task OnPostCDRRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a post charge detail record request had been received.
    /// </summary>
    public delegate Task OnPostCDRResponseDelegate(DateTime                                    LogTimestamp,
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


    #region OnGetTokensRequest/-Response

    /// <summary>
    /// A delegate called whenever a get tokens request will be send.
    /// </summary>
    public delegate Task OnGetTokensRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a get tokens request had been received.
    /// </summary>
    public delegate Task OnGetTokensResponseDelegate(DateTime                                    LogTimestamp,
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

    #region OnPostTokenRequest/-Response

    /// <summary>
    /// A delegate called whenever a post token request will be send.
    /// </summary>
    public delegate Task OnPostTokenRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a post token request had been received.
    /// </summary>
    public delegate Task OnPostTokenResponseDelegate(DateTime                                    LogTimestamp,
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


    #region OnSetChargingProfileRequest/-Response

    /// <summary>
    /// A delegate called whenever a set charging profile request will be send.
    /// </summary>
    public delegate Task OnSetChargingProfileRequestDelegate(DateTime                                    LogTimestamp,
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
    /// A delegate called whenever a response to a set charging profile request had been received.
    /// </summary>
    public delegate Task OnSetChargingProfileResponseDelegate(DateTime                                    LogTimestamp,
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
