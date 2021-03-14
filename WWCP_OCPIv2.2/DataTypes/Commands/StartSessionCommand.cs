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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The 'start session' command.
    /// </summary>
    public readonly struct StartSessionCommand : IEquatable<StartSessionCommand>,
                                                 IComparable<StartSessionCommand>,
                                                 IComparable
    {

        #region Properties

        /// <summary>
        /// The token the charge point has to use to start a new session.
        /// The Token provided in this request is authorized by the eMSP.
        /// </summary>
        [Mandatory]
        public Token                    Token                     { get; }

        /// <summary>
        /// Location identification of the location (belonging to the CPO this request is send to)
        /// on which a session is to be started.
        /// </summary>
        [Mandatory]
        public Location_Id              LocationId                { get; }

        /// <summary>
        /// Optional EVSE identification of the EVSE of this location on which a session is to be started.
        /// </summary>
        [Mandatory]
        public EVSE_UId?                EVSEUId                   { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP, when given, this reference will be provided
        /// in the relevant session and/or CDR.
        /// </summary>
        [Mandatory]
        public AuthorizationReference?  AuthorizationReference    { get; }

        /// <summary>
        /// URL that the CommandResult POST should be sent to. This URL might contain an
        /// unique identification to be able to distinguish between 'start session'
        /// command requests.
        /// </summary>
        [Mandatory]
        public URL                      ResponseURL               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'start session' command command.
        /// </summary>
        /// <param name="Token">The token the charge point has to use to start a new session. The Token provided in this request is authorized by the eMSP.</param>
        /// <param name="LocationId">Location identification of the location (belonging to the CPO this request is send to) on which a session is to be started.</param>
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'start session' command requests.</param>
        /// 
        /// <param name="EVSEUId">Optional EVSE identification of the EVSE of this location if a specific EVSE has to be reserved.</param>
        /// <param name="AuthorizationReference">Optional reference to the authorization given by the eMSP, when given, this reference will be provided in the relevant session and/or CDR.</param>
        public StartSessionCommand(Token                    Token,
                                   Location_Id              LocationId,
                                   URL                      ResponseURL,

                                   EVSE_UId?                EVSEUId                  = null,
                                   AuthorizationReference?  AuthorizationReference   = null)
        {

            this.Token                   = Token;
            this.LocationId              = LocationId;
            this.ResponseURL             = ResponseURL;

            this.EVSEUId                 = EVSEUId;
            this.AuthorizationReference  = AuthorizationReference;

        }

        #endregion


        #region (static) Parse   (JSON, CustomStartSessionCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of an 'start session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomStartSessionCommandParser">A delegate to parse custom 'start session' command JSON objects.</param>
        public static StartSessionCommand Parse(JObject                                           JSON,
                                                CustomJObjectParserDelegate<StartSessionCommand>  CustomStartSessionCommandParser   = null)
        {

            if (TryParse(JSON,
                         out StartSessionCommand  unlockConnectorCommand,
                         out String               ErrorResponse,
                         CustomStartSessionCommandParser))
            {
                return unlockConnectorCommand;
            }

            throw new ArgumentException("The given JSON representation of an 'start session' command is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomStartSessionCommandParser = null)

        /// <summary>
        /// Parse the given text representation of an 'start session' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomStartSessionCommandParser">A delegate to parse custom 'start session' command JSON objects.</param>
        public static StartSessionCommand Parse(String                                            Text,
                                                CustomJObjectParserDelegate<StartSessionCommand>  CustomStartSessionCommandParser   = null)
        {

            if (TryParse(Text,
                         out StartSessionCommand  unlockConnectorCommand,
                         out String               ErrorResponse,
                         CustomStartSessionCommandParser))
            {
                return unlockConnectorCommand;
            }

            throw new ArgumentException("The given text representation of an 'start session' command is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomStartSessionCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'start session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomStartSessionCommandParser">A delegate to parse custom 'start session' command JSON objects.</param>
        public static StartSessionCommand? TryParse(JObject                                           JSON,
                                                    CustomJObjectParserDelegate<StartSessionCommand>  CustomStartSessionCommandParser   = null)
        {

            if (TryParse(JSON,
                         out StartSessionCommand  unlockConnectorCommand,
                         out String               ErrorResponse,
                         CustomStartSessionCommandParser))
            {
                return unlockConnectorCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomStartSessionCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'start session' command.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomStartSessionCommandParser">A delegate to parse custom 'start session' command JSON objects.</param>
        public static StartSessionCommand? TryParse(String                                            Text,
                                                    CustomJObjectParserDelegate<StartSessionCommand>  CustomStartSessionCommandParser   = null)
        {

            if (TryParse(Text,
                         out StartSessionCommand  unlockConnectorCommand,
                         out String               ErrorResponse,
                         CustomStartSessionCommandParser))
            {
                return unlockConnectorCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out StartSessionCommand, out ErrorResponse, CustomStartSessionCommandParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an 'start session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="StartSessionCommand">The parsed 'start session' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                  JSON,
                                       out StartSessionCommand  StartSessionCommand,
                                       out String               ErrorResponse)

            => TryParse(JSON,
                        out StartSessionCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an 'start session' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="StartSessionCommand">The parsed 'start session' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStartSessionCommandParser">A delegate to parse custom 'start session' command JSON objects.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       out StartSessionCommand                           StartSessionCommand,
                                       out String                                        ErrorResponse,
                                       CustomJObjectParserDelegate<StartSessionCommand>  CustomStartSessionCommandParser   = null)
        {

            try
            {

                StartSessionCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Token                     [mandatory]

                if (!JSON.ParseMandatoryJSON2("token",
                                              "token",
                                              OCPIv2_2.Token.TryParse,
                                              out Token Token,
                                              out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse LocationId                [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id LocationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId                   [optional]

                if (JSON.ParseOptional("evse_uid",
                                       "EVSE identification",
                                       EVSE_UId.TryParse,
                                       out EVSE_UId? EVSEUId,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse AuthorizationReference    [optional]

                if (JSON.ParseOptional("authorization_reference",
                                       "authorization reference",
                                       OCPIv2_2.AuthorizationReference.TryParse,
                                       out AuthorizationReference? AuthorizationReference,
                                       out ErrorResponse))
                {
                    if (ErrorResponse != null)
                        return false;
                }

                #endregion

                #region Parse ResponseURL               [mandatory]

                if (!JSON.ParseMandatory("response_url",
                                         "response URL",
                                         URL.TryParse,
                                         out URL ResponseURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                StartSessionCommand = new StartSessionCommand(Token,
                                                              LocationId,
                                                              ResponseURL,

                                                              EVSEUId,
                                                              AuthorizationReference);

                if (CustomStartSessionCommandParser != null)
                    StartSessionCommand = CustomStartSessionCommandParser(JSON,
                                                                          StartSessionCommand);

                return true;

            }
            catch (Exception e)
            {
                StartSessionCommand  = default;
                ErrorResponse        = "The given JSON representation of an 'start session' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out StartSessionCommand, out ErrorResponse, CustomStartSessionCommandParser = null)

        /// <summary>
        /// Try to parse the given text representation of an 'start session' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="StartSessionCommand">The parsed unlockConnectorCommand.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomStartSessionCommandParser">A delegate to parse custom 'start session' command JSON objects.</param>
        public static Boolean TryParse(String                                            Text,
                                       out StartSessionCommand                           StartSessionCommand,
                                       out String                                        ErrorResponse,
                                       CustomJObjectParserDelegate<StartSessionCommand>  CustomStartSessionCommandParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out StartSessionCommand,
                                out ErrorResponse,
                                CustomStartSessionCommandParser);

            }
            catch (Exception e)
            {
                StartSessionCommand  = default;
                ErrorResponse        = "The given text representation of an 'start session' command is invalid: " + e.Message;
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
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<StartSessionCommand>  CustomStartSessionCommandSerializer   = null,
                              CustomJObjectSerializerDelegate<Token>                CustomTokenSerializer                 = null,
                              CustomJObjectSerializerDelegate<EnergyContract>       CustomEnergyContractSerializer        = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("token",                          Token.                 ToJSON(CustomTokenSerializer,
                                                                                                         CustomEnergyContractSerializer)),
                           new JProperty("location_id",                    LocationId.            ToString()),

                           EVSEUId.HasValue
                               ? new JProperty("evse_uid",                 EVSEUId.               ToString())
                               : null,

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",  AuthorizationReference.ToString())
                               : null,

                           new JProperty("response_url",                   ResponseURL.           ToString())

                       );

            return CustomStartSessionCommandSerializer != null
                       ? CustomStartSessionCommandSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">An 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (StartSessionCommand StartSessionCommand1,
                                           StartSessionCommand StartSessionCommand2)

            => StartSessionCommand1.Equals(StartSessionCommand2);

        #endregion

        #region Operator != (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">An 'start session' command.</param>
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
        /// <param name="StartSessionCommand1">An 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (StartSessionCommand StartSessionCommand1,
                                          StartSessionCommand StartSessionCommand2)

            => StartSessionCommand1.CompareTo(StartSessionCommand2) < 0;

        #endregion

        #region Operator <= (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">An 'start session' command.</param>
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
        /// <param name="StartSessionCommand1">An 'start session' command.</param>
        /// <param name="StartSessionCommand2">Another 'start session' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (StartSessionCommand StartSessionCommand1,
                                          StartSessionCommand StartSessionCommand2)

            => StartSessionCommand1.CompareTo(StartSessionCommand2) > 0;

        #endregion

        #region Operator >= (StartSessionCommand1, StartSessionCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand1">An 'start session' command.</param>
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
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is StartSessionCommand unlockConnectorCommand
                   ? CompareTo(unlockConnectorCommand)
                   : throw new ArgumentException("The given object is not an 'start session' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(StartSessionCommand)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="StartSessionCommand">An object to compare with.</param>
        public Int32 CompareTo(StartSessionCommand StartSessionCommand)
        {

            var c = Token.     CompareTo(StartSessionCommand.Token);

            if (c == 0)
                c = LocationId.CompareTo(StartSessionCommand.LocationId);

            if (c == 0 && EVSEUId.HasValue && StartSessionCommand.EVSEUId.HasValue)
                c = EVSEUId.Value.CompareTo(StartSessionCommand.EVSEUId.Value);

            if (c == 0 && AuthorizationReference.HasValue && StartSessionCommand.AuthorizationReference.HasValue)
                c = AuthorizationReference.Value.CompareTo(StartSessionCommand.AuthorizationReference.Value);

            if (c == 0)
                c = ResponseURL.CompareTo(StartSessionCommand.ResponseURL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<StartSessionCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is StartSessionCommand unlockConnectorCommand &&
                   Equals(unlockConnectorCommand);

        #endregion

        #region Equals(StartSessionCommand)

        /// <summary>
        /// Compares two 'start session' commands for equality.
        /// </summary>
        /// <param name="StartSessionCommand">An 'start session' command to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(StartSessionCommand StartSessionCommand)

            => Token.                 Equals(StartSessionCommand.Token)                  &&
               LocationId.            Equals(StartSessionCommand.LocationId)             &&
               EVSEUId.               Equals(StartSessionCommand.EVSEUId)                &&
               AuthorizationReference.Equals(StartSessionCommand.AuthorizationReference) &&
               ResponseURL.           Equals(StartSessionCommand.ResponseURL);

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

                return Token.     GetHashCode() * 11 ^
                       LocationId.GetHashCode() *  7 ^

                       (EVSEUId.HasValue
                            ? EVSEUId.GetHashCode()
                            : 0) * 5 ^

                       (AuthorizationReference.HasValue
                            ? AuthorizationReference.GetHashCode()
                            : 0) * 3 ^

                       ResponseURL.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Token, " / ", LocationId,
                             EVSEUId.               HasValue ? " / " + EVSEUId : "",
                             AuthorizationReference.HasValue ? " / " + AuthorizationReference : "",
                             " => ",
                             ResponseURL);

        #endregion

    }

}
