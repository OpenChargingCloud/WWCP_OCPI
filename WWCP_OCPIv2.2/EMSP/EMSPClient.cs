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
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;

using org.GraphDefined.WWCP;

using cloud.charging.open.protocols.OCPIv2_2.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    #region OnGetLocationsRequest/-Response

    /// <summary>
    /// A delegate called whenever a get locations request will be send.
    /// </summary>
    public delegate Task OnGetLocationsRequestDelegate(DateTime                                    LogTimestamp,
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
                                                           EVSE_Id                                 EVSEId,

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
                                                           EVSE_Id                                 EVSEId,

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
    public delegate Task OnGetTariffsRequestDelegate(DateTime                                    LogTimestamp,
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


    /// <summary>
    /// The EMSP client.
    /// </summary>
    public partial class EMSPClient : CommonClient
    {

        public class EMSPCounters
        {

            public CounterValues  GetLocations    { get; }

            public EMSPCounters(CounterValues? GetLocations = null)
            {

                this.GetLocations = GetLocations ?? new CounterValues();

            }

            public JObject ToJSON()

                => JSONObject.Create(
                       new JProperty("GetLocations", GetLocations.ToJSON())

                    
                   );

        }


        #region Data

        #endregion

        #region Properties

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
        public event ClientRequestLogHandler        OnGetEVSEByUIdHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a EVSE by its identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler       OnGetEVSEByUIdHTTPResponse;

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
        public event ClientRequestLogHandler         OnGetTariffsHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting all tariffs HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnGetTariffsHTTPResponse;

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
        public event ClientRequestLogHandler         OnGetTariffByIdHTTPRequest;

        /// <summary>
        /// An event fired whenever a response to a getting a tariff by it identification HTTP request had been received.
        /// </summary>
        public event ClientResponseLogHandler        OnGetTariffByIdHTTPResponse;

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

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new EMSP client.
        /// </summary>
        /// <param name="AccessToken">The access token.</param>
        /// <param name="URL">The remote URL to connect to.</param>
        /// <param name="VirtualHostname">An optional HTTP virtual hostname.</param>
        /// <param name="RemoteCertificateValidator">An optional remote SSL/TLS certificate validator.</param>
        /// <param name="RequestTimeout">An optional request timeout.</param>
        /// <param name="MaxNumberOfRetries">The maximum number of transmission retries.</param>
        /// <param name="DNSClient">An optional DNS client to use.</param>
        public EMSPClient(AccessToken                          AccessToken,
                          URL                                  URL,
                          HTTPHostname?                        VirtualHostname              = null,
                          RemoteCertificateValidationCallback  RemoteCertificateValidator   = null,
                          TimeSpan?                            RequestTimeout               = null,
                          Byte?                                MaxNumberOfRetries           = null,
                          DNSClient                            DNSClient                    = null)

            : base(AccessToken,
                   URL,
                   VirtualHostname,
                   RemoteCertificateValidator,
                   RequestTimeout,
                   MaxNumberOfRetries,
                   DNSClient)

        { }

        #endregion


        #region GetLocations(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetLocationsRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    LocationsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Locations &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (LocationsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (LocationsURL.Hostname,
                                                             RemotePort:  LocationsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(LocationsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  LocationsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationsURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetLocationsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetLocationById(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetLocationByIdRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    LocationsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Locations &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (LocationsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (LocationsURL.Hostname,
                                                             RemotePort:  LocationsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(LocationsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  LocationsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationsURL.Path + LocationId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetLocationByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetEVSEByUId(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetEVSEByUIdRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    LocationsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Locations &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (LocationsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (LocationsURL.Hostname,
                                                             RemotePort:  LocationsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(LocationsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  LocationsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationsURL.Path + LocationId.ToString() + EVSEUId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetEVSEByUIdResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetConnectorById(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<Connector>>

            GetConnectorById(Location_Id         LocationId,
                             EVSE_Id             EVSEId,
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetConnectorByIdRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    LocationsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Locations &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (LocationsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (LocationsURL.Hostname,
                                                             RemotePort:  LocationsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(LocationsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  LocationsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               LocationsURL.Path + LocationId.ToString() + EVSEId.ToString() + ConnectorId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetConnectorByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetTariffs(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetTariffsRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    TariffsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Tariffs &&
                                                                                     endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (TariffsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (TariffsURL.Hostname,
                                                             RemotePort:  TariffsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(TariffsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  TariffsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               TariffsURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetTariffsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetTariffById(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Tariff>>>

            GetTariffById(Tariff_Id         TariffId,
                          Version_Id?         VersionId           = null,
                          Request_Id?         RequestId           = null,
                          Correlation_Id?     CorrelationId       = null,

                          DateTime?           Timestamp           = null,
                          CancellationToken?  CancellationToken   = null,
                          EventTracking_Id    EventTrackingId     = null,
                          TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Tariff>> response;

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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetTariffByIdRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    TariffsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Tariffs &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (TariffsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (TariffsURL.Hostname,
                                                             RemotePort:  TariffsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(TariffsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  TariffsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               TariffsURL.Path + TariffId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetTariffByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetSessions(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetSessionsRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    SessionsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Sessions &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (SessionsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (SessionsURL.Hostname,
                                                             RemotePort:  SessionsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(SessionsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  SessionsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               SessionsURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetSessionsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetSessionById(...)

        /// <summary>
        /// Start a charging session.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Session>>>

            GetSessionById(Session_Id         SessionId,
                            Version_Id?         VersionId           = null,
                            Request_Id?         RequestId           = null,
                            Correlation_Id?     CorrelationId       = null,

                            DateTime?           Timestamp           = null,
                            CancellationToken?  CancellationToken   = null,
                            EventTracking_Id    EventTrackingId     = null,
                            TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Session>> response;

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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetSessionByIdRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    SessionsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Sessions &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (SessionsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (SessionsURL.Hostname,
                                                             RemotePort:  SessionsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(SessionsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  SessionsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               SessionsURL.Path + SessionId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetSessionByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion

        // PUT     ~/sessions/{session_id}/charging_preferences


        #region GetCDRs(...)

        /// <summary>
        /// Start a charging CDR.
        /// </summary>
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetCDRsRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    CDRsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.CDRs &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (CDRsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (CDRsURL.Hostname,
                                                             RemotePort:  CDRsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(CDRsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  CDRsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               CDRsURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetCDRsResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region GetCDRById(...)

        /// <summary>
        /// Start a charging CDR.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<CDR>>>

            GetCDRById(CDR_Id         CDRId,
                       Version_Id?         VersionId           = null,
                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,

                       DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<CDR>> response;

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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetCDRByIdRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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

                    CDRsURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.CDRs &&
                                                                                       endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (CDRsURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (CDRsURL.Hostname,
                                                             RemotePort:  CDRsURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(CDRsURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  CDRsURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               CDRsURL.Path + CDRId.ToString(),
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetCDRByIdResponse));
            }

            #endregion

            return response;

        }

        #endregion


        #region GetToken(...)

        /// <summary>
        /// Start a charging token.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Token>>>

            GetToken(Version_Id?         VersionId           = null,
                     Request_Id?         RequestId           = null,
                     Correlation_Id?     CorrelationId       = null,

                     DateTime?           Timestamp           = null,
                     CancellationToken?  CancellationToken   = null,
                     EventTracking_Id    EventTrackingId     = null,
                     TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Token>> response;

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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetTokenRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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


                URL TokenURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    TokenURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Tokens &&
                                                                                   endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (TokenURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (TokenURL.Hostname,
                                                             RemotePort:  TokenURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(TokenURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  TokenURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               TokenURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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

                response = OCPIResponse<Token>.ParseJArray(HTTPResponse,
                                                              requestId,
                                                              correlationId,
                                                              json => Token.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, IEnumerable<Token>>("",
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnGetTokenResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PutToken(...)

        /// <summary>
        /// Start a charging token.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Token>>>

            PutToken(Version_Id?         VersionId           = null,
                     Request_Id?         RequestId           = null,
                     Correlation_Id?     CorrelationId       = null,

                     DateTime?           Timestamp           = null,
                     CancellationToken?  CancellationToken   = null,
                     EventTracking_Id    EventTrackingId     = null,
                     TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Token>> response;

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
                e.Log(nameof(EMSPClient) + "." + nameof(OnPutTokenRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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


                URL TokenURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    TokenURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Tokens &&
                                                                                   endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (TokenURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (TokenURL.Hostname,
                                                             RemotePort:  TokenURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(TokenURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  TokenURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               TokenURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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

                response = OCPIResponse<Token>.ParseJArray(HTTPResponse,
                                                              requestId,
                                                              correlationId,
                                                              json => Token.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, IEnumerable<Token>>("",
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnPutTokenResponse));
            }

            #endregion

            return response;

        }

        #endregion

        #region PatchToken(...)

        /// <summary>
        /// Start a charging token.
        /// </summary>
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<OCPIResponse<IEnumerable<Token>>>

            PatchToken(Version_Id?         VersionId           = null,
                       Request_Id?         RequestId           = null,
                       Correlation_Id?     CorrelationId       = null,

                       DateTime?           Timestamp           = null,
                       CancellationToken?  CancellationToken   = null,
                       EventTracking_Id    EventTrackingId     = null,
                       TimeSpan?           RequestTimeout      = null)

        {

            OCPIResponse<IEnumerable<Token>> response;

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
                e.Log(nameof(EMSPClient) + "." + nameof(OnPatchTokenRequest));
            }

            #endregion

            try
            {

                var versionId      = VersionId     ?? SelectedOCPIVersionId;
                var requestId      = RequestId     ?? Request_Id.Random();
                var correlationId  = CorrelationId ?? Correlation_Id.Random();

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


                URL TokenURL   = default;

                if (versionId.HasValue &&
                    VersionDetails.TryGetValue(versionId.Value, out VersionDetail versionDetails))
                {

                    TokenURL = versionDetails.Endpoints.FirstOrDefault(endpoint => endpoint.Identifier == ModuleIDs.Tokens &&
                                                                                   endpoint.Role       == InterfaceRoles.SENDER).URL;

                }


                #region Upstream HTTP request...

                var HTTPResponse = await (TokenURL.Protocol == HTTPProtocols.http

                                           ? new HTTPClient (TokenURL.Hostname,
                                                             RemotePort:  TokenURL.Port ?? IPPort.HTTP,
                                                             DNSClient:   DNSClient)

                                           : new HTTPSClient(TokenURL.Hostname,
                                                             RemoteCertificateValidator,
                                                             RemotePort:  TokenURL.Port ?? IPPort.HTTPS,
                                                             DNSClient:   DNSClient)).

                                        Execute(client => client.CreateRequest(HTTPMethod.GET,
                                                                               TokenURL.Path,
                                                                               requestbuilder => {
                                                                                   requestbuilder.Authorization = TokenAuth;
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

                response = OCPIResponse<Token>.ParseJArray(HTTPResponse,
                                                              requestId,
                                                              correlationId,
                                                              json => Token.Parse(json));

            }

            catch (Exception e)
            {

                response = new OCPIResponse<String, IEnumerable<Token>>("",
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
                e.Log(nameof(EMSPClient) + "." + nameof(OnPatchTokenResponse));
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
