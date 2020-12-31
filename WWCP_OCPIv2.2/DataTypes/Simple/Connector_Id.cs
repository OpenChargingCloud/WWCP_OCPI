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
    /// The unique identification of a connector.
    /// </summary>
    public readonly struct Connector_Id : IId<Connector_Id>

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
        /// The length of the connector identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new connector identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the connector identification.</param>
        private Connector_Id(String String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a connector identification.
        /// </summary>
        /// <param name="Text">A text representation of a connector identification.</param>
        public static Connector_Id Parse(String Text)
        {

            if (TryParse(Text, out Connector_Id connectorId))
                return connectorId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a connector identification must not be null or empty!");

            throw new ArgumentException("The given text representation of a connector identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text as a connector identification.
        /// </summary>
        /// <param name="Text">A text representation of a connector identification.</param>
        public static Connector_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Connector_Id connectorId))
                return connectorId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out ConnectorId)

        /// <summary>
        /// Try to parse the given text as a connector identification.
        /// </summary>
        /// <param name="Text">A text representation of a connector identification.</param>
        /// <param name="ConnectorId">The parsed connector identification.</param>
        public static Boolean TryParse(String Text, out Connector_Id ConnectorId)
        {

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ConnectorId = new Connector_Id(Text.Trim());
                    return true;
                }
                catch (Exception)
                { }
            }

            ConnectorId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this connector identification.
        /// </summary>
        public Connector_Id Clone

            => new Connector_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Connector_Id ConnectorId1,
                                           Connector_Id ConnectorId2)

            => ConnectorId1.Equals(ConnectorId2);

        #endregion

        #region Operator != (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Connector_Id ConnectorId1,
                                           Connector_Id ConnectorId2)

            => !(ConnectorId1 == ConnectorId2);

        #endregion

        #region Operator <  (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Connector_Id ConnectorId1,
                                          Connector_Id ConnectorId2)

            => ConnectorId1.CompareTo(ConnectorId2) < 0;

        #endregion

        #region Operator <= (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Connector_Id ConnectorId1,
                                           Connector_Id ConnectorId2)

            => !(ConnectorId1 > ConnectorId2);

        #endregion

        #region Operator >  (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Connector_Id ConnectorId1,
                                          Connector_Id ConnectorId2)

            => ConnectorId1.CompareTo(ConnectorId2) > 0;

        #endregion

        #region Operator >= (ConnectorId1, ConnectorId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId1">A connector identification.</param>
        /// <param name="ConnectorId2">Another connector identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Connector_Id ConnectorId1,
                                           Connector_Id ConnectorId2)

            => !(ConnectorId1 < ConnectorId2);

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

            if (Object is Connector_Id connectorId)
                return CompareTo(connectorId);

            throw new ArgumentException("The given object is not a connector identification!",
                                        nameof(Object));

        }

        #endregion

        #region CompareTo(ConnectorId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ConnectorId">An object to compare with.</param>
        public Int32 CompareTo(Connector_Id ConnectorId)

            => String.Compare(InternalId,
                              ConnectorId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

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

            if (Object is Connector_Id connectorId)
                return Equals(connectorId);

            return false;

        }

        #endregion

        #region Equals(ConnectorId)

        /// <summary>
        /// Compares two connector identifications for equality.
        /// </summary>
        /// <param name="ConnectorId">An connector identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Connector_Id ConnectorId)

            => String.Equals(InternalId,
                             ConnectorId.InternalId,
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
