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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The Location object describes the location and its properties
    /// where a group of EVSEs that belong together are installed.
    /// 
    /// Typically the Location object is the exact location of the group
    /// of EVSEs, but it can also be the entrance of a parking garage
    /// which contains these EVSEs. The exact way to reach each EVSE
    /// can then be further specified by its own properties.
    /// </summary>
    public class Location : AEMobilityEntity<Location_Id>,
                            IEquatable<Location>, IComparable<Location>, IComparable,
                            IEnumerable<EVSE>
    {

        #region Properties

        /// <summary>
        /// The general type of the charge point location.
        /// </summary>
        [Mandatory]
        public LocationTypes LocationType { get; }

        /// <summary>
        /// Display name of the location.
        /// </summary>
        [Optional]
        public I18NString Name { get; }

        /// <summary>
        /// Address of the location.
        /// </summary>
        [Mandatory]
        public Address Address { get; }

        /// <summary>
        /// The geographical location of this location.
        /// </summary>
        [Mandatory]
        public GeoCoordinate GeoCoordinates { get; }

        /// <summary>
        /// Geographical location of related geo coordinates relevant to the ev customer.
        /// </summary>
        [Optional]
        public ReactiveSet<AdditionalGeoLocation> RelatedLocations { get; }

        /// <summary>
        /// Human-readable directions on how to reach the location.
        /// </summary>
        [Optional]
        public I18NString Directions { get; }

        /// <summary>
        /// Information of the Charging Station Operator.
        /// </summary>
        [Optional]
        public BusinessDetails Operator { get; }

        /// <summary>
        /// Information of the suboperator if available.
        /// </summary>
        [Optional]
        public BusinessDetails SubOperator { get; }

        /// <summary>
        /// Information of the Charging Station Operator. When not specified,
        /// the information retreived from the api_info endpoint
        /// should be used instead.
        /// </summary>
        [Optional]
        public Hours OpeningTimes { get; }

        /// <summary>
        /// Indicates if the EVSEs are still charging outside the opening
        /// hours of the location. E.g. when the parking garage closes its
        /// barriers over night, is it allowed to charge till the next
        /// morning? [Default: true]
        /// </summary>
        [Optional]
        public Boolean ChargingWhenClosed { get; }

        /// <summary>
        /// Links to images related to the location such as photos or logos.
        /// </summary>
        [Optional]
        public ReactiveSet<Image> Images { get; }

        /// <summary>
        /// All Electric Vehicle Supply Equipments (EVSE) present
        /// within this charging station.
        /// </summary>
        public IEnumerable<EVSE> EVSEs { get; }

        /// <summary>
        /// The unique identifications of all Electric Vehicle Supply Equipment (EVSEs)
        /// present within this charging station.
        /// </summary>
        public IEnumerable<EVSE_Id> EVSEIds
            => EVSEs.SafeSelect(evse => evse.Id);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The Location object describes the location and its properties
        /// where a group of EVSEs that belong together are installed.
        /// </summary>
        /// <param name="Id">Uniquely identifies the location within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Operator">Information of the evse operator.</param>
        /// <param name="SubOperator">Information of the evse suboperator if available.</param>
        public Location(Location_Id      Id,
                        BusinessDetails  Operator,
                        BusinessDetails  SubOperator = null)

            : base(Id)

        {

            this.Operator            = Operator ?? throw new ArgumentNullException("Operator", "The given parameter must not be null!");
            this.SubOperator         = SubOperator;

            //this._OpeningTimes        = Hours.TwentyFourSevenOpen();
            //this._ChargingWhenClosed  = true;
            this.Images              = new ReactiveSet<Image>();

        }

        #endregion


        #region IEnumerable<EVSE> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _EVSEs.Values.GetEnumerator();
        }

        public IEnumerator<EVSE> GetEnumerator()
        {
            return _EVSEs.Values.GetEnumerator();
        }

        #endregion


        #region IComparable<Location> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Location location
                   ? CompareTo(location)
                   : throw new ArgumentException("The given object is not a charging location!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Location)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Location">An Location to compare with.</param>
        public Int32 CompareTo(Location Location)
        {

            if (Location is null)
                throw new ArgumentNullException(nameof(Location),  "The given chargiong location must not be null!");

            return Id.CompareTo(Location.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Location> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Location location &&
                   Equals(location);

        #endregion

        #region Equals(Location)

        /// <summary>
        /// Compares two locations for equality.
        /// </summary>
        /// <param name="Location">A location to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Location Location)

            => (!(Location is null)) &&
                   Id.Equals(Location.Id);

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
