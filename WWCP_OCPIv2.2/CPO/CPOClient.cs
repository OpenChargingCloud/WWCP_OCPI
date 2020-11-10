/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
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



    /// <summary>
    /// The CPO client.
    /// </summary>
    public partial class CPOClient : CommonClient
    {

        public class CPOCounters
        {

            public CounterValues GetTokens  { get; }
            public CounterValues PostTokens { get; }

            public CPOCounters(CounterValues? GetTokens  = null,
                               CounterValues? PostTokens = null)
            {

                this.GetTokens  = GetTokens  ?? new CounterValues();
                this.PostTokens = PostTokens ?? new CounterValues();

            }

            public JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("GetTokens",  GetTokens. ToJSON()),
                       new JProperty("PostTokens", PostTokens.ToJSON())
                   );

        }


        #region Events

        #region OnGetLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a location will be send.
        /// </summary>
        public event OnGetLocationRequestDelegate   OnGetLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a location will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnGetLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a location request had been received.
        /// </summary>
        public event OnGetLocationResponseDelegate  OnGetLocationResponse;

        #endregion

        #region OnPutLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a location will be send.
        /// </summary>
        public event OnPutLocationRequestDelegate   OnPutLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a location will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnPutLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnPutLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a location request had been received.
        /// </summary>
        public event OnPutLocationResponseDelegate  OnPutLocationResponse;

        #endregion

        #region OnPatchLocationRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a location will be send.
        /// </summary>
        public event OnPatchLocationRequestDelegate   OnPatchLocationRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a location will be send.
        /// </summary>
        public event ClientRequestLogHandler          OnPatchLocationHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a location HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler         OnPatchLocationHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a location request had been received.
        /// </summary>
        public event OnPatchLocationResponseDelegate  OnPatchLocationResponse;

        #endregion


        #region OnGetEVSERequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a EVSE will be send.
        /// </summary>
        public event OnGetEVSERequestDelegate   OnGetEVSERequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a EVSE will be send.
        /// </summary>
        public event ClientRequestLogHandler    OnGetEVSEHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a EVSE HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler   OnGetEVSEHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a EVSE request had been received.
        /// </summary>
        public event OnGetEVSEResponseDelegate  OnGetEVSEResponse;

        #endregion

        #region OnPutEVSERequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a EVSE will be send.
        /// </summary>
        public event OnPutEVSERequestDelegate   OnPutEVSERequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a EVSE will be send.
        /// </summary>
        public event ClientRequestLogHandler    OnPutEVSEHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a EVSE HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler   OnPutEVSEHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a EVSE request had been received.
        /// </summary>
        public event OnPutEVSEResponseDelegate  OnPutEVSEResponse;

        #endregion

        #region OnPatchEVSERequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a EVSE will be send.
        /// </summary>
        public event OnPatchEVSERequestDelegate   OnPatchEVSERequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a EVSE will be send.
        /// </summary>
        public event ClientRequestLogHandler      OnPatchEVSEHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a EVSE HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler     OnPatchEVSEHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a EVSE request had been received.
        /// </summary>
        public event OnPatchEVSEResponseDelegate  OnPatchEVSEResponse;

        #endregion


        #region OnGetConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a connector will be send.
        /// </summary>
        public event OnGetConnectorRequestDelegate   OnGetConnectorRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a connector will be send.
        /// </summary>
        public event ClientRequestLogHandler         OnGetConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a connector HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnGetConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a connector request had been received.
        /// </summary>
        public event OnGetConnectorResponseDelegate  OnGetConnectorResponse;

        #endregion

        #region OnPutConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a connector will be send.
        /// </summary>
        public event OnPutConnectorRequestDelegate   OnPutConnectorRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a connector will be send.
        /// </summary>
        public event ClientRequestLogHandler         OnPutConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a connector HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnPutConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a connector request had been received.
        /// </summary>
        public event OnPutConnectorResponseDelegate  OnPutConnectorResponse;

        #endregion

        #region OnPatchConnectorRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a connector will be send.
        /// </summary>
        public event OnPatchConnectorRequestDelegate   OnPatchConnectorRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a connector will be send.
        /// </summary>
        public event ClientRequestLogHandler           OnPatchConnectorHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a connector HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler          OnPatchConnectorHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a connector request had been received.
        /// </summary>
        public event OnPatchConnectorResponseDelegate  OnPatchConnectorResponse;

        #endregion



        #region OnGetTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a tariff will be send.
        /// </summary>
        public event OnGetTariffRequestDelegate   OnGetTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a tariff will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnGetTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff request had been received.
        /// </summary>
        public event OnGetTariffResponseDelegate  OnGetTariffResponse;

        #endregion

        #region OnPutTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a tariff will be send.
        /// </summary>
        public event OnPutTariffRequestDelegate   OnPutTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a tariff will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnPutTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a tariff HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnPutTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a tariff request had been received.
        /// </summary>
        public event OnPutTariffResponseDelegate  OnPutTariffResponse;

        #endregion

        #region OnPatchTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a tariff will be send.
        /// </summary>
        public event OnPatchTariffRequestDelegate   OnPatchTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a tariff will be send.
        /// </summary>
        public event ClientRequestLogHandler          OnPatchTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a tariff HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler         OnPatchTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a tariff request had been received.
        /// </summary>
        public event OnPatchTariffResponseDelegate  OnPatchTariffResponse;

        #endregion

        #region OnDeleteTariffRequest/-Response

        /// <summary>
        /// An event fired whenever a request deleting a tariff will be send.
        /// </summary>
        public event OnDeleteTariffRequestDelegate   OnDeleteTariffRequest;

        /// <summary>
        /// An event fired whenever a HTTP request deleting a tariff will be send.
        /// </summary>
        public event ClientRequestLogHandler         OnDeleteTariffHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a deleting a tariff HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnDeleteTariffHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a deleting a tariff request had been received.
        /// </summary>
        public event OnDeleteTariffResponseDelegate  OnDeleteTariffResponse;

        #endregion



        #region OnGetSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a session will be send.
        /// </summary>
        public event OnGetSessionRequestDelegate   OnGetSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a session will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnGetSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a session HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a session request had been received.
        /// </summary>
        public event OnGetSessionResponseDelegate  OnGetSessionResponse;

        #endregion

        #region OnPutSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request putting a session will be send.
        /// </summary>
        public event OnPutSessionRequestDelegate   OnPutSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request putting a session will be send.
        /// </summary>
        public event ClientRequestLogHandler        OnPutSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a putting a session HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnPutSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a putting a session request had been received.
        /// </summary>
        public event OnPutSessionResponseDelegate  OnPutSessionResponse;

        #endregion

        #region OnPatchSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request patching a session will be send.
        /// </summary>
        public event OnPatchSessionRequestDelegate   OnPatchSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request patching a session will be send.
        /// </summary>
        public event ClientRequestLogHandler          OnPatchSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a patching a session HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler         OnPatchSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a patching a session request had been received.
        /// </summary>
        public event OnPatchSessionResponseDelegate  OnPatchSessionResponse;

        #endregion

        #region OnDeleteSessionRequest/-Response

        /// <summary>
        /// An event fired whenever a request deleting a session will be send.
        /// </summary>
        public event OnDeleteSessionRequestDelegate   OnDeleteSessionRequest;

        /// <summary>
        /// An event fired whenever a HTTP request deleting a session will be send.
        /// </summary>
        public event ClientRequestLogHandler         OnDeleteSessionHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a deleting a session HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnDeleteSessionHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a deleting a session request had been received.
        /// </summary>
        public event OnDeleteSessionResponseDelegate  OnDeleteSessionResponse;

        #endregion



        #region OnGetCDRRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting a charge detail record will be send.
        /// </summary>
        public event OnGetCDRRequestDelegate   OnGetCDRRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting a charge detail record will be send.
        /// </summary>
        public event ClientRequestLogHandler   OnGetCDRHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a charge detail record HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler  OnGetCDRHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting a charge detail record request had been received.
        /// </summary>
        public event OnGetCDRResponseDelegate  OnGetCDRResponse;

        #endregion

        #region OnPostCDRRequest/-Response

        /// <summary>
        /// An event fired whenever a request posting a charge detail record will be send.
        /// </summary>
        public event OnPostCDRRequestDelegate   OnPostCDRRequest;

        /// <summary>
        /// An event fired whenever a HTTP request posting a charge detail record will be send.
        /// </summary>
        public event ClientRequestLogHandler    OnPostCDRHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a posting a charge detail record HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler   OnPostCDRHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a posting a charge detail record request had been received.
        /// </summary>
        public event OnPostCDRResponseDelegate  OnPostCDRResponse;

        #endregion



        #region OnGetTokensRequest/-Response

        /// <summary>
        /// An event fired whenever a request getting tokens will be send.
        /// </summary>
        public event OnGetTokensRequestDelegate   OnGetTokensRequest;

        /// <summary>
        /// An event fired whenever a HTTP request getting tokens will be send.
        /// </summary>
        public event ClientRequestLogHandler      OnGetTokensHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting tokens HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler     OnGetTokensHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a getting tokens request had been received.
        /// </summary>
        public event OnGetTokensResponseDelegate  OnGetTokensResponse;

        #endregion

        #region OnPostTokenRequest/-Response

        /// <summary>
        /// An event fired whenever a request posting a token will be send.
        /// </summary>
        public event OnPostTokenRequestDelegate   OnPostTokenRequest;

        /// <summary>
        /// An event fired whenever a HTTP request posting a token will be send.
        /// </summary>
        public event ClientRequestLogHandler      OnPostTokenHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a posting a token HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler     OnPostTokenHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a posting a token request had been received.
        /// </summary>
        public event OnPostTokenResponseDelegate  OnPostTokenResponse;

        #endregion


        #region OnSetChargingProfileRequest/-Response

        /// <summary>
        /// An event fired whenever a request setting a charging profile will be send.
        /// </summary>
        public event OnSetChargingProfileRequestDelegate   OnSetChargingProfileRequest;

        /// <summary>
        /// An event fired whenever a HTTP request setting a charging profile will be send.
        /// </summary>
        public event ClientRequestLogHandler               OnSetChargingProfileHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a set charging profile HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler              OnSetChargingProfileHTTPResponse;

        /// <summary>
        /// An event fired whenever a response to a set charging profile request had been received.
        /// </summary>
        public event OnSetChargingProfileResponseDelegate  OnSetChargingProfileResponse;

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP client.
        /// </summary>
        /// <param name="AccessToken">The access token.</param>
        /// <param name="RemoteVersionsURL">The remote URL of the VERSIONS endpoint to connect to.</param>
        /// <param name="MyCommandsURL">The local URL of the COMMANDS endpoint.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="RemoteCertificateValidator">An optional remote SSL/TLS certificate validator.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        public CPOClient(AccessToken                          AccessToken,
                         URL                                  RemoteVersionsURL,
                         URL                                  MyCommandsURL,
                         HTTPHostname?                        VirtualHostname              = null,
                         RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                         TimeSpan?                            RequestTimeout               = null,
                         Byte?                                MaxNumberOfRetries           = null,
                         DNSClient                            DNSClient                    = null)

            : base(AccessToken,
                   RemoteVersionsURL,
                   MyCommandsURL,
                   VirtualHostname,
                   RemoteCertificateValidator,
                   RequestTimeout,
                   MaxNumberOfRetries,
                   DNSClient)

        { }

        #endregion



        #region GetLocation    (CountryCode, PartyId, LocationId, ...)

        /// <summary>
        /// Get the charging location specified by the given location identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            GetLocation(CountryCode         CountryCode,
                        Party_Id            PartyId,
                        Location_Id         LocationId,

                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,
                        Version_Id?         VersionId           = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken?  CancellationToken   = null,
                        EventTracking_Id    EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Location> response;

            #region Send OnGetLocationRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetLocation.IncRequests();

                //if (OnGetLocationRequest != null)
                //    await Task.WhenAll(OnGetLocationRequest.GetInvocationList().
                //                       Cast<OnGetLocationRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetLocationRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            LocationId. ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnGetLocationHTTPRequest,
                                                      ResponseLogDelegate:  OnGetLocationHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Location>("",
                                                                  default,
                                                                  -1,
                                                                  "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Location>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnGetLocationResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetLocationResponse != null)
                //    await Task.WhenAll(OnGetLocationResponse.GetInvocationList().
                //                       Cast<OnGetLocationResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutLocation    (Location, ...)

        /// <summary>
        /// Put/store the given charging location on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            PutLocation(Location            Location,

                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,
                        Version_Id?         VersionId           = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken?  CancellationToken   = null,
                        EventTracking_Id    EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Location> response;

            #region Send OnPutLocationRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PutLocation.IncRequests();

                //if (OnPutLocationRequest != null)
                //    await Task.WhenAll(OnPutLocationRequest.GetInvocationList().
                //                       Cast<OnPutLocationRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutLocationRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     remoteURL.Value.Path + Location.CountryCode.ToString() +
                                                                                                            Location.PartyId.    ToString() +
                                                                                                            Location.Id.         ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = Location.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPutLocationHTTPRequest,
                                                    ResponseLogDelegate:  OnPutLocationHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Location>("",
                                                                  default,
                                                                  -1,
                                                                  "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Location>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnPutLocationResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPutLocationResponse != null)
                //    await Task.WhenAll(OnPutLocationResponse.GetInvocationList().
                //                       Cast<OnPutLocationResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchLocation  (CountryCode, PartyId, LocationId, LocationPatch, ...)

        /// <summary>
        /// Patch a location.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            PatchLocation(CountryCode         CountryCode,
                          Party_Id            PartyId,
                          Location_Id         LocationId,
                          JObject             LocationPatch,

                          Request_Id?         RequestId           = null,
                          Correlation_Id?     CorrelationId       = null,
                          Version_Id?         VersionId           = null,

                          DateTime?           Timestamp           = null,
                          CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Location> response;

            #region Send OnPatchLocationRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PatchLocation.IncRequests();

                //if (OnPatchLocationRequest != null)
                //    await Task.WhenAll(OnPatchLocationRequest.GetInvocationList().
                //                       Cast<OnPatchLocationRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchLocationRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PATCH,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            LocationId. ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = LocationPatch.ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPatchLocationHTTPRequest,
                                                    ResponseLogDelegate:  OnPatchLocationHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Location>("",
                                                                  default,
                                                                  -1,
                                                                  "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Location>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnPatchLocationResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPatchLocationResponse != null)
                //    await Task.WhenAll(OnPatchLocationResponse.GetInvocationList().
                //                       Cast<OnPatchLocationResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetEVSE        (CountryCode, PartyId, LocationId, EVSEUId, ...)

        /// <summary>
        /// Get the EVSE specified by the given EVSE unique identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Location>>

            GetEVSE(CountryCode         CountryCode,
                    Party_Id            PartyId,
                    Location_Id         LocationId,
                    EVSE_UId            EVSEUId,

                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,
                    Version_Id?         VersionId           = null,

                    DateTime?           Timestamp           = null,
                    CancellationToken?  CancellationToken   = null,
                    EventTracking_Id    EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Location> response;

            #region Send OnGetLocationRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetLocation.IncRequests();

                //if (OnGetLocationRequest != null)
                //    await Task.WhenAll(OnGetLocationRequest.GetInvocationList().
                //                       Cast<OnGetLocationRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetLocationRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            LocationId. ToString() +
                                                                                                            EVSEUId.    ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnGetEVSEHTTPRequest,
                                                      ResponseLogDelegate:  OnGetEVSEHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Location>("",
                                                                  default,
                                                                  -1,
                                                                  "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Location>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnGetLocationResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetLocationResponse != null)
                //    await Task.WhenAll(OnGetLocationResponse.GetInvocationList().
                //                       Cast<OnGetLocationResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetLocationResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutEVSE        (EVSE, ...)

        /// <summary>
        /// Put/store the given EVSE on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<EVSE>>

            PutEVSE(EVSE                EVSE,

                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,
                    Version_Id?         VersionId           = null,

                    DateTime?           Timestamp           = null,
                    CancellationToken?  CancellationToken   = null,
                    EventTracking_Id    EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<EVSE> response;

            #region Send OnPutEVSERequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PutEVSE.IncRequests();

                //if (OnPutEVSERequest != null)
                //    await Task.WhenAll(OnPutEVSERequest.GetInvocationList().
                //                       Cast<OnPutEVSERequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutEVSERequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     remoteURL.Value.Path + EVSE.ParentLocation.CountryCode.ToString() +
                                                                                                            EVSE.ParentLocation.PartyId.    ToString() +
                                                                                                            EVSE.ParentLocation.Id.         ToString() +
                                                                                                            EVSE.               UId.        ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = EVSE.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPutEVSEHTTPRequest,
                                                    ResponseLogDelegate:  OnPutEVSEHTTPResponse,
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

                else
                    response = new OCPIResponse<String, EVSE>("",
                                                              default,
                                                              -1,
                                                              "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, EVSE>("",
                                                          default,
                                                          -1,
                                                          e.Message,
                                                          e.StackTrace);

            }


            #region Send OnPutEVSEResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPutEVSEResponse != null)
                //    await Task.WhenAll(OnPutEVSEResponse.GetInvocationList().
                //                       Cast<OnPutEVSEResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutEVSEResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchEVSE      (CountryCode, PartyId, LocationId, EVSEUId, EVSEPatch, ...)

        /// <summary>
        /// Patch a location.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional location to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<EVSE>>

            PatchEVSE(CountryCode         CountryCode,
                      Party_Id            PartyId,
                      Location_Id         LocationId,
                      EVSE_UId            EVSEUId,
                      JObject             EVSEPatch,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      DateTime?           Timestamp           = null,
                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id    EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<EVSE> response;

            #region Send OnPatchEVSERequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PatchEVSE.IncRequests();

                //if (OnPatchEVSERequest != null)
                //    await Task.WhenAll(OnPatchEVSERequest.GetInvocationList().
                //                       Cast<OnPatchEVSERequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchEVSERequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PATCH,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            LocationId. ToString() +
                                                                                                            EVSEUId.    ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = EVSEPatch.ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPatchEVSEHTTPRequest,
                                                    ResponseLogDelegate:  OnPatchEVSEHTTPResponse,
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

                else
                    response = new OCPIResponse<String, EVSE>("",
                                                              default,
                                                              -1,
                                                              "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, EVSE>("",
                                                          default,
                                                          -1,
                                                          e.Message,
                                                          e.StackTrace);

            }


            #region Send OnPatchEVSEResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPatchEVSEResponse != null)
                //    await Task.WhenAll(OnPatchEVSEResponse.GetInvocationList().
                //                       Cast<OnPatchEVSEResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchEVSEResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetConnector   (CountryCode, PartyId, LocationId, EVSEUId, ConnectorId, ...)

        /// <summary>
        /// Get the connector specified by the given connector identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional connector to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Connector>>

            GetConnector(CountryCode         CountryCode,
                         Party_Id            PartyId,
                         Location_Id         LocationId,
                         EVSE_UId            EVSEUId,
                         Connector_Id        ConnectorId,

                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,
                         Version_Id?         VersionId           = null,

                         DateTime?           Timestamp           = null,
                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Connector> response;

            #region Send OnGetConnectorRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetConnector.IncRequests();

                //if (OnGetConnectorRequest != null)
                //    await Task.WhenAll(OnGetConnectorRequest.GetInvocationList().
                //                       Cast<OnGetConnectorRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetConnectorRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            LocationId. ToString() +
                                                                                                            EVSEUId.    ToString() +
                                                                                                            ConnectorId.ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnGetConnectorHTTPRequest,
                                                      ResponseLogDelegate:  OnGetConnectorHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Connector>("",
                                                                  default,
                                                                  -1,
                                                                  "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Connector>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnGetConnectorResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetConnectorResponse != null)
                //    await Task.WhenAll(OnGetConnectorResponse.GetInvocationList().
                //                       Cast<OnGetConnectorResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutConnector   (Connector, ...)

        /// <summary>
        /// Put/store the given charging connector on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional connector to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Connector>>

            PutConnector(Connector           Connector,

                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,
                         Version_Id?         VersionId           = null,

                         DateTime?           Timestamp           = null,
                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Connector> response;

            #region Send OnPutConnectorRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PutConnector.IncRequests();

                //if (OnPutConnectorRequest != null)
                //    await Task.WhenAll(OnPutConnectorRequest.GetInvocationList().
                //                       Cast<OnPutConnectorRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutConnectorRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     remoteURL.Value.Path + Connector.ParentEVSE.ParentLocation.CountryCode.ToString() +
                                                                                                            Connector.ParentEVSE.ParentLocation.PartyId.    ToString() +
                                                                                                            Connector.ParentEVSE.ParentLocation.Id.         ToString() +
                                                                                                            Connector.ParentEVSE.               UId.        ToString() +
                                                                                                            Connector.                          Id.         ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = Connector.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPutConnectorHTTPRequest,
                                                    ResponseLogDelegate:  OnPutConnectorHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Connector>("",
                                                                  default,
                                                                  -1,
                                                                  "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Connector>("",
                                                              default,
                                                              -1,
                                                              e.Message,
                                                              e.StackTrace);

            }


            #region Send OnPutConnectorResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPutConnectorResponse != null)
                //    await Task.WhenAll(OnPutConnectorResponse.GetInvocationList().
                //                       Cast<OnPutConnectorResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchConnector (CountryCode, PartyId, LocationId, EVSEUId, ConnectorId, ConnectorPatch, ...)

        /// <summary>
        /// Patch a connector.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional connector to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Connector>>

            PatchConnector(CountryCode         CountryCode,
                           Party_Id            PartyId,
                           Location_Id         LocationId,
                           EVSE_UId            EVSEUId,
                           Connector_Id        ConnectorId,
                           JObject             ConnectorPatch,

                           Request_Id?         RequestId           = null,
                           Correlation_Id?     CorrelationId       = null,
                           Version_Id?         VersionId           = null,

                           DateTime?           Timestamp           = null,
                           CancellationToken?  CancellationToken   = null,
                           EventTracking_Id    EventTrackingId     = null,
                           TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Connector> response;

            #region Send OnPatchConnectorRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PatchConnector.IncRequests();

                //if (OnPatchConnectorRequest != null)
                //    await Task.WhenAll(OnPatchConnectorRequest.GetInvocationList().
                //                       Cast<OnPatchConnectorRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchConnectorRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Locations,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PATCH,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            LocationId. ToString() +
                                                                                                            EVSEUId.    ToString() +
                                                                                                            ConnectorId.ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = ConnectorPatch.ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPatchConnectorHTTPRequest,
                                                    ResponseLogDelegate:  OnPatchConnectorHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Connector>("",
                                                                   default,
                                                                   -1,
                                                                   "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Connector>("",
                                                               default,
                                                               -1,
                                                               e.Message,
                                                               e.StackTrace);

            }


            #region Send OnPatchConnectorResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPatchConnectorResponse != null)
                //    await Task.WhenAll(OnPatchConnectorResponse.GetInvocationList().
                //                       Cast<OnPatchConnectorResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchConnectorResponse));
            }

            #endregion

            return response;

        }

        #endregion



        #region GetTariff      (CountryCode, PartyId, TariffId, ...)

        /// <summary>
        /// Get the charging tariff specified by the given tariff identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional tariff to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Tariff>>

            GetTariff(CountryCode         CountryCode,
                      Party_Id            PartyId,
                      Tariff_Id           TariffId,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      DateTime?           Timestamp           = null,
                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id    EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Tariff> response;

            #region Send OnGetTariffRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetTariff.IncRequests();

                //if (OnGetTariffRequest != null)
                //    await Task.WhenAll(OnGetTariffRequest.GetInvocationList().
                //                       Cast<OnGetTariffRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetTariffRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Tariffs,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            TariffId.   ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnGetTariffHTTPRequest,
                                                      ResponseLogDelegate:  OnGetTariffHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Tariff>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Tariff>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnGetTariffResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetTariffResponse != null)
                //    await Task.WhenAll(OnGetTariffResponse.GetInvocationList().
                //                       Cast<OnGetTariffResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutTariff      (Tariff, ...)

        /// <summary>
        /// Put/store the given charging tariff on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional tariff to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Tariff>>

            PutTariff(Tariff              Tariff,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      DateTime?           Timestamp           = null,
                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id    EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Tariff> response;

            #region Send OnPutTariffRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PutTariff.IncRequests();

                //if (OnPutTariffRequest != null)
                //    await Task.WhenAll(OnPutTariffRequest.GetInvocationList().
                //                       Cast<OnPutTariffRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutTariffRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Tariffs,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     remoteURL.Value.Path + Tariff.CountryCode.ToString() +
                                                                                                            Tariff.PartyId.    ToString() +
                                                                                                            Tariff.Id.         ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = Tariff.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPutTariffHTTPRequest,
                                                    ResponseLogDelegate:  OnPutTariffHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Tariff>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Tariff>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnPutTariffResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPutTariffResponse != null)
                //    await Task.WhenAll(OnPutTariffResponse.GetInvocationList().
                //                       Cast<OnPutTariffResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchTariff    (CountryCode, PartyId, TariffId, TariffPatch, ...)

        /// <summary>
        /// Patch a tariff.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional tariff to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Tariff>>

            PatchTariff(CountryCode         CountryCode,
                        Party_Id            PartyId,
                        Tariff_Id           TariffId,
                        JObject             TariffPatch,

                        Request_Id?         RequestId           = null,
                        Correlation_Id?     CorrelationId       = null,
                        Version_Id?         VersionId           = null,

                        DateTime?           Timestamp           = null,
                        CancellationToken?  CancellationToken   = null,
                        EventTracking_Id    EventTrackingId     = null,
                        TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Tariff> response;

            #region Send OnPatchTariffRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PatchTariff.IncRequests();

                //if (OnPatchTariffRequest != null)
                //    await Task.WhenAll(OnPatchTariffRequest.GetInvocationList().
                //                       Cast<OnPatchTariffRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchTariffRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Tariffs,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PATCH,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            TariffId. ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = TariffPatch.ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPatchTariffHTTPRequest,
                                                    ResponseLogDelegate:  OnPatchTariffHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Tariff>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Tariff>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnPatchTariffResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPatchTariffResponse != null)
                //    await Task.WhenAll(OnPatchTariffResponse.GetInvocationList().
                //                       Cast<OnPatchTariffResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteTariff   (CountryCode, PartyId, TariffId, ...)

        /// <summary>
        /// Delete the charging tariff specified by the given tariff identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional tariff to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Tariff>>

            DeleteTariff(CountryCode         CountryCode,
                         Party_Id            PartyId,
                         Tariff_Id           TariffId,

                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,
                         Version_Id?         VersionId           = null,

                         DateTime?           Timestamp           = null,
                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Tariff> response;

            #region Send OnDeleteTariffRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.DeleteTariff.IncRequests();

                //if (OnDeleteTariffRequest != null)
                //    await Task.WhenAll(OnDeleteTariffRequest.DeleteInvocationList().
                //                       Cast<OnDeleteTariffRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnDeleteTariffRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Tariffs,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.DELETE,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            TariffId.   ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnDeleteTariffHTTPRequest,
                                                      ResponseLogDelegate:  OnDeleteTariffHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Tariff>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Tariff>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnDeleteTariffResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnDeleteTariffResponse != null)
                //    await Task.WhenAll(OnDeleteTariffResponse.DeleteInvocationList().
                //                       Cast<OnDeleteTariffResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnDeleteTariffResponse));
            }

            #endregion

            return response;

        }

        #endregion



        #region GetSession     (CountryCode, PartyId, SessionId, ...)

        /// <summary>
        /// Get the charging session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional session to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Session>>

            GetSession(CountryCode         CountryCode,
                       Party_Id            PartyId,
                       Session_Id          SessionId,

                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,
                       Version_Id?         VersionId           = null,

                       DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Session> response;

            #region Send OnGetSessionRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetSession.IncRequests();

                //if (OnGetSessionRequest != null)
                //    await Task.WhenAll(OnGetSessionRequest.GetInvocationList().
                //                       Cast<OnGetSessionRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetSessionRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Sessions,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            SessionId.   ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnGetSessionHTTPRequest,
                                                      ResponseLogDelegate:  OnGetSessionHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Session>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Session>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnGetSessionResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetSessionResponse != null)
                //    await Task.WhenAll(OnGetSessionResponse.GetInvocationList().
                //                       Cast<OnGetSessionResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutSession     (Session, ...)

        /// <summary>
        /// Put/store the given charging session on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional session to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Session>>

            PutSession(Session             Session,

                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,
                       Version_Id?         VersionId           = null,

                       DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Session> response;

            #region Send OnPutSessionRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PutSession.IncRequests();

                //if (OnPutSessionRequest != null)
                //    await Task.WhenAll(OnPutSessionRequest.GetInvocationList().
                //                       Cast<OnPutSessionRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutSessionRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Sessions,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     remoteURL.Value.Path + Session.CountryCode.ToString() +
                                                                                                            Session.PartyId.    ToString() +
                                                                                                            Session.Id.         ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = Session.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPutSessionHTTPRequest,
                                                    ResponseLogDelegate:  OnPutSessionHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Session>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Session>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnPutSessionResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPutSessionResponse != null)
                //    await Task.WhenAll(OnPutSessionResponse.GetInvocationList().
                //                       Cast<OnPutSessionResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPutSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchSession   (CountryCode, PartyId, SessionId, SessionPatch, ...)

        /// <summary>
        /// Patch a session.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional session to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Session>>

            PatchSession(CountryCode         CountryCode,
                         Party_Id            PartyId,
                         Session_Id          SessionId,
                         JObject             SessionPatch,

                         Request_Id?         RequestId           = null,
                         Correlation_Id?     CorrelationId       = null,
                         Version_Id?         VersionId           = null,

                         DateTime?           Timestamp           = null,
                         CancellationToken?  CancellationToken   = null,
                         EventTracking_Id    EventTrackingId     = null,
                         TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Session> response;

            #region Send OnPatchSessionRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PatchSession.IncRequests();

                //if (OnPatchSessionRequest != null)
                //    await Task.WhenAll(OnPatchSessionRequest.GetInvocationList().
                //                       Cast<OnPatchSessionRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchSessionRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Sessions,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PATCH,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            SessionId. ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = SessionPatch.ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPatchSessionHTTPRequest,
                                                    ResponseLogDelegate:  OnPatchSessionHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Session>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Session>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnPatchSessionResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPatchSessionResponse != null)
                //    await Task.WhenAll(OnPatchSessionResponse.GetInvocationList().
                //                       Cast<OnPatchSessionResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPatchSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region DeleteSession  (CountryCode, PartyId, SessionId, ...)

        /// <summary>
        /// Delete the charging session specified by the given session identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional session to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Session>>

            DeleteSession(CountryCode         CountryCode,
                          Party_Id            PartyId,
                          Session_Id          SessionId,

                          Request_Id?         RequestId           = null,
                          Correlation_Id?     CorrelationId       = null,
                          Version_Id?         VersionId           = null,

                          DateTime?           Timestamp           = null,
                          CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Session> response;

            #region Send OnDeleteSessionRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.DeleteSession.IncRequests();

                //if (OnDeleteSessionRequest != null)
                //    await Task.WhenAll(OnDeleteSessionRequest.DeleteInvocationList().
                //                       Cast<OnDeleteSessionRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnDeleteSessionRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Sessions,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.DELETE,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            SessionId.   ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnDeleteSessionHTTPRequest,
                                                      ResponseLogDelegate:  OnDeleteSessionHTTPResponse,
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

                else
                    response = new OCPIResponse<String, Session>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, Session>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnDeleteSessionResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnDeleteSessionResponse != null)
                //    await Task.WhenAll(OnDeleteSessionResponse.DeleteInvocationList().
                //                       Cast<OnDeleteSessionResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnDeleteSessionResponse));
            }

            #endregion

            return response;

        }

        #endregion



        #region GetCDR         (CountryCode, PartyId, CDRId, ...)   // The concrete URL is not specified by OCPI! m(

        /// <summary>
        /// Get the charge detail record specified by the given charge detail record identification from the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional charge detail record to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<CDR>>

            GetCDR(CountryCode         CountryCode,
                   Party_Id            PartyId,
                   CDR_Id              CDRId,

                   Request_Id?         RequestId           = null,
                   Correlation_Id?     CorrelationId       = null,
                   Version_Id?         VersionId           = null,

                   DateTime?           Timestamp           = null,
                   CancellationToken?  CancellationToken   = null,
                   EventTracking_Id    EventTrackingId     = null,
                   TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<CDR> response;

            #region Send OnGetCDRRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetCDR.IncRequests();

                //if (OnGetCDRRequest != null)
                //    await Task.WhenAll(OnGetCDRRequest.GetInvocationList().
                //                       Cast<OnGetCDRRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetCDRRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.CDRs,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path + CountryCode.ToString() +
                                                                                                            PartyId.    ToString() +
                                                                                                            CDRId.      ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      RequestLogDelegate:   OnGetCDRHTTPRequest,
                                                      ResponseLogDelegate:  OnGetCDRHTTPResponse,
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

                else
                    response = new OCPIResponse<String, CDR>("",
                                                                default,
                                                                -1,
                                                                "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, CDR>("",
                                                            default,
                                                            -1,
                                                            e.Message,
                                                            e.StackTrace);

            }


            #region Send OnGetCDRResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetCDRResponse != null)
                //    await Task.WhenAll(OnGetCDRResponse.GetInvocationList().
                //                       Cast<OnGetCDRResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetCDRResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PostCDR        (CDR, ...)

        /// <summary>
        /// Post/store the given charge detail record on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional charge detail record to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<CDR>>

            PostCDR(CDR                 CDR,

                    Request_Id?         RequestId           = null,
                    Correlation_Id?     CorrelationId       = null,
                    Version_Id?         VersionId           = null,

                    DateTime?           Timestamp           = null,
                    CancellationToken?  CancellationToken   = null,
                    EventTracking_Id    EventTrackingId     = null,
                    TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<CDR> response;

            #region Send OnPostCDRRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PostCDR.IncRequests();

                //if (OnPostCDRRequest != null)
                //    await Task.WhenAll(OnPostCDRRequest.GetInvocationList().
                //                       Cast<OnPostCDRRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPostCDRRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.CDRs,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path + CDR.CountryCode.ToString() +    // <= Unclear if this URL is correct!
                                                                                                            CDR.PartyId.    ToString() +
                                                                                                            CDR.Id.         ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = CDR.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnPostCDRHTTPRequest,
                                                    ResponseLogDelegate:  OnPostCDRHTTPResponse,
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

                else
                    response = new OCPIResponse<String, CDR>("",
                                                             default,
                                                             -1,
                                                             "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, CDR>("",
                                                         default,
                                                         -1,
                                                         e.Message,
                                                         e.StackTrace);

            }


            #region Send OnPostCDRResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPostCDRResponse != null)
                //    await Task.WhenAll(OnPostCDRResponse.GetInvocationList().
                //                       Cast<OnPostCDRResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPostCDRResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetTokens      (...)

        /// <summary>
        /// Get all tokens from the remote API.
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
        public async Task<OCPIResponse<IEnumerable<Token>>>

            GetTokens(Version_Id?         VersionId           = null,
                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,

                      DateTime?           Timestamp           = null,
                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id    EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Token>> response;

            #region Send OnGetTokensRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.GetTokens.IncRequests();

                //if (OnGetTokensRequest != null)
                //    await Task.WhenAll(OnGetTokensRequest.GetInvocationList().
                //                       Cast<OnGetTokensRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetTokensRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Tokens,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                                     remoteURL.Value.Path,
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                    RequestLogDelegate:   OnGetTokensHTTPRequest,
                                                    ResponseLogDelegate:  OnGetTokensHTTPResponse,
                                                    CancellationToken:    CancellationToken,
                                                    EventTrackingId:      EventTrackingId,
                                                    RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<Token>.ParseJArray(HTTPResponse,
                                                             requestId,
                                                             correlationId,
                                                             json => Token.Parse(json));

                }

                else
                    response = new OCPIResponse<String, IEnumerable<Token>>("",
                                                                            default,
                                                                            -1,
                                                                            "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, IEnumerable<Token>>("",
                                                                        default,
                                                                        -1,
                                                                        e.Message,
                                                                        e.StackTrace);

            }


            #region Send OnGetTokensResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnGetTokensResponse != null)
                //    await Task.WhenAll(OnGetTokensResponse.GetInvocationList().
                //                       Cast<OnGetTokensResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnGetTokensResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PostToken      (Token, TokenType = null, ...)

        /// <summary>
        /// Post/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Token>>

            PostToken(Token               Token,
                      TokenTypes?         TokenType           = null,
                      LocationReference?  LocationReference   = null,

                      Request_Id?         RequestId           = null,
                      Correlation_Id?     CorrelationId       = null,
                      Version_Id?         VersionId           = null,

                      DateTime?           Timestamp           = null,
                      CancellationToken?  CancellationToken   = null,
                      EventTracking_Id    EventTrackingId     = null,
                      TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<Token> response;

            #region Send OnPostTokenRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.PostToken.IncRequests();

                //if (OnPostTokenRequest != null)
                //    await Task.WhenAll(OnPostTokenRequest.GetInvocationList().
                //                       Cast<OnPostTokenRequestDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPostTokenRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.Tokens,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.POST,
                                                                                     remoteURL.Value.Path + Token.Id.ToString() + "authorize",
                                                                                     requestbuilder => {

                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);

                                                                                         if (TokenType.HasValue)
                                                                                             requestbuilder.QueryString.Add("type", TokenType.ToString());

                                                                                         if (LocationReference.HasValue)
                                                                                         {
                                                                                             requestbuilder.ContentType = HTTPContentType.JSON_UTF8;
                                                                                             requestbuilder.Content     = LocationReference.Value.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         }

                                                                                     }),

                                                    RequestLogDelegate:   OnPostTokenHTTPRequest,
                                                    ResponseLogDelegate:  OnPostTokenHTTPResponse,
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


            #region Send OnPostTokenResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnPostTokenResponse != null)
                //    await Task.WhenAll(OnPostTokenResponse.GetInvocationList().
                //                       Cast<OnPostTokenResponseDelegate>().
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
                e.Log(nameof(CPOClient) + "." + nameof(OnPostTokenResponse));
            }

            #endregion

            return response;

        }

        #endregion


        // Commands

        #region SetChargingProfile(Token, ExpiryDate, ReservationId, LocationId, EVSEUId, AuthorizationReference, ...)

        /// <summary>
        /// Put/store the given token on/within the remote API.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<ChargingProfileResponse>>

            SetChargingProfile(Session_Id          SessionId,
                               ChargingProfile     ChargingProfile,

                               Request_Id?         RequestId           = null,
                               Correlation_Id?     CorrelationId       = null,
                               Version_Id?         VersionId           = null,

                               DateTime?           Timestamp           = null,
                               CancellationToken?  CancellationToken   = null,
                               EventTracking_Id    EventTrackingId     = null,
                               TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<ChargingProfileResponse> response;

            var Command = new SetChargingProfileCommand(ChargingProfile,
                                                        MyCommandsURL + "SET_CHARGING_PROFILE" + random.RandomString(50));

            #region Send OnSetChargingProfileRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                //Counters.SetChargingProfile.IncRequests();

                //if (OnSetChargingProfileRequest != null)
                //    await Task.WhenAll(OnSetChargingProfileRequest.GetInvocationList().
                //                       Cast<OnSetChargingProfileRequestDelegate>().
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnSetChargingProfileRequest));
            }

            #endregion


            try
            {

                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();
                var remoteURL      = await GetRemoteURL(VersionId,
                                                        ModuleIDs.ChargingProfiles,
                                                        InterfaceRoles.RECEIVER);

                if (remoteURL.HasValue)
                {

                    #region Upstream HTTP request...

                    var HTTPResponse = await (remoteURL.Value.Protocol == HTTPProtocols.http

                                                  ? new HTTPClient (remoteURL.Value.Hostname,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTP,
                                                                    DNSClient:   DNSClient)

                                                  : new HTTPSClient(remoteURL.Value.Hostname,
                                                                    RemoteCertificateValidator,
                                                                    RemotePort:  remoteURL.Value.Port ?? IPPort.HTTPS,
                                                                    DNSClient:   DNSClient)).

                                              Execute(client => client.CreateRequest(HTTPMethod.PUT,
                                                                                     remoteURL.Value.Path + SessionId.ToString(),
                                                                                     requestbuilder => {
                                                                                         requestbuilder.Authorization = TokenAuth;
                                                                                         requestbuilder.ContentType   = HTTPContentType.JSON_UTF8;
                                                                                         requestbuilder.Content       = Command.ToJSON().ToUTF8Bytes(JSONFormat);
                                                                                         requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                         requestbuilder.Set("X-Request-ID",      requestId);
                                                                                         requestbuilder.Set("X-Correlation-ID",  correlationId);
                                                                                     }),

                                                      //RequestLogDelegate:   OnSetChargingProfileHTTPRequest,
                                                      //ResponseLogDelegate:  OnSetChargingProfileHTTPResponse,
                                                      CancellationToken:    CancellationToken,
                                                      EventTrackingId:      EventTrackingId,
                                                      RequestTimeout:       RequestTimeout ?? this.RequestTimeout).

                                              ConfigureAwait(false);

                    #endregion

                    response = OCPIResponse<ChargingProfileResponse>.ParseJObject(HTTPResponse,
                                                                                  requestId,
                                                                                  correlationId,
                                                                                  json => ChargingProfileResponse.Parse(json));

                }

                else
                    response = new OCPIResponse<String, ChargingProfileResponse>("",
                                                                                 default,
                                                                                 -1,
                                                                                 "No remote URL available!");

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, ChargingProfileResponse>("",
                                                                             default,
                                                                             -1,
                                                                             e.Message,
                                                                             e.StackTrace);

            }


            #region Send OnSetChargingProfileResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                // Update counters
                //if (response.HTTPStatusCode == HTTPStatusCode.OK && response.Content.RequestStatus.Code == 1)
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_OK();
                //else
                //    Counters.SetChargingPoolAvailabilityStatus.IncResponses_Error();


                //if (OnSetChargingProfileResponse != null)
                //    await Task.WhenAll(OnSetChargingProfileResponse.GetInvocationList().
                //                       Cast<OnSetChargingProfileResponseDelegate>().
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnSetChargingProfileResponse));
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
