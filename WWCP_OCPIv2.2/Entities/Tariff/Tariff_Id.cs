/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
 * This file is part of WWCP Core <https://github.com/GraphDefined/WWCP_Core>
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

namespace org.GraphDefined.WWCP
{

    /// <summary>
    /// The unique identification of a charging tariff.
    /// </summary>
    public class Tariff_Id : IId,
                             IEquatable<Tariff_Id>,
                             IComparable<Tariff_Id>

    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        protected readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Returns the length of the identification.
        /// </summary>
        public UInt64 Length
        {
            get
            {
                return (UInt64) InternalId.Length;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Generate a new tariff identification based on the given string.
        /// </summary>
        private Tariff_Id(String String)
        {
            InternalId = String.Trim();
        }

        #endregion


        #region New

        /// <summary>
        /// Returns a new Tariff identification.
        /// </summary>
        public static Tariff_Id New
        {
            get
            {
                return Tariff_Id.Parse(Guid.NewGuid().ToString());
            }
        }

        #endregion

        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a Tariff identification.
        /// </summary>
        /// <param name="Text">A string representation of a Tariff identification.</param>
        public static Tariff_Id Parse(String Text)
        {
            return new Tariff_Id(Text);
        }

        #endregion

        #region TryParse(Text, out TariffId)

        /// <summary>
        /// Parse the given string as a Tariff identification.
        /// </summary>
        /// <param name="Text">A text representation of a Tariff identification.</param>
        /// <param name="TariffId">The parsed Tariff identification.</param>
        public static Boolean TryParse(String Text, out Tariff_Id TariffId)
        {
            try
            {
                TariffId = new Tariff_Id(Text);
                return true;
            }
            catch (Exception)
            {
                TariffId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this Tariff identification.
        /// </summary>
        public Tariff_Id Clone
        {
            get
            {
                return new Tariff_Id(InternalId);
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A TariffId.</param>
        /// <param name="TariffId2">Another TariffId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Tariff_Id TariffId1, Tariff_Id TariffId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(TariffId1, TariffId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) TariffId1 == null) || ((Object) TariffId2 == null))
                return false;

            return TariffId1.Equals(TariffId2);

        }

        #endregion

        #region Operator != (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A TariffId.</param>
        /// <param name="TariffId2">Another TariffId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Tariff_Id TariffId1, Tariff_Id TariffId2)
        {
            return !(TariffId1 == TariffId2);
        }

        #endregion

        #region Operator <  (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A TariffId.</param>
        /// <param name="TariffId2">Another TariffId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Tariff_Id TariffId1, Tariff_Id TariffId2)
        {

            if ((Object) TariffId1 == null)
                throw new ArgumentNullException("The given TariffId1 must not be null!");

            return TariffId1.CompareTo(TariffId2) < 0;

        }

        #endregion

        #region Operator <= (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A TariffId.</param>
        /// <param name="TariffId2">Another TariffId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Tariff_Id TariffId1, Tariff_Id TariffId2)
        {
            return !(TariffId1 > TariffId2);
        }

        #endregion

        #region Operator >  (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A TariffId.</param>
        /// <param name="TariffId2">Another TariffId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Tariff_Id TariffId1, Tariff_Id TariffId2)
        {

            if ((Object) TariffId1 == null)
                throw new ArgumentNullException("The given TariffId1 must not be null!");

            return TariffId1.CompareTo(TariffId2) > 0;

        }

        #endregion

        #region Operator >= (TariffId1, TariffId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId1">A TariffId.</param>
        /// <param name="TariffId2">Another TariffId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Tariff_Id TariffId1, Tariff_Id TariffId2)
        {
            return !(TariffId1 < TariffId2);
        }

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

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an TariffId.
            var TariffId = Object as Tariff_Id;
            if ((Object) TariffId == null)
                throw new ArgumentException("The given object is not a TariffId!");

            return CompareTo(TariffId);

        }

        #endregion

        #region CompareTo(TariffId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffId">An object to compare with.</param>
        public Int32 CompareTo(Tariff_Id TariffId)
        {

            if ((Object) TariffId == null)
                throw new ArgumentNullException("The given TariffId must not be null!");

            // Compare the length of the TariffIds
            var _Result = this.Length.CompareTo(TariffId.Length);

            // If equal: Compare Ids
            if (_Result == 0)
                _Result = InternalId.CompareTo(TariffId.InternalId);

            return _Result;

        }

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

            if (Object == null)
                return false;

            // Check if the given object is an TariffId.
            var TariffId = Object as Tariff_Id;
            if ((Object) TariffId == null)
                return false;

            return this.Equals(TariffId);

        }

        #endregion

        #region Equals(TariffId)

        /// <summary>
        /// Compares two TariffIds for equality.
        /// </summary>
        /// <param name="TariffId">A TariffId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Tariff_Id TariffId)
        {

            if ((Object) TariffId == null)
                return false;

            return InternalId.Equals(TariffId.InternalId);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            return InternalId.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return InternalId.ToString();
        }

        #endregion

    }

}
