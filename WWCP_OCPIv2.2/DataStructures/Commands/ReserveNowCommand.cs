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
    /// The 'reserve now' command.
    /// </summary>
    public class ReserveNowCommand : AOCPICommand<ReserveNowCommand>
    {

        #region Properties

        /// <summary>
        /// Token for how to reserve this charge point (and specific EVSE).
        /// </summary>
        [Mandatory]
        public Token                    Token                     { get; }

        /// <summary>
        /// The timestamp when this reservation ends.
        /// </summary>
        public DateTime                 ExpiryDate                { get; }

        /// <summary>
        /// Reservation identification. If the receiver (typically a charge point operator)
        /// already has a reservation that matches this reservation identification for that
        /// location it will replace the reservation.
        /// </summary>
        [Mandatory]
        public Reservation_Id           ReservationId             { get; }

        /// <summary>
        /// Location identification of the location (belonging to the CPO this request is send to)
        /// for which to reserve an EVSE.
        /// </summary>
        [Mandatory]
        public Location_Id              LocationId                { get; }

        /// <summary>
        /// Optional EVSE identification of the EVSE of this location if a specific EVSE has to be reserved.
        /// </summary>
        [Mandatory]
        public EVSE_UId?                EVSEUId                   { get; }

        /// <summary>
        /// Reference to the authorization given by the eMSP, when given, this reference will be provided
        /// in the relevant session and/or CDR.
        /// </summary>
        [Mandatory]
        public AuthorizationReference?  AuthorizationReference    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'reserve now' command command.
        /// </summary>
        /// <param name="Token">Token for how to reserve this charge point (and specific EVSE).</param>
        /// <param name="ExpiryDate">The timestamp when this reservation ends.</param>
        /// <param name="ReservationId">Reservation identification. If the receiver (typically a charge point operator) already has a reservation that matches this reservation identification for that location it will replace the reservation.</param>
        /// <param name="LocationId">Location identification of the location (belonging to the CPO this request is send to) for which to reserve an EVSE.</param>
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'reserve now' command requests.</param>
        /// 
        /// <param name="EVSEUId">Optional EVSE identification of the EVSE of this location if a specific EVSE has to be reserved.</param>
        /// <param name="AuthorizationReference">Optional reference to the authorization given by the eMSP, when given, this reference will be provided in the relevant session and/or CDR.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        public ReserveNowCommand(Token                    Token,
                                 DateTime                 ExpiryDate,
                                 Reservation_Id           ReservationId,
                                 Location_Id              LocationId,
                                 URL                      ResponseURL,

                                 EVSE_UId?                EVSEUId                  = null,
                                 AuthorizationReference?  AuthorizationReference   = null,

                                 Request_Id?              RequestId                = null,
                                 Correlation_Id?          CorrelationId            = null)

            : base(ResponseURL,
                   RequestId,
                   CorrelationId)

        {

            this.Token                   = Token;
            this.ExpiryDate              = ExpiryDate;
            this.ReservationId           = ReservationId;
            this.LocationId              = LocationId;

            this.EVSEUId                 = EVSEUId;
            this.AuthorizationReference  = AuthorizationReference;

        }

        #endregion


        #region (static) Parse   (JSON, CustomReserveNowCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of an 'reserve now' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomReserveNowCommandParser">A delegate to parse custom 'reserve now' command JSON objects.</param>
        public static ReserveNowCommand Parse(JObject                                         JSON,
                                              CustomJObjectParserDelegate<ReserveNowCommand>  CustomReserveNowCommandParser   = null)
        {

            if (TryParse(JSON,
                         out ReserveNowCommand  unlockConnectorCommand,
                         out String             ErrorResponse,
                         CustomReserveNowCommandParser))
            {
                return unlockConnectorCommand;
            }

            throw new ArgumentException("The given JSON representation of an 'reserve now' command is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomReserveNowCommandParser = null)

        /// <summary>
        /// Parse the given text representation of an 'reserve now' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomReserveNowCommandParser">A delegate to parse custom 'reserve now' command JSON objects.</param>
        public static ReserveNowCommand Parse(String                                          Text,
                                              CustomJObjectParserDelegate<ReserveNowCommand>  CustomReserveNowCommandParser   = null)
        {

            if (TryParse(Text,
                         out ReserveNowCommand  unlockConnectorCommand,
                         out String             ErrorResponse,
                         CustomReserveNowCommandParser))
            {
                return unlockConnectorCommand;
            }

            throw new ArgumentException("The given text representation of an 'reserve now' command is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomReserveNowCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'reserve now' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomReserveNowCommandParser">A delegate to parse custom 'reserve now' command JSON objects.</param>
        public static ReserveNowCommand? TryParse(JObject                                         JSON,
                                                  CustomJObjectParserDelegate<ReserveNowCommand>  CustomReserveNowCommandParser   = null)
        {

            if (TryParse(JSON,
                         out ReserveNowCommand  unlockConnectorCommand,
                         out String             ErrorResponse,
                         CustomReserveNowCommandParser))
            {
                return unlockConnectorCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomReserveNowCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'reserve now' command.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomReserveNowCommandParser">A delegate to parse custom 'reserve now' command JSON objects.</param>
        public static ReserveNowCommand? TryParse(String                                          Text,
                                                  CustomJObjectParserDelegate<ReserveNowCommand>  CustomReserveNowCommandParser   = null)
        {

            if (TryParse(Text,
                         out ReserveNowCommand  unlockConnectorCommand,
                         out String             ErrorResponse,
                         CustomReserveNowCommandParser))
            {
                return unlockConnectorCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out ReserveNowCommand, out ErrorResponse, CustomReserveNowCommandParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an 'reserve now' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ReserveNowCommand">The parsed 'reserve now' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                JSON,
                                       out ReserveNowCommand  ReserveNowCommand,
                                       out String             ErrorResponse)

            => TryParse(JSON,
                        out ReserveNowCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an 'reserve now' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ReserveNowCommand">The parsed 'reserve now' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReserveNowCommandParser">A delegate to parse custom 'reserve now' command JSON objects.</param>
        public static Boolean TryParse(JObject                                         JSON,
                                       out ReserveNowCommand                           ReserveNowCommand,
                                       out String                                      ErrorResponse,
                                       CustomJObjectParserDelegate<ReserveNowCommand>  CustomReserveNowCommandParser   = null)
        {

            try
            {

                ReserveNowCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Token                     [mandatory]

                if (!JSON.ParseMandatoryJSON("token",
                                             "token",
                                             OCPIv2_2.Token.TryParse,
                                             out Token Token,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ExpiryDate                [mandatory]

                if (!JSON.ParseMandatory("expiry_date",
                                         "expiry date",
                                         DateTime.TryParse,
                                         out DateTime ExpiryDate,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ReservationId             [mandatory]

                if (!JSON.ParseMandatory("reservation_id",
                                         "reservation identification",
                                         Reservation_Id.TryParse,
                                         out Reservation_Id ReservationId,
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
                    return false;
                }

                #endregion


                ReserveNowCommand = new ReserveNowCommand(Token,
                                                          ExpiryDate,
                                                          ReservationId,
                                                          LocationId,
                                                          ResponseURL,

                                                          EVSEUId,
                                                          AuthorizationReference);

                if (CustomReserveNowCommandParser is not null)
                    ReserveNowCommand = CustomReserveNowCommandParser(JSON,
                                                                      ReserveNowCommand);

                return true;

            }
            catch (Exception e)
            {
                ReserveNowCommand  = default;
                ErrorResponse      = "The given JSON representation of an 'reserve now' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out ReserveNowCommand, out ErrorResponse, CustomReserveNowCommandParser = null)

        /// <summary>
        /// Try to parse the given text representation of an 'reserve now' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="ReserveNowCommand">The parsed unlockConnectorCommand.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomReserveNowCommandParser">A delegate to parse custom 'reserve now' command JSON objects.</param>
        public static Boolean TryParse(String                                          Text,
                                       out ReserveNowCommand                           ReserveNowCommand,
                                       out String                                      ErrorResponse,
                                       CustomJObjectParserDelegate<ReserveNowCommand>  CustomReserveNowCommandParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out ReserveNowCommand,
                                out ErrorResponse,
                                CustomReserveNowCommandParser);

            }
            catch (Exception e)
            {
                ReserveNowCommand  = default;
                ErrorResponse      = "The given text representation of an 'reserve now' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomReserveNowCommandSerializer = null, CustomTokenSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomReserveNowCommandSerializer">A delegate to serialize custom 'reserve now' command JSON objects.</param>
        /// <param name="CustomTokenSerializer">A delegate to serialize custom token JSON objects.</param>
        /// <param name="CustomEnergyContractSerializer">A delegate to serialize custom energy contract JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ReserveNowCommand>  CustomReserveNowCommandSerializer   = null,
                              CustomJObjectSerializerDelegate<Token>              CustomTokenSerializer               = null,
                              CustomJObjectSerializerDelegate<EnergyContract>     CustomEnergyContractSerializer      = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("token",                          Token.                 ToJSON(CustomTokenSerializer,
                                                                                                         CustomEnergyContractSerializer)),
                           new JProperty("expiry_date",                    ExpiryDate.            ToIso8601()),
                           new JProperty("reservation_id",                 ReservationId.         ToString()),
                           new JProperty("location_id",                    LocationId.            ToString()),

                           EVSEUId.HasValue
                               ? new JProperty("evse_uid",                 EVSEUId.               ToString())
                               : null,

                           AuthorizationReference.HasValue
                               ? new JProperty("authorization_reference",  AuthorizationReference.ToString())
                               : null,

                           new JProperty("response_url",                   ResponseURL.           ToString())

                       );

            return CustomReserveNowCommandSerializer is not null
                       ? CustomReserveNowCommandSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ReserveNowCommand1, ReserveNowCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowCommand1">An 'reserve now' command.</param>
        /// <param name="ReserveNowCommand2">Another 'reserve now' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ReserveNowCommand ReserveNowCommand1,
                                           ReserveNowCommand ReserveNowCommand2)
        {

            if (Object.ReferenceEquals(ReserveNowCommand1, ReserveNowCommand2))
                return true;

            if (ReserveNowCommand1 is null || ReserveNowCommand2 is null)
                return false;

            return ReserveNowCommand1.Equals(ReserveNowCommand2);

        }

        #endregion

        #region Operator != (ReserveNowCommand1, ReserveNowCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowCommand1">An 'reserve now' command.</param>
        /// <param name="ReserveNowCommand2">Another 'reserve now' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ReserveNowCommand ReserveNowCommand1,
                                           ReserveNowCommand ReserveNowCommand2)

            => !(ReserveNowCommand1 == ReserveNowCommand2);

        #endregion

        #region Operator <  (ReserveNowCommand1, ReserveNowCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowCommand1">An 'reserve now' command.</param>
        /// <param name="ReserveNowCommand2">Another 'reserve now' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ReserveNowCommand ReserveNowCommand1,
                                          ReserveNowCommand ReserveNowCommand2)

            => ReserveNowCommand1 is null
                   ? throw new ArgumentNullException(nameof(ReserveNowCommand1), "The given 'reserve now' command must not be null!")
                   : ReserveNowCommand1.CompareTo(ReserveNowCommand2) < 0;

        #endregion

        #region Operator <= (ReserveNowCommand1, ReserveNowCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowCommand1">An 'reserve now' command.</param>
        /// <param name="ReserveNowCommand2">Another 'reserve now' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ReserveNowCommand ReserveNowCommand1,
                                           ReserveNowCommand ReserveNowCommand2)

            => !(ReserveNowCommand1 > ReserveNowCommand2);

        #endregion

        #region Operator >  (ReserveNowCommand1, ReserveNowCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowCommand1">An 'reserve now' command.</param>
        /// <param name="ReserveNowCommand2">Another 'reserve now' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ReserveNowCommand ReserveNowCommand1,
                                          ReserveNowCommand ReserveNowCommand2)

            => ReserveNowCommand1 is null
                   ? throw new ArgumentNullException(nameof(ReserveNowCommand1), "The given 'reserve now' command must not be null!")
                   : ReserveNowCommand1.CompareTo(ReserveNowCommand2) > 0;

        #endregion

        #region Operator >= (ReserveNowCommand1, ReserveNowCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowCommand1">An 'reserve now' command.</param>
        /// <param name="ReserveNowCommand2">Another 'reserve now' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ReserveNowCommand ReserveNowCommand1,
                                           ReserveNowCommand ReserveNowCommand2)

            => !(ReserveNowCommand1 < ReserveNowCommand2);

        #endregion

        #endregion

        #region IComparable<ReserveNowCommand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is ReserveNowCommand unlockConnectorCommand
                   ? CompareTo(unlockConnectorCommand)
                   : throw new ArgumentException("The given object is not an 'reserve now' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ReserveNowCommand)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ReserveNowCommand">An object to compare with.</param>
        public override Int32 CompareTo(ReserveNowCommand ReserveNowCommand)
        {

            if (ReserveNowCommand is null)
                throw new ArgumentNullException(nameof(ReserveNowCommand), "The given 'reserve now' command must not be null!");

            var c = Token.        CompareTo(ReserveNowCommand.Token);

            if (c == 0)
                c = ExpiryDate.   CompareTo(ReserveNowCommand.ExpiryDate);

            if (c == 0)
                c = ReservationId.CompareTo(ReserveNowCommand.ReservationId);

            if (c == 0)
                c = LocationId.   CompareTo(ReserveNowCommand.LocationId);

            if (c == 0 && EVSEUId.HasValue && ReserveNowCommand.EVSEUId.HasValue)
                c = EVSEUId.Value.CompareTo(ReserveNowCommand.EVSEUId.Value);

            if (c == 0 && AuthorizationReference.HasValue && ReserveNowCommand.AuthorizationReference.HasValue)
                c = AuthorizationReference.Value.CompareTo(ReserveNowCommand.AuthorizationReference.Value);

            if (c == 0)
                c = ResponseURL.  CompareTo(ReserveNowCommand.ResponseURL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ReserveNowCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ReserveNowCommand unlockConnectorCommand &&
                   Equals(unlockConnectorCommand);

        #endregion

        #region Equals(ReserveNowCommand)

        /// <summary>
        /// Compares two 'reserve now' commands for equality.
        /// </summary>
        /// <param name="ReserveNowCommand">An 'reserve now' command to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ReserveNowCommand ReserveNowCommand)
        {

            if (ReserveNowCommand is null)
                throw new ArgumentNullException(nameof(ReserveNowCommand), "The given 'reserve now' command must not be null!");

            return Token.                 Equals(ReserveNowCommand.Token)                  &&
                   ExpiryDate.            Equals(ReserveNowCommand.ExpiryDate)             &&
                   ReservationId.         Equals(ReserveNowCommand.ReservationId)          &&
                   LocationId.            Equals(ReserveNowCommand.LocationId)             &&
                   EVSEUId.               Equals(ReserveNowCommand.EVSEUId)                &&
                   AuthorizationReference.Equals(ReserveNowCommand.AuthorizationReference) &&
                   ResponseURL.           Equals(ReserveNowCommand.ResponseURL);

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

                return Token.        GetHashCode() * 17 ^
                       ExpiryDate.   GetHashCode() * 13 ^
                       ReservationId.GetHashCode() * 11 ^
                       LocationId.   GetHashCode() *  7 ^

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

            => String.Concat(Token, " / ", ExpiryDate, " /", ReservationId, " / ", LocationId,
                             EVSEUId.               HasValue ? " / " + EVSEUId : "",
                             AuthorizationReference.HasValue ? " / " + AuthorizationReference : "",
                             " => ",
                             ResponseURL);

        #endregion

    }

}
