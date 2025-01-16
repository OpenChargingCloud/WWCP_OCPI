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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
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
        /// The command leading to this response.
        /// </summary>
        public IOCPICommand              Command    { get; }

        /// <summary>
        /// Response from the CPO on the command request.
        /// </summary>
        [Mandatory]
        public CommandResponseTypes      Result     { get; }

        /// <summary>
        /// Timeout for this command in seconds. When the Result is not received within
        /// this timeout, the eMSP can assume that the message might never be send.
        /// </summary>
        [Mandatory]
        public TimeSpan                  Timeout    { get; }

        /// <summary>
        /// Human-readable description of the result (if one can be provided),
        /// multiple languages can be provided.
        /// </summary>
        [Mandatory]
        public IEnumerable<DisplayText>  Message    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new command response.
        /// </summary>
        /// <param name="Command">The command leading to this response.</param>
        /// <param name="Result">Response from the CPO on the command request.</param>
        /// <param name="Timeout">Timeout for this command in seconds. When the Result is not received within this timeout, the eMSP can assume that the message might never be send.</param>
        /// <param name="Message">Human-readable description of the result (if one can be provided), multiple languages can be provided.</param>
        public CommandResponse(IOCPICommand              Command,
                               CommandResponseTypes      Result,
                               TimeSpan                  Timeout,
                               IEnumerable<DisplayText>  Message)
        {

            this.Command  = Command;
            this.Result   = Result;
            this.Timeout  = Timeout;
            this.Message  = Message?.Distinct() ?? Array.Empty<DisplayText>();

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
        /// <param name="Command">The command leading to this response.</param>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CommandResponse">The parsed command response.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCommandResponseParser">A delegate to parse custom command response JSON objects.</param>
        public static Boolean TryParse(IOCPICommand                                   Command,
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

                #region Parse Result     [mandatory]

                if (!JSON.ParseMandatoryEnum("result",
                                             "command response",
                                             out CommandResponseTypes Result,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Timeout    [mandatory]

                if (!JSON.ParseMandatory("timeout",
                                         "command timeout",
                                         out TimeSpan Timeout,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Message    [optional]

                if (JSON.ParseOptionalJSON("message",
                                           "message",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Message,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                CommandResponse = new CommandResponse(Command,
                                                      Result,
                                                      Timeout,
                                                      Message);

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

                                 new JProperty("result",   Result.ToString()),
                                 new JProperty("timeout",  (UInt32) Timeout.TotalSeconds),

                           Message.SafeAny()
                               ? new JProperty("message",  new JArray(Message.Select(displayText => displayText.ToJSON(CustomDisplayTextSerializer))))
                               : null

                       );

            return CustomCommandResponseSerializer is not null
                       ? CustomCommandResponseSerializer(this, json)
                       : json;

        }

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

               Message.Count().Equals(CommandResponse.Message.Count()) &&
               Message.All(message => CommandResponse.Message.Contains(message));

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

                return Result. GetHashCode() * 5 ^
                       Timeout.GetHashCode() * 3 ^
                       Message.CalcHashCode();

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
                   Message.AggregateWith(", ")

               );

        #endregion

    }

}
