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

using System.Text;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

using BCx509 = Org.BouncyCastle.X509;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Logging;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.HTTPTest;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    public delegate String    OCPILogfileCreatorDelegate               (String         LoggingPath,
                                                                        IRemoteParty?  RemoteParty,
                                                                        String         Context,
                                                                        String         LogfileName);

    public delegate TimeSpan  DefaultMaxCSRCertificateLifeTimeDelegate (CSRInfo        CSR);
    public delegate TimeSpan  DefaultMaxCertificateLifeTimeDelegate    (CSRInfo        CSR);


    /// <summary>
    /// A structured object capturing all the data we care about from the CSR.
    /// </summary>
    public sealed class CSRInfo(AsymmetricKeyParameter         PublicKey,
                                X509Name                       Subject,
                                Dictionary<String, String>?    ParsedSubject      = null,
                                Asn1Set?                       Attributes         = null,
                                IEnumerable<ParsedAttribute>?  ParsedAttributes   = null,
                                String?                        KeyId              = null,
                                DateTimeOffset?                NotBefore          = null,
                                DateTimeOffset?                NotAfter           = null,
                                IEnumerable<String>?           PartyIds           = null,
                                IEnumerable<String>?           SubCPOIds          = null,
                                IEnumerable<String>?           SubEMSPIds         = null)
    {

        /// <summary>
        /// The subject public key of the CSR.
        /// </summary>
        public AsymmetricKeyParameter        PublicKey           { get; set; } = PublicKey;


        /// <summary>
        /// The Subject of the certificate signing request.
        /// </summary>
        public X509Name                      Subject             { get; set; } = Subject;

        /// <summary>
        /// Subject fields (e.g., CN, O, OU, etc.) as OID -> string.
        /// </summary>
        public Dictionary<String, String>    ParsedSubject       { get; set; } = ParsedSubject    ?? [];


        /// <summary>
        /// The attributes as ASN.1 set.
        /// </summary>
        public Asn1Set?                      Attributes          { get; set; } = Attributes;

        /// <summary>
        /// List of all attributes found in the CSR, with OIDs and decoded data.
        /// </summary>
        public IEnumerable<ParsedAttribute>  ParsedAttributes    { get; set; } = ParsedAttributes ?? [];


        public String?                       KeyId               { get; set; } = KeyId;
        public DateTimeOffset?               NotBefore           { get; set; } = NotBefore;
        public DateTimeOffset?               NotAfter            { get; set; } = NotAfter;

        public IEnumerable<String>           PartyIds            { get; set; } = PartyIds         ?? [];
        public IEnumerable<String>           SubCPOIds           { get; set; } = SubCPOIds        ?? [];
        public IEnumerable<String>           SubEMSPIds          { get; set; } = SubEMSPIds       ?? [];


    }

    /// <summary>
    /// Captures a single Attribute from the CSR (OID plus the list of parsed values).
    /// </summary>
    public sealed class ParsedAttribute(String               OID,
                                        IEnumerable<String>  DecodedData)
    {
        public String               OID            { get; set; } = OID;
        public IEnumerable<String>  DecodedData    { get; set; } = DecodedData;

    }

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



    /// <summary>
    /// The OCPI Common HTTP API.
    /// </summary>
    public class CommonHTTPAPI : AHTTPExtAPIXExtension<HTTPExtAPIX>
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String         DefaultHTTPServerName           = "GraphDefined OCPI Common HTTP API";

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String         DefaultHTTPServiceName          = "GraphDefined OCPI Common HTTP API";

        ///// <summary>
        ///// The default HTTP server TCP port.  
        ///// </summary>
        //public new static readonly IPPort         DefaultHTTPServerPort           = IPPort.Parse(8080);

        ///// <summary>
        ///// The default HTTP URL path prefix.  
        ///// </summary>
        //public new static readonly HTTPPath       DefaultURLPathPrefix            = HTTPPath.Parse("io/OCPI/");

        /// <summary>
        /// The default database file name for all remote party configuration.
        /// </summary>
        public const               String         DefaultRemotePartyDBFileName    = "RemoteParties.db";

        /// <summary>
        /// The default database file name for all OCPI assets.
        /// </summary>
        public const               String         DefaultAssetsDBFileName         = "Assets.db";

        private readonly           LogFileWriter  logFileWriter                   = new (10000);

        #region Commands

        public const String addRemoteParty                     = "addRemoteParty";
        public const String addRemotePartyIfNotExists          = "addRemotePartyIfNotExists";
        public const String addOrUpdateRemoteParty             = "addOrUpdateRemoteParty";
        public const String updateRemoteParty                  = "updateRemoteParty";
        public const String removeRemoteParty                  = "removeRemoteParty";
        public const String removeAllRemoteParties             = "removeAllRemoteParties";

        public const String addLocation                        = "addLocation";
        public const String addLocationIfNotExists             = "addLocationIfNotExists";
        public const String addOrUpdateLocation                = "addOrUpdateLocation";
        public const String updateLocation                     = "updateLocation";
        public const String removeLocation                     = "removeLocation";
        public const String removeAllLocations                 = "removeAllLocations";

        public const String addEVSE                            = "addEVSE";
        public const String addEVSEIfNotExists                 = "addEVSEIfNotExists";
        public const String addOrUpdateEVSE                    = "addOrUpdateEVSE";
        public const String updateEVSE                         = "updateEVSE";
        public const String removeEVSE                         = "removeEVSE";
        public const String removeAllEVSEs                     = "removeAllEVSEs";

        public const String addTariff                          = "addTariff";
        public const String addTariffIfNotExists               = "addTariffIfNotExists";
        public const String addOrUpdateTariff                  = "addOrUpdateTariff";
        public const String updateTariff                       = "updateTariff";
        public const String removeTariff                       = "removeTariff";
        public const String removeAllTariffs                   = "removeAllTariffs";

        public const String addSession                         = "addSession";
        public const String addSessionIfNotExists              = "addSessionIfNotExists";
        public const String addOrUpdateSession                 = "addOrUpdateSession";
        public const String updateSession                      = "updateSession";
        public const String removeSession                      = "removeSession";
        public const String removeAllSessions                  = "removeAllSessions";

        public const String addToken                           = "addToken";
        public const String addTokenIfNotExists                = "addTokenIfNotExists";
        public const String addOrUpdateToken                   = "addOrUpdateToken";
        public const String updateToken                        = "updateToken";
        public const String removeToken                        = "removeToken";
        public const String removeAllTokens                    = "removeAllTokens";

        public const String addTokenStatus                     = "addTokenStatus";
        public const String addTokenStatusIfNotExists          = "addTokenStatusIfNotExists";
        public const String addOrUpdateTokenStatus             = "addOrUpdateTokenStatus";
        public const String updateTokenStatus                  = "updateTokenStatus";
        public const String removeTokenStatus                  = "removeTokenStatus";
        public const String removeAllTokenStatus               = "removeAllTokenStatus";

        public const String addChargeDetailRecord              = "addChargeDetailRecord";
        public const String addChargeDetailRecordIfNotExists   = "addChargeDetailRecordIfNotExists";
        public const String addOrUpdateChargeDetailRecord      = "addOrUpdateChargeDetailRecord";
        public const String updateChargeDetailRecord           = "updateChargeDetailRecord";
        public const String removeChargeDetailRecord           = "removeChargeDetailRecord";
        public const String removeAllChargeDetailRecords       = "removeAllChargeDetailRecords";

        public const String addTerminal                        = "addTerminal";
        public const String addTerminalIfNotExists             = "addTerminalIfNotExists";
        public const String addOrUpdateTerminal                = "addOrUpdateTerminal";
        public const String updateTerminal                     = "updateTerminal";
        public const String removeTerminal                     = "removeTerminal";
        public const String removeAllTerminals                 = "removeAllTerminals";

        public const String addBooking                         = "addBooking";
        public const String addBookingIfNotExists              = "addBookingIfNotExists";
        public const String addOrUpdateBooking                 = "addOrUpdateBooking";
        public const String updateBooking                      = "updateBooking";
        public const String removeBooking                      = "removeBooking";
        public const String removeAllBookings                  = "removeAllBookings";

        public const String addBookingLocation                 = "addBookingLocation";
        public const String addBookingLocationIfNotExists      = "addBookingLocationIfNotExists";
        public const String addOrUpdateBookingLocation         = "addOrUpdateBookingLocation";
        public const String updateBookingLocation              = "updateBookingLocation";
        public const String removeBookingLocation              = "removeBookingLocation";
        public const String removeAllBookingLocations          = "removeAllBookingLocations";

        #endregion

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
        /// Allow anonymous access to tariffs as Open Data.
        /// </summary>
        public Boolean                  TariffsAsOpenData          { get; }

        /// <summary>
        /// (Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.
        /// OCPI v2.2 does not define any behaviour for this.
        /// </summary>
        public Boolean?                 AllowDowngrades            { get; }

        ///// <summary>
        ///// The logging context.
        ///// </summary>
        //public String?                  LoggingContext             { get; }

        /// <summary>
        /// A template for OCPI client configurations.
        /// </summary>
        public ClientConfigurator       ClientConfigurations       { get; }

        /// <summary>
        /// The OCPI Common HTTP API logger.
        /// </summary>
        public CommonHTTPAPILogger?     Logger                     { get; set; }

        #endregion

        #region Events

        #region (protected internal) GetRootRequest      (Request)

        /// <summary>
        /// An event sent whenever a GET / request was received.
        /// </summary>
        public HTTPRequestLogEventX OnGetRootRequest = new();

        /// <summary>
        /// An event sent whenever a GET / request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task GetRootRequest(DateTimeOffset     Timestamp,
                                               HTTPAPIX           API,
                                               HTTPRequest        Request,
                                               CancellationToken  CancellationToken)

            => OnGetRootRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetRootResponse     (Response)

        /// <summary>
        /// An event sent whenever a GET / response was sent.
        /// </summary>
        public HTTPResponseLogEventX OnGetRootResponse = new();

        /// <summary>
        /// An event sent whenever a GET / response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task GetRootResponse(DateTimeOffset     Timestamp,
                                                HTTPAPIX           API,
                                                HTTPRequest        Request,
                                                HTTPResponse       Response,
                                                CancellationToken  CancellationToken)

            => OnGetRootResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion


        #region (protected internal) GetVersionsRequest  (Request)

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        public HTTPRequestLogEventX OnGetVersionsRequest = new();

        /// <summary>
        /// An event sent whenever a GET versions request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task GetVersionsRequest(DateTimeOffset     Timestamp,
                                                   HTTPAPIX           API,
                                                   HTTPRequest        Request,
                                                   CancellationToken  CancellationToken)

            => OnGetVersionsRequest.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   CancellationToken
               );

        #endregion

        #region (protected internal) GetVersionsResponse (Response)

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        public HTTPResponseLogEventX OnGetVersionsResponse = new();

        /// <summary>
        /// An event sent whenever a GET versions response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The Common API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task GetVersionsResponse(DateTimeOffset     Timestamp,
                                                    HTTPAPIX           API,
                                                    HTTPRequest        Request,
                                                    HTTPResponse       Response,
                                                    CancellationToken  CancellationToken)

            => OnGetVersionsResponse.WhenAll(
                   Timestamp,
                   API,
                   Request,
                   Response,
                   CancellationToken
               );

        #endregion

        #endregion

        #region Custom JSON serializers

        public CustomJObjectSerializerDelegate<VersionInformation>?           CustomVersionInformationSerializer            { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CommonBaseAPI.
        /// </summary>
        /// <param name="OurBaseURL">The URL for your API endpoint.</param>
        /// <param name="OurVersionsURL">The URL of our VERSIONS endpoint.</param>
        /// 
        /// <param name="AdditionalURLPathPrefix"></param>
        /// <param name="LocationsAsOpenData">Allow anonymous access to locations as Open Data.</param>
        /// <param name="TariffsAsOpenData">Allow anonymous access to tariffs as Open Data.</param>
        /// <param name="AllowDowngrades">(Dis-)allow PUTting of object having an earlier 'LastUpdated'-timestamp then already existing objects.</param>
        /// 
        /// <param name="ExternalDNSName">The official URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServerName">The default HTTP server name, used whenever no HTTP Host-header has been given.</param>
        /// 
        /// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        /// <param name="HTTPServiceName">The name of the HTTP service.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="DisableLogging">Disable any logging.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        public CommonHTTPAPI(HTTPExtAPIX                    HTTPAPI,
                             URL                            OurBaseURL,
                             URL                            OurVersionsURL,

                             IEnumerable<HTTPHostname>?     Hostnames                 = null,
                             HTTPPath?                      RootPath                  = null,
                             IEnumerable<HTTPContentType>?  HTTPContentTypes          = null,
                             I18NString?                    Description               = null,

                             HTTPPath?                      BasePath                  = null,  // For URL prefixes in HTML!

                             String?                        ExternalDNSName           = null,
                             String?                        HTTPServerName            = DefaultHTTPServerName,
                             String?                        HTTPServiceName           = DefaultHTTPServiceName,
                             String?                        APIVersionHash            = null,
                             JObject?                       APIVersionHashes          = null,

                             EMailAddress?                  APIRobotEMailAddress      = null,
                             String?                        APIRobotGPGPassphrase     = null,
                             ISMTPClient?                   SMTPClient                = null,

                             HTTPPath?                      AdditionalURLPathPrefix   = null,
                             Boolean                        LocationsAsOpenData       = true,
                             Boolean                        TariffsAsOpenData         = false,
                             Boolean?                       AllowDowngrades           = null,

                             Boolean?                       IsDevelopment             = null,
                             IEnumerable<String>?           DevelopmentServers        = null,
                             //Boolean?                       SkipURLTemplates          = false,
                             String?                        DatabaseFileName          = DefaultAssetsDBFileName,
                             Boolean?                       DisableNotifications      = false,

                             Boolean?                       DisableLogging            = null,
                             String?                        LoggingContext            = null,
                             String?                        LoggingPath               = null,
                             String?                        LogfileName               = null,
                             OCPILogfileCreatorDelegate?    LogfileCreator            = null)

            : base(Description ?? I18NString.Create("OCPI Common HTTP API"),
                   HTTPAPI,
                   RootPath,
                   BasePath,

                   ExternalDNSName,
                   HTTPServerName,
                   HTTPServiceName,
                   APIVersionHash,
                   APIVersionHashes,

                   IsDevelopment,
                   DevelopmentServers,
                   DisableLogging,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator is not null
                       ? (loggingPath, context, logfileName) => LogfileCreator(loggingPath, null, context, logfileName)
                       : null)

        {

            this.OurBaseURL               = OurBaseURL;
            this.OurVersionsURL           = OurVersionsURL;

            this.AdditionalURLPathPrefix  = AdditionalURLPathPrefix;
            this.LocationsAsOpenData      = LocationsAsOpenData;
            this.TariffsAsOpenData        = TariffsAsOpenData;
            this.AllowDowngrades          = AllowDowngrades;

            this.ClientConfigurations     = new ClientConfigurator();

            RegisterURLTemplates();

            if (!this.DisableLogging)
                Logger = new CommonHTTPAPILogger(
                             this,
                             LoggingPath ?? AppContext.BaseDirectory,
                             LoggingContext,
                             LogfileCreator: LogfileCreator
                         );

        }

        #endregion



        #region AccessTokens

        private readonly ConcurrentDictionary<AccessToken, AccessStatus> accessTokens = [];

        public AccessStatus DefaultAccessStatus { get; set; } = AccessStatus.ALLOWED;


        #region AddAccessToken    (Token, Status)

        public async Task AddAccessToken(AccessToken   Token,
                                         AccessStatus  Status)
        {

            accessTokens.AddOrUpdate(
                Token,
                Status,
                (a, b) => b
            );

            //ToDo: Persist!

        }

        #endregion

        #region RemoveAccessToken (Token)

        public async Task RemoveAccessToken(AccessToken Token)
        {

            accessTokens.TryRemove(Token, out _);

            //ToDo: Persist!

        }

        #endregion


        public Boolean AccessTokenIs(AccessToken   Token,
                                     AccessStatus  Status)
        {

            if (accessTokens.TryGetValue(Token, out var status))
                return status == Status;

            return DefaultAccessStatus ==  Status;

        }

        public Boolean AccessTokenIsAllowed(AccessToken Token)

            => AccessTokenIs(
                   Token,
                   AccessStatus.ALLOWED
               );

        public Boolean AccessTokenIsBlocked(AccessToken Token)

            => AccessTokenIs(
                   Token,
                   AccessStatus.BLOCKED
               );

        #endregion



        #region (private) RegisterURLTemplates()

        private void RegisterURLTemplates()
        {

            #region OPTIONS  ~/

            HTTPBaseAPI.AddHandler(
                HTTPMethod.OPTIONS,
                URLPathPrefix,
                HTTPDelegate: request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            Connection                 = ConnectionType.KeepAlive
                        }.AsImmutable)

            );

            #endregion

            #region GET      ~/

            HTTPBaseAPI.AddHandler(
                HTTPMethod.GET,
                URLPathPrefix,
                HTTPContentType.Text.PLAIN,
                HTTPRequestLogger:   GetRootRequest,
                HTTPResponseLogger:  GetRootResponse,
                HTTPDelegate:        request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            ContentType                = HTTPContentType.Text.PLAIN,
                            Content                    = "This is an Open Charge Point Interface v2.x HTTP service!\r\nPlease check ~/versions!".ToUTF8Bytes(),
                            Connection                 = ConnectionType.KeepAlive,
                            Vary                       = "Accept"
                        }.AsImmutable)

            );

            HTTPBaseAPI.AddHandler(
                HTTPMethod.GET,
                URLPathPrefix,
                HTTPContentType.Application.JSON_UTF8,
                HTTPRequestLogger:   GetRootRequest,
                HTTPResponseLogger:  GetRootResponse,
                HTTPDelegate:        request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                            Content                    = JSONObject.Create(
                                                             new JProperty(
                                                                 "message",
                                                                 "This is an Open Charge Point Interface v2.x HTTP service! Please check ~/versions!"
                                                             )
                                                         ).ToUTF8Bytes(),
                            Connection                 = ConnectionType.KeepAlive,
                            Vary                       = "Accept"
                        }.AsImmutable),

                AllowReplacement: URLReplacement.Allow

            );

            #endregion


            #region OPTIONS  ~/versions

            // ----------------------------------------------------
            // curl -v -X OPTIONS http://127.0.0.1:2502/versions
            // ----------------------------------------------------
            HTTPBaseAPI.AddHandler(
                HTTPMethod.OPTIONS,
                URLPathPrefix + "versions",
                HTTPDelegate: request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            Vary                       = "Accept"
                        }.AsImmutable)

            );

            #endregion

            #region GET      ~/versions

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2502/versions
            // ----------------------------------------------------------------------
            HTTPBaseAPI.AddHandler(
                HTTPMethod.GET,
                URLPathPrefix + "versions",
                HTTPContentType.Application.JSON_UTF8,
                HTTPRequestLogger:   GetVersionsRequest,
                HTTPResponseLogger:  GetVersionsResponse,
                HTTPDelegate:        request => {

                    var requestId        = request.TryParseHeaderField<Request_Id>    ("X-Request-ID",           Request_Id.    TryParse) ?? Request_Id.    NewRandom(IsLocal: true);
                    var correlationId    = request.TryParseHeaderField<Correlation_Id>("X-Correlation-ID",       Correlation_Id.TryParse) ?? Correlation_Id.NewRandom(IsLocal: true);
                    var toCountryCode    = request.TryParseHeaderField<CountryCode>   ("OCPI-to-country-code",   CountryCode.   TryParse);
                    var toPartyId        = request.TryParseHeaderField<Party_Id>      ("OCPI-to-party-id",       Party_Id.      TryParse);
                    var fromCountryCode  = request.TryParseHeaderField<CountryCode>   ("OCPI-from-country-code", CountryCode.   TryParse);
                    var fromPartyId      = request.TryParseHeaderField<Party_Id>      ("OCPI-from-party-id",     Party_Id.      TryParse);


                    #region Check access token

                    AccessToken? accessToken1 = null;

                    if (request.Authorization is HTTPTokenAuthentication TokenAuth &&
                        //TokenAuth.Token.TryParseBASE64_UTF8(out var decodedToken, out var errorResponse) &&
                        AccessToken.TryParse(TokenAuth.Token, out var accessToken))
                    {
                        accessToken1 = accessToken;
                    }

                    else if (request.Authorization is HTTPBasicAuthentication BasicAuth &&
                        AccessToken.TryParse(BasicAuth.Username, out accessToken))
                    {
                        accessToken1 = accessToken;
                    }

                    if (accessToken1.HasValue)
                    {

                        if (AccessTokenIsBlocked(accessToken1.Value))
                        {

                            var httpResponseBuilder2 =
                                new HTTPResponse.Builder(request) {
                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                    Server                     = HTTPServiceName,
                                    Date                       = Timestamp.Now,
                                    AccessControlAllowOrigin   = "*",
                                    AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                                    Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                                    AccessControlAllowHeaders  = [ "Authorization" ],
                                    ContentType                = HTTPContentType.Application.JSON_UTF8,
                                    Content                    = JSONObject.Create(
                                                                     new JProperty("status_code",     2000),
                                                                     new JProperty("status_message", "Invalid or blocked access token!")
                                                                 ).ToUTF8Bytes(),
                                    Connection                 = ConnectionType.KeepAlive,
                                    Vary                       = "Accept"
                                };

                            httpResponseBuilder2.Set("X-Request-ID",      requestId);
                            httpResponseBuilder2.Set("X-Correlation-ID",  correlationId);

                            return Task.FromResult(httpResponseBuilder2.AsImmutable);

                        }

                    }

                    #endregion


                    var httpResponseBuilder =
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServiceName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "OPTIONS", "GET" ],
                            Allow                      = [ HTTPMethod.OPTIONS, HTTPMethod.GET ],
                            AccessControlAllowHeaders  = [ "Authorization" ],
                            ContentType                = HTTPContentType.Application.JSON_UTF8,
                            Content                    = JSONObject.Create(
                                                             new JProperty("status_code",      1000),
                                                             new JProperty("status_message",  "Hello world!"),
                                                             new JProperty("data",             new JArray(
                                                                                                   VersionInformations.
                                                                                                       OrderBy(versionInformation => versionInformation.Id).
                                                                                                       Select (versionInformation => versionInformation.ToJSON(CustomVersionInformationSerializer))
                                                                                               ))
                                                         ).ToUTF8Bytes(),
                            Connection                 = ConnectionType.KeepAlive,
                            Vary                       = "Accept"
                        };

                    httpResponseBuilder.Set("X-Request-ID",      requestId);
                    httpResponseBuilder.Set("X-Correlation-ID",  correlationId);

                    return Task.FromResult(httpResponseBuilder.AsImmutable);

                }

            );

            #endregion


            #region GET      ~/support

            HTTPBaseAPI.AddHandler(
                HTTPMethod.GET,
                URLPathPrefix + "/support",
                HTTPContentType.Text.PLAIN,
                HTTPDelegate: request =>

                    Task.FromResult(
                        new HTTPResponse.Builder(request) {
                            HTTPStatusCode             = HTTPStatusCode.OK,
                            Server                     = HTTPServerName,
                            Date                       = Timestamp.Now,
                            AccessControlAllowOrigin   = "*",
                            AccessControlAllowMethods  = [ "GET" ],
                            AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                            ContentType                = HTTPContentType.Text.PLAIN,
                            Content                    = "https://github.com/OpenChargingCloud/WWCP_OCPI".ToUTF8Bytes(),
                            Connection                 = ConnectionType.KeepAlive,
                            Vary                       = "Accept"
                        }.AsImmutable
                    )

            );

            #endregion

        }

        #endregion



        #region OCPI Version Informations

        private readonly ConcurrentDictionary<Version_Id, VersionInformation>  versionInformations   = [];

        public IEnumerable<VersionInformation> VersionInformations
            => versionInformations.Values;


        #region AddVersionInformation            (VersionInformation, ...)

        public async Task<AddResult<VersionInformation>>

            AddVersionInformation(VersionInformation  VersionInformation,
                                  Boolean             SkipNotifications   = false,
                                  EventTracking_Id?   EventTrackingId     = null,
                                  User_Id?            CurrentUserId       = null)

        {

            EventTrackingId ??= EventTracking_Id.New;

            if (versionInformations.TryAdd(VersionInformation.Id, VersionInformation))
            {

                //await LogAsset(
                //          addLocation,
                //          Location.ToJSON(true,
                //                          true,
                //                          true,
                //                          CustomLocationSerializer,
                //                          CustomPublishTokenSerializer,
                //                          CustomAddressSerializer,
                //                          CustomAdditionalGeoLocationSerializer,
                //                          CustomChargingStationSerializer,
                //                          CustomEVSESerializer,
                //                          CustomStatusScheduleSerializer,
                //                          CustomConnectorSerializer,
                //                          CustomEnergyMeterSerializer,
                //                          CustomTransparencySoftwareStatusSerializer,
                //                          CustomTransparencySoftwareSerializer,
                //                          CustomDisplayTextSerializer,
                //                          CustomBusinessDetailsSerializer,
                //                          CustomHoursSerializer,
                //                          CustomImageSerializer,
                //                          CustomEnergyMixSerializer,
                //                          CustomEnergySourceSerializer,
                //                          CustomEnvironmentalImpactSerializer,
                //                          CustomLocationMaxPowerSerializer),
                //          EventTrackingId,
                //          CurrentUserId
                //      );

                //if (!SkipNotifications)
                //{

                //    var OnLocationAddedLocal = OnLocationAdded;
                //    if (OnLocationAddedLocal is not null)
                //    {
                //        try
                //        {
                //            await OnLocationAddedLocal(Location);
                //        }
                //        catch (Exception e)
                //        {
                //            DebugX.LogT($"OCPI {Version.String} {nameof(CommonAPI)} ", nameof(AddLocation), " ", nameof(OnLocationAdded), ": ",
                //                        Environment.NewLine, e.Message,
                //                        Environment.NewLine, e.StackTrace ?? "");
                //        }
                //    }

                //}

                return AddResult<VersionInformation>.Success(
                           EventTrackingId,
                           VersionInformation
                       );

            }

            return AddResult<VersionInformation>.Failed(
                       EventTrackingId,
                       VersionInformation,
                       "The given version information already exists!"
                   );

        }

        #endregion

        #endregion


        //ToDo: Wrap the following into a pluggable interface!

        #region Read/Write Database Files

        #region WriteToDatabase                          (FileName, Text,   ...)

        public ValueTask WriteToDatabase(String             FileName,
                                         String             Text,
                                         CancellationToken  CancellationToken   = default)

          => logFileWriter.EnqueueAsync(
                 FileName,
                 Text,
                 CancellationToken
             );

        #endregion

        #region WriteToDatabase                          (FileName, JToken, ...)

        public ValueTask WriteToDatabase(String             FileName,
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
                             new JProperty("timestamp",        Timestamp.Now.  ToISO8601()),
                             new JProperty("eventTrackingId",  EventTrackingId.ToString()),

                       CurrentUserId is not null
                           ? new JProperty("userId",           CurrentUserId)
                           : null).

                   ToString(Newtonsoft.Json.Formatting.None),

                   CancellationToken

               );

        #endregion

        #region WriteCommentToDatabase                   (FileName, Text,   ...)

        public ValueTask WriteCommentToDatabase(String             FileName,
                                                String             Text,
                                                EventTracking_Id   EventTrackingId,
                                                User_Id?           CurrentUserId       = null,
                                                CancellationToken  CancellationToken   = default)

            => WriteToDatabase(
                   FileName,
                   $"//{Timestamp.Now.ToISO8601()} {EventTrackingId} {(CurrentUserId is not null ? CurrentUserId : "-")}: {Text}{Environment.NewLine}",
                   CancellationToken
               );

        #endregion


        #region LoadCommandsFromDatabaseFile             (DBFileName)

        public IEnumerable<Command> LoadCommandsFromDatabaseFile(String DBFileName)
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
            catch (FileNotFoundException)
            { }
            catch (Exception e)
            {
                DebugX.LogException(e, $"OCPI.CommonAPIBase.ReadDatabaseFile({DBFileName})");
            }

            return [];

        }

        #endregion

        #region ParseCommands                            (Commands)

        public IEnumerable<Command> ParseCommands(IEnumerable<String> Commands)
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
                        DebugX.LogException(e, $"OCPI.CommonAPIBase.ProcessCommands()");
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

        #region LoadCommandsWithMetadataFromDatabaseFile (DBFileName)

        public IEnumerable<CommandWithMetadata> LoadCommandsWithMetadataFromDatabaseFile(String DBFileName)
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
                DebugX.LogException(e, $"OCPI.CommonAPIBase.ReadDatabaseFileWithMetadata({DBFileName})");
            }

            return [];

        }

        #endregion

        #region ParseCommandsWithMetadata                (Commands)

        public IEnumerable<CommandWithMetadata> ParseCommandsWithMetadata(IEnumerable<String> Commands)
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


    }

}
