/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/OpenChargingCloud/WWCP_OCPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      *     http://www.apache.org/licenses/LICENSE-2.0
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
    /// The unique identification of a currency.
    /// </summary>
    public readonly struct Currency : IId<Currency>
    {

        #region Data

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
        /// The length of the currency.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new currency based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the currency.</param>
        private Currency(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a currency.
        /// </summary>
        /// <param name="Text">A text representation of a currency.</param>
        public static Currency Parse(String Text)
        {

            if (TryParse(Text, out Currency currency))
                return currency;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a currency must not be null or empty!");

            throw new ArgumentException("The given text representation of a currency is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a currency.
        /// </summary>
        /// <param name="Text">A text representation of a currency.</param>
        public static Currency? TryParse(String Text)
        {

            if (TryParse(Text, out Currency currency))
                return currency;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out AuthId)

        /// <summary>
        /// Try to parse the given text as a currency.
        /// </summary>
        /// <param name="Text">A text representation of a currency.</param>
        /// <param name="AuthId">The parsed currency.</param>
        public static Boolean TryParse(String Text, out Currency AuthId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    AuthId = new Currency(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            AuthId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this currency.
        /// </summary>
        public Currency Clone

            => new Currency(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Static defaults

        /// <summary>
        /// EUR, €.
        /// </summary>
        public static Currency EUR = Currency.Parse("EUR");

        #endregion


        #region Operator overloading

        #region Operator == (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A currency.</param>
        /// <param name="AuthId2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Currency AuthId1,
                                           Currency AuthId2)

            => AuthId1.Equals(AuthId2);

        #endregion

        #region Operator != (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A currency.</param>
        /// <param name="AuthId2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Currency AuthId1,
                                           Currency AuthId2)

            => !(AuthId1 == AuthId2);

        #endregion

        #region Operator <  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A currency.</param>
        /// <param name="AuthId2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Currency AuthId1,
                                          Currency AuthId2)

            => AuthId1.CompareTo(AuthId2) < 0;

        #endregion

        #region Operator <= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A currency.</param>
        /// <param name="AuthId2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Currency AuthId1,
                                           Currency AuthId2)

            => !(AuthId1 > AuthId2);

        #endregion

        #region Operator >  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A currency.</param>
        /// <param name="AuthId2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Currency AuthId1,
                                          Currency AuthId2)

            => AuthId1.CompareTo(AuthId2) > 0;

        #endregion

        #region Operator >= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A currency.</param>
        /// <param name="AuthId2">Another currency.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Currency AuthId1,
                                           Currency AuthId2)

            => !(AuthId1 < AuthId2);

        #endregion

        #endregion

        #region IComparable<AuthId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Currency currency
                   ? CompareTo(currency)
                   : throw new ArgumentException("The given object is not a currency!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(AuthId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId">An object to compare with.</param>
        public Int32 CompareTo(Currency AuthId)

            => String.Compare(InternalId,
                              AuthId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<AuthId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Currency currency &&
                   Equals(currency);

        #endregion

        #region Equals(AuthId)

        /// <summary>
        /// Compares two currencys for equality.
        /// </summary>
        /// <param name="AuthId">An currency to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Currency AuthId)

            => String.Equals(InternalId,
                             AuthId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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
