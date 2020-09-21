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
using Newtonsoft.Json.Linq;
using System.Linq;

#endregion

namespace cloud.charging.open.protocols.OCPIv2_2
{

    /// <summary>
    /// The EVSE object describes the part that controls the power supply to a single EV in a single session.
    /// </summary>
    public class EVSE : IHasId<EVSE_Id>,
                        IEquatable<EVSE>,
                        IComparable<EVSE>,
                        IComparable,
                        IEnumerable<Connector>
    {

        #region Properties

        /// <summary>
        /// The location object that contains this EVSE.
        /// </summary>
        [Mandatory]
        public EVSE_UId                          UId                        { get; }

        /// <summary>
        /// The location object that contains this EVSE.
        /// </summary>
        /// <remarks>Mandatory within this implementation.</remarks>
        [Mandatory]
        public EVSE_Id                           Id                         { get; }

        /// <summary>
        /// Indicates the current status of the EVSE.
        /// </summary>
        [Mandatory]
        public StatusTypes                       Status                     { get; }

        /// <summary>
        /// Indicates a planned status in the future of the EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<StatusSchedule>       StatusSchedule             { get; }

        /// <summary>
        /// List of functionalities that the EVSE is capable of.
        /// </summary>
        [Optional]
        public IEnumerable<CapabilityTypes>      Capabilities               { get; }

        /// <summary>
        /// List of available connectors at this EVSE.
        /// </summary>
        [Mandatory]
        public IEnumerable<Connector>            Connectors                 { get; }

        /// <summary>
        /// The unique identifications of all connectors at this EVSE.
        /// </summary>
        [Optional]
        public IEnumerable<Connector_Id>         ConnectorIds
            => Connectors.SafeSelect(connector => connector.Id);

        /// <summary>
        /// Level on which the EVSE is located (in garage buildings) in the locally displayed numbering scheme.  // 4
        /// </summary>
        [Optional]
        public String                            FloorLevel                 { get; }

        /// <summary>
        /// The geographical location of this EVSE.
        /// </summary>
        [Optional]
        public GeoCoordinate?                    Coordinates                { get; }

        /// <summary>
        /// A number/string printed on the outside of the EVSE for visual identification. // 16
        /// </summary>
        [Optional]
        public String                            PhysicalReference          { get; }

        /// <summary>
        /// Multi-language human-readable directions when more detailed
        /// information on how to reach the EVSE from the location is required.
        /// </summary>
        [Optional]
        public I18NString                        Directions                 { get; }

        /// <summary>
        /// The restrictions that apply to the parking spot.
        /// </summary>
        [Optional]
        public IEnumerable<ParkingRestrictions>  ParkingRestrictions        { get; }

        /// <summary>
        /// Links to images related to the EVSE such as photos or logos.
        /// </summary>
        [Optional]
        public IEnumerable<Image>                Images                     { get; }

        /// <summary>
        /// Timestamp when this EVSE was last updated (or created).
        /// </summary>
        [Mandatory]
        public DateTime                          LastUpdated                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// The EVSE object describes the part that controls the power supply to a single EV in a single session.
        /// </summary>
        public EVSE(EVSE_UId                          UId,
                    EVSE_Id                           Id,
                    StatusTypes                       Status,
                    IEnumerable<Connector>            Connectors,

                    IEnumerable<StatusSchedule>       StatusSchedule        = null,
                    IEnumerable<CapabilityTypes>      Capabilities          = null,
                    String                            FloorLevel            = null,
                    GeoCoordinate?                    Coordinates           = null,
                    String                            PhysicalReference     = null,
                    I18NString                        Directions            = null,
                    IEnumerable<ParkingRestrictions>  ParkingRestrictions   = null,
                    IEnumerable<Image>                Images                = null,

                    DateTime?                         LastUpdated           = null)

        {

            this.UId                  = UId;
            this.Id                   = Id;
            this.Status               = Status;
            this.Connectors           = Connectors;

            this.StatusSchedule       = StatusSchedule;
            this.Capabilities         = Capabilities;
            this.FloorLevel           = FloorLevel;
            this.Coordinates          = Coordinates;
            this.PhysicalReference    = PhysicalReference;
            this.Directions           = Directions;
            this.ParkingRestrictions   = ParkingRestrictions;
            this.Images               = Images;

            this.LastUpdated          = LastUpdated ?? DateTime.Now;

        }

        #endregion



        public JObject ToJSON()
        {

            var JSON = JSONObject.Create(

                           new JProperty("uid",                         UId.   ToString()),
                           new JProperty("evse_id",                     Id.    ToString()),
                           new JProperty("status",                      Status.ToString()),

                           StatusSchedule.SafeAny()
                               ? new JProperty("status_schedule",       new JArray(StatusSchedule.Select(status     => status.    ToJSON())))
                               : null,

                           Capabilities.SafeAny()
                               ? new JProperty("capabilities",          new JArray(Capabilities.  Select(capability => capability.ToString())))
                               : null,

                           Connectors.SafeAny()
                               ? new JProperty("connectors",            new JArray(Connectors.    Select(connector  => connector. ToJSON())))
                               : null,

                           FloorLevel.IsNotNullOrEmpty()
                               ? new JProperty("floor_level",           FloorLevel)
                               : null,

                           Coordinates.HasValue
                               ? new JProperty("coordinates",           new JObject(
                                                                            new JProperty("latitude",  Coordinates.Value.Latitude. Value.ToString()),
                                                                            new JProperty("longitude", Coordinates.Value.Longitude.Value.ToString())
                                                                        ))
                               : null,

                           PhysicalReference.IsNotNullOrEmpty()
                               ? new JProperty("physical_reference",    PhysicalReference)
                               : null,

                           Directions.IsNeitherNullNorEmpty()
                               ? new JProperty("directions",            Directions.ToJSON())
                               : null,

                           ParkingRestrictions.SafeAny()
                               ? new JProperty("parking_restrictions",  new JArray(ParkingRestrictions.Select(parking => parking.ToString())))
                               : null,

                           Images.SafeAny()
                               ? new JProperty("images",                new JArray(Images.Select(image => image.ToJSON())))
                               : null,

                           new JProperty("last_updated",                LastUpdated.ToIso8601())

                       );

            return JSON;

        }


        #region IEnumerable<Connectors> Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => Connectors.GetEnumerator();

        public IEnumerator<Connector> GetEnumerator()
            => Connectors.GetEnumerator();

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
