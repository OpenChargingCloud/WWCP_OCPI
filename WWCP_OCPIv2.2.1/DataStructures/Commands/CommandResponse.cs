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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// A command response.
    /// </summary>
    public class CommandResponse : IEquatable<CommandResponse>,
                                   IComparable<CommandResponse>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// The optional command leading to this response.
        /// </summary>
        [Optional]
        public IOCPICommand?         Command     { get; }

        /// <summary>
        /// Response from the CPO on the command request.
        /// </summary>
        [Mandatory]
        public CommandResponseTypes  Result      { get; }

        /// <summary>
        /// Timeout for this command in seconds. When the Result is not received within
        /// this timeout, the eMSP can assume that the message might never be send.
        /// </summary>
        [Mandatory]
        public TimeSpan              Timeout     { get; }

        /// <summary>
        /// Human-readable description of the result (if one can be provided),
        /// multiple languages can be provided.
        /// </summary>
        [Mandatory]
        public DisplayTexts          Messages    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new command response.
        /// </summary>
        /// <param name="Result">Response from the CPO on the command request.</param>
        /// <param name="Timeout">Timeout for this command in seconds. When the Result is not received within this timeout, the eMSP can assume that the message might never be send.</param>
        /// <param name="Messages">A human-readable description of the result (if one can be provided), multiple languages can be provided.</param>
        private CommandResponse(CommandResponseTypes  Result,
                                TimeSpan              Timeout,
                                DisplayTexts          Messages)
        {

            this.Result    = Result;
            this.Timeout   = Timeout;
            this.Messages  = Messages ?? DisplayTexts.Empty;

        }


        /// <summary>
        /// Create new command response.
        /// </summary>
        /// <param name="Command">The command leading to this response.</param>
        /// <param name="Result">Response from the CPO on the command request.</param>
        /// <param name="Timeout">Timeout for this command in seconds. When the Result is not received within this timeout, the eMSP can assume that the message might never be send.</param>
        /// <param name="Messages">Human-readable description of the result (if one can be provided), multiple languages can be provided.</param>
        public CommandResponse(IOCPICommand          Command,
                               CommandResponseTypes  Result,
                               TimeSpan              Timeout,
                               DisplayTexts          Messages)
        {

            this.Command   = Command;
            this.Result    = Result;
            this.Timeout   = Timeout;
            this.Messages  = Messages ?? DisplayTexts.Empty;

        }

        #endregion


        #region (static) Parse   (Command, JSON, CustomCommandResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a command response.
        /// </summary>
        /// <param name="Command">The command leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCommandResponseParser">A delegate to parse custom command response JSON objects.</param>
        public static CommandResponse Parse(IOCPICommand                                   Command,
                                            JObject                                        JSON,
                                            CustomJObjectParserDelegate<CommandResponse>?  CustomCommandResponseParser   = null)
        {

            if (TryParse(Command,
                         JSON,
                         out var commandResponse,
                         out var errorResponse,
                         CustomCommandResponseParser))
            {
                return commandResponse;
            }

            throw new ArgumentException("The given JSON representation of a command response is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(Command, JSON, out CommandResponse, out ErrorResponse, CustomCommandResponseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a command response.
        /// </summary>
        /// <param name="Command">The command leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CommandResponse">The parsed command response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(IOCPICommand                               Command,
                                       JObject                                    JSON,
                                       [NotNullWhen(true)]  out CommandResponse?  CommandResponse,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(Command,
                        JSON,
                        out CommandResponse,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a command response.
        /// </summary>
        /// <param name="command">The command leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CommandResponse">The parsed command response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCommandResponseParser">A delegate to parse custom command response JSON objects.</param>
        public static Boolean TryParse(IOCPICommand                                   command,
                                       JObject                                        JSON,
                                       [NotNullWhen(true)]  out CommandResponse?      CommandResponse,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<CommandResponse>?  CustomCommandResponseParser   = null)
        {

            try
            {

                CommandResponse = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Result      [mandatory]

                if (!JSON.ParseMandatoryEnum("result",
                                             "command response",
                                             out CommandResponseTypes result,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Timeout     [mandatory]

                if (!JSON.ParseMandatory("timeout",
                                         "command timeout",
                                         out TimeSpan timeout,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Messages    [optional]

                if (JSON.ParseOptionalJSONArray("message",
                                                "message",
                                                DisplayTexts.TryParse,
                                                out DisplayTexts messages,
                                                out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                CommandResponse = new CommandResponse(
                                      command,
                                      result,
                                      timeout,
                                      messages
                                  );

                if (CustomCommandResponseParser is not null)
                    CommandResponse = CustomCommandResponseParser(JSON,
                                                                  CommandResponse);

                return true;

            }
            catch (Exception e)
            {
                CommandResponse  = default;
                ErrorResponse    = "The given JSON representation of a command response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCommandResponseSerializer = null, CustomDisplayTextSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCommandResponseSerializer">A delegate to serialize custom command response JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CommandResponse>?  CustomCommandResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<DisplayText>?      CustomDisplayTextSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("result",    Result.ToString()),
                                 new JProperty("timeout",   (UInt32) Timeout.TotalSeconds),

                           Messages.SafeAny()
                               ? new JProperty("message",   new JArray(Messages.Select(displayText => displayText.ToJSON(CustomDisplayTextSerializer))))
                               : null

                       );

            return CustomCommandResponseSerializer is not null
                       ? CustomCommandResponseSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static methods

        #region NOT_SUPPORTED   (Timeout, Messages)

        /// <summary>
        /// The requested command is not supported by this CPO, charge point, EVSE etc.
        /// </summary>
        /// <param name="Timeout">Timeout for this command in seconds. When the Result is not received within this timeout, the eMSP can assume that the message might never be send.</param>
        /// <param name="Messages">A human-readable description of the result (if one can be provided), multiple languages can be provided.</param>
        public static CommandResponse NOT_SUPPORTED(TimeSpan      Timeout,
                                                    DisplayTexts  Messages)

            => new (CommandResponseTypes.NOT_SUPPORTED,
                    Timeout,
                    Messages);

        #endregion

        #region REJECTED        (Timeout, Messages)

        /// <summary>
        /// The command was rejected by the CPO,
        /// e.g. because the command is not supported or the command parameters are invalid.
        /// </summary>
        /// <param name="Timeout">Timeout for this command in seconds. When the Result is not received within this timeout, the eMSP can assume that the message might never be send.</param>
        /// <param name="Messages">A human-readable description of the result (if one can be provided), multiple languages can be provided.</param>
        public static CommandResponse REJECTED(TimeSpan      Timeout,
                                               DisplayTexts  Messages)

            => new (CommandResponseTypes.REJECTED,
                    Timeout,
                    Messages);

        #endregion

        #region ACCEPTED        (Timeout, Messages)

        /// <summary>
        /// The command was accepted by the CPO,
        /// but the command execution has not yet started.
        /// </summary>
        /// <param name="Timeout">Timeout for this command in seconds. When the Result is not received within this timeout, the eMSP can assume that the message might never be send.</param>
        /// <param name="Messages">A human-readable description of the result (if one can be provided), multiple languages can be provided.</param>
        public static CommandResponse ACCEPTED(TimeSpan      Timeout,
                                               DisplayTexts  Messages)

            => new (CommandResponseTypes.ACCEPTED,
                    Timeout,
                    Messages);

        #endregion

        #region UNKNOWN_SESSION (Timeout, Messages)

        /// <summary>
        /// The Session in the requested command is not known by this CPO.
        /// </summary>
        /// <param name="Timeout">Timeout for this command in seconds. When the Result is not received within this timeout, the eMSP can assume that the message might never be send.</param>
        /// <param name="Messages">A human-readable description of the result (if one can be provided), multiple languages can be provided.</param>
        public static CommandResponse UNKNOWN_SESSION(TimeSpan      Timeout,
                                                      DisplayTexts  Messages)

            => new (CommandResponseTypes.UNKNOWN_SESSION,
                    Timeout,
                    Messages);

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (CommandResponse1, CommandResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResponse1">A command response.</param>
        /// <param name="CommandResponse2">Another command response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CommandResponse CommandResponse1,
                                           CommandResponse CommandResponse2)
        {

            if (Object.ReferenceEquals(CommandResponse1, CommandResponse2))
                return true;

            if (CommandResponse1 is null || CommandResponse2 is null)
                return false;

            return CommandResponse1.Equals(CommandResponse2);

        }

        #endregion

        #region Operator != (CommandResponse1, CommandResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResponse1">A command response.</param>
        /// <param name="CommandResponse2">Another command response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CommandResponse CommandResponse1,
                                           CommandResponse CommandResponse2)

            => !(CommandResponse1 == CommandResponse2);

        #endregion

        #region Operator <  (CommandResponse1, CommandResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResponse1">A command response.</param>
        /// <param name="CommandResponse2">Another command response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CommandResponse CommandResponse1,
                                          CommandResponse CommandResponse2)

            => CommandResponse1 is null
                   ? throw new ArgumentNullException(nameof(CommandResponse1), "The given command response must not be null!")
                   : CommandResponse1.CompareTo(CommandResponse2) < 0;

        #endregion

        #region Operator <= (CommandResponse1, CommandResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResponse1">A command response.</param>
        /// <param name="CommandResponse2">Another command response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CommandResponse CommandResponse1,
                                           CommandResponse CommandResponse2)

            => !(CommandResponse1 > CommandResponse2);

        #endregion

        #region Operator >  (CommandResponse1, CommandResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResponse1">A command response.</param>
        /// <param name="CommandResponse2">Another command response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CommandResponse CommandResponse1,
                                          CommandResponse CommandResponse2)

            => CommandResponse1 is null
                   ? throw new ArgumentNullException(nameof(CommandResponse1), "The given command response must not be null!")
                   : CommandResponse1.CompareTo(CommandResponse2) > 0;

        #endregion

        #region Operator >= (CommandResponse1, CommandResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResponse1">A command response.</param>
        /// <param name="CommandResponse2">Another command response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CommandResponse CommandResponse1,
                                           CommandResponse CommandResponse2)

            => !(CommandResponse1 < CommandResponse2);

        #endregion

        #endregion

        #region IComparable<CommandResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two command responses.
        /// </summary>
        /// <param name="Object">A command response to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CommandResponse commandResponse
                   ? CompareTo(commandResponse)
                   : throw new ArgumentException("The given object is not a command response!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CommandResponse)

        /// <summary>
        /// Compares two command responses.
        /// </summary>
        /// <param name="CommandResponse">A command response to compare with.</param>
        public Int32 CompareTo(CommandResponse? CommandResponse)
        {

            if (CommandResponse is null)
                throw new ArgumentNullException(nameof(CommandResponse), "The given command response must not be null!");

            var c = Result. CompareTo(CommandResponse.Result);

            if (c == 0)
                c = Timeout.CompareTo(CommandResponse.Timeout);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CommandResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two command responses for equality.
        /// </summary>
        /// <param name="CommandResponse">A command response to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CommandResponse commandResponse &&
                   Equals(commandResponse);

        #endregion

        #region Equals(CommandResponse)

        /// <summary>
        /// Compares two command responses for equality.
        /// </summary>
        /// <param name="CommandResponse">A command response to compare with.</param>
        public Boolean Equals(CommandResponse? CommandResponse)

            => CommandResponse is not null &&

               Result. Equals(CommandResponse.Result)  &&
               Timeout.Equals(CommandResponse.Timeout) &&

               Messages.Count().Equals(CommandResponse.Messages.Count()) &&
               Messages.All(message => CommandResponse.Messages.Contains(message));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Result.  GetHashCode() * 5 ^
                       Timeout. GetHashCode() * 3 ^
                       Messages.CalcHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Result,
                   " after ",
                   Timeout.TotalSeconds,
                   " second(s) => ",
                   Messages.AggregateWith(", ")

               );

        #endregion

    }

}
