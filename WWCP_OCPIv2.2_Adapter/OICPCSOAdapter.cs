using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.WWCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WWCP_OCPIv2._2_Adapter
{

    /// <summary>
    /// Receive charging stations downstream from an OICP partner...
    /// </summary>
    public class OICPCSOAdapter : ACryptoEMobilityEntity<CSORoamingProvider_Id>,
                                  ICSORoamingProvider,
                                  //ISendAuthenticationData,

                                  IEquatable<OICPCSOAdapter>,
                                  IComparable<OICPCSOAdapter>,
                                  IComparable

    {

        #region Properties

        #endregion

        #region Events

        public event OnAuthorizeStartRequestDelegate      OnAuthorizeStartRequest;
        public event OnAuthorizeStartResponseDelegate     OnAuthorizeStartResponse;
        public event OnAuthorizeStopRequestDelegate       OnAuthorizeStopRequest;
        public event OnAuthorizeStopResponseDelegate      OnAuthorizeStopResponse;

        public event OnReserveRequestDelegate             OnReserveRequest;
        public event OnReserveResponseDelegate            OnReserveResponse;
        public event OnNewReservationDelegate             OnNewReservation;

        public event OnCancelReservationRequestDelegate   OnCancelReservationRequest;
        public event OnCancelReservationResponseDelegate  OnCancelReservationResponse;
        public event OnReservationCanceledDelegate        OnReservationCanceled;

        public event OnRemoteStartRequestDelegate         OnRemoteStartRequest;
        public event OnRemoteStartResponseDelegate        OnRemoteStartResponse;
        public event OnRemoteStopRequestDelegate          OnRemoteStopRequest;
        public event OnRemoteStopResponseDelegate         OnRemoteStopResponse;

        public event OnNewChargingSessionDelegate         OnNewChargingSession;

        public event OnSendCDRsRequestDelegate            OnChargeDetailRecordRequest;
        public event OnSendCDRsResponseDelegate           OnChargeDetailRecordResponse;
        public event OnNewChargeDetailRecordDelegate      OnNewChargeDetailRecord;

        public event OnGetCDRsRequestDelegate             OnGetChargeDetailRecordsRequest;
        public event OnGetCDRsResponseDelegate            OnGetChargeDetailRecordsResponse;

        #endregion


        public OICPCSOAdapter(CSORoamingProvider_Id  Id,
                              I18NString             Name,
                              RoamingNetwork         RoamingNetwork)

            : base(Id,
                   Name,
                   RoamingNetwork)

        {
        
        }


        #region Data/Status

        public bool DisablePullData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<EVSEDataPull> PullEVSEData(DateTime? LastCall = null, GeoCoordinate? SearchCenter = null, float DistanceKM = 0, eMobilityProvider_Id? ProviderId = null, IEnumerable<ChargingStationOperator_Id> OperatorIdFilter = null, IEnumerable<Country> CountryCodeFilter = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<EVSEStatusPull> PullEVSEStatus(DateTime? LastCall = null, GeoCoordinate? SearchCenter = null, float DistanceKM = 0, EVSEStatusTypes? EVSEStatusFilter = null, eMobilityProvider_Id? ProviderId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion


















        public IEnumerable<ChargingReservation> ChargingReservations => throw new NotImplementedException();

        public IEnumerable<ChargingSession> ChargingSessions => throw new NotImplementedException();





        public Task<IEnumerable<ChargeDetailRecord>> GetChargeDetailRecords(DateTime From, DateTime? To = null, eMobilityProvider_Id? ProviderId = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingReservationById(ChargingReservation_Id ReservationId, out ChargingReservation ChargingReservation)
        {
            throw new NotImplementedException();
        }

        public bool TryGetChargingSessionById(ChargingSession_Id ChargingSessionId, out ChargingSession ChargingSession)
        {
            throw new NotImplementedException();
        }





        public Task<ReservationResult> Reserve(ChargingLocation ChargingLocation, ChargingReservationLevel ReservationLevel = ChargingReservationLevel.EVSE, DateTime? StartTime = null, TimeSpan? Duration = null, ChargingReservation_Id? ReservationId = null, eMobilityProvider_Id? ProviderId = null, RemoteAuthentication RemoteAuthentication = null, ChargingProduct ChargingProduct = null, IEnumerable<Auth_Token> AuthTokens = null, IEnumerable<eMobilityAccount_Id> eMAIds = null, IEnumerable<uint> PINs = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }
        public Task<CancelReservationResult> CancelReservation(ChargingReservation_Id ReservationId, ChargingReservationCancellationReason Reason, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteStartResult> RemoteStart(ChargingLocation ChargingLocation, ChargingProduct ChargingProduct = null, ChargingReservation_Id? ReservationId = null, ChargingSession_Id? SessionId = null, eMobilityProvider_Id? ProviderId = null, RemoteAuthentication RemoteAuthentication = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }

        public Task<RemoteStopResult> RemoteStop(ChargingSession_Id SessionId, ReservationHandling? ReservationHandling = null, eMobilityProvider_Id? ProviderId = null, RemoteAuthentication RemoteAuthentication = null, DateTime? Timestamp = null, CancellationToken? CancellationToken = null, EventTracking_Id EventTrackingId = null, TimeSpan? RequestTimeout = null)
        {
            throw new NotImplementedException();
        }











        public int CompareTo(OICPCSOAdapter other)

            => other is null
                   ? throw new ArgumentException("The given object is not an OICP CSO adapter!")
                   : Id.CompareTo(other.Id);

        public override int CompareTo(Object other)

            => other is OICPCSOAdapter oicpCSOAdapter
                   ? Id.CompareTo(oicpCSOAdapter.Id)
                   : throw new ArgumentException("The given object is not an OICP CSO adapter!");


        public Boolean Equals(OICPCSOAdapter other)

            => !(other is null) &&
                   Id.Equals(other.Id);


    }

}
