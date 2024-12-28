﻿/*
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

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    public interface IPartyIssuedObject<TId> : WithId<TId>
        where TId : struct, IId
    {

        /// <summary>
        /// The party identification of the party that issued this object.
        /// </summary>
        [Mandatory]
        public Party_Idv3  PartyId      { get; }

        /// <summary>
        /// The version identification of the object.
        /// </summary>
        [Mandatory]
        public UInt64      VersionId    { get; }

    }
    public interface WithId<TId>// : IHasId<TId>
        where TId : struct, IId
    {

        /// <summary>
        /// The unique identification of this object.
        /// </summary>
        [Mandatory]
        public TId         Id           { get; }

    }

    public interface WithId2<TId, TParent>// : IHasId<TId>
        where TId     : struct, IId
        where TParent : class
    {

        /// <summary>
        /// The parent object.
        /// </summary>
        [Mandatory]
        public TParent?    Parent       { get; }

        /// <summary>
        /// The unique identification of this object.
        /// </summary>
        [Mandatory]
        public TId         Id           { get; }

    }

}
