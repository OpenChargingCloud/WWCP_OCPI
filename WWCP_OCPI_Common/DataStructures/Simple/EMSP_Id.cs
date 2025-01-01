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

using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPI
{

    /// <summary>
    /// Extension methods for EMSP identifications.
    /// </summary>
    public static class EMSPIdExtensions
    {

        /// <summary>
        /// Indicates whether this EMSP identification is null or empty.
        /// </summary>
        /// <param name="EMSPId">An EMSP identification.</param>
        public static Boolean IsNullOrEmpty(this EMSP_Id? EMSPId)
            => !EMSPId.HasValue || EMSPId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this EMSP identification is NOT null or empty.
        /// </summary>
        /// <param name="EMSPId">An EMSP identification.</param>
        public static Boolean IsNotNullOrEmpty(this EMSP_Id? EMSPId)
            => EMSPId.HasValue && EMSPId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of an EMSP.
    /// </summary>
    public readonly struct EMSP_Id : IId<EMSP_Id>
    {

        #region Data

        public static readonly Regex EMSPId_RegEx = new ("^([a-zA-Z0-9]{2})\\-([a-zA-Z0-9]{2,10})$");

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
        /// Create a new EMSP identification
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        private EMSP_Id(CountryCode  CountryCode,
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
        /// Parse the given country code and party identification as an EMSP identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public static EMSP_Id Parse(CountryCode  CountryCode,
                                    Party_Id     PartyId)

            => new (CountryCode,
                    PartyId);

        #endregion

        #region (static) TryParse(CountryCode, PartyId)

        /// <summary>
        /// Parse the given country code and party identification as an EMSP identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public static EMSP_Id? TryParse(CountryCode?  CountryCode,
                                     Party_Id?     PartyId)

            => CountryCode.HasValue && PartyId.HasValue

                   ? Parse(CountryCode.Value,
                           PartyId.    Value)

                   : null;

        #endregion

        #region (static) From    (RemotePartyId)

        /// <summary>
        /// Convert the given remote party identification into an EMSP identification.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        public static EMSP_Id From(RemoteParty_Id RemotePartyId)

            => new (RemotePartyId.CountryCode,
                    RemotePartyId.PartyId);

        #endregion

        #region (static) From    (RemoteParty)

        ///// <summary>
        ///// Convert the given remote party into an EMSP identification.
        ///// </summary>
        ///// <param name="RemoteParty">A remote party.</param>
        //public static EMSP_Id From(RemoteParty RemoteParty)

        //    => new (RemoteParty.Id.CountryCode,
        //            RemoteParty.Id.PartyId);

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as an EMSP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMSP identification.</param>
        public static EMSP_Id Parse(String Text)
        {

            if (TryParse(Text, out var EMSPId))
                return EMSPId;

            throw new ArgumentException($"Invalid text representation of an EMSP identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as an EMSP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMSP identification.</param>
        public static EMSP_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var EMSPId))
                return EMSPId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out EMSPId)

        /// <summary>
        /// Try to parse the given text as an EMSP identification.
        /// </summary>
        /// <param name="Text">A text representation of an EMSP identification.</param>
        /// <param name="EMSPId">The parsed EMSP identification.</param>
        public static Boolean TryParse(String Text, out EMSP_Id EMSPId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    var matchCollection = EMSPId_RegEx.Matches(Text);

                    if (matchCollection.Count == 1 &&
                        CountryCode.    TryParse(matchCollection[0].Groups[1].Value, out var countryCode) &&
                        Party_Id.       TryParse(matchCollection[0].Groups[2].Value, out var partyId))
                    {

                        EMSPId = new EMSP_Id(countryCode,
                                           partyId);

                        return true;

                    }

                }
                catch
                { }
            }

            EMSPId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this EMSP identification.
        /// </summary>
        public EMSP_Id Clone()

            => new (
                   CountryCode.Clone(),
                   PartyId.    Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (EMSP_Id EMSPId1,
                                           EMSP_Id EMSPId2)

            => EMSPId1.Equals(EMSPId2);

        #endregion

        #region Operator != (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (EMSP_Id EMSPId1,
                                           EMSP_Id EMSPId2)

            => !EMSPId1.Equals(EMSPId2);

        #endregion

        #region Operator <  (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (EMSP_Id EMSPId1,
                                          EMSP_Id EMSPId2)

            => EMSPId1.CompareTo(EMSPId2) < 0;

        #endregion

        #region Operator <= (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (EMSP_Id EMSPId1,
                                           EMSP_Id EMSPId2)

            => EMSPId1.CompareTo(EMSPId2) <= 0;

        #endregion

        #region Operator >  (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (EMSP_Id EMSPId1,
                                          EMSP_Id EMSPId2)

            => EMSPId1.CompareTo(EMSPId2) > 0;

        #endregion

        #region Operator >= (EMSPId1, EMSPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EMSPId1">An EMSP identification.</param>
        /// <param name="EMSPId2">Another EMSP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (EMSP_Id EMSPId1,
                                           EMSP_Id EMSPId2)

            => EMSPId1.CompareTo(EMSPId2) >= 0;

        #endregion

        #endregion

        #region IComparable<EMSPId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two EMSP identifications.
        /// </summary>
        /// <param name="Object">An EMSP identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is EMSP_Id emspId
                   ? CompareTo(emspId)
                   : throw new ArgumentException("The given object is not an EMSP identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EMSPId)

        /// <summary>
        /// Compares two EMSP identifications.
        /// </summary>
        /// <param name="EMSPId">An EMSP identification to compare with.</param>
        public Int32 CompareTo(EMSP_Id EMSPId)
{

            var c = CountryCode.CompareTo(EMSPId.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(EMSPId.PartyId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<EMSPId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two EMSP identifications for equality.
        /// </summary>
        /// <param name="Object">An EMSP identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is EMSP_Id emspId &&
                   Equals(emspId);

        #endregion

        #region Equals(EMSPId)

        /// <summary>
        /// Compares two EMSP identifications for equality.
        /// </summary>
        /// <param name="EMSPId">An EMSP identification to compare with.</param>
        public Boolean Equals(EMSP_Id EMSPId)

            => CountryCode.Equals(EMSPId.CountryCode) &&
               PartyId.    Equals(EMSPId.PartyId);

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
