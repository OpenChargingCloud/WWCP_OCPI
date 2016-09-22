/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
 * This file is part of WWCP Core <https://github.com/GraphDefined/WWCP_Core>
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

#endregion

namespace org.GraphDefined.WWCP
{

    /// <summary>
    /// The unique identification of a energy meter.
    /// </summary>
    public class Meter_Id : IId,
                           IEquatable<Meter_Id>,
                           IComparable<Meter_Id>

    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        protected readonly String _Id;

        #endregion

        #region Properties

        #region New

        /// <summary>
        /// Returns a new Meter identification.
        /// </summary>
        public static Meter_Id New
        {
            get
            {
                return Meter_Id.Parse(Guid.NewGuid().ToString());
            }
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of the identificator.
        /// </summary>
        public UInt64 Length
        {
            get
            {
                return (UInt64) _Id.Length;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Generate a new energy meter identification based on the given string.
        /// </summary>
        private Meter_Id(String String)
        {
            _Id = String.Trim();
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a energy meter identification.
        /// </summary>
        /// <param name="Text">A string representation of a energy meter identification.</param>
        public static Meter_Id Parse(String Text)
        {
            return new Meter_Id(Text);
        }

        #endregion

        #region TryParse(Text, out MeterId)

        /// <summary>
        /// Parse the given string as a energy meter identification.
        /// </summary>
        /// <param name="Text">A text representation of a energy meter identification.</param>
        /// <param name="MeterId">The parsed energy meter identification.</param>
        public static Boolean TryParse(String Text, out Meter_Id MeterId)
        {
            try
            {
                MeterId = new Meter_Id(Text);
                return true;
            }
            catch (Exception)
            {
                MeterId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this energy meter identification.
        /// </summary>
        public Meter_Id Clone
        {
            get
            {
                return new Meter_Id(_Id);
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A MeterId.</param>
        /// <param name="MeterId2">Another MeterId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Meter_Id MeterId1, Meter_Id MeterId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(MeterId1, MeterId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) MeterId1 == null) || ((Object) MeterId2 == null))
                return false;

            return MeterId1.Equals(MeterId2);

        }

        #endregion

        #region Operator != (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A MeterId.</param>
        /// <param name="MeterId2">Another MeterId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Meter_Id MeterId1, Meter_Id MeterId2)
        {
            return !(MeterId1 == MeterId2);
        }

        #endregion

        #region Operator <  (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A MeterId.</param>
        /// <param name="MeterId2">Another MeterId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Meter_Id MeterId1, Meter_Id MeterId2)
        {

            if ((Object) MeterId1 == null)
                throw new ArgumentNullException("The given MeterId1 must not be null!");

            return MeterId1.CompareTo(MeterId2) < 0;

        }

        #endregion

        #region Operator <= (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A MeterId.</param>
        /// <param name="MeterId2">Another MeterId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Meter_Id MeterId1, Meter_Id MeterId2)
        {
            return !(MeterId1 > MeterId2);
        }

        #endregion

        #region Operator >  (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A MeterId.</param>
        /// <param name="MeterId2">Another MeterId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Meter_Id MeterId1, Meter_Id MeterId2)
        {

            if ((Object) MeterId1 == null)
                throw new ArgumentNullException("The given MeterId1 must not be null!");

            return MeterId1.CompareTo(MeterId2) > 0;

        }

        #endregion

        #region Operator >= (MeterId1, MeterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId1">A MeterId.</param>
        /// <param name="MeterId2">Another MeterId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Meter_Id MeterId1, Meter_Id MeterId2)
        {
            return !(MeterId1 < MeterId2);
        }

        #endregion

        #endregion

        #region IComparable<MeterId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an MeterId.
            var MeterId = Object as Meter_Id;
            if ((Object) MeterId == null)
                throw new ArgumentException("The given object is not a MeterId!");

            return CompareTo(MeterId);

        }

        #endregion

        #region CompareTo(MeterId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterId">An object to compare with.</param>
        public Int32 CompareTo(Meter_Id MeterId)
        {

            if ((Object) MeterId == null)
                throw new ArgumentNullException("The given MeterId must not be null!");

            // Compare the length of the MeterIds
            var _Result = this.Length.CompareTo(MeterId.Length);

            // If equal: Compare Ids
            if (_Result == 0)
                _Result = _Id.CompareTo(MeterId._Id);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<MeterId> Members

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

            // Check if the given object is an MeterId.
            var MeterId = Object as Meter_Id;
            if ((Object) MeterId == null)
                return false;

            return this.Equals(MeterId);

        }

        #endregion

        #region Equals(MeterId)

        /// <summary>
        /// Compares two MeterIds for equality.
        /// </summary>
        /// <param name="MeterId">A MeterId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Meter_Id MeterId)
        {

            if ((Object) MeterId == null)
                return false;

            return _Id.Equals(MeterId._Id);

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
            return _Id.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return _Id.ToString();
        }

        #endregion

    }

}
