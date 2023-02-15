/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Receive charging stations downstream from an OCPI partner...
    /// </summary>
    public class OCPICSOAdapter : AWWCPEMPAdapter<ChargeDetailRecord>,
                                  IEMPRoamingProvider,
                                  IEquatable <OCPICSOAdapter>,
                                  IComparable<OCPICSOAdapter>,
                                  IComparable
    {

        #region Properties

        public Boolean                                      PullEVSEData_IsDisabled              { get; set; }

        public HTTPServer                                   HTTPServer                           { get; }
        public HTTPPath                                     HTTPPathPrefix                       { get; }


        public WWCPEVSEId_2_EVSEId_Delegate?                CustomEVSEIdConverter                { get; }
        public WWCPEVSE_2_EVSE_Delegate?                    CustomEVSEConverter                  { get; }
        public WWCPEVSEStatusUpdate_2_StatusType_Delegate?  CustomEVSEStatusUpdateConverter      { get; }
        public WWCPChargeDetailRecord_2_CDR_Delegate?       CustomChargeDetailRecordConverter    { get; }

        #endregion

        #region Events

        // from OCPI CSO
        public event OnAuthorizeStartRequestDelegate?           OnAuthorizeStartRequest;
        public event OnAuthorizeStartResponseDelegate?          OnAuthorizeStartResponse;

        public event OnAuthorizeStopRequestDelegate?            OnAuthorizeStopRequest;
        public event OnAuthorizeStopResponseDelegate?           OnAuthorizeStopResponse;

        public event OnNewChargingSessionDelegate?              OnNewChargingSession;

        public event OnSendCDRsRequestDelegate?                 OnChargeDetailRecordRequest;
        public event OnSendCDRsResponseDelegate?                OnChargeDetailRecordResponse;
        public event OnNewChargeDetailRecordDelegate?           OnNewChargeDetailRecord;



        // from OCPI EMSP
        public event OnReserveRequestDelegate?                  OnReserveRequest;
        public event OnReserveResponseDelegate?                 OnReserveResponse;
        public event OnNewReservationDelegate?                  OnNewReservation;

        public event WWCP.OnCancelReservationRequestDelegate?   OnCancelReservationRequest;
        public event WWCP.OnCancelReservationResponseDelegate?  OnCancelReservationResponse;
        public event OnReservationCanceledDelegate?             OnReservationCanceled;

        public event OnRemoteStartRequestDelegate?              OnRemoteStartRequest;
        public event OnRemoteStartResponseDelegate?             OnRemoteStartResponse;

        public event OnRemoteStopRequestDelegate?               OnRemoteStopRequest;
        public event OnRemoteStopResponseDelegate?              OnRemoteStopResponse;

        public event WWCP.OnGetCDRsRequestDelegate?             OnGetChargeDetailRecordsRequest;
        public event WWCP.OnGetCDRsResponseDelegate?            OnGetChargeDetailRecordsResponse;
        public event OnSendCDRsResponseDelegate?                OnSendCDRsResponse;

        #endregion


        #region Constructor(s)

        public OCPICSOAdapter(EMPRoamingProvider_Id                        Id,
                              I18NString                                   Name,
                              I18NString                                   Description,
                              RoamingNetwork                               RoamingNetwork,

                              HTTPServer                                   HTTPServer,
                              HTTPPath                                     HTTPPathPrefix,

                              WWCPEVSEId_2_EVSEId_Delegate?                CustomEVSEIdConverter               = null,
                              WWCPEVSE_2_EVSE_Delegate?                    CustomEVSEConverter                 = null,
                              WWCPEVSEStatusUpdate_2_StatusType_Delegate?  CustomEVSEStatusUpdateConverter     = null,
                              WWCPChargeDetailRecord_2_CDR_Delegate?       CustomChargeDetailRecordConverter   = null,

                              IncludeEVSEIdDelegate?                       IncludeEVSEIds                      = null,
                              IncludeEVSEDelegate?                         IncludeEVSEs                        = null,
                              IncludeChargingPoolIdDelegate?               IncludeChargingPoolIds              = null,
                              IncludeChargingPoolDelegate?                 IncludeChargingPools                = null,
                              ChargeDetailRecordFilterDelegate?            ChargeDetailRecordFilter            = null,

                              TimeSpan?                                    ServiceCheckEvery                   = null,
                              TimeSpan?                                    StatusCheckEvery                    = null,
                              TimeSpan?                                    CDRCheckEvery                       = null,

                              Boolean                                      DisablePushData                     = false,
                              Boolean                                      DisablePushStatus                   = false,
                              Boolean                                      DisableAuthentication               = false,
                              Boolean                                      DisableSendChargeDetailRecords      = false)

            : base(Id,
                   RoamingNetwork,
                   Name,
                   Description,

                   IncludeEVSEIds,
                   IncludeEVSEs,
                   null,
                   null,
                   IncludeChargingPoolIds,
                   IncludeChargingPools,
                   ChargeDetailRecordFilter,

                   ServiceCheckEvery,
                   StatusCheckEvery,
                   CDRCheckEvery,

                   DisablePushData,
                   DisablePushStatus,
                   DisableAuthentication,
                   DisableSendChargeDetailRecords)

        {

            this.HTTPServer                         = HTTPServer;
            this.HTTPPathPrefix                     = HTTPPathPrefix;

            this.CustomEVSEIdConverter              = CustomEVSEIdConverter;
            this.CustomEVSEConverter                = CustomEVSEConverter;
            this.CustomEVSEStatusUpdateConverter    = CustomEVSEStatusUpdateConverter;
            this.CustomChargeDetailRecordConverter  = CustomChargeDetailRecordConverter;

        }

        #endregion


        #region PullEVSEData

        public Task<POIDataPull<cloud.charging.open.protocols.WWCP.EVSE>> PullEVSEData(DateTime? LastCall = null, GeoCoordinate? SearchCenter = null, float DistanceKM = 0, EMobilityProvider_Id? ProviderId = null, IEnumerable<ChargingStationOperator_Id> OperatorIdFilter = null, IEnumerable<Country> CountryCodeFilter = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PullEVSEStatus

        public Task<StatusPull<EVSEStatus>> PullEVSEStatus(DateTime? LastCall = null, GeoCoordinate? SearchCenter = null, float DistanceKM = 0, EVSEStatusTypes? EVSEStatusFilter = null, EMobilityProvider_Id? ProviderId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion



        public IEnumerable<ChargingReservation> ChargingReservations
            => throw new NotImplementedException();

        public bool TryGetChargingReservationById(ChargingReservation_Id ReservationId, out ChargingReservation ChargingReservation)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingSessionById(ChargingSession_Id ChargingSessionId, out ChargingSession ChargingSession)
        {
            throw new NotImplementedException();
        }






        public IEnumerable<ChargingSession> ChargingSessions
            => throw new NotImplementedException();

        public TimeSpan MaxReservationDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IId AuthId => throw new NotImplementedException();

        IId ISendChargeDetailRecords.Id => throw new NotImplementedException();

        public Task<IEnumerable<ChargeDetailRecord>> GetChargeDetailRecords(DateTime From, DateTime? To = null, EMobilityProvider_Id? ProviderId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }





        #region (Set/Add/Update/Delete) Roaming network...
        public Task<PushEVSEDataResult> SetStaticData(RoamingNetwork RoamingNetwork, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> AddStaticData(RoamingNetwork RoamingNetwork, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> UpdateStaticData(RoamingNetwork RoamingNetwork, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> DeleteStaticData(RoamingNetwork RoamingNetwork, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushRoamingNetworkAdminStatusResult> UpdateAdminStatus(IEnumerable<RoamingNetworkAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushRoamingNetworkStatusResult> UpdateStatus(IEnumerable<RoamingNetworkStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging station operator(s)...

        public Task<PushEVSEDataResult> SetStaticData(ChargingStationOperator ChargingStationOperator, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> AddStaticData(ChargingStationOperator ChargingStationOperator, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> UpdateStaticData(ChargingStationOperator ChargingStationOperator, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> DeleteStaticData(ChargingStationOperator ChargingStationOperator, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }



        public Task<PushEVSEDataResult> SetStaticData(IEnumerable<ChargingStationOperator> ChargingStationOperators, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> AddStaticData(IEnumerable<ChargingStationOperator> ChargingStationOperators, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> UpdateStaticData(IEnumerable<ChargingStationOperator> ChargingStationOperators, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> DeleteStaticData(IEnumerable<ChargingStationOperator> ChargingStationOperators, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }



        public Task<PushChargingStationOperatorAdminStatusResult> UpdateAdminStatus(IEnumerable<ChargingStationOperatorAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationOperatorStatusResult> UpdateStatus(IEnumerable<ChargingStationOperatorStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging pool(s)...

        #region SetStaticData   (ChargingPool, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.SetStaticData(WWCP.IChargingPool      ChargingPool,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            #region Enqueue, if requested...

            if (TransmissionType == WWCP.TransmissionTypes.Enqueue)
            {

                #region Send OnEnqueueSendCDRRequest event

                //try
                //{

                //    OnEnqueueSendCDRRequest?.Invoke(Timestamp.Now,
                //                                    Timestamp.Value,
                //                                    this,
                //                                    EventTrackingId,
                //                                    RoamingNetwork.Id,
                //                                    ChargeDetailRecord,
                //                                    RequestTimeout);

                //}
                //catch (Exception e)
                //{
                //    DebugX.LogException(e, nameof(WWCPEMPAdapter) + "." + nameof(OnSendCDRRequest));
                //}

                #endregion

                var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

                try
                {

                    if (lockTaken)
                    {

                        if (IncludeChargingPools is null ||
                           (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                        {

                            ChargingPoolsToAddQueue.Add(ChargingPool);

                            FlushEVSEDataAndStatusTimer.Change(FlushEVSEDataAndStatusEvery,
                                                               TimeSpan.FromMilliseconds(-1));

                        }

                    }

                }
                finally
                {
                    if (lockTaken)
                        DataAndStatusLock.Release();
                }

                return lockTaken
                           ? WWCP.PushChargingPoolDataResult.Enqueued   (Id, this, new IChargingPool[] { ChargingPool })
                           : WWCP.PushChargingPoolDataResult.LockTimeout(Id, this, new IChargingPool[] { ChargingPool });

            }

            #endregion

            throw new NotImplementedException();

            //return await PushEVSEData(ChargingPool.EVSEs,
            //                          ActionTypes.Insert,
            //                          null,

            //                          Timestamp,
            //                          CancellationToken,
            //                          EventTrackingId,
            //                          RequestTimeout).

            //                          ConfigureAwait(false);

        }

        #endregion

        #region AddStaticData   (ChargingPool, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.AddStaticData(WWCP.IChargingPool      ChargingPool,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            #region Enqueue, if requested...

            if (TransmissionType == WWCP.TransmissionTypes.Enqueue)
            {

                #region Send OnEnqueueSendCDRRequest event

                //try
                //{

                //    OnEnqueueSendCDRRequest?.Invoke(Timestamp.Now,
                //                                    Timestamp.Value,
                //                                    this,
                //                                    EventTrackingId,
                //                                    RoamingNetwork.Id,
                //                                    ChargeDetailRecord,
                //                                    RequestTimeout);

                //}
                //catch (Exception e)
                //{
                //    DebugX.LogException(e, nameof(WWCPEMPAdapter) + "." + nameof(OnSendCDRRequest));
                //}

                #endregion

                var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

                try
                {

                    if (lockTaken)
                    {

                        if (IncludeChargingPools is null ||
                           (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                        {

                            ChargingPoolsToAddQueue.Add(ChargingPool);

                            FlushEVSEDataAndStatusTimer.Change(FlushEVSEDataAndStatusEvery,
                                                               TimeSpan.FromMilliseconds(-1));

                        }

                    }

                }
                finally
                {
                    if (lockTaken)
                        DataAndStatusLock.Release();
                }

                return lockTaken
                           ? WWCP.PushChargingPoolDataResult.Enqueued   (Id, this, new IChargingPool[] { ChargingPool })
                           : WWCP.PushChargingPoolDataResult.LockTimeout(Id, this, new IChargingPool[] { ChargingPool });

            }

            #endregion

            throw new NotImplementedException();

            //return await PushEVSEData(ChargingPool.EVSEs,
            //                          ActionTypes.Insert,
            //                          null,

            //                          Timestamp,
            //                          CancellationToken,
            //                          EventTrackingId,
            //                          RequestTimeout).

            //                          ConfigureAwait(false);

        }

        #endregion

        public Task<PushChargingPoolDataResult> UpdateStaticData(IChargingPool ChargingPool, String? PropertyName = null, Object? OldValue = null, Object? NewValue = null, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolDataResult> DeleteStaticData(IChargingPool ChargingPool, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushChargingPoolDataResult> SetStaticData(IEnumerable<IChargingPool> ChargingPools, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolDataResult> AddStaticData(IEnumerable<IChargingPool> ChargingPools, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolDataResult> UpdateStaticData(IEnumerable<IChargingPool> ChargingPools, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolDataResult> DeleteStaticData(IEnumerable<IChargingPool> ChargingPools, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushChargingPoolAdminStatusResult> UpdateAdminStatus(IEnumerable<ChargingPoolAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolStatusResult> UpdateStatus(IEnumerable<ChargingPoolStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging station(s)...

        #region SetStaticData   (ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given charging station.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.SetStaticData(WWCP.IChargingStation   ChargingStation,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            #region Enqueue, if requested...

            if (TransmissionType == WWCP.TransmissionTypes.Enqueue)
            {

                #region Send OnEnqueueSendCDRRequest event

                //try
                //{

                //    OnEnqueueSendCDRRequest?.Invoke(Timestamp.Now,
                //                                    Timestamp.Value,
                //                                    this,
                //                                    EventTrackingId,
                //                                    RoamingNetwork.Id,
                //                                    ChargeDetailRecord,
                //                                    RequestTimeout);

                //}
                //catch (Exception e)
                //{
                //    DebugX.LogException(e, nameof(WWCPEMPAdapter) + "." + nameof(OnSendCDRRequest));
                //}

                #endregion

                var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

                try
                {

                    if (lockTaken)
                    {

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                EVSEsToAddQueue.Add(evse);

                                FlushEVSEDataAndStatusTimer.Change(FlushEVSEDataAndStatusEvery,
                                                                   TimeSpan.FromMilliseconds(-1));

                            }

                        }

                    }

                }
                finally
                {
                    if (lockTaken)
                        DataAndStatusLock.Release();
                }

                return lockTaken
                           ? WWCP.PushChargingStationDataResult.Enqueued   (Id, this, new IChargingStation[] { ChargingStation })
                           : WWCP.PushChargingStationDataResult.LockTimeout(Id, this, new IChargingStation[] { ChargingStation });

            }

            #endregion

            throw new NotImplementedException();

            //return await PushEVSEData(ChargingStation.EVSEs,
            //                          ActionTypes.Insert,
            //                          null,

            //                          Timestamp,
            //                          CancellationToken,
            //                          EventTrackingId,
            //                          RequestTimeout).

            //                          ConfigureAwait(false);

        }

        #endregion

        #region AddStaticData   (ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given charging station.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.AddStaticData(WWCP.IChargingStation    ChargingStation,
                                            WWCP.TransmissionTypes   TransmissionType,

                                            DateTime?                Timestamp,
                                            CancellationToken?       CancellationToken,
                                            EventTracking_Id?        EventTrackingId,
                                            TimeSpan?                RequestTimeout)

        {

            #region Enqueue, if requested...

            if (TransmissionType == WWCP.TransmissionTypes.Enqueue)
            {

                #region Send OnEnqueueSendCDRRequest event

                //try
                //{

                //    OnEnqueueSendCDRRequest?.Invoke(Timestamp.Now,
                //                                    Timestamp.Value,
                //                                    this,
                //                                    EventTrackingId,
                //                                    RoamingNetwork.Id,
                //                                    ChargeDetailRecord,
                //                                    RequestTimeout);

                //}
                //catch (Exception e)
                //{
                //    DebugX.LogException(e, nameof(WWCPEMPAdapter) + "." + nameof(OnSendCDRRequest));
                //}

                #endregion

                var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

                try
                {

                    if (lockTaken)
                    {

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                EVSEsToAddQueue.Add(evse);

                                FlushEVSEDataAndStatusTimer.Change(FlushEVSEDataAndStatusEvery,
                                                                   TimeSpan.FromMilliseconds(-1));

                            }

                        }

                    }

                }
                finally
                {
                    if (lockTaken)
                        DataAndStatusLock.Release();
                }

                return lockTaken
                           ? WWCP.PushChargingStationDataResult.Enqueued   (Id, this, new IChargingStation[] { ChargingStation })
                           : WWCP.PushChargingStationDataResult.LockTimeout(Id, this, new IChargingStation[] { ChargingStation });

            }

            #endregion

            throw new NotImplementedException();

            //return await PushEVSEData(ChargingStation.EVSEs,
            //                          ActionTypes.Insert,
            //                          null,

            //                          Timestamp,
            //                          CancellationToken,
            //                          EventTrackingId,
            //                          RequestTimeout).

            //                          ConfigureAwait(false);

        }

        #endregion

        public Task<PushChargingStationDataResult> UpdateStaticData(IChargingStation ChargingStation, String? PropertyName = null, Object? OldValue = null, Object? NewValue = null, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> DeleteStaticData(IChargingStation ChargingStation, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushChargingStationDataResult> SetStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> AddStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> UpdateStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> DeleteStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushChargingStationAdminStatusResult> UpdateAdminStatus(IEnumerable<ChargingStationAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationStatusResult> UpdateStatus(IEnumerable<ChargingStationStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region (Set/Add/Update/Delete) EVSE(s)...

        public Task<PushEVSEDataResult> SetStaticData(IEVSE EVSE, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> AddStaticData(IEVSE EVSE, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> UpdateStaticData(IEVSE EVSE, String? PropertyName = null, Object? OldValue = null, Object? NewValue = null, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> DeleteStaticData(IEVSE EVSE, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushEVSEDataResult> SetStaticData(IEnumerable<IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> AddStaticData(IEnumerable<IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> UpdateStaticData(IEnumerable<IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> DeleteStaticData(IEnumerable<IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushEVSEAdminStatusResult> UpdateAdminStatus(IEnumerable<EVSEAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEStatusResult> UpdateStatus(IEnumerable<EVSEStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region AuthorizeStart/-Stop

        public Task<AuthStartResult> AuthorizeStart(LocalAuthentication LocalAuthentication, ChargingLocation ChargingLocation = null, ChargingProduct ChargingProduct = null, ChargingSession_Id? SessionId = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<AuthStopResult> AuthorizeStop(ChargingSession_Id SessionId, LocalAuthentication LocalAuthentication, ChargingLocation ChargingLocation = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Reserve

        public Task<ReservationResult> Reserve(ChargingLocation ChargingLocation, ChargingReservationLevel ReservationLevel = ChargingReservationLevel.EVSE, DateTime? StartTime = null, TimeSpan? Duration = null, ChargingReservation_Id? ReservationId = null, ChargingReservation_Id? LinkedReservationId = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, ChargingProduct? ChargingProduct = null, IEnumerable<AuthenticationToken>? AuthTokens = null, IEnumerable<eMobilityAccount_Id>? eMAIds = null, IEnumerable<UInt32>? PINs = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<CancelReservationResult> CancelReservation(ChargingReservation_Id ReservationId, ChargingReservationCancellationReason Reason, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region RemoteStart/-Stop

        public Task<RemoteStartResult> RemoteStart(ChargingLocation ChargingLocation, ChargingProduct? ChargingProduct = null, ChargingReservation_Id? ReservationId = null, ChargingSession_Id? SessionId = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteStopResult> RemoteStop(ChargingSession_Id SessionId, ReservationHandling? ReservationHandling = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SendChargeDetailRecords

        public Task<SendCDRsResult> SendChargeDetailRecords(IEnumerable<ChargeDetailRecord> ChargeDetailRecords, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion




        protected override Boolean SkipFlushEVSEDataAndStatusQueues()
        {
            throw new NotImplementedException();
        }

        protected override Task FlushEVSEDataAndStatusQueues()
        {
            throw new NotImplementedException();
        }

        protected override Boolean SkipFlushEVSEFastStatusQueues()
        {
            throw new NotImplementedException();
        }

        protected override Task FlushEVSEFastStatusQueues()
        {
            throw new NotImplementedException();
        }

        protected override Boolean SkipFlushChargeDetailRecordsQueues()
        {
            throw new NotImplementedException();
        }

        protected override Task FlushChargeDetailRecordsQueues(IEnumerable<ChargeDetailRecord> ChargeDetailsRecords)
        {
            throw new NotImplementedException();
        }



        public int CompareTo(OCPICSOAdapter? other)

            => other is null
                   ? throw new ArgumentException("The given object is not an OCPI CSO adapter!")
                   : Id.CompareTo(other.Id);

        public override int CompareTo(Object? other)

            => other is OCPICSOAdapter OCPICSOAdapter
                   ? Id.CompareTo(OCPICSOAdapter.Id)
                   : throw new ArgumentException("The given object is not an OCPI CSO adapter!");


        public Boolean Equals(OCPICSOAdapter? other)

            => other is not null &&
                   Id.Equals(other.Id);

    }

}
