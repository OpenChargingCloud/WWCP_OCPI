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

using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    public delegate IEnumerable<Tariff>     GetTariffs_Delegate  (WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId,
                                                                  WWCP.ChargingPool_Id?             ChargingPoolId,
                                                                  WWCP.ChargingStation_Id?          ChargingStationId,
                                                                  WWCP.EVSE_Id?                     EVSEId,
                                                                  WWCP.ChargingConnector_Id?        ChargingConnectorId,
                                                                  WWCP.EMobilityProvider_Id?        EMobilityProviderId);

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
        public  const       String         DefaultLoggingContext        = "OCPIv2.2_CSOAdapter";

        public  const       String         DefaultHTTPAPI_LoggingPath   = "default";

        public  const       String         DefaultHTTPAPI_LogfileName   = "OCPIv2.2_CSOAdapter.log";


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
        //public event WWCP.OnAuthorizeStartRequestDelegate?       OnAuthorizeStartRequest;
        //public event WWCP.OnAuthorizeStartResponseDelegate?      OnAuthorizeStartResponse;

        //public event WWCP.OnAuthorizeStopRequestDelegate?        OnAuthorizeStopRequest;
        //public event WWCP.OnAuthorizeStopResponseDelegate?       OnAuthorizeStopResponse;

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
        //public event WWCP.OnSendCDRsResponseDelegate?            OnSendCDRsResponse;


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

        public OCPICSOAdapter(WWCP.EMPRoamingProvider_Id                      Id,
                              I18NString                                      Name,
                              I18NString                                      Description,
                              WWCP.IRoamingNetwork                            RoamingNetwork,

                              HTTP.CommonAPI                                  CommonAPI,
                              OCPI.CountryCode                                DefaultCountryCode,
                              OCPI.Party_Id                                   DefaultPartyId,

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
            this.CPOAPI                             = new HTTP.CPOAPI(

                                                          this.CommonAPI,
                                                          DefaultCountryCode,
                                                          DefaultPartyId,
                                                          null, // AllowDowngrades

                                                          null, // HTTPHostname
                                                          null, // ExternalDNSName
                                                          null, // HTTPServiceName
                                                          null, // BasePath

                                                          this.CommonAPI.URLPathPrefix + Version.String + "cpo",
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

            Link();

        }


        public OCPICSOAdapter(WWCP.EMPRoamingProvider_Id                      Id,
                              I18NString                                      Name,
                              I18NString                                      Description,
                              WWCP.IRoamingNetwork                            RoamingNetwork,

                              HTTP.CPOAPI                                     CPOAPI,

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

            this.CPOAPI                             = CPOAPI;
            this.CommonAPI                          = CPOAPI.CommonAPI;

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

            Link();

        }

        #endregion


        private void Link()
        {

            #region OnStartSessionCommand

            this.CPOAPI.OnStartSessionCommand += async (emspId, startSessionCommand) => {

                if (!CommonAPI.TryGetLocation(startSessionCommand.LocationId, out var location) || location is null)
                    return new CommandResponse(startSessionCommand,
                                               CommandResponseTypes.REJECTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StartSessionCommand rejected!")
                                               });

                if (!startSessionCommand.EVSEUId.HasValue)
                    return new CommandResponse(startSessionCommand,
                                               CommandResponseTypes.REJECTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StartSessionCommand rejected!")
                                               });

                if (!location.TryGetEVSE(startSessionCommand.EVSEUId.Value, out var evse) || evse is null)
                    return new CommandResponse(startSessionCommand,
                                               CommandResponseTypes.REJECTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StartSessionCommand rejected!")
                                               });

                if (!evse.EVSEId.HasValue)
                    return new CommandResponse(startSessionCommand,
                                               CommandResponseTypes.REJECTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StartSessionCommand rejected!")
                                               });

                var wwcpEVSEId = evse.EVSEId.Value.ToWWCP();

                if (!wwcpEVSEId.HasValue)
                    return new CommandResponse(startSessionCommand,
                                               CommandResponseTypes.REJECTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StartSessionCommand rejected!")
                                               });


                var result = await RoamingNetwork.RemoteStart(ChargingLocation:       WWCP.ChargingLocation.FromEVSEId(wwcpEVSEId.Value),
                                                              ChargingProduct:        null,
                                                              ReservationId:          null,
                                                              SessionId:              WWCP.ChargingSession_Id.NewRandom, // OCPI does not have its own charging session identification!
                                                              ProviderId:             emspId.ToWWCP(),
                                                              RemoteAuthentication:   WWCP.RemoteAuthentication.FromRemoteIdentification(WWCP.EMobilityAccount_Id.Parse(startSessionCommand.Token.Id.ToString())));


                if (result.Result == WWCP.RemoteStartResultTypes.Success)
                {
                    return new CommandResponse(startSessionCommand,
                                               CommandResponseTypes.ACCEPTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StartSessionCommand accepted!")
                                               });
                }

                else
                    return new CommandResponse(startSessionCommand,
                                               CommandResponseTypes.REJECTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StartSessionCommand rejected!")
                                               });

            };

            #endregion

            #region OnStopSessionCommand

            this.CPOAPI.OnStopSessionCommand += async (emspId, stopSessionCommand) => {

                var result = await RoamingNetwork.RemoteStop(SessionId:             WWCP.ChargingSession_Id.Parse(stopSessionCommand.SessionId.ToString()),
                                                             ReservationHandling:   WWCP.ReservationHandling.Close,
                                                             ProviderId:            emspId.ToWWCP());


                if (result.Result == WWCP.RemoteStopResultTypes.Success)
                {
                    return new CommandResponse(stopSessionCommand,
                                               CommandResponseTypes.ACCEPTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StopSessionCommand accepted!")
                                               });
                }

                else
                    return new CommandResponse(stopSessionCommand,
                                               CommandResponseTypes.REJECTED,
                                               TimeSpan.FromMinutes(1),
                                               new[] {
                                                   DisplayText.Create(Languages.en, "StopSessionCommand rejected!")
                                               });

            };

            #endregion


        }



        #region AddRemoteParty(...)

        public Task<Boolean> AddRemoteParty(CountryCode               CountryCode,
                                            Party_Id                  PartyId,
                                            Roles                     Role,
                                            BusinessDetails           BusinessDetails,

                                            AccessToken               AccessToken,

                                            AccessToken               RemoteAccessToken,
                                            URL                       RemoteVersionsURL,
                                            IEnumerable<Version_Id>?  RemoteVersionIds            = null,
                                            Version_Id?               SelectedVersionId           = null,

                                            DateTime?                 LocalAccessNotBefore        = null,
                                            DateTime?                 LocalAccessNotAfter         = null,

                                            Boolean?                  AccessTokenBase64Encoding   = null,
                                            Boolean?                  AllowDowngrades             = false,
                                            AccessStatus              AccessStatus                = AccessStatus.      ALLOWED,
                                            RemoteAccessStatus?       RemoteStatus                = RemoteAccessStatus.ONLINE,
                                            PartyStatus               PartyStatus                 = PartyStatus.       ENABLED)

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

        public Task<Boolean> AddRemoteParty(CountryCode      CountryCode,
                                            Party_Id         PartyId,
                                            Roles            Role,
                                            BusinessDetails  BusinessDetails,

                                            AccessToken      AccessToken,
                                            DateTime?        LocalAccessNotBefore        = null,
                                            DateTime?        LocalAccessNotAfter         = null,
                                            Boolean?         AccessTokenBase64Encoding   = null,
                                            Boolean?         AllowDowngrades             = false,
                                            AccessStatus     AccessStatus                = AccessStatus.ALLOWED,

                                            PartyStatus      PartyStatus                 = PartyStatus. ENABLED)

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


        #region (Set/Add/Update/Delete) Charging pool(s)...

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
        public override async Task<WWCP.PushChargingPoolDataResult>

            SetStaticData(WWCP.IChargingPool      ChargingPool,
                          WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                          DateTime?               Timestamp           = null,
                          CancellationToken       CancellationToken   = default,
                          EventTracking_Id?       EventTrackingId     = null,
                          TimeSpan?               RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            EventTrackingId ??= EventTracking_Id.New;

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
                                                           evseId      => true,
                                                           connectorId => true,
                                                           null,
                                                           out warnings);

                        if (location is not null)
                            await CommonAPI.AddLocation(location,
                                                        false,
                                                        EventTrackingId);


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
        public override async Task<WWCP.PushChargingPoolDataResult>

            AddStaticData(WWCP.IChargingPool      ChargingPool,
                          WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                          DateTime?               Timestamp           = null,
                          CancellationToken       CancellationToken   = default,
                          EventTracking_Id?       EventTrackingId     = null,
                          TimeSpan?               RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            EventTrackingId ??= EventTracking_Id.New;

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
                                                           evseId      => true,
                                                           connectorId => true,
                                                           null,
                                                           out warnings);

                        if (location is not null)
                        {

                            var result = await CommonAPI.AddLocation(location,
                                                                     false,
                                                                     EventTrackingId);


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
        public override async Task<WWCP.PushChargingPoolDataResult>

            UpdateStaticData(WWCP.IChargingPool      ChargingPool,
                             String                  PropertyName,
                             Object?                 NewValue,
                             Object?                 OldValue            = null,
                             Context?                DataSource          = null,
                             WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?               Timestamp           = null,
                             CancellationToken       CancellationToken   = default,
                             EventTracking_Id?       EventTrackingId     = null,
                             TimeSpan?               RequestTimeout      = null)
        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            EventTrackingId ??= EventTracking_Id.New;

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
                                                           evseId      => true,
                                                           connectorId => true,
                                                           null,
                                                           out warnings);

                        if (location is not null)
                        {

                            var result = await CommonAPI.UpdateLocation(location,
                                                                        null,
                                                                        false,
                                                                        EventTrackingId);

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
        public override async Task<WWCP.PushChargingPoolDataResult>

            DeleteStaticData(WWCP.IChargingPool      ChargingPool,
                             WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?               Timestamp           = null,
                             CancellationToken       CancellationToken   = default,
                             EventTracking_Id?       EventTrackingId     = null,
                             TimeSpan?               RequestTimeout      = null)

        {

            await Task.Delay(100);

            return WWCP.PushChargingPoolDataResult.NoOperation(
                       AuthId:                 Id,
                       SendPOIData:            this,
                       RejectedChargingPools:  Array.Empty<WWCP.IChargingPool>(),
                       Description:            null,
                       Warnings:               null,
                       Runtime:                null
                   );

        }

        #endregion


        #region SetStaticData   (ChargingPools, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.PushChargingPoolDataResult>

            SetStaticData(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                          WWCP.TransmissionTypes           TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                          DateTime?                        Timestamp           = null,
                          CancellationToken                CancellationToken   = default,
                          EventTracking_Id?                EventTrackingId     = null,
                          TimeSpan?                        RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    if (!ChargingPools.Any())
                        return WWCP.PushChargingPoolDataResult.NoOperation(
                                    Id,
                                    this,
                                    ChargingPools
                                );

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    foreach (var chargingPool in ChargingPools)
                    {
                        if (IncludeChargingPools is null ||
                           (IncludeChargingPools is not null && IncludeChargingPools(chargingPool)))
                        {


                            var location = chargingPool.ToOCPI(CustomEVSEUIdConverter,
                                                               CustomEVSEIdConverter,
                                                               evseId      => true,
                                                               connectorId => true,
                                                               null,
                                                               out warnings);

                            if (location is not null)
                                CommonAPI.AddLocation(location);

                        }
                    }

                    return WWCP.PushChargingPoolDataResult.Enqueued(
                                Id,
                                this,
                                ChargingPools,
                                "",
                                warnings,
                                TimeSpan.FromMilliseconds(10)
                            );

                }
                else
                    return WWCP.PushChargingPoolDataResult.LockTimeout(
                                Id,
                                this,
                                ChargingPools
                            );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region SetStaticData   (ChargingPools, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.PushChargingPoolDataResult>

            AddStaticData(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                          WWCP.TransmissionTypes           TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                          DateTime?                        Timestamp           = null,
                          CancellationToken                CancellationToken   = default,
                          EventTracking_Id?                EventTrackingId     = null,
                          TimeSpan?                        RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    if (!ChargingPools.Any())
                        return WWCP.PushChargingPoolDataResult.NoOperation(
                                    Id,
                                    this,
                                    ChargingPools
                                );

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    foreach (var chargingPool in ChargingPools)
                    {
                        if (IncludeChargingPools is null ||
                           (IncludeChargingPools is not null && IncludeChargingPools(chargingPool)))
                        {

                            var location = chargingPool.ToOCPI(CustomEVSEUIdConverter,
                                                               CustomEVSEIdConverter,
                                                               evseId      => true,
                                                               connectorId => true,
                                                               null,
                                                               out warnings);

                            if (location is not null)
                            {

                                var result = CommonAPI.AddLocation(location);


                                //ToDo: Process errors!!!


                            }
                        }
                    }

                    return WWCP.PushChargingPoolDataResult.Enqueued(
                               Id,
                               this,
                               ChargingPools,
                               "",
                               warnings,
                               TimeSpan.FromMilliseconds(10)
                           );

                }
                else
                    return WWCP.PushChargingPoolDataResult.LockTimeout(
                               Id,
                               this,
                               ChargingPools
                           );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region UpdateStaticData(ChargingPools, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.PushChargingPoolDataResult>

            UpdateStaticData(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                             WWCP.TransmissionTypes           TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?                        Timestamp           = null,
                             CancellationToken                CancellationToken   = default,
                             EventTracking_Id?                EventTrackingId     = null,
                             TimeSpan?                        RequestTimeout      = null)

        {

            await Task.Delay(100);

            return WWCP.PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   );

        }

        #endregion

        #region DeleteStaticData(ChargingPools, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.PushChargingPoolDataResult>

            DeleteStaticData(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                             WWCP.TransmissionTypes           TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?                        Timestamp           = null,
                             CancellationToken                CancellationToken   = default,
                             EventTracking_Id?                EventTrackingId     = null,
                             TimeSpan?                        RequestTimeout      = null)

        {

            await Task.Delay(100);

            return WWCP.PushChargingPoolDataResult.NoOperation(
                       Id,
                       this,
                       ChargingPools
                   );

        }

        #endregion

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
        public override async Task<WWCP.PushEVSEDataResult>

            SetStaticData(WWCP.IEVSE              EVSE,
                          WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                          DateTime?               Timestamp           = null,
                          CancellationToken       CancellationToken   = default,
                          EventTracking_Id?       EventTrackingId     = null,
                          TimeSpan?               RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            EventTrackingId ??= EventTracking_Id.New;

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

                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        connectorId => true,
                                                        null,
                                                        EVSE.Status.Timestamp > EVSE.LastChange
                                                            ? EVSE.Status.Timestamp
                                                            : EVSE.LastChange,
                                                        out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(location,
                                                                             evse2,
                                                                             null,
                                                                             false,
                                                                             EventTrackingId);
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
        public override async Task<WWCP.PushEVSEDataResult>

            AddStaticData(WWCP.IEVSE              EVSE,
                          WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                          DateTime?               Timestamp           = null,
                          CancellationToken       CancellationToken   = default,
                          EventTracking_Id?       EventTrackingId     = null,
                          TimeSpan?               RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            EventTrackingId ??= EventTracking_Id.New;

            try
            {

                if (lockTaken)
                {

                    AddResult<EVSE> result;

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

                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        connectorId => true,
                                                        null,
                                                        EVSE.Status.Timestamp > EVSE.LastChange
                                                            ? EVSE.Status.Timestamp
                                                            : EVSE.LastChange,
                                                        out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddEVSE(location,
                                                                     evse2,
                                                                     false,
                                                                     EventTrackingId);
                                else
                                    result = AddResult<EVSE>.Failed("Could not convert the given EVSE!");

                            }
                            else
                                result = AddResult<EVSE>.Failed("Unknown location identification!");

                        }
                        else
                            result = AddResult<EVSE>.Failed("The given EVSE was filtered!");

                    }
                    else
                        result = AddResult<EVSE>.Failed("Invalid location identification!");


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
        public override async Task<WWCP.PushEVSEDataResult>

            UpdateStaticData(WWCP.IEVSE              EVSE,
                             String                  PropertyName,
                             Object?                 NewValue,
                             Object?                 OldValue,
                             Context?                DataSource,
                             WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?               Timestamp           = null,
                             CancellationToken       CancellationToken   = default,
                             EventTracking_Id?       EventTrackingId     = null,
                             TimeSpan?               RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            EventTrackingId ??= EventTracking_Id.New;

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

                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        connectorId => true,
                                                        null,
                                                        EVSE.Status.Timestamp > EVSE.LastChange
                                                            ? EVSE.Status.Timestamp
                                                            : EVSE.LastChange,
                                                        out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(location,
                                                                             evse2,
                                                                             null,
                                                                             false,
                                                                             EventTrackingId);
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
        public override async Task<WWCP.PushEVSEDataResult>

            DeleteStaticData(WWCP.IEVSE              EVSE,
                             WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?               Timestamp           = null,
                             CancellationToken       CancellationToken   = default,
                             EventTracking_Id?       EventTrackingId     = null,
                             TimeSpan?               RequestTimeout      = null)

        {

            await Task.Delay(100);

            return WWCP.PushEVSEDataResult.NoOperation(
                       AuthId:          Id,
                       SendPOIData:     this,
                       RejectedEVSEs:   Array.Empty<WWCP.IEVSE>(),
                       Description:     null,
                       Warnings:        null,
                       Runtime:         null
                   );

        }

        #endregion


        #region SetStaticData   (EVSEs, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.PushEVSEDataResult>

            SetStaticData(IEnumerable<WWCP.IEVSE>  EVSEs,
                          WWCP.TransmissionTypes   TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                          DateTime?                Timestamp           = null,
                          CancellationToken        CancellationToken   = default,
                          EventTracking_Id?        EventTrackingId     = null,
                          TimeSpan?                RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    if (!EVSEs.Any())
                        return WWCP.PushEVSEDataResult.NoOperation(
                                    Id,
                                    this,
                                    EVSEs
                                );

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    foreach (var evse in EVSEs)
                    {
                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                        {


                            var _evse = evse.ToOCPI(CustomEVSEUIdConverter,
                                                    CustomEVSEIdConverter,
                                                    connectorId => true,
                                                    null,
                                                    evse.Status.Timestamp > evse.LastChange
                                                        ? evse.Status.Timestamp
                                                        : evse.LastChange,
                                                    out warnings);

                            //if (_evse is not null)
                            //    CommonAPI.AddLocation(location);

                        }
                    }

                    return WWCP.PushEVSEDataResult.Enqueued(
                                Id,
                                this,
                                EVSEs,
                                "",
                                warnings,
                                TimeSpan.FromMilliseconds(10)
                            );

                }
                else
                    return WWCP.PushEVSEDataResult.LockTimeout(
                                Id,
                                this,
                                EVSEs
                            );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region SetStaticData   (EVSEs, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.PushEVSEDataResult>

            AddStaticData(IEnumerable<WWCP.IEVSE>  EVSEs,
                          WWCP.TransmissionTypes           TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                          DateTime?                        Timestamp           = null,
                          CancellationToken                CancellationToken   = default,
                          EventTracking_Id?                EventTrackingId     = null,
                          TimeSpan?                        RequestTimeout      = null)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime);

            try
            {

                if (lockTaken)
                {

                    if (!EVSEs.Any())
                        return WWCP.PushEVSEDataResult.NoOperation(
                                    Id,
                                    this,
                                    EVSEs
                                );

                    IEnumerable<Warning> warnings = Array.Empty<Warning>();

                    foreach (var evse in EVSEs)
                    {
                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                        {

                            var _evse = evse.ToOCPI(CustomEVSEUIdConverter,
                                                    CustomEVSEIdConverter,
                                                    connectorId => true,
                                                    null,
                                                    evse.Status.Timestamp > evse.LastChange
                                                        ? evse.Status.Timestamp
                                                        : evse.LastChange,
                                                    out warnings);

                            if (_evse is not null)
                            {

                                //var result = CommonAPI.AddLocation(location);


                                //ToDo: Process errors!!!


                            }
                        }
                    }

                    return WWCP.PushEVSEDataResult.Enqueued(
                               Id,
                               this,
                               EVSEs,
                               "",
                               warnings,
                               TimeSpan.FromMilliseconds(10)
                           );

                }
                else
                    return WWCP.PushEVSEDataResult.LockTimeout(
                               Id,
                               this,
                               EVSEs
                           );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region UpdateStaticData(EVSEs, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.PushEVSEDataResult>

            UpdateStaticData(IEnumerable<WWCP.IEVSE>  EVSEs,
                             WWCP.TransmissionTypes   TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?                Timestamp           = null,
                             CancellationToken        CancellationToken   = default,
                             EventTracking_Id?        EventTrackingId     = null,
                             TimeSpan?                RequestTimeout      = null)

        {

            await Task.Delay(100);

            return WWCP.PushEVSEDataResult.NoOperation(
                       Id,
                       this,
                       EVSEs
                   );

        }

        #endregion

        #region DeleteStaticData(EVSEs, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.PushEVSEDataResult>

            DeleteStaticData(IEnumerable<WWCP.IEVSE>  EVSEs,
                             WWCP.TransmissionTypes   TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?                Timestamp           = null,
                             CancellationToken        CancellationToken   = default,
                             EventTracking_Id?        EventTrackingId     = null,
                             TimeSpan?                RequestTimeout      = null)

        {

            await Task.Delay(100);

            return WWCP.PushEVSEDataResult.NoOperation(
                       Id,
                       this,
                       EVSEs
                   );

        }

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

                                var locationId = evse.ChargingPool is not null
                                                     ? Location_Id.TryParse(evse.ChargingPool.Id.ToString())
                                                     : null;

                                if (locationId.HasValue)
                                {

                                    if (CommonAPI.TryGetLocation(locationId.Value, out var location) &&
                                        location is not null)
                                    {

                                        var evse2 = evse.ToOCPI(CustomEVSEUIdConverter,
                                                                CustomEVSEIdConverter,
                                                                connectorId => true,
                                                                null,
                                                                evse.Status.Timestamp > evse.LastChange
                                                                    ? evse.Status.Timestamp
                                                                    : evse.LastChange,
                                                                ref warnings);

                                        if (evse2 is not null)
                                        {

                                            var result2 = await CommonAPI.UpdateEVSE(location, evse2);

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


            DateTime              endtime;
            TimeSpan              runtime;
            WWCP.AuthStartResult? authStartResult = null;


            if (DisableAuthentication)
                authStartResult = WWCP.AuthStartResult.AdminDown(
                                      AuthorizatorId:           Id,
                                      ISendAuthorizeStartStop:  this,
                                      SessionId:                SessionId,
                                      Description:              I18NString.Create(Languages.en, "Authentication is disabled!"),
                                      Runtime:                  TimeSpan.Zero
                                  );

            else
            {

                var remotes = new PriorityList<RemoteParty>();
                foreach (var remote in CommonAPI.GetRemoteParties(OCPI.Roles.EMSP))
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


                                                                                           var remoteAccessInfo  = remoteParty.RemoteAccessInfos.FirstOrDefault(remoteAccessInfo => remoteAccessInfo.Status == OCPI.RemoteAccessStatus.ONLINE);

                                                                                           if (remoteAccessInfo is null)
                                                                                               return new AuthorizationInfo(
                                                                                                          Allowed:      AllowedType.NOT_ALLOWED,
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
                                                                                                            authorizationInfo.Data.Location,
                                                                                                            authorizationInfo.Data.Info,
                                                                                                            remoteParty,
                                                                                                            EMSP_Id.From(remoteParty.Id),
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
                               ProviderId:                authorizationInfo.EMSPId.ToWWCP(),
                                                          //WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
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
                               ProviderId:                authorizationInfo.EMSPId.ToWWCP(),
                                                          //WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
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
                               ProviderId:                authorizationInfo.EMSPId.ToWWCP(),
                                                          //WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
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
                               ProviderId:                authorizationInfo.EMSPId.ToWWCP(),
                                                          //WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
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

            }

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

        #region AuthorizeStop (SessionId, LocalAuthentication, ...)

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
        public async Task<WWCP.AuthStopResult>

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


            DateTime             endtime;
            TimeSpan             runtime;
            WWCP.AuthStopResult? authStopResult = null;

            if (DisableAuthentication)
                authStopResult = WWCP.AuthStopResult.AdminDown(
                                     AuthorizatorId:           Id,
                                     ISendAuthorizeStartStop:  this,
                                     SessionId:                SessionId,
                                     Description:              I18NString.Create(Languages.en, "Authentication is disabled!"),
                                     Runtime:                  TimeSpan.Zero
                                 );

            else
            {

                var remotes = new PriorityList<RemoteParty>();
                foreach (var remote in CommonAPI.GetRemoteParties(OCPI.Roles.EMSP))
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


                                                                                           var remoteAccessInfo  = remoteParty.RemoteAccessInfos.FirstOrDefault(remoteAccessInfo => remoteAccessInfo.Status == OCPI.RemoteAccessStatus.ONLINE);

                                                                                           if (remoteAccessInfo is null)
                                                                                               return new AuthorizationInfo(
                                                                                                          Allowed:      AllowedType.NOT_ALLOWED,
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
                                                                                                            authorizationInfo.Data.Location,
                                                                                                            authorizationInfo.Data.Info,
                                                                                                            remoteParty,
                                                                                                            EMSP_Id.From(remoteParty.Id),
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





                if (authorizationInfo is null)
                    authStopResult = WWCP.AuthStopResult.CommunicationTimeout(Id, this, SessionId);

                else if (authorizationInfo.Allowed == AllowedType.ALLOWED)
                    authStopResult = WWCP.AuthStopResult.Authorized(
                               AuthorizatorId:            Id,
                               ISendAuthorizeStartStop:   this,
                               SessionId:                 SessionId,
                               ProviderId:                authorizationInfo.EMSPId.ToWWCP(),
                                                          //WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                               Description:               null,
                               AdditionalInfo:            null,
                               NumberOfRetries:           0,
                               Runtime:                   null
                           );

                else if (authorizationInfo.Allowed == AllowedType.BLOCKED)
                    authStopResult = WWCP.AuthStopResult.Blocked(
                               AuthorizatorId:            Id,
                               ISendAuthorizeStartStop:   this,
                               SessionId:                 SessionId,
                               ProviderId:                authorizationInfo.EMSPId.ToWWCP(),
                                                          //WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                               Description:               null,
                               AdditionalInfo:            null,
                               NumberOfRetries:           0,
                               Runtime:                   null
                           );

                //else if (authorizationInfo.Allowed == AllowedType.EXPIRED)
                //    authStopResult = WWCP.AuthStopResult.Expired(
                //               AuthorizatorId:            Id,
                //               ISendAuthorizeStartStop:   this,
                //               SessionId:                 SessionId,
                //               ProviderId:                authorizationInfo.EMSPId.ToWWCP(),
                //                                          //WWCP.EMobilityProvider_Id.Parse($"{authorizationInfo.RemoteParty?.CountryCode.ToString() ?? "XX"}-{authorizationInfo.RemoteParty?.PartyId.ToString() ?? "XXX"}"),
                //               Description:               null,
                //               AdditionalInfo:            null,
                //               NumberOfRetries:           0,
                //               Runtime:                   null
                //           );

                else if (authorizationInfo.Allowed == AllowedType.NOT_ALLOWED)
                    authStopResult = WWCP.AuthStopResult.NotAuthorized(
                               AuthorizatorId:            Id,
                               ISendAuthorizeStartStop:   this,
                               SessionId:                 SessionId,
                               ProviderId:                null,
                               Description:               null,
                               AdditionalInfo:            null,
                               NumberOfRetries:           0,
                               Runtime:                   null
                           );

            }

            authStopResult ??= WWCP.AuthStopResult.Error(Id, this, SessionId);


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

            return authStopResult;

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

            DateTime?             endtime          = null;
            TimeSpan?             runtime          = null;
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

            else
            {

                var warnings        = new List<Warning>();
                var sendCDRResults  = new List<WWCP.SendCDRResult>();

                foreach (var chargeDetailRecord in ChargeDetailRecords)
                {

                    if (chargeDetailRecord.ProviderIdStart.HasValue)
                    {

                        #region Setup remote party

                        var emspId            = EMSP_Id.Parse(chargeDetailRecord.ProviderIdStart.Value.ToString());
                        var remoteParty       = CommonAPI.GetRemoteParty(RemoteParty_Id.From(emspId));
                        var remoteAccessInfo  = remoteParty?.RemoteAccessInfos.FirstOrDefault(remoteAccessInfo => remoteAccessInfo.Status == OCPI.RemoteAccessStatus.ONLINE);

                        //if (remoteAccessInfo is null)
                        //    return new AuthorizationInfo(
                        //               Allowed:      AllowedType.NOT_ALLOWED,
                        //               Info:         new DisplayText(Languages.en, $"No remote access information for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                        //               RemoteParty:  remoteParty
                        //           );


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

                        //if (cpoClient is null)
                        //    return new AuthorizationInfo(
                        //               Allowed:      AllowedType.NOT_ALLOWED,
                        //               Info:         new DisplayText(Languages.en, $"Could not get/create a CPO client for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                        //               RemoteParty:  remoteParty
                        //           );


                        var cpoClientLogger = new CPO.HTTP.CPOClient.Logger(
                                                  cpoClient,
                                                  ClientsLoggingPath ?? DefaultHTTPAPI_LoggingPath,
                                                  ClientsLoggingContext ?? DefaultLoggingContext,
                                                  ClientsLogfileCreator
                                              );

                        #endregion

                        #region Convert and send charge detail record

                        var cdr = chargeDetailRecord.ToOCPI(CustomEVSEUIdConverter,
                                                            CustomEVSEIdConverter,
                                                            CommonAPI.GetTariffIds,
                                                            EMSP_Id.Parse(chargeDetailRecord.ProviderIdStart.Value.ToString()),
                                                            CommonAPI.GetTariff,
                                                            ref warnings);

                        if (cdr is null)
                        {

                            endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                            runtime = endtime - startTime;
                            sendCDRResults.Add(WWCP.SendCDRResult.Error(endtime.Value,
                                                                        chargeDetailRecord,
                                                                        warnings,
                                                                        I18NString.Create(Languages.en, $"Converting the charge detail record to OCPI {Version.String} failed!"),
                                                                        runtime));

                        }
                        else
                        {

                            var addOrUpdateResult  = await CommonAPI.AddOrUpdateCDR(CDR:                cdr,
                                                                                    AllowDowngrades:    false,
                                                                                    SkipNotifications:  false,
                                                                                    EventTrackingId:    EventTrackingId);

                            var response           = await cpoClient.PostCDR(
                                                               CDR: cdr,
                                                               EMSPId: emspId
                                                           );

                            if (response is not null)
                            {

                                endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                                runtime = endtime - startTime;

                                if (response.StatusCode == 1000)
                                    sendCDRResults.Add(WWCP.SendCDRResult.Success(endtime.Value,
                                                                                  chargeDetailRecord,
                                                                                  warnings,
                                                                                  Location:  response.Location,
                                                                                  Runtime:   runtime));

                                else
                                    sendCDRResults.Add(WWCP.SendCDRResult.Error(endtime.Value,
                                                                                chargeDetailRecord,
                                                                                warnings,
                                                                                I18NString.Create(Languages.en, "Sending the charge detail record failed: " + response.StatusMessage + " (" + response.StatusCode + ")!"),
                                                                                runtime));

                            }
                            else
                            {

                                endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                                runtime = endtime - startTime;
                                sendCDRResults.Add(WWCP.SendCDRResult.Error(endtime.Value,
                                                                            chargeDetailRecord,
                                                                            warnings,
                                                                            I18NString.Create(Languages.en, "Sending the charge detail record failed!"),
                                                                            runtime));

                            }

                        }

                        #endregion

                    }
                    else
                    {

                        endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                        runtime = endtime - startTime;
                        sendCDRResults.Add(WWCP.SendCDRResult.Error(endtime.Value,
                                                                    chargeDetailRecord,
                                                                    I18NString.Create(Languages.en, "No ProviderIdStart defined!"),
                                                                    runtime));

                    }

                }

                endtime         = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                runtime         = endtime - startTime;
                sendCDRsResult  = WWCP.SendCDRResultTypesExtensions.Combine(sendCDRResults,
                                                                            Id,
                                                                            this,
                                                                            Warnings: warnings);

            }


            endtime         ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            runtime         ??= endtime - startTime;
            sendCDRsResult  ??= WWCP.SendCDRsResult.Error(endtime.Value,
                                                          Id,
                                                          this,
                                                          ChargeDetailRecords,
                                                          I18NString.Create(Languages.en, "Unknown error!"));


            #region Send OnSendCDRsRequest event

            try
            {

                OnSendCDRsResponse?.Invoke(endtime.Value,
                                           Timestamp.Value,
                                           this,
                                           Id.ToString(),
                                           EventTrackingId,
                                           RoamingNetwork.Id,
                                           ChargeDetailRecords,
                                           RequestTimeout,
                                           sendCDRsResult,
                                           runtime.Value);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPICSOAdapter) + "." + nameof(OnSendCDRsRequest));
            }

            #endregion

            return sendCDRsResult;

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

            => Object is OCPICSOAdapter ocpiCSOAdapter
                   ? CompareTo(ocpiCSOAdapter)
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
