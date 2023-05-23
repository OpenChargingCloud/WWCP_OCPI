/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Receive charging stations downstream from an OCPI partner...
    /// </summary>
    public class OCPIEMPAdapter : //AWWCPEMPAdapter<ChargeDetailRecord>,
                                  ICSORoamingProvider,
                                  IEquatable <OCPIEMPAdapter>,
                                  IComparable<OCPIEMPAdapter>,
                                  IComparable

    {

        #region Data

        #endregion

        #region Properties

        /// <summary>
        /// The global unique identification.
        /// </summary>
        [Mandatory]
        public CSORoamingProvider_Id                        Id                                   { get; }

        /// <summary>
        /// The multi-language name.
        /// </summary>
        [Optional]
        public I18NString                                   Name                                 { get; }

        /// <summary>
        /// The multi-language description.
        /// </summary>
        [Optional]
        public I18NString                                   Description                          { get; }

        /// <summary>
        /// The roaming network.
        /// </summary>
        [Mandatory]
        public IRoamingNetwork                              RoamingNetwork                       { get; }


        public EVSEId_2_WWCPEVSEId_Delegate?                CustomEVSEIdConverter                { get; }
        public EVSE_2_WWCPEVSE_Delegate?                    CustomEVSEConverter                  { get; }
        public StatusType_2_WWCPEVSEStatusUpdate_Delegate?  CustomEVSEStatusUpdateConverter      { get; }
        public CDR_2_WWCPChargeDetailRecord_Delegate?       CustomChargeDetailRecordConverter    { get; }



        public HTTP.CommonAPI                               CommonAPI                            { get; }

        public HTTP.EMSPAPI                                 EMSPAPI                              { get; }

        #endregion

        #region Events

        event OnGetCDRsRequestDelegate ICSORoamingProvider.OnGetChargeDetailRecordsRequest
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

        event OnGetCDRsResponseDelegate ICSORoamingProvider.OnGetChargeDetailRecordsResponse
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

        event OnAuthorizeStartRequestDelegate ICSORoamingProvider.OnAuthorizeStartRequest
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

        event OnAuthorizeStartResponseDelegate ICSORoamingProvider.OnAuthorizeStartResponse
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

        event OnAuthorizeStopRequestDelegate ICSORoamingProvider.OnAuthorizeStopRequest
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

        event OnAuthorizeStopResponseDelegate ICSORoamingProvider.OnAuthorizeStopResponse
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

        event OnSendCDRsRequestDelegate ICSORoamingProvider.OnChargeDetailRecordRequest
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

        event OnSendCDRsResponseDelegate ICSORoamingProvider.OnChargeDetailRecordResponse
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

        event OnReserveRequestDelegate? IChargingReservations.OnReserveRequest
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

        event OnReserveResponseDelegate? IChargingReservations.OnReserveResponse
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

        event OnNewReservationDelegate? IChargingReservations.OnNewReservation
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

        event OnCancelReservationRequestDelegate? IChargingReservations.OnCancelReservationRequest
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

        event OnCancelReservationResponseDelegate? IChargingReservations.OnCancelReservationResponse
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

        event OnReservationCanceledDelegate? IChargingReservations.OnReservationCanceled
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

        event OnRemoteStartRequestDelegate IRemoteStartStop.OnRemoteStartRequest
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

        event OnRemoteStartResponseDelegate IRemoteStartStop.OnRemoteStartResponse
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

        event OnNewChargingSessionDelegate IChargingSessions.OnNewChargingSession
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

        event OnRemoteStopRequestDelegate IRemoteStartStop.OnRemoteStopRequest
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

        event OnRemoteStopResponseDelegate IRemoteStartStop.OnRemoteStopResponse
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

        event OnNewChargeDetailRecordDelegate IChargeDetailRecords.OnNewChargeDetailRecord
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

        #endregion


        #region Constructor(s)

        public OCPIEMPAdapter(CSORoamingProvider_Id                        Id,
                              I18NString                                   Name,
                              I18NString                                   Description,
                              IRoamingNetwork                              RoamingNetwork,

                              HTTP.CommonAPI                               CommonAPI,
                              OCPI.CountryCode                             DefaultCountryCode,
                              OCPI.Party_Id                                DefaultPartyId,

                              EVSEId_2_WWCPEVSEId_Delegate?                CustomEVSEIdConverter               = null,
                              EVSE_2_WWCPEVSE_Delegate?                    CustomEVSEConverter                 = null,
                              StatusType_2_WWCPEVSEStatusUpdate_Delegate?  CustomEVSEStatusUpdateConverter     = null,
                              CDR_2_WWCPChargeDetailRecord_Delegate?       CustomChargeDetailRecordConverter   = null,

                              IncludeChargingStationOperatorIdDelegate?    IncludeChargingStationOperatorIds   = null,
                              IncludeChargingStationOperatorDelegate?      IncludeChargingStationOperators     = null,
                              IncludeChargingPoolIdDelegate?               IncludeChargingPoolIds              = null,
                              IncludeChargingPoolDelegate?                 IncludeChargingPools                = null,
                              IncludeChargingStationIdDelegate?            IncludeChargingStationIds           = null,
                              IncludeChargingStationDelegate?              IncludeChargingStations             = null,
                              IncludeEVSEIdDelegate?                       IncludeEVSEIds                      = null,
                              IncludeEVSEDelegate?                         IncludeEVSEs                        = null,
                              ChargeDetailRecordFilterDelegate?            ChargeDetailRecordFilter            = null,

                              Boolean                                      DisablePushData                     = false,
                              Boolean                                      DisablePushAdminStatus              = false,
                              Boolean                                      DisablePushStatus                   = false,
                              Boolean                                      DisablePushEnergyStatus             = false,
                              Boolean                                      DisableAuthentication               = false,
                              Boolean                                      DisableSendChargeDetailRecords      = false)

        {

            this.Id                                 = Id;
            this.Name                               = Name;
            this.Description                        = Description;
            this.RoamingNetwork                     = RoamingNetwork;

            this.CommonAPI                          = CommonAPI;
            this.EMSPAPI                            = new HTTP.EMSPAPI(
                                                          this.CommonAPI,
                                                          DefaultCountryCode,
                                                          DefaultPartyId,
                                                          URLPathPrefix: CommonAPI.URLPathPrefix + Version.Number + "emsp"
                                                      );

            this.CustomEVSEIdConverter              = CustomEVSEIdConverter;
            this.CustomEVSEConverter                = CustomEVSEConverter;
            this.CustomEVSEStatusUpdateConverter    = CustomEVSEStatusUpdateConverter;
            this.CustomChargeDetailRecordConverter  = CustomChargeDetailRecordConverter;

            //this.IncludeChargingStationOperatorIds  = IncludeChargingStationOperatorIds ?? (chargingStationOperatorId  => true);
            //this.IncludeChargingStationOperators    = IncludeChargingStationOperators   ?? (chargingStationOperator    => true);
            //this.IncludeChargingPoolIds             = IncludeChargingPoolIds            ?? (chargingPoolId             => true);
            //this.IncludeChargingPools               = IncludeChargingPools              ?? (chargingPool               => true);
            //this.IncludeChargingStationIds          = IncludeChargingStationIds         ?? (chargingStationId          => true);
            //this.IncludeChargingStations            = IncludeChargingStations           ?? (chargingStation            => true);
            //this.IncludeEVSEIds                     = IncludeEVSEIds                    ?? (evseid                     => true);
            //this.IncludeEVSEs                       = IncludeEVSEs                      ?? (evse                       => true);
            //this.ChargeDetailRecordFilter           = ChargeDetailRecordFilter          ?? (chargeDetailRecord         => ChargeDetailRecordFilters.forward);

            //this.DisablePushData                    = DisablePushData;
            //this.DisablePushAdminStatus             = DisablePushAdminStatus;
            //this.DisablePushStatus                  = DisablePushStatus;
            //this.DisablePushEnergyStatus            = DisablePushEnergyStatus;
            //this.DisableAuthentication              = DisableAuthentication;
            //this.DisableSendChargeDetailRecords     = DisableSendChargeDetailRecords;

            //this.chargingPoolsUpdateLog             = new Dictionary<IChargingPool, List<PropertyUpdateInfo>>();

        }


        public OCPIEMPAdapter(CSORoamingProvider_Id                        Id,
                              I18NString                                   Name,
                              I18NString                                   Description,
                              IRoamingNetwork                              RoamingNetwork,

                              HTTP.EMSPAPI                                 EMSPAPI,

                              EVSEId_2_WWCPEVSEId_Delegate?                CustomEVSEIdConverter               = null,
                              EVSE_2_WWCPEVSE_Delegate?                    CustomEVSEConverter                 = null,
                              StatusType_2_WWCPEVSEStatusUpdate_Delegate?  CustomEVSEStatusUpdateConverter     = null,
                              CDR_2_WWCPChargeDetailRecord_Delegate?       CustomChargeDetailRecordConverter   = null,

                              IncludeChargingStationOperatorIdDelegate?    IncludeChargingStationOperatorIds   = null,
                              IncludeChargingStationOperatorDelegate?      IncludeChargingStationOperators     = null,
                              IncludeChargingPoolIdDelegate?               IncludeChargingPoolIds              = null,
                              IncludeChargingPoolDelegate?                 IncludeChargingPools                = null,
                              IncludeChargingStationIdDelegate?            IncludeChargingStationIds           = null,
                              IncludeChargingStationDelegate?              IncludeChargingStations             = null,
                              IncludeEVSEIdDelegate?                       IncludeEVSEIds                      = null,
                              IncludeEVSEDelegate?                         IncludeEVSEs                        = null,
                              ChargeDetailRecordFilterDelegate?            ChargeDetailRecordFilter            = null,

                              Boolean                                      DisablePushData                     = false,
                              Boolean                                      DisablePushAdminStatus              = false,
                              Boolean                                      DisablePushStatus                   = false,
                              Boolean                                      DisablePushEnergyStatus             = false,
                              Boolean                                      DisableAuthentication               = false,
                              Boolean                                      DisableSendChargeDetailRecords      = false)

        {

            this.Id                                 = Id;
            this.Name                               = Name;
            this.Description                        = Description;
            this.RoamingNetwork                     = RoamingNetwork;

            this.CommonAPI                          = EMSPAPI.CommonAPI;
            this.EMSPAPI                            = EMSPAPI;

            this.CustomEVSEIdConverter              = CustomEVSEIdConverter;
            this.CustomEVSEConverter                = CustomEVSEConverter;
            this.CustomEVSEStatusUpdateConverter    = CustomEVSEStatusUpdateConverter;
            this.CustomChargeDetailRecordConverter  = CustomChargeDetailRecordConverter;

            //this.IncludeChargingStationOperatorIds  = IncludeChargingStationOperatorIds ?? (chargingStationOperatorId  => true);
            //this.IncludeChargingStationOperators    = IncludeChargingStationOperators   ?? (chargingStationOperator    => true);
            //this.IncludeChargingPoolIds             = IncludeChargingPoolIds            ?? (chargingPoolId             => true);
            //this.IncludeChargingPools               = IncludeChargingPools              ?? (chargingPool               => true);
            //this.IncludeChargingStationIds          = IncludeChargingStationIds         ?? (chargingStationId          => true);
            //this.IncludeChargingStations            = IncludeChargingStations           ?? (chargingStation            => true);
            //this.IncludeEVSEIds                     = IncludeEVSEIds                    ?? (evseid                     => true);
            //this.IncludeEVSEs                       = IncludeEVSEs                      ?? (evse                       => true);
            //this.ChargeDetailRecordFilter           = ChargeDetailRecordFilter          ?? (chargeDetailRecord         => ChargeDetailRecordFilters.forward);

            //this.DisablePushData                    = DisablePushData;
            //this.DisablePushAdminStatus             = DisablePushAdminStatus;
            //this.DisablePushStatus                  = DisablePushStatus;
            //this.DisablePushEnergyStatus            = DisablePushEnergyStatus;
            //this.DisableAuthentication              = DisableAuthentication;
            //this.DisableSendChargeDetailRecords     = DisableSendChargeDetailRecords;

            //this.chargingPoolsUpdateLog             = new Dictionary<IChargingPool, List<PropertyUpdateInfo>>();

        }

        #endregion


        #region AddRemoteParty(...)

        public Task<Boolean> AddRemoteParty(OCPI.CountryCode               CountryCode,
                                            OCPI.Party_Id                  PartyId,
                                            OCPI.Roles                     Role,
                                            OCPI.BusinessDetails           BusinessDetails,

                                            OCPI.AccessToken               AccessToken,

                                            OCPI.AccessToken               RemoteAccessToken,
                                            URL                            RemoteVersionsURL,
                                            IEnumerable<OCPI.Version_Id>?  RemoteVersionIds            = null,
                                            OCPI.Version_Id?               SelectedVersionId           = null,

                                            DateTime?                      LocalAccessNotBefore        = null,
                                            DateTime?                      LocalAccessNotAfter         = null,

                                            Boolean?                       AccessTokenBase64Encoding   = null,
                                            Boolean?                       AllowDowngrades             = false,
                                            OCPI.AccessStatus              AccessStatus                = OCPI.AccessStatus.      ALLOWED,
                                            OCPI.RemoteAccessStatus?       RemoteStatus                = OCPI.RemoteAccessStatus.ONLINE,
                                            OCPI.PartyStatus               PartyStatus                 = OCPI.PartyStatus.       ENABLED)

            => CommonAPI.AddRemoteParty(CountryCode,
                                        PartyId,
                                        Role,
                                        BusinessDetails,

                                        AccessToken,

                                        RemoteAccessToken,
                                        RemoteVersionsURL,
                                        RemoteVersionIds,
                                        SelectedVersionId,

                                        LocalAccessNotBefore,
                                        LocalAccessNotAfter,

                                        AccessTokenBase64Encoding,
                                        AllowDowngrades,
                                        AccessStatus,
                                        RemoteStatus,
                                        PartyStatus);

        #endregion

        #region AddRemoteParty(...)

        public Task<Boolean> AddRemoteParty(OCPI.CountryCode      CountryCode,
                                            OCPI.Party_Id         PartyId,
                                            OCPI.Roles            Role,
                                            OCPI.BusinessDetails  BusinessDetails,

                                            OCPI.AccessToken      AccessToken,
                                            DateTime?             LocalAccessNotBefore        = null,
                                            DateTime?             LocalAccessNotAfter         = null,
                                            Boolean?              AccessTokenBase64Encoding   = null,
                                            Boolean?              AllowDowngrades             = false,

                                            OCPI.AccessStatus     AccessStatus                = OCPI.AccessStatus.ALLOWED,

                                            OCPI.PartyStatus      PartyStatus                 = OCPI.PartyStatus. ENABLED)

            => CommonAPI.AddRemoteParty(CountryCode,
                                        PartyId,
                                        Role,
                                        BusinessDetails,
                                        AccessToken,
                                        LocalAccessNotBefore,
                                        LocalAccessNotAfter,
                                        AccessTokenBase64Encoding,
                                        AllowDowngrades,
                                        AccessStatus,
                                        PartyStatus);

        #endregion


        // ICSORoamingProvider methods

        #region Pull EVSE Data/-Status

        Boolean IPullEVSEData.PullEVSEData_IsDisabled { get; set; }


        Task<POIDataPull<WWCP.EVSE>> IPullEVSEData.PullEVSEData(DateTime? LastCall, GeoCoordinate? SearchCenter, Single DistanceKM, EMobilityProvider_Id? ProviderId, IEnumerable<ChargingStationOperator_Id> OperatorIdFilter, IEnumerable<Country> CountryCodeFilter, DateTime? Timestamp, CancellationToken CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        Task<StatusPull<EVSEStatus>> IPullEVSEStatus.PullEVSEStatus(DateTime? LastCall, GeoCoordinate? SearchCenter, Single DistanceKM, EVSEStatusTypes? EVSEStatusFilter, EMobilityProvider_Id? ProviderId, DateTime? Timestamp, CancellationToken CancellationToken, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Charging Reservations

        TimeSpan IChargingReservations.MaxReservationDuration { get; set; }

        IEnumerable<ChargingReservation> IChargingReservations.ChargingReservations => throw new NotImplementedException();

        Boolean IChargingReservations.TryGetChargingReservationById(ChargingReservation_Id ReservationId, out ChargingReservation? ChargingReservation)
        {
            throw new NotImplementedException();
        }

        Task<ReservationResult> IChargingReservations.Reserve(ChargingLocation ChargingLocation, ChargingReservationLevel ReservationLevel, DateTime? StartTime, TimeSpan? Duration, ChargingReservation_Id? ReservationId, ChargingReservation_Id? LinkedReservationId, EMobilityProvider_Id? ProviderId, RemoteAuthentication? RemoteAuthentication, ChargingProduct? ChargingProduct, IEnumerable<AuthenticationToken>? AuthTokens, IEnumerable<eMobilityAccount_Id>? eMAIds, IEnumerable<UInt32>? PINs, DateTime? Timestamp, CancellationToken CancellationToken, EventTracking_Id? EventTrackingId, TimeSpan? RequestTimeout)
        {
            return Task.FromResult(ReservationResult.NoOperation());
        }

        Task<CancelReservationResult> IChargingReservations.CancelReservation(ChargingReservation_Id ReservationId, ChargingReservationCancellationReason Reason, DateTime? Timestamp, CancellationToken CancellationToken, EventTracking_Id? EventTrackingId, TimeSpan? RequestTimeout)
        {
            return Task.FromResult(CancelReservationResult.NoOperation(ReservationId));
        }

        #endregion

        #region Remote Start/-Stop

        Task<RemoteStartResult> IRemoteStartStop.RemoteStart(ChargingLocation ChargingLocation, ChargingProduct? ChargingProduct, ChargingReservation_Id? ReservationId, ChargingSession_Id? SessionId, EMobilityProvider_Id? ProviderId, RemoteAuthentication? RemoteAuthentication, DateTime? Timestamp, CancellationToken CancellationToken, EventTracking_Id? EventTrackingId, TimeSpan? RequestTimeout)
        {
            return Task.FromResult(RemoteStartResult.NoOperation());
        }

        Task<RemoteStopResult> IRemoteStartStop.RemoteStop(ChargingSession_Id SessionId, ReservationHandling? ReservationHandling, EMobilityProvider_Id? ProviderId, RemoteAuthentication? RemoteAuthentication, DateTime? Timestamp, CancellationToken CancellationToken, EventTracking_Id? EventTrackingId, TimeSpan? RequestTimeout)
        {
            return Task.FromResult(RemoteStopResult.NoOperation(SessionId));
        }

        #endregion

        #region Charging Sessions

        IEnumerable<ChargingSession> IChargingSessions.ChargingSessions => throw new NotImplementedException();


        Boolean IChargingSessions.TryGetChargingSessionById(ChargingSession_Id ChargingSessionId, out ChargingSession? ChargingSession)
        {
            ChargingSession = null;
            return false;
        }

        #endregion

        #region Charge Detail Records

        Task<IEnumerable<ChargeDetailRecord>> ICSORoamingProvider.GetChargeDetailRecords(DateTime From, DateTime? To, EMobilityProvider_Id? ProviderId, DateTime? Timestamp, CancellationToken CancellationToken, EventTracking_Id? EventTrackingId, TimeSpan? RequestTimeout)
        {
            return Task.FromResult((IEnumerable<ChargeDetailRecord>) Array.Empty<ChargeDetailRecord>());
        }

        #endregion



        #region Operator overloading

        #region Operator == (OCPIEMPAdapter1, OCPIEMPAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIEMPAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPIEMPAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OCPIEMPAdapter OCPIEMPAdapter1,
                                           OCPIEMPAdapter OCPIEMPAdapter2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(OCPIEMPAdapter1, OCPIEMPAdapter2))
                return true;

            // If one is null, but not both, return false.
            if (OCPIEMPAdapter1 is null || OCPIEMPAdapter2 is null)
                return false;

            return OCPIEMPAdapter1.Equals(OCPIEMPAdapter2);

        }

        #endregion

        #region Operator != (OCPIEMPAdapter1, OCPIEMPAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIEMPAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPIEMPAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OCPIEMPAdapter OCPIEMPAdapter1,
                                           OCPIEMPAdapter OCPIEMPAdapter2)

            => !(OCPIEMPAdapter1 == OCPIEMPAdapter2);

        #endregion

        #region Operator <  (OCPIEMPAdapter1, OCPIEMPAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIEMPAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPIEMPAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OCPIEMPAdapter OCPIEMPAdapter1,
                                          OCPIEMPAdapter OCPIEMPAdapter2)
        {

            if (OCPIEMPAdapter1 is null)
                throw new ArgumentNullException(nameof(OCPIEMPAdapter1), "The given OCPI CSO adapter must not be null!");

            return OCPIEMPAdapter1.CompareTo(OCPIEMPAdapter2) < 0;

        }

        #endregion

        #region Operator <= (OCPIEMPAdapter1, OCPIEMPAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIEMPAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPIEMPAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OCPIEMPAdapter OCPIEMPAdapter1,
                                           OCPIEMPAdapter OCPIEMPAdapter2)

            => !(OCPIEMPAdapter1 > OCPIEMPAdapter2);

        #endregion

        #region Operator >  (OCPIEMPAdapter1, OCPIEMPAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIEMPAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPIEMPAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OCPIEMPAdapter OCPIEMPAdapter1,
                                          OCPIEMPAdapter OCPIEMPAdapter2)
        {

            if (OCPIEMPAdapter1 is null)
                throw new ArgumentNullException(nameof(OCPIEMPAdapter1), "The given OCPI CSO adapter must not be null!");

            return OCPIEMPAdapter1.CompareTo(OCPIEMPAdapter2) > 0;

        }

        #endregion

        #region Operator >= (OCPIEMPAdapter1, OCPIEMPAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPIEMPAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPIEMPAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OCPIEMPAdapter OCPIEMPAdapter1,
                                           OCPIEMPAdapter OCPIEMPAdapter2)

            => !(OCPIEMPAdapter1 < OCPIEMPAdapter2);

        #endregion

        #endregion

        #region IComparable<OCPIEMPAdapter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two OCPI CSO adapters.
        /// </summary>
        /// <param name="Object">An OCPI CSO adapter to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is OCPIEMPAdapter evseDataRecord
                   ? CompareTo(evseDataRecord)
                   : throw new ArgumentException("The given object is not an OCPI CSO adapter!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OCPIEMPAdapter)

        /// <summary>
        /// Compares two OCPI CSO adapters.
        /// </summary>
        /// <param name="OCPIEMPAdapter">An OCPI CSO adapter to compare with.</param>
        public Int32 CompareTo(OCPIEMPAdapter? OCPIEMPAdapter)
        {

            if (OCPIEMPAdapter is null)
                throw new ArgumentNullException(nameof(OCPIEMPAdapter),
                                                "The given OCPI CSO adapter must not be null!");

            return Id.CompareTo(OCPIEMPAdapter.Id);

        }

        #endregion

        #endregion

        #region IEquatable<OCPIEMPAdapter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OCPI CSO adapters for equality.
        /// </summary>
        /// <param name="Object">An OCPI CSO adapter to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OCPIEMPAdapter ocpiEMPAdapter &&
                   Equals(ocpiEMPAdapter);

        #endregion

        #region Equals(OCPIEMPAdapter)

        /// <summary>
        /// Compares two OCPI CSO adapters for equality.
        /// </summary>
        /// <param name="OCPIEMPAdapter">An OCPI CSO adapter to compare with.</param>
        public Boolean Equals(OCPIEMPAdapter? OCPIEMPAdapter)

            => OCPIEMPAdapter is not null &&
                   Id.Equals(OCPIEMPAdapter.Id);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion


    }

}
