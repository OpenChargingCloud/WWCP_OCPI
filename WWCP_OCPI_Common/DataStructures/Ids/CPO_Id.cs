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
    /// Extension methods for CPO identifications.
    /// </summary>
    public static class CPOIdExtensions
    {

        /// <summary>
        /// Indicates whether this CPO identification is null or empty.
        /// </summary>
        /// <param name="CPOId">A CPO identification.</param>
        public static Boolean IsNullOrEmpty(this CPO_Id? CPOId)
            => !CPOId.HasValue || CPOId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this CPO identification is NOT null or empty.
        /// </summary>
        /// <param name="CPOId">A CPO identification.</param>
        public static Boolean IsNotNullOrEmpty(this CPO_Id? CPOId)
            => CPOId.HasValue && CPOId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a CPO.
    /// </summary>
    public readonly struct CPO_Id : IId<CPO_Id>
    {

        #region Data

        public static readonly Regex CPOId_RegEx = new ("^([a-zA-Z0-9]{2})\\*([a-zA-Z0-9]{2,10})$");

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
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => CountryCode.IsNullOrEmpty || PartyId.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => CountryCode.IsNotNullOrEmpty && PartyId.IsNotNullOrEmpty;

        /// <summary>
        /// The length of the remote party identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) ToString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new CPO identification
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        private CPO_Id(CountryCode  CountryCode,
                       Party_Id     PartyId)
        {

            this.CountryCode  = CountryCode;
            this.PartyId      = PartyId;

            unchecked
            {

                this.hashCode = this.CountryCode.GetHashCode() * 3 ^
                                this.PartyId.    GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (CountryCode, PartyId)

        /// <summary>
        /// Parse the given country code and party identification as a CPO identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public static CPO_Id Parse(CountryCode  CountryCode,
                                   Party_Id     PartyId)

            => new (CountryCode,
                    PartyId);

        #endregion

        #region (static) TryParse(CountryCode, PartyId)

        /// <summary>
        /// Parse the given country code and party identification as a CPO identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public static CPO_Id? TryParse(CountryCode?  CountryCode,
                                       Party_Id?     PartyId)

            => CountryCode.HasValue && PartyId.HasValue

                   ? Parse(CountryCode.Value,
                           PartyId.    Value)

                   : null;

        #endregion

        #region (static) From    (RemotePartyId)

        /// <summary>
        /// Convert the given remote party identification into a CPO identification.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        public static CPO_Id From(RemoteParty_Id RemotePartyId)

            => new (RemotePartyId.CountryCode,
                    RemotePartyId.PartyId);

        #endregion

        #region (static) From    (RemoteParty)

        ///// <summary>
        ///// Convert the given remote party into a CPO identification.
        ///// </summary>
        ///// <param name="RemoteParty">A remote party.</param>
        //public static CPO_Id From(RemoteParty RemoteParty)

        //    => new (RemoteParty.Id.CountryCode,
        //            RemoteParty.Id.PartyId);

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a CPO identification.
        /// </summary>
        /// <param name="Text">A text representation of a CPO identification.</param>
        public static CPO_Id Parse(String Text)
        {

            if (TryParse(Text, out var CPOId))
                return CPOId;

            throw new ArgumentException($"Invalid text representation of a CPO identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a CPO identification.
        /// </summary>
        /// <param name="Text">A text representation of a CPO identification.</param>
        public static CPO_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var CPOId))
                return CPOId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CPOId)

        /// <summary>
        /// Try to parse the given text as a CPO identification.
        /// </summary>
        /// <param name="Text">A text representation of a CPO identification.</param>
        /// <param name="CPOId">The parsed CPO identification.</param>
        public static Boolean TryParse(String Text, out CPO_Id CPOId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    var matchCollection = CPOId_RegEx.Matches(Text);

                    if (matchCollection.Count == 1 &&
                        CountryCode.    TryParse(matchCollection[0].Groups[1].Value, out var countryCode) &&
                        Party_Id.       TryParse(matchCollection[0].Groups[2].Value, out var partyId))
                    {

                        CPOId = new CPO_Id(countryCode,
                                           partyId);

                        return true;

                    }

                }
                catch
                { }
            }

            CPOId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this CPO identification.
        /// </summary>
        public CPO_Id Clone()

            => new (
                   CountryCode.Clone(),
                   PartyId.    Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CPO_Id CPOId1,
                                           CPO_Id CPOId2)

            => CPOId1.Equals(CPOId2);

        #endregion

        #region Operator != (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CPO_Id CPOId1,
                                           CPO_Id CPOId2)

            => !CPOId1.Equals(CPOId2);

        #endregion

        #region Operator <  (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CPO_Id CPOId1,
                                          CPO_Id CPOId2)

            => CPOId1.CompareTo(CPOId2) < 0;

        #endregion

        #region Operator <= (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CPO_Id CPOId1,
                                           CPO_Id CPOId2)

            => CPOId1.CompareTo(CPOId2) <= 0;

        #endregion

        #region Operator >  (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CPO_Id CPOId1,
                                          CPO_Id CPOId2)

            => CPOId1.CompareTo(CPOId2) > 0;

        #endregion

        #region Operator >= (CPOId1, CPOId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CPOId1">A CPO identification.</param>
        /// <param name="CPOId2">Another CPO identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CPO_Id CPOId1,
                                           CPO_Id CPOId2)

            => CPOId1.CompareTo(CPOId2) >= 0;

        #endregion

        #endregion

        #region IComparable<CPOId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two CPO identifications.
        /// </summary>
        /// <param name="Object">A CPO identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CPO_Id cpoId
                   ? CompareTo(cpoId)
                   : throw new ArgumentException("The given object is not a CPO identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CPOId)

        /// <summary>
        /// Compares two CPO identifications.
        /// </summary>
        /// <param name="CPOId">A CPO identification to compare with.</param>
        public Int32 CompareTo(CPO_Id CPOId)
{

            var c = CountryCode.CompareTo(CPOId.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(CPOId.PartyId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<CPOId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two CPO identifications for equality.
        /// </summary>
        /// <param name="Object">A CPO identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CPO_Id cpoId &&
                   Equals(cpoId);

        #endregion

        #region Equals(CPOId)

        /// <summary>
        /// Compares two CPO identifications for equality.
        /// </summary>
        /// <param name="CPOId">A CPO identification to compare with.</param>
        public Boolean Equals(CPO_Id CPOId)

            => CountryCode.Equals(CPOId.CountryCode) &&
               PartyId.    Equals(CPOId.PartyId);

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

            => $"{CountryCode}*{PartyId}";

        #endregion

    }

}
