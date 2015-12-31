/*
 * Copyright (c) 2015-2016 GraphDefined GmbH
 * This file is part of WWCP OCPI <https://github.com/GraphDefined/WWCP_OCPI>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
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

#endregion

namespace org.GraphDefined.WWCP.OCPI_2_0
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

        #region LocationType

        private LocationType _LocationType;

        /// <summary>
        /// The general type of the charge point location.
        /// </summary>
        [Mandatory]
        public LocationType LocationType
        {

            get
            {
                return _LocationType;
            }

            set
            {

                if (_LocationType != value)
                    SetProperty(ref _LocationType, value);

            }

        }

        #endregion

        #region Name

        private readonly I18NString _Name;

        /// <summary>
        /// Display name of the location.
        /// </summary>
        [Optional]
        public I18NString Name
        {
            get
            {
                return _Name;
            }
        }

        #endregion

        #region Address

        private Address _Address;

        /// <summary>
        /// Address of the location.
        /// </summary>
        [Mandatory]
        public Address Address
        {

            get
            {
                return _Address;
            }

            set
            {

                if (_Address != value)
                    SetProperty(ref _Address, value);

            }

        }

        #endregion

        #region GeoLocation

        private GeoCoordinate _GeoCoordinates;

        /// <summary>
        /// The geographical location of this location.
        /// </summary>
        [Mandatory]
        public GeoCoordinate GeoCoordinates
        {

            get
            {
                return _GeoCoordinates;
            }

            set
            {

                if (value == null)
                    value = new GeoCoordinate(new Latitude(0), new Longitude(0));

                if (_GeoCoordinates != value)
                {

                    SetProperty(ref _GeoCoordinates, value);

                    _EVSEs.Values.ForEach(station => station._GeoCoordinates = null);

                }

            }

        }

        #endregion

        #region RelatedLocations

        private readonly ReactiveSet<AdditionalGeoLocation> _RelatedLocations;

        /// <summary>
        /// Geographical location of related geo coordinates relevant to the ev customer.
        /// </summary>
        [Optional]
        public ReactiveSet<AdditionalGeoLocation> RelatedLocations
        {
            get
            {
                return _RelatedLocations;
            }
        }

        #endregion

        #region Directions

        private I18NString _Directions;

        /// <summary>
        /// Human-readable directions on how to reach the location.
        /// </summary>
        [Optional]
        public I18NString Directions
        {
            get
            {
                return _Directions;
            }
        }

        #endregion

        #region Operator

        private readonly BusinessDetails _Operator;

        /// <summary>
        /// Information of the EVSE operator.
        /// </summary>
        [Optional]
        public BusinessDetails Operator
        {
            get
            {
                return _Operator;
            }
        }

        #endregion

        #region SubOperator

        private readonly BusinessDetails _SubOperator;

        /// <summary>
        /// Information of the suboperator if available.
        /// </summary>
        [Optional]
        public BusinessDetails SubOperator
        {
            get
            {
                return _SubOperator;
            }
        }

        #endregion

        #region OpeningTimes

        private Hours _OpeningTimes;

        /// <summary>
        /// Information of the EVSE operator. When not specified,
        /// the information retreived from the api_info endpoint
        /// should be used instead.
        /// </summary>
        [Optional]
        public Hours OpeningTimes
        {

            get
            {
                return _OpeningTimes;
            }

            set
            {

                if (_OpeningTimes != value)
                    SetProperty(ref _OpeningTimes, value);

            }

        }

        #endregion

        #region ChargingWhenClosed

        private Boolean _ChargingWhenClosed;

        /// <summary>
        /// Indicates if the EVSEs are still charging outside the opening
        /// hours of the location. E.g. when the parking garage closes its
        /// barriers over night, is it allowed to charge till the next
        /// morning? [Default: true]
        /// </summary>
        [Optional]
        public Boolean ChargingWhenClosed
        {

            get
            {
                return _ChargingWhenClosed;
            }

            set
            {

                if (_ChargingWhenClosed != value)
                    SetProperty(ref _ChargingWhenClosed, value);

            }

        }

        #endregion

        #region Images

        private readonly ReactiveSet<Image> _Images;

        /// <summary>
        /// Links to images related to the location such as photos or logos.
        /// </summary>
        [Optional]
        public ReactiveSet<Image> Images
        {
            get
            {
                return _Images;
            }
        }

        #endregion


        #region EVSEs

        private readonly ConcurrentDictionary<EVSE_Id, EVSE> _EVSEs;

        /// <summary>
        /// All Electric Vehicle Supply Equipments (EVSE) present
        /// within this charging station.
        /// </summary>
        public IEnumerable<EVSE> EVSEs
        {
            get
            {
                return _EVSEs.Select(KVP => KVP.Value);
            }
        }

        #endregion

        #region EVSEIds

        /// <summary>
        /// The unique identifications of all Electric Vehicle Supply Equipment (EVSEs)
        /// present within this charging station.
        /// </summary>
        public IEnumerable<EVSE_Id> EVSEIds
        {
            get
            {
                return _EVSEs.Values.Select(v => v.Id);
            }
        }

        #endregion

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

            #region Initial checks

            if (Operator == null)
                throw new ArgumentNullException("Operator", "The given parameter must not be null!");

            #endregion

            #region Init data and properties

            this._Operator            = Operator;
            this._SubOperator         = SubOperator;

            this._OpeningTimes        = Hours.TwentyFourSevenOpen();
            this._ChargingWhenClosed  = true;
            this._Images              = new ReactiveSet<Image>();

            #endregion

            #region Init events

            //// ChargingStation events
            //this.EVSEAddition             = new VotingNotificator<DateTime, ChargingStation, EVSE, Boolean>(() => new VetoVote(), true);
            //this.EVSERemoval              = new VotingNotificator<DateTime, ChargingStation, EVSE, Boolean>(() => new VetoVote(), true);
            //
            //// EVSE events
            //this.SocketOutletAddition     = new VotingNotificator<DateTime, EVSE, SocketOutlet, Boolean>(() => new VetoVote(), true);
            //this.SocketOutletRemoval      = new VotingNotificator<DateTime, EVSE, SocketOutlet, Boolean>(() => new VetoVote(), true);

            #endregion

            #region Link events

            //// ChargingStation events
            //this.OnEVSEAddition.           OnVoting       += (timestamp, station, evse, vote)      => ChargingPool.EVSEAddition.           SendVoting      (timestamp, station, evse, vote);
            //this.OnEVSEAddition.           OnNotification += (timestamp, station, evse)            => ChargingPool.EVSEAddition.           SendNotification(timestamp, station, evse);
            //
            //this.OnEVSERemoval.            OnVoting       += (timestamp, station, evse, vote)      => ChargingPool.EVSERemoval .           SendVoting      (timestamp, station, evse, vote);
            //this.OnEVSERemoval.            OnNotification += (timestamp, station, evse)            => ChargingPool.EVSERemoval .           SendNotification(timestamp, station, evse);
            //
            //// EVSE events
            //this.SocketOutletAddition.     OnVoting       += (timestamp, evse, outlet, vote)       => ChargingPool.SocketOutletAddition.   SendVoting      (timestamp, evse, outlet, vote);
            //this.SocketOutletAddition.     OnNotification += (timestamp, evse, outlet)             => ChargingPool.SocketOutletAddition.   SendNotification(timestamp, evse, outlet);
            //
            //this.SocketOutletRemoval.      OnVoting       += (timestamp, evse, outlet, vote)       => ChargingPool.SocketOutletRemoval.    SendVoting      (timestamp, evse, outlet, vote);
            //this.SocketOutletRemoval.      OnNotification += (timestamp, evse, outlet)             => ChargingPool.SocketOutletRemoval.    SendNotification(timestamp, evse, outlet);

            #endregion

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
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an Location.
            var Location = Object as Location;
            if ((Object) Location == null)
                throw new ArgumentException("The given object is not an Location!");

            return CompareTo(Location);

        }

        #endregion

        #region CompareTo(Location)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Location">An Location to compare with.</param>
        public Int32 CompareTo(Location Location)
        {

            if ((Object) Location == null)
                throw new ArgumentNullException("The given Location must not be null!");

            return _Id.CompareTo(Location._Id);

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
        {

            if (Object == null)
                return false;

            // Check if the given object is an Location.
            var Location = Object as Location;
            if ((Object) Location == null)
                return false;

            return this.Equals(Location);

        }

        #endregion

        #region Equals(Location)

        /// <summary>
        /// Compares two Locations for equality.
        /// </summary>
        /// <param name="Location">An Location to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Location Location)
        {

            if ((Object) Location == null)
                return false;

            return _Id.Equals(Location._Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return _Id.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return _Id.ToString();
        }

        #endregion

    }

}
