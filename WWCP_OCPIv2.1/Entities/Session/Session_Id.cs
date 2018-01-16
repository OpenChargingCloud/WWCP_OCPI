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
    /// The unique identification of a charging session.
    /// </summary>
    public class Session_Id : IId,
                              IEquatable<Session_Id>,
                              IComparable<Session_Id>

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
        /// Returns a new Session identification.
        /// </summary>
        public static Session_Id New
        {
            get
            {
                return Session_Id.Parse(Guid.NewGuid().ToString());
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
        /// Generate a new charging session identification based on the given string.
        /// </summary>
        private Session_Id(String String)
        {
            _Id = String.Trim();
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a charging session identification.
        /// </summary>
        /// <param name="Text">A string representation of a charging session identification.</param>
        public static Session_Id Parse(String Text)
        {
            return new Session_Id(Text);
        }

        #endregion

        #region TryParse(Text, out SessionId)

        /// <summary>
        /// Parse the given string as a charging session identification.
        /// </summary>
        /// <param name="Text">A text representation of a charging session identification.</param>
        /// <param name="SessionId">The parsed charging session identification.</param>
        public static Boolean TryParse(String Text, out Session_Id SessionId)
        {
            try
            {
                SessionId = new Session_Id(Text);
                return true;
            }
            catch (Exception)
            {
                SessionId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this charging session identification.
        /// </summary>
        public Session_Id Clone
        {
            get
            {
                return new Session_Id(_Id);
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A SessionId.</param>
        /// <param name="SessionId2">Another SessionId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Session_Id SessionId1, Session_Id SessionId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(SessionId1, SessionId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SessionId1 == null) || ((Object) SessionId2 == null))
                return false;

            return SessionId1.Equals(SessionId2);

        }

        #endregion

        #region Operator != (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A SessionId.</param>
        /// <param name="SessionId2">Another SessionId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Session_Id SessionId1, Session_Id SessionId2)
        {
            return !(SessionId1 == SessionId2);
        }

        #endregion

        #region Operator <  (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A SessionId.</param>
        /// <param name="SessionId2">Another SessionId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Session_Id SessionId1, Session_Id SessionId2)
        {

            if ((Object) SessionId1 == null)
                throw new ArgumentNullException("The given SessionId1 must not be null!");

            return SessionId1.CompareTo(SessionId2) < 0;

        }

        #endregion

        #region Operator <= (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A SessionId.</param>
        /// <param name="SessionId2">Another SessionId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Session_Id SessionId1, Session_Id SessionId2)
        {
            return !(SessionId1 > SessionId2);
        }

        #endregion

        #region Operator >  (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A SessionId.</param>
        /// <param name="SessionId2">Another SessionId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Session_Id SessionId1, Session_Id SessionId2)
        {

            if ((Object) SessionId1 == null)
                throw new ArgumentNullException("The given SessionId1 must not be null!");

            return SessionId1.CompareTo(SessionId2) > 0;

        }

        #endregion

        #region Operator >= (SessionId1, SessionId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId1">A SessionId.</param>
        /// <param name="SessionId2">Another SessionId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Session_Id SessionId1, Session_Id SessionId2)
        {
            return !(SessionId1 < SessionId2);
        }

        #endregion

        #endregion

        #region IComparable<SessionId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an SessionId.
            var SessionId = Object as Session_Id;
            if ((Object) SessionId == null)
                throw new ArgumentException("The given object is not a SessionId!");

            return CompareTo(SessionId);

        }

        #endregion

        #region CompareTo(SessionId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SessionId">An object to compare with.</param>
        public Int32 CompareTo(Session_Id SessionId)
        {

            if ((Object) SessionId == null)
                throw new ArgumentNullException("The given SessionId must not be null!");

            // Compare the length of the SessionIds
            var _Result = this.Length.CompareTo(SessionId.Length);

            // If equal: Compare Ids
            if (_Result == 0)
                _Result = _Id.CompareTo(SessionId._Id);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<SessionId> Members

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

            // Check if the given object is an SessionId.
            var SessionId = Object as Session_Id;
            if ((Object) SessionId == null)
                return false;

            return this.Equals(SessionId);

        }

        #endregion

        #region Equals(SessionId)

        /// <summary>
        /// Compares two SessionIds for equality.
        /// </summary>
        /// <param name="SessionId">A SessionId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Session_Id SessionId)
        {

            if ((Object) SessionId == null)
                return false;

            return _Id.Equals(SessionId._Id);

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
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return _Id.ToString();
        }

        #endregion

    }

}
