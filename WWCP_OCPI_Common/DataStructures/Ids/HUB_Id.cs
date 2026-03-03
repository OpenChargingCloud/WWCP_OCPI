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

using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for HUB identifications.
    /// </summary>
    public static class HUBIdExtensions
    {

        /// <summary>
        /// Indicates whether this HUB identification is null or empty.
        /// </summary>
        /// <param name="HUBId">A HUB identification.</param>
        public static Boolean IsNullOrEmpty(this HUB_Id? HUBId)
            => !HUBId.HasValue || HUBId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this HUB identification is NOT null or empty.
        /// </summary>
        /// <param name="HUBId">A HUB identification.</param>
        public static Boolean IsNotNullOrEmpty(this HUB_Id? HUBId)
            => HUBId.HasValue && HUBId.Value.IsNotNullOrEmpty;



        public static EMSP_Id AsEMSPId(this HUB_Id HUBId)
            => EMSP_Id.Parse(HUBId.ToString());

        public static EMSP_Id? AsEMSPId(this HUB_Id? HUBId)
            => HUBId.HasValue
                   ? HUBId.Value.AsEMSPId()
                   : null;


    }


    /// <summary>
    /// The unique identification of a HUB.
    /// </summary>
    public readonly struct HUB_Id : IId<HUB_Id>
    {

        #region Data

        public static readonly Regex HUBId_RegEx = new ("^([a-zA-Z0-9]{2})\\-([a-zA-Z0-9]{2,10})$");

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
        /// Create a new HUB identification
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        private HUB_Id(CountryCode  CountryCode,
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


        /// <summary>
        /// Create a new HUB identification
        /// </summary>
        /// <param name="PartyId">A party identification.</param>
        private HUB_Id(Party_Idv3  PartyId)
        {

            this.CountryCode  = PartyId.CountryCode;
            this.PartyId      = PartyId.PartyId;

            unchecked
            {

                this.hashCode = this.CountryCode.GetHashCode() * 3 ^
                                this.PartyId.    GetHashCode();

            }

        }

        #endregion


        #region (static) From     (CountryCode, PartyId)

        /// <summary>
        /// Parse the given country code and party identification as a HUB identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public static HUB_Id From(CountryCode  CountryCode,
                                  Party_Id     PartyId)

            => new (CountryCode,
                    PartyId);

        #endregion

        #region (static) From     (             PartyId)

        /// <summary>
        /// Parse the given party identification as a HUB identification.
        /// </summary>
        /// <param name="PartyId">A party identification.</param>
        public static HUB_Id From(Party_Idv3  PartyId)

            => new (PartyId);

        #endregion

        #region (static) TryParse (CountryCode, PartyId)

        /// <summary>
        /// Parse the given country code and party identification as a HUB identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public static HUB_Id? TryParse(CountryCode?  CountryCode,
                                        Party_Id?     PartyId)

            => CountryCode.HasValue && PartyId.HasValue

                   ? From(CountryCode.Value,
                          PartyId.    Value)

                   : null;

        #endregion

        #region (static) TryParse (             PartyId)

        /// <summary>
        /// Parse the given party identification as a HUB identification.
        /// </summary>
        /// <param name="PartyId">A party identification.</param>
        public static HUB_Id? TryParse(Party_Idv3?  PartyId)

            => PartyId.HasValue
                   ? From(PartyId.Value)
                   : null;

        #endregion

        #region (static) From     (RemotePartyId)

        /// <summary>
        /// Convert the given remote party identification into a HUB identification.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        public static HUB_Id From(RemoteParty_Id RemotePartyId)

            => new (RemotePartyId.CountryCode,
                    RemotePartyId.PartyId);

        #endregion

        #region (static) From     (RemoteParty)

        ///// <summary>
        ///// Convert the given remote party into a HUB identification.
        ///// </summary>
        ///// <param name="RemoteParty">A remote party.</param>
        //public static HUB_Id From(RemoteParty RemoteParty)

        //    => new (RemoteParty.Id.CountryCode,
        //            RemoteParty.Id.PartyId);

        #endregion


        #region (static) Parse    (Text)

        /// <summary>
        /// Parse the given text as a HUB identification.
        /// </summary>
        /// <param name="Text">A text representation of a HUB identification.</param>
        public static HUB_Id Parse(String Text)
        {

            if (TryParse(Text, out var HUBId))
                return HUBId;

            throw new ArgumentException($"Invalid text representation of a HUB identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse (Text)

        /// <summary>
        /// Try to parse the given text as a HUB identification.
        /// </summary>
        /// <param name="Text">A text representation of a HUB identification.</param>
        public static HUB_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var HUBId))
                return HUBId;

            return null;

        }

        #endregion

        #region (static) TryParse (Text, out HUBId)

        /// <summary>
        /// Try to parse the given text as a HUB identification.
        /// </summary>
        /// <param name="Text">A text representation of a HUB identification.</param>
        /// <param name="HUBId">The parsed HUB identification.</param>
        public static Boolean TryParse(String Text, out HUB_Id HUBId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    var matchCollection = HUBId_RegEx.Matches(Text);

                    if (matchCollection.Count == 1 &&
                        CountryCode.    TryParse(matchCollection[0].Groups[1].Value, out var countryCode) &&
                        Party_Id.       TryParse(matchCollection[0].Groups[2].Value, out var partyId))
                    {

                        HUBId = new HUB_Id(
                                     countryCode,
                                     partyId
                                 );

                        return true;

                    }

                }
                catch
                { }
            }

            HUBId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this HUB identification.
        /// </summary>
        public HUB_Id Clone()

            => new (
                   CountryCode.Clone(),
                   PartyId.    Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (HUBId1, HUBId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HUBId1">A HUB identification.</param>
        /// <param name="HUBId2">Another HUB identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (HUB_Id HUBId1,
                                           HUB_Id HUBId2)

            => HUBId1.Equals(HUBId2);

        #endregion

        #region Operator != (HUBId1, HUBId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HUBId1">A HUB identification.</param>
        /// <param name="HUBId2">Another HUB identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (HUB_Id HUBId1,
                                           HUB_Id HUBId2)

            => !HUBId1.Equals(HUBId2);

        #endregion

        #region Operator <  (HUBId1, HUBId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HUBId1">A HUB identification.</param>
        /// <param name="HUBId2">Another HUB identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (HUB_Id HUBId1,
                                          HUB_Id HUBId2)

            => HUBId1.CompareTo(HUBId2) < 0;

        #endregion

        #region Operator <= (HUBId1, HUBId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HUBId1">A HUB identification.</param>
        /// <param name="HUBId2">Another HUB identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (HUB_Id HUBId1,
                                           HUB_Id HUBId2)

            => HUBId1.CompareTo(HUBId2) <= 0;

        #endregion

        #region Operator >  (HUBId1, HUBId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HUBId1">A HUB identification.</param>
        /// <param name="HUBId2">Another HUB identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (HUB_Id HUBId1,
                                          HUB_Id HUBId2)

            => HUBId1.CompareTo(HUBId2) > 0;

        #endregion

        #region Operator >= (HUBId1, HUBId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="HUBId1">A HUB identification.</param>
        /// <param name="HUBId2">Another HUB identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (HUB_Id HUBId1,
                                           HUB_Id HUBId2)

            => HUBId1.CompareTo(HUBId2) >= 0;

        #endregion

        #endregion

        #region IComparable<HUBId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two HUB identifications.
        /// </summary>
        /// <param name="Object">A HUB identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is HUB_Id HUBId
                   ? CompareTo(HUBId)
                   : throw new ArgumentException("The given object is not a HUB identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(HUBId)

        /// <summary>
        /// Compares two HUB identifications.
        /// </summary>
        /// <param name="HUBId">A HUB identification to compare with.</param>
        public Int32 CompareTo(HUB_Id HUBId)
{

            var c = CountryCode.CompareTo(HUBId.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(HUBId.PartyId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<HUBId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two HUB identifications for equality.
        /// </summary>
        /// <param name="Object">A HUB identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is HUB_Id HUBId &&
                   Equals(HUBId);

        #endregion

        #region Equals(HUBId)

        /// <summary>
        /// Compares two HUB identifications for equality.
        /// </summary>
        /// <param name="HUBId">A HUB identification to compare with.</param>
        public Boolean Equals(HUB_Id HUBId)

            => CountryCode.Equals(HUBId.CountryCode) &&
               PartyId.    Equals(HUBId.PartyId);

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

            => $"{CountryCode}-{PartyId}";

        #endregion

    }

}
