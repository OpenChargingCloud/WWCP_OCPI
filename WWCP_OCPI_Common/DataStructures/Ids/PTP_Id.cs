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
    /// Extension methods for PTP identifications.
    /// </summary>
    public static class PTPIdExtensions
    {

        /// <summary>
        /// Indicates whether this PTP identification is null or empty.
        /// </summary>
        /// <param name="PTPId">A PTP identification.</param>
        public static Boolean IsNullOrEmpty(this PTP_Id? PTPId)
            => !PTPId.HasValue || PTPId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this PTP identification is NOT null or empty.
        /// </summary>
        /// <param name="PTPId">A PTP identification.</param>
        public static Boolean IsNotNullOrEmpty(this PTP_Id? PTPId)
            => PTPId.HasValue && PTPId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a PTP.
    /// </summary>
    public readonly struct PTP_Id : IId<PTP_Id>
    {

        #region Data

        public static readonly Regex PTPId_RegEx = new ("^([a-zA-Z0-9]{2})\\*([a-zA-Z0-9]{2,10})$");

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
        /// Create a new PTP identification
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        private PTP_Id(CountryCode  CountryCode,
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
        /// Parse the given country code and party identification as a PTP identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public static PTP_Id Parse(CountryCode  CountryCode,
                                   Party_Id     PartyId)

            => new (CountryCode,
                    PartyId);

        #endregion

        #region (static) TryParse(CountryCode, PartyId)

        /// <summary>
        /// Parse the given country code and party identification as a PTP identification.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        /// <param name="PartyId">A party identification.</param>
        public static PTP_Id? TryParse(CountryCode?  CountryCode,
                                       Party_Id?     PartyId)

            => CountryCode.HasValue && PartyId.HasValue

                   ? Parse(CountryCode.Value,
                           PartyId.    Value)

                   : null;

        #endregion

        #region (static) From    (RemotePartyId)

        /// <summary>
        /// Convert the given remote party identification into a PTP identification.
        /// </summary>
        /// <param name="RemotePartyId">A remote party identification.</param>
        public static PTP_Id From(RemoteParty_Id RemotePartyId)

            => new (RemotePartyId.CountryCode,
                    RemotePartyId.PartyId);

        #endregion

        #region (static) From    (RemoteParty)

        ///// <summary>
        ///// Convert the given remote party into a PTP identification.
        ///// </summary>
        ///// <param name="RemoteParty">A remote party.</param>
        //public static PTP_Id From(RemoteParty RemoteParty)

        //    => new (RemoteParty.Id.CountryCode,
        //            RemoteParty.Id.PartyId);

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a PTP identification.
        /// </summary>
        /// <param name="Text">A text representation of a PTP identification.</param>
        public static PTP_Id Parse(String Text)
        {

            if (TryParse(Text, out var PTPId))
                return PTPId;

            throw new ArgumentException($"Invalid text representation of a PTP identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a PTP identification.
        /// </summary>
        /// <param name="Text">A text representation of a PTP identification.</param>
        public static PTP_Id? TryParse(String Text)
        {

            if (TryParse(Text, out var PTPId))
                return PTPId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out PTPId)

        /// <summary>
        /// Try to parse the given text as a PTP identification.
        /// </summary>
        /// <param name="Text">A text representation of a PTP identification.</param>
        /// <param name="PTPId">The parsed PTP identification.</param>
        public static Boolean TryParse(String Text, out PTP_Id PTPId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    var matchCollection = PTPId_RegEx.Matches(Text);

                    if (matchCollection.Count == 1 &&
                        CountryCode.    TryParse(matchCollection[0].Groups[1].Value, out var countryCode) &&
                        Party_Id.       TryParse(matchCollection[0].Groups[2].Value, out var partyId))
                    {

                        PTPId = new PTP_Id(countryCode,
                                           partyId);

                        return true;

                    }

                }
                catch
                { }
            }

            PTPId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this PTP identification.
        /// </summary>
        public PTP_Id Clone()

            => new (
                   CountryCode.Clone(),
                   PartyId.    Clone()
               );

        #endregion


        #region Operator overloading

        #region Operator == (PTPId1, PTPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PTPId1">A PTP identification.</param>
        /// <param name="PTPId2">Another PTP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (PTP_Id PTPId1,
                                           PTP_Id PTPId2)

            => PTPId1.Equals(PTPId2);

        #endregion

        #region Operator != (PTPId1, PTPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PTPId1">A PTP identification.</param>
        /// <param name="PTPId2">Another PTP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (PTP_Id PTPId1,
                                           PTP_Id PTPId2)

            => !PTPId1.Equals(PTPId2);

        #endregion

        #region Operator <  (PTPId1, PTPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PTPId1">A PTP identification.</param>
        /// <param name="PTPId2">Another PTP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (PTP_Id PTPId1,
                                          PTP_Id PTPId2)

            => PTPId1.CompareTo(PTPId2) < 0;

        #endregion

        #region Operator <= (PTPId1, PTPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PTPId1">A PTP identification.</param>
        /// <param name="PTPId2">Another PTP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (PTP_Id PTPId1,
                                           PTP_Id PTPId2)

            => PTPId1.CompareTo(PTPId2) <= 0;

        #endregion

        #region Operator >  (PTPId1, PTPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PTPId1">A PTP identification.</param>
        /// <param name="PTPId2">Another PTP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (PTP_Id PTPId1,
                                          PTP_Id PTPId2)

            => PTPId1.CompareTo(PTPId2) > 0;

        #endregion

        #region Operator >= (PTPId1, PTPId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PTPId1">A PTP identification.</param>
        /// <param name="PTPId2">Another PTP identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (PTP_Id PTPId1,
                                           PTP_Id PTPId2)

            => PTPId1.CompareTo(PTPId2) >= 0;

        #endregion

        #endregion

        #region IComparable<PTPId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two PTP identifications.
        /// </summary>
        /// <param name="Object">A PTP identification to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is PTP_Id cpoId
                   ? CompareTo(cpoId)
                   : throw new ArgumentException("The given object is not a PTP identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(PTPId)

        /// <summary>
        /// Compares two PTP identifications.
        /// </summary>
        /// <param name="PTPId">A PTP identification to compare with.</param>
        public Int32 CompareTo(PTP_Id PTPId)
{

            var c = CountryCode.CompareTo(PTPId.CountryCode);

            if (c == 0)
                c = PartyId.    CompareTo(PTPId.PartyId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<PTPId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two PTP identifications for equality.
        /// </summary>
        /// <param name="Object">A PTP identification to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PTP_Id cpoId &&
                   Equals(cpoId);

        #endregion

        #region Equals(PTPId)

        /// <summary>
        /// Compares two PTP identifications for equality.
        /// </summary>
        /// <param name="PTPId">A PTP identification to compare with.</param>
        public Boolean Equals(PTP_Id PTPId)

            => CountryCode.Equals(PTPId.CountryCode) &&
               PartyId.    Equals(PTPId.PartyId);

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
