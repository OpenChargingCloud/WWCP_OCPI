/*
 * Copyright (c) 2014-2017 GraphDefined GmbH
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
    /// The unique identification of a connector.
    /// </summary>
    public class Connector_Id : IId,
                                IEquatable<Connector_Id>,
                                IComparable<Connector_Id>

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
        /// Returns a new connector identification.
        /// </summary>
        public static Connector_Id New
        {
            get
            {
                return Connector_Id.Parse(Guid.NewGuid().ToString());
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
        /// Generate a new connector identification based on the given string.
        /// </summary>
        private Connector_Id(String String)
        {
            _Id = String.Trim();
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as a connector identification.
        /// </summary>
        /// <param name="Text">A string representation of a connector identification.</param>
        public static Connector_Id Parse(String Text)
        {
            return new Connector_Id(Text);
        }

        #endregion

        #region TryParse(Text, out ConnectorId)

        /// <summary>
        /// Parse the given string as a connector identification.
        /// </summary>
        /// <param name="Text">A text representation of a connector identification.</param>
        /// <param name="ConnectorId">The parsed connector identification.</param>
        public static Boolean TryParse(String Text, out Connector_Id ConnectorId)
        {
            try
            {
                ConnectorId = new Connector_Id(Text);
                return true;
            }
            catch (Exception)
            {
                ConnectorId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this connector identification.
        /// </summary>
        public Connector_Id Clone
        {
            get
            {
                return new Connector_Id(_Id);
            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A ConnectorId.</param>
        /// <param name="ConnectorId2">Another ConnectorId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Connector_Id ConnectorId1, Connector_Id ConnectorId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ConnectorId1, ConnectorId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ConnectorId1 == null) || ((Object) ConnectorId2 == null))
                return false;

            return ConnectorId1.Equals(ConnectorId2);

        }

        #endregion

        #region Operator != (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A ConnectorId.</param>
        /// <param name="ConnectorId2">Another ConnectorId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Connector_Id ConnectorId1, Connector_Id ConnectorId2)
        {
            return !(ConnectorId1 == ConnectorId2);
        }

        #endregion

        #region Operator <  (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A ConnectorId.</param>
        /// <param name="ConnectorId2">Another ConnectorId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Connector_Id ConnectorId1, Connector_Id ConnectorId2)
        {

            if ((Object) ConnectorId1 == null)
                throw new ArgumentNullException("The given ConnectorId1 must not be null!");

            return ConnectorId1.CompareTo(ConnectorId2) < 0;

        }

        #endregion

        #region Operator <= (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A ConnectorId.</param>
        /// <param name="ConnectorId2">Another ConnectorId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Connector_Id ConnectorId1, Connector_Id ConnectorId2)
        {
            return !(ConnectorId1 > ConnectorId2);
        }

        #endregion

        #region Operator >  (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A ConnectorId.</param>
        /// <param name="ConnectorId2">Another ConnectorId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Connector_Id ConnectorId1, Connector_Id ConnectorId2)
        {

            if ((Object) ConnectorId1 == null)
                throw new ArgumentNullException("The given ConnectorId1 must not be null!");

            return ConnectorId1.CompareTo(ConnectorId2) > 0;

        }

        #endregion

        #region Operator >= (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A ConnectorId.</param>
        /// <param name="ConnectorId2">Another ConnectorId.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Connector_Id ConnectorId1, Connector_Id ConnectorId2)
        {
            return !(ConnectorId1 < ConnectorId2);
        }

        #endregion

        #endregion

        #region IComparable<ConnectorId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an ConnectorId.
            var ConnectorId = Object as Connector_Id;
            if ((Object) ConnectorId == null)
                throw new ArgumentException("The given object is not a ConnectorId!");

            return CompareTo(ConnectorId);

        }

        #endregion

        #region CompareTo(ConnectorId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId">An object to compare with.</param>
        public Int32 CompareTo(Connector_Id ConnectorId)
        {

            if ((Object) ConnectorId == null)
                throw new ArgumentNullException("The given ConnectorId must not be null!");

            // Compare the length of the ConnectorIds
            var _Result = this.Length.CompareTo(ConnectorId.Length);

            // If equal: Compare Ids
            if (_Result == 0)
                _Result = _Id.CompareTo(ConnectorId._Id);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<ConnectorId> Members

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

            // Check if the given object is an ConnectorId.
            var ConnectorId = Object as Connector_Id;
            if ((Object) ConnectorId == null)
                return false;

            return this.Equals(ConnectorId);

        }

        #endregion

        #region Equals(ConnectorId)

        /// <summary>
        /// Compares two ConnectorIds for equality.
        /// </summary>
        /// <param name="ConnectorId">A ConnectorId to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Connector_Id ConnectorId)
        {

            if ((Object) ConnectorId == null)
                return false;

            return _Id.Equals(ConnectorId._Id);

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
