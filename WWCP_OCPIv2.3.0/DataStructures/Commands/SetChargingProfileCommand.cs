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

#endregion

namespace cloud.charging.open.protocols.OCPIv2_3_0
{

    /// <summary>
    /// The 'set charging profile' command.
    /// </summary>
    public class SetChargingProfileCommand : AOCPICommand<SetChargingProfileCommand>
    {

        #region Properties

        /// <summary>
        /// The charging profile containing limits for the available power or current over time.
        /// </summary>
        [Mandatory]
        public ChargingProfile  ChargingProfile    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new 'set charging profile' command command.
        /// </summary>
        /// <param name="ChargingProfile">Session identification of the charging session that is requested to be stopped.</param>
        /// 
        /// <param name="ResponseURL">URL that the CommandResult POST should be sent to. This URL might contain an unique identification to be able to distinguish between 'set charging profile' command requests.</param>
        /// <param name="Id">An optional unique identification of the command.</param>
        /// <param name="RequestId">An optional unique request identification.</param>
        /// <param name="CorrelationId">An optional unique request correlation identification.</param>
        public SetChargingProfileCommand(ChargingProfile  ChargingProfile,

                                         URL              ResponseURL,
                                         Command_Id?      Id              = null,
                                         Request_Id?      RequestId       = null,
                                         Correlation_Id?  CorrelationId   = null)

            : base(ResponseURL,
                   Id,
                   RequestId,
                   CorrelationId)

        {

            this.ChargingProfile  = ChargingProfile;

        }

        #endregion


        #region (static) Parse   (JSON, CustomSetChargingProfileParserCommand = null)

