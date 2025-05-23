﻿/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

    public delegate IEnumerable<Tariff>          GetTariffs_Delegate  (WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId,
                                                                       WWCP.ChargingPool_Id?             ChargingPoolId,
                                                                       WWCP.ChargingStation_Id?          ChargingStationId,
                                                                       WWCP.EVSE_Id?                     EVSEId,
                                                                       WWCP.ChargingConnector_Id?        ChargingConnectorId,
                                                                       WWCP.EMobilityProvider_Id?        EMobilityProviderId);

    public delegate IEnumerable<OCPI.Tariff_Id>  GetTariffIds_Delegate(WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId,
                                                                       WWCP.ChargingPool_Id?             ChargingPoolId,
                                                                       WWCP.ChargingStation_Id?          ChargingStationId,
                                                                       WWCP.EVSE_Id?                     EVSEId,
                                                                       WWCP.ChargingConnector_Id?        ChargingConnectorId,
                                                                       WWCP.EMobilityProvider_Id?        EMobilityProviderId);


    /// <summary>
    /// Receive charging stations downstream from an OCPI partner...
    /// </summary>
    public class OCPICSOAdapter : WWCP.AWWCPCSOAdapter<CDR>,
                                  WWCP.ICSORoamingProvider,
                                  WWCP.ISendEnergyStatus,
                                  IEquatable <OCPICSOAdapter>,
                                  IComparable<OCPICSOAdapter>,
                                  IComparable

    {

        #region Data

        /// <summary>
        /// The default logging context.
        /// </summary>
        public  const       String         DefaultLoggingContext        = $"OCPI{Version.String}_CSOAdapter";

        public  const       String         DefaultHTTPAPI_LoggingPath   = "default";

        public  const       String         DefaultHTTPAPI_LogfileName   = $"OCPI{Version.String}_CSOAdapter.log";


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


        public new OCPILogfileCreatorDelegate? ClientsLogfileCreator { get; }

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

        public OCPICSOAdapter(WWCP.CSORoamingProvider_Id                      Id,
                              I18NString                                      Name,
                              I18NString                                      Description,
                              WWCP.IRoamingNetwork                            RoamingNetwork,

                              HTTP.CommonAPI                                  CommonAPI,

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
                              OCPILogfileCreatorDelegate?                     LogfileCreator                      = null,

                              String?                                         ClientsLoggingPath                  = DefaultHTTPAPI_LoggingPath,
                              String?                                         ClientsLoggingContext               = DefaultLoggingContext,
                              OCPILogfileCreatorDelegate?                     ClientsLogfileCreator               = null,
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
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator       (loggingPath, null, context, logfileName)
                       : null,
                   ClientsLoggingPath,
                   ClientsLoggingContext,
                   ClientsLogfileCreator is not null
                       ? (loggingPath, context, logfileName) => ClientsLogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   DNSClient)

        {

            this.CommonAPI                          = CommonAPI;
            this.CPOAPI                             = new HTTP.CPOAPI(

                                                          this.CommonAPI,
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
                                                          // AutoStart

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

            this.ClientsLogfileCreator              = ClientsLogfileCreator;

            WireIncomingRequests();

        }


        public OCPICSOAdapter(WWCP.CSORoamingProvider_Id                      Id,
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
                              OCPILogfileCreatorDelegate?                     ClientsLogfileCreator               = null,
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
                   ClientsLogfileCreator is not null
                       ? (loggingPath, context, logfileName) => ClientsLogfileCreator(loggingPath, null, context, logfileName)
                       : null,
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

            this.ClientsLogfileCreator              = ClientsLogfileCreator;

            WireIncomingRequests();

        }

        #endregion


        private void WireIncomingRequests()
        {

            #region OnStartSessionCommand  => RemoteStart

            this.CPOAPI.OnStartSessionCommand += async (emspId, startSessionCommand) => {

                if (!CommonAPI.TryGetLocation(startSessionCommand.LocationId, out var location) || location is null)
                    return new CommandResponse(
                               startSessionCommand,
                               CommandResponseTypes.REJECTED,
                               TimeSpan.FromMinutes(1),
                               [ DisplayText.Create(Languages.en, "StartSessionCommand rejected!") ]
                           );

                if (!startSessionCommand.EVSEUId.HasValue)
                    return new CommandResponse(
                               startSessionCommand,
                               CommandResponseTypes.REJECTED,
                               TimeSpan.FromMinutes(1),
                               [ DisplayText.Create(Languages.en, "StartSessionCommand rejected!") ]
                           );

                if (!location.TryGetEVSE(startSessionCommand.EVSEUId.Value, out var evse) || evse is null)
                    return new CommandResponse(
                               startSessionCommand,
                               CommandResponseTypes.REJECTED,
                               TimeSpan.FromMinutes(1),
                               [ DisplayText.Create(Languages.en, "StartSessionCommand rejected!") ]
                           );

                if (!evse.EVSEId.HasValue)
                    return new CommandResponse(
                               startSessionCommand,
                               CommandResponseTypes.REJECTED,
                               TimeSpan.FromMinutes(1),
                               [ DisplayText.Create(Languages.en, "StartSessionCommand rejected!") ]
                           );

                var wwcpEVSEId = evse.EVSEId.Value.ToWWCP();

                if (!wwcpEVSEId.HasValue)
                    return new CommandResponse(
                               startSessionCommand,
                               CommandResponseTypes.REJECTED,
                               TimeSpan.FromMinutes(1),
                               [
                                   DisplayText.Create(
                                       Languages.en,
                                       "StartSessionCommand rejected!"
                                   )
                               ]
                           );


                var providerId  = emspId.ToWWCP();

                var result      = await RoamingNetwork.RemoteStart(
                                            WWCP.ChargingLocation.FromEVSEId(wwcpEVSEId.Value),
                                            null,                                   // ChargingProduct
                                            null,                                   // ReservationId
                                            providerId.HasValue                     // OCPI does not have its own charging session identification!
                                                ? WWCP.ChargingSession_Id.NewRandom(providerId.Value)
                                                : WWCP.ChargingSession_Id.NewRandom(),
                                            providerId,
                                            WWCP.RemoteAuthentication.FromRemoteIdentification(WWCP.EMobilityAccount_Id.Parse(startSessionCommand.Token.Id.ToString())),
                                            null,
                                            WWCP.Auth_Path.Parse(Id.ToString())     // Authentication path == CSO Roaming Provider identification!
                                        );


                if (result.Result == WWCP.RemoteStartResultTypes.Success)
                {

                    return new CommandResponse(
                               startSessionCommand,
                               CommandResponseTypes.ACCEPTED,
                               TimeSpan.FromMinutes(1),
                               [ DisplayText.Create(Languages.en, "StartSessionCommand accepted!") ]
                           );

                }

                else
                    return new CommandResponse(
                               startSessionCommand,
                               CommandResponseTypes.REJECTED,
                               TimeSpan.FromMinutes(1),
                               [ DisplayText.Create(Languages.en, "StartSessionCommand rejected!") ]
                           );

            };

            #endregion

            #region OnStopSessionCommand   => RemoteStop

            this.CPOAPI.OnStopSessionCommand += async (emspId, stopSessionCommand) => {

                var result = await RoamingNetwork.RemoteStop(
                                       WWCP.ChargingSession_Id.Parse(stopSessionCommand.SessionId.ToString()),
                                       WWCP.ReservationHandling.Close,
                                       emspId.ToWWCP(),
                                       null,                                   // Remote authentication
                                       WWCP.Auth_Path.Parse(Id.ToString())     // Authentication path == CSO Roaming Provider identification!
                                   );


                if (result.Result == WWCP.RemoteStopResultTypes.Success)
                {

                    return new CommandResponse(
                               stopSessionCommand,
                               CommandResponseTypes.ACCEPTED,
                               TimeSpan.FromMinutes(1),
                               [ DisplayText.Create(Languages.en, "StopSessionCommand accepted!") ]
                           );

                }

                else
                    return new CommandResponse(
                               stopSessionCommand,
                               CommandResponseTypes.REJECTED,
                               TimeSpan.FromMinutes(1),
                               [ DisplayText.Create(Languages.en, "StopSessionCommand rejected!") ]
                           );

            };

            #endregion

        }


        #region AddRemoteParty(...)

        public Task<Boolean> AddRemoteParty(OCPI.CountryCode          CountryCode,
                                            OCPI.Party_Id             PartyId,
                                            OCPI.Role                 Role,
                                            OCPI.BusinessDetails      BusinessDetails,

                                            OCPI.AccessToken          AccessToken,

                                            OCPI.AccessToken          RemoteAccessToken,
                                            URL                       RemoteVersionsURL,
                                            IEnumerable<Version_Id>?  RemoteVersionIds            = null,
                                            Version_Id?               SelectedVersionId           = null,

                                            DateTime?                 LocalAccessNotBefore        = null,
                                            DateTime?                 LocalAccessNotAfter         = null,

                                            Boolean?                  AccessTokenBase64Encoding   = null,
                                            Boolean?                  AllowDowngrades             = false,
                                            OCPI.AccessStatus         AccessStatus                = OCPI.AccessStatus.      ALLOWED,
                                            OCPI.RemoteAccessStatus?  RemoteStatus                = OCPI.RemoteAccessStatus.ONLINE,
                                            OCPI.PartyStatus          PartyStatus                 = OCPI.PartyStatus.       ENABLED)

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
                                            OCPI.Role             Role,
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


        #region (Set/Add/Update/Delete) Charging pool(s)...

        #region AddChargingPool         (ChargingPool,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public override async Task<WWCP.AddChargingPoolResult>

            AddChargingPool(WWCP.IChargingPool      ChargingPool,
                            WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                            DateTime?               Timestamp           = null,
                            EventTracking_Id?       EventTrackingId     = null,
                            TimeSpan?               RequestTimeout      = null,
                            User_Id?                CurrentUserId       = null,
                            CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            try
            {

                if (lockTaken)
                {

                    EventTrackingId ??= EventTracking_Id.New;

                    IEnumerable<Warning> warnings = [];

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {

                        var location = ChargingPool.ToOCPI(
                                           CustomEVSEUIdConverter,
                                           CustomEVSEIdConverter,
                                           evseId      => true,
                                           connectorId => true,
                                           null,
                                           out warnings
                                       );

                        if (location is not null)
                        {

                            var result = await CommonAPI.AddLocation(
                                                   Location:            location,
                                                   SkipNotifications:   false,
                                                   EventTrackingId:     EventTrackingId,
                                                   CurrentUserId:       null,
                                                   CancellationToken:   CancellationToken
                                               );

                            //ToDo: Handle errors!!!

                        }

                        return WWCP.AddChargingPoolResult.Enqueued(
                                   ChargingPool,
                                   EventTrackingId,
                                   Id,
                                   this,
                                   Warnings: warnings
                               );

                    }
                    else
                        return WWCP.AddChargingPoolResult.NoOperation(
                                   ChargingPool,
                                   EventTrackingId,
                                   Id,
                                   this
                               );

                }
                else
                    return WWCP.AddChargingPoolResult.LockTimeout(
                               ChargingPool,
                               MaxLockWaitingTime,
                               EventTrackingId,
                               Id,
                               this
                           );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region AddOrUpdateChargingPool (ChargingPool,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public override async Task<WWCP.AddOrUpdateChargingPoolResult>

            AddOrUpdateChargingPool(WWCP.IChargingPool      ChargingPool,
                                    WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                                    DateTime?               Timestamp           = null,
                                    EventTracking_Id?       EventTrackingId     = null,
                                    TimeSpan?               RequestTimeout      = null,
                                    User_Id?                CurrentUserId       = null,
                                    CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            EventTrackingId ??= EventTracking_Id.New;

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = [];

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {


                        var location = ChargingPool.ToOCPI(
                                           CustomEVSEUIdConverter,
                                           CustomEVSEIdConverter,
                                           evseId      => true,
                                           connectorId => true,
                                           null,
                                           out warnings
                                       );

                        if (location is not null)
                        {

                            var result = await CommonAPI.AddLocation(
                                                   location,
                                                   false,
                                                   EventTrackingId,
                                                   CurrentUserId,
                                                   CancellationToken
                                               );

                            //ToDo: Handle errors!

                        }


                        return WWCP.AddOrUpdateChargingPoolResult.Enqueued(
                                   ChargingPool,
                                   EventTrackingId,
                                   Id,
                                   this,
                                   Warnings: warnings
                               );

                    }
                    else
                        return WWCP.AddOrUpdateChargingPoolResult.NoOperation(
                                   ChargingPool,
                                   EventTrackingId,
                                   Id,
                                   this
                               );

                }
                else
                    return WWCP.AddOrUpdateChargingPoolResult.LockTimeout(
                               ChargingPool,
                               MaxLockWaitingTime,
                               EventTrackingId,
                               Id,
                               this
                           );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region UpdateChargingPool      (ChargingPool,  PropertyName = null, NewValue = null, OldValue = null, DataSource = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="PropertyName">The name of the charging pool property to update, if any specific.</param>
        /// <param name="NewValue">The new value of the charging pool property to update.</param>
        /// <param name="OldValue">The optional old value of the charging pool property to update.</param>
        /// <param name="DataSource">An optional data source or context for the charging pool property update.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public override async Task<WWCP.UpdateChargingPoolResult>

            UpdateChargingPool(WWCP.IChargingPool      ChargingPool,
                               String?                 PropertyName        = null,
                               Object?                 NewValue            = null,
                               Object?                 OldValue            = null,
                               Context?                DataSource          = null,
                               WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                               DateTime?               Timestamp           = null,
                               EventTracking_Id?       EventTrackingId     = null,
                               TimeSpan?               RequestTimeout      = null,
                               User_Id?                CurrentUserId       = null,
                               CancellationToken       CancellationToken   = default)
        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            EventTrackingId ??= EventTracking_Id.New;

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = [];

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

                            var result = await CommonAPI.UpdateLocation(
                                                   Location:            location,
                                                   AllowDowngrades:     null,
                                                   SkipNotifications:   false,
                                                   EventTrackingId:     EventTrackingId,
                                                   CurrentUserId:       null,
                                                   CancellationToken:   CancellationToken
                                               );

                            //ToDo: Process errors!!!

                            if (PropertyName is not null)
                            {

                                if (chargingPoolsUpdateLog.TryGetValue(ChargingPool, out var propertyUpdateInfos))
                                    propertyUpdateInfos.Add(new PropertyUpdateInfo(PropertyName, OldValue, NewValue));

                                else
                                    chargingPoolsUpdateLog.Add(
                                        ChargingPool,
                                        [ new PropertyUpdateInfo(PropertyName, OldValue, NewValue) ]
                                    );

                            }

                            return WWCP.UpdateChargingPoolResult.Enqueued(
                                       ChargingPool,
                                       EventTrackingId,
                                       Id,
                                       this,
                                       Warnings: warnings
                                   );

                        }

                    }

                    return WWCP.UpdateChargingPoolResult.NoOperation(
                               ChargingPool,
                               EventTrackingId,
                               Id,
                               this
                           );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return WWCP.UpdateChargingPoolResult.LockTimeout(
                       ChargingPool,
                       MaxLockWaitingTime,
                       EventTrackingId,
                       Id,
                       this
                   );

        }

        #endregion


        #region AddChargingPools        (ChargingPools, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.AddChargingPoolsResult>

            AddChargingPools(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                             WWCP.TransmissionTypes           TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                             DateTime?                        Timestamp           = null,
                             EventTracking_Id?                EventTrackingId     = null,
                             TimeSpan?                        RequestTimeout      = null,
                             User_Id?                         CurrentUserId       = null,
                             CancellationToken                CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            EventTrackingId ??= EventTracking_Id.New;

            try
            {

                if (lockTaken)
                {

                    if (!ChargingPools.Any())
                        return WWCP.AddChargingPoolsResult.NoOperation(
                                   ChargingPools,
                                   Id,
                                   this,
                                   EventTrackingId
                               );

                    IEnumerable<Warning> warnings = [];

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

                                var result = await CommonAPI.AddLocation(location);


                                //ToDo: Process errors!!!


                            }

                        }
                    }

                    return WWCP.AddChargingPoolsResult.Enqueued(
                               ChargingPools,
                               Id,
                               this,
                               EventTrackingId
                           );

                }
                else
                    return WWCP.AddChargingPoolsResult.LockTimeout(
                               ChargingPools,
                               MaxLockWaitingTime,
                               Id,
                               this
                           );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region AddOrUpdateChargingPools(ChargingPools, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.AddOrUpdateChargingPoolsResult>

            AddOrUpdateChargingPools(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                                     WWCP.TransmissionTypes           TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                                     DateTime?                        Timestamp           = null,
                                     EventTracking_Id?                EventTrackingId     = null,
                                     TimeSpan?                        RequestTimeout      = null,
                                     User_Id?                         CurrentUserId       = null,
                                     CancellationToken                CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            try
            {

                if (lockTaken)
                {

                    if (!ChargingPools.Any())
                        return WWCP.AddOrUpdateChargingPoolsResult.NoOperation(
                                   ChargingPools,
                                   Id,
                                   this,
                                   EventTrackingId
                               );

                    IEnumerable<Warning> warnings = [];

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

                                var result = await CommonAPI.AddLocation(location);

                                //ToDo: Handle errors!

                            }

                        }
                    }

                    return WWCP.AddOrUpdateChargingPoolsResult.Enqueued(
                               ChargingPools,
                               Id,
                               this,
                               EventTrackingId
                           );

                }
                else
                    return WWCP.AddOrUpdateChargingPoolsResult.LockTimeout(
                               ChargingPools,
                               MaxLockWaitingTime,
                               Id,
                               this,
                               EventTrackingId
                           );

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

        }

        #endregion

        #region UpdateChargingPools     (ChargingPools, TransmissionType = Enqueue, ...)

        public override async Task<WWCP.UpdateChargingPoolsResult>

            UpdateChargingPools(IEnumerable<WWCP.IChargingPool>  ChargingPools,
                                WWCP.TransmissionTypes           TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                                DateTime?                        Timestamp           = null,
                                EventTracking_Id?                EventTrackingId     = null,
                                TimeSpan?                        RequestTimeout      = null,
                                User_Id?                         CurrentUserId       = null,
                                CancellationToken                CancellationToken   = default)

        {

            await Task.Delay(100, CancellationToken);

            return WWCP.UpdateChargingPoolsResult.NoOperation(
                       ChargingPools,
                       Id,
                       this,
                       EventTrackingId
                   );

        }

        #endregion

        #endregion


        #region UpdateChargingStation        (ChargingStation,  PropertyName = null, NewValue = null, OldValue = null, DataSource = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given charging station.
        /// </summary>
        /// <param name="ChargingStation">A charging station to update.</param>
        /// <param name="PropertyName">The name of the charging station property to update, if any specific.</param>
        /// <param name="NewValue">The new value of the charging station property to update.</param>
        /// <param name="OldValue">The optional old value of the charging station property to update.</param>
        /// <param name="DataSource">An optional data source or context for the charging station property update.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public override async Task<WWCP.UpdateChargingStationResult>

            UpdateChargingStation(WWCP.IChargingStation   ChargingStation,
                                  String?                 PropertyName        = null,
                                  Object?                 NewValue            = null,
                                  Object?                 OldValue            = null,
                                  Context?                DataSource          = null,
                                  WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                                  DateTime?               Timestamp           = null,
                                  EventTracking_Id?       EventTrackingId     = null,
                                  TimeSpan?               RequestTimeout      = null,
                                  User_Id?                CurrentUserId       = null,
                                  CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            EventTrackingId ??= EventTracking_Id.New;

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<IEnumerable<EVSE>> result = AddOrUpdateResult<IEnumerable<EVSE>>.NoOperation(EventTrackingId, "No EVSEs found!");

                    IEnumerable<Warning> warnings = [];

                    var locationId  = ChargingStation.ChargingPool is not null
                                          ? Location_Id.TryParse(ChargingStation.ChargingPool.Id.ToString())
                                          : null;

                    if (locationId.HasValue)
                    {

                        var evses = new List<EVSE>();

                        foreach (var evse in ChargingStation.EVSEs)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                var evse2 = evse.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        connectorId => true,
                                                        new DateTime[] {
                                                            evse.Status.Timestamp,
                                                            evse.LastChangeDate,
                                                            ChargingStation.LastChangeDate
                                                        }.Max(),
                                                        out warnings);

                                if (evse2 is not null)
                                    evses.Add(evse2);

                                else
                                    result = AddOrUpdateResult<IEnumerable<EVSE>>.Failed(EventTrackingId, "Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<IEnumerable<EVSE>>.Failed(EventTrackingId, "The given EVSE was filtered!");

                            if (!result.IsSuccess)
                                break;

                        }

                        if (evses.Count > 0)
                        {

                            if (CommonAPI.TryGetLocation(locationId.Value, out var location))
                                result = await CommonAPI.AddOrUpdateEVSEs(
                                                   Location:            location,
                                                   EVSEs:               evses,
                                                   AllowDowngrades:     true, // Multiple EVSEs per station will lead to the same timestamp!
                                                   SkipNotifications:   false,
                                                   EventTrackingId:     EventTrackingId,
                                                   CurrentUserId:       null,
                                                   CancellationToken:   CancellationToken
                                               );

                            else
                                result = AddOrUpdateResult<IEnumerable<EVSE>>.Failed(EventTrackingId, "Unknown location identification!");

                        }

                    }
                    else
                        result = AddOrUpdateResult<IEnumerable<EVSE>>.Failed(EventTrackingId, "Invalid location identification!");


                    return result.IsSuccess

                               ? WWCP.UpdateChargingStationResult.Success(
                                     ChargingStation,
                                     EventTrackingId,
                                     Id,
                                     this,
                                     Warnings: warnings
                                 )

                               : WWCP.UpdateChargingStationResult.Error(
                                     ChargingStation,
                                     I18NString.Create(
                                         Languages.en,
                                         result.ErrorResponse ?? "error"
                                     ),
                                     EventTrackingId,
                                     Id,
                                     this,
                                     Warnings: warnings
                                 );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.UpdateChargingStationResult.Enqueued   (ChargingStation,                     EventTrackingId, Id, this)
                       : WWCP.UpdateChargingStationResult.LockTimeout(ChargingStation, MaxLockWaitingTime, EventTrackingId, Id, this);

        }

        #endregion


        #region (Set/Add/Update/Delete) EVSE(s)...

        #region AddEVSE         (EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given EVSE.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public override async Task<WWCP.AddEVSEResult>

            AddEVSE(WWCP.IEVSE              EVSE,
                    WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                    DateTime?               Timestamp           = null,
                    EventTracking_Id?       EventTrackingId     = null,
                    TimeSpan?               RequestTimeout      = null,
                    User_Id?                CurrentUserId       = null,
                    CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            EventTrackingId ??= EventTracking_Id.New;

            try
            {

                if (lockTaken)
                {

                    AddResult<EVSE> result;

                    IEnumerable<Warning> warnings = [];

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
                                                        //null,
                                                        EVSE.Status.Timestamp > EVSE.LastChangeDate
                                                            ? EVSE.Status.Timestamp
                                                            : EVSE.LastChangeDate,
                                                        out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddEVSE(location,
                                                                     evse2,
                                                                     false,
                                                                     EventTrackingId);
                                else
                                    result = AddResult<EVSE>.Failed(EventTrackingId, "Could not convert the given EVSE!");

                            }
                            else
                                result = AddResult<EVSE>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = AddResult<EVSE>.Failed(EventTrackingId, "The given EVSE was filtered!");

                    }
                    else
                        result = AddResult<EVSE>.Failed(EventTrackingId, "Invalid location identification!");


                    return result.IsSuccess

                               ? WWCP.AddEVSEResult.Success(
                                     EVSE,
                                     EventTrackingId,
                                     Id,
                                     this,
                                     Warnings: warnings
                                 )

                               : WWCP.AddEVSEResult.Error(
                                     EVSE,
                                     I18NString.Create(
                                         Languages.en,
                                         result.ErrorResponse ?? "error"
                                     ),
                                     EventTrackingId,
                                     Id,
                                     this,
                                     Warnings: warnings
                                 );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.AddEVSEResult.Enqueued   (EVSE,                     EventTrackingId, Id, this)
                       : WWCP.AddEVSEResult.LockTimeout(EVSE, MaxLockWaitingTime, EventTrackingId, Id, this);

        }

        #endregion

        #region AddOrUpdateEVSE (EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given EVSE as new static EVSE data at the OCPI server.
        /// </summary>
        /// <param name="EVSE">An EVSE to upload.</param>
        /// <param name="TransmissionType">Whether to send the EVSE directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public override async Task<WWCP.AddOrUpdateEVSEResult>

            AddOrUpdateEVSE(WWCP.IEVSE              EVSE,
                            WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                            DateTime?               Timestamp           = null,
                            EventTracking_Id?       EventTrackingId     = null,
                            TimeSpan?               RequestTimeout      = null,
                            User_Id?                CurrentUserId       = null,
                            CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            EventTrackingId ??= EventTracking_Id.New;

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = [];

                    var locationId  = EVSE.ChargingPool is not null
                                          ? Location_Id.TryParse(EVSE.ChargingPool.Id.ToString())
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
                                                        //null,
                                                        EVSE.Status.Timestamp > EVSE.LastChangeDate
                                                            ? EVSE.Status.Timestamp
                                                            : EVSE.LastChangeDate,
                                                        out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(
                                                       location,
                                                       evse2,
                                                       null,
                                                       false,
                                                       EventTrackingId,
                                                       null,
                                                       CancellationToken
                                                   );
                                else
                                    result = AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "The given EVSE was filtered!");

                    }
                    else
                        result = AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Invalid location identification!");


                    return result.IsSuccess

                               ? result.WasCreated == true

                                     ? WWCP.AddOrUpdateEVSEResult.Added(
                                           EVSE,
                                           EventTrackingId,
                                           Id,
                                           this,
                                           Warnings: warnings
                                       )

                                     : WWCP.AddOrUpdateEVSEResult.Updated(
                                           EVSE,
                                           EventTrackingId,
                                           Id,
                                           this,
                                           Warnings: warnings
                                       )

                               : WWCP.AddOrUpdateEVSEResult.Error(
                                     EVSE,
                                     I18NString.Create(
                                         Languages.en,
                                         result.ErrorResponse ?? "error"
                                     ),
                                     EventTrackingId,
                                     Id,
                                     this,
                                     Warnings: warnings
                                 );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.AddOrUpdateEVSEResult.Enqueued   (EVSE,                     EventTrackingId, Id, this)
                       : WWCP.AddOrUpdateEVSEResult.LockTimeout(EVSE, MaxLockWaitingTime, EventTrackingId, Id, this);

        }

        #endregion

        #region UpdateEVSE      (EVSE, PropertyName = null, NewValue = null, OldValue = null, DataSource = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the EVSE data of the given charging pool within the static EVSE data at the OCPI server.
        /// </summary>
        /// <param name="EVSE">An EVSE to upload.</param>
        /// <param name="PropertyName">The name of the EVSE property to update.</param>
        /// <param name="NewValue">The new value of the EVSE property to update, if any specific.</param>
        /// <param name="OldValue">The optional old value of the EVSE property to update.</param>
        /// <param name="DataSource">An optional data source or context for the EVSE property update.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public override async Task<WWCP.UpdateEVSEResult>

            UpdateEVSE(WWCP.IEVSE              EVSE,
                       String?                 PropertyName        = null,
                       Object?                 NewValue            = null,
                       Object?                 OldValue            = null,
                       Context?                DataSource          = null,
                       WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                       DateTime?               Timestamp           = null,
                       EventTracking_Id?       EventTrackingId     = null,
                       TimeSpan?               RequestTimeout      = null,
                       User_Id?                CurrentUserId       = null,
                       CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            EventTrackingId ??= EventTracking_Id.New;

            try
            {

                if (lockTaken)
                {

                    AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = [];

                    var locationId  = EVSE.ChargingPool is not null
                                          ? Location_Id.TryParse(EVSE.ChargingPool.Id.ToString())
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
                                                        //null,
                                                        EVSE.Status.Timestamp > EVSE.LastChangeDate
                                                            ? EVSE.Status.Timestamp
                                                            : EVSE.LastChangeDate,
                                                        out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(
                                                       location,
                                                       evse2,
                                                       null,
                                                       false,
                                                       EventTrackingId,
                                                       null,
                                                       CancellationToken
                                                   );
                                else
                                    result = AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Could not convert the given EVSE!");

                            }
                            else
                                result = AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "The given EVSE was filtered!");

                    }
                    else
                        result = AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Invalid location identification!");


                    return result.IsSuccess

                               ? WWCP.UpdateEVSEResult.Success(
                                     EVSE,
                                     EventTrackingId,
                                     Id,
                                     this,
                                     Warnings: warnings
                                 )

                               : WWCP.UpdateEVSEResult.Error(
                                     EVSE,
                                     I18NString.Create(
                                         Languages.en,
                                         result.ErrorResponse ?? "error"
                                     ),
                                     EventTrackingId,
                                     Id,
                                     this,
                                     Warnings: warnings
                                 );

                }

            }
            finally
            {
                if (lockTaken)
                    DataAndStatusLock.Release();
            }

            return lockTaken
                       ? WWCP.UpdateEVSEResult.Enqueued   (EVSE,                     EventTrackingId, Id, this)
                       : WWCP.UpdateEVSEResult.LockTimeout(EVSE, MaxLockWaitingTime, EventTrackingId, Id, this);

        }

        #endregion

        #region DeleteEVSE      (EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Delete the EVSE data of the given EVSE from the static EVSE data at the OCPI server.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public override Task<WWCP.DeleteEVSEResult>

            DeleteEVSE(WWCP.IEVSE              EVSE,
                       WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                       DateTime?               Timestamp           = null,
                       EventTracking_Id?       EventTrackingId     = null,
                       TimeSpan?               RequestTimeout      = null,
                       User_Id?                CurrentUserId       = null,
                       CancellationToken       CancellationToken   = default)

        {

            return Task.FromResult(WWCP.DeleteEVSEResult.NoOperation(EVSE, EventTrackingId, Id, this));

        }

        #endregion


        #region UpdateEVSEStatus(EVSEStatusUpdates, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given enumeration of EVSE status updates.
        /// </summary>
        /// <param name="EVSEStatusUpdates">An enumeration of EVSE status updates.</param>
        /// <param name="TransmissionType">Whether to send the EVSE status updates directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        async Task<WWCP.PushEVSEStatusResult>

            WWCP.ISendStatus.UpdateEVSEStatus(IEnumerable<WWCP.EVSEStatusUpdate>  EVSEStatusUpdates,
                                              WWCP.TransmissionTypes              TransmissionType,

                                              DateTime?                           Timestamp,
                                              EventTracking_Id?                   EventTrackingId,
                                              TimeSpan?                           RequestTimeout,
                                              User_Id?                            CurrentUserId,
                                              CancellationToken                   CancellationToken)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(
                                      MaxLockWaitingTime,
                                      CancellationToken
                                  );

            try
            {

                if (lockTaken)
                {

                    WWCP.PushEVSEStatusResult result;

                    var startTime  = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                    var warnings   = new List<Warning>();
                    var results    = new List<WWCP.PushEVSEStatusResult>();

                    foreach (var evseStatusUpdate in EVSEStatusUpdates)
                    {

                        if (RoamingNetwork.TryGetEVSEById(evseStatusUpdate.Id, out var evse) && evse is not null)
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
                                                                //null,
                                                                evse.Status.Timestamp > evse.LastChangeDate
                                                                    ? evse.Status.Timestamp
                                                                    : evse.LastChangeDate,
                                                                ref warnings);

                                        if (evse2 is not null)
                                        {

                                            var result2 = await CommonAPI.UpdateEVSE(location, evse2);

                                            result = result2.IsSuccess
                                                         ? WWCP.PushEVSEStatusResult.Success(Id, this, null, warnings)
                                                         : WWCP.PushEVSEStatusResult.Failed (Id, this, EVSEStatusUpdates, result2.ErrorResponse, warnings);

                                        }
                                        else
                                            result = WWCP.PushEVSEStatusResult.Failed(Id, this, EVSEStatusUpdates, "Could not convert the given EVSE!");

                                    }
                                    else
                                        result = WWCP.PushEVSEStatusResult.Failed(Id, this, EVSEStatusUpdates, "Unknown location identification!");

                                }
                                else
                                    result = WWCP.PushEVSEStatusResult.Failed(Id, this, EVSEStatusUpdates, "Invalid location identification!");

                            }
                            else
                                result = WWCP.PushEVSEStatusResult.Failed(Id, this, EVSEStatusUpdates, "The given EVSE was filtered!");

                        }
                        else
                            result = WWCP.PushEVSEStatusResult.Failed(Id, this, EVSEStatusUpdates, "The given EVSE does not exist!");

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


        #region AuthorizeStart         (LocalAuthentication, ...)

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
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<WWCP.AuthStartResult>

            AuthorizeStart(WWCP.LocalAuthentication          LocalAuthentication,
                           WWCP.ChargingLocation?            ChargingLocation      = null,
                           WWCP.ChargingProduct?             ChargingProduct       = null,
                           WWCP.ChargingSession_Id?          SessionId             = null,
                           WWCP.ChargingSession_Id?          CPOPartnerSessionId   = null,
                           WWCP.ChargingStationOperator_Id?  OperatorId            = null,

                           DateTime?                         Timestamp             = null,
                           EventTracking_Id?                 EventTrackingId       = null,
                           TimeSpan?                         RequestTimeout        = null,
                           CancellationToken                 CancellationToken     = default)

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


            if (DisableAuthorization)
                authStartResult = WWCP.AuthStartResult.AdminDown(
                                      AuthorizatorId:           Id,
                                      ISendAuthorizeStartStop:  this,
                                      SessionId:                SessionId,
                                      Description:              I18NString.Create("Authentication is disabled!"),
                                      Runtime:                  TimeSpan.Zero
                                  );

            else
            {

                var remotes = new PriorityList<RemoteParty>();
                foreach (var remote in CommonAPI.GetRemoteParties(OCPI.Role.EMSP))
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


                                                                                           var remotePartyLoggingPath = Path.Combine(ClientsLoggingPath, remoteParty.Id.ToString()) + Path.DirectorySeparatorChar;

                                                                                           if (!Directory.Exists(remotePartyLoggingPath))
                                                                                                Directory.CreateDirectory(remotePartyLoggingPath);

                                                                                           var cpoClient = new CPO.HTTP.CPO2EMSPClient(

                                                                                                               CPOAPI,
                                                                                                               remoteParty,
                                                                                                               null, // VirtualHostname
                                                                                                               null, // Description
                                                                                                               null, // HTTPLogger

                                                                                                               DisableLogging,
                                                                                                               remotePartyLoggingPath,
                                                                                                               ClientsLoggingContext,
                                                                                                               ClientsLogfileCreator,
                                                                                                               DNSClient

                                                                                                           );

                                                                                           if (cpoClient is null)
                                                                                               return new AuthorizationInfo(
                                                                                                          Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                          Info:         new DisplayText(Languages.en, $"Could not get/create a CPO client for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                                                                                                          RemoteParty:  remoteParty
                                                                                                      );

                                                                                           //ToDo: Make client debugging more flexible!
                                                                                           cpoClient.HTTPLogger.Debug("all", LogTargets.Disc);

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

                else if (authorizationInfo.Allowed == OCPI.AllowedType.ALLOWED)
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

                else if (authorizationInfo.Allowed == OCPI.AllowedType.BLOCKED)
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

                else if (authorizationInfo.Allowed == OCPI.AllowedType.EXPIRED)
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

                else if (authorizationInfo.Allowed == OCPI.AllowedType.NO_CREDIT)
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

                else if (authorizationInfo.Allowed == OCPI.AllowedType.NOT_ALLOWED)
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

        #region AuthorizeStop          (SessionId, LocalAuthentication, ...)

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
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<WWCP.AuthStopResult>

            AuthorizeStop(WWCP.ChargingSession_Id           SessionId,
                          WWCP.LocalAuthentication          LocalAuthentication,
                          WWCP.ChargingLocation?            ChargingLocation      = null,
                          WWCP.ChargingSession_Id?          CPOPartnerSessionId   = null,
                          WWCP.ChargingStationOperator_Id?  OperatorId            = null,

                          DateTime?                         Timestamp             = null,
                          EventTracking_Id?                 EventTrackingId       = null,
                          TimeSpan?                         RequestTimeout        = null,
                          CancellationToken                 CancellationToken     = default)

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

            if (DisableAuthorization)
                authStopResult = WWCP.AuthStopResult.AdminDown(
                                     AuthorizatorId:           Id,
                                     ISendAuthorizeStartStop:  this,
                                     SessionId:                SessionId,
                                     Description:              I18NString.Create("Authentication is disabled!"),
                                     Runtime:                  TimeSpan.Zero
                                 );

            else
            {

                var remotes = new PriorityList<RemoteParty>();
                foreach (var remote in CommonAPI.GetRemoteParties(OCPI.Role.EMSP))
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


                                                                                           var cpoClient = new CPO.HTTP.CPO2EMSPClient(

                                                                                                               CPOAPI,
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

                                                                                           if (cpoClient is null)
                                                                                               return new AuthorizationInfo(
                                                                                                          Allowed:      AllowedType.NOT_ALLOWED,
                                                                                                          Info:         new DisplayText(Languages.en, $"Could not get/create a CPO client for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                                                                                                          RemoteParty:  remoteParty
                                                                                                      );

                                                                                           //ToDo: Make client debugging more flexible!
                                                                                           cpoClient.HTTPLogger.Debug("all", LogTargets.Disc);

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


        public Func<WWCP.ChargeDetailRecord, CDR, CDR?>? CustomCDRMapper { get; set; }

        #region SendChargeDetailRecord (ChargeDetailRecord,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Send a charge detail record to an OCPI server.
        /// </summary>
        /// <param name="ChargeDetailRecord">A charge detail record.</param>
        /// <param name="TransmissionType">Whether to send the CDR directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<WWCP.SendCDRResult>

            SendChargeDetailRecord(WWCP.ChargeDetailRecord  ChargeDetailRecord,
                                   WWCP.TransmissionTypes   TransmissionType,

                                   DateTime?                Timestamp,
                                   EventTracking_Id?        EventTrackingId,
                                   TimeSpan?                RequestTimeout,
                                   CancellationToken        CancellationToken)

            => (await SendChargeDetailRecords(
                   [ ChargeDetailRecord ],
                   TransmissionType,

                   Timestamp,
                   EventTrackingId,
                   RequestTimeout,
                   CancellationToken
               )).First();

        #endregion

        #region SendChargeDetailRecords(ChargeDetailRecords, TransmissionType, ...)

        /// <summary>
        /// Send charge detail records to an OCPI server.
        /// </summary>
        /// <param name="ChargeDetailRecords">An enumeration of charge detail records.</param>
        /// <param name="TransmissionType">Whether to send the CDRs directly or enqueue them for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">An optional token to cancel this request.</param>
        public async Task<WWCP.SendCDRsResult>

            SendChargeDetailRecords(IEnumerable<WWCP.ChargeDetailRecord>      ChargeDetailRecords,
                                    WWCP.TransmissionTypes                    TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                                    DateTime?                                 Timestamp           = null,
                                    EventTracking_Id?                         EventTrackingId     = null,
                                    TimeSpan?                                 RequestTimeout      = null,
                                    CancellationToken                         CancellationToken   = default)

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
                    filteredCDRs.Add(
                        WWCP.SendCDRResult.Filtered(
                            org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                            Id,
                            cdr,
                            Warnings: Warnings.Create("This charge detail record was filtered!")
                        )
                    );

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
                                      I18NString.Create("Sending charge detail records is disabled!"),
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
                        var remoteAccessInfo  = remoteParty?.RemoteAccessInfos.FirstOrDefault(remoteAccessInfo => remoteAccessInfo.Status == RemoteAccessStatus.ONLINE);

                        //if (remoteAccessInfo is null)
                        //    return new AuthorizationInfo(
                        //               Allowed:      AllowedType.NOT_ALLOWED,
                        //               Info:         new DisplayText(Languages.en, $"No remote access information for '{remoteParty.BusinessDetails.Name} ({remoteParty.CountryCode}-{remoteParty.PartyId})'"),
                        //               RemoteParty:  remoteParty
                        //           );

                        //LogfileCreatorDelegate clientsLogfileCreator = (loggingPath, context, logfileName) => String.Concat(
                        //                                                                                          loggingPath,
                        //                                                                                          context is not null ? context + "_" : "",
                        //                                                                                          logfileName, "_",
                        //                                                                                          org.GraphDefined.Vanaheimr.Illias.Timestamp.Now.Year, "-",
                        //                                                                                          org.GraphDefined.Vanaheimr.Illias.Timestamp.Now.Month.ToString("D2"),
                        //                                                                                          ".log"
                        //                                                                                      );

                        var remotePartyLoggingPath = Path.Combine(ClientsLoggingPath, remoteParty.Id.ToString()) + Path.DirectorySeparatorChar;

                        if (!Directory.Exists(remotePartyLoggingPath))
                            Directory.CreateDirectory(remotePartyLoggingPath);

                        //var cpoClient = CommonAPI.GetCPOClient(remoteParty);

                        var cpoClient = new CPO.HTTP.CPO2EMSPClient(

                                            CPOAPI:            CPOAPI,
                                            RemoteParty:       remoteParty,
                                            VirtualHostname:   null,
                                            Description:       null,
                                            HTTPLogger:        null,

                                            DisableLogging:    DisableLogging,
                                            LoggingPath:       remotePartyLoggingPath,  //ClientsLoggingPath    ?? DefaultHTTPAPI_LoggingPath,
                                            LoggingContext:    ClientsLoggingContext ?? "CPOClient",
                                            LogfileCreator:    ClientsLogfileCreator,
                                            DNSClient:         DNSClient

                                        );

                        //ToDo: Make client debugging more flexible!
                        cpoClient.HTTPLogger.Debug("all", LogTargets.Disc);

                        #endregion

                        #region Convert and send charge detail record

                        var cdr = chargeDetailRecord.ToOCPI(
                                      CustomEVSEUIdConverter,
                                      CustomEVSEIdConverter,
                                      CommonAPI.GetTariffIds,
                                      EMSP_Id.Parse(chargeDetailRecord.ProviderIdStart.Value.ToString()),
                                      CommonAPI.GetTariff,
                                      ref warnings
                                  );

                        if (cdr is not null && CustomCDRMapper is not null)
                            cdr = CustomCDRMapper(chargeDetailRecord, cdr);

                        if (cdr is null)
                        {

                            endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                            runtime = endtime - startTime;
                            sendCDRResults.Add(WWCP.SendCDRResult.Error(
                                                   endtime.Value,
                                                   Id,
                                                   chargeDetailRecord,
                                                   I18NString.Create($"Converting the charge detail record to OCPI {Version.String} failed!"),
                                                   warnings,
                                                   runtime
                                               ));

                        }
                        else
                        {

                            var addOrUpdateResult   = await CommonAPI.AddOrUpdateCDR(
                                                                CDR:                cdr,
                                                                AllowDowngrades:    false,
                                                                SkipNotifications:  false,
                                                                EventTrackingId:    EventTrackingId
                                                            );

                            var response            = await cpoClient.PostCDR(
                                                                CDR:                cdr,
                                                                EMSPId:             emspId,
                                                                CancellationToken:  CancellationToken
                                                            );

                            if (response is not null)
                            {

                                endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                                runtime = endtime - startTime;

                                if (response.StatusCode == 1000)
                                    sendCDRResults.Add(
                                        WWCP.SendCDRResult.Success(
                                            endtime.Value,
                                            Id,
                                            chargeDetailRecord,
                                            null,
                                            warnings,
                                            response.HTTPLocation,
                                            runtime
                                        )
                                    );

                                else
                                    sendCDRResults.Add(
                                        WWCP.SendCDRResult.Error(
                                            endtime.Value,
                                            Id,
                                            chargeDetailRecord,
                                            I18NString.Create($"Sending the charge detail record failed: {response.StatusMessage} ({response.StatusCode})!"),
                                            warnings,
                                            runtime
                                        )
                                    );

                            }
                            else
                            {

                                endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                                runtime = endtime - startTime;
                                sendCDRResults.Add(
                                    WWCP.SendCDRResult.Error(
                                        endtime.Value,
                                        Id,
                                        chargeDetailRecord,
                                        I18NString.Create("Sending the charge detail record failed!"),
                                        warnings,
                                        runtime
                                    )
                                );

                            }

                        }

                        #endregion

                    }
                    else
                    {

                        endtime = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                        runtime = endtime - startTime;
                        sendCDRResults.Add(
                            WWCP.SendCDRResult.UnknownProviderIdStart(
                                endtime.Value,
                                Id,
                                chargeDetailRecord,
                                Runtime: runtime
                            )
                        );

                    }

                }

                endtime         = org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
                runtime         = endtime - startTime;
                sendCDRsResult  = WWCP.SendCDRResultTypesExtensions.Combine(
                                      sendCDRResults,
                                      Id,
                                      this,
                                      Warnings: warnings
                                  );

            }


            endtime         ??= org.GraphDefined.Vanaheimr.Illias.Timestamp.Now;
            runtime         ??= endtime - startTime;
            sendCDRsResult  ??= WWCP.SendCDRsResult.Error(
                                    endtime.Value,
                                    Id,
                                    this,
                                    ChargeDetailRecords,
                                    I18NString.Create("Unknown error!")
                                );


            await RoamingNetwork.ReceiveSendChargeDetailRecordResults(sendCDRsResult);


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


    }

}
