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
    /// The unique identification of a charge detail record.
    /// </summary>
    public class CDR_Id : IId,
                          IEquatable<CDR_Id>,
                          IComparable<CDR_Id>

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
        /// Returns a new CDR identification.
        /// </summary>
        public static CDR_Id New
        {
            get
            {
                return CDR_Id.Parse(Guid.NewGuid().ToString());
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
        /// Generate a new charge detail record identification based on the given string.
        /// </summary>
        private CDR_Id(String String)
        {
            _Id = String.Trim();
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a charge detail record identification.
        /// </summary>
        /// <param name="Text">A string representation of a charge detail record identification.</param>
        public static CDR_Id Parse(String Text)
        {
            return new CDR_Id(Text);
        }

        #endregion

        #region TryParse(Text, out CDRId)

        /// <summary>
        /// Parse the given string as a charge detail record identification.
        /// </summary>
        /// <param name="Text">A text representation of a charge detail record identification.</param>
        /// <param name="CDRId">The parsed charge detail record identification.</param>
        public static Boolean TryParse(String Text, out CDR_Id CDRId)
        {
            try
            {
                CDRId = new CDR_Id(Text);
                return true;
            }
            catch (Exception)
            {
                CDRId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charge detail record identification.
        /// </summary>
        public CDR_Id Clone
        {
            get
            {
                return new CDR_Id(_Id);
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A CDRId.</param>
        /// <param name="CDRId2">Another CDRId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (CDR_Id CDRId1, CDR_Id CDRId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(CDRId1, CDRId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) CDRId1 == null) || ((Object) CDRId2 == null))
                return false;

            return CDRId1.Equals(CDRId2);

        }

        #endregion

        #region Operator != (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A CDRId.</param>
        /// <param name="CDRId2">Another CDRId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (CDR_Id CDRId1, CDR_Id CDRId2)
        {
            return !(CDRId1 == CDRId2);
        }

        #endregion

        #region Operator <  (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A CDRId.</param>
        /// <param name="CDRId2">Another CDRId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (CDR_Id CDRId1, CDR_Id CDRId2)
        {

            if ((Object) CDRId1 == null)
                throw new ArgumentNullException("The given CDRId1 must not be null!");

            return CDRId1.CompareTo(CDRId2) < 0;

        }

        #endregion

        #region Operator <= (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A CDRId.</param>
        /// <param name="CDRId2">Another CDRId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (CDR_Id CDRId1, CDR_Id CDRId2)
        {
            return !(CDRId1 > CDRId2);
        }

        #endregion

        #region Operator >  (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A CDRId.</param>
        /// <param name="CDRId2">Another CDRId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (CDR_Id CDRId1, CDR_Id CDRId2)
        {

            if ((Object) CDRId1 == null)
                throw new ArgumentNullException("The given CDRId1 must not be null!");

            return CDRId1.CompareTo(CDRId2) > 0;

        }

        #endregion

        #region Operator >= (CDRId1, CDRId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId1">A CDRId.</param>
        /// <param name="CDRId2">Another CDRId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (CDR_Id CDRId1, CDR_Id CDRId2)
        {
            return !(CDRId1 < CDRId2);
        }

        #endregion

        #endregion

        #region IComparable<CDRId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an CDRId.
            var CDRId = Object as CDR_Id;
            if ((Object) CDRId == null)
                throw new ArgumentException("The given object is not a CDRId!");

            return CompareTo(CDRId);

        }

        #endregion

        #region CompareTo(CDRId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRId">An object to compare with.</param>
        public Int32 CompareTo(CDR_Id CDRId)
        {

            if ((Object) CDRId == null)
                throw new ArgumentNullException("The given CDRId must not be null!");

            // Compare the length of the CDRIds
            var _Result = this.Length.CompareTo(CDRId.Length);

            // If equal: Compare Ids
            if (_Result == 0)
                _Result = _Id.CompareTo(CDRId._Id);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<CDRId> Members

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

            // Check if the given object is an CDRId.
            var CDRId = Object as CDR_Id;
            if ((Object) CDRId == null)
                return false;

            return this.Equals(CDRId);

        }

        #endregion

        #region Equals(CDRId)

        /// <summary>
        /// Compares two CDRIds for equality.
        /// </summary>
        /// <param name="CDRId">A CDRId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDR_Id CDRId)
        {

            if ((Object) CDRId == null)
                return false;

            return _Id.Equals(CDRId._Id);

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
