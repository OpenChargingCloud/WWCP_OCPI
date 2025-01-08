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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.OCPI;

#endregion

namespace cloud.charging.open.protocols.OCPIv3_0
{

    /// <summary>
    /// The address of a location.
    /// </summary>
    public class Address : IEquatable<Address>,
                           IComparable<Address>,
                           IComparable
    {

        #region Properties

        /// <summary>
        /// Street/block name and house number if available.
        /// </summary>
        [Mandatory]
        public String         Street         { get; }

        /// <summary>
        /// City or town.
        /// </summary>
        [Mandatory]
        public String         City           { get; }

        /// <summary>
        /// The optional postal code.
        /// </summary>
        [Optional]
        public String?        PostalCode     { get; }

        /// <summary>
        /// The optional state or province.
        /// This is intended to be used only in locales where a state or province is commonly given in addresses.
        /// This field would typically be filled for Locations in the United States of America and be left unset for Locations in The Netherlands for example.
        /// </summary>
        [Optional]
        public String?        State          { get; }

        /// <summary>
        /// ISO 3166-1 alpha-3 code for the country.
        /// </summary>
        [Mandatory]
        public CountryCode    Country        { get; }

        /// <summary>
        /// Coordinates of the Location.
        /// This could be the geographical location of one or more Charging Stations within a facility, but it can also be the entrance of a parking or
        /// other facility where Charging Stations are located.It is up to the CPO to use the point that makes the most sense to a Driver for a given
        /// Location.Once arrived at the Location’s coordinates, any further instructions to reach a Charging Station from the Location coordinates are
        /// stored in the Charging Station object itself (such as the floor number, visual identification or written instructions).
        /// </summary>
        [Mandatory]
        public GeoCoordinate  Coordinates    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new location address.
        /// </summary>
        /// <param name="Street">Street/block name and house number if available.</param>
        /// <param name="City">City or town.</param>
        /// <param name="Country">ISO 3166-1 alpha-3 code for the country.</param>
        /// <param name="Coordinates">Coordinates of the Location.</param>
        /// <param name="PostalCode">An optional postal code.</param>
        /// <param name="State">An optional state or province.</param>
        public Address(String         Street,
                       String         City,
                       CountryCode    Country,
                       GeoCoordinate  Coordinates,
                       String?        PostalCode   = null,
                       String?        State        = null)
        {

            this.Street       = Street.     Trim();
            this.City         = City.       Trim();
            this.Country      = Country;
            this.Coordinates  = Coordinates;
            this.PostalCode   = PostalCode?.Trim();
            this.State        = State?.     Trim();

            unchecked
            {

                hashCode = this.Street.     GetHashCode()       * 13 ^
                           this.City.       GetHashCode()       * 11 ^
                           this.Country.    GetHashCode()       *  7 ^
                           this.Coordinates.GetHashCode()       *  5 ^
                          (this.PostalCode?.GetHashCode() ?? 0) *  3 ^
                          (this.State?.     GetHashCode() ?? 0);

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomAddressParser = null)

        /// <summary>
        /// Parse the given JSON representation of an address.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomAddressParser">A delegate to parse custom address JSON objects.</param>
        public static Address Parse(JObject                                JSON,
                                    CustomJObjectParserDelegate<Address>?  CustomAddressParser   = null)
        {

            if (TryParse(JSON,
                         out var address,
                         out var errorResponse,
                         CustomAddressParser))
            {
                return address!;
            }

            throw new ArgumentException("The given JSON representation of an address is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Address, out ErrorResponse, CustomAddressParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of an address.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Address">The parsed address.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                            JSON,
                                       [NotNullWhen(true)]  out Address?  Address,
                                       [NotNullWhen(false)] out String?   ErrorResponse)

            => TryParse(JSON,
                        out Address,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of an address.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Address">The parsed address.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomAddressParser">A delegate to parse custom address JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Address?      Address,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CustomJObjectParserDelegate<Address>?  CustomAddressParser   = null)
        {

            try
            {

                Address = null;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Street         [mandatory]

                if (!JSON.ParseMandatoryText("address",
                                             "street/block name and house number",
                                             out String? Street,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse City           [mandatory]

                if (!JSON.ParseMandatoryText("city",
                                             "city",
                                             out String? City,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Country        [mandatory]

                if (!JSON.ParseMandatory("country",
                                         "country code",
                                         CountryCode.TryParse,
                                         out CountryCode Country,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Coordinates    [mandatory]

                if (!JSON.ParseMandatoryJSON("coordinates",
                                             "geo coordinates",
                                             GeoCoordinate.TryParse,
                                             out GeoCoordinate? Coordinates,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PostalCode     [optional]

                var PostalCode  = JSON["postal_code"]?.Value<String>();

                #endregion

                #region Parse State          [optional]

                var State       = JSON["state"]?.Value<String>();

                #endregion


                Address = new Address(
                              Street,
                              City,
                              Country,
                              Coordinates.Value,
                              PostalCode,
                              State
                          );

                if (CustomAddressParser is not null)
                    Address = CustomAddressParser(JSON,
                                                  Address);

                return true;

            }
            catch (Exception e)
            {
                Address        = default;
                ErrorResponse  = "The given JSON representation of an address is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomAddressSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomAddressSerializer">A delegate to serialize custom address JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Address>?  CustomAddressSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("address",       Street),
                                 new JProperty("city",          City),
                                 new JProperty("country",       Country.    ToString()),
                                 new JProperty("coordinates",   Coordinates.ToJSON  (Embedded: true)),

                           PostalCode.IsNotNullOrEmpty()
                               ? new JProperty("postal_code",   PostalCode)
                               : null,

                           State.     IsNotNullOrEmpty()
                               ? new JProperty("state",         State)
                               : null

                       );

            return CustomAddressSerializer is not null
                       ? CustomAddressSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Address Clone()

            => new (
                   Street.     CloneString(),
                   City.       CloneString(),
                   Country.    Clone(),
                   Coordinates.Clone(),
                   PostalCode?.CloneNullableString(),
                   State?.     CloneNullableString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Address Address1,
                                           Address Address2)

            => Address1.Equals(Address2);

        #endregion

        #region Operator != (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Address Address1,
                                           Address Address2)

            => !(Address1 == Address2);

        #endregion

        #region Operator <  (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Address Address1,
                                          Address Address2)

            => Address1.CompareTo(Address2) < 0;

        #endregion

        #region Operator <= (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Address Address1,
                                           Address Address2)

            => !(Address1 > Address2);

        #endregion

        #region Operator >  (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Address Address1,
                                          Address Address2)

            => Address1.CompareTo(Address2) > 0;

        #endregion

        #region Operator >= (Address1, Address2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Address1">An address.</param>
        /// <param name="Address2">Another address.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Address Address1,
                                           Address Address2)

            => !(Address1 < Address2);

        #endregion

        #endregion

        #region IComparable<Address> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two addresses.
        /// </summary>
        /// <param name="Object">An address to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Address address
                   ? CompareTo(address)
                   : throw new ArgumentException("The given object is not an address!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Address)

        /// <summary>
        /// Compares two addresses.
        /// </summary>
        /// <param name="Address">An address to compare with.</param>
        public Int32 CompareTo(Address? Address)
        {

            if (Address is null)
                throw new ArgumentNullException(nameof(Address), "The given address must not be null!");

            var c = 0;

            if (c == 0)
                c = Street.     CompareTo(Address.Street);

            if (c == 0)
                c = City.       CompareTo(Address.City);

            if (c == 0)
                c = Country.    CompareTo(Address.Country);

            if (c == 0)
                c = Coordinates.CompareTo(Address.Coordinates);

            if (c == 0 && PostalCode is not null && Address.PostalCode is not null)
                c = PostalCode.CompareTo(Address.PostalCode);

            if (c == 0 && State      is not null && Address.State      is not null)
                c = State.     CompareTo(Address.State);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Address> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two addresses for equality.
        /// </summary>
        /// <param name="Object">An address to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Address address &&
                   Equals(address);

        #endregion

        #region Equals(Address)

        /// <summary>
        /// Compares two addresses for equality.
        /// </summary>
        /// <param name="Address">An address to compare with.</param>
        public Boolean Equals(Address? Address)

            => Address is not null &&

               Street.     Equals(Address.Street)      &&
               City.       Equals(Address.City)        &&
               Country.    Equals(Address.Country)     &&
               Coordinates.Equals(Address.Coordinates) &&

               (PostalCode?.Equals(Address.PostalCode) ?? Address.PostalCode is null) &&
               (State?.     Equals(Address.State)      ?? Address.State      is null);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Street} {PostalCode ?? ""} {City} {(State is not null ? State + " " : "")}{Country} @ {Coordinates}";

        #endregion

    }

}
