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

using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_3_0.PTP.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0.HTTP
{

    /// <summary>
    /// The HTTP API for Payment Terminal Providers.
    /// CPOs will connect to this API.
    /// </summary>
    public class PTPAPI : HTTPAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName   = "GraphDefined OCPI PTP HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort    = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix     = HTTPPath.Parse("ptp/");

        /// <summary>
        /// The default PTP API logfile name.
        /// </summary>
        public     static readonly String    DefaultLogfileName       = $"OCPI{Version.Id}_PTPAPI.log";


        protected Newtonsoft.Json.Formatting JSONFormat               = Newtonsoft.Json.Formatting.Indented;

        #endregion

        #region Properties

        /// <summary>
        /// The CommonAPI.
        /// </summary>
        public CommonAPI      CommonAPI             { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.3 does not define any behaviour for this.
        /// </summary>
        public Boolean?       AllowDowngrades       { get; }

        /// <summary>
        /// The PTP API logger.
        /// </summary>
        public PTPAPILogger?  PTPAPILogger          { get; }

        #endregion

        #region Custom JSON parsers

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Terminal>?                    CustomTerminalSerializer                      { get; set; }

        public CustomJObjectSerializerDelegate<Location>?                    CustomLocationSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<PublishToken>?                CustomPublishTokenSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?       CustomAdditionalGeoLocationSerializer         { get; set; }
        public CustomJObjectSerializerDelegate<EVSE>?                        CustomEVSESerializer                          { get; set; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?              CustomStatusScheduleSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<Connector>?                   CustomConnectorSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<Terminal>>?       CustomTerminalEnergyMeterSerializer           { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMeter<EVSE>>?           CustomEVSEEnergyMeterSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?  CustomTransparencySoftwareStatusSerializer    { get; set; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?        CustomTransparencySoftwareSerializer          { get; set; }
        public CustomJObjectSerializerDelegate<DisplayText>?                 CustomDisplayTextSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<Parking>?                     CustomParkingSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?             CustomBusinessDetailsSerializer               { get; set; }
        public CustomJObjectSerializerDelegate<Hours>?                       CustomHoursSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EVSEParking>?                 CustomEVSEParkingSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<Image>?                       CustomImageSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                   CustomEnergyMixSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<EnergySource>?                CustomEnergySourceSerializer                  { get; set; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?         CustomEnvironmentalImpactSerializer           { get; set; }


        public CustomJObjectSerializerDelegate<Tariff>?                      CustomTariffSerializer                        { get; set; }
        public CustomJObjectSerializerDelegate<Price>?                       CustomPriceSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<PriceLimit>?                  CustomPriceLimitSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<TariffElement>?               CustomTariffElementSerializer                 { get; set; }
        public CustomJObjectSerializerDelegate<PriceComponent>?              CustomPriceComponentSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<TaxAmount>?                   CustomTaxAmountSerializer                     { get; set; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?          CustomTariffRestrictionsSerializer            { get; set; }


        public CustomJObjectSerializerDelegate<Session>?                     CustomSessionSerializer                       { get; set; }
        public CustomJObjectSerializerDelegate<CDRToken>?                    CustomCDRTokenSerializer                      { get; set; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?              CustomChargingPeriodSerializer                { get; set; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                CustomCDRDimensionSerializer                  { get; set; }


        public CustomJObjectSerializerDelegate<CDR>?                         CustomCDRSerializer                           { get; set; }
        public CustomJObjectSerializerDelegate<CDRLocation>?                 CustomCDRLocationSerializer                   { get; set; }
        public CustomJObjectSerializerDelegate<SignedData>?                  CustomSignedDataSerializer                    { get; set; }
        public CustomJObjectSerializerDelegate<SignedValue>?                 CustomSignedValueSerializer                   { get; set; }


        public CustomJObjectSerializerDelegate<Token>?                       CustomTokenSerializer                         { get; set; }
        public CustomJObjectSerializerDelegate<EnergyContract>?              CustomEnergyContractSerializer                { get; set; }

        #endregion

        #region Events

        #region Terminal(s)

        #region (protected internal) GetTerminalsRequest             (Request)

        /// <summary>
        /// An event sent whenever a GET terminals request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTerminalsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET terminals request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTerminalsRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    OCPIRequest  Request)

            => OnGetTerminalsRequest.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) GetTerminalsResponse            (Response)

        /// <summary>
        /// An event sent whenever a GET terminals response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTerminalsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET terminals response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTerminalsResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     OCPIRequest   Request,
                                                     OCPIResponse  Response)

            => OnGetTerminalsResponse.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) GetTerminalRequest              (Request)

        /// <summary>
        /// An event sent whenever a GET terminal request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetTerminalRequest = new ();

        /// <summary>
        /// An event sent whenever a GET terminal request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetTerminalRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetTerminalRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) GetTerminalResponse             (Response)

        /// <summary>
        /// An event sent whenever a GET terminal response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetTerminalResponse = new ();

        /// <summary>
        /// An event sent whenever a GET terminal response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetTerminalResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetTerminalResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) PutTerminalRequest              (Request)

        /// <summary>
        /// An event sent whenever a PUT terminal request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutTerminalRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT terminal request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutTerminalRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnPutTerminalRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) PutTerminalResponse             (Response)

        /// <summary>
        /// An event sent whenever a PUT terminal response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutTerminalResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT terminal response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutTerminalResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnPutTerminalResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) PatchTerminalRequest            (Request)

        /// <summary>
        /// An event sent whenever a PATCH terminal request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPatchTerminalRequest = new ();

        /// <summary>
        /// An event sent whenever a PATCH terminal request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PatchTerminalRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnPatchTerminalRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) PatchTerminalResponse           (Response)

        /// <summary>
        /// An event sent whenever a PATCH terminal response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPatchTerminalResponse = new ();

        /// <summary>
        /// An event sent whenever a PATCH terminal response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PatchTerminalResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnPatchTerminalResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) PostTerminalsActivateRequest    (Request)

        /// <summary>
        /// An event sent whenever a PATCH terminal request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTerminalsActivateRequest = new ();

        /// <summary>
        /// An event sent whenever a PATCH terminal request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTerminalsActivateRequest(DateTime     Timestamp,
                                                             HTTPAPI      API,
                                                             OCPIRequest  Request)

            => OnPostTerminalsActivateRequest.WhenAll(Timestamp,
                                                      API ?? this,
                                                      Request);

        #endregion

        #region (protected internal) PostTerminalsActivateResponse   (Response)

        /// <summary>
        /// An event sent whenever a PATCH terminal response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostTerminalsActivateResponse = new ();

        /// <summary>
        /// An event sent whenever a PATCH terminal response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTerminalsActivateResponse(DateTime      Timestamp,
                                                              HTTPAPI       API,
                                                              OCPIRequest   Request,
                                                              OCPIResponse  Response)

            => OnPostTerminalsActivateResponse.WhenAll(Timestamp,
                                                       API ?? this,
                                                       Request,
                                                       Response);

        #endregion


        #region (protected internal) PostTerminalsDeactivateRequest  (Request)

        /// <summary>
        /// An event sent whenever a PATCH terminal request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostTerminalsDeactivateRequest = new ();

        /// <summary>
        /// An event sent whenever a PATCH terminal request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostTerminalsDeactivateRequest(DateTime     Timestamp,
                                                               HTTPAPI      API,
                                                               OCPIRequest  Request)

            => OnPostTerminalsDeactivateRequest.WhenAll(Timestamp,
                                                        API ?? this,
                                                        Request);

        #endregion

        #region (protected internal) PostTerminalsDeactivateResponse (Response)

        /// <summary>
        /// An event sent whenever a PATCH terminal response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostTerminalsDeactivateResponse = new ();

        /// <summary>
        /// An event sent whenever a PATCH terminal response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the response.</param>
        /// <param name="API">The PTP API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostTerminalsDeactivateResponse(DateTime      Timestamp,
                                                                HTTPAPI       API,
                                                                OCPIRequest   Request,
                                                                OCPIResponse  Response)

            => OnPostTerminalsDeactivateResponse.WhenAll(Timestamp,
                                                         API ?? this,
                                                         Request,
                                                         Response);

        #endregion

        #endregion


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an instance of the HTTP API for Payment Terminal Providers
        /// using the given CommonAPI.
        /// </summary>
        /// <param name="CommonAPI">The OCPI common API.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="URLPathPrefix">An optional URL path prefix.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        public PTPAPI(CommonAPI                    CommonAPI,
                      Boolean?                     AllowDowngrades           = null,

                      HTTPHostname?                HTTPHostname              = null,
                      String?                      ExternalDNSName           = "",
                      String?                      HTTPServiceName           = DefaultHTTPServiceName,
                      HTTPPath?                    BasePath                  = null,

                      HTTPPath?                    URLPathPrefix             = null,
                      JObject?                     APIVersionHashes          = null,

                      Boolean?                     DisableMaintenanceTasks   = false,
                      TimeSpan?                    MaintenanceInitialDelay   = null,
                      TimeSpan?                    MaintenanceEvery          = null,

                      Boolean?                     DisableWardenTasks        = false,
                      TimeSpan?                    WardenInitialDelay        = null,
                      TimeSpan?                    WardenCheckEvery          = null,

                      Boolean?                     IsDevelopment             = false,
                      IEnumerable<String>?         DevelopmentServers        = null,
                      Boolean?                     DisableLogging            = false,
                      String?                      LoggingContext            = null,
                      String?                      LoggingPath               = null,
                      String?                      LogfileName               = null,
                      OCPILogfileCreatorDelegate?  LogfileCreator            = null,
                      Boolean                      AutoStart                 = false)

            : base(CommonAPI.HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   BasePath,

                   URLPathPrefix   ?? DefaultURLPathPrefix,
                   null, //HTMLTemplate,
                   APIVersionHashes,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName     ?? DefaultLogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   AutoStart)

        {

            this.CommonAPI           = CommonAPI;
            this.AllowDowngrades     = AllowDowngrades;

            this.PTPAPILogger        = this.DisableLogging == false
                                           ? new PTPAPILogger(
                                                 this,
                                                 LoggingContext,
                                                 LoggingPath,
                                                 LogfileCreator
                                             )
                                           : null;

            RegisterURLTemplates();

        }

        #endregion


        #region PTP2CPO  Clients

        private readonly ConcurrentDictionary<CPO_Id, PTP2CPOClient> ptp2cpoClients = new ();

        /// <summary>
        /// Return an enumeration of all PTP2CPO clients.
        /// </summary>
        public IEnumerable<PTP2CPOClient> PTP2CPOClients
            => ptp2cpoClients.Values;


        #region GetCPOClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote PTP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote PTP.</param>
        /// <param name="PartyId">The party identification of the remote PTP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public PTP2CPOClient? GetCPOClient(CountryCode  CountryCode,
                                           Party_Id     PartyId,
                                           I18NString?  Description          = null,
                                           Boolean      AllowCachedClients   = true)
        {

            var cpoId          = CPO_Id.        Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (cpoId);

            if (AllowCachedClients &&
                ptp2cpoClients.TryGetValue(cpoId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (CommonAPI.TryGetRemoteParty(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new PTP2CPOClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(remotePartyId),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                ptp2cpoClients.TryAdd(cpoId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote PTP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public PTP2CPOClient? GetCPOClient(RemoteParty  RemoteParty,
                                           I18NString?  Description          = null,
                                           Boolean      AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                ptp2cpoClients.TryGetValue(cpoId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new PTP2CPOClient(
                                    this,
                                    RemoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                ptp2cpoClients.TryAdd(cpoId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote PTP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public PTP2CPOClient? GetCPOClient(RemoteParty_Id  RemotePartyId,
                                           I18NString?     Description          = null,
                                           Boolean         AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                ptp2cpoClients.TryGetValue(cpoId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (CommonAPI.TryGetRemoteParty(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new PTP2CPOClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? CommonAPI.BaseAPI.ClientConfigurations.Description?.Invoke(RemotePartyId),
                                    null,
                                    CommonAPI.BaseAPI.ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                    CommonAPI.BaseAPI.ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                ptp2cpoClients.TryAdd(cpoId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region ~/payments/terminals

            #region OPTIONS  ~/payments/terminals

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/terminals",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/payments/terminals

            // https://example.com/ocpi/2.3.0/ptp/payments/terminals/?date_from=2025-03-01T12:00:00&date_to=2025-03-11T12:00:00&offset=50&limit=100
            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.GET,
                URLPathPrefix + "payments/terminals",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   GetTerminalsRequest,
                OCPIResponseLogger:  GetTerminalsResponse,
                OCPIRequestHandler:  request => {

                    #region Check access token

                    if ((request.LocalAccessInfo is not null || CommonAPI.BaseAPI.LocationsAsOpenData == false) &&
                        (request.LocalAccessInfo?.Status           != AccessStatus.ALLOWED ||
                         request.LocalAccessInfo?.IsNot(Role.EMSP) == true))
                    {


                    //if (Request.LocalAccessInfo?.IsNot(Role.EMSP) == true ||
                    //    Request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    //{

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods   = [ "OPTIONS", "GET", "DELETE" ],
                                    AccessControlAllowHeaders   = [ "Authorization" ],
                                    AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                                }
                            });

                    }

                    #endregion


                    var withExtensions            = request.QueryString.GetBoolean ("withExtensions", false);

                    var filters                   = request.GetDateAndPaginationFilters();
                    var matchFilter               = request.QueryString.CreateStringFilter<Terminal>(
                                                        "match",
                                                        (paymentTerminal, pattern) => paymentTerminal.Id.                           Matches (pattern)         ||
                                                                                      paymentTerminal.CustomerReference?.ToString().Contains(pattern) == true ||
                                                                                      paymentTerminal.CountryCode?.      ToString().Contains(pattern) == true ||
                                                                                      paymentTerminal.PartyId?.          ToString().Contains(pattern) == true ||
                                                                                      paymentTerminal.Address.                      Matches (pattern)         ||
                                                                                      paymentTerminal.City.                         Matches (pattern)         ||
                                                                                      paymentTerminal.PostalCode.                   Matches (pattern)         ||
                                                                                      paymentTerminal.State.                        Matches (pattern)         ||
                                                                                      paymentTerminal.Country?.          ToString().Contains(pattern) == true ||
                                                                                      paymentTerminal.FloorLevel.                   Matches (pattern)         ||
                                                                                      paymentTerminal.PhysicalReference.            Matches (pattern)         ||
                                                                                      paymentTerminal.Coordinates?.      ToString().Matches (pattern) == true ||
                                                                                      paymentTerminal.Directions.                   Matches (pattern)         ||
                                                                                      // Capabilities
                                                                                      // PaymentMethods
                                                                                      // PaymentBrands
                                                                                      paymentTerminal.LocationIds.                  Matches (pattern)         ||
                                                                                      paymentTerminal.EVSEUIds.                     Matches (pattern)
                                                    );

                                                                           //ToDo: Filter to NOT show all payments/terminals to everyone!
                    var allPaymentTerminals       = CommonAPI.GetPaymentTerminals().//terminal => Request.AccessInfo.Value.Roles.Any(role => role.CountryCode == terminal.CountryCode &&
                                                                             //                                                       role.PartyId     == terminal.PartyId)).
                                                              ToArray();

                    var filteredPaymentTerminals  = allPaymentTerminals.Where(matchFilter).
                                                                        Where(terminal => !filters.From.HasValue || terminal.LastUpdated >  filters.From.Value).
                                                                        Where(terminal => !filters.To.  HasValue || terminal.LastUpdated <= filters.To.  Value).
                                                                        ToArray();


                    var httpResponseBuilder       = new HTTPResponse.Builder(request.HTTPRequest) {
                                                        HTTPStatusCode              = HTTPStatusCode.OK,
                                                        Server                      = DefaultHTTPServerName,
                                                        Date                        = Timestamp.Now,
                                                        AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                                        AccessControlAllowHeaders   = [ "Authorization" ],
                                                        AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                                                    }.

                                                    // The overall number of payments/terminals
                                                    Set("X-Total-Count",     allPaymentTerminals.     Length).

                                                    // The number of payments/terminals matching search filters
                                                    Set("X-Filtered-Count",  filteredPaymentTerminals.Length).

                                                    // The maximum number of payments/terminals that the server WILL return within a single request
                                                    Set("X-Limit",           allPaymentTerminals.     Length);


                    #region When the limit query parameter was set & this is not the last pagination page...

                    if (filters.Limit.HasValue &&
                        allPaymentTerminals.ULongLength() > ((filters.Offset ?? 0) + (filters.Limit ?? 0)))
                    {

                        // The new query parameters for the "next" page of pagination within the HTTP Link header
                        var queryParameters    = new List<String?>() {
                                                     filters.From. HasValue ? $"from={filters.From.Value}" :                             null,
                                                     filters.To.   HasValue ? $"to={filters.To.Value}" :                                 null,
                                                     filters.Limit.HasValue ? $"offset={(filters.Offset ?? 0) + (filters.Limit ?? 0)}" : null,
                                                     filters.Limit.HasValue ? $"limit={filters.Limit ?? 0}" :                            null
                                                 }.Where(queryParameter => queryParameter is not null).
                                                   AggregateWith("&");

                        if (queryParameters.Length > 0)
                            queryParameters = "?" + queryParameters;

                        // Link to the 'next' page should be provided when this is NOT the last page, e.g.:
                        //   - Link: <https://www.server.com/ocpi/ptp/2.3.0/payments/terminals/?offset=150&limit=50>; rel="next"
                        httpResponseBuilder.Set("Link", $"<{(ExternalDNSName.IsNotNullOrEmpty()
                                                                                     ? $"https://{ExternalDNSName}"
                                                                                     : $"http://127.0.0.1:{HTTPServer.IPPorts.First()}")}" +
                                                        $"{URLPathPrefix}/payments/terminals{queryParameters}>; rel=\"next\"");

                    }

                    #endregion


                    return Task.FromResult(
                               new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = httpResponseBuilder,
                                   Data                 = new JArray(
                                                              filteredPaymentTerminals.
                                                                  OrderBy       (paymentTerminal => paymentTerminal.Created).
                                                                  SkipTakeFilter(filters.Offset,
                                                                                 filters.Limit).
                                                                  Select        (paymentTerminal => paymentTerminal.ToJSON(true,
                                                                                                                           true,
                                                                                                                           CustomTerminalSerializer,
                                                                                                                           CustomDisplayTextSerializer,
                                                                                                                           CustomImageSerializer))
                                                          )
                               }
                           );

                });

            #endregion

            #endregion

            #region ~/payments/terminals/{terminalId}

            #region OPTIONS  ~/payments/terminals/{terminalId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/terminals/{terminalId}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/payments/terminals/{terminalId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.GET,
                URLPathPrefix + "payments/terminals/{terminalId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   GetTerminalRequest,
                OCPIResponseLogger:  GetTerminalResponse,
                OCPIRequestHandler:  request => {

                    #region Check access token

                    if (request.LocalAccessInfo?.IsNot(Role.EMSP) == true ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return Task.FromResult(
                            new OCPIResponse.Builder(request) {
                                StatusCode           = 2000,
                                StatusMessage        = "Invalid or blocked access token!",
                                HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                    HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    AccessControlAllowHeaders  = [ "Authorization" ]
                                }
                            });

                    }

                    #endregion

                    #region Check terminal

                    if (!request.ParseTerminal(CommonAPI,
                                               //Request.AccessInfo.Value.Roles.Select(role => new Tuple<CountryCode, Party_Id>(role.CountryCode, role.PartyId)),
                                               CommonAPI.OurCredentialRoles.Select(credentialRole => new Tuple<CountryCode, Party_Id>(credentialRole.CountryCode, credentialRole.PartyId)),
                                               out var terminalId,
                                               out var terminal,
                                               out var ocpiResponseBuilder,
                                               FailOnMissingTerminal: true) ||
                         terminal is null)
                    {
                        return Task.FromResult(ocpiResponseBuilder!);
                    }

                    #endregion


                    return Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               StatusCode           = 1000,
                               StatusMessage        = "Hello world!",
                               Data                 = terminal.ToJSON(true,
                                                                      true,
                                                                      CustomTerminalSerializer,
                                                                      CustomDisplayTextSerializer,
                                                                      CustomImageSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.OK,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ],
                                   LastModified               = terminal.LastUpdated,
                                   ETag                       = terminal.ETag
                               }
                        });

                });

            #endregion

            #region PUT      ~/payments/terminals/{terminalId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.PUT,
                URLPathPrefix + "payments/terminals/{terminalId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   PutTerminalRequest,
                OCPIResponseLogger:  PutTerminalResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check existing location

                    if (!request.ParseTerminal(CommonAPI,
                                               out var terminalId,
                                               out var existingTerminal,
                                               out var ocpiResponseBuilder,
                                               FailOnMissingTerminal: false))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse new or updated location JSON

                    if (!request.TryParseJObjectRequestBody(out var locationJSON, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Terminal.TryParse(locationJSON,
                                           out var newOrUpdatedTerminal,
                                           out var errorResponse,
                                           terminalId))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given location JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion


                    var addOrUpdateResult = await CommonAPI.AddOrUpdateTerminal(
                                                      newOrUpdatedTerminal,
                                                      AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade"),
                                                      false, //SkipNotifications
                                                      request.HTTPRequest.EventTrackingId,
                                                      CurrentUserId: null
                                                  );

                    if (addOrUpdateResult.IsSuccess &&
                        addOrUpdateResult.Data is not null)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   Data                 = addOrUpdateResult.Data.ToJSON(true,
                                                                                        true,
                                                                                        CustomTerminalSerializer,
                                                                                        CustomDisplayTextSerializer,
                                                                                        CustomImageSerializer),
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                                                                        ? HTTPStatusCode.Created
                                                                        : HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ],
                                       LastModified               = addOrUpdateResult.Data.LastUpdated,
                                       ETag                       = addOrUpdateResult.Data.ETag
                                   }
                               };

                    }

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = addOrUpdateResult.ErrorResponse,
                               Data                 = newOrUpdatedTerminal.ToJSON(true,
                                                                                  true,
                                                                                  CustomTerminalSerializer,
                                                                                  CustomDisplayTextSerializer,
                                                                                  CustomImageSerializer),
                        HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }

            );

            #endregion

            #region PATCH    ~/payments/terminals/{terminalId}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.PATCH,
                URLPathPrefix + "payments/terminals/{terminalId}",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   PatchTerminalRequest,
                OCPIResponseLogger:  PatchTerminalResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Check location

                    if (!request.ParseMandatoryTerminal(CommonAPI,
                                                       // out var countryCode,
                                                       // out var partyId,
                                                        out var locationId,
                                                        out var existingTerminal,
                                                        out var ocpiResponseBuilder,
                                                        FailOnMissingTerminal: true))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion

                    #region Parse Payment Terminal JSON patch

                    if (!request.TryParseJObjectRequestBody(out var paymentTerminalPatch, out ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    #endregion


                    // Validation-Checks for PATCHes
                    // (E-Tag, Timestamp, ...)

                    var patchedTerminal = await CommonAPI.TryPatchTerminal(
                                                    existingTerminal,
                                                    paymentTerminalPatch,
                                                    AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade"),
                                                    false, //SkipNotifications,
                                                    request.HTTPRequest.EventTrackingId,
                                                    CurrentUserId: null
                                                );


                    //ToDo: Handle update errors!
                    if (patchedTerminal.IsSuccess)
                        return new OCPIResponse.Builder(request) {
                                       StatusCode           = 1000,
                                       StatusMessage        = "Hello world!",
                                       Data                 = patchedTerminal.PatchedData.ToJSON(true,
                                                                                                 true,
                                                                                                 CustomTerminalSerializer,
                                                                                                 CustomDisplayTextSerializer,
                                                                                                 CustomImageSerializer),
                                       HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                           HTTPStatusCode             = HTTPStatusCode.OK,
                                           AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                           AccessControlAllowHeaders  = [ "Authorization" ],
                                           LastModified               = patchedTerminal.PatchedData.LastUpdated,
                                           ETag                       = patchedTerminal.PatchedData.ETag
                                       }
                                   };

                    return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = patchedTerminal.ErrorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "GET", "PUT", "PATCH" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                }

            );

            #endregion

            #endregion

            #region ~/payments/terminals/activate

            #region OPTIONS  ~/payments/terminals/activate

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/terminals/activate",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.POST ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/payments/terminals/activate

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.POST,
                URLPathPrefix + "payments/terminals/activate",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   PostTerminalsActivateRequest,
                OCPIResponseLogger:  PostTerminalsActivateResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Parse payment terminal JSON (will autogenerate a missing terminal id!)

                    if (!request.TryParseJObjectRequestBody(out var terminalJSON, out var ocpiResponseBuilder))
                        return ocpiResponseBuilder;

                    if (!Terminal.TryParse(terminalJSON,
                                           out var terminalToActivate,
                                           out var errorResponse,
                                           GenerateMissingTerminalId: true))
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2001,
                                   StatusMessage        = "Could not parse the given terminal JSON: " + errorResponse,
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    if (terminalToActivate.Reference.IsNullOrEmpty())
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "The payment terminal reference must not be null!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };



                    //var addOrUpdateResult = await CommonAPI.AddOrUpdateTerminal(
                    //                                  newOrUpdatedTerminal,
                    //                                  AllowDowngrades ?? request.QueryString.GetBoolean("forceDowngrade"),
                    //                                  false, //SkipNotifications
                    //                                  request.HTTPRequest.EventTrackingId,
                    //                                  CurrentUserId: null
                    //                              );

                    //if (addOrUpdateResult.IsSuccess &&
                    //    addOrUpdateResult.Data is not null)
                    //{

                    //    return new OCPIResponse.Builder(request) {
                    //               StatusCode           = 1000,
                    //               StatusMessage        = "Hello world!",
                    //               Data                 = addOrUpdateResult.Data.ToJSON(true,
                    //                                                                    true,
                    //                                                                    CustomTerminalSerializer,
                    //                                                                    CustomDisplayTextSerializer,
                    //                                                                    CustomImageSerializer),
                    //               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                    //                   HTTPStatusCode             = addOrUpdateResult.WasCreated == true
                    //                                                    ? HTTPStatusCode.Created
                    //                                                    : HTTPStatusCode.OK,
                    //                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                    //                   AccessControlAllowHeaders  = [ "Authorization" ],
                    //                   LastModified               = addOrUpdateResult.Data.LastUpdated,
                    //                   ETag                       = addOrUpdateResult.Data.ETag
                    //               }
                    //           };

                    //}

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = "!",// addOrUpdateResult.ErrorResponse,
                               //Data                 = newOrUpdatedTerminal.ToJSON(true,
                               //                                                   true,
                               //                                                   CustomTerminalSerializer,
                               //                                                   CustomDisplayTextSerializer,
                               //                                                   CustomImageSerializer),
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }

            );

            #endregion

            #endregion

            #region ~/payments/terminals/{terminalId}/deactivate

            #region OPTIONS  ~/payments/terminals/{terminalId}/deactivate

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/terminals/{terminalId}/deactivate",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.POST ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                               }
                        })

            );

            #endregion

            #region POST     ~/payments/terminals/{terminalId}/deactivate

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.POST,
                URLPathPrefix + "payments/terminals/{terminalId}/deactivate",
                HTTPContentType.Application.JSON_UTF8,
                OCPIRequestLogger:   PostTerminalsDeactivateRequest,
                OCPIResponseLogger:  PostTerminalsDeactivateResponse,
                OCPIRequestHandler:  async request => {

                    #region Check access token

                    if (request.LocalAccessInfo.IsNot(Role.CPO) ||
                        request.LocalAccessInfo?.Status != AccessStatus.ALLOWED)
                    {

                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 2000,
                                   StatusMessage        = "Invalid or blocked access token!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    }

                    #endregion

                    #region Get terminal identification

                    if (!request.ParseTerminalId(CommonAPI,
                                                 out var terminalId,
                                                 out var ocpiResponseBuilder))
                    {
                        return ocpiResponseBuilder;
                    }

                    #endregion


                    var terminalFound = false;


                    if (terminalFound)
                        return new OCPIResponse.Builder(request) {
                                   StatusCode           = 1000,
                                   StatusMessage        = "Hello world!",
                                   HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                       HTTPStatusCode             = HTTPStatusCode.OK,
                                       AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                       AccessControlAllowHeaders  = [ "Authorization" ]
                                   }
                               };

                    return new OCPIResponse.Builder(request) {
                               StatusCode           = 2000,
                               StatusMessage        = "Unknown terminal!",
                               HTTPResponseBuilder  = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                   AccessControlAllowMethods  = [ "OPTIONS", "POST" ],
                                   AccessControlAllowHeaders  = [ "Authorization" ]
                               }
                           };

                }

            );

            #endregion

            #endregion


            #region ~/payments/financial-advice-confirmations

            #region OPTIONS  ~/payments/financial-advice-confirmations

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/financial-advice-confirmations",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID", "Link", "X-Total-Count", "X-Filtered-Count" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/payments/financial-advice-confirmations

            #endregion

            #endregion

            #region ~/payments/financial-advice-confirmations/{financial_advice_confirmation_id}

            #region OPTIONS  ~/payments/financial-advice-confirmations/{financial_advice_confirmation_id}

            CommonAPI.AddOCPIMethod(

                HTTPHostname.Any,
                HTTPMethod.OPTIONS,
                URLPathPrefix + "payments/financial-advice-confirmations/{financial_advice_confirmation_id}",
                OCPIRequestHandler: request =>

                    Task.FromResult(
                        new OCPIResponse.Builder(request) {
                               HTTPResponseBuilder = new HTTPResponse.Builder(request.HTTPRequest) {
                                   HTTPStatusCode              = HTTPStatusCode.OK,
                                   Allow                       = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                   AccessControlAllowMethods   = [ "OPTIONS", "GET" ],
                                   AccessControlAllowHeaders   = [ "Authorization" ],
                                   AccessControlExposeHeaders  = [ "X-Request-ID", "X-Correlation-ID" ]
                               }
                        })

            );

            #endregion

            #region GET      ~/payments/financial-advice-confirmations/{financial_advice_confirmation_id}

            #endregion

            #endregion


        }

        #endregion


    }

}
