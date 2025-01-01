/*
 * Copyright (c) 2015-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// A generic party issued object reference.
    /// </summary>
    /// <param name="Id">An unique identification of this object.</param>
    public abstract class PartyIssuedObjectReference<TId>(TId Id) : IHasId<TId>
        where TId : struct, IId
    {

        #region Properties

        /// <summary>
        /// The unique identification of this object.
        /// </summary>
        [Mandatory]
        public TId  Id    { get; } = Id;

        #endregion


        #region IComparable<PartyRole> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two party issued object references.
        /// </summary>
        /// <param name="Object">A party issued object reference to compare with.</param>
        public virtual Int32 CompareTo(Object? Object)

            => Object is PartyIssuedObjectReference<TId> partyIssuedObjectReference
                   ? CompareTo(partyIssuedObjectReference)
                   : throw new ArgumentException("The given object is not a party issued object reference of the specified type!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PartyRole)

        /// <summary>
        /// Compares two party issued object references.
        /// </summary>
        /// <param name="PartyRole">A party issued object reference to compare with.</param>
        public virtual Int32 CompareTo(PartyIssuedObjectReference<TId> PartyIssuedObjectReference)
        {

            if (PartyIssuedObjectReference is null)
                throw new ArgumentNullException(nameof(PartyIssuedObjectReference), "The given party issued object reference must not be null!");

            return PartyIssuedObjectReference.Id.CompareTo(PartyIssuedObjectReference.Id);

        }

        #endregion

        #endregion


        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
