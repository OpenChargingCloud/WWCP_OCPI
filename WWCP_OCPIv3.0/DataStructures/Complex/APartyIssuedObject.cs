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

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPI;
using cloud.charging.open.protocols.OCPIv3_0;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// A generic party issued update.
    /// </summary>
    /// <param name="PartyId">The party identification of the party that issued this object.</param>
    /// <param name="Id">An unique identification of this object.</param>
    /// <param name="VersionId">An version of the Party Issued Object that is in the payload field.</param>
    public abstract class APartyIssuedObject<TId>(CommonAPI?  CommonAPI,
                                                  Party_Idv3    PartyId,
                                                  TId         Id,
                                                  UInt64      VersionId) : IPartyIssuedObject<TId>
        where TId   : struct, IId

    {

        #region Data

        protected readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent CommonAPI of this charging location.
        /// </summary>
        internal CommonAPI?  CommonAPI    { get; set; } = CommonAPI;


        /// <summary>
        /// The party identification of the party that issued this object.
        /// </summary>
        [Mandatory]
        public   Party_Idv3  PartyId      { get; }      = PartyId;

        /// <summary>
        /// The unique identification of this object.
        /// </summary>
        [Mandatory]
        public   TId         Id           { get; }      = Id;

        /// <summary>
        /// The version of the Party Issued Object that is in the payload field.
        /// </summary>
        [Mandatory]
        public   UInt64      VersionId    { get; }      = VersionId;

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{PartyId}:{Id} v{VersionId}";

        #endregion


        public class ABuilder(CommonAPI?   CommonAPI   = null,
                              Party_Idv3?  PartyId     = null,
                              TId?         Id          = null,
                              UInt64?      VersionId   = null)
        {

            /// <summary>
            /// The parent CommonAPI of this charging location.
            /// </summary>
            internal  CommonAPI?   CommonAPI    { get; set; } = CommonAPI;

            /// <summary>
            /// The party identification of the party that issued this object.
            /// </summary>
            [Mandatory]
            public    Party_Idv3?  PartyId      { get; set; } = PartyId;

            /// <summary>
            /// The unique identification of this object.
            /// </summary>
            [Mandatory]
            public    TId?         Id           { get; set; } = Id;

            /// <summary>
            /// The version of the Party Issued Object that is in the payload field.
            /// </summary>
            [Mandatory]
            public    UInt64?      VersionId    { get; set; } = VersionId;


            public ABuilder(Party_Idv3?  PartyId     = null,
                            TId?         Id          = null,
                            UInt64?      VersionId   = null)

                : this (null,
                        PartyId,
                        Id,
                        VersionId)

            { }


        }

    }



    /// <summary>
    /// A generic party issued update.
    /// </summary>
    /// <param name="PartyId">The party identification of the party that issued this object.</param>
    /// <param name="Id">An unique identification of this object.</param>
    /// <param name="VersionId">An version of the Party Issued Object that is in the payload field.</param>
    public abstract class APartyIssuedObject2<TId>(Party_Idv3  PartyId,
                                                   TId         Id,
                                                   UInt64      VersionId) : IPartyIssuedObject<TId>
        where TId   : struct, IId

    {

        #region Data

        protected readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The party identification of the party that issued this object.
        /// </summary>
        [Mandatory]
        public   Party_Idv3  PartyId      { get; } = PartyId;

        /// <summary>
        /// The unique identification of this object.
        /// </summary>
        [Mandatory]
        public   TId         Id           { get; } = Id;

        /// <summary>
        /// The version of the Party Issued Object that is in the payload field.
        /// </summary>
        [Mandatory]
        public   UInt64      VersionId    { get; } = VersionId;

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{PartyId}:{Id} v{VersionId}";

        #endregion


        public class ABuilder(Party_Idv3?  PartyId     = null,
                              TId?         Id          = null,
                              UInt64?      VersionId   = null)
        {

            /// <summary>
            /// The party identification of the party that issued this object.
            /// </summary>
            [Mandatory]
            public   Party_Idv3?   PartyId      { get; set; } = PartyId;

            /// <summary>
            /// The unique identification of this object.
            /// </summary>
            [Mandatory]
            public   TId?        Id           { get; set; } = Id;

            /// <summary>
            /// The version of the Party Issued Object that is in the payload field.
            /// </summary>
            [Mandatory]
            public   UInt64?     VersionId    { get; set; } = VersionId;

        }

    }


    /// <summary>
    /// A generic party issued update.
    /// </summary>
    /// <param name="Parent">The parent object.</param>
    /// <param name="Id">An unique identification of this object.</param>
    public abstract class APartyIssuedObject3<TId, TParent>(TParent?  Parent,
                                                            TId       Id) : WithId2<TId, TParent>
        where TId     : struct, IId
        where TParent : class

    {

        #region Data

        protected readonly Lock patchLock = new();

        #endregion

        #region Properties

        /// <summary>
        /// The parent object.
        /// </summary>
        [Mandatory]
        public   TParent?  Parent    { get; internal set; } = Parent;

        /// <summary>
        /// The unique identification of this object.
        /// </summary>
        [Mandatory]
        public   TId       Id        { get; }               = Id;

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Id}";

        #endregion


        public class ABuilder(TParent?  Parent   = null,
                              TId?      Id       = null)
        {

            /// <summary>
            /// The parent object.
            /// </summary>
            [Mandatory]
            public TParent?   Parent       { get; set; } = Parent;

            /// <summary>
            /// The unique identification of this object.
            /// </summary>
            [Mandatory]
            public   TId?     Id           { get; set; } = Id;

        }

    }

}
