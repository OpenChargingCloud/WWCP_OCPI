/*
 * Copyright (c) 2015-2020 GraphDefined GmbH
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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The CdrLocation class contains only the relevant information from the
    /// location object that is needed in a CDR.
    /// </summary>
    public class CDRLocation : IHasId<Location_Id>,
                               IEquatable<CDRLocation>,
                               IComparable<CDRLocation>,
                               IComparable
    {

        #region Properties

        /// <summary>
        /// The identification of the location within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public Location_Id                         Id                       { get; }

        /// <summary>
        /// Display name of the location. // 255
        /// </summary>
        [Optional]
        public String                              Name                     { get; }

        /// <summary>
        /// Address of the location. // 45
        /// </summary>
        [Mandatory]
        public String                              Address                  { get; }

        /// <summary>
        /// Address of the location. // 45
        /// </summary>
        [Mandatory]
        public String                              City                     { get; }

        /// <summary>
        /// Address of the location. // 10
        /// </summary>
        [Mandatory]
        public String                              PostalCode               { get; }

        /// <summary>
        /// Address of the location. // 3
        /// </summary>
        [Mandatory]
        public String                              Country                  { get; }

        /// <summary>
        /// The geographical location of this location.
        [Mandatory]
        public GeoCoordinate                       Coordinates              { get; }

        /// <summary>
        /// The internal identification of the Electric Vehicle Supply Equipment (EVSE).
        /// </summary>
        [Mandatory]
        public EVSE_UId                            EVSEUId                  { get; }

        /// <summary>
        /// The official identification of the Electric Vehicle Supply Equipment (EVSE).
        /// </summary>
        [Mandatory]
        public EVSE_Id                             EVSEId                   { get; }

        /// <summary>
        /// The official identification of the connector within the EVSE.
        /// </summary>
        [Mandatory]
        public Connector_Id                        ConnectorId              { get; }

        /// <summary>
        /// The standard of the installed connector.
        /// </summary>
        public ConnectorTypes                      ConnectorStandard        { get; }

        /// <summary>
        /// The format (socket/cable) of the installed connector.
        /// </summary>
        public ConnectorFormats                    ConnectorFormat          { get; }

        /// <summary>
        /// The power type of the installed connector.
        /// </summary>
        public PowerTypes                          ConnectorPowerType       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charge detail record location containing only the relevant
        /// information from the official location.
        /// </summary>
        public CDRLocation(Location_Id       Id,
                           String            Address,
                           String            City,
                           String            PostalCode,
                           String            Country,
                           GeoCoordinate     Coordinates,
                           EVSE_UId          EVSEUId,
                           EVSE_Id           EVSEId,
                           Connector_Id      ConnectorId,
                           ConnectorTypes    ConnectorStandard,
                           ConnectorFormats  ConnectorFormat,
                           PowerTypes        ConnectorPowerType,

                           String            Name = null)

        {

            this.Id                   = Id;
            this.Address              = Address;
            this.City                 = City;
            this.PostalCode           = PostalCode;
            this.Country              = Country;
            this.Coordinates          = Coordinates;
            this.EVSEUId              = EVSEUId;
            this.EVSEId               = EVSEId;
            this.ConnectorId          = ConnectorId;
            this.ConnectorStandard    = ConnectorStandard;
            this.ConnectorFormat      = ConnectorFormat;
            this.ConnectorPowerType   = ConnectorPowerType;

            this.Name                 = Name;

        }

        #endregion


        #region IComparable<CDRLocation> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is CDRLocation location
                   ? CompareTo(location)
                   : throw new ArgumentException("The given object is not a charging location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(CDRLocation)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="CDRLocation">An CDRLocation to compare with.</param>
        public Int32 CompareTo(CDRLocation CDRLocation)
        {

            if (CDRLocation is null)
                throw new ArgumentNullException(nameof(CDRLocation),  "The given chargiong location must not be null!");

            return Id.CompareTo(CDRLocation.Id);

        }

        #endregion

        #endregion

        #region IEquatable<CDRLocation> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is CDRLocation location &&
                   Equals(location);

        #endregion

        #region Equals(CDRLocation)

        /// <summary>
        /// Compares two locations for equality.
        /// </summary>
        /// <param name="CDRLocation">A location to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(CDRLocation CDRLocation)

            => (!(CDRLocation is null)) &&
                   Id.Equals(CDRLocation.Id);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()

            => Id.ToString();

        #endregion

    }

}
