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

using org.GraphDefined.Vanaheimr.Illias;

using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPIv2_1_1.CPO.HTTP;
using org.GraphDefined.Vanaheimr.Styx;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// Receive charging stations downstream from an OCPI partner...
    /// </summary>
    public class OCPICSOAdapter : //AWWCPEMPAdapter<ChargeDetailRecord>,
                                  IEMPRoamingProvider,
                                  ISendEnergyStatus,
                                  IEquatable <OCPICSOAdapter>,
                                  IComparable<OCPICSOAdapter>,
                                  IComparable

    {

        #region Data

        protected readonly  SemaphoreSlim  DataAndStatusLock            = new(1, 1);

        protected readonly  TimeSpan       MaxLockWaitingTime           = TimeSpan.FromSeconds(120);

        protected readonly Dictionary<IChargingPool, List<PropertyUpdateInfo>> chargingPoolsUpdateLog;


        /// <summary>
        /// The default logging context.
        /// </summary>
        public  const       String         DefaultLoggingContext        = "OCPIv2.2_CSOAdapter";

        public  const       String         DefaultHTTPAPI_LoggingPath   = "default";

        public  const       String         DefaultHTTPAPI_LogfileName   = "OCPIv2.2_CSOAdapter.log";


        /// <summary>
        /// The request timeout.
        /// </summary>
        public readonly     TimeSpan       RequestTimeout               = TimeSpan.FromSeconds(60);

        #endregion

        #region Properties

        public CommonAPI                                    CommonAPI                            { get; }

        public CPOAPI                                       CPOAPI                               { get; }


        /// <summary>
        /// The global unique identification.
        /// </summary>
        [Mandatory]
        public EMPRoamingProvider_Id                        Id                                   { get; }

        public IId AuthId => Id;

        IId ISendChargeDetailRecords.SendChargeDetailRecordsId => Id;


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


        /// <summary>
        /// A delegate for filtering charge detail records.
        /// </summary>
        public ChargeDetailRecordFilterDelegate             ChargeDetailRecordFilter             { get; }


        public WWCPEVSEId_2_EVSEId_Delegate?                CustomEVSEIdConverter                { get; }
        public WWCPEVSE_2_EVSE_Delegate?                    CustomEVSEConverter                  { get; }
        public WWCPEVSEStatusUpdate_2_StatusType_Delegate?  CustomEVSEStatusUpdateConverter      { get; }
        public WWCPChargeDetailRecord_2_CDR_Delegate?       CustomChargeDetailRecordConverter    { get; }





        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisablePushData                      { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisablePushAdminStatus               { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisablePushStatus                    { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisablePushEnergyStatus              { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisableAuthentication                { get; set; }

        /// <summary>
        /// This service can be disabled, e.g. for debugging reasons.
        /// </summary>
        public Boolean                                      DisableSendChargeDetailRecords       { get; set; }


        public Boolean?                                     DisableLogging                       { get; set; }


        public String?                                      ClientsLoggingPath                   { get; }
        public String?                                      ClientsLoggingContext                { get; }
        public LogfileCreatorDelegate?                      ClientsLogfileCreator                { get; }
        public DNSClient?                                   DNSClient                            { get; }

        #endregion

        #region Events

        // from OCPI CSO
        //public event OnAuthorizeStartRequestDelegate?           OnAuthorizeStartRequest;
        //public event OnAuthorizeStartResponseDelegate?          OnAuthorizeStartResponse;

        //public event OnAuthorizeStopRequestDelegate?            OnAuthorizeStopRequest;
        //public event OnAuthorizeStopResponseDelegate?           OnAuthorizeStopResponse;

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
        //public event OnSendCDRsResponseDelegate?                OnSendCDRsResponse;


        // WWCP

        #region OnAuthorizeStartRequest/-Response

        /// <summary>
        /// An event fired whenever an authentication token will be verified for charging.
        /// </summary>
        public event WWCP.OnAuthorizeStartRequestDelegate?   OnAuthorizeStartRequest;

        /// <summary>
        /// An event fired whenever an authentication token had been verified for charging.
        /// </summary>
        public event WWCP.OnAuthorizeStartResponseDelegate?  OnAuthorizeStartResponse;

        #endregion

        #region OnAuthorizeStopRequest/-Response

        /// <summary>
        /// An event fired whenever an authentication token will be verified to stop a charging process.
        /// </summary>
        public event WWCP.OnAuthorizeStopRequestDelegate?   OnAuthorizeStopRequest;

        /// <summary>
        /// An event fired whenever an authentication token had been verified to stop a charging process.
        /// </summary>
        public event WWCP.OnAuthorizeStopResponseDelegate?  OnAuthorizeStopResponse;

        #endregion


        #region OnSendCDRsRequest/-Response

        /// <summary>
        /// An event fired whenever a charge detail record was enqueued for later sending upstream.
        /// </summary>
        public event WWCP.OnSendCDRsRequestDelegate?   OnEnqueueSendCDRsRequest;

        /// <summary>
        /// An event fired whenever a charge detail record will be send upstream.
        /// </summary>
        public event WWCP.OnSendCDRsRequestDelegate?   OnSendCDRsRequest;

        /// <summary>
        /// An event fired whenever a charge detail record had been sent upstream.
        /// </summary>
        public event WWCP.OnSendCDRsResponseDelegate?  OnSendCDRsResponse;

        #endregion

        #endregion


        #region Constructor(s)

        public OCPICSOAdapter(EMPRoamingProvider_Id                        Id,
                              I18NString                                   Name,
                              I18NString                                   Description,
                              IRoamingNetwork                              RoamingNetwork,

                              CommonAPI                                    CommonAPI,
                              CountryCode                                  DefaultCountryCode,
                              Party_Id                                     DefaultPartyId,

                              WWCPEVSEId_2_EVSEId_Delegate?                CustomEVSEIdConverter               = null,
                              WWCPEVSE_2_EVSE_Delegate?                    CustomEVSEConverter                 = null,
                              WWCPEVSEStatusUpdate_2_StatusType_Delegate?  CustomEVSEStatusUpdateConverter     = null,
                              WWCPChargeDetailRecord_2_CDR_Delegate?       CustomChargeDetailRecordConverter   = null,

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
                              Boolean                                      DisableSendChargeDetailRecords      = false,

                              Boolean?                                     IsDevelopment                       = null,
                              IEnumerable<String>?                         DevelopmentServers                  = null,
                              Boolean?                                     DisableLogging                      = false,
                              String?                                      LoggingPath                         = DefaultHTTPAPI_LoggingPath,
                              String?                                      LoggingContext                      = DefaultLoggingContext,
                              String?                                      LogfileName                         = DefaultHTTPAPI_LogfileName,
                              LogfileCreatorDelegate?                      LogfileCreator                      = null,

                              String?                                      ClientsLoggingPath                  = DefaultHTTPAPI_LoggingPath,
                              String?                                      ClientsLoggingContext               = DefaultLoggingContext,
                              LogfileCreatorDelegate?                      ClientsLogfileCreator               = null,
                              DNSClient?                                   DNSClient                           = null)

        {

            this.Id                                 = Id;
            this.Name                               = Name;
            this.Description                        = Description;
            this.RoamingNetwork                     = RoamingNetwork;

            this.CommonAPI                          = CommonAPI;

            this.CustomEVSEIdConverter              = CustomEVSEIdConverter;
            this.CustomEVSEConverter                = CustomEVSEConverter;
            this.CustomEVSEStatusUpdateConverter    = CustomEVSEStatusUpdateConverter;
            this.CustomChargeDetailRecordConverter  = CustomChargeDetailRecordConverter;

            this.IncludeChargingStationOperatorIds  = IncludeChargingStationOperatorIds ?? (chargingStationOperatorId  => true);
            this.IncludeChargingStationOperators    = IncludeChargingStationOperators   ?? (chargingStationOperator    => true);
            this.IncludeChargingPoolIds             = IncludeChargingPoolIds            ?? (chargingPoolId             => true);
            this.IncludeChargingPools               = IncludeChargingPools              ?? (chargingPool               => true);
            this.IncludeChargingStationIds          = IncludeChargingStationIds         ?? (chargingStationId          => true);
            this.IncludeChargingStations            = IncludeChargingStations           ?? (chargingStation            => true);
            this.IncludeEVSEIds                     = IncludeEVSEIds                    ?? (evseid                     => true);
            this.IncludeEVSEs                       = IncludeEVSEs                      ?? (evse                       => true);
            this.ChargeDetailRecordFilter           = ChargeDetailRecordFilter          ?? (chargeDetailRecord         => ChargeDetailRecordFilters.forward);

            this.DisablePushData                    = DisablePushData;
            this.DisablePushAdminStatus             = DisablePushAdminStatus;
            this.DisablePushStatus                  = DisablePushStatus;
            this.DisablePushEnergyStatus            = DisablePushEnergyStatus;
            this.DisableAuthentication              = DisableAuthentication;
            this.DisableSendChargeDetailRecords     = DisableSendChargeDetailRecords;

            this.chargingPoolsUpdateLog             = new Dictionary<IChargingPool, List<PropertyUpdateInfo>>();

            this.DisableLogging                     = DisableLogging;
            this.ClientsLoggingPath                 = ClientsLoggingPath;
            this.ClientsLoggingContext              = ClientsLoggingContext;
            this.ClientsLogfileCreator              = ClientsLogfileCreator;
            this.DNSClient                          = DNSClient;

            this.CPOAPI                             = new CPOAPI(

                                                          this.CommonAPI,
                                                          DefaultCountryCode,
                                                          DefaultPartyId,
                                                          null, // AllowDowngrades

                                                          null, // HTTPHostname
                                                          null, // ExternalDNSName
                                                          null, // HTTPServiceName
                                                          null, // BasePath

                                                          CommonAPI.URLPathPrefix + "2.1.1/cpo",
                                                          null, // APIVersionHashes

                                                          null, // DisableMaintenanceTasks
                                                          null, // MaintenanceInitialDelay
                                                          null, // MaintenanceEvery

                                                          null, // DisableWardenTasks
                                                          null, // WardenInitialDelay
                                                          null, // WardenCheckEvery

                                                          IsDevelopment,
                                                          DevelopmentServers,
                                                          DisableLogging,
                                                          LoggingPath,
                                                          LogfileName,
                                                          LogfileCreator
                                                          // Autostart

                                                      );

        }

        #endregion


        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode               CountryCode,
                                      Party_Id                  PartyId,
                                      Roles                     Role,
                                      BusinessDetails           BusinessDetails,

                                      AccessToken               AccessToken,

                                      AccessToken               RemoteAccessToken,
                                      URL                       RemoteVersionsURL,
                                      IEnumerable<Version_Id>?  RemoteVersionIds            = null,
                                      Version_Id?               SelectedVersionId           = null,

                                      Boolean?                  AccessTokenBase64Encoding   = null,
                                      AccessStatus              AccessStatus                = AccessStatus.      ALLOWED,
                                      RemoteAccessStatus?       RemoteStatus                = RemoteAccessStatus.ONLINE,
                                      PartyStatus               PartyStatus                 = PartyStatus.       ENABLED)
        {

            return CommonAPI.AddRemoteParty(CountryCode,
                                            PartyId,
                                            Role,
                                            BusinessDetails,

                                            AccessToken,

                                            RemoteAccessToken,
                                            RemoteVersionsURL,
                                            RemoteVersionIds,
                                            SelectedVersionId,

                                            AccessTokenBase64Encoding,
                                            AccessStatus,
                                            RemoteStatus,
                                            PartyStatus);

        }

        #endregion

        #region AddRemoteParty(...)

        public Boolean AddRemoteParty(CountryCode      CountryCode,
                                      Party_Id         PartyId,
                                      Roles            Role,
                                      BusinessDetails  BusinessDetails,

                                      AccessToken      AccessToken,
                                      AccessStatus     AccessStatus   = AccessStatus.ALLOWED,

                                      PartyStatus      PartyStatus    = PartyStatus. ENABLED)
        {

            return CommonAPI.AddRemoteParty(CountryCode,
                                            PartyId,
                                            Role,
                                            BusinessDetails,
                                            AccessToken,
                                            AccessStatus,
                                            PartyStatus);
        }

        #endregion


        #region (Set/Add/Update/Delete) Roaming network...
        Task<PushEVSEDataResult> ISendPOIData.SetStaticData(IRoamingNetwork     RoamingNetwork,
                                                            TransmissionTypes   TransmissionType,
                                                            DateTime?           Timestamp,
                                                            CancellationToken?  CancellationToken,
                                                            EventTracking_Id?   EventTrackingId,
                                                            TimeSpan?           RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.AddStaticData(IRoamingNetwork     RoamingNetwork,
                                                            TransmissionTypes   TransmissionType,
                                                            DateTime?           Timestamp,
                                                            CancellationToken?  CancellationToken,
                                                            EventTracking_Id?   EventTrackingId,
                                                            TimeSpan?           RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.UpdateStaticData(IRoamingNetwork     RoamingNetwork,
                                                               String              PropertyName,
                                                               Object?             OldValue,
                                                               Object?             NewValue,
                                                               TransmissionTypes   TransmissionType,
                                                               DateTime?           Timestamp,
                                                               CancellationToken?  CancellationToken,
                                                               EventTracking_Id?   EventTrackingId,
                                                               TimeSpan?           RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.DeleteStaticData(IRoamingNetwork     RoamingNetwork,
                                                               TransmissionTypes   TransmissionType,
                                                               DateTime?           Timestamp,
                                                               CancellationToken?  CancellationToken,
                                                               EventTracking_Id?   EventTrackingId,
                                                               TimeSpan?           RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }


        Task<PushRoamingNetworkAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<RoamingNetworkAdminStatusUpdate>  AdminStatusUpdates,
                                                                                     TransmissionTypes                             TransmissionType,
                                                                                     DateTime?                                     Timestamp,
                                                                                     CancellationToken?                            CancellationToken,
                                                                                     EventTracking_Id?                             EventTrackingId,
                                                                                     TimeSpan?                                     RequestTimeout)
        {
            return Task.FromResult(PushRoamingNetworkAdminStatusResult.NoOperation(Id, this));
        }

        Task<PushRoamingNetworkStatusResult> ISendStatus.UpdateStatus(IEnumerable<RoamingNetworkStatusUpdate>  StatusUpdates,
                                                                      TransmissionTypes                        TransmissionType,
                                                                      DateTime?                                Timestamp,
                                                                      CancellationToken?                       CancellationToken,
                                                                      EventTracking_Id?                        EventTrackingId,
                                                                      TimeSpan?                                RequestTimeout)
        {
            return Task.FromResult(PushRoamingNetworkStatusResult.NoOperation(Id, this));
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging station operator(s)...


        /// <summary>
        /// Only include charging station identifications matching the given delegate.
        /// </summary>
        public IncludeChargingStationOperatorIdDelegate  IncludeChargingStationOperatorIds    { get; }

        /// <summary>
        /// Only include charging stations matching the given delegate.
        /// </summary>
        public IncludeChargingStationOperatorDelegate    IncludeChargingStationOperators      { get; }


        Task<PushEVSEDataResult> ISendPOIData.SetStaticData(IChargingStationOperator  ChargingStationOperator,
                                                            TransmissionTypes         TransmissionType,
                                                            DateTime?                 Timestamp,
                                                            CancellationToken?        CancellationToken,
                                                            EventTracking_Id?         EventTrackingId,
                                                            TimeSpan?                 RequestTimeout)

            => Task.FromResult(PushEVSEDataResult.NoOperation(Id,
                                                              this,
                                                              null));//new IChargingStationOperator[] { ChargingStationOperator }));



        Task<PushEVSEDataResult> ISendPOIData.AddStaticData(IChargingStationOperator  ChargingStationOperator,
                                                            TransmissionTypes         TransmissionType,
                                                            DateTime?                 Timestamp,
                                                            CancellationToken?        CancellationToken,
                                                            EventTracking_Id?         EventTrackingId,
                                                            TimeSpan?                 RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.UpdateStaticData(IChargingStationOperator  ChargingStationOperator,
                                                               String                    PropertyName,
                                                               Object?                   OldValue,
                                                               Object?                   NewValue,
                                                               TransmissionTypes         TransmissionType,
                                                               DateTime?                 Timestamp,
                                                               CancellationToken?        CancellationToken,
                                                               EventTracking_Id?         EventTrackingId,
                                                               TimeSpan?                 RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.DeleteStaticData(IChargingStationOperator  ChargingStationOperator,
                                                               TransmissionTypes         TransmissionType,
                                                               DateTime?                 Timestamp,
                                                               CancellationToken?        CancellationToken,
                                                               EventTracking_Id?         EventTrackingId,
                                                               TimeSpan?                 RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }



        Task<PushEVSEDataResult> ISendPOIData.SetStaticData(IEnumerable<IChargingStationOperator>  ChargingStationOperators,
                                                            TransmissionTypes                      TransmissionType,
                                                            DateTime?                              Timestamp,
                                                            CancellationToken?                     CancellationToken,
                                                            EventTracking_Id?                      EventTrackingId,
                                                            TimeSpan?                              RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.AddStaticData(IEnumerable<IChargingStationOperator>  ChargingStationOperators,
                                                            TransmissionTypes                      TransmissionType,
                                                            DateTime?                              Timestamp,
                                                            CancellationToken?                     CancellationToken,
                                                            EventTracking_Id?                      EventTrackingId,
                                                            TimeSpan?                              RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.UpdateStaticData(IEnumerable<IChargingStationOperator>  ChargingStationOperators,
                                                               TransmissionTypes                      TransmissionType,
                                                               DateTime?                              Timestamp,
                                                               CancellationToken?                     CancellationToken,
                                                               EventTracking_Id?                      EventTrackingId,
                                                               TimeSpan?                              RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<PushEVSEDataResult> ISendPOIData.DeleteStaticData(IEnumerable<IChargingStationOperator>  ChargingStationOperators,
                                                               TransmissionTypes                      TransmissionType,
                                                               DateTime?                              Timestamp,
                                                               CancellationToken?                     CancellationToken,
                                                               EventTracking_Id?                      EventTrackingId,
                                                               TimeSpan?                              RequestTimeout)
        {
            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, null));
        }



        Task<PushChargingStationOperatorAdminStatusResult> ISendAdminStatus.UpdateAdminStatus(IEnumerable<ChargingStationOperatorAdminStatusUpdate>  AdminStatusUpdates,
                                                                                              TransmissionTypes                                      TransmissionType,
                                                                                              DateTime?                                              Timestamp,
                                                                                              CancellationToken?                                     CancellationToken,
                                                                                              EventTracking_Id?                                      EventTrackingId,
                                                                                              TimeSpan?                                              RequestTimeout)
        {
            return Task.FromResult(PushChargingStationOperatorAdminStatusResult.NoOperation(Id, this));
        }

        Task<PushChargingStationOperatorStatusResult> ISendStatus.UpdateStatus(IEnumerable<ChargingStationOperatorStatusUpdate>  StatusUpdates,
                                                                               TransmissionTypes                                 TransmissionType,
                                                                               DateTime?                                         Timestamp,
                                                                               CancellationToken?                                CancellationToken,
                                                                               EventTracking_Id?                                 EventTrackingId,
                                                                               TimeSpan?                                         RequestTimeout)
        {
            return Task.FromResult(PushChargingStationOperatorStatusResult.NoOperation(Id, this));
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging pool(s)...

        /// <summary>
        /// Only include charging pool identifications matching the given delegate.
        /// </summary>
        public IncludeChargingPoolIdDelegate  IncludeChargingPoolIds    { get; }

        /// <summary>
        /// Only include charging pools matching the given delegate.
        /// </summary>
        public IncludeChargingPoolDelegate    IncludeChargingPools      { get; }


        #region SetStaticData   (ChargingPool,  TransmissionType = Enqueue, ...)

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

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    if (IncludeChargingPools is null ||
                        (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {


                        var location = ChargingPool.ToOCPI(out warnings);

                        if (location is not null)
                            CommonAPI.AddLocation(location);


                        return WWCP.PushChargingPoolDataResult.Enqueued(
                                    Id,
                                    this,
                                    new IChargingPool[] {
                                        ChargingPool
                                    },
                                    "",
                                    warnings,
                                    TimeSpan.FromMilliseconds(10)
                                );

                    }
                    else
                        return WWCP.PushChargingPoolDataResult.NoOperation(
                                    Id,
                                    this,
                                    new IChargingPool[] {
                                        ChargingPool
                                    }
                                );

                }
                else
                    return WWCP.PushChargingPoolDataResult.LockTimeout(
                                Id,
                                this,
                                new IChargingPool[] {
                                    ChargingPool
                                }
                            );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region AddStaticData   (ChargingPool,  TransmissionType = Enqueue, ...)

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

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {

                        var location = ChargingPool.ToOCPI(out warnings);

                        if (location is not null)
                        {

                            var result = CommonAPI.AddLocation(location);


                            //ToDo: Process errors!!!


                        }

                        return WWCP.PushChargingPoolDataResult.Enqueued(
                                   Id,
                                   this,
                                   new IChargingPool[] {
                                       ChargingPool
                                   },
                                   "",
                                   warnings,
                                   TimeSpan.FromMilliseconds(10)
                               );

                    }
                    else
                        return WWCP.PushChargingPoolDataResult.NoOperation(
                                   Id,
                                   this,
                                   new IChargingPool[] {
                                       ChargingPool
                                   }
                               );

                }
                else
                    return WWCP.PushChargingPoolDataResult.LockTimeout(
                               Id,
                               this,
                               new IChargingPool[] {
                                   ChargingPool
                               }
                           );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region UpdateStaticData(ChargingPool,  PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="PropertyName">The optional name of a charging pool property to update.</param>
        /// <param name="OldValue">The optional old value of a charging pool property to update.</param>
        /// <param name="NewValue">The optional new value of a charging pool property to update.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<PushChargingPoolDataResult> UpdateStaticData(IChargingPool       ChargingPool,
                                                                       String?             PropertyName        = null,
                                                                       Object?             OldValue            = null,
                                                                       Object?             NewValue            = null,
                                                                       TransmissionTypes   TransmissionType    = TransmissionTypes.Enqueue,
                                                                       DateTime?           Timestamp           = null,
                                                                       CancellationToken?  CancellationToken   = null,
                                                                       EventTracking_Id?   EventTrackingId     = null,
                                                                       TimeSpan?           RequestTimeout      = null)
        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {

                        var location = ChargingPool.ToOCPI(out warnings);

                        if (location is not null)
                        {

                            var result = CommonAPI.UpdateLocation(location);

                            //ToDo: Process errors!!!

                            if (PropertyName is not null)
                            {

                                if (chargingPoolsUpdateLog.TryGetValue(ChargingPool, out var propertyUpdateInfos))
                                    propertyUpdateInfos.Add(new PropertyUpdateInfo(PropertyName, OldValue, NewValue));

                                else
                                    chargingPoolsUpdateLog.Add(ChargingPool,
                                                               new List<PropertyUpdateInfo> {
                                                                   new PropertyUpdateInfo(PropertyName, OldValue, NewValue)
                                                               });

                            }

                            return WWCP.PushChargingPoolDataResult.Enqueued(
                                       Id,
                                       this,
                                       new IChargingPool[] { ChargingPool },
                                       String.Empty,
                                       warnings,
                                       TimeSpan.Zero
                                   );

                        }

                    }

                    return WWCP.PushChargingPoolDataResult.NoOperation(
                               Id,
                               this,
                               new IChargingPool[] { ChargingPool },
                               String.Empty,
                               warnings,
                               TimeSpan.Zero
                           );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return PushChargingPoolDataResult.LockTimeout(
                       Id,
                       this,
                       new IChargingPool[] { ChargingPool },
                       "",
                       Array.Empty<Warning>(),
                       TimeSpan.Zero
                   );

        }

        #endregion

        #region DeleteStaticData(ChargingPool,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Delete the charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.DeleteStaticData(WWCP.IChargingPool       ChargingPool,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken?       CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(PushChargingPoolDataResult.NoOperation(Id, this, new IChargingPool[] { ChargingPool }));

        }

        #endregion


        #region SetStaticData   (ChargingPools, TransmissionType = Enqueue, ...)

        public Task<PushChargingPoolDataResult> SetStaticData(IEnumerable<IChargingPool>  ChargingPools,
                                                              WWCP.TransmissionTypes      TransmissionType,

                                                              DateTime?                   Timestamp,
                                                              CancellationToken?          CancellationToken,
                                                              EventTracking_Id?           EventTrackingId,
                                                              TimeSpan?                   RequestTimeout)

            => Task.FromResult(
                   PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   )
               );

        #endregion

        #region SetStaticData   (ChargingPools, TransmissionType = Enqueue, ...)

        public Task<PushChargingPoolDataResult> AddStaticData(IEnumerable<IChargingPool>  ChargingPools,
                                                              WWCP.TransmissionTypes      TransmissionType,

                                                              DateTime?                   Timestamp,
                                                              CancellationToken?          CancellationToken,
                                                              EventTracking_Id?           EventTrackingId,
                                                              TimeSpan?                   RequestTimeout)

            => Task.FromResult(
                   PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   )
               );

        #endregion

        #region UpdateStaticData(ChargingPools, TransmissionType = Enqueue, ...)

        public Task<PushChargingPoolDataResult> UpdateStaticData(IEnumerable<IChargingPool>  ChargingPools,
                                                                 WWCP.TransmissionTypes      TransmissionType,

                                                                 DateTime?                   Timestamp,
                                                                 CancellationToken?          CancellationToken,
                                                                 EventTracking_Id?           EventTrackingId,
                                                                 TimeSpan?                   RequestTimeout)

            => Task.FromResult(
                   PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   )
               );

        #endregion

        #region DeleteStaticData(ChargingPools, TransmissionType = Enqueue, ...)

        public Task<PushChargingPoolDataResult> DeleteStaticData(IEnumerable<IChargingPool>  ChargingPools,
                                                                 WWCP.TransmissionTypes      TransmissionType,

                                                                 DateTime?                   Timestamp,
                                                                 CancellationToken?          CancellationToken,
                                                                 EventTracking_Id?           EventTrackingId,
                                                                 TimeSpan?                   RequestTimeout)

            => Task.FromResult(
                   PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   )
               );

        #endregion


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


        /// <summary>
        /// Only include charging station identifications matching the given delegate.
        /// </summary>
        public IncludeChargingStationIdDelegate  IncludeChargingStationIds    { get; }

        /// <summary>
        /// Only include charging stations matching the given delegate.
        /// </summary>
        public IncludeChargingStationDelegate    IncludeChargingStations      { get; }


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

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var locationId = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (locationId.HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                                (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(out var warning);

                                    if (evse2 is not null)
                                        results.Add(await CommonAPI.AddOrUpdateEVSE(location, evse2));
                                    else
                                        results.Add(AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!"));

                                    if (warning is not null && warning.Any())
                                        warnings.AddRange(warning);

                                }

                            }

                        }


                        //ToDo: Process errors!!!


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

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var locationId = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (locationId.HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(out var warning);

                                    if (evse2 is not null)
                                        results.Add(await CommonAPI.AddOrUpdateEVSE(location, evse2));
                                    else
                                        results.Add(AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!"));

                                    if (warning is not null && warning.Any())
                                        warnings.AddRange(warning);

                                }

                            }

                        }


                        //ToDo: Process errors!!!


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

        #region UpdateStaticData(ChargingStation, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the EVSE data of the given charging station within the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="PropertyName">The name of the charging station property to update.</param>
        /// <param name="OldValue">The old value of the charging station property to update.</param>
        /// <param name="NewValue">The new value of the charging station property to update.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.UpdateStaticData(WWCP.IChargingStation    ChargingStation,
                                               String?                  PropertyName,
                                               Object?                  OldValue,
                                               Object?                  NewValue,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken?       CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var locationId = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (locationId.HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(out var warning);

                                    if (evse2 is not null)
                                        results.Add(await CommonAPI.AddOrUpdateEVSE(location, evse2));
                                    else
                                        results.Add(AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!"));

                                    if (warning is not null && warning.Any())
                                        warnings.AddRange(warning);

                                }

                            }

                        }


                        //ToDo: Process errors!!!


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

        #region DeleteStaticData(ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Delete the EVSE data of the given charging station from the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="ChargingStation">A charging station.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.DeleteStaticData(WWCP.IChargingStation    ChargingStation,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken?       CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(PushChargingStationDataResult.NoOperation(Id, this, new IChargingStation[] { ChargingStation }));

        }

        #endregion

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

        /// <summary>
        /// Only include EVSE identifications matching the given delegate.
        /// </summary>
        public IncludeEVSEIdDelegate  IncludeEVSEIds    { get; }

        /// <summary>
        /// Only include EVSEs matching the given delegate.
        /// </summary>
        public IncludeEVSEDelegate    IncludeEVSEs      { get; }


        #region SetStaticData   (EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given EVSE as new static EVSE data at the OICP server.
        /// </summary>
        /// <param name="EVSE">An EVSE to upload.</param>
        /// <param name="TransmissionType">Whether to send the EVSE directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.SetStaticData(WWCP.IEVSE              EVSE,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    var locationId  = EVSE.ChargingPool is not null
                                          ? Location_Id.TryParse(EVSE.ChargingPool.Id.Suffix)
                                          : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(location, evse2);
                                else
                                    result = AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<EVSE>.Failed("Unknown location identification!");

                        }
                        else
                            result = AddOrUpdateResult<EVSE>.Failed("The given EVSE was filtered!");

                    }
                    else
                        result = AddOrUpdateResult<EVSE>.Failed("Invalid location identification!");


                    return result.IsSuccess

                               ? new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Success,
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Error,
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Success,
                                               warnings
                                           )
                                       },
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new IEVSE[] { EVSE });

        }

        #endregion

        #region AddStaticData   (EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given EVSE.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.AddStaticData(IEVSE                   EVSE,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken?      CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    var locationId  = EVSE.ChargingPool is not null
                                          ? Location_Id.TryParse(EVSE.ChargingPool.ToString())
                                          : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(location, evse2);
                                else
                                    result = AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<EVSE>.Failed("Unknown location identification!");

                        }
                        else
                            result = AddOrUpdateResult<EVSE>.Failed("The given EVSE was filtered!");

                    }
                    else
                        result = AddOrUpdateResult<EVSE>.Failed("Invalid location identification!");


                    return result.IsSuccess

                               ? new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Success,
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Error,
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Success,
                                               warnings
                                           )
                                       },
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new IEVSE[] { EVSE });

        }

        #endregion

        #region UpdateStaticData(EVSE, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the EVSE data of the given charging pool within the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="PropertyName">The name of the charging pool property to update.</param>
        /// <param name="OldValue">The old value of the charging pool property to update.</param>
        /// <param name="NewValue">The new value of the charging pool property to update.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.UpdateStaticData(WWCP.IEVSE              EVSE,
                                               String?                 PropertyName,
                                               Object?                 OldValue,
                                               Object?                 NewValue,
                                               WWCP.TransmissionTypes  TransmissionType,

                                               DateTime?               Timestamp,
                                               CancellationToken?      CancellationToken,
                                               EventTracking_Id?       EventTrackingId,
                                               TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    var locationId  = EVSE.ChargingPool is not null
                                          ? Location_Id.TryParse(EVSE.ChargingPool.Id.Suffix)
                                          : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(location, evse2);
                                else
                                    result = AddOrUpdateResult<EVSE>.Failed("Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<EVSE>.Failed("Unknown location identification!");

                        }
                        else
                            result = AddOrUpdateResult<EVSE>.Failed("The given EVSE was filtered!");

                    }
                    else
                        result = AddOrUpdateResult<EVSE>.Failed("Invalid location identification!");


                    return result.IsSuccess

                               ? new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Success,
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new PushEVSEDataResult(
                                       Id,
                                       this,
                                       PushDataResultTypes.Error,
                                       Array.Empty<PushSingleEVSEDataResult>(),
                                       new PushSingleEVSEDataResult[] {
                                           new PushSingleEVSEDataResult(
                                               EVSE,
                                               PushSingleDataResultTypes.Success,
                                               warnings
                                           )
                                       },
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new IEVSE[] { EVSE });

        }

        #endregion

        #region DeleteStaticData(EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Delete the EVSE data of the given EVSE from the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.DeleteStaticData(WWCP.IEVSE               EVSE,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken?       CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(PushEVSEDataResult.NoOperation(Id, this, new IEVSE[] { EVSE }));

        }

        #endregion


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


        #region UpdateAdminStatus (AdminStatusUpdates,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given enumeration of EVSE admin status updates.
        /// </summary>
        /// <param name="AdminStatusUpdates">An enumeration of EVSE admin status updates.</param>
        /// <param name="TransmissionType">Whether to send the EVSE admin status updates directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushEVSEAdminStatusResult>

            WWCP.ISendAdminStatus.UpdateAdminStatus(IEnumerable<WWCP.EVSEAdminStatusUpdate>  AdminStatusUpdates,
                                                    WWCP.TransmissionTypes                   TransmissionType,

                                                    DateTime?                                Timestamp,
                                                    CancellationToken?                       CancellationToken,
                                                    EventTracking_Id?                        EventTrackingId,
                                                    TimeSpan?                                RequestTimeout)


                => Task.FromResult(WWCP.PushEVSEAdminStatusResult.NoOperation(Id, this));

        #endregion

        #region UpdateStatus      (StatusUpdates,       TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given enumeration of EVSE status updates.
        /// </summary>
        /// <param name="StatusUpdates">An enumeration of EVSE status updates.</param>
        /// <param name="TransmissionType">Whether to send the EVSE status updates directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        async Task<WWCP.PushEVSEStatusResult>

            ISendStatus.UpdateStatus(IEnumerable<WWCP.EVSEStatusUpdate>  StatusUpdates,
                                     WWCP.TransmissionTypes              TransmissionType,

                                     DateTime?                           Timestamp,
                                     CancellationToken?                  CancellationToken,
                                     EventTracking_Id?                   EventTrackingId,
                                     TimeSpan?                           RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    PushEVSEStatusResult result;

                    var startTime  = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                    var warnings   = new List<Warning>();
                    var results    = new List<PushEVSEStatusResult>();

                    foreach (var statusUpdate in StatusUpdates)
                    {

                        if (RoamingNetwork.TryGetEVSEById(statusUpdate.Id, out var evse) && evse is not null)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                var locationId = evse.ChargingPool is not null
                                                     ? Location_Id.TryParse(evse.ChargingPool.Id.Suffix)
                                                     : null;

                                if (locationId.HasValue)
                                {

                                    if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                        location is not null)
                                    {

                                        var evse2 = evse.ToOCPI(ref warnings);

                                        if (evse2 is not null)
                                        {

                                            var result2 = await CommonAPI.AddOrUpdateEVSE(location, evse2);

                                            result = result2.IsSuccess
                                                         ? PushEVSEStatusResult.Success(Id, this, null, warnings)
                                                         : PushEVSEStatusResult.Failed (Id, this, StatusUpdates, result2.ErrorResponse, warnings);

                                        }
                                        else
                                            result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Could not convert the given EVSE!");

                                    }
                                    else
                                        result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Unknown location identification!");

                                }
                                else
                                    result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Invalid location identification!");

                            }
                            else
                                result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "The given EVSE was filtered!");

                        }
                        else
                            result = PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "The given EVSE does not exist!");

                        results.Add(result);

                    }

                    return PushEVSEStatusResult.Flatten(
                               Id,
                               this,
                               results,
                               org.GraphDefined.Vanaheimr.Illias.Timestamp.Now - startTime
                           );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.PushEVSEStatusResult.Enqueued   (Id, this)
                       : WWCP.PushEVSEStatusResult.LockTimeout(Id, this);

        }

        #endregion

        #region UpdateEnergyStatus(EnergyStatusUpdates, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given enumeration of EVSE status updates.
        /// </summary>
        /// <param name="EnergyStatusUpdates">An enumeration of EVSE status updates.</param>
        /// <param name="TransmissionType">Whether to send the EVSE status updates directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        Task<WWCP.PushEVSEEnergyStatusResult>

            ISendEnergyStatus.UpdateEnergyStatus(IEnumerable<WWCP.EVSEEnergyStatusUpdate>  EnergyStatusUpdates,
                                                 WWCP.TransmissionTypes                    TransmissionType,

                                                 DateTime?                                 Timestamp,
                                                 CancellationToken?                        CancellationToken,
                                                 EventTracking_Id?                         EventTrackingId,
                                                 TimeSpan?                                 RequestTimeout)

                => Task.FromResult(WWCP.PushEVSEEnergyStatusResult.NoOperation(Id, this));

        #endregion

        #endregion



        #region AuthorizeStart(LocalAuthentication, ...)

        /// <summary>
        /// Create an authorize start request at the given charging location.
        /// </summary>
        /// <param name="LocalAuthentication">An user identification.</param>
        /// <param name="ChargingLocation">The charging location.</param>
        /// <param name="ChargingProduct">An optional charging product.</param>
        /// <param name="SessionId">An optional session identification.</param>
        /// <param name="CPOPartnerSessionId">An optional session identification of the CPO.</param>
        /// <param name="OperatorId">An optional charging station operator identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<AuthStartResult>

            AuthorizeStart(LocalAuthentication          LocalAuthentication,
                           ChargingLocation?            ChargingLocation      = null,
                           ChargingProduct?             ChargingProduct       = null,
                           ChargingSession_Id?          SessionId             = null,
                           ChargingSession_Id?          CPOPartnerSessionId   = null,
                           ChargingStationOperator_Id?  OperatorId            = null,

                           DateTime?                    Timestamp             = null,
                           CancellationToken?           CancellationToken     = null,
                           EventTracking_Id?            EventTrackingId       = null,
                           TimeSpan?                    RequestTimeout        = null)

        {

            #region Initial checks

            Timestamp         ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            CancellationToken ??= new CancellationTokenSource().Token;
            EventTrackingId   ??= EventTracking_Id.New;
            RequestTimeout    ??= this.RequestTimeout;

            #endregion

            #region Send OnAuthorizeStartRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnAuthorizeStartRequest?.Invoke(startTime,
                                                Timestamp.Value,
                                                this,
                                                Id.ToString(),
                                                EventTrackingId,
                                                RoamingNetwork.Id,
                                                null,
                                                Id,
                                                OperatorId,
                                                LocalAuthentication,
                                                ChargingLocation,
                                                ChargingProduct,
                                                SessionId,
                                                CPOPartnerSessionId,
                                                Array.Empty<ISendAuthorizeStartStop>(),
                                                RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPICSOAdapter) + "." + nameof(OnAuthorizeStartRequest));
            }

            #endregion


            if (DisableAuthentication)
                return AuthStartResult.AdminDown(
                           AuthorizatorId:           Id,
                           ISendAuthorizeStartStop:  this,
                           SessionId:                SessionId,
                           Description:              I18NString.Create(Languages.en, "Authentication is disabled!"),
                           Runtime:                  TimeSpan.Zero
                       );


            var remotes = new PriorityList<RemoteParty>();
            foreach (var remote in CommonAPI.GetRemoteParties(Roles.EMSP))
                remotes.Add(remote);

            var authorizationInfo = await remotes.WhenFirst(Work:            async remoteParty => {

                                                                                       #region Initial checks

                                                                                       var authToken = LocalAuthentication.AuthToken?.ToString();

                                                                                       if (authToken is null)
                                                                                           return new AuthorizationInfo(
                                                                                                      Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                      Info:         new DisplayText(Languages.en, $"The local authentication must not be null!"),
                                                                                                      RemoteParty:  remoteParty
                                                                                                  );


                                                                                       var tokenId = Token_Id.TryParse(authToken);

                                                                                       if (!tokenId.HasValue)
                                                                                           return new AuthorizationInfo(
                                                                                                      Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                      Info:         new DisplayText(Languages.en, $"The token identification is invalid!"),
                                                                                                      RemoteParty:  remoteParty
                                                                                                  );


                                                                                       var remoteAccessInfo  = remoteParty.RemoteAccessInfos.FirstOrDefault(remoteAccessInfo => remoteAccessInfo.Status == RemoteAccessStatus.ONLINE);

                                                                                       if (remoteAccessInfo is null)
                                                                                           return new AuthorizationInfo(
                                                                                                      Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                      Info:         new DisplayText(Languages.en, $"No remote access information for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                                                                                                      RemoteParty:  remoteParty
                                                                                                  );


                                                                                       var cpoClient = new CPOClient(

                                                                                                           remoteAccessInfo.VersionsURL,
                                                                                                           remoteAccessInfo.AccessToken,
                                                                                                           CommonAPI,
                                                                                                           null, // VirtualHostname             
                                                                                                           null, // Description                 
                                                                                                           null, // RemoteCertificateValidator  
                                                                                                           null, // ClientCertificateSelector   
                                                                                                           null, // ClientCert                  
                                                                                                           null, // TLSProtocol                 
                                                                                                           null, // PreferIPv4                  
                                                                                                           null, // HTTPUserAgent               
                                                                                                           null, // RequestTimeout              
                                                                                                           null, // TransmissionRetryDelay      
                                                                                                           null, // MaxNumberOfRetries          
                                                                                                           null, // UseHTTPPipelining           
                                                                                                           null, // HTTPLogger                  
                                                                                                           remoteAccessInfo.AccessTokenBase64Encoding,

                                                                                                           DisableLogging,
                                                                                                           ClientsLoggingPath    ?? DefaultHTTPAPI_LoggingPath,
                                                                                                           ClientsLoggingContext ?? DefaultLoggingContext,
                                                                                                           ClientsLogfileCreator,
                                                                                                           DNSClient

                                                                                                       );

                                                                                       if (cpoClient is null)
                                                                                           return new AuthorizationInfo(
                                                                                                      Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                      Info:         new DisplayText(Languages.en, $"Could not get/create a CPO client for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                                                                                                      RemoteParty:  remoteParty
                                                                                                  );


                                                                                       var cpoClientLogger = new CPOClient.Logger(
                                                                                                                 cpoClient,
                                                                                                                 ClientsLoggingPath    ?? DefaultHTTPAPI_LoggingPath,
                                                                                                                 ClientsLoggingContext ?? DefaultLoggingContext,
                                                                                                                 ClientsLogfileCreator
                                                                                                             );

                                                                                       #endregion

                                                                                       var authorizationInfo = await cpoClient.PostToken(
                                                                                                                         TokenId:            tokenId.Value,
                                                                                                                         TokenType:          TokenType.RFID,
                                                                                                                         LocationReference:  null
                                                                                                                     );

                                                                                       return authorizationInfo.Data is not null

                                                                                                  ? new AuthorizationInfo(
                                                                                                        authorizationInfo.Data.Allowed,
                                                                                                        authorizationInfo.Data.Location,
                                                                                                        authorizationInfo.Data.Info,
                                                                                                        remoteParty,
                                                                                                        authorizationInfo.Data.Runtime
                                                                                                    )

                                                                                                  : new AuthorizationInfo(
                                                                                                        Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                        Info:         new DisplayText(Languages.en, authorizationInfo.StatusMessage ?? $"No valid response from '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                                                                                                        RemoteParty:  remoteParty
                                                                                                    );

                                                                                   },

                                                            VerifyResult:    result  => result.Allowed == AllowedType.ALLOWED,

                                                            Timeout:         RequestTimeout ?? TimeSpan.FromSeconds(10),

                                                            OnException:     null,

                                                            DefaultResult:   runtime  => new AuthorizationInfo(
                                                                                             Allowed:   AllowedType.NOT_ALLOWED,
                                                                                             Location:  null,
                                                                                             Info:      new DisplayText(Languages.en, "No authorization service returned a positiv result!"),
                                                                                             Runtime:   runtime
                                                                                         ));


            DateTime         endtime;
            TimeSpan         runtime;
            AuthStartResult? authStartResult = null;


            if (authorizationInfo is null)
                authStartResult = AuthStartResult.CommunicationTimeout(Id, this, SessionId);

            else if (authorizationInfo.Allowed == AllowedType.ALLOWED)
                authStartResult = AuthStartResult.Authorized(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           EMPPartnerSessionId:       null,
                           ContractId:                null,
                           PrintedNumber:             null,
                           ExpiryDate:                null,
                           MaxkW:                     null,
                           MaxkWh:                    null,
                           MaxDuration:               null,
                           ChargingTariffs:           null,
                           ListOfAuthStopTokens:      null,
                           ListOfAuthStopPINs:        null,
                           ProviderId:                EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

            else if (authorizationInfo.Allowed == AllowedType.BLOCKED)
                authStartResult = AuthStartResult.Blocked(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

            else if (authorizationInfo.Allowed == AllowedType.EXPIRED)
                authStartResult = AuthStartResult.Expired(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

            else if (authorizationInfo.Allowed == AllowedType.NO_CREDIT)
                authStartResult = AuthStartResult.NoCredit(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

            else if (authorizationInfo.Allowed == AllowedType.NOT_ALLOWED)
                authStartResult = AuthStartResult.NotAuthorized(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                null,
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );


            authStartResult ??= AuthStartResult.Error(Id, this, SessionId);


            #region Send OnAuthorizeStartResponse event

            endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            runtime = endtime - startTime;

            try
            {

                OnAuthorizeStartResponse?.Invoke(endtime,
                                                 Timestamp.Value,
                                                 this,
                                                 Id.ToString(),
                                                 EventTrackingId,
                                                 RoamingNetwork.Id,
                                                 null,
                                                 Id,
                                                 OperatorId,
                                                 LocalAuthentication,
                                                 ChargingLocation,
                                                 ChargingProduct,
                                                 SessionId,
                                                 CPOPartnerSessionId,
                                                 Array.Empty<WWCP.ISendAuthorizeStartStop>(),
                                                 RequestTimeout,
                                                 authStartResult,
                                                 runtime);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPICSOAdapter) + "." + nameof(OnAuthorizeStartResponse));
            }

            #endregion

            return authStartResult;

        }

        #endregion

        #region AuthorizeStop (SessionId, LocalAuthentication, 

        /// <summary>
        /// Create an authorize stop request at the given charging location.
        /// </summary>
        /// <param name="SessionId">The session identification from the AuthorizeStart request.</param>
        /// <param name="LocalAuthentication">A local user identification.</param>
        /// <param name="ChargingLocation">The charging location.</param>
        /// <param name="CPOPartnerSessionId">An optional session identification of the CPO.</param>
        /// <param name="OperatorId">An optional charging station operator identification.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public Task<AuthStopResult>

            AuthorizeStop(ChargingSession_Id           SessionId,
                          LocalAuthentication          LocalAuthentication,
                          ChargingLocation?            ChargingLocation      = null,
                          ChargingSession_Id?          CPOPartnerSessionId   = null,
                          ChargingStationOperator_Id?  OperatorId            = null,

                          DateTime?                    Timestamp             = null,
                          CancellationToken?           CancellationToken     = null,
                          EventTracking_Id?            EventTrackingId       = null,
                          TimeSpan?                    RequestTimeout        = null)

        {

            #region Initial checks

            Timestamp         ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            CancellationToken ??= new CancellationTokenSource().Token;
            EventTrackingId   ??= EventTracking_Id.New;
            RequestTimeout    ??= this.RequestTimeout;

            #endregion

            #region Send OnAuthorizeStopRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnAuthorizeStopRequest?.Invoke(startTime,
                                               Timestamp.Value,
                                               this,
                                               Id.ToString(),
                                               EventTrackingId,
                                               RoamingNetwork.Id,
                                               null,
                                               Id,
                                               OperatorId,
                                               ChargingLocation,
                                               SessionId,
                                               CPOPartnerSessionId,
                                               LocalAuthentication,
                                               RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPICSOAdapter) + "." + nameof(OnAuthorizeStopRequest));
            }

            #endregion


            DateTime              endtime;
            TimeSpan              runtime;
            WWCP.AuthStopResult?  authStopResult   = null;

            if (DisableAuthentication)
                authStopResult = AuthStopResult.AdminDown(
                                     AuthorizatorId:           Id,
                                     ISendAuthorizeStartStop:  this,
                                     SessionId:                SessionId,
                                     Description:              I18NString.Create(Languages.en, "Authentication is disabled!"),
                                     Runtime:                  TimeSpan.Zero
                                 );

            authStopResult ??= AuthStopResult.NotAuthorized(Id, this);


            endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            runtime = endtime - startTime;

            #region Send OnAuthorizeStopResponse event

            try
            {

                OnAuthorizeStopResponse?.Invoke(endtime,
                                                Timestamp.Value,
                                                this,
                                                Id.ToString(),
                                                EventTrackingId,
                                                RoamingNetwork.Id,
                                                null,
                                                Id,
                                                OperatorId,
                                                ChargingLocation,
                                                SessionId,
                                                CPOPartnerSessionId,
                                                LocalAuthentication,
                                                RequestTimeout,
                                                authStopResult,
                                                runtime);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPICSOAdapter) + "." + nameof(OnAuthorizeStopResponse));
            }

            #endregion

            return Task.FromResult(authStopResult);

        }

        #endregion


        #region SendChargeDetailRecords

        /// <summary>
        /// Send charge detail records to an OICP server.
        /// </summary>
        /// <param name="ChargeDetailRecords">An enumeration of charge detail records.</param>
        /// <param name="TransmissionType">Whether to send the CDR directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public async Task<WWCP.SendCDRsResult>

            SendChargeDetailRecords(IEnumerable<ChargeDetailRecord>  ChargeDetailRecords,
                                    TransmissionTypes                TransmissionType    = TransmissionTypes.Enqueue,

                                    DateTime?                        Timestamp           = null,
                                    CancellationToken?               CancellationToken   = null,
                                    EventTracking_Id?                EventTrackingId     = null,
                                    TimeSpan?                        RequestTimeout      = null)

        {

            #region Initial checks

            Timestamp         ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            CancellationToken ??= new CancellationTokenSource().Token;
            EventTrackingId   ??= EventTracking_Id.New;
            RequestTimeout    ??= this.RequestTimeout;

            #endregion

            #region Filter charge detail records

            var forwardedCDRs  = new List<WWCP.ChargeDetailRecord>();
            var filteredCDRs   = new List<WWCP.SendCDRResult>();

            foreach (var cdr in ChargeDetailRecords)
            {

                if (ChargeDetailRecordFilter(cdr) == WWCP.ChargeDetailRecordFilters.forward)
                    forwardedCDRs.Add(cdr);

                else
                    filteredCDRs.Add(WWCP.SendCDRResult.Filtered(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                                                 cdr,
                                                                 Warning.Create(I18NString.Create(Languages.en, "This charge detail record was filtered!"))));

            }

            #endregion

            #region Send OnSendCDRsRequest event

            var startTime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;

            try
            {

                OnSendCDRsRequest?.Invoke(startTime,
                                          Timestamp.Value,
                                          this,
                                          Id.ToString(),
                                          EventTrackingId,
                                          RoamingNetwork.Id,
                                          ChargeDetailRecords,
                                          RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPICSOAdapter) + "." + nameof(OnSendCDRsRequest));
            }

            #endregion


            #region if disabled => 'AdminDown'...

            DateTime         endtime;
            TimeSpan         runtime;
            SendCDRsResult?  sendCDRsResult   = null;

            if (DisableSendChargeDetailRecords)
            {

                endtime         = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                runtime         = endtime - startTime;
                sendCDRsResult  = SendCDRsResult.AdminDown(
                                      org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                      Id,
                                      this,
                                      ChargeDetailRecords,
                                      I18NString.Create(Languages.en, "Sending charge detail records is disabled!"),
                                      Runtime: runtime
                                  );

            }

            #endregion

            #region ..., or when there are no charge detail records...

            else if (!ChargeDetailRecords.Any())
            {

                endtime         = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                runtime         = endtime - startTime;
                sendCDRsResult  = SendCDRsResult.NoOperation(
                                      org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                      Id,
                                      this,
                                      ChargeDetailRecords,
                                      Runtime: runtime
                                  );

            }

            #endregion


            return SendCDRsResult.NoOperation(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, Id, this, ChargeDetailRecords);

        }

        #endregion



        #region Operator overloading

        #region Operator == (OCPICSOAdapter1, OCPICSOAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPICSOAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPICSOAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OCPICSOAdapter OCPICSOAdapter1,
                                           OCPICSOAdapter OCPICSOAdapter2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(OCPICSOAdapter1, OCPICSOAdapter2))
                return true;

            // If one is null, but not both, return false.
            if (OCPICSOAdapter1 is null || OCPICSOAdapter2 is null)
                return false;

            return OCPICSOAdapter1.Equals(OCPICSOAdapter2);

        }

        #endregion

        #region Operator != (OCPICSOAdapter1, OCPICSOAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPICSOAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPICSOAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OCPICSOAdapter OCPICSOAdapter1,
                                           OCPICSOAdapter OCPICSOAdapter2)

            => !(OCPICSOAdapter1 == OCPICSOAdapter2);

        #endregion

        #region Operator <  (OCPICSOAdapter1, OCPICSOAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPICSOAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPICSOAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OCPICSOAdapter OCPICSOAdapter1,
                                          OCPICSOAdapter OCPICSOAdapter2)
        {

            if (OCPICSOAdapter1 is null)
                throw new ArgumentNullException(nameof(OCPICSOAdapter1), "The given OCPI CSO adapter must not be null!");

            return OCPICSOAdapter1.CompareTo(OCPICSOAdapter2) < 0;

        }

        #endregion

        #region Operator <= (OCPICSOAdapter1, OCPICSOAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPICSOAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPICSOAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OCPICSOAdapter OCPICSOAdapter1,
                                           OCPICSOAdapter OCPICSOAdapter2)

            => !(OCPICSOAdapter1 > OCPICSOAdapter2);

        #endregion

        #region Operator >  (OCPICSOAdapter1, OCPICSOAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPICSOAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPICSOAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OCPICSOAdapter OCPICSOAdapter1,
                                          OCPICSOAdapter OCPICSOAdapter2)
        {

            if (OCPICSOAdapter1 is null)
                throw new ArgumentNullException(nameof(OCPICSOAdapter1), "The given OCPI CSO adapter must not be null!");

            return OCPICSOAdapter1.CompareTo(OCPICSOAdapter2) > 0;

        }

        #endregion

        #region Operator >= (OCPICSOAdapter1, OCPICSOAdapter2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OCPICSOAdapter1">An OCPI CSO adapter.</param>
        /// <param name="OCPICSOAdapter2">Another OCPI CSO adapter.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OCPICSOAdapter OCPICSOAdapter1,
                                           OCPICSOAdapter OCPICSOAdapter2)

            => !(OCPICSOAdapter1 < OCPICSOAdapter2);

        #endregion

        #endregion

        #region IComparable<OCPICSOAdapter> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two OCPI CSO adapters.
        /// </summary>
        /// <param name="Object">An OCPI CSO adapter to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is OCPICSOAdapter evseDataRecord
                   ? CompareTo(evseDataRecord)
                   : throw new ArgumentException("The given object is not an OCPI CSO adapter!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OCPICSOAdapter)

        /// <summary>
        /// Compares two OCPI CSO adapters.
        /// </summary>
        /// <param name="OCPICSOAdapter">An OCPI CSO adapter to compare with.</param>
        public Int32 CompareTo(OCPICSOAdapter? OCPICSOAdapter)
        {

            if (OCPICSOAdapter is null)
                throw new ArgumentNullException(nameof(OCPICSOAdapter),
                                                "The given OCPI CSO adapter must not be null!");

            return Id.CompareTo(OCPICSOAdapter.Id);

        }

        #endregion

        #endregion

        #region IEquatable<OCPICSOAdapter> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two OCPI CSO adapters for equality.
        /// </summary>
        /// <param name="Object">An OCPI CSO adapter to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is OCPICSOAdapter ocpiCSOAdapter &&
                   Equals(ocpiCSOAdapter);

        #endregion

        #region Equals(OCPICSOAdapter)

        /// <summary>
        /// Compares two OCPI CSO adapters for equality.
        /// </summary>
        /// <param name="OCPICSOAdapter">An OCPI CSO adapter to compare with.</param>
        public Boolean Equals(OCPICSOAdapter? OCPICSOAdapter)

            => OCPICSOAdapter is not null &&
                   Id.Equals(OCPICSOAdapter.Id);

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
