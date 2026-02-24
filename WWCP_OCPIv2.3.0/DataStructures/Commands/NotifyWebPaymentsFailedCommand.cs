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

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The 'NotifyWebPaymentsFailed' command.
    /// </summary>
    public class NotifyWebPaymentsFailedCommand : AOCPICommand<NotifyWebPaymentsFailedCommand>
    {

        #region Properties

        /// <summary>
        /// The identification of the location at which the WebPayment process was aborted or failed.
        /// </summary>
        [Mandatory]
        public Location_Id    LocationId      { get; }

        /// <summary>
        /// The unique identification of the EVSE at which the WebPayment process was aborted or failed.
        /// </summary>
        [Mandatory]
        public EVSE_UId       EVSEUId         { get; }

        /// <summary>
        /// An optional identification of the EVSE at which the WebPayment process was aborted or failed.
        /// This can support debugging and logging, but is not required as the EVSE can be identified
        /// by the combination of location_id and evse_uid (ISO 15118 format, e.g. DE*GEF*E12345678*1).
        /// </summary>
        [Optional]
        public EVSE_Id?       EVSEId          { get; }

        /// <summary>
        /// An optional identification of the connector at which the WebPayment process was aborted or failed.
        /// This field is required when the capability: START_SESSION_CONNECTOR_REQUIRED is set on the EVSE.
        /// </summary>
        [Optional]
        public Connector_Id?  ConnectorId     { get; }

        /// <summary>
        /// An optional error message why the WebPayment process was aborted or failed.
        /// </summary>
        [Mandatory]
        public DisplayTexts?  ErrorMessage    { get; }

        /// <summary>
        /// Optional custom OCPP data to be forwarded to the charging station.
        /// </summary>
        [Mandatory]
        public JObject?       CustomData      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'NotifyWebPaymentsFailed' command command.
        /// </summary>
        /// <param name="ResponseURL">The URL to which the response of this command should be sent to.</param>
        /// 
        /// <param name="LocationId">The identification of the location at which the WebPayment process was aborted or failed.</param>
        /// <param name="EVSEUId">The unique identification of the EVSE at which the WebPayment process was aborted or failed.</param>
        /// <param name="EVSEId">An optional identification of the EVSE at which the WebPayment process was aborted or failed. This can support debugging and logging, but is not required as the EVSE can be identified by the combination of location_id and evse_uid (ISO 15118 format, e.g. DE*GEF*E12345678*1).</param>
        /// <param name="ConnectorId">An optional identification of the connector at which the WebPayment process was aborted or failed. This field is required when the capability: START_SESSION_CONNECTOR_REQUIRED is set on the EVSE.</param>
        /// <param name="ErrorMessage">An optional error message why the WebPayment process was aborted or failed.</param>
        /// <param name="CustomData">Optional custom OCPP data to be forwarded to the charging station.</param>
        /// 
        /// <param name="Id">An optional unique identification of the command.</param>
        /// <param name="RequestId">An optional unique request identification.</param>
        /// <param name="CorrelationId">An optional unique request correlation identification.</param>
        public NotifyWebPaymentsFailedCommand(URL              ResponseURL,

                                              Location_Id      LocationId,
                                              EVSE_UId         EVSEUId,
                                              EVSE_Id?         EVSEId          = null,
                                              Connector_Id?    ConnectorId     = null,
                                              DisplayTexts?    ErrorMessage    = null,
                                              JObject?         CustomData      = null,

                                              Command_Id?      Id              = null,
                                              Request_Id?      RequestId       = null,
                                              Correlation_Id?  CorrelationId   = null)

            : base(ResponseURL,
                   Id,
                   RequestId,
                   CorrelationId)

        {

            this.LocationId    = LocationId;
            this.EVSEUId       = EVSEUId;
            this.EVSEId        = EVSEId;
            this.ConnectorId   = ConnectorId;
            this.ErrorMessage  = ErrorMessage;
            this.CustomData    = CustomData;

            unchecked
            {

                hashCode = this.LocationId.             GetHashCode()       * 29 ^
                           this.EVSEUId.                GetHashCode()       * 23 ^
                          (this.EVSEId?.                GetHashCode() ?? 0) * 19 ^
                          (this.ConnectorId?.           GetHashCode() ?? 0) * 17 ^
                          (this.ErrorMessage?.          GetHashCode() ?? 0) * 13 ^
                          (this.CustomData?.            GetHashCode() ?? 0) * 11 ^

                           this.ResponseURL.            GetHashCode()       *  7 ^
                           this.Id.                     GetHashCode()       *  5 ^
                           this.RequestId.              GetHashCode()       *  3 ^
                           this.CorrelationId.          GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomNotifyWebPaymentsFailedCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of a 'NotifyWebPaymentsFailed' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomNotifyWebPaymentsFailedCommandParser">A delegate to parse custom 'NotifyWebPaymentsFailed' command JSON objects.</param>
        public static NotifyWebPaymentsFailedCommand Parse(JObject                                                       JSON,
                                                           CustomJObjectParserDelegate<NotifyWebPaymentsFailedCommand>?  CustomNotifyWebPaymentsFailedCommandParser   = null)
        {

            if (TryParse(JSON,
                         out var notifyWebPaymentsFailedCommand,
                         out var errorResponse,
                         CustomNotifyWebPaymentsFailedCommandParser))
            {
                return notifyWebPaymentsFailedCommand;
            }

            throw new ArgumentException("The given JSON representation of a 'NotifyWebPaymentsFailed' command is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out NotifyWebPaymentsFailedCommand, out ErrorResponse, CustomNotifyWebPaymentsFailedCommandParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a 'NotifyWebPaymentsFailed' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="NotifyWebPaymentsFailedCommand">The parsed 'NotifyWebPaymentsFailed' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                                   JSON,
                                       [NotNullWhen(true)]  out NotifyWebPaymentsFailedCommand?  NotifyWebPaymentsFailedCommand,
                                       [NotNullWhen(false)] out String?                          ErrorResponse)

            => TryParse(JSON,
                        out NotifyWebPaymentsFailedCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a 'NotifyWebPaymentsFailed' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="NotifyWebPaymentsFailedCommand">The parsed 'NotifyWebPaymentsFailed' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomNotifyWebPaymentsFailedCommandParser">A delegate to parse custom 'NotifyWebPaymentsFailed' command JSON objects.</param>
        public static Boolean TryParse(JObject                                                       JSON,
                                       [NotNullWhen(true)]  out NotifyWebPaymentsFailedCommand?      NotifyWebPaymentsFailedCommand,
                                       [NotNullWhen(false)] out String?                              ErrorResponse,
                                       CustomJObjectParserDelegate<NotifyWebPaymentsFailedCommand>?  CustomNotifyWebPaymentsFailedCommandParser   = null)
        {

            try
            {

                NotifyWebPaymentsFailedCommand = default;

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

                #region Parse ErrorMessage     [optional]

                if (!JSON.ParseOptionalJSONArray("error_message",
                                                 "web payment error message",
                                                 DisplayTexts.TryParse,
                                                 out DisplayTexts? errorMessage,
                                                 out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                NotifyWebPaymentsFailedCommand = new NotifyWebPaymentsFailedCommand(

                                                     responseURL,

                                                     locationId,
                                                     evseUId,
                                                     evseId,
                                                     connectorId,
                                                     errorMessage,
                                                     customData,

                                                     commandId,
                                                     requestId,
                                                     correlationId

                                                 );

                if (CustomNotifyWebPaymentsFailedCommandParser is not null)
                    NotifyWebPaymentsFailedCommand = CustomNotifyWebPaymentsFailedCommandParser(JSON,
                                                                                                NotifyWebPaymentsFailedCommand);

                return true;

            }
            catch (Exception e)
            {
                NotifyWebPaymentsFailedCommand  = default;
                ErrorResponse                   = "The given JSON representation of a 'NotifyWebPaymentsFailed' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomNotifyWebPaymentsFailedCommandSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomNotifyWebPaymentsFailedCommandSerializer">A delegate to serialize custom 'NotifyWebPaymentsFailed' command JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<NotifyWebPaymentsFailedCommand>? CustomNotifyWebPaymentsFailedCommandSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("location_id",     LocationId.  ToString()),
                                 new JProperty("evse_uid",        EVSEUId.     ToString()),

                           EVSEId.     HasValue
                               ? new JProperty("evse_id",         EVSEId.      ToString())
                               : null,

                           ConnectorId.HasValue
                               ? new JProperty("connector_id",    ConnectorId. ToString())
                               : null,

                           ErrorMessage is not null
                               ? new JProperty("error_message",   ErrorMessage.ToJSON())
                               : null,

                           CustomData   is not null
                               ? new JProperty("custom_data",     CustomData)
                               : null,

                                 new JProperty("response_url",    ResponseURL. ToString())

                       );

            return CustomNotifyWebPaymentsFailedCommandSerializer is not null
                       ? CustomNotifyWebPaymentsFailedCommandSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotifyWebPaymentsFailedCommand1, NotifyWebPaymentsFailedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand1">A 'NotifyWebPaymentsFailed' command.</param>
        /// <param name="NotifyWebPaymentsFailedCommand2">Another 'NotifyWebPaymentsFailed' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand1,
                                           NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand2)
        {

            if (Object.ReferenceEquals(NotifyWebPaymentsFailedCommand1, NotifyWebPaymentsFailedCommand2))
                return true;

            if (NotifyWebPaymentsFailedCommand1 is null || NotifyWebPaymentsFailedCommand2 is null)
                return false;

            return NotifyWebPaymentsFailedCommand1.Equals(NotifyWebPaymentsFailedCommand2);

        }

        #endregion

        #region Operator != (NotifyWebPaymentsFailedCommand1, NotifyWebPaymentsFailedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand1">A 'NotifyWebPaymentsFailed' command.</param>
        /// <param name="NotifyWebPaymentsFailedCommand2">Another 'NotifyWebPaymentsFailed' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand1,
                                           NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand2)

            => !(NotifyWebPaymentsFailedCommand1 == NotifyWebPaymentsFailedCommand2);

        #endregion

        #region Operator <  (NotifyWebPaymentsFailedCommand1, NotifyWebPaymentsFailedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand1">A 'NotifyWebPaymentsFailed' command.</param>
        /// <param name="NotifyWebPaymentsFailedCommand2">Another 'NotifyWebPaymentsFailed' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand1,
                                          NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand2)

            => NotifyWebPaymentsFailedCommand1 is null
                   ? throw new ArgumentNullException(nameof(NotifyWebPaymentsFailedCommand1), "The given 'NotifyWebPaymentsFailed' command must not be null!")
                   : NotifyWebPaymentsFailedCommand1.CompareTo(NotifyWebPaymentsFailedCommand2) < 0;

        #endregion

        #region Operator <= (NotifyWebPaymentsFailedCommand1, NotifyWebPaymentsFailedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand1">A 'NotifyWebPaymentsFailed' command.</param>
        /// <param name="NotifyWebPaymentsFailedCommand2">Another 'NotifyWebPaymentsFailed' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand1,
                                           NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand2)

            => !(NotifyWebPaymentsFailedCommand1 > NotifyWebPaymentsFailedCommand2);

        #endregion

        #region Operator >  (NotifyWebPaymentsFailedCommand1, NotifyWebPaymentsFailedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand1">A 'NotifyWebPaymentsFailed' command.</param>
        /// <param name="NotifyWebPaymentsFailedCommand2">Another 'NotifyWebPaymentsFailed' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand1,
                                          NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand2)

            => NotifyWebPaymentsFailedCommand1 is null
                   ? throw new ArgumentNullException(nameof(NotifyWebPaymentsFailedCommand1), "The given 'NotifyWebPaymentsFailed' command must not be null!")
                   : NotifyWebPaymentsFailedCommand1.CompareTo(NotifyWebPaymentsFailedCommand2) > 0;

        #endregion

        #region Operator >= (NotifyWebPaymentsFailedCommand1, NotifyWebPaymentsFailedCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand1">A 'NotifyWebPaymentsFailed' command.</param>
        /// <param name="NotifyWebPaymentsFailedCommand2">Another 'NotifyWebPaymentsFailed' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand1,
                                           NotifyWebPaymentsFailedCommand NotifyWebPaymentsFailedCommand2)

            => !(NotifyWebPaymentsFailedCommand1 < NotifyWebPaymentsFailedCommand2);

        #endregion

        #endregion

        #region IComparable<NotifyWebPaymentsFailedCommand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two 'NotifyWebPaymentsFailed' commands.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand">A 'NotifyWebPaymentsFailed' command to compare with.</param>
        public override Int32 CompareTo(Object? Object)

            => Object is NotifyWebPaymentsFailedCommand notifyWebPaymentsFailedCommand
                   ? CompareTo(notifyWebPaymentsFailedCommand)
                   : throw new ArgumentException("The given object is not a 'NotifyWebPaymentsFailed' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(NotifyWebPaymentsFailedCommand)

        /// <summary>
        /// Compares two 'NotifyWebPaymentsFailed' commands.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand">A 'NotifyWebPaymentsFailed' command to compare with.</param>
        public override Int32 CompareTo(NotifyWebPaymentsFailedCommand? NotifyWebPaymentsFailedCommand)
        {

            if (NotifyWebPaymentsFailedCommand is null)
                throw new ArgumentNullException(nameof(NotifyWebPaymentsFailedCommand), "The given 'NotifyWebPaymentsFailed' command must not be null!");

            var c = LocationId.       CompareTo(NotifyWebPaymentsFailedCommand.LocationId);

            if (c == 0)
                c = EVSEUId.          CompareTo(NotifyWebPaymentsFailedCommand.EVSEUId);

            if (c == 0 && EVSEId.     HasValue && NotifyWebPaymentsFailedCommand.EVSEId.     HasValue)
                c = EVSEId.     Value.CompareTo(NotifyWebPaymentsFailedCommand.EVSEId.Value);

            if (c == 0 && ConnectorId.HasValue && NotifyWebPaymentsFailedCommand.ConnectorId.HasValue)
                c = ConnectorId.Value.CompareTo(NotifyWebPaymentsFailedCommand.ConnectorId.Value);

            if (c == 0 && ErrorMessage is not null && NotifyWebPaymentsFailedCommand.ErrorMessage is not null)
                c = ErrorMessage.     CompareTo(NotifyWebPaymentsFailedCommand.ErrorMessage);

            if (c == 0 && CustomData   is not null && NotifyWebPaymentsFailedCommand.CustomData   is not null)
                c = CustomData.ToString(Newtonsoft.Json.Formatting.None).
                                      CompareTo(NotifyWebPaymentsFailedCommand.CustomData.ToString(Newtonsoft.Json.Formatting.None));


            if (c == 0)
                c = ResponseURL.      CompareTo(NotifyWebPaymentsFailedCommand.ResponseURL);

            if (c == 0)
                c = Id.               CompareTo(NotifyWebPaymentsFailedCommand.Id);

            if (c == 0)
                c = RequestId.        CompareTo(NotifyWebPaymentsFailedCommand.RequestId);

            if (c == 0)
                c = CorrelationId.    CompareTo(NotifyWebPaymentsFailedCommand.CorrelationId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<NotifyWebPaymentsFailedCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two 'NotifyWebPaymentsFailed' commands for equality.
        /// </summary>
        /// <param name="Object">A 'NotifyWebPaymentsFailed' command to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NotifyWebPaymentsFailedCommand notifyWebPaymentsFailedCommand &&
                   Equals(notifyWebPaymentsFailedCommand);

        #endregion

        #region Equals(NotifyWebPaymentsFailedCommand)

        /// <summary>
        /// Compares two 'NotifyWebPaymentsFailed' commands for equality.
        /// </summary>
        /// <param name="NotifyWebPaymentsFailedCommand">A 'NotifyWebPaymentsFailed' command to compare with.</param>
        public override Boolean Equals(NotifyWebPaymentsFailedCommand? NotifyWebPaymentsFailedCommand)

            => NotifyWebPaymentsFailedCommand is not null &&

               LocationId.   Equals(NotifyWebPaymentsFailedCommand.LocationId)   &&
               EVSEUId.      Equals(NotifyWebPaymentsFailedCommand.EVSEUId)      &&
               EVSEId.       Equals(NotifyWebPaymentsFailedCommand.EVSEId)       &&
               ConnectorId.  Equals(NotifyWebPaymentsFailedCommand.ConnectorId)  &&

             ((ErrorMessage is     null && NotifyWebPaymentsFailedCommand.ErrorMessage is     null) ||
              (ErrorMessage is not null && NotifyWebPaymentsFailedCommand.ErrorMessage is not null && ErrorMessage. Equals(NotifyWebPaymentsFailedCommand.ErrorMessage))) &&

               // CustomData

               ResponseURL.  Equals(NotifyWebPaymentsFailedCommand.ResponseURL)  &&
               Id.           Equals(NotifyWebPaymentsFailedCommand.Id)           &&
               RequestId.    Equals(NotifyWebPaymentsFailedCommand.RequestId)    &&
               CorrelationId.Equals(NotifyWebPaymentsFailedCommand.CorrelationId);

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

                   ErrorMessage is not null
                       ? $", error message: {ErrorMessage}"
                       : null,

                   CustomData is not null
                       ? $", custom data: {CustomData.ToString(Newtonsoft.Json.Formatting.None)}"
                       : "",

                   $" [{Id}] => ",
                   ResponseURL

               );

        #endregion

    }

}
