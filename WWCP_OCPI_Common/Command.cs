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

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// An OCPI API command.
    /// </summary>
    public sealed class Command
    {

        #region Properties

        /// <summary>
        /// The name of the command.
        /// </summary>
        public String    CommandName    { get; }

        /// <summary>
        /// An optional command JSON object parameter.
        /// </summary>
        public JObject?  JSONObject     { get; }

        /// <summary>
        /// An optional command JSON array parameter.
        /// </summary>
        public JArray?   JSONArray      { get; }

        /// <summary>
        /// An optional command message parameter.
        /// </summary>
        public String?   Message        { get; }

        /// <summary>
        /// An optional command Integer parameter.
        /// </summary>
        public Int64?    Integer        { get; }

        /// <summary>
        /// An optional command Single/Float parameter.
        /// </summary>
        public Single?   Single         { get; }

        /// <summary>
        /// An optional command Boolean parameter.
        /// </summary>
        public Boolean?  Boolean        { get; }

        #endregion

        #region Constructor(s)

        #region Command(CommandName, Message)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="Message">A command message parameter.</param>
        public Command(String    CommandName,
                        String?   Message)
        {

            this.CommandName  = CommandName;
            this.Message      = Message;

        }

        #endregion

        #region Command(CommandName, JSONObject)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="JSONObject">A command JSON object parameter.</param>
        public Command(String    CommandName,
                        JObject?  JSONObject)
        {

            this.CommandName  = CommandName;
            this.JSONObject   = JSONObject;

        }

        #endregion

        #region Command(CommandName, JSONArray)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="JSONArray">A command JSON array parameter.</param>
        public Command(String   CommandName,
                        JArray?  JSONArray)
        {

            this.CommandName  = CommandName;
            this.JSONArray    = JSONArray;

        }

        #endregion

        #region Command(CommandName, Integer)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="Integer">A command Integer parameter.</param>
        public Command(String    CommandName,
                        Int64?    Integer)
        {

            this.CommandName  = CommandName;
            this.Integer       = Integer;

        }

        #endregion

        #region Command(CommandName, Single)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="Single">A command single/float parameter.</param>
        public Command(String    CommandName,
                        Single?   Single)
        {

            this.CommandName  = CommandName;
            this.Single       = Single;

        }

        #endregion

        #region Command(CommandName, Boolean)

        /// <summary>
        /// Create a new command using the given command name and message.
        /// </summary>
        /// <param name="CommandName">The name of the command.</param>
        /// <param name="Boolean">A command Boolean parameter.</param>
        public Command(String    CommandName,
                        Boolean?  Boolean)
        {

            this.CommandName  = CommandName;
            this.Boolean      = Boolean;

        }

        #endregion

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"'{CommandName}' => {Message                                                               ??
                                        JSONObject?.   ToString(Newtonsoft.Json.Formatting.None)?.SubstringMax(100) ??
                                        Integer?.ToString()                                                   ??
                                        Single?. ToString()                                                   ??
                                        String.Empty}";

        #endregion

    }

}
