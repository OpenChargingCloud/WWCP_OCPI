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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The EVSE object describes the part that controls the power supply to a single EV in a single session.
    /// </summary>
    public class EVSE : AEMobilityEntity<EVSE_Id>,
                        IEquatable<EVSE>, IComparable<EVSE>, IComparable,
                        IEnumerable<Connector>
    {

        #region Properties

        /// <summary>
        /// The location object that contains this EVSE.
        /// </summary>
        [Mandatory]
        public Location Location { get; }

        /// <summary>
        /// The id of the Location object that contains this EVSE.
        /// If the Location object does not exist, this EVSE may be discarded (and it should not have been sent in the first place).
        /// </summary>
        public Location_Id? LocationId
            => Location?.Id;

        #region Status

        private EVSEStatusTypes _Status;

        /// <summary>
        /// Indicates the current status of the EVSE.
        /// </summary>
        [Mandatory]
        public EVSEStatusTypes Status
        {

            get
            {
                return _Status;
            }

            set
            {

                if (_Status != value)
                    SetProperty(ref _Status, value);

            }

        }

        #endregion

        #region StatusSchedule

        private readonly ReactiveSet<EVSEStatusSchedule> _StatusSchedule;

        /// <summary>
        /// Indicates a planned status in the future of the EVSE.
        /// </summary>
        [Optional]
        public ReactiveSet<EVSEStatusSchedule> StatusSchedule
        {
            get
            {
                return _StatusSchedule;
            }
        }

        #endregion

        #region Capabilities

        private readonly ReactiveSet<CapabilityTypes> _Capabilities;

        /// <summary>
        /// List of functionalities that the EVSE is capable of.
        /// </summary>
        [Optional]
        public ReactiveSet<CapabilityTypes> Capabilities
        {
            get
            {
                return _Capabilities;
            }
        }

        #endregion

        #region Connectors

        private ReactiveSet<Connector> _Connectors;

        /// <summary>
        /// List of available connectors on the EVSE.
        /// </summary>
        [Mandatory]
        public ReactiveSet<Connector> Connectors
        {

            get
            {
                return _Connectors;
            }

            set
            {

                if (_Connectors != value)
                    SetProperty(ref _Connectors, value);

            }

        }

        #endregion

        #region FloorLevel

        private String _FloorLevel;

        /// <summary>
        /// Level on which the charging station is located (in garage buildings) in the locally displayed numbering scheme.
        /// </summary>
        [Optional]
        public String FloorLevel
        {

            get
            {
                return _FloorLevel;
            }

            set
            {

                if (_FloorLevel != value)
                    SetProperty(ref _FloorLevel, value);

            }

        }

        #endregion

        #region GeoCoordinates

        internal GeoCoordinate? _GeoCoordinates;

        /// <summary>
        /// The geographical location of this charging station.
        /// </summary>
        [Optional]
        public GeoCoordinate? GeoCoordinates
        {

            get
            {

                return _GeoCoordinates.HasValue
                    ? _GeoCoordinates
                    : Location.GeoCoordinates;

            }

            set
            {

                if (value == Location.GeoCoordinates)
                    return;

                if (GeoCoordinates != value)
                {

                    if (value == null)
                        DeleteProperty(ref _GeoCoordinates);

                    else
                        SetProperty(ref _GeoCoordinates, value);

                }

            }

        }

        #endregion

        /// <summary>
        /// A number on the EVSE for visual identification.
        /// </summary>
        [Optional]
        public String PhysicalNumber { get; }

        /// <summary>
        /// Multi-language human-readable directions when more detailed
        /// information on how to reach the EVSE from the Location is required.
        /// </summary>
        [Optional]
        public I18NString Directions { get; }

        #region ParkingRestriction

        private readonly ReactiveSet<ParkingRestrictionTypes> _ParkingRestriction;

        /// <summary>
        /// The restrictions that apply to the parking spot.
        /// </summary>
        [Optional]
        public ReactiveSet<ParkingRestrictionTypes> ParkingRestriction
        {
            get
            {
                return _ParkingRestriction;
            }
        }

        #endregion

        #region Images

        private readonly ReactiveSet<Image> _Images;

        /// <summary>
        /// Links to images related to the EVSE such as photos or logos.
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

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The EVSE object describes the part that controls the power supply to a single EV in a single session.
        /// </summary>
        /// <param name="Id">Uniquely identifies the EVSE within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Location">The charging location object that contains this EVSE.</param>
        internal EVSE(EVSE_Id   Id,
                      Location  Location)

            : base(Id)

        {

            #region Initial checks

            if (Location == null)
                throw new ArgumentNullException("Location", "The given parameter must not be null!");

            #endregion

            #region Init data and properties

            this.Location            = Location;

            this._Status              = EVSEStatusTypes.Unknown;
            this._StatusSchedule      = new ReactiveSet<EVSEStatusSchedule>();
            this._Capabilities        = new ReactiveSet<CapabilityTypes>();
            this._Connectors          = new ReactiveSet<Connector>();
            this.Directions          = new I18NString();
            this._ParkingRestriction  = new ReactiveSet<ParkingRestrictionTypes>();
            this._Images              = new ReactiveSet<Image>();

            #endregion

            #region Init events

            //// EVSE events
            //this.SocketOutletAddition     = new VotingNotificator<DateTime, EVSE, SocketOutlet, Boolean>(() => new VetoVote(), true);
            //this.SocketOutletRemoval      = new VotingNotificator<DateTime, EVSE, SocketOutlet, Boolean>(() => new VetoVote(), true);

            #endregion

            #region Link events

            //// EVSE events
            //this.SocketOutletAddition.     OnVoting       += (timestamp, evse, outlet, vote)       => ChargingStation.SocketOutletAddition.   SendVoting      (timestamp, evse, outlet, vote);
            //this.SocketOutletAddition.     OnNotification += (timestamp, evse, outlet)             => ChargingStation.SocketOutletAddition.   SendNotification(timestamp, evse, outlet);
            //
            //this.SocketOutletRemoval.      OnVoting       += (timestamp, evse, outlet, vote)       => ChargingStation.SocketOutletRemoval.    SendVoting      (timestamp, evse, outlet, vote);
            //this.SocketOutletRemoval.      OnNotification += (timestamp, evse, outlet)             => ChargingStation.SocketOutletRemoval.    SendNotification(timestamp, evse, outlet);

            #endregion

        }

        #endregion


        #region IEnumerable<Connectors> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Connectors.GetEnumerator();
        }

        public IEnumerator<Connector> GetEnumerator()
        {
            return _Connectors.GetEnumerator();
        }

        #endregion


        #region IComparable<EVSE> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is EVSE evse
                   ? CompareTo(evse)
                   : throw new ArgumentException("The given object is not an EVSE!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(EVSE)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="EVSE">An EVSE to compare with.</param>
        public Int32 CompareTo(EVSE EVSE)
        {

            if (EVSE is null)
                throw new ArgumentNullException(nameof(EVSE), "The given EVSE must not be null!");

            return Id.CompareTo(EVSE.Id);

        }

        #endregion

        #endregion

        #region IEquatable<EVSE> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is EVSE evse &&
                   Equals(evse);

        #endregion

        #region Equals(EVSE)

        /// <summary>
        /// Compares two EVSEs for equality.
        /// </summary>
        /// <param name="EVSE">An EVSE to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(EVSE EVSE)

            => (!(EVSE is null)) &&
                   Id.Equals(EVSE.Id);

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
