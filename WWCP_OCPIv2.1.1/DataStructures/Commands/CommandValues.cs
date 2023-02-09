/*
 * Copyright (c) 2015-2023 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The common interface for all OCPI commands.
    /// </summary>
    public class CommandValues
    {

        #region Properties

        /// <summary>
        /// The command.
        /// </summary>
        public IOCPICommand      Command            { get; }

        /// <summary>
        /// The optional received upstream command.
        /// </summary>
        public IOCPICommand?     UpstreamCommand    { get; }

        /// <summary>
        /// The command response.
        /// </summary>
        public CommandResponse?  Response           { get; internal set; }

        /// <summary>
        /// The (later async) command result.
        /// </summary>
        public CommandResult?    Result             { get; internal set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create new command values.
        /// </summary>
        /// <param name="Command">The command.</param>
        /// <param name="UpstreamCommand">The optional received upstream command.</param>
        /// <param name="Response">The command response.</param>
        /// <param name="Result">The (later async) command result.</param>
        private CommandValues(IOCPICommand      Command,
                              IOCPICommand?     UpstreamCommand,
                              CommandResponse?  Response   = null,
                              CommandResult?    Result     = null)
        {

            this.Command          = Command;
            this.UpstreamCommand  = UpstreamCommand;
            this.Response         = Response;
            this.Result           = Result;

        }

        #endregion


        #region FromCommand(Command)

        /// <summary>
        /// Create new command values.
        /// </summary>
        /// <param name="Command">The command.</param>
        public static CommandValues FromCommand(IOCPICommand Command)

            => new (Command,
                    null);

        #endregion

        #region FromUpstreamCommand(UpstreamCommand)

        /// <summary>
        /// Create new command values.
        /// </summary>
        /// <param name="UpstreamCommand">The received upstream command.</param>
        public static CommandValues FromUpstreamCommand(IOCPICommand UpstreamCommand)

            => new (null,
                    UpstreamCommand);

        #endregion

        #region FromUpstreamCommand(Command, UpstreamCommand)

        /// <summary>
        /// Create new command values.
        /// </summary>
        /// <param name="Command">The command.</param>
        /// <param name="UpstreamCommand">The received upstream command.</param>
        public static CommandValues FromUpstreamCommand(IOCPICommand  Command,
                                                        IOCPICommand  UpstreamCommand)

            => new (Command,
                    UpstreamCommand);

        #endregion

    }

}
