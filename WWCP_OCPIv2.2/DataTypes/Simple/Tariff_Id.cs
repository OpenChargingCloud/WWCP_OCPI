/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
 * This file is part of WWCP Core <https://github.com/OpenChargingCloud/WWCP_Core>
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

namespace cloud.charging.open.protocols
{

    /// <summary>
    /// The unique identification of a charging tariff.
    /// </summary>
    public struct Tariff_Id : IId<Tariff_Id>

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
        /// The length of the charging tariff identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging tariff identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the charging tariff identification.</param>
        private Tariff_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a charging tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging tariff identification.</param>
        public static Tariff_Id Parse(String Text)
        {

            if (TryParse(Text, out Tariff_Id tariffId))
                return tariffId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a charging tariff identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a charging tariff identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a charging tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging tariff identification.</param>
        public static Tariff_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Tariff_Id tariffId))
                return tariffId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out TariffId)

        /// <summary>
        /// Try to parse the given text as a charging tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging tariff identification.</param>
        /// <param name="TariffId">The parsed charging tariff identification.</param>
        public static Boolean TryParse(String Text, out Tariff_Id TariffId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    TariffId = new Tariff_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            TariffId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging tariff identification.
        /// </summary>
        public Tariff_Id Clone

            => new Tariff_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A charging tariff identification.</param>
        /// <param name="TariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tariff_Id TariffId1,
                                           Tariff_Id TariffId2)

            => TariffId1.Equals(TariffId2);

        #endregion

        #region Operator != (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A charging tariff identification.</param>
        /// <param name="TariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tariff_Id TariffId1,
                                           Tariff_Id TariffId2)

            => !(TariffId1 == TariffId2);

        #endregion

        #region Operator <  (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A charging tariff identification.</param>
        /// <param name="TariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tariff_Id TariffId1,
                                          Tariff_Id TariffId2)

            => TariffId1.CompareTo(TariffId2) < 0;

        #endregion

        #region Operator <= (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A charging tariff identification.</param>
        /// <param name="TariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tariff_Id TariffId1,
                                           Tariff_Id TariffId2)

            => !(TariffId1 > TariffId2);

        #endregion

        #region Operator >  (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A charging tariff identification.</param>
        /// <param name="TariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tariff_Id TariffId1,
                                          Tariff_Id TariffId2)

            => TariffId1.CompareTo(TariffId2) > 0;

        #endregion

        #region Operator >= (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A charging tariff identification.</param>
        /// <param name="TariffId2">Another charging tariff identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tariff_Id TariffId1,
                                           Tariff_Id TariffId2)

            => !(TariffId1 < TariffId2);

        #endregion

        #endregion

        #region IComparable<TariffId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is Tariff_Id tariffId)
                return CompareTo(tariffId);

            throw new ArgumentException("The given object is not a charging tariff identification!",
                                        nameof(Object));

        }

        #endregion

        #region CompareTo(TariffId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId">An object to compare with.</param>
        public Int32 CompareTo(Tariff_Id TariffId)

            => String.Compare(InternalId,
                              TariffId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<TariffId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is Tariff_Id tariffId)
                return Equals(tariffId);

            return false;

        }

        #endregion

        #region Equals(TariffId)

        /// <summary>
        /// Compares two charging tariff identifications for equality.
        /// </summary>
        /// <param name="TariffId">An charging tariff identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Tariff_Id TariffId)

            => String.Equals(InternalId,
                             TariffId.InternalId,
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
