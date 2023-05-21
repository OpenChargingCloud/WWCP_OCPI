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

using System.Text;
using System.Net.Security;
using System.Security.Authentication;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// The Common API Base.
    /// </summary>
    public class CommonAPIBase : HTTPAPI
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
            public Func<RemoteParty_Id, String>?                  Description       { get; set; }

            /// <summary>
            /// Whether logging is disabled for this OCPI client.
            /// </summary>
            public Func<RemoteParty_Id, Boolean>?                 DisableLogging    { get; set; }

            /// <summary>
            /// The logging path for this OCPI client.
            /// </summary>
            public Func<RemoteParty_Id, String>?                  LoggingPath       { get; set; }

            /// <summary>
            /// The logging context for this OCPI client.
            /// </summary>
            public Func<RemoteParty_Id, String>?                  LoggingContext    { get; set; }

            /// <summary>
            /// The logfile creator for this OCPI client.
            /// </summary>
            public Func<RemoteParty_Id, LogfileCreatorDelegate>?  LogfileCreator    { get; set; }

        }

        #endregion

        #region (class) Command

        public class Command
        {

            #region Properties

            public String    CommandName    { get; }

            public String?   Message        { get; }

            public JObject?  JSON           { get; }

            public Int64?    Number         { get; }

            #endregion

            #region Constructor(s)

            #region Command(CommandName, Message)


            public Command(String    CommandName,
                           String?   Message)
            {

                this.CommandName  = CommandName;
                this.Message      = Message;

            }

            #endregion

            #region Command(CommandName, JSON)

            public Command(String    CommandName,
                           JObject?  JSON)
            {

                this.CommandName  = CommandName;
                this.JSON         = JSON;

            }

            #endregion

            #region Command(CommandName, Number)

            public Command(String    CommandName,
                           Int64?    Number)
            {

                this.CommandName  = CommandName;
                this.Number       = Number;

            }

            #endregion

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
        /// The absolute path to the CommonAPI log file.
        /// </summary>
        private readonly           String      logfileName;

        /// <summary>
        /// The default database file name for all remote party configuration.
        /// </summary>
        public const               String      DefaultRemotePartyDBFileName    = "RemoteParties.db";

        /// <summary>
        /// The default database file name for all OCPI assets.
        /// </summary>
        public const               String      DefaultAssetsDBFileName         = "Assets.db";


        protected const String addRemoteParty             = "addRemoteParty";
        protected const String addRemotePartyIfNotExists  = "addRemotePartyIfNotExists";
        protected const String addOrUpdateRemoteParty     = "addOrUpdateRemoteParty";
        protected const String updateRemoteParty          = "updateRemoteParty";
        protected const String removeRemoteParty          = "removeRemoteParty";
        protected const String removeAllRemoteParties     = "removeAllRemoteParties";

        protected const String addTariff                  = "addTariff";
        protected const String addTariffIfNotExists       = "addTariffIfNotExists";
        protected const String addOrUpdateTariff          = "addOrUpdateTariff";
        protected const String updateTariff               = "updateTariff";
        protected const String removeTariff               = "removeTariff";
        protected const String removeAllTariffs           = "removeAllTariffs";

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
        /// <param name="ServerCertificateSelector">An optional delegate to select a SSL/TLS server certificate.</param>
        /// <param name="ClientCertificateValidator">An optional delegate to verify the SSL/TLS client certificate used for authentication.</param>
        /// <param name="ClientCertificateSelector">An optional delegate to select the SSL/TLS client certificate used for authentication.</param>
        /// <param name="AllowedTLSProtocols">The SSL/TLS protocol(s) allowed for this connection.</param>
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
        /// <param name="Autostart">Whether to start the API automatically.</param>
        public CommonAPIBase(Version_Id                            OCPIVersion,
                             URL                                   OurBaseURL,
                             URL                                   OurVersionsURL,

                             HTTPPath?                             AdditionalURLPathPrefix       = null,
                             Boolean                               LocationsAsOpenData           = true,
                             Boolean?                              AllowDowngrades               = null,
                             Boolean                               Disable_RootServices          = true,

                             HTTPHostname?                         HTTPHostname                  = null,
                             String?                               ExternalDNSName               = null,
                             IPPort?                               HTTPServerPort                = null,
                             HTTPPath?                             BasePath                      = null,
                             String?                               HTTPServerName                = DefaultHTTPServerName,

                             HTTPPath?                             URLPathPrefix                 = null,
                             String?                               HTTPServiceName               = DefaultHTTPServiceName,
                             JObject?                              APIVersionHashes              = null,

                             ServerCertificateSelectorDelegate?    ServerCertificateSelector     = null,
                             RemoteCertificateValidationCallback?  ClientCertificateValidator    = null,
                             LocalCertificateSelectionCallback?    ClientCertificateSelector     = null,
                             SslProtocols?                         AllowedTLSProtocols           = null,
                             Boolean?                              ClientCertificateRequired     = null,
                             Boolean?                              CheckCertificateRevocation    = null,

                             String?                               ServerThreadName              = null,
                             ThreadPriority?                       ServerThreadPriority          = null,
                             Boolean?                              ServerThreadIsBackground      = null,

                             ConnectionIdBuilder?                  ConnectionIdBuilder           = null,
                             TimeSpan?                             ConnectionTimeout             = null,
                             UInt32?                               MaxClientConnections          = null,

                             Boolean?                              DisableMaintenanceTasks       = null,
                             TimeSpan?                             MaintenanceInitialDelay       = null,
                             TimeSpan?                             MaintenanceEvery              = null,

                             Boolean?                              DisableWardenTasks            = null,
                             TimeSpan?                             WardenInitialDelay            = null,
                             TimeSpan?                             WardenCheckEvery              = null,

                             Boolean?                              IsDevelopment                 = null,
                             IEnumerable<String>?                  DevelopmentServers            = null,
                             Boolean?                              DisableLogging                = null,
                             String?                               LoggingContext                = null,
                             String?                               LoggingPath                   = null,
                             String?                               LogfileName                   = null,
                             LogfileCreatorDelegate?               LogfileCreator                = null,
                             String?                               RemotePartyDBFileName         = null,
                             String?                               AssetsDBFileName              = null,
                             DNSClient?                            DNSClient                     = null,
                             Boolean                               Autostart                     = false)

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
                   ClientCertificateSelector,
                   AllowedTLSProtocols,
                   ClientCertificateRequired,
                   CheckCertificateRevocation,

                   ServerThreadName,
                   ServerThreadPriority,
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
                   LogfileCreator,
                   DNSClient,
                   Autostart)

        {

            this.OCPIVersion              = OCPIVersion;
            this.OurBaseURL               = OurBaseURL;
            this.OurVersionsURL           = OurVersionsURL;
            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;
            this.Disable_RootServices     = Disable_RootServices;
            this.LoggingContext           = LoggingContext;

            this.logfileName              = Path.Combine(this.LoggingPath,
                                                         this.LogfileName);

            this.RemotePartyDBFileName    = RemotePartyDBFileName ?? Path.Combine(this.LoggingPath,
                                                                                  DefaultRemotePartyDBFileName);

            this.AssetsDBFileName         = AssetsDBFileName      ?? Path.Combine(this.LoggingPath,
                                                                                  DefaultAssetsDBFileName);

            this.ClientConfigurations     = new ClientConfigurator();

            if (!Disable_RootServices)
                RegisterURLTemplates();

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
        /// <param name="Autostart">Whether to start the API automatically.</param>
        public CommonAPIBase(Version_Id               OCPIVersion,
                             URL                      OurBaseURL,
                             URL                      OurVersionsURL,
                             HTTPServer               HTTPServer,

                             HTTPPath?                AdditionalURLPathPrefix    = null,
                             Boolean                  LocationsAsOpenData        = true,
                             Boolean?                 AllowDowngrades            = null,
                             Boolean                  Disable_RootServices       = false,

                             HTTPHostname?            HTTPHostname               = null,
                             String?                  ExternalDNSName            = "",
                             String?                  HTTPServiceName            = DefaultHTTPServiceName,
                             HTTPPath?                BasePath                   = null,

                             HTTPPath?                URLPathPrefix              = null,
                             JObject?                 APIVersionHashes           = null,

                             Boolean?                 DisableMaintenanceTasks    = false,
                             TimeSpan?                MaintenanceInitialDelay    = null,
                             TimeSpan?                MaintenanceEvery           = null,

                             Boolean?                 DisableWardenTasks         = false,
                             TimeSpan?                WardenInitialDelay         = null,
                             TimeSpan?                WardenCheckEvery           = null,

                             Boolean?                 IsDevelopment              = false,
                             IEnumerable<String>?     DevelopmentServers         = null,
                             Boolean?                 DisableLogging             = false,
                             String?                  LoggingContext             = null,
                             String?                  LoggingPath                = null,
                             String?                  LogfileName                = null,
                             LogfileCreatorDelegate?  LogfileCreator             = null,
                             String?                  RemotePartyDBFileName      = null,
                             String?                  AssetsDBFileName           = null,
                             Boolean                  Autostart                  = false)

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
                   LogfileCreator,
                   Autostart)

        {

            this.OCPIVersion              = OCPIVersion;
            this.OurBaseURL               = OurBaseURL;
            this.OurVersionsURL           = OurVersionsURL;
            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;
            this.Disable_RootServices     = Disable_RootServices;
            this.LoggingContext           = LoggingContext;

            this.logfileName              = Path.Combine(this.LoggingPath,
                                                         this.LogfileName);

            this.RemotePartyDBFileName    = RemotePartyDBFileName ?? Path.Combine(this.LoggingPath,
                                                                                  DefaultRemotePartyDBFileName);

            this.AssetsDBFileName         = AssetsDBFileName      ?? Path.Combine(this.LoggingPath,
                                                                                  DefaultAssetsDBFileName);

            // Link HTTP events...
            HTTPServer.RequestLog        += (HTTPProcessor, ServerTimestamp, Request)                                 => RequestLog. WhenAll(HTTPProcessor, ServerTimestamp, Request);
            HTTPServer.ResponseLog       += (HTTPProcessor, ServerTimestamp, Request, Response)                       => ResponseLog.WhenAll(HTTPProcessor, ServerTimestamp, Request, Response);
            HTTPServer.ErrorLog          += (HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException) => ErrorLog.   WhenAll(HTTPProcessor, ServerTimestamp, Request, Response, Error, LastException);

            this.ClientConfigurations     = new ClientConfigurator();

            if (!Disable_RootServices)
                RegisterURLTemplates();

        }

        #endregion

        #endregion


        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {


        }

        #endregion


        #region (private, static) WriteToDatabase       (FileName, Text)

        private static Task WriteToDatabase(String  FileName,
                                            String  Text)

            => File.AppendAllTextAsync(FileName,
                                       Text + Environment.NewLine,
                                       Encoding.UTF8);

        #endregion

        #region (private, static) WriteToDatabase       (FileName, JToken, UserId = null)

        private static Task WriteToDatabase(String   FileName,
                                            String   Command,
                                            JToken?  JToken,
                                            String?  UserId   = null)

            => WriteToDatabase(FileName,
                               JSONObject.Create(

                                         new JProperty("timestamp",  Timestamp.Now),
                                         new JProperty(Command,      JToken),

                                   UserId is not null
                                       ? new JProperty("userId",     UserId)
                                       : null).
                               ToString(Newtonsoft.Json.Formatting.None));

        #endregion

        #region (private, static) WriteCommentToDatabase(FileName, Text, UserId = null)

        private static Task WriteCommentToDatabase(String   FileName,
                                                   String   Text,
                                                   String?  UserId   = null)

            => File.AppendAllTextAsync(FileName,
                                       $"//{Timestamp.Now.ToIso8601()} {(UserId is not null ? UserId : "-")}: {Text}{Environment.NewLine}",
                                       Encoding.UTF8);

        #endregion


        #region LogRemoteParty       (Command, Text = null, UserId = null)

        protected Task LogRemoteParty(String   Command,
                                      String?  Text     = null,
                                      String?  UserId   = null)

            => WriteToDatabase(RemotePartyDBFileName,
                               Command,
                               Text is not null
                                   ? JToken.Parse(Text)
                                   : null,
                               UserId);

        #endregion

        #region LogRemoteParty       (Command, JSON,        UserId = null)

        protected Task LogRemoteParty(String   Command,
                                      JObject  JSON,
                                      String?  UserId   = null)

            => WriteToDatabase(RemotePartyDBFileName,
                               Command,
                               JSON,
                               UserId);

        #endregion

        #region LogRemoteParty       (Command, Number,      UserId = null)

        protected Task Log(String   Command,
                           Int64    Number,
                           String?  UserId   = null)

            => WriteToDatabase(RemotePartyDBFileName,
                               Command,
                               Number,
                               UserId);

        #endregion

        #region LogRemotePartyComment(Text,                 UserId = null)

        protected Task LogRemotePartyComment(String   Text,
                                             String?  UserId = null)

            => WriteCommentToDatabase(RemotePartyDBFileName,
                                      Text,
                                      UserId);

        #endregion

        #region ReadRemotePartyDatabaseFile()

        protected IEnumerable<Command> ReadRemotePartyDatabaseFile()
        {

            try
            {

                var list = new List<Command>();

                foreach (var line in File.ReadLines(RemotePartyDBFileName,
                                                    Encoding.UTF8))
                {

                    try
                    {

                        var json = JObject.Parse(line);

                        if (json.Properties().First().Value.Type == JTokenType.String)
                            list.Add(new Command(json.Properties().First().Name,
                                                 json.Properties().First().Value<String>()));

                        else if (json.Properties().First().Value.Type == JTokenType.Object)
                            list.Add(new Command(json.Properties().First().Name,
                                                 json.Properties().First().Value as JObject));

                        else if (json.Properties().First().Value.Type == JTokenType.Integer)
                            list.Add(new Command(json.Properties().First().Name,
                                                 json.Properties().First().Value<Int64>()));

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, "OCPI.CommonAPIBase.ReadRemotePartyDatabaseFile()");
                    }

                }

                return list;

            }
            catch
            {
                return Array.Empty<Command>();
            }

        }

        #endregion


        #region LogAsset       (Command, Text = null, UserId = null)

        protected Task LogAsset(String   Command,
                                String?  Text     = null,
                                String?  UserId   = null)

            => WriteToDatabase(AssetsDBFileName,
                               Command,
                               Text is not null
                                   ? JToken.Parse(Text)
                                   : null,
                               UserId);

        #endregion

        #region LogAsset       (Command, JSON,        UserId = null)

        protected Task LogAsset(String   Command,
                                JObject  JSON,
                                String?  UserId   = null)

            => WriteToDatabase(AssetsDBFileName,
                               Command,
                               JSON,
                               UserId);

        #endregion

        #region LogAsset       (Command, Number,      UserId = null)

        protected Task LogAsset(String   Command,
                                Int64    Number,
                                String?  UserId   = null)

            => WriteToDatabase(AssetsDBFileName,
                               Command,
                               Number,
                               UserId);

        #endregion

        #region LogAssetComment(Text,                 UserId = null)

        protected Task LogAssetComment(String   Text,
                                       String?  UserId   = null)

            => WriteCommentToDatabase(AssetsDBFileName,
                                      Text,
                                      UserId);

        #endregion

        #region ReadAssetsDatabaseFile()

        protected IEnumerable<Command> ReadAssetsDatabaseFile()
        {

            try
            {

                var list = new List<Command>();

                foreach (var line in File.ReadLines(AssetsDBFileName,
                                                    Encoding.UTF8))
                {

                    try
                    {

                        var json = JObject.Parse(line);

                        if (json.Properties().First().Value.Type == JTokenType.String)
                            list.Add(new Command(json.Properties().First().Name,
                                                 json.Properties().First().Value<String>()));

                        else if (json.Properties().First().Value.Type == JTokenType.Object)
                            list.Add(new Command(json.Properties().First().Name,
                                                 json.Properties().First().Value as JObject));

                        else if (json.Properties().First().Value.Type == JTokenType.Integer)
                            list.Add(new Command(json.Properties().First().Name,
                                                 json.Properties().First().Value<Int64>()));

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(e, "OCPI.CommonAPIBase.ReadAssetsDatabaseFile()");
                    }

                }

                return list;

            }
            catch
            {
                return Array.Empty<Command>();
            }

        }

        #endregion


        #region Start()

        /// <summary>
        /// Start this CommonAPI.
        /// </summary>
        public override Boolean Start()
        {

            if (HTTPServer.IsStarted)
                return true;

            var result = base.Start();

            LogAsset("started").GetAwaiter().GetResult();

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

        #region Shutdown(Message = null, Wait = true)

        /// <summary>
        /// Shutdown this CommonAPI.
        /// </summary>
        /// <param name="Message">An optional shutdown message.</param>
        /// <param name="Wait">Whether to wait for the shutdown to complete.</param>
        public override Boolean Shutdown(String?  Message   = null,
                                         Boolean  Wait      = true)
        {

            if (!HTTPServer.IsStarted)
                return true;

            var result = base.Shutdown(Message,
                                       Wait);

            LogAsset("shutdown", Message).GetAwaiter().GetResult();

            //SendShutdown(this, Timestamp.Now);

            return result;

        }

        #endregion


    }

}
