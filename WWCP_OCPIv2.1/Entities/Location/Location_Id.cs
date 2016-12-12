/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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

#endregion

namespace org.GraphDefined.WWCP
{

    /// <summary>
    /// The unique identification of a charging location.
    /// </summary>
    public class Location_Id : IId,
                               IEquatable<Location_Id>,
                               IComparable<Location_Id>

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
        /// Returns a new Location identification.
        /// </summary>
        public static Location_Id New
        {
            get
            {
                return Location_Id.Parse(Guid.NewGuid().ToString());
            }
        }

        #endregion

        #region Length

        /// <summary>
        /// Returns the length of the identification.
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
        /// Generate a new charging location identification based on the given string.
        /// </summary>
        private Location_Id(String String)
        {
            _Id = String.Trim();
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a charging location identification.
        /// </summary>
        /// <param name="Text">A string representation of a charging location identification.</param>
        public static Location_Id Parse(String Text)
        {
            return new Location_Id(Text);
        }

        #endregion

        #region TryParse(Text, out LocationId)

        /// <summary>
        /// Parse the given string as a charging location identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging location identification.</param>
        /// <param name="LocationId">The parsed charging location identification.</param>
        public static Boolean TryParse(String Text, out Location_Id LocationId)
        {
            try
            {
                LocationId = new Location_Id(Text);
                return true;
            }
            catch (Exception)
            {
                LocationId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging location identification.
        /// </summary>
        public Location_Id Clone
        {
            get
            {
                return new Location_Id(_Id);
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A LocationId.</param>
        /// <param name="LocationId2">Another LocationId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Location_Id LocationId1, Location_Id LocationId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(LocationId1, LocationId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) LocationId1 == null) || ((Object) LocationId2 == null))
                return false;

            return LocationId1.Equals(LocationId2);

        }

        #endregion

        #region Operator != (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A LocationId.</param>
        /// <param name="LocationId2">Another LocationId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Location_Id LocationId1, Location_Id LocationId2)
        {
            return !(LocationId1 == LocationId2);
        }

        #endregion

        #region Operator <  (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A LocationId.</param>
        /// <param name="LocationId2">Another LocationId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Location_Id LocationId1, Location_Id LocationId2)
        {

            if ((Object) LocationId1 == null)
                throw new ArgumentNullException("The given LocationId1 must not be null!");

            return LocationId1.CompareTo(LocationId2) < 0;

        }

        #endregion

        #region Operator <= (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A LocationId.</param>
        /// <param name="LocationId2">Another LocationId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Location_Id LocationId1, Location_Id LocationId2)
        {
            return !(LocationId1 > LocationId2);
        }

        #endregion

        #region Operator >  (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A LocationId.</param>
        /// <param name="LocationId2">Another LocationId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Location_Id LocationId1, Location_Id LocationId2)
        {

            if ((Object) LocationId1 == null)
                throw new ArgumentNullException("The given LocationId1 must not be null!");

            return LocationId1.CompareTo(LocationId2) > 0;

        }

        #endregion

        #region Operator >= (LocationId1, LocationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId1">A LocationId.</param>
        /// <param name="LocationId2">Another LocationId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Location_Id LocationId1, Location_Id LocationId2)
        {
            return !(LocationId1 < LocationId2);
        }

        #endregion

        #endregion

        #region IComparable<LocationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an LocationId.
            var LocationId = Object as Location_Id;
            if ((Object) LocationId == null)
                throw new ArgumentException("The given object is not a LocationId!");

            return CompareTo(LocationId);

        }

        #endregion

        #region CompareTo(LocationId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LocationId">An object to compare with.</param>
        public Int32 CompareTo(Location_Id LocationId)
        {

            if ((Object) LocationId == null)
                throw new ArgumentNullException("The given LocationId must not be null!");

            // Compare the length of the LocationIds
            var _Result = this.Length.CompareTo(LocationId.Length);

            // If equal: Compare Ids
            if (_Result == 0)
                _Result = _Id.CompareTo(LocationId._Id);

            return _Result;

        }

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
        {

            if (Object == null)
                return false;

            // Check if the given object is an LocationId.
            var LocationId = Object as Location_Id;
            if ((Object) LocationId == null)
                return false;

            return this.Equals(LocationId);

        }

        #endregion

        #region Equals(LocationId)

        /// <summary>
        /// Compares two LocationIds for equality.
        /// </summary>
        /// <param name="LocationId">A LocationId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Location_Id LocationId)
        {

            if ((Object) LocationId == null)
                return false;

            return _Id.Equals(LocationId._Id);

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
