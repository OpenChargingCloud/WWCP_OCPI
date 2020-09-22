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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;

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
        /// The ISO-3166 alpha-2 country code of the CPO that 'owns' this location.
        /// </summary>
        [Optional]
        public CountryCode                         CountryCode              { get; }

        /// <summary>
        /// The Id of the CPO that 'owns' this location (following the ISO-15118 standard).
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


        #region (static) Parse   (JSON, LocationIdURL = null, CustomLocationParser = null)

        /// <summary>
        /// Parse the given JSON representation of an Location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="LocationIdURL">An optional location identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Location Parse(JObject                                JSON,
                                     Location_Id?                           LocationIdURL          = null,
                                     CustomJObjectParserDelegate<Location>  CustomLocationParser   = null)
        {

            if (TryParse(JSON,
                         out Location location,
                         out String   ErrorResponse,
                         LocationIdURL,
                         CustomLocationParser))
            {
                return location;
            }

            throw new ArgumentException("The given JSON representation of a location is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, LocationIdURL = null, CustomLocationParser = null)

        /// <summary>
        /// Parse the given text representation of an Location.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="LocationIdURL">An optional location identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Location Parse(String                                 Text,
                                     Location_Id?                           LocationIdURL          = null,
                                     CustomJObjectParserDelegate<Location>  CustomLocationParser   = null)
        {

            if (TryParse(Text,
                         out Location location,
                         out String   ErrorResponse,
                         LocationIdURL,
                         CustomLocationParser))
            {
                return location;
            }

            throw new ArgumentException("The given text representation of a location is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out Location, out ErrorResponse, LocationIdURL = null, CustomLocationParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of an Location.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Location">The parsed location.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="LocationIdURL">An optional location identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       out Location                           Location,
                                       out String                             ErrorResponse,
                                       Location_Id?                           LocationIdURL          = null,
                                       CustomJObjectParserDelegate<Location>  CustomLocationParser   = null)
        {

            try
            {

                Location = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Id                  [optional]

                if (JSON.ParseOptionalStruct("id",
                                             "location identification",
                                             Location_Id.TryParse,
                                             out Location_Id? LocationIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!LocationIdURL.HasValue && !LocationIdBody.HasValue)
                {
                    ErrorResponse = "The location identification is missing!";
                    return false;
                }

                if (LocationIdURL.HasValue && LocationIdBody.HasValue && LocationIdURL.Value != LocationIdBody.Value)
                {
                    ErrorResponse = "The optional location identification given within the JSON body does not match the one given in the URL!";
                    return false;
                }

                #endregion





                ErrorResponse = null;

                if (CustomLocationParser != null)
                    Location = CustomLocationParser(JSON,
                                            Location);

                return true;

            }
            catch (Exception e)
            {
                Location           = default;
                ErrorResponse  = "The given JSON representation of a location is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out Location, out ErrorResponse, LocationIdURL = null, CustomLocationParser = null)

        /// <summary>
        /// Try to parse the given text representation of an Location.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="Location">The parsed location.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="LocationIdURL">An optional location identification, e.g. from the HTTP URL.</param>
        /// <param name="CustomLocationParser">A delegate to parse custom location JSON objects.</param>
        public static Boolean TryParse(String                                 Text,
                                       out Location                           Location,
                                       out String                             ErrorResponse,
                                       Location_Id?                           LocationIdURL          = null,
                                       CustomJObjectParserDelegate<Location>  CustomLocationParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out Location,
                                out ErrorResponse,
                                LocationIdURL,
                                CustomLocationParser);

            }
            catch (Exception e)
            {
                Location      = null;
                ErrorResponse  = "The given text representation of a location is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomLocationSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomLocationSerializer">A delegate to serialize custom location JSON objects.</param>
        /// <param name="CustomPublishTokenTypeSerializer">A delegate to serialize custom publish token type JSON objects.</param>
        /// <param name="CustomAdditionalGeoLocationSerializer">A delegate to serialize custom additional geo location JSON objects.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSE JSON objects.</param>
        /// <param name="CustomStatusScheduleSerializer">A delegate to serialize custom status schedule JSON objects.</param>
        /// <param name="CustomConnectorSerializer">A delegate to serialize custom Connector JSON objects.</param>
        /// <param name="CustomBusinessDetailsSerializer">A delegate to serialize custom business details JSON objects.</param>
        /// <param name="CustomImageSerializer">A delegate to serialize custom image JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Location>               CustomLocationSerializer                = null,
                              CustomJObjectSerializerDelegate<PublishTokenType>       CustomPublishTokenTypeSerializer        = null,
                              CustomJObjectSerializerDelegate<AdditionalGeoLocation>  CustomAdditionalGeoLocationSerializer   = null,
                              CustomJObjectSerializerDelegate<EVSE>                   CustomEVSESerializer                    = null,
                              CustomJObjectSerializerDelegate<StatusSchedule>         CustomStatusScheduleSerializer          = null,
                              CustomJObjectSerializerDelegate<Connector>              CustomConnectorSerializer               = null,
                              CustomJObjectSerializerDelegate<BusinessDetails>        CustomBusinessDetailsSerializer         = null,
                              CustomJObjectSerializerDelegate<Image>                  CustomImageSerializer                   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("country_code",                    CountryCode.ToString()),
                           new JProperty("party_id",                        PartyId.    ToString()),
                           new JProperty("id",                              Id.         ToString()),
                           new JProperty("publish",                         Publish),

                           Publish == false && PublishAllowedTo.SafeAny()
                               ? new JProperty("publish_allowed_to",        new JArray(PublishAllowedTo.Select(publishAllowedTo => publishAllowedTo.ToJSON(CustomPublishTokenTypeSerializer))))
                               : null,

                           Name.IsNotNullOrEmpty()
                               ? new JProperty("name",                      Name)
                               : null,

                           new JProperty("address",                         Address),
                           new JProperty("city",                            City),

                           PostalCode.IsNotNullOrEmpty()
                               ? new JProperty("postal_code",               PostalCode)
                               : null,

                           State.IsNotNullOrEmpty()
                               ? new JProperty("state",                     State)
                               : null,

                           new JProperty("country",                         Country),
                           new JProperty("coordinates",                     new JObject(
                                                                                new JProperty("latitude",  Coordinates.Latitude. Value.ToString()),
                                                                                new JProperty("longitude", Coordinates.Longitude.Value.ToString())
                                                                            )),

                           RelatedLocations.SafeAny()
                               ? new JProperty("related_locations",         new JArray(RelatedLocations.Select(location => location.ToJSON(CustomAdditionalGeoLocationSerializer))))
                               : null,

                           ParkingType.HasValue
                               ? new JProperty("parking_type",              ParkingType.Value.ToString())
                               : null,

                           EVSEs.SafeAny()
                               ? new JProperty("evses",                     new JArray(EVSEs.Select(evse => evse.ToJSON(CustomEVSESerializer,
                                                                                                                        CustomStatusScheduleSerializer,
                                                                                                                        CustomConnectorSerializer))))
                               : null,

                           Directions.IsNeitherNullNorEmpty()
                               ? new JProperty("directions",                Directions. ToJSON())
                               : null,

                           Operator != null
                               ? new JProperty("operator",                  Operator.   ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           SubOperator != null
                               ? new JProperty("suboperator",               SubOperator.ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           Owner != null
                               ? new JProperty("owner",                     Owner.      ToJSON(CustomBusinessDetailsSerializer))
                               : null,

                           Facilities.SafeAny()
                               ? new JProperty("facilities",                new JArray(Facilities.Select(facility => facility.ToString())))
                               : null,

                           new JProperty("time_zone",                       Timezone),

                           new JProperty("opening_times",                   OpeningTimes.ToJSON()),

                           ChargingWhenClosed.HasValue
                               ? new JProperty("charging_when_closed",      ChargingWhenClosed.Value)
                               : null,

                           Images.SafeAny()
                               ? new JProperty("images",                    new JArray(Images.Select(image => image.ToJSON(CustomImageSerializer))))
                               : null,

                           EnergyMix != null
                               ? new JProperty("energy_mix",                EnergyMix.ToJSON())
                               : null,

                           new JProperty("last_updated",                    LastUpdated.ToIso8601())

                       );

            return CustomLocationSerializer != null
                       ? CustomLocationSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        public Boolean TryGetEVSE(EVSE_UId  EVSEId,
                                  out EVSE  EVSE)
        {

            EVSE = null;
            return false;

        }

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

            => !(Location is null)
                   ? Id.CompareTo(Location.Id)
                   : throw new ArgumentNullException(nameof(Location),
                                                     "The given charging location must not be null!");

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
