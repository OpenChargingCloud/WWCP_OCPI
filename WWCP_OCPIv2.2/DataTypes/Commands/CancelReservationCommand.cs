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
    /// The 'cancel reservation' command.
    /// </summary>
    public class CancelReservationCommand : AOCPICommand<CancelReservationCommand>
    {

        #region Properties

        /// <summary>
        /// Reservation identification unique for this reservation. If the charge point already
        /// has a reservation that matches this reservation identification the charge point will
        /// replace the reservation.
        /// </summary>
        [Mandatory]
        public Reservation_Id  ReservationId     { get; }

        /// <summary>
        /// URL that the CommandResult POST should be sent to. This URL might contain an
        /// unique identification to be able to distinguish between 'cancel reservation' command requests.
        /// </summary>
        [Mandatory]
        public URL             ResponseURL       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'cancel reservation' command command.
        /// </summary>
        /// <param name="ReservationId">Reservation identification unique for this reservation.</param>
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'cancel reservation' command requests.</param>
        /// 
        /// <param name="RequestId">An optional request identification.</param>
        /// <param name="CorrelationId">An optional request correlation identification.</param>
        public CancelReservationCommand(Reservation_Id   ReservationId,
                                        URL              ResponseURL,

                                        Request_Id?      RequestId       = null,
                                        Correlation_Id?  CorrelationId   = null)

            : base(RequestId,
                   CorrelationId)

        {

            this.ReservationId  = ReservationId;
            this.ResponseURL    = ResponseURL;

        }

        #endregion


        #region (static) Parse   (JSON, CustomCancelReservationCommandParser = null)

        /// <summary>
        /// Parse the given JSON representation of a 'cancel reservation' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCancelReservationCommandParser">A delegate to parse custom 'cancel reservation' command JSON objects.</param>
        public static CancelReservationCommand Parse(JObject                                                JSON,
                                                     CustomJObjectParserDelegate<CancelReservationCommand>  CustomCancelReservationCommandParser   = null)
        {

            if (TryParse(JSON,
                         out CancelReservationCommand  stopReservationCommand,
                         out String                    ErrorResponse,
                         CustomCancelReservationCommandParser))
            {
                return stopReservationCommand;
            }

            throw new ArgumentException("The given JSON representation of a 'cancel reservation' command is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomCancelReservationCommandParser = null)

        /// <summary>
        /// Parse the given text representation of a 'cancel reservation' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomCancelReservationCommandParser">A delegate to parse custom 'cancel reservation' command JSON objects.</param>
        public static CancelReservationCommand Parse(String                                                 Text,
                                                     CustomJObjectParserDelegate<CancelReservationCommand>  CustomCancelReservationCommandParser   = null)
        {

            if (TryParse(Text,
                         out CancelReservationCommand  stopReservationCommand,
                         out String                    ErrorResponse,
                         CustomCancelReservationCommandParser))
            {
                return stopReservationCommand;
            }

            throw new ArgumentException("The given text representation of a 'cancel reservation' command is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, CustomCancelReservationCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'cancel reservation' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomCancelReservationCommandParser">A delegate to parse custom 'cancel reservation' command JSON objects.</param>
        public static CancelReservationCommand? TryParse(JObject                                                JSON,
                                                         CustomJObjectParserDelegate<CancelReservationCommand>  CustomCancelReservationCommandParser   = null)
        {

            if (TryParse(JSON,
                         out CancelReservationCommand  stopReservationCommand,
                         out String                    ErrorResponse,
                         CustomCancelReservationCommandParser))
            {
                return stopReservationCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(Text, CustomCancelReservationCommandParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a 'cancel reservation' command.
        /// </summary>
        /// <param name="Text">The JSON to parse.</param>
        /// <param name="CustomCancelReservationCommandParser">A delegate to parse custom 'cancel reservation' command JSON objects.</param>
        public static CancelReservationCommand? TryParse(String                                                 Text,
                                                         CustomJObjectParserDelegate<CancelReservationCommand>  CustomCancelReservationCommandParser   = null)
        {

            if (TryParse(Text,
                         out CancelReservationCommand  stopReservationCommand,
                         out String                    ErrorResponse,
                         CustomCancelReservationCommandParser))
            {
                return stopReservationCommand;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out CancelReservationCommand, out ErrorResponse, CustomCancelReservationCommandParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a 'cancel reservation' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CancelReservationCommand">The parsed 'cancel reservation' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                       JSON,
                                       out CancelReservationCommand  CancelReservationCommand,
                                       out String                    ErrorResponse)

            => TryParse(JSON,
                        out CancelReservationCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a 'cancel reservation' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CancelReservationCommand">The parsed 'cancel reservation' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCancelReservationCommandParser">A delegate to parse custom 'cancel reservation' command JSON objects.</param>
        public static Boolean TryParse(JObject                                                JSON,
                                       out CancelReservationCommand                           CancelReservationCommand,
                                       out String                                             ErrorResponse,
                                       CustomJObjectParserDelegate<CancelReservationCommand>  CustomCancelReservationCommandParser   = null)
        {

            try
            {

                CancelReservationCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ReservationId     [mandatory]

                if (!JSON.ParseMandatory("reservation_id",
                                         "reservation identification",
                                         Reservation_Id.TryParse,
                                         out Reservation_Id ReservationId,
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


                CancelReservationCommand = new CancelReservationCommand(ReservationId,
                                                                        ResponseURL);

                if (CustomCancelReservationCommandParser != null)
                    CancelReservationCommand = CustomCancelReservationCommandParser(JSON,
                                                                                    CancelReservationCommand);

                return true;

            }
            catch (Exception e)
            {
                CancelReservationCommand  = default;
                ErrorResponse             = "The given JSON representation of a 'cancel reservation' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out CancelReservationCommand, out ErrorResponse, CustomCancelReservationCommandParser = null)

        /// <summary>
        /// Try to parse the given text representation of a 'cancel reservation' command.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CancelReservationCommand">The parsed stopReservationCommand.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomCancelReservationCommandParser">A delegate to parse custom 'cancel reservation' command JSON objects.</param>
        public static Boolean TryParse(String                                                 Text,
                                       out CancelReservationCommand                           CancelReservationCommand,
                                       out String                                             ErrorResponse,
                                       CustomJObjectParserDelegate<CancelReservationCommand>  CustomCancelReservationCommandParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out CancelReservationCommand,
                                out ErrorResponse,
                                CustomCancelReservationCommandParser);

            }
            catch (Exception e)
            {
                CancelReservationCommand  = default;
                ErrorResponse             = "The given text representation of a 'cancel reservation' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomCancelReservationCommandSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomCancelReservationCommandSerializer">A delegate to serialize custom 'cancel reservation' command JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<CancelReservationCommand> CustomCancelReservationCommandSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("reservation_id",  ReservationId.ToString()),
                           new JProperty("response_url",    ResponseURL.  ToString())
                       );

            return CustomCancelReservationCommandSerializer != null
                       ? CustomCancelReservationCommandSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (CancelReservationCommand1, CancelReservationCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CancelReservationCommand1">An 'cancel reservation' command.</param>
        /// <param name="CancelReservationCommand2">Another 'cancel reservation' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CancelReservationCommand CancelReservationCommand1,
                                           CancelReservationCommand CancelReservationCommand2)
        {

            if (Object.ReferenceEquals(CancelReservationCommand1, CancelReservationCommand2))
                return true;

            if (CancelReservationCommand1 is null || CancelReservationCommand2 is null)
                return false;

            return CancelReservationCommand1.Equals(CancelReservationCommand2);

        }

        #endregion

        #region Operator != (CancelReservationCommand1, CancelReservationCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CancelReservationCommand1">An 'cancel reservation' command.</param>
        /// <param name="CancelReservationCommand2">Another 'cancel reservation' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CancelReservationCommand CancelReservationCommand1,
                                           CancelReservationCommand CancelReservationCommand2)

            => !(CancelReservationCommand1 == CancelReservationCommand2);

        #endregion

        #region Operator <  (CancelReservationCommand1, CancelReservationCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CancelReservationCommand1">An 'cancel reservation' command.</param>
        /// <param name="CancelReservationCommand2">Another 'cancel reservation' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CancelReservationCommand CancelReservationCommand1,
                                          CancelReservationCommand CancelReservationCommand2)

            => CancelReservationCommand1 is null
                   ? throw new ArgumentNullException(nameof(CancelReservationCommand1), "The given 'cancel reservation' command must not be null!")
                   : CancelReservationCommand1.CompareTo(CancelReservationCommand2) < 0;

        #endregion

        #region Operator <= (CancelReservationCommand1, CancelReservationCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CancelReservationCommand1">An 'cancel reservation' command.</param>
        /// <param name="CancelReservationCommand2">Another 'cancel reservation' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CancelReservationCommand CancelReservationCommand1,
                                           CancelReservationCommand CancelReservationCommand2)

            => !(CancelReservationCommand1 > CancelReservationCommand2);

        #endregion

        #region Operator >  (CancelReservationCommand1, CancelReservationCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CancelReservationCommand1">An 'cancel reservation' command.</param>
        /// <param name="CancelReservationCommand2">Another 'cancel reservation' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CancelReservationCommand CancelReservationCommand1,
                                          CancelReservationCommand CancelReservationCommand2)

            => CancelReservationCommand1 is null
                   ? throw new ArgumentNullException(nameof(CancelReservationCommand1), "The given 'cancel reservation' command must not be null!")
                   : CancelReservationCommand1.CompareTo(CancelReservationCommand2) > 0;

        #endregion

        #region Operator >= (CancelReservationCommand1, CancelReservationCommand2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CancelReservationCommand1">An 'cancel reservation' command.</param>
        /// <param name="CancelReservationCommand2">Another 'cancel reservation' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CancelReservationCommand CancelReservationCommand1,
                                           CancelReservationCommand CancelReservationCommand2)

            => !(CancelReservationCommand1 < CancelReservationCommand2);

        #endregion

        #endregion

        #region IComparable<CancelReservationCommand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is CancelReservationCommand stopReservationCommand
                   ? CompareTo(stopReservationCommand)
                   : throw new ArgumentException("The given object is not a 'cancel reservation' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CancelReservationCommand)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CancelReservationCommand">An object to compare with.</param>
        public override Int32 CompareTo(CancelReservationCommand CancelReservationCommand)
        {

            if (CancelReservationCommand is null)
                throw new ArgumentNullException(nameof(CancelReservationCommand), "The given 'cancel reservation' command must not be null!");

            var c = ReservationId.CompareTo(CancelReservationCommand.ReservationId);

            if (c == 0)
                c = ResponseURL.  CompareTo(CancelReservationCommand.ResponseURL);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CancelReservationCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CancelReservationCommand stopReservationCommand &&
                   Equals(stopReservationCommand);

        #endregion

        #region Equals(CancelReservationCommand)

        /// <summary>
        /// Compares two 'cancel reservation' commands for equality.
        /// </summary>
        /// <param name="CancelReservationCommand">An 'cancel reservation' command to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(CancelReservationCommand CancelReservationCommand)
        {

            if (CancelReservationCommand is null)
                throw new ArgumentNullException(nameof(CancelReservationCommand), "The given 'cancel reservation' command must not be null!");

            return ReservationId.Equals(CancelReservationCommand.ReservationId) &&
                   ResponseURL.  Equals(CancelReservationCommand.ResponseURL);

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

                return ReservationId.GetHashCode() * 3 ^
                       ResponseURL.  GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ReservationId, " => ",
                             ResponseURL);

        #endregion

    }

}
