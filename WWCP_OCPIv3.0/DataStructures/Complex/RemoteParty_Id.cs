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

using System.Text.RegularExpressions;
using cloud.charging.open.protocols.OCPI;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// Extension methods for remote party identifications.
    /// </summary>
    public static class RemotePartyIdExtensions
    {

        /// <summary>
        /// Indicates whether this remote party identification is null or empty.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        public static Boolean IsNullOrEmpty(this RemoteParty_Id? RemotePartyId)
            => !RemotePartyId.HasValue || RemotePartyId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this remote party identification is NOT null or empty.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        public static Boolean IsNotNullOrEmpty(this RemoteParty_Id? RemotePartyId)
            => RemotePartyId.HasValue && RemotePartyId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a remote party.
    /// </summary>
    public readonly struct RemoteParty_Id : IId<RemoteParty_Id>
    {

        #region Data

        public static readonly Regex RemotePartyId_RegEx = new ("^([a-zA-Z0-9]{2})\\-([a-zA-Z0-9]{2,10})_([a-zA-Z0-9]{2,10})$");

        #endregion

        #region Properties

        /// <summary>
        /// The party identification.
        /// </summary>
        public Party_Idv3  PartyId    { get;}

        /// <summary>
        /// The party role.
        /// </summary>
        public Roles     Role       { get;}


        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => PartyId.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => PartyId.IsNotNullOrEmpty;

        /// <summary>
        /// The length of the remote party identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remote party identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A party role.</param>
        private RemoteParty_Id(Party_Idv3  PartyId,
                               Roles     Role)
        {

            this.PartyId  = PartyId;
            this.Role     = Role;

            unchecked
            {

                this.hashCode = this.PartyId.GetHashCode() * 3 ^
                                this.Role.   GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (PartyId, Role)

        /// <summary>
        /// Parse the given party identification as a remote party identification.
        /// </summary>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A party role.</param>
        public static RemoteParty_Id Parse(Party_Idv3  PartyId,
                                           Roles     Role)

            => new (PartyId,
                    Role);

        #endregion

        #region (static) From    (CPOId)

        /// <summary>
        /// Convert the given CPO identification into a remote party identification.
        /// </summary>
        /// <param name="CPOId">A CPO identification.</param>
        public static RemoteParty_Id From(CPO_Id CPOId)

            => new (Party_Idv3.Parse(CPOId.PartyId.ToString()),
                    Roles.CPO);

        #endregion

        #region (static) From    (EMSPId)

        /// <summary>
        /// Convert the given EMSP identification into a remote party identification.
        /// </summary>
        /// <param name="EMSPId">A EMSP identification.</param>
        public static RemoteParty_Id From(EMSP_Id EMSPId)

            => new (Party_Idv3.Parse(EMSPId.PartyId.ToString()),
                    Roles.EMSP);

        #endregion


        #region (static) Unknown

        /// <summary>
        /// Create a temporary unknown remote party identification.
        /// </summary>
        public static RemoteParty_Id Unknown

            => new (Party_Idv3.Parse("XXX"),
                    Roles.   OTHER);

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a remote party identification.
        /// </summary>
        /// <param name="Text">A text representation of a remote party identification.</param>
        public static RemoteParty_Id Parse(String Text)
        {

            if (TryParse(Text, out var remotePartyId))
                return remotePartyId;

            throw new ArgumentException($"Invalid text representation of a remote party identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a remote party identification.
        /// </summary>
        /// <param name="Text">A text representation of a remote party identification.</param>
        public static RemoteParty_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var remotePartyId))
                return remotePartyId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out RemotePartyId)

        /// <summary>
        /// Try to parse the given text as a remote party identification.
        /// </summary>
        /// <param name="Text">A text representation of a remote party identification.</param>
        /// <param name="RemotePartyId">The parsed remote party identification.</param>
        public static Boolean TryParse(String Text, out RemoteParty_Id RemotePartyId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    var matchCollection = RemotePartyId_RegEx.Matches(Text);

                    if (matchCollection.Count == 1 &&
                        Party_Idv3.     TryParse(matchCollection[0].Groups[1].Value + matchCollection[0].Groups[2].Value, out var partyId)     &&
                        RolesExtensions.TryParse(matchCollection[0].Groups[3].Value, out var role))
                    {

                        RemotePartyId = new RemoteParty_Id(
                                            partyId,
                                            role
                                        );

                        return true;

                    }

                }
                catch
                { }
            }

            RemotePartyId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this remote party identification.
        /// </summary>
        public RemoteParty_Id Clone

            => new (
                   PartyId.Clone(),
                   Role
               );

        #endregion


        #region Operator overloading

        #region Operator == (RemotePartyId1, RemotePartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemotePartyId1">A remote party identification.</param>
        /// <param name="RemotePartyId2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (RemoteParty_Id RemotePartyId1,
                                           RemoteParty_Id RemotePartyId2)

            => RemotePartyId1.Equals(RemotePartyId2);

        #endregion

        #region Operator != (RemotePartyId1, RemotePartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemotePartyId1">A remote party identification.</param>
        /// <param name="RemotePartyId2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (RemoteParty_Id RemotePartyId1,
                                           RemoteParty_Id RemotePartyId2)

            => !RemotePartyId1.Equals(RemotePartyId2);

        #endregion

        #region Operator <  (RemotePartyId1, RemotePartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemotePartyId1">A remote party identification.</param>
        /// <param name="RemotePartyId2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (RemoteParty_Id RemotePartyId1,
                                          RemoteParty_Id RemotePartyId2)

            => RemotePartyId1.CompareTo(RemotePartyId2) < 0;

        #endregion

        #region Operator <= (RemotePartyId1, RemotePartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemotePartyId1">A remote party identification.</param>
        /// <param name="RemotePartyId2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (RemoteParty_Id RemotePartyId1,
                                           RemoteParty_Id RemotePartyId2)

            => RemotePartyId1.CompareTo(RemotePartyId2) <= 0;

        #endregion

        #region Operator >  (RemotePartyId1, RemotePartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemotePartyId1">A remote party identification.</param>
        /// <param name="RemotePartyId2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (RemoteParty_Id RemotePartyId1,
                                          RemoteParty_Id RemotePartyId2)

            => RemotePartyId1.CompareTo(RemotePartyId2) > 0;

        #endregion

        #region Operator >= (RemotePartyId1, RemotePartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemotePartyId1">A remote party identification.</param>
        /// <param name="RemotePartyId2">Another remote party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (RemoteParty_Id RemotePartyId1,
                                           RemoteParty_Id RemotePartyId2)

            => RemotePartyId1.CompareTo(RemotePartyId2) >= 0;

        #endregion

        #endregion

        #region IComparable<RemotePartyId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two remote party identifications.
        /// </summary>
        /// <param name="Object">A remote party identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RemoteParty_Id remotePartyId
                   ? CompareTo(remotePartyId)
                   : throw new ArgumentException("The given object is not a remote party identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemotePartyId)

        /// <summary>
        /// Compares two remote party identifications.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification to compare with.</param>
        public Int32 CompareTo(RemoteParty_Id RemotePartyId)
        {

            var c = PartyId.CompareTo(RemotePartyId.PartyId);

            if (c == 0)
                c = Role.   CompareTo(RemotePartyId.Role);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<RemotePartyId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two remote party identifications for equality.
        /// </summary>
        /// <param name="Object">A remote party identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is RemoteParty_Id remotePartyId &&
                   Equals(remotePartyId);

        #endregion

        #region Equals(RemotePartyId)

        /// <summary>
        /// Compares two remote party identifications for equality.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification to compare with.</param>
        public Boolean Equals(RemoteParty_Id RemotePartyId)

            => PartyId.Equals(RemotePartyId.PartyId) &&
               Role.   Equals(RemotePartyId.Role);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{PartyId}_{Role}";

        #endregion

    }

}
