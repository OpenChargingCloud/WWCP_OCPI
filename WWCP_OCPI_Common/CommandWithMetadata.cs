/*
 * Copyright (c) 2015-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// An OCPI API command with additional metadata.
    /// </summary>
    public sealed class CommandWithMetadata
    {

        #region Properties

        /// <summary>
        /// The name of the command.
        /// </summary>
        public String            CommandName        { get; }

        /// <summary>
        /// An optional command JSON object parameter.
        /// </summary>
        public JObject?          JSONObject         { get; }

        /// <summary>
        /// An optional command JSON array parameter.
        /// </summary>
        public JArray?           JSONArray          { get; }

        /// <summary>
        /// An optional command message parameter.
        /// </summary>
        public String?           Message            { get; }

        /// <summary>
        /// An optional command Integer parameter.
        /// </summary>
        public Int64?            Integer            { get; }

        /// <summary>
        /// An optional command Single/Float parameter.
        /// </summary>
        public Single?           Single             { get; }

        /// <summary>
        /// An optional command Boolean parameter.
        /// </summary>
        public Boolean?          Boolean            { get; }

        /// <summary>
        /// The timestamp of the command.
        /// </summary>
        public DateTimeOffset    Timestamp          { get; }

        /// <summary>
        /// The unique event tracking identification for correlating this request with other events.
        /// </summary>
        public EventTracking_Id  EventTrackingId    { get; }

        /// <summary>
        /// The optional user identification initiating this command/request.
        /// </summary>
        public User_Id?          UserId             { get; }

        #endregion

        #region Constructor(s)

        #region CommandWithMetadata(CommandName, Message,    Timestamp, UserId = null)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="Message">A command message parameter.</param>
        /// <param name="Timestamp">The timestamp of the command.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="UserId">An optional user identification initiating this command/request.</param>
        public CommandWithMetadata(String            CommandName,
                                   String?           Message,
                                   DateTimeOffset    Timestamp,
                                   EventTracking_Id  EventTrackingId,
                                   User_Id?          UserId   = null)
        {

            this.CommandName      = CommandName;
            this.Message          = Message;
            this.Timestamp        = Timestamp;
            this.EventTrackingId  = EventTrackingId;
            this.UserId           = UserId;

        }

        #endregion

        #region CommandWithMetadata(CommandName, JSONObject, Timestamp, UserId = null)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="JSONObject">A command JSON object parameter.</param>
        /// <param name="Timestamp">The timestamp of the command.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="UserId">An optional user identification initiating this command/request.</param>
        public CommandWithMetadata(String            CommandName,
                                   JObject?          JSONObject,
                                   DateTimeOffset    Timestamp,
                                   EventTracking_Id  EventTrackingId,
                                   User_Id?          UserId   = null)
        {

            this.CommandName      = CommandName;
            this.JSONObject       = JSONObject;
            this.Timestamp        = Timestamp;
            this.EventTrackingId  = EventTrackingId;
            this.UserId           = UserId;

        }

        #endregion

        #region CommandWithMetadata(CommandName, JSONArray,  Timestamp, UserId = null)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="JSONArray">A command JSON array parameter.</param>
        /// <param name="Timestamp">The timestamp of the command.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="UserId">An optional user identification initiating this command/request.</param>
        public CommandWithMetadata(String            CommandName,
                                   JArray?           JSONArray,
                                   DateTimeOffset    Timestamp,
                                   EventTracking_Id  EventTrackingId,
                                   User_Id?          UserId   = null)
        {

            this.CommandName      = CommandName;
            this.JSONArray        = JSONArray;
            this.Timestamp        = Timestamp;
            this.EventTrackingId  = EventTrackingId;
            this.UserId           = UserId;

        }

        #endregion

        #region CommandWithMetadata(CommandName, Integer,    Timestamp, UserId = null)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="Boolean">A command Integer parameter.</param>
        /// <param name="Timestamp">The timestamp of the command.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="UserId">An optional user identification initiating this command/request.</param>
        public CommandWithMetadata(String            CommandName,
                                   Int64?            Boolean,
                                   DateTimeOffset    Timestamp,
                                   EventTracking_Id  EventTrackingId,
                                   User_Id?          UserId   = null)
        {

            this.CommandName      = CommandName;
            this.Integer          = Boolean;
            this.Timestamp        = Timestamp;
            this.EventTrackingId  = EventTrackingId;
            this.UserId           = UserId;

        }

        #endregion

        #region CommandWithMetadata(CommandName, Single,     Timestamp, UserId = null)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="Single">A command Single parameter.</param>
        /// <param name="Timestamp">The timestamp of the command.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="UserId">An optional user identification initiating this command/request.</param>
        public CommandWithMetadata(String            CommandName,
                                   Single?           Single,
                                   DateTimeOffset    Timestamp,
                                   EventTracking_Id  EventTrackingId,
                                   User_Id?          UserId   = null)
        {

            this.CommandName      = CommandName;
            this.Single           = Single;
            this.Timestamp        = Timestamp;
            this.EventTrackingId  = EventTrackingId;
            this.UserId           = UserId;

        }

        #endregion

        #region CommandWithMetadata(CommandName, Boolean,    Timestamp, UserId = null)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="Boolean">A command Boolean parameter.</param>
        /// <param name="Timestamp">The timestamp of the command.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="UserId">An optional user identification initiating this command/request.</param>
        public CommandWithMetadata(String            CommandName,
                                   Boolean?          Boolean,
                                   DateTimeOffset    Timestamp,
                                   EventTracking_Id  EventTrackingId,
                                   User_Id?          UserId   = null)
        {

            this.CommandName      = CommandName;
            this.Boolean          = Boolean;
            this.Timestamp        = Timestamp;
            this.EventTrackingId  = EventTrackingId;
            this.UserId           = UserId;

        }

        #endregion

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{CommandName}' {(UserId is not null
                                        ? $"({UserId})"
                                        : String.Empty)} => {Message                                                              ??
                                                            JSONObject?.  ToString(Newtonsoft.Json.Formatting.None)?.SubstringMax(100) ??
                                                            Integer?.ToString()                                                   ??
                                                            String.Empty}";

        #endregion

    }

}
