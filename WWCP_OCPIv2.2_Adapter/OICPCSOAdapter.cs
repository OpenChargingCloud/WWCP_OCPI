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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2.HTTP
{

    /// <summary>
    /// Receive charging stations downstream from an OCPI partner...
    /// </summary>
    public class OCPICSOAdapter : ACryptoEMobilityEntity<CSORoamingProvider_Id,
                                                         CSORoamingProviderAdminStatusTypes,
                                                         CSORoamingProviderStatusTypes>,
                                  ICSORoamingProvider,
                                  //ISendAuthenticationData,
                                  IEquatable<OCPICSOAdapter>,
                                  IComparable<OCPICSOAdapter>,
                                  IComparable

    {

        #region Properties

        public Boolean  PullEVSEData_IsDisabled    { get; set; }

        #endregion

        #region Events

        // from OCPI partner
        public event OnAuthorizeStartRequestDelegate?           OnAuthorizeStartRequest;
        public event OnAuthorizeStartResponseDelegate?          OnAuthorizeStartResponse;

        public event OnAuthorizeStopRequestDelegate?            OnAuthorizeStopRequest;
        public event OnAuthorizeStopResponseDelegate?           OnAuthorizeStopResponse;

        public event OnNewChargingSessionDelegate?              OnNewChargingSession;

        public event OnSendCDRsRequestDelegate?                 OnChargeDetailRecordRequest;
        public event OnSendCDRsResponseDelegate?                OnChargeDetailRecordResponse;
        public event OnNewChargeDetailRecordDelegate?           OnNewChargeDetailRecord;



        // from roaming network
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

        #endregion


        public OCPICSOAdapter(CSORoamingProvider_Id  Id,
                              I18NString             Name,
                              RoamingNetwork         RoamingNetwork)

            : base(Id,
                   RoamingNetwork,
                   Name)

        {
        
        }


        #region POIData

        public Task<POIDataPull<cloud.charging.open.protocols.WWCP.EVSE>> PullEVSEData(DateTime? LastCall = null, GeoCoordinate? SearchCenter = null, float DistanceKM = 0, EMobilityProvider_Id? ProviderId = null, IEnumerable<ChargingStationOperator_Id> OperatorIdFilter = null, IEnumerable<Country> CountryCodeFilter = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Status

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

        public Task<ReservationResult> Reserve(ChargingLocation ChargingLocation, ChargingReservationLevel ReservationLevel = ChargingReservationLevel.EVSE, DateTime? StartTime = null, TimeSpan? Duration = null, ChargingReservation_Id? ReservationId = null, ChargingReservation_Id? LinkedReservationId = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, ChargingProduct? ChargingProduct = null, IEnumerable<Auth_Token>? AuthTokens = null, IEnumerable<eMobilityAccount_Id>? eMAIds = null, IEnumerable<UInt32>? PINs = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<CancelReservationResult> CancelReservation(ChargingReservation_Id ReservationId, ChargingReservationCancellationReason Reason, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }



        public Task<RemoteStartResult> RemoteStart(ChargingLocation ChargingLocation, ChargingProduct? ChargingProduct = null, ChargingReservation_Id? ReservationId = null, ChargingSession_Id? SessionId = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteStopResult> RemoteStop(ChargingSession_Id SessionId, ReservationHandling? ReservationHandling = null, EMobilityProvider_Id? ProviderId = null, RemoteAuthentication? RemoteAuthentication = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }






        public IEnumerable<ChargingSession> ChargingSessions
            => throw new NotImplementedException();
        public Task<IEnumerable<ChargeDetailRecord>> GetChargeDetailRecords(DateTime From, DateTime? To = null, EMobilityProvider_Id? ProviderId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id? EventTrackingId = null, TimeSpan? RequestTimeout = null)
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

            => !(other is null) &&
                   Id.Equals(other.Id);

    }

}
