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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The 'start session' command.
    /// </summary>
    public class StartSessionCommand : AOCPICommand<StartSessionCommand>
    {

        #region Properties

        /// <summary>
        /// The token the charge point has to use to start a new session.
        /// The Token provided in this request is authorized by the eMSP.
        /// </summary>
        [Mandatory]
        public Token        Token         { get; }

        /// <summary>
        /// Location identification of the location (belonging to the CPO this request is send to)
        /// on which a session is to be started.
        /// </summary>
        [Mandatory]
        public Location_Id  LocationId    { get; }

        /// <summary>
        /// Optional EVSE identification of the EVSE of this location on which a session is to be started.
        /// </summary>
        [Optional]
        public EVSE_UId?    EVSEUId       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'start session' command command.
        /// </summary>
        /// <param name="Token">The token the charge point has to use to start a new session. The Token provided in this request is authorized by the eMSP.</param>
        /// <param name="LocationId">Location identification of the location (belonging to the CPO this request is send to) on which a session is to be started.</param>
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'start session' command requests.</param>
        /// <param name="EVSEUId">Optional EVSE identification of the EVSE of this location if a specific EVSE has to be reserved.</param>
        /// 
        /// <param name="Id">An optional unique identification of the command.</param>
        /// <param name="RequestId">An optional unique request identification.</param>
        /// <param name="CorrelationId">An optional unique request correlation identification.</param>
        public StartSessionCommand(Token            Token,
                                   Location_Id      LocationId,
                                   URL              ResponseURL,
                                   EVSE_UId?        EVSEUId         = null,

                                   Command_Id?      Id              = null,
                                   Request_Id?      RequestId       = null,
                                   Correlation_Id?  CorrelationId   = null)

            : base(ResponseURL,
                   Id,
                   RequestId,
                   CorrelationId)

        {

            this.Token       = Token;
            this.LocationId  = LocationId;
            this.EVSEUId     = EVSEUId;

            unchecked
            {

                hashCode = this.Token.        GetHashCode()       * 17 ^
                           this.LocationId.   GetHashCode()       * 13 ^
                          (this.EVSEUId?.     GetHashCode() ?? 0) * 11 ^

                           this.ResponseURL.  GetHashCode()       *  7 ^
                           this.Id.           GetHashCode()       *  5 ^
                           this.RequestId.    GetHashCode()       *  3 ^
                           this.CorrelationId.GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomStartSessionCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of a 'start session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomStartSessionCommandParser">A delegate to parse custom 'start session' command JSON objects.</param>
        public static StartSessionCommand Parse(JObject                                            JSON,
                                                CountryCode?                                       CountryCodeURL                    = null,
                                                Party_Id?                                          PartyIdURL                        = null,
                                                CustomJObjectParserDelegate<StartSessionCommand>?  CustomStartSessionCommandParser   = null)
        {

            if (TryParse(JSON,
                         out var startSessionCommand,
                         out var errorResponse,
                         CountryCodeURL,
                         PartyIdURL,
                         CustomStartSessionCommandParser))
            {
                return startSessionCommand;
            }

            throw new ArgumentException("The given JSON representation of a 'start session' command is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out StartSessionCommand, out ErrorResponse, CustomStartSessionCommandParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a 'start session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="StartSessionCommand">The parsed 'start session' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out StartSessionCommand?  StartSessionCommand,
                                       [NotNullWhen(false)] out String?               ErrorResponse)

            => TryParse(JSON,
                        out StartSessionCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a 'start session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="StartSessionCommand">The parsed 'start session' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStartSessionCommandParser">A delegate to parse custom 'start session' command JSON objects.</param>
        public static Boolean TryParse(JObject                                            JSON,
                                       [NotNullWhen(true)]  out StartSessionCommand?      StartSessionCommand,
                                       [NotNullWhen(false)] out String?                   ErrorResponse,
                                       CountryCode?                                       CountryCodeURL                    = null,
                                       Party_Id?                                          PartyIdURL                        = null,
                                       CustomJObjectParserDelegate<StartSessionCommand>?  CustomStartSessionCommandParser   = null)
        {

            try
            {

                StartSessionCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Token            [mandatory]

                if (JSON["token"] is not JObject jsonToken)
                {
                    ErrorResponse = "Invalid token!";
                    return false;
                }

                if (!OCPIv2_1_1.Token.TryParse(jsonToken,
                                               out Token? Token,
                                               out ErrorResponse,
                                               CountryCodeURL,
                                               PartyIdURL) ||
                     Token is null)
                {
                    return false;
                }

                //if (!JSON.ParseMandatoryJSON("token",
                //                             "token",
                //                             OCPIv2_1_1.Token.TryParse,
                //                             out Token? Token,
                //                             out ErrorResponse) ||
                //     Token is null)
                //{
                //    return false;
                //}

                #endregion

                #region Parse LocationId       [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id LocationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId          [optional]

                if (JSON.ParseOptional("evse_uid",
                                       "EVSE identification",
                                       EVSE_UId.TryParse,
                                       out EVSE_UId? EVSEUId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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


                StartSessionCommand = new StartSessionCommand(Token,
                                                              LocationId,
                                                              ResponseURL,
                                                              EVSEUId,

                                                              CommandId,
                                                              RequestId,
                                                              CorrelationId);

                if (CustomStartSessionCommandParser is not null)
                    StartSessionCommand = CustomStartSessionCommandParser(JSON,
                                                                          StartSessionCommand);

                return true;

            }
            catch (Exception e)
            {
                StartSessionCommand  = default;
                ErrorResponse        = "The given JSON representation of a 'start session' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomStartSessionCommandSerializer = null, CustomTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomStartSessionCommandSerializer">A delegate to serialize custom 'start session' command JSON objects.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StartSessionCommand>?  CustomStartSessionCommandSerializer   = null,
                              CustomJObjectSerializerDelegate<Token>?                CustomTokenSerializer                 = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("token",         Token.      ToJSON(false,
                                                                                   CustomTokenSerializer)),

                                 new JProperty("location_id",   LocationId. ToString()),

                           EVSEUId.HasValue
                               ? new JProperty("evse_uid",      EVSEUId.    ToString())
                               : null,

                                 new JProperty("response_url",  ResponseURL.ToString())

                       );

            return CustomStartSessionCommandSerializer is not null
                       ? CustomStartSessionCommandSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">A 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StartSessionCommand StartSessionCommand1,
                                           StartSessionCommand StartSessionCommand2)
        {

            if (Object.ReferenceEquals(StartSessionCommand1, StartSessionCommand2))
                return true;

            if (StartSessionCommand1 is null || StartSessionCommand2 is null)
                return false;

            return StartSessionCommand1.Equals(StartSessionCommand2);

        }

        #endregion

        #region Operator != (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">A 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (StartSessionCommand StartSessionCommand1,
                                           StartSessionCommand StartSessionCommand2)

            => !(StartSessionCommand1 == StartSessionCommand2);

        #endregion

        #region Operator <  (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">A 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StartSessionCommand StartSessionCommand1,
                                          StartSessionCommand StartSessionCommand2)

            => StartSessionCommand1 is null
                   ? throw new ArgumentNullException(nameof(StartSessionCommand1), "The given 'start session' command must not be null!")
                   : StartSessionCommand1.CompareTo(StartSessionCommand2) < 0;

        #endregion

        #region Operator <= (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">A 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (StartSessionCommand StartSessionCommand1,
                                           StartSessionCommand StartSessionCommand2)

            => !(StartSessionCommand1 > StartSessionCommand2);

        #endregion

        #region Operator >  (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">A 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StartSessionCommand StartSessionCommand1,
                                          StartSessionCommand StartSessionCommand2)

            => StartSessionCommand1 is null
                   ? throw new ArgumentNullException(nameof(StartSessionCommand1), "The given 'start session' command must not be null!")
                   : StartSessionCommand1.CompareTo(StartSessionCommand2) > 0;

        #endregion

        #region Operator >= (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">A 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (StartSessionCommand StartSessionCommand1,
                                           StartSessionCommand StartSessionCommand2)

            => !(StartSessionCommand1 < StartSessionCommand2);

        #endregion

        #endregion

        #region IComparable<StartSessionCommand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two 'start session' commands.
        /// </summary>
        /// <param name="StartSessionCommand">A 'start session' command to compare with.</param>
        public override Int32 CompareTo(Object? Object)

            => Object is StartSessionCommand startSessionCommand
                   ? CompareTo(startSessionCommand)
                   : throw new ArgumentException("The given object is not a 'start session' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StartSessionCommand)

        /// <summary>
        /// Compares two 'start session' commands.
        /// </summary>
        /// <param name="StartSessionCommand">A 'start session' command to compare with.</param>
        public override Int32 CompareTo(StartSessionCommand? StartSessionCommand)
        {

            if (StartSessionCommand is null)
                throw new ArgumentNullException(nameof(StartSessionCommand), "The given 'start session' command must not be null!");

            var c = Token.        CompareTo(StartSessionCommand.Token);

            if (c == 0)
                c = LocationId.   CompareTo(StartSessionCommand.LocationId);

            if (c == 0)
                c = RequestId.    CompareTo(StartSessionCommand.RequestId);

            if (c == 0)
                c = CorrelationId.CompareTo(StartSessionCommand.CorrelationId);

            if (c == 0)
                c = ResponseURL.  CompareTo(StartSessionCommand.ResponseURL);

            if (c == 0 && EVSEUId.HasValue && StartSessionCommand.EVSEUId.HasValue)
                c = EVSEUId.Value.CompareTo(StartSessionCommand.EVSEUId.Value);


            if (c == 0)
                c = ResponseURL.  CompareTo(StartSessionCommand.ResponseURL);

            if (c == 0)
                c = Id.           CompareTo(StartSessionCommand.Id);

            if (c == 0)
                c = RequestId.    CompareTo(StartSessionCommand.RequestId);

            if (c == 0)
                c = CorrelationId.CompareTo(StartSessionCommand.CorrelationId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<StartSessionCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two 'start session' commands for equality.
        /// </summary>
        /// <param name="Object">A 'start session' command to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is StartSessionCommand startSessionCommand &&
                   Equals(startSessionCommand);

        #endregion

        #region Equals(StartSessionCommand)

        /// <summary>
        /// Compares two 'start session' commands for equality.
        /// </summary>
        /// <param name="StartSessionCommand">A 'start session' command to compare with.</param>
        public override Boolean Equals(StartSessionCommand? StartSessionCommand)

            => StartSessionCommand is not null &&

               Token.        Equals(StartSessionCommand.Token)         &&
               LocationId.   Equals(StartSessionCommand.LocationId)    &&
               EVSEUId.      Equals(StartSessionCommand.EVSEUId)       &&

               ResponseURL.  Equals(StartSessionCommand.ResponseURL)   &&
               Id.           Equals(StartSessionCommand.Id)            &&
               RequestId.    Equals(StartSessionCommand.RequestId)     &&
               CorrelationId.Equals(StartSessionCommand.CorrelationId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Id,
                   ": ",
                   Token,
                   " @ ",
                   LocationId,

                   EVSEUId.HasValue
                       ? ", EVSE UId: " + EVSEUId
                       : "",

                   " => ",
                   ResponseURL

               );

        #endregion

    }

}
