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

using System.Collections.Concurrent;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv2_1_1.CPO.HTTP;
using cloud.charging.open.protocols.OCPIv2_1_1.EMSP.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1.HTTP
{

    /// <summary>
    /// A delegate for filtering remote parties.
    /// </summary>
    public delegate Boolean IncludeRemoteParty(RemoteParty RemoteParty);

    public delegate IEnumerable<Tariff>     GetTariffs2_Delegate  (CountryCode    CPOCountryCode,
                                                                   Party_Id       CPOPartyId,
                                                                   Location_Id?   LocationId       = null,
                                                                   EVSE_Id?       EVSEId           = null,
                                                                   Connector_Id?  ConnectorId      = null,
                                                                   EMSP_Id?       EMSPId           = null);


    public delegate IEnumerable<Tariff_Id>  GetTariffIds2_Delegate(CountryCode    CPOCountryCode,
                                                                   Party_Id       CPOPartyId,
                                                                   Location_Id?   LocationId       = null,
                                                                   EVSE_Id?       EVSEId           = null,
                                                                   Connector_Id?  ConnectorId      = null,
                                                                   EMSP_Id?       EMSPId           = null);

    public delegate Tariff?                 GetTariff2_Delegate   (Tariff_Id      TariffId,
                                                                   DateTime?      StartTimestamp   = null,
                                                                   TimeSpan?      EVSEUId          = null);


    /// <summary>
    /// The Common API.
    /// </summary>
    public class CommonAPI : CommonAPIBase
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServerName     = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String    DefaultHTTPServiceName    = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.
        /// </summary>
        public new static readonly IPPort    DefaultHTTPServerPort     = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.
        /// </summary>
        public new static readonly HTTPPath  DefaultURLPathPrefix      = HTTPPath.Parse("io/OCPI/");

        /// <summary>
        /// The default log file name.
        /// </summary>
        public static readonly     String    DefaultLogfileName        = $"OCPI{Version.Id}-CommonAPI.log";

        /// <summary>
        /// The command values store.
        /// </summary>
        public readonly ConcurrentDictionary<Command_Id, CommandValues> CommandValueStore = new ();

        #endregion

        #region Properties

        /// <summary>
        /// Business details of this party.
        /// </summary>
        [Mandatory]
        public BusinessDetails          OurBusinessDetails         { get; }

        /// <summary>
        /// ISO-3166 alpha-2 country code of the country this party is operating in.
        /// </summary>
        [Mandatory]
        public CountryCode              OurCountryCode             { get; }

        /// <summary>
        /// CPO, eMSP (or other role) ID of this party (following the ISO-15118 standard).
        /// </summary>
        [Mandatory]
        public Party_Id                 OurPartyId                 { get; }

        /// <summary>
        /// Our business role.
        /// </summary>
        [Mandatory]
        public Roles                    OurRole                    { get; }


        /// <summary>
        /// Whether to keep or delete EVSEs marked as "REMOVED".
        /// </summary>
        public Func<EVSE, Boolean>      KeepRemovedEVSEs           { get; }

        /// <summary>
        /// The Common API logger.
        /// </summary>
        public CommonAPILogger?         CommonAPILogger            { get; }

        #endregion

        #region Events

        #region (protected internal) GetVersionsRequest       (Request)

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetVersionsRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   OCPIRequest  Request)

            => OnGetVersionsRequest.WhenAll(Timestamp,
                                            API ?? this,
                                            Request);

        #endregion

        #region (protected internal) GetVersionsResponse      (Response)

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetVersionsResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    OCPIRequest   Request,
                                                    OCPIResponse  Response)

            => OnGetVersionsResponse.WhenAll(Timestamp,
                                             API ?? this,
                                             Request,
                                             Response);

        #endregion


        #region (protected internal) GetVersionRequest        (Request)

        /// <summary>
        /// An event sent whenever a GET version request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetVersionRequest = new ();

        /// <summary>
        /// An event sent whenever a GET version request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetVersionRequest(DateTime     Timestamp,
                                                  HTTPAPI      API,
                                                  OCPIRequest  Request)

            => OnGetVersionRequest.WhenAll(Timestamp,
                                           API ?? this,
                                           Request);

        #endregion

        #region (protected internal) GetVersionResponse       (Response)

        /// <summary>
        /// An event sent whenever a GET version response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetVersionResponse = new ();

        /// <summary>
        /// An event sent whenever a GET version response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetVersionResponse(DateTime      Timestamp,
                                                   HTTPAPI       API,
                                                   OCPIRequest   Request,
                                                   OCPIResponse  Response)

            => OnGetVersionResponse.WhenAll(Timestamp,
                                            API ?? this,
                                            Request,
                                            Response);

        #endregion


        #region (protected internal) GetCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a GET credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnGetCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a GET credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task GetCredentialsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnGetCredentialsRequest.WhenAll(Timestamp,
                                               API ?? this,
                                               Request);

        #endregion

        #region (protected internal) GetCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a GET credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnGetCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a GET credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task GetCredentialsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnGetCredentialsResponse.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response);

        #endregion


        #region (protected internal) PostCredentialsRequest   (Request)

        /// <summary>
        /// An event sent whenever a POST credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPostCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a POST credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PostCredentialsRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       OCPIRequest  Request)

            => OnPostCredentialsRequest.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) PostCredentialsResponse  (Response)

        /// <summary>
        /// An event sent whenever a POST credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPostCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a POST credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PostCredentialsResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        OCPIRequest   Request,
                                                        OCPIResponse  Response)

            => OnPostCredentialsResponse.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion


        #region (protected internal) PutCredentialsRequest    (Request)

        /// <summary>
        /// An event sent whenever a PUT credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnPutCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a PUT credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task PutCredentialsRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      OCPIRequest  Request)

            => OnPutCredentialsRequest.WhenAll(Timestamp,
                                               API ?? this,
                                               Request) ?? Task.CompletedTask;

        #endregion

        #region (protected internal) PutCredentialsResponse   (Response)

        /// <summary>
        /// An event sent whenever a PUT credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnPutCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a PUT credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task PutCredentialsResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       OCPIRequest   Request,
                                                       OCPIResponse  Response)

            => OnPutCredentialsResponse.WhenAll(Timestamp,
                                                API ?? this,
                                                Request,
                                                Response) ?? Task.CompletedTask;

        #endregion


        #region (protected internal) DeleteCredentialsRequest (Request)

        /// <summary>
        /// An event sent whenever a DELETE credentials request was received.
        /// </summary>
        public OCPIRequestLogEvent OnDeleteCredentialsRequest = new ();

        /// <summary>
        /// An event sent whenever a DELETE credentials request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        protected internal Task DeleteCredentialsRequest(DateTime     Timestamp,
                                                         HTTPAPI      API,
                                                         OCPIRequest  Request)

            => OnDeleteCredentialsRequest.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request);

        #endregion

        #region (protected internal) DeleteCredentialsResponse(Response)

        /// <summary>
        /// An event sent whenever a DELETE credentials response was sent.
        /// </summary>
        public OCPIResponseLogEvent OnDeleteCredentialsResponse = new ();

        /// <summary>
        /// An event sent whenever a DELETE credentials response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">An OCPI request.</param>
        /// <param name="Response">An OCPI response.</param>
        protected internal Task DeleteCredentialsResponse(DateTime      Timestamp,
                                                          HTTPAPI       API,
                                                          OCPIRequest   Request,
                                                          OCPIResponse  Response)

            => OnDeleteCredentialsResponse.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request,
                                                   Response);

        #endregion

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<Tariff>?                       CustomTariffSerializer                        { get; }
        public CustomJObjectSerializerDelegate<DisplayText>?                  CustomDisplayTextSerializer                   { get; }
        public CustomJObjectSerializerDelegate<TariffElement>?                CustomTariffElementSerializer                 { get; }
        public CustomJObjectSerializerDelegate<PriceComponent>?               CustomPriceComponentSerializer                { get; }
        public CustomJObjectSerializerDelegate<TariffRestrictions>?           CustomTariffRestrictionsSerializer            { get; }
        public CustomJObjectSerializerDelegate<EnergyMix>?                    CustomEnergyMixSerializer                     { get; }
        public CustomJObjectSerializerDelegate<EnergySource>?                 CustomEnergySourceSerializer                  { get; }
        public CustomJObjectSerializerDelegate<EnvironmentalImpact>?          CustomEnvironmentalImpactSerializer           { get; }


        public CustomJObjectSerializerDelegate<Session>?                      CustomSessionSerializer                       { get; }
        public CustomJObjectSerializerDelegate<Location>?                     CustomLocationSerializer                      { get; }
        public CustomJObjectSerializerDelegate<AdditionalGeoLocation>?        CustomAdditionalGeoLocationSerializer         { get; }
        public CustomJObjectSerializerDelegate<EVSE>?                         CustomEVSESerializer                          { get; }
        public CustomJObjectSerializerDelegate<StatusSchedule>?               CustomStatusScheduleSerializer                { get; }
        public CustomJObjectSerializerDelegate<Connector>?                    CustomConnectorSerializer                     { get; }
        public CustomJObjectSerializerDelegate<EnergyMeter>?                  CustomEnergyMeterSerializer                   { get; }
        public CustomJObjectSerializerDelegate<TransparencySoftwareStatus>?   CustomTransparencySoftwareStatusSerializer    { get; }
        public CustomJObjectSerializerDelegate<TransparencySoftware>?         CustomTransparencySoftwareSerializer          { get; }
        public CustomJObjectSerializerDelegate<BusinessDetails>?              CustomBusinessDetailsSerializer               { get; }
        public CustomJObjectSerializerDelegate<Hours>?                        CustomHoursSerializer                         { get; }
        public CustomJObjectSerializerDelegate<Image>?                        CustomImageSerializer                         { get; }
        public CustomJObjectSerializerDelegate<ChargingPeriod>?               CustomChargingPeriodSerializer                { get; }
        public CustomJObjectSerializerDelegate<CDRDimension>?                 CustomCDRDimensionSerializer                  { get; }
        public CustomJObjectSerializerDelegate<CDRCostDetails>?               CustomCDRCostDetailsSerializer                { get; }


        public CustomJObjectSerializerDelegate<Token>?                        CustomTokenSerializer                         { get; }
        public CustomJObjectSerializerDelegate<TokenStatus>?                  CustomTokenStatusSerializer                   { get; }

        public CustomJObjectSerializerDelegate<LocationReference>?            CustomLocationReferenceSerializer             { get; }


        public CustomJObjectSerializerDelegate<CDR>?                          CustomCDRSerializer                           { get; }
        public CustomJObjectSerializerDelegate<SignedData>?                   CustomSignedDataSerializer                    { get; }
        public CustomJObjectSerializerDelegate<SignedValue>?                  CustomSignedValueSerializer                   { get; }

        #endregion

        #region Constructor(s)

        #region CommonAPI(HTTPServerName, ...)

        /// <summary>
        /// Create a new CommonAPI.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="OurBusinessDetails"></param>
        /// <param name="OurCountryCode"></param>
        /// <param name="OurPartyId"></param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// <param name="Disable_RootServices">Whether to disable / and /versions HTTP services.</param>
        /// 
        /// <param name="HTTPHostname">The HTTP hostname for all URLs within this API.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="HTTPServerPort">A TCP port to listen on.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServerName">The default HTTP servername, used whenever no HTTP Host-header has been given.</param>
        /// 
        /// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        /// <param name="HTTPServiceName">The name of the HTTP service.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="ServerCertificateSelector">An optional delegate to select a TLS server certificate.</param>
        /// <param name="ClientCertificateValidator">An optional delegate to verify the TLS client certificate used for authentication.</param>
        /// <param name="LocalCertificateSelector">An optional delegate to select the TLS client certificate used for authentication.</param>
        /// <param name="AllowedTLSProtocols">The TLS protocol(s) allowed for this connection.</param>
        /// 
        /// <param name="ServerThreadName">The optional name of the TCP server thread.</param>
        /// <param name="ServerThreadPriority">The optional priority of the TCP server thread.</param>
        /// <param name="ServerThreadIsBackground">Whether the TCP server thread is a background thread or not.</param>
        /// <param name="ConnectionIdBuilder">An optional delegate to build a connection identification based on IP socket information.</param>
        /// <param name="ConnectionTimeout">The TCP client timeout for all incoming client connections in seconds (default: 30 sec).</param>
        /// <param name="MaxClientConnections">The maximum number of concurrent TCP client connections (default: 4096).</param>
        /// 
        /// <param name="DisableMaintenanceTasks">Disable all maintenance tasks.</param>
        /// <param name="MaintenanceInitialDelay">The initial delay of the maintenance tasks.</param>
        /// <param name="MaintenanceEvery">The maintenance intervall.</param>
        /// 
        /// <param name="DisableWardenTasks">Disable all warden tasks.</param>
        /// <param name="WardenInitialDelay">The initial delay of the warden tasks.</param>
        /// <param name="WardenCheckEvery">The warden intervall.</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable any logging.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        /// <param name="DNSClient">The DNS client of the API.</param>
        /// <param name="AutoStart">Whether to start the API automatically.</param>
        public CommonAPI(URL                                                        OurBaseURL,
                         URL                                                        OurVersionsURL,
                         BusinessDetails                                            OurBusinessDetails,
                         CountryCode                                                OurCountryCode,
                         Party_Id                                                   OurPartyId,

                         HTTPPath?                                                  AdditionalURLPathPrefix      = null,
                         Func<EVSE, Boolean>?                                       KeepRemovedEVSEs             = null,
                         Boolean                                                    LocationsAsOpenData          = true,
                         Boolean?                                                   AllowDowngrades              = null,
                         Boolean                                                    Disable_RootServices         = true,

                         HTTPHostname?                                              HTTPHostname                 = null,
                         String?                                                    ExternalDNSName              = null,
                         IPPort?                                                    HTTPServerPort               = null,
                         HTTPPath?                                                  BasePath                     = null,
                         String?                                                    HTTPServerName               = DefaultHTTPServerName,

                         HTTPPath?                                                  URLPathPrefix                = null,
                         String?                                                    HTTPServiceName              = DefaultHTTPServiceName,
                         JObject?                                                   APIVersionHashes             = null,

                         ServerCertificateSelectorDelegate?                         ServerCertificateSelector    = null,
                         RemoteTLSClientCertificateValidationHandler<IHTTPServer>?  ClientCertificateValidator   = null,
                         LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                         SslProtocols?                                              AllowedTLSProtocols          = null,
                         Boolean?                                                   ClientCertificateRequired    = null,
                         Boolean?                                                   CheckCertificateRevocation   = null,

                         ServerThreadNameCreatorDelegate?                           ServerThreadNameCreator      = null,
                         ServerThreadPriorityDelegate?                              ServerThreadPrioritySetter   = null,
                         Boolean?                                                   ServerThreadIsBackground     = null,
                         ConnectionIdBuilder?                                       ConnectionIdBuilder          = null,
                         TimeSpan?                                                  ConnectionTimeout            = null,
                         UInt32?                                                    MaxClientConnections         = null,

                         Boolean?                                                   DisableMaintenanceTasks      = null,
                         TimeSpan?                                                  MaintenanceInitialDelay      = null,
                         TimeSpan?                                                  MaintenanceEvery             = null,

                         Boolean?                                                   DisableWardenTasks           = null,
                         TimeSpan?                                                  WardenInitialDelay           = null,
                         TimeSpan?                                                  WardenCheckEvery             = null,

                         Boolean?                                                   IsDevelopment                = null,
                         IEnumerable<String>?                                       DevelopmentServers           = null,
                         Boolean?                                                   DisableLogging               = null,
                         String?                                                    LoggingContext               = null,
                         String?                                                    LoggingPath                  = null,
                         String?                                                    LogfileName                  = null,
                         OCPILogfileCreatorDelegate?                                LogfileCreator               = null,
                         String?                                                    DatabaseFilePath             = null,
                         String?                                                    RemotePartyDBFileName        = null,
                         String?                                                    AssetsDBFileName             = null,
                         DNSClient?                                                 DNSClient                    = null,
                         Boolean                                                    AutoStart                    = false)

            : base(Version.Id,
                   OurBaseURL,
                   OurVersionsURL,

                   AdditionalURLPathPrefix,
                   LocationsAsOpenData,
                   AllowDowngrades,
                   Disable_RootServices,

                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServerPort,
                   BasePath,
                   HTTPServerName,

                   URLPathPrefix,
                   HTTPServiceName,
                   APIVersionHashes,

                   ServerCertificateSelector,
                   ClientCertificateValidator,
                   LocalCertificateSelector,
                   AllowedTLSProtocols,
                   ClientCertificateRequired,
                   CheckCertificateRevocation,

                   ServerThreadNameCreator,
                   ServerThreadPrioritySetter,
                   ServerThreadIsBackground,
                   ConnectionIdBuilder,
                   ConnectionTimeout,
                   MaxClientConnections,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingContext,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator,
                   DatabaseFilePath,
                   RemotePartyDBFileName,
                   AssetsDBFileName,
                   DNSClient,
                   AutoStart)

        {

            this.OurBusinessDetails  = OurBusinessDetails;
            this.OurCountryCode      = OurCountryCode;
            this.OurPartyId          = OurPartyId;

            this.KeepRemovedEVSEs    = KeepRemovedEVSEs ?? (evse => true);

            this.CommonAPILogger     = this.DisableLogging == false
                                           ? new CommonAPILogger(
                                                 this,
                                                 LoggingContext,
                                                 LoggingPath,
                                                 LogfileCreator
                                             )
                                           : null;

            if (!this.DisableLogging)
            {
                ReadRemotePartyDatabaseFile();
                ReadAssetsDatabaseFile();
            }

            if (!Disable_RootServices)
                RegisterURLTemplates();

        }

        #endregion

        #region CommonAPI(HTTPServer, ...)

        /// <summary>
        /// Create a new CommonAPI using the given HTTP server.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="OurBusinessDetails"></param>
        /// <param name="OurCountryCode"></param>
        /// <param name="OurPartyId"></param>
        /// 
        /// <param name="HTTPServer">A HTTP server.</param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
        /// <param name="KeepRemovedEVSEs">Whether to keep or delete EVSEs marked as "REMOVED" (default: keep).</param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// <param name="Disable_RootServices">Whether to disable / and /versions HTTP services.</param>
        /// 
        /// <param name="HTTPHostname">An optional HTTP hostname.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="HTTPServiceName">An optional name of the HTTP API service.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// 
        /// <param name="URLPathPrefix">An optional URL path prefix, used when defining URL templates.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="DisableMaintenanceTasks">Disable all maintenance tasks.</param>
        /// <param name="MaintenanceInitialDelay">The initial delay of the maintenance tasks.</param>
        /// <param name="MaintenanceEvery">The maintenance intervall.</param>
        /// 
        /// <param name="DisableWardenTasks">Disable all warden tasks.</param>
        /// <param name="WardenInitialDelay">The initial delay of the warden tasks.</param>
        /// <param name="WardenCheckEvery">The warden intervall.</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable the log file.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        /// <param name="AutoStart">Whether to start the API automatically.</param>
        public CommonAPI(URL                          OurBaseURL,
                         URL                          OurVersionsURL,
                         BusinessDetails              OurBusinessDetails,
                         CountryCode                  OurCountryCode,
                         Party_Id                     OurPartyId,
                         Roles                        OurRole,

                         HTTPServer                   HTTPServer,

                         HTTPPath?                    AdditionalURLPathPrefix   = null,
                         Func<EVSE, Boolean>?         KeepRemovedEVSEs          = null,
                         Boolean                      LocationsAsOpenData       = true,
                         Boolean?                     AllowDowngrades           = null,
                         Boolean                      Disable_RootServices      = false,

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
                         String?                      DatabaseFilePath          = null,
                         String?                      RemotePartyDBFileName     = null,
                         String?                      AssetsDBFileName          = null,
                         Boolean                      AutoStart                 = false)

            : base(Version.Id,
                   OurBaseURL,
                   OurVersionsURL,
                   HTTPServer,

                   AdditionalURLPathPrefix,
                   LocationsAsOpenData,
                   AllowDowngrades,
                   Disable_RootServices,

                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName,
                   BasePath,

                   URLPathPrefix,
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
                   LoggingContext,
                   LoggingPath,
                   LogfileName ?? DefaultLogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, remotePartyId, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   DatabaseFilePath,
                   RemotePartyDBFileName,
                   AssetsDBFileName,
                   AutoStart)

        {

            this.OurBusinessDetails    = OurBusinessDetails;
            this.OurCountryCode        = OurCountryCode;
            this.OurPartyId            = OurPartyId;
            this.OurRole               = OurRole;

            this.KeepRemovedEVSEs      = KeepRemovedEVSEs ?? (evse => true);

            // Link HTTP events...
            HTTPServer.RequestLog     += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog    += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog       += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            this.CommonAPILogger       = this.DisableLogging == false
                                             ? new CommonAPILogger(
                                                   this,
                                                   LoggingContext,
                                                   LoggingPath,
                                                   LogfileCreator
                                               )
                                             : null;

            if (!this.DisableLogging)
            {
                ReadRemotePartyDatabaseFile();
                ReadAssetsDatabaseFile();
            }

            if (!Disable_RootServices)
                RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region (private) ReadRemotePartyDatabaseFile()

        private new void ReadRemotePartyDatabaseFile()
        {

            foreach (var command in base.ReadRemotePartyDatabaseFile())
            {

                String?      errorResponse   = null;
                RemoteParty? remoteParty;

                var errorResponses = new List<Tuple<Command, String>>();

                switch (command.CommandName)
                {

                    #region addRemoteParty

                    case addRemoteParty:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse) &&
                                remoteParty is not null)
                            {
                                remoteParties.TryAdd(remoteParty.Id, remoteParty);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addRemotePartyIfNotExists

                    case addRemotePartyIfNotExists:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse) &&
                                remoteParty is not null)
                            {
                                remoteParties.TryAdd(remoteParty.Id, remoteParty);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addOrUpdateRemoteParty

                    case addOrUpdateRemoteParty:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse) &&
                                remoteParty is not null)
                            {

                                if (remoteParties.ContainsKey(remoteParty.Id))
                                    remoteParties.Remove(remoteParty.Id, out _);

                                remoteParties.TryAdd(remoteParty.Id, remoteParty);

                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateRemoteParty

                    case updateRemoteParty:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse) &&
                                remoteParty is not null)
                            {
                                remoteParties.Remove(remoteParty.Id, out _);
                                remoteParties.TryAdd(remoteParty.Id, remoteParty);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateRemoteParty

                    case removeRemoteParty:
                        try
                        {
                            if (command.JSONObject is not null &&
                                RemoteParty.TryParse(command.JSONObject,
                                                     out remoteParty,
                                                     out errorResponse) &&
                                remoteParty is not null)
                            {
                                remoteParties.Remove(remoteParty.Id, out _);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region removeAllRemoteParties

                    case removeAllRemoteParties:
                        remoteParties.Clear();
                        break;

                    #endregion


                }


            }

        }

        #endregion

        #region (private) ReadAssetsDatabaseFile()

        private new void ReadAssetsDatabaseFile()
        {

            foreach (var command in base.ReadAssetsDatabaseFile())
            {

                String?       errorResponse   = null;
                Location?     location;
                Tariff?       tariff;
                Session?      session;
                TokenStatus  _tokenStatus;
                CDR?          cdr;

                var errorResponses = new List<Tuple<Command, String>>();

                switch (command.CommandName)
                {

                    #region addLocation

                    case addLocation:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Location.TryParse(command.JSONObject,
                                                  out location,
                                                  out errorResponse) &&
                                location is not null)
                            {
                                locations.TryAdd(location.Id, location);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addLocationIfNotExists

                    case addLocationIfNotExists:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Location.TryParse(command.JSONObject,
                                                  out location,
                                                  out errorResponse) &&
                                location is not null)
                            {
                                locations.TryAdd(location.Id, location);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addOrUpdateLocation

                    case addOrUpdateLocation:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Location.TryParse(command.JSONObject,
                                                  out location,
                                                  out errorResponse) &&
                                location is not null)
                            {

                                if (locations.ContainsKey(location.Id))
                                    locations.Remove(location.Id, out _);

                                locations.TryAdd(location.Id, location);

                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateLocation

                    case updateLocation:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Location.TryParse(command.JSONObject,
                                                  out location,
                                                  out errorResponse) &&
                                location is not null)
                            {
                                locations.Remove(location.Id, out _);
                                locations.TryAdd(location.Id, location);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateLocation

                    case removeLocation:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Location.TryParse(command.JSONObject,
                                                  out location,
                                                  out errorResponse) &&
                                location is not null)
                            {
                                locations.Remove(location.Id, out _);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region removeAllLocations

                    case removeAllLocations:
                        locations.Clear();
                        break;

                    #endregion


                    #region addTariff

                    case addTariff:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Tariff.TryParse(command.JSONObject,
                                                out tariff,
                                                out errorResponse) &&
                                tariff is not null)
                            {
                                tariffs.TryAdd(tariff.Id, tariff);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addTariffIfNotExists

                    case addTariffIfNotExists:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Tariff.TryParse(command.JSONObject,
                                                out tariff,
                                                out errorResponse) &&
                                tariff is not null)
                            {
                                tariffs.TryAdd(tariff.Id, tariff);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addOrUpdateTariff

                    case addOrUpdateTariff:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Tariff.TryParse(command.JSONObject,
                                                out tariff,
                                                out errorResponse) &&
                                tariff is not null)
                            {

                                if (tariffs.ContainsKey(tariff.Id))
                                    tariffs.Remove(tariff.Id);

                                tariffs.TryAdd(tariff.Id, tariff);

                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateTariff

                    case updateTariff:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Tariff.TryParse(command.JSONObject,
                                                out tariff,
                                                out errorResponse) &&
                                tariff is not null)
                            {
                                tariffs.Remove(tariff.Id);
                                tariffs.TryAdd(tariff.Id, tariff);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateTariff

                    case removeTariff:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Tariff.TryParse(command.JSONObject,
                                                out tariff,
                                                out errorResponse) &&
                                tariff is not null)
                            {
                                tariffs.Remove(tariff.Id);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region removeAllTariffs

                    case removeAllTariffs:
                        tariffs.Clear();
                        break;

                    #endregion


                    #region addSession

                    case addSession:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Session.TryParse(command.JSONObject,
                                                 out session,
                                                 out errorResponse) &&
                                session is not null)
                            {
                                chargingSessions.TryAdd(session.Id, session);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addSessionIfNotExists

                    case addSessionIfNotExists:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Session.TryParse(command.JSONObject,
                                                 out session,
                                                 out errorResponse) &&
                                session is not null)
                            {
                                chargingSessions.TryAdd(session.Id, session);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addOrUpdateSession

                    case addOrUpdateSession:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Session.TryParse(command.JSONObject,
                                                 out session,
                                                 out errorResponse) &&
                                session is not null)
                            {

                                if (chargingSessions.ContainsKey(session.Id))
                                    chargingSessions.Remove(session.Id, out _);

                                chargingSessions.TryAdd(session.Id, session);

                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateSession

                    case updateSession:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Session.TryParse(command.JSONObject,
                                                 out session,
                                                 out errorResponse) &&
                                session is not null)
                            {
                                chargingSessions.Remove(session.Id, out _);
                                chargingSessions.TryAdd(session.Id, session);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateSession

                    case removeSession:
                        try
                        {
                            if (command.JSONObject is not null &&
                                Session.TryParse(command.JSONObject,
                                                 out session,
                                                 out errorResponse) &&
                                session is not null)
                            {
                                chargingSessions.Remove(session.Id, out _);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region removeAllSessions

                    case removeAllSessions:
                        chargingSessions.Clear();
                        break;

                    #endregion


                    #region addToken

                    case addTokenStatus:
                        try
                        {
                            if (command.JSONObject is not null &&
                                TokenStatus.TryParse(command.JSONObject,
                                                     out _tokenStatus,
                                                     out errorResponse))
                            {
                                tokenStatus.TryAdd(_tokenStatus.Token.Id, _tokenStatus);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addTokenIfNotExists

                    case addTokenStatusIfNotExists:
                        try
                        {
                            if (command.JSONObject is not null &&
                                TokenStatus.TryParse(command.JSONObject,
                                                     out _tokenStatus,
                                                     out errorResponse))
                            {
                                tokenStatus.TryAdd(_tokenStatus.Token.Id, _tokenStatus);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addOrUpdateToken

                    case addOrUpdateTokenStatus:
                        try
                        {
                            if (command.JSONObject is not null &&
                                TokenStatus.TryParse(command.JSONObject,
                                                     out _tokenStatus,
                                                     out errorResponse))
                            {

                                if (tokenStatus.ContainsKey(_tokenStatus.Token.Id))
                                    tokenStatus.Remove(_tokenStatus.Token.Id, out _);

                                tokenStatus.TryAdd(_tokenStatus.Token.Id, _tokenStatus);

                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateToken

                    case updateTokenStatus:
                        try
                        {
                            if (command.JSONObject is not null &&
                                TokenStatus.TryParse(command.JSONObject,
                                                     out _tokenStatus,
                                                     out errorResponse))
                            {
                                tokenStatus.Remove(_tokenStatus.Token.Id, out _);
                                tokenStatus.TryAdd(_tokenStatus.Token.Id, _tokenStatus);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateToken

                    case removeTokenStatus:
                        try
                        {
                            if (command.JSONObject is not null &&
                                TokenStatus.TryParse(command.JSONObject,
                                                     out _tokenStatus,
                                                     out errorResponse))
                            {
                                tokenStatus.Remove(_tokenStatus.Token.Id, out _);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region removeAllTokens

                    case removeAllTokenStatus:
                        tokenStatus.Clear();
                        break;

                    #endregion


                    #region addChargeDetailRecord

                    case addChargeDetailRecord:
                        try
                        {
                            if (command.JSONObject is not null &&
                                CDR.TryParse(command.JSONObject,
                                             out cdr,
                                             out errorResponse) &&
                                cdr is not null)
                            {
                                chargeDetailRecords.TryAdd(cdr.Id, cdr);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addChargeDetailRecordIfNotExists

                    case addChargeDetailRecordIfNotExists:
                        try
                        {
                            if (command.JSONObject is not null &&
                                CDR.TryParse(command.JSONObject,
                                             out cdr,
                                             out errorResponse) &&
                                cdr is not null)
                            {
                                chargeDetailRecords.TryAdd(cdr.Id, cdr);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region addOrUpdateChargeDetailRecord

                    case addOrUpdateChargeDetailRecord:
                        try
                        {
                            if (command.JSONObject is not null &&
                                CDR.TryParse(command.JSONObject,
                                             out cdr,
                                             out errorResponse) &&
                                cdr is not null)
                            {

                                if (chargeDetailRecords.ContainsKey(cdr.Id))
                                    chargeDetailRecords.Remove(cdr.Id, out _);

                                chargeDetailRecords.TryAdd(cdr.Id, cdr);

                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateChargeDetailRecord

                    case updateChargeDetailRecord:
                        try
                        {
                            if (command.JSONObject is not null &&
                                CDR.TryParse(command.JSONObject,
                                                out cdr,
                                                out errorResponse) &&
                                cdr is not null)
                            {
                                chargeDetailRecords.Remove(cdr.Id, out _);
                                chargeDetailRecords.TryAdd(cdr.Id, cdr);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region updateCDR

                    case removeChargeDetailRecord:
                        try
                        {
                            if (command.JSONObject is not null &&
                                CDR.TryParse(command.JSONObject,
                                             out cdr,
                                             out errorResponse) &&
                                cdr is not null)
                            {
                                chargeDetailRecords.Remove(cdr.Id, out _);
                            }
                        }
                        catch (Exception e)
                        {
                            errorResponse ??= e.Message;
                        }
                        if (errorResponse is not null)
                            errorResponses.Add(new Tuple<Command, String>(command, errorResponse));
                        break;

                    #endregion

                    #region removeAllCDRs

                    case removeAllChargeDetailRecords:
                        chargeDetailRecords.Clear();
                        break;

                    #endregion


                    default:
                        DebugX.Log($"Unknown OCPI {Version.String} command: '{command}'!");
                        break;

                }


            }

        }

        #endregion


        #region GetModuleURL(ModuleId, Prefix = "")

        /// <summary>
        /// Return the URL of an OCPI module.
        /// </summary>
        /// <param name="ModuleId">The identification of an OCPI module.</param>
        /// <param name="Prefix">An optional prefix.</param>
        public URL GetModuleURL(Module_Id  ModuleId,
                                String     Prefix   = "")

            => OurBaseURL + Prefix + ModuleId.ToString();

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region OPTIONS     ~/

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServiceName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                     Allow                      = new[] {
                                                                                      HTTPMethod.OPTIONS,
                                                                                      HTTPMethod.GET
                                                                                  },
                                                     AccessControlAllowHeaders  = [ "Authorization" ],
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/

            //HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
            //                                   URLPrefix + "/", "cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CommonAPI.HTTPRoot",
            //                                   Assembly.GetCallingAssembly());

            //this.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             new HTTPPath[] {
            //                                 URLPrefix + "/index.html",
            //                                 URLPrefix + "/"
            //                             },
            //                             HTTPContentType.Text.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream = new MemoryStream();
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CommonAPI.HTTPRoot._header.html").SeekAndCopyTo(_MemoryStream, 3);
            //                                 typeof(CommonAPI).Assembly.GetManifestResourceStream("cloud.charging.open.protocols.OCPIv2_1_1.HTTPAPI.CommonAPI.HTTPRoot._footer.html").SeekAndCopyTo(_MemoryStream, 3);

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     ContentType     = HTTPContentType.Text.HTML_UTF8,
            //                                     Content         = _MemoryStream.ToArray(),
            //                                     Connection      = ConnectionType.Close
            //                                 };

            //                             });

            #region Text

            HTTPServer.AddMethodCallback(this,
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix,
                                         HTTPContentType.Text.PLAIN,
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServiceName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                     AccessControlAllowHeaders  = [ "Authorization" ],
                                                     ContentType                = HTTPContentType.Text.PLAIN,
                                                     Content                    = "This is an Open Charge Point Interface HTTP service!\r\nPlease check ~/versions!".ToUTF8Bytes(),
                                                     Connection                 = ConnectionType.Close
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion


            #region OPTIONS     ~/versions

            // ----------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions
            // ----------------------------------------------------
            this.AddOCPIMethod(Hostname,
                                HTTPMethod.OPTIONS,
                                URLPathPrefix + "versions",
                                OCPIRequestHandler: Request => {

                                    return Task.FromResult(
                                        new OCPIResponse.Builder(Request) {
                                            HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                HTTPStatusCode             = HTTPStatusCode.OK,
                                                Server                     = HTTPServiceName,
                                                Date                       = Timestamp.Now,
                                                AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                Allow                      = new[] {
                                                                                HTTPMethod.OPTIONS,
                                                                                HTTPMethod.GET
                                                                            },
                                                AccessControlAllowHeaders  = [ "Authorization" ],
                                                Vary                       = "Accept"
                                            }
                                        });

                                });

            #endregion

            #region GET         ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions
            // ----------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "versions",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetVersionsRequest,
                               OCPIResponseLogger:  GetVersionsResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                               StatusCode           = 2000,
                                               StatusMessage        = "Invalid or blocked access token!",
                                               HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                   HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                   AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                   AccessControlAllowHeaders  = [ "Authorization" ]
                                               }
                                           });

                                   }

                                   #endregion

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           StatusCode           = 1000,
                                           StatusMessage        = "Hello world!",
                                           Data                 = new JArray(
                                                                      new VersionInformation[] {
                                                                          new VersionInformation(
                                                                              Version.Id,
                                                                              URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                      (Request.Host + (URLPathPrefix + AdditionalURLPathPrefix + $"/versions/{Version.Id}")).Replace("//", "/"))
                                                                          )
                                                                      }.Where (version => version is not null).
                                                                      Select(version => version.ToJSON())
                                                                  ),
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                        //       Server                     = ServiceName,
                                        //       Date                       = Timestamp.Now,
                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                               Vary                       = "Accept"
                                           }
                                       });

                               });

            #endregion


            #region OPTIONS     ~/versions/{versionId}

            // --------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions/{id}
            // --------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.OPTIONS,
                               URLPathPrefix + "versions/{versionId}",
                               OCPIRequestHandler: Request => {

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           HTTPResponseBuilder = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                               Allow                      = new[] {
                                                                                HTTPMethod.OPTIONS,
                                                                                HTTPMethod.GET
                                                                            },
                                               AccessControlAllowHeaders  = [ "Authorization" ],
                                               Vary                       = "Accept"
                                           }
                                       });
                               });

            #endregion

            #region GET         ~/versions/{versionId}

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions/{id}
            // ---------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "versions/{versionId}",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetVersionRequest,
                               OCPIResponseLogger:  GetVersionResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   #endregion

                                   #region Get version identification URL parameter

                                   if (Request.ParsedURLParameters.Length < 1)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Version identification is missing!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   if (!Version_Id.TryParse(Request.ParsedURLParameters[0], out var versionId))
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Version identification is invalid!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   #endregion

                                   #region Only allow OCPI version v2.1.1

                                   if (versionId.ToString() != Version.Id.ToString())
                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "This OCPI version is not supported!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   #endregion


                                   var prefix = URLPathPrefix + AdditionalURLPathPrefix + $"v{versionId}";

                                   #region Common credential endpoints...

                                   var endpoints  = new List<VersionEndpoint>() {

                                                        new VersionEndpoint(Module_Id.Credentials,
                                                                            URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                      (Request.Host + (prefix + "credentials")).Replace("//", "/")))

                                                    };

                                   #endregion


                                   #region The other side is a CPO...

                                   if (Request.RemoteParty?.Role == Roles.CPO)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") + 
                                                                                   (Request.Host + (prefix + "emsp/locations")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tariffs,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/tariffs")).  Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Sessions,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/sessions")). Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.CDRs,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/cdrs")).     Replace("//", "/"))));


                                       endpoints.Add(new VersionEndpoint(Module_Id.Commands,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/commands")). Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tokens,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "emsp/tokens")).   Replace("//", "/"))));

                                   }

                                   #endregion

                                   #region We are a CPO, the other side is unauthenticated and we export locations as Open Data...

                                   if (OurRole == Roles.CPO && Request.RemoteParty is null && LocationsAsOpenData)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/locations")).Replace("//", "/"))));

                                   }

                                   #endregion

                                   #region The other side is an EMSP...

                                   if (Request.RemoteParty?.Role == Roles.EMSP)
                                   {

                                       endpoints.Add(new VersionEndpoint(Module_Id.Locations,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/locations")).Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tariffs,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/tariffs")).  Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Sessions,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/sessions")). Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.CDRs,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/cdrs")).     Replace("//", "/"))));


                                       endpoints.Add(new VersionEndpoint(Module_Id.Commands,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/commands")). Replace("//", "/"))));

                                       endpoints.Add(new VersionEndpoint(Module_Id.Tokens,
                                                                         URL.Parse((OurVersionsURL.Protocol == URLProtocols.https ? "https://" : "http://") +
                                                                                   (Request.Host + (prefix + "cpo/tokens")).   Replace("//", "/"))));

                                   }

                                   #endregion


                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                              StatusCode           = 1000,
                                              StatusMessage        = "Hello world!",
                                              Data                 = new VersionDetail(
                                                                         versionId,
                                                                         endpoints
                                                                     ).ToJSON(),
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ],
                                                  Vary                       = "Accept"
                                              }
                                          });

                               });

            #endregion


            #region OPTIONS     ~/{versionId}/credentials

            // ----------------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/2.1.1/credentials
            // ----------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.OPTIONS,
                               URLPathPrefix + "{versionId}/credentials",
                               OCPIRequestHandler: Request => {

                                   #region Defaults

                                   var accessControlAllowMethods  = new List<String> {
                                                                        "OPTIONS",
                                                                        "GET"
                                                                    };

                                   var allow                      = new List<HTTPMethod>() {
                                                                        HTTPMethod.OPTIONS,
                                                                        HTTPMethod.GET
                                                                    };

                                   #endregion

                                   #region Check the access token whether the client is known, and its access is allowed!

                                   if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                   {

                                       accessControlAllowMethods.Add("POST");

                                       allow.Add(HTTPMethod.POST);

                                       // Only when the party is fully registered!
                                       if (Request.LocalAccessInfo?.VersionsURL.HasValue == true)
                                       {

                                           accessControlAllowMethods.Add("PUT");
                                           accessControlAllowMethods.Add("DELETE");

                                           allow.Add(HTTPMethod.PUT);
                                           allow.Add(HTTPMethod.DELETE);

                                       }

                                    }

                                   #endregion


                                   return Task.FromResult(
                                              new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 1000,
                                                  StatusMessage        = "Hello world!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      AccessControlAllowHeaders  = [ "Authorization" ],
                                                      AccessControlAllowMethods  = accessControlAllowMethods,
                                                      Allow                      = allow
                                                  }
                                              });

                               });

            #endregion

            #region GET         ~/{versionId}/credentials

            // Retrieves the credentials object to access the server's platform.
            // The response contains the credentials object to access the server's platform.
            // This credentials object also contains extra information about the server such as its business details.

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.1.1/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.GET,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   GetCredentialsRequest,
                               OCPIResponseLogger:  GetCredentialsResponse,
                               OCPIRequestHandler:  Request => {

                                   #region Check access token... not allowed!

                                   if (Request.LocalAccessInfo is not null &&
                                       Request.LocalAccessInfo.Status != AccessStatus.ALLOWED)
                                   {

                                       return Task.FromResult(
                                           new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "Invalid or blocked access token!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          });

                                   }

                                   #endregion

                                   return Task.FromResult(
                                       new OCPIResponse.Builder(Request) {
                                           StatusCode           = 1000,
                                           StatusMessage        = "Hello world!",
                                           Data                 = new Credentials(
                                                                      Request.LocalAccessInfo?.AccessToken ?? AccessToken.Parse("<any>"),
                                                                      OurVersionsURL,
                                                                      OurBusinessDetails,
                                                                      OurCountryCode,
                                                                      OurPartyId
                                                                  ).ToJSON(),
                                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                               HTTPStatusCode             = HTTPStatusCode.OK,
                                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                               AccessControlAllowHeaders  = [ "Authorization" ]
                                           }
                                       });

                               });

            #endregion

            #region POST        ~/{versionId}/credentials

            // REGISTER new OCPI party!

            // -----------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.1.1/credentials
            // -----------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.POST,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   PostCredentialsRequest,
                               OCPIResponseLogger:  PostCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                   {

                                       if (Request.LocalAccessInfo?.VersionsURL.HasValue == true)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,                                              // CREDENTIALS_TOKEN_A
                                                      StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' is already registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                                      }
                                                  };

                                       return await POSTOrPUTCredentials(Request);

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                  AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          };

                               });

            #endregion

            #region PUT         ~/{versionId}/credentials

            // UPDATE the registration of an existing OCPI party!

            // Provides the server with updated credentials to access the client's system.
            // This credentials object also contains extra information about the client such as its business details.

            // A PUT will switch to the version that contains this credentials endpoint if it's different from the current version.
            // The server must fetch the client's endpoints again, even if the version has not changed.

            // If successful, the server must generate a new token for the client and respond with the client's updated credentials to access the server's system.
            // The credentials object in the response also contains extra information about the server such as its business details.

            // This must return a HTTP status code 405: method not allowed if the client was not registered yet.

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.1.1/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.PUT,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   PutCredentialsRequest,
                               OCPIResponseLogger:  PutCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   #region The access token is known...

                                   if (Request.LocalAccessInfo is not null)
                                   {

                                       #region ...but access is blocked!

                                       if (Request.LocalAccessInfo?.Status == AccessStatus.BLOCKED)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = "The given access token '" + (Request.AccessToken?.ToString() ?? "") + "' is blocked!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                          AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                                      }
                                                  };

                                       #endregion

                                       #region ...and access is allowed, but maybe not yet full registered!

                                       if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                       {

                                           // The party is not yet fully registered!
                                           if (!Request.LocalAccessInfo?.VersionsURL.HasValue == true)
                                               return new OCPIResponse.Builder(Request) {
                                                          StatusCode           = 2000,
                                                          StatusMessage        = "The given access token '" + (Request.AccessToken?.ToString() ?? "") + "' is not yet registered!",
                                                          HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                              HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                              AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST" },
                                                              AccessControlAllowHeaders  = [ "Authorization" ]
                                                          }
                                                      };

                                           return await POSTOrPUTCredentials(Request);

                                       }

                                       #endregion

                                   }

                                   #endregion

                                   return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 2000,
                                                  StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                      AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                      AccessControlAllowHeaders  = [ "Authorization" ]
                                                  }
                                              };

                               });

            #endregion

            #region DELETE      ~/{versionId}/credentials

            // UNREGISTER an existing OCPI party!

            // Informs the server that its credentials to access the client's system are now invalid and can no longer be used.
            // Both parties must end any automated communication.
            // This is the unregistration process.

            // This must return a HTTP status code 405: method not allowed if the client was not registered.

            // ---------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/2.1.1/credentials
            // ---------------------------------------------------------------------------------
            this.AddOCPIMethod(Hostname,
                               HTTPMethod.DELETE,
                               URLPathPrefix + "{versionId}/credentials",
                               HTTPContentType.Application.JSON_UTF8,
                               OCPIRequestLogger:   DeleteCredentialsRequest,
                               OCPIResponseLogger:  DeleteCredentialsResponse,
                               OCPIRequestHandler:  async Request => {

                                   if (Request.LocalAccessInfo?.Status == AccessStatus.ALLOWED)
                                   {

                                       #region Validations

                                       if (!Request.LocalAccessInfo.VersionsURL.HasValue)
                                           return new OCPIResponse.Builder(Request) {
                                                      StatusCode           = 2000,
                                                      StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' is not fully registered!",
                                                      HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                          HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                          AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "DELETE" },
                                                          AccessControlAllowHeaders  = [ "Authorization" ]
                                                      }
                                                  };

                                       #endregion

                                       await RemoveAccessToken(Request.LocalAccessInfo.AccessToken);

                                       return new OCPIResponse.Builder(Request) {
                                                  StatusCode           = 1000,
                                                  StatusMessage        = $"The given access token '{Request.LocalAccessInfo.AccessToken}' was deleted!",
                                                  HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                      HTTPStatusCode             = HTTPStatusCode.OK,
                                                      AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                                                      AccessControlAllowHeaders  = [ "Authorization" ]
                                                  }
                                              };

                                   }

                                   return new OCPIResponse.Builder(Request) {
                                              StatusCode           = 2000,
                                              StatusMessage        = "You need to be registered before trying to invoke this protected method!",
                                              HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                                                  HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                                                  AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                                  AccessControlAllowHeaders  = [ "Authorization" ]
                                              }
                                          };

                               });

            #endregion

        }

        #endregion


        #region (private) POSTOrPUTCredentials(Request)

        private async Task<OCPIResponse.Builder> POSTOrPUTCredentials(OCPIRequest Request)
        {

            #region Validate CREDENTIALS_TOKEN_A

            var CREDENTIALS_TOKEN_A     = Request.AccessToken;

            if (!CREDENTIALS_TOKEN_A.HasValue)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "The received credential token must not be null!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            #region Validate old remote party

            var oldRemoteParty = GetRemoteParties(remoteParty => remoteParty.LocalAccessInfos.Any(accessInfoStatus => accessInfoStatus.AccessToken == CREDENTIALS_TOKEN_A.Value)).FirstOrDefault();

            if (oldRemoteParty is null)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"There is no remote party having the given access token '{CREDENTIALS_TOKEN_A}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            #region Parse JSON

            var errorResponse = String.Empty;

            if (!Request.TryParseJObjectRequestBody(out var JSON,
                                                    out var responseBuilder,
                                                    AllowEmptyHTTPBody: false))
            {
                return responseBuilder;
            }

            if (!Credentials.TryParse(JSON,
                                      out var receivedCredentials,
                                      out errorResponse) ||
                receivedCredentials is null)
            {

                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not parse the received credentials JSON object: " + errorResponse,
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            }

            #endregion

            #region Additional security checks... (Non-Standard)

            //lock (AccessTokens)
            //{
            //    foreach (var credentialsRole in receivedCredentials.Roles)
            //    {

            //        var result = AccessTokens.Values.Where(accessToken => accessToken.Roles.Any(role => role.CountryCode == credentialsRole.CountryCode &&
            //                                                                                            role.PartyId     == credentialsRole.PartyId &&
            //                                                                                            role.Role        == credentialsRole.Role)).ToArray();

            //        if (result.Length == 0)
            //        {

            //            return new OCPIResponse.Builder(Request) {
            //                       StatusCode           = 2000,
            //                       StatusMessage        = "The given combination of country code, party identification and role is unknown!",
            //                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
            //                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
            //                           AccessControlAllowHeaders  = [ "Authorization" ]
            //                       }
            //                   };

            //        }

            //        if (result.Length > 0 &&
            //            result.First().VersionsURL.HasValue)
            //        {

            //            return new OCPIResponse.Builder(Request) {
            //                       StatusCode           = 2000,
            //                       StatusMessage        = "The given combination of country code, party identification and role is already registered!",
            //                       HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
            //                           HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
            //                           AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
            //                           AccessControlAllowHeaders  = [ "Authorization" ]
            //                       }
            //                   };

            //        }

            //    }
            //}

            #endregion


            var commonClient              = new CommonClient(this,
                                                             receivedCredentials.URL,
                                                             receivedCredentials.Token,  // CREDENTIALS_TOKEN_B
                                                             DNSClient: HTTPServer.DNSClient);

            var otherVersions             = await commonClient.GetVersions();

            #region ...or send error!

            if (otherVersions.StatusCode != 1000)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = "Could not fetch VERSIONS information from '" + receivedCredentials.URL + "'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            var justMySupportedVersion    = otherVersions.Data?.Where(version => version.Id == Version.Id).ToArray() ?? Array.Empty<VersionInformation>();

            #region ...or send error!

            if (justMySupportedVersion.Length == 0)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3003,
                           StatusMessage        = $"Could not find {Version.String} at '{receivedCredentials.URL}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            var otherVersion2_1_1Details  = await commonClient.GetVersionDetails(Version.Id);

            #region ...or send error!

            if (otherVersion2_1_1Details.StatusCode != 1000)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 3001,
                           StatusMessage        = $"Could not fetch {Version.String} information from '{justMySupportedVersion.First().URL}'!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.MethodNotAllowed,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion


            #region Validate, that neither the country code nor the party identification was changed!

            if (oldRemoteParty.CountryCode != receivedCredentials.CountryCode)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"Updating the country code from '{oldRemoteParty.CountryCode}' to '{receivedCredentials.CountryCode}' is not allowed!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            if (oldRemoteParty.PartyId != receivedCredentials.PartyId)
                return new OCPIResponse.Builder(Request) {
                           StatusCode           = 2000,
                           StatusMessage        = $"Updating the party identification from '{oldRemoteParty.PartyId}' to '{receivedCredentials.PartyId}' is not allowed!",
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.BadRequest,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

            #endregion

            var CREDENTIALS_TOKEN_C       = AccessToken.NewRandom();

            // Remove the old access token
            await RemoveAccessToken     (CREDENTIALS_TOKEN_A.Value);

            // Store credential of the other side!
            await AddOrUpdateRemoteParty(receivedCredentials.CountryCode,
                                         receivedCredentials.PartyId,
                                         oldRemoteParty.     Role,
                                         receivedCredentials.BusinessDetails,

                                         CREDENTIALS_TOKEN_C,

                                         receivedCredentials.Token,
                                         receivedCredentials.URL,
                                         otherVersions.Data?.Select(version => version.Id) ?? Array.Empty<Version_Id>(),
                                         Version.Id,

                                         null,
                                         null,
                                         null,
                                         null,
                                         AccessStatus.      ALLOWED,
                                         RemoteAccessStatus.ONLINE,
                                         PartyStatus.       ENABLED);


            return new OCPIResponse.Builder(Request) {
                           StatusCode           = 1000,
                           StatusMessage        = "Hello world!",
                           Data                 = new Credentials(
                                                      CREDENTIALS_TOKEN_C,
                                                      OurVersionsURL,
                                                      OurBusinessDetails,
                                                      OurCountryCode,
                                                      OurPartyId
                                                  ).ToJSON(),
                           HTTPResponseBuilder  = new HTTPResponse.Builder(Request.HTTPRequest) {
                               HTTPStatusCode             = HTTPStatusCode.OK,
                               AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "POST", "PUT", "DELETE" },
                               AccessControlAllowHeaders  = [ "Authorization" ]
                           }
                       };

        }

        #endregion


        //ToDo: Wrap the following into a plugable interface!

        #region AccessTokens

        // An access token might be used by more than one CountryCode + PartyId + Role combination!

        #region RemoveAccessToken(AccessToken)

        public async Task<CommonAPI> RemoveAccessToken(AccessToken        AccessToken,
                                                       EventTracking_Id?  EventTrackingId   = null,
                                                       User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.Where(party => party.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken)))
            {

                #region The remote party has only a single local access token, or...

                if (remoteParty.LocalAccessInfos.Count() <= 1)
                {

                    remoteParties.TryRemove(remoteParty.Id, out _);

                    await LogAsset(removeRemoteParty,
                                   remoteParty.ToJSON(true),
                                   EventTrackingId ?? EventTracking_Id.New,
                                   CurrentUserId);

                }

                #endregion

                #region ...the remote party has multiple local access tokens!

                else
                {

                    remoteParties.TryRemove(remoteParty.Id, out _);

                    var newRemoteParty = new RemoteParty(
                                                remoteParty.CountryCode,
                                                remoteParty.PartyId,
                                                remoteParty.Role,
                                                remoteParty.BusinessDetails,
                                                remoteParty.LocalAccessInfos.Where(localAccessInfo => localAccessInfo.AccessToken != AccessToken),
                                                remoteParty.RemoteAccessInfos,
                                                remoteParty.Status
                                            );

                    if (remoteParties.TryAdd(newRemoteParty.Id,
                                                newRemoteParty))
                    {

                        await LogRemoteParty(updateRemoteParty,
                                             newRemoteParty.ToJSON(true),
                                             EventTrackingId ?? EventTracking_Id.New,
                                             CurrentUserId);

                    }

                }

                #endregion

            }

            return this;

        }

        #endregion

        #region TryGetLocalAccessInfo(AccessToken, out LocalAccessInfo)

        public Boolean TryGetLocalAccessInfo(AccessToken AccessToken, out LocalAccessInfo LocalAccessInfo)
        {

            var accessInfos = remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken)).
                                                   SelectMany(remoteParty => remoteParty.LocalAccessInfos).
                                                   ToArray();

            if (accessInfos.Length == 1)
            {
                LocalAccessInfo = accessInfos.First();
                return true;
            }

            LocalAccessInfo = default;
            return false;

        }

        #endregion

        #region GetLocalAccessInfos(AccessToken)

        public IEnumerable<LocalAccessInfo> GetLocalAccessInfos(AccessToken AccessToken)

            => remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken)).
                                    SelectMany(remoteParty => remoteParty.LocalAccessInfos);

        #endregion

        #region GetLocalAccessInfos(AccessToken, AccessStatus)

        public IEnumerable<LocalAccessInfo> GetLocalAccessInfos(AccessToken   AccessToken,
                                                                AccessStatus  AccessStatus)

            => remoteParties.Values.Where     (remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken  == AccessToken &&
                                                                                                             accessInfo.Status       == AccessStatus)).
                                    SelectMany(remoteParty => remoteParty.LocalAccessInfos);

        #endregion

        #endregion

        #region RemoteParties

        #region Data

        private readonly ConcurrentDictionary<RemoteParty_Id, RemoteParty> remoteParties = new ();

        /// <summary>
        /// Return an enumeration of all remote parties.
        /// </summary>
        public IEnumerable<RemoteParty> RemoteParties
            => remoteParties.Values;

        #endregion


        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(CountryCode                                                CountryCode,
                                                  Party_Id                                                   PartyId,
                                                  Roles                                                      Role,
                                                  BusinessDetails                                            BusinessDetails,

                                                  AccessToken                                                AccessToken,

                                                  AccessToken                                                RemoteAccessToken,
                                                  URL                                                        RemoteVersionsURL,
                                                  IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                  Version_Id?                                                SelectedVersionId            = null,

                                                  DateTime?                                                  LocalAccessNotBefore         = null,
                                                  DateTime?                                                  LocalAccessNotAfter          = null,

                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = false,
                                                  AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                  RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                  PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                  DateTime?                                                  RemoteAccessNotBefore        = null,
                                                  DateTime?                                                  RemoteAccessNotAfter         = null,

                                                  Boolean?                                                   PreferIPv4                   = null,
                                                  RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                  LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                  X509Certificate?                                           ClientCert                   = null,
                                                  SslProtocols?                                              TLSProtocol                  = null,
                                                  String?                                                    HTTPUserAgent                = null,
                                                  TimeSpan?                                                  RequestTimeout               = null,
                                                  TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                  UInt16?                                                    MaxNumberOfRetries           = null,
                                                  UInt32?                                                    InternalBufferSize           = null,
                                                  Boolean?                                                   UseHTTPPipelining            = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(CountryCode,
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
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty)) {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(CountryCode                                                CountryCode,
                                                  Party_Id                                                   PartyId,
                                                  Roles                                                      Role,
                                                  BusinessDetails                                            BusinessDetails,

                                                  AccessToken                                                AccessToken,
                                                  DateTime?                                                  LocalAccessNotBefore         = null,
                                                  DateTime?                                                  LocalAccessNotAfter          = null,
                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = false,
                                                  AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                  PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

                                                  Boolean?                                                   PreferIPv4                   = null,
                                                  RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                  LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                  X509Certificate?                                           ClientCert                   = null,
                                                  SslProtocols?                                              TLSProtocol                  = null,
                                                  String?                                                    HTTPUserAgent                = null,
                                                  TimeSpan?                                                  RequestTimeout               = null,
                                                  TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                  UInt16?                                                    MaxNumberOfRetries           = null,
                                                  UInt32?                                                    InternalBufferSize           = null,
                                                  Boolean?                                                   UseHTTPPipelining            = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(CountryCode,
                                                 PartyId,
                                                 Role,
                                                 BusinessDetails,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(CountryCode                                                CountryCode,
                                                  Party_Id                                                   PartyId,
                                                  Roles                                                      Role,
                                                  BusinessDetails                                            BusinessDetails,

                                                  AccessToken                                                RemoteAccessToken,
                                                  URL                                                        RemoteVersionsURL,
                                                  IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                  Version_Id?                                                SelectedVersionId            = null,

                                                  Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                  Boolean?                                                   AllowDowngrades              = null,
                                                  RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                  PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                  DateTime?                                                  RemoteAccessNotBefore        = null,
                                                  DateTime?                                                  RemoteAccessNotAfter         = null,

                                                  Boolean?                                                   PreferIPv4                   = null,
                                                  RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                  LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                  X509Certificate?                                           ClientCert                   = null,
                                                  SslProtocols?                                              TLSProtocol                  = null,
                                                  String?                                                    HTTPUserAgent                = null,
                                                  TimeSpan?                                                  RequestTimeout               = null,
                                                  TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                  UInt16?                                                    MaxNumberOfRetries           = null,
                                                  UInt32?                                                    InternalBufferSize           = null,
                                                  Boolean?                                                   UseHTTPPipelining            = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(CountryCode,
                                                 PartyId,
                                                 Role,
                                                 BusinessDetails,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemoteParty(...)

        public async Task<Boolean> AddRemoteParty(CountryCode                          CountryCode,
                                                  Party_Id                             PartyId,
                                                  Roles                                Role,
                                                  BusinessDetails                      BusinessDetails,

                                                  IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                  IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                  PartyStatus                                                Status                       = PartyStatus.ENABLED,

                                                  Boolean?                                                   PreferIPv4                   = null,
                                                  RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                  LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                  X509Certificate?                                           ClientCert                   = null,
                                                  SslProtocols?                                              TLSProtocol                  = null,
                                                  String?                                                    HTTPUserAgent                = null,
                                                  TimeSpan?                                                  RequestTimeout               = null,
                                                  TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                  UInt16?                                                    MaxNumberOfRetries           = null,
                                                  UInt32?                                                    InternalBufferSize           = null,
                                                  Boolean?                                                   UseHTTPPipelining            = null,

                                                  DateTime?                                                  LastUpdated                  = null,

                                                  EventTracking_Id?                                          EventTrackingId              = null,
                                                  User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(
                                     CountryCode,
                                     PartyId,
                                     Role,
                                     BusinessDetails,

                                     LocalAccessInfos,
                                     RemoteAccessInfos,

                                     Status,

                                     PreferIPv4,
                                     RemoteCertificateValidator,
                                     LocalCertificateSelector,
                                     ClientCert,
                                     TLSProtocol,
                                     HTTPUserAgent,
                                     RequestTimeout,
                                     TransmissionRetryDelay,
                                     MaxNumberOfRetries,
                                     InternalBufferSize,
                                     UseHTTPPipelining,

                                     LastUpdated
                                 );

            if (remoteParties.TryAdd(newRemoteParty.Id,
                                     newRemoteParty))
            {

                await LogRemoteParty(addRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(CountryCode                                                CountryCode,
                                                             Party_Id                                                   PartyId,
                                                             Roles                                                      Role,
                                                             BusinessDetails                                            BusinessDetails,

                                                             AccessToken                                                AccessToken,

                                                             AccessToken                                                RemoteAccessToken,
                                                             URL                                                        RemoteVersionsURL,
                                                             IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                             Version_Id?                                                SelectedVersionId            = null,

                                                             DateTime?                                                  LocalAccessNotBefore         = null,
                                                             DateTime?                                                  LocalAccessNotAfter          = null,

                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = false,
                                                             AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                             RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                             PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                             DateTime?                                                  RemoteAccessNotBefore        = null,
                                                             DateTime?                                                  RemoteAccessNotAfter         = null,

                                                             Boolean?                                                   PreferIPv4                   = null,
                                                             RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                             LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                             X509Certificate?                                           ClientCert                   = null,
                                                             SslProtocols?                                              TLSProtocol                  = null,
                                                             String?                                                    HTTPUserAgent                = null,
                                                             TimeSpan?                                                  RequestTimeout               = null,
                                                             TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                             UInt16?                                                    MaxNumberOfRetries           = null,
                                                             UInt32?                                                    InternalBufferSize           = null,
                                                             Boolean?                                                   UseHTTPPipelining            = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(CountryCode,
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
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(CountryCode                                                CountryCode,
                                                             Party_Id                                                   PartyId,
                                                             Roles                                                      Role,
                                                             BusinessDetails                                            BusinessDetails,

                                                             AccessToken                                                AccessToken,
                                                             DateTime?                                                  LocalAccessNotBefore         = null,
                                                             DateTime?                                                  LocalAccessNotAfter          = null,
                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = false,
                                                             AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                             PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

                                                             Boolean?                                                   PreferIPv4                   = null,
                                                             RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                             LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                             X509Certificate?                                           ClientCert                   = null,
                                                             SslProtocols?                                              TLSProtocol                  = null,
                                                             String?                                                    HTTPUserAgent                = null,
                                                             TimeSpan?                                                  RequestTimeout               = null,
                                                             TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                             UInt16?                                                    MaxNumberOfRetries           = null,
                                                             UInt32?                                                    InternalBufferSize           = null,
                                                             Boolean?                                                   UseHTTPPipelining            = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(CountryCode,
                                                 PartyId,
                                                 Role,
                                                 BusinessDetails,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(CountryCode                                                CountryCode,
                                                             Party_Id                                                   PartyId,
                                                             Roles                                                      Role,
                                                             BusinessDetails                                            BusinessDetails,

                                                             AccessToken                                                RemoteAccessToken,
                                                             URL                                                        RemoteVersionsURL,
                                                             IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                             Version_Id?                                                SelectedVersionId            = null,

                                                             Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                             Boolean?                                                   AllowDowngrades              = null,
                                                             RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                             PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                             DateTime?                                                  RemoteAccessNotBefore        = null,
                                                             DateTime?                                                  RemoteAccessNotAfter         = null,

                                                             Boolean?                                                   PreferIPv4                   = null,
                                                             RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                             LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                             X509Certificate?                                           ClientCert                   = null,
                                                             SslProtocols?                                              TLSProtocol                  = null,
                                                             String?                                                    HTTPUserAgent                = null,
                                                             TimeSpan?                                                  RequestTimeout               = null,
                                                             TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                             UInt16?                                                    MaxNumberOfRetries           = null,
                                                             UInt32?                                                    InternalBufferSize           = null,
                                                             Boolean?                                                   UseHTTPPipelining            = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(CountryCode,
                                                 PartyId,
                                                 Role,
                                                 BusinessDetails,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region AddRemotePartyIfNotExists(...)

        public async Task<Boolean> AddRemotePartyIfNotExists(CountryCode                                                CountryCode,
                                                             Party_Id                                                   PartyId,
                                                             Roles                                                      Role,
                                                             BusinessDetails                                            BusinessDetails,

                                                             IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                             IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                             PartyStatus                                                Status                       = PartyStatus.ENABLED,

                                                             Boolean?                                                   PreferIPv4                   = null,
                                                             RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                             LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                             X509Certificate?                                           ClientCert                   = null,
                                                             SslProtocols?                                              TLSProtocol                  = null,
                                                             String?                                                    HTTPUserAgent                = null,
                                                             TimeSpan?                                                  RequestTimeout               = null,
                                                             TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                             UInt16?                                                    MaxNumberOfRetries           = null,
                                                             UInt32?                                                    InternalBufferSize           = null,
                                                             Boolean?                                                   UseHTTPPipelining            = null,

                                                             DateTime?                                                  LastUpdated                  = null,

                                                             EventTracking_Id?                                          EventTrackingId              = null,
                                                             User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(CountryCode,
                                                 PartyId,
                                                 Role,
                                                 BusinessDetails,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

                                                 Status,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining,

                                                 LastUpdated);

            var result = remoteParties.GetOrAdd(newRemoteParty.Id,
                                                value => newRemoteParty);

            if (result == newRemoteParty)
            {

                await LogRemoteParty(addRemotePartyIfNotExists,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(CountryCode                                                CountryCode,
                                                          Party_Id                                                   PartyId,
                                                          Roles                                                      Role,
                                                          BusinessDetails                                            BusinessDetails,

                                                          AccessToken                                                AccessToken,

                                                          AccessToken                                                RemoteAccessToken,
                                                          URL                                                        RemoteVersionsURL,
                                                          IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                          Version_Id?                                                SelectedVersionId            = null,

                                                          DateTime?                                                  LocalAccessNotBefore         = null,
                                                          DateTime?                                                  LocalAccessNotAfter          = null,

                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = false,
                                                          AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                          RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                          PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                          DateTime?                                                  RemoteAccessNotBefore        = null,
                                                          DateTime?                                                  RemoteAccessNotAfter         = null,

                                                          Boolean?                                                   PreferIPv4                   = null,
                                                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                          X509Certificate?                                           ClientCert                   = null,
                                                          SslProtocols?                                              TLSProtocol                  = null,
                                                          String?                                                    HTTPUserAgent                = null,
                                                          TimeSpan?                                                  RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                          UInt16?                                                    MaxNumberOfRetries           = null,
                                                          UInt32?                                                    InternalBufferSize           = null,
                                                          Boolean?                                                   UseHTTPPipelining            = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(CountryCode,
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
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(CountryCode                                                CountryCode,
                                                          Party_Id                                                   PartyId,
                                                          Roles                                                      Role,
                                                          BusinessDetails                                            BusinessDetails,

                                                          AccessToken                                                AccessToken,
                                                          DateTime?                                                  LocalAccessNotBefore         = null,
                                                          DateTime?                                                  LocalAccessNotAfter          = null,
                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = false,
                                                          AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                          PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

                                                          Boolean?                                                   PreferIPv4                   = null,
                                                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                          X509Certificate?                                           ClientCert                   = null,
                                                          SslProtocols?                                              TLSProtocol                  = null,
                                                          String?                                                    HTTPUserAgent                = null,
                                                          TimeSpan?                                                  RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                          UInt16?                                                    MaxNumberOfRetries           = null,
                                                          UInt32?                                                    InternalBufferSize           = null,
                                                          Boolean?                                                   UseHTTPPipelining            = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(CountryCode,
                                                 PartyId,
                                                 Role,
                                                 BusinessDetails,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(CountryCode                                                CountryCode,
                                                          Party_Id                                                   PartyId,
                                                          Roles                                                      Role,
                                                          BusinessDetails                                            BusinessDetails,

                                                          AccessToken                                                RemoteAccessToken,
                                                          URL                                                        RemoteVersionsURL,
                                                          IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                          Version_Id?                                                SelectedVersionId            = null,

                                                          Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                          Boolean?                                                   AllowDowngrades              = null,
                                                          RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                          PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                          DateTime?                                                  RemoteAccessNotBefore        = null,
                                                          DateTime?                                                  RemoteAccessNotAfter         = null,

                                                          Boolean?                                                   PreferIPv4                   = null,
                                                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                          X509Certificate?                                           ClientCert                   = null,
                                                          SslProtocols?                                              TLSProtocol                  = null,
                                                          String?                                                    HTTPUserAgent                = null,
                                                          TimeSpan?                                                  RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                          UInt16?                                                    MaxNumberOfRetries           = null,
                                                          UInt32?                                                    InternalBufferSize           = null,
                                                          Boolean?                                                   UseHTTPPipelining            = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(CountryCode,
                                                 PartyId,
                                                 Role,
                                                 BusinessDetails,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion

        #region AddOrUpdateRemoteParty(...)

        public async Task<Boolean> AddOrUpdateRemoteParty(CountryCode                                                CountryCode,
                                                          Party_Id                                                   PartyId,
                                                          Roles                                                      Role,
                                                          BusinessDetails                                            BusinessDetails,

                                                          IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                          IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                          PartyStatus                                                Status                       = PartyStatus.ENABLED,

                                                          Boolean?                                                   PreferIPv4                   = null,
                                                          RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                          LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                          X509Certificate?                                           ClientCert                   = null,
                                                          SslProtocols?                                              TLSProtocol                  = null,
                                                          String?                                                    HTTPUserAgent                = null,
                                                          TimeSpan?                                                  RequestTimeout               = null,
                                                          TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                          UInt16?                                                    MaxNumberOfRetries           = null,
                                                          UInt32?                                                    InternalBufferSize           = null,
                                                          Boolean?                                                   UseHTTPPipelining            = null,

                                                          DateTime?                                                  LastUpdated                  = null,

                                                          EventTracking_Id?                                          EventTrackingId              = null,
                                                          User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(CountryCode,
                                                 PartyId,
                                                 Role,
                                                 BusinessDetails,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

                                                 Status,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining,

                                                 LastUpdated);

            var added = false;

            remoteParties.AddOrUpdate(newRemoteParty.Id,

                                      // Add
                                      id => {
                                          added = true;
                                          return newRemoteParty;
                                      },

                                      // Update
                                      (id, oldRemoteParty) => {
                                          return newRemoteParty;
                                      });

            await LogRemoteParty(addOrUpdateRemoteParty,
                                 newRemoteParty.ToJSON(true),
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

            return added;

        }

        #endregion


        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                           ExistingRemoteParty,
                                                     BusinessDetails                       BusinessDetails,

                                                     AccessToken                           AccessToken,

                                                     AccessToken                                                RemoteAccessToken,
                                                     URL                                                        RemoteVersionsURL,
                                                     IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                     Version_Id?                                                SelectedVersionId            = null,

                                                     DateTime?                                                  LocalAccessNotBefore         = null,
                                                     DateTime?                                                  LocalAccessNotAfter          = null,

                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = false,
                                                     AccessStatus                                               AccessStatus                 = AccessStatus.      ALLOWED,
                                                     RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.ONLINE,
                                                     PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                     DateTime?                                                  RemoteAccessNotBefore        = null,
                                                     DateTime?                                                  RemoteAccessNotAfter         = null,

                                                     Boolean?                                                   PreferIPv4                   = null,
                                                     RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                     LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                     X509Certificate?                                           ClientCert                   = null,
                                                     SslProtocols?                                              TLSProtocol                  = null,
                                                     String?                                                    HTTPUserAgent                = null,
                                                     TimeSpan?                                                  RequestTimeout               = null,
                                                     TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                     UInt16?                                                    MaxNumberOfRetries           = null,
                                                     UInt32?                                                    InternalBufferSize           = null,
                                                     Boolean?                                                   UseHTTPPipelining            = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)

        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.CountryCode,
                                                 ExistingRemoteParty.PartyId,
                                                 ExistingRemoteParty.Role,
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
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                                                     BusinessDetails                                            BusinessDetails,

                                                     AccessToken                                                AccessToken,
                                                     DateTime?                                                  LocalAccessNotBefore         = null,
                                                     DateTime?                                                  LocalAccessNotAfter          = null,
                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = false,
                                                     AccessStatus                                               AccessStatus                 = AccessStatus.ALLOWED,

                                                     PartyStatus                                                PartyStatus                  = PartyStatus. ENABLED,

                                                     Boolean?                                                   PreferIPv4                   = null,
                                                     RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                     LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                     X509Certificate?                                           ClientCert                   = null,
                                                     SslProtocols?                                              TLSProtocol                  = null,
                                                     String?                                                    HTTPUserAgent                = null,
                                                     TimeSpan?                                                  RequestTimeout               = null,
                                                     TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                     UInt16?                                                    MaxNumberOfRetries           = null,
                                                     UInt32?                                                    InternalBufferSize           = null,
                                                     Boolean?                                                   UseHTTPPipelining            = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.CountryCode,
                                                 ExistingRemoteParty.PartyId,
                                                 ExistingRemoteParty.Role,
                                                 BusinessDetails,

                                                 AccessToken,
                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 AccessStatus,

                                                 PartyStatus,
                                                 LocalAccessNotBefore,
                                                 LocalAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                           ExistingRemoteParty,
                                                     BusinessDetails                       BusinessDetails,

                                                     AccessToken                                                RemoteAccessToken,
                                                     URL                                                        RemoteVersionsURL,
                                                     IEnumerable<Version_Id>?                                   RemoteVersionIds             = null,
                                                     Version_Id?                                                SelectedVersionId            = null,

                                                     Boolean?                                                   AccessTokenBase64Encoding    = null,
                                                     Boolean?                                                   AllowDowngrades              = null,
                                                     RemoteAccessStatus?                                        RemoteStatus                 = RemoteAccessStatus.UNKNOWN,
                                                     PartyStatus                                                PartyStatus                  = PartyStatus.       ENABLED,
                                                     DateTime?                                                  RemoteAccessNotBefore        = null,
                                                     DateTime?                                                  RemoteAccessNotAfter         = null,

                                                     Boolean?                                                   PreferIPv4                   = null,
                                                     RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                     LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                     X509Certificate?                                           ClientCert                   = null,
                                                     SslProtocols?                                              TLSProtocol                  = null,
                                                     String?                                                    HTTPUserAgent                = null,
                                                     TimeSpan?                                                  RequestTimeout               = null,
                                                     TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                     UInt16?                                                    MaxNumberOfRetries           = null,
                                                     UInt32?                                                    InternalBufferSize           = null,
                                                     Boolean?                                                   UseHTTPPipelining            = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.CountryCode,
                                                 ExistingRemoteParty.PartyId,
                                                 ExistingRemoteParty.Role,
                                                 BusinessDetails,

                                                 RemoteAccessToken,
                                                 RemoteVersionsURL,
                                                 RemoteVersionIds,
                                                 SelectedVersionId,

                                                 AccessTokenBase64Encoding,
                                                 AllowDowngrades,
                                                 RemoteStatus,
                                                 PartyStatus,
                                                 RemoteAccessNotBefore,
                                                 RemoteAccessNotAfter,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UpdateRemoteParty(...)

        public async Task<Boolean> UpdateRemoteParty(RemoteParty                                                ExistingRemoteParty,
                                                     BusinessDetails                                            BusinessDetails,

                                                     IEnumerable<LocalAccessInfo>                               LocalAccessInfos,
                                                     IEnumerable<RemoteAccessInfo>                              RemoteAccessInfos,

                                                     PartyStatus                                                Status                       = PartyStatus.ENABLED,

                                                     Boolean?                                                   PreferIPv4                   = null,
                                                     RemoteTLSServerCertificateValidationHandler<IHTTPClient>?  RemoteCertificateValidator   = null,
                                                     LocalCertificateSelectionHandler?                          LocalCertificateSelector     = null,
                                                     X509Certificate?                                           ClientCert                   = null,
                                                     SslProtocols?                                              TLSProtocol                  = null,
                                                     String?                                                    HTTPUserAgent                = null,
                                                     TimeSpan?                                                  RequestTimeout               = null,
                                                     TransmissionRetryDelayDelegate?                            TransmissionRetryDelay       = null,
                                                     UInt16?                                                    MaxNumberOfRetries           = null,
                                                     UInt32?                                                    InternalBufferSize           = null,
                                                     Boolean?                                                   UseHTTPPipelining            = null,

                                                     DateTime?                                                  LastUpdated                  = null,

                                                     EventTracking_Id?                                          EventTrackingId              = null,
                                                     User_Id?                                                   CurrentUserId                = null)
        {

            var newRemoteParty = new RemoteParty(ExistingRemoteParty.CountryCode,
                                                 ExistingRemoteParty.PartyId,
                                                 ExistingRemoteParty.Role,
                                                 BusinessDetails,

                                                 LocalAccessInfos,
                                                 RemoteAccessInfos,

                                                 Status,

                                                 PreferIPv4,
                                                 RemoteCertificateValidator,
                                                 LocalCertificateSelector,
                                                 ClientCert,
                                                 TLSProtocol,
                                                 HTTPUserAgent,
                                                 RequestTimeout,
                                                 TransmissionRetryDelay,
                                                 MaxNumberOfRetries,
                                                 InternalBufferSize,
                                                 UseHTTPPipelining,

                                                 LastUpdated);

            if (remoteParties.TryUpdate(newRemoteParty.Id,
                                        newRemoteParty,
                                        ExistingRemoteParty))
            {

                await LogRemoteParty(updateRemoteParty,
                                     newRemoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion


        #region ContainsRemoteParty(RemotePartyId)

        /// <summary>
        /// Whether this API contains a remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public Boolean ContainsRemoteParty(RemoteParty_Id RemotePartyId)

            => remoteParties.ContainsKey(RemotePartyId);

        #endregion

        #region GetRemoteParty     (RemotePartyId)

        /// <summary>
        /// Get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        public RemoteParty? GetRemoteParty(RemoteParty_Id RemotePartyId)
        {

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty))
                return remoteParty;

            return null;

        }

        #endregion

        #region TryGetRemoteParty  (RemotePartyId, out RemoteParty)

        /// <summary>
        /// Try to get the remote party having the given unique identification.
        /// </summary>
        /// <param name="RemotePartyId">The unique identification of the remote party.</param>
        /// <param name="RemoteParty">The remote party.</param>
        public Boolean TryGetRemoteParty(RemoteParty_Id    RemotePartyId,
                                         out RemoteParty?  RemoteParty)

            => remoteParties.TryGetValue(RemotePartyId,
                                         out RemoteParty);

        #endregion

        #region GetRemoteParties   (IncludeFilter = null)

        /// <summary>
        /// Get all remote parties machting the given optional filter.
        /// </summary>
        /// <param name="IncludeFilter">A delegate for filtering remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(IncludeRemoteParty? IncludeFilter = null)

            => IncludeFilter is null
                   ? remoteParties.Values
                   : remoteParties.Values.
                                   Where(remoteParty => IncludeFilter(remoteParty));

        #endregion

        #region GetRemoteParties   (CountryCode, PartyId)

        /// <summary>
        /// Get all remote parties having the given country code and party identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Id     PartyId)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                  remoteParty.PartyId     == PartyId);

        #endregion

        #region GetRemoteParties   (CountryCode, PartyId, Role)

        /// <summary>
        /// Get all remote parties having the given country code, party identification and role.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A role.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(CountryCode  CountryCode,
                                                         Party_Id     PartyId,
                                                         Roles        Role)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                  remoteParty.PartyId     == PartyId     &&
                                                  remoteParty.Role        == Role);

        #endregion

        #region GetRemoteParties   (Role)

        /// <summary>
        /// Get all remote parties having the given role.
        /// </summary>
        /// <param name="Role">The role of the remote parties.</param>
        public IEnumerable<RemoteParty> GetRemoteParties(Roles Role)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.Role == Role);

        #endregion

        #region GetRemoteParties   (AccessToken)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken AccessToken)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(accessInfo => accessInfo.AccessToken == AccessToken));

        #endregion

        #region GetRemoteParties   (AccessToken, AccessStatus)

        public IEnumerable<RemoteParty> GetRemoteParties(AccessToken   AccessToken,
                                                         AccessStatus  AccessStatus)

            => remoteParties.Values.
                             Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken &&
                                                                                                      localAccessInfo.Status      == AccessStatus));

        #endregion

        #region GetRemoteParties   (AccessToken, out RemoteParties)

        public Boolean TryGetRemoteParties(AccessToken                   AccessToken,
                                           out IEnumerable<RemoteParty>  RemoteParties)
        {

            RemoteParties = remoteParties.Values.
                                          Where(remoteParty => remoteParty.LocalAccessInfos.Any(localAccessInfo => localAccessInfo.AccessToken == AccessToken));

            return RemoteParties.Any();

        }

        #endregion


        #region RemoveRemoteParty(RemoteParty)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty        RemoteParty,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.TryRemove(RemoteParty.Id, out var remoteParty))
            {

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty(RemotePartyId)

        public async Task<Boolean> RemoveRemoteParty(RemoteParty_Id     RemotePartyId,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            if (remoteParties.Remove(RemotePartyId, out var remoteParty))
            {

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, Role)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     Roles              Role,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in GetRemoteParties(CountryCode,
                                                         PartyId,
                                                         Role))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

            }

            return true;

        }

        #endregion

        #region RemoveRemoteParty(CountryCode, PartyId, AccessToken)

        public async Task<Boolean> RemoveRemoteParty(CountryCode        CountryCode,
                                                     Party_Id           PartyId,
                                                     AccessToken        AccessToken,
                                                     EventTracking_Id?  EventTrackingId   = null,
                                                     User_Id?           CurrentUserId     = null)
        {

            foreach (var remoteParty in remoteParties.Values.Where(remoteParty => remoteParty.CountryCode == CountryCode &&
                                                                                  remoteParty.PartyId     == PartyId     &&
                                                                                  remoteParty.RemoteAccessInfos.Any(remoteAccessInfo => remoteAccessInfo.AccessToken == AccessToken)))
            {

                remoteParties.TryRemove(remoteParty.Id, out _);

                await LogRemoteParty(removeRemoteParty,
                                     remoteParty.ToJSON(true),
                                     EventTrackingId ?? EventTracking_Id.New,
                                     CurrentUserId);

            }

            return true;

        }

        #endregion

        #region RemoveAllRemoteParties()

        public async Task RemoveAllRemoteParties(EventTracking_Id?  EventTrackingId   = null,
                                                 User_Id?           CurrentUserId     = null)
        {

            remoteParties.Clear();

            await LogRemoteParty("removeAllRemoteParties",
                                 EventTrackingId ?? EventTracking_Id.New,
                                 CurrentUserId);

        }

        #endregion

        #endregion

        #region CPOClients

        private readonly ConcurrentDictionary<EMSP_Id, CPOClient> cpoClients = new ();

        /// <summary>
        /// Return an enumeration of all CPO clients.
        /// </summary>
        public IEnumerable<CPOClient> CPOClients
            => cpoClients.Values;


        #region GetCPOClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(CountryCode  CountryCode,
                                       Party_Id     PartyId,
                                       I18NString?  Description          = null,
                                       Boolean      AllowCachedClients   = true)
        {

            var emspId         = EMSP_Id.       Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (emspId);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (remoteParties.TryGetValue(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(remotePartyId),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                    ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                    ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(RemoteParty  RemoteParty,
                                       I18NString?  Description          = null,
                                       Boolean      AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    RemoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                    ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                    ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #region GetCPOClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a CPO create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached CPO clients.</param>
        public CPOClient? GetCPOClient(RemoteParty_Id  RemotePartyId,
                                       I18NString?     Description          = null,
                                       Boolean         AllowCachedClients   = true)
        {

            var emspId = EMSP_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                cpoClients.TryGetValue(emspId, out var cachedCPOClient))
            {
                return cachedCPOClient;
            }

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var cpoClient = new CPOClient(
                                    this,
                                    remoteParty,
                                    null,
                                    Description ?? ClientConfigurations.Description?.Invoke(RemotePartyId),
                                    null,
                                    ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                    ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                    ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                    ClientConfigurations.LogfileCreator,
                                    DNSClient
                                );

                cpoClients.TryAdd(emspId, cpoClient);

                return cpoClient;

            }

            return null;

        }

        #endregion

        #endregion

        #region EMSPClients

        private readonly ConcurrentDictionary<CPO_Id, EMSPClient> emspClients = new ();

        /// <summary>
        /// Return an enumeration of all EMSP clients.
        /// </summary>
        public IEnumerable<EMSPClient> EMSPClients
            => emspClients.Values;


        #region GetEMSPClient(CountryCode, PartyId, Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="CountryCode">The country code of the remote EMSP.</param>
        /// <param name="PartyId">The party identification of the remote EMSP.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(CountryCode  CountryCode,
                                         Party_Id     PartyId,
                                         I18NString?  Description          = null,
                                         Boolean      AllowCachedClients   = true)
        {

            var cpoId          = CPO_Id.        Parse(CountryCode, PartyId);
            var remotePartyId  = RemoteParty_Id.From (cpoId);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (remoteParties.TryGetValue(remotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     remoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(remotePartyId),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(remotePartyId),
                                     ClientConfigurations.LoggingPath?.   Invoke(remotePartyId),
                                     ClientConfigurations.LoggingContext?.Invoke(remotePartyId),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient(RemoteParty,          Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemoteParty">A remote party.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(RemoteParty  RemoteParty,
                                         I18NString?  Description          = null,
                                         Boolean      AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemoteParty.Id);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (RemoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     RemoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(RemoteParty.Id),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(RemoteParty.Id),
                                     ClientConfigurations.LoggingPath?.   Invoke(RemoteParty.Id),
                                     ClientConfigurations.LoggingContext?.Invoke(RemoteParty.Id),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #region GetEMSPClient(RemotePartyId,        Description = null, AllowCachedClients = true)

        /// <summary>
        /// As a EMSP create a client to access e.g. a remote EMSP.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        /// <param name="Description">A description for the OCPI client.</param>
        /// <param name="AllowCachedClients">Whether to allow to return cached EMSP clients.</param>
        public EMSPClient? GetEMSPClient(RemoteParty_Id  RemotePartyId,
                                         I18NString?     Description          = null,
                                         Boolean         AllowCachedClients   = true)
        {

            var cpoId = CPO_Id.From(RemotePartyId);

            if (AllowCachedClients &&
                emspClients.TryGetValue(cpoId, out var cachedEMSPClient))
            {
                return cachedEMSPClient;
            }

            if (remoteParties.TryGetValue(RemotePartyId, out var remoteParty) &&
                remoteParty?.RemoteAccessInfos?.Any() == true)
            {

                var emspClient = new EMSPClient(
                                     this,
                                     remoteParty,
                                     null,
                                     Description ?? ClientConfigurations.Description?.Invoke(RemotePartyId),
                                     null,
                                     ClientConfigurations.DisableLogging?.Invoke(RemotePartyId),
                                     ClientConfigurations.LoggingPath?.   Invoke(RemotePartyId),
                                     ClientConfigurations.LoggingContext?.Invoke(RemotePartyId),
                                     ClientConfigurations.LogfileCreator,
                                     DNSClient
                                 );

                emspClients.TryAdd(cpoId, emspClient);

                return emspClient;

            }

            return null;

        }

        #endregion

        #endregion


        #region Locations

        #region Data

        private readonly ConcurrentDictionary<Location_Id , Location> locations = new();

        public delegate Task OnLocationAddedDelegate    (Location  Location);
        public delegate Task OnLocationChangedDelegate  (Location  Location);
        public delegate Task OnLocationRemovedDelegate  (Location  Location);
        public delegate Task OnEVSEAddedDelegate        (EVSE      EVSE);
        public delegate Task OnEVSEChangedDelegate      (EVSE      EVSE);
        public delegate Task OnEVSEStatusChangedDelegate(DateTime  Timestamp, EVSE EVSE, StatusType NewEVSEStatus, StatusType? OldEVSEStatus = null);
        public delegate Task OnEVSERemovedDelegate      (EVSE      EVSE);

        public event OnLocationAddedDelegate?      OnLocationAdded;
        public event OnLocationChangedDelegate?    OnLocationChanged;
        public event OnLocationRemovedDelegate?    OnLocationRemoved;
        public event OnEVSEAddedDelegate?          OnEVSEAdded;
        public event OnEVSEChangedDelegate?        OnEVSEChanged;
        public event OnEVSEStatusChangedDelegate?  OnEVSEStatusChanged;
        public event OnEVSERemovedDelegate?        OnEVSERemoved;

        #endregion


        #region AddLocation            (Location,                                                           SkipNotifications = false)

        public async Task<AddResult<Location>> AddLocation(Location           Location,
                                                           Boolean            SkipNotifications   = false,
                                                           EventTracking_Id?  EventTrackingId     = null,
                                                           User_Id?           CurrentUserId       = null)
        {

            if (locations.TryAdd(Location.Id, Location))
            {

                DebugX.Log($"OCPI v2.1.1 Location '{Location.Id}': '{Location}' added...");

                Location.CommonAPI = this;

                await LogAsset(addLocation,
                               Location.ToJSON(true,
                                               true,
                                               null,
                                               CustomLocationSerializer,
                                               CustomAdditionalGeoLocationSerializer,
                                               CustomEVSESerializer,
                                               CustomStatusScheduleSerializer,
                                               CustomConnectorSerializer,
                                               CustomEnergyMeterSerializer,
                                               CustomTransparencySoftwareStatusSerializer,
                                               CustomTransparencySoftwareSerializer,
                                               CustomDisplayTextSerializer,
                                               CustomBusinessDetailsSerializer,
                                               CustomHoursSerializer,
                                               CustomImageSerializer,
                                               CustomEnergyMixSerializer,
                                               CustomEnergySourceSerializer,
                                               CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnLocationAddedLocal = OnLocationAdded;
                    if (OnLocationAddedLocal is not null)
                    {
                        try
                        {
                            await OnLocationAddedLocal(Location);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var OnEVSEAddedLocal = OnEVSEAdded;
                    if (OnEVSEAddedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in Location.EVSEs)
                                await OnEVSEAddedLocal(evse);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnEVSEAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Location>.Success(Location);

            }

            //DebugX.Log($"OCPI v2.1.1 Adding Location '{Location.Id}': '{Location}' failed!");

            return AddResult<Location>.Failed(Location,
                                              "TryAdd(Location.Id, Location) failed!");

        }

        #endregion

        #region AddLocationIfNotExists (Location,                                                           SkipNotifications = false)

        public async Task<AddResult<Location>> AddLocationIfNotExists(Location           Location,
                                                                      Boolean            SkipNotifications   = false,
                                                                      EventTracking_Id?  EventTrackingId     = null,
                                                                      User_Id?           CurrentUserId       = null)
        {

            if (locations.TryAdd(Location.Id, Location))
            {

                Location.CommonAPI = this;

                await LogAsset(addLocationIfNotExists,
                               Location.ToJSON(true,
                                               true,
                                               null,
                                               CustomLocationSerializer,
                                               CustomAdditionalGeoLocationSerializer,
                                               CustomEVSESerializer,
                                               CustomStatusScheduleSerializer,
                                               CustomConnectorSerializer,
                                               CustomEnergyMeterSerializer,
                                               CustomTransparencySoftwareStatusSerializer,
                                               CustomTransparencySoftwareSerializer,
                                               CustomDisplayTextSerializer,
                                               CustomBusinessDetailsSerializer,
                                               CustomHoursSerializer,
                                               CustomImageSerializer,
                                               CustomEnergyMixSerializer,
                                               CustomEnergySourceSerializer,
                                               CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnLocationAddedLocal = OnLocationAdded;
                    if (OnLocationAddedLocal is not null)
                    {
                        try
                        {
                            OnLocationAddedLocal(Location).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocationIfNotExists), " ", nameof(OnLocationAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var OnEVSEAddedLocal = OnEVSEAdded;
                    if (OnEVSEAddedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in Location.EVSEs)
                                await OnEVSEAddedLocal(evse);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocationIfNotExists), " ", nameof(OnEVSEAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Location>.Success(Location);

            }

            return AddResult<Location>.NoOperation(Location);

        }

        #endregion

        #region AddOrUpdateLocation    (Location,                                  AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<Location>> AddOrUpdateLocation(Location           Location,
                                                                           Boolean?           AllowDowngrades     = false,
                                                                           Boolean            SkipNotifications   = false,
                                                                           EventTracking_Id?  EventTrackingId     = null,
                                                                           User_Id?           CurrentUserId       = null)
        {

            #region Update an existing location

            if (locations.TryGetValue(Location.Id, out var existingLocation))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Location.LastUpdated <= existingLocation.LastUpdated)
                {
                    return AddOrUpdateResult<Location>.Failed(Location,
                                                              "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");
                }

                //if (Location.LastUpdated.ToIso8601() == existingLocation.LastUpdated.ToIso8601())
                //    return AddOrUpdateResult<Location>.NoOperation(Location,
                //                                                   "The 'lastUpdated' timestamp of the new location must be newer then the timestamp of the existing location!");

                if (locations.TryUpdate(Location.Id,
                                        Location,
                                        existingLocation))
                {

                    Location.CommonAPI = this;

                    await LogAsset(addOrUpdateLocation,
                                   Location.ToJSON(true,
                                                   true,
                                                   null,
                                                   CustomLocationSerializer,
                                                   CustomAdditionalGeoLocationSerializer,
                                                   CustomEVSESerializer,
                                                   CustomStatusScheduleSerializer,
                                                   CustomConnectorSerializer,
                                                   CustomEnergyMeterSerializer,
                                                   CustomTransparencySoftwareStatusSerializer,
                                                   CustomTransparencySoftwareSerializer,
                                                   CustomDisplayTextSerializer,
                                                   CustomBusinessDetailsSerializer,
                                                   CustomHoursSerializer,
                                                   CustomImageSerializer,
                                                   CustomEnergyMixSerializer,
                                                   CustomEnergySourceSerializer,
                                                   CustomEnvironmentalImpactSerializer),
                                   EventTrackingId ?? EventTracking_Id.New,
                                   CurrentUserId);

                    if (!SkipNotifications)
                    {

                        var OnLocationChangedLocal = OnLocationChanged;
                        if (OnLocationChangedLocal is not null)
                        {
                            try
                            {
                                OnLocationChangedLocal(Location).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                        var oldEVSEUIds = new HashSet<EVSE_UId>(existingLocation.EVSEs.Select(evse => evse.UId));
                        var newEVSEUIds = new HashSet<EVSE_UId>(Location.        EVSEs.Select(evse => evse.UId));

                        foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                        {

                            if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId) && existingLocation.GetEVSE(evseUId)! != Location.GetEVSE(evseUId)!)
                            {
                                var OnEVSEChangedLocal = OnEVSEChanged;
                                if (OnEVSEChangedLocal is not null)
                                {
                                    try
                                    {
                                        await OnEVSEChangedLocal(existingLocation.GetEVSE(evseUId)!);
                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                    Environment.NewLine, e.Message,
                                                    Environment.NewLine, e.StackTrace ?? "");
                                    }
                                }
                            }
                            else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                            {
                                var OnEVSEAddedLocal = OnEVSEAdded;
                                if (OnEVSEAddedLocal is not null)
                                {
                                    try
                                    {
                                        await OnEVSEAddedLocal(existingLocation.GetEVSE(evseUId)!);
                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                                                    Environment.NewLine, e.Message,
                                                    Environment.NewLine, e.StackTrace ?? "");
                                    }
                                }
                            }
                            else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                            {
                                var OnEVSERemovedLocal = OnEVSERemoved;
                                if (OnEVSERemovedLocal is not null)
                                {
                                    try
                                    {
                                        await OnEVSERemovedLocal(existingLocation.GetEVSE(evseUId)!);
                                    }
                                    catch (Exception e)
                                    {
                                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSERemoved), ": ",
                                                    Environment.NewLine, e.Message,
                                                    Environment.NewLine, e.StackTrace ?? "");
                                    }
                                }
                            }

                        }

                    }

                    return AddOrUpdateResult<Location>.Success(Location,
                                                               WasCreated: false);

                }
                return AddOrUpdateResult<Location>.Failed(Location,
                                                          "locations.TryUpdate(Location.Id, Location, Location) failed!");

            }

            #endregion

            #region Add a new location

            if (locations.TryAdd(Location.Id, Location))
            {

                Location.CommonAPI = this;

                await LogAsset(addOrUpdateLocation,
                               Location.ToJSON(true,
                                               true,
                                               null,
                                               CustomLocationSerializer,
                                               CustomAdditionalGeoLocationSerializer,
                                               CustomEVSESerializer,
                                               CustomStatusScheduleSerializer,
                                               CustomConnectorSerializer,
                                               CustomEnergyMeterSerializer,
                                               CustomTransparencySoftwareStatusSerializer,
                                               CustomTransparencySoftwareSerializer,
                                               CustomDisplayTextSerializer,
                                               CustomBusinessDetailsSerializer,
                                               CustomHoursSerializer,
                                               CustomImageSerializer,
                                               CustomEnergyMixSerializer,
                                               CustomEnergySourceSerializer,
                                               CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnLocationAddedLocal = OnLocationAdded;
                    if (OnLocationAddedLocal is not null)
                    {
                        try
                        {
                            OnLocationAddedLocal(Location).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnLocationAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var OnEVSEAddedLocal = OnEVSEAdded;
                    if (OnEVSEAddedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in Location.EVSEs)
                                await OnEVSEAddedLocal(evse);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Location>.Success(Location,
                                                           WasCreated: true);

            }

            return AddOrUpdateResult<Location>.Failed(Location,
                                                      "locations.TryAdd(Location.Id, Location) failed!");

            #endregion

        }

        #endregion

        #region UpdateLocation         (Location,                                  AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<Location>> UpdateLocation(Location           Location,
                                                                 Boolean?           AllowDowngrades     = false,
                                                                 Boolean            SkipNotifications   = false,
                                                                 EventTracking_Id?  EventTrackingId     = null,
                                                                 User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (locations.TryGetValue(Location.Id, out var existingLocation))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Location.LastUpdated <= existingLocation.LastUpdated)
                {

                    return UpdateResult<Location>.Failed(Location,
                                                         "The 'lastUpdated' timestamp of the new charging location must be newer then the timestamp of the existing location!");

                }

            }
            else
                return UpdateResult<Location>.Failed(Location,
                                                     $"Unknown charging location identification '{Location.Id}'!");

            #endregion


            if (locations.TryUpdate(Location.Id,
                                    Location,
                                    existingLocation))
            {

                Location.CommonAPI = this;

                await LogAsset(updateLocation,
                               Location.ToJSON(true,
                                               true,
                                               null,
                                               CustomLocationSerializer,
                                               CustomAdditionalGeoLocationSerializer,
                                               CustomEVSESerializer,
                                               CustomStatusScheduleSerializer,
                                               CustomConnectorSerializer,
                                               CustomEnergyMeterSerializer,
                                               CustomTransparencySoftwareStatusSerializer,
                                               CustomTransparencySoftwareSerializer,
                                               CustomDisplayTextSerializer,
                                               CustomBusinessDetailsSerializer,
                                               CustomHoursSerializer,
                                               CustomImageSerializer,
                                               CustomEnergyMixSerializer,
                                               CustomEnergySourceSerializer,
                                               CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnLocationChangedLocal = OnLocationChanged;
                    if (OnLocationChangedLocal is not null)
                    {
                        try
                        {
                            OnLocationChangedLocal(Location).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnLocationChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var oldEVSEUIds = new HashSet<EVSE_UId>(existingLocation.EVSEs.Select(evse => evse.UId));
                    var newEVSEUIds = new HashSet<EVSE_UId>(Location.        EVSEs.Select(evse => evse.UId));

                    foreach (var evseUId in new HashSet<EVSE_UId>(oldEVSEUIds.Union(newEVSEUIds)))
                    {

                        if      ( oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                        {

                            if (existingLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                                Location.        TryGetEVSE(evseUId, out var newEVSE) &&
                                oldEVSE is not null &&
                                newEVSE is not null)
                            {

                                if (oldEVSE != newEVSE)
                                {
                                    var OnEVSEChangedLocal = OnEVSEChanged;
                                    if (OnEVSEChangedLocal is not null)
                                    {
                                        try
                                        {
                                            await OnEVSEChangedLocal(newEVSE);
                                        }
                                        catch (Exception e)
                                        {
                                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                        Environment.NewLine, e.Message,
                                                        Environment.NewLine, e.StackTrace ?? "");
                                        }
                                    }
                                }

                                if (oldEVSE.Status != newEVSE.Status)
                                {
                                    var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                                    if (OnEVSEStatusChangedLocal is not null)
                                    {
                                        try
                                        {
                                            await OnEVSEStatusChangedLocal(Timestamp.Now,
                                                                           newEVSE,
                                                                           newEVSE.Status,
                                                                           oldEVSE.Status);
                                        }
                                        catch (Exception e)
                                        {
                                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                        Environment.NewLine, e.Message,
                                                        Environment.NewLine, e.StackTrace ?? "");
                                        }
                                    }
                                }

                            }

                        }
                        else if (!oldEVSEUIds.Contains(evseUId) &&  newEVSEUIds.Contains(evseUId))
                        {

                            var OnEVSEAddedLocal = OnEVSEAdded;
                            if (OnEVSEAddedLocal is not null)
                            {
                                try
                                {
                                    if (Location.TryGetEVSE(evseUId, out var evse) &&
                                        evse is not null)
                                    {
                                        await OnEVSEAddedLocal(evse);
                                    }
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEAdded), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                            var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                            if (OnEVSEStatusChangedLocal is not null)
                            {
                                try
                                {
                                    if (Location.TryGetEVSE(evseUId, out var evse) &&
                                        evse is not null)
                                    {
                                        await OnEVSEStatusChangedLocal(Timestamp.Now,
                                                                       evse,
                                                                       evse.Status);
                                    }
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                        }
                        else if ( oldEVSEUIds.Contains(evseUId) && !newEVSEUIds.Contains(evseUId))
                        {

                            var OnEVSERemovedLocal = OnEVSERemoved;
                            if (OnEVSERemovedLocal is not null)
                            {
                                try
                                {
                                    if (existingLocation.TryGetEVSE(evseUId, out var evse) &&
                                        evse is not null)
                                    {
                                        await OnEVSERemovedLocal(evse);
                                    }
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSERemoved), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                            var OnEVSEStatusChangedLocal = OnEVSEStatusChanged;
                            if (OnEVSEStatusChangedLocal is not null)
                            {
                                try
                                {
                                    if (existingLocation.TryGetEVSE(evseUId, out var oldEVSE) &&
                                        Location.        TryGetEVSE(evseUId, out var newEVSE) &&
                                        oldEVSE is not null &&
                                        newEVSE is not null)
                                    {
                                        await OnEVSEStatusChangedLocal(Timestamp.Now,
                                                                       oldEVSE,
                                                                       newEVSE.Status,
                                                                       oldEVSE.Status);
                                    }
                                }
                                catch (Exception e)
                                {
                                    DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateLocation), " ", nameof(OnEVSEChanged), ": ",
                                                Environment.NewLine, e.Message,
                                                Environment.NewLine, e.StackTrace ?? "");
                                }
                            }

                        }

                    }

                }

                return UpdateResult<Location>.Success(Location);

            }

            return UpdateResult<Location>.Failed(Location,
                                                 "locations.TryUpdate(Location.Id, Location, Location) failed!");

        }

        #endregion

        #region TryPatchLocation       (Location, LocationPatch,                   AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<Location>> TryPatchLocation(Location           Location,
                                                                  JObject            LocationPatch,
                                                                  Boolean?           AllowDowngrades     = false,
                                                                  Boolean            SkipNotifications   = false,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  User_Id?           CurrentUserId       = null)
        {

            if (!LocationPatch.HasValues)
                return PatchResult<Location>.Failed(Location,
                                                    "The given location patch must not be null or empty!");

            var patchResult = Location.TryPatch(LocationPatch,
                                                AllowDowngrades ?? this.AllowDowngrades ?? false);

            if (patchResult.IsSuccess &&
                patchResult.PatchedData is not null)
            {

                var updateLocationResult = await UpdateLocation(patchResult.PatchedData,
                                                                AllowDowngrades,
                                                                SkipNotifications,
                                                                EventTrackingId,
                                                                CurrentUserId);

                if (updateLocationResult.IsFailed)
                    return PatchResult<Location>.Failed(Location,
                                                        updateLocationResult.ErrorResponse ?? "Unknown error!");

            }

            return patchResult;

        }

        #endregion


        #region AddEVSE                (Location, EVSE,                                                     SkipNotifications = false)

        public async Task<AddResult<EVSE>> AddEVSE(Location           Location,
                                                   EVSE               EVSE,
                                                   Boolean            SkipNotifications   = false,
                                                   EventTracking_Id?  EventTrackingId     = null,
                                                   User_Id?           CurrentUserId       = null)
        {

            if (Location.EVSEExists(EVSE.UId))
                return AddResult<EVSE>.Failed(EVSE,
                                              $"The given EVSE '{EVSE.UId}' already exists!");

            DebugX.Log($"OCPI v2.1.1 EVSE '{EVSE.UId}'/'{EVSE.EVSEId}': '{EVSE}' added...");

            var newLocation = Location.Update(locationBuilder => {
                                                  locationBuilder.SetEVSE(EVSE);
                                                  locationBuilder.LastUpdated  = EVSE.LastUpdated;
                                              },
                                              out var warnings);

            if (newLocation is null)
                return AddResult<EVSE>.Failed(
                           EVSE,
                           warnings.First().Text.FirstText()
                       );


            var updateLocationResult = await UpdateLocation(
                                                 newLocation,
                                                 AllowDowngrades,
                                                 SkipNotifications,
                                                 EventTrackingId,
                                                 CurrentUserId
                                             );


            return updateLocationResult.IsSuccess

                       ? AddResult<EVSE>.Success(
                             EVSE
                         )

                       : AddResult<EVSE>.Failed (
                             EVSE,
                             updateLocationResult.ErrorResponse ?? "Unknown error!"
                         );

        }

        #endregion

        #region AddEVSEIfNotExists     (Location, EVSE,                                                     SkipNotifications = false)

        public async Task<AddResult<EVSE>> AddEVSEIfNotExists(Location           Location,
                                                              EVSE               EVSE,
                                                              Boolean            SkipNotifications   = false,
                                                              EventTracking_Id?  EventTrackingId     = null,
                                                              User_Id?           CurrentUserId       = null)
        {

            if (Location.EVSEExists(EVSE.UId))
                return AddResult<EVSE>.Failed(EVSE,
                                              $"The given EVSE '{EVSE.UId}' already exists!");


            var newLocation = Location.Update(locationBuilder => {
                                                  locationBuilder.SetEVSE(EVSE);
                                                  locationBuilder.LastUpdated  = EVSE.LastUpdated;
                                              },
                                              out var warnings);

            if (newLocation is null)
                return AddResult<EVSE>.Failed(EVSE,
                                              warnings.First().Text.FirstText());


            var updateLocationResult = await UpdateLocation(newLocation,
                                                            AllowDowngrades,
                                                            SkipNotifications,
                                                            EventTrackingId,
                                                            CurrentUserId);

            return updateLocationResult.IsSuccess
                       ? AddResult<EVSE>.Success    (EVSE)
                       : AddResult<EVSE>.NoOperation(EVSE,
                                                     updateLocationResult.ErrorResponse);

        }

        #endregion

        #region AddOrUpdateEVSE        (Location, EVSE,                            AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<EVSE>> AddOrUpdateEVSE(Location           Location,
                                                                   EVSE               EVSE,
                                                                   Boolean?           AllowDowngrades     = false,
                                                                   Boolean            SkipNotifications   = false,
                                                                   EventTracking_Id?  EventTrackingId     = null,
                                                                   User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (Location.TryGetEVSE(EVSE.UId, out var existingEVSE) &&
                existingEVSE is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    EVSE.LastUpdated <= existingEVSE.LastUpdated)
                {
                    return AddOrUpdateResult<EVSE>.Failed     (EVSE,
                                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");
                }

                //if (EVSE.LastUpdated.ToIso8601() == existingEVSE.LastUpdated.ToIso8601())
                //    return AddOrUpdateResult<EVSE>.NoOperation(EVSE,
                //                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");

            }

            #endregion

            var newLocation = Location.Update(locationBuilder => {

                                                  if (EVSE.Status != StatusType.REMOVED || KeepRemovedEVSEs(EVSE))
                                                      locationBuilder.SetEVSE(EVSE);
                                                  else
                                                      locationBuilder.RemoveEVSE(EVSE);

                                                  locationBuilder.LastUpdated  = EVSE.LastUpdated;

                                              },
                                              out var warnings);

            if (newLocation is null)
                return AddOrUpdateResult<EVSE>.Failed(EVSE,
                                                      warnings.First().Text.FirstText());


            var updateLocationResult = await UpdateLocation(newLocation,
                                                            (AllowDowngrades ?? this.AllowDowngrades) == false,
                                                            SkipNotifications,
                                                            EventTrackingId,
                                                            CurrentUserId);

            return updateLocationResult.IsSuccess
                       ? AddOrUpdateResult<EVSE>.Success(EVSE,
                                                         WasCreated: existingEVSE is null)
                       : AddOrUpdateResult<EVSE>.Failed (EVSE,
                                                         updateLocationResult.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region UpdateEVSE             (Location, EVSE,                            AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<EVSE>> UpdateEVSE(Location           Location,
                                                         EVSE               EVSE,
                                                         Boolean?           AllowDowngrades     = false,
                                                         Boolean            SkipNotifications   = false,
                                                         EventTracking_Id?  EventTrackingId     = null,
                                                         User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (Location.TryGetEVSE(EVSE.UId, out var existingEVSE) &&
                existingEVSE is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    EVSE.LastUpdated <= existingEVSE.LastUpdated)
                {
                    return UpdateResult<EVSE>.Failed(EVSE,
                                                     "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");
                }

                //if (EVSE.LastUpdated.ToIso8601() == existingEVSE.LastUpdated.ToIso8601())
                //    return AddOrUpdateResult<EVSE>.NoOperation(EVSE,
                //                                               "The 'lastUpdated' timestamp of the new EVSE must be newer then the timestamp of the existing EVSE!");

            }
            else
                return UpdateResult<EVSE>.Failed(EVSE,
                                                 $"The given EVSE '{EVSE.UId}' does not exist!");

            #endregion

            var newLocation = Location.Update(locationBuilder => {

                                                  if (EVSE.Status != StatusType.REMOVED || KeepRemovedEVSEs(EVSE))
                                                      locationBuilder.SetEVSE(EVSE);
                                                  else
                                                      locationBuilder.RemoveEVSE(EVSE);

                                                  locationBuilder.LastUpdated  = EVSE.LastUpdated;

                                              },
                                              out var warnings);

            if (newLocation is null)
                return UpdateResult<EVSE>.Failed(EVSE,
                                                 warnings.First().Text.FirstText());


            var updateLocationResult = await UpdateLocation(newLocation,
                                                            (AllowDowngrades ?? this.AllowDowngrades) == false,
                                                            SkipNotifications,
                                                            EventTrackingId,
                                                            CurrentUserId);

            return updateLocationResult.IsSuccess
                       ? UpdateResult<EVSE>.Success(EVSE)
                       : UpdateResult<EVSE>.Failed (EVSE,
                                                    updateLocationResult.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region TryPatchEVSE           (Location, EVSE, EVSEPatch,                 AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<EVSE>> TryPatchEVSE(Location           Location,
                                                          EVSE               EVSE,
                                                          JObject            EVSEPatch,
                                                          Boolean?           AllowDowngrades     = false,
                                                          Boolean            SkipNotifications   = false,
                                                          EventTracking_Id?  EventTrackingId     = null,
                                                          User_Id?           CurrentUserId       = null)
        {

            if (!EVSEPatch.HasValues)
                return PatchResult<EVSE>.Failed(EVSE,
                                                "The given EVSE patch must not be null or empty!");

            var patchResult = EVSE.TryPatch(EVSEPatch,
                                            AllowDowngrades ?? this.AllowDowngrades ?? false);

            //var justAStatusChange  = EVSEPatch.Children().Count() == 2 && EVSEPatch.ContainsKey("status") && EVSEPatch.ContainsKey("last_updated");

            if (patchResult.IsSuccess &&
                patchResult.PatchedData is not null)
            {

                var updateEVSEResult = await UpdateEVSE(Location,
                                                        patchResult.PatchedData,
                                                        AllowDowngrades,
                                                        SkipNotifications);

                if (updateEVSEResult.IsFailed)
                    return PatchResult<EVSE>.Failed(EVSE,
                                                    updateEVSEResult.ErrorResponse ?? "Unknown error!");

            }

            return patchResult;

        }

        #endregion


        #region AddConnector           (Location, EVSE, Connector,                                          SkipNotifications = false)

        public async Task<AddResult<Connector>> AddConnector(Location           Location,
                                                             EVSE               EVSE,
                                                             Connector          Connector,
                                                             Boolean            SkipNotifications   = false,
                                                             EventTracking_Id?  EventTrackingId     = null,
                                                             User_Id?           CurrentUserId       = null)
        {

            if (EVSE.ConnectorExists(Connector.Id))
                return AddResult<Connector>.Failed(Connector,
                                                   $"The given charging connector identification '{Connector.Id}' already exists!");


            var newEVSE = EVSE.Update(evseBuilder => {
                                          evseBuilder.SetConnector(Connector);
                                          evseBuilder.LastUpdated = Connector.LastUpdated;
                                      },
                                      out var warnings);

            if (newEVSE is null)
                return AddResult<Connector>.Failed(Connector,
                                                   warnings.First().Text.FirstText());


            var updateEVSEResult = await UpdateEVSE(Location,
                                                    newEVSE,
                                                    (AllowDowngrades ?? this.AllowDowngrades) == false,
                                                    SkipNotifications);

            return updateEVSEResult.IsSuccess
                       ? AddResult<Connector>.Success(Connector)
                       : AddResult<Connector>.Failed (Connector,
                                                      updateEVSEResult.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region AddConnectorIfNotExists(Location, EVSE, Connector,                                          SkipNotifications = false)

        public async Task<AddResult<Connector>> AddConnectorIfNotExists(Location           Location,
                                                                        EVSE               EVSE,
                                                                        Connector          Connector,
                                                                        Boolean            SkipNotifications   = false,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        User_Id?           CurrentUserId       = null)
        {

            if (EVSE.ConnectorExists(Connector.Id))
                return AddResult<Connector>.Failed(Connector,
                                                   $"The given charging connector identification '{Connector.Id}' already exists!");


            var newEVSE = EVSE.Update(evseBuilder => {
                                          evseBuilder.SetConnector(Connector);
                                          evseBuilder.LastUpdated = Connector.LastUpdated;
                                      },
                                      out var warnings);

            if (newEVSE is null)
                return AddResult<Connector>.Failed(Connector,
                                                   warnings.First().Text.FirstText());


            var updateEVSEResult = await UpdateEVSE(Location,
                                                    newEVSE,
                                                    (AllowDowngrades ?? this.AllowDowngrades) == false,
                                                    SkipNotifications);

            return updateEVSEResult.IsSuccess
                       ? AddResult<Connector>.Success    (Connector)
                       : AddResult<Connector>.NoOperation(Connector,
                                                          updateEVSEResult.ErrorResponse);

        }

        #endregion

        #region AddOrUpdateConnector   (Location, EVSE, Connector,                 AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<Connector>> AddOrUpdateConnector(Location           Location,
                                                                             EVSE               EVSE,
                                                                             Connector          Connector,
                                                                             Boolean?           AllowDowngrades     = false,
                                                                             Boolean            SkipNotifications   = false,
                                                                             EventTracking_Id?  EventTrackingId     = null,
                                                                             User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (EVSE.TryGetConnector(Connector.Id, out var existingConnector) &&
                existingConnector is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Connector.LastUpdated <= existingConnector.LastUpdated)
                {
                    return AddOrUpdateResult<Connector>.Failed(Connector,
                                                               "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");
                }

                //if (newOrUpdatedConnector.LastUpdated.ToIso8601() == existingConnector.LastUpdated.ToIso8601())
                //    return AddOrUpdateResult<Connector>.NoOperation(newOrUpdatedConnector,
                //                                                    "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");

            }

            #endregion


            var newEVSE = EVSE.Update(evseBuilder => {
                                          evseBuilder.SetConnector(Connector);
                                          evseBuilder.LastUpdated  = Connector.LastUpdated;
                                      },
                                      out var warnings);

            if (newEVSE is null)
                return AddOrUpdateResult<Connector>.Failed(Connector,
                                                           warnings.First().Text.FirstText());


            var result = await AddOrUpdateEVSE(Location,
                                               newEVSE,
                                               (AllowDowngrades ?? this.AllowDowngrades) == false,
                                               SkipNotifications);

            if (result.IsSuccess)
            {

                if (!SkipNotifications)
                {
                    var OnLocationChangedLocal = OnLocationChanged;
                    if (OnLocationChangedLocal is not null)
                    {
                        try
                        {
                            if (Connector.ParentEVSE?.ParentLocation is not null)
                                await OnLocationChangedLocal(Connector.ParentEVSE.ParentLocation);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateConnector), " ", nameof(OnLocationChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }
                }

                return AddOrUpdateResult<Connector>.Success(Connector,
                                                            WasCreated: result.WasCreated ?? false);

            }

            return AddOrUpdateResult<Connector>.Failed(Connector,
                                                       result.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region UpdateConnector        (Location, EVSE, Connector,                 AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<Connector>> UpdateConnector(Location           Location,
                                                                   EVSE               EVSE,
                                                                   Connector          Connector,
                                                                   Boolean?           AllowDowngrades     = false,
                                                                   Boolean            SkipNotifications   = false,
                                                                   EventTracking_Id?  EventTrackingId     = null,
                                                                   User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (EVSE.TryGetConnector(Connector.Id, out var existingConnector) &&
                existingConnector is not null)
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Connector.LastUpdated <= existingConnector.LastUpdated)
                {
                    return UpdateResult<Connector>.Failed(Connector,
                                                          "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");
                }

                //if (newOrUpdatedConnector.LastUpdated.ToIso8601() == existingConnector.LastUpdated.ToIso8601())
                //    return AddOrUpdateResult<Connector>.NoOperation(newOrUpdatedConnector,
                //                                                    "The 'lastUpdated' timestamp of the new connector must be newer then the timestamp of the existing connector!");

            }
            else
                return UpdateResult<Connector>.Failed(Connector,
                                                      $"The given charging connector '{Connector.Id}' does not exist!");

            #endregion


            var newEVSE = EVSE.Update(evseBuilder => {
                                          evseBuilder.SetConnector(Connector);
                                          evseBuilder.LastUpdated  = Connector.LastUpdated;
                                      },
                                      out var warnings);

            if (newEVSE is null)
                return UpdateResult<Connector>.Failed(Connector,
                                                      warnings.First().Text.FirstText());


            var updateEVSEResult = await UpdateEVSE(Location,
                                                    newEVSE,
                                                    (AllowDowngrades ?? this.AllowDowngrades) == false,
                                                    SkipNotifications);

            return updateEVSEResult.IsSuccess
                       ? UpdateResult<Connector>.Success(Connector)
                       : UpdateResult<Connector>.Failed (Connector,
                                                         updateEVSEResult.ErrorResponse ?? "Unknown error!");

        }

        #endregion

        #region TryPatchConnector      (Location, EVSE, Connector, ConnectorPatch, AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<Connector>> TryPatchConnector(Location           Location,
                                                                    EVSE               EVSE,
                                                                    Connector          Connector,
                                                                    JObject            ConnectorPatch,
                                                                    Boolean?           AllowDowngrades     = false,
                                                                    Boolean            SkipNotifications   = false,
                                                                    EventTracking_Id?  EventTrackingId     = null,
                                                                    User_Id?           CurrentUserId       = null)
        {

            if (!ConnectorPatch.HasValues)
                return PatchResult<Connector>.Failed(Connector,
                                                     "The given connector patch must not be null or empty!");

            var patchResult = Connector.TryPatch(ConnectorPatch,
                                                 AllowDowngrades ?? this.AllowDowngrades ?? false);

            if (patchResult.IsSuccess &&
                patchResult.PatchedData is not null)
            {

                var updateConnectorResult = await UpdateConnector(Location,
                                                                  EVSE,
                                                                  patchResult.PatchedData,
                                                                  AllowDowngrades,
                                                                  SkipNotifications);

                if (updateConnectorResult.IsFailed)
                    return PatchResult<Connector>.Failed(Connector,
                                                         updateConnectorResult.ErrorResponse ?? "Unknown error!");

            }

            return patchResult;

        }

        #endregion


        #region LocationExists(LocationId)

        public Boolean LocationExists(Location_Id LocationId)

            => locations.ContainsKey(LocationId);

        #endregion

        #region TryGetLocation(LocationId, out Location)

        public Boolean TryGetLocation(Location_Id    LocationId,
                                      out Location?  Location)
        {

            if (locations.TryGetValue(LocationId, out Location))
                return true;

            Location = null;
            return false;

        }

        #endregion

        #region GetLocations  (IncludeLocation = null)

        public IEnumerable<Location> GetLocations(Func<Location, Boolean>? IncludeLocation = null)

            => IncludeLocation is null
                   ? locations.Values
                   : locations.Values.Where(IncludeLocation);

        #endregion

        #region GetLocations  (CountryCode, PartyId)

        public IEnumerable<Location> GetLocations(CountryCode  CountryCode,
                                                  Party_Id     PartyId)

            => locations.Values.Where(location => location.CountryCode == CountryCode &&
                                                  location.PartyId     == PartyId);

        #endregion


        #region RemoveLocation    (Location,             SkipNotifications = false)

        /// <summary>
        /// Remove the given charging location.
        /// </summary>
        /// <param name="Location">A charging location.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<Location>> RemoveLocation(Location           Location,
                                                           Boolean            SkipNotifications   = false,
                                                           EventTracking_Id?  EventTrackingId     = null,
                                                           User_Id?           CurrentUserId       = null)

            => RemoveLocation(Location.Id,
                              SkipNotifications);

        #endregion

        #region RemoveLocation    (LocationId,           SkipNotifications = false)

        /// <summary>
        /// Remove the given charging location.
        /// </summary>
        /// <param name="LocationId">An unique charging location identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<Location>> RemoveLocation(Location_Id        LocationId,
                                                                 Boolean            SkipNotifications   = false,
                                                                 EventTracking_Id?  EventTrackingId     = null,
                                                                 User_Id?           CurrentUserId       = null)
        {

            if (locations.Remove(LocationId, out var location))
            {

                await LogAsset(removeLocation,
                               location.ToJSON(true,
                                               true,
                                               null,
                                               CustomLocationSerializer,
                                               CustomAdditionalGeoLocationSerializer,
                                               CustomEVSESerializer,
                                               CustomStatusScheduleSerializer,
                                               CustomConnectorSerializer,
                                               CustomEnergyMeterSerializer,
                                               CustomTransparencySoftwareStatusSerializer,
                                               CustomTransparencySoftwareSerializer,
                                               CustomDisplayTextSerializer,
                                               CustomBusinessDetailsSerializer,
                                               CustomHoursSerializer,
                                               CustomImageSerializer,
                                               CustomEnergyMixSerializer,
                                               CustomEnergySourceSerializer,
                                               CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnLocationRemovedLocal = OnLocationRemoved;
                    if (OnLocationRemovedLocal is not null)
                    {
                        try
                        {
                            await OnLocationRemovedLocal(location);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                    var OnEVSERemovedLocal = OnEVSERemoved;
                    if (OnEVSERemovedLocal is not null)
                    {
                        try
                        {
                            foreach (var evse in location.EVSEs)
                                await OnEVSERemovedLocal(evse);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnEVSERemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<Location>.Success(location);

            }

            return RemoveResult<Location>.Failed(null,
                                                 "RemoveLocation(LocationId, ...) failed!");

        }

        #endregion

        #region RemoveAllLocations(                      SkipNotifications = false)

        /// <summary>
        /// Remove all charging locations.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> RemoveAllLocations(Boolean            SkipNotifications   = false,
                                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                                  User_Id?           CurrentUserId       = null)
        {

            var existingLocations = locations.Values.ToArray();

            locations.Clear();

            await LogAsset(removeAllLocations,
                           EventTrackingId ?? EventTracking_Id.New,
                           CurrentUserId);

            if (!SkipNotifications)
            {

                var OnLocationRemovedLocal = OnLocationRemoved;
                if (OnLocationRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var location in existingLocations)
                            await OnLocationRemovedLocal(location);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllLocations), " ", nameof(OnLocationRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

                var OnEVSERemovedLocal = OnEVSERemoved;
                if (OnEVSERemovedLocal is not null)
                {
                    try
                    {
                        foreach (var evse in existingLocations.SelectMany(location => location.EVSEs))
                            await OnEVSERemovedLocal(evse);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllLocations), " ", nameof(OnEVSERemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<Location>>.Success(existingLocations);

        }

        #endregion

        #region RemoveAllLocations(IncludeLocations,     SkipNotifications = false)

        /// <summary>
        /// Remove all matching charging locations.
        /// </summary>
        /// <param name="IncludeLocations">A charging location filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> RemoveAllLocations(Func<Location, Boolean>  IncludeLocations,
                                                                                  Boolean                  SkipNotifications   = false)
        {

            var removedLocations  = new List<Location>();
            var failedLocations   = new List<RemoveResult<Location>>();

            foreach (var location in locations.Values.Where(IncludeLocations).ToArray())
            {

                var result = await RemoveLocation(location.Id,
                                                  SkipNotifications);

                if (result.IsSuccess)
                    removedLocations.Add(location);
                else
                    failedLocations. Add(result);

            }

            return removedLocations.Any() && !failedLocations.Any()
                       ? RemoveResult<IEnumerable<Location>>.Success(removedLocations)

                       : !removedLocations.Any() && !failedLocations.Any()
                             ? RemoveResult<IEnumerable<Location>>.NoOperation(Array.Empty<Location>())
                             : RemoveResult<IEnumerable<Location>>.Failed     (failedLocations.Select(location => location.Data)!,
                                                                               failedLocations.Select(location => location.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllLocations(IncludeLocationIds,   SkipNotifications = false)

        /// <summary>
        /// Remove all matching charging locations.
        /// </summary>
        /// <param name="IncludeLocationIds">A charging location identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> RemoveAllLocations(Func<Location_Id, Boolean>  IncludeLocationIds,
                                                                                  Boolean                     SkipNotifications   = false)
        {

            var removedLocations  = new List<Location>();
            var failedLocations   = new List<RemoveResult<Location>>();

            foreach (var location in locations.Where  (kvp => IncludeLocationIds(kvp.Key)).
                                               Select (kvp => kvp.Value).
                                               ToArray())
            {

                var result = await RemoveLocation(location.Id,
                                                  SkipNotifications);

                if (result.IsSuccess)
                    removedLocations.Add(location);
                else
                    failedLocations. Add(result);

            }

            return removedLocations.Any() && !failedLocations.Any()
                       ? RemoveResult<IEnumerable<Location>>.Success(removedLocations)

                       : !removedLocations.Any() && !failedLocations.Any()
                             ? RemoveResult<IEnumerable<Location>>.NoOperation(Array.Empty<Location>())
                             : RemoveResult<IEnumerable<Location>>.Failed     (failedLocations.Select(location => location.Data)!,
                                                                               failedLocations.Select(location => location.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllLocations(CountryCode, PartyId, SkipNotifications = false)

        /// <summary>
        /// Remove all charging locations owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Location>>> RemoveAllLocations(CountryCode  CountryCode,
                                                                                  Party_Id     PartyId,
                                                                                  Boolean      SkipNotifications   = false)
        {

            var removedLocations  = new List<Location>();
            var failedLocations   = new List<RemoveResult<Location>>();

            foreach (var location in locations.Values.Where  (location => CountryCode == location.CountryCode &&
                                                                          PartyId     == location.PartyId).
                                                      ToArray())
            {

                var result = await RemoveLocation(location.Id,
                                                  SkipNotifications);

                if (result.IsSuccess)
                    removedLocations.Add(location);
                else
                    failedLocations. Add(result);

            }

            return removedLocations.Any() && !failedLocations.Any()
                       ? RemoveResult<IEnumerable<Location>>.Success(removedLocations)

                       : !removedLocations.Any() && !failedLocations.Any()
                             ? RemoveResult<IEnumerable<Location>>.NoOperation(Array.Empty<Location>())
                             : RemoveResult<IEnumerable<Location>>.Failed     (failedLocations.Select(location => location.Data)!,
                                                                               failedLocations.Select(location => location.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #endregion

        #region Tariffs

        #region Data

        private readonly TimeRangeDictionary<Tariff_Id , Tariff> tariffs = new();


        public delegate Task OnTariffAddedDelegate  (Tariff               Tariff);
        public delegate Task OnTariffChangedDelegate(Tariff               Tariff);
        public delegate Task OnTariffRemovedDelegate(IEnumerable<Tariff>  Tariff);

        public event OnTariffAddedDelegate?    OnTariffAdded;
        public event OnTariffChangedDelegate?  OnTariffChanged;
        public event OnTariffRemovedDelegate?  OnTariffRemoved;

        #endregion


        public GetTariffs2_Delegate?    GetTariffsDelegate      { get; set; }

        public GetTariffIds2_Delegate?  GetTariffIdsDelegate    { get; set; }


        #region AddTariff            (Tariff,                                       SkipNotifications = false, ...)

        public async Task<AddResult<Tariff>> AddTariff(Tariff             Tariff,
                                                       Boolean            SkipNotifications   = false,
                                                       EventTracking_Id?  EventTrackingId     = null,
                                                       User_Id?           CurrentUserId       = null)
        {

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(addTariff,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffAddedLocal = OnTariffAdded;
                    if (OnTariffAddedLocal is not null)
                    {
                        try
                        {
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddTariff), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Tariff>.Success(Tariff);

            }

            return AddResult<Tariff>.Failed(Tariff,
                                            "TryAdd(Tariff.Id, Tariff) failed!");

        }

        #endregion

        #region AddTariffIfNotExists (Tariff,                                       SkipNotifications = false, ...)

        public async Task<AddResult<Tariff>> AddTariffIfNotExists(Tariff             Tariff,
                                                                  Boolean            SkipNotifications   = false,
                                                                  EventTracking_Id?  EventTrackingId     = null,
                                                                  User_Id?           CurrentUserId       = null)
        {

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(addTariffIfNotExists,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffAddedLocal = OnTariffAdded;
                    if (OnTariffAddedLocal is not null)
                    {
                        try
                        {
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddTariffIfNotExists), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Tariff>.Success(Tariff);

            }

            return AddResult<Tariff>.NoOperation(Tariff);

        }

        #endregion

        #region AddOrUpdateTariff    (Tariff,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<AddOrUpdateResult<Tariff>> AddOrUpdateTariff(Tariff             Tariff,
                                                                       Boolean?           AllowDowngrades     = false,
                                                                       Boolean            SkipNotifications   = false,
                                                                       EventTracking_Id?  EventTrackingId     = null,
                                                                       User_Id?           CurrentUserId       = null)
        {

            #region Update an existing tariff

            if (tariffs.TryGetValue(Tariff.Id,
                                    out var existingTariff,
                                    Tariff.NotBefore ?? DateTime.MinValue))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Tariff.LastUpdated <= existingTariff.LastUpdated)
                {
                    return AddOrUpdateResult<Tariff>.Failed(Tariff,
                                                            "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!");
                }

                tariffs.AddOrUpdate(Tariff.Id, Tariff);
                Tariff.CommonAPI = this;

                await LogAsset(addOrUpdateTariff,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffChangedLocal = OnTariffChanged;
                    if (OnTariffChangedLocal is not null)
                    {
                        try
                        {
                            OnTariffChangedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Tariff>.Success(Tariff,
                                                         WasCreated: false);

            }

            #endregion

            #region Add a new tariff

            if (tariffs.TryAdd(Tariff.Id, Tariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(addOrUpdateTariff,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffAddedLocal = OnTariffAdded;
                    if (OnTariffAddedLocal is not null)
                    {
                        try
                        {
                            OnTariffAddedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateTariff), " ", nameof(OnTariffAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Tariff>.Success(Tariff,
                                                         WasCreated: true);

            }

            return AddOrUpdateResult<Tariff>.Failed(Tariff,
                                                    "AddOrUpdateTariff(Tariff.Id, Tariff) failed!");

            #endregion

        }

        #endregion

        #region UpdateTariff         (Tariff,              AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<UpdateResult<Tariff>> UpdateTariff(Tariff             Tariff,
                                                             Boolean?           AllowDowngrades     = false,
                                                             Boolean            SkipNotifications   = false,
                                                             EventTracking_Id?  EventTrackingId     = null,
                                                             User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (tariffs.TryGetValue(Tariff.Id, out var existingTariff, Timestamp.Now))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Tariff.LastUpdated <= existingTariff.LastUpdated)
                {

                    return UpdateResult<Tariff>.Failed(Tariff,
                                                       "The 'lastUpdated' timestamp of the new charging tariff must be newer then the timestamp of the existing tariff!");

                }

            }
            else
                return UpdateResult<Tariff>.Failed(Tariff,
                                                   $"Unknown tariff identification '{Tariff.Id}'!");

            #endregion

            if (tariffs.TryUpdate(Tariff.Id, Tariff, existingTariff))
            {

                Tariff.CommonAPI = this;

                await LogAsset(updateTariff,
                               Tariff.ToJSON(true,
                                             true,
                                             CustomTariffSerializer,
                                             CustomDisplayTextSerializer,
                                             CustomTariffElementSerializer,
                                             CustomPriceComponentSerializer,
                                             CustomTariffRestrictionsSerializer,
                                             CustomEnergyMixSerializer,
                                             CustomEnergySourceSerializer,
                                             CustomEnvironmentalImpactSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffChangedLocal = OnTariffChanged;
                    if (OnTariffChangedLocal is not null)
                    {
                        try
                        {
                            OnTariffChangedLocal(Tariff).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateTariff), " ", nameof(OnTariffChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<Tariff>.Success(Tariff);

            }

            return UpdateResult<Tariff>.Failed(Tariff,
                                               "UpdateTariff(Tariff.Id, Tariff, Tariff) failed!");

        }

        #endregion

        #region TryPatchTariff       (Tariff, TariffPatch, AllowDowngrades = false, SkipNotifications = false, ...)

        public async Task<PatchResult<Tariff>> TryPatchTariff(Tariff             Tariff,
                                                              JObject            TariffPatch,
                                                              Boolean?           AllowDowngrades     = false,
                                                              Boolean            SkipNotifications   = false,
                                                              EventTracking_Id?  EventTrackingId     = null,
                                                              User_Id?           CurrentUserId       = null)
        {

            if (!TariffPatch.HasValues)
                return PatchResult<Tariff>.Failed(Tariff,
                                                  "The given charging tariff patch must not be null or empty!");

            if (tariffs.TryGetValue(Tariff.Id, out var existingTariff, Timestamp.Now))
            {

                var patchResult = existingTariff.TryPatch(TariffPatch,
                                                          AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    tariffs.TryUpdate(Tariff.Id, Tariff, patchResult.PatchedData);

                    await LogAsset(updateTariff,
                                   Tariff.ToJSON(true,
                                                 true,
                                                 CustomTariffSerializer,
                                                 CustomDisplayTextSerializer,
                                                 CustomTariffElementSerializer,
                                                 CustomPriceComponentSerializer,
                                                 CustomTariffRestrictionsSerializer,
                                                 CustomEnergyMixSerializer,
                                                 CustomEnergySourceSerializer,
                                                 CustomEnvironmentalImpactSerializer),
                                   EventTrackingId ?? EventTracking_Id.New,
                                   CurrentUserId);

                    if (!SkipNotifications)
                    {

                        var OnTariffChangedLocal = OnTariffChanged;
                        if (OnTariffChangedLocal is not null)
                        {
                            try
                            {
                                OnTariffChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchTariff), " ", nameof(OnTariffChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<Tariff>.Failed(Tariff,
                                                  "The given charging tariff does not exist!");

        }

        #endregion

        #region RemoveTariff         (Tariff,                                       SkipNotifications = false, ...)

        /// <summary>
        /// Remove the given charging tariff.
        /// </summary>
        /// <param name="Tariff">A charging tariff.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<IEnumerable<Tariff>>> RemoveTariff(Tariff             Tariff,
                                                                    Boolean            SkipNotifications   = false,
                                                                    EventTracking_Id?  EventTrackingId     = null,
                                                                    User_Id?           CurrentUserId       = null)

            => RemoveTariff(Tariff.Id,
                            SkipNotifications,
                            EventTrackingId,
                            CurrentUserId);

        #endregion

        #region RemoveTariff         (TariffId,                                     SkipNotifications = false, ...)

        /// <summary>
        /// Remove the given charging tariff.
        /// </summary>
        /// <param name="TariffId">An unique charging tariff identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveTariff(Tariff_Id          TariffId,
                                                                          Boolean            SkipNotifications   = false,
                                                                          EventTracking_Id?  EventTrackingId     = null,
                                                                          User_Id?           CurrentUserId       = null)
        {

            if (tariffs.TryRemove(TariffId, out var removedTariffs))
            {

                await LogAsset(removeTariff,
                               new JArray(removedTariffs.Select(removedTariff => removedTariff.ToJSON(true,
                                                                                                      true,
                                                                                                      CustomTariffSerializer,
                                                                                                      CustomDisplayTextSerializer,
                                                                                                      CustomTariffElementSerializer,
                                                                                                      CustomPriceComponentSerializer,
                                                                                                      CustomTariffRestrictionsSerializer,
                                                                                                      CustomEnergyMixSerializer,
                                                                                                      CustomEnergySourceSerializer,
                                                                                                      CustomEnvironmentalImpactSerializer))),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTariffRemovedLocal = OnTariffRemoved;
                    if (OnTariffRemovedLocal is not null)
                    {
                        try
                        {
                            await OnTariffRemovedLocal(removedTariffs);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveTariff), " ", nameof(OnTariffRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs);

            }

            return RemoveResult<IEnumerable<Tariff>>.Failed(null,
                                                            "RemoveTariff(TariffId, ...) failed!");

        }

        #endregion

        #region RemoveAllTariffs     (                                              SkipNotifications = false, ...)

        /// <summary>
        /// Remove all charging tariffs.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Boolean            SkipNotifications   = false,
                                                                              EventTracking_Id?  EventTrackingId     = null,
                                                                              User_Id?           CurrentUserId       = null)
        {

            var existingTariffs = tariffs.Values().ToArray();

            tariffs.Clear();

            await LogAsset(removeAllTariffs,
                           EventTrackingId ?? EventTracking_Id.New,
                           CurrentUserId);

            if (!SkipNotifications)
            {

                var OnTariffRemovedLocal = OnTariffRemoved;
                if (OnTariffRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var existingTariff in existingTariffs)
                            await OnTariffRemovedLocal([ existingTariff ]);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllTariffs), " ", nameof(OnTariffRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<Tariff>>.Success(existingTariffs);

        }

        #endregion

        #region RemoveAllTariffs     (IncludeTariffs,                               SkipNotifications = false, ...)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeTariffs">A charging tariff filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Func<Tariff, Boolean>  IncludeTariffs,
                                                                              Boolean                SkipNotifications   = false,
                                                                              EventTracking_Id?      EventTrackingId     = null,
                                                                              User_Id?               CurrentUserId       = null)
        {

            var removedTariffs  = new List<Tariff>();
            var failedTariffs   = new List<RemoveResult<IEnumerable<Tariff>>>();

            foreach (var tariff in tariffs.Values().Where(IncludeTariffs).ToArray())
            {

                var result = await RemoveTariff(
                                       tariff.Id,
                                       SkipNotifications
                                   );

                if (result.IsSuccess)
                    removedTariffs.Add(tariff);
                else
                    failedTariffs. Add(result);

            }

            return removedTariffs.Count != 0 && failedTariffs.Count == 0
                       ? RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs)

                       : removedTariffs.Count == 0 && failedTariffs.Count == 0
                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation([])
                             : RemoveResult<IEnumerable<Tariff>>.Failed     (failedTariffs.SelectMany(tariff => tariff.Data ?? []),
                                                                             failedTariffs.Select    (tariff => tariff.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllTariffs     (IncludeTariffIds,                             SkipNotifications = false, ...)

        /// <summary>
        /// Remove all matching charging tariffs.
        /// </summary>
        /// <param name="IncludeTariffIds">A charging tariff identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(Func<Tariff_Id, Boolean>  IncludeTariffIds,
                                                                              Boolean                   SkipNotifications   = false,
                                                                              EventTracking_Id?         EventTrackingId     = null,
                                                                              User_Id?                  CurrentUserId       = null)
        {

            var removedTariffs  = new List<Tariff>();
            var failedTariffs   = new List<RemoveResult<IEnumerable<Tariff>>>();

            foreach (var tariffsToRemove in tariffs.Where  (kvp => IncludeTariffIds(kvp.Key)).
                                                    Select (kvp => kvp.Value).
                                                    ToArray())
            {

                var result = await RemoveTariff(tariffsToRemove.First().Id,
                                                SkipNotifications);

                if (result.IsSuccess)
                    removedTariffs.AddRange(tariffsToRemove);
                else
                    failedTariffs. Add(result);

            }

            return removedTariffs.Count != 0 && failedTariffs.Count == 0
                       ? RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs)

                       : removedTariffs.Count == 0 && failedTariffs.Count == 0
                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation([])
                             : RemoveResult<IEnumerable<Tariff>>.Failed     (failedTariffs.SelectMany(tariff => tariff.Data ?? []),
                                                                             failedTariffs.Select    (tariff => tariff.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllTariffs     (CountryCode, PartyId,                         SkipNotifications = false, ...)

        /// <summary>
        /// Remove all charging tariffs owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Tariff>>> RemoveAllTariffs(CountryCode        CountryCode,
                                                                              Party_Id           PartyId,
                                                                              Boolean            SkipNotifications   = false,
                                                                              EventTracking_Id?  EventTrackingId     = null,
                                                                              User_Id?           CurrentUserId       = null)
        {

            await LogAssetComment($"{removeAllTariffs}: {CountryCode} {PartyId}",
                                  EventTrackingId ?? EventTracking_Id.New,
                                  CurrentUserId);

            var removedTariffs  = new List<Tariff>();
            var failedTariffs   = new List<RemoveResult<IEnumerable<Tariff>>>();

            foreach (var tariff in tariffs.Values().Where  (tariff => CountryCode == tariff.CountryCode &&
                                                                      PartyId     == tariff.PartyId).
                                                    ToArray())
            {

                var result = await RemoveTariff(tariff.Id,
                                                SkipNotifications);

                if (result.IsSuccess)
                    removedTariffs.Add(tariff);
                else
                    failedTariffs. Add(result);

            }

            return removedTariffs.Any() && !failedTariffs.Any()
                       ? RemoveResult<IEnumerable<Tariff>>.Success(removedTariffs)

                       : !removedTariffs.Any() && !failedTariffs.Any()
                             ? RemoveResult<IEnumerable<Tariff>>.NoOperation(Array.Empty<Tariff>())
                             : RemoveResult<IEnumerable<Tariff>>.Failed     (failedTariffs.SelectMany(tariff => tariff.Data ?? []),
                                                                             failedTariffs.Select    (tariff => tariff.ErrorResponse).AggregateWith(", "));

        }

        #endregion


        #region TariffExists(TariffId, Timestamp = null, Tolerance = null)

        public Boolean TariffExists(Tariff_Id  TariffId,
                                    DateTime?  Timestamp   = null,
                                    TimeSpan?  Tolerance   = null)

            => tariffs.ContainsKey(TariffId,
                                   Timestamp,
                                   Tolerance);

        #endregion

        #region TryGetTariff(TariffId, out Tariff, Timestamp = null, Tolerance = null)

        public Boolean TryGetTariff(Tariff_Id    TariffId,
                                    out Tariff?  Tariff,
                                    DateTime?    Timestamp   = null,
                                    TimeSpan?    Tolerance   = null)
        {

            if (tariffs.TryGetValue(TariffId,
                                    out Tariff,
                                    Timestamp,
                                    Tolerance))
            {
                return true;
            }

            Tariff = null;
            return false;

        }

        #endregion

        #region GetTariff   (TariffId, Timestamp = null, Tolerance = null)

        public Tariff? GetTariff(Tariff_Id  TariffId,
                                 DateTime?  Timestamp   = null,
                                 TimeSpan?  Tolerance   = null)
        {

            if (tariffs.TryGetValue(TariffId,
                                    out var tariff,
                                    Timestamp,
                                    Tolerance))
            {
                return tariff;
            }

            return null;

        }

        #endregion

        #region GetTariffs  (IncludeTariff = null, Timestamp = null, Tolerance = null)

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, Boolean>?  IncludeTariff   = null,
                                              DateTime?               Timestamp       = null,
                                              TimeSpan?               Tolerance       = null)

            => IncludeTariff is null
                   ? tariffs.Values(Timestamp, Tolerance)
                   : tariffs.Values(Timestamp, Tolerance).Where(IncludeTariff);

        #endregion

        #region GetTariffs  (CountryCode, PartyId, Timestamp = null, Tolerance = null)

        public IEnumerable<Tariff> GetTariffs(CountryCode  CountryCode,
                                              Party_Id     PartyId,
                                              DateTime?    Timestamp   = null,
                                              TimeSpan?    Tolerance   = null)

            => tariffs.Values(Timestamp, Tolerance).
                       Where (tariff => tariff.CountryCode == CountryCode &&
                                        tariff.PartyId     == PartyId);

        #endregion


        #region GetTariffIds(CountryCode?, PartyId?, LocationId?, EVSEUId?, ConnectorId?, EMSPId?)

        public IEnumerable<Tariff_Id> GetTariffIds(CountryCode    CountryCode,
                                                   Party_Id       PartyId,
                                                   Location_Id?   LocationId,
                                                   EVSE_Id?       EVSEUId,
                                                   Connector_Id?  ConnectorId,
                                                   EMSP_Id?       EMSPId)
        {

            return GetTariffIdsDelegate?.Invoke(CountryCode,
                                                PartyId,
                                                LocationId,
                                                EVSEUId,
                                                ConnectorId,
                                                EMSPId) ?? [];

        }

        #endregion

        #endregion

        #region Sessions

        #region Data

        private readonly ConcurrentDictionary<Session_Id , Session> chargingSessions = new();


        public delegate Task OnSessionAddedDelegate          (Session Session);
        public delegate Task OnChargingSessionChangedDelegate(Session Session);
        public delegate Task OnSessionRemovedDelegate        (Session Session);

        public event OnSessionAddedDelegate?            OnSessionAdded;
        public event OnChargingSessionChangedDelegate?  OnSessionChanged;
        public event OnSessionRemovedDelegate?          OnSessionRemoved;

        #endregion


        #region AddSession           (Session,                          SkipNotifications = false)

        public async Task<AddResult<Session>> AddSession(Session            Session,
                                                         Boolean            SkipNotifications   = false,
                                                         EventTracking_Id?  EventTrackingId     = null,
                                                         User_Id?           CurrentUserId       = null)
        {

            if (chargingSessions.TryAdd(Session.Id, Session))
            {

                Session.CommonAPI = this;

                await LogAsset(addSession,
                               Session.ToJSON(true,
                                              true,
                                              null,
                                              CustomSessionSerializer,
                                              CustomLocationSerializer,
                                              CustomAdditionalGeoLocationSerializer,
                                              CustomEVSESerializer,
                                              CustomStatusScheduleSerializer,
                                              CustomConnectorSerializer,
                                              CustomEnergyMeterSerializer,
                                              CustomTransparencySoftwareStatusSerializer,
                                              CustomTransparencySoftwareSerializer,
                                              CustomDisplayTextSerializer,
                                              CustomBusinessDetailsSerializer,
                                              CustomHoursSerializer,
                                              CustomImageSerializer,
                                              CustomEnergyMixSerializer,
                                              CustomEnergySourceSerializer,
                                              CustomEnvironmentalImpactSerializer,
                                              CustomChargingPeriodSerializer,
                                              CustomCDRDimensionSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnChargingSessionAddedLocal = OnSessionAdded;
                    if (OnChargingSessionAddedLocal is not null)
                    {
                        try
                        {
                            OnChargingSessionAddedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddSession), " ", nameof(OnSessionAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Session>.Success(Session);

            }

            return AddResult<Session>.Failed(Session,
                                            "AddSession(Session.Id, Session) failed!");

        }

        #endregion

        #region AddSessionIfNotExists(Session,                          SkipNotifications = false)

        public async Task<AddResult<Session>> AddSessionIfNotExists(Session            Session,
                                                                    Boolean            SkipNotifications   = false,
                                                                    EventTracking_Id?  EventTrackingId     = null,
                                                                    User_Id?           CurrentUserId       = null)
        {

            if (chargingSessions.TryAdd(Session.Id, Session))
            {

                Session.CommonAPI = this;

                await LogAsset(addSessionIfNotExists,
                               Session.ToJSON(true,
                                              true,
                                              null,
                                              CustomSessionSerializer,
                                              CustomLocationSerializer,
                                              CustomAdditionalGeoLocationSerializer,
                                              CustomEVSESerializer,
                                              CustomStatusScheduleSerializer,
                                              CustomConnectorSerializer,
                                              CustomEnergyMeterSerializer,
                                              CustomTransparencySoftwareStatusSerializer,
                                              CustomTransparencySoftwareSerializer,
                                              CustomDisplayTextSerializer,
                                              CustomBusinessDetailsSerializer,
                                              CustomHoursSerializer,
                                              CustomImageSerializer,
                                              CustomEnergyMixSerializer,
                                              CustomEnergySourceSerializer,
                                              CustomEnvironmentalImpactSerializer,
                                              CustomChargingPeriodSerializer,
                                              CustomCDRDimensionSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnChargingSessionAddedLocal = OnSessionAdded;
                    if (OnChargingSessionAddedLocal is not null)
                    {
                        try
                        {
                            OnChargingSessionAddedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddSessionIfNotExists), " ", nameof(OnSessionAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Session>.Success(Session);

            }

            return AddResult<Session>.NoOperation(Session);

        }

        #endregion

        #region AddOrUpdateSession   (Session, AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<Session>> AddOrUpdateSession(Session            Session,
                                                                         Boolean?           AllowDowngrades     = false,
                                                                         Boolean            SkipNotifications   = false,
                                                                         EventTracking_Id?  EventTrackingId     = null,
                                                                         User_Id?           CurrentUserId       = null)
        {

            #region Update an existing session

            if (chargingSessions.TryGetValue(Session.Id, out var existingSession))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Session.LastUpdated <= existingSession.LastUpdated)
                {
                    return AddOrUpdateResult<Session>.Failed(Session,
                                                            "The 'lastUpdated' timestamp of the new charging session must be newer then the timestamp of the existing session!");
                }

                chargingSessions[Session.Id] = Session;
                Session.CommonAPI = this;

                await LogAsset(addOrUpdateSession,
                               Session.ToJSON(true,
                                              true,
                                              null,
                                              CustomSessionSerializer,
                                              CustomLocationSerializer,
                                              CustomAdditionalGeoLocationSerializer,
                                              CustomEVSESerializer,
                                              CustomStatusScheduleSerializer,
                                              CustomConnectorSerializer,
                                              CustomEnergyMeterSerializer,
                                              CustomTransparencySoftwareStatusSerializer,
                                              CustomTransparencySoftwareSerializer,
                                              CustomDisplayTextSerializer,
                                              CustomBusinessDetailsSerializer,
                                              CustomHoursSerializer,
                                              CustomImageSerializer,
                                              CustomEnergyMixSerializer,
                                              CustomEnergySourceSerializer,
                                              CustomEnvironmentalImpactSerializer,
                                              CustomChargingPeriodSerializer,
                                              CustomCDRDimensionSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnChargingSessionChangedLocal = OnSessionChanged;
                    if (OnChargingSessionChangedLocal is not null)
                    {
                        try
                        {
                            OnChargingSessionChangedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Session>.Success(Session,
                                                          WasCreated: false);

            }

            #endregion

            #region Add a new session

            if (chargingSessions.TryAdd(Session.Id, Session))
            {

                Session.CommonAPI = this;

                await LogAsset(addOrUpdateSession,
                               Session.ToJSON(true,
                                              true,
                                              null,
                                              CustomSessionSerializer,
                                              CustomLocationSerializer,
                                              CustomAdditionalGeoLocationSerializer,
                                              CustomEVSESerializer,
                                              CustomStatusScheduleSerializer,
                                              CustomConnectorSerializer,
                                              CustomEnergyMeterSerializer,
                                              CustomTransparencySoftwareStatusSerializer,
                                              CustomTransparencySoftwareSerializer,
                                              CustomDisplayTextSerializer,
                                              CustomBusinessDetailsSerializer,
                                              CustomHoursSerializer,
                                              CustomImageSerializer,
                                              CustomEnergyMixSerializer,
                                              CustomEnergySourceSerializer,
                                              CustomEnvironmentalImpactSerializer,
                                              CustomChargingPeriodSerializer,
                                              CustomCDRDimensionSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnSessionAddedLocal = OnSessionAdded;
                    if (OnSessionAddedLocal is not null)
                    {
                        try
                        {
                            OnSessionAddedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateSession), " ", nameof(OnSessionAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Session>.Success(Session,
                                                          WasCreated: true);

            }

            return AddOrUpdateResult<Session>.Failed(Session,
                                                     "AddOrUpdateSession(Session.Id, Session) failed!");

            #endregion

        }

        #endregion

        #region UpdateSession        (Session, AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<Session>> UpdateSession(Session            Session,
                                                               Boolean?           AllowDowngrades     = false,
                                                               Boolean            SkipNotifications   = false,
                                                               EventTracking_Id?  EventTrackingId     = null,
                                                               User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (chargingSessions.TryGetValue(Session.Id, out var existingSession))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Session.LastUpdated <= existingSession.LastUpdated)
                {

                    return UpdateResult<Session>.Failed(Session,
                                                        "The 'lastUpdated' timestamp of the new charging session must be newer then the timestamp of the existing session!");

                }

            }
            else
                return UpdateResult<Session>.Failed(Session,
                                                    $"Unknown session identification '{Session.Id}'!");

            #endregion


            if (chargingSessions.TryUpdate(Session.Id, Session, existingSession))
            {

                Session.CommonAPI = this;

                await LogAsset(updateSession,
                               Session.ToJSON(true,
                                              true,
                                              null,
                                              CustomSessionSerializer,
                                              CustomLocationSerializer,
                                              CustomAdditionalGeoLocationSerializer,
                                              CustomEVSESerializer,
                                              CustomStatusScheduleSerializer,
                                              CustomConnectorSerializer,
                                              CustomEnergyMeterSerializer,
                                              CustomTransparencySoftwareStatusSerializer,
                                              CustomTransparencySoftwareSerializer,
                                              CustomDisplayTextSerializer,
                                              CustomBusinessDetailsSerializer,
                                              CustomHoursSerializer,
                                              CustomImageSerializer,
                                              CustomEnergyMixSerializer,
                                              CustomEnergySourceSerializer,
                                              CustomEnvironmentalImpactSerializer,
                                              CustomChargingPeriodSerializer,
                                              CustomCDRDimensionSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnChargingSessionChangedLocal = OnSessionChanged;
                    if (OnChargingSessionChangedLocal is not null)
                    {
                        try
                        {
                            OnChargingSessionChangedLocal(Session).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateSession), " ", nameof(OnSessionChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<Session>.Success(Session);

            }

            return UpdateResult<Session>.Failed(Session,
                                                "UpdateSession(Session.Id, Session, Session) failed!");

        }

        #endregion


        #region TryPatchSession      (Session, SessionPatch, AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<Session>> TryPatchSession(Session            Session,
                                                                JObject            SessionPatch,
                                                                Boolean?           AllowDowngrades     = false,
                                                                Boolean            SkipNotifications   = false,
                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                User_Id?           CurrentUserId       = null)
        {

            if (SessionPatch is null || !SessionPatch.HasValues)
                return PatchResult<Session>.Failed(Session,
                                                  "The given charging session patch must not be null or empty!");

            if (chargingSessions.TryGetValue(Session.Id, out var existingSession))
            {

                var patchResult = existingSession.TryPatch(SessionPatch,
                                                           AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    chargingSessions[Session.Id] = patchResult.PatchedData;

                    await LogAsset(updateSession,
                                   Session.ToJSON(true,
                                                  true,
                                                  null,
                                                  CustomSessionSerializer,
                                                  CustomLocationSerializer,
                                                  CustomAdditionalGeoLocationSerializer,
                                                  CustomEVSESerializer,
                                                  CustomStatusScheduleSerializer,
                                                  CustomConnectorSerializer,
                                                  CustomEnergyMeterSerializer,
                                                  CustomTransparencySoftwareStatusSerializer,
                                                  CustomTransparencySoftwareSerializer,
                                                  CustomDisplayTextSerializer,
                                                  CustomBusinessDetailsSerializer,
                                                  CustomHoursSerializer,
                                                  CustomImageSerializer,
                                                  CustomEnergyMixSerializer,
                                                  CustomEnergySourceSerializer,
                                                  CustomEnvironmentalImpactSerializer,
                                                  CustomChargingPeriodSerializer,
                                                  CustomCDRDimensionSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                    if (!SkipNotifications)
                    {

                        var OnChargingSessionChangedLocal = OnSessionChanged;
                        if (OnChargingSessionChangedLocal is not null)
                        {
                            try
                            {
                                OnChargingSessionChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchSession), " ", nameof(OnSessionChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<Session>.Failed(Session,
                                                  "The given charging session does not exist!");

        }

        #endregion


        #region SessionExists(SessionId)

        public Boolean SessionExists(Session_Id SessionId)

            => chargingSessions.ContainsKey(SessionId);

        #endregion

        #region TryGetSession(CountryCode, PartyId, SessionId, out Session)

        public Boolean TryGetSession(Session_Id    SessionId,
                                     out Session?  Session)
        {

            if (chargingSessions.TryGetValue(SessionId, out Session))
                return true;

            Session = null;
            return false;

        }

        #endregion

        #region GetSessions  (IncludeSession = null)

        public IEnumerable<Session> GetSessions(Func<Session, Boolean>? IncludeSession = null)

            => IncludeSession is null
                   ? chargingSessions.Values
                   : chargingSessions.Values.Where(IncludeSession);

        #endregion

        #region GetSessions  (CountryCode, PartyId)

        public IEnumerable<Session> GetSessions(CountryCode  CountryCode,
                                                Party_Id     PartyId)

            => chargingSessions.Values.Where(chargingSession => chargingSession.CountryCode == CountryCode &&
                                                                chargingSession.PartyId     == PartyId);

        #endregion


        #region RemoveSession    (Session,              SkipNotifications = false)

        /// <summary>
        /// Remove the given charging session.
        /// </summary>
        /// <param name="Session">A charging session.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<Session>> RemoveSession(Session            Session,
                                                         Boolean            SkipNotifications   = false,
                                                         EventTracking_Id?  EventTrackingId     = null,
                                                         User_Id?           CurrentUserId       = null)

            => RemoveSession(Session.Id,
                             SkipNotifications);

        #endregion

        #region RemoveSession    (SessionId,            SkipNotifications = false)

        /// <summary>
        /// Remove the given charging session.
        /// </summary>
        /// <param name="SessionId">An unique charging session identification.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<Session>> RemoveSession(Session_Id         SessionId,
                                                               Boolean            SkipNotifications   = false,
                                                               EventTracking_Id?  EventTrackingId     = null,
                                                               User_Id?           CurrentUserId       = null)
        {

            if (chargingSessions.Remove(SessionId, out var session))
            {

                await LogAsset(removeTariff,
                               session.ToJSON(true,
                                              true,
                                              null,
                                              CustomSessionSerializer,
                                              CustomLocationSerializer,
                                              CustomAdditionalGeoLocationSerializer,
                                              CustomEVSESerializer,
                                              CustomStatusScheduleSerializer,
                                              CustomConnectorSerializer,
                                              CustomEnergyMeterSerializer,
                                              CustomTransparencySoftwareStatusSerializer,
                                              CustomTransparencySoftwareSerializer,
                                              CustomDisplayTextSerializer,
                                              CustomBusinessDetailsSerializer,
                                              CustomHoursSerializer,
                                              CustomImageSerializer,
                                              CustomEnergyMixSerializer,
                                              CustomEnergySourceSerializer,
                                              CustomEnvironmentalImpactSerializer,
                                              CustomChargingPeriodSerializer,
                                              CustomCDRDimensionSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnSessionRemovedLocal = OnSessionRemoved;
                    if (OnSessionRemovedLocal is not null)
                    {
                        try
                        {
                            await OnSessionRemovedLocal(session);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveSession), " ", nameof(OnSessionRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<Session>.Success(session);

            }

            return RemoveResult<Session>.Failed(null,
                                                "RemoveSession(SessionId, ...) failed!");

        }

        #endregion

        #region RemoveAllSessions(                      SkipNotifications = false)

        /// <summary>
        /// Remove all charging sessions.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>> RemoveAllSessions(Boolean            SkipNotifications   = false,
                                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                                User_Id?           CurrentUserId       = null)
        {

            var existingSessions = chargingSessions.Values.ToArray();

            chargingSessions.Clear();

            await LogAsset(removeAllSessions,
                           EventTrackingId ?? EventTracking_Id.New,
                           CurrentUserId);

            if (!SkipNotifications)
            {

                var OnSessionRemovedLocal = OnSessionRemoved;
                if (OnSessionRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var location in existingSessions)
                            await OnSessionRemovedLocal(location);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllSessions), " ", nameof(OnSessionRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<Session>>.Success(existingSessions);

        }

        #endregion

        #region RemoveAllSessions(IncludeSessions,      SkipNotifications = false)

        /// <summary>
        /// Remove all matching sessions.
        /// </summary>
        /// <param name="IncludeSessions">A charging session filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>> RemoveAllSessions(Func<Session, Boolean>  IncludeSessions,
                                                                                Boolean                 SkipNotifications   = false,
                                                                                EventTracking_Id?       EventTrackingId     = null,
                                                                                User_Id?                CurrentUserId       = null)
        {

            var removedSessions  = new List<Session>();
            var failedSessions   = new List<RemoveResult<Session>>();

            foreach (var session in chargingSessions.Values.Where(IncludeSessions).ToArray())
            {

                var result = await RemoveSession(session.Id,
                                                 SkipNotifications);

                if (result.IsSuccess)
                    removedSessions.Add(session);
                else
                    failedSessions. Add(result);

            }

            return removedSessions.Any() && !failedSessions.Any()
                       ? RemoveResult<IEnumerable<Session>>.Success(removedSessions)

                       : !removedSessions.Any() && !failedSessions.Any()
                             ? RemoveResult<IEnumerable<Session>>.NoOperation(Array.Empty<Session>())
                             : RemoveResult<IEnumerable<Session>>.Failed     (failedSessions.Select(session => session.Data)!,
                                                                              failedSessions.Select(session => session.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllSessions(IncludeSessionIds,    SkipNotifications = false)

        /// <summary>
        /// Remove all matching sessions.
        /// </summary>
        /// <param name="IncludeSessionIds">A charging session identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>> RemoveAllSessions(Func<Session_Id, Boolean>  IncludeSessionIds,
                                                                                Boolean                    SkipNotifications   = false,
                                                                                EventTracking_Id?          EventTrackingId     = null,
                                                                                User_Id?                   CurrentUserId       = null)
        {

            var removedSessions  = new List<Session>();
            var failedSessions   = new List<RemoveResult<Session>>();

            foreach (var session in chargingSessions.Where  (kvp => IncludeSessionIds(kvp.Key)).
                                                     Select (kvp => kvp.Value).
                                                     ToArray())
            {

                var result = await RemoveSession(session.Id,
                                                 SkipNotifications);

                if (result.IsSuccess)
                    removedSessions.Add(session);
                else
                    failedSessions. Add(result);

            }

            return removedSessions.Any() && !failedSessions.Any()
                       ? RemoveResult<IEnumerable<Session>>.Success(removedSessions)

                       : !removedSessions.Any() && !failedSessions.Any()
                             ? RemoveResult<IEnumerable<Session>>.NoOperation(Array.Empty<Session>())
                             : RemoveResult<IEnumerable<Session>>.Failed     (failedSessions.Select(session => session.Data)!,
                                                                              failedSessions.Select(session => session.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllSessions(CountryCode, PartyId, SkipNotifications = false)

        /// <summary>
        /// Remove all charging sessions owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Session>>> RemoveAllSessions(CountryCode        CountryCode,
                                                                                Party_Id           PartyId,
                                                                                Boolean            SkipNotifications   = false,
                                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                                User_Id?           CurrentUserId       = null)
        {

            var removedSessions  = new List<Session>();
            var failedSessions   = new List<RemoveResult<Session>>();

            foreach (var session in chargingSessions.Values.Where  (session => CountryCode == session.CountryCode &&
                                                                               PartyId     == session.PartyId).
                                                            ToArray())
            {

                var result = await RemoveSession(session.Id,
                                                 SkipNotifications);

                if (result.IsSuccess)
                    removedSessions.Add(session);
                else
                    failedSessions. Add(result);

            }

            return removedSessions.Any() && !failedSessions.Any()
                       ? RemoveResult<IEnumerable<Session>>.Success(removedSessions)

                       : !removedSessions.Any() && !failedSessions.Any()
                             ? RemoveResult<IEnumerable<Session>>.NoOperation(Array.Empty<Session>())
                             : RemoveResult<IEnumerable<Session>>.Failed     (failedSessions.Select(session => session.Data)!,
                                                                              failedSessions.Select(session => session.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #endregion

        #region Tokens

        #region Data

        private readonly ConcurrentDictionary<Token_Id, TokenStatus> tokenStatus = new();


        public delegate Task               OnTokenAddedDelegate  (Token     Token);
        public delegate Task               OnTokenChangedDelegate(Token     Token);
        public delegate Task               OnTokenRemovedDelegate(Token     Token);
        public delegate Task<TokenStatus>  OnVerifyTokenDelegate (Token_Id  TokenId);

        public event OnTokenAddedDelegate?    OnTokenAdded;
        public event OnTokenChangedDelegate?  OnTokenChanged;
        public event OnTokenRemovedDelegate?  OnTokenRemoved;
        public event OnVerifyTokenDelegate?   OnVerifyToken;

        #endregion


        #region AddToken           (Token, Status = AllowedTypes.ALLOWED,                          SkipNotifications = false)

        public async Task<AddResult<Token>> AddToken(Token              Token,
                                                     AllowedType?       Status              = null,
                                                     Boolean            SkipNotifications   = false,
                                                     EventTracking_Id?  EventTrackingId     = null,
                                                     User_Id?           CurrentUserId       = null)
        {

            var newTokenStatus = new TokenStatus(Token,
                                                 Status ??= AllowedType.ALLOWED);

            if (tokenStatus.TryAdd(Token.Id, newTokenStatus))
            {

                Token.CommonAPI = this;

                await LogAsset(addTokenStatus,
                               newTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                     true,
                                                     CustomTokenSerializer,
                                                     CustomLocationReferenceSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTokenAddedLocal = OnTokenAdded;
                    if (OnTokenAddedLocal is not null)
                    {
                        try
                        {
                            OnTokenAddedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddToken), " ", nameof(OnTokenAddedLocal), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Token>.Success(Token);

            }

            return AddResult<Token>.Failed(Token,
                                           "TryAdd(Token.Id, newTokenStatus) failed!");

        }

        #endregion

        #region AddTokenIfNotExists(Token, Status = AllowedTypes.ALLOWED,                          SkipNotifications = false)

        public async Task<AddResult<Token>> AddTokenIfNotExists(Token              Token,
                                                                AllowedType?       Status              = null,
                                                                Boolean            SkipNotifications   = false,
                                                                EventTracking_Id?  EventTrackingId     = null,
                                                                User_Id?           CurrentUserId       = null)
        {

            var newTokenStatus = new TokenStatus(Token,
                                                 Status ??= AllowedType.ALLOWED);

            if (tokenStatus.TryAdd(Token.Id, newTokenStatus))
            {

                Token.CommonAPI = this;

                await LogAsset(addTokenStatus,
                               newTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                     true,
                                                     CustomTokenSerializer,
                                                     CustomLocationReferenceSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTokenAddedLocal = OnTokenAdded;
                    if (OnTokenAddedLocal is not null)
                    {
                        try
                        {
                            OnTokenAddedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddToken), " ", nameof(OnTokenAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<Token>.Success(Token);

            }

            return AddResult<Token>.NoOperation(Token);

        }

        #endregion

        #region AddOrUpdateToken   (Token, Status = AllowedTypes.ALLOWED, AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<Token>> AddOrUpdateToken(Token              Token,
                                                                     AllowedType?       Status              = null,
                                                                     Boolean?           AllowDowngrades     = false,
                                                                     Boolean            SkipNotifications   = false,
                                                                     EventTracking_Id?  EventTrackingId     = null,
                                                                     User_Id?           CurrentUserId       = null)
        {

            #region Update an existing token

            if (tokenStatus.TryGetValue(Token.Id, out var existingTokenStatus))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Token.LastUpdated <= existingTokenStatus.Token.LastUpdated)
                {
                    return AddOrUpdateResult<Token>.Failed(Token,
                                                           "The 'lastUpdated' timestamp of the new token must be newer then the timestamp of the existing token!");
                }

                var updatedTokenStatus = new TokenStatus(Token,
                                                         Status ?? existingTokenStatus.Status);

                tokenStatus[Token.Id] = updatedTokenStatus;
                Token.CommonAPI = this;

                await LogAsset(addOrUpdateTokenStatus,
                               updatedTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                         true,
                                                         CustomTokenSerializer,
                                                         CustomLocationReferenceSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTokenChangedLocal = OnTokenChanged;
                    if (OnTokenChangedLocal is not null)
                    {
                        try
                        {
                            OnTokenChangedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateToken), " ", nameof(OnTokenChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Token>.Success(Token,
                                                        WasCreated: false);

            }

            #endregion

            #region Add a new token

            var newTokenStatus = new TokenStatus(Token,
                                                 Status ??= AllowedType.ALLOWED);

            if (tokenStatus.TryAdd(Token.Id, newTokenStatus))
            {

                Token.CommonAPI = this;

                await LogAsset(addOrUpdateTokenStatus,
                               newTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                     true,
                                                     CustomTokenSerializer,
                                                     CustomLocationReferenceSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTokenAddedLocal = OnTokenAdded;
                    if (OnTokenAddedLocal is not null)
                    {
                        try
                        {
                            OnTokenAddedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateToken), " ", nameof(OnTokenAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<Token>.Success(Token,
                                                        WasCreated: true);

            }

            return AddOrUpdateResult<Token>.Failed(Token,
                                                   "AddOrUpdateToken(Token.Id, Token) failed!");

            #endregion

        }

        #endregion

        #region UpdateToken        (Token, Status = AllowedTypes.ALLOWED, AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<Token>> UpdateToken(Token              Token,
                                                           AllowedType?       Status              = null,
                                                           Boolean?           AllowDowngrades     = false,
                                                           Boolean            SkipNotifications   = false,
                                                           EventTracking_Id?  EventTrackingId     = null,
                                                           User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (tokenStatus.TryGetValue(Token.Id, out var existingTokenStatus))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    Token.LastUpdated <= existingTokenStatus.Token.LastUpdated)
                {

                    return UpdateResult<Token>.Failed(Token,
                                                      "The 'lastUpdated' timestamp of the new charging token must be newer then the timestamp of the existing token!");

                }

            }
            else
                return UpdateResult<Token>.Failed(Token,
                                                  $"Unknown token identification '{Token.Id}'!");

            #endregion


            var updatedTokenStatus = new TokenStatus(Token,
                                                     AllowedType.ALLOWED);

            if (tokenStatus.TryUpdate(Token.Id,
                                      updatedTokenStatus,
                                      existingTokenStatus))
            {

                Token.CommonAPI = this;

                await LogAsset(updateTokenStatus,
                               updatedTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                         true,
                                                         CustomTokenSerializer,
                                                         CustomLocationReferenceSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTokenChangedLocal = OnTokenChanged;
                    if (OnTokenChangedLocal is not null)
                    {
                        try
                        {
                            OnTokenChangedLocal(Token).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateToken), " ", nameof(OnTokenChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<Token>.Success(Token);

            }

            return UpdateResult<Token>.Failed(Token,
                                              "UpdateToken(Token.Id, Token, Token) failed!");

        }

        #endregion


        #region TryPatchToken      (Token, TokenPatch, AllowDowngrades = false, SkipNotifications = false)

        public async Task<PatchResult<Token>> TryPatchToken(Token              Token,
                                                            JObject            TokenPatch,
                                                            Boolean?           AllowDowngrades     = false,
                                                            Boolean            SkipNotifications   = false,
                                                            EventTracking_Id?  EventTrackingId     = null,
                                                            User_Id?           CurrentUserId       = null)
        {

            if (TokenPatch is null || !TokenPatch.HasValues)
                return PatchResult<Token>.Failed(Token,
                                                 "The given token patch must not be null or empty!");

            if (tokenStatus.TryGetValue(Token.Id, out var existingTokenStatus))
            {

                var patchResult = existingTokenStatus.Token.TryPatch(TokenPatch,
                                                                     AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    var patchedTokenStatus = new TokenStatus(patchResult.PatchedData,
                                                             existingTokenStatus.Status);

                    tokenStatus[Token.Id] = patchedTokenStatus;

                    await LogAsset(updateTokenStatus,
                                   patchedTokenStatus.ToJSON(CustomTokenStatusSerializer,
                                                             true,
                                                             CustomTokenSerializer,
                                                             CustomLocationReferenceSerializer),
                                   EventTrackingId ?? EventTracking_Id.New,
                                   CurrentUserId);

                    if (!SkipNotifications)
                    {

                        var OnTokenChangedLocal = OnTokenChanged;
                        if (OnTokenChangedLocal is not null)
                        {
                            try
                            {
                                OnTokenChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchToken), " ", nameof(OnTokenChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<Token>.Failed(Token,
                                                  "The given token does not exist!");

        }

        #endregion


        #region TokenExists(TokenId)

        public Boolean TokenExists(Token_Id TokenId)

            => tokenStatus.ContainsKey(TokenId);

        #endregion

        #region TryGetToken(TokenId, out TokenWithStatus)

        public Boolean TryGetToken(Token_Id         TokenId,
                                   out TokenStatus  TokenWithStatus)
        {

            if (tokenStatus.TryGetValue(TokenId, out TokenWithStatus))
                return true;

            TokenWithStatus = default;
            return false;

        }

        #endregion

        #region GetTokens  (IncludeToken)

        public IEnumerable<TokenStatus> GetTokens(Func<Token, Boolean> IncludeToken)

            => IncludeToken is null
                   ? tokenStatus.Values
                   : tokenStatus.Values.Where(tokenStatus => IncludeToken(tokenStatus.Token));

        #endregion

        #region GetTokens  (IncludeTokenStatus = null)

        public IEnumerable<TokenStatus> GetTokens(Func<TokenStatus, Boolean>? IncludeTokenStatus = null)

            => IncludeTokenStatus is null
                   ? tokenStatus.Values
                   : tokenStatus.Values.Where(IncludeTokenStatus);

        #endregion

        #region GetTokens  (CountryCode, PartyId)

        public IEnumerable<TokenStatus> GetTokens(CountryCode  CountryCode,
                                                  Party_Id     PartyId)

            => tokenStatus.Values.Where(tokenStatus => tokenStatus.Token.CountryCode == CountryCode &&
                                                       tokenStatus.Token.PartyId     == PartyId);

        #endregion


        #region RemoveToken    (Token,                SkipNotifications = false)

        /// <summary>
        /// Remove the given token.
        /// </summary>
        /// <param name="Token">A token.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<Token>> RemoveToken(Token              Token,
                                                     Boolean            SkipNotifications   = false,
                                                     EventTracking_Id?  EventTrackingId     = null,
                                                     User_Id?           CurrentUserId       = null)

            => RemoveToken(Token.Id,
                           SkipNotifications);

        #endregion

        #region RemoveToken    (TokenId,              SkipNotifications = false)

        /// <summary>
        /// Remove the given token.
        /// </summary>
        /// <param name="TokenId">A unique identification of a token.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<Token>> RemoveToken(Token_Id           TokenId,
                                                           Boolean            SkipNotifications   = false,
                                                           EventTracking_Id?  EventTrackingId     = null,
                                                           User_Id?           CurrentUserId       = null)
        {

            if (tokenStatus.Remove(TokenId, out var existingTokenStatus))
            {

                await LogAsset(removeTokenStatus,
                               existingTokenStatus.Token.ToJSON(true,
                                                                CustomTokenSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnTokenRemovedLocal = OnTokenRemoved;
                    if (OnTokenRemovedLocal is not null)
                    {
                        try
                        {
                            await OnTokenRemovedLocal(existingTokenStatus.Token);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveToken), " ", nameof(OnTokenRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<Token>.Success(existingTokenStatus.Token);

            }

            return RemoveResult<Token>.Failed(null,
                                              "RemoveToken(TokenId, ...) failed!");

        }

        #endregion

        #region RemoveAllTokens(                      SkipNotifications = false)

        /// <summary>
        /// Remove all tokens.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>> RemoveAllTokens(Boolean            SkipNotifications   = false,
                                                                            EventTracking_Id?  EventTrackingId     = null,
                                                                            User_Id?           CurrentUserId       = null)
        {

            var existingTokenStatus = tokenStatus.Values.ToArray();

            tokenStatus.Clear();

            await LogAsset(removeAllTokenStatus,
                           EventTrackingId ?? EventTracking_Id.New,
                           CurrentUserId);

            if (!SkipNotifications)
            {

                var OnTokenRemovedLocal = OnTokenRemoved;
                if (OnTokenRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var tokenStatus in existingTokenStatus)
                            await OnTokenRemovedLocal(tokenStatus.Token);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllTokens), " ", nameof(OnTokenRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<Token>>.Success(existingTokenStatus.Select(tokenStatus => tokenStatus.Token));

        }

        #endregion

        #region RemoveAllTokens(IncludeTokens,        SkipNotifications = false)

        /// <summary>
        /// Remove all tokens.
        /// </summary>
        /// <param name="IncludeTokens">A token filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>> RemoveAllTokens(Func<Token, Boolean>  IncludeTokens,
                                                                            Boolean               SkipNotifications   = false,
                                                                            EventTracking_Id?     EventTrackingId     = null,
                                                                            User_Id?              CurrentUserId       = null)
        {

            var removedTokens  = new List<Token>();
            var failedTokens   = new List<RemoveResult<Token>>();

            foreach (var token_status in tokenStatus.Values.Where(tokenstatus => IncludeTokens(tokenstatus.Token)).ToArray())
            {

                var result = await RemoveToken(token_status.Token.Id,
                                               SkipNotifications);

                if (result.IsSuccess)
                    removedTokens.Add(token_status.Token);
                else
                    failedTokens. Add(result);

            }

            return removedTokens.Any() && !failedTokens.Any()
                       ? RemoveResult<IEnumerable<Token>>.Success(removedTokens)

                       : !removedTokens.Any() && !failedTokens.Any()
                             ? RemoveResult<IEnumerable<Token>>.NoOperation(Array.Empty<Token>())
                             : RemoveResult<IEnumerable<Token>>.Failed     (failedTokens.Select(token => token.Data)!,
                                                                            failedTokens.Select(token => token.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllTokens(IncludeTokenIds,      SkipNotifications = false)

        /// <summary>
        /// Remove all matching tokens.
        /// </summary>
        /// <param name="IncludeTokenIds">A token identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>> RemoveAllTokens(Func<Token_Id, Boolean>  IncludeTokenIds,
                                                                            Boolean                  SkipNotifications   = false,
                                                                            EventTracking_Id?        EventTrackingId     = null,
                                                                            User_Id?                 CurrentUserId       = null)
        {

            var removedTokens  = new List<Token>();
            var failedTokens   = new List<RemoveResult<Token>>();

            foreach (var token_status in tokenStatus.Where  (kvp => IncludeTokenIds(kvp.Key)).
                                                     Select (kvp => kvp.Value).
                                                     ToArray())
            {

                var result = await RemoveToken(token_status.Token.Id,
                                               SkipNotifications);

                if (result.IsSuccess)
                    removedTokens.Add(token_status.Token);
                else
                    failedTokens. Add(result);

            }

            return removedTokens.Any() && !failedTokens.Any()
                       ? RemoveResult<IEnumerable<Token>>.Success(removedTokens)

                       : !removedTokens.Any() && !failedTokens.Any()
                             ? RemoveResult<IEnumerable<Token>>.NoOperation(Array.Empty<Token>())
                             : RemoveResult<IEnumerable<Token>>.Failed     (failedTokens.Select(token => token.Data)!,
                                                                             failedTokens.Select(token => token.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllTokens(CountryCode, PartyId, SkipNotifications = false)

        /// <summary>
        /// Remove all tokens owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<Token>>> RemoveAllTokens(CountryCode        CountryCode,
                                                                            Party_Id           PartyId,
                                                                            Boolean            SkipNotifications   = false,
                                                                            EventTracking_Id?  EventTrackingId     = null,
                                                                            User_Id?           CurrentUserId       = null)
        {

            var removedTokens  = new List<Token>();
            var failedTokens   = new List<RemoveResult<Token>>();

            foreach (var token_status in tokenStatus.Values.Where  (tokenstatus => CountryCode == tokenstatus.Token.CountryCode &&
                                                                                   PartyId     == tokenstatus.Token.PartyId).
                                                            ToArray())
            {

                var result = await RemoveToken(token_status.Token.Id,
                                               SkipNotifications);

                if (result.IsSuccess)
                    removedTokens.Add(token_status.Token);
                else
                    failedTokens. Add(result);

            }

            return removedTokens.Any() && !failedTokens.Any()
                       ? RemoveResult<IEnumerable<Token>>.Success(removedTokens)

                       : !removedTokens.Any() && !failedTokens.Any()
                             ? RemoveResult<IEnumerable<Token>>.NoOperation(Array.Empty<Token>())
                             : RemoveResult<IEnumerable<Token>>.Failed     (failedTokens.Select(token => token.Data)!,
                                                                             failedTokens.Select(token => token.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #endregion

        #region ChargeDetailRecords

        #region Data

        private readonly ConcurrentDictionary<CDR_Id, CDR> chargeDetailRecords = new();


        public delegate Task OnChargeDetailRecordAddedDelegate  (CDR CDR);
        public delegate Task OnChargeDetailRecordChangedDelegate(CDR CDR);
        public delegate Task OnChargeDetailRecordRemovedDelegate(CDR CDR);

        public event OnChargeDetailRecordAddedDelegate?    OnCDRAdded;
        public event OnChargeDetailRecordChangedDelegate?  OnCDRChanged;
        public event OnChargeDetailRecordRemovedDelegate?  OnCDRRemoved;

        #endregion


        #region AddCDR           (CDR,                          SkipNotifications = false)

        public async Task<AddResult<CDR>> AddCDR(CDR                CDR,
                                                 Boolean            SkipNotifications   = false,
                                                 EventTracking_Id?  EventTrackingId     = null,
                                                 User_Id?           CurrentUserId       = null)
        {

            if (chargeDetailRecords.TryAdd(CDR.Id, CDR))
            {

                CDR.CommonAPI = this;

                await LogAsset(addChargeDetailRecord,
                               CDR.ToJSON(true,
                                          true,
                                          true,
                                          null,
                                          CustomCDRSerializer,
                                          CustomLocationSerializer,
                                          CustomAdditionalGeoLocationSerializer,
                                          CustomEVSESerializer,
                                          CustomStatusScheduleSerializer,
                                          CustomConnectorSerializer,
                                          CustomEnergyMeterSerializer,
                                          CustomTransparencySoftwareStatusSerializer,
                                          CustomTransparencySoftwareSerializer,
                                          CustomDisplayTextSerializer,
                                          CustomBusinessDetailsSerializer,
                                          CustomHoursSerializer,
                                          CustomImageSerializer,
                                          CustomEnergyMixSerializer,
                                          CustomEnergySourceSerializer,
                                          CustomEnvironmentalImpactSerializer,
                                          CustomTariffSerializer,
                                          CustomTariffElementSerializer,
                                          CustomPriceComponentSerializer,
                                          CustomTariffRestrictionsSerializer,
                                          CustomChargingPeriodSerializer,
                                          CustomCDRDimensionSerializer,
                                          CustomCDRCostDetailsSerializer,
                                          CustomSignedDataSerializer,
                                          CustomSignedValueSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnChargeDetailRecordAddedLocal = OnCDRAdded;
                    if (OnChargeDetailRecordAddedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordAddedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddCDR), " ", nameof(OnCDRAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<CDR>.Success(CDR);

            }

            return AddResult<CDR>.Failed(CDR,
                                         "TryAdd(CDR.Id, CDR) failed!");

        }

        #endregion

        #region AddCDRIfNotExists(CDR,                          SkipNotifications = false)

        public async Task<AddResult<CDR>> AddCDRIfNotExists(CDR                CDR,
                                                            Boolean            SkipNotifications   = false,
                                                            EventTracking_Id?  EventTrackingId     = null,
                                                            User_Id?           CurrentUserId       = null)
        {

            if (chargeDetailRecords.TryAdd(CDR.Id, CDR))
            {

                CDR.CommonAPI = this;

                await LogAsset(addChargeDetailRecord,
                               CDR.ToJSON(true,
                                          true,
                                          true,
                                          null,
                                          CustomCDRSerializer,
                                          CustomLocationSerializer,
                                          CustomAdditionalGeoLocationSerializer,
                                          CustomEVSESerializer,
                                          CustomStatusScheduleSerializer,
                                          CustomConnectorSerializer,
                                          CustomEnergyMeterSerializer,
                                          CustomTransparencySoftwareStatusSerializer,
                                          CustomTransparencySoftwareSerializer,
                                          CustomDisplayTextSerializer,
                                          CustomBusinessDetailsSerializer,
                                          CustomHoursSerializer,
                                          CustomImageSerializer,
                                          CustomEnergyMixSerializer,
                                          CustomEnergySourceSerializer,
                                          CustomEnvironmentalImpactSerializer,
                                          CustomTariffSerializer,
                                          CustomTariffElementSerializer,
                                          CustomPriceComponentSerializer,
                                          CustomTariffRestrictionsSerializer,
                                          CustomChargingPeriodSerializer,
                                          CustomCDRDimensionSerializer,
                                          CustomCDRCostDetailsSerializer,
                                          CustomSignedDataSerializer,
                                          CustomSignedValueSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnChargeDetailRecordAddedLocal = OnCDRAdded;
                    if (OnChargeDetailRecordAddedLocal is not null)
                    {
                        try
                        {
                            OnChargeDetailRecordAddedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddCDR), " ", nameof(OnCDRAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddResult<CDR>.Success(CDR);

            }

            return AddResult<CDR>.NoOperation(CDR);

        }

        #endregion

        #region AddOrUpdateCDR   (CDR, AllowDowngrades = false, SkipNotifications = false)

        public async Task<AddOrUpdateResult<CDR>> AddOrUpdateCDR(CDR                CDR,
                                                                 Boolean?           AllowDowngrades     = false,
                                                                 Boolean            SkipNotifications   = false,
                                                                 EventTracking_Id?  EventTrackingId     = null,
                                                                 User_Id?           CurrentUserId       = null)
        {

            #region Update an existing charge detail record

            if (chargeDetailRecords.TryGetValue(CDR.Id, out var existingCDR))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    CDR.LastUpdated <= existingCDR.LastUpdated)
                {
                    return AddOrUpdateResult<CDR>.Failed(CDR,
                                                         "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");
                }

                chargeDetailRecords[CDR.Id] = CDR;
                CDR.CommonAPI = this;

                await LogAsset(addOrUpdateChargeDetailRecord,
                               CDR.ToJSON(true,
                                          true,
                                          true,
                                          null,
                                          CustomCDRSerializer,
                                          CustomLocationSerializer,
                                          CustomAdditionalGeoLocationSerializer,
                                          CustomEVSESerializer,
                                          CustomStatusScheduleSerializer,
                                          CustomConnectorSerializer,
                                          CustomEnergyMeterSerializer,
                                          CustomTransparencySoftwareStatusSerializer,
                                          CustomTransparencySoftwareSerializer,
                                          CustomDisplayTextSerializer,
                                          CustomBusinessDetailsSerializer,
                                          CustomHoursSerializer,
                                          CustomImageSerializer,
                                          CustomEnergyMixSerializer,
                                          CustomEnergySourceSerializer,
                                          CustomEnvironmentalImpactSerializer,
                                          CustomTariffSerializer,
                                          CustomTariffElementSerializer,
                                          CustomPriceComponentSerializer,
                                          CustomTariffRestrictionsSerializer,
                                          CustomChargingPeriodSerializer,
                                          CustomCDRDimensionSerializer,
                                          CustomCDRCostDetailsSerializer,
                                          CustomSignedDataSerializer,
                                          CustomSignedValueSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnCDRChangedLocal = OnCDRChanged;
                    if (OnCDRChangedLocal is not null)
                    {
                        try
                        {
                            OnCDRChangedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnCDRChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<CDR>.Success(CDR,
                                                      WasCreated: false);

            }

            #endregion

            #region Add a new charge detail record

            if (chargeDetailRecords.TryAdd(CDR.Id, CDR))
            {

                CDR.CommonAPI = this;

                if (!SkipNotifications)
                {

                    var OnCDRAddedLocal = OnCDRAdded;
                    if (OnCDRAddedLocal is not null)
                    {
                        try
                        {
                            OnCDRAddedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddOrUpdateCDR), " ", nameof(OnCDRAdded), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return AddOrUpdateResult<CDR>.Success(CDR,
                                                      WasCreated: true);

            }

            return AddOrUpdateResult<CDR>.Failed(CDR,
                                                 "AddOrUpdateCDR(CDR.Id, CDR) failed!");

            #endregion

        }

        #endregion

        #region UpdateCDR        (CDR, AllowDowngrades = false, SkipNotifications = false)

        public async Task<UpdateResult<CDR>> UpdateCDR(CDR                CDR,
                                                       Boolean?           AllowDowngrades     = false,
                                                       Boolean            SkipNotifications   = false,
                                                       EventTracking_Id?  EventTrackingId     = null,
                                                       User_Id?           CurrentUserId       = null)
        {

            #region Validate AllowDowngrades

            if (chargeDetailRecords.TryGetValue(CDR.Id, out var existingCDR))
            {

                if ((AllowDowngrades ?? this.AllowDowngrades) == false &&
                    CDR.LastUpdated <= existingCDR.LastUpdated)
                {

                    return UpdateResult<CDR>.Failed(CDR,
                                                    "The 'lastUpdated' timestamp of the new charge detail record must be newer then the timestamp of the existing charge detail record!");

                }

            }
            else
                return UpdateResult<CDR>.Failed(CDR,
                                                $"Unknown charge detail record identification '{CDR.Id}'!");

            #endregion


            if (chargeDetailRecords.TryUpdate(CDR.Id, CDR, existingCDR))
            {

                CDR.CommonAPI = this;

                await LogAsset(updateChargeDetailRecord,
                               CDR.ToJSON(true,
                                          true,
                                          true,
                                          null,
                                          CustomCDRSerializer,
                                          CustomLocationSerializer,
                                          CustomAdditionalGeoLocationSerializer,
                                          CustomEVSESerializer,
                                          CustomStatusScheduleSerializer,
                                          CustomConnectorSerializer,
                                          CustomEnergyMeterSerializer,
                                          CustomTransparencySoftwareStatusSerializer,
                                          CustomTransparencySoftwareSerializer,
                                          CustomDisplayTextSerializer,
                                          CustomBusinessDetailsSerializer,
                                          CustomHoursSerializer,
                                          CustomImageSerializer,
                                          CustomEnergyMixSerializer,
                                          CustomEnergySourceSerializer,
                                          CustomEnvironmentalImpactSerializer,
                                          CustomTariffSerializer,
                                          CustomTariffElementSerializer,
                                          CustomPriceComponentSerializer,
                                          CustomTariffRestrictionsSerializer,
                                          CustomChargingPeriodSerializer,
                                          CustomCDRDimensionSerializer,
                                          CustomCDRCostDetailsSerializer,
                                          CustomSignedDataSerializer,
                                          CustomSignedValueSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnCDRChangedLocal = OnCDRChanged;
                    if (OnCDRChangedLocal is not null)
                    {
                        try
                        {
                            OnCDRChangedLocal(CDR).Wait();
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(UpdateCDR), " ", nameof(OnCDRChanged), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return UpdateResult<CDR>.Success(CDR);

            }

            return UpdateResult<CDR>.Failed(CDR,
                                            "UpdateCDR(CDR.Id, CDR, CDR) failed!");

        }

        #endregion


        #region TryPatchCDR      (CDR, CDRPatch, AllowDowngrades = false, SkipNotifications = false)   // Non-Standard

        public async Task<PatchResult<CDR>> TryPatchCDR(CDR                CDR,
                                                        JObject            CDRPatch,
                                                        Boolean?           AllowDowngrades     = false,
                                                        Boolean            SkipNotifications   = false,
                                                        EventTracking_Id?  EventTrackingId     = null,
                                                        User_Id?           CurrentUserId       = null)
        {

            if (CDRPatch is null || !CDRPatch.HasValues)
                return PatchResult<CDR>.Failed(CDR,
                                               "The given charge detail record patch must not be null or empty!");

            if (chargeDetailRecords.TryGetValue(CDR.Id, out var existingCDR))
            {

                var patchResult = existingCDR.TryPatch(CDRPatch,
                                                       AllowDowngrades ?? this.AllowDowngrades ?? false);

                if (patchResult.IsSuccess &&
                    patchResult.PatchedData is not null)
                {

                    chargeDetailRecords[CDR.Id] = patchResult.PatchedData;

                    await LogAsset(updateChargeDetailRecord,
                                   CDR.ToJSON(true,
                                              true,
                                              true,
                                              null,
                                              CustomCDRSerializer,
                                              CustomLocationSerializer,
                                              CustomAdditionalGeoLocationSerializer,
                                              CustomEVSESerializer,
                                              CustomStatusScheduleSerializer,
                                              CustomConnectorSerializer,
                                              CustomEnergyMeterSerializer,
                                              CustomTransparencySoftwareStatusSerializer,
                                              CustomTransparencySoftwareSerializer,
                                              CustomDisplayTextSerializer,
                                              CustomBusinessDetailsSerializer,
                                              CustomHoursSerializer,
                                              CustomImageSerializer,
                                              CustomEnergyMixSerializer,
                                              CustomEnergySourceSerializer,
                                              CustomEnvironmentalImpactSerializer,
                                              CustomTariffSerializer,
                                              CustomTariffElementSerializer,
                                              CustomPriceComponentSerializer,
                                              CustomTariffRestrictionsSerializer,
                                              CustomChargingPeriodSerializer,
                                              CustomCDRDimensionSerializer,
                                              CustomCDRCostDetailsSerializer,
                                              CustomSignedDataSerializer,
                                              CustomSignedValueSerializer),
                                   EventTrackingId ?? EventTracking_Id.New,
                                   CurrentUserId);

                    if (!SkipNotifications)
                    {

                        var OnChargeDetailRecordChangedLocal = OnCDRChanged;
                        if (OnChargeDetailRecordChangedLocal is not null)
                        {
                            try
                            {
                                OnChargeDetailRecordChangedLocal(patchResult.PatchedData).Wait();
                            }
                            catch (Exception e)
                            {
                                DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(TryPatchCDR), " ", nameof(OnCDRChanged), ": ",
                                            Environment.NewLine, e.Message,
                                            Environment.NewLine, e.StackTrace ?? "");
                            }
                        }

                    }

                }

                return patchResult;

            }

            else
                return PatchResult<CDR>.Failed(CDR,
                                                  "The given charge detail record does not exist!");

        }

        #endregion


        #region CDRExists(CDRId)

        public Boolean CDRExists(CDR_Id CDRId)

            => chargeDetailRecords.ContainsKey(CDRId);

        #endregion

        #region TryGetCDR(CDRId, out CDR)

        public Boolean TryGetCDR(CDR_Id    CDRId,
                                 out CDR?  CDR)
        {

            if (chargeDetailRecords.TryGetValue(CDRId, out CDR))
                return true;

            CDR = null;
            return false;

        }

        #endregion

        #region GetCDRs  (IncludeCDRs = null)

        /// <summary>
        /// Return all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">An optional charge detail record filter.</param>
        public IEnumerable<CDR> GetCDRs(Func<CDR, Boolean>? IncludeCDRs = null)

            => IncludeCDRs is null
                   ? chargeDetailRecords.Values
                   : chargeDetailRecords.Values.Where(IncludeCDRs);

        #endregion

        #region GetCDRs  (CountryCode, PartyId)

        /// <summary>
        /// Return all charge detail records owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        public IEnumerable<CDR> GetCDRs(CountryCode  CountryCode,
                                        Party_Id     PartyId)

            => chargeDetailRecords.Values.Where(cdr => cdr.CountryCode == CountryCode &&
                                                       cdr.PartyId     == PartyId);

        #endregion


        #region RemoveCDR    (CDR,                  SkipNotifications = false)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="CDR">A charge detail record.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public Task<RemoveResult<CDR>> RemoveCDR(CDR                CDR,
                                                 Boolean            SkipNotifications   = false,
                                                 EventTracking_Id?  EventTrackingId     = null,
                                                 User_Id?           CurrentUserId       = null)

            => RemoveCDR(CDR.Id,
                         SkipNotifications);

        #endregion

        #region RemoveCDR    (CDRId,                SkipNotifications = false)

        /// <summary>
        /// Remove the given charge detail record.
        /// </summary>
        /// <param name="CDRId">A unique identification of a charge detail record.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<CDR>> RemoveCDR(CDR_Id             CDRId,
                                                       Boolean            SkipNotifications   = false,
                                                       EventTracking_Id?  EventTrackingId     = null,
                                                       User_Id?           CurrentUserId       = null)
        {

            if (chargeDetailRecords.Remove(CDRId, out var cdr))
            {

                await LogAsset(removeChargeDetailRecord,
                               cdr.ToJSON(true,
                                          true,
                                          true,
                                          null,
                                          CustomCDRSerializer,
                                          CustomLocationSerializer,
                                          CustomAdditionalGeoLocationSerializer,
                                          CustomEVSESerializer,
                                          CustomStatusScheduleSerializer,
                                          CustomConnectorSerializer,
                                          CustomEnergyMeterSerializer,
                                          CustomTransparencySoftwareStatusSerializer,
                                          CustomTransparencySoftwareSerializer,
                                          CustomDisplayTextSerializer,
                                          CustomBusinessDetailsSerializer,
                                          CustomHoursSerializer,
                                          CustomImageSerializer,
                                          CustomEnergyMixSerializer,
                                          CustomEnergySourceSerializer,
                                          CustomEnvironmentalImpactSerializer,
                                          CustomTariffSerializer,
                                          CustomTariffElementSerializer,
                                          CustomPriceComponentSerializer,
                                          CustomTariffRestrictionsSerializer,
                                          CustomChargingPeriodSerializer,
                                          CustomCDRDimensionSerializer,
                                          CustomCDRCostDetailsSerializer,
                                          CustomSignedDataSerializer,
                                          CustomSignedValueSerializer),
                               EventTrackingId ?? EventTracking_Id.New,
                               CurrentUserId);

                if (!SkipNotifications)
                {

                    var OnCDRRemovedLocal = OnCDRRemoved;
                    if (OnCDRRemovedLocal is not null)
                    {
                        try
                        {
                            await OnCDRRemovedLocal(cdr);
                        }
                        catch (Exception e)
                        {
                            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveCDR), " ", nameof(OnCDRRemoved), ": ",
                                        Environment.NewLine, e.Message,
                                        Environment.NewLine, e.StackTrace ?? "");
                        }
                    }

                }

                return RemoveResult<CDR>.Success(cdr);

            }

            return RemoveResult<CDR>.Failed(null,
                                            "Remove(CDRId, ...) failed!");

        }

        #endregion

        #region RemoveAllCDRs(                      SkipNotifications = false)

        /// <summary>
        /// Remove all charge detail records.
        /// </summary>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>> RemoveAllCDRs(Boolean            SkipNotifications   = false,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        User_Id?           CurrentUserId       = null)
        {

            var existingCDRs = chargeDetailRecords.Values.ToArray();

            chargeDetailRecords.Clear();

            await LogAsset(removeAllChargeDetailRecords,
                           EventTrackingId ?? EventTracking_Id.New,
                           CurrentUserId);

            if (!SkipNotifications)
            {

                var OnCDRRemovedLocal = OnCDRRemoved;
                if (OnCDRRemovedLocal is not null)
                {
                    try
                    {
                        foreach (var location in existingCDRs)
                            await OnCDRRemovedLocal(location);
                    }
                    catch (Exception e)
                    {
                        DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(RemoveAllCDRs), " ", nameof(OnCDRRemoved), ": ",
                                    Environment.NewLine, e.Message,
                                    Environment.NewLine, e.StackTrace ?? "");
                    }
                }

            }

            return RemoveResult<IEnumerable<CDR>>.Success(existingCDRs);

        }

        #endregion

        #region RemoveAllCDRs(IncludeCDRs,          SkipNotifications = false)

        /// <summary>
        /// Remove all matching charge detail records.
        /// </summary>
        /// <param name="IncludeCDRs">A charge detail record filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>> RemoveAllCDRs(Func<CDR, Boolean>  IncludeCDRs,
                                                                        Boolean             SkipNotifications   = false,
                                                                        EventTracking_Id?   EventTrackingId     = null,
                                                                        User_Id?            CurrentUserId       = null)
        {

            var removedCDRs  = new List<CDR>();
            var failedCDRs   = new List<RemoveResult<CDR>>();

            foreach (var cdr in chargeDetailRecords.Values.Where(IncludeCDRs).ToArray())
            {

                var result = await RemoveCDR(cdr.Id,
                                             SkipNotifications);

                if (result.IsSuccess)
                    removedCDRs.Add(cdr);
                else
                    failedCDRs. Add(result);

            }

            return removedCDRs.Any() && !failedCDRs.Any()
                       ? RemoveResult<IEnumerable<CDR>>.Success(removedCDRs)

                       : !removedCDRs.Any() && !failedCDRs.Any()
                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(Array.Empty<CDR>())
                             : RemoveResult<IEnumerable<CDR>>.Failed     (failedCDRs.Select(cdr => cdr.Data)!,
                                                                          failedCDRs.Select(cdr => cdr.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllCDRs(IncludeCDRIds,        SkipNotifications = false)

        /// <summary>
        /// Remove all matching cdrs.
        /// </summary>
        /// <param name="IncludeCDRIds">A charging cdr identification filter.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>> RemoveAllCDRs(Func<CDR_Id, Boolean>  IncludeCDRIds,
                                                                        Boolean                SkipNotifications   = false,
                                                                        EventTracking_Id?      EventTrackingId     = null,
                                                                        User_Id?               CurrentUserId       = null)
        {

            var removedCDRs  = new List<CDR>();
            var failedCDRs   = new List<RemoveResult<CDR>>();

            foreach (var cdr in chargeDetailRecords.Where  (kvp => IncludeCDRIds(kvp.Key)).
                                                    Select (kvp => kvp.Value).
                                                    ToArray())
            {

                var result = await RemoveCDR(cdr.Id,
                                             SkipNotifications);

                if (result.IsSuccess)
                    removedCDRs.Add(cdr);
                else
                    failedCDRs. Add(result);

            }

            return removedCDRs.Any() && !failedCDRs.Any()
                       ? RemoveResult<IEnumerable<CDR>>.Success(removedCDRs)

                       : !removedCDRs.Any() && !failedCDRs.Any()
                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(Array.Empty<CDR>())
                             : RemoveResult<IEnumerable<CDR>>.Failed     (failedCDRs.Select(cdr => cdr.Data)!,
                                                                          failedCDRs.Select(cdr => cdr.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #region RemoveAllCDRs(CountryCode, PartyId, SkipNotifications = false)

        /// <summary>
        /// Remove all charge detail records owned by the given party.
        /// </summary>
        /// <param name="CountryCode">The country code of the party.</param>
        /// <param name="PartyId">The identification of the party.</param>
        /// <param name="SkipNotifications">Skip sending notifications.</param>
        public async Task<RemoveResult<IEnumerable<CDR>>> RemoveAllCDRs(CountryCode        CountryCode,
                                                                        Party_Id           PartyId,
                                                                        Boolean            SkipNotifications   = false,
                                                                        EventTracking_Id?  EventTrackingId     = null,
                                                                        User_Id?           CurrentUserId       = null)
        {

            var removedCDRs  = new List<CDR>();
            var failedCDRs   = new List<RemoveResult<CDR>>();

            foreach (var cdr in chargeDetailRecords.Values.Where  (cdr => CountryCode == cdr.CountryCode &&
                                                                          PartyId     == cdr.PartyId).
                                                           ToArray())
            {

                var result = await RemoveCDR(cdr.Id,
                                             SkipNotifications);

                if (result.IsSuccess)
                    removedCDRs.Add(cdr);
                else
                    failedCDRs. Add(result);

            }

            return removedCDRs.Any() && !failedCDRs.Any()
                       ? RemoveResult<IEnumerable<CDR>>.Success(removedCDRs)

                       : !removedCDRs.Any() && !failedCDRs.Any()
                             ? RemoveResult<IEnumerable<CDR>>.NoOperation(Array.Empty<CDR>())
                             : RemoveResult<IEnumerable<CDR>>.Failed     (failedCDRs.Select(cdr => cdr.Data)!,
                                                                          failedCDRs.Select(cdr => cdr.ErrorResponse).AggregateWith(", "));

        }

        #endregion

        #endregion


    }

}
