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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Send charging stations upstream towards an OICP partner...
    /// </summary>
    public class OICPEMPAdapter : ACryptoEMobilityEntity<EMPRoamingProvider_Id>,
                                  //ICSORoamingProvider,
                                  //ISendAuthenticationData,
                                  IEMPRoamingProvider,

                                  IEquatable<OICPEMPAdapter>,
                                  IComparable<OICPEMPAdapter>,
                                  IComparable

    {

        #region Properties


        #endregion





        #region  fff


        EMPRoamingProvider_Id IEMPRoamingProvider.Id => throw new NotImplementedException();

        IId ISendChargeDetailRecords.Id => throw new NotImplementedException();

        bool ISendData.DisablePushData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool ISendAdminStatus.DisablePushAdminStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool ISendStatus.DisablePushStatus { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool ISendAuthorizeStartStop.DisableAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        IId IAuthorizeStartStop.AuthId => throw new NotImplementedException();

        IEnumerable<IId> ISendChargeDetailRecords.Ids => throw new NotImplementedException();

        bool ISendChargeDetailRecords.DisableSendChargeDetailRecords { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ChargeDetailRecordFilterDelegate ISendChargeDetailRecords.ChargeDetailRecordFilter => throw new NotImplementedException();

        event OnAuthorizeStartRequestDelegate IAuthorizeStartStop.OnAuthorizeStartRequest
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event OnAuthorizeStartResponseDelegate IAuthorizeStartStop.OnAuthorizeStartResponse
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event OnAuthorizeStopRequestDelegate IAuthorizeStartStop.OnAuthorizeStopRequest
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event OnAuthorizeStopResponseDelegate IAuthorizeStartStop.OnAuthorizeStopResponse
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event OnSendCDRsResponseDelegate ISendChargeDetailRecords.OnSendCDRsResponse
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public int CompareTo(OICPEMPAdapter other)
        {
            throw new NotImplementedException();
        }

        public override int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals(OICPEMPAdapter other)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(EVSE EVSE, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(IEnumerable<EVSE> EVSEs, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(ChargingStation ChargingStation, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(IEnumerable<ChargingStation> ChargingStations, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(ChargingPool ChargingPool, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(IEnumerable<ChargingPool> ChargingPools, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(ChargingStationOperator ChargingStationOperator, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(IEnumerable<ChargingStationOperator> ChargingStationOperators, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.AddStaticData(RoamingNetwork RoamingNetwork, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<AuthStartResult> IAuthorizeStartStop.AuthorizeStart(LocalAuthentication LocalAuthentication, ChargingLocation ChargingLocation, ChargingProduct ChargingProduct, ChargingSession_Id? SessionId, ChargingStationOperator_Id? OperatorId, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<AuthStopResult> IAuthorizeStartStop.AuthorizeStop(ChargingSession_Id SessionId, LocalAuthentication LocalAuthentication, ChargingLocation ChargingLocation, ChargingStationOperator_Id? OperatorId, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(EVSE EVSE, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(IEnumerable<EVSE> EVSEs, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(ChargingStation ChargingStation, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(IEnumerable<ChargingStation> ChargingStations, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(ChargingPool ChargingPool, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(IEnumerable<ChargingPool> ChargingPools, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(ChargingStationOperator ChargingStationOperator, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(IEnumerable<ChargingStationOperator> ChargingStationOperators, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.DeleteStaticData(RoamingNetwork RoamingNetwork, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<SendCDRsResult> ISendChargeDetailRecords.SendChargeDetailRecords(IEnumerable<ChargeDetailRecord> ChargeDetailRecords, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(EVSE EVSE, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(IEnumerable<EVSE> EVSEs, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(ChargingStation ChargingStation, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(IEnumerable<ChargingStation> ChargingStations, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(ChargingPool ChargingPool, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(IEnumerable<ChargingPool> ChargingPools, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(ChargingStationOperator ChargingStationOperator, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(IEnumerable<ChargingStationOperator> ChargingStationOperators, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.SetStaticData(RoamingNetwork RoamingNetwork, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<EVSEAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushChargingStationAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<ChargingStationAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushChargingPoolAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<ChargingPoolAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushChargingStationOperatorAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<ChargingStationOperatorAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushRoamingNetworkAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<RoamingNetworkAdminStatusUpdate> AdminStatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(EVSE EVSE, string PropertyName, object OldValue, object NewValue, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(IEnumerable<EVSE> EVSEs, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(ChargingStation ChargingStation, string PropertyName, object OldValue, object NewValue, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(IEnumerable<ChargingStation> ChargingStations, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(ChargingPool ChargingPool, string PropertyName, object OldValue, object NewValue, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(IEnumerable<ChargingPool> ChargingPools, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(ChargingStationOperator ChargingStationOperator, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(IEnumerable<ChargingStationOperator> ChargingStationOperators, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEDataResult> ISendData.UpdateStaticData(RoamingNetwork RoamingNetwork, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushEVSEStatusResult> ISendStatus.UpdateStatus(IEnumerable<EVSEStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushChargingStationStatusResult> ISendStatus.UpdateStatus(IEnumerable<ChargingStationStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushChargingPoolStatusResult> ISendStatus.UpdateStatus(IEnumerable<ChargingPoolStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushChargingStationOperatorStatusResult> ISendStatus.UpdateStatus(IEnumerable<ChargingStationOperatorStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<PushRoamingNetworkStatusResult> ISendStatus.UpdateStatus(IEnumerable<RoamingNetworkStatusUpdate> StatusUpdates, TransmissionTypes TransmissionType, DateTime? Timestamp, CancellationToken? CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        #endregion


        public OICPEMPAdapter(EMPRoamingProvider_Id  Id,
                              I18NString             Name,
                              RoamingNetwork         RoamingNetwork)

            : base(Id,
                   Name,
                   RoamingNetwork)

        {
        
        }


    }

}
