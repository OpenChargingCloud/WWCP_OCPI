/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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
    /// The unique identification of a authentication credential.
    /// </summary>
    public class Auth_Id : IId,
                           IEquatable<Auth_Id>,
                           IComparable<Auth_Id>

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
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Generate a new authentication credential identification based on the given string.
        /// </summary>
        private Auth_Id(String String)
        {
            InternalId = String.Trim();
        }

        #endregion


        #region New

        /// <summary>
        /// Returns a new Auth identification.
        /// </summary>
        public static Auth_Id New
        {
            get
            {
                return Auth_Id.Parse(Guid.NewGuid().ToString());
            }
        }

        #endregion

        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a authentication credential identification.
        /// </summary>
        /// <param name="Text">A string representation of a authentication credential identification.</param>
        public static Auth_Id Parse(String Text)
        {
            return new Auth_Id(Text);
        }

        #endregion

        #region TryParse(Text, out AuthId)

        /// <summary>
        /// Parse the given string as a authentication credential identification.
        /// </summary>
        /// <param name="Text">A text representation of a authentication credential identification.</param>
        /// <param name="AuthId">The parsed authentication credential identification.</param>
        public static Boolean TryParse(String Text, out Auth_Id AuthId)
        {
            try
            {
                AuthId = new Auth_Id(Text);
                return true;
            }
            catch (Exception)
            {
                AuthId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this authentication credential identification.
        /// </summary>
        public Auth_Id Clone
        {
            get
            {
                return new Auth_Id(InternalId);
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A AuthId.</param>
        /// <param name="AuthId2">Another AuthId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Auth_Id AuthId1, Auth_Id AuthId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(AuthId1, AuthId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) AuthId1 == null) || ((Object) AuthId2 == null))
                return false;

            return AuthId1.Equals(AuthId2);

        }

        #endregion

        #region Operator != (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A AuthId.</param>
        /// <param name="AuthId2">Another AuthId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Auth_Id AuthId1, Auth_Id AuthId2)
        {
            return !(AuthId1 == AuthId2);
        }

        #endregion

        #region Operator <  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A AuthId.</param>
        /// <param name="AuthId2">Another AuthId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Auth_Id AuthId1, Auth_Id AuthId2)
        {

            if ((Object) AuthId1 == null)
                throw new ArgumentNullException("The given AuthId1 must not be null!");

            return AuthId1.CompareTo(AuthId2) < 0;

        }

        #endregion

        #region Operator <= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A AuthId.</param>
        /// <param name="AuthId2">Another AuthId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Auth_Id AuthId1, Auth_Id AuthId2)
        {
            return !(AuthId1 > AuthId2);
        }

        #endregion

        #region Operator >  (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A AuthId.</param>
        /// <param name="AuthId2">Another AuthId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Auth_Id AuthId1, Auth_Id AuthId2)
        {

            if ((Object) AuthId1 == null)
                throw new ArgumentNullException("The given AuthId1 must not be null!");

            return AuthId1.CompareTo(AuthId2) > 0;

        }

        #endregion

        #region Operator >= (AuthId1, AuthId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId1">A AuthId.</param>
        /// <param name="AuthId2">Another AuthId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Auth_Id AuthId1, Auth_Id AuthId2)
        {
            return !(AuthId1 < AuthId2);
        }

        #endregion

        #endregion

        #region IComparable<AuthId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an AuthId.
            var AuthId = Object as Auth_Id;
            if ((Object) AuthId == null)
                throw new ArgumentException("The given object is not a AuthId!");

            return CompareTo(AuthId);

        }

        #endregion

        #region CompareTo(AuthId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AuthId">An object to compare with.</param>
        public Int32 CompareTo(Auth_Id AuthId)
        {

            if ((Object) AuthId == null)
                throw new ArgumentNullException("The given AuthId must not be null!");

            // Compare the length of the AuthIds
            var _Result = this.Length.CompareTo(AuthId.Length);

            // If equal: Compare Ids
            if (_Result == 0)
                _Result = InternalId.CompareTo(AuthId.InternalId);

            return _Result;

        }

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
        {

            if (Object == null)
                return false;

            // Check if the given object is an AuthId.
            var AuthId = Object as Auth_Id;
            if ((Object) AuthId == null)
                return false;

            return this.Equals(AuthId);

        }

        #endregion

        #region Equals(AuthId)

        /// <summary>
        /// Compares two AuthIds for equality.
        /// </summary>
        /// <param name="AuthId">A AuthId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Auth_Id AuthId)
        {

            if ((Object) AuthId == null)
                return false;

            return InternalId.Equals(AuthId.InternalId);

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
