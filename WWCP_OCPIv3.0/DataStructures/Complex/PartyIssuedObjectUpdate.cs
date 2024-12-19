/*
 * Copyright (c) 2015-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A generic party issued update.
    /// </summary>
    /// <param name="Id">An unique identification of this object.</param>
    /// <param name="Version">An version of the Party Issued Object that is in the payload field.</param>
    /// <param name="Payload">An Party Issued Object at the version given in the version field.</param>
    public abstract class PartyIssuedObjectUpdate<TId, TData>(Party_Id  IssuerParty,
                                                              TId       Id,
                                                              UInt64    Version,
                                                              TData     Payload) : PartyIssuedObjectReference<TId>(Id)
        where TId   : struct, IId
        where TData : class

    {

        #region Properties

        /// <summary>
        /// The party ID of the party that issued this object.
        /// </summary>
        [Mandatory]
        public Party_Id  IssuerParty    { get; } = IssuerParty;

        /// <summary>
        /// The version of the Party Issued Object that is in the payload field.
        /// </summary>
        [Mandatory]
        public UInt64    Version        { get; } = Version;

        /// <summary>
        /// The representation of the Party Issued Object at the version given in the version field.
        /// </summary>
        [Mandatory]
        public TData     Payload        { get; } = Payload;

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Id} v{Version}";

        #endregion

    }

}
