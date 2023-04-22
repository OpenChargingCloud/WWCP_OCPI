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

using System;
using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2_1
{

    /// <summary>
    /// The unique identification of a remote party.
    /// </summary>
    public readonly struct RemoteParty_Id : IId<RemoteParty_Id>
    {

        #region Data

        public static readonly Regex RemotePartyId_RegEx = new ("^([a-zA-Z0-9]{2,5})\\-([a-zA-Z0-9]{2,10})_([a-zA-Z0-9]{2,10})$");

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the remote party identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);


        public CountryCode  CountryCode    { get;}
        public Party_Id     PartyId        { get;}
        public Roles        Role           { get;}

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new remote party identification based on the given text.
        /// </summary>
        private RemoteParty_Id(CountryCode  CountryCode,
                               Party_Id     PartyId,
                               Roles        Role)
        {

            this.CountryCode  = CountryCode;
            this.PartyId      = PartyId;
            this.Role         = Role;
            this.InternalId   = String.Concat(CountryCode, "-", PartyId, "_", Role);

        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a remote party identification.
        /// </summary>
        /// <param name="Text">A text representation of a remote party identification.</param>
        public static RemoteParty_Id Parse(String Text)
        {

            if (TryParse(Text, out var locationId))
                return locationId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a remote party identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a remote party identification is invalid!", nameof(Text));

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

                    var MatchCollection = RemotePartyId_RegEx.Matches(Text);

                    if (MatchCollection.Count == 1 &&
                        CountryCode.    TryParse(MatchCollection[0].Groups[1].Value, out var countryCode) &&
                        Party_Id.       TryParse(MatchCollection[0].Groups[2].Value, out var partyId)     &&
                        RolesExtensions.TryParse(MatchCollection[0].Groups[3].Value, out var role))
                    {

                        RemotePartyId = new RemoteParty_Id(countryCode,
                                                           partyId,
                                                           role);

                        return true;

                    }

                }
                catch (Exception)
                { }
            }

            RemotePartyId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this remote party identification.
        /// </summary>
        public RemoteParty_Id Clone

            => new RemoteParty_Id(
                   CountryCode.Clone,
                   PartyId.Clone,
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

            => !(RemotePartyId1 == RemotePartyId2);

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

            => !(RemotePartyId1 > RemotePartyId2);

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

            => !(RemotePartyId1 < RemotePartyId2);

        #endregion

        #endregion

        #region IComparable<RemotePartyId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is RemoteParty_Id locationId
                   ? CompareTo(locationId)
                   : throw new ArgumentException("The given object is not a remote party identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(RemotePartyId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="RemotePartyId">An object to compare with.</param>
        public Int32 CompareTo(RemoteParty_Id RemotePartyId)

            => String.Compare(InternalId,
                              RemotePartyId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<RemotePartyId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is RemoteParty_Id locationId &&
                   Equals(locationId);

        #endregion

        #region Equals(RemotePartyId)

        /// <summary>
        /// Compares two remote party identifications for equality.
        /// </summary>
        /// <param name="RemotePartyId">An remote party identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(RemoteParty_Id RemotePartyId)

            => String.Equals(InternalId,
                             RemotePartyId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
