/*
 * Copyright (c) 2015-2021 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The 'stop session' command.
    /// </summary>
    public readonly struct StopSessionCommand : IEquatable<StopSessionCommand>,
                                                IComparable<StopSessionCommand>,
                                                IComparable
    {

        #region Properties

        /// <summary>
        /// Session identification of the charging session that is requested to be stopped.
        /// </summary>
        [Mandatory]
        public Session_Id  SessionId      { get; }

        /// <summary>
        /// URL that the CommandResult POST should be sent to. This URL might contain an
        /// unique identification to be able to distinguish between 'stop session' command requests.
        /// </summary>
        [Mandatory]
        public URL         ResponseURL    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'stop session' command command.
        /// </summary>
        /// <param name="SessionId">Session identification of the charging session that is requested to be stopped.</param>
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'stop session' command requests.</param>
        public StopSessionCommand(Session_Id  SessionId,
                                  URL         ResponseURL)
        {

            this.SessionId    = SessionId;
            this.ResponseURL  = ResponseURL;

        }

        #endregion


        #region (static) Parse   (JSON, CustomStopSessionCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of a 'stop session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomStopSessionCommandParser">A delegate to parse custom 'stop session' command JSON objects.</param>
        public static StopSessionCommand Parse(JObject                                          JSON,
                                               CustomJObjectParserDelegate<StopSessionCommand>  CustomStopSessionCommandParser   = null)
        {

            if (TryParse(JSON,
                         out StopSessionCommand  stopSessionCommand,
                         out String              ErrorResponse,
                         CustomStopSessionCommandParser))
            {
                return stopSessionCommand;
            }

            throw new ArgumentException("The given JSON representation of a 'stop session' command is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomStopSessionCommandParser = null)

        /// <summary>
        /// Parse the given text representation of a 'stop session' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomStopSessionCommandParser">A delegate to parse custom 'stop session' command JSON objects.</param>
        public static StopSessionCommand Parse(String                                           Text,
                                               CustomJObjectParserDelegate<StopSessionCommand>  CustomStopSessionCommandParser   = null)
        {

            if (TryParse(Text,
                         out StopSessionCommand  stopSessionCommand,
                         out String              ErrorResponse,
                         CustomStopSessionCommandParser))
            {
                return stopSessionCommand;
            }

            throw new ArgumentException("The given text representation of a 'stop session' command is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomStopSessionCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'stop session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomStopSessionCommandParser">A delegate to parse custom 'stop session' command JSON objects.</param>
        public static StopSessionCommand? TryParse(JObject                                          JSON,
                                                   CustomJObjectParserDelegate<StopSessionCommand>  CustomStopSessionCommandParser   = null)
        {

            if (TryParse(JSON,
                         out StopSessionCommand  stopSessionCommand,
                         out String              ErrorResponse,
                         CustomStopSessionCommandParser))
            {
                return stopSessionCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomStopSessionCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'stop session' command.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomStopSessionCommandParser">A delegate to parse custom 'stop session' command JSON objects.</param>
        public static StopSessionCommand? TryParse(String                                           Text,
                                                   CustomJObjectParserDelegate<StopSessionCommand>  CustomStopSessionCommandParser   = null)
        {

            if (TryParse(Text,
                         out StopSessionCommand  stopSessionCommand,
                         out String              ErrorResponse,
                         CustomStopSessionCommandParser))
            {
                return stopSessionCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out StopSessionCommand, out ErrorResponse, CustomStopSessionCommandParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a 'stop session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="StopSessionCommand">The parsed 'stop session' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                 JSON,
                                       out StopSessionCommand  StopSessionCommand,
                                       out String              ErrorResponse)

            => TryParse(JSON,
                        out StopSessionCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a 'stop session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="StopSessionCommand">The parsed 'stop session' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStopSessionCommandParser">A delegate to parse custom 'stop session' command JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       out StopSessionCommand                           StopSessionCommand,
                                       out String                                       ErrorResponse,
                                       CustomJObjectParserDelegate<StopSessionCommand>  CustomStopSessionCommandParser   = null)
        {

            try
            {

                StopSessionCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse SessionId      [mandatory]

                if (!JSON.ParseMandatory("session_id",
                                         "session identification",
                                         Session_Id.TryParse,
                                         out Session_Id SessionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ResponseURL    [mandatory]

                if (!JSON.ParseMandatory("response_url",
                                         "response URL",
                                         URL.TryParse,
                                         out URL ResponseURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                StopSessionCommand = new StopSessionCommand(SessionId,
                                                            ResponseURL);

                if (CustomStopSessionCommandParser != null)
                    StopSessionCommand = CustomStopSessionCommandParser(JSON,
                                                                        StopSessionCommand);

                return true;

            }
            catch (Exception e)
            {
                StopSessionCommand  = default;
                ErrorResponse       = "The given JSON representation of a 'stop session' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out StopSessionCommand, out ErrorResponse, CustomStopSessionCommandParser = null)

        /// <summary>
        /// Try to parse the given text representation of a 'stop session' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="StopSessionCommand">The parsed stopSessionCommand.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStopSessionCommandParser">A delegate to parse custom 'stop session' command JSON objects.</param>
        public static Boolean TryParse(String                                           Text,
                                       out StopSessionCommand                           StopSessionCommand,
                                       out String                                       ErrorResponse,
                                       CustomJObjectParserDelegate<StopSessionCommand>  CustomStopSessionCommandParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out StopSessionCommand,
                                out ErrorResponse,
                                CustomStopSessionCommandParser);

            }
            catch (Exception e)
            {
                StopSessionCommand  = default;
                ErrorResponse       = "The given text representation of a 'stop session' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStopSessionCommandSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStopSessionCommandSerializer">A delegate to serialize custom 'stop session' command JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StopSessionCommand> CustomStopSessionCommandSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("session_id",    SessionId.  ToString()),
                           new JProperty("response_url",  ResponseURL.ToString())
                       );

            return CustomStopSessionCommandSerializer != null
                       ? CustomStopSessionCommandSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">An 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StopSessionCommand StopSessionCommand1,
                                           StopSessionCommand StopSessionCommand2)

            => StopSessionCommand1.Equals(StopSessionCommand2);

        #endregion

        #region Operator != (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">An 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StopSessionCommand StopSessionCommand1,
                                           StopSessionCommand StopSessionCommand2)

            => !(StopSessionCommand1 == StopSessionCommand2);

        #endregion

        #region Operator <  (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">An 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StopSessionCommand StopSessionCommand1,
                                          StopSessionCommand StopSessionCommand2)

            => StopSessionCommand1.CompareTo(StopSessionCommand2) < 0;

        #endregion

        #region Operator <= (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">An 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StopSessionCommand StopSessionCommand1,
                                           StopSessionCommand StopSessionCommand2)

            => !(StopSessionCommand1 > StopSessionCommand2);

        #endregion

        #region Operator >  (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">An 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StopSessionCommand StopSessionCommand1,
                                          StopSessionCommand StopSessionCommand2)

            => StopSessionCommand1.CompareTo(StopSessionCommand2) > 0;

        #endregion

        #region Operator >= (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">An 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StopSessionCommand StopSessionCommand1,
                                           StopSessionCommand StopSessionCommand2)

            => !(StopSessionCommand1 < StopSessionCommand2);

        #endregion

        #endregion

        #region IComparable<StopSessionCommand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is StopSessionCommand stopSessionCommand
                   ? CompareTo(stopSessionCommand)
                   : throw new ArgumentException("The given object is not a 'stop session' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StopSessionCommand)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand">An object to compare with.</param>
        public Int32 CompareTo(StopSessionCommand StopSessionCommand)
        {

            var c = SessionId.  CompareTo(StopSessionCommand.SessionId);

            if (c == 0)
                c = ResponseURL.CompareTo(StopSessionCommand.ResponseURL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<StopSessionCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is StopSessionCommand stopSessionCommand &&
                   Equals(stopSessionCommand);

        #endregion

        #region Equals(StopSessionCommand)

        /// <summary>
        /// Compares two 'stop session' commands for equality.
        /// </summary>
        /// <param name="StopSessionCommand">An 'stop session' command to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(StopSessionCommand StopSessionCommand)

            => SessionId.  Equals(StopSessionCommand.SessionId) &&
               ResponseURL.Equals(StopSessionCommand.ResponseURL);

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

                return SessionId.  GetHashCode() * 3 ^
                       ResponseURL.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(SessionId, " => ",
                             ResponseURL);

        #endregion

    }

}
