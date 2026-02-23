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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The 'NotifyWebPaymentsStarted' command.
    /// </summary>
    public class NotifyWebPaymentsStartedCommand : AOCPICommand<NotifyWebPaymentsStartedCommand>
    {

        #region Data

        /// <summary>
        /// The default timeout after which the WebPayment process is considered failed of aborted.
        /// </summary>
        public static readonly TimeSpan  DefaultTimeout   = TimeSpan.FromMinutes(2);

        #endregion

        #region Properties

        /// <summary>
        /// The identification of the location for which a WebPayment process shall be started.
        /// </summary>
        [Mandatory]
        public Location_Id    LocationId     { get; }

        /// <summary>
        /// The unique identification of the EVSE for which a WebPayment process shall be started.
        /// </summary>
        [Mandatory]
        public EVSE_UId       EVSEUId        { get; }

        /// <summary>
        /// An optional identification of the EVSE for which a WebPayment process shall be started.
        /// This can support debugging and logging, but is not required as the EVSE can be identified
        /// by the combination of location_id and evse_uid.
        /// </summary>
        [Optional]
        public EVSE_Id?       EVSEId         { get; }

        /// <summary>
        /// An optional identification of the connector for which a WebPayment process shall be started.
        /// This field is required when the capability: START_SESSION_CONNECTOR_REQUIRED is set on the EVSE
        /// (ISO 15118 format, e.g. DE*GEF*E12345678*1).
        /// </summary>
        [Optional]
        public Connector_Id?  ConnectorId    { get; }

        /// <summary>
        /// The timeout after which the WebPayment process is considered failed of aborted.
        /// Should be <= 5 minutes.
        /// </summary>
        [Mandatory]
        public TimeSpan       Timeout        { get; }

        /// <summary>
        /// Optional custom OCPP data to be forwarded to the charging station.
        /// </summary>
        [Mandatory]
        public JObject?       CustomData     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'NotifyWebPaymentsStarted' command command.
        /// </summary>
        /// <param name="ResponseURL">The URL to which the response of this command should be sent to.</param>
        /// 
        /// <param name="LocationId">The identification of the location for which a WebPayment process shall be started.</param>
        /// <param name="EVSEUId">The unique identification of the EVSE for which a WebPayment process shall be started.</param>
        /// <param name="EVSEId">An optional identification of the EVSE for which a WebPayment process shall be started. This can support debugging and logging, but is not required as the EVSE can be identified by the combination of location_id and evse_uid (ISO 15118 format, e.g. DE*GEF*E12345678*1).</param>
        /// <param name="ConnectorId">An optional identification of the connector for which a WebPayment process shall be started. This field is required when the capability: START_SESSION_CONNECTOR_REQUIRED is set on the EVSE.</param>
        /// <param name="Timeout">An optional timeout after which the WebPayment process is considered failed of aborted. Should be <= 5 minutes. Default: 2 Minutes.</param>
        /// <param name="CustomData">Optional custom OCPP data to be forwarded to the charging station.</param>
        /// 
        /// <param name="Id">An optional unique identification of the command.</param>
        /// <param name="RequestId">An optional unique request identification.</param>
        /// <param name="CorrelationId">An optional unique request correlation identification.</param>
        public NotifyWebPaymentsStartedCommand(URL              ResponseURL,

                                               Location_Id      LocationId,
                                               EVSE_UId         EVSEUId,
                                               EVSE_Id?         EVSEId          = null,
                                               Connector_Id?    ConnectorId     = null,
                                               TimeSpan?        Timeout         = null,
                                               JObject?         CustomData      = null,

                                               Command_Id?      Id              = null,
                                               Request_Id?      RequestId       = null,
                                               Correlation_Id?  CorrelationId   = null)

            : base(ResponseURL,
                   Id,
                   RequestId,
                   CorrelationId)

        {

            this.LocationId   = LocationId;
            this.EVSEUId      = EVSEUId;
            this.EVSEId       = EVSEId;
            this.ConnectorId  = ConnectorId;
            this.Timeout      = Timeout ?? DefaultTimeout;
            this.CustomData   = CustomData;

            unchecked
            {

                hashCode = this.LocationId.             GetHashCode()       * 29 ^
                           this.EVSEUId.                GetHashCode()       * 23 ^
                          (this.EVSEId?.                GetHashCode() ?? 0) * 19 ^
                          (this.ConnectorId?.           GetHashCode() ?? 0) * 17 ^
                           this.Timeout.                GetHashCode()       * 13 ^
                          (this.CustomData?.            GetHashCode() ?? 0) * 11 ^

                           this.ResponseURL.            GetHashCode()       *  7 ^
                           this.Id.                     GetHashCode()       *  5 ^
                           this.RequestId.              GetHashCode()       *  3 ^
                           this.CorrelationId.          GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomNotifyWebPaymentsStartedCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of a 'NotifyWebPaymentsStarted' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomNotifyWebPaymentsStartedCommandParser">A delegate to parse custom 'NotifyWebPaymentsStarted' command JSON objects.</param>
        public static NotifyWebPaymentsStartedCommand Parse(JObject                                                        JSON,
                                                            CustomJObjectParserDelegate<NotifyWebPaymentsStartedCommand>?  CustomNotifyWebPaymentsStartedCommandParser   = null)
        {

            if (TryParse(JSON,
                         out var notifyWebPaymentsStartedCommand,
                         out var errorResponse,
                         CustomNotifyWebPaymentsStartedCommandParser))
            {
                return notifyWebPaymentsStartedCommand;
            }

            throw new ArgumentException("The given JSON representation of a 'NotifyWebPaymentsStarted' command is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out NotifyWebPaymentsStartedCommand, out ErrorResponse, CustomNotifyWebPaymentsStartedCommandParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a 'NotifyWebPaymentsStarted' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="NotifyWebPaymentsStartedCommand">The parsed 'NotifyWebPaymentsStarted' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                    JSON,
                                       [NotNullWhen(true)]  out NotifyWebPaymentsStartedCommand?  NotifyWebPaymentsStartedCommand,
                                       [NotNullWhen(false)] out String?                           ErrorResponse)

            => TryParse(JSON,
                        out NotifyWebPaymentsStartedCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a 'NotifyWebPaymentsStarted' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="NotifyWebPaymentsStartedCommand">The parsed 'NotifyWebPaymentsStarted' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyWebPaymentsStartedCommandParser">A delegate to parse custom 'NotifyWebPaymentsStarted' command JSON objects.</param>
        public static Boolean TryParse(JObject                                                        JSON,
                                       [NotNullWhen(true)]  out NotifyWebPaymentsStartedCommand?      NotifyWebPaymentsStartedCommand,
                                       [NotNullWhen(false)] out String?                               ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyWebPaymentsStartedCommand>?  CustomNotifyWebPaymentsStartedCommandParser   = null)
        {

            try
            {

                NotifyWebPaymentsStartedCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse LocationId       [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id locationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId          [mandatory]

                if (!JSON.ParseMandatory("evse_uid",
                                         "EVSE unique identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId evseUId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEId           [optional]

                if (JSON.ParseOptional("evse_id",
                                       "EVSE identification",
                                       EVSE_Id.TryParse,
                                       out EVSE_Id? evseId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse ConnectorId      [optional]

                if (JSON.ParseOptional("connector_id",
                                       "connector identification",
                                       Connector_Id.TryParse,
                                       out Connector_Id? connectorId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Timeout          [mandatory]

                if (!JSON.ParseMandatory("timeout",
                                         "web payment process timeout",
                                         out TimeSpan timeout,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CustomData       [optional]

                var customData = JSON["custom_data"] as JObject;

                #endregion


                #region Parse ResponseURL      [mandatory]

                if (!JSON.ParseMandatory("response_url",
                                         "response URL",
                                         URL.TryParse,
                                         out URL responseURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CommandId        [optional, internal]

                if (JSON.ParseOptional("id",
                                       "command identification",
                                       Command_Id.TryParse,
                                       out Command_Id? commandId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse RequestId        [optional, internal]

                if (JSON.ParseOptional("request_id",
                                       "request identification",
                                       Request_Id.TryParse,
                                       out Request_Id? requestId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse CorrelationId    [optional, internal]

                if (JSON.ParseOptional("correlation_Id",
                                       "correlation identification",
                                       Correlation_Id.TryParse,
                                       out Correlation_Id? correlationId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                NotifyWebPaymentsStartedCommand = new NotifyWebPaymentsStartedCommand(

                                                      responseURL,

                                                      locationId,
                                                      evseUId,
                                                      evseId,
                                                      connectorId,
                                                      timeout,
                                                      customData,

                                                      commandId,
                                                      requestId,
                                                      correlationId

                                                  );

                if (CustomNotifyWebPaymentsStartedCommandParser is not null)
                    NotifyWebPaymentsStartedCommand = CustomNotifyWebPaymentsStartedCommandParser(JSON,
                                                                                                  NotifyWebPaymentsStartedCommand);

                return true;

            }
            catch (Exception e)
            {
                NotifyWebPaymentsStartedCommand  = default;
                ErrorResponse                    = "The given JSON representation of a 'NotifyWebPaymentsStarted' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyWebPaymentsStartedCommandSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyWebPaymentsStartedCommandSerializer">A delegate to serialize custom 'NotifyWebPaymentsStarted' command JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyWebPaymentsStartedCommand>? CustomNotifyWebPaymentsStartedCommandSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("location_id",    LocationId. ToString()),
                                 new JProperty("evse_uid",       EVSEUId.    ToString()),

                           EVSEId.     HasValue
                               ? new JProperty("evse_id",        EVSEId.     ToString())
                               : null,

                           ConnectorId.HasValue
                               ? new JProperty("connector_id",   ConnectorId.ToString())
                               : null,

                                 new JProperty("timeout",        Timeout.    TotalSeconds),

                           CustomData is not null
                               ? new JProperty("custom_data",    CustomData)
                               : null,

                                 new JProperty("response_url",   ResponseURL.ToString())

                       );

            return CustomNotifyWebPaymentsStartedCommandSerializer is not null
                       ? CustomNotifyWebPaymentsStartedCommandSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyWebPaymentsStartedCommand1, NotifyWebPaymentsStartedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand1">A 'NotifyWebPaymentsStarted' command.</param>
        /// <param name="NotifyWebPaymentsStartedCommand2">Another 'NotifyWebPaymentsStarted' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand1,
                                           NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand2)
        {

            if (Object.ReferenceEquals(NotifyWebPaymentsStartedCommand1, NotifyWebPaymentsStartedCommand2))
                return true;

            if (NotifyWebPaymentsStartedCommand1 is null || NotifyWebPaymentsStartedCommand2 is null)
                return false;

            return NotifyWebPaymentsStartedCommand1.Equals(NotifyWebPaymentsStartedCommand2);

        }

        #endregion

        #region Operator != (NotifyWebPaymentsStartedCommand1, NotifyWebPaymentsStartedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand1">A 'NotifyWebPaymentsStarted' command.</param>
        /// <param name="NotifyWebPaymentsStartedCommand2">Another 'NotifyWebPaymentsStarted' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand1,
                                           NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand2)

            => !(NotifyWebPaymentsStartedCommand1 == NotifyWebPaymentsStartedCommand2);

        #endregion

        #region Operator <  (NotifyWebPaymentsStartedCommand1, NotifyWebPaymentsStartedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand1">A 'NotifyWebPaymentsStarted' command.</param>
        /// <param name="NotifyWebPaymentsStartedCommand2">Another 'NotifyWebPaymentsStarted' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand1,
                                          NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand2)

            => NotifyWebPaymentsStartedCommand1 is null
                   ? throw new ArgumentNullException(nameof(NotifyWebPaymentsStartedCommand1), "The given 'NotifyWebPaymentsStarted' command must not be null!")
                   : NotifyWebPaymentsStartedCommand1.CompareTo(NotifyWebPaymentsStartedCommand2) < 0;

        #endregion

        #region Operator <= (NotifyWebPaymentsStartedCommand1, NotifyWebPaymentsStartedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand1">A 'NotifyWebPaymentsStarted' command.</param>
        /// <param name="NotifyWebPaymentsStartedCommand2">Another 'NotifyWebPaymentsStarted' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand1,
                                           NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand2)

            => !(NotifyWebPaymentsStartedCommand1 > NotifyWebPaymentsStartedCommand2);

        #endregion

        #region Operator >  (NotifyWebPaymentsStartedCommand1, NotifyWebPaymentsStartedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand1">A 'NotifyWebPaymentsStarted' command.</param>
        /// <param name="NotifyWebPaymentsStartedCommand2">Another 'NotifyWebPaymentsStarted' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand1,
                                          NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand2)

            => NotifyWebPaymentsStartedCommand1 is null
                   ? throw new ArgumentNullException(nameof(NotifyWebPaymentsStartedCommand1), "The given 'NotifyWebPaymentsStarted' command must not be null!")
                   : NotifyWebPaymentsStartedCommand1.CompareTo(NotifyWebPaymentsStartedCommand2) > 0;

        #endregion

        #region Operator >= (NotifyWebPaymentsStartedCommand1, NotifyWebPaymentsStartedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand1">A 'NotifyWebPaymentsStarted' command.</param>
        /// <param name="NotifyWebPaymentsStartedCommand2">Another 'NotifyWebPaymentsStarted' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand1,
                                           NotifyWebPaymentsStartedCommand NotifyWebPaymentsStartedCommand2)

            => !(NotifyWebPaymentsStartedCommand1 < NotifyWebPaymentsStartedCommand2);

        #endregion

        #endregion

        #region IComparable<NotifyWebPaymentsStartedCommand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two 'NotifyWebPaymentsStarted' commands.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand">A 'NotifyWebPaymentsStarted' command to compare with.</param>
        public override Int32 CompareTo(Object? Object)

            => Object is NotifyWebPaymentsStartedCommand notifyWebPaymentsStartedCommand
                   ? CompareTo(notifyWebPaymentsStartedCommand)
                   : throw new ArgumentException("The given object is not a 'NotifyWebPaymentsStarted' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(NotifyWebPaymentsStartedCommand)

        /// <summary>
        /// Compares two 'NotifyWebPaymentsStarted' commands.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand">A 'NotifyWebPaymentsStarted' command to compare with.</param>
        public override Int32 CompareTo(NotifyWebPaymentsStartedCommand? NotifyWebPaymentsStartedCommand)
        {

            if (NotifyWebPaymentsStartedCommand is null)
                throw new ArgumentNullException(nameof(NotifyWebPaymentsStartedCommand), "The given 'NotifyWebPaymentsStarted' command must not be null!");

            var c = LocationId.       CompareTo(NotifyWebPaymentsStartedCommand.LocationId);

            if (c == 0)
                c = EVSEUId.          CompareTo(NotifyWebPaymentsStartedCommand.EVSEUId);

            if (c == 0)
                c = Timeout.          CompareTo(NotifyWebPaymentsStartedCommand.Timeout);

            if (c == 0 && EVSEId.     HasValue && NotifyWebPaymentsStartedCommand.EVSEId.     HasValue)
                c = EVSEId.     Value.CompareTo(NotifyWebPaymentsStartedCommand.EVSEId.Value);

            if (c == 0 && ConnectorId.HasValue && NotifyWebPaymentsStartedCommand.ConnectorId.HasValue)
                c = ConnectorId.Value.CompareTo(NotifyWebPaymentsStartedCommand.ConnectorId.Value);

            if (c == 0 && CustomData is not null && NotifyWebPaymentsStartedCommand.CustomData is not null)
                c = CustomData.ToString(Newtonsoft.Json.Formatting.None).
                                      CompareTo(NotifyWebPaymentsStartedCommand.CustomData.ToString(Newtonsoft.Json.Formatting.None));


            if (c == 0)
                c = ResponseURL.      CompareTo(NotifyWebPaymentsStartedCommand.ResponseURL);

            if (c == 0)
                c = Id.               CompareTo(NotifyWebPaymentsStartedCommand.Id);

            if (c == 0)
                c = RequestId.        CompareTo(NotifyWebPaymentsStartedCommand.RequestId);

            if (c == 0)
                c = CorrelationId.    CompareTo(NotifyWebPaymentsStartedCommand.CorrelationId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<NotifyWebPaymentsStartedCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two 'NotifyWebPaymentsStarted' commands for equality.
        /// </summary>
        /// <param name="Object">A 'NotifyWebPaymentsStarted' command to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyWebPaymentsStartedCommand notifyWebPaymentsStartedCommand &&
                   Equals(notifyWebPaymentsStartedCommand);

        #endregion

        #region Equals(NotifyWebPaymentsStartedCommand)

        /// <summary>
        /// Compares two 'NotifyWebPaymentsStarted' commands for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentsStartedCommand">A 'NotifyWebPaymentsStarted' command to compare with.</param>
        public override Boolean Equals(NotifyWebPaymentsStartedCommand? NotifyWebPaymentsStartedCommand)

            => NotifyWebPaymentsStartedCommand is not null &&

               LocationId.   Equals(NotifyWebPaymentsStartedCommand.LocationId)  &&
               EVSEUId.      Equals(NotifyWebPaymentsStartedCommand.EVSEUId)     &&
               EVSEId.       Equals(NotifyWebPaymentsStartedCommand.EVSEId)      &&
               ConnectorId.  Equals(NotifyWebPaymentsStartedCommand.ConnectorId) &&
               Timeout.      Equals(NotifyWebPaymentsStartedCommand.Timeout)     &&
               // CustomData

               ResponseURL.  Equals(NotifyWebPaymentsStartedCommand.ResponseURL) &&
               Id.           Equals(NotifyWebPaymentsStartedCommand.Id)          &&
               RequestId.    Equals(NotifyWebPaymentsStartedCommand.RequestId)   &&
               CorrelationId.Equals(NotifyWebPaymentsStartedCommand.CorrelationId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   LocationId,
                   "/",
                   EVSEUId,

                   EVSEId.HasValue
                       ? $", EVSE: {EVSEId}"
                       : "",

                   ConnectorId.HasValue
                       ? $", connector: {ConnectorId}"
                       : "",

                   $", timeout: {Timeout.TotalSeconds} seconds",

                   CustomData is not null
                       ? $", custom data: {CustomData.ToString(Newtonsoft.Json.Formatting.None)}"
                       : "",

                   $" [{Id}] => ",
                   ResponseURL

               );

        #endregion

    }

}
