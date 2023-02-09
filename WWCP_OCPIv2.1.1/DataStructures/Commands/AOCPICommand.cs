/*
 * Copyright (c) 2015-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The common interface for all OCPI commands.
    /// </summary>
    public interface IOCPICommand : IComparable
    {

        Command_Id      Id               { get; }

        /// <summary>
        /// URL that the CommandResult POST should be sent to. This URL might contain an
        /// unique identification to be able to distinguish between 'reserve now' command requests.
        /// </summary>
        URL             ResponseURL      { get; }

        /// <summary>
        /// The request identification.
        /// </summary>
        Request_Id      RequestId        { get; }

        /// <summary>
        /// The request correlation identification.
        /// </summary>
        Correlation_Id  CorrelationId    { get; }

    }

    /// <summary>
    /// The common generic interface for all OCPI commands.
    /// </summary>
    /// <typeparam name="T">The type of the command.</typeparam>
    public interface IOCPICommand<T> : IOCPICommand,
                                       IEquatable<T>,
                                       IComparable<T>
    {

    }

    /// <summary>
    /// An abstract OCPI command.
    /// </summary>
    /// <typeparam name="T">the type of the command.</typeparam>
    public abstract class AOCPICommand<T> : IOCPICommand<T>,
                                            IEquatable<T>,
                                            IComparable<T>
    {

        #region Properties

        /// <summary>
        /// URL that the CommandResult POST should be sent to. This URL might contain an
        /// unique identification to be able to distinguish between 'reserve now' command requests.
        /// </summary>
        [Mandatory]
        public URL             ResponseURL      { get; }

        /// <summary>
        /// The unique identification of the command.
        /// </summary>
        public Command_Id      Id               { get; }

        /// <summary>
        /// The unique request identification.
        /// </summary>
        public Request_Id      RequestId        { get; }

        /// <summary>
        /// The unique request correlation identification.
        /// </summary>
        public Correlation_Id  CorrelationId    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract OCPI command.
        /// </summary>
        /// <param name="ResponseURL">The response URL.</param>
        /// <param name="Id">An optional unique identification of the command.</param>
        /// <param name="RequestId">An optional unique request identification.</param>
        /// <param name="CorrelationId">An optional unique request correlation identification.</param>
        public AOCPICommand(URL              ResponseURL,
                            Command_Id?      Id              = null,
                            Request_Id?      RequestId       = null,
                            Correlation_Id?  CorrelationId   = null)
        {

            this.ResponseURL    = ResponseURL;
            this.Id             = Id            ?? Command_Id.    NewRandom();
            this.RequestId      = RequestId     ?? Request_Id.    NewRandom();
            this.CorrelationId  = CorrelationId ?? Correlation_Id.NewRandom();

        }

        #endregion


        public abstract Int32   CompareTo(Object? Command);

        public abstract Int32   CompareTo(T? Command);

        public abstract Boolean Equals   (T? Command);


    }

}
