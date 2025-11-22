/*
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

using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPI;
using System.Net.Security;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// Send charging stations upstream towards an OCPI partner...
    /// </summary>
    public class OCPIEMPAdapter : AWWCPEMPAdapter,
                                  IEMPRoamingProvider,
                                  IEquatable<OCPIEMPAdapter>,
                                  IComparable<OCPIEMPAdapter>,
                                  IComparable

    {

        #region Data

        #endregion

        #region Properties

        public EVSEId_2_WWCPEVSEId_Delegate?                CustomEVSEIdConverter                { get; }
        public EVSE_2_WWCPEVSE_Delegate?                    CustomEVSEConverter                  { get; }
        public StatusType_2_WWCPEVSEStatusUpdate_Delegate?  CustomEVSEStatusUpdateConverter      { get; }
        public CDR_2_WWCPChargeDetailRecord_Delegate?       CustomChargeDetailRecordConverter    { get; }



        public CommonAPI                                    CommonAPI                            { get; }

        public EMSPAPI                                      EMSPAPI                              { get; }

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

                              CommonAPI                                         CommonAPI,

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
                              DateTimeOffset?                                   Created                             = null,
                              DateTimeOffset?                                   LastChange                          = null,

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
                   Created,
                   LastChange,

                   CustomData,
                   InternalData)

        {

            this.CommonAPI                          = CommonAPI;

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

            this.EMSPAPI                            = new EMSPAPI(
                                                          this.CommonAPI,
                                                          URLPathPrefix: HTTPPath.Parse($"{Version.String}/emsp")//CommonAPI.URLPathPrefix + Version.String + "emsp"
                                                      );

        }

        #endregion


        #region AddRemoteParty(...)

        public Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                            IEnumerable<CredentialsRole>                               CredentialsRoles,

                                            AccessToken                                                LocalAccessToken,
                                            URL                                                        RemoteVersionsURL,
                                            AccessToken                                                RemoteAccessToken,

                                            PartyStatus                                                PartyStatus                       = PartyStatus.       ENABLED,

                                            Boolean?                                                   RemoteAccessTokenBase64Encoding   = null,
                                            TOTPConfig?                                                RemoteTOTPConfig                  = null,
                                            DateTimeOffset?                                            RemoteAccessNotBefore             = null,
                                            DateTimeOffset?                                            RemoteAccessNotAfter              = null,
                                            RemoteAccessStatus?                                        RemoteStatus                      = RemoteAccessStatus.ONLINE,
                                            IEnumerable<Version_Id>?                                   RemoteVersionIds                  = null,
                                            Version_Id?                                                SelectedVersionId                 = null,
                                            Boolean?                                                   RemoteAllowDowngrades             = null,

                                            Boolean?                                                   LocalAccessTokenBase64Encoding    = null,
                                            TOTPConfig?                                                LocalTOTPConfig                   = null,
                                            DateTimeOffset?                                            LocalAccessNotBefore              = null,
                                            DateTimeOffset?                                            LocalAccessNotAfter               = null,
                                            Boolean?                                                   LocalAllowDowngrades              = false,
                                            AccessStatus?                                              LocalAccessStatus                 = AccessStatus.ALLOWED,

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

                                            EventTracking_Id?                                          EventTrackingId                   = null,
                                            User_Id?                                                   CurrentUserId                     = null,
                                            DateTimeOffset?                                            Created                           = null,
                                            DateTimeOffset?                                            LastUpdated                       = null)

            => CommonAPI.AddRemoteParty(

                   Id,
                   CredentialsRoles,

                   LocalAccessToken,
                   RemoteVersionsURL,
                   RemoteAccessToken,

                   PartyStatus,

                   RemoteAccessTokenBase64Encoding,
                   RemoteTOTPConfig,
                   RemoteAccessNotBefore,
                   RemoteAccessNotAfter,
                   RemoteStatus,
                   RemoteVersionIds,
                   SelectedVersionId,
                   RemoteAllowDowngrades,

                   LocalAccessTokenBase64Encoding,
                   LocalTOTPConfig,
                   LocalAccessNotBefore,
                   LocalAccessNotAfter,
                   LocalAllowDowngrades,
                   LocalAccessStatus,

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

                   EventTrackingId,
                   CurrentUserId,
                   Created,
                   LastUpdated

               );

        #endregion

        #region AddRemoteParty(...)

        public Task<Boolean> AddRemoteParty(RemoteParty_Id                                             Id,
                                            IEnumerable<CredentialsRole>                               CredentialsRoles,

                                            AccessToken                                                LocalAccessToken,

                                            PartyStatus?                                               Status                           = PartyStatus.ENABLED,

                                            Boolean?                                                   LocalAccessTokenBase64Encoding   = null,
                                            TOTPConfig?                                                LocalTOTPConfig                  = null,
                                            DateTimeOffset?                                            LocalAccessNotBefore             = null,
                                            DateTimeOffset?                                            LocalAccessNotAfter              = null,
                                            Boolean?                                                   LocalAllowDowngrades             = false,
                                            AccessStatus?                                              LocalAccessStatus                = AccessStatus.ALLOWED,

                                            Boolean?                                                   PreferIPv4                       = null,
                                            RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator       = null,
                                            LocalCertificateSelectionHandler?                          LocalCertificateSelector         = null,
                                            IEnumerable<X509Certificate2>?                             ClientCertificates               = null,
                                            SslStreamCertificateContext?                               ClientCertificateContext         = null,
                                            IEnumerable<X509Certificate2>?                             ClientCertificateChain           = null,
                                            SslProtocols?                                              TLSProtocols                     = null,
                                            HTTPContentType?                                           ContentType                      = null,
                                            AcceptTypes?                                               Accept                           = null,
                                            String?                                                    HTTPUserAgent                    = null,
                                            TimeSpan?                                                  RequestTimeout                   = null,
                                            TransmissionRetryDelayDelegate?                            TransmissionRetryDelay           = null,
                                            UInt16?                                                    MaxNumberOfRetries               = null,
                                            UInt32?                                                    InternalBufferSize               = null,
                                            Boolean?                                                   UseHTTPPipelining                = null,

                                            EventTracking_Id?                                          EventTrackingId                  = null,
                                            User_Id?                                                   CurrentUserId                    = null,
                                            DateTimeOffset?                                            Created                          = null,
                                            DateTimeOffset?                                            LastUpdated                      = null)

            => CommonAPI.AddRemoteParty(

                   Id,
                   CredentialsRoles,

                   LocalAccessToken,

                   Status,

                   LocalAccessTokenBase64Encoding,
                   LocalTOTPConfig,
                   LocalAccessNotBefore,
                   LocalAccessNotAfter,
                   LocalAllowDowngrades,
                   LocalAccessStatus,

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

                   EventTrackingId,
                   CurrentUserId,
                   Created,
                   LastUpdated

               );

        #endregion


        // ICSORoamingProvider methods

        #region Pull EVSE Data/-Status

        Boolean IPullEVSEData.PullEVSEData_IsDisabled { get; set; }


        Task<POIDataPull<WWCP.EVSE>> IPullEVSEData.PullEVSEData(DateTimeOffset? LastCall, GeoCoordinate? SearchCenter, Single DistanceKM, EMobilityProvider_Id? ProviderId, IEnumerable<ChargingStationOperator_Id> OperatorIdFilter, IEnumerable<Country> CountryCodeFilter, DateTimeOffset? Timestamp, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout, CancellationToken CancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<StatusPull<EVSEStatus>> IPullEVSEStatus.PullEVSEStatus(DateTimeOffset? LastCall, GeoCoordinate? SearchCenter, Single DistanceKM, EVSEStatusType? EVSEStatusFilter, EMobilityProvider_Id? ProviderId, DateTimeOffset? Timestamp, EventTracking_Id EventTrackingId, TimeSpan? RequestTimeout, CancellationToken CancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Charging Reservations

        TimeSpan IChargingReservations.MaxReservationDuration { get; set; }

        IEnumerable<ChargingReservation> IChargingReservations.ChargingReservations => throw new NotImplementedException();

        Task<ReservationResult> IChargingReservations.Reserve(ChargingLocation ChargingLocation,
                                                              ChargingReservationLevel ReservationLevel,
                                                              DateTimeOffset? StartTime,
                                                              TimeSpan? Duration,
                                                              ChargingReservation_Id? ReservationId,
                                                              ChargingReservation_Id? LinkedReservationId,
                                                              EMobilityProvider_Id? ProviderId,
                                                              RemoteAuthentication? RemoteAuthentication,
                                                              Auth_Path? AuthenticationPath,
                                                              ChargingProduct? ChargingProduct,
                                                              IEnumerable<AuthenticationToken>? AuthTokens,
                                                              IEnumerable<EMobilityAccount_Id>? eMAIds,
                                                              IEnumerable<UInt32>? PINs,
                                                              ICSORoamingProvider? CSORoamingProvider,

                                                              DateTimeOffset? RequestTimestamp,
                                                              EventTracking_Id? EventTrackingId,
                                                              TimeSpan? RequestTimeout,
                                                              CancellationToken CancellationToken)
        {
            return Task.FromResult(ReservationResult.NoOperation());
        }

        Task<CancelReservationResult> IChargingReservations.CancelReservation(ChargingReservation_Id ReservationId,
                                                                              ChargingReservationCancellationReason Reason,
                                                                              ICSORoamingProvider? CSORoamingProvider,

                                                                              DateTimeOffset? RequestTimestamp,
                                                                              EventTracking_Id? EventTrackingId,
                                                                              TimeSpan? RequestTimeout,
                                                                              CancellationToken CancellationToken)
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

        Task<RemoteStartResult> IRemoteStartStop.RemoteStart(ChargingLocation         ChargingLocation,
                                                             ChargingProduct?         ChargingProduct,
                                                             ChargingReservation_Id?  ReservationId,
                                                             ChargingSession_Id?      SessionId,
                                                             EMobilityProvider_Id?    ProviderId,
                                                             RemoteAuthentication?    RemoteAuthentication,
                                                             JObject?                 AdditionalSessionInfos,
                                                             Auth_Path?               AuthenticationPath,
                                                             ICSORoamingProvider?     CSORoamingProvider,

                                                             DateTimeOffset?          RequestTimestamp,
                                                             EventTracking_Id?        EventTrackingId,
                                                             TimeSpan?                RequestTimeout,
                                                             CancellationToken        CancellationToken)
        {
            return Task.FromResult(RemoteStartResult.NoOperation(System_Id.Local));
        }

        Task<RemoteStopResult> IRemoteStartStop.RemoteStop(ChargingSession_Id     SessionId,
                                                           ReservationHandling?   ReservationHandling,
                                                           EMobilityProvider_Id?  ProviderId,
                                                           RemoteAuthentication?  RemoteAuthentication,
                                                           Auth_Path?             AuthenticationPath,
                                                           ICSORoamingProvider?   CSORoamingProvider,

                                                           DateTimeOffset?        RequestTimestamp,
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
        ChargingSession? IChargingSessions.GetChargingSessionById(ChargingSession_Id ChargingSessionId)
        {
            return null;
        }

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

        Task<IEnumerable<ChargeDetailRecord>> IEMPRoamingProvider.GetChargeDetailRecords(DateTimeOffset               From,
                                                                                         DateTimeOffset?              To,
                                                                                         EMobilityProvider_Id?  ProviderId,

                                                                                         DateTimeOffset?              Timestamp,
                                                                                         EventTracking_Id?      EventTrackingId,
                                                                                         TimeSpan?              RequestTimeout,
                                                                                         CancellationToken      CancellationToken)
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


    }

}
