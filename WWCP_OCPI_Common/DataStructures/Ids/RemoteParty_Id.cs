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

using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
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
        /// The country code.
        /// </summary>
        public CountryCode  CountryCode    { get;}

        /// <summary>
        /// The party identification.
        /// </summary>
        public Party_Id     PartyId        { get;}

        /// <summary>
        /// The party role.
        /// </summary>
        public Role         Role           { get;}


        /// <summary>
        /// This remote party identification as a CPO identification.
        /// </summary>
        public CPO_Id AsCPOId
            => CPO_Id.Parse($"{CountryCode}*{PartyId}");

        /// <summary>
        /// This remote party identification as a EMSP identification.
        /// </summary>
        public EMSP_Id AsEMSPId
            => EMSP_Id.Parse($"{CountryCode}-{PartyId}");


        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean      IsNullOrEmpty
            => CountryCode.IsNullOrEmpty || PartyId.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean      IsNotNullOrEmpty
            => CountryCode.IsNotNullOrEmpty && PartyId.IsNotNullOrEmpty;

        /// <summary>
        /// The length of the remote party identification.
        /// </summary>
        public UInt64       Length
            => (UInt64) ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remote party identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A party role.</param>
        private RemoteParty_Id(CountryCode  CountryCode,
                               Party_Id     PartyId,
                               Role         Role)
        {

            this.CountryCode  = CountryCode;
            this.PartyId      = PartyId;
            this.Role         = Role;

            unchecked
            {

                this.hashCode = this.CountryCode.GetHashCode() * 5 ^
                                this.PartyId.    GetHashCode() * 3 ^
                                this.Role.       GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (PartyIdv3,            Role)

        /// <summary>
        /// Parse the given party identification and role as a remote party identification.
        /// </summary>
        /// <param name="PartyIdv3">A party identification.</param>
        /// <param name="Role">A party role.</param>
        public static RemoteParty_Id Parse(Party_Idv3  PartyIdv3,
                                           Role        Role)

            => new (PartyIdv3.CountryCode,
                    PartyIdv3.Party,
                    Role);

        #endregion

        #region (static) Parse   (CountryCode, PartyId, Role)

        /// <summary>
        /// Parse the given country code, party identification and role as a remote party identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        /// <param name="Role">A party role.</param>
        public static RemoteParty_Id Parse(CountryCode  CountryCode,
                                           Party_Id     PartyId,
                                           Role         Role)

            => new (CountryCode,
                    PartyId,
                    Role);

        #endregion

        #region (static) From    (CPOId)

        /// <summary>
        /// Convert the given CPO identification into a remote party identification.
        /// </summary>
        /// <param name="CPOId">A CPO identification.</param>
        public static RemoteParty_Id From(CPO_Id CPOId)

            => new (CPOId.CountryCode,
                    CPOId.PartyId,
                    Role.CPO);

        #endregion

        #region (static) From    (EMSPId)

        /// <summary>
        /// Convert the given EMSP identification into a remote party identification.
        /// </summary>
        /// <param name="EMSPId">A EMSP identification.</param>
        public static RemoteParty_Id From(EMSP_Id EMSPId)

            => new (EMSPId.CountryCode,
                    EMSPId.PartyId,
                    Role.EMSP);

        #endregion

        #region (static) From    (PTPId)

        /// <summary>
        /// Convert the given PTP identification into a remote party identification.
        /// </summary>
        /// <param name="PTPId">A PTP identification.</param>
        public static RemoteParty_Id From(PTP_Id PTPId)

            => new (PTPId.CountryCode,
                    PTPId.PartyId,
                    Role.PTP);

        #endregion


        #region (static) Unknown

        /// <summary>
        /// Create a temporary unknown remote party identification.
        /// </summary>
        public static RemoteParty_Id Unknown

            => new (CountryCode.Parse("XX"),
                    Party_Id.   Parse("XXX"),
                    Role.       OTHER);

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
                        CountryCode.    TryParse(matchCollection[0].Groups[1].Value, out var countryCode) &&
                        Party_Id.       TryParse(matchCollection[0].Groups[2].Value, out var partyId)     &&
                        Role.           TryParse(matchCollection[0].Groups[3].Value, out var role))
                    {

                        RemotePartyId = new RemoteParty_Id(
                                            countryCode,
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
        public RemoteParty_Id Clone()

            => new (
                   CountryCode.Clone(),
                   PartyId.    Clone(),
                   Role.       Clone()
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

            var c = CountryCode.CompareTo(RemotePartyId.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(RemotePartyId.PartyId);

            if (c == 0)
                c = Role.       CompareTo(RemotePartyId.Role);

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

            => CountryCode.Equals(RemotePartyId.CountryCode) &&
               PartyId.    Equals(RemotePartyId.PartyId)     &&
               Role.       Equals(RemotePartyId.Role);

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

            => $"{CountryCode}-{PartyId}_{Role}";

        #endregion

    }

}
