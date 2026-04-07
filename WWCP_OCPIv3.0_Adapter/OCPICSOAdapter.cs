/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Diagnostics;
using System.Net.Security;
using System.Security.Authentication;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

using Org.BouncyCastle.Crypto.Parameters;

using org.GraphDefined.Vanaheimr.Styx;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    public delegate IEnumerable<Tariff_Id>  GetTariffIds_Delegate(WWCP.RoamingNetwork_Id?           RoamingNetworkId,
                                                                  WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId,
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

        public CommonAPI                                            CommonAPI                            { get; }

        public CPOAPI                                               CPOAPI                               { get; }

        public GetTariffIds_Delegate?                               GetTariffIds                         { get; }

        public ChargingPoolId_2_LocationId_Delegate?                CustomChargingPoolIdConverter        { get; }
        public WWCPChargingStationId_2_ChargingStationId_Delegate?  CustomChargingStationIdConverter     { get; }
        public WWCPEVSEId_2_EVSEUId_Delegate?                       CustomEVSEUIdConverter               { get; }
        public WWCPEVSEId_2_EVSEId_Delegate?                        CustomEVSEIdConverter                { get; }
        public WWCPEVSE_2_EVSE_Delegate?                            CustomEVSEConverter                  { get; }
        public WWCPEVSEStatusUpdate_2_StatusType_Delegate?          CustomEVSEStatusUpdateConverter      { get; }
        public WWCPChargeDetailRecord_2_CDR_Delegate?               CustomChargeDetailRecordConverter    { get; }

        public new OCPILogfileCreatorDelegate?                      ClientsLogfileCreator                { get; }

        #endregion

        #region Events

        // from OCPI CSO
        //public event OnAuthorizeStartRequestDelegate?            OnAuthorizeStartRequest;
        //public event OnAuthorizeStartResponseDelegate?           OnAuthorizeStartResponse;

        //public event OnAuthorizeStopRequestDelegate?             OnAuthorizeStopRequest;
        //public event OnAuthorizeStopResponseDelegate?            OnAuthorizeStopResponse;

        public event WWCP.OnNewChargingSessionDelegate?          OnNewChargingSession;

        public event WWCP.OnChargeDetailRecordsRequestDelegate?  OnChargeDetailRecordRequest;
        public event WWCP.OnChargeDetailRecordsResponseDelegate? OnChargeDetailRecordResponse;
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
        public event WWCP.OnChargeDetailRecordsRequestDelegate?    OnEnqueueSendCDRsRequest;

        /// <summary>
        /// An event fired whenever a charge detail record will be send upstream.
        /// </summary>
        public event WWCP.OnChargeDetailRecordsRequestDelegate?    OnSendCDRsRequest;

        /// <summary>
        /// An event fired whenever a charge detail record had been sent upstream.
        /// </summary>
        public event WWCP.OnChargeDetailRecordsResponseDelegate?   OnChargeDetailRecordsResponse;
        public event WWCP.OnChargeDetailRecordsRequestDelegate? OnChargeDetailRecordsRequest;

        #endregion

        #endregion


        #region Constructor(s)

        public OCPICSOAdapter(WWCP.CSORoamingProvider_Id                      Id,
                              I18NString                                      Name,
                              I18NString                                      Description,
                              WWCP.IRoamingNetwork                            RoamingNetwork,

                              CommonAPI                                       CommonAPI,

                              GetTariffIds_Delegate?                          GetTariffIds                        = null,

                              ChargingPoolId_2_LocationId_Delegate?           CustomChargingPoolIdConverter       = null,
                              LocationId_2_ChargingPoolId_Delegate?           CustomLocationIdConverter           = null,
                              WWCPEVSEId_2_EVSEUId_Delegate?                  CustomEVSEUIdConverter              = null,
                              WWCPEVSEId_2_EVSEId_Delegate?                   CustomEVSEIdConverter               = null,
                              WWCPEVSE_2_EVSE_Delegate?                       CustomEVSEConverter                 = null,
                              EVSEId_2_WWCPEVSEId_Delegate?                   CustomEVSEUId2Converter             = null,
                              WWCPConnectorId_2_ConnectorId_Delegate?         CustomConnectorIdConverter          = null,
                              ConnectorId_2_WWCPConnectorId_Delegate?         CustomConnectorId2Converter         = null,
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

            this.CommonAPI                           = CommonAPI;

            this.GetTariffIds                        = GetTariffIds;

            if (this.GetTariffIds is not null) {

                this.CommonAPI.GetTariffIdsDelegate += (cpoPartyId,
                                                        locationId,
                                                        evseUId,
                                                        connectorId,
                                                        remotePartyId) => {

                    if (locationId. HasValue &&
                        evseUId.    HasValue &&
                        connectorId.HasValue &&
                        CommonAPI.TryGetLocation(cpoPartyId,    locationId.Value, out var location) &&
                        location. TryGetEVSE    (evseUId.Value,                   out var evse) &&
                        evse.EVSEId.HasValue)
                    {

                        return this.GetTariffIds(
                                   this.RoamingNetwork.Id,
                                   WWCP.ChargingStationOperator_Id.Parse($"{cpoPartyId}"),
                                   CustomLocationIdConverter is not null
                                       ? CustomLocationIdConverter(cpoPartyId, locationId.Value)
                                       : WWCP.ChargingPool_Id.     Parse($"{cpoPartyId}*P{locationId.Value}"),
                                   null,
                                   CustomEVSEUId2Converter is not null
                                       ? CustomEVSEUId2Converter(evseUId.Value)
                                       : WWCP.EVSE_Id.             Parse(evse.EVSEId.Value.ToString()),
                                   WWCP.ChargingConnector_Id.      Parse(connectorId.Value.ToString()),
                                   remotePartyId.HasValue
                                       ? WWCP.EMobilityProvider_Id.Parse($"{remotePartyId.Value.CountryCode}-{remotePartyId.Value.PartyId}")
                                       : null
                               );
                    }

                    return [];

                };

            }

            this.CustomEVSEUIdConverter             = CustomEVSEUIdConverter;
            this.CustomEVSEIdConverter              = CustomEVSEIdConverter;
            this.CustomEVSEConverter                = CustomEVSEConverter;
            this.CustomEVSEStatusUpdateConverter    = CustomEVSEStatusUpdateConverter;
            this.CustomChargeDetailRecordConverter  = CustomChargeDetailRecordConverter;

            this.ClientsLogfileCreator              = ClientsLogfileCreator;

            this.CPOAPI                             = new CPOAPI(

                                                          this.CommonAPI,
                                                          null, // AllowDowngrades

                                                          null,

                                                          null, // BasePath
                                                          CommonAPI.URLPathPrefix + Version.String + "cpo",

                                                          null, // ExternalDNSName
                                                          null, // HTTPServerName
                                                          null, // HTTPServiceName
                                                          null, // APIVersionHash
                                                          null, // APIVersionHashes

                                                          IsDevelopment,
                                                          DevelopmentServers,
                                                          DisableLogging,
                                                          LoggingContext,
                                                          LoggingPath,
                                                          LogfileName,
                                                          LogfileCreator
                                                          // AutoStart

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

        public Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               CredentialsRoles,

                           AccessToken                                                LocalAccessToken,

                           URL                                                        RemoteVersionsURL,
                           AccessToken                                                RemoteAccessToken,
                           Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                           TOTPConfig?                                                RemoteTOTPConfig                  = null,

                           Boolean?                                                   PreferIPv4                        = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                           SslStreamCertificateContext?                               ClientCertificateContext          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                           SslProtocols?                                              TLSProtocols                      = null,
                           HTTPContentType?                                           ContentType                       = null,
                           AcceptTypes?                                               Accept                            = null,
                           String?                                                    HTTPUserAgent                     = null,
                           TimeSpan?                                                  RequestTimeout                    = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                           UInt16?                                                    MaxNumberOfRetries                = null,
                           UInt32?                                                    InternalBufferSize                = null,
                           Boolean?                                                   UseHTTPPipelining                 = null,
                           HTTPModifiers?                                             OUT                               = null,
                           RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                           IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                           Version_Id?                                                SelectedVersionId                 = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                           TOTPConfig?                                                LocalTOTPConfig                   = null,
                           HTTPModifiers?                                             IN                                = null,
                           DateTimeOffset?                                            LocalAccessNotBefore              = null,
                           DateTimeOffset?                                            LocalAccessNotAfter               = null,
                           Boolean?                                                   LocalAllowDowngrades              = false,
                           AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                           PartyStatus?                                               PartyStatus                       = null,
                           IEnumerable<Version_Id>?                                   VisibleVersionIds                 = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)

            => CommonAPI.AddRemoteParty(

                   Id,
                   CredentialsRoles,

                   LocalAccessToken,

                   RemoteVersionsURL,
                   RemoteAccessToken,
                   RemoteAccessTokenBase64Encoding,
                   RemoteTOTPConfig,

                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCertificates,
                   ClientCertificateContext,
                   ClientCertificateChain,
                   TLSProtocols,
                   ContentType,
                   Accept,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,
                   OUT,
                   RemoteStatus,
                   RemoteVersionIds,
                   SelectedVersionId,
                   RemoteAccessNotBefore,
                   RemoteAccessNotAfter,
                   RemoteAllowDowngrades,

                   LocalAccessTokenBase64Encoding,
                   LocalTOTPConfig,
                   IN,
                   LocalAccessNotBefore,
                   LocalAccessNotAfter,
                   LocalAllowDowngrades,
                   LocalAccessStatus,

                   PartyStatus,
                   VisibleVersionIds,

                   Created,
                   LastUpdated,
                   EventTrackingId,
                   CurrentUserId

               );

        #endregion

        #region AddRemoteParty(...)

        public Task<AddResult<RemoteParty>>

            AddRemoteParty(RemoteParty_Id                                             Id,
                           IEnumerable<CredentialsRole>                               CredentialsRoles,

                           AccessToken                                                LocalAccessToken,
                           Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                           TOTPConfig?                                                LocalTOTPConfig                   = null,
                           HTTPModifiers?                                             IN                                = null,
                           DateTimeOffset?                                            LocalAccessNotBefore              = null,
                           DateTimeOffset?                                            LocalAccessNotAfter               = null,
                           Boolean?                                                   LocalAllowDowngrades              = false,
                           AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

                           Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                           TOTPConfig?                                                RemoteTOTPConfig                  = null,
                           DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                           DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                           Boolean?                                                   PreferIPv4                        = null,
                           RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator        = null,
                           LocalCertificateSelectionHandler?                          LocalCertificateSelector          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificates                = null,
                           SslStreamCertificateContext?                               ClientCertificateContext          = null,
                           IEnumerable<X509Certificate2>?                             ClientCertificateChain            = null,
                           SslProtocols?                                              TLSProtocols                      = null,
                           HTTPContentType?                                           ContentType                       = null,
                           AcceptTypes?                                               Accept                            = null,
                           String?                                                    HTTPUserAgent                     = null,
                           TimeSpan?                                                  RequestTimeout                    = null,
                           TransmissionRetryDelayDelegate?                            TransmissionRetryDelay            = null,
                           UInt16?                                                    MaxNumberOfRetries                = null,
                           UInt32?                                                    InternalBufferSize                = null,
                           Boolean?                                                   UseHTTPPipelining                 = null,
                           HTTPModifiers?                                             OUT                               = null,
                           RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                           Boolean?                                                   RemoteAllowDowngrades             = null,

                           PartyStatus?                                               Status                            = null,
                           IEnumerable<Version_Id>?                                   VisibleVersionIds                 = null,

                           DateTimeOffset?                                            Created                           = null,
                           DateTimeOffset?                                            LastUpdated                       = null,
                           EventTracking_Id?                                          EventTrackingId                   = null,
                           User_Id?                                                   CurrentUserId                     = null)

            => CommonAPI.AddRemoteParty(

                   Id,
                   CredentialsRoles,

                   LocalAccessToken,
                   LocalAccessTokenBase64Encoding,
                   LocalTOTPConfig,
                   IN,
                   LocalAccessNotBefore,
                   LocalAccessNotAfter,
                   LocalAllowDowngrades,
                   LocalAccessStatus,

                   RemoteAccessTokenBase64Encoding,
                   RemoteTOTPConfig,
                   RemoteAccessNotBefore,
                   RemoteAccessNotAfter,
                   PreferIPv4,
                   RemoteCertificateValidator,
                   LocalCertificateSelector,
                   ClientCertificates,
                   ClientCertificateContext,
                   ClientCertificateChain,
                   TLSProtocols,
                   ContentType,
                   Accept,
                   HTTPUserAgent,
                   RequestTimeout,
                   TransmissionRetryDelay,
                   MaxNumberOfRetries,
                   InternalBufferSize,
                   UseHTTPPipelining,
                   OUT,
                   RemoteStatus,
                   RemoteAllowDowngrades,

                   Status,
                   VisibleVersionIds,

                   Created,
                   LastUpdated,
                   EventTrackingId,
                   CurrentUserId

               );

        #endregion


        #region (Set/Add/Update/Delete) Charging pool(s)...

        #region AddChargingPool         (ChargingPool, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public override async Task<WWCP.AddChargingPoolResult>

            AddChargingPool(WWCP.IChargingPool      ChargingPool,
                            WWCP.TransmissionTypes  TransmissionType,

                            DateTimeOffset?         Timestamp,
                            EventTracking_Id?       EventTrackingId,
                            TimeSpan?               RequestTimeout,
                            User_Id?                CurrentUserId,
                            CancellationToken       CancellationToken)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = [];

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {

                        var location = ChargingPool.ToOCPI(
                                           CustomChargingStationIdConverter,
                                           CustomEVSEUIdConverter,
                                           CustomEVSEIdConverter,
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

        #region AddOrUpdateChargingPool (ChargingPool, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Set the given charging pool.
        /// </summary>
        /// <param name="ChargingPool">A charging pool.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public override async Task<WWCP.AddOrUpdateChargingPoolResult>

            AddOrUpdateChargingPool(WWCP.IChargingPool      ChargingPool,
                                    WWCP.TransmissionTypes  TransmissionType,

                                    DateTimeOffset?         Timestamp,
                                    EventTracking_Id?       EventTrackingId,
                                    TimeSpan?               RequestTimeout,
                                    User_Id?                CurrentUserId,
                                    CancellationToken       CancellationToken)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = [];

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {


                        var location = ChargingPool.ToOCPI(CustomChargingStationIdConverter,
                                                           CustomEVSEUIdConverter,
                                                           CustomEVSEIdConverter,
                                                           out warnings);

                        if (location is not null)
                        {

                            var result = CommonAPI.AddLocation(location);

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

        #region UpdateChargingPool      (ChargingPool, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

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
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public override async Task<WWCP.UpdateChargingPoolResult>

            UpdateChargingPool(WWCP.IChargingPool      ChargingPool,
                               String?                 PropertyName,
                               Object?                 OldValue,
                               Object?                 NewValue,
                               Context?                DataSource,
                               WWCP.TransmissionTypes  TransmissionType,

                               DateTimeOffset?         Timestamp,
                               EventTracking_Id?       EventTrackingId,
                               TimeSpan?               RequestTimeout,
                               User_Id?                CurrentUserId,
                               CancellationToken       CancellationToken)
        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    IEnumerable<Warning> warnings = [];

                    if (IncludeChargingPools is null ||
                       (IncludeChargingPools is not null && IncludeChargingPools(ChargingPool)))
                    {

                        var location = ChargingPool.ToOCPI(CustomChargingStationIdConverter,
                                                           CustomEVSEUIdConverter,
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

        #endregion

        #region (Set/Add/Update/Delete) ChargingStation(s)...

        #region AddChargingStation         (ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given ChargingStation.
        /// </summary>
        /// <param name="ChargingStation">An ChargingStation to add.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public override async Task<WWCP.AddChargingStationResult>

            AddChargingStation(WWCP.IChargingStation   ChargingStation,
                               WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                               DateTimeOffset?         Timestamp           = null,
                               EventTracking_Id?       EventTrackingId     = null,
                               TimeSpan?               RequestTimeout      = null,
                               User_Id?                CurrentUserId       = null,
                               CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    OCPI.AddOrUpdateResult<ChargingStation> result;

                    IEnumerable<Warning> warnings = [];

                    var partyId      = ChargingStation.Id.OperatorId.ToOCPI();
                    var locationId   = ChargingStation.ChargingPool is not null
                                           ? Location_Id.TryParse(ChargingStation.ChargingPool.Id.ToString())
                                           : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeChargingStations is null ||
                           (IncludeChargingStations is not null && IncludeChargingStations(ChargingStation)))
                        {

                            if (CommonAPI.TryGetLocation(partyId,
                                                         locationId.Value,
                                                         out var location) &&
                                location is not null)
                            {

                                var station2 = ChargingStation.ToOCPI(CustomChargingStationIdConverter,
                                                                      CustomEVSEUIdConverter,
                                                                      CustomEVSEIdConverter,
                                                                      //ChargingStation.Status.Timestamp > ChargingStation.LastChangeDate
                                                                      //    ? ChargingStation.Status.Timestamp
                                                                      //    : ChargingStation.LastChangeDate,
                                                                      out warnings);

                                if (station2 is not null)
                                    result = await CommonAPI.AddOrUpdateChargingStation(
                                                       location.PartyId,
                                                       location.Id,
                                                       station2
                                                   );

                                else
                                    result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Could not convert the given ChargingStation!");

                            }
                            else
                                result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "The given ChargingStation was filtered!");

                    }
                    else
                        result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Invalid location identification!");


                    return result.IsSuccess

                               ? WWCP.AddChargingStationResult.Success(
                                     ChargingStation,
                                     EventTrackingId,
                                     Id,
                                     this,
                                     Warnings: warnings
                                 )

                               : WWCP.AddChargingStationResult.Error(
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
                       ? WWCP.AddChargingStationResult.Enqueued   (ChargingStation,                     EventTrackingId, Id, this)
                       : WWCP.AddChargingStationResult.LockTimeout(ChargingStation, MaxLockWaitingTime, EventTrackingId, Id, this);

        }

        #endregion

        #region AddOrUpdateChargingStation (ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add or update the given ChargingStation.
        /// </summary>
        /// <param name="ChargingStation">An ChargingStation to add or update.</param>
        /// <param name="TransmissionType">Whether to send the ChargingStation directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public override async Task<WWCP.AddOrUpdateChargingStationResult>

            AddOrUpdateChargingStation(WWCP.IChargingStation   ChargingStation,
                                       WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                                       DateTimeOffset?         Timestamp           = null,
                                       EventTracking_Id?       EventTrackingId     = null,
                                       TimeSpan?               RequestTimeout      = null,
                                       User_Id?                CurrentUserId       = null,
                                       CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    OCPI.AddOrUpdateResult<ChargingStation> result;

                    IEnumerable<Warning> warnings = [];

                    var partyId      = ChargingStation.Id.OperatorId.ToOCPI();
                    var locationId   = ChargingStation.ChargingPool is not null
                                           ? OCPI.Location_Id.TryParse(ChargingStation.ChargingPool.Id.ToString())
                                           : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeChargingStations is null ||
                           (IncludeChargingStations is not null && IncludeChargingStations(ChargingStation)))
                        {

                            if (CommonAPI.TryGetLocation(partyId,
                                                         locationId.Value,
                                                         out var location) &&
                                location is not null)
                            {

                                var station = ChargingStation.ToOCPI(CustomChargingStationIdConverter,
                                                                     CustomEVSEUIdConverter,
                                                                     CustomEVSEIdConverter,
                                                                     //ChargingStation.Status.Timestamp > ChargingStation.LastChangeDate
                                                                     //    ? ChargingStation.Status.Timestamp
                                                                     //    : ChargingStation.LastChangeDate,
                                                                     out warnings);

                                if (station is not null)
                                    result = await CommonAPI.AddOrUpdateChargingStation(
                                                       location.PartyId,
                                                       location.Id,
                                                       station
                                                   );

                                else
                                    result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Could not convert the given ChargingStation!");

                            }
                            else
                                result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "The given ChargingStation was filtered!");

                    }
                    else
                        result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Invalid location identification!");


                    return result.IsSuccess

                               ? result.WasCreated == true

                                     ? WWCP.AddOrUpdateChargingStationResult.Added(
                                           ChargingStation,
                                           EventTrackingId,
                                           Id,
                                           this,
                                           Warnings: warnings
                                       )

                                     : WWCP.AddOrUpdateChargingStationResult.Updated(
                                           ChargingStation,
                                           EventTrackingId,
                                           Id,
                                           this,
                                           Warnings: warnings
                                       )

                               : WWCP.AddOrUpdateChargingStationResult.Error(
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
                       ? WWCP.AddOrUpdateChargingStationResult.Enqueued   (ChargingStation,                     EventTrackingId, Id, this)
                       : WWCP.AddOrUpdateChargingStationResult.LockTimeout(ChargingStation, MaxLockWaitingTime, EventTrackingId, Id, this);

        }

        #endregion

        #region UpdateChargingStation      (ChargingStation, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the ChargingStation data of the given charging pool within the static ChargingStation data at the OICP server.
        /// </summary>
        /// <param name="ChargingStation">An ChargingStation.</param>
        /// <param name="PropertyName">The name of the charging pool property to update.</param>
        /// <param name="OldValue">The old value of the charging pool property to update.</param>
        /// <param name="NewValue">The new value of the charging pool property to update.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public override async Task<WWCP.UpdateChargingStationResult>

            UpdateChargingStation(WWCP.IChargingStation   ChargingStation,
                                  String                  PropertyName,
                                  Object?                 NewValue,
                                  Object?                 OldValue,
                                  Context?                DataSource,
                                  WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                                  DateTimeOffset?         Timestamp           = null,
                                  EventTracking_Id?       EventTrackingId     = null,
                                  TimeSpan?               RequestTimeout      = null,
                                  User_Id?                CurrentUserId       = null,
                                  CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    OCPI.AddOrUpdateResult<ChargingStation> result;

                    IEnumerable<Warning> warnings = [];

                    var partyId      = ChargingStation.Id.OperatorId.ToOCPI();
                    var locationId   = ChargingStation.ChargingPool is not null
                                           ? OCPI.Location_Id.TryParse(ChargingStation.ChargingPool.Id.Suffix)
                                           : null;

                    if (locationId.HasValue)
                    {

                        if (IncludeChargingStations is null ||
                           (IncludeChargingStations is not null && IncludeChargingStations(ChargingStation)))
                        {

                            if (CommonAPI.TryGetLocation(partyId,
                                                         locationId.Value,
                                                         out var location) &&
                                location is not null)
                            {

                                var station = ChargingStation.ToOCPI(CustomChargingStationIdConverter,
                                                                     CustomEVSEUIdConverter,
                                                                     CustomEVSEIdConverter,
                                                                     //ChargingStation.Status.Timestamp > ChargingStation.LastChangeDate
                                                                     //    ? ChargingStation.Status.Timestamp
                                                                     //    : ChargingStation.LastChangeDate,
                                                                     out warnings);

                                if (station is not null)
                                    result = await CommonAPI.AddOrUpdateChargingStation(
                                                       location.PartyId,
                                                       location.Id,
                                                       station
                                                   );

                                else
                                    result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Could not convert the given ChargingStation!");

                            }
                            else
                                result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "The given ChargingStation was filtered!");

                    }
                    else
                        result = OCPI.AddOrUpdateResult<ChargingStation>.Failed(EventTrackingId, "Invalid location identification!");


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

        #region DeleteChargingStation      (ChargingStation, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Delete the ChargingStation data of the given ChargingStation from the static ChargingStation data at the OICP server.
        /// </summary>
        /// <param name="ChargingStation">An ChargingStation.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public override Task<WWCP.DeleteChargingStationResult>

            DeleteChargingStation(WWCP.IChargingStation   ChargingStation,
                                  WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                                  DateTimeOffset?         Timestamp           = null,
                                  EventTracking_Id?       EventTrackingId     = null,
                                  TimeSpan?               RequestTimeout      = null,
                                  User_Id?                CurrentUserId       = null,
                                  CancellationToken       CancellationToken   = default)

        {

            return Task.FromResult(WWCP.DeleteChargingStationResult.NoOperation(ChargingStation, EventTrackingId, Id, this));

        }

        #endregion

        #endregion

        #region (Set/Add/Update/Delete) EVSE(s)...

        #region AddEVSE         (EVSE, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Add the given EVSE.
        /// </summary>
        /// <param name="EVSE">An EVSE to add.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public override async Task<WWCP.AddEVSEResult>

            AddEVSE(WWCP.IEVSE              EVSE,
                    WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                    DateTimeOffset?         Timestamp           = null,
                    EventTracking_Id?       EventTrackingId     = null,
                    TimeSpan?               RequestTimeout      = null,
                    User_Id?                CurrentUserId       = null,
                    CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    OCPI.AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = [];

                    var partyId     = EVSE.Id.OperatorId.   ToOCPI();
                    var locationId  = EVSE.ChargingPool?.Id.ToOCPI();

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(partyId,
                                                         locationId.Value,
                                                         out var location) &&
                                location.TryGetChargingStation(EVSE.ChargingStation.Id.ToOCPI(), out var chargingStation))
                            {



                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        EVSE.Status.Timestamp > EVSE.LastChangeDate
                                                            ? EVSE.Status.Timestamp
                                                            : EVSE.LastChangeDate,
                                                        out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(chargingStation, evse2);
                                else
                                    result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Could not convert the given EVSE!");

                            }
                            else
                                result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "The given EVSE was filtered!");

                    }
                    else
                        result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Invalid location identification!");


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
        /// Add or update the given EVSE.
        /// </summary>
        /// <param name="EVSE">An EVSE to add or update.</param>
        /// <param name="TransmissionType">Whether to send the EVSE directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public override async Task<WWCP.AddOrUpdateEVSEResult>

            AddOrUpdateEVSE(WWCP.IEVSE              EVSE,
                            WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                            DateTimeOffset?         Timestamp           = null,
                            EventTracking_Id?       EventTrackingId     = null,
                            TimeSpan?               RequestTimeout      = null,
                            User_Id?                CurrentUserId       = null,
                            CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    OCPI.AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = [];

                    var partyId     = EVSE.Id.OperatorId.   ToOCPI();
                    var locationId  = EVSE.ChargingPool?.Id.ToOCPI();

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(partyId,
                                                         locationId.Value,
                                                         out var location) &&
                                location.TryGetChargingStation(EVSE.ChargingStation.Id.ToOCPI(), out var chargingStation))
                            {

                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                          CustomEVSEIdConverter,
                                                          EVSE.Status.Timestamp > EVSE.LastChangeDate
                                                              ? EVSE.Status.Timestamp
                                                              : EVSE.LastChangeDate,
                                                          out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(chargingStation, evse2);
                                else
                                    result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Could not convert the given EVSE!");

                            }
                            else
                                result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "The given EVSE was filtered!");

                    }
                    else
                        result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Invalid location identification!");


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

        #region UpdateEVSE      (EVSE, PropertyName = null, OldValue = null, NewValue = null, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the EVSE data of the given charging pool within the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        /// <param name="PropertyName">The name of the charging pool property to update.</param>
        /// <param name="OldValue">The old value of the charging pool property to update.</param>
        /// <param name="NewValue">The new value of the charging pool property to update.</param>
        /// <param name="TransmissionType">Whether to send the charging pool update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public override async Task<WWCP.UpdateEVSEResult>

            UpdateEVSE(WWCP.IEVSE              EVSE,
                       String?                 PropertyName,
                       Object?                 NewValue,
                       Object?                 OldValue,
                       Context?                DataSource,
                       WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                       DateTimeOffset?         Timestamp           = null,
                       EventTracking_Id?       EventTrackingId     = null,
                       TimeSpan?               RequestTimeout      = null,
                       User_Id?                CurrentUserId       = null,
                       CancellationToken       CancellationToken   = default)

        {

            var lockTaken = await DataAndStatusLock.WaitAsync(MaxLockWaitingTime,
                                                              CancellationToken);

            try
            {

                if (lockTaken)
                {

                    OCPI.AddOrUpdateResult<EVSE> result;

                    IEnumerable<Warning> warnings = [];

                    var partyId     = EVSE.Id.OperatorId.   ToOCPI();
                    var locationId  = EVSE.ChargingPool?.Id.ToOCPI();

                    if (locationId.HasValue)
                    {

                        if (IncludeEVSEs is null ||
                           (IncludeEVSEs is not null && IncludeEVSEs(EVSE)))
                        {

                            if (CommonAPI.TryGetLocation(partyId,
                                                         locationId.Value,
                                                         out var location) &&
                                location.TryGetChargingStation(EVSE.ChargingStation.Id.ToOCPI(), out var chargingStation))
                            {

                                var evse2 = EVSE.ToOCPI(CustomEVSEUIdConverter,
                                                        CustomEVSEIdConverter,
                                                        EVSE.Status.Timestamp > EVSE.LastChangeDate
                                                            ? EVSE.Status.Timestamp
                                                            : EVSE.LastChangeDate,
                                                        out warnings);

                                if (evse2 is not null)
                                    result = await CommonAPI.AddOrUpdateEVSE(chargingStation, evse2);
                                else
                                    result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Could not convert the given EVSE!");

                            }
                            else
                                result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Unknown location identification!");

                        }
                        else
                            result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "The given EVSE was filtered!");

                    }
                    else
                        result = OCPI.AddOrUpdateResult<EVSE>.Failed(EventTrackingId, "Invalid location identification!");


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
        /// Delete the EVSE data of the given EVSE from the static EVSE data at the OICP server.
        /// </summary>
        /// <param name="EVSE">An EVSE.</param>
        /// <param name="TransmissionType">Whether to send the charging station update directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        public override Task<WWCP.DeleteEVSEResult>

            DeleteEVSE(WWCP.IEVSE              EVSE,
                       WWCP.TransmissionTypes  TransmissionType    = WWCP.TransmissionTypes.Enqueue,

                       DateTimeOffset?         Timestamp           = null,
                       EventTracking_Id?       EventTrackingId     = null,
                       TimeSpan?               RequestTimeout      = null,
                       User_Id?                CurrentUserId       = null,
                       CancellationToken       CancellationToken   = default)

        {

            return Task.FromResult(WWCP.DeleteEVSEResult.NoOperation(EVSE, EventTrackingId, Id, this));

        }

        #endregion


        #region UpdateEVSEStatus      (EVSEStatusUpdates,       TransmissionType = Enqueue, ...)

        /// <summary>
        /// Update the given enumeration of EVSE status updates.
        /// </summary>
        /// <param name="EVSEStatusUpdates">An enumeration of EVSE status updates.</param>
        /// <param name="TransmissionType">Whether to send the EVSE status updates directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        async Task<WWCP.PushEVSEStatusResult>

            WWCP.ISendStatus.UpdateEVSEStatus(IEnumerable<WWCP.EVSEStatusUpdate>  EVSEStatusUpdates,
                                              WWCP.TransmissionTypes              TransmissionType,

                                              DateTimeOffset?                     Timestamp,
                                              EventTracking_Id?                   EventTrackingId,
                                              TimeSpan?                           RequestTimeout,
                                              User_Id?                            CurrentUserId,
                                              CancellationToken                   CancellationToken)

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

                    foreach (var statusUpdate in EVSEStatusUpdates)
                    {

                        if (RoamingNetwork.TryGetEVSEById(statusUpdate.Id, out var evse) && evse is not null)
                        {

                            if (IncludeEVSEs is null ||
                               (IncludeEVSEs is not null && IncludeEVSEs(evse)))
                            {

                                var partyId     = evse.Id.OperatorId.   ToOCPI();
                                var locationId  = evse.ChargingPool?.Id.ToOCPI();

                                if (locationId.HasValue)
                                {

                                    if (CommonAPI.TryGetLocation(partyId,
                                                                 locationId.Value,
                                                                 out var location) &&
                                        location.TryGetChargingStation(evse.ChargingStation.Id.ToOCPI(), out var chargingStation))
                                    {

                                        var evse2 = evse.ToOCPI(CustomEVSEUIdConverter,
                                                                CustomEVSEIdConverter,
                                                                evse.Status.Timestamp > evse.LastChangeDate
                                                                    ? evse.Status.Timestamp
                                                                    : evse.LastChangeDate,
                                                                ref warnings);

                                        if (evse2 is not null)
                                        {

                                            var result2 = await CommonAPI.AddOrUpdateEVSE(chargingStation, evse2);

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


        #region (private) PostToken     (LocalAuthentication, ...)

        /// <summary>
        /// Create a PostToken request at the given charging location.
        /// </summary>
        /// <param name="LocalAuthentication">A local authentication.</param>
        /// <param name="ChargingStationOperatorId">An optional charging station operator identification (from: ...).</param>
        /// <param name="EMobilityProviderId">An optional e-mobility service provider identification (to: ...).</param>
        /// 
        /// <param name="RequestTimestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        private async Task<AuthorizationInfo>

            PostToken(WWCP.LocalAuthentication          LocalAuthentication,
                      WWCP.ChargingLocation?            ChargingLocation            = null,
                    //  WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId   = null,
                      WWCP.EMobilityProvider_Id?        EMobilityProviderId         = null,

                      DateTimeOffset?                   RequestTimestamp            = null,
                      EventTracking_Id?                 EventTrackingId             = null,
                      TimeSpan?                         RequestTimeout              = null,
                      CancellationToken                 CancellationToken           = default)

        {

            #region Initial checks

            RequestTimestamp ??= Timestamp.Now;
            EventTrackingId  ??= EventTracking_Id.New;
            RequestTimeout   ??= this.RequestTimeout;

            #endregion

            #region Convert ChargingLocation          into an OCPI location reference

            var locationReference = ChargingLocation.ToOCPI(
                                        CustomChargingPoolIdConverter,
                                        CustomEVSEUIdConverter
                                    );

            if (ChargingLocation  is not null &&
                locationReference is null)
            {

                return new AuthorizationInfo(
                           Allowed:  AllowedType.NOT_ALLOWED,
                           Token:    null, //ToDo: Token should be optional within AuthorizationInfo!
                           Info:     new DisplayText(Languages.en, $"The given charging location could not be converted into an OCPI location reference!")
                       );

            }

            #endregion

            #region Convert LocalAuthentication       into an OCPI token identification

            var authToken        = LocalAuthentication.AuthToken?.ToString();

            if (authToken is null)
                return new AuthorizationInfo(
                           Allowed:  AllowedType.NOT_ALLOWED,
                           Token:    null, //ToDo: Token should be optional within AuthorizationInfo!
                           Info:     new DisplayText(Languages.en, $"The local authentication must not be null!")
                       );


            var tokenId = Token_Id.TryParse(authToken);

            if (!tokenId.HasValue)
                return new AuthorizationInfo(
                           Allowed:  AllowedType.NOT_ALLOWED,
                           Token:    null, //ToDo: Token should be optional within AuthorizationInfo!
                           Info:     new DisplayText(Languages.en, $"The token identification is invalid!")
                       );

            #endregion

            #region Convert ChargingStationOperatorId into an OCPI party identification

            var fromChargingStationOperatorId = //ChargingStationOperatorId?.ToOCPI_PartyIdv3() ??
                                                ChargingLocation?.ChargingStationOperatorId?.ToOCPI_PartyIdv3();

            if (ChargingLocation?.ChargingStationOperatorId.HasValue == true &&
               !fromChargingStationOperatorId.HasValue)
            {

                return new AuthorizationInfo(
                           Allowed:  AllowedType.NOT_ALLOWED,
                           Token:    null, //ToDo: Token should be optional within AuthorizationInfo!
                           Info:     new DisplayText(Languages.en, $"The given charging station operator identification could not be converted into an OCPI party identification!")
                       );

            }

            #endregion

            #region Convert EMobilityProviderId       into an OCPI party identification

            var toEMobilityProviderId = EMobilityProviderId?.ToOCPI_PartyIdv3();

            if (EMobilityProviderId.  HasValue &&
               !toEMobilityProviderId.HasValue)
            {

                return new AuthorizationInfo(
                           Allowed:  AllowedType.NOT_ALLOWED,
                           Token:    null, //ToDo: Token should be optional within AuthorizationInfo!
                           Info:     new DisplayText(Languages.en, $"The given e-mobility provider identification could not be converted into an OCPI party identification!")
                       );

            }

            #endregion


            #region Get all remote parties with online access

            var remotes = new PriorityList<RemoteParty>();
            foreach (var remoteParty in CommonAPI.GetRemoteParties(Role.EMSP, Role.HUB))
            {
                if (remoteParty.Status == PartyStatus.ENABLED)
                {

                    var remoteAccessInfo = remoteParty.RemoteAccessInfos.FirstOrDefault(remoteAccessInfo => remoteAccessInfo.Status == RemoteAccessStatus.ONLINE);

                    if ( remoteAccessInfo is not null)
                        remotes.Add(remoteParty);

                }
            }

            #endregion


            return await remotes.WhenFirst(

                             Work:                   async (remoteParty, cancellationToken) => {

                                                          OCPIResponse<AuthorizationInfo>? authorizationInfo = null;

                                                          #region Setup HTTP client for EMSP

                                                          if (remoteParty.Id.Role == Role.EMSP)
                                                          {

                                                              //var cpoClient = new CPO.HTTP.CPO2EMSP_HTTPClient(

                                                              //                    CPO_HTTPAPI,
                                                              //                    remoteParty,
                                                              //                    null, // VirtualHostname
                                                              //                    null, // Description
                                                              //                    null, // HTTPLogger

                                                              //                    DisableLogging,
                                                              //                    ClientsLoggingPath ?? DefaultHTTPAPI_LoggingPath,
                                                              //                    ClientsLoggingContext ?? DefaultLoggingContext,
                                                              //                    ClientsLogfileCreator,
                                                              //                    DNSClient

                                                              //                );

                                                              //if (cpoClient is null)
                                                              //    return new AuthorizationInfo(
                                                              //               Allowed:      AllowedType.NOT_ALLOWED,
                                                              //               Token:        null,
                                                              //               Info:         new DisplayText(Languages.en, $"Could not get/create a CPO client for '{remoteParty.Id})'"),
                                                              //               RemoteParty:  remoteParty
                                                              //           );


                                                              //var cpoClientLogger  = new CPO.HTTP.CPO2EMSP_HTTPClient.HTTPClientLogger(
                                                              //                           cpoClient,
                                                              //                           ClientsLoggingPath    ?? DefaultHTTPAPI_LoggingPath,
                                                              //                           ClientsLoggingContext ?? DefaultLoggingContext,
                                                              //                           ClientsLogfileCreator
                                                              //                       );

                                                              //authorizationInfo    = await cpoClient.PostToken(
                                                              //                                 TokenId:             tokenId.Value,
                                                              //                                 TokenType:           TokenType.RFID,
                                                              //                                 LocationReference:   locationReference,
                                                              //                                 From:                fromChargingStationOperatorId,
                                                              //                                 To:                  toEMobilityProviderId,
                                                              //                                 CancellationToken:   cancellationToken
                                                              //                             );

                                                          }

                                                          #endregion

                                                          #region Setup HTTP client for HUB

                                                          if (remoteParty.Id.Role == Role.HUB)
                                                          {

                                                              //var cpoClient = new CPO.HUB.HTTP.CPO2HUB_HTTPClient(

                                                              //                    CPO_HTTPAPI,
                                                              //                    remoteParty,
                                                              //                    null, // VirtualHostname
                                                              //                    null, // Description
                                                              //                    null, // HTTPLogger

                                                              //                    DisableLogging,
                                                              //                    ClientsLoggingPath ?? DefaultHTTPAPI_LoggingPath,
                                                              //                    ClientsLoggingContext ?? DefaultLoggingContext,
                                                              //                    ClientsLogfileCreator,
                                                              //                    DNSClient

                                                              //                );

                                                              //if (cpoClient is null)
                                                              //    return new AuthorizationInfo(
                                                              //               Allowed:      AllowedType.NOT_ALLOWED,
                                                              //               Token:        null,
                                                              //               Info:         new DisplayText(Languages.en, $"Could not get/create a CPO client for '{remoteParty.Id})'"),
                                                              //               RemoteParty:  remoteParty
                                                              //           );


                                                              //var cpoClientLogger  = new CPO.HUB.HTTP.CPO2HUB_HTTPClient.HTTPClientLogger(
                                                              //                           cpoClient,
                                                              //                           ClientsLoggingPath    ?? DefaultHTTPAPI_LoggingPath,
                                                              //                           ClientsLoggingContext ?? DefaultLoggingContext,
                                                              //                           ClientsLogfileCreator
                                                              //                       );

                                                              //authorizationInfo    = await cpoClient.PostToken(
                                                              //                                 TokenId:             tokenId.Value,
                                                              //                                 TokenType:           TokenType.RFID,
                                                              //                                 LocationReference:   locationReference,
                                                              //                                 From:                fromChargingStationOperatorId,
                                                              //                                 To:                  remoteParty.Id.AsPartyId(),
                                                              //                                 CancellationToken:   cancellationToken
                                                              //                             );

                                                          }

                                                          #endregion

                                                          return authorizationInfo?.Data is not null

                                                                     ? new AuthorizationInfo(
                                                                           authorizationInfo.Data.Allowed,
                                                                           authorizationInfo.Data.Token,
                                                                           authorizationInfo.Data.Location,
                                                                           authorizationInfo.Data.AuthorizationReference,
                                                                           authorizationInfo.Data.Info,
                                                                           remoteParty,
                                                                           authorizationInfo.Data.Runtime
                                                                       )

                                                                     : new AuthorizationInfo(
                                                                           Allowed:      AllowedType.NOT_ALLOWED,
                                                                           Token:        null, //ToDo: Token should be optional within AuthorizationInfo!
                                                                           Info:         new DisplayText(Languages.en, authorizationInfo?.StatusMessage ?? $"No valid response from '{remoteParty.Id}'!"),
                                                                           RemoteParty:  remoteParty
                                                                       );

                                                     },

                             VerifyResult:           result  => result.Allowed == AllowedType.ALLOWED,

                             Timeout:                RequestTimeout ?? TimeSpan.FromSeconds(10),

                             ExceptionHandler:       null,

                             DefaultResult:          runtime  => new AuthorizationInfo(
                                                                     Allowed:   AllowedType.NOT_ALLOWED,
                                                                     Token:     null, //ToDo: Token should be optional within AuthorizationInfo!
                                                                     Location:  null,
                                                                     Info:      new DisplayText(Languages.en, $"No authorization service ({remotes.Select(remote => remote.Id.ToString()).AggregateWith(", ")}) returned a positiv result!"),
                                                                     Runtime:   runtime
                                                                 ),

                             ExternalCancellation:   CancellationToken

                         ).ConfigureAwait(false);

        }

        #endregion

        #region Authorize               (LocalAuthentication, ...)

        /// <summary>
        /// Create an authorize start request at the given charging location.
        /// </summary>
        /// <param name="LocalAuthentication">A local authentication.</param>
        /// <param name="ChargingLocation">The charging location.</param>
        /// <param name="ChargingProduct">An optional charging product.</param>
        /// <param name="SessionId">An optional session identification.</param>
        /// <param name="CPOPartnerSessionId">An optional session identification of the CPO.</param>
        /// <param name="ChargingStationOperatorId">An optional charging station operator identification.</param>
        /// 
        /// <param name="RequestTimestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<WWCP.AuthStartResult>

            Authorize(WWCP.LocalAuthentication          LocalAuthentication,
                      WWCP.ChargingLocation?            ChargingLocation            = null,
                      WWCP.ChargingProduct?             ChargingProduct             = null,
                      WWCP.ChargingSession_Id?          SessionId                   = null,
                      WWCP.ChargingSession_Id?          CPOPartnerSessionId         = null,
                      //WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId   = null,
                      WWCP.EMobilityProvider_Id?        EMobilityProviderId         = null,

                      DateTimeOffset?                   RequestTimestamp            = null,
                      EventTracking_Id?                 EventTrackingId             = null,
                      TimeSpan?                         RequestTimeout              = null,
                      CancellationToken                 CancellationToken           = default)

        {

            #region Initial checks

            EventTrackingId ??= EventTracking_Id.New;
            RequestTimeout  ??= this.RequestTimeout;

            #endregion

            #region Send OnAuthorizeStartRequest event

            var startTime  = Timestamp.Now;
            var stopwatch  = Stopwatch.StartNew();

            RequestTimestamp ??= startTime;

            await LogEvent(
                      OnAuthorizeStartRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          RequestTimestamp.Value,
                          this,
                          Id.ToString(),
                          EventTrackingId,
                          RoamingNetwork.Id,
                          null,
                          Id,
                          //ChargingStationOperatorId,
                          LocalAuthentication,
                          EMobilityProviderId,
                          ChargingLocation,
                          ChargingProduct,
                          SessionId,
                          CPOPartnerSessionId,
                          [],
                          RequestTimeout,
                          CancellationToken
                      )
                  );

            #endregion


            if (DisableAuthorization)
                return WWCP.AuthStartResult.AdminDown(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           Description:               I18NString.Create("Authentication is disabled!"),
                           Runtime:                   TimeSpan.Zero
                       );

            var authorizationInfo = await PostToken(

                                              LocalAuthentication,
                                              ChargingLocation,

                                              //ChargingStationOperatorId,
                                              EMobilityProviderId,

                                              RequestTimestamp,
                                              EventTrackingId,
                                              RequestTimeout,
                                              CancellationToken

                                          ).ConfigureAwait(false);


            WWCP.AuthStartResult? authStartResult = null;


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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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


            authStartResult ??= WWCP.AuthStartResult.Error(
                                    Id,
                                    this,
                                    SessionId
                                );


            #region Send OnAuthorizeStartResponse event

            var endTime = Timestamp.Now;
            stopwatch.Stop();

            await LogEvent(
                      OnAuthorizeStartResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endTime,
                          RequestTimestamp.Value,
                          this,
                          Id.ToString(),
                          EventTrackingId,
                          RoamingNetwork.Id,
                          null,
                          Id,
                          //ChargingStationOperatorId,
                          LocalAuthentication,
                          EMobilityProviderId,
                          ChargingLocation,
                          ChargingProduct,
                          SessionId,
                          CPOPartnerSessionId,
                          [],
                          RequestTimeout,
                          authStartResult,
                          stopwatch.Elapsed,
                          CancellationToken
                      )
                  );

            #endregion

            return authStartResult;

        }

        #endregion

        #region AuthorizeStart          (LocalAuthentication, ...)

        /// <summary>
        /// Create an authorize start request at the given charging location.
        /// </summary>
        /// <param name="LocalAuthentication">A local authentication.</param>
        /// <param name="ChargingLocation">The charging location.</param>
        /// <param name="ChargingProduct">An optional charging product.</param>
        /// <param name="SessionId">An optional session identification.</param>
        /// <param name="CPOPartnerSessionId">An optional session identification of the CPO.</param>
        /// <param name="ChargingStationOperatorId">An optional charging station operator identification.</param>
        /// 
        /// <param name="RequestTimestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<WWCP.AuthStartResult>

            AuthorizeStart(WWCP.LocalAuthentication          LocalAuthentication,
                           WWCP.ChargingLocation?            ChargingLocation            = null,
                           WWCP.ChargingProduct?             ChargingProduct             = null,
                           WWCP.ChargingSession_Id?          SessionId                   = null,
                           WWCP.ChargingSession_Id?          CPOPartnerSessionId         = null,
                           //WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId   = null,
                           WWCP.EMobilityProvider_Id?        EMobilityProviderId         = null,

                           DateTimeOffset?                   RequestTimestamp            = null,
                           EventTracking_Id?                 EventTrackingId             = null,
                           TimeSpan?                         RequestTimeout              = null,
                           CancellationToken                 CancellationToken           = default)

        {

            #region Initial checks

            EventTrackingId ??= EventTracking_Id.New;
            RequestTimeout  ??= this.RequestTimeout;

            #endregion

            #region Send OnAuthorizeStartRequest event

            var startTime  = Timestamp.Now;
            var stopwatch  = Stopwatch.StartNew();

            RequestTimestamp ??= startTime;

            await LogEvent(
                      OnAuthorizeStartRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          RequestTimestamp.Value,
                          this,
                          Id.ToString(),
                          EventTrackingId,
                          RoamingNetwork.Id,
                          null,
                          Id,
                          //ChargingStationOperatorId,
                          LocalAuthentication,
                          EMobilityProviderId,
                          ChargingLocation,
                          ChargingProduct,
                          SessionId,
                          CPOPartnerSessionId,
                          [],
                          RequestTimeout,
                          CancellationToken
                      )
                  );

            #endregion


            if (DisableAuthorization)
                return WWCP.AuthStartResult.AdminDown(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           Description:               I18NString.Create("Authentication is disabled!"),
                           Runtime:                   TimeSpan.Zero
                       );


            var authorizationInfo = await PostToken(

                                              LocalAuthentication,
                                              ChargingLocation,

                                              //ChargingStationOperatorId,
                                              EMobilityProviderId,

                                              RequestTimestamp,
                                              EventTrackingId,
                                              RequestTimeout,
                                              CancellationToken

                                          ).ConfigureAwait(false);


            WWCP.AuthStartResult? authStartResult = null;


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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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

            var endTime = Timestamp.Now;
            stopwatch.Stop();

            await LogEvent(
                      OnAuthorizeStartResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endTime,
                          RequestTimestamp.Value,
                          this,
                          Id.ToString(),
                          EventTrackingId,
                          RoamingNetwork.Id,
                          null,
                          Id,
                          //ChargingStationOperatorId,
                          LocalAuthentication,
                          EMobilityProviderId,
                          ChargingLocation,
                          ChargingProduct,
                          SessionId,
                          CPOPartnerSessionId,
                          [],
                          RequestTimeout,
                          authStartResult,
                          stopwatch.Elapsed,
                          CancellationToken
                      )
                  );

            #endregion

            return authStartResult;

        }

        #endregion

        #region AuthorizeStop           (SessionId, LocalAuthentication, ...)

        /// <summary>
        /// Create an authorize stop request at the given charging location.
        /// </summary>
        /// <param name="SessionId">The session identification from the AuthorizeStart request.</param>
        /// <param name="LocalAuthentication">A local user identification.</param>
        /// <param name="ChargingLocation">The charging location.</param>
        /// <param name="CPOPartnerSessionId">An optional session identification of the CPO.</param>
        /// <param name="ChargingStationOperatorId">An optional charging station operator identification.</param>
        /// 
        /// <param name="RequestTimestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        public async Task<WWCP.AuthStopResult>

            AuthorizeStop(WWCP.ChargingSession_Id           SessionId,
                          WWCP.LocalAuthentication          LocalAuthentication,
                          WWCP.ChargingLocation?            ChargingLocation            = null,
                          WWCP.ChargingSession_Id?          CPOPartnerSessionId         = null,
                          //WWCP.ChargingStationOperator_Id?  ChargingStationOperatorId   = null,
                          WWCP.EMobilityProvider_Id?        EMobilityProviderId         = null,

                          DateTimeOffset?                   RequestTimestamp            = null,
                          EventTracking_Id?                 EventTrackingId             = null,
                          TimeSpan?                         RequestTimeout              = null,
                          CancellationToken                 CancellationToken           = default)

        {

            #region Initial checks

            EventTrackingId ??= EventTracking_Id.New;
            RequestTimeout  ??= this.RequestTimeout;

            #endregion

            #region Send OnAuthorizeStopRequest event

            var startTime  = Timestamp.Now;
            var stopwatch  = Stopwatch.StartNew();

            RequestTimestamp ??= startTime;

            await LogEvent(
                      OnAuthorizeStopRequest,
                      loggingDelegate => loggingDelegate.Invoke(
                          startTime,
                          RequestTimestamp.Value,
                          this,
                          Id.ToString(),
                          EventTrackingId,
                          RoamingNetwork.Id,
                          null,
                          Id,
                          //ChargingStationOperatorId,
                          ChargingLocation,
                          SessionId,
                          CPOPartnerSessionId,
                          LocalAuthentication,
                          EMobilityProviderId,
                          RequestTimeout,
                          CancellationToken
                      )
                  );

            #endregion


            if (DisableAuthorization)
                return WWCP.AuthStopResult.AdminDown(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           Description:               I18NString.Create("Authentication is disabled!"),
                           Runtime:                   TimeSpan.Zero
                       );


            var authorizationInfo = await PostToken(

                                              LocalAuthentication,
                                              ChargingLocation,

                                              //ChargingStationOperatorId,
                                              EMobilityProviderId,

                                              RequestTimestamp,
                                              EventTrackingId,
                                              RequestTimeout,
                                              CancellationToken

                                          ).ConfigureAwait(false);


            WWCP.AuthStopResult? authStopResult = null;


            if (authorizationInfo is null)
                authStopResult = WWCP.AuthStopResult.CommunicationTimeout(Id, this, SessionId);

            else if (authorizationInfo.Allowed == AllowedType.ALLOWED)
                authStopResult = WWCP.AuthStopResult.Authorized(
                           AuthorizatorId:            Id,
                           ISendAuthorizeStartStop:   this,
                           SessionId:                 SessionId,
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
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
                           ProviderId:                authorizationInfo.RemoteParty?.Id.ToEMSPId().ToWWCP(),
                           Description:               null,
                           AdditionalInfo:            null,
                           NumberOfRetries:           0,
                           Runtime:                   null
                       );

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


            authStopResult ??= WWCP.AuthStopResult.Error(
                                   Id,
                                   this,
                                   SessionId
                               );


            #region Send OnAuthorizeStartResponse event

            var endTime = Timestamp.Now;
            stopwatch.Stop();

            await LogEvent(
                      OnAuthorizeStopResponse,
                      loggingDelegate => loggingDelegate.Invoke(
                          endTime,
                          RequestTimestamp.Value,
                          this,
                          Id.ToString(),
                          EventTrackingId,
                          RoamingNetwork.Id,
                          null,
                          Id,
                          //ChargingStationOperatorId,
                          ChargingLocation,
                          SessionId,
                          CPOPartnerSessionId,
                          LocalAuthentication,
                          EMobilityProviderId,
                          RequestTimeout,
                          authStopResult,
                          stopwatch.Elapsed,
                          CancellationToken
                      )
                  );

            #endregion

            return authStopResult;

        }

        #endregion



        #region SendChargeDetailRecord (ChargeDetailRecord,  TransmissionType = Enqueue, ...)

        /// <summary>
        /// Send a charge detail record to an OCHP server.
        /// </summary>
        /// <param name="ChargeDetailRecord">A charge detail record.</param>
        /// <param name="TransmissionType">Whether to send the CDR directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        async Task<WWCP.SendCDRResult>

            WWCP.ISendChargeDetailRecords.SendChargeDetailRecord(WWCP.ChargeDetailRecord  ChargeDetailRecord,
                                                                 WWCP.TransmissionTypes   TransmissionType,

                                                                 DateTimeOffset?          Timestamp,
                                                                 EventTracking_Id?        EventTrackingId,
                                                                 TimeSpan?                RequestTimeout,
                                                                 CancellationToken        CancellationToken)

            => (await (this as WWCP.ISendChargeDetailRecords).SendChargeDetailRecords(
                      [ ChargeDetailRecord ],
                      TransmissionType,
                      Timestamp,
                      EventTrackingId,
                      RequestTimeout,
                      CancellationToken)).First();

        #endregion

        #region SendChargeDetailRecords(ChargeDetailRecords, TransmissionType = Enqueue, ...)

        /// <summary>
        /// Send charge detail records to an OICP server.
        /// </summary>
        /// <param name="ChargeDetailRecords">An enumeration of charge detail records.</param>
        /// <param name="TransmissionType">Whether to send the CDR directly or enqueue it for a while.</param>
        /// 
        /// <param name="Timestamp">The optional timestamp of the request.</param>
        /// <param name="EventTrackingId">An optional event tracking identification for correlating this request with other events.</param>
        /// <param name="RequestTimeout">An optional timeout for this request.</param>
        /// <param name="CancellationToken">A cancellation token to cancel the operation.</param>
        async Task<WWCP.SendCDRsResult>

            WWCP.ISendChargeDetailRecords.SendChargeDetailRecords(IEnumerable<WWCP.ChargeDetailRecord>  ChargeDetailRecords,
                                                                  WWCP.TransmissionTypes                TransmissionType,

                                                                  DateTimeOffset?                       Timestamp,
                                                                  EventTracking_Id?                     EventTrackingId,
                                                                  TimeSpan?                             RequestTimeout,
                                                                  CancellationToken                     CancellationToken)

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
                                          RequestTimeout,
                                          CancellationToken);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(OCPICSOAdapter) + "." + nameof(OnSendCDRsRequest));
            }

            #endregion


            #region if disabled => 'AdminDown'...

            DateTimeOffset        endtime;
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


            return WWCP.SendCDRsResult.NoOperation(
                       org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                       Id,
                       this,
                       ChargeDetailRecords
                   );

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


        #region (private) LogEvent (Logger, LogHandler, ...)

        private Task LogEvent<TDelegate>(TDelegate?                                         Logger,
                                         Func<TDelegate, Task>                              LogHandler,
                                         [CallerArgumentExpression(nameof(Logger))] String  EventName     = "",
                                         [CallerMemberName()]                       String  OICPCommand   = "")

            where TDelegate : Delegate

            => LogEvent(
                   nameof(OCPICSOAdapter),
                   Logger,
                   LogHandler,
                   EventName,
                   OICPCommand
               );

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
