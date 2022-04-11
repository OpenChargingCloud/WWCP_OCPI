/*
 * Copyright (c) 2015-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPIv2_1_1
{

    /// <summary>
    /// The unique identification of a country code.
    /// </summary>
    public struct CountryCode : IId<CountryCode>
    {

        #region Data

        // CiString(2)

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
        /// The length of the country code identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new country code identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the country code identification.</param>
        private CountryCode(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a country code identification.
        /// </summary>
        /// <param name="Text">A text representation of a country code identification.</param>
        public static CountryCode Parse(String Text)
        {

            if (TryParse(Text, out CountryCode locationId))
                return locationId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a country code identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a country code identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a country code identification.
        /// </summary>
        /// <param name="Text">A text representation of a country code identification.</param>
        public static CountryCode? TryParse(String Text)
        {

            if (TryParse(Text, out CountryCode locationId))
                return locationId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out LocationId)

        /// <summary>
        /// Try to parse the given text as a country code identification.
        /// </summary>
        /// <param name="Text">A text representation of a country code identification.</param>
        /// <param name="LocationId">The parsed country code identification.</param>
        public static Boolean TryParse(String Text, out CountryCode LocationId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    LocationId = new CountryCode(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            LocationId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this country code identification.
        /// </summary>
        public CountryCode Clone

            => new CountryCode(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A country code identification.</param>
        /// <param name="LocationId2">Another country code identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CountryCode LocationId1,
                                           CountryCode LocationId2)

            => LocationId1.Equals(LocationId2);

        #endregion

        #region Operator != (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A country code identification.</param>
        /// <param name="LocationId2">Another country code identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CountryCode LocationId1,
                                           CountryCode LocationId2)

            => !(LocationId1 == LocationId2);

        #endregion

        #region Operator <  (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A country code identification.</param>
        /// <param name="LocationId2">Another country code identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CountryCode LocationId1,
                                          CountryCode LocationId2)

            => LocationId1.CompareTo(LocationId2) < 0;

        #endregion

        #region Operator <= (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A country code identification.</param>
        /// <param name="LocationId2">Another country code identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CountryCode LocationId1,
                                           CountryCode LocationId2)

            => !(LocationId1 > LocationId2);

        #endregion

        #region Operator >  (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A country code identification.</param>
        /// <param name="LocationId2">Another country code identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CountryCode LocationId1,
                                          CountryCode LocationId2)

            => LocationId1.CompareTo(LocationId2) > 0;

        #endregion

        #region Operator >= (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A country code identification.</param>
        /// <param name="LocationId2">Another country code identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CountryCode LocationId1,
                                           CountryCode LocationId2)

            => !(LocationId1 < LocationId2);

        #endregion

        #endregion

        #region IComparable<LocationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CountryCode locationId
                   ? CompareTo(locationId)
                   : throw new ArgumentException("The given object is not a country code identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(LocationId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId">An object to compare with.</param>
        public Int32 CompareTo(CountryCode LocationId)

            => String.Compare(InternalId,
                              LocationId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<LocationId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CountryCode locationId &&
                   Equals(locationId);

        #endregion

        #region Equals(LocationId)

        /// <summary>
        /// Compares two country code identifications for equality.
        /// </summary>
        /// <param name="LocationId">An country code identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CountryCode LocationId)

            => String.Equals(InternalId,
                             LocationId.InternalId,
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
