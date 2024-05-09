/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// Receive charging stations downstream from an OCPI partner...
    /// </summary>
    public class OCPIEMPAdapter : AWWCPEMPAdapter,
                                  IRemoteEMobilityProvider,
                                  IEMPRoamingProvider,
                                  IEquatable <OCPIEMPAdapter>,
                                  IComparable<OCPIEMPAdapter>,
                                  IComparable

    {

        #region Data

        //public  const           String                                                           Default_LoggingPath                     = "default";

        /// <summary>
        /// The default logging context.
        /// </summary>
        public  const       String         DefaultLoggingContext        = $"OCPI{Version.String}_EMPAdapter";

        public  const       String         DefaultHTTPAPI_LoggingPath   = "default";

        public  const       String         DefaultHTTPAPI_LogfileName   = $"OCPI{Version.String}_EMPAdapter.log";

        #endregion

        #region Properties

        public EVSEId_2_WWCPEVSEId_Delegate?                CustomEVSEIdConverter                { get; }
        public EVSE_2_WWCPEVSE_Delegate?                    CustomEVSEConverter                  { get; }
        public StatusType_2_WWCPEVSEStatusUpdate_Delegate?  CustomEVSEStatusUpdateConverter      { get; }
        public CDR_2_WWCPChargeDetailRecord_Delegate?       CustomChargeDetailRecordConverter    { get; }



        public HTTP.CommonAPI                               CommonAPI                            { get; }

        public HTTP.EMSPAPI                                 EMSPAPI                              { get; }


        #region Logging

        public Boolean?                                   IsDevelopment            { get; }
        public IEnumerable<String>?                       DevelopmentServers       { get; }
        public Boolean?                                   DisableLogging           { get; set; }
        public String                                     LoggingPath              { get; }
        public String?                                    LoggingContext           { get; }
        public String?                                    LogfileName              { get; }
        public LogfileCreatorDelegate?                    LogfileCreator           { get; }

        public String                                     ClientsLoggingPath       { get; }
        public String?                                    ClientsLoggingContext    { get; }
        public OCPILogfileCreatorDelegate?                ClientsLogfileCreator    { get; }

        #endregion

        public DNSClient?  DNSClient    { get; }

        #endregion

        #region Events

        event OnGetCDRsRequestDelegate IEMPRoamingProvider.OnGetChargeDetailRecordsRequest
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

        event OnGetCDRsResponseDelegate IEMPRoamingProvider.OnGetChargeDetailRecordsResponse
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

        event OnAuthorizeStartRequestDelegate IEMPRoamingProvider.OnAuthorizeStartRequest
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

        event OnAuthorizeStartResponseDelegate IEMPRoamingProvider.OnAuthorizeStartResponse
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

        event OnAuthorizeStopRequestDelegate IEMPRoamingProvider.OnAuthorizeStopRequest
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

        event OnAuthorizeStopResponseDelegate IEMPRoamingProvider.OnAuthorizeStopResponse
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

        event OnSendCDRsRequestDelegate IEMPRoamingProvider.OnChargeDetailRecordRequest
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

        event OnSendCDRsResponseDelegate IEMPRoamingProvider.OnChargeDetailRecordResponse
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

        public OCPIEMPAdapter(EMPRoamingProvider_Id                             Id,
                              I18NString                                        Name,
                              I18NString                                        Description,
                              IRoamingNetwork                                   RoamingNetwork,

                              HTTP.CommonAPI                                    CommonAPI,
                              OCPI.CountryCode                                  DefaultCountryCode,
                              OCPI.Party_Id                                     DefaultPartyId,

                              EVSEId_2_WWCPEVSEId_Delegate?                     CustomEVSEIdConverter               = null,
                              EVSE_2_WWCPEVSE_Delegate?                         CustomEVSEConverter                 = null,
                              StatusType_2_WWCPEVSEStatusUpdate_Delegate?       CustomEVSEStatusUpdateConverter     = null,
                              CDR_2_WWCPChargeDetailRecord_Delegate?            CustomChargeDetailRecordConverter   = null,

                              IncludeChargingStationOperatorIdDelegate?         IncludeChargingStationOperatorIds   = null,
                              IncludeChargingStationOperatorDelegate?           IncludeChargingStationOperators     = null,
                              IncludeChargingPoolIdDelegate?                    IncludeChargingPoolIds              = null,
                              IncludeChargingPoolDelegate?                      IncludeChargingPools                = null,
                              IncludeChargingStationIdDelegate?                 IncludeChargingStationIds           = null,
                              IncludeChargingStationDelegate?                   IncludeChargingStations             = null,
                              IncludeEVSEIdDelegate?                            IncludeEVSEIds                      = null,
                              IncludeEVSEDelegate?                              IncludeEVSEs                        = null,
                              ChargeDetailRecordFilterDelegate?                 ChargeDetailRecordFilter            = null,

                              Boolean                                           DisablePushData                     = false,
                              Boolean                                           DisablePushAdminStatus              = false,
                              Boolean                                           DisablePushStatus                   = false,
                              Boolean                                           DisablePushEnergyStatus             = false,
                              Boolean                                           DisableAuthentication               = false,
                              Boolean                                           DisableSendChargeDetailRecords      = false,

                              Timestamped<EMPRoamingProviderAdminStatusTypes>?  InitialAdminStatus                  = null,
                              Timestamped<EMPRoamingProviderStatusTypes>?       InitialStatus                       = null,
                              UInt16                                            MaxAdminStatusScheduleSize          = DefaultMaxAdminStatusScheduleSize,
                              UInt16                                            MaxStatusScheduleSize               = DefaultMaxStatusScheduleSize,

                              String?                                           DataSource                          = null,
                              DateTime?                                         LastChange                          = null,

                              JObject?                                          CustomData                          = null,
                              UserDefinedDictionary?                            InternalData                        = null)

            : base(Id,
                   RoamingNetwork,

                   Name,
                   Description,

                   null, // EllipticCurve
                   null, // PrivateKey
                   null, // PublicKeyCertificates

                   InitialAdminStatus,
                   InitialStatus,
                   MaxAdminStatusScheduleSize,
                   MaxStatusScheduleSize,

                   DataSource,
                   LastChange,

                   CustomData,
                   InternalData)

        {

            this.CommonAPI                          = CommonAPI;
            this.EMSPAPI                            = new HTTP.EMSPAPI(
                                                          this.CommonAPI,
                                                          DefaultCountryCode,
                                                          DefaultPartyId,
                                                          URLPathPrefix: CommonAPI.URLPathPrefix + Version.String + "emsp"
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


        public OCPIEMPAdapter(EMPRoamingProvider_Id                             Id,
                              I18NString                                        Name,
                              I18NString                                        Description,
                              IRoamingNetwork                                   RoamingNetwork,

                              HTTP.EMSPAPI                                      EMSPAPI,

                              EVSEId_2_WWCPEVSEId_Delegate?                     CustomEVSEIdConverter               = null,
                              EVSE_2_WWCPEVSE_Delegate?                         CustomEVSEConverter                 = null,
                              StatusType_2_WWCPEVSEStatusUpdate_Delegate?       CustomEVSEStatusUpdateConverter     = null,
                              CDR_2_WWCPChargeDetailRecord_Delegate?            CustomChargeDetailRecordConverter   = null,

                              IncludeChargingStationOperatorIdDelegate?         IncludeChargingStationOperatorIds   = null,
                              IncludeChargingStationOperatorDelegate?           IncludeChargingStationOperators     = null,
                              IncludeChargingPoolIdDelegate?                    IncludeChargingPoolIds              = null,
                              IncludeChargingPoolDelegate?                      IncludeChargingPools                = null,
                              IncludeChargingStationIdDelegate?                 IncludeChargingStationIds           = null,
                              IncludeChargingStationDelegate?                   IncludeChargingStations             = null,
                              IncludeEVSEIdDelegate?                            IncludeEVSEIds                      = null,
                              IncludeEVSEDelegate?                              IncludeEVSEs                        = null,
                              ChargeDetailRecordFilterDelegate?                 ChargeDetailRecordFilter            = null,

                              Boolean                                           DisablePushData                     = false,
                              Boolean                                           DisablePushAdminStatus              = false,
                              Boolean                                           DisablePushStatus                   = false,
                              Boolean                                           DisablePushEnergyStatus             = false,
                              Boolean                                           DisableAuthentication               = false,
                              Boolean                                           DisableSendChargeDetailRecords      = false,

                              Timestamped<EMPRoamingProviderAdminStatusTypes>?  InitialAdminStatus                  = null,
                              Timestamped<EMPRoamingProviderStatusTypes>?       InitialStatus                       = null,
                              UInt16                                            MaxAdminStatusScheduleSize          = DefaultMaxAdminStatusScheduleSize,
                              UInt16                                            MaxStatusScheduleSize               = DefaultMaxStatusScheduleSize,

                              String?                                           DataSource                          = null,
                              DateTime?                                         LastChange                          = null,

                              JObject?                                          CustomData                          = null,
                              UserDefinedDictionary?                            InternalData                        = null)

            : base(Id,
                   RoamingNetwork,

                   Name,
                   Description,

                   null, // EllipticCurve
                   null, // PrivateKey
                   null, // PublicKeyCertificates

                   InitialAdminStatus,
                   InitialStatus,
                   MaxAdminStatusScheduleSize,
                   MaxStatusScheduleSize,

                   DataSource,
                   LastChange,

                   CustomData,
                   InternalData)

        {

            this.CommonAPI                          = EMSPAPI.CommonAPI;
            this.EMSPAPI                            = EMSPAPI;

            this.EMSPAPI.OnRFIDAuthToken           += async (countryCode, partyId, tokenId, locationReference) => {

                                                           var response = await AuthorizeStart(LocalAuthentication:   LocalAuthentication.FromAuthToken(
                                                                                                                          AuthenticationToken.Parse(tokenId.ToString())
                                                                                                                      ),
                                                                                               ChargingLocation:      null,
                                                                                               ChargingProduct:       null,
                                                                                               SessionId:             null,
                                                                                               CPOPartnerSessionId:   null,
                                                                                               OperatorId:            null,
                                                                                               Timestamp:             null,
                                                                                               CancellationToken:     default,
                                                                                               EventTrackingId:       null,
                                                                                               RequestTimeout:        null);

                                                           if (response.Result == AuthStartResultTypes.Authorized)
                                                               return new AuthorizationInfo(
                                                                          Allowed:       AllowedType.ALLOWED,
                                                                          Location:      null,
                                                                          Info:          null,
                                                                          RemoteParty:   null,
                                                                          EMSPId:        null,
                                                                          Runtime:       null
                                                                      );

                                                           else //if (response.Result == AuthStartResultTypes.NotAuthorized)
                                                               return new AuthorizationInfo(
                                                                          Allowed:       AllowedType.NOT_ALLOWED,
                                                                          Location:      null,
                                                                          Info:          null,
                                                                          RemoteParty:   null,
                                                                          EMSPId:        null,
                                                                          Runtime:       null
                                                                      );

                                                      };


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


        public event OnAuthorizeStartRequestDelegate  OnAuthorizeStartRequest;
        public event OnAuthorizeStartResponseDelegate OnAuthorizeStartResponse;

        public event OnAuthorizeStopRequestDelegate   OnAuthorizeStopRequest;
        public event OnAuthorizeStopResponseDelegate  OnAuthorizeStopResponse;

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

        Task<ReservationResult> IChargingReservations.Reserve(ChargingLocation                   ChargingLocation,
                                                              ChargingReservationLevel           ReservationLevel,
                                                              DateTime?                          StartTime,
                                                              TimeSpan?                          Duration,
                                                              ChargingReservation_Id?            ReservationId,
                                                              ChargingReservation_Id?            LinkedReservationId,
                                                              EMobilityProvider_Id?              ProviderId,
                                                              RemoteAuthentication?              RemoteAuthentication,
                                                              Auth_Path?                         AuthenticationPath,
                                                              ChargingProduct?                   ChargingProduct,
                                                              IEnumerable<AuthenticationToken>?  AuthTokens,
                                                              IEnumerable<EMobilityAccount_Id>?  eMAIds,
                                                              IEnumerable<UInt32>?               PINs,

                                                              DateTime?                          Timestamp,
                                                              EventTracking_Id?                  EventTrackingId,
                                                              TimeSpan?                          RequestTimeout,
                                                              CancellationToken                  CancellationToken)
        {
            return Task.FromResult(ReservationResult.NoOperation());
        }

        Task<CancelReservationResult> IChargingReservations.CancelReservation(ChargingReservation_Id                 ReservationId,
                                                                              ChargingReservationCancellationReason  Reason,
                                                                              DateTime?                              Timestamp,
                                                                              EventTracking_Id?                      EventTrackingId,
                                                                              TimeSpan?                              RequestTimeout,
                                                                              CancellationToken                      CancellationToken)
        {
            return Task.FromResult(CancelReservationResult.NoOperation(ReservationId));
        }


        public Boolean TryGetChargingReservationById(ChargingReservation_Id ReservationId, out ChargingReservation? ChargingReservation)
        {
            throw new NotImplementedException();
        }

        public Boolean TryGetChargingReservationsById(ChargingReservation_Id ReservationId, out ChargingReservationCollection? ChargingReservations)
        {
            throw new NotImplementedException();
        }

        public ChargingReservation? GetChargingReservationById(ChargingReservation_Id ReservationId)
        {
            throw new NotImplementedException();
        }

        public ChargingReservationCollection? GetChargingReservationsById(ChargingReservation_Id ReservationId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Remote Start/-Stop

        async Task<RemoteStartResult> IRemoteStartStop.RemoteStart(ChargingLocation         ChargingLocation,
                                                                   ChargingProduct?         ChargingProduct,
                                                                   ChargingReservation_Id?  ReservationId,
                                                                   ChargingSession_Id?      SessionId,
                                                                   EMobilityProvider_Id?    ProviderId,
                                                                   RemoteAuthentication?    RemoteAuthentication,
                                                                   Auth_Path?               AuthenticationPath,

                                                                   DateTime?                Timestamp,
                                                                   EventTracking_Id?        EventTrackingId,
                                                                   TimeSpan?                RequestTimeout,
                                                                   CancellationToken        CancellationToken)
        {

            var eventTrackingId  = EventTrackingId ?? EventTracking_Id.New;

            var operatorId       = ChargingLocation.EVSEId?.           OperatorId ??
                                   ChargingLocation.ChargingStationId?.OperatorId ??
                                   ChargingLocation.ChargingPoolId?.   OperatorId ??
                                   ChargingLocation.ChargingStationOperatorId;

            if (operatorId.HasValue)
            {

                var cpoId             = CPO_Id.Parse(operatorId.Value.ToString());
                var remoteParty       = CommonAPI.GetRemoteParty(RemoteParty_Id.From(cpoId));
                var remoteAccessInfo  = remoteParty?.RemoteAccessInfos.FirstOrDefault(remoteAccessInfo => remoteAccessInfo.Status == OCPI.RemoteAccessStatus.ONLINE);

                if (remoteAccessInfo is null)
                    return RemoteStartResult.Error(System_Id.Local);

                var emspClient = new EMSP.HTTP.EMSPClient(

                                     CommonAPI,
                                     remoteParty,
                                     null, // VirtualHostname
                                     null, // Description
                                     null, // HTTPLogger

                                     DisableLogging,
                                     ClientsLoggingPath    ?? DefaultHTTPAPI_LoggingPath,
                                     ClientsLoggingContext ?? DefaultLoggingContext,
                                     ClientsLogfileCreator,
                                     DNSClient

                                 );

                var emspClientLogger = new EMSP.HTTP.EMSPClient.Logger(
                                           emspClient,
                                           ClientsLoggingPath    ?? DefaultHTTPAPI_LoggingPath,
                                           ClientsLoggingContext ?? DefaultLoggingContext,
                                           ClientsLogfileCreator
                                       );


                var evseId    = ChargingLocation.EVSEId.Value.ToOCPI_EVSEId()!;
                var location  = CommonAPI.GetLocations(location => location.EVSEs.FirstOrDefault(evse => evse.EVSEId.Value == evseId) is not null).FirstOrDefault();
                var evse      = location.EVSEs.FirstOrDefault(evse => evse.EVSEId.Value == evseId);

                var response  = await emspClient.StartSession(Token:               new Token(
                                                                                       CountryCode.Parse(operatorId.Value.CountryCode.Alpha2Code),
                                                                                       Party_Id.   Parse(operatorId.Value.Suffix),
                                                                                       Token_Id.   Parse(RemoteAuthentication.RemoteIdentification.ToString()),
                                                                                       TokenType.OTHER,
                                                                                       Auth_Id.    Parse(RemoteAuthentication.RemoteIdentification.ToString()),
                                                                                       "Issuer",
                                                                                       true,

                                                                                       WhitelistType:   WhitelistTypes.NEVER,
                                                                                       VisualNumber:    RemoteAuthentication.RemoteIdentification.ToString(),
                                                                                       UILanguage:      null,

                                                                                       Created:         null,
                                                                                       LastUpdated:     null
                                                                                   ),
                                                              LocationId:          location.Id,
                                                              EVSEUId:             evse.UId,

                                                              CommandId:           null,
                                                              RequestId:           null,
                                                              CorrelationId:       null,
                                                              VersionId:           null,

                                                              CancellationToken:   CancellationToken,
                                                              EventTrackingId:     EventTrackingId,
                                                              RequestTimeout:      RequestTimeout);

                if (response.StatusCode == 1000)
                    // The OCPI response is just a "command accepted" information!
                    return RemoteStartResult.AsyncOperation(new ChargingSession(
                                                                ChargingSession_Id.NewRandom(),
                                                                RoamingNetwork,
                                                                eventTrackingId
                                                            ),
                                                            System_Id.Local);

                return RemoteStartResult.Error(System_Id.Local);

            }

            return RemoteStartResult.NoOperation(System_Id.Local);

        }

        Task<RemoteStopResult> IRemoteStartStop.RemoteStop(ChargingSession_Id     SessionId,
                                                           ReservationHandling?   ReservationHandling,
                                                           EMobilityProvider_Id?  ProviderId,
                                                           RemoteAuthentication?  RemoteAuthentication,
                                                           Auth_Path?             AuthenticationPath,

                                                           DateTime?              Timestamp,
                                                           EventTracking_Id?      EventTrackingId,
                                                           TimeSpan?              RequestTimeout,
                                                           CancellationToken      CancellationToken)
        {

            return Task.FromResult(RemoteStopResult.NoOperation(SessionId, System_Id.Local));

        }

        #endregion

        #region Charging Sessions

        IEnumerable<ChargingSession> IChargingSessions.ChargingSessions
            => Array.Empty<ChargingSession>();

        public IEnumerable<KeyValuePair<LocalAuthentication, TokenAuthorizationResultType>> AllTokens => throw new NotImplementedException();

        public IEnumerable<KeyValuePair<LocalAuthentication, TokenAuthorizationResultType>> AuthorizedTokens => throw new NotImplementedException();

        public IEnumerable<KeyValuePair<LocalAuthentication, TokenAuthorizationResultType>> NotAuthorizedTokens => throw new NotImplementedException();

        public IEnumerable<KeyValuePair<LocalAuthentication, TokenAuthorizationResultType>> BlockedTokens => throw new NotImplementedException();

        public IId AuthId => throw new NotImplementedException();


        /// <summary>
        /// Whether the given charging session identification is known within the roaming network.
        /// </summary>
        /// <param name="ChargingSessionId">The charging session identification.</param>
        public Boolean ContainsChargingSessionId(ChargingSession_Id ChargingSessionId)
            => false;

        /// <summary>
        /// Return the charging session specified by the given charging session identification.
        /// </summary>
        /// <param name="ChargingSessionId">The charging session identification.</param>
        /// <param name="ChargingSession">The charging session.</param>
        Boolean IChargingSessions.TryGetChargingSessionById(ChargingSession_Id    ChargingSessionId,
                                                            out ChargingSession?  ChargingSession)
        {
            ChargingSession = null;
            return false;
        }

        #endregion

        #region Charge Detail Records

        Task<IEnumerable<ChargeDetailRecord>> IEMPRoamingProvider.GetChargeDetailRecords(DateTime From, DateTime? To, EMobilityProvider_Id? ProviderId, DateTime? Timestamp, EventTracking_Id? EventTrackingId, TimeSpan? RequestTimeout, CancellationToken CancellationToken)
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
        /// Compares two OCPI EMP adapters.
        /// </summary>
        /// <param name="Object">An OCPI EMP adapter to compare with.</param>
        public override Int32 CompareTo(Object? Object)

            => Object is OCPIEMPAdapter ocpiEMPAdapter
                   ? CompareTo(ocpiEMPAdapter)
                   : throw new ArgumentException("The given object is not an OCPI EMP adapter!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(OCPIEMPAdapter)

        /// <summary>
        /// Compares two OCPI EMP adapters.
        /// </summary>
        /// <param name="OCPIEMPAdapter">An OCPI EMP adapter to compare with.</param>
        public Int32 CompareTo(OCPIEMPAdapter? OCPIEMPAdapter)
        {

            if (OCPIEMPAdapter is null)
                throw new ArgumentNullException(nameof(OCPIEMPAdapter),
                                                "The given OCPI EMP adapter must not be null!");

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

        #region (override) GetHashCode()

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



        #region AuthorizeStart(LocalAuthentication, ...)

        public async Task<AuthStartResult> AuthorizeStart(LocalAuthentication          LocalAuthentication,
                                                          ChargingLocation?            ChargingLocation      = null,
                                                          ChargingProduct?             ChargingProduct       = null,
                                                          ChargingSession_Id?          SessionId             = null,
                                                          ChargingSession_Id?          CPOPartnerSessionId   = null,
                                                          ChargingStationOperator_Id?  OperatorId            = null,

                                                          DateTime?                    Timestamp             = null,
                                                          EventTracking_Id?            EventTrackingId       = null,
                                                          TimeSpan?                    RequestTimeout        = null,
                                                          CancellationToken            CancellationToken     = default)
        {

            return await RoamingNetwork.AuthorizeStart(LocalAuthentication,
                                                       ChargingLocation,
                                                       ChargingProduct,
                                                       SessionId,
                                                       CPOPartnerSessionId,
                                                       OperatorId,

                                                       Timestamp,
                                                       EventTrackingId,
                                                       RequestTimeout,
                                                       CancellationToken);

        }

        #endregion

        #region AuthorizeStop(SessionId, LocalAuthentication, ...)

        public async Task<AuthStopResult> AuthorizeStop(ChargingSession_Id           SessionId,
                                                        LocalAuthentication          LocalAuthentication,
                                                        ChargingLocation?            ChargingLocation      = null,
                                                        ChargingSession_Id?          CPOPartnerSessionId   = null,
                                                        ChargingStationOperator_Id?  OperatorId            = null,

                                                        DateTime?                    Timestamp             = null,
                                                        EventTracking_Id?            EventTrackingId       = null,
                                                        TimeSpan?                    RequestTimeout        = null,
                                                        CancellationToken            CancellationToken     = default)
        {

            return await RoamingNetwork.AuthorizeStop(SessionId,
                                                      LocalAuthentication,
                                                      ChargingLocation,
                                                      CPOPartnerSessionId,
                                                      OperatorId,

                                                      Timestamp,
                                                      EventTrackingId,
                                                      RequestTimeout,
                                                      CancellationToken);

        }

        #endregion

        #region ReceiveChargeDetailRecords(ChargeDetailRecords, ...)

        public async Task<SendCDRsResult> ReceiveChargeDetailRecords(IEnumerable<ChargeDetailRecord>  ChargeDetailRecords,

                                                                     DateTime?                        Timestamp             = null,
                                                                     EventTracking_Id?                EventTrackingId       = null,
                                                                     TimeSpan?                        RequestTimeout        = null,
                                                                     CancellationToken                CancellationToken     = default)
        {

            return await RoamingNetwork.ReceiveChargeDetailRecords(ChargeDetailRecords,

                                                                   Timestamp,
                                                                   EventTrackingId,
                                                                   RequestTimeout,
                                                                   CancellationToken);

        }

        #endregion


    }

}
