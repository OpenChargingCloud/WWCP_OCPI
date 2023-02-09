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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// Extension methods for country codes.
    /// </summary>
    public static class CountryCodeExtensions
    {

        /// <summary>
        /// Indicates whether this country code is null or empty.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        public static Boolean IsNullOrEmpty(this CountryCode? CountryCode)
            => !CountryCode.HasValue || CountryCode.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this country code is NOT null or empty.
        /// </summary>
        /// <param name="CountryCode">A country code.</param>
        public static Boolean IsNotNullOrEmpty(this CountryCode? CountryCode)
            => CountryCode.HasValue && CountryCode.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique ISO-3166 alpha-2 country code of the country a party is operating in.
    /// </summary>
    public readonly struct CountryCode : IId<CountryCode>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this country code is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this country code is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the country code.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new country code based on the given text.
        /// </summary>
        /// <param name="Text">A text representation of a country code.</param>
        private CountryCode(String Text)
        {
            this.InternalId = Text;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text as a country code.
        /// </summary>
        /// <param name="Text">A text representation of a country code.</param>
        public static CountryCode Parse(String Text)
        {

            if (TryParse(Text, out var countryCode))
                return countryCode;

            throw new ArgumentException("Invalid text representation of a country code: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a country code.
        /// </summary>
        /// <param name="Text">A text representation of a country code.</param>
        public static CountryCode? TryParse(String Text)
        {

            if (TryParse(Text, out var countryCode))
                return countryCode;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out CountryCode)

        /// <summary>
        /// Try to parse the given text as a country code.
        /// </summary>
        /// <param name="Text">A text representation of a country code.</param>
        /// <param name="CountryCode">The parsed country code.</param>
        public static Boolean TryParse(String Text, out CountryCode CountryCode)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty() &&
                Text.Length == 2)
            {
                try
                {
                    CountryCode = new CountryCode(Text);
                    return true;
                }
                catch (Exception)
                { }
            }

            CountryCode = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this country code.
        /// </summary>
        public CountryCode Clone

            => new (
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (CountryCode1, CountryCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CountryCode1">A country code.</param>
        /// <param name="CountryCode2">Another country code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CountryCode CountryCode1,
                                           CountryCode CountryCode2)

            => CountryCode1.Equals(CountryCode2);

        #endregion

        #region Operator != (CountryCode1, CountryCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CountryCode1">A country code.</param>
        /// <param name="CountryCode2">Another country code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CountryCode CountryCode1,
                                           CountryCode CountryCode2)

            => !CountryCode1.Equals(CountryCode2);

        #endregion

        #region Operator <  (CountryCode1, CountryCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CountryCode1">A country code.</param>
        /// <param name="CountryCode2">Another country code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CountryCode CountryCode1,
                                          CountryCode CountryCode2)

            => CountryCode1.CompareTo(CountryCode2) < 0;

        #endregion

        #region Operator <= (CountryCode1, CountryCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CountryCode1">A country code.</param>
        /// <param name="CountryCode2">Another country code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CountryCode CountryCode1,
                                           CountryCode CountryCode2)

            => CountryCode1.CompareTo(CountryCode2) <= 0;

        #endregion

        #region Operator >  (CountryCode1, CountryCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CountryCode1">A country code.</param>
        /// <param name="CountryCode2">Another country code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CountryCode CountryCode1,
                                          CountryCode CountryCode2)

            => CountryCode1.CompareTo(CountryCode2) > 0;

        #endregion

        #region Operator >= (CountryCode1, CountryCode2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CountryCode1">A country code.</param>
        /// <param name="CountryCode2">Another country code.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CountryCode CountryCode1,
                                           CountryCode CountryCode2)

            => CountryCode1.CompareTo(CountryCode2) >= 0;

        #endregion

        #endregion

        #region IComparable<CountryCode> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two country codes.
        /// </summary>
        /// <param name="Object">A country code to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is CountryCode countryCode
                   ? CompareTo(countryCode)
                   : throw new ArgumentException("The given object is not a country code!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CountryCode)

        /// <summary>
        /// Compares two country codes.
        /// </summary>
        /// <param name="CountryCode">A country code to compare with.</param>
        public Int32 CompareTo(CountryCode CountryCode)

            => String.Compare(InternalId,
                              CountryCode.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<CountryCode> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two country codes for equality.
        /// </summary>
        /// <param name="Object">A country code to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is CountryCode countryCode &&
                   Equals(countryCode);

        #endregion

        #region Equals(CountryCode)

        /// <summary>
        /// Compares two country codes for equality.
        /// </summary>
        /// <param name="CountryCode">A country code to compare with.</param>
        public Boolean Equals(CountryCode CountryCode)

            => String.Equals(InternalId,
                             CountryCode.InternalId,
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
