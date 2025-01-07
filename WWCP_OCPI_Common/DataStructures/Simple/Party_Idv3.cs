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
using cloud.charging.open.protocols.OCPI;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for party identifications.
    /// </summary>
    public static class PartyIdv3Extensions
    {

        /// <summary>
        /// Indicates whether this party identification is null or empty.
        /// </summary>
        /// <param name="PartyId">A party identification.</param>
        public static Boolean IsNullOrEmpty(this Party_Idv3? PartyId)
            => !PartyId.HasValue || PartyId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this party identification is NOT null or empty.
        /// </summary>
        /// <param name="PartyId">A party identification.</param>
        public static Boolean IsNotNullOrEmpty(this Party_Idv3? PartyId)
            => PartyId.HasValue && PartyId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a party, e.g. "DEGDF"
    /// </summary>
    public readonly struct Party_Idv3 : IId<Party_Idv3>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        /// <summary>
        /// The regular expression for parsing party identifications.
        /// </summary>
        public static readonly Regex PartyId_RegEx = new ("^([a-zA-Z0-9]{5})$");

        #endregion

        #region Properties

        /// <summary>
        /// The country code.
        /// </summary>
        public CountryCode  CountryCode
            => CountryCode.Parse(InternalId[..2]);

        /// <summary>
        /// The party identification.
        /// </summary>
        public Party_Id     Party
            => Party_Id.Parse(InternalId.Substring(2, 3));


        /// <summary>
        /// Indicates whether this party identification is null or empty.
        /// </summary>
        public Boolean      IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this party identification is NOT null or empty.
        /// </summary>
        public Boolean      IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the party identification.
        /// </summary>
        public UInt64       Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new party identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a party identification.</param>
        private Party_Idv3(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (CountryCode, PartyId)

        /// <summary>
        /// Parse the given country code and party identification as a party identification.
        /// </summary>
        /// <param name="CountryCode">The country code.</param>
        /// <param name="PartyId">The party identification.</param>
        public static Party_Idv3 From(CountryCode  CountryCode,
                                      Party_Id     PartyId)

            => new ($"{CountryCode}{PartyId}");

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a party identification.
        /// </summary>
        /// <param name="Text">A text representation of a party identification.</param>
        public static Party_Idv3 Parse(String Text)
        {

            if (TryParse(Text, out var partyId))
                return partyId;

            throw new ArgumentException($"Invalid text representation of a party identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a party identification.
        /// </summary>
        /// <param name="Text">A text representation of a party identification.</param>
        public static Party_Idv3? TryParse(String Text)
        {

            if (TryParse(Text, out var partyId))
                return partyId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PartyId)

        /// <summary>
        /// Try to parse the given text as a party identification.
        /// </summary>
        /// <param name="Text">A text representation of a party identification.</param>
        /// <param name="PartyId">The parsed party identification.</param>
        public static Boolean TryParse(String Text, out Party_Idv3 PartyId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                Text.Length == 5 &&
                PartyId_RegEx.IsMatch(Text))
            {
                try
                {
                    PartyId = new Party_Idv3(Text);
                    return true;
                }
                catch
                { }
            }

            PartyId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this party identification.
        /// </summary>
        public Party_Idv3 Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Party_Idv3 PartyId1,
                                           Party_Idv3 PartyId2)

            => PartyId1.Equals(PartyId2);

        #endregion

        #region Operator != (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Party_Idv3 PartyId1,
                                           Party_Idv3 PartyId2)

            => !PartyId1.Equals(PartyId2);

        #endregion

        #region Operator <  (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Party_Idv3 PartyId1,
                                          Party_Idv3 PartyId2)

            => PartyId1.CompareTo(PartyId2) < 0;

        #endregion

        #region Operator <= (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Party_Idv3 PartyId1,
                                           Party_Idv3 PartyId2)

            => PartyId1.CompareTo(PartyId2) <= 0;

        #endregion

        #region Operator >  (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Party_Idv3 PartyId1,
                                          Party_Idv3 PartyId2)

            => PartyId1.CompareTo(PartyId2) > 0;

        #endregion

        #region Operator >= (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Party_Idv3 PartyId1,
                                           Party_Idv3 PartyId2)

            => PartyId1.CompareTo(PartyId2) >= 0;

        #endregion

        #endregion

        #region IComparable<PartyId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two party identifications.
        /// </summary>
        /// <param name="Object">A party identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Party_Idv3 partyId
                   ? CompareTo(partyId)
                   : throw new ArgumentException("The given object is not a party identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PartyId)

        /// <summary>
        /// Compares two party identifications.
        /// </summary>
        /// <param name="PartyId">A party identification to compare with.</param>
        public Int32 CompareTo(Party_Idv3 PartyId)

            => String.Compare(InternalId,
                              PartyId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<PartyId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two party identifications for equality.
        /// </summary>
        /// <param name="Object">A party identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Party_Idv3 partyId &&
                   Equals(partyId);

        #endregion

        #region Equals(PartyId)

        /// <summary>
        /// Compares two party identifications for equality.
        /// </summary>
        /// <param name="PartyId">A party identification to compare with.</param>
        public Boolean Equals(Party_Idv3 PartyId)

            => String.Equals(InternalId,
                             PartyId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

        #endregion

        #region ToString(Role)

        /// <summary>
        /// Return a role-based text representation of this object.
        /// </summary>
        public String ToString(Roles Role)

            => $"{CountryCode}{(Role == Roles.EMSP ? "-" : "*")}{Party}";

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
