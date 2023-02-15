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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Send charging stations upstream towards an OCPI partner...
    /// </summary>
    public class OCPIEMPAdapter : ACryptoEMobilityEntity<EMPRoamingProvider_Id,
                                                         EMPRoamingProviderAdminStatusTypes,
                                                         EMPRoamingProviderStatusTypes>,
                                  IEMPRoamingProvider,
                                  IEquatable<OCPIEMPAdapter>,
                                  IComparable<OCPIEMPAdapter>,
                                  IComparable

    {

        #region Properties

        public Boolean  DisablePushData                     { get; set; }
        public Boolean  DisablePushAdminStatus              { get; set; }
        public Boolean  DisablePushStatus                   { get; set; }
        public Boolean  DisableAuthentication               { get; set; }
        public Boolean  DisableSendChargeDetailRecords      { get; set; }

        #endregion

        #region Events

        public event OnAuthorizeStartRequestDelegate   OnAuthorizeStartRequest;
        public event OnAuthorizeStartResponseDelegate  OnAuthorizeStartResponse;

        public event OnAuthorizeStopRequestDelegate    OnAuthorizeStopRequest;
        public event OnAuthorizeStopResponseDelegate   OnAuthorizeStopResponse;

        public event OnSendCDRsResponseDelegate        OnSendCDRsResponse;

        #endregion


        public OCPIEMPAdapter(EMPRoamingProvider_Id  Id,
                              I18NString             Name,
                              RoamingNetwork         RoamingNetwork)

            : base(Id,
                   RoamingNetwork,
                   Name)

        {
        
        }





        public IId AuthId => throw new NotImplementedException();

        public ChargeDetailRecordFilterDelegate ChargeDetailRecordFilter => throw new NotImplementedException();

        IId ISendChargeDetailRecords.Id => throw new NotImplementedException();



        public Task<AuthStartResult> AuthorizeStart(LocalAuthentication LocalAuthentication, ChargingLocation ChargingLocation = null, ChargingProduct ChargingProduct = null, ChargingSession_Id? SessionId = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<AuthStopResult> AuthorizeStop(ChargingSession_Id SessionId, LocalAuthentication LocalAuthentication, ChargingLocation ChargingLocation = null, ChargingSession_Id? CPOPartnerSessionId = null, ChargingStationOperator_Id? OperatorId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<SendCDRsResult> SendChargeDetailRecords(IEnumerable<ChargeDetailRecord> ChargeDetailRecords, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }




        #region StaticData

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

        #endregion

        #region (Set/Add/Update/Delete) Charging pool(s)...

        public Task<PushChargingPoolDataResult> SetStaticData(IChargingPool ChargingPool, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolDataResult> AddStaticData(IChargingPool ChargingPool, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

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

        #endregion

        #region (Set/Add/Update/Delete) Charging station(s)...
        public Task<PushChargingStationDataResult> SetStaticData(IChargingStation ChargingStation, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> AddStaticData(IChargingStation ChargingStation, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> UpdateStaticData(IChargingStation ChargingStation, string PropertyName = null, object OldValue = null, object NewValue = null, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> DeleteStaticData(IChargingStation ChargingStation, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushChargingStationDataResult> SetStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> AddStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> UpdateStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationDataResult> DeleteStaticData(IEnumerable<IChargingStation> ChargingStations, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region (Set/Add/Update/Delete) EVSE(s)...

        public Task<PushEVSEDataResult> SetStaticData(cloud.charging.open.protocols.WWCP.IEVSE EVSE, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> AddStaticData(cloud.charging.open.protocols.WWCP.IEVSE EVSE, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> UpdateStaticData(cloud.charging.open.protocols.WWCP.IEVSE EVSE, string PropertyName = null, object OldValue = null, object NewValue = null, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }
        public Task<PushEVSEDataResult> DeleteStaticData(cloud.charging.open.protocols.WWCP.IEVSE EVSE, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }


        public Task<PushEVSEDataResult> SetStaticData(IEnumerable<cloud.charging.open.protocols.WWCP.IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> AddStaticData(IEnumerable<cloud.charging.open.protocols.WWCP.IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> UpdateStaticData(IEnumerable<cloud.charging.open.protocols.WWCP.IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushEVSEDataResult> DeleteStaticData(IEnumerable<cloud.charging.open.protocols.WWCP.IEVSE> EVSEs, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region UpdateAdminStatus


        public Task<PushEVSEAdminStatusResult> UpdateAdminStatus(IEnumerable<EVSEAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationAdminStatusResult> UpdateAdminStatus(IEnumerable<ChargingStationAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolAdminStatusResult> UpdateAdminStatus(IEnumerable<ChargingPoolAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationOperatorAdminStatusResult> UpdateAdminStatus(IEnumerable<ChargingStationOperatorAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushRoamingNetworkAdminStatusResult> UpdateAdminStatus(IEnumerable<RoamingNetworkAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region UpdateStatus

        public Task<PushEVSEStatusResult> UpdateStatus(IEnumerable<EVSEStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationStatusResult> UpdateStatus(IEnumerable<ChargingStationStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingPoolStatusResult> UpdateStatus(IEnumerable<ChargingPoolStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushChargingStationOperatorStatusResult> UpdateStatus(IEnumerable<ChargingStationOperatorStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<PushRoamingNetworkStatusResult> UpdateStatus(IEnumerable<RoamingNetworkStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType = TransmissionTypes.Enqueue, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public override int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion




        public int CompareTo(OCPIEMPAdapter other)
        {
            throw new NotImplementedException();
        }


        public bool Equals(OCPIEMPAdapter other)
        {
            throw new NotImplementedException();
        }



    }

}
