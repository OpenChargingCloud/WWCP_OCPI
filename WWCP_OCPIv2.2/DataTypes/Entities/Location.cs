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
    public class Location : IHasId<Location_Id>,
                            IEquatable<Location>,
                            IComparable<Location>,
                            IComparable,
                            IEnumerable<EVSE>
    {

        #region Properties

        /// <summary>
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this Location.
        /// </summary>
        [Optional]
        public CountryCode                         CountryCode              { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this Location (following the ISO-15118 standard).
        /// </summary>
        [Optional]
        public Party_Id                            PartyId                  { get; }

        /// <summary>
        /// The identification of the location within the CPOs platform (and suboperator platforms). 
        /// </summary>
        [Mandatory]
        public Location_Id                         Id                       { get; }

        /// <summary>
        /// Whether this location may be published on an website or app etc., or not.
        /// </summary>
        [Mandatory]
        public Boolean                             Publish                  { get; }

        /// <summary>
        /// This field may only be used when the publish field is set to false.
        /// Only owners of Tokens that match all the set fields of one PublishToken in the list are allowed to be shown this location.
        /// </summary>
        [Optional]
        public IEnumerable<PublishTokenType>       PublishAllowedTo         { get; }

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
        [Optional]
        public String                              PostalCode               { get; }

        /// <summary>
        /// Address of the location. // 20
        /// </summary>
        [Optional]
        public String                              State                    { get; }

        /// <summary>
        /// Address of the location. // 3
        /// </summary>
        [Mandatory]
        public String                              Country                  { get; }

        /// <summary>
        /// The geographical location of this location.
        /// </summary>
        [Mandatory]
        public GeoCoordinate                       Coordinates              { get; }

        /// <summary>
        /// Geographical location of related geo coordinates relevant to the ev customer.
        /// </summary>
        [Optional]
        public IEnumerable<AdditionalGeoLocation>  RelatedLocations         { get; }

        /// <summary>
        /// The general type of parking at the charge point location.
        /// </summary>
        [Optional]
        public ParkingTypes?                       ParkingType              { get; }

        /// <summary>
        /// All Electric Vehicle Supply Equipments (EVSE) present
        /// within this charging station.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE>                   EVSEs                    { get; }

        /// <summary>
        /// The unique identifications of all Electric Vehicle Supply Equipment (EVSEs)
        /// present within this charging station.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_Id>                EVSEIds
            => EVSEs.SafeSelect(evse => evse.Id);

        /// <summary>
        /// The unique identifications of all Electric Vehicle Supply Equipment (EVSEs)
        /// present within this charging station.
        /// </summary>
        [Optional]
        public IEnumerable<EVSE_UId>               EVSEUIds
            => EVSEs.SafeSelect(evse => evse.UId);

        /// <summary>
        /// Human-readable directions on how to reach the location.
        /// </summary>
        [Optional]
        public I18NString                          Directions               { get; }

        /// <summary>
        /// Information of the charging station operator.
        /// </summary>
        /// <remarks>When not specified, the information retrieved from the Credentials module should be used instead.</remarks>
        [Optional]
        public BusinessDetails                     Operator                 { get; }

        /// <summary>
        /// Information of the suboperator if available.
        /// </summary>
        [Optional]
        public BusinessDetails                     SubOperator              { get; }

        /// <summary>
        /// Information of the suboperator if available.
        /// </summary>
        [Optional]
        public BusinessDetails                     Owner                    { get; }

        /// <summary>
        /// Information of the suboperator if available.
        /// </summary>
        [Optional]
        public IEnumerable<Facilities>             Facilities               { get; }

        /// <summary>
        /// One of IANA tzdata’s TZ-values representing the time zone of the location (http://www.iana.org/time-zones).
        /// </summary>
        /// <example>"Europe/Oslo", "Europe/Zurich"</example>
        [Mandatory]
        public String                              Timezone                 { get; }

        /// <summary>
        /// Information of the Charging Station Operator. When not specified,
        /// the information retreived from the api_info endpoint
        /// should be used instead.
        /// </summary>
        [Optional]
        public Hours                               OpeningTimes             { get; }

        /// <summary>
        /// Indicates if the EVSEs are still charging outside the opening
        /// hours of the location. E.g. when the parking garage closes its
        /// barriers over night, is it allowed to charge till the next
        /// morning? [Default: true]
        /// </summary>
        [Optional]
        public Boolean?                            ChargingWhenClosed       { get; }

        /// <summary>
        /// Links to images related to the location such as photos or logos.
        /// </summary>
        [Optional]
        public IEnumerable<Image>                  Images                   { get; }

        /// <summary>
        /// Links to images related to the location such as photos or logos.
        /// </summary>
        [Optional]
        public EnergyMix                           EnergyMix                { get; }

        /// <summary>
        /// Timestamp when this location was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                            LastUpdated              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The Location object describes the location and its properties
        /// where a group of EVSEs that belong together are installed.
        /// </summary>
        /// <param name="Id">Uniquely identifies the location within the CPOs platform (and suboperator platforms).</param>
        /// <param name="Operator">Information of the evse operator.</param>
        /// <param name="SubOperator">Information of the evse suboperator if available.</param>
        public Location(CountryCode                         CountryCode,
                        Party_Id                            PartyId,
                        Location_Id                         Id,
                        Boolean                             Publish,
                        String                              Address,
                        String                              City,
                        String                              Country,
                        GeoCoordinate                       Coordinates,
                        String                              Timezone,

                        IEnumerable<PublishTokenType>       PublishAllowedTo     = null,
                        String                              Name                 = null,
                        String                              PostalCode           = null,
                        String                              State                = null,
                        IEnumerable<AdditionalGeoLocation>  RelatedLocations     = null,
                        ParkingTypes?                       ParkingType          = null,
                        IEnumerable<EVSE>                   EVSEs                = null,
                        I18NString                          Directions           = null,
                        BusinessDetails                     Operator             = null,
                        BusinessDetails                     SubOperator          = null,
                        BusinessDetails                     Owner                = null,
                        IEnumerable<Facilities>             Facilities           = null,
                        Hours                               OpeningTimes         = null,
                        Boolean?                            ChargingWhenClosed   = null,
                        IEnumerable<Image>                  Images               = null,
                        EnergyMix                           EnergyMix            = null,

                        DateTime?                           LastUpdated          = null)

        {

            this.CountryCode          = CountryCode;
            this.PartyId              = PartyId;
            this.Id                   = Id;
            this.Publish              = Publish;
            this.Address              = Address;
            this.City                 = City;
            this.Country              = Country;
            this.Coordinates          = Coordinates;
            this.Timezone             = Timezone;

            this.PublishAllowedTo     = PublishAllowedTo;
            this.Name                 = Name;
            this.PostalCode           = PostalCode;
            this.State                = State;
            this.RelatedLocations     = RelatedLocations;
            this.ParkingType          = ParkingType;
            this.EVSEs                = EVSEs;
            this.Directions           = Directions;
            this.Operator             = Operator;
            this.SubOperator          = SubOperator;
            this.Owner                = Owner;
            this.Facilities           = Facilities;
            this.OpeningTimes         = OpeningTimes;
            this.ChargingWhenClosed   = ChargingWhenClosed;
            this.Images               = Images;
            this.EnergyMix            = EnergyMix;

            this.LastUpdated          = LastUpdated ?? DateTime.Now;

        }

        #endregion


        #region IEnumerable<EVSE> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => EVSEs.GetEnumerator();

        public IEnumerator<EVSE> GetEnumerator()
            => EVSEs.GetEnumerator();

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
