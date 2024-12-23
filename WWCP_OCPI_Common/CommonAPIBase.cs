﻿/*
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

using System.Text;
using System.Security.Authentication;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    public delegate String OCPILogfileCreatorDelegate(String         LoggingPath,
                                                      IRemoteParty?  RemoteParty,
                                                      String         Context,
                                                      String         LogfileName);


    /// <summary>
    /// The CommonAPI Base.
    /// </summary>
    public class CommonAPIBase : HTTPAPI,
                                 IServerStartStop
    {

        #region (class) ClientConfigurator

        /// <summary>
        /// A OCPI client configurator.
        /// </summary>
        public sealed class ClientConfigurator
        {

            /// <summary>
            /// The description of the OCPI client.
            /// </summary>
            public Func<RemoteParty_Id, I18NString>?  Description       { get; set; }

            /// <summary>
            /// Whether logging is disabled for this OCPI client.
            /// </summary>
            public Func<RemoteParty_Id, Boolean>?     DisableLogging    { get; set; }

            /// <summary>
            /// The logging path for this OCPI client.
            /// </summary>
            public Func<RemoteParty_Id, String>?      LoggingPath       { get; set; }

            /// <summary>
            /// The logging context for this OCPI client.
            /// </summary>
            public Func<RemoteParty_Id, String>?      LoggingContext    { get; set; }

            /// <summary>
            /// The logfile creator for this OCPI client.
            /// </summary>
            public OCPILogfileCreatorDelegate?        LogfileCreator    { get; set; }

        }

        #endregion

        #region (class) Command

        /// <summary>
        /// An OCPI API command.
        /// </summary>
        public sealed class Command
        {

            #region Properties

            /// <summary>
            /// The name of the command.
            /// </summary>
            public String    CommandName    { get; }

            /// <summary>
            /// An optional command JSON object parameter.
            /// </summary>
            public JObject?  JSONObject     { get; }

            /// <summary>
            /// An optional command JSON array parameter.
            /// </summary>
            public JArray?   JSONArray      { get; }

            /// <summary>
            /// An optional command message parameter.
            /// </summary>
            public String?   Message        { get; }

            /// <summary>
            /// An optional command Integer parameter.
            /// </summary>
            public Int64?    Integer        { get; }

            /// <summary>
            /// An optional command Single/Float parameter.
            /// </summary>
            public Single?   Single         { get; }

            /// <summary>
            /// An optional command Boolean parameter.
            /// </summary>
            public Boolean?  Boolean        { get; }

            #endregion

            #region Constructor(s)

            #region Command(CommandName, Message)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="Message">A command message parameter.</param>
            public Command(String    CommandName,
                           String?   Message)
            {

                this.CommandName  = CommandName;
                this.Message      = Message;

            }

            #endregion

            #region Command(CommandName, JSONObject)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="JSONObject">A command JSON object parameter.</param>
            public Command(String    CommandName,
                           JObject?  JSONObject)
            {

                this.CommandName  = CommandName;
                this.JSONObject   = JSONObject;

            }

            #endregion

            #region Command(CommandName, JSONArray)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="JSONArray">A command JSON array parameter.</param>
            public Command(String   CommandName,
                           JArray?  JSONArray)
            {

                this.CommandName  = CommandName;
                this.JSONArray    = JSONArray;

            }

            #endregion

            #region Command(CommandName, Integer)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="Integer">A command Integer parameter.</param>
            public Command(String    CommandName,
                           Int64?    Integer)
            {

                this.CommandName  = CommandName;
                this.Integer       = Integer;

            }

            #endregion

            #region Command(CommandName, Single)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="Single">A command single/float parameter.</param>
            public Command(String    CommandName,
                           Single?   Single)
            {

                this.CommandName  = CommandName;
                this.Single       = Single;

            }

            #endregion

            #region Command(CommandName, Boolean)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="Boolean">A command Boolean parameter.</param>
            public Command(String    CommandName,
                           Boolean?  Boolean)
            {

                this.CommandName  = CommandName;
                this.Boolean      = Boolean;

            }

            #endregion

            #endregion

            #region (override) ToString()

            /// <summary>
            /// Return a text representation of this object.
            /// </summary>
            public override String ToString()

                => $"'{CommandName}' => {Message                                                               ??
                                         JSONObject?.   ToString(Newtonsoft.Json.Formatting.None)?.SubstringMax(100) ??
                                         Integer?.ToString()                                                   ??
                                         Single?. ToString()                                                   ??
                                         String.Empty}";

            #endregion

        }

        #endregion

        #region (class) CommandWithMetadata

        /// <summary>
        /// An OCPI API command with additional metadata.
        /// </summary>
        public sealed class CommandWithMetadata
        {

            #region Properties

            /// <summary>
            /// The name of the command.
            /// </summary>
            public String            CommandName        { get; }

            /// <summary>
            /// An optional command JSON object parameter.
            /// </summary>
            public JObject?          JSONObject         { get; }

            /// <summary>
            /// An optional command JSON array parameter.
            /// </summary>
            public JArray?           JSONArray          { get; }

            /// <summary>
            /// An optional command message parameter.
            /// </summary>
            public String?           Message            { get; }

            /// <summary>
            /// An optional command Integer parameter.
            /// </summary>
            public Int64?            Integer            { get; }

            /// <summary>
            /// An optional command Single/Float parameter.
            /// </summary>
            public Single?           Single             { get; }

            /// <summary>
            /// An optional command Boolean parameter.
            /// </summary>
            public Boolean?          Boolean            { get; }

            /// <summary>
            /// The timestamp of the command.
            /// </summary>
            public DateTime          Timestamp          { get; }

            /// <summary>
            /// The unique event tracking identification for correlating this request with other events.
            /// </summary>
            public EventTracking_Id  EventTrackingId    { get; }

            /// <summary>
            /// The optional user identification initiating this command/request.
            /// </summary>
            public User_Id?          UserId             { get; }

            #endregion

            #region Constructor(s)

            #region CommandWithMetadata(CommandName, Message,    Timestamp, UserId = null)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="Message">A command message parameter.</param>
            /// <param name="Timestamp">The timestamp of the command.</param>
            /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
            /// <param name="UserId">An optional user identification initiating this command/request.</param>
            public CommandWithMetadata(String            CommandName,
                                       String?           Message,
                                       DateTime          Timestamp,
                                       EventTracking_Id  EventTrackingId,
                                       User_Id?          UserId   = null)
            {

                this.CommandName      = CommandName;
                this.Message          = Message;
                this.Timestamp        = Timestamp;
                this.EventTrackingId  = EventTrackingId;
                this.UserId           = UserId;

            }

            #endregion

            #region CommandWithMetadata(CommandName, JSONObject, Timestamp, UserId = null)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="JSONObject">A command JSON object parameter.</param>
            /// <param name="Timestamp">The timestamp of the command.</param>
            /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
            /// <param name="UserId">An optional user identification initiating this command/request.</param>
            public CommandWithMetadata(String            CommandName,
                                       JObject?          JSONObject,
                                       DateTime          Timestamp,
                                       EventTracking_Id  EventTrackingId,
                                       User_Id?          UserId   = null)
            {

                this.CommandName      = CommandName;
                this.JSONObject       = JSONObject;
                this.Timestamp        = Timestamp;
                this.EventTrackingId  = EventTrackingId;
                this.UserId           = UserId;

            }

            #endregion

            #region CommandWithMetadata(CommandName, JSONArray,  Timestamp, UserId = null)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="JSONArray">A command JSON array parameter.</param>
            /// <param name="Timestamp">The timestamp of the command.</param>
            /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
            /// <param name="UserId">An optional user identification initiating this command/request.</param>
            public CommandWithMetadata(String            CommandName,
                                       JArray?           JSONArray,
                                       DateTime          Timestamp,
                                       EventTracking_Id  EventTrackingId,
                                       User_Id?          UserId   = null)
            {

                this.CommandName      = CommandName;
                this.JSONArray        = JSONArray;
                this.Timestamp        = Timestamp;
                this.EventTrackingId  = EventTrackingId;
                this.UserId           = UserId;

            }

            #endregion

            #region CommandWithMetadata(CommandName, Integer,    Timestamp, UserId = null)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="Boolean">A command Integer parameter.</param>
            /// <param name="Timestamp">The timestamp of the command.</param>
            /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
            /// <param name="UserId">An optional user identification initiating this command/request.</param>
            public CommandWithMetadata(String            CommandName,
                                       Int64?            Boolean,
                                       DateTime          Timestamp,
                                       EventTracking_Id  EventTrackingId,
                                       User_Id?          UserId   = null)
            {

                this.CommandName      = CommandName;
                this.Integer          = Boolean;
                this.Timestamp        = Timestamp;
                this.EventTrackingId  = EventTrackingId;
                this.UserId           = UserId;

            }

            #endregion

            #region CommandWithMetadata(CommandName, Single,     Timestamp, UserId = null)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="Single">A command Single parameter.</param>
            /// <param name="Timestamp">The timestamp of the command.</param>
            /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
            /// <param name="UserId">An optional user identification initiating this command/request.</param>
            public CommandWithMetadata(String            CommandName,
                                       Single?           Single,
                                       DateTime          Timestamp,
                                       EventTracking_Id  EventTrackingId,
                                       User_Id?          UserId   = null)
            {

                this.CommandName      = CommandName;
                this.Single           = Single;
                this.Timestamp        = Timestamp;
                this.EventTrackingId  = EventTrackingId;
                this.UserId           = UserId;

            }

            #endregion

            #region CommandWithMetadata(CommandName, Boolean,    Timestamp, UserId = null)

            /// <summary>
            /// Create a new command using the given command name and message.
            /// </summary>
            /// <param name="CommandName">The name of the command.</param>
            /// <param name="Boolean">A command Boolean parameter.</param>
            /// <param name="Timestamp">The timestamp of the command.</param>
            /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
            /// <param name="UserId">An optional user identification initiating this command/request.</param>
            public CommandWithMetadata(String            CommandName,
                                       Boolean?          Boolean,
                                       DateTime          Timestamp,
                                       EventTracking_Id  EventTrackingId,
                                       User_Id?          UserId   = null)
            {

                this.CommandName      = CommandName;
                this.Boolean          = Boolean;
                this.Timestamp        = Timestamp;
                this.EventTrackingId  = EventTrackingId;
                this.UserId           = UserId;

            }

            #endregion

            #endregion

            #region (override) ToString()

            /// <summary>
            /// Return a text representation of this object.
            /// </summary>
            public override String ToString()

                => $"'{CommandName}' {(UserId is not null
                                           ? $"({UserId})"
                                           : String.Empty)} => {Message                                                              ??
                                                                JSONObject?.  ToString(Newtonsoft.Json.Formatting.None)?.SubstringMax(100) ??
                                                                Integer?.ToString()                                                   ??
                                                                String.Empty}";

            #endregion

        }

        #endregion


        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String      DefaultHTTPServerName           = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String      DefaultHTTPServiceName          = "GraphDefined OCPI HTTP API v0.1";

        /// <summary>
        /// The default HTTP server TCP port.  
        /// </summary>
        public new static readonly IPPort      DefaultHTTPServerPort           = IPPort.Parse(8080);

        /// <summary>
        /// The default HTTP URL path prefix.  
        /// </summary>
        public new static readonly HTTPPath    DefaultURLPathPrefix            = HTTPPath.Parse("io/OCPI/");

        /// <summary>
        /// The (max supported) OCPI version.
        /// </summary>
        private readonly           Version_Id  OCPIVersion;

        /// <summary>
        /// The default database file name for all remote party configuration.
        /// </summary>
        public const               String      DefaultRemotePartyDBFileName    = "RemoteParties.db";

        /// <summary>
        /// The default database file name for all OCPI assets.
        /// </summary>
        public const               String      DefaultAssetsDBFileName         = "Assets.db";


        protected const String addRemoteParty                     = "addRemoteParty";
        protected const String addRemotePartyIfNotExists          = "addRemotePartyIfNotExists";
        protected const String addOrUpdateRemoteParty             = "addOrUpdateRemoteParty";
        protected const String updateRemoteParty                  = "updateRemoteParty";
        protected const String removeRemoteParty                  = "removeRemoteParty";
        protected const String removeAllRemoteParties             = "removeAllRemoteParties";

        protected const String addLocation                        = "addLocation";
        protected const String addLocationIfNotExists             = "addLocationIfNotExists";
        protected const String addOrUpdateLocation                = "addOrUpdateLocation";
        protected const String updateLocation                     = "updateLocation";
        protected const String removeLocation                     = "removeLocation";
        protected const String removeAllLocations                 = "removeAllLocations";

        protected const String addEVSE                            = "addEVSE";
        protected const String addEVSEIfNotExists                 = "addEVSEIfNotExists";
        protected const String addOrUpdateEVSE                    = "addOrUpdateEVSE";
        protected const String updateEVSE                         = "updateEVSE";
        protected const String removeEVSE                         = "removeEVSE";
        protected const String removeAllEVSEs                     = "removeAllEVSEs";

        protected const String addTariff                          = "addTariff";
        protected const String addTariffIfNotExists               = "addTariffIfNotExists";
        protected const String addOrUpdateTariff                  = "addOrUpdateTariff";
        protected const String updateTariff                       = "updateTariff";
        protected const String removeTariff                       = "removeTariff";
        protected const String removeAllTariffs                   = "removeAllTariffs";

        protected const String addSession                         = "addSession";
        protected const String addSessionIfNotExists              = "addSessionIfNotExists";
        protected const String addOrUpdateSession                 = "addOrUpdateSession";
        protected const String updateSession                      = "updateSession";
        protected const String removeSession                      = "removeSession";
        protected const String removeAllSessions                  = "removeAllSessions";

        protected const String addTokenStatus                     = "addTokenStatus";
        protected const String addTokenStatusIfNotExists          = "addTokenStatusIfNotExists";
        protected const String addOrUpdateTokenStatus             = "addOrUpdateTokenStatus";
        protected const String updateTokenStatus                  = "updateTokenStatus";
        protected const String removeTokenStatus                  = "removeTokenStatus";
        protected const String removeAllTokenStatus               = "removeAllTokenStatus";

        protected const String addChargeDetailRecord              = "addChargeDetailRecord";
        protected const String addChargeDetailRecordIfNotExists   = "addChargeDetailRecordIfNotExists";
        protected const String addOrUpdateChargeDetailRecord      = "addOrUpdateChargeDetailRecord";
        protected const String updateChargeDetailRecord           = "updateChargeDetailRecord";
        protected const String removeChargeDetailRecord           = "removeChargeDetailRecord";
        protected const String removeAllChargeDetailRecords       = "removeAllChargeDetailRecords";

        #endregion

        #region Properties

        /// <summary>
        /// The URL for your API endpoint.
        /// </summary>
        public URL                      OurBaseURL                 { get; }

        /// <summary>
        /// The URL to your OCPI /versions endpoint.
        /// </summary>
        [Mandatory]
        public URL                      OurVersionsURL             { get; }

        /// <summary>
        /// An optional additional URL path prefix.
        /// </summary>
        public HTTPPath?                AdditionalURLPathPrefix    { get; }

        /// <summary>
        /// Allow anonymous access to locations as Open Data.
        /// </summary>
        public Boolean                  LocationsAsOpenData        { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?                 AllowDowngrades            { get; }

        /// <summary>
        /// Whether to disable the HTTP root services.
        /// </summary>
        public Boolean                  Disable_RootServices       { get; }

        /// <summary>
        /// The logging context.
        /// </summary>
        public String?                  LoggingContext             { get; }

        /// <summary>
        /// A template for OCPI client configurations.
        /// </summary>
        public ClientConfigurator       ClientConfigurations       { get; }


        public String                   DatabaseFilePath           { get; }

        /// <summary>
        /// The database file name for all remote party configuration.
        /// </summary>
        public String                   RemotePartyDBFileName      { get; }

        /// <summary>
        /// The database file name for all OCPI assets.
        /// </summary>
        public String                   AssetsDBFileName           { get; }

        #endregion

        #region Constructor(s)

        #region CommonAPI(HTTPServerName, ...)

        /// <summary>
        /// Create a new CommonAPI.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
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
        public CommonAPIBase(Version_Id                                                 OCPIVersion,
                             URL                                                        OurBaseURL,
                             URL                                                        OurVersionsURL,

                             HTTPPath?                                                  AdditionalURLPathPrefix       = null,
                             Boolean                                                    LocationsAsOpenData           = true,
                             Boolean?                                                   AllowDowngrades               = null,
                             Boolean                                                    Disable_RootServices          = true,

                             HTTPHostname?                                              HTTPHostname                  = null,
                             String?                                                    ExternalDNSName               = null,
                             IPPort?                                                    HTTPServerPort                = null,
                             HTTPPath?                                                  BasePath                      = null,
                             String?                                                    HTTPServerName                = DefaultHTTPServerName,

                             HTTPPath?                                                  URLPathPrefix                 = null,
                             String?                                                    HTTPServiceName               = DefaultHTTPServiceName,
                             JObject?                                                   APIVersionHashes              = null,

                             ServerCertificateSelectorDelegate?                         ServerCertificateSelector     = null,
                             RemoteTLSClientCertificateValidationHandler<IHTTPServer>?  ClientCertificateValidator    = null,
                             LocalCertificateSelectionHandler?                          LocalCertificateSelector      = null,
                             SslProtocols?                                              AllowedTLSProtocols           = null,
                             Boolean?                                                   ClientCertificateRequired     = null,
                             Boolean?                                                   CheckCertificateRevocation    = null,

                             ServerThreadNameCreatorDelegate?                           ServerThreadNameCreator       = null,
                             ServerThreadPriorityDelegate?                              ServerThreadPrioritySetter    = null,
                             Boolean?                                                   ServerThreadIsBackground      = null,
                             ConnectionIdBuilder?                                       ConnectionIdBuilder           = null,
                             TimeSpan?                                                  ConnectionTimeout             = null,
                             UInt32?                                                    MaxClientConnections          = null,

                             Boolean?                                                   DisableMaintenanceTasks       = null,
                             TimeSpan?                                                  MaintenanceInitialDelay       = null,
                             TimeSpan?                                                  MaintenanceEvery              = null,

                             Boolean?                                                   DisableWardenTasks            = null,
                             TimeSpan?                                                  WardenInitialDelay            = null,
                             TimeSpan?                                                  WardenCheckEvery              = null,

                             Boolean?                                                   IsDevelopment                 = null,
                             IEnumerable<String>?                                       DevelopmentServers            = null,
                             Boolean?                                                   DisableLogging                = null,
                             String?                                                    LoggingContext                = null,
                             String?                                                    LoggingPath                   = null,
                             String?                                                    LogfileName                   = null,
                             OCPILogfileCreatorDelegate?                                LogfileCreator                = null,
                             String?                                                    DatabaseFilePath              = null,
                             String?                                                    RemotePartyDBFileName         = null,
                             String?                                                    AssetsDBFileName              = null,
                             DNSClient?                                                 DNSClient                     = null,
                             Boolean                                                    AutoStart                     = false)

            : base(HTTPHostname,
                   ExternalDNSName,
                   HTTPServerPort  ?? DefaultHTTPServerPort,
                   BasePath,
                   HTTPServerName  ?? DefaultHTTPServerName,

                   URLPathPrefix   ?? DefaultURLPathPrefix,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   null, //HTMLTemplate,
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
                   LoggingPath,
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   DNSClient,
                   AutoStart)

        {

            this.OCPIVersion               = OCPIVersion;
            this.OurBaseURL                = OurBaseURL;
            this.OurVersionsURL            = OurVersionsURL;
            this.AdditionalURLPathPrefix   = AdditionalURLPathPrefix;
            this.LocationsAsOpenData       = LocationsAsOpenData;
            this.AllowDowngrades           = AllowDowngrades;
            this.Disable_RootServices      = Disable_RootServices;
            this.LoggingContext            = LoggingContext;

            //this.logfileName               = Path.Combine(this.LoggingPath,
            //                                              this.LogfileName);

            this.DatabaseFilePath          = DatabaseFilePath      ?? Path.Combine(AppContext.BaseDirectory,
                                                                                   DefaultHTTPAPI_LoggingPath);

            if (this.DatabaseFilePath[^1] != Path.DirectorySeparatorChar)
                this.DatabaseFilePath     += Path.DirectorySeparatorChar;

            this.RemotePartyDBFileName     = Path.Combine(this.DatabaseFilePath,
                                                          RemotePartyDBFileName ?? DefaultRemotePartyDBFileName);

            this.AssetsDBFileName          = Path.Combine(this.DatabaseFilePath,
                                                          AssetsDBFileName      ?? DefaultAssetsDBFileName);

            this.ClientConfigurations      = new ClientConfigurator();

        }

        #endregion

        #region CommonAPI(HTTPServer, ...)

        /// <summary>
        /// Create a new CommonAPI using the given HTTP server.
        /// </summary>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// <param name="HTTPServer">A HTTP server.</param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
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
        public CommonAPIBase(Version_Id                   OCPIVersion,
                             URL                          OurBaseURL,
                             URL                          OurVersionsURL,
                             HTTPServer                   HTTPServer,

                             HTTPPath?                    AdditionalURLPathPrefix    = null,
                             Boolean                      LocationsAsOpenData        = true,
                             Boolean?                     AllowDowngrades            = null,
                             Boolean                      Disable_RootServices       = false,

                             HTTPHostname?                HTTPHostname               = null,
                             String?                      ExternalDNSName            = "",
                             String?                      HTTPServiceName            = DefaultHTTPServiceName,
                             HTTPPath?                    BasePath                   = null,

                             HTTPPath?                    URLPathPrefix              = null,
                             JObject?                     APIVersionHashes           = null,

                             Boolean?                     DisableMaintenanceTasks    = false,
                             TimeSpan?                    MaintenanceInitialDelay    = null,
                             TimeSpan?                    MaintenanceEvery           = null,

                             Boolean?                     DisableWardenTasks         = false,
                             TimeSpan?                    WardenInitialDelay         = null,
                             TimeSpan?                    WardenCheckEvery           = null,

                             Boolean?                     IsDevelopment              = false,
                             IEnumerable<String>?         DevelopmentServers         = null,
                             Boolean?                     DisableLogging             = false,
                             String?                      LoggingContext             = null,
                             String?                      LoggingPath                = null,
                             String?                      LogfileName                = null,
                             OCPILogfileCreatorDelegate?  LogfileCreator             = null,
                             String?                      DatabaseFilePath           = null,
                             String?                      RemotePartyDBFileName      = null,
                             String?                      AssetsDBFileName           = null,
                             Boolean                      AutoStart                  = false)

            : base(HTTPServer,
                   HTTPHostname,
                   ExternalDNSName,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   BasePath,

                   URLPathPrefix,//   ?? DefaultURLPathPrefix,
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
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null,
                   AutoStart)

        {

            this.OCPIVersion               = OCPIVersion;
            this.OurBaseURL                = OurBaseURL;
            this.OurVersionsURL            = OurVersionsURL;
            this.AdditionalURLPathPrefix   = AdditionalURLPathPrefix;
            this.LocationsAsOpenData       = LocationsAsOpenData;
            this.AllowDowngrades           = AllowDowngrades;
            this.Disable_RootServices      = Disable_RootServices;
            this.LoggingContext            = LoggingContext;

            //this.logfileName               = Path.Combine(this.LoggingPath,
            //                                              this.LogfileName);

            this.DatabaseFilePath          = DatabaseFilePath                   ?? Path.Combine(AppContext.BaseDirectory,
                                                                                                DefaultHTTPAPI_LoggingPath);

            if (this.DatabaseFilePath[^1] != Path.DirectorySeparatorChar)
                this.DatabaseFilePath     += Path.DirectorySeparatorChar;

            this.RemotePartyDBFileName     = Path.Combine(this.DatabaseFilePath,
                                                          RemotePartyDBFileName ?? DefaultRemotePartyDBFileName);

            this.AssetsDBFileName          = Path.Combine(this.DatabaseFilePath,
                                                          AssetsDBFileName      ?? DefaultAssetsDBFileName);

            // Link HTTP events...
            HTTPServer.RequestLog         += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog        += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog           += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            this.ClientConfigurations      = new ClientConfigurator();

        }

        #endregion

        #endregion


        //ToDo: Wrap the following into a plugable interface!

        #region (private) Read/Write Database Files

        #region (private static) WriteToDatabase                          (FileName, Text, ...)

        private static Task WriteToDatabase(String             FileName,
                                            String             Text,
                                            CancellationToken  CancellationToken   = default)

            => File.AppendAllTextAsync(
                   FileName,
                   Text + Environment.NewLine,
                   Encoding.UTF8,
                   CancellationToken
               );

        #endregion

        #region (private static) WriteToDatabase                          (FileName, JToken, ...)

        private static Task WriteToDatabase(String             FileName,
                                            String             Command,
                                            JToken?            JToken,
                                            EventTracking_Id   EventTrackingId,
                                            User_Id?           CurrentUserId       = null,
                                            CancellationToken  CancellationToken   = default)

            => WriteToDatabase(

                   FileName,

                   JSONObject.Create(

                             // Command is always the first property!
                             new JProperty(Command,            JToken),
                             new JProperty("timestamp",        Timestamp.Now.  ToIso8601()),
                             new JProperty("eventTrackingId",  EventTrackingId.ToString()),

                       CurrentUserId is not null
                           ? new JProperty("userId",           CurrentUserId)
                           : null).

                   ToString(Newtonsoft.Json.Formatting.None),

                   CancellationToken

               );

        #endregion

        #region (private static) WriteCommentToDatabase                   (FileName, Text, ...)

        private static Task WriteCommentToDatabase(String             FileName,
                                                   String             Text,
                                                   EventTracking_Id   EventTrackingId,
                                                   User_Id?           CurrentUserId       = null,
                                                   CancellationToken  CancellationToken   = default)

            => File.AppendAllTextAsync(
                   FileName,
                   $"//{Timestamp.Now.ToIso8601()} {EventTrackingId} {(CurrentUserId is not null ? CurrentUserId : "-")}: {Text}{Environment.NewLine}",
                   Encoding.UTF8,
                   CancellationToken
               );

        #endregion


        #region (private static) LoadCommandsFromDatabaseFile             (DBFileName)

        private static IEnumerable<Command> LoadCommandsFromDatabaseFile(String DBFileName)
        {

            try
            {

                return ParseCommands(
                           File.ReadLines(
                               DBFileName,
                               Encoding.UTF8
                           )
                       );

            }
            catch (Exception e)
            {
                DebugX.Log(e, $"OCPI.CommonAPIBase.ReadDatabaseFile({DBFileName})");
            }

            return [];

        }

        #endregion

        #region (private static) ParseCommands                            (Commands)

        private static IEnumerable<Command> ParseCommands(IEnumerable<String> Commands)
        {

            try
            {

                var commands = new List<Command>();

                foreach (var line in Commands)
                {

                    try
                    {

                        if (line.StartsWith("//") || line.StartsWith('#'))
                            continue;

                        var json     = JObject.Parse(line);
                        var command  = json.Properties().First();

                        if (     command.Value.Type == JTokenType.Object)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value as JObject
                                )
                            );

                        else if (command.Value.Type == JTokenType.Array)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value as JArray
                                )
                            );

                        else if (command.Value.Type == JTokenType.String)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value<String>()
                                )
                            );

                        else if (command.Value.Type == JTokenType.Integer)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value<Int64>()
                                )
                            );

                        else if (command.Value.Type == JTokenType.Float)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value<Single>()
                                )
                            );

                        else if (command.Value.Type == JTokenType.Boolean)
                            commands.Add(
                                new Command(
                                    command.Name,
                                    command.Value<Boolean>()
                                )
                            );

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, $"OCPI.CommonAPIBase.ProcessCommands()");
                    }

                }

                return commands;

            }
            catch
            {
                return [];
            }

        }

        #endregion

        #region (private static) LoadCommandsWithMetadataFromDatabaseFile (DBFileName)

        private static IEnumerable<CommandWithMetadata> LoadCommandsWithMetadataFromDatabaseFile(String DBFileName)
        {

            try
            {

                return ParseCommandsWithMetadata(
                           File.ReadLines(
                               DBFileName,
                               Encoding.UTF8
                           )
                       );

            }
            catch (Exception e)
            {
                DebugX.Log(e, $"OCPI.CommonAPIBase.ReadDatabaseFileWithMetadata({DBFileName})");
            }

            return [];

        }

        #endregion

        #region (private static) ParseCommandsWithMetadata                (Commands)

        private static IEnumerable<CommandWithMetadata> ParseCommandsWithMetadata(IEnumerable<String> Commands)
        {

            try
            {

                var commands = new List<CommandWithMetadata>();

                foreach (var command in Commands)
                {

                    try
                    {

                        if (command.StartsWith("//"))
                            continue;

                        var json             = JObject.Parse(command);
                        var ocpiCommand      = json.Properties().First();
                        var timestamp        = json["timestamp"]?.      Value<DateTime>();
                        var eventtrackingid  = json["eventTrackingId"]?.Value<String>();
                        var eventTrackingId  = eventtrackingid is not null ? EventTracking_Id.Parse(eventtrackingid) : null;
                        var userid           = json["userId"]?.         Value<String>();
                        var userId           = userid          is not null ? User_Id.         Parse(userid)          : new User_Id?();

                        if (timestamp.HasValue &&
                            eventTrackingId is not null)
                        {

                            if (ocpiCommand.Value.Type == JTokenType.String)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<String>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Object)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value as JObject,
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Integer)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<Int64>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );


                            if (     ocpiCommand.Value.Type == JTokenType.Object)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value as JObject,
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Array)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value as JArray,
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.String)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<String>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Integer)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<Int64>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Float)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<Single>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );

                            else if (ocpiCommand.Value.Type == JTokenType.Boolean)
                                commands.Add(
                                    new CommandWithMetadata(
                                        ocpiCommand.Name,
                                        ocpiCommand.Value<Boolean>(),
                                        timestamp.Value,
                                        eventTrackingId,
                                        userId
                                    )
                                );


                            else
                                DebugX.Log($"OCPI.CommonAPIBase.ProcessCommandsWithMetadata(): Invalid command: '{command}'!");

                        }
                        else
                            DebugX.Log($"OCPI.CommonAPIBase.ProcessCommandsWithMetadata(): Invalid command: '{command}'!");

                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e, $"OCPI.CommonAPIBase.ProcessCommandsWithMetadata()");
                    }

                }

                return commands;

            }
            catch
            {
                return [];
            }

        }

        #endregion

        #endregion

        #region Log/Read   Remote Parties

        #region (protected) LogRemoteParty        (Command,              ...)

        protected Task LogRemoteParty(String            Command,
                                      EventTracking_Id  EventTrackingId,
                                      User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   null,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region (protected) LogRemoteParty        (Command, Text = null, ...)

        protected Task LogRemoteParty(String            Command,
                                      String?           Text,
                                      EventTracking_Id  EventTrackingId,
                                      User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   Text is not null
                       ? JToken.Parse(Text)
                       : null,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region (protected) LogRemoteParty        (Command, JSON,        ...)

        protected Task LogRemoteParty(String            Command,
                                      JObject           JSON,
                                      EventTracking_Id  EventTrackingId,
                                      User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   JSON,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region (protected) LogRemoteParty        (Command, Number,      ...)

        protected Task Log(String            Command,
                           Int64             Number,
                           EventTracking_Id  EventTrackingId,
                           User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   RemotePartyDBFileName,
                   Command,
                   Number,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region (protected) LogRemotePartyComment (Text,                 ...)

        protected Task LogRemotePartyComment(String           Text,
                                            EventTracking_Id  EventTrackingId,
                                            User_Id?          CurrentUserId   = null)

            => WriteCommentToDatabase(
                   RemotePartyDBFileName,
                   Text,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion


        #region ReadRemotePartyDatabaseFile       (DatabaseFileName = null)

        public IEnumerable<Command> ReadRemotePartyDatabaseFile(String? DatabaseFileName = null)

            => LoadCommandsFromDatabaseFile(DatabaseFileName ?? RemotePartyDBFileName);

        #endregion

        #endregion

        #region Log/Read   Assets

        #region (protected) LogAsset              (Command,              ...)

        protected Task LogAsset(String            Command,
                                EventTracking_Id  EventTrackingId,
                                User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   null,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region (protected) LogAsset              (Command, Text = null, ...)

        protected Task LogAsset(String             Command,
                                String?            Text,
                                EventTracking_Id?  EventTrackingId   = null,
                                User_Id?           CurrentUserId     = null)

            => WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   Text is not null
                       ? JToken.Parse(Text)
                       : null,
                   EventTrackingId ?? EventTracking_Id.New,
                   CurrentUserId
               );

        #endregion

        #region (protected) LogAsset              (Command, JSONObject,  ...)

        protected Task LogAsset(String             Command,
                                JObject            JSONObject,
                                EventTracking_Id   EventTrackingId,
                                User_Id?           CurrentUserId       = null,
                                CancellationToken  CancellationToken   = default)

            => WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   JSONObject,
                   EventTrackingId,
                   CurrentUserId,
                   CancellationToken
               );

        #endregion

        #region (protected) LogAsset              (Command, JSONArray,   ...)

        protected Task LogAsset(String            Command,
                                JArray            JSONArray,
                                EventTracking_Id  EventTrackingId,
                                User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   JSONArray,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region (protected) LogAsset              (Command, Number,      ...)

        protected Task LogAsset(String            Command,
                                Int64             Number,
                                EventTracking_Id  EventTrackingId,
                                User_Id?          CurrentUserId   = null)

            => WriteToDatabase(
                   AssetsDBFileName,
                   Command,
                   Number,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion

        #region (protected) LogAssetComment       (Text,                 ...)

        protected Task LogAssetComment(String            Text,
                                       EventTracking_Id  EventTrackingId,
                                       User_Id?          CurrentUserId   = null)

            => WriteCommentToDatabase(
                   AssetsDBFileName,
                   Text,
                   EventTrackingId,
                   CurrentUserId
               );

        #endregion


        #region ReadAssetsDatabaseFile            (DatabaseFileName = null)

        public IEnumerable<Command> ReadAssetsDatabaseFile(String? DatabaseFileName = null)

            => LoadCommandsFromDatabaseFile(DatabaseFileName ?? AssetsDBFileName);

        #endregion

        #endregion


        #region Start(...)

        /// <summary>
        /// Start this CommonAPI.
        /// </summary>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Boolean> Start(EventTracking_Id?  EventTrackingId   = null,
                                         User_Id?           CurrentUserId     = null)
        {

            if (HTTPServer.IsStarted)
                return true;

            EventTrackingId ??= EventTracking_Id.New;

            var result = await base.Start();

            await LogAsset(
                      "started",
                      EventTrackingId,
                      CurrentUserId
                  );

            #region Send 'Open Data API restarted'-e-mail...

            //var Message0 = new HTMLEMailBuilder() {
            //    From        = _APIEMailAddress,
            //    To          = _APIAdminEMail,
            //    Subject     = "Open Data API '" + _ServiceName + "' restarted! at " + Timestamp.Now.ToString(),
            //    PlainText   = "Open Data API '" + _ServiceName + "' restarted! at " + Timestamp.Now.ToString(),
            //    HTMLText    = "Open Data API <b>'" + _ServiceName + "'</b> restarted! at " + Timestamp.Now.ToString(),
            //    Passphrase  = _APIPassphrase
            //};
            //
            //var SMTPTask = _APISMTPClient.Send(Message0);
            //SMTPTask.Wait();

            //var r = SMTPTask.Result;

            #endregion

            //SendStarted(this, Timestamp.Now);

            return result;

        }

        #endregion

        #region Start    (Delay, EventTrackingId = null, InBackground = true)

        public override Task<Boolean> Start(TimeSpan           Delay,
                                            EventTracking_Id?  EventTrackingId   = null,
                                            Boolean            InBackground      = true)

            => Start(Delay,
                     EventTrackingId,
                     InBackground,
                     null);


        /// <summary>
        /// Start the server after a little delay.
        /// </summary>
        /// <param name="Delay">The delay.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="InBackground">Whether to wait on the main thread or in a background thread.</param>
        public async virtual Task<Boolean> Start(TimeSpan           Delay,
                                                 EventTracking_Id?  EventTrackingId   = null,
                                                 Boolean            InBackground      = true,
                                                 User_Id?           CurrentUserId     = null)
        {

            EventTrackingId ??= EventTracking_Id.New;

            var result = await HTTPServer.Start(
                                   Delay,
                                   EventTrackingId,
                                   InBackground
                               );

            //SendStarted(this, CurrentTimestamp);

            await LogAsset(
                      "started",
                      EventTrackingId,
                      CurrentUserId
                  );

            return result;

        }

        #endregion

        #region Shutdown(EventTrackingId = null, Message = null, Wait = true, ...)

        public override Task<Boolean> Shutdown(EventTracking_Id?  EventTrackingId   = null,
                                               String?            Message           = null,
                                               Boolean            Wait              = true)

            => Shutdown(
                   EventTrackingId,
                   Message,
                   Wait,
                   null
               );


        /// <summary>
        /// Shutdown this CommonAPI.
        /// </summary>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Whether to wait for the shutdown to complete.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Boolean> Shutdown(EventTracking_Id?  EventTrackingId   = null,
                                            String?            Message           = null,
                                            Boolean            Wait              = true,
                                            User_Id?           CurrentUserId     = null)
        {

            if (!HTTPServer.IsStarted)
                return true;

            EventTrackingId ??= EventTracking_Id.New;

            var result = await base.Shutdown(
                                   EventTrackingId,
                                   Message,
                                   Wait
                               );

            await LogAsset(
                      "shutdown",
                      Message,
                      EventTrackingId,
                      CurrentUserId
                  );

            //SendShutdown(this, Timestamp.Now);

            return result;

        }

        #endregion


    }

}