        /// <summary>
        /// Parse the given JSON representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomSetChargingProfileParserCommand">A delegate to parse custom 'set charging profile' command JSON objects.</param>
        public static SetChargingProfileCommand Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<SetChargingProfileCommand>?  CustomSetChargingProfileParserCommand   = null)
        {

            if (TryParse(JSON,
                         out var setChargingProfileCommandCommand,
                         out var errorResponse,
                         CustomSetChargingProfileParserCommand))
            {
                return setChargingProfileCommandCommand;
            }

            throw new ArgumentException("The given JSON representation of a 'set charging profile' command is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SetChargingProfileCommand, out ErrorResponse, CustomSetChargingProfileParserCommand = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SetChargingProfileCommand">The parsed 'set charging profile' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                         JSON,
                                       out SetChargingProfileCommand?  SetChargingProfileCommand,
                                       out String?                     ErrorResponse)

            => TryParse(JSON,
                        out SetChargingProfileCommand,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a 'set charging profile' command.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="SetChargingProfileCommand">The parsed 'set charging profile' command.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSetChargingProfileParserCommand">A delegate to parse custom 'set charging profile' command JSON objects.</param>
        public static Boolean TryParse(JObject                                                  JSON,
                                       out SetChargingProfileCommand?                           SetChargingProfileCommand,
                                       out String?                                              ErrorResponse,
                                       CustomJObjectParserDelegate<SetChargingProfileCommand>?  CustomSetChargingProfileParserCommand   = null)
        {

            try
            {

                SetChargingProfileCommand = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ChargingProfile   [mandatory]

                if (!JSON.ParseMandatoryJSON("charging_profile",
                                             "charging profile",
                                             OCPIv2_3_0.ChargingProfile.TryParse,
                                             out ChargingProfile? ChargingProfile,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (ChargingProfile is null)
                    return false;

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

                #region Parse CommandId         [optional, internal]

                if (JSON.ParseOptional("id",
                                       "command identification",
                                       Command_Id.TryParse,
                                       out Command_Id? CommandId,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse RequestId         [optional, internal]

                if (JSON.ParseOptional("request_id",
                                       "request identification",
                                       Request_Id.TryParse,
                                       out Request_Id? RequestId,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CorrelationId     [optional, internal]

                if (JSON.ParseOptional("correlation_Id",
                                       "correlation identification",
                                       Correlation_Id.TryParse,
                                       out Correlation_Id? CorrelationId,
                                       out ErrorResponse))
                {
                    return false;
                }

                #endregion


                SetChargingProfileCommand = new SetChargingProfileCommand(ChargingProfile,

                                                                          ResponseURL,
                                                                          CommandId,
                                                                          RequestId,
                                                                          CorrelationId);

                if (CustomSetChargingProfileParserCommand is not null)
                    SetChargingProfileCommand = CustomSetChargingProfileParserCommand(JSON,
                                                                                      SetChargingProfileCommand);

                return true;

            }
            catch (Exception e)
            {
                SetChargingProfileCommand  = default;
                ErrorResponse              = "The given JSON representation of a 'set charging profile' command is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSetChargingProfileSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSetChargingProfileSerializer">A delegate to serialize custom 'set charging profile' command JSON objects.</param>
        /// <param name="CustomChargingProfileSerializer">A delegate to serialize custom charging profile JSON objects.</param>
        /// <param name="CustomChargingProfilePeriodSerializer">A delegate to serialize custom charging profile period JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SetChargingProfileCommand>?  CustomSetChargingProfileSerializer      = null,
                              CustomJObjectSerializerDelegate<ChargingProfile>?            CustomChargingProfileSerializer         = null,
                              CustomJObjectSerializerDelegate<ChargingProfilePeriod>?      CustomChargingProfilePeriodSerializer   = null)
        {

            var json = JSONObject.Create(

                           new JProperty("charging_profile",  ChargingProfile.ToJSON(CustomChargingProfileSerializer,
                                                                                     CustomChargingProfilePeriodSerializer)),

                           new JProperty("response_url",      ResponseURL.    ToString())

                       );

            return CustomSetChargingProfileSerializer is not null
                       ? CustomSetChargingProfileSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">A 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SetChargingProfileCommand SetChargingProfile1,
                                           SetChargingProfileCommand SetChargingProfile2)
        {

            if (Object.ReferenceEquals(SetChargingProfile1, SetChargingProfile2))
                return true;

            if (SetChargingProfile1 is null || SetChargingProfile2 is null)
                return false;

            return SetChargingProfile1.Equals(SetChargingProfile2);

        }

        #endregion

        #region Operator != (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">A 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SetChargingProfileCommand SetChargingProfile1,
                                           SetChargingProfileCommand SetChargingProfile2)

            => !(SetChargingProfile1 == SetChargingProfile2);

        #endregion

        #region Operator <  (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">A 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SetChargingProfileCommand SetChargingProfile1,
                                          SetChargingProfileCommand SetChargingProfile2)

            => SetChargingProfile1 is null
                   ? throw new ArgumentNullException(nameof(SetChargingProfile2), "The given 'set charging profile' command must not be null!")
                   : SetChargingProfile1.CompareTo(SetChargingProfile2) < 0;

        #endregion

        #region Operator <= (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">A 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SetChargingProfileCommand SetChargingProfile1,
                                           SetChargingProfileCommand SetChargingProfile2)

            => !(SetChargingProfile1 > SetChargingProfile2);

        #endregion

        #region Operator >  (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">A 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SetChargingProfileCommand SetChargingProfile1,
                                          SetChargingProfileCommand SetChargingProfile2)

            => SetChargingProfile1 is null
                   ? throw new ArgumentNullException(nameof(SetChargingProfile2), "The given 'set charging profile' command must not be null!")
                   : SetChargingProfile1.CompareTo(SetChargingProfile2) > 0;

        #endregion

        #region Operator >= (SetChargingProfile1, SetChargingProfile2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SetChargingProfile1">A 'set charging profile' command.</param>
        /// <param name="SetChargingProfile2">Another 'set charging profile' command.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SetChargingProfileCommand SetChargingProfile1,
                                           SetChargingProfileCommand SetChargingProfile2)

            => !(SetChargingProfile1 < SetChargingProfile2);

        #endregion

        #endregion

        #region IComparable<SetChargingProfileCommand> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two 'set charging profile' commands.
        /// </summary>
        /// <param name="Object">A 'set charging profile' command to compare with.</param>
        public override Int32 CompareTo(Object? Object)

            => Object is SetChargingProfileCommand setChargingProfileCommand
                   ? CompareTo(setChargingProfileCommand)
                   : throw new ArgumentException("The given object is not a 'set charging profile' command!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(SetChargingProfileCommand)

        /// <summary>
        /// Compares two 'set charging profile' commands.
        /// </summary>
        /// <param name="SetChargingProfileCommand">A 'set charging profile' command to compare with.</param>
        public override Int32 CompareTo(SetChargingProfileCommand? SetChargingProfileCommand)
        {

            if (SetChargingProfileCommand is null)
                throw new ArgumentNullException(nameof(SetChargingProfileCommand), "The given 'set charging profile' command must not be null!");

            var c = ChargingProfile.CompareTo(SetChargingProfileCommand.ChargingProfile);


            if (c == 0)
                c = ResponseURL.    CompareTo(SetChargingProfileCommand.ResponseURL);

            if (c == 0)
                c = Id.             CompareTo(SetChargingProfileCommand.Id);

            if (c == 0)
                c = RequestId.      CompareTo(SetChargingProfileCommand.RequestId);

            if (c == 0)
                c = CorrelationId.  CompareTo(SetChargingProfileCommand.CorrelationId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<SetChargingProfileCommand> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two 'set charging profile' commands for equality.
        /// </summary>
        /// <param name="Object">A 'set charging profile' command to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SetChargingProfileCommand setChargingProfileCommand &&
                   Equals(setChargingProfileCommand);

        #endregion

        #region Equals(SetChargingProfileCommand)

        /// <summary>
        /// Compares two 'set charging profile' commands for equality.
        /// </summary>
        /// <param name="SetChargingProfileCommand">A 'set charging profile' command to compare with.</param>
        public override Boolean Equals(SetChargingProfileCommand? SetChargingProfileCommand)

            => SetChargingProfileCommand is not null &&

               ChargingProfile.Equals(SetChargingProfileCommand.ChargingProfile) &&

               ResponseURL.    Equals(SetChargingProfileCommand.ResponseURL)     &&
               Id.             Equals(SetChargingProfileCommand.Id)              &&
               RequestId.      Equals(SetChargingProfileCommand.RequestId)       &&
               CorrelationId.  Equals(SetChargingProfileCommand.CorrelationId);

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

                return ChargingProfile.GetHashCode() * 11 ^

                       ResponseURL.    GetHashCode() *  7 ^
                       Id.             GetHashCode() *  5 ^
                       RequestId.      GetHashCode() *  3 ^
                       CorrelationId.  GetHashCode();

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
                   ChargingProfile,
                   " => ",
                   ResponseURL

               );

        #endregion

    }

}
