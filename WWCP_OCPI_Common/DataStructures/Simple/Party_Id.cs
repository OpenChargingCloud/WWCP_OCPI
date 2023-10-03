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

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for party identifications.
    /// </summary>
    public static class PartyIdExtensions
    {

        /// <summary>
        /// Indicates whether this party identification is null or empty.
        /// </summary>
        /// <param name="PartyId">A party identification.</param>
        public static Boolean IsNullOrEmpty(this Party_Id? PartyId)
            => !PartyId.HasValue || PartyId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this party identification is NOT null or empty.
        /// </summary>
        /// <param name="PartyId">A party identification.</param>
        public static Boolean IsNotNullOrEmpty(this Party_Id? PartyId)
            => PartyId.HasValue && PartyId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a party.
    /// CiString(3)
    /// </summary>
    public readonly struct Party_Id : IId<Party_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this party identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this party identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the party identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new party identification based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a party identification.</param>
        private Party_Id(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a party identification.
        /// </summary>
        /// <param name="Text">A text representation of a party identification.</param>
        public static Party_Id Parse(String Text)
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
        public static Party_Id? TryParse(String Text)
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
        public static Boolean TryParse(String Text, out Party_Id PartyId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                Text.Length >= 1        &&
                Text.Length <= 3)
            {
                try
                {
                    PartyId = new Party_Id(Text);
                    return true;
                }
                catch
                { }
            }

            PartyId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this party identification.
        /// </summary>
        public Party_Id Clone

            => new (
                   new String(InternalId?.ToCharArray())
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
        public static Boolean operator == (Party_Id PartyId1,
                                           Party_Id PartyId2)

            => PartyId1.Equals(PartyId2);

        #endregion

        #region Operator != (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Party_Id PartyId1,
                                           Party_Id PartyId2)

            => !PartyId1.Equals(PartyId2);

        #endregion

        #region Operator <  (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Party_Id PartyId1,
                                          Party_Id PartyId2)

            => PartyId1.CompareTo(PartyId2) < 0;

        #endregion

        #region Operator <= (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Party_Id PartyId1,
                                           Party_Id PartyId2)

            => PartyId1.CompareTo(PartyId2) <= 0;

        #endregion

        #region Operator >  (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Party_Id PartyId1,
                                          Party_Id PartyId2)

            => PartyId1.CompareTo(PartyId2) > 0;

        #endregion

        #region Operator >= (PartyId1, PartyId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PartyId1">A party identification.</param>
        /// <param name="PartyId2">Another party identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Party_Id PartyId1,
                                           Party_Id PartyId2)

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

            => Object is Party_Id partyId
                   ? CompareTo(partyId)
                   : throw new ArgumentException("The given object is not a party identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PartyId)

        /// <summary>
        /// Compares two party identifications.
        /// </summary>
        /// <param name="PartyId">A party identification to compare with.</param>
        public Int32 CompareTo(Party_Id PartyId)

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

            => Object is Party_Id partyId &&
                   Equals(partyId);

        #endregion

        #region Equals(PartyId)

        /// <summary>
        /// Compares two party identifications for equality.
        /// </summary>
        /// <param name="PartyId">A party identification to compare with.</param>
        public Boolean Equals(Party_Id PartyId)

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

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
