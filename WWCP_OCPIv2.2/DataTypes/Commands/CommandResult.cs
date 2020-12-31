/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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

using System;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// A command result.
    /// </summary>
    public readonly struct CommandResult : IEquatable<CommandResult>,
                                           IComparable<CommandResult>,
                                           IComparable
    {

        #region Properties

        /// <summary>
        /// Result of the command request as sent by the Charge Point to the CPO.
        /// </summary>
        [Mandatory]
        public CommandResultTypes        Result     { get; }

        /// <summary>
        /// Human-readable description of the reason (if one can be provided),
        /// multiple languages can be provided.
        /// </summary>
        [Mandatory]
        public IEnumerable<DisplayText>  Message    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new command result.
        /// </summary>
        /// <param name="Result">Human-readable description of the reason (if one can be provided), multiple languages can be provided.</param>
        /// <param name="Message">Result of the command request as sent by the charge point to the CPO.</param>
        public CommandResult(CommandResultTypes        Result,
                             IEnumerable<DisplayText>  Message)
        {

            this.Result   = Result;
            this.Message  = Message?.Distinct() ?? new DisplayText[0];

        }

        #endregion


        #region (static) Parse   (JSON, CustomCommandResultParser = null)

        /// <summary>
        /// Parse the given JSON representation of a command result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCommandResultParser">A delegate to parse custom command result JSON objects.</param>
        public static CommandResult Parse(JObject                                     JSON,
                                          CustomJObjectParserDelegate<CommandResult>  CustomCommandResultParser   = null)
        {

            if (TryParse(JSON,
                         out CommandResult  commandResult,
                         out String         ErrorResponse,
                         CustomCommandResultParser))
            {
                return commandResult;
            }

            throw new ArgumentException("The given JSON representation of a command result is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomCommandResultParser = null)

        /// <summary>
        /// Parse the given text representation of a command result.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomCommandResultParser">A delegate to parse custom command result JSON objects.</param>
        public static CommandResult Parse(String                                      Text,
                                          CustomJObjectParserDelegate<CommandResult>  CustomCommandResultParser   = null)
        {

            if (TryParse(Text,
                         out CommandResult  commandResult,
                         out String         ErrorResponse,
                         CustomCommandResultParser))
            {
                return commandResult;
            }

            throw new ArgumentException("The given text representation of a command result is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomCommandResultParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a command result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCommandResultParser">A delegate to parse custom command result JSON objects.</param>
        public static CommandResult? TryParse(JObject                                     JSON,
                                              CustomJObjectParserDelegate<CommandResult>  CustomCommandResultParser   = null)
        {

            if (TryParse(JSON,
                         out CommandResult  commandResult,
                         out String         ErrorResponse,
                         CustomCommandResultParser))
            {
                return commandResult;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomCommandResultParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a command result.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomCommandResultParser">A delegate to parse custom command result JSON objects.</param>
        public static CommandResult? TryParse(String                                      Text,
                                              CustomJObjectParserDelegate<CommandResult>  CustomCommandResultParser   = null)
        {

            if (TryParse(Text,
                         out CommandResult  commandResult,
                         out String         ErrorResponse,
                         CustomCommandResultParser))
            {
                return commandResult;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out CommandResult, out ErrorResponse, CustomCommandResultParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a command result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CommandResult">The parsed command result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject            JSON,
                                       out CommandResult  CommandResult,
                                       out String         ErrorResponse)

            => TryParse(JSON,
                        out CommandResult,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a command result.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CommandResult">The parsed command result.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCommandResultParser">A delegate to parse custom command result JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       out CommandResult                           CommandResult,
                                       out String                                  ErrorResponse,
                                       CustomJObjectParserDelegate<CommandResult>  CustomCommandResultParser   = null)
        {

            try
            {

                CommandResult = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Result        [mandatory]

                if (!JSON.ParseMandatoryEnum("result",
                                             "command result",
                                             out CommandResultTypes Result,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Message       [optional]

                if (JSON.ParseOptionalJSON("message",
                                           "message",
                                           DisplayText.TryParse,
                                           out IEnumerable<DisplayText> Message,
                                           out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion


                CommandResult = new CommandResult(Result,
                                                  Message);

                if (CustomCommandResultParser != null)
                    CommandResult = CustomCommandResultParser(JSON,
                                                              CommandResult);

                return true;

            }
            catch (Exception e)
            {
                CommandResult  = default;
                ErrorResponse  = "The given JSON representation of a command result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out CommandResult, out ErrorResponse, CustomCommandResultParser = null)

        /// <summary>
        /// Try to parse the given text representation of a command result.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CommandResult">The parsed commandResult.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCommandResultParser">A delegate to parse custom command result JSON objects.</param>
        public static Boolean TryParse(String                                      Text,
                                       out CommandResult                           CommandResult,
                                       out String                                  ErrorResponse,
                                       CustomJObjectParserDelegate<CommandResult>  CustomCommandResultParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out CommandResult,
                                out ErrorResponse,
                                CustomCommandResultParser);

            }
            catch (Exception e)
            {
                CommandResult  = default;
                ErrorResponse  = "The given text representation of a command result is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCommandResultSerializer = null, CustomDisplayTextSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCommandResultSerializer">A delegate to serialize custom command result JSON objects.</param>
        /// <param name="CustomDisplayTextSerializer">A delegate to serialize custom multi-language text JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CommandResult>  CustomCommandResultSerializer   = null,
                              CustomJObjectSerializerDelegate<DisplayText>    CustomDisplayTextSerializer     = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("result",  Result.ToString()),

                           Message.SafeAny()
                               ? new JProperty("message",  new JArray(Message.Select(message => message.ToJSON(CustomDisplayTextSerializer))))
                               : null

                       );

            return CustomCommandResultSerializer != null
                       ? CustomCommandResultSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CommandResult1, CommandResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResult1">A command result.</param>
        /// <param name="CommandResult2">Another command result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CommandResult CommandResult1,
                                           CommandResult CommandResult2)

            => CommandResult1.Equals(CommandResult2);

        #endregion

        #region Operator != (CommandResult1, CommandResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResult1">A command result.</param>
        /// <param name="CommandResult2">Another command result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CommandResult CommandResult1,
                                           CommandResult CommandResult2)

            => !(CommandResult1 == CommandResult2);

        #endregion

        #region Operator <  (CommandResult1, CommandResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResult1">A command result.</param>
        /// <param name="CommandResult2">Another command result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CommandResult CommandResult1,
                                          CommandResult CommandResult2)

            => CommandResult1.CompareTo(CommandResult2) < 0;

        #endregion

        #region Operator <= (CommandResult1, CommandResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResult1">A command result.</param>
        /// <param name="CommandResult2">Another command result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CommandResult CommandResult1,
                                           CommandResult CommandResult2)

            => !(CommandResult1 > CommandResult2);

        #endregion

        #region Operator >  (CommandResult1, CommandResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResult1">A command result.</param>
        /// <param name="CommandResult2">Another command result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CommandResult CommandResult1,
                                          CommandResult CommandResult2)

            => CommandResult1.CompareTo(CommandResult2) > 0;

        #endregion

        #region Operator >= (CommandResult1, CommandResult2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResult1">A command result.</param>
        /// <param name="CommandResult2">Another command result.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CommandResult CommandResult1,
                                           CommandResult CommandResult2)

            => !(CommandResult1 < CommandResult2);

        #endregion

        #endregion

        #region IComparable<CommandResult> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CommandResult commandResult
                   ? CompareTo(commandResult)
                   : throw new ArgumentException("The given object is not a command result!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CommandResult)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CommandResult">An object to compare with.</param>
        public Int32 CompareTo(CommandResult CommandResult)

            => Result.CompareTo(CommandResult.Result);

        #endregion

        #endregion

        #region IEquatable<CommandResult> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CommandResult commandResult &&
                   Equals(commandResult);

        #endregion

        #region Equals(CommandResult)

        /// <summary>
        /// Compares two command results for equality.
        /// </summary>
        /// <param name="CommandResult">A command result to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CommandResult CommandResult)

            => Result.Equals(CommandResult.Result) &&

               Message.Count().Equals(CommandResult.Message.Count()) &&
               Message.All(message => CommandResult.Message.Contains(message));

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Result. GetHashCode() * 3 ^
                       Message.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Result, " => ",
                             Message.AggregateWith(", "));

        #endregion

    }

}
