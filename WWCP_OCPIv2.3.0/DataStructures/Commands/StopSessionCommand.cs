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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The 'stop session' command.
    /// </summary>
    public class StopSessionCommand : AOCPICommand<StopSessionCommand>
    {

        #region Properties

        /// <summary>
        /// Session identification of the charging session that is requested to be stopped.
        /// </summary>
        [Mandatory]
        public Session_Id  SessionId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'stop session' command command.
        /// </summary>
        /// <param name="SessionId">Session identification of the charging session that is requested to be stopped.</param>
        /// 
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'stop session' command requests.</param>
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        public StopSessionCommand(Session_Id       SessionId,

                                  URL              ResponseURL,
                                  Command_Id?      Id              = null,
                                  Request_Id?      RequestId       = null,
                                  Correlation_Id?  CorrelationId   = null)

            : base(ResponseURL,
                   Id,
                   RequestId,
                   CorrelationId)

        {

            this.SessionId  = SessionId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomStopSessionCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of a 'stop session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomStopSessionCommandParser">A delegate to parse custom 'stop session' command JSON objects.</param>
        public static StopSessionCommand Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<StopSessionCommand>?  CustomStopSessionCommandParser   = null)
        {

            if (TryParse(JSON,
                         out var stopSessionCommand,
                         out var errorResponse,
                         CustomStopSessionCommandParser))
            {
                return stopSessionCommand;
            }

            throw new ArgumentException("The given JSON representation of a 'stop session' command is invalid: " + errorResponse,
                                        nameof(JSON));

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
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out StopSessionCommand?  StopSessionCommand,
                                       [NotNullWhen(false)] out String?              ErrorResponse)

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
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out StopSessionCommand?      StopSessionCommand,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<StopSessionCommand>?  CustomStopSessionCommandParser   = null)
        {

            try
            {

                StopSessionCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse SessionId        [mandatory]

                if (!JSON.ParseMandatory("session_id",
                                         "session identification",
                                         Session_Id.TryParse,
                                         out Session_Id SessionId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                #region Parse ResponseURL      [mandatory]

                if (!JSON.ParseMandatory("response_url",
                                         "response URL",
                                         URL.TryParse,
                                         out URL ResponseURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CommandId        [optional, internal]

                if (JSON.ParseOptional("id",
                                       "command identification",
                                       Command_Id.TryParse,
                                       out Command_Id? CommandId,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse RequestId        [optional, internal]

                if (JSON.ParseOptional("request_id",
                                       "request identification",
                                       Request_Id.TryParse,
                                       out Request_Id? RequestId,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CorrelationId    [optional, internal]

                if (JSON.ParseOptional("correlation_Id",
                                       "correlation identification",
                                       Correlation_Id.TryParse,
                                       out Correlation_Id? CorrelationId,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                StopSessionCommand = new StopSessionCommand(SessionId,

                                                            ResponseURL,
                                                            CommandId,
                                                            RequestId,
                                                            CorrelationId);

                if (CustomStopSessionCommandParser is not null)
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

        #region ToJSON(CustomStopSessionCommandSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStopSessionCommandSerializer">A delegate to serialize custom 'stop session' command JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StopSessionCommand>? CustomStopSessionCommandSerializer = null)
        {

            var json = JSONObject.Create(
                           new JProperty("session_id",    SessionId.  ToString()),
                           new JProperty("response_url",  ResponseURL.ToString())
                       );

            return CustomStopSessionCommandSerializer is not null
                       ? CustomStopSessionCommandSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">A 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StopSessionCommand StopSessionCommand1,
                                           StopSessionCommand StopSessionCommand2)
        {

            if (Object.ReferenceEquals(StopSessionCommand1, StopSessionCommand2))
                return true;

            if (StopSessionCommand1 is null || StopSessionCommand2 is null)
                return false;

            return StopSessionCommand1.Equals(StopSessionCommand2);

        }

        #endregion

        #region Operator != (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">A 'stop session' command.</param>
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
        /// <param name="StopSessionCommand1">A 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StopSessionCommand StopSessionCommand1,
                                          StopSessionCommand StopSessionCommand2)

            => StopSessionCommand1 is null
                   ? throw new ArgumentNullException(nameof(StopSessionCommand1), "The given 'stop session' command must not be null!")
                   : StopSessionCommand1.CompareTo(StopSessionCommand2) > 0;

        #endregion

        #region Operator <= (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">A 'stop session' command.</param>
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
        /// <param name="StopSessionCommand1">A 'stop session' command.</param>
        /// <param name="StopSessionCommand2">Another 'stop session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StopSessionCommand StopSessionCommand1,
                                          StopSessionCommand StopSessionCommand2)

            => StopSessionCommand1 is null
                   ? throw new ArgumentNullException(nameof(StopSessionCommand1), "The given 'stop session' command must not be null!")
                   : StopSessionCommand1.CompareTo(StopSessionCommand2) < 0;

        #endregion

        #region Operator >= (StopSessionCommand1, StopSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StopSessionCommand1">A 'stop session' command.</param>
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
        /// Compares two 'stop session' commands.
        /// </summary>
        /// <param name="StopSessionCommand">A 'stop session' command to compare with.</param>
        public override Int32 CompareTo(Object? Object)

            => Object is StopSessionCommand stopSessionCommand
                   ? CompareTo(stopSessionCommand)
                   : throw new ArgumentException("The given object is not a 'stop session' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StopSessionCommand)

        /// <summary>
        /// Compares two 'stop session' commands.
        /// </summary>
        /// <param name="StopSessionCommand">A 'stop session' command to compare with.</param>
        public override Int32 CompareTo(StopSessionCommand? StopSessionCommand)
        {

            if (StopSessionCommand is null)
                throw new ArgumentNullException(nameof(StopSessionCommand), "The given 'stop session' command must not be null!");

            var c = SessionId.    CompareTo(StopSessionCommand.SessionId);


            if (c == 0)
                c = ResponseURL.  CompareTo(StopSessionCommand.ResponseURL);

            if (c == 0)
                c = Id.           CompareTo(StopSessionCommand.Id);

            if (c == 0)
                c = RequestId.    CompareTo(StopSessionCommand.RequestId);

            if (c == 0)
                c = CorrelationId.CompareTo(StopSessionCommand.CorrelationId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<StopSessionCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two 'stop session' commands for equality.
        /// </summary>
        /// <param name="StopSessionCommand">A 'stop session' command to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StopSessionCommand stopSessionCommand &&
                   Equals(stopSessionCommand);

        #endregion

        #region Equals(StopSessionCommand)

        /// <summary>
        /// Compares two 'stop session' commands for equality.
        /// </summary>
        /// <param name="StopSessionCommand">A 'stop session' command to compare with.</param>
        public override Boolean Equals(StopSessionCommand? StopSessionCommand)

            => StopSessionCommand is not null &&

               SessionId.    Equals(StopSessionCommand.SessionId)     &&

               ResponseURL.  Equals(StopSessionCommand.ResponseURL)   &&
               Id.           Equals(StopSessionCommand.Id)            &&
               RequestId.    Equals(StopSessionCommand.RequestId)     &&
               CorrelationId.Equals(StopSessionCommand.CorrelationId);

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

                return SessionId.    GetHashCode() * 11 ^

                       ResponseURL.  GetHashCode() *  7 ^
                       Id.           GetHashCode() *  5 ^
                       RequestId.    GetHashCode() *  3 ^
                       CorrelationId.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,
                   ": ",
                   SessionId,
                   " => ",
                   ResponseURL

               );

        #endregion

    }

}
