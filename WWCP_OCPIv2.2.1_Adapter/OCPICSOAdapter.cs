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

using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{


    public delegate IEnumerable<Tariff_Id>  GetTariffIds_Delegate(WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId,
                                                                  WWCP.ChargingPool_Id?             ChargingPoolId,
                                                                  WWCP.ChargingStation_Id?          ChargingStationId,
                                                                  WWCP.EVSE_Id?                     EVSEId,
                                                                  WWCP.ChargingConnector_Id?        ChargingConnectorId,
                                                                  WWCP.EMobilityProvider_Id?        EMobilityProviderId);



    /// <summary>
    /// Receive charging stations downstream from an OCPI partner...
    /// </summary>
    public class OCPICSOAdapter : WWCP.AWWCPEMPAdapter<CDR>,
                                  WWCP.IEMPRoamingProvider,
                                  WWCP.ISendEnergyStatus,
                                  IEquatable <OCPICSOAdapter>,
                                  IComparable<OCPICSOAdapter>,
                                  IComparable
    {

        #region Data

        protected readonly  SemaphoreSlim  DataAndStatusLock            = new(1, 1);

        protected readonly  TimeSpan       MaxLockWaitingTime           = TimeSpan.FromSeconds(120);


        /// <summary>
        /// The default logging context.
        /// </summary>
        public  const       String         DefaultLoggingContext        = "OCPIv2.1.1_CSOAdapter";

        public  const       String         DefaultHTTPAPI_LoggingPath   = "default";

        public  const       String         DefaultHTTPAPI_LogfileName   = "OCPIv2.1.1_CSOAdapter.log";


        /// <summary>
        /// The request timeout.
        /// </summary>
        public readonly     TimeSpan       RequestTimeout               = TimeSpan.FromSeconds(60);

        #endregion

        #region Properties

        public IId AuthId => Id;

        IId WWCP.ISendChargeDetailRecords.SendChargeDetailRecordsId => Id;

        public HTTP.CommonAPI                               CommonAPI                            { get; }

        public HTTP.CPOAPI                                  CPOAPI                               { get; }


        public GetTariffIds_Delegate?                       GetTariffIds                         { get; }


        public WWCPEVSEId_2_EVSEUId_Delegate?               CustomEVSEUIdConverter               { get; }
        public WWCPEVSEId_2_EVSEId_Delegate?                CustomEVSEIdConverter                { get; }
        public WWCPEVSE_2_EVSE_Delegate?                    CustomEVSEConverter                  { get; }
        public WWCPEVSEStatusUpdate_2_StatusType_Delegate?  CustomEVSEStatusUpdateConverter      { get; }
        public WWCPChargeDetailRecord_2_CDR_Delegate?       CustomChargeDetailRecordConverter    { get; }

        #endregion

        #region Events

        // from OCPI CSO
        //public event OnAuthorizeStartRequestDelegate?            OnAuthorizeStartRequest;
        //public event OnAuthorizeStartResponseDelegate?           OnAuthorizeStartResponse;

        //public event OnAuthorizeStopRequestDelegate?             OnAuthorizeStopRequest;
        //public event OnAuthorizeStopResponseDelegate?            OnAuthorizeStopResponse;

        public event WWCP.OnNewChargingSessionDelegate?          OnNewChargingSession;

        public event WWCP.OnSendCDRsRequestDelegate?             OnChargeDetailRecordRequest;
        public event WWCP.OnSendCDRsResponseDelegate?            OnChargeDetailRecordResponse;
        public event WWCP.OnNewChargeDetailRecordDelegate?       OnNewChargeDetailRecord;



        // from OCPI EMSP
        public event WWCP.OnReserveRequestDelegate?              OnReserveRequest;
        public event WWCP.OnReserveResponseDelegate?             OnReserveResponse;
        public event WWCP.OnNewReservationDelegate?              OnNewReservation;

        public event WWCP.OnCancelReservationRequestDelegate?    OnCancelReservationRequest;
        public event WWCP.OnCancelReservationResponseDelegate?   OnCancelReservationResponse;
        public event WWCP.OnReservationCanceledDelegate?         OnReservationCanceled;

        public event WWCP.OnRemoteStartRequestDelegate?          OnRemoteStartRequest;
        public event WWCP.OnRemoteStartResponseDelegate?         OnRemoteStartResponse;

        public event WWCP.OnRemoteStopRequestDelegate?           OnRemoteStopRequest;
        public event WWCP.OnRemoteStopResponseDelegate?          OnRemoteStopResponse;

        public event WWCP.OnGetCDRsRequestDelegate?              OnGetChargeDetailRecordsRequest;
        public event WWCP.OnGetCDRsResponseDelegate?             OnGetChargeDetailRecordsResponse;
        //public event OnSendCDRsResponseDelegate?                 OnSendCDRsResponse;


        // WWCP

        #region OnAuthorizeStartRequest/-Response

        /// <summary>
        /// An event fired whenever an authentication token will be verified for charging.
        /// </summary>
        public event WWCP.OnAuthorizeStartRequestDelegate?    OnAuthorizeStartRequest;

        /// <summary>
        /// An event fired whenever an authentication token had been verified for charging.
        /// </summary>
        public event WWCP.OnAuthorizeStartResponseDelegate?   OnAuthorizeStartResponse;

        #endregion

        #region OnAuthorizeStopRequest/-Response

        /// <summary>
        /// An event fired whenever an authentication token will be verified to stop a charging process.
        /// </summary>
        public event WWCP.OnAuthorizeStopRequestDelegate?    OnAuthorizeStopRequest;

        /// <summary>
        /// An event fired whenever an authentication token had been verified to stop a charging process.
        /// </summary>
        public event WWCP.OnAuthorizeStopResponseDelegate?   OnAuthorizeStopResponse;

        #endregion


        #region OnSendCDRsRequest/-Response

        /// <summary>
        /// An event fired whenever a charge detail record was enqueued for later sending upstream.
        /// </summary>
        public event WWCP.OnSendCDRsRequestDelegate?    OnEnqueueSendCDRsRequest;

        /// <summary>
        /// An event fired whenever a charge detail record will be send upstream.
        /// </summary>
        public event WWCP.OnSendCDRsRequestDelegate?    OnSendCDRsRequest;

        /// <summary>
        /// An event fired whenever a charge detail record had been sent upstream.
        /// </summary>
        public event WWCP.OnSendCDRsResponseDelegate?   OnSendCDRsResponse;

        #endregion

        #endregion


        #region Constructor(s)

        public OCPICSOAdapter(WWCP.EMPRoamingProvider_Id                      Id,
                              I18NString                                      Name,
                              I18NString                                      Description,
                              WWCP.RoamingNetwork                             RoamingNetwork,

                              HTTP.CommonAPI                                  CommonAPI,
                              CountryCode                                     DefaultCountryCode,
                              Party_Id                                        DefaultPartyId,

                              GetTariffIds_Delegate?                          GetTariffIds                        = null,

                              WWCPEVSEId_2_EVSEUId_Delegate?                  CustomEVSEUIdConverter              = null,
                              WWCPEVSEId_2_EVSEId_Delegate?                   CustomEVSEIdConverter               = null,
                              WWCPEVSE_2_EVSE_Delegate?                       CustomEVSEConverter                 = null,
                              WWCPEVSEStatusUpdate_2_StatusType_Delegate?     CustomEVSEStatusUpdateConverter     = null,
                              WWCPChargeDetailRecord_2_CDR_Delegate?          CustomChargeDetailRecordConverter   = null,

                              WWCP.IncludeChargingStationOperatorIdDelegate?  IncludeChargingStationOperatorIds   = null,
                              WWCP.IncludeChargingStationOperatorDelegate?    IncludeChargingStationOperators     = null,
                              WWCP.IncludeChargingPoolIdDelegate?             IncludeChargingPoolIds              = null,
                              WWCP.IncludeChargingPoolDelegate?               IncludeChargingPools                = null,
                              WWCP.IncludeChargingStationIdDelegate?          IncludeChargingStationIds           = null,
                              WWCP.IncludeChargingStationDelegate?            IncludeChargingStations             = null,
                              WWCP.IncludeEVSEIdDelegate?                     IncludeEVSEIds                      = null,
                              WWCP.IncludeEVSEDelegate?                       IncludeEVSEs                        = null,
                              WWCP.ChargeDetailRecordFilterDelegate?          ChargeDetailRecordFilter            = null,

                              TimeSpan?                                       FlushEVSEDataAndStatusEvery         = null,
                              TimeSpan?                                       FlushEVSEFastStatusEvery            = null,
                              TimeSpan?                                       FlushChargeDetailRecordsEvery       = null,

                              Boolean                                         DisablePushData                     = false,
                              Boolean                                         DisablePushAdminStatus              = false,
                              Boolean                                         DisablePushStatus                   = false,
                              Boolean                                         DisablePushEnergyStatus             = false,
                              Boolean                                         DisableAuthentication               = false,
                              Boolean                                         DisableSendChargeDetailRecords      = false,

                              String                                          EllipticCurve                       = "P-256",
                              ECPrivateKeyParameters?                         PrivateKey                          = null,
                              WWCP.PublicKeyCertificates?                     PublicKeyCertificates               = null,

                              Boolean?                                        IsDevelopment                       = null,
                              IEnumerable<String>?                            DevelopmentServers                  = null,
                              Boolean?                                        DisableLogging                      = false,
                              String?                                         LoggingPath                         = DefaultHTTPAPI_LoggingPath,
                              String?                                         LoggingContext                      = DefaultLoggingContext,
                              String?                                         LogfileName                         = DefaultHTTPAPI_LogfileName,
                              LogfileCreatorDelegate?                         LogfileCreator                      = null,

                              String?                                         ClientsLoggingPath                  = DefaultHTTPAPI_LoggingPath,
                              String?                                         ClientsLoggingContext               = DefaultLoggingContext,
                              LogfileCreatorDelegate?                         ClientsLogfileCreator               = null,
                              DNSClient?                                      DNSClient                           = null)

            : base(Id,
                   RoamingNetwork,

                   Name,
                   Description,

                   IncludeEVSEIds,
                   IncludeEVSEs,
                   IncludeChargingStationIds,
                   IncludeChargingStations,
                   IncludeChargingPoolIds,
                   IncludeChargingPools,
                   IncludeChargingStationOperatorIds,
                   IncludeChargingStationOperators,
                   ChargeDetailRecordFilter,

                   FlushEVSEDataAndStatusEvery,
                   FlushEVSEFastStatusEvery,
                   null,
                   FlushChargeDetailRecordsEvery,

                   DisablePushData,
                   DisablePushAdminStatus,
                   DisablePushStatus,
                   true,
                   DisablePushEnergyStatus,
                   DisableAuthentication,
                   DisableSendChargeDetailRecords,

                   EllipticCurve,
                   PrivateKey,
                   PublicKeyCertificates,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LoggingContext,
                   LogfileName,
                   LogfileCreator,

                   ClientsLoggingPath,
                   ClientsLoggingContext,
                   ClientsLogfileCreator,
                   DNSClient)

        {

            this.CommonAPI                          = CommonAPI;

            this.GetTariffIds                       = GetTariffIds;

            if (this.GetTariffIds is not null) {

                this.CommonAPI.GetTariffIdsDelegate += (cpoCountryCode,
                                                        cpoPartyId,
                                                        locationId,
                                                        evseUId,
                                                        connectorId,
                                                        empId) =>

                    this.GetTariffIds(                       WWCP.ChargingStationOperator_Id.Parse($"{cpoCountryCode}*{cpoPartyId}"),
                                      locationId. HasValue ? WWCP.ChargingPool_Id.           Parse(locationId. Value.ToString()) : null,
                                      null,
                                      evseUId.    HasValue ? WWCP.EVSE_Id.                   Parse(evseUId.    Value.ToString()) : null,
                                      connectorId.HasValue ? WWCP.ChargingConnector_Id.      Parse(connectorId.Value.ToString()) : null,
                                      empId.      HasValue ? WWCP.EMobilityProvider_Id.      Parse(empId.      Value.ToString()) : null);

            }

            this.CustomEVSEUIdConverter             = CustomEVSEUIdConverter;
            this.CustomEVSEIdConverter              = CustomEVSEIdConverter;
            this.CustomEVSEConverter                = CustomEVSEConverter;
            this.CustomEVSEStatusUpdateConverter    = CustomEVSEStatusUpdateConverter;
            this.CustomChargeDetailRecordConverter  = CustomChargeDetailRecordConverter;

            this.CPOAPI                             = new HTTP.CPOAPI(

                                                          this.CommonAPI,
                                                          DefaultCountryCode,
                                                          DefaultPartyId,
                                                          null, // AllowDowngrades

                                                          null, // HTTPHostname
                                                          null, // ExternalDNSName
                                                          null, // HTTPServiceName
                                                          null, // BasePath

                                                          CommonAPI.URLPathPrefix + "2.2/cpo",
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
                                                          LoggingContext,
                                                          LoggingPath,
                                                          LogfileName,
                                                          LogfileCreator
                                                          // Autostart

                                                      );

        }

        #endregion


        //public IEnumerable<Tariff_Id> GetTariffIds(WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId,
        //                                           WWCP.ChargingPool_Id?             ChargingPoolId,
        //                                           WWCP.ChargingStation_Id?          ChargingStationId,
        //                                           WWCP.EVSE_Id?                     EVSEId,
        //                                           WWCP.ChargingConnector_Id?        ChargingConnectorId)
        //{

        //    var isDC = EVSEId.HasValue && ChargingConnectorId.HasValue
        //                   ? RoamingNetwork.GetEVSEById(EVSEId.Value)?.ChargingConnectors.First(chargingConnector => chargingConnector.Id == ChargingConnectorId.Value).IsDC
        //                   : null;

        //    if (isDC == true)
        //        return new[] { Tariff_Id.Parse("DC") };

        //    if (isDC == false)
        //        return new[] { Tariff_Id.Parse("AC") };

        //    return Array.Empty<Tariff_Id>();

        //}


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
                                      Boolean?         AccessTokenBase64Encoding   = null,
                                      AccessStatus     AccessStatus                = AccessStatus.ALLOWED,

                                      PartyStatus      PartyStatus                 = PartyStatus. ENABLED)
        {

            return CommonAPI.AddRemoteParty(CountryCode,
                                            PartyId,
                                            Role,
                                            BusinessDetails,
                                            AccessToken,
                                            AccessTokenBase64Encoding,
                                            AccessStatus,
                                            PartyStatus);
        }

        #endregion



        #region (Set/Add/Update/Delete) Roaming network...
        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.SetStaticData(WWCP.IRoamingNetwork    RoamingNetwork,
                                                                      WWCP.TransmissionTypes  TransmissionType,
                                                                      DateTime?               Timestamp,
                                                                      CancellationToken       CancellationToken,
                                                                      EventTracking_Id?       EventTrackingId,
                                                                      TimeSpan?               RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.AddStaticData(WWCP.IRoamingNetwork    RoamingNetwork,
                                                                      WWCP.TransmissionTypes  TransmissionType,
                                                                      DateTime?               Timestamp,
                                                                      CancellationToken       CancellationToken,
                                                                      EventTracking_Id?       EventTrackingId,
                                                                      TimeSpan?               RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.UpdateStaticData(WWCP.IRoamingNetwork    RoamingNetwork,
                                                                         String                  PropertyName,
                                                                         Object?                 NewValue,
                                                                         Object?                 OldValue,
                                                                         Context?                DataSource,
                                                                         WWCP.TransmissionTypes  TransmissionType,
                                                                         DateTime?               Timestamp,
                                                                         CancellationToken       CancellationToken,
                                                                         EventTracking_Id?       EventTrackingId,
                                                                         TimeSpan?               RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.DeleteStaticData(WWCP.IRoamingNetwork    RoamingNetwork,
                                                                         WWCP.TransmissionTypes  TransmissionType,
                                                                         DateTime?               Timestamp,
                                                                         CancellationToken       CancellationToken,
                                                                         EventTracking_Id?       EventTrackingId,
                                                                         TimeSpan?               RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }


        Task<WWCP.PushRoamingNetworkAdminStatusResult> WWCP.ISendAdminStatus.UpdateAdminStatus(IEnumerable<WWCP.RoamingNetworkAdminStatusUpdate>  AdminStatusUpdates,
                                                                                               WWCP.TransmissionTypes                             TransmissionType,
                                                                                               DateTime?                                          Timestamp,
                                                                                               CancellationToken                                  CancellationToken,
                                                                                               EventTracking_Id?                                  EventTrackingId,
                                                                                               TimeSpan?                                          RequestTimeout)
        {
            return Task.FromResult(WWCP.PushRoamingNetworkAdminStatusResult.NoOperation(Id, this));
        }

        Task<WWCP.PushRoamingNetworkStatusResult> WWCP.ISendStatus.UpdateStatus(IEnumerable<WWCP.RoamingNetworkStatusUpdate>  StatusUpdates,
                                                                                WWCP.TransmissionTypes                        TransmissionType,
                                                                                DateTime?                                     Timestamp,
                                                                                CancellationToken                             CancellationToken,
                                                                                EventTracking_Id?                             EventTrackingId,
                                                                                TimeSpan?                                     RequestTimeout)
        {
            return Task.FromResult(WWCP.PushRoamingNetworkStatusResult.NoOperation(Id, this));
        }

        #endregion

        #region (Set/Add/Update/Delete) Charging station operator(s)...

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.SetStaticData(WWCP.IChargingStationOperator  ChargingStationOperator,
                                                                      WWCP.TransmissionTypes         TransmissionType,
                                                                      DateTime?                      Timestamp,
                                                                      CancellationToken              CancellationToken,
                                                                      EventTracking_Id?              EventTrackingId,
                                                                      TimeSpan?                      RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.AddStaticData(WWCP.IChargingStationOperator  ChargingStationOperator,
                                                                      WWCP.TransmissionTypes         TransmissionType,
                                                                      DateTime?                      Timestamp,
                                                                      CancellationToken              CancellationToken,
                                                                      EventTracking_Id?              EventTrackingId,
                                                                      TimeSpan?                      RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.UpdateStaticData(WWCP.IChargingStationOperator  ChargingStationOperator,
                                                                         String                         PropertyName,
                                                                         Object?                        NewValue,
                                                                         Object?                        OldValue,
                                                                         Context?                       DataSource,
                                                                         WWCP.TransmissionTypes         TransmissionType,
                                                                         DateTime?                      Timestamp,
                                                                         CancellationToken              CancellationToken,
                                                                         EventTracking_Id?              EventTrackingId,
                                                                         TimeSpan?                      RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.DeleteStaticData(WWCP.IChargingStationOperator  ChargingStationOperator,
                                                                         WWCP.TransmissionTypes         TransmissionType,
                                                                         DateTime?                      Timestamp,
                                                                         CancellationToken              CancellationToken,
                                                                         EventTracking_Id?              EventTrackingId,
                                                                         TimeSpan?                      RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }



        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.SetStaticData(IEnumerable<WWCP.IChargingStationOperator>  ChargingStationOperators,
                                                                      WWCP.TransmissionTypes                      TransmissionType,
                                                                      DateTime?                                   Timestamp,
                                                                      CancellationToken                           CancellationToken,
                                                                      EventTracking_Id?                           EventTrackingId,
                                                                      TimeSpan?                                   RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.AddStaticData(IEnumerable<WWCP.IChargingStationOperator>  ChargingStationOperators,
                                                                      WWCP.TransmissionTypes                      TransmissionType,
                                                                      DateTime?                                   Timestamp,
                                                                      CancellationToken                           CancellationToken,
                                                                      EventTracking_Id?                           EventTrackingId,
                                                                      TimeSpan?                                   RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.UpdateStaticData(IEnumerable<WWCP.IChargingStationOperator>  ChargingStationOperators,
                                                                         WWCP.TransmissionTypes                      TransmissionType,
                                                                         DateTime?                                   Timestamp,
                                                                         CancellationToken                           CancellationToken,
                                                                         EventTracking_Id?                           EventTrackingId,
                                                                         TimeSpan?                                   RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }

        Task<WWCP.PushEVSEDataResult> WWCP.ISendPOIData.DeleteStaticData(IEnumerable<WWCP.IChargingStationOperator>  ChargingStationOperators,
                                                                         WWCP.TransmissionTypes                      TransmissionType,
                                                                         DateTime?                                   Timestamp,
                                                                         CancellationToken                           CancellationToken,
                                                                         EventTracking_Id?                           EventTrackingId,
                                                                         TimeSpan?                                   RequestTimeout)
        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, null));
        }



        Task<WWCP.PushChargingStationOperatorAdminStatusResult> WWCP.ISendAdminStatus.UpdateAdminStatus(IEnumerable<WWCP.ChargingStationOperatorAdminStatusUpdate>  AdminStatusUpdates,
                                                                                                        WWCP.TransmissionTypes                                      TransmissionType,
                                                                                                        DateTime?                                                   Timestamp,
                                                                                                        CancellationToken                                           CancellationToken,
                                                                                                        EventTracking_Id?                                           EventTrackingId,
                                                                                                        TimeSpan?                                                   RequestTimeout)
        {
            return Task.FromResult(WWCP.PushChargingStationOperatorAdminStatusResult.NoOperation(Id, this));
        }

        Task<WWCP.PushChargingStationOperatorStatusResult> WWCP.ISendStatus.UpdateStatus(IEnumerable<WWCP.ChargingStationOperatorStatusUpdate>  StatusUpdates,
                                                                                         WWCP.TransmissionTypes                                 TransmissionType,
                                                                                         DateTime?                                              Timestamp,
                                                                                         CancellationToken                                      CancellationToken,
                                                                                         EventTracking_Id?                                      EventTrackingId,
                                                                                         TimeSpan?                                              RequestTimeout)
        {
            return Task.FromResult(WWCP.PushChargingStationOperatorStatusResult.NoOperation(Id, this));
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
                                            CancellationToken       CancellationToken,
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


                        var location = ChargingPool.ToOCPI(CustomEVSEUIdConverter,
                                                           CustomEVSEIdConverter,
                                                           out warnings);

                        if (location is not null)
                            CommonAPI.AddLocation(location);


                        return WWCP.PushChargingPoolDataResult.Enqueued(
                                    Id,
                                    this,
                                    new WWCP.IChargingPool[] {
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
                                    new WWCP.IChargingPool[] {
                                        ChargingPool
                                    }
                                );

                }
                else
                    return WWCP.PushChargingPoolDataResult.LockTimeout(
                                Id,
                                this,
                                new WWCP.IChargingPool[] {
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
                                            CancellationToken       CancellationToken,
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

                        var location = ChargingPool.ToOCPI(CustomEVSEUIdConverter,
                                                           CustomEVSEIdConverter,
                                                           out warnings);

                        if (location is not null)
                        {

                            var result = CommonAPI.AddLocation(location);


                            //ToDo: Process errors!!!


                        }

                        return WWCP.PushChargingPoolDataResult.Enqueued(
                                   Id,
                                   this,
                                   new WWCP.IChargingPool[] {
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
                                   new WWCP.IChargingPool[] {
                                       ChargingPool
                                   }
                               );

                }
                else
                    return WWCP.PushChargingPoolDataResult.LockTimeout(
                               Id,
                               this,
                               new WWCP.IChargingPool[] {
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

        #region UpdateStaticData(ChargingPool, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

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
        public async Task<WWCP.PushChargingPoolDataResult> UpdateStaticData(WWCP.IChargingPool      ChargingPool,
                                                                            String?                 PropertyName        = null,
                                                                            Object?                 OldValue            = null,
                                                                            Object?                 NewValue            = null,
                                                                            WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,
                                                                            DateTime?               Timestamp           = null,
                                                                            CancellationToken?      CancellationToken   = null,
                                                                            EventTracking_Id?       EventTrackingId     = null,
                                                                            TimeSpan?               RequestTimeout      = null)
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

                        var location = ChargingPool.ToOCPI(CustomEVSEUIdConverter,
                                                           CustomEVSEIdConverter,
                                                           out warnings);

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
                                       new WWCP.IChargingPool[] { ChargingPool },
                                       String.Empty,
                                       warnings,
                                       TimeSpan.Zero
                                   );

                        }

                    }

                    return WWCP.PushChargingPoolDataResult.NoOperation(
                               Id,
                               this,
                               new WWCP.IChargingPool[] { ChargingPool },
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

            return WWCP.PushChargingPoolDataResult.LockTimeout(
                       Id,
                       this,
                       new WWCP.IChargingPool[] { ChargingPool },
                       "",
                       Array.Empty<Warning>(),
                       TimeSpan.Zero
                   );

        }

        #endregion

        #region DeleteStaticData(ChargingPool, TransmissionType = Enqueue, ...)

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
                                               CancellationToken        CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(WWCP.PushChargingPoolDataResult.NoOperation(Id, this, new WWCP.IChargingPool[] { ChargingPool }));

        }

        #endregion


        Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.SetStaticData(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                                            WWCP.TransmissionTypes           TransmissionType,

                                            DateTime?                        Timestamp,
                                            CancellationToken                CancellationToken,
                                            EventTracking_Id?                EventTrackingId,
                                            TimeSpan?                        RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingPoolDataResult.NoOperation(Id, this, ChargingPools));
        }

        Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.AddStaticData(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                                            WWCP.TransmissionTypes           TransmissionType,

                                            DateTime?                        Timestamp,
                                            CancellationToken                CancellationToken,
                                            EventTracking_Id?                EventTrackingId,
                                            TimeSpan?                        RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingPoolDataResult.NoOperation(Id, this, ChargingPools));
        }

        Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.UpdateStaticData(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                                               WWCP.TransmissionTypes           TransmissionType,

                                               DateTime?                        Timestamp,
                                               CancellationToken                CancellationToken,
                                               EventTracking_Id?                EventTrackingId,
                                               TimeSpan?                        RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingPoolDataResult.NoOperation(Id, this, ChargingPools));
        }

        Task<WWCP.PushChargingPoolDataResult>

            WWCP.ISendPOIData.DeleteStaticData(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                                               WWCP.TransmissionTypes           TransmissionType,

                                               DateTime?                        Timestamp,
                                               CancellationToken                CancellationToken,
                                               EventTracking_Id?                EventTrackingId,
                                               TimeSpan?                        RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingPoolDataResult.NoOperation(Id, this, ChargingPools));
        }


        Task<WWCP.PushChargingPoolAdminStatusResult>

            WWCP.ISendAdminStatus.UpdateAdminStatus(IEnumerable<WWCP.ChargingPoolAdminStatusUpdate>  AdminStatusUpdates,
                                                    WWCP.TransmissionTypes                           TransmissionType,

                                                    DateTime?                                        Timestamp,
                                                    CancellationToken                                CancellationToken,
                                                    EventTracking_Id?                                EventTrackingId,
                                                    TimeSpan?                                        RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingPoolAdminStatusResult.NoOperation(Id, this));
        }

        Task<WWCP.PushChargingPoolStatusResult>

            WWCP.ISendStatus.UpdateStatus(IEnumerable<WWCP.ChargingPoolStatusUpdate>  StatusUpdates,
                                          WWCP.TransmissionTypes                      TransmissionType,

                                          DateTime?                                   Timestamp,
                                          CancellationToken                           CancellationToken,
                                          EventTracking_Id?                           EventTrackingId,
                                          TimeSpan?                                   RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingPoolStatusResult.NoOperation(Id, this));
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
                                            CancellationToken       CancellationToken,
                                            EventTracking_Id?       EventTrackingId,
                                            TimeSpan?               RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var countryCode  = CountryCode.TryParse(ChargingStation.Id.OperatorId.CountryCode.Alpha2Code);
                    var partyId      = Party_Id.   TryParse(ChargingStation.Id.OperatorId.Suffix);
                    var locationId   = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (countryCode.HasValue &&
                        partyId.    HasValue &&
                        locationId. HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(countryCode.Value,
                                                             partyId.    Value,
                                                             locationId. Value,
                                                             out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(CustomEVSEUIdConverter,
                                                            CustomEVSEIdConverter,
                                                            out var warning);

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
                        ? WWCP.PushChargingStationDataResult.Enqueued   (Id, this, new WWCP.IChargingStation[] { ChargingStation })
                        : WWCP.PushChargingStationDataResult.LockTimeout(Id, this, new WWCP.IChargingStation[] { ChargingStation });

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
                                            CancellationToken        CancellationToken,
                                            EventTracking_Id?        EventTrackingId,
                                            TimeSpan?                RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var countryCode  = CountryCode.TryParse(ChargingStation.Id.OperatorId.CountryCode.Alpha2Code);
                    var partyId      = Party_Id.   TryParse(ChargingStation.Id.OperatorId.Suffix);
                    var locationId   = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (countryCode.HasValue &&
                        partyId.    HasValue &&
                        locationId. HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(countryCode.Value,
                                                             partyId.    Value,
                                                             locationId. Value,
                                                             out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(CustomEVSEUIdConverter,
                                                            CustomEVSEIdConverter,
                                                            out var warning);

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
                       ? WWCP.PushChargingStationDataResult.Enqueued   (Id, this, new WWCP.IChargingStation[] { ChargingStation })
                       : WWCP.PushChargingStationDataResult.LockTimeout(Id, this, new WWCP.IChargingStation[] { ChargingStation });

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
                                               String                   PropertyName,
                                               Object?                  NewValue,
                                               Object?                  OldValue,
                                               Context?                 DataSource,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken        CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    var countryCode  = CountryCode.TryParse(ChargingStation.Id.OperatorId.CountryCode.Alpha2Code);
                    var partyId      = Party_Id.   TryParse(ChargingStation.Id.OperatorId.Suffix);
                    var locationId   = Location_Id.TryParse(ChargingStation.ChargingPool?.Id.ToString() ?? "");

                    if (countryCode.HasValue &&
                        partyId.    HasValue &&
                        locationId. HasValue)
                    {

                        var warnings  = new List<Warning>();
                        var results   = new List<AddOrUpdateResult<EVSE>>();

                        foreach (var evse in ChargingStation)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                if (CommonAPI.TryGetLocation(countryCode.Value,
                                                             partyId.    Value,
                                                             locationId. Value,
                                                             out var location) &&
                                    location is not null)
                                {

                                    var evse2 = evse.ToOCPI(CustomEVSEUIdConverter,
                                                            CustomEVSEIdConverter,
                                                            out var warning);

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
                       ? WWCP.PushChargingStationDataResult.Enqueued   (Id, this, new WWCP.IChargingStation[] { ChargingStation })
                       : WWCP.PushChargingStationDataResult.LockTimeout(Id, this, new WWCP.IChargingStation[] { ChargingStation });

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
                                               CancellationToken        CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(WWCP.PushChargingStationDataResult.NoOperation(Id, this, new WWCP.IChargingStation[] { ChargingStation }));

        }

        #endregion


        Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.SetStaticData(IEnumerable<WWCP.IChargingStation>  ChargingStations,
                                            WWCP.TransmissionTypes              TransmissionType,

                                            DateTime?                           Timestamp,
                                            CancellationToken                   CancellationToken,
                                            EventTracking_Id?                   EventTrackingId,
                                            TimeSpan?                           RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingStationDataResult.NoOperation(Id, this, ChargingStations));
        }

        Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.AddStaticData(IEnumerable<WWCP.IChargingStation>  ChargingStations,
                                            WWCP.TransmissionTypes              TransmissionType,

                                            DateTime?                           Timestamp,
                                            CancellationToken                   CancellationToken,
                                            EventTracking_Id?                   EventTrackingId,
                                            TimeSpan?                           RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingStationDataResult.NoOperation(Id, this, ChargingStations));
        }

        Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.UpdateStaticData(IEnumerable<WWCP.IChargingStation>  ChargingStations,
                                               WWCP.TransmissionTypes              TransmissionType,

                                               DateTime?                           Timestamp,
                                               CancellationToken                   CancellationToken,
                                               EventTracking_Id?                   EventTrackingId,
                                               TimeSpan?                           RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingStationDataResult.NoOperation(Id, this, ChargingStations));
        }

        Task<WWCP.PushChargingStationDataResult>

            WWCP.ISendPOIData.DeleteStaticData(IEnumerable<WWCP.IChargingStation>  ChargingStations,
                                               WWCP.TransmissionTypes              TransmissionType,

                                               DateTime?                           Timestamp,
                                               CancellationToken                   CancellationToken,
                                               EventTracking_Id?                   EventTrackingId,
                                               TimeSpan?                           RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingStationDataResult.NoOperation(Id, this, ChargingStations));
        }


        Task<WWCP.PushChargingStationAdminStatusResult>

            WWCP.ISendAdminStatus.UpdateAdminStatus(IEnumerable<WWCP.ChargingStationAdminStatusUpdate>  AdminStatusUpdates,
                                                    WWCP.TransmissionTypes                              TransmissionType,

                                                    DateTime?                                           Timestamp,
                                                    CancellationToken                                   CancellationToken,
                                                    EventTracking_Id?                                   EventTrackingId,
                                                    TimeSpan?                                           RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingStationAdminStatusResult.NoOperation(Id, this));
        }

        Task<WWCP.PushChargingStationStatusResult>
            WWCP.ISendStatus.UpdateStatus(IEnumerable<WWCP.ChargingStationStatusUpdate>  StatusUpdates,
                                          WWCP.TransmissionTypes                         TransmissionType,

                                          DateTime?                                      Timestamp,
                                          CancellationToken                              CancellationToken,
                                          EventTracking_Id?                              EventTrackingId,
                                          TimeSpan?                                      RequestTimeout)

        {
            return Task.FromResult(WWCP.PushChargingStationStatusResult.NoOperation(Id, this));
        }

        #endregion

        #region (Set/Add/Update/Delete) EVSE(s)...

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
                                            CancellationToken       CancellationToken,
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

                    var countryCode  = CountryCode.TryParse(EVSE.Id.OperatorId.CountryCode.Alpha2Code);
                    var partyId      = Party_Id.   TryParse(EVSE.Id.OperatorId.Suffix);
                    var locationId   = EVSE.ChargingPool is not null
                                           ? Location_Id.TryParse(EVSE.ChargingPool.Id.Suffix)
                                           : null;

                    if (countryCode.HasValue &&
                        partyId.    HasValue &&
                        locationId. HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(countryCode.Value,
                                                         partyId.    Value,
                                                         locationId. Value,
                                                         out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        out warnings);

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

                               ? new WWCP.PushEVSEDataResult(
                                       Id,
                                       this,
                                       WWCP.PushDataResultTypes.Success,
                                       new WWCP.PushSingleEVSEDataResult[] {
                                           new WWCP.PushSingleEVSEDataResult(
                                               EVSE,
                                               WWCP.PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<WWCP.PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new WWCP.PushEVSEDataResult(
                                       Id,
                                       this,
                                       WWCP.PushDataResultTypes.Error,
                                       Array.Empty<WWCP.PushSingleEVSEDataResult>(),
                                       new WWCP.PushSingleEVSEDataResult[] {
                                           new WWCP.PushSingleEVSEDataResult(
                                               EVSE,
                                               WWCP.PushSingleDataResultTypes.Success,
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
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new WWCP.IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new WWCP.IEVSE[] { EVSE });

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

            WWCP.ISendPOIData.AddStaticData(WWCP.IEVSE              EVSE,
                                            WWCP.TransmissionTypes  TransmissionType,

                                            DateTime?               Timestamp,
                                            CancellationToken       CancellationToken,
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

                    var countryCode  = CountryCode.TryParse(EVSE.Id.OperatorId.CountryCode.Alpha2Code);
                    var partyId      = Party_Id.   TryParse(EVSE.Id.OperatorId.Suffix);
                    var locationId   = EVSE.ChargingPool is not null
                                           ? Location_Id.TryParse(EVSE.ChargingPool.Id.Suffix)
                                           : null;

                    if (countryCode.HasValue &&
                        partyId.    HasValue &&
                        locationId. HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(countryCode.Value,
                                                         partyId.    Value,
                                                         locationId. Value,
                                                         out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        out warnings);

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

                               ? new WWCP.PushEVSEDataResult(
                                       Id,
                                       this,
                                       WWCP.PushDataResultTypes.Success,
                                       new WWCP.PushSingleEVSEDataResult[] {
                                           new WWCP.PushSingleEVSEDataResult(
                                               EVSE,
                                               WWCP.PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<WWCP.PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new WWCP.PushEVSEDataResult(
                                       Id,
                                       this,
                                       WWCP.PushDataResultTypes.Error,
                                       Array.Empty<WWCP.PushSingleEVSEDataResult>(),
                                       new WWCP.PushSingleEVSEDataResult[] {
                                           new WWCP.PushSingleEVSEDataResult(
                                               EVSE,
                                               WWCP.PushSingleDataResultTypes.Success,
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
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new WWCP.IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new WWCP.IEVSE[] { EVSE });

        }

        #endregion

        #region UpdateStaticData(ChargingPool, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

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
                                               String                  PropertyName,
                                               Object?                 NewValue,
                                               Object?                 OldValue,
                                               Context?                DataSource,
                                               WWCP.TransmissionTypes  TransmissionType,

                                               DateTime?               Timestamp,
                                               CancellationToken       CancellationToken,
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

                    var countryCode  = CountryCode.TryParse(EVSE.Id.OperatorId.CountryCode.Alpha2Code);
                    var partyId      = Party_Id.   TryParse(EVSE.Id.OperatorId.Suffix);
                    var locationId   = EVSE.ChargingPool is not null
                                           ? Location_Id.TryParse(EVSE.ChargingPool.Id.Suffix)
                                           : null;

                    if (countryCode.HasValue &&
                        partyId.    HasValue &&
                        locationId. HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(countryCode.Value,
                                                         partyId.    Value,
                                                         locationId. Value,
                                                         out var location) &&
                                location is not null)
                            {

                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        out warnings);

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

                               ? new WWCP.PushEVSEDataResult(
                                       Id,
                                       this,
                                       WWCP.PushDataResultTypes.Success,
                                       new WWCP.PushSingleEVSEDataResult[] {
                                           new WWCP.PushSingleEVSEDataResult(
                                               EVSE,
                                               WWCP.PushSingleDataResultTypes.Error,
                                               warnings
                                           )
                                       },
                                       Array.Empty<WWCP.PushSingleEVSEDataResult>(),
                                       result.ErrorResponse,
                                       warnings,
                                       TimeSpan.FromMilliseconds(10)
                                   )

                               : new WWCP.PushEVSEDataResult(
                                       Id,
                                       this,
                                       WWCP.PushDataResultTypes.Error,
                                       Array.Empty<WWCP.PushSingleEVSEDataResult>(),
                                       new WWCP.PushSingleEVSEDataResult[] {
                                           new WWCP.PushSingleEVSEDataResult(
                                               EVSE,
                                               WWCP.PushSingleDataResultTypes.Success,
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
                       ? WWCP.PushEVSEDataResult.Enqueued   (Id, this, new WWCP.IEVSE[] { EVSE })
                       : WWCP.PushEVSEDataResult.LockTimeout(Id, this, new WWCP.IEVSE[] { EVSE });

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
                                               CancellationToken        CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {

            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, new WWCP.IEVSE[] { EVSE }));

        }

        #endregion


        Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.SetStaticData(IEnumerable<WWCP.IEVSE>  EVSEs,
                                            WWCP.TransmissionTypes   TransmissionType,

                                            DateTime?                Timestamp,
                                            CancellationToken        CancellationToken,
                                            EventTracking_Id?        EventTrackingId,
                                            TimeSpan?                RequestTimeout)

        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, EVSEs));
        }

        Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.AddStaticData(IEnumerable<WWCP.IEVSE>  EVSEs,
                                            WWCP.TransmissionTypes   TransmissionType,

                                            DateTime?                Timestamp,
                                            CancellationToken        CancellationToken,
                                            EventTracking_Id?        EventTrackingId,
                                            TimeSpan?                RequestTimeout)

        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, EVSEs));
        }

        Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.UpdateStaticData(IEnumerable<WWCP.IEVSE>  EVSEs,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken        CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, EVSEs));
        }

        Task<WWCP.PushEVSEDataResult>

            WWCP.ISendPOIData.DeleteStaticData(IEnumerable<WWCP.IEVSE>  EVSEs,
                                               WWCP.TransmissionTypes   TransmissionType,

                                               DateTime?                Timestamp,
                                               CancellationToken        CancellationToken,
                                               EventTracking_Id?        EventTrackingId,
                                               TimeSpan?                RequestTimeout)

        {
            return Task.FromResult(WWCP.PushEVSEDataResult.NoOperation(Id, this, EVSEs));
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
                                                    CancellationToken                        CancellationToken,
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

            WWCP.ISendStatus.UpdateStatus(IEnumerable<WWCP.EVSEStatusUpdate>  StatusUpdates,
                                          WWCP.TransmissionTypes              TransmissionType,

                                          DateTime?                           Timestamp,
                                          CancellationToken                   CancellationToken,
                                          EventTracking_Id?                   EventTrackingId,
                                          TimeSpan?                           RequestTimeout)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    WWCP.PushEVSEStatusResult result;

                    var startTime  = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                    var warnings   = new List<Warning>();
                    var results    = new List<WWCP.PushEVSEStatusResult>();

                    foreach (var statusUpdate in StatusUpdates)
                    {

                        if (RoamingNetwork.TryGetEVSEById(statusUpdate.Id, out var evse) && evse is not null)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                var countryCode  = CountryCode.TryParse(evse.Id.OperatorId.CountryCode.Alpha2Code);
                                var partyId      = Party_Id.   TryParse(evse.Id.OperatorId.Suffix);
                                var locationId   = evse.ChargingPool is not null
                                                       ? Location_Id.TryParse(evse.ChargingPool.Id.Suffix)
                                                       : null;

                                if (countryCode.HasValue &&
                                    partyId.    HasValue &&
                                    locationId. HasValue)
                                {

                                    if (CommonAPI.TryGetLocation(countryCode.Value,
                                                                 partyId.    Value,
                                                                 locationId. Value,
                                                                 out var location) &&
                                        location is not null)
                                    {

                                        var evse2 = evse.ToOCPI(CustomEVSEUIdConverter,
                                                                CustomEVSEIdConverter,
                                                                ref warnings);

                                        if (evse2 is not null)
                                        {

                                            var result2 = await CommonAPI.AddOrUpdateEVSE(location, evse2);

                                            result = result2.IsSuccess
                                                         ? WWCP.PushEVSEStatusResult.Success(Id, this, null, warnings)
                                                         : WWCP.PushEVSEStatusResult.Failed (Id, this, StatusUpdates, result2.ErrorResponse, warnings);

                                        }
                                        else
                                            result = WWCP.PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Could not convert the given EVSE!");

                                    }
                                    else
                                        result = WWCP.PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Unknown location identification!");

                                }
                                else
                                    result = WWCP.PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "Invalid location identification!");

                            }
                            else
                                result = WWCP.PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "The given EVSE was filtered!");

                        }
                        else
                            result = WWCP.PushEVSEStatusResult.Failed(Id, this, StatusUpdates, "The given EVSE does not exist!");

                        results.Add(result);

                    }

                    return WWCP.PushEVSEStatusResult.Flatten(
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

            WWCP.ISendEnergyStatus.UpdateEnergyStatus(IEnumerable<WWCP.EVSEEnergyStatusUpdate>  EnergyStatusUpdates,
                                                      WWCP.TransmissionTypes                    TransmissionType,

                                                      DateTime?                                 Timestamp,
                                                      CancellationToken                         CancellationToken,
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
        public async Task<WWCP.AuthStartResult>

            AuthorizeStart(WWCP.LocalAuthentication          LocalAuthentication,
                           WWCP.ChargingLocation?            ChargingLocation      = null,
                           WWCP.ChargingProduct?             ChargingProduct       = null,
                           WWCP.ChargingSession_Id?          SessionId             = null,
                           WWCP.ChargingSession_Id?          CPOPartnerSessionId   = null,
                           WWCP.ChargingStationOperator_Id?  OperatorId            = null,

                           DateTime?                         Timestamp             = null,
                           CancellationToken                 CancellationToken     = default,
                           EventTracking_Id?                 EventTrackingId       = null,
                           TimeSpan?                         RequestTimeout        = null)

        {

            #region Initial checks

            Timestamp       ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            EventTrackingId ??= EventTracking_Id.New;
            RequestTimeout  ??= this.RequestTimeout;

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
                                                Array.Empty<WWCP.ISendAuthorizeStartStop>(),
                                                RequestTimeout);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPICSOAdapter) + "." + nameof(OnAuthorizeStartRequest));
            }

            #endregion


            if (DisableAuthentication)
                return WWCP.AuthStartResult.AdminDown(
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

                                                                                       var authToken        = LocalAuthentication.AuthToken?.ToString();

                                                                                       if (authToken is null)
                                                                                           return new AuthorizationInfo(
                                                                                                      Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                      Token:        null, //ToDo: Token should be optional within AuthorizationInfo!
                                                                                                      Info:         new DisplayText(Languages.en, $"The local authentication must not be null!"),
                                                                                                      RemoteParty:  remoteParty
                                                                                                  );


                                                                                       var tokenId = Token_Id.TryParse(authToken);

                                                                                       if (!tokenId.HasValue)
                                                                                           return new AuthorizationInfo(
                                                                                                      Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                      Token:        null, //ToDo: Token should be optional within AuthorizationInfo!
                                                                                                      Info:         new DisplayText(Languages.en, $"The token identification is invalid!"),
                                                                                                      RemoteParty:  remoteParty
                                                                                                  );


                                                                                       var remoteAccessInfo  = remoteParty.RemoteAccessInfos.FirstOrDefault(remoteAccessInfo => remoteAccessInfo.Status == RemoteAccessStatus.ONLINE);

                                                                                       if (remoteAccessInfo is null)
                                                                                           return new AuthorizationInfo(
                                                                                                      Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                      Token:        null, //ToDo: Token should be optional within AuthorizationInfo!
                                                                                                      Info:         new DisplayText(Languages.en, $"No remote access information for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                                                                                                      RemoteParty:  remoteParty
                                                                                                  );


                                                                                       var cpoClient = new CPO.HTTP.CPOClient(

                                                                                                           remoteParty,
                                                                                                           CommonAPI,
                                                                                                           null, // VirtualHostname
                                                                                                           null, // Description
                                                                                                           null, // HTTPLogger

                                                                                                           DisableLogging,
                                                                                                           ClientsLoggingPath ?? DefaultHTTPAPI_LoggingPath,
                                                                                                           ClientsLoggingContext ?? DefaultLoggingContext,
                                                                                                           ClientsLogfileCreator,
                                                                                                           DNSClient

                                                                                                       );

                                                                                       if (cpoClient is null)
                                                                                           return new AuthorizationInfo(
                                                                                                      Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                      Token:        null, //ToDo: Token should be optional within AuthorizationInfo!
                                                                                                      Info:         new DisplayText(Languages.en, $"Could not get/create a CPO client for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                                                                                                      RemoteParty:  remoteParty
                                                                                                  );


                                                                                       var cpoClientLogger = new CPO.HTTP.CPOClient.Logger(
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
                                                                                                        authorizationInfo.Data.Token,
                                                                                                        authorizationInfo.Data.Location,
                                                                                                        null, // AuthReference
                                                                                                        authorizationInfo.Data.Info,
                                                                                                        remoteParty,
                                                                                                        authorizationInfo.Data.Runtime
                                                                                                    )

                                                                                                  : new AuthorizationInfo(
                                                                                                        Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                        Token:        null, //ToDo: Token should be optional within AuthorizationInfo!
                                                                                                        Info:         new DisplayText(Languages.en, authorizationInfo.StatusMessage ?? $"No valid response from '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                                                                                                        RemoteParty:  remoteParty
                                                                                                    );

                                                                                   },

                                                            VerifyResult:    result  => result.Allowed == AllowedType.ALLOWED,

                                                            Timeout:         RequestTimeout ?? TimeSpan.FromSeconds(10),

                                                            OnException:     null,

                                                            DefaultResult:   runtime  => new AuthorizationInfo(
                                                                                             Allowed:   AllowedType.NOT_ALLOWED,
                                                                                             Token:     null, //ToDo: Token should be optional within AuthorizationInfo!
                                                                                             Location:  null,
                                                                                             Info:      new DisplayText(Languages.en, "No authorization service returned a positiv result!"),
                                                                                             Runtime:   runtime
                                                                                         ));


            DateTime               endtime;
            TimeSpan               runtime;
            WWCP.AuthStartResult?  authStartResult   = null;


            if (authorizationInfo is null)
                authStartResult = WWCP.AuthStartResult.CommunicationTimeout(Id, this, SessionId);

            else if (authorizationInfo.Allowed == AllowedType.ALLOWED)
                authStartResult = WWCP.AuthStartResult.Authorized(
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
                           ProviderId:                WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

            else if (authorizationInfo.Allowed == AllowedType.BLOCKED)
                authStartResult = WWCP.AuthStartResult.Blocked(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

            else if (authorizationInfo.Allowed == AllowedType.EXPIRED)
                authStartResult = WWCP.AuthStartResult.Expired(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

            else if (authorizationInfo.Allowed == AllowedType.NO_CREDIT)
                authStartResult = WWCP.AuthStartResult.NoCredit(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

            else if (authorizationInfo.Allowed == AllowedType.NOT_ALLOWED)
                authStartResult = WWCP.AuthStartResult.NotAuthorized(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                null,
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );


            authStartResult ??= WWCP.AuthStartResult.Error(Id, this, SessionId);


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
        public Task<WWCP.AuthStopResult>

            AuthorizeStop(WWCP.ChargingSession_Id           SessionId,
                          WWCP.LocalAuthentication          LocalAuthentication,
                          WWCP.ChargingLocation?            ChargingLocation      = null,
                          WWCP.ChargingSession_Id?          CPOPartnerSessionId   = null,
                          WWCP.ChargingStationOperator_Id?  OperatorId            = null,

                          DateTime?                         Timestamp             = null,
                          CancellationToken                 CancellationToken     = default,
                          EventTracking_Id?                 EventTrackingId       = null,
                          TimeSpan?                         RequestTimeout        = null)

        {

            #region Initial checks

            Timestamp       ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            EventTrackingId ??= EventTracking_Id.New;
            RequestTimeout  ??= this.RequestTimeout;

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
                authStopResult = WWCP.AuthStopResult.AdminDown(
                                     AuthorizatorId:           Id,
                                     ISendAuthorizeStartStop:  this,
                                     SessionId:                SessionId,
                                     Description:              I18NString.Create(Languages.en, "Authentication is disabled!"),
                                     Runtime:                  TimeSpan.Zero
                                 );

            authStopResult ??= WWCP.AuthStopResult.NotAuthorized(Id, this);


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
        async Task<WWCP.SendCDRsResult>

            WWCP.ISendChargeDetailRecords.SendChargeDetailRecords(IEnumerable<WWCP.ChargeDetailRecord>  ChargeDetailRecords,
                                                                  WWCP.TransmissionTypes                TransmissionType,

                                                                  DateTime?                             Timestamp,
                                                                  CancellationToken                     CancellationToken,
                                                                  EventTracking_Id?                     EventTrackingId,
                                                                  TimeSpan?                             RequestTimeout)

        {

            #region Initial checks

            Timestamp       ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            EventTrackingId ??= EventTracking_Id.New;
            RequestTimeout  ??= this.RequestTimeout;

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

            DateTime              endtime;
            TimeSpan              runtime;
            WWCP.SendCDRsResult?  sendCDRsResult   = null;

            if (DisableSendChargeDetailRecords)
            {

                endtime         = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                runtime         = endtime - startTime;
                sendCDRsResult  = WWCP.SendCDRsResult.AdminDown(
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
                sendCDRsResult  = WWCP.SendCDRsResult.NoOperation(
                                      org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                                      Id,
                                      this,
                                      ChargeDetailRecords,
                                      Runtime: runtime
                                  );

            }

            #endregion


            return WWCP.SendCDRsResult.NoOperation(org.GraphDefined.Vanaheimr.Illias.Timestamp.Now, Id, this, ChargeDetailRecords);

        }

        #endregion


        #region Skip/Flush

        protected override Boolean SkipFlushEVSEDataAndStatusQueues()
        {
            return true;
        }

        protected override async Task FlushEVSEDataAndStatusQueues()
        {
            
        }

        protected override Boolean SkipFlushEVSEFastStatusQueues()
        {
            return true;
        }

        protected override async Task FlushEVSEFastStatusQueues()
        {
            
        }

        protected override Boolean SkipFlushChargeDetailRecordsQueues()
        {
            return true;
        }

        protected override async Task FlushChargeDetailRecordsQueues(IEnumerable<CDR> ChargeDetailsRecords)
        {
            
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
        public override Int32 CompareTo(Object? Object)

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
