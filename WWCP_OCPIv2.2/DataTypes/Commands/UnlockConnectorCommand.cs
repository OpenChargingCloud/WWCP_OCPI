/*
 * Copyright (c) 2015-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// The 'unlock connector' command.
    /// </summary>
    public class UnlockConnectorCommand : AOCPICommand<UnlockConnectorCommand>
    {

        #region Properties

        /// <summary>
        /// Location identification of the location (belonging to the CPO this request is send to)
        /// of which it is requested to unlock the connector.
        /// </summary>
        [Mandatory]
        public Location_Id   LocationId      { get; }

        /// <summary>
        /// EVSE identification of the EVSE of this location of which it is requested to unlock the connector.
        /// </summary>
        [Mandatory]
        public EVSE_UId      EVSEUId         { get; }

        /// <summary>
        /// Connector identification of the connector of this location of which it is requested to unlock.
        /// </summary>
        [Mandatory]
        public Connector_Id  ConnectorId     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'unlock connector' command command.
        /// </summary>
        /// <param name="LocationId">Location identification of the location (belonging to the CPO this request is send to) of which it is requested to unlock the connector.</param>
        /// <param name="EVSEUId">EVSE identification of the EVSE of this location of which it is requested to unlock the connector.</param>
        /// <param name="ConnectorId">Connector identification of the connector of this location of which it is requested to unlock.</param>
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'unlock connector' command requests.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        public UnlockConnectorCommand(Location_Id      LocationId,
                                      EVSE_UId         EVSEUId,
                                      Connector_Id     ConnectorId,
                                      URL              ResponseURL,

                                      Request_Id?      RequestId       = null,
                                      Correlation_Id?  CorrelationId   = null)

            : base(ResponseURL,
                   RequestId,
                   CorrelationId)

        {

            this.LocationId   = LocationId;
            this.EVSEUId      = EVSEUId;
            this.ConnectorId  = ConnectorId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomUnlockConnectorCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of an 'unlock connector' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomUnlockConnectorCommandParser">A delegate to parse custom 'unlock connector' command JSON objects.</param>
        public static UnlockConnectorCommand Parse(JObject                                              JSON,
                                                   CustomJObjectParserDelegate<UnlockConnectorCommand>  CustomUnlockConnectorCommandParser   = null)
        {

            if (TryParse(JSON,
                         out UnlockConnectorCommand  unlockConnectorCommand,
                         out String                  ErrorResponse,
                         CustomUnlockConnectorCommandParser))
            {
                return unlockConnectorCommand;
            }

            throw new ArgumentException("The given JSON representation of an 'unlock connector' command is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomUnlockConnectorCommandParser = null)

        /// <summary>
        /// Parse the given text representation of an 'unlock connector' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomUnlockConnectorCommandParser">A delegate to parse custom 'unlock connector' command JSON objects.</param>
        public static UnlockConnectorCommand Parse(String                                               Text,
                                                   CustomJObjectParserDelegate<UnlockConnectorCommand>  CustomUnlockConnectorCommandParser   = null)
        {

            if (TryParse(Text,
                         out UnlockConnectorCommand  unlockConnectorCommand,
                         out String                  ErrorResponse,
                         CustomUnlockConnectorCommandParser))
            {
                return unlockConnectorCommand;
            }

            throw new ArgumentException("The given text representation of an 'unlock connector' command is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomUnlockConnectorCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'unlock connector' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomUnlockConnectorCommandParser">A delegate to parse custom 'unlock connector' command JSON objects.</param>
        public static UnlockConnectorCommand? TryParse(JObject                                              JSON,
                                                       CustomJObjectParserDelegate<UnlockConnectorCommand>  CustomUnlockConnectorCommandParser   = null)
        {

            if (TryParse(JSON,
                         out UnlockConnectorCommand  unlockConnectorCommand,
                         out String                  ErrorResponse,
                         CustomUnlockConnectorCommandParser))
            {
                return unlockConnectorCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomUnlockConnectorCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'unlock connector' command.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomUnlockConnectorCommandParser">A delegate to parse custom 'unlock connector' command JSON objects.</param>
        public static UnlockConnectorCommand? TryParse(String                                               Text,
                                                       CustomJObjectParserDelegate<UnlockConnectorCommand>  CustomUnlockConnectorCommandParser   = null)
        {

            if (TryParse(Text,
                         out UnlockConnectorCommand  unlockConnectorCommand,
                         out String                  ErrorResponse,
                         CustomUnlockConnectorCommandParser))
            {
                return unlockConnectorCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out UnlockConnectorCommand, out ErrorResponse, CustomUnlockConnectorCommandParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an 'unlock connector' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="UnlockConnectorCommand">The parsed 'unlock connector' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                     JSON,
                                       out UnlockConnectorCommand  UnlockConnectorCommand,
                                       out String                  ErrorResponse)

            => TryParse(JSON,
                        out UnlockConnectorCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an 'unlock connector' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="UnlockConnectorCommand">The parsed 'unlock connector' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorCommandParser">A delegate to parse custom 'unlock connector' command JSON objects.</param>
        public static Boolean TryParse(JObject                                              JSON,
                                       out UnlockConnectorCommand                           UnlockConnectorCommand,
                                       out String                                           ErrorResponse,
                                       CustomJObjectParserDelegate<UnlockConnectorCommand>  CustomUnlockConnectorCommandParser   = null)
        {

            try
            {

                UnlockConnectorCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse LocationId        [mandatory]

                if (!JSON.ParseMandatory("location_id",
                                         "location identification",
                                         Location_Id.TryParse,
                                         out Location_Id LocationId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EVSEUId           [mandatory]

                if (!JSON.ParseMandatory("evse_uid",
                                         "EVSE identification",
                                         EVSE_UId.TryParse,
                                         out EVSE_UId EVSEUId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ConnectorId       [mandatory]

                if (!JSON.ParseMandatory("connector_id",
                                         "connector identification",
                                         Connector_Id.TryParse,
                                         out Connector_Id ConnectorId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ResponseURL       [mandatory]

                if (!JSON.ParseMandatory("response_url",
                                         "response URL",
                                         URL.TryParse,
                                         out URL ResponseURL,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                UnlockConnectorCommand = new UnlockConnectorCommand(LocationId,
                                                                    EVSEUId,
                                                                    ConnectorId,
                                                                    ResponseURL);

                if (CustomUnlockConnectorCommandParser is not null)
                    UnlockConnectorCommand = CustomUnlockConnectorCommandParser(JSON,
                                                                                UnlockConnectorCommand);

                return true;

            }
            catch (Exception e)
            {
                UnlockConnectorCommand  = default;
                ErrorResponse           = "The given JSON representation of an 'unlock connector' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out UnlockConnectorCommand, out ErrorResponse, CustomUnlockConnectorCommandParser = null)

        /// <summary>
        /// Try to parse the given text representation of an 'unlock connector' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="UnlockConnectorCommand">The parsed unlockConnectorCommand.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnlockConnectorCommandParser">A delegate to parse custom 'unlock connector' command JSON objects.</param>
        public static Boolean TryParse(String                                               Text,
                                       out UnlockConnectorCommand                           UnlockConnectorCommand,
                                       out String                                           ErrorResponse,
                                       CustomJObjectParserDelegate<UnlockConnectorCommand>  CustomUnlockConnectorCommandParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out UnlockConnectorCommand,
                                out ErrorResponse,
                                CustomUnlockConnectorCommandParser);

            }
            catch (Exception e)
            {
                UnlockConnectorCommand  = default;
                ErrorResponse           = "The given text representation of an 'unlock connector' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnlockConnectorCommandSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnlockConnectorCommandSerializer">A delegate to serialize custom 'unlock connector' command JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnlockConnectorCommand> CustomUnlockConnectorCommandSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("location_id",   LocationId. ToString()),
                           new JProperty("evse_uid",      EVSEUId.    ToString()),
                           new JProperty("connector_id",  ConnectorId.ToString()),
                           new JProperty("response_url",  ResponseURL.ToString())
                       );

            return CustomUnlockConnectorCommandSerializer is not null
                       ? CustomUnlockConnectorCommandSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UnlockConnectorCommand1, UnlockConnectorCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorCommand1">An 'unlock connector' command.</param>
        /// <param name="UnlockConnectorCommand2">Another 'unlock connector' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UnlockConnectorCommand UnlockConnectorCommand1,
                                           UnlockConnectorCommand UnlockConnectorCommand2)
        {

            if (Object.ReferenceEquals(UnlockConnectorCommand1, UnlockConnectorCommand2))
                return true;

            if (UnlockConnectorCommand1 is null || UnlockConnectorCommand2 is null)
                return false;

            return UnlockConnectorCommand1.Equals(UnlockConnectorCommand2);

        }

        #endregion

        #region Operator != (UnlockConnectorCommand1, UnlockConnectorCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorCommand1">An 'unlock connector' command.</param>
        /// <param name="UnlockConnectorCommand2">Another 'unlock connector' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UnlockConnectorCommand UnlockConnectorCommand1,
                                           UnlockConnectorCommand UnlockConnectorCommand2)

            => !(UnlockConnectorCommand1 == UnlockConnectorCommand2);

        #endregion

        #region Operator <  (UnlockConnectorCommand1, UnlockConnectorCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorCommand1">An 'unlock connector' command.</param>
        /// <param name="UnlockConnectorCommand2">Another 'unlock connector' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (UnlockConnectorCommand UnlockConnectorCommand1,
                                          UnlockConnectorCommand UnlockConnectorCommand2)

            => UnlockConnectorCommand1 is null
                   ? throw new ArgumentNullException(nameof(UnlockConnectorCommand1), "The given 'unlock connector' command must not be null!")
                   : UnlockConnectorCommand1.CompareTo(UnlockConnectorCommand2) < 0;

        #endregion

        #region Operator <= (UnlockConnectorCommand1, UnlockConnectorCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorCommand1">An 'unlock connector' command.</param>
        /// <param name="UnlockConnectorCommand2">Another 'unlock connector' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (UnlockConnectorCommand UnlockConnectorCommand1,
                                           UnlockConnectorCommand UnlockConnectorCommand2)

            => !(UnlockConnectorCommand1 > UnlockConnectorCommand2);

        #endregion

        #region Operator >  (UnlockConnectorCommand1, UnlockConnectorCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorCommand1">An 'unlock connector' command.</param>
        /// <param name="UnlockConnectorCommand2">Another 'unlock connector' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (UnlockConnectorCommand UnlockConnectorCommand1,
                                          UnlockConnectorCommand UnlockConnectorCommand2)

            => UnlockConnectorCommand1 is null
                   ? throw new ArgumentNullException(nameof(UnlockConnectorCommand1), "The given 'unlock connector' command must not be null!")
                   : UnlockConnectorCommand1.CompareTo(UnlockConnectorCommand2) > 0;

        #endregion

        #region Operator >= (UnlockConnectorCommand1, UnlockConnectorCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorCommand1">An 'unlock connector' command.</param>
        /// <param name="UnlockConnectorCommand2">Another 'unlock connector' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (UnlockConnectorCommand UnlockConnectorCommand1,
                                           UnlockConnectorCommand UnlockConnectorCommand2)

            => !(UnlockConnectorCommand1 < UnlockConnectorCommand2);

        #endregion

        #endregion

        #region IComparable<UnlockConnectorCommand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is UnlockConnectorCommand unlockConnectorCommand
                   ? CompareTo(unlockConnectorCommand)
                   : throw new ArgumentException("The given object is not an 'unlock connector' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(UnlockConnectorCommand)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnlockConnectorCommand">An object to compare with.</param>
        public override Int32 CompareTo(UnlockConnectorCommand UnlockConnectorCommand)
        {

            if (UnlockConnectorCommand is null)
                throw new ArgumentNullException(nameof(UnlockConnectorCommand), "The given 'unlock connector' command must not be null!");

            var c = LocationId. CompareTo(UnlockConnectorCommand.LocationId);

            if (c == 0)
                c = EVSEUId.    CompareTo(UnlockConnectorCommand.EVSEUId);

            if (c == 0)
                c = ConnectorId.CompareTo(UnlockConnectorCommand.ConnectorId);

            if (c == 0)
                c = ResponseURL.CompareTo(UnlockConnectorCommand.ResponseURL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<UnlockConnectorCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is UnlockConnectorCommand unlockConnectorCommand &&
                   Equals(unlockConnectorCommand);

        #endregion

        #region Equals(UnlockConnectorCommand)

        /// <summary>
        /// Compares two 'unlock connector' commands for equality.
        /// </summary>
        /// <param name="UnlockConnectorCommand">An 'unlock connector' command to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(UnlockConnectorCommand UnlockConnectorCommand)
        {

            if (UnlockConnectorCommand is null)
                throw new ArgumentNullException(nameof(UnlockConnectorCommand), "The given 'unlock connector' command must not be null!");

            return LocationId. Equals(UnlockConnectorCommand.LocationId)  &&
                   EVSEUId.    Equals(UnlockConnectorCommand.EVSEUId)     &&
                   ConnectorId.Equals(UnlockConnectorCommand.ConnectorId) &&
                   ResponseURL.Equals(UnlockConnectorCommand.ResponseURL);

        }

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

                return LocationId. GetHashCode() * 7 ^
                       EVSEUId.    GetHashCode() * 5 ^
                       ConnectorId.GetHashCode() * 3 ^
                       ResponseURL.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(LocationId,  " / ",
                             EVSEUId,     " / ",
                             ConnectorId, " => ",
                             ResponseURL);

        #endregion

    }

}
